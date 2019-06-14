namespace Lykke.Tools.BlockchainBalancesReport.ExplorerUrlFormatters
{
    public interface IExplorerUrlFormatter
    {
        string BlockchainType { get; }
        string Format(string address, string asset);
    }
}
