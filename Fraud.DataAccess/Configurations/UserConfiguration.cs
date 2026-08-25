using Fraud.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fraud.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(x => x.UserCode)
                .IsUnique();

            builder.Property(x => x.FirstName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.LastName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(x => x.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(x => x.Role)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}