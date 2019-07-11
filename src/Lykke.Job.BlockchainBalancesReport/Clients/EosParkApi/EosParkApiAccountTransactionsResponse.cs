using Newtonsoft.Json;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiAccountTransactionsResponse
    {
        [JsonProperty("errno")]
        public long ErrNo { get; set; }

        [JsonProperty("errmsg")]
        public string ErrMsg { get; set; }

        [JsonProperty("data")]
        public EosParkApiAccountTransactionsData Data { get; set; }
    }
}