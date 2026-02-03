using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Migrations
{
 /// <summary>
 /// Design-time DbContext factory for running EF Core tools from the Migrations project.
 /// This allows `dotnet ef` to instantiate the context without starting an app.
 /// </summary>
 public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<DataLib.AppDbContext>
 {
 public DataLib.AppDbContext CreateDbContext(string[] args)
 {
 // Read connection string from environment variables (matching docker-compose)
            var server = Environment.GetEnvironmentVariable("MYSQL_HOST") ?? "localhost";
            var port = Environment.GetEnvironmentVariable("MYSQL_PORT") ?? "3306";
            var databaseName = Environment.GetEnvironmentVariable("MYSQL_DATABASE") ?? "db_production";
            var user = Environment.GetEnvironmentVariable("MYSQL_USER") ?? "root";
            var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? Environment.GetEnvironmentVariable("MYSQL_ROOT_PASSWORD") ?? "root";

            var connectionString = $"Server={server};Port={port};Database={databaseName};User={user};Password={password};TreatTinyAsBoolean=true;";

            var optionsBuilder = new DbContextOptionsBuilder<DataLib.AppDbContext>();
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
            mySqlOptions =>
            {
                mySqlOptions.MigrationsAssembly("Migrations");
                mySqlOptions.MigrationsHistoryTable("__efmigrationshistory", "dbo");
                mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });

            return new DataLib.AppDbContext(optionsBuilder.Options);
        }
    }
}
