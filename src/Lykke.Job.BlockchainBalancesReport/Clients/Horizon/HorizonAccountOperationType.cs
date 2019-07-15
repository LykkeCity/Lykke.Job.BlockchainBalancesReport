using System.Runtime.Serialization;

namespace Lykke.Job.BlockchainBalancesReport.Clients.Horizon
{
    public enum HorizonAccountOperationType
    {
        [EnumMember(Value = "account_merge")]
        AccountMerge,
        [EnumMember(Value = "create_account")]
        CreateAccount,
        [EnumMember(Value = "payment")]
        Payment,
        [EnumMember(Value = "path_payment")]
        PathPayment,
        [EnumMember(Value = "change_trust")]
        ChangeTrust

    };
}
