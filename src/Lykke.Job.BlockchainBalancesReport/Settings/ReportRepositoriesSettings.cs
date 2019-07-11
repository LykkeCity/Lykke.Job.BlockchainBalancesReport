namespace Lykke.Tools.BlockchainBalancesReport.Configuration
{
    public class ReportRepositoriesSettings
    {
        public ReportFileRepositorySettings File { get; set; }
        public AzureSqlRepositorySettings Sql { get; set; }
    }
}
