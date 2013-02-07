//  File:        JournalTransactionModel.cs
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
using System.Collections.Generic;
using CSharpSamples.Common.Helpers;

namespace CSharpSamples.JournalTransactions.Models
{
    /// <summary>
    ///     JournalTransaction schema
    /// </summary>
    public class JournalTransactionModel
    {
        public JournalType JournalType { get; set; }
        public string JournalTypeDisplay { get { return JournalType.ToString().AddSpacesToSentence(); } }

        public DateTime? DateOccurred { get; set; }

        public DateTime? DatePosted { get; set; }

        public string Description { get; set; }
        
        /// <summary>
        ///     Lines
        /// </summary>
        ///[Required]
        public virtual IEnumerable<JournalTransactionLine> Lines { get; set; }
        
        public Uri Uri { get; set; }

        public string Id { get; set; }

        public long RowVersion { get; set; }
    
    }
    
    public enum JournalType
    { 
        General = 0,
        Sale = 1,
        Purchase = 2,
        CashPayment = 3,
        CashReceipt = 4,
        Inventory = 5,
        All = 6
    }
}
