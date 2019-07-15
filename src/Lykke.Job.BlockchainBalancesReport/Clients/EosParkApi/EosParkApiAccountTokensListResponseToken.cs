using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosParkApi
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