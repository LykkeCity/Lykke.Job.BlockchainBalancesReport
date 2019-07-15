namespace Lykke.Job.BlockchainBalancesReport.Blockchains.SolarCoin
{
    public class SolarCoinUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "SolarCoin";
        public string Format(string address, Asset asset)
        {
            return $"https://chainz.cryptoid.info/slr/address.dws?{address}.htm";
        }
    }
}
