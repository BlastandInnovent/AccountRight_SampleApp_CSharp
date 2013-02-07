//  File:        JournalTransactionLine.cs
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

namespace CSharpSamples.JournalTransactions.Models
{
    public class JournalTransactionLine
    {
        public String AccountId { get; set; }

        public AccountType AccountType { get; set; }
        public SubAccountType SubAccountType { get; set; }
        public string AccountTypeDisplay { get { return AccountType.ToString(); } }
        public string SubAccountTypeDisplay { get { return SubAccountType.ToString(); } }

        public decimal Amount { get; set; }

        public String JobId { get; set; }

        public bool IsCredit { get; set; }

        public string LineDescription { get; set; }

        public DateTime? ReconciledDate { get; set; }
    }
}