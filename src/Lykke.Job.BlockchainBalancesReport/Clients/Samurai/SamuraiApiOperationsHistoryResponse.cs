using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Samurai
{
    public class SamuraiApiOperationsHistoryResponse
    {
        [JsonProperty("history")]
        public IReadOnlyCollection<SamuraiApiOperation> History { get; set; }
    }
}