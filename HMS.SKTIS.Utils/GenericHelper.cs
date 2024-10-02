using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HMS.SKTIS.Utils
{
    public static class GenericHelper
    {
        public static int ConvertDayToString(string day)
        {
            switch (day.ToLower())
            {
                case "monday":
                    return 1;
                case "tuesday":
                    return 2;
                case "wednesday":
                    return 3;
                case "thursday":
                    return 4;
                case "friday":
                    return 5;
                case "saturday":
                    return 6;
                case "sunday":
                    return 7;
                default:
                    return 0;
            }
        }

        public static Dictionary<int, string> GetListOfMonth()
        {
            var result = new Dictionary<int, string>();
            if (DateTimeFormatInfo.CurrentInfo == null) return result;

            var month = DateTimeFormatInfo.CurrentInfo.MonthNames;
            for (var i = 0; i < month.Length; i++)
            {
                if (!string.IsNullOrEmpty(month[i]))
                    result.Add(i + 1, month[i]);
            }

            return result;
        }


        public static int ConvertMonthToInt(string month)
        {
            switch (month.ToLower())
            {
                case "januari":
                    return 1;
                case "februari":
                    return 2;
                case "maret":
                    return 3;
                case "april":
                    return 4;
                case "mei":
                    return 5;
                case "juni":
                    return 6;
                case "juli":
                    return 7;
                case "agustus":
                    return 8;
                case "september":
                    return 9;
                case "oktober":
                    return 10;
                case "november":
                    return 11;
                case "desember":
                    return 12;
                default:
                    return 0;
            }
        }

        public static string ReplaceHexadecimalSymbols(string txt)
        {
            string r = "[\x00-\x08\x0B\x0C\x0E-\x1F\x26]";
            return Regex.Replace(txt, r, "", RegexOptions.Compiled);
        }

        public static string ConvertDateTimeToString(DateTime? dateTimeValue)
        {
            return dateTimeValue.HasValue ? dateTimeValue.Value.ToString("dd/MM/yyyy") : string.Empty;
        }

        public static bool IsNumeric(object value)
        {
            try
            {
               var result =  Convert.ToDouble(value);
                return true;
            }
            catch (Exception)
            {
                
                return false;
            }
        }

        public static string ConvertDoubleToString2FormatDecimal(double? value)
        {
            if (value.HasValue)
                return value.Value.ToString("f2", CultureInfo.CurrentCulture);

            return "0.00";
        }

        public static float? ConvertStringToFloatNull(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return null;
                return Convert.ToSingle(value, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return null;
            }
            
        }

        public static double ConvertStringToDoublel(string value)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return 0;
                return Convert.ToDouble(value, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return 0;
            }

        }

        public static string ConvertFloatToStringMoney(float? value)
        {
            if (value.HasValue)
            {
                var cultureInfo = CultureInfo.CurrentCulture;   // You can also hardcode the culture, e.g. var cultureInfo = new CultureInfo("fr-FR"), but then you lose culture-specific formatting such as decimal point (. or ,) or the position of the currency symbol (before or after)
                var numberFormatInfo = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
                numberFormatInfo.CurrencySymbol = ""; // Replace with "$" or "£" or whatever you need
                numberFormatInfo.CurrencyNegativePattern = 1;

                return value.Value.ToString("C2", numberFormatInfo);
            }
            return "0.00";
        }
        
        public static string ConvertIntToString2FormatDecimal(int? value) {
            if (value.HasValue)
                return value.Value.ToString("n2", CultureInfo.CurrentCulture);

            return "0.00";
        }

        public static string ConvertFloatToString2FormatDecimal(float? value)
        {
            if (value.HasValue)
                return value.Value.ToString("n2", CultureInfo.CurrentCulture);

            return "0.00";
        }
		

        public static string ConvertDoubleToString2FormatDecimalCommaThousand(double? value)
        {
            if (value.HasValue)
                return value.Value.ToString("n2", CultureInfo.CurrentCulture);

            return "0.00";
        }

        public static string ConvertDoubleToStringCommaThousand(double? value)
        {
            if (value.HasValue)
                return value.Value.ToString("n0", CultureInfo.CurrentCulture);

            return "0";
        }

        public static string ConvertDecimalToString2FormatDecimalCommaThousand(decimal? value)
        {
            if (value.HasValue)
                return value.Value.ToString("n2", CultureInfo.CurrentCulture);

            return "0.00";
        }
    }
}
