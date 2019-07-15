using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Job.BlockchainBalancesReport.Settings;

namespace Lykke.Job.BlockchainBalancesReport.Reporting
{
    public class AzureSqlReportRepository : IReportRepository
    {
        private readonly ILog _log;
        private readonly string _connectionString;
        private readonly bool _createTable;
        
        public AzureSqlReportRepository(
            ILogFactory logFactory,
            ReportAzureSqlRepositorySettings settings)
        {
            _log = logFactory.CreateLog(this);

            _connectionString = settings.ConnString;
            _createTable = settings.CreateTable;
        }

        public async Task SaveAsync(DateTime at, IReadOnlyCollection<ReportItem> items)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                if (_createTable)
                {
                    var connectionStringBuilder = new SqlConnectionStringBuilder(_connectionString);

                    _log.Info($"Ensuring that SQL table {connectionStringBuilder.DataSource}:{connectionStringBuilder.InitialCatalog}.HotWalletBalances is created...");

                    await EnsureTableIsCreatedAsync(connection);
                }

                _log.Info("Ensuring that there are no balances saved yet...");

                await RemoveItems(connection, at, items);

                _log.Info("Saving balances...");

                await InsertItems(connection, at, items);

                _log.Info($"Saving done. {items.Count} balances saved");
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

        private static async Task RemoveItems(SqlConnection connection, DateTime at, IReadOnlyCollection<ReportItem> items)
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
                    date = {DateTimeValue(at)} and
                    blockchain = {StringValue(item.BlockchainType)} and
                    address = {StringValue(item.Address)} and
                    blockchainAsset = {StringValue(item.Asset.BlockchainId)}
                    {(item == lastItem ? "" : "or")}"
                );
            }

            await ExecuteNonQueryCommandAsync(connection, sqlBuilder.ToString());
        }

        private static async Task InsertItems(SqlConnection connection, DateTime at, IReadOnlyCollection<ReportItem> items)
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
                        {DateTimeValue(at)},
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
