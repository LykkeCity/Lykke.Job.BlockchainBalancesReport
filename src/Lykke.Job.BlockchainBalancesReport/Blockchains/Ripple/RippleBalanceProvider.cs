using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.RippleDataApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Options;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Ripple
{
    public class RippleBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Ripple";

        private readonly RippleDataApiClient _client;

        public RippleBalanceProvider(IOptions<RippleSettings> settings)
        {
            _client = new RippleDataApiClient(settings.Value.DataApiUrl);
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var response = await _client.GetBalances(address, at);

            if (response.Result != "success")
            {
                throw new InvalidOperationException($"Result is {response.Result} but 'success' is expected");
            }

            return response.Balances
                .Select(x => new
                {
                    Asset = x.Currency, 
                    Balance = decimal.Parse(x.Value, CultureInfo.InvariantCulture)
                })
                .GroupBy(x => x.Asset)
                .Select(g => new
                {
                    Asset = g.Key, 
                    Balance = g.Sum(x => x.Balance)
                })
                .ToDictionary(x => GetAsset(x.Asset), x => x.Balance);
        }

        private Asset GetAsset(string asset)
        {
            return asset == "XRP" 
                ? new Asset("XRP", "XRP", "463b1b32-b801-4ea9-a321-7e81bb73d947") 
                // TODO: Get asset id from assets service
                : null;
        }
    }
}
