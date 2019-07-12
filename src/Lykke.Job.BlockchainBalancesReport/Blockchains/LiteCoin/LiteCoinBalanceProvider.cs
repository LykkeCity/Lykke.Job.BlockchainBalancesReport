﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains.LiteCoin
{
    public class LiteCoinBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "LiteCoin";

        private readonly Network _network;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public LiteCoinBalanceProvider(
            ILogFactory logFactory,
            IOptions<LiteCoinSettings> settings)
        {
            Litecoin.Instance.EnsureRegistered();

            _network = Litecoin.Instance.Mainnet;
            _balanceProvider = new InsightApiBalanceProvider
            (
                logFactory,
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address,
            DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<Asset, decimal>
            {
                {new Asset("LTC", "LTC", "2971fbd8-8cbc-4797-8823-9fbde8be3b1c"), balance}
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