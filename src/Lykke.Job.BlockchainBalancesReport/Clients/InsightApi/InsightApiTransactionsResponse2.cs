using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiTransactionsResponse2
    {
        [JsonProperty("to")]
        public int To { get; set; }

        [JsonProperty("items")]
        public IReadOnlyCollection<InsightApiTransaction> Transactions { get; set; }
    }
}
