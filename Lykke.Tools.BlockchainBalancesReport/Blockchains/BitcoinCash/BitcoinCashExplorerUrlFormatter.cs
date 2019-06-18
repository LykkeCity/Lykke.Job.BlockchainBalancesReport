﻿namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.BitcoinCash
{
    public class BitcoinCashExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "BitcoinCash";
        public string Format(string address, string asset)
        {
            return $"https://blockchair.com/bitcoin-cash/address/{address}";
        }
    }
}