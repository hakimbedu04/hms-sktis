using System;
using System.Globalization;
using System.Threading;
using AutoMapper;
using AutoMapper.Internal;
using HMS.SKTIS.BusinessObjects.DTOs.Planning;
using HMS.SKTIS.Core;

namespace HMS.SKTIS.AutoMapperExtensions
{
    /// <summary>
    /// Resolve String as CultureInfo.InvariantCulture to a nullable DateTime
    /// </summary>
    public class StringToDateResolver : ValueResolver<object, DateTime?>
    {
        protected override DateTime? ResolveCore(object value)
        {
            string inputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(inputAsString))
                return null;

            return DateTime.ParseExact(inputAsString, Constants.DefaultDateFormat, CultureInfo.InvariantCulture);
        }
    }

    public class StringToFloatResolver : ValueResolver<object, float?>
    {
        protected override float? ResolveCore(object value)
        {
            string inputAsString = value.ToNullSafeString();
            inputAsString = String.Format("{0:0.00}", inputAsString);
            if (string.IsNullOrWhiteSpace(inputAsString))
                return null;

            return float.Parse(inputAsString, CultureInfo.CurrentCulture.NumberFormat);
        }
    }

    /// <summary>
    /// Resolve nullable DateTime to a String as CultureInfo.InvariantCulture
    /// </summary>
    public class DateToStringResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            return ((DateTime)value).ToString(Constants.DefaultDateFormat);
        }
    }

    public class FloatToStringResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            return ((float) value).ToString("F2", CultureInfo.CurrentCulture);
        }
    }

  

    /// <summary>
    /// Resolve nullable DateTime to a String as CultureInfo.InvariantCulture
    /// </summary>
    public class DateOnlyToStringResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            return ((DateTime)value).ToString(Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
        }
    }

    public class StringToDateOnlyResolver : ValueResolver<object, DateTime?>
    {
        protected override DateTime? ResolveCore(object value)
        {
            string inputAsString = value.ToNullSafeString();

            if (string.IsNullOrWhiteSpace(inputAsString))
                return null;

            return DateTime.ParseExact(inputAsString, Constants.DefaultDateOnlyFormat, CultureInfo.InvariantCulture);
        }
    }

    public class NullableBooleanToBooleanResolver : ValueResolver<object, bool>
    {
        protected override bool ResolveCore(object value)
        {
            if (value == null)
                return false;

            return ((bool)value);
        }
    }

    public class NullDecimalToZero : ValueResolver<decimal?, decimal?>
    {
        protected override decimal? ResolveCore(decimal? source)
        {
            if (source == null)
                return 0;

            return source;
        }
    }

    public class NullableIntToZero : ValueResolver<int?, int>
    {
        protected override int ResolveCore(int? source)
        {
            if (source == null)
                return 0;

            return (int)source;
        }
    }

    public class NullableFloatToZero : ValueResolver<float?, float>
    {
        protected override float ResolveCore(float? source)
        {
            if (source == null)
                return 0;

            return (float)source;
        }
    }

    public class NullableDoubleToZero : ValueResolver<double?, double>
    {
        protected override double ResolveCore(double? source)
        {
            if (source == null)
                return 0;

            return (double)source;
        }
    }

    public class DecimalToFloatResolver : ValueResolver<decimal, float>
    {
        protected override float ResolveCore(decimal source)
        {
            if (source == null)
                return 0;

            return (float)source;
        }
    }

    public class IndividualCapacityByReferenceWorkHours : ValueResolver<PlanningPlantIndividualCapacityByReferenceDTO, decimal?>
    {
        protected override decimal? ResolveCore(PlanningPlantIndividualCapacityByReferenceDTO source)
        {
            if (source.WorkHours == 3)
            {
                return source.HoursCapacity3;
            }
            else if (source.WorkHours == 5)
            {
                return source.HoursCapacity5;
            }
            else if (source.WorkHours == 6)
            {
                return source.HoursCapacity6;
            }
            else if (source.WorkHours == 7)
            {
                return source.HoursCapacity7;
            }
            else if (source.WorkHours == 8)
            {
                return source.HoursCapacity8;
            }
            else if (source.WorkHours == 9)
            {
                return source.HoursCapacity9;
            }
            else if (source.WorkHours == 10)
            {
                return source.HoursCapacity10;
            }
            else
            {
                return 0;
            }
        }
    }

    /// <summary>
    /// Convert SQL Time to String Hour and Minute
    /// </summary>
    public class TimeToStringHourAndMinuteOnlyResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            var hour = ((TimeSpan)value).Hours;
            var minute = ((TimeSpan)value).Minutes;
            var minuteWithZeroLeading = (minute < 10) ? "0" + minute.ToString() : minute.ToString();
            return String.Concat(hour, ":", minuteWithZeroLeading);
        }
    }
    /// <summary>
    /// Convert string to boolean
    /// </summary>
    public class StringToBoolean : ValueResolver<String, Boolean?>
    {
        protected override Boolean? ResolveCore(string value)
        {
            return Convert.ToBoolean(value);
        }
    }

    public class DayOfWeekToDayName : ValueResolver<int, string>
    {
        protected override string ResolveCore(int source)
        {
            var result = "";
            switch (source)
            {
                case 0:
                    result = "Minggu";
                    break;
                case 1:
                    result = "Senin";
                    break;
                case 2:
                    result = "Selasa";
                    break;
                case 3:
                    result = "Rabu";
                    break;
                case 4:
                    result = "Kamis";
                    break;
                case 5:
                    result = "Jumat";
                    break;
                case 6:
                    result = "Sabtu";
                    break;
                case 7:
                    result = "Minggu";
                    break;
            }

            return result;
        }

    }

    public class NullableFloatToString : ValueResolver<float?, string>
    {
        protected override string ResolveCore(float? source)
        {
            if (source == null)
                return null;

            try
            {
                var result = Convert.ToDouble(source).ToString(CultureInfo.CurrentCulture);
                if (result.Length > 4)
                {
                    var convertResult = Convert.ToDouble(result, CultureInfo.CurrentCulture)
                        .ToString("f2", CultureInfo.CurrentCulture);
                    if (convertResult.Substring(convertResult.Length-1, 1) == "0")
                        convertResult = convertResult.Substring(0, convertResult.Length - 1);

                    return convertResult;
                }

                return result;
            }
            catch (Exception)
            {
                return null;
            }
           
        }
    }

    public class StringToNullableFloat : ValueResolver<string, float?>
    {
        protected override float? ResolveCore(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            try
            {
                return Convert.ToSingle(source, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return null;
            }

        }
    }

    public class StringToNullableDecimal : ValueResolver<string, decimal?>
    {
        protected override decimal? ResolveCore(string source)
        {
            if (string.IsNullOrEmpty(source))
                return null;

            try
            {
                return Convert.ToDecimal(source, CultureInfo.CurrentCulture);
            }
            catch (Exception)
            {
                return null;
            }

        }
    }

    public class FloatToStringMoneyResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            var cultureInfo = CultureInfo.CurrentCulture;   // You can also hardcode the culture, e.g. var cultureInfo = new CultureInfo("fr-FR"), but then you lose culture-specific formatting such as decimal point (. or ,) or the position of the currency symbol (before or after)
            var numberFormatInfo = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormatInfo.CurrencySymbol = ""; // Replace with "$" or "£" or whatever you need

            return ((float)value).ToString("C0", numberFormatInfo);
        }
    }

    public class DoubleToStringMoneyResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            var cultureInfo = CultureInfo.CurrentCulture;   // You can also hardcode the culture, e.g. var cultureInfo = new CultureInfo("fr-FR"), but then you lose culture-specific formatting such as decimal point (. or ,) or the position of the currency symbol (before or after)
            var numberFormatInfo = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormatInfo.CurrencySymbol = ""; // Replace with "$" or "£" or whatever you need

            return ((double)value).ToString("C0", numberFormatInfo);
        }
    }

    public class FloatToStringMoney2FormatDecimalResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return null;

            var cultureInfo = CultureInfo.CurrentCulture;   // You can also hardcode the culture, e.g. var cultureInfo = new CultureInfo("fr-FR"), but then you lose culture-specific formatting such as decimal point (. or ,) or the position of the currency symbol (before or after)
            var numberFormatInfo = (NumberFormatInfo)cultureInfo.NumberFormat.Clone();
            numberFormatInfo.CurrencySymbol = ""; // Replace with "$" or "£" or whatever you need
            numberFormatInfo.CurrencyNegativePattern = 1;
            
            return ((float)value).ToString("C2", numberFormatInfo);
        }
    }
    
     public class DoubleToString2DecimalCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0.00";

            return ((double)value).ToString("n2", CultureInfo.CurrentCulture);
        }
    }

    public class DoubleToStringCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0";

            return ((double)value).ToString("n0", CultureInfo.CurrentCulture);
        }
    }

    public class DecimalToString2DecimalCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0.00";
            
            return ((decimal)value).ToString("n2", CultureInfo.CurrentCulture);
        }
    }

    public class DecimalToString2DecimalCommaSeparatorTrailingZerosResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value) {
            if (value == null)
                return "0.00";

            return ((decimal)value).ToString("#,0.##", CultureInfo.CurrentCulture);
        }
    }

    public class DecimalToString3DecimalPlacesTrailingZerosResolver : ValueResolver<object, string>
    {
        // If value dont have decimal then decimal point will not shows up
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0";

            return ((decimal)value).ToString("G29", CultureInfo.CurrentCulture);
        }
    }

    public class IntToStringCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0";

            return ((int)value).ToString("N0", CultureInfo.CurrentCulture);
        }
    }

    public class LongToStringCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value) {
            if (value == null)
                return "0";

            return ((long)value).ToString("N0", CultureInfo.CurrentCulture);
        }
    }

    public class FloatToString2DecimalCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0";

            return ((float)value).ToString("n2", CultureInfo.CurrentCulture);
        }
    }

    public class FloatToString3DecimalCommaSeparatorResolver : ValueResolver<object, string>
    {
        protected override string ResolveCore(object value)
        {
            if (value == null)
                return "0";

            return ((float)value).ToString("n3", CultureInfo.CurrentCulture);
        }
    }
}
