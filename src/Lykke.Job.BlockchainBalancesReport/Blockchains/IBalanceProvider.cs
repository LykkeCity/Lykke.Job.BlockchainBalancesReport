using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Job.BlockchainBalancesReport.Blockchains
{
    public interface IBalanceProvider
    {
        string BlockchainType { get; }
        Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at);
    }
}
