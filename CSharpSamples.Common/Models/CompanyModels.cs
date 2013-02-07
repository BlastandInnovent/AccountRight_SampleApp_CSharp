//  File:        CompanyModels.cs
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
using System.Linq;
using System.Web;

namespace CSharpSamples.Common.Models
{
    public class CompanyModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        //public string ComplianceRegion { get; set; }
        // public string Name { get; set; }
        public string LibraryPath { get; set; }
        // public string ProductCode { get; set; }
        public string ProductVersion { get; set; }
        // public bool Readonly { get; set; }
        public string Uri { get; set; }
        
        public bool IsCloud { get; set; }
    }
    
}
