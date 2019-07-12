using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.InsightApi
{
    public class InsightApiTransactionInput
    {
        // ReSharper disable once StringLiteralTypo
        [JsonProperty("addr")]
        public string Address { get; set; }

        [JsonProperty("value")]
        public decimal Value { get; set; }
    }
}
