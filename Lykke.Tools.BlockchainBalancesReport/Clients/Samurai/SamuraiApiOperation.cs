using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Samurai
{
    public class SamuraiApiOperation
    {
        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }

        [JsonProperty("blockTimestamp")]
        public long BlockTimestamp { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("hasError")]
        public bool HasError { get; set; }

        [JsonProperty("messageIndex")]
        public long MessageIndex { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }

        [JsonProperty("transactionIndex")]
        public long TransactionIndex { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }

        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }
    }
}