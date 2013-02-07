//  File:        BusinessController.cs
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
using CSharpSamples.Common.Exceptions;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Helpers;
using System.Configuration;
using System.Net;
using CSharpSamples.Common.Context;
using System.Web;

namespace CSharpSamples.Common
{
    public class BusinessController<T> : ControllerBase, IBusinessController
        where T : class
    {
        public bool IsLogon
        {
            get
            {
                return Context != null;
            }
        }
        public string LoginInfo
        {
            get
            {
                if (CompanyFile == null) return string.Empty;

                return "<small>"
                     + "Name: " + CompanyFile.Name
                     + "<br />Version: " + CompanyFile.ProductVersion
                     + "<br />Path:" + CompanyFile.LibraryPath
                     + "<br />User:" + CurrentUser
                     + "<br />Server:" + (CompanyFile.IsCloud ? CloudApi : NetworkApi)
                     + "</small>";
            }
        }
        public string LoginInfoTitle
        {
            get
            {
                if (CompanyFile == null) return string.Empty;

                return "<small>"
                     + "Connected to " + (CompanyFile.IsCloud ? "Cloud" : "Network")
                     + "</small>";
            }
        }

        protected virtual string Module { get { return string.Empty; } }
        

        private string GetPreviousPageLink(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                var builder = new UriBuilder(url);
                var queries = HttpUtility.ParseQueryString(builder.Query);
                int skip = 0;
                int top = 0;
                Int32.TryParse(queries.GetValues("$skip").FirstOrDefault(), out skip);
                Int32.TryParse(queries.GetValues("$top").FirstOrDefault(), out top);

                if (top > 0)
                {
                    var prevPageSkip = skip - top;
                    if (prevPageSkip >= 0)
                    {
                        queries.Set("$skip", prevPageSkip.ToString());

                        var queryStrings = queries.AllKeys.Select(k => k + "=" + queries[k]);

                        builder.Query = string.Join("&", queryStrings);
                        return builder.Uri.ToString();
                    }
                }
            }

            return string.Empty;
        }
        public JsonResult GetNextPage(string nextPageLink)
        {
            var pageItem = Context.Get<PageItem<T>>(nextPageLink);
            pageItem.PrevPageLink = GetPreviousPageLink(nextPageLink);

            return Json(pageItem);
        }
        public JsonResult GetPrevPage(string prevPageLink)
        {
            var pageItem = Context.Get<PageItem<T>>(prevPageLink);
            pageItem.PrevPageLink = GetPreviousPageLink(prevPageLink);

            return Json(pageItem);
        }

        protected PageItem<T> GetAll(int? pageCount)
        {
            return GetPageItem(null, LogicalOperator.and, null, pageCount);
        }

        protected IEnumerable<T> GetAll(IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null)
        {
            return Context.Get<PageItem<T>>(Module, searches, logical, sorting).Items;
        }

        protected PageItem<T> GetPageItem(IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null, int? pageCount = null)
        {
            return Context.GetPageItem<T>(Module, searches, logical, sorting, pageCount);
        }
        
        protected T GetById(string id)
        {
            return Context.GetById<T>(Module, id);
        }

        protected IEnumerable<T> OrderBy(string field, SortDirection direction)
        {
            var pageItems = OrderBy<PageItem<T>>(field, direction);

            return pageItems.Items;
        }
        protected TS OrderBy<TS>(string field, SortDirection direction, string webApiUrl = null)
        {
            return Context.Get<TS>(Module, null, LogicalOperator.and, new[] { new SortDescription { Field = field, Direction = direction } });
        }

        protected void Delete(string id)
        {
            Context.Delete(Module, id);
        }

        protected void Post(T obj)
        {
            Context.Post(Module, obj);
        }

        protected void Put(T obj, string id)
        {
            Context.Put(Module, obj, id);
        }
        
        public string CurrentUser
        {
            get
            {
                if(Context==null) return string.Empty;

                return Context.LoginUser;
            }
        }
        public CompanyModel CompanyFile
        {
            get
            {
                if (CompanyFileResource == null) return null;

                return CompanyFileResource.CompanyFile;
            }

        }
        public CompanyResourceModel CompanyFileResource
        {
            get
            {
                if (Context == null) return null;

                return Context.CompanyFileResource;
            }
        }
    }

}