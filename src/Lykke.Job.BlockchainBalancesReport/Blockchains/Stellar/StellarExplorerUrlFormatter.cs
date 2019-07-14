namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Stellar
{
    public class StellarExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Stellar";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"https://stellarchain.io/address/{address}";
        }
    }
}
