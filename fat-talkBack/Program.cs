using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using fat_talkBack.Data;
using fat_talkBack.Models;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

//Configure DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={Path.Combine(Directory.GetCurrentDirectory(), "Database", "app_database.sqlite")}")
);

//Fix Circular Reference
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseHttpsRedirection();

//Enable serving static files for uploaded images
app.UseStaticFiles();

// ------------------- API Endpoints -------------------

//Fetch all users (with items)
app.MapGet("/users", async (AppDbContext db) =>
{
    var users = await db.Users
        .Include(u => u.Items)
        .ToListAsync();

    return Results.Ok(users);
});

//Fetch all items
app.MapGet("/items", async (AppDbContext db) =>
{
    var items = await db.Items.ToListAsync();
    return Results.Ok(items);
});

//User SignUp
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

//Upload Item with Image (multipart/form-data)
app.MapPost("/users/{userId}/items", async (HttpRequest request, AppDbContext db, int userId) =>
{
    var user = await db.Users.FindAsync(userId);
    if (user == null)
        return Results.NotFound("User not found.");

    var form = await request.ReadFormAsync();
    var description = form["Description"];
    var price = decimal.Parse(form["Price"]);
    var location = form["Location"];
    var productType = form["ProductType"];
    var imageFile = form.Files["ImageFile"];

    string imagePath = null;

    if (imageFile != null && imageFile.Length > 0)
    {
        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await imageFile.CopyToAsync(stream);
        }

        imagePath = $"/uploads/{fileName}";
    }

    var newItem = new Item
    {
        Description = description,
        Price = price,
        Location = location,
        ProductType = productType,
        Image = imagePath,
        UserId = userId
    };

    db.Items.Add(newItem);
    await db.SaveChangesAsync();

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

//Get all items of a user
app.MapGet("/users/{userId}/items", async (AppDbContext db, int userId) =>
{
    var items = await db.Items
        .Where(i => i.UserId == userId)
        .Select(i => new
        {
            i.Id,
            i.Description,
            i.Price,
            i.Image
        })
        .ToListAsync();

    if (!items.Any())
        return Results.NotFound("No items found for this user.");

    return Results.Ok(items);
});

// Enable Swagger
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
