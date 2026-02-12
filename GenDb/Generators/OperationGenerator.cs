using DataLib;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace GenDb.Generators
{
    public class OperationGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;
        private readonly ILogger _logger;

        public OperationGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:Operations"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_GAMM.CSV";

            var logDir = Path.Combine("logs", $"{DateTime.Now:yyyyMMdd}");
            Directory.CreateDirectory(logDir);
            var filePath = Path.Combine(logDir, "OperationGenerator.log");
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(filePath, retainedFileCountLimit: null, shared: true)
                .CreateLogger();
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OperationGenerator::Update() => Transaction interrompue.");
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
                    _logger.Error(ex, "OperationGenerator::GenerateTable() => Transaction interrompue.");
                }
            }
        }

        private static DateTimeOffset ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return DateTimeOffset.UtcNow;
            var styles = DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal;
            if (DateTimeOffset.TryParse(raw, new CultureInfo("fr-FR"), styles, out var dto)) return dto;
            if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, styles, out dto)) return dto;
            string[] formats = new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss", "yyyyMMdd", "yyyyMMddHHmmss" };
            if (DateTimeOffset.TryParseExact(raw, formats, CultureInfo.InvariantCulture, styles, out dto)) return dto;
            return DateTimeOffset.UtcNow;
        }
        private static int ParseInt(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return 0;
            if (int.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v)) return v;
            if (int.TryParse(raw, NumberStyles.Integer, new CultureInfo("fr-FR"), out v)) return v;
            return 0;
        }
        private static decimal ParseDecimal(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return 0m;
            if (decimal.TryParse(raw, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, CultureInfo.InvariantCulture, out var v)) return v;
            if (decimal.TryParse(raw, NumberStyles.Number | NumberStyles.AllowCurrencySymbol, new CultureInfo("fr-FR"), out v)) return v;
            var normalized = raw.Replace(',', '.');
            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out v)) return v;
            return 0m;
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            var rows = CsvParser.Parse(_csvPath);
            var existing = context.Operations
                .ToList()
                .GroupBy(o => (o.ProductUid, o.OperationNumber))
                .ToDictionary(g => g.Key, g => g.First());
            int inserted = 0, updated = 0, skipped = 0;

            foreach (var row in rows)
            {
                if (row.Length < 7)
                {
                    skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "Operation", "Colonnes insuffisantes", string.Join("|", row)); continue;
                }
                var productUid = row[0];
                var opNumber = ParseInt(row[1]);
                if (string.IsNullOrWhiteSpace(productUid))
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "Operation", "ProductUid manquant", string.Join("|", row)); continue; }
                var key = (productUid, opNumber);
                var label = row[2];
                var machine = row[3];
                var prep = ParseDecimal(row[4]);
                var prod = ParseDecimal(row[5]);
                var created = ParseDate(row[6]);

                if (existing.TryGetValue(key, out var op))
                {
                    bool anyChange = false;
                    if (op.Label != label) { _logger.Information("Update {Entity} Key=({ProductUid},{OperationNumber}) Field={Field} Old={Old} New={New}", "Operation", productUid, opNumber, "Label", op.Label, label); op.Label = label; anyChange = true; }
                    if (op.Machine != machine) { _logger.Information("Update {Entity} Key=({ProductUid},{OperationNumber}) Field={Field} Old={Old} New={New}", "Operation", productUid, opNumber, "Machine", op.Machine, machine); op.Machine = machine; anyChange = true; }
                    if (op.PrepTime != prep) { _logger.Information("Update {Entity} Key=({ProductUid},{OperationNumber}) Field={Field} Old={Old} New={New}", "Operation", productUid, opNumber, "PrepTime", op.PrepTime, prep); op.PrepTime = prep; anyChange = true; }
                    if (op.ProdTime != prod) { _logger.Information("Update {Entity} Key=({ProductUid},{OperationNumber}) Field={Field} Old={Old} New={New}", "Operation", productUid, opNumber, "ProdTime", op.ProdTime, prod); op.ProdTime = prod; anyChange = true; }
                    if (op.CreationDate != created) { _logger.Information("Update {Entity} Key=({ProductUid},{OperationNumber}) Field={Field} Old={Old} New={New}", "Operation", productUid, opNumber, "CreationDate", op.CreationDate, created); op.CreationDate = created; anyChange = true; }
                    if (anyChange) updated++; else _logger.Information("NoChange {Entity} Key=({ProductUid},{OperationNumber})", "Operation", productUid, opNumber);
                }
                else
                {
                    context.Operations.Add(new Operation()
                    {
                        OperationUid = Guid.NewGuid().ToString(),
                        ProductUid = productUid,
                        OperationNumber = opNumber,
                        Label = label,
                        Machine = machine,
                        PrepTime = prep,
                        ProdTime = prod,
                        CreationDate = created
                    });
                    inserted++;
                    _logger.Information("Insert {Entity} Key=({ProductUid},{OperationNumber}) Label={Label} Machine={Machine} PrepTime={PrepTime} ProdTime={ProdTime} CreationDate={CreationDate}",
                        "Operation", productUid, opNumber, label, machine, prep, prod, created);
                }
            }

            context.SaveChanges();
            transaction.Commit();
            _logger.Information("Summary {Entity} Inserted={Inserted} Updated={Updated} Skipped={Skipped}", "Operation", inserted, updated, skipped);
        }
    }
}
