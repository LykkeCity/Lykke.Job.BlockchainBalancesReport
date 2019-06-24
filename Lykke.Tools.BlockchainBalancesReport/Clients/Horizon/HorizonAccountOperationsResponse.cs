using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonAccountOperationsResponse
    {
        [JsonProperty("_embedded")]
        public HorizonAccountOperationsEmbedded Embedded { get; set; }
    }
}
