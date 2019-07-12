using System;
using System.Threading.Tasks;
using AzureStorage;
using AzureStorage.Tables;
using Lykke.AzureStorage.Tables;
using Lykke.Common.Log;
using Lykke.SettingsReader;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class LastReportOccurrenceRepository
    {
        private class Entity : AzureTableEntity
        {
            public DateTime LastReportOccurrence { get; set; }
        }

        private readonly INoSQLTableStorage<Entity> _storage;

        public LastReportOccurrenceRepository(
            ILogFactory logFactory,
            IReloadingManager<string> connectionString)
        {
            _storage = AzureTableStorage<Entity>.Create(connectionString, "LastReportOccurrence", logFactory);
        }

        public async Task<DateTime?> GetLastOccurrenceOrDefaultAsync()
        {
            var entity = await _storage.GetDataAsync(string.Empty, string.Empty);

            return entity?.LastReportOccurrence;
        }

        public async Task SaveLastOccurrenceAsync(DateTime lastOccurrence)
        {
            var entity = new Entity
            {
                PartitionKey = string.Empty,
                RowKey = string.Empty,
                LastReportOccurrence = lastOccurrence
            };

            await _storage.InsertOrReplaceAsync(entity, existingEntity => existingEntity.LastReportOccurrence < lastOccurrence);
        }
    }
}
