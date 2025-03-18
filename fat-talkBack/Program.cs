using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using fat_talkBack.Data;
using fat_talkBack.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// ✅ Configure DbContext using Dependency Injection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database", "app_database.sqlite")}")
);

// ✅ Fix Circular Reference Issue
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ✅ Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();

// ✅ API to fetch all users
app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users.ToListAsync();
    return Results.Ok(users);
});

// ✅ API to fetch all items from all users
app.MapGet("/items", async (AppDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});

// ✅ API to sign up new users
app.MapPost("/signup", async (AppDbContext db, SignUpUser newUser) =>
{
    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
    if (existingUser != null)
        return Results.BadRequest("Email already in use.");

    if (newUser.Password != newUser.ConfirmPass)
        return Results.BadRequest("Passwords do not match.");

    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{newUser.Id}", newUser);
});

// ✅ API to store user items
app.MapPost("/users/{userId}/items", async (AppDbContext db, int userId, Item newItem) =>
{
    var user = await db.Users.FindAsync(userId);
    if (user == null)
        return Results.NotFound("User not found.");

    newItem.UserId = userId;
    db.Items.Add(newItem);
    await db.SaveChangesAsync();

    // Return only necessary fields to avoid circular reference
    var response = new
    {
        newItem.Id,
        newItem.Description,
        newItem.Price,
        newItem.Location,
        newItem.ProductType,
        newItem.Image,
        newItem.UserId
    };

    return Results.Created($"/users/{userId}/items/{newItem.Id}", response);
});


// ✅ API to get all items from a specific user (Fixes Circular Reference)
app.MapGet("/users/{userId}/items", async (AppDbContext db, int userId) =>
{
    var items = await db.Items
        .Where(i => i.UserId == userId)
        .Select(i => new
        {
            i.Id,
            i.Description,
            i.Price
        })
        .ToListAsync();

    if (!items.Any())
        return Results.NotFound("No items found for this user.");

    return Results.Ok(items);
});

// ✅ Enable Swagger & Controllers
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
