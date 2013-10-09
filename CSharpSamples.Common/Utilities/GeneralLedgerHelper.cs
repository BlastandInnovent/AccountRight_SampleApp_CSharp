using MYOB.AccountRight.SDK.Contracts.Version2.GeneralLedger;

namespace CSharpSamples.Common.Utilities
{
    public static class GeneralLedgerHelper
    {
        public static int GetAccountNumberPrefix(this Classification classification)
        {
            switch (classification)
            {
                case Classification.Asset:
                    return 1;
                case Classification.Liability:
                    return 2;
                case Classification.Equity:
                    return 3;
                case Classification.Income:
                    return 4;
                case Classification.CostOfSales:
                    return 5;
                case Classification.Expense:
                    return 6;
                case Classification.OtherIncome:
                    return 8;
                case Classification.OtherExpense:
                    return 9;
            }
            return 1;
        }
    }
}