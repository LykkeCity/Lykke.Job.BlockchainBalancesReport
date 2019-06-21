﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.BitcoinGold
{
    public class BitcoinGoldBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "BitcoinGold";

        private readonly Network _network;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public BitcoinGoldBalanceProvider(
            ILoggerFactory loggerFactory,
            IOptions<BitcoinGoldSettings> settings)
        {
            BGold.Instance.EnsureRegistered();

            _network = BGold.Instance.Mainnet;
            _balanceProvider = new InsightApiBalanceProvider
            (
                loggerFactory.CreateLogger<InsightApiBalanceProvider>(),
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {("BTG", "a4954205-48eb-4286-9c82-07792169f4db"), balance}
            };
        }

        private string NormalizeOrDefault(string address)
        {
            try
            {
                var bitcoinAddress = BitcoinAddress.Create(address, _network);

                return bitcoinAddress.ToString();
            }
            catch (FormatException)
            {
                return null;
            }
        }
    }
}
