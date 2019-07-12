using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonTransactionResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("paging_token")]
        public string PagingToken { get; set; }

        [JsonProperty("successful")]
        public bool Successful { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("ledger")]
        public long Ledger { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("source_account")]
        public string SourceAccount { get; set; }

        [JsonProperty("source_account_sequence")]
        public string SourceAccountSequence { get; set; }

        [JsonProperty("fee_paid")]
        public long FeePaid { get; set; }

        [JsonProperty("fee_charged")]
        public long FeeCharged { get; set; }

        [JsonProperty("max_fee")]
        public long MaxFee { get; set; }

        [JsonProperty("operation_count")]
        public long OperationCount { get; set; }

        [JsonProperty("envelope_xdr")]
        public string EnvelopeXdr { get; set; }

        [JsonProperty("result_xdr")]
        public string ResultXdr { get; set; }

        [JsonProperty("result_meta_xdr")]
        public string ResultMetaXdr { get; set; }

        [JsonProperty("fee_meta_xdr")]
        public string FeeMetaXdr { get; set; }

        [JsonProperty("memo_type")]
        public string MemoType { get; set; }

        [JsonProperty("signatures")]
        public List<string> Signatures { get; set; }
    }
}