using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fraud.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.DataAccess.Configurations
{
    public class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.ToTable("Cards");

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.ValidDate)
                .IsRequired();

            builder.Property(x => x.TransferLimit)
                .HasDefaultValue((byte)0);

            builder.HasIndex(x => x.IsDelete);
        }
    }
}
