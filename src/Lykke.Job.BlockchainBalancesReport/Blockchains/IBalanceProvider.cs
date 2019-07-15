using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains
{
    public interface IBalanceProvider
    {
        string BlockchainType { get; }

        Task<IReadOnlyDictionary<BlockchainAsset, decimal>> GetBalancesAsync(string address, DateTime at);
    }
}
