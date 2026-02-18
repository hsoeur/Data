using DataLib;
using Microsoft.EntityFrameworkCore.Storage;
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
            }
        }

        private void GenerateTable()
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    ImportData(_context, transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.Error(ex, "ProductFirmwareGenerator::Update() => Transaction interrompue.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            var rows = CsvParser.Parse(_csvPath);
            var existing = context.FirmwareVersions.ToList().GroupBy(v => (v.ArticleUid, v.Version)).ToDictionary(g => g.Key, g => g.First());
            int inserted =0, skipped =0;

            foreach (var row in rows)
            {
                if (row.Length <2)
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "FirmwareVersion", "Colonnes insuffisantes", string.Join("|", row)); continue; }
                var articleUid = row[0]; var version = row[1];
                if (string.IsNullOrWhiteSpace(articleUid) || string.IsNullOrWhiteSpace(version))
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "FirmwareVersion", "ArticleUid/Version manquants", string.Join("|", row)); continue; }
                var key = (articleUid, version);

                if (!existing.ContainsKey(key))
                {
                    context.FirmwareVersions.Add(new FirmwareVersion()
                    {
                        FirmwareVersionUid = Guid.NewGuid().ToString(),
                        ArticleUid = articleUid,
                        Version = version
                    });
                    inserted++;
                    _logger.Information("Insert {Entity} Key=({ArticleUid},{Version})", "FirmwareVersion", articleUid, version);
                }
                else
                {
                    _logger.Information("NoChange {Entity} Key=({ArticleUid},{Version})", "FirmwareVersion", articleUid, version);
                }
            }

            context.SaveChanges();
            transaction.Commit();
            _logger.Information("Summary {Entity} Inserted={Inserted} Skipped={Skipped}", "FirmwareVersion", inserted, skipped);
        }
    }
}
