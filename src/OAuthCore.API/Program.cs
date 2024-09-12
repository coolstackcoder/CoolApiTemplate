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

var connectionString = $"Host={Environment.GetEnvironmentVariable("DB_HOST")};Port={Environment.GetEnvironmentVariable("POSTGRES_PORT")};Database={Environment.GetEnvironmentVariable("POSTGRES_DB")};Username={Environment.GetEnvironmentVariable("POSTGRES_USER")};Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD")}";
builder.Services.AddDbContext<OAuthDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAuthorizationCodeService, AuthorizationCodeService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Enhance logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();


var app = builder.Build();

// Add diagnostic middleware
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation($"Request received for path: {context.Request.Path}, Method: {context.Request.Method}");
    await next();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthorization();

// Add logging before controller execution
app.Use(async (context, next) =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("About to execute controller");
    await next();
    logger.LogInformation("Finished executing controller");
});

app.MapControllers();

// Add fallback route for unhandled requests
app.MapFallback(async context =>
{
    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
    logger.LogWarning($"No route found for {context.Request.Method} {context.Request.Path}");
    await context.Response.WriteAsync("No route found");
});

app.Run();