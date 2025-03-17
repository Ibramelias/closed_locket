using Microsoft.EntityFrameworkCore;
using System.IO;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "Database", "app_database.sqlite");
        options.UseSqlite($"Data Source={dbPath}");
    }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
}
