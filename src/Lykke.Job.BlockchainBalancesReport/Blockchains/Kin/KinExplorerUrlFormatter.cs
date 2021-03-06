﻿namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Kin
{
    public class KinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Kin";

        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://kinexplorer.com/account/{address}";
        }
    }
}
