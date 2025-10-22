using Microsoft.EntityFrameworkCore;
using ParParWebsite.Api.Models;

namespace ParParWebsite.Api.Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Portfolio> Portfolios => Set<Portfolio>();
        public DbSet<PortfolioImage> PortfolioImages => Set<PortfolioImage>();
        public DbSet<Config> Config => Set<Config>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Portfolio>()
                .HasMany(p => p.Images)
                .WithOne(i => i.Portfolio)
                .HasForeignKey(i => i.CollectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
