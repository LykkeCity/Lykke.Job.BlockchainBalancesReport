using System;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.BlockCypherApi
{
    public class BlockCypherApiTxRef
    {
        [JsonProperty("tx_hash")]
        public string TxHash { get; set; }

        [JsonProperty("block_height")]
        public long BlockHeight { get; set; }

        [JsonProperty("tx_input_n")]
        public long TxInputN { get; set; }

        [JsonProperty("tx_output_n")]
        public long TxOutputN { get; set; }

        [JsonProperty("value")]
        public long Value { get; set; }

        [JsonProperty("ref_balance")]
        public long RefBalance { get; set; }

        [JsonProperty("spent")]
        public bool Spent { get; set; }

        [JsonProperty("confirmations")]
        public long Confirmations { get; set; }

        [JsonProperty("confirmed")]
        public DateTimeOffset Confirmed { get; set; }

        [JsonProperty("double_spend")]
        public bool DoubleSpend { get; set; }

        [JsonProperty("spent_by", NullValueHandling = NullValueHandling.Ignore)]
        public string SpentBy { get; set; }
    }
}
