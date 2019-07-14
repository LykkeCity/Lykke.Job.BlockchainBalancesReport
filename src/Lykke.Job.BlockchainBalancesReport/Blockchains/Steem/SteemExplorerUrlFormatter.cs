namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Steem
{
    public class SteemExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Steem";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://steemblockexplorer.com/@{address}";
        }
    }
}
