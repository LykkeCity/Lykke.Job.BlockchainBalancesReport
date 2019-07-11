using System.Collections.Generic;
using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonAccountOperationsEmbedded
    {
        [JsonProperty("records")]
        public IReadOnlyCollection<HorizonAccountOperation> Records { get; set; }
    }
}
