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

        public OrderLineGenerator(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _csvPath = configuration["CsvPaths:OrderLines"] ?? "C:\\Data\\Export_GPAO\\EXPORT_BDDPROD_CMLIG.CSV";
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
                Log.Error(ex, "OrderLineGenerator::Update() => Transaction interrompue.");
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
                    Log.Error(ex, "OrderLineGenerator::GenerateTable() => Transaction interrompue.");
                }
            }
        }

        private void ImportData(AppDbContext context, IDbContextTransaction transaction)
        {
            List<string[]> stringOrderLines = CsvParser.Parse(_csvPath);

            context.OrderLines.RemoveRange(context.OrderLines.ToList());
            context.SaveChanges();

            foreach (var stringOrderLine in stringOrderLines)
            {
                int quantity =0;
                if (stringOrderLine.Length >2)
                {
                    int.TryParse(stringOrderLine[2], out quantity);
                }

                context.OrderLines.Add(new OrderLine()
                {
                    UidOrderLine = stringOrderLine[0],
                    Product = stringOrderLine[1],
                    Quantity = quantity,
                });

                Console.WriteLine($"Code: {stringOrderLine[0]}, Description: {stringOrderLine[1]}, Référence: {stringOrderLine[2]}");
            }

            context.SaveChanges();
            transaction.Commit();
            Log.Information("OrderLineGenerator::Update() => Transaction effectuée.");
        }
    }
}
