// using Microsoft.EntityFrameworkCore;

// using System.IO;

// public class AppDbContext : DbContext
// {
//     public DbSet<SignUpUser> Users { get; set; }

//     protected override void OnConfiguring(DbContextOptionsBuilder options)
//     {
//         string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "app_database.sqlite");
//         options.UseSqlite($"Data Source={dbPath}");
//     }
// }



// public class Items
// {
//     public int Id { get; set;}
//     public string Description { get; set;}
//     public int Price { get; set;}
//     public string Location { get; set;}
//     public string ProductType { get; set;}
//     public string Image { get; set;}

// }


using Microsoft.EntityFrameworkCore;
using System.IO;

public class AppDbContext : DbContext
{
    public DbSet<SignUpUser> Users { get; set; }
    public DbSet<Item> Items { get; set; } // Register Item entity

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "app_database.sqlite");
        options.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define one-to-many relationship: One user -> Many items
        modelBuilder.Entity<Item>()
            .HasOne(i => i.User)
            .WithMany(u => u.Items)
            .HasForeignKey(i => i.UserId);
    }
}
