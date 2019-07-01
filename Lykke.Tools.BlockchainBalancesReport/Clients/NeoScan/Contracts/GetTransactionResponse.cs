using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.NeoScan.Contracts
{
    public class TransactionResponse
    {
        [JsonProperty("txid")]
        public string TxHash { get; set; }

        [JsonProperty("block_height")]
        public int BlockHeight { get; set; }

        [JsonProperty("block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty("time")]
        public int Timestamp { get; set; }
    }
}
