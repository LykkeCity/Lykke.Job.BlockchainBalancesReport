using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiAccountTokensListResponse
    {
        [JsonProperty("errno")]
        public long ErrNo { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty("data")]
        public EosParkApiAccountTokensListResponseData Data { get; set; }
    }
}