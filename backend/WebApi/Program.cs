using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebApi.Api.Common;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddUserSecrets<Program>()
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine($"ConnectionString: {connectionString}");

builder.Services.AddDbContext<EfDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
builder.Services.AddControllers(); // Add controllers

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "your-issuer",
            ValidAudience = "your-audience",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345SuperSecretKey12345")) 
            // TODO: anpassen
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization(); // Ensure authorization is used if needed

// Füge die Autorisierung hinzu, um geschützte Endpunkte zu ermöglichen
builder.Services.AddAuthorization();

app.MapControllers(); // Map controllers instead of individual endpoints

app.Run();