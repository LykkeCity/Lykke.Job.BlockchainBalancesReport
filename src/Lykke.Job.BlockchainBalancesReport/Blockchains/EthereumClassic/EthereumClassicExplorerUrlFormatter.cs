namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Ethereum
{
    public class EthereumClassicExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "EthereumClassic";

        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://gastracker.io/addr/{address}";
        }
    }
}
