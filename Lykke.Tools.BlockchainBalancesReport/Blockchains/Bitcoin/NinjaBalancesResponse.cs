using System.Collections.Generic;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Bitcoin
{
    public class NinjaBalancesResponse
    {
        public string Continuation { get; set; }
        public IReadOnlyCollection<NinjaBalancesOperation> Operations { get; set; }
    }
}