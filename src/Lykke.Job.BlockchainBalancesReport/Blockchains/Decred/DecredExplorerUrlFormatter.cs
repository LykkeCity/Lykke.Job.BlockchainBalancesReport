
namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Decred
{
    public class DecredExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Decred";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://explorer.dcrdata.org/address/{address}";
        }
    }
}
