using DataLib;
using GenDb.Generators;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Migrations;
using Serilog;

internal class Program
{
    private static int Main(string[] args)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        try
        {
            ConfigureLogging();
            Console.WriteLine("GenDb démarré. Les logs s'affichent ci-dessous et sont écrits dans le dossier logs.");
            Log.Information("GenDb starting up");
            Log.Information("WorkingDirectory={WorkingDirectory}", Directory.GetCurrentDirectory());
            int result = InstanciateAndGenerateDb();
            stopwatch.Stop();

            var elapsed = stopwatch.Elapsed;
            Log.Information("Execution finished in {Seconds:F2} seconds ({Minutes:F2} minutes)", elapsed.TotalSeconds, elapsed.TotalMinutes);
            Console.WriteLine($"Temps d'exécution : {elapsed.TotalSeconds:F2} secondes ({elapsed.TotalMinutes:F2} minutes).");

            return result;
        }
        catch (FileNotFoundException ex)
        {
            stopwatch.Stop();
            Log.Fatal(ex, "Le fichier de configuration appsettings.json n'a pas été trouvé.");
            Console.WriteLine("Le fichier de configuration appsettings.json n'a pas été trouvé.");
            return -1;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            Log.Fatal(ex, "Unhandled exception during startup");
            Console.WriteLine($"Erreur inattendue au démarrage: {ex.Message}");
            return -2;
        }
        finally
        {
            Log.CloseAndFlush();
            PauseIfInteractive(args);
        }
    }

    private static void ConfigureLogging()
    {
        try
        {
            var baseDir = AppContext.BaseDirectory;
            var logDir = Path.Combine(baseDir, "logs");
            Directory.CreateDirectory(logDir);
            var logFile = Path.Combine(logDir, $"GEN_DB_HORIZON_LOGS_{DateTime.Now:yyyyMMdd}.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                // Console sink excludes logs originating from generators
                .WriteTo.Logger(lc => lc
                .Filter.ByExcluding(e =>
                e.Properties.TryGetValue("SourceContext", out var source) &&
                source.ToString().StartsWith("\"GenDb.Generators"))
                .WriteTo.Console())
                // File sink captures everything including generators
                .WriteTo.File(logFile, rollingInterval: RollingInterval.Infinite, retainedFileCountLimit: null, shared: true)
                .CreateLogger();

            Log.Information("Logging configured: BaseDir={BaseDir}, File={LogFile}", baseDir, logFile);
            Console.WriteLine($"Logs: {logFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Configuration du logging échouée: {ex.Message}");
        }
    }

    private static int InstanciateAndGenerateDb()
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        Log.Information("Environment detected: {Environment}", env);

        // Always resolve configuration files from the executable directory to avoid issues when run by Task Scheduler
        var baseDir = AppContext.BaseDirectory;
        var builder = new ConfigurationBuilder()
            .SetBasePath(baseDir)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables();

        IConfigurationRoot configuration = builder.Build();
        Log.Information("Configuration loaded from BaseDir={BaseDir} (appsettings.json optional, env-specific optional)", baseDir);

        var connectionString = configuration.GetConnectionString("ProductionDatabase");
        if (string.IsNullOrEmpty(connectionString))
        {
            Log.Error("[MAIN] - Impossible de se connecter à la base de données: aucune chaîne de connexion.");
            Console.WriteLine("[MAIN] - Impossible de se connecter à la base de données.");
            return -1;
        }
        else
        {
            Log.Information("Connection string retrieved. Starting generators...");
            LaunchGenerator(configuration, connectionString);
            Log.Information("Generators finished successfully");
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
        Log.Information("MySQL environment variables set from connection string");
    }

    private static AppDbContext CreateDbContext(string connectionString)
    {
        Log.Information("Creating DbContext...");
        ApplyMySqlEnvFromConnectionString(connectionString);
        var factory = new DesignTimeDbContextFactory();
        var ctx = factory.CreateDbContext(Array.Empty<string>());
        Log.Information("DbContext created");
        return ctx;
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

        Log.Information("Starting ProcessGenerator...");
        ProcessGenerator(
            generatorTableArticle,
            generatorTableNomenclature,
            generatorTableOperateur,
            generatorTableOrdreFabrication,
            generatorTableVersionProduit,
            generatorTableOperations,
            generatorTableOrderLine);
        Log.Information("ProcessGenerator completed");
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
        Log.Information("Update Article...");
        generatorTableArticle.Update();
        Log.Information("Update Nomenclature...");
        generatorTableNomenclature.Update();
        Log.Information("Update Operateur...");
        generatorTableOperateur.Update();
        Log.Information("Update OrdreFabrication...");
        generatorTableOrdreFabrication.Update();
        Log.Information("Update VersionProduit...");
        generatorTableVersionProduit.Update();
        Log.Information("Update OrderLine...");
        generatorTableOrderLine.Update();
        Log.Information("Update Operations...");
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

    private static void PauseIfInteractive(string[] args)
    {
        // Close automatically by default. Only pause if --wait is explicitly provided.
        if (Environment.UserInteractive && args != null && args.Contains("--wait"))
        {
            Console.WriteLine("Appuyez sur une touche pour fermer...");
            try { Console.ReadKey(true); } catch { /* ignore */ }
        }
    }
}