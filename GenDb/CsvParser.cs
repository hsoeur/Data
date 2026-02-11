using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
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
    }
}
