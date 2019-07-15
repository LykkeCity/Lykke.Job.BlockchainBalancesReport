using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ReportFileRepositorySettings
    {
        [Optional]
        public bool IsEnabled { get; set; }

        [Optional]
        public string FilePath { get; set; }
    }
}
