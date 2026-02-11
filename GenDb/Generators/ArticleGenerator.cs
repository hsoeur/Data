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

        public ArticleGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:Articles"] ?? Path.Combine(Directory.GetCurrentDirectory(), "EXPORT_GPAO", "EXPORT_BDDPROD_ART.CSV");
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
                Log.Error(ex, "ArticleGenerator::Update() => Transaction interrompue.");

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
                    Log.Error(ex, "ArticleGenerator::GenerateTable() => Transaction interrompue.");

                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            List<string[]> stringArticles = CsvParser.Parse(_csvPath);



            // Supprimer les articles existants avant d'insérer les nouveaux
            context.Articles.RemoveRange(context.Articles
                .ToList());

            context.SaveChanges(); // Appliquer la suppression avant d'insérer les nouveaux articles

            foreach (var stringArticle in stringArticles)
            {
                var existingArticle = context.Articles.FirstOrDefault(a => a.ArticleUid == stringArticle[0]);

                if (existingArticle != null)
                {
                    // Si l'article existe on le garde tel quel
                    continue;
                }

                int gpaoQuantity =0;
                if (stringArticle.Length >7)
                {
                    int.TryParse(stringArticle[7], out gpaoQuantity);
                }

                context.Articles.Add(new Article()
                {
                    ArticleUid = stringArticle[0],
                    Designation = stringArticle[1],
                    Store = stringArticle[2],
                    Family = stringArticle[3],
                    Packaging = stringArticle[4],
                    ManageStock = stringArticle[5],
                    SupplierReference = stringArticle[6],
                    GpaoQuantity = gpaoQuantity,
                });

                Console.WriteLine($"Code: {stringArticle[0]}, Description: {stringArticle[1]}, Référence: {stringArticle[2]}");
            }

            context.SaveChanges();
            transaction.Commit();
            Log.Information("ArticlesGenerator::Update() => Transaction effectuée.");
        }
    }
}
