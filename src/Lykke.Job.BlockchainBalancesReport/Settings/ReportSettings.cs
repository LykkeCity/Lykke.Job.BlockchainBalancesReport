using System;
using System.Collections.Generic;

namespace Lykke.Job.BlockchainBalancesReport.Settings
{
    public class ReportSettings
    {
        public ReportRepositoriesSettings Repositories { get; set; }

        public TimeSpan BalancesIntervalFromSchedule { get; set; }

        /// <summary>
        /// Blockchains dictionary. Each blockchain contains dictionary of addresses by their names
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Addresses { get; set; }
    }
}
