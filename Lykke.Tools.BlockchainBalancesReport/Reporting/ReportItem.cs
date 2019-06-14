namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class ReportItem
    {
        public string BlockchainType { get; set; }
        public string AddressName { get; set; }
        public string Address { get; set; }
        public string BlockchainAsset { get; set; }
        public string AssetId { get; set; }
        public decimal Balance { get; set; }
        public string ExplorerUrl { get; set; }
    }
}
