namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Ethereum
{
    public class EthereumExplorerUrlFormatter : IExplorerUrlFormatter
    {
        public string BlockchainType => "Ethereum";

        public string Format(string address, Asset asset)
        {
            if (asset.BlockchainId != "ETH")
            {
                return $"https://etherscan.io/token/{asset.BlockchainId}?a={address}";
            }

            return $"https://etherscan.io/address/{address}";
        }
    }
}
