using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiTransactionOutput
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("scriptPubKey")]
        public InsightApiScriptPubKey ScriptPubKey { get; set; }
    }
}
