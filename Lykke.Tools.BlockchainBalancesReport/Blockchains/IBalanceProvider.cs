﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains
{
    public interface IBalanceProvider
    {
        string BlockchainType { get; }
        Task<IReadOnlyDictionary<Asset, decimal>> GetBalancesAsync(string address, DateTime at);
    }
}
