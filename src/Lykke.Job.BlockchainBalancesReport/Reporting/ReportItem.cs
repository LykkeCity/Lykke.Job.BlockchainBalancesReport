using System;
using Lykke.Job.BlockchainBalancesReport.Blockchains;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class ReportItem
    {
        public DateTime At { get; set; }
        public string BlockchainType { get; set; }
        public string AddressName { get; set; }
        public string Address { get; set; }
        public BlockchainAsset Asset { get; set; }
        public decimal Balance { get; set; }
        public string ExplorerUrl { get; set; }
    }
}
