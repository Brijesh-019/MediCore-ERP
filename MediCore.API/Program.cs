using MediCore.API.Extensions;
using MediCore.Application.Interfaces;
using MediCore.Infrastructure.Data;
using MediCore.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IHospitalRepository, HospitalRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


var temporaryPasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
Console.WriteLine($"Admin Password Hash: {temporaryPasswordHash}");

var app = builder.Build();

// Global Exception Middleware
app.UseGlobalExceptionHandler();

// Swagger માત્ર Development environmentમાં
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MediCore ERP API v1");

        options.DocumentTitle = "MediCore ERP API";
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();