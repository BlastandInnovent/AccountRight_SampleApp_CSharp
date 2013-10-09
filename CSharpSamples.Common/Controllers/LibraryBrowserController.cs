//  File:        LibraryBrowserController.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Helpers;
using CSharpSamples.Common.Context;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Communication;
using MYOB.AccountRight.SDK.Services;

namespace CSharpSamples.Common.Controllers
{
    public class LibraryBrowserController : ControllerBase
    {
        #region company search & order
        public JsonResult Search(string search, SortDescription sort, string webApiUrl, bool isCloud)
        {
            if (isCloud)
            {
                APIConfiguration = new ApiConfiguration(OAuthInformation.Key, OAuthInformation.Secret, OAuthInformation.RedirectUri);
            }
            else
            {
                APIConfiguration = new ApiConfiguration(webApiUrl);                 
            }

            var searchCriteria = new List<SearchCriteria>();
            if (!string.IsNullOrEmpty(search))
                searchCriteria = new[]{ 
                    new SearchCriteria
                    {
                        Field = "Name",
                        SearchText = search,
                        FieldType = typeof(string)
                    },
                    new SearchCriteria
                    {
                        Field = "ProductVersion",
                        SearchText = search,
                        FieldType = typeof(string)
                    },
                    new SearchCriteria
                    {
                        Field = "LibraryPath",
                        SearchText = search,
                        FieldType = typeof(string)
                    }}.ToList();

            var query = QueryStringHelper.CombineQuery(searchCriteria, LogicalOperator.or, new[]{sort}, null);
            var service = new CompanyFileService(APIConfiguration, null, KeyService);
            var model = service.GetRange(query).ToList() ; 
            return Json(model);
        }
        #endregion
    }
}
