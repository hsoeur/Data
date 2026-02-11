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
    public class BomGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;

        public BomGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:BillOfMaterials"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_NOM.CSV";
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "BomGenerator::Update() => Impossible d'importer les données.");
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
                    Log.Error(ex, "BomGenerator::GenerateTable() => Transaction interrompue.");
                }
            }
        }

        private static DateTimeOffset ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return DateTimeOffset.UtcNow;

            var styles = DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal;

            // Try common cultures first (fr-FR then invariant)
            if (DateTimeOffset.TryParse(raw, new CultureInfo("fr-FR"), styles, out var dto)) return dto;
            if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, styles, out dto)) return dto;

            // Try a set of common formats
            string[] formats = new[]
            {
                "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss",
                "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyyMMddHHmmss"
            };
            if (DateTimeOffset.TryParseExact(raw, formats, CultureInfo.InvariantCulture, styles, out dto)) return dto;

            return DateTimeOffset.UtcNow;
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            try
            {
                List<string[]> stringsNomenclatures = CsvParser.Parse(_csvPath);
                // Purge table avant import
                context.BillOfMaterials.RemoveRange(context.BillOfMaterials.ToList());
                context.SaveChanges();

                foreach (var stringNomenclature in stringsNomenclatures)
                {
                    // Sécuriser l'accès aux index
                    if (stringNomenclature.Length <7)
                        continue;

                    var bom = new BillOfMaterial()
                    {
                        BillOfMaterialsUid = Guid.NewGuid().ToString(),
                        Product = stringNomenclature[0],
                        Component = stringNomenclature[1],
                        Operation = stringNomenclature[2],
                        Quantity = stringNomenclature[3],
                        QuantityDiv = stringNomenclature[4],
                        CreationDate = ParseDate(stringNomenclature[5]),
                        EndDate = ParseDate(stringNomenclature[6])
                    };

                    context.BillOfMaterials.Add(bom);

                    Console.WriteLine($"Compound: {bom.Product}, Component: {bom.Component}, Operation: {bom.Operation}, Quantite: {bom.Quantity}");
                }

                context.SaveChanges();
                transaction.Commit();
                Log.Information("BomGenerator::Update() => Transaction effectuée.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "BomGenerator::Update() => Transaction interrompue.");
            }
        }
    }
}
