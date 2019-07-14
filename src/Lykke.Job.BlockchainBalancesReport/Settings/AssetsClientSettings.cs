using System;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class AssetsClientSettings
    {
        [HttpCheck("/api/isalive")]
        public string ServiceUrl { get; set; }

        public TimeSpan CacheExpirationPeriod { get; set; }
    }
}
