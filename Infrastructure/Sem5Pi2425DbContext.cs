using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.SystemUser;
using Sem5Pi2425.Infrastructure.Users;

namespace Sem5Pi2425.Infrastructure
{
    public class Sem5Pi2425DbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public Sem5Pi2425DbContext(DbContextOptions options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}