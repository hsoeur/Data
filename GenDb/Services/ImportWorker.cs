using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Serilog;

namespace GenDb.Services;

public sealed class ImportWorker : BackgroundService
{
    private readonly ExportBatchCoordinator _coordinator;
    private readonly TimeSpan _pollInterval;

    public ImportWorker(
        ExportBatchCoordinator coordinator,
        IOptions<ImportServiceOptions> options)
    {
        _coordinator = coordinator;
        _pollInterval = TimeSpan.FromSeconds(Math.Max(5, options.Value.PollIntervalSeconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Log.Information("Surveillance des exports GPAO démarrée Interval={Interval}", _pollInterval);

        await CheckSafelyAsync(stoppingToken);
        using var timer = new PeriodicTimer(_pollInterval);
        while (await timer.WaitForNextTickAsync(stoppingToken))
            await CheckSafelyAsync(stoppingToken);
    }

    private async Task CheckSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _coordinator.CheckAndImportAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Échec du contrôle/import du lot GPAO. Une nouvelle tentative aura lieu.");
        }
    }
}
