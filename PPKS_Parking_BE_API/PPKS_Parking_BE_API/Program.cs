using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PPKS_Parking_BE_API;
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

builder.Services.AddHostedService<SensorSimulatorService>();

var app = builder.Build();
//app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// WebSocket setup

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
        // Index view socket

        // Console.WriteLine("WebSocket poziv stigao na /ws/parking");

        if (context.WebSockets.IsWebSocketRequest)
        {
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var services = context.RequestServices;
            await ParkingWebSocketHandler.HandleAsync(webSocket, services);
        }
        else
        {
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }
    else if (context.Request.Path == "/ws/singleparking")
    {
        // Details view socket

        // Console.WriteLine("WebSocket poziv stigao na /ws/singleparking");

        if (context.WebSockets.IsWebSocketRequest)
        {
            var query = context.Request.Query;
            if (!query.TryGetValue("id", out var idValues) || !int.TryParse(idValues.FirstOrDefault(), out var parkingId))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Nedostaje ili je nevazeci 'id' parametar.");
                return;
            }

            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var services = context.RequestServices;
            await ParkingWebSocketHandler.HandleSingleAsync(webSocket, services, parkingId);
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
    // Seed data

    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    await SeedScripts.SeedRolesAndUsersAsync(roleManager, userManager);
    await SeedScripts.SeedParkingDataAsync(context);
    await SeedScripts.SeedParkingLogsAsync(context);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();