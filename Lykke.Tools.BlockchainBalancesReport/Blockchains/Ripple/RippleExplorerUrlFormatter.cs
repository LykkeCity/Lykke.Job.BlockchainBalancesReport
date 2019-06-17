namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Ripple
{
    public class RippleExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Ripple";
        public string Format(string address, string asset)
        {
            return $"https://xrpscan.com/account/{address}";
        }
    }
}