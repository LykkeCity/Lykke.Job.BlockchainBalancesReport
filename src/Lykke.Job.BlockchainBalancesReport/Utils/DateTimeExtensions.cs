using System;
using Common.RemoteUi;

namespace Lykke.Job.BlockchainBalancesReport.Utils
{
    public static class DateTimeExtensions
    {
        public static DateTime AsUtc(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, DateTimeKind.Utc);
        }
    }
}
