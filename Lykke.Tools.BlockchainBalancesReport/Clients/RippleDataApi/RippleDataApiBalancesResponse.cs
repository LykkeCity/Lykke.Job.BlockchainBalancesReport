using System;
using System.Collections.Generic;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.RippleDataApi
{
    public class RippleDataApiBalancesResponse
    {
        public string Result { get; set; }
        public int LedgerIndex { get; set; }
        public DateTime CloseTime { get; set; }
        public int Limit { get; set; }
        public IReadOnlyCollection<RippleDataApiBalance> Balances { get; set; }
    }
}
