using DataLib;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GenDb.Generators
{
    public class ArticleGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;
        private readonly ILogger _logger;

        public ArticleGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:Articles"] ?? Path.Combine(Directory.GetCurrentDirectory(), "EXPORT_GPAO", "EXPORT_BDDPROD_ART.CSV");

            var logDir = Path.Combine("logs", $"{DateTime.Now:yyyyMMdd}");
            Directory.CreateDirectory(logDir);
            var filePath = Path.Combine(logDir, "ArticleGenerator.log");
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
                _logger.Error(ex, "ArticleGenerator::Update() => Transaction interrompue.");
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
                    _logger.Error(ex, "ArticleGenerator::GenerateTable() => Transaction interrompue.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            var rows = CsvParser.Parse(_csvPath);
            var existing = context.Articles.ToList().ToDictionary(a => a.ArticleUid, a => a);
            int inserted =0, updated =0, skipped =0;

            foreach (var row in rows)
            {
                if (row.Length <7)
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "Article", "Colonnes insuffisantes", string.Join("|", row)); continue; }
                var uid = row[0];
                if (string.IsNullOrWhiteSpace(uid))
                { skipped++; _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "Article", "ArticleUid manquant", string.Join("|", row)); continue; }
                int gpaoQuantity =0; if (row.Length >7) int.TryParse(row[7], out gpaoQuantity);

                if (existing.TryGetValue(uid, out var a))
                {
                    bool anyChange = false;
                    if (a.Designation != row[1]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "Designation", a.Designation, row[1]); a.Designation = row[1]; anyChange = true; }
                    if (a.Store != row[2]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "Store", a.Store, row[2]); a.Store = row[2]; anyChange = true; }
                    if (a.Family != row[3]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "Family", a.Family, row[3]); a.Family = row[3]; anyChange = true; }
                    if (a.Packaging != row[4]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "Packaging", a.Packaging, row[4]); a.Packaging = row[4]; anyChange = true; }
                    if (a.ManageStock != row[5]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "ManageStock", a.ManageStock, row[5]); a.ManageStock = row[5]; anyChange = true; }
                    if (a.SupplierReference != row[6]) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "SupplierReference", a.SupplierReference, row[6]); a.SupplierReference = row[6]; anyChange = true; }
                    if (a.GpaoQuantity != gpaoQuantity) { _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "Article", uid, "GpaoQuantity", a.GpaoQuantity, gpaoQuantity); a.GpaoQuantity = gpaoQuantity; anyChange = true; }
                    if (anyChange) updated++; else _logger.Information("NoChange {Entity} {Uid}", "Article", uid);
                }
                else
                {
                    context.Articles.Add(new Article()
                    {
                        ArticleUid = uid,
                        Designation = row[1],
                        Store = row[2],
                        Family = row[3],
                        Packaging = row[4],
                        ManageStock = row[5],
                        SupplierReference = row[6],
                        GpaoQuantity = gpaoQuantity,
                    });
                    inserted++;
                    _logger.Information("Insert {Entity} {Uid} Designation={Designation} Store={Store} Family={Family} Packaging={Packaging} ManageStock={ManageStock} SupplierReference={SupplierReference} GpaoQuantity={GpaoQuantity}",
                     "Article", uid, row[1], row[2], row[3], row[4], row[5], row[6], gpaoQuantity);
                }
            }

            context.SaveChanges();
            transaction.Commit();
            _logger.Information("Summary {Entity} Inserted={Inserted} Updated={Updated} Skipped={Skipped}", "Article", inserted, updated, skipped);
        }
    }
}
