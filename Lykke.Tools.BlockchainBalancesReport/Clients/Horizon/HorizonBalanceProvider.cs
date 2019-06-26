using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using stellar_dotnet_sdk;
using stellar_dotnet_sdk.xdr;

namespace Lykke.Tools.BlockchainBalancesReport.Clients.Horizon
{
    public class HorizonBalanceProvider
    {
        private readonly decimal _nativeAssetMultiplier;
        private readonly string _nativeAssetCode;
        private readonly HorizonClient _client;

        public HorizonBalanceProvider(
            string horizonUrl, 
            decimal nativeAssetMultiplier,
            string nativeAssetCode)
        {
            _nativeAssetMultiplier = nativeAssetMultiplier;
            _nativeAssetCode = nativeAssetCode;
            _client = new HorizonClient(horizonUrl);
        }

        public async Task<IReadOnlyDictionary<string, decimal>> GetBalancesAsync(string address, DateTime at)
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

                    if (operation.TransactionSuccessful.HasValue &&
                        !operation.TransactionSuccessful.Value)
                    {
                        throw new NotSupportedException
                        (
                            $"Only successful transactions are supported. Implement support of not successful transactions. Address: {address}, cursor: {cursor}, operation id: {operation.Id} type index {operation.TypeI}"
                        );
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

                        case HorizonAccountOperationType.ChangeTrust:
                            operationAmount = ProcessChangeTrust(operation, address);
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
                        var fee = tx.FeePaid * _nativeAssetMultiplier;

                        if (!balances.TryGetValue(_nativeAssetCode, out var nativeBalance))
                        {
                            throw new InvalidOperationException("Not enough balance to pay fee");
                        }

                        balances[_nativeAssetCode] = nativeBalance - fee;
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

            return balances;
        }

        private (string Asset, decimal Value) ProcessChangeTrust(HorizonAccountOperation operation, string address)
        {
            return (GetAsset(operation), 0M);
        }

        private (string, decimal) ProcessPayment(HorizonAccountOperation operation, string address)
        {
            var amount = decimal.Parse(operation.Amount, CultureInfo.InvariantCulture);

            if (address.Equals(operation.To, StringComparison.InvariantCultureIgnoreCase))
            {
                return (GetAsset(operation), amount);
            }

            return (GetAsset(operation), -amount);
        }

        private (string, decimal) ProcessCreateAccount(HorizonAccountOperation operation, string address)
        {
            var amount = decimal.Parse(operation.StartingBalance, CultureInfo.InvariantCulture);

            if (address.Equals(operation.Account, StringComparison.InvariantCultureIgnoreCase))
            {
                return (_nativeAssetCode, amount);
            }

            return (_nativeAssetCode, -amount);
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
            var amount = sourceAccountStateMeta.State.Data.Account.Balance.InnerValue * _nativeAssetMultiplier;

            if (address.Equals(operation.Into, StringComparison.InvariantCultureIgnoreCase))
            {
                return (GetAsset(operation), amount);
            }

            return (GetAsset(operation), -amount);
        }

        private string GetAsset(HorizonAccountOperation operation)
        {
            var asset = operation.AssetType == "native"
                ? _nativeAssetCode
                : operation.AssetCode;

            if (string.IsNullOrWhiteSpace(asset))
            {
                throw new InvalidOperationException($"Unknown asset code. Asset type is: {operation.AssetType}");
            }

            return asset;
        }
    }
}
