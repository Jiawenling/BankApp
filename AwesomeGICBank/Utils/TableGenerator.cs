using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Utils
{
    internal static class TableGenerator
    {
        public static void PrintTable(string tableHeader, List<string> headers, List<List<string>> data)
        {
            int numCols = headers.Count;
            int numRows = data.Count;

            int[] columnWidths = new int[numCols];
            for (int i = 0; i < numCols; i++)
            {
                columnWidths[i] = headers[i].Length; // Initial width based on header length
                for (int j = 0; j < numRows; j++)
                {
                    columnWidths[i] = Math.Max(columnWidths[i], data[j][i].Length);
                }
            }

            void PrintRow(List<string> rowData)
            {
                Console.Write("|");
                for (int i = 0; i < numCols; i++)
                {
                    Console.Write($" {rowData[i].PadRight(columnWidths[i])} |");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
            Console.WriteLine(tableHeader);
            PrintRow(headers); 
            for (int i = 0; i < numRows; i++) 
            {
                PrintRow(data[i]);
            }
            Console.WriteLine();
        }

    }
}
