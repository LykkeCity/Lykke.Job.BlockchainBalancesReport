using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonClient
    {
        private readonly string _url;
        private readonly NewtonsoftJsonSerializer _serializer;

        public HorizonClient(string url)
        {
            _url = url;

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() }
            };
            _serializer = new NewtonsoftJsonSerializer(serializerSettings);
        }

        public async Task<HorizonAccountOperationsResponse> GetAccountOperationsAsync(string account, string cursor)
        {
            return await _url
                .AppendPathSegments("accounts", account, "operations")
                .SetQueryParams(new
                {
                    includeFailed = true,
                    cursor = cursor,
                    order = "asc",
                    limit = 100
                })
                .ConfigureRequest(c =>
                {
                    c.JsonSerializer = _serializer;
                })
                .GetJsonAsync<HorizonAccountOperationsResponse>();
        }

        public async Task<HorizonTransactionResponse> GetTransactionAsync(string transactionId)
        {
            return await _url
                .AppendPathSegments("transactions", transactionId)
                .GetJsonAsync<HorizonTransactionResponse>();
        }
    }
}
