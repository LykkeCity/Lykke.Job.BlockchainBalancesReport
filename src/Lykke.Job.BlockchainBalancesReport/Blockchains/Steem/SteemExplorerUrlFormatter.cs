namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Steem
{
    public class SteemExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Steem";
        public string Format(string address, Asset asset)
        {
            return $"https://steemblockexplorer.com/@{address}";
        }
    }
}
