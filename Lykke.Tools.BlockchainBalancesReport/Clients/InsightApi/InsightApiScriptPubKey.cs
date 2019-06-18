using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiScriptPubKey
    {
        [JsonProperty("addresses")]
        public IReadOnlyCollection<string> Addresses { get; set; }
    }
}