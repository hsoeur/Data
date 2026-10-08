using DataLib;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenDb.Generators
{
    public class ProductFirmwareGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;
        private readonly ILogger _logger;

        public ProductFirmwareGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:FirmwareVersions"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_COMP.csv";

            // Use shared logger
            _logger = Log.ForContext<ProductFirmwareGenerator>();
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ProductFirmwareGenerator::Update() => Impossible d'importer les données.");
                throw;
            }
        }

        private void GenerateTable()
        {
            ImportData(_context);
        }

        private void ImportData(AppDbContext context)
        {
            var rows = CsvParser.Parse(_csvPath);
            var existing = context.FirmwareVersions
                .ToList()
                .GroupBy(v => v.ArticleUid)
                .ToDictionary(g => g.Key, g => g.ToList());
            int inserted =0, updated =0, deleted =0, skipped =0;

            foreach (var row in rows)
            {
                if (row.Length <2)
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "FirmwareVersion", "Colonnes insuffisantes", string.Join("|", row)); continue; }
                var articleUid = row[0]; var version = row[1];
                if (string.IsNullOrWhiteSpace(articleUid))
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "FirmwareVersion", "ArticleUid manquant", string.Join("|", row)); continue; }
                // Normalize empty version to placeholder "Vx.xx"
                version = string.IsNullOrWhiteSpace(version) ? "Vxx.xx" : version;
                if (!existing.TryGetValue(articleUid, out var firmwareVersions))
                {
                    var firmwareVersion = new FirmwareVersion()
                    {
                        FirmwareVersionUid = Guid.NewGuid().ToString(),
                        ArticleUid = articleUid,
                        Version = version
                    };

                    context.FirmwareVersions.Add(firmwareVersion);
                    existing[articleUid] = new List<FirmwareVersion> { firmwareVersion };
                    inserted++;
                    _logger.Information("Insert {Entity} Key=({ArticleUid},{Version})", "FirmwareVersion", articleUid, version);
                }
                else
                {
                    var firmwareVersion = firmwareVersions.First();
                    var duplicateFirmwareVersions = firmwareVersions.Skip(1).ToList();

                    if (firmwareVersion.Version != version)
                    {
                        firmwareVersion.Version = version;
                        updated++;
                        _logger.Information("Update {Entity} Key=({ArticleUid}) Version={Version}", "FirmwareVersion", articleUid, version);
                    }
                    else
                    {
                        _logger.Information("NoChange {Entity} Key=({ArticleUid},{Version})", "FirmwareVersion", articleUid, version);
                    }

                    if (duplicateFirmwareVersions.Count >0)
                    {
                        context.FirmwareVersions.RemoveRange(duplicateFirmwareVersions);
                        deleted += duplicateFirmwareVersions.Count;
                        existing[articleUid] = new List<FirmwareVersion> { firmwareVersion };
                        _logger.Warning("DeleteDuplicate {Entity} Key=({ArticleUid}) Count={Count}", "FirmwareVersion", articleUid, duplicateFirmwareVersions.Count);
                    }
                }
            }

            context.SaveChanges();
            _logger.Information("Summary {Entity} Inserted={Inserted} Updated={Updated} Deleted={Deleted} Skipped={Skipped}", "FirmwareVersion", inserted, updated, deleted, skipped);
        }
    }
}
