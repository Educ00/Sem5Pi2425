using Microsoft.EntityFrameworkCore;
using Sem5Pi2425.Domain.Categories;
using Sem5Pi2425.Domain.Products;
using Sem5Pi2425.Domain.Families;
using Sem5Pi2425.Domain.SystemUser;
using Sem5Pi2425.Infraestructure.Users;
using Sem5Pi2425.Infrastructure.Categories;
using Sem5Pi2425.Infrastructure.Products;

namespace Sem5Pi2425.Infrastructure
{
    public class Sem5Pi2425DbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Family> Families { get; set; }
        
        public DbSet<User> Users { get; set; }

        public Sem5Pi2425DbContext(DbContextOptions options) : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CategoryEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new FamilyEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserEntityTypeConfiguration());
        }
    }
}