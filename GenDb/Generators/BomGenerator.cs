using DataLib;
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
        private readonly ILogger _logger;

        public BomGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:BillOfMaterials"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_NOM.CSV";

            // Use shared logger
            _logger = Log.ForContext<BomGenerator>();
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "BomGenerator::Update() => Impossible d'importer les données.");
                throw;
            }
        }

        private void GenerateTable()
        {
            ImportData(_context);
        }

        private static DateTimeOffset ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return DateTimeOffset.UtcNow;
            var styles = DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal;
            if (DateTimeOffset.TryParse(raw, new CultureInfo("fr-FR"), styles, out var dto)) return dto;
            if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, styles, out dto)) return dto;
            string[] formats = new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "dd-MM-yyyy", "dd-MM-yyyy HH:mm:ss", "yyyy-MM-dd", "yyyy-MM-dd HH:mm:ss", "yyyyMMdd", "yyyyMMddHHmmss" };
            if (DateTimeOffset.TryParseExact(raw, formats, CultureInfo.InvariantCulture, styles, out dto)) return dto;
            return DateTimeOffset.UtcNow;
        }

        private void ImportData(AppDbContext context)
        {
            var rows = CsvParser.Parse(_csvPath);
            var existing = context.BillOfMaterials.ToList().GroupBy(b => (b.Product, b.Component, b.Operation)).ToDictionary(g => g.Key, g => g.First());
            int inserted =0, updated =0, skipped =0;

            foreach (var row in rows)
            {
                if (row.Length <7)
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "BillOfMaterial", "Colonnes insuffisantes", string.Join("|", row)); continue; }
                var product = row[0]; var component = row[1]; var operation = row[2];
                if (string.IsNullOrWhiteSpace(product) || string.IsNullOrWhiteSpace(component))
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "BillOfMaterial", "Product/Component manquants", string.Join("|", row)); continue; }
                var key = (product, component, operation);
                var quantity = row[3]; var quantityDiv = row[4]; var creationDate = ParseDate(row[5]); var endDate = ParseDate(row[6]);

                if (existing.TryGetValue(key, out var bom))
                {
                    bool anyChange = false;
                    if (bom.Quantity != quantity) { _logger.Information("Update {Entity} Key=({Product},{Component},{Operation}) Field={Field} Old={Old} New={New}", "BillOfMaterial", product, component, operation, "Quantity", bom.Quantity, quantity); bom.Quantity = quantity; anyChange = true; }
                    if (bom.QuantityDiv != quantityDiv) { _logger.Information("Update {Entity} Key=({Product},{Component},{Operation}) Field={Field} Old={Old} New={New}", "BillOfMaterial", product, component, operation, "QuantityDiv", bom.QuantityDiv, quantityDiv); bom.QuantityDiv = quantityDiv; anyChange = true; }
                    if (bom.CreationDate != creationDate) { _logger.Information("Update {Entity} Key=({Product},{Component},{Operation}) Field={Field} Old={Old} New={New}", "BillOfMaterial", product, component, operation, "CreationDate", bom.CreationDate, creationDate); bom.CreationDate = creationDate; anyChange = true; }
                    if (bom.EndDate != endDate) { _logger.Information("Update {Entity} Key=({Product},{Component},{Operation}) Field={Field} Old={Old} New={New}", "BillOfMaterial", product, component, operation, "EndDate", bom.EndDate, endDate); bom.EndDate = endDate; anyChange = true; }
                    if (anyChange) updated++; else _logger.Information("NoChange {Entity} Key=({Product},{Component},{Operation})", "BillOfMaterial", product, component, operation);
                }
                else
                {
                    context.BillOfMaterials.Add(new BillOfMaterial()
                    {
                        BillOfMaterialsUid = Guid.NewGuid().ToString(),
                        Product = product,
                        Component = component,
                        Operation = operation,
                        Quantity = quantity,
                        QuantityDiv = quantityDiv,
                        CreationDate = creationDate,
                        EndDate = endDate
                    });
                    inserted++;
                    _logger.Information("Insert {Entity} Key=({Product},{Component},{Operation}) Quantity={Quantity} QuantityDiv={QuantityDiv} CreationDate={CreationDate} EndDate={EndDate}",
                     "BillOfMaterial", product, component, operation, quantity, quantityDiv, creationDate, endDate);
                }
            }

            context.SaveChanges();
            _logger.Information("Summary {Entity} Inserted={Inserted} Updated={Updated} Skipped={Skipped}", "BillOfMaterial", inserted, updated, skipped);
        }
    }
}
