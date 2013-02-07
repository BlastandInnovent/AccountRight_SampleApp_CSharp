//  File:        GeneralJournalModels.cs
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
using Newtonsoft.Json;

namespace CSharpSamples.GeneralJournal.Models
{
    public class CategoryModel
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
    public enum GSTReportingMethod
    {
        Unknown = -1,
        Sale = 0,
        Purchase
    }

    public class GeneralJournalModel
    {
        [JsonIgnore]
        public bool IsNew
        {
            get
            {
                return string.IsNullOrEmpty(Id);
            }
        }
        public long RowVersion { get; set; }
        public string Id { get; set; }
        public DateTime DateOccurred { get; set; }
        public string Memo { get; set; }
        public GSTReportingMethod GSTReportingMethod { get; set; }
        public bool IsYearEndAdjustment { get; set; }
        public bool IsTaxInclusive { get; set; }
        public string CategoryId { get; set; }
        public string Uri { get; set; }
        public List<JournalLineModel> Lines { get; set; }
    }

    
}
