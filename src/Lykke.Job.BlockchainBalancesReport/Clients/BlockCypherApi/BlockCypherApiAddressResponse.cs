using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.BlockCypherApi
{
    public class BlockCypherApiAddressResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("total_received")]
        public long TotalReceived { get; set; }

        [JsonProperty("total_sent")]
        public long TotalSent { get; set; }

        [JsonProperty("balance")]
        public long Balance { get; set; }

        [JsonProperty("unconfirmed_balance")]
        public long UnconfirmedBalance { get; set; }

        [JsonProperty("final_balance")]
        public long FinalBalance { get; set; }

        [JsonProperty("n_tx")]
        public long NTx { get; set; }

        [JsonProperty("unconfirmed_n_tx")]
        public long UnconfirmedNTx { get; set; }

        [JsonProperty("final_n_tx")]
        public long FinalNTx { get; set; }

        [JsonProperty("txrefs")]
        public List<BlockCypherApiTxRef> TxRefs { get; set; }

        [JsonProperty("hasMore")]
        public bool HasMore { get; set; }

        [JsonProperty("tx_url")]
        public Uri TxUrl { get; set; }
    }
}
