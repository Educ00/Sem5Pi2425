using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.SystemUser;

namespace Sem5Pi2425.Infrastructure.Users {
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> {

        public void Configure(EntityTypeBuilder<User> builder) {
            builder.HasKey(b => b.Id);
            
            builder.HasKey(b => b.Id);

            builder.Property(u => u.Id).HasConversion(
                id => id.Value,
                value => new UserId(value));

            builder.Property(u => u.Email).HasConversion(
                    email => email.Value,
                    value => new Email(value))
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.FullName).HasConversion(
                    name => name.Value,
                    value => new FullName(value))
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PhoneNumber).HasConversion(
                    number => number.Value,
                    value => new PhoneNumber(value))
                .HasMaxLength(20);

            builder.Property(u => u.Username).HasConversion(
                    username => username.Value,
                    value => new Username(value))
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.Password).HasConversion(
                    password => password.Value,
                    value => new Password(value))
                .HasMaxLength(255);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(u => u.ActivationToken)
                .HasMaxLength(100);

            builder.HasIndex(u => u.Email).IsUnique();
            builder.HasIndex(u => u.ActivationToken);
        }
    }
}