using System;
using System.Globalization;
using System.Text;

namespace Leaf.Core.System
{
    public static class TimeSpanExtensions
    {
        public static string ToPrettyString(this TimeSpan self)
        {
            var sb = new StringBuilder();

            if (self.Days > 0)
            {
                sb.Append(self.Days);
                sb.Append(" дней ");
            }
            sb.AppendFormat(CultureInfo.InvariantCulture,
                "{0:00}:{1:00}:{2:00}", self.Hours, self.Minutes, self.Seconds);

            return sb.ToString();
        }
    }
}
