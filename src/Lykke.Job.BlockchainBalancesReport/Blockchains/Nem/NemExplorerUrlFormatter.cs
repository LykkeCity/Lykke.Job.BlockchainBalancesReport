namespace Lykke.Job.BlockchainBalancesReport.Blockchains.Nem
{
    public class NemExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Nem";
        public string Format(string address, BlockchainAsset asset)
        {
            return $"http://explorer.nemchina.com/#/s_account?account={address}";
        }
    }
}
