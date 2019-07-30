using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Blockchains;
using Lykke.Job.BlockchainBalancesReport.Utils;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Samurai
{
    public class SamuraiBalanceProvider : IAsyncInitialization
    {
        public Task AsyncInitialization { get; }

        private readonly BlockchainAsset _nativeAsset;
        private readonly SamuraiClient _client;
        private readonly Dictionary<string, SamuraiErc20TokenResponse> _tokensCache;
        private IReadOnlyDictionary<string, Asset> _ercTokenAssets;

        public SamuraiBalanceProvider(
            string url,
            BlockchainAsset nativeAsset,
            IAssetsServiceWithCache assetsServiceClient)
        {
            _nativeAsset = nativeAsset;
            _client = new SamuraiClient(url);
            _tokensCache = new Dictionary<string, SamuraiErc20TokenResponse>();

            AsyncInitialization = InitializeAsync(assetsServiceClient);
        }

        public async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var atTime = new DateTimeOffset(at, TimeSpan.Zero).ToUnixTimeSeconds();
            var normalizedAddress = address.ToLower();
            var ethBalance = await GetEthBalanceAsync(normalizedAddress, atTime);
            var erc20Balances = await GetErc20BalancesAsync(normalizedAddress, atTime);

            return erc20Balances
                .Concat
                (
                    new[]
                    {
                        new KeyValuePair<BlockchainAsset, decimal>(_nativeAsset, ethBalance)
                    }
                )
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private async Task InitializeAsync(IAssetsServiceWithCache assetsServiceClient)
        {
            var allAssets = await assetsServiceClient.GetAllAssetsAsync(includeNonTradable: true);

            _ercTokenAssets = allAssets
                .Where(x => x.Blockchain == Blockchain.Ethereum && 
                            string.IsNullOrWhiteSpace(x.BlockchainIntegrationLayerId) &&
                            !string.IsNullOrWhiteSpace(x.BlockChainAssetId))
                .ToDictionary(x => x.BlockChainAssetId.ToLower());
        }

        private async Task<decimal> GetEthBalanceAsync(string address, long at)
        {
            var balance = 0M;
            var start = 0;
            const int count = 500;
            
            do
            {
                var response = await _client.GetOperationsHistoryAsync(address, start, count);

                foreach (var operation in response.History)
                {
                    if (operation.BlockTimestamp > at)
                    {
                        continue;
                    }

                    var value = decimal.Parse(operation.Value) * 0.000000000000000001M;

                    if (address.Equals(operation.From, StringComparison.InvariantCultureIgnoreCase))
                    {
                        var gasPrice = long.Parse(operation.GasPrice) * 0.000000000000000001M;
                        var gasUsed = long.Parse(operation.GasUsed);
                        var fee = gasPrice * gasUsed;

                        balance -= fee;

                        if (!operation.HasError)
                        {
                            balance -= value;
                        }
                    }
                    else if(!operation.HasError)
                    {
                        balance += value;
                    }
                }

                if (response.History.Count < count)
                {
                    break;
                }

                start += response.History.Count;
            }
            while (true);
            
            return balance;
        }

        private async Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetErc20BalancesAsync(string address, long at)
        {
            var balances = new Dictionary<(string Address, string Name), decimal>();
            var start = 0;
            const int count = 1000;

            do
            {
                var operations = await _client.GetErc20OperationsHistory(address, start, count);

                foreach (var operation in operations)
                {
                    if (operation.BlockTimestamp > at)
                    {
                        continue;
                    }

                    var token = await GetErc20TokenAsync(operation.Contract);

                    if (token == null)
                    {
                        continue;
                    }

                    var multiplier = (decimal)Math.Pow(10, -token.Decimals);
                    var value = decimal.Parse(operation.TransferAmount) * multiplier;
                    var balanceChange = address.Equals
                        (operation.From, StringComparison.InvariantCultureIgnoreCase)
                        ? -value
                        : value;

                    var contract = (operation.Contract, token.Symbol);

                    if (balances.TryGetValue(contract, out var balance))
                    {
                        balances[contract] = balance + balanceChange;
                    }
                    else
                    {
                        balances.Add(contract, balanceChange);
                    }
                }

                if (operations.Count < count)
                {
                    break;
                }

                start += operations.Count;
            }
            while (true);

            return balances.ToDictionary(x => GetErcAsset(x.Key.Address, x.Key.Name), x => x.Value);
        }

        private async Task<SamuraiErc20TokenResponse> GetErc20TokenAsync(string contractAddress)
        {
            if (_tokensCache.TryGetValue(contractAddress, out var token))
            {
                return token;
            }

            token = await _client.GetErc20Token(contractAddress);

            _tokensCache.Add(contractAddress, token);

            return token;
        }

        private BlockchainAsset GetErcAsset(string contractAddress, string name)
        {
            _ercTokenAssets.TryGetValue(contractAddress.ToLower(), out var lykkeAsset);

            return new BlockchainAsset(name, contractAddress, lykkeAsset?.Id);
        }
    }
}
