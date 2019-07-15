using Lykke.Sdk.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class AppSettings : BaseAppSettings
    {
        public AzureStorageSettings AzureStorage { get; set; }

        public MongoStorageSettings MongoStorage { get; set; }

        public ScheduleSettings Schedule { get; set; }

        public ReportSettings Report { get; set; }

        public BlockchainsSettings Blockchains { get; set; }

        public AssetsClientSettings Assets { get; set; }
    }
}
