using System;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonAccountOperation
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("paging_token")]
        public string PagingToken { get; set; }

        [JsonProperty("transaction_successful")]
        public bool TransactionSuccessful { get; set; }

        [JsonProperty("source_account")]
        public string SourceAccount { get; set; }

        [JsonProperty("type")]
        public HorizonAccountOperationType Type { get; set; }

        [JsonProperty("type_i")]
        public long TypeI { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("transaction_hash")]
        public string TransactionHash { get; set; }

        [JsonProperty("starting_balance", NullValueHandling = NullValueHandling.Ignore)]
        public string StartingBalance { get; set; }

        [JsonProperty("funder", NullValueHandling = NullValueHandling.Ignore)]
        public string Funder { get; set; }

        [JsonProperty("account", NullValueHandling = NullValueHandling.Ignore)]
        public string Account { get; set; }

        [JsonProperty("asset_type", NullValueHandling = NullValueHandling.Ignore)]
        public string AssetType { get; set; }

        [JsonProperty("from", NullValueHandling = NullValueHandling.Ignore)]
        public string From { get; set; }

        [JsonProperty("to", NullValueHandling = NullValueHandling.Ignore)]
        public string To { get; set; }

        [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
        public string Amount { get; set; }

        [JsonProperty("into", NullValueHandling = NullValueHandling.Ignore)]
        public string Into { get; set; }
    }
}
