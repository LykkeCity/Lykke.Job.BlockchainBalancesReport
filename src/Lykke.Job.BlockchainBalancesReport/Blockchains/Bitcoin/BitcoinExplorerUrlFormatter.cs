namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Bitcoin
{
    public class BitcoinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Bitcoin";

        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://blockchair.com/bitcoin/address/{address}";
        }
    }
}
