using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiAccountTokensListResponseToken
    {
        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("balance")]
        public string Balance { get; set; }
    }
}