using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sem5Pi2425.Domain.Users;

namespace Sem5Pi2425.Infrastructure.Users {
    public class UserEntityTypeConfiguration : IEntityTypeConfiguration<User> {

        public void Configure(EntityTypeBuilder<User> builder) {
            builder.HasKey(b => b.Id);
            builder.OwnsOne(u => u.Email);
            builder.OwnsOne(u => u.FullName);
            builder.OwnsOne(u => u.Username);
            builder.OwnsOne(u => u.PhoneNumber);
            builder.OwnsOne(u => u.Password);
        }
    }
}