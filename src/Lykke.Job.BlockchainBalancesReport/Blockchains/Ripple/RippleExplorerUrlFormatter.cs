namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Ripple
{
    public class RippleExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Ripple";
        public string Format(string address, Asset asset)
        {
            return $"https://xrpscan.com/account/{address}";
        }
    }
}
