using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PPKS_Parking_BE_API.Data;
using PPKS_Parking_BE_API.Models;
using PPKS_Parking_BE_API.WebSockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(x =>
    {
        //x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        x.JsonSerializerOptions.WriteIndented = true;
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        x.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Dozvole za front

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
            policy.WithOrigins("https://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});


var app = builder.Build();

//app.UseHttpsRedirection();

app.UseCors("AllowFrontend");


var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromSeconds(30),
};
webSocketOptions.AllowedOrigins.Add("https://localhost:3000");
webSocketOptions.AllowedOrigins.Add("http://localhost:3000");

app.UseWebSockets(webSocketOptions);

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws/parking")
    {
        Console.WriteLine("WebSocket poziv stigao na /ws/parking");

        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var dbContext = context.RequestServices.GetRequiredService<ApplicationDbContext>();
            await ParkingWebSocketHandler.HandleAsync(webSocket, dbContext);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else
    {
        await next();
    }
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await SeedRolesAndUsersAsync(roleManager, userManager);
    await SeedParkingDataAsync(context);
}

// Dozvola za front



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


async Task SeedRolesAndUsersAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
{
    // Dodaj uloge ako ne postoje

    string[] roles = new[] { "ADMIN" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Seed ADMIN

    var adminEmail = "admin@example.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin123!");
        await userManager.AddToRoleAsync(adminUser, "ADMIN");
    }
}

async Task SeedParkingDataAsync(ApplicationDbContext context)
{
    // Seedanje parkinga

    if (context.Parkings.Any())
    {
        return;
    }

    var parking1 = new Parking
    {
        Name = "Parking 1",
        Blocks = new List<Block>
        {
            new Block
            {
                Name = "A",
                ParkingSpots = Enumerable.Range(1, 5).Select(i => new ParkingSpot
                {
                    Name = $"A{i}",
                    IsOccupied = false
                }).ToList()
            },
            new Block
            {
                Name = "B",
                ParkingSpots = Enumerable.Range(1, 3).Select(i => new ParkingSpot
                {
                    Name = $"B{i}",
                    IsOccupied = false
                }).ToList()
            }
        }
    };

    var parking2 = new Parking
    {
        Name = "Parking 2",
        Blocks = new List<Block>
        {
            new Block
            {
                Name = "A",
                ParkingSpots = Enumerable.Range(1, 4).Select(i => new ParkingSpot
                {
                    Name = $"A{i}",
                    IsOccupied = false
                }).ToList()
            }
        }
    };

    context.Parkings.AddRange(parking1, parking2);
    await context.SaveChangesAsync();
}
