using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonAccountOperationsResponse
    {
        [JsonProperty("_embedded")]
        public HorizonAccountOperationsEmbedded Embedded { get; set; }
    }
}
