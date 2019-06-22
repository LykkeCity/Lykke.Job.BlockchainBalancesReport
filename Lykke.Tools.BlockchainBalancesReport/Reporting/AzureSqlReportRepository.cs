using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Logging;

namespace Lykke.Tools.BlockchainBalancesReport.Reporting
{
    public class AzureSqlReportRepository : IReportRepository
    {
        private readonly ILogger<AzureSqlReportRepository> _logger;
        private readonly SqlConnectionStringBuilder _connectionBuilder;

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
        }

        public async Task SaveAsync(IReadOnlyCollection<ReportItem> items)
        {
            using (var connection = new SqlConnection(_connectionBuilder.ConnectionString))
            {
                await connection.OpenAsync();

                _logger.LogInformation($"Ensuring that SQL table {_connectionBuilder.DataSource}:{_connectionBuilder.InitialCatalog}.HotWalletBalances is created...");

                await EnsureTableIsCreatedAsync(connection);

                _logger.LogInformation("Ensuring that there are not balances saved yet...");

                await RemoveItems(connection, items);

                _logger.LogInformation("Inserting balances...");

                await InsertItems(connection, items);

                _logger.LogInformation($"Inserting done. {items.Count} balances saved");
            }
        }

        private async Task EnsureTableIsCreatedAsync(SqlConnection connection)
        {
            try
            {
                await ExecuteNonQueryCommandAsync
                (
                    connection,
                    @"
                        create table HotWalletBalances
                        (
                            date datetime not null
                                constraint HotWalletBalances_pk primary key,
                            blockchain varchar(64) not null
                                constraint HotWalletBalances_pk primary key,
                            addressName varchar(64) not null,
                            address varchar(128) not null,
                                constraint HotWalletBalances_pk primary key,
                            blockchainAsset varchar(16) not null,
                                constraint HotWalletBalances_pk primary key
                            assetId varchar(64),
                            balance decimal(38, 16) not null,
                            explorerUrl varchar(256)
                        )"
                );
            }
            catch (SqlException ex)
            {
            }

            await ExecuteNonQueryCommandAsync
            (
                connection,
                "exec sp_addextendedproperty 'MS_Description', 'Blockchain hot and cold wallet daily balances', 'SCHEMA', 'dbo', 'TABLE', 'HotWalletBalances'"
            );
        }

        private async Task RemoveItems(SqlConnection connection, IReadOnlyCollection<ReportItem> items)
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
                    blockchainType = {StringValue(item.BlockchainType)} and
                    address = {StringValue(item.Address)} and
                    blockchainAsset = {StringValue(item.BlockchainAsset)}
                    {(item == lastItem ? "" : "or")}"
                );
            }

            await ExecuteNonQueryCommandAsync(connection, sqlBuilder.ToString());
        }

        private async Task InsertItems(SqlConnection connection, IReadOnlyCollection<ReportItem> items)
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
                    blockchainAsset,
                    assetId,
                    balance,
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
                        {StringValue(item.BlockchainAsset)},
                        {StringValue(item.AssetId)},
                        {DecimalValue(item.Balance)},
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
            return $"CAST('{value:yyyy-MM-ddTHH-mm-ss}' AS DATETIME)";
        }

        private static string DecimalValue(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        private static string StringValue(string value)
        {
            return value == null
                ? "NULL"
                : $"'{value.Replace("'", "''")}";
        }
    }
}
