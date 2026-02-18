using DataLib;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GenDb.Generators
{
    public class ManufacturingOrderGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;
        private readonly ILogger _logger;

        public ManufacturingOrderGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:ManufacturingOrders"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_OF.CSV";

            // Use shared logger
            _logger = Log.ForContext<ManufacturingOrderGenerator>();
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ManufacturingOrderGenerator::Update() => Impossible d'importer les données.");
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
                    _logger.Error(ex, "ManufacturingOrderGenerator::Update() => Transaction annulée.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            try
            {
                var rows = CsvParser.Parse(_csvPath);
                var existing = context.ManufacturingOrders.ToDictionary(of => of.ManufacturingOrderUid);
                int inserted =0, updated =0, skipped =0;

                foreach (var row in rows)
                {
                    if (row.Length <6)
                    { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "ManufacturingOrder", "Colonnes insuffisantes", string.Join("|", row)); continue; }
                    if (!int.TryParse(row[0], out var uid))
                    { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "ManufacturingOrder", "Uid invalide", string.Join("|", row)); continue; }
                    var productUid = row[1];
                    if (!CsvParser.TryParseInt(row[2], out var quantity)) { _logger.Warning("ParseFail {Entity} {Uid} Field={Field} Raw={Raw}", "ManufacturingOrder", uid, "Quantity", row[2]); }
                    if (!CsvParser.TryParseInt(row[3], out var firstSerialNumber)) { _logger.Warning("ParseFail {Entity} {Uid} Field={Field} Raw={Raw}", "ManufacturingOrder", uid, "FirstSerialNumber", row[3]); }
                    var internalOrder = row[4] ?? string.Empty;
                    var externalOrder = row[5] ?? string.Empty;

                    if (existing.TryGetValue(uid, out var of))
                    {
                        // Log differences only, do not update existing records
                        bool anyChange = false;
                        if (of.ProductUid != productUid) { _logger.Information("Diff {Entity} {Uid} Field={Field} Old={Old} New={New}", "ManufacturingOrder", uid, "ProductUid", of.ProductUid, productUid); anyChange = true; }
                        if (of.Quantity != quantity) { _logger.Information("Diff {Entity} {Uid} Field={Field} Old={Old} New={New}", "ManufacturingOrder", uid, "Quantity", of.Quantity, quantity); anyChange = true; }
                        if (of.FirstSerialNumber != firstSerialNumber) { _logger.Information("Diff {Entity} {Uid} Field={Field} Old={Old} New={New}", "ManufacturingOrder", uid, "FirstSerialNumber", of.FirstSerialNumber, firstSerialNumber); anyChange = true; }
                        if (of.InternalOrderNumber != internalOrder) { _logger.Information("Diff {Entity} {Uid} Field={Field} Old={Old} New={New}", "ManufacturingOrder", uid, "InternalOrderNumber", of.InternalOrderNumber, internalOrder); anyChange = true; }
                        if (of.ExternalOrderNumber != externalOrder) { _logger.Information("Diff {Entity} {Uid} Field={Field} Old={Old} New={New}", "ManufacturingOrder", uid, "ExternalOrderNumber", of.ExternalOrderNumber, externalOrder); anyChange = true; }
                        if (anyChange) updated++; else _logger.Information("NoChange {Entity} {Uid}", "ManufacturingOrder", uid);
                    }
                    else
                    {
                        var newOF = new ManufacturingOrder()
                        {
                            ManufacturingOrderUid = uid,
                            ProductUid = productUid,
                            Quantity = quantity,
                            FirstSerialNumber = firstSerialNumber,
                            InternalOrderNumber = internalOrder,
                            ExternalOrderNumber = externalOrder,
                            Status = "CREATED"
                        };
                        context.ManufacturingOrders.Add(newOF);
                        inserted++;
                        _logger.Information("Insert {Entity} {Uid} ProductUid={ProductUid} Quantity={Quantity} FirstSerialNumber={FirstSerialNumber} InternalOrderNumber={InternalOrderNumber} ExternalOrderNumber={ExternalOrderNumber}", "ManufacturingOrder", uid, productUid, quantity, firstSerialNumber, internalOrder, externalOrder);
                    }
                }

                context.SaveChanges();
                transaction.Commit();
                _logger.Information("Summary {Entity} Inserted={Inserted} Updated(LogOnly)={Updated} Skipped={Skipped}", "ManufacturingOrder", inserted, updated, skipped);
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.Error(ex, "ManufacturingOrderGenerator::ImportData() => Erreur import.");
                throw;
            }
        }
    }
}
