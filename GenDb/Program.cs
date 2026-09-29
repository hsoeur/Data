using GenDb.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

internal class Program
{
    private static async Task<int> Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            var baseDir = AppContext.BaseDirectory;
            var settings = new HostApplicationBuilderSettings
            {
                Args = args,
                ContentRootPath = baseDir
            };
            var builder = Host.CreateApplicationBuilder(settings);
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile(
                    $"appsettings.{builder.Environment.EnvironmentName}.json",
                    optional: true,
                    reloadOnChange: false)
                .AddEnvironmentVariables();

            builder.Services.AddWindowsService(options =>
                options.ServiceName = "GenDb GPAO Import Service");
            builder.Services.Configure<ImportServiceOptions>(
                builder.Configuration.GetSection(ImportServiceOptions.SectionName));
            builder.Services.AddSingleton<BatchStateStore>();
            builder.Services.AddSingleton<ExportBatchCoordinator>();
            builder.Services.AddSingleton<DatabaseImporter>();
            builder.Services.AddHostedService<ImportWorker>();

            Log.Information(
                "GenDb service starting Host={Host} BaseDir={BaseDir} Environment={Environment}",
                Environment.MachineName,
                baseDir,
                builder.Environment.EnvironmentName);

            using var host = builder.Build();
            await host.RunAsync();
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "GenDb service terminated unexpectedly");
            return -1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureLogging()
    {
        var baseDir = AppContext.BaseDirectory;
        var logDir = Path.Combine(baseDir, "logs");
        Directory.CreateDirectory(logDir);
        var logFile = Path.Combine(logDir, "GEN_DB_HORIZON_LOGS_.log");

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File(
                logFile,
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 31,
                shared: true)
            .CreateLogger();

        Log.Information("Logging configured BaseDir={BaseDir} File={LogFile}", baseDir, logFile);
    }
}
