namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Eos
{
    public class EosExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Eos";

        public string Format(string address, string asset)
        {
            return $"https://eosauthority.com/account/{address}";
        }
    }
}