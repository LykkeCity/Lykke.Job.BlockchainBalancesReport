using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.NeoScan.Contracts
{
    public class BalanceResponse
    {
        [JsonProperty("balance")]
        public IEnumerable<Balance> Balances { get; set; }

        public class Balance
        {
            [JsonProperty("unspent")]
            public IList<Unspent> Unspents { get; set; }

            [JsonProperty("asset_symbol")]
            public string AssetSymbol { get; set; }

            [JsonProperty("asset_hash")]
            public string AssetHash { get; set; }

            [JsonProperty("asset")]
            public string Asset { get; set; }

            [JsonProperty("amount")]
            public decimal Amount { get; set; }
        }

        public class Unspent
        {
            [JsonProperty("value")]
            public decimal Value { get; set; }

            [JsonProperty("txid")]
            public string TxId { get; set; }

            [JsonProperty("n")]
            public uint N { get; set; }
        }
    }
}
