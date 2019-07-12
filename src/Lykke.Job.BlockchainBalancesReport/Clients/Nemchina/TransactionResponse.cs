using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Nemchina
{
    public class TransactionsResponse
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("recipient")]
        public string Recipient { get; set; }

        [JsonProperty("amount")]
        public decimal Amount { get; set; }

        [JsonProperty("fee")]
        public decimal Fee { get; set; }

        [JsonProperty("timeStamp")]
        public long TimeStamp { get; set; }
    }
}
