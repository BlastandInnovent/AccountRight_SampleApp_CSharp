//  File:        BankingDetailsModel.cs
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

namespace CSharpSamples.Account.Models
{
    public class BankingDetailsModel
    {
        /// <summary>
        /// For Australia: formatted as BSBNumber-BankAccountNumber e.g.
        /// For New Zealand: formatted as BankCode-BankAccountNumber-Suffix e.g.
        /// </summary>
        public string BankAccountNumber { get; set; }

        public string BankAccountName { get; set; }

        public string CompanyTradingName { get; set; }

        public string BankCode { get; set; }

        public bool CreateBankFile { get; set; }

        public string DirectEntryUserId { get; set; }

        public bool IncludeSelfBalancingTransaction { get; set; }

        /// <summary>
        /// Statement Code (New Zealand only)
        /// </summary>
        public string StatementCode { get; set; }

        /// <summary>
        /// Statement Reference (New Zealand only)
        /// </summary>
        public string StatementReference { get; set; }

        /// <summary>
        /// Statement Particulars (Statement Text - Australia, Particulars - New Zealand)
        /// </summary>
        public string StatementParticulars { get; set; }
    }
}