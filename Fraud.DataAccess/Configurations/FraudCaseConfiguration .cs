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
    public class FraudCaseConfiguration : IEntityTypeConfiguration<FraudCase>
    {
        public void Configure(EntityTypeBuilder<FraudCase> builder)
        {
            builder.ToTable("FraudCases");

            builder.HasKey(fc => fc.Id);

            builder.Property(fc => fc.Reason)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(fc => fc.Status)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.HasIndex(fc => fc.Status);
        }
    }
}
