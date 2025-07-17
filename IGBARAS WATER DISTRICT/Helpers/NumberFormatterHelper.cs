using System;
using System.Globalization;

namespace IGBARAS_WATER_DISTRICT.Helpers
{
    internal static class NumberFormatterHelper
    {
        public static string FormatWithCommas(decimal number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static string FormatWithCommas(float number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static string FormatWithCommas(double number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static string FormatWithCommas(int number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        public static string FormatWithCommas(long number)
        {
            return number.ToString("N0", CultureInfo.InvariantCulture);
        }

        // Optional for decimal with 2 places
        public static string FormatWithCommas2(decimal number)
        {
            return number.ToString("N2", CultureInfo.InvariantCulture);
        }
    }
}
