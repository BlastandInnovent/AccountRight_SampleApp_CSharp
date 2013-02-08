//  File:        CompanyFileContext.cs
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
using System.Net;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Helpers;
using System;
using System.Text;

namespace CSharpSamples.Common.Context
{
    public class CompanyFileContext : ApiContext
    {
        protected override void SetAuthorizationHeader(HttpWebRequest req, bool isCloud, string accessToken)
        {
            if (_login != null)
            {
                if (isCloud)
                {
                    req.Headers.Add("x-myobapi-cftoken", _login.EncodedCredential);
                    req.Headers.Add("x-myobapi-key", OAuthInfo.DevKey);
                }
                else
                {
                    req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + _login.EncodedCredential);
                    req.Headers.Add("Authorization-token", _login.AuthorizationToken);
                }
            }
        }

        protected override void AfterResponse(HttpWebResponse rep)
        {
            _login.AuthorizationToken = rep.Headers["Authorization-token"];
        }
    
        public CompanyFileContext(OAuthInfo oauthInfo)
        {
            OAuthInfo = oauthInfo;
        }

        private LoginContext _login;
        public string LoginUser { get { return _login == null ? string.Empty : _login.UserId; } }
        public OAuthInfo OAuthInfo { get; private set; }
        public CompanyResourceModel CompanyFileResource { get; private set; }

        internal CompanyResourceModel Login(CompanyModel companyFile, string userId, string password)
        {
            _login = new LoginContext { UserId = userId, Password = password };

            var companyFileResource = Get<CompanyResourceModel>(companyFile.GetUrl(OAuthInfo), companyFile.IsCloud, OAuthInfo.AccessToken);
            CompanyFileResource = companyFileResource;
            CompanyFileResource.CompanyFile.IsCloud = companyFile.IsCloud;

            return companyFileResource;
        }

        internal T Get<T>(string url)
        {
            return base.Get<T>(url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken);
        }

        internal PageItem<T> GetPageItem<T>(string module, IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null, int? pageCount = null)
        {
            var url = CompanyFileResource.GetResourceUri(module, OAuthInfo);
            return base.Get<PageItem<T>>(url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken, searches, logical, sorting, pageCount);
        }
        internal T Get<T>(string module, IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null)
        {
            var url = CompanyFileResource.GetResourceUri(module, OAuthInfo);
            return base.Get<T>(url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken, searches, logical, sorting);
        }

        internal T GetById<T>(string module, string id)
        {
            var url = CompanyFileResource.GetResourceByIdUri(module, id, OAuthInfo);
            return Get<T>(url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken);
        }

        internal void Post(string module, object obj)
        {
            var url = CompanyFileResource.GetResourceUri(module, OAuthInfo);
            Save(obj, "POST", url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken);
        }

        internal void Put(string module, object obj, string id)
        {
            var url = CompanyFileResource.GetResourceByIdUri(module, id, OAuthInfo);
            Save(obj, "PUT", url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken);
        }

        internal void Delete(string module, string id)
        {
            var url = CompanyFileResource.GetResourceByIdUri(module, id, OAuthInfo);
            Delete(url, CompanyFileResource.CompanyFile.IsCloud, OAuthInfo.AccessToken);
        }

    }
}
