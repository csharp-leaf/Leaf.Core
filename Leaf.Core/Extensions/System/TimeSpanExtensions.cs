using System;
using System.Globalization;
using System.Text;

namespace Leaf.Core.Extensions.System
{
    /// <summary>
    /// Расширения при работе со временем.
    /// </summary>
    public static class TimeSpanExtensions
    {
        /// <summary>
        /// Выводит время в формате N дней {00 часов}:{00 минут}:{00 секунд}.
        /// </summary>
        /// <param name="self">Время</param>
        /// <param name="english">Использовать английский язык</param>
        /// <returns>Вернет отформатированное время с указанием дней, часов, минут и секунд.</returns>
        public static string ToPrettyString(this TimeSpan self, bool english = false)
        {
            var sb = new StringBuilder();

            if (self.Days > 0)
            {
                sb.Append(self.Days);
                sb.Append(!english ? " дней " : " days ");
            }
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0:00}:{1:00}:{2:00}", self.Hours, self.Minutes, self.Seconds);

            return sb.ToString();
        }
    }
}
