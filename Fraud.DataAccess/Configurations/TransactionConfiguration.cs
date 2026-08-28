using Fraud.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DataAccess.Configurations
{
    public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
    {
        public void Configure(EntityTypeBuilder<Transaction> builder)
        {
            builder.ToTable("Transactions");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(t => t.RiskScore)
                .HasColumnType("decimal(5,2)"); // e.g. 0.00 - 100.00, nullable

            builder.Property(t => t.FailureReason)
                .HasMaxLength(500);

            builder.Property(t => t.Type)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(t => t.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            // Sender side. Restrict delete: a Card with transaction history must not
            // be deletable in a way that silently orphans/cascades financial records.
            builder.HasOne(t => t.FromCard)
                .WithMany()
                .HasForeignKey(t => t.FromCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receiver side. Two FKs to the same Card table need distinct navigation
            // configs, and both must be Restrict for the same reason as above.
            builder.HasOne(t => t.ToCard)
                .WithMany()
                .HasForeignKey(t => t.ToCardId)
                .OnDelete(DeleteBehavior.Restrict);

            // Optional link to a FraudCase. SetNull: if a FraudCase were ever removed,
            // the transaction itself must survive as a financial record.
            builder.HasOne(t => t.FraudCase)
                .WithMany(fc => fc.Transactions)
                .HasForeignKey(t => t.FraudCaseId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes for the fields called out as frequently queried.
            builder.HasIndex(t => t.FromCardId);
            builder.HasIndex(t => t.ToCardId);
            builder.HasIndex(t => t.Status);
            builder.HasIndex(t => t.FraudCaseId);
            builder.HasIndex(t => t.CreateDate);

            // Common fraud-review query shape: "recent transactions by status" —
            // composite index pays off once the engine is querying this table a lot.
            builder.HasIndex(t => new { t.Status, t.CreateDate });
        }
    }
}
