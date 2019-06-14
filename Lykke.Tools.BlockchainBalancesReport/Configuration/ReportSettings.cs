using System;
using System.Collections.Generic;

namespace Lykke.Tools.BlockchainBalancesReport.Configuration
{
    public class ReportSettings
    {
        private DateTime _balancesAt;
        public string ReportFilePath { get; set; }

        public DateTime BalancesAt
        {
            get => _balancesAt;
            set => _balancesAt = new DateTime(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, DateTimeKind.Utc);
        }

        /// <summary>
        /// Blockchains dictionary. Each blockchain contains dictionary of addresses by their names
        /// </summary>
        public IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> Addresses { get; set; }
    }
}
