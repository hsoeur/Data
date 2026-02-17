using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenDb
{
    public class CsvParser
    {
        public static List<string[]> Parse(string filePath)
        {
            List<string[]> stringObjects = new List<string[]>();

            using (TextFieldParser parser = new TextFieldParser(filePath))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");

                while (!parser.EndOfData)
                {
                    string[] fields = parser.ReadFields();

                    if (fields != null)
                    {
                        stringObjects.Add(fields);
                    }
                }

                return stringObjects;   
            }
        }

        // Robust integer parsing for CSV numbers that may contain spaces, thousand separators, or commas.
        public static bool TryParseInt(string? input, out int value)
        {
            value =0;
            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Normalize: trim, remove spaces, unify comma to dot, then try parsing with various cultures/styles
            var s = input.Trim();
            // Remove non-breaking spaces and regular spaces used as thousands separators
            s = s.Replace("\u00A0", string.Empty).Replace(" ", string.Empty);

            // If decimal comma is used but we expect integer, cut at decimal separator
            // Replace commas with dots to handle cases like "1,234" or "123,0"
            s = s.Replace(',', '.');

            // If there is a decimal part, drop it (we only want integer quantities)
            var dotIdx = s.IndexOf('.');
            if (dotIdx >=0)
            {
                s = s.Substring(0, dotIdx);
            }

            // Allow leading sign and thousands just in case
            return int.TryParse(s, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out value);
        }
    }
}
