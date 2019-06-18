using System;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Ninja
{
    public class NinjaBalancesOperation
    {
        public long Amount { get; set; }
        public DateTimeOffset FirstSeen { get; set; }
    }
}
