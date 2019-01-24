using System.Globalization;
using Leaf.Core.Extensions.String;

namespace Leaf.Core.Extensions
{
    /// <summary>
    /// Расширения для форматирования чисел к виду с разделителем тысяч.
    /// </summary>
    public static class NumberFormatExtensions
    {
        /// <summary>
        /// Вернет число в виде строки в которой разряды тысяч разделяются пробелом.
        /// </summary>
        public static string PrettyString(this ulong self)
        {
            return self.ToString(StringExtensions.ThousandNumberFormatInfo);
        }

        /// <inheritdoc cref="PrettyString(ulong)"/>
        public static string PrettyString(this long self)
        {
            return self.ToString(StringExtensions.ThousandNumberFormatInfo);
        }

        /// <inheritdoc cref="PrettyString(ulong)"/>
        public static string PrettyString(this uint self)
        {
            return self.ToString(StringExtensions.ThousandNumberFormatInfo);
        }

        /// <inheritdoc cref="PrettyString(ulong)"/>
        public static string PrettyString(this int self)
        {
            return self.ToString(StringExtensions.ThousandNumberFormatInfo);
        }

        /// <inheritdoc cref="PrettyString(ulong)"/>
        public static string PrettyString(this ushort self)
        {
            return self.ToString(StringExtensions.ThousandNumberFormatInfo);
        }

        /// <inheritdoc cref="PrettyString(ulong)"/>
        public static string PrettyString(this short self)
        {
            return self.ToString(StringExtensions.ThousandNumberFormatInfo);
        }

        /// <summary>
        /// Вернет число в виде строки "###.## %".
        /// </summary>
        public static string PercentageString(this float self)
        {
            return self.ToString("###.##", CultureInfo.InvariantCulture) + SpaceAndPercent;
        }

        /// <inheritdoc cref="PercentageString(float)"/>
        public static string PercentageString(this double self)
        {
            return self.ToString("###.##", CultureInfo.InvariantCulture) + SpaceAndPercent;
        }

        /// <inheritdoc cref="PercentageString(float)"/>
        public static string PercentageString(this decimal self)
        {
            return self.ToString("###.##", CultureInfo.InvariantCulture) + SpaceAndPercent;
        }

        private const string SpaceAndPercent = " %";
    }
}
