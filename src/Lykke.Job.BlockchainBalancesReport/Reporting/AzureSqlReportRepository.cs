using System;
using System.Data.SqlClient;
using System.Globalization;
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

        public async Task AddBalanceAsync(ReportItem item)
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

                _log.Debug("Ensuring that there is no balances saved yet...");

                await RemoveItem(connection, item);

                _log.Debug("Saving balance...");

                await InsertItem(connection, item);

                _log.Debug("Balance saved");
            }
        }

        public Task FlushAsync()
        {
            return Task.CompletedTask;
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
                            assetName varchar(64) not null,
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

        private static async Task RemoveItem(SqlConnection connection, ReportItem item)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine
            (
                $@"
                delete from HotWalletBalances 
                where
                    date = {DateTimeValue(item.At)} and
                    blockchain = {StringValue(item.BlockchainType)} and
                    address = {StringValue(item.Address)} and
                    blockchainAsset = {StringValue(item.Asset.BlockchainId)}"
            );

            await ExecuteNonQueryCommandAsync(connection, sqlBuilder.ToString());
        }

        private static async Task InsertItem(SqlConnection connection, ReportItem item)
        {
            var sqlBuilder = new StringBuilder();

            sqlBuilder.AppendLine
            (
                $@"
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
                values
                    (
                        {DateTimeValue(item.At)},
                        {StringValue(item.BlockchainType)},
                        {StringValue(item.AddressName)},
                        {StringValue(item.Address)},
                        {StringValue(item.Asset.Name)},
                        {DecimalValue(Math.Round(item.Balance, 16))},
                        {StringValue(item.Asset.BlockchainId)},
                        {StringValue(item.Asset.LykkeId)},                        
                        {StringValue(item.ExplorerUrl)}
                    )"
            );


            await ExecuteNonQueryCommandAsync(connection, sqlBuilder.ToString());
        }

        private static async Task ExecuteNonQueryCommandAsync(SqlConnection connection, string sql)
        {
            try
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;

                    await command.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to execute command: {sql}", ex);
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
