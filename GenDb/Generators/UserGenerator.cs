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
    public class UserGenerator : IGenerator
    {
        private readonly AppDbContext _context;
        private readonly string _csvPath;

        public UserGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:Users"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_OPE.CSV";
        }
        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "UserGenerator::Update() => Impossible d'importer les données.");
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
                    Log.Error(ex, "UserGenerator::Update() => Transaction interrompue.");
                }
            }
        }

        private static DateTimeOffset ParseDate(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return DateTimeOffset.UtcNow;

            var styles = DateTimeStyles.AssumeLocal | DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal;

            if (DateTimeOffset.TryParse(raw, new CultureInfo("fr-FR"), styles, out var dto)) return dto;
            if (DateTimeOffset.TryParse(raw, CultureInfo.InvariantCulture, styles, out dto)) return dto;

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
            List<string[]> stringOpes = CsvParser.Parse(_csvPath);

            context.Users.RemoveRange(context.Users.ToList());
            context.SaveChanges();

            foreach (var stringOpe in stringOpes)
            {
                if (stringOpe.Length <6)
                    continue;

                context.Users.Add(new User()
                {
                    UserUid = stringOpe[0],
                    LastName = stringOpe[1],
                    FirstName = stringOpe[2],
                    EntryDate = ParseDate(stringOpe[3]),
                    ExitDate = ParseDate(stringOpe[4]),
                    Department = stringOpe[5]
                });

                Console.WriteLine($"UIDOperateur: {stringOpe[0]}, Nom: {stringOpe[1]}, Prenom: {stringOpe[2]}, DateEntree: {stringOpe[3]}");
            }

            context.SaveChanges();
            transaction.Commit();
            Log.Information("UserGenerator::Update() => Transaction effectuée.");
        }
    }
}
