using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiScriptPubKey
    {
        [JsonProperty("addresses")]
        public IReadOnlyCollection<string> Addresses { get; set; }
    }
}