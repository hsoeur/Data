using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic.FileIO;
using Serilog;
using System.Security.Cryptography;

namespace GenDb.Services;

public sealed class ExportBatchCoordinator
{
    private static readonly IReadOnlyDictionary<string, int> MinimumColumns =
        new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            ["Articles"] = 7,
            ["BillOfMaterials"] = 7,
            ["Users"] = 6,
            ["Operations"] = 7,
            ["ManufacturingOrders"] = 6,
            ["FirmwareVersions"] = 2,
            ["OrderLines"] = 3
        };

    private readonly IConfiguration _configuration;
    private readonly ImportServiceOptions _options;
    private readonly BatchStateStore _stateStore;
    private readonly DatabaseImporter _importer;
    private string? _candidateSignature;
    private DateTime _candidateObservedAtUtc;
    private string? _lastWaitingReason;

    public ExportBatchCoordinator(
        IConfiguration configuration,
        IOptions<ImportServiceOptions> options,
        BatchStateStore stateStore,
        DatabaseImporter importer)
    {
        _configuration = configuration;
        _options = options.Value;
        _stateStore = stateStore;
        _importer = importer;
    }

    public async Task CheckAndImportAsync(CancellationToken cancellationToken)
    {
        var expected = GetExpectedExports();
        var observations = InspectFiles(expected);
        if (observations is null)
            return;

        if (!ValidateTimestamps(observations))
            return;

        var previousBatch = await _stateStore.LoadAsync(cancellationToken);
        if (!IsNewBatch(observations, previousBatch))
        {
            Wait("Tous les fichiers n'ont pas encore été renouvelés depuis le dernier import réussi.");
            return;
        }

        var signature = string.Join(
            '|',
            observations.OrderBy(file => file.Export.Key).Select(file =>
                $"{file.Export.Key}:{file.Length}:{file.LastWriteTimeUtc.Ticks}"));
        var now = DateTime.UtcNow;
        if (!string.Equals(signature, _candidateSignature, StringComparison.Ordinal))
        {
            _candidateSignature = signature;
            _candidateObservedAtUtc = now;
            foreach (var file in observations)
            {
                Log.Information(
                    "Export candidat Key={Key} Path={Path} LastWriteTimeUtc={LastWriteTimeUtc} SizeBytes={SizeBytes}",
                    file.Export.Key,
                    file.Export.SourcePath,
                    file.LastWriteTimeUtc,
                    file.Length);
            }
            Wait("Nouveau lot détecté ; attente de sa stabilisation.");
            return;
        }

        var stableFor = now - _candidateObservedAtUtc;
        if (stableFor < TimeSpan.FromSeconds(Math.Max(1, _options.StabilitySeconds)))
        {
            Wait($"Lot stable depuis {stableFor.TotalSeconds:F0}s ; délai requis {_options.StabilitySeconds}s.");
            return;
        }

        if (!CanOpenExclusively(observations))
            return;

        PreparedBatch? preparedBatch = null;
        try
        {
            preparedBatch = await PrepareSnapshotAsync(observations, cancellationToken);
            ValidateRowCounts(preparedBatch.Files, previousBatch);
            await _importer.ImportAsync(preparedBatch.SnapshotConfiguration, cancellationToken);

            var state = new SuccessfulBatchState(
                preparedBatch.BatchId,
                DateTime.UtcNow,
                preparedBatch.Files);
            await _stateStore.SaveAsync(state, cancellationToken);

            Log.Information(
                "Lot GPAO importé BatchId={BatchId} Files={FileCount}",
                preparedBatch.BatchId,
                preparedBatch.Files.Count);
            _candidateSignature = null;
            _lastWaitingReason = null;

            if (!_options.KeepSuccessfulSnapshots)
            {
                try
                {
                    Directory.Delete(preparedBatch.SnapshotDirectory, recursive: true);
                }
                catch (Exception ex)
                {
                    Log.Warning(
                        ex,
                        "Impossible de supprimer le snapshot importé {SnapshotDirectory}",
                        preparedBatch.SnapshotDirectory);
                }
            }
        }
        catch (BatchNotReadyException ex)
        {
            Wait(ex.Message);
            _candidateSignature = null;
            if (preparedBatch is not null && Directory.Exists(preparedBatch.SnapshotDirectory))
                TryDeleteSnapshot(preparedBatch.SnapshotDirectory, "refusé");
        }
        catch
        {
            if (preparedBatch is not null)
                Log.Error("Le snapshot en échec est conservé dans {SnapshotDirectory}", preparedBatch.SnapshotDirectory);
            throw;
        }
    }

    private IReadOnlyList<ExpectedExport> GetExpectedExports()
    {
        var exports = new List<ExpectedExport>();
        foreach (var item in MinimumColumns)
        {
            var configuredPath = _configuration[$"CsvPaths:{item.Key}"];
            if (string.IsNullOrWhiteSpace(configuredPath))
                throw new InvalidOperationException($"CsvPaths:{item.Key} n'est pas configuré.");

            exports.Add(new ExpectedExport(
                item.Key,
                Path.GetFullPath(configuredPath, AppContext.BaseDirectory),
                item.Value));
        }

        return exports;
    }

    private IReadOnlyList<FileObservation>? InspectFiles(IReadOnlyList<ExpectedExport> expected)
    {
        var observations = new List<FileObservation>(expected.Count);
        foreach (var export in expected)
        {
            var info = new FileInfo(export.SourcePath);
            if (!info.Exists)
            {
                Wait($"Fichier absent : {export.SourcePath}");
                return null;
            }

            if (info.Length == 0)
            {
                Wait($"Fichier vide : {export.SourcePath}");
                return null;
            }

            observations.Add(new FileObservation(export, info.Length, info.LastWriteTimeUtc));
        }

        return observations;
    }

    private bool ValidateTimestamps(IReadOnlyList<FileObservation> files)
    {
        var now = DateTime.UtcNow;
        var oldest = files.Min(file => file.LastWriteTimeUtc);
        var newest = files.Max(file => file.LastWriteTimeUtc);

        if (oldest < now.AddHours(-Math.Max(1, _options.MaximumFileAgeHours)))
        {
            Wait($"Lot périmé : le fichier le plus ancien date de {oldest:O}.");
            return false;
        }

        if (newest > now.AddMinutes(5))
        {
            Wait($"Horodatage futur détecté : {newest:O}.");
            return false;
        }

        var spread = newest - oldest;
        if (spread > TimeSpan.FromMinutes(Math.Max(1, _options.MaximumTimestampSpreadMinutes)))
        {
            Wait($"Lot incohérent : écart d'horodatage de {spread.TotalMinutes:F1} minutes.");
            return false;
        }

        return true;
    }

    private static bool IsNewBatch(
        IReadOnlyList<FileObservation> current,
        SuccessfulBatchState? previous)
    {
        if (previous is null)
            return true;

        var previousByKey = previous.Files.ToDictionary(file => file.Key, StringComparer.OrdinalIgnoreCase);
        return current.All(file =>
            !previousByKey.TryGetValue(file.Export.Key, out var oldFile) ||
            oldFile.Length != file.Length ||
            oldFile.LastWriteTimeUtc != file.LastWriteTimeUtc);
    }

    private bool CanOpenExclusively(IReadOnlyList<FileObservation> files)
    {
        foreach (var file in files)
        {
            try
            {
                using var stream = new FileStream(
                    file.Export.SourcePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None);
            }
            catch (IOException)
            {
                Wait($"Fichier encore utilisé : {file.Export.SourcePath}");
                return false;
            }
            catch (UnauthorizedAccessException)
            {
                Wait($"Accès exclusif refusé : {file.Export.SourcePath}");
                return false;
            }
        }

        return true;
    }

    private async Task<PreparedBatch> PrepareSnapshotAsync(
        IReadOnlyList<FileObservation> observations,
        CancellationToken cancellationToken)
    {
        var batchId = $"{DateTime.UtcNow:yyyyMMddTHHmmssfffZ}-{Guid.NewGuid():N}";
        var snapshotDirectory = Path.Combine(_options.ProcessingRoot, batchId);
        Directory.CreateDirectory(snapshotDirectory);
        var states = new List<ExportFileState>(observations.Count);

        try
        {
            foreach (var observation in observations)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var destination = Path.Combine(
                    snapshotDirectory,
                    Path.GetFileName(observation.Export.SourcePath));

                File.Copy(observation.Export.SourcePath, destination, overwrite: false);

                var sourceAfterCopy = new FileInfo(observation.Export.SourcePath);
                if (sourceAfterCopy.Length != observation.Length ||
                    sourceAfterCopy.LastWriteTimeUtc != observation.LastWriteTimeUtc)
                {
                    throw new BatchNotReadyException(
                        $"Le fichier {observation.Export.SourcePath} a changé pendant la copie.");
                }

                var sourceHash = await ComputeSha256Async(observation.Export.SourcePath, cancellationToken);
                var copyHash = await ComputeSha256Async(destination, cancellationToken);
                if (!string.Equals(sourceHash, copyHash, StringComparison.OrdinalIgnoreCase))
                {
                    throw new BatchNotReadyException(
                        $"Le fichier {observation.Export.SourcePath} a changé pendant la copie.");
                }

                var rowCount = ValidateCsv(destination, observation.Export.MinimumColumns);
                if (rowCount < Math.Max(1, _options.MinimumRowsPerFile))
                {
                    throw new BatchNotReadyException(
                        $"Le fichier {destination} ne contient que {rowCount} ligne(s).");
                }

                states.Add(new ExportFileState(
                    observation.Export.Key,
                    observation.Export.SourcePath,
                    destination,
                    observation.Length,
                    observation.LastWriteTimeUtc,
                    copyHash,
                    rowCount));
            }
        }
        catch
        {
            TryDeleteSnapshot(snapshotDirectory, "incomplet");
            throw;
        }

        var overrides = states.ToDictionary(
            state => $"CsvPaths:{state.Key}",
            state => (string?)state.SnapshotPath,
            StringComparer.OrdinalIgnoreCase);
        var snapshotConfiguration = new ConfigurationBuilder()
            .AddConfiguration(_configuration)
            .AddInMemoryCollection(overrides)
            .Build();

        Log.Information(
            "Snapshot validé BatchId={BatchId} Directory={Directory}",
            batchId,
            snapshotDirectory);
        return new PreparedBatch(batchId, snapshotDirectory, states, snapshotConfiguration);
    }

    private void ValidateRowCounts(
        IReadOnlyList<ExportFileState> currentFiles,
        SuccessfulBatchState? previousBatch)
    {
        if (previousBatch is null || _options.MinimumRowCountRatio <= 0)
            return;

        var previousByKey = previousBatch.Files.ToDictionary(
            file => file.Key,
            StringComparer.OrdinalIgnoreCase);
        foreach (var current in currentFiles)
        {
            if (!previousByKey.TryGetValue(current.Key, out var previous) || previous.RowCount <= 0)
                continue;

            var ratio = (double)current.RowCount / previous.RowCount;
            if (ratio < _options.MinimumRowCountRatio)
            {
                throw new BatchNotReadyException(
                    $"Volume anormal pour {current.Key} : {current.RowCount} lignes contre {previous.RowCount} au lot précédent.");
            }
        }
    }

    private static void TryDeleteSnapshot(string snapshotDirectory, string kind)
    {
        if (!Directory.Exists(snapshotDirectory))
            return;

        try
        {
            Directory.Delete(snapshotDirectory, recursive: true);
        }
        catch (Exception ex)
        {
            Log.Warning(
                ex,
                "Impossible de supprimer le snapshot {Kind} {SnapshotDirectory}",
                kind,
                snapshotDirectory);
        }
    }

    private static int ValidateCsv(string path, int minimumColumns)
    {
        var rowNumber = 0;
        using var parser = new TextFieldParser(path);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(";");

        while (!parser.EndOfData)
        {
            rowNumber++;
            string[]? fields;
            try
            {
                fields = parser.ReadFields();
            }
            catch (MalformedLineException ex)
            {
                throw new BatchNotReadyException(
                    $"CSV invalide {path}, ligne {rowNumber} : {ex.Message}");
            }

            if (fields is null || fields.Length < minimumColumns)
            {
                throw new BatchNotReadyException(
                    $"CSV invalide {path}, ligne {rowNumber} : {fields?.Length ?? 0} colonnes au lieu de {minimumColumns}.");
            }

            ValidateRequiredValues(path, rowNumber, fields);
        }

        return rowNumber;
    }

    private static void ValidateRequiredValues(string path, int rowNumber, string[] fields)
    {
        if (string.IsNullOrWhiteSpace(fields[0]))
            throw InvalidValue(path, rowNumber, "identifiant vide");

        var fileName = Path.GetFileName(path);
        if (fileName.Equals("EXPORT_BDDPROD_NOM.CSV", StringComparison.OrdinalIgnoreCase) &&
            string.IsNullOrWhiteSpace(fields[1]))
        {
            throw InvalidValue(path, rowNumber, "composant vide");
        }

        if (fileName.Equals("EXPORT_BDDPROD_OF.CSV", StringComparison.OrdinalIgnoreCase))
        {
            if (!int.TryParse(fields[0], out _))
                throw InvalidValue(path, rowNumber, $"numéro d'OF invalide : {fields[0]}");
            if (!CsvParser.TryParseInt(fields[2], out _))
                throw InvalidValue(path, rowNumber, $"quantité invalide : {fields[2]}");
            if (!CsvParser.TryParseInt(fields[3], out _))
                throw InvalidValue(path, rowNumber, $"premier numéro de série invalide : {fields[3]}");
        }

        if (fileName.Equals("EXPORT_BDDPROD_CMLIG.CSV", StringComparison.OrdinalIgnoreCase) &&
            !CsvParser.TryParseInt(fields[2], out _))
        {
            throw InvalidValue(path, rowNumber, $"quantité invalide : {fields[2]}");
        }

        if (fileName.Equals("EXPORT_BDDPROD_ART.CSV", StringComparison.OrdinalIgnoreCase) &&
            fields.Length > 7 &&
            !string.IsNullOrWhiteSpace(fields[7]) &&
            !CsvParser.TryParseInt(fields[7], out _))
        {
            throw InvalidValue(path, rowNumber, $"quantité GPAO invalide : {fields[7]}");
        }
    }

    private static BatchNotReadyException InvalidValue(
        string path,
        int rowNumber,
        string reason) =>
        new($"CSV invalide {path}, ligne {rowNumber} : {reason}.");

    private static async Task<string> ComputeSha256Async(
        string path,
        CancellationToken cancellationToken)
    {
        await using var stream = new FileStream(
            path,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            81920,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash);
    }

    private void Wait(string reason)
    {
        if (string.Equals(reason, _lastWaitingReason, StringComparison.Ordinal))
            return;

        _lastWaitingReason = reason;
        Log.Information("Import en attente Reason={Reason}", reason);
    }

    private sealed record FileObservation(
        ExpectedExport Export,
        long Length,
        DateTime LastWriteTimeUtc);

    private sealed class BatchNotReadyException : Exception
    {
        public BatchNotReadyException(string message) : base(message)
        {
        }
    }
}
