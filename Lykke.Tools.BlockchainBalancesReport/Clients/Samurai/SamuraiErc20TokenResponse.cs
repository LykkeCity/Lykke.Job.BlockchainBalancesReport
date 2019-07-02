using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Samurai
{
    public class SamuraiErc20TokenResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("decimals")]
        public long Decimals { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("totalSupply")]
        public string TotalSupply { get; set; }
    }
}