using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.InsightApi;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NBitcoin;
using NBitcoin.Altcoins;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.BitcoinCash
{
    public class BitcoinCashBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "BitcoinCash";

        private readonly Network _btcNetwork;
        private readonly Network _bchNetwork;
        private readonly InsightApiBalanceProvider _balanceProvider;

        public BitcoinCashBalanceProvider(
            ILoggerFactory loggerFactory,
            IOptions<BitcoinCashSettings> settings)
        {
            BCash.Instance.EnsureRegistered();

            _btcNetwork = Network.Main;
            _bchNetwork = BCash.Instance.Mainnet;
            _balanceProvider = new InsightApiBalanceProvider
            (
                loggerFactory.CreateLogger<InsightApiBalanceProvider>(),
                new InsightApiClient(settings.Value.InsightApiUrl),
                NormalizeOrDefault
            );
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(
            string address,
            DateTime at)
        {
            var balance = await _balanceProvider.GetBalanceAsync(address, at);

            return new Dictionary<(string BlockchainAsset, string AssetId), decimal>
            {
                {("BCH", "2a34d6a6-5839-40e5-836f-c1178fa09b89"), balance}
            };
        }
        
        private string NormalizeOrDefault(string address)
        {
            // ReSharper disable CommentTypo
            // eg: moc231tgxApbRSwLNrc9ZbSVDktTRo3acK
            var legacyAddress = GetBitcoinAddress(address, _btcNetwork);
            if (legacyAddress != null)
            {
                return legacyAddress.ScriptPubKey.GetDestinationAddress(_bchNetwork).ToString();
            }

            // eg: bitcoincash:qpm2qsznhks23z7629mms6s4cwef74vcwvy22gdx6a
            var canonicalAddress = GetBitcoinAddress(address, _bchNetwork);
            if (canonicalAddress != null)
            {
                return canonicalAddress.ToString();
            }

            // eg: qpm2qsznhks23z7629mms6s4cwef74vcwvy22gdx6a
            // ReSharper restore CommentTypo
            var addressWithoutPrefix = GetBitcoinAddress($"{GetAddressPrefix(_bchNetwork)}:{address}", _bchNetwork);

            return addressWithoutPrefix?.ToString();
        }

        private static BitcoinAddress GetBitcoinAddress(string address, Network network)
        {
            try
            {
                return BitcoinAddress.Create(address, network);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string GetAddressPrefix(Network bchNetwork)
        {
            if (bchNetwork == BCash.Instance.Mainnet)
            {
                // ReSharper disable once StringLiteralTypo
                return "bitcoincash";
            }

            if (bchNetwork == BCash.Instance.Regtest)
            {
                // ReSharper disable once StringLiteralTypo
                return "bchreg";
            }

            if (bchNetwork == BCash.Instance.Testnet)
            {
                // ReSharper disable once StringLiteralTypo
                return "bchtest";
            }

            throw new ArgumentException("Unknown Bitcoin Cash network", nameof(bchNetwork));
        }
    }
}
