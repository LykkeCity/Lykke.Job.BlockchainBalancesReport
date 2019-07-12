
namespace Lykke.Job.BlockchainBalancesReport.Blockchains.LiteCoin
{
    public class LiteCoinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "LiteCoin";
        public string Format(string address, Asset asset)
        {
            return $"https://blockchair.com/litecoin/address/{address}";
        }
    }
}
