//  File:        LoginContext.cs
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
using System.Text;

namespace CSharpSamples.Common.Context
{
    public class LoginContext
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string AuthorizationToken { get; set; }

        public string EncodedCredential
        {
            get
            {
                var credential = UserId + ":" + Password;
                var westernEuropeanEncoding = Encoding.GetEncoding("ISO-8859-1");
                return Convert.ToBase64String(westernEuropeanEncoding.GetBytes(credential));
            }
        }
    }
}
