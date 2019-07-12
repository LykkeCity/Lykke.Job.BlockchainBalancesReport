using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiAccountTransactionsData
    {
        [JsonProperty("trace_count")]
        public long TraceCount { get; set; }

        [JsonProperty("last_irreversible_block_num")]
        public long LastIrreversibleBlockNum { get; set; }

        [JsonProperty("trace_list")]
        public List<EosParkApiAccountTransaction> TraceList { get; set; }
    }
}