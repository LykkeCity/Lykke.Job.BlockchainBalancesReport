namespace Lykke.Job.BlockchainBalancesReport.Blockchains.BitcoinGold
{
    public class BitcoinGoldExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "BitcoinGold";
        public string Format(string address, Asset asset)
        {
            return $"https://explorer.bitcoingold.org/insight/address/{address}";
        }
    }
}
