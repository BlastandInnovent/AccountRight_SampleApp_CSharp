//  File:        MutableBusinessController.cs
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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CSharpSamples.Common.Controllers;
using CSharpSamples.Common.Exceptions;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Helpers;
using System.Configuration;
using System.Net;
using CSharpSamples.Common.Context;
using System.Web;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Contracts.Version2;
using MYOB.AccountRight.SDK.Contracts;
using MYOB.AccountRight.SDK.Services;

namespace CSharpSamples.Common
{
    public class MutableBusinessController<T, S> : ReadableBusinessController<T,S> 
        where T : BaseEntity
        where S : MutableService<T>
    {
        
        protected void Delete(string id)
        {
            Service.Delete(CompanyFile, new Guid(id), CompanyCredential);            
        }

        protected void Post(T obj)
        {
            Service.Insert(CompanyFile, obj, CompanyCredential);
        }

        protected void Put(T obj, string id)
        {
            Service.Update(CompanyFile, obj, CompanyCredential); 
        }
        
        
    }

}