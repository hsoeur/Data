using DataLib;
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
        private readonly ILogger _logger;

        public UserGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:Users"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_OPE.CSV";

            // Use shared logger
            _logger = Log.ForContext<UserGenerator>();
        }
        public void Update()
        {
            try
            {
                GenerateTable();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "UserGenerator::Update() => Impossible d'importer les données.");
                throw;
            }
        }

        private void GenerateTable()
        {
            ImportData(_context);
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

        private void ImportData(AppDbContext context)
        {
            List<string[]> stringOpes = CsvParser.Parse(_csvPath);
            var existing = context.Users
                .ToList()
                .ToDictionary(u => u.UserUid, u => u);

            int inserted =0, updated =0, skipped =0;

            foreach (var row in stringOpes)
            {
                // Validation structurée par ligne
                if (row.Length <6)
                {
                    skipped++;
                    _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "User", "Colonnes insuffisantes", string.Join("|", row));
                    continue;
                }
                var uid = row[0];
                if (string.IsNullOrWhiteSpace(uid))
                {
                    skipped++;
                    _logger.Warning("Skip {Entity} Reason={Reason} RawLine={RawLine}", "User", "UserUid manquant", string.Join("|", row));
                    continue;
                }

                var ln = row[1];
                var fn = row[2];
                var entry = ParseDate(row[3]);
                var exit = ParseDate(row[4]);
                var dep = row[5];

                if (existing.TryGetValue(uid, out var user))
                {
                    bool anyChange = false;
                    if (user.LastName != ln)
                    {
                        _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "User", uid, "LastName", user.LastName, ln);
                        user.LastName = ln; anyChange = true;
                    }
                    if (user.FirstName != fn)
                    {
                        _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "User", uid, "FirstName", user.FirstName, fn);
                        user.FirstName = fn; anyChange = true;
                    }
                    if (user.EntryDate != entry)
                    {
                        _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "User", uid, "EntryDate", user.EntryDate, entry);
                        user.EntryDate = entry; anyChange = true;
                    }
                    if (user.ExitDate != exit)
                    {
                        _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "User", uid, "ExitDate", user.ExitDate, exit);
                        user.ExitDate = exit; anyChange = true;
                    }
                    if (user.Department != dep)
                    {
                        _logger.Information("Update {Entity} {Uid} Field={Field} Old={Old} New={New}", "User", uid, "Department", user.Department, dep);
                        user.Department = dep; anyChange = true;
                    }
                    if (anyChange) updated++;
                    else _logger.Information("NoChange {Entity} {Uid}", "User", uid);
                }
                else
                {
                    context.Users.Add(new User()
                    {
                        UserUid = uid,
                        LastName = ln,
                        FirstName = fn,
                        EntryDate = entry,
                        ExitDate = exit,
                        Department = dep
                    });
                    inserted++;
                    _logger.Information("Insert {Entity} {Uid} LastName={LastName} FirstName={FirstName} EntryDate={EntryDate} ExitDate={ExitDate} Department={Department}",
                     "User", uid, ln, fn, entry, exit, dep);
                }
            }

            context.SaveChanges();

            _logger.Information("Summary {Entity} Inserted={Inserted} Updated={Updated} Skipped={Skipped}", "User", inserted, updated, skipped);
        }
    }
}
