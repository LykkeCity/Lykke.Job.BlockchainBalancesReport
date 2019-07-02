
namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Decred
{
    public class DecredExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Decred";
        public string Format(string address, Asset asset)
        {
            return $"https://explorer.dcrdata.org/address/{address}";
        }
    }
}
