using Microsoft.Extensions.Options;
using System.Text.Json;

namespace GenDb.Services;

public sealed class BatchStateStore
{
    private readonly ImportServiceOptions _options;
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public BatchStateStore(IOptions<ImportServiceOptions> options)
    {
        _options = options.Value;
    }

    public async Task<SuccessfulBatchState?> LoadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_options.StateFile))
            return null;

        await using var stream = File.OpenRead(_options.StateFile);
        return await JsonSerializer.DeserializeAsync<SuccessfulBatchState>(
            stream,
            JsonOptions,
            cancellationToken);
    }

    public async Task SaveAsync(SuccessfulBatchState state, CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(_options.StateFile)
            ?? throw new InvalidOperationException("Le chemin du fichier d'état est invalide.");
        Directory.CreateDirectory(directory);

        var temporaryPath = _options.StateFile + ".tmp";
        await using (var stream = new FileStream(
            temporaryPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            4096,
            FileOptions.WriteThrough))
        {
            await JsonSerializer.SerializeAsync(stream, state, JsonOptions, cancellationToken);
            await stream.FlushAsync(cancellationToken);
        }

        File.Move(temporaryPath, _options.StateFile, overwrite: true);
    }
}
