using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AwesomeGICBank.Utils
{
    internal static class InputParser
    {
        public static DateTime ParseDate(string input, string format=null)
        {
            if (string.IsNullOrWhiteSpace(format)) format = Constants.DATEFORMAT;
            if (!DateTime.TryParseExact(input, format,
                       CultureInfo.InvariantCulture,
                       DateTimeStyles.None, out DateTime date))
                throw new Exception($"Date must be in {format} format");
            return date;
        }

        public static double ParseAmount(string input)
        {
            if (!double.TryParse(input, out double amount) || amount <= 0 || Math.Round(amount, 2) != amount)
                throw new Exception("Amount must be a positive float with two decimal places");
            return amount;
        }

        public static double ParseInterest(string input)
        {
            if (!double.TryParse(input, out double amount) || amount <= 0 || amount >= 100 ||  Math.Round(amount, 2) != amount)
                throw new Exception("Amount must be a positive float between 0.00 to 100.00");
            return amount;
        }

        public static char ParseTypeTransaction(string input)
        {
            input = input.ToLower();
            if( input == "d" || input == "w") return input.ToUpper().ToCharArray().First();
            throw new Exception("Transaction type must be either D or W");
        }

    }
}
