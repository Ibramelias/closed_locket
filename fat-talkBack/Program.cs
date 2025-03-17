using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(); // Register SQLite Database
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-create the database if it doesn’t exist
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();  // Creates database and tables if not exist
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// API to fetch all users from the database
app.MapGet("/users", (AppDbContext db) =>
{
    return db.Users.ToList();
});

// API to sigup new users 
app.MapPost("/signup", async (AppDbContext db, SignUpUser newUser) =>
{
    // Check if the email already exists
    var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == newUser.Email);
    if (existingUser != null)
    {
        return Results.BadRequest("Email already in use.");
    }

    // Validate password
    if (newUser.Password != newUser.ConfirmPass)
    {
        return Results.BadRequest("Passwords do not match.");
    }

    // Add user to database
    db.Users.Add(newUser);
    await db.SaveChangesAsync();

    return Results.Created($"/users/{newUser.Id}", newUser);
});






app.Run();