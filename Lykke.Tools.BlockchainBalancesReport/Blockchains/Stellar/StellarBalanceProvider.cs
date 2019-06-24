using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Tools.BlockchainBalancesReport.Clients.Horizon;
using Lykke.Tools.BlockchainBalancesReport.Configuration;
using Microsoft.Extensions.Options;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.xdr;

namespace Lykke.Tools.BlockchainBalancesReport.Blockchains.Stellar
{
    public class StellarBalanceProvider : IBalanceProvider
    {
        public string BlockchainType => "Stellar";

        private readonly HorizonClient _client;

        public StellarBalanceProvider(IOptions<StellarSettings> settings)
        {
            _client = new HorizonClient(settings.Value.HorizonUrl);
        }

        public async Task<IReadOnlyDictionary<(string BlockchainAsset, string AssetId), decimal>> GetBalancesAsync(string address, DateTime at)
        {
            var cursor = default(string);
            var balances = new Dictionary<string, decimal>();

            do
            {
                var response = await _client.GetAccountOperationsAsync(address, cursor);

                if (!response.Embedded.Records.Any())
                {
                    break;
                }

                foreach (var operation in response.Embedded.Records)
                {
                    cursor = operation.PagingToken;

                    if (operation.CreatedAt > at)
                    {
                        continue;
                    }

                    if (!operation.TransactionSuccessful)
                    {
                        throw new NotSupportedException($"Only successful transactions are supported. Implement support of not successful transactions. Address: {address}, cursor: {cursor}, operation id: {operation.Id} type index {operation.TypeI}");
                    }

                    (string Asset, decimal Value) operationAmount;

                    switch (operation.Type)
                    {
                        case HorizonAccountOperationType.AccountMerge:
                            operationAmount = await ProcessAccountMergeAsync(operation, address);
                            break;

                        case HorizonAccountOperationType.CreateAccount:
                            operationAmount = ProcessCreateAccount(operation, address);
                            break;

                        case HorizonAccountOperationType.Payment:
                        case HorizonAccountOperationType.PathPayment:
                            operationAmount = ProcessPayment(operation, address);
                            break;

                        default:
                            throw new ArgumentOutOfRangeException
                            (
                                nameof(operation.Type),
                                operation.Type,
                                $"Unknown operation type. Address: {address}, cursor: {cursor}, operation id: {operation.Id} type index {operation.TypeI}"
                            );
                    }

                    if (operation.SourceAccount == address)
                    {
                        var tx = await _client.GetTransactionAsync(operation.TransactionHash);
                        var fee = tx.FeePaid * 0.0000001M;

                        operationAmount.Value -= fee;
                    }

                    if (!balances.TryGetValue(operationAmount.Asset, out var balance))
                    {
                        balances.Add(operationAmount.Asset, operationAmount.Value);
                    }
                    else
                    {
                        balances[operationAmount.Asset] = balance + operationAmount.Value;
                    }
                }
            }
            while (true);

            return balances.ToDictionary(x => GetBalancesKey(x.Key), x => x.Value);
        }

        private static (string BlockchainAsset, string AssetId) GetBalancesKey(string assetType)
        {
            return assetType == "native"
                ? ("XLM", "b5a0389c-fe57-425f-ab17-af41638f6b89")
                : (assetType, null);
        }

        private static (string, decimal) ProcessPayment(HorizonAccountOperation operation, string address)
        {
            var amount = decimal.Parse(operation.Amount, CultureInfo.InvariantCulture);

            if (address.Equals(operation.To, StringComparison.InvariantCultureIgnoreCase))
            {
                return (operation.AssetType, amount);
            }

            return (operation.AssetType, -amount);
        }

        private static (string, decimal) ProcessCreateAccount(HorizonAccountOperation operation, string address)
        {
            var amount = decimal.Parse(operation.StartingBalance, CultureInfo.InvariantCulture);

            if (address.Equals(operation.Account, StringComparison.InvariantCultureIgnoreCase))
            {
                return ("native", amount);
            }

            return ("native", -amount);
        }

        private async Task<(string, decimal)> ProcessAccountMergeAsync(HorizonAccountOperation operation, string address)
        {
            var tx = await _client.GetTransactionAsync(operation.TransactionHash);

            var xdr = Convert.FromBase64String(tx.ResultMetaXdr);
            var reader = new XdrDataInputStream(xdr);
            var txMeta = TransactionMeta.Decode(reader);
            var mergeMeta = txMeta.Operations.First
            (
                op => op.Changes.InnerValue.Any
                (
                    c => c.Discriminant.InnerValue == LedgerEntryChangeType.LedgerEntryChangeTypeEnum.LEDGER_ENTRY_REMOVED &&
                         KeyPair.FromXdrPublicKey(c.Removed.Account.AccountID.InnerValue).Address == operation.SourceAccount
                )
            );
            var sourceAccountStateMeta = mergeMeta.Changes.InnerValue
                .First
                (
                    c => c.Discriminant.InnerValue == LedgerEntryChangeType.LedgerEntryChangeTypeEnum.LEDGER_ENTRY_STATE &&
                         KeyPair.FromXdrPublicKey(c.State.Data.Account.AccountID.InnerValue).Address == operation.SourceAccount
                );
            var amount = sourceAccountStateMeta.State.Data.Account.Balance.InnerValue * 0.0000001M;
            
            if (address.Equals(operation.Into, StringComparison.InvariantCultureIgnoreCase))
            {
                return ("native", amount);
            }

            return ("native", -amount);
        }
    }
}
