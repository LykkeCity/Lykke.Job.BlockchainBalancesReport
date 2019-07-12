using Newtonsoft.Json;

namespace Lykke.Job.BlockchainBalancesReport.Clients.EosAuthorityApi
{
    public class EosAuthorityApiAccountGenesisResponse
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("eos_key")]
        public string EosKey { get; set; }

        [JsonProperty("balance_wallet")]
        public string BalanceWallet { get; set; }

        [JsonProperty("balance_unclaimed")]
        public string BalanceUnclaimed { get; set; }

        [JsonProperty("balance_reclaimed")]
        public string BalanceReclaimed { get; set; }

        [JsonProperty("balance_total")]
        public string BalanceTotal { get; set; }

        [JsonProperty("registered")]
        public string Registered { get; set; }

        [JsonProperty("fallback")]
        public string Fallback { get; set; }

        [JsonProperty("register_error")]
        public object RegisterError { get; set; }

        [JsonProperty("fallback_error")]
        public object FallbackError { get; set; }

        [JsonProperty("valid")]
        public string Valid { get; set; }

        [JsonProperty("deterministic_index")]
        public string DeterministicIndex { get; set; }

        [JsonProperty("account_name")]
        public string AccountName { get; set; }

        [JsonProperty("first_seen")]
        public string FirstSeen { get; set; }

        [JsonProperty("cpu_used")]
        public string CpuUsed { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("statusCode")]
        public long StatusCode { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}