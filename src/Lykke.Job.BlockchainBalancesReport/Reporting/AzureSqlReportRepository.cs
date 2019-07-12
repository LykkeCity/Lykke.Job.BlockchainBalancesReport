using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Job.BlockchainBalancesReport.Settings;
using Microsoft.Extensions.Logging;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class AzureSqlReportRepository : IReportRepository
    {
        private readonly ILogger<AzureSqlReportRepository> _logger;
        private readonly SqlConnectionStringBuilder _connectionBuilder;
        private readonly bool _createTable;

        public AzureSqlReportRepository(
            ILogger<AzureSqlReportRepository> logger,
            AzureSqlRepositorySettings settings)
        {
            _logger = logger;

            _connectionBuilder = new SqlConnectionStringBuilder
            {
                DataSource = settings.Server,
                UserID = settings.User,
                Password = settings.Password,
                InitialCatalog = settings.Database
            };

            _createTable = settings.CreateTable;
        }

        public async Task SaveAsync(IReadOnlyCollection<ReportItem> items)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                if (_createTable)
                {
                    _logger.LogInformation($"Ensuring that SQL table {_connectionBuilder.DataSource}:{_connectionBuilder.InitialCatalog}.HotWalletBalances is created...");

                    await EnsureTableIsCreatedAsync(connection);
                }

                _logger.LogInformation("Ensuring that there are not balances saved yet...");

                await RemoveItems(connection, items);

                _logger.LogInformation("Saving balances...");

                await InsertItems(connection, items);

                _logger.LogInformation($"Saving done. {items.Count} balances saved");
            }
        }

        private static async Task EnsureTableIsCreatedAsync(SqlConnection connection)
        {
            try
            {
                await ExecuteNonQueryCommandAsync
                (
                    connection,
                    @"
                        create table HotWalletBalances
                        (
                            date datetime not null,                                
                            blockchain varchar(64) not null,
                            addressName varchar(64) not null,
                            address varchar(128) not null,
                            assetName varchar(32) not null,
                            balance decimal(38, 16) not null,
                            blockchainAsset varchar(64) not null,
                            assetId varchar(64),                            
                            explorerUrl varchar(256)

                            constraint HotWalletBalances_pk primary key (date, blockchain, address, blockchainAsset)
                        )"
                );
            }
            catch (SqlException ex) when (ex.Number == 2714) // The table is already exists
            {
            }

            try
            {
                await ExecuteNonQueryCommandAsync
                (
                    connection,
                    "exec sp_addextendedproperty 'MS_Description', 'Blockchain hot and cold wallet daily balances', 'SCHEMA', 'dbo', 'TABLE', 'HotWalletBalances'"
                );
            }
            catch (SqlException ex) when (ex.Number == 15233) // The property is already exists
            {
            }
        }

        private static async Task RemoveItems(SqlConnection connection, IReadOnlyCollection<ReportItem> items)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine
            (
                @"
                delete from HotWalletBalances 
                where"
            );

            var lastItem = items.Last();

            foreach (var item in items)
            {
                sqlBuilder.AppendLine
                (
                    $@"
                    date = {DateTimeValue(item.Date)} and
                    blockchain = {StringValue(item.BlockchainType)} and
                    address = {StringValue(item.Address)} and
                    blockchainAsset = {StringValue(item.Asset.BlockchainId)}
                    {(item == lastItem ? "" : "or")}"
                );
            }

            await ExecuteNonQueryCommandAsync(connection, sqlBuilder.ToString());
        }

        private static async Task InsertItems(SqlConnection connection, IReadOnlyCollection<ReportItem> items)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine
            (
                @"
                insert into HotWalletBalances 
                (
                    date,
                    blockchain,
                    addressName,
                    address,
                    assetName,
                    balance,
                    blockchainAsset,
                    assetId,                    
                    explorerUrl
                )
                values"
            );

            var lastItem = items.Last();

            foreach (var item in items)
            {
                sqlBuilder.AppendLine
                (
                    $@"
                    (
                        {DateTimeValue(item.Date)},
                        {StringValue(item.BlockchainType)},
                        {StringValue(item.AddressName)},
                        {StringValue(item.Address)},
                        {StringValue(item.Asset.Name)},
                        {DecimalValue(Math.Round(item.Balance, 16))},
                        {StringValue(item.Asset.BlockchainId)},
                        {StringValue(item.Asset.LykkeId)},                        
                        {StringValue(item.ExplorerUrl)}
                    ){(item == lastItem ? ";" : ",")}"
                );
            }

            await ExecuteNonQueryCommandAsync(connection, sqlBuilder.ToString());
        }

        private static async Task ExecuteNonQueryCommandAsync(SqlConnection connection, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;

                await command.ExecuteNonQueryAsync();
            }
        }

        private static string DateTimeValue(DateTime value)
        {
            return $"'{value:yyyy-MM-ddTHH:mm:ss}'";
        }

        private static string DecimalValue(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static string StringValue(string value)
        {
            return value == null
                ? "NULL"
                : $"'{value.Replace("'", "''")}'";
        }
    }
}
