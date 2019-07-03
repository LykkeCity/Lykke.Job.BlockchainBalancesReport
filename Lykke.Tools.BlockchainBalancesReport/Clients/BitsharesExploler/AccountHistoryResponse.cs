using System;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.BitsharesExploler
{
    public class AccountHistoryResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("op")]
        public Op Op { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }
    }

    public class Op
    {
        [JsonProperty("amount_")]
        public Amount Amount { get; set; }

        [JsonProperty("fee")]
        public Amount Fee { get; set; }

        [JsonProperty("to")]
        public string To { get; set; }
    }

    public class Amount
    {
        [JsonProperty("amount")]
        public decimal Value { get; set; }

        [JsonProperty("asset_id")]
        public string AssetId { get; set; }
    }
}
