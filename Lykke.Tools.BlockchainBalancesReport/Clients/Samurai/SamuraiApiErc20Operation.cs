using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Samurai
{
    public class SamuraiApiErc20Operation
    {
        [JsonProperty("blockHash")]
        public string BlockHash { get; set; }

        [JsonProperty("blockNumber")]
        public long BlockNumber { get; set; }

        [JsonProperty("blockTimestamp")]
        public long BlockTimestamp { get; set; }

        [JsonProperty("contract")]
        public string Contract { get; set; }

        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("logIndex")]
        public long LogIndex { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("transactionHash")]
        public string TransactionHash { get; set; }

        [JsonProperty("transactionIndex")]
        public long TransactionIndex { get; set; }

        [JsonProperty("transferAmount")]
        public string TransferAmount { get; set; }

        [JsonProperty("gasUsed")]
        public string GasUsed { get; set; }

        [JsonProperty("gasPrice")]
        public string GasPrice { get; set; }
    }
}