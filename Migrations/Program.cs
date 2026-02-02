using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

// Console app that can apply migrations or emit SQL script.
// Usage examples:
// dotnet run -- --update
// dotnet run -- --script ./Migrations/sql/deploy.sql

var builder = Host.CreateApplicationBuilder(args);

// Optional: read appsettings.json for connection info, fallback to env vars.
builder.Configuration
 .AddJsonFile("appsettings.json", optional: true)
 .AddEnvironmentVariables();

// Build DbContext options same as design-time factory
var server = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
var databaseName = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "db_production";
var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "root";
var connectionString = $"Server={server};Port={port};Database={databaseName};User={user};Password={password};TreatTinyAsBoolean=true;";

var services = builder.Services;
services.AddDbContext<DataLib.AppDbContext>(options =>
 options.UseMySql(
 connectionString,
 ServerVersion.AutoDetect(connectionString),
 mySqlOptions =>
 {
 mySqlOptions.MigrationsAssembly("Migrations");
 // Remove schema for MySQL; optionally ignore schema behavior
 // mySqlOptions.MigrationsHistoryTable("__efmigrationshistory");
 mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
 }));

var host = builder.Build();

var outputArgIndex = Array.FindIndex(args, a => a.Equals("--script", StringComparison.OrdinalIgnoreCase));
var updateArg = Array.Exists(args, a => a.Equals("--update", StringComparison.OrdinalIgnoreCase));

using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<DataLib.AppDbContext>();

if (outputArgIndex >=0)
{
 var outputPath = args.Length > outputArgIndex +1
 ? args[outputArgIndex +1]
 : "Migrations/sql/deploy.sql";

 Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

 // Generate SQL script for all migrations (idempotent)
 var migrator = context.GetService<IMigrator>();
 var script = migrator.GenerateScript(options: MigrationsSqlGenerationOptions.Idempotent);
 File.WriteAllText(outputPath, script);
 Console.WriteLine($"Migration script written to {outputPath}");
}
else if (updateArg)
{
 Console.WriteLine("Applying database migrations...");
 context.Database.Migrate();
 Console.WriteLine("Database updated.");
}
else
{
 Console.WriteLine("Specify --update to apply migrations or --script <path> to generate SQL.");
}
