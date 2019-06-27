using System;
using Lykke.Tools.BlockchainBalancesReport.Blockchains;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class ReportItem
    {
        public DateTime Date { get; set; }
        public string BlockchainType { get; set; }
        public string AddressName { get; set; }
        public string Address { get; set; }
        public Asset Asset { get; set; }
        public decimal Balance { get; set; }
        public string ExplorerUrl { get; set; }
    }
}
