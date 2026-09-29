using DataLib;
using GenDb.Generators;
using Microsoft.Extensions.Configuration;
using Migrations;
using Serilog;

namespace GenDb.Services;

public sealed class DatabaseImporter
{
    public Task ImportAsync(IConfiguration configuration, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var connectionString = configuration.GetConnectionString("ProductionDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("La chaîne de connexion ProductionDatabase est absente.");

        ApplyMySqlEnvironment(connectionString);
        using AppDbContext context = new DesignTimeDbContextFactory().CreateDbContext([]);
        using var transaction = context.Database.BeginTransaction();

        try
        {
            IGenerator[] generators =
            [
                new ArticleGenerator(context, configuration),
                new BomGenerator(context, configuration),
                new UserGenerator(context, configuration),
                new ManufacturingOrderGenerator(context, configuration),
                new ProductFirmwareGenerator(context, configuration),
                new OrderLineGenerator(context, configuration),
                new OperationGenerator(context, configuration)
            ];

            foreach (var generator in generators)
            {
                cancellationToken.ThrowIfCancellationRequested();
                Log.Information("Démarrage générateur {Generator}", generator.GetType().Name);
                generator.Update();
            }

            transaction.Commit();
            Log.Information("Transaction globale du lot GPAO validée");
            return Task.CompletedTask;
        }
        catch
        {
            transaction.Rollback();
            context.ChangeTracker.Clear();
            Log.Error("Transaction globale du lot GPAO annulée");
            throw;
        }
    }

    private static void ApplyMySqlEnvironment(string connectionString)
    {
        foreach (var part in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            var keyValue = part.Split('=', 2);
            if (keyValue.Length != 2)
                continue;

            var key = keyValue[0].Trim().ToLowerInvariant();
            var value = keyValue[1].Trim();
            switch (key)
            {
                case "server":
                case "host":
                    Environment.SetEnvironmentVariable("MYSQL_HOST", value);
                    break;
                case "port":
                    Environment.SetEnvironmentVariable("MYSQL_PORT", value);
                    break;
                case "database":
                case "initial catalog":
                    Environment.SetEnvironmentVariable("MYSQL_DATABASE", value);
                    break;
                case "user":
                case "user id":
                case "uid":
                    Environment.SetEnvironmentVariable("MYSQL_USER", value);
                    break;
                case "password":
                case "pwd":
                    Environment.SetEnvironmentVariable("MYSQL_PASSWORD", value);
                    Environment.SetEnvironmentVariable("MYSQL_ROOT_PASSWORD", value);
                    break;
            }
        }
    }
}
