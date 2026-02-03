using System.Diagnostics;
using System.Linq;
using DataLib;
using Api.Data.Repositories;
using Api.Data.Repositories.ManufacturingOrderNotificationRepo;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.OpenApi.Models;
using Api.Data.Swagger;
using Api.Data.Repositories.ArticleRepo;
using Api.Data.Repositories.ManufacturingOrderRepo;
using Api.Data.Repositories.BillOfMaterialRepo;
using Api.Data.Repositories.ArticleVersionRepo;
using Api.Data.Repositories.FirmwareRepo;
using Api.Data.Repositories.UserRepo;
using Api.Data.Repositories.MachineRepo;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Optionnel: infos de base
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Api.Data",
        Version = "v1",
        Description = "Uniformisation et centralisation des appels aux données de production."
    });
    // Active les annotations (SwaggerOperation, SwaggerSchema, etc.)
    options.EnableAnnotations();
    // Ajoute la note générale sur chaque opération
    options.OperationFilter<CommonOperationDescriptionFilter>();
});
builder.Services.AddControllers();

// Connection string from environment variables (same defaults as migrations)
var server = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
var databaseName = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "db_production";
var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "root";
var connectionString = $"Server={server};Port={port};Database={databaseName};User={user};Password={password};TreatTinyAsBoolean=true;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        connectionString,
        ServerVersion.AutoDetect(connectionString),
        mySqlOptions =>
        {
            // Ensure EF Core uses the Migrations project for migrations
            mySqlOptions.MigrationsAssembly("Migrations");
            // Align schema/history behavior with design-time factory
            mySqlOptions.MigrationsHistoryTable("__efmigrationshistory", "dbo");
            mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
        }));

// Repositories
builder.Services.AddScoped<IManufacturingOrderNotificationRepository, ManufacturingOrderNotificationRepository>();
builder.Services.AddScoped<IManufacturingOrderRepository, ManufacturingOrderRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IBillOfMaterialRepository, BillOfMaterialRepository>();
builder.Services.AddScoped<IArticleVersionRepository, ArticleVersionRepository>();
builder.Services.AddScoped<IFirmwareRepository, FirmwareRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();

var app = builder.Build();

// Enable Swagger/OpenAPI in all environments
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Api.Data v1");
    options.RoutePrefix = string.Empty; // serve UI at application root
});

// Open browser automatically to Swagger UI at startup (dev or prod)
app.Lifetime.ApplicationStarted.Register(() =>
{
    try
    {
        var server = app.Services.GetRequiredService<IServer>();
        var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
        var url = addresses?.FirstOrDefault() ?? "http://localhost:5000";
        var target = $"{url.TrimEnd('/')}/"; // Swagger UI is served at root
        Process.Start(new ProcessStartInfo { FileName = target, UseShellExecute = true });
    }
    catch
    {
        // ignore failures (e.g., headless environments)
    }
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
