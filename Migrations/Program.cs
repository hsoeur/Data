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
// dotnet run -- --create-script ./Migrations/sql/green_schema_baseline.sql

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
 new MySqlServerVersion(new Version(8, 0, 36)),
 mySqlOptions =>
 {
 mySqlOptions.MigrationsAssembly("Migrations");
 mySqlOptions.MigrationsHistoryTable("__efmigrationshistory", "dbo");
 mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
 }));

var host = builder.Build();

var outputArgIndex = Array.FindIndex(args, a => a.Equals("--script", StringComparison.OrdinalIgnoreCase));
var createScriptArgIndex = Array.FindIndex(args, a => a.Equals("--create-script", StringComparison.OrdinalIgnoreCase));
var updateArg = Array.Exists(args, a => a.Equals("--update", StringComparison.OrdinalIgnoreCase));

using var scope = host.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<DataLib.AppDbContext>();

if (createScriptArgIndex >= 0)
{
 var outputPath = args.Length > createScriptArgIndex + 1
 ? args[createScriptArgIndex + 1]
 : "Migrations/sql/green_schema_baseline.sql";

 var outputDirectory = Path.GetDirectoryName(outputPath);
 if (!string.IsNullOrWhiteSpace(outputDirectory))
 {
  Directory.CreateDirectory(outputDirectory);
 }

 // Generate the final EF model directly. Unlike the historical migration script,
 // this does not create and then drop obsolete tables or columns.
 var script = "-- API Data - Green schema baseline\n"
  + "-- Generated from the current EF Core model for MySQL 8.0.36.\n"
  + "-- Apply only to a new, empty Green database selected by the connection.\n"
  + "-- This script creates no business data. It must never be run on Blue.\n\n"
  + context.Database.GenerateCreateScript();

 // This index was introduced by a raw-SQL migration and is therefore not part of
 // the EF model returned by GenerateCreateScript(). Keep it in the Green baseline.
 script += Environment.NewLine
  + "CREATE INDEX `IX_ProductTraceabilities_ProductUid_OperationNumber` "
  + "ON `ProductTraceabilities` (`ProductUid`(255), `OperationNumber`);"
  + Environment.NewLine;

 // A database created from the final model must also record every migration that
 // is already represented by the baseline. Otherwise EF would replay destructive
 // historical migrations discovered by EF on the next Database.Migrate() call.
 var historyRepository = context.GetService<IHistoryRepository>();
 script += Environment.NewLine + historyRepository.GetCreateScript() + Environment.NewLine;

 foreach (var migrationId in context.Database.GetMigrations())
 {
  script += historyRepository.GetInsertScript(new HistoryRow(migrationId, ProductInfo.GetVersion()));
  script += Environment.NewLine;
 }

 File.WriteAllText(outputPath, script);
 Console.WriteLine($"Green baseline creation script written to {outputPath}");
}
else if (outputArgIndex >=0)
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
 Console.WriteLine("Specify --update, --script <path>, or --create-script <path>.");
}
