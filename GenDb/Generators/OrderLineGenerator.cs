using DataLib;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenDb.Generators
{
    public class OrderLineGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;
        private readonly ILogger _logger;

        public OrderLineGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:OrderLines"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_CMLIG.CSV";

            // Use shared logger
            _logger = Log.ForContext<OrderLineGenerator>();
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "OrderLineGenerator::Update() => Transaction interrompue.");
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
                    _logger.Error(ex, "OrderLineGenerator::GenerateTable() => Transaction interrompue.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            var rows = CsvParser.Parse(_csvPath);
            var existing = context.OrderLines.ToList().ToDictionary(o => o.UidOrderLine, o => o);
            int inserted = 0, updated = 0, skipped = 0;

            foreach (var row in rows)
            {
                if (row.Length < 3)
                {
                    skipped++;
                    _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "OrderLine", "Colonnes insuffisantes", string.Join("|", row));
                    continue;
                }
                var uid = row[0];
                if (string.IsNullOrWhiteSpace(uid))
                {
                    skipped++;
                    _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "OrderLine", "Uid manquant", string.Join("|", row));
                    continue;
                }
                int quantity = 0;
                if (!CsvParser.TryParseInt(row[2], out quantity))
                {
                    _logger.Warning("ParseFail {Entity} {Uid} Field={Field} Raw={Raw}", "OrderLine", uid, "Quantity", row[2]);
                }

                if (existing.TryGetValue(uid, out var ol))
                {
                    bool anyChange = false;
                    if (ol.Product != row[1]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "OrderLine", uid, "Product", ol.Product, row[1]); ol.Product = row[1]; anyChange = true; }
                    if (ol.Quantity != quantity) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "OrderLine", uid, "Quantity", ol.Quantity, quantity); ol.Quantity = quantity; anyChange = true; }
                    if (anyChange) updated++; else _logger.Information("NoChange {Entity} {Uid}", "OrderLine", uid);
                }
                else
                {
                    context.OrderLines.Add(new OrderLine()
                    {
                        UidOrderLine = uid,
                        Product = row[1],
                        Quantity = quantity,
                    });
                    inserted++;
                    _logger.Information("Insert {Entity} {Uid} Product={Product} Quantity={Quantity}", "OrderLine", uid, row[1], quantity);
                }
            }

            context.SaveChanges();
            transaction.Commit();
            _logger.Information("Summary {Entity} Inserted={Inserted} Updated={Updated} Skipped={Skipped}", "OrderLine", inserted, updated, skipped);
        }
    }
}
