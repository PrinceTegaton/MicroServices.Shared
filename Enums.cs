using System.ComponentModel;

namespace MicroServices
{
    public enum Currency
    {
        [Description("NGN")]
        Naira = 1,

        [Description("USD")]
        Dollar = 2,

        [Description("GBP")]
        Pound = 3,

        [Description("EUR")]
        Euro = 4,

        [Description("YEN")]
        Yen = 5
    }

    public enum SqlQueryType
    {
        Text = 1,
        StoredProcedure = 2
    }
}