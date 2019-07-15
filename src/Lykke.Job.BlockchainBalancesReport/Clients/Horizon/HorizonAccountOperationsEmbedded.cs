using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonAccountOperationsEmbedded
    {
        [JsonProperty("records")]
        public IReadOnlyCollection<HorizonAccountOperation> Records { get; set; }
    }
}
