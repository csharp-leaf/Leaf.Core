using System;
// ReSharper disable UnusedMember.Global

namespace Leaf.Core.Extensions.System
{
    public static class DateTimeExtensions
    {
        public static DateTime FirstJanuary1970
        {
            get {
                if (_firstJanuary1970 == default(DateTime))
                    _firstJanuary1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

                return _firstJanuary1970;
            }
        }
        private static DateTime _firstJanuary1970;

        public static ulong MillisecondsFrom1970 => (ulong) (DateTime.UtcNow - FirstJanuary1970).TotalMilliseconds;
    }
}