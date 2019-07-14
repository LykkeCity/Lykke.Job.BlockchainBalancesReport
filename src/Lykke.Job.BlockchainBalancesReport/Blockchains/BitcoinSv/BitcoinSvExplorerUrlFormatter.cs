namespace Lykke.Job.BlockchainBalancesReport.Blockchains.BitcoinSv
{
    public class BitcoinSvExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "BitcoinSv";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://blockchair.com/bitcoin-sv/address/{address}";
        }
    }
}
