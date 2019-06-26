namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Kin
{
    public class KinExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Kin";

        public string Format(string address, string asset)
        {
            return $"https://kinexplorer.com/account/{address}";
        }
    }
}