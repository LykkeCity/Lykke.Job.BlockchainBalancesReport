using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains
{
    public class ExplorerUrlFormattersFactory
    {
        private readonly Dictionary<string, IExplorerUrlFormatter> _formatters;

        public ExplorerUrlFormattersFactory(IEnumerable<IExplorerUrlFormatter> formatters)
        {
            _formatters = formatters.ToDictionary(x => x.BlockchainType);
        }

        public IExplorerUrlFormatter GetFormatter(string blockchainType)
        {
            var formatter = GetFormatterOrDefault(blockchainType);
            if(formatter != null)
            {
                return formatter;
            }

            throw new InvalidOperationException($"Balance provider for blockchain {blockchainType} not found");
        }

        public IExplorerUrlFormatter GetFormatterOrDefault(string blockchainType)
        {
            return _formatters.TryGetValue(blockchainType, out var formatter) ? formatter : null;
        }
    }
}
