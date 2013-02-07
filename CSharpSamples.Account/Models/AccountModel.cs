//  File:        AccountModel.cs
//  Copyright:   Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
//  Website:     http://www.myob.com
//  Author:      MYOB
//  E-mail:      info@myob.com
//
//Documentation, code and sample applications provided by MYOB Australia are for 
//information purposes only. MYOB Technology Pty Ltd and its suppliers make no 
//warranties, either express or implied, in this document. 
//
//Information in this document or code, including website references, is subject
//to change without notice. Unless otherwise noted, the example companies, 
//organisations, products, domain names, email addresses, people, places, and 
//events are fictitious. 
//
//The entire risk of the use of this documentation or code remains with the user. 
//Complying with all applicable copyright laws is the responsibility of the user. 
//
//Copyright 2012 MYOB Technology Pty Ltd. All rights reserved.
using System;
using CSharpSamples.Common.Enums;
using Newtonsoft.Json;

namespace CSharpSamples.Account.Models
{
    public class AccountModel
    {
        [JsonIgnore]
        public bool IsNew
        {
            get { return string.IsNullOrEmpty(Id); }
        }


        [JsonIgnore]
        public string AccountNumberFormatted
        {
            get
            {
                return string.Format("{0}-{1}", AccountType.GetHashCode(), AccountNumber.PadRight(4, '0'));
            }
        }

        /// <summary>
        /// Row's timestamp (Read Only)
        /// </summary>
        public long RowVersion { get; set; }

        /// <summary>
        /// Account Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Uri
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Name of the Account
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Number of the Account
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// Description of the Account
        /// </summary>
        public string AccountDescription { get; set; }

        /// <summary>
        /// Parent Id of the Account
        /// </summary>
        public string ParentAccountId { get; set; }

        /// <summary>
        /// URI of the Parent Account
        /// </summary>
        public Uri ParentAccountUri { get; set; }

        /// <summary>
        /// Is the Account Active
        /// </summary>
        public bool IsInactive { get; set; }

        /// <summary>
        /// Tax Code associated with the account
        /// </summary>
        public string TaxCodeId { get; set; }

        /// <summary>
        /// Currency of the Account
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Level of the Account within the Chart of Accounts
        /// </summary>
        public short AccountLevel { get; set; }

        /// <summary>
        /// Type of Account (Asset, Liability, Equity, ...)
        /// </summary>
        public AccountType AccountType { get; set; }

        [JsonIgnore]
        public string AccountTypeDisplay { get { return AccountType.ToString(); } }

        public BankingDetailsModel BankingDetails { get; set; }

        /// <summary>
        /// is the account a header or a child
        /// </summary>
        public bool IsHeader { get; set; }

        /// <summary>
        /// SubType of the Account
        /// </summary>
        public SubAccountType SubType { get; set; }

        [JsonIgnore]
        public string SubTypeDisplay { get { return SubType.ToString(); } }

        /// <summary>
        /// Opening Balance of the Account for the current financial year
        /// </summary>
        public decimal OpeningAccountBalance { get; set; }

        /// <summary>
        /// Current Account Balance
        /// </summary>
        public decimal CurrentAccountBalance { get; set; }

        [JsonIgnore]
        public string OpeningAccountBalanceDisplay
        {
            get
            {
                if (OpeningAccountBalance == 0) return string.Empty;

                return OpeningAccountBalance.ToString("0,0.00");
            }
        }

        [JsonIgnore]
        public string CurrentAccountBalanceDisplay
        {
            get
            {
                if (CurrentAccountBalance == 0) return string.Empty;

                return CurrentAccountBalance.ToString("0,0.00");
            }
        }
    }

}