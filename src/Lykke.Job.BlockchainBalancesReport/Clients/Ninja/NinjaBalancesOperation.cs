using System;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Ninja
{
    public class NinjaBalancesOperation
    {
        public long Amount { get; set; }
        public DateTimeOffset FirstSeen { get; set; }
    }
}
