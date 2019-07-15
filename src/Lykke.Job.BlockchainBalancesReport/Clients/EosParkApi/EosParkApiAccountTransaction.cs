using System;
using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosParkApi
{
    public class EosParkApiAccountTransaction
    {
        [JsonProperty("data_md5")]
        public string DataMd5 { get; set; }

        [JsonProperty("trx_id")]
        public string TrxId { get; set; }

        [JsonProperty("timestamp")]
        public DateTimeOffset Timestamp { get; set; }

        [JsonProperty("receiver")]
        public string Receiver { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("quantity")]
        public string Quantity { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("block_num")]
        public long BlockNum { get; set; }
    }
}