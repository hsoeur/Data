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
using Api.Data.Repositories.FirmwareRepo;
using Api.Data.Repositories.UserRepo;
using Api.Data.Repositories.MachineRepo;
using Api.Data.Repositories.OperationRepo;
using Api.Data.Repositories.SequenceRepo;
using Api.Data.Repositories.ProductTraceabilityRepo;
using Api.Data.Repositories.LabelTypeRepo;
using Api.Data.Repositories.LabelTemplateRepo;
using Api.Data.Repositories.SolderingProgramRepo;
using Api.Data.Repositories.SimCardRepo;
using Api.Data.Repositories.OrderLineRepo;
using Api.Data.Repositories.ArticleVersionRepo;
using Api.Data.Repositories.DatabaseNotificationRepo;
using Api.Data.Repositories.LabelRepo;
using Api.Data.Repositories.KitTraceabilityRepo;
using Api.Data.Repositories.KitAccTraceabilityRepo;
using Api.Data.Repositories.MacAddressRepo;
using Serilog;
using Api.Data.Repositories.MeasurementTraceabilityRepo;

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
  .Enrich.WithProperty("Application", "Api.Data")
    .WriteTo.Console()
    .WriteTo.File(
        path: "Logs/api-data-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30, // Keep 30 days of logs
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

try
{
    Log.Information("Starting Api.Data application");

var builder = WebApplication.CreateBuilder(args);

// Use Serilog for logging
builder.Host.UseSerilog();

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

// CORS: allow all origins, headers, and methods (no credentials)
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowFrontend",
      policy => policy
            .AllowAnyOrigin()
  .AllowAnyHeader()
       .AllowAnyMethod());
});

// Connection string from environment variables (same defaults as migrations)
var server = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
var databaseName = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "db_production";
var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "root";

// Add connection and command timeout parameters to handle slow/unavailable connections
var connectionString = $"Server={server};Port={port};Database={databaseName};User={user};Password={password};TreatTinyAsBoolean=true;ConnectionTimeout=60;DefaultCommandTimeout=60;";

Log.Information("Connecting to MySQL database at {Server}:{Port}/{Database}", server, port, databaseName);

// Use a static ServerVersion instead of AutoDetect to avoid initial connection timeout
var serverVersion = new MySqlServerVersion(new Version(8, 0, 36));

builder.Services.AddDbContext<AppDbContext>(options =>
options.UseMySql(
    connectionString,
   serverVersion,
      mySqlOptions =>
        {
 // Ensure EF Core uses the Migrations project for migrations
   mySqlOptions.MigrationsAssembly("Migrations");
     // Align schema/history behavior with design-time factory
      mySqlOptions.MigrationsHistoryTable("__efmigrationshistory", "dbo");
            mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
 // Enable retry on transient failures in production
      mySqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
    maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
            );
  }));

// Repositories
builder.Services.AddScoped<IManufacturingOrderNotificationRepository, ManufacturingOrderNotificationRepository>();
builder.Services.AddScoped<IManufacturingOrderRepository, ManufacturingOrderRepository>();
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddScoped<IBillOfMaterialRepository, BillOfMaterialRepository>();
builder.Services.AddScoped<IFirmwareVersionRepository, FirmwareVersionRepository>();
builder.Services.AddScoped<IFirmwareRepository, FirmwareRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IMachineRepository, MachineRepository>();
builder.Services.AddScoped<IOperationRepository, OperationRepository>();
builder.Services.AddScoped<ISequenceRepository, SequenceRepository>();
builder.Services.AddScoped<IProductTraceabilityRepository, ProductTraceabilityRepository>();
builder.Services.AddScoped<ILabelTypeRepository, LabelTypeRepository>();
builder.Services.AddScoped<ILabelTemplateRepository, LabelTemplateRepository>();
builder.Services.AddScoped<ISolderingProgramRepository, SolderingProgramRepository>();
builder.Services.AddScoped<ISimCardRepository, SimCardRepository>();
builder.Services.AddScoped<IOrderLineRepository, OrderLineRepository>();
builder.Services.AddScoped<IDatabaseNotificationRepository, DatabaseNotificationRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<IKitTraceabilityRepository, KitTraceabilityRepository>();
builder.Services.AddScoped<IKitAccTraceabilityRepository, KitAccTraceabilityRepository>();
builder.Services.AddScoped<IMacAddressRepository, MacAddressRepository>();
builder.Services.AddScoped<IMeasurementTraceabilityRepository, MeasurementTraceabilityRepository>();

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
        Log.Information("Opening browser to Swagger UI at {Url}", target);
        Process.Start(new ProcessStartInfo { FileName = target, UseShellExecute = true });
    }
    catch (Exception ex)
    {
        Log.Warning(ex, "Failed to open browser automatically");
    }
});

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.MapControllers();

    Log.Information("Api.Data application started successfully");
app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Api.Data application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}