namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ReportRepositoriesSettings
    {
        public ReportFileRepositorySettings File { get; set; }
        public AzureSqlRepositorySettings Sql { get; set; }
    }
}
