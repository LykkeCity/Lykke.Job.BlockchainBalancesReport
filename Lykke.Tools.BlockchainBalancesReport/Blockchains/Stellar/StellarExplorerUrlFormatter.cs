namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Stellar
{
    public class StellarExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Stellar";
        public string Format(string address, Asset asset)
        {
            return $"https://stellarchain.io/address/{address}";
        }
    }
}
