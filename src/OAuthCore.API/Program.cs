using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using OAuthCore.API.Middleware;
using OAuthCore.Application.Interfaces;
using OAuthCore.Infrastructure.Data;
using OAuthCore.Infrastructure.Services;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = $"Host=localhost;Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";
builder.Services.AddDbContext<OAuthDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();