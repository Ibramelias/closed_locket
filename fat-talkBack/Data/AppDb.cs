using Microsoft.EntityFrameworkCore;
using fat_talkBack.Models;

namespace fat_talkBack.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<SignUpUser> Users { get; set; }
        public DbSet<Item> Items { get; set; }

        // ✅ Correct Constructor for Dependency Injection
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Item>()
                .HasOne(i => i.User)
                .WithMany(u => u.Items)
                .HasForeignKey(i => i.UserId);
        }
    }
}
