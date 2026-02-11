using DataLib;
using GenDb.Generators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Migrations;

internal class Program
{
    private static int Main(string[] args)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            int result = InstanciateAndGenerateDb();
            stopwatch.Stop();

            var elapsed = stopwatch.Elapsed;
            Console.WriteLine($"Temps d'exécution : {elapsed.TotalSeconds:F2} secondes ({elapsed.TotalMinutes:F2} minutes).");

            return result;
        }
        catch (FileNotFoundException)
        {
            stopwatch.Stop();
            Console.WriteLine("Le fichier de configuration appsettings.json n'a pas été trouvé.");
            return -1;
        }
    }

    private static int InstanciateAndGenerateDb()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        IConfigurationRoot configuration = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false)
                            .AddJsonFile($"appsettings.{env}.json", optional: true)
                            .Build();

        var connectionString = configuration.GetConnectionString("ProductionDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("[MAIN] - Impossible de se connecter à la base de données.");
            return -1;
        }
        else
        {
            LaunchGenerator(configuration, connectionString);
        }
        return 0;
    }

    private static void ApplyMySqlEnvFromConnectionString(string connectionString)
    {
        var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2) continue;
            var key = kv[0].Trim();
            var val = kv[1].Trim();
            switch (key.ToLowerInvariant())
            {
                case "server": Environment.SetEnvironmentVariable("MYSQL_HOST", val); break;
                case "port": Environment.SetEnvironmentVariable("MYSQL_PORT", val); break;
                case "database": Environment.SetEnvironmentVariable("MYSQL_DATABASE", val); break;
                case "user": Environment.SetEnvironmentVariable("MYSQL_USER", val); break;
                case "password": Environment.SetEnvironmentVariable("MYSQL_PASSWORD", val); Environment.SetEnvironmentVariable("MYSQL_ROOT_PASSWORD", val); break;
            }
        }
    }

    private static AppDbContext CreateDbContext(string connectionString)
    {
        ApplyMySqlEnvFromConnectionString(connectionString);
        var factory = new DesignTimeDbContextFactory();
        return factory.CreateDbContext(Array.Empty<string>());
    }

    private static void LaunchGenerator(IConfiguration configuration, string connectionString)
    {
        using var context = CreateDbContext(connectionString);

        IGenerator generatorTableArticle = new ArticleGenerator(context, configuration);
        IGenerator generatorTableNomenclature = new BomGenerator(context, configuration);
        IGenerator generatorTableOperateur = new UserGenerator(context, configuration);
        IGenerator generatorTableOrdreFabrication = new ManufacturingOrderGenerator(context, configuration);
        IGenerator generatorTableVersionProduit = new ProductFirmwareGenerator(context, configuration);
        IGenerator generatorTableOperations = new OperationGenerator(context, configuration);
        IGenerator generatorTableOrderLine = new OrderLineGenerator(context, configuration);

        ProcessGenerator(
            generatorTableArticle,
            generatorTableNomenclature,
            generatorTableOperateur,
            generatorTableOrdreFabrication,
            generatorTableVersionProduit,
            generatorTableOperations,
            generatorTableOrderLine);
    }

    private static void ProcessGenerator(
        IGenerator generatorTableArticle,
        IGenerator generatorTableNomenclature,
        IGenerator generatorTableOperateur,
        IGenerator generatorTableOrdreFabrication,
        IGenerator generatorTableVersionProduit,
        IGenerator generatorTableOperations,
        IGenerator generatorTableOrderLine)
    {
        generatorTableArticle.Update();
        generatorTableNomenclature.Update();
        generatorTableOperateur.Update();
        generatorTableOrdreFabrication.Update();
        generatorTableVersionProduit.Update();
        generatorTableOrderLine.Update();
        generatorTableOperations.Update();
    }

    private static void DisplayInfosConsole()
    {
        Console.ForegroundColor = ConsoleColor.Green;

        Console.WriteLine("********************************************");
        Console.WriteLine("*                                          *");
        Console.WriteLine("*         DATABASE HORIZON PRODUCTION      *");
        Console.WriteLine("*                                          *");
        Console.WriteLine("********************************************");

        Console.ResetColor();
        Console.WriteLine("\n");
    }
}