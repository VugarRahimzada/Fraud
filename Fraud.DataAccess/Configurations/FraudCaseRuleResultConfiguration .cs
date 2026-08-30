using Fraud.Core.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fraud.DataAccess.Configurations
{
    public class FraudCaseRuleResultConfiguration : IEntityTypeConfiguration<FraudCaseRuleResult>
    {
        public void Configure(EntityTypeBuilder<FraudCaseRuleResult> builder)
        {
            builder.Property(r => r.RuleName).HasMaxLength(100).IsRequired();
            builder.Property(r => r.RiskScore).HasColumnType("decimal(5,2)");
            builder.Property(r => r.Severity).HasConversion<string>().HasMaxLength(20);
            builder.Property(r => r.Reason).HasMaxLength(500);

            builder.HasOne(r => r.FraudCase)
                .WithMany(f => f.RuleResults)
                .HasForeignKey(r => r.FraudCaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(r => r.Transaction)
                .WithMany()
                .HasForeignKey(r => r.TransactionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(r => r.FraudCaseId)
                .HasDatabaseName("IX_FraudCaseRuleResults_FraudCaseId");
        }
    }
}
