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

namespace CSharpSamples.Common.Controllers
{
    public class LibraryBrowserController : ControllerBase
    {
        #region company search & order
        public JsonResult Search(string search, SortDescription sort, string webApiUrl, bool isCloud)
        {
            if (isCloud)
            {
                CloudApi = webApiUrl;
                webApiUrl = webApiUrl.GetUrl(isCloud, OAuthInformation.ClientId);
            }
            else
                NetworkApi = webApiUrl;


            if (string.IsNullOrEmpty(search) && (sort==null || string.IsNullOrEmpty(sort.Field)))
            {
                var companyList = ApiContext.Get<List<CompanyModel>>(webApiUrl, isCloud, OAuthInformation.AccessToken);
                companyList.ForEach(c => c.IsCloud = isCloud);
                return Json(companyList);
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

            var sorting = sort == null ? Enumerable.Empty<SortDescription>() : new[] { sort };
            var model = ApiContext.Get<List<CompanyModel>>(webApiUrl, isCloud, OAuthInformation.AccessToken, searchCriteria, LogicalOperator.or, sorting);
            model.ForEach(c => c.IsCloud = isCloud);
            return Json(model);
        }

        #endregion
    }
}
