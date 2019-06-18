using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiTransaction
    {
        // ReSharper disable once StringLiteralTypo
        [JsonProperty("txid")]
        public string Id { get; set; }

        [JsonProperty("vin")]
        public IReadOnlyCollection<InsightApiTransactionInput> Inputs { get; set; }

        // ReSharper disable once StringLiteralTypo
        [JsonProperty("vout")]
        public IReadOnlyCollection<InsightApiTransactionOutput> Outputs { get; set; }

        [JsonProperty("time")]
        public long Time { get; set; }

    }
}
