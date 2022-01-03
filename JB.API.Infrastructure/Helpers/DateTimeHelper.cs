using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JB.Infrastructure.Helpers
{
    public static class DateTimeHelper
    {
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById("Mountain Standard Time");
            return new DateTimeOffset(DateTime.UtcNow, timeZone.BaseUtcOffset);
        }
    }
}
