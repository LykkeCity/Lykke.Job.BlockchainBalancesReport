using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.BitsharesExploler
{
    public class AssetResponse
    {
        [JsonProperty("precision")]
        public int Precision { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
