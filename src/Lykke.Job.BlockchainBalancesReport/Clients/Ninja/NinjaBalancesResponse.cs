using System.Collections.Generic;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Ninja
{
    public class NinjaBalancesResponse
    {
        public string Continuation { get; set; }
        public IReadOnlyCollection<NinjaBalancesOperation> Operations { get; set; }
    }
}
