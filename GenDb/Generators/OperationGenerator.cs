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

        public OperationGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:Operations"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_GAMM.CSV";
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception outside of the transaction: {ex}");
                Log.Error(ex, "OperationGenerator::Update() => Transaction interrompue.");
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
                    Log.Error(ex, "OperationGenerator::GenerateTable() => Transaction interrompue.");
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
            // Try replacing comma/point
            var normalized = raw.Replace(',', '.');
            if (decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out v)) return v;
            return 0m;
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            List<string[]> stringOperations = CsvParser.Parse(_csvPath);
            context.Operations.RemoveRange(context.Operations.ToList());
            context.SaveChanges();

            foreach (var stringOperation in stringOperations)
            {
                if (stringOperation.Length < 7)
                    continue;

                var op = new Operation()
                {
                    OperationUid = Guid.NewGuid().ToString(),
                    ProductUid = stringOperation[0],
                    OperationNumber = ParseInt(stringOperation[1]),
                    Label = stringOperation[2],
                    Machine = stringOperation[3],
                    PrepTime = ParseDecimal(stringOperation[4]),
                    ProdTime = ParseDecimal(stringOperation[5]),
                    CreationDate = ParseDate(stringOperation[6])
                };

                context.Operations.Add(op);

                Console.WriteLine($"Produit: {op.ProductUid}, Opération: {op.OperationNumber}, Label: {op.Label}");
            }

            context.SaveChanges();
            transaction.Commit();
            Log.Information("OperationGenerator::Update() => Transaction effectuée.");
        }
    }
}
