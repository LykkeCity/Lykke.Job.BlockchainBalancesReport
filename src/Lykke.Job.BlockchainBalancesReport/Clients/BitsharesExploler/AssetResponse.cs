using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.BitsharesExploler
{
    public class AssetResponse
    {
        [JsonProperty("precision")]
        public int Precision { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
