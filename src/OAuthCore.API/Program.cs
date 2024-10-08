using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using OAuthCore.API.Middleware;
using OAuthCore.Application.Data;
using OAuthCore.Application.Interfaces;
using OAuthCore.Application.Repositories;
using OAuthCore.Infrastructure.Configuration;
using OAuthCore.Infrastructure.Data;
using OAuthCore.Infrastructure.Repositories;
using OAuthCore.Infrastructure.Services;

Env.TraversePath().Load();

// Map Env Settings to OAuthSettings
var oauthCoreSettings = EnvSettingsMapping.MapTo<OAuthCoreSettings>();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = oauthCoreSettings.DbConnectionString;
builder.Services.AddDbContext<OAuthDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register OAuthSettings in DI
builder.Services.AddSingleton(oauthCoreSettings);

builder.Services.AddScoped<IDbContext>(provider => provider.GetRequiredService<OAuthDbContext>());
builder.Services.AddScoped<IDatabaseFactory, PostgresDatabaseFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IAuthorizationCodeService, AuthorizationCodeService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// Enhance logging
builder.Logging.ClearProviders();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddJsonConsole(options =>
    {
        options.IncludeScopes = true;
        options.UseUtcTimestamp = true;
        options.TimestampFormat = "yyyy-MM-dd HH:mm:ss";
    });
}

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