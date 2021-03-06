﻿namespace Lykke.Job.BlockchainBalancesReport.Blockchains.BitcoinCash
{
    public class BitcoinCashExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "BitcoinCash";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://blockchair.com/bitcoin-cash/address/{address}";
        }
    }
}
