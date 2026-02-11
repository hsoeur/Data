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

        public ProductFirmwareGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:FirmwareVersions"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_COMP.csv";
        }

        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "ProductFirmwareGenerator::Update() => Impossible d'importer les données.");
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
                    Log.Error(ex, "ProductFirmwareGenerator::Update() => Transaction interrompue.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            List<string[]> stringVersions = CsvParser.Parse(_csvPath);
            context.FirmwareVersions.RemoveRange(context.FirmwareVersions.ToList());
            context.SaveChanges();

            foreach (var stringVersion in stringVersions)
            {
                if (stringVersion.Length <2) continue;

                context.FirmwareVersions.Add(new FirmwareVersion()
                {
                    FirmwareVersionUid = Guid.NewGuid().ToString(),
                    ArticleUid = stringVersion[0],
                    Version = stringVersion[1]
                });

                Console.WriteLine($"UIDArticle: {stringVersion[0]}, Version: {stringVersion[1]}");
            }

            context.SaveChanges();
            transaction.Commit();
            Log.Information("ProductFirmwareGenerator::Update() => Transaction effectuée.");
        }
    }
}
