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

        public ManufacturingOrderGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:ManufacturingOrders"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_OF.CSV";
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ManufacturingOrderGenerator::Update() => Impossible d'importer les données.");
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
                    Log.Error(ex, "ManufacturingOrderGenerator::Update() => Transaction annulée.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            try
            {
                List<string[]> stringOFs = CsvParser.Parse(_csvPath);
                var existingOFs = context.ManufacturingOrders.ToDictionary(of => of.ManufacturingOrderUid);
                var csvManufacturingOrderUids = new HashSet<int>();

                foreach (var stringOF in stringOFs)
                {
                    if (stringOF.Length < 6)
                        continue;

                    if (!int.TryParse(stringOF[0], out var manufacturingOrderUid))
                        continue;
                    csvManufacturingOrderUids.Add(manufacturingOrderUid);

                    var productUid = stringOF[1];
                    int.TryParse(stringOF[2], out var quantity);
                    int.TryParse(stringOF[3], out var firstSerialNumber);
                    var horizonOrder = stringOF[4] ?? string.Empty;
                    var amphitechOrder = stringOF[5] ?? string.Empty;

                    if (existingOFs.TryGetValue(manufacturingOrderUid, out var existingOF))
                    {
                        LogDifferences(existingOF, productUid, quantity, firstSerialNumber, horizonOrder, amphitechOrder);
                    }
                    else
                    {
                        var newOF = new ManufacturingOrder()
                        {
                            ManufacturingOrderUid = manufacturingOrderUid,
                            ProductUid = productUid,
                            Quantity = quantity,
                            FirstSerialNumber = firstSerialNumber,
                            InternalOrderNumber = horizonOrder,
                            ExternalOrderNumber = amphitechOrder,
                            Status = "CREATED"
                        };
                        context.ManufacturingOrders.Add(newOF);
                    }
                }

                var ofToDelete = existingOFs
                    .Where(kv => kv.Value.Status != "ACTIVE" && !csvManufacturingOrderUids.Contains(kv.Key))
                    .Select(kv => kv.Value)
                    .ToList();
                foreach (var of in ofToDelete)
                {
                    context.ManufacturingOrders.Remove(of);
                }

                context.SaveChanges();
                transaction.Commit();
                Log.Information("Transaction validée avec succès.");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                Log.Error(ex, "Une erreur s'est produite lors de l'importation des données. Transaction annulée.");
                throw;
            }
        }

        private void LogDifferences(ManufacturingOrder existingOF, string productUid, int quantity, int firstSerialNumber, string internalOrder, string externalOrder)
        {
            List<string> differences = new();
            if (existingOF.ProductUid != productUid) differences.Add($"Produit UID différent - Ancien: {existingOF.ProductUid}, Nouveau: {productUid}");
            if (existingOF.Quantity != quantity) differences.Add($"Quantité différente - Ancien: {existingOF.Quantity}, Nouveau: {quantity}");
            if (existingOF.FirstSerialNumber != firstSerialNumber) differences.Add($"Numéro de série premier différent - Ancien: {existingOF.FirstSerialNumber}, Nouveau: {firstSerialNumber}");
            if (existingOF.InternalOrderNumber != internalOrder) differences.Add($"Commande Horizon différente - Ancien: {existingOF.InternalOrderNumber}, Nouveau: {internalOrder}");
            if (existingOF.ExternalOrderNumber != externalOrder) differences.Add($"Commande Amphitech différente - Ancien: {existingOF.ExternalOrderNumber}, Nouveau: {externalOrder}");
            if (differences.Any())
            {
                Log.Warning($"OF {existingOF.ManufacturingOrderUid} a des différences détectées:\n{string.Join("\n", differences)}");
            }
        }
    }
}
