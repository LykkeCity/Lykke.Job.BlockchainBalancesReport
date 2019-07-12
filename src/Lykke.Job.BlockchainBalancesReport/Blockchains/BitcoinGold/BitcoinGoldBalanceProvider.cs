﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.BitcoinGold
{
    public class BitcoinGoldBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "BitcoinGold";

        private readonly Network _network;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public BitcoinGoldBalanceProvider(
            ILogFactory logFactory,
            IOptions<BitcoinGoldSettings> settings)
        {
            BGold.Instance.EnsureRegistered();

            _network = BGold.Instance.Mainnet;
            _balanceProvider = new InsightApiBalanceProvider
            (
                logFactory,
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<Asset, decimal>
            {
                {new Asset("BTG", "BTG", "a4954205-48eb-4286-9c82-07792169f4db"), balance}
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
