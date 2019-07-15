using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Lykke.Job.BlockchainBalancesReport.Clients.RippleDataApi
{
    public class RippleDataApiClient
    {
        private readonly string _url;
        private readonly NewtonsoftJsonSerializer _serializer;

        public RippleDataApiClient(string url)
        {
            _url = url;

            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver {NamingStrategy = new SnakeCaseNamingStrategy()}
            };
            _serializer = new NewtonsoftJsonSerializer(serializerSettings);
        }

        public async Task<RippleDataApiBalancesResponse> GetBalances(string account, DateTime at)
        {
            const int limit = 100;

            var response = await _url
                .AppendPathSegments("v2", "accounts", account, "balances")
                .SetQueryParams(new
                {
                    date = at.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    limit = limit
                })
                .ConfigureRequest(c =>
                {
                    c.JsonSerializer = _serializer;
                })
                .GetJsonAsync<RippleDataApiBalancesResponse>();

            if (response.Balances.Count >= limit)
            {
                throw new InvalidOperationException("Balances count limit reached in the response. Looks like you need to increase the limit");
            }

            return response;
        }
    }
}
