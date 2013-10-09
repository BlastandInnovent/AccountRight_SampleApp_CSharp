//  File:        ControllerBase.cs
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
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Net;
using MYOB.AccountRight.SDK;
using MYOB.AccountRight.SDK.Contracts;
using MYOB.AccountRight.SDK.Services;
using Newtonsoft.Json;
using System.Text;
using CSharpSamples.Common.Exceptions;
using CSharpSamples.Common.Models;
using CSharpSamples.Common.Helpers;
using System.Collections.Generic;
using CSharpSamples.Common.Context;
using System.Configuration;


namespace CSharpSamples.Common
{
    public abstract class ControllerBase : Controller
    {
        public class LogonResult
        {
            public bool ok { get; set; }
            public string message { get; set; }
        }


        public IApiConfiguration APIConfiguration 
        { 
            get {
                var apiconfiguration = Session["APIConfiguration"] as IApiConfiguration ??
                                       new ApiConfiguration(OAuthInformation.Key, OAuthInformation.Secret, OAuthInformation.RedirectUri);

                return apiconfiguration;   
            }
            set { Session["APIConfiguration"] = value; }
        }

        public bool IsLogon
        {
            get
            {
                return CompanyFileResource != null;
            }
        }
        public CompanyFile CompanyFile
        {
            get
            {
                if (CompanyFileResource == null) return null;

                return CompanyFileResource.CompanyFile;
            }

        }
        public string CurrentUser
        {
            get
            {
                if (CompanyCredential == null) return string.Empty;
                return CompanyCredential.Username;
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
                     + "<br />Server:" + (APIConfiguration.ApiBaseUrl)
                     + "</small>";
            }
        }
        public string LoginInfoTitle
        {
            get
            {
                if (CompanyFile == null) return string.Empty;

                return "<small>"
                    + "Connected to " + (string.IsNullOrEmpty(APIConfiguration.ClientId) ? "Network" : "Cloud")
                     + "</small>";
            }
        }

        public string CloudApi
        {
            get
            {
                return (string.IsNullOrEmpty(APIConfiguration.ClientId) ? "" : APIConfiguration.ApiBaseUrl);
            }
            internal set
            {

            }
        }
        public string NetworkApi
        {
            get
            {
                return (!string.IsNullOrEmpty(APIConfiguration.ClientId) ? APIConfiguration.ApiBaseUrl : "");
            }
            internal set
            {

            }
        }

        public CompanyFileWithResources CompanyFileResource
        {
            get { return Session["CompanyFileResources"] as CompanyFileWithResources; }
            set { Session["CompanyFileResources"] = value; }
        }
        protected CompanyFileCredentials CompanyCredential
        {
            get { return Session["CompanyFileCredentials"] as CompanyFileCredentials; }
            set { Session["CompanyFileCredentials"] = value; }
        }

        protected IOAuthKeyService KeyService
        {
            get
            {
                var keyService = Session["KeyService"] as IOAuthKeyService ?? new SimpleOAuthKeyService();

                return keyService;
            }
            set { Session["KeyService"] = value; }
        }


        public JsonResult CompanyLogon(CompanyFile companyFile, string userId, string password)
        {
            try
            {                                
                Login(companyFile, userId, password);
            }
            catch (Exception ex)
            {
                return Json(new LogonResult { ok = false, message = ex.Message });
            }

            return Json(new LogonResult { ok = true });
        }

        internal CompanyFileWithResources Login(CompanyFile companyFile, string userId, string password)
        {
            var _login = new LoginContext { Username = userId, Password = password };

            var _service = new CompanyFileService(APIConfiguration, null, KeyService);
            CompanyFileResource = _service.Get(companyFile, _login);
            CompanyCredential = new CompanyFileCredentials(userId, password); 
            return CompanyFileResource;
        }

        public JsonResult CompanyLogOut()
        {
            CompanyFileResource = null;
            CompanyCredential = null; 
            return Json(new LogonResult { ok = true });
        }

        public void CallOAuthAuthentication()
        {
            OAuthInformation.GetAuthorizationCode();
        }

        public void SetOAuthInfo(string authorizationUri, string tokenUri, string key, string secret, string redirectUri, string scope, string cloudApi)
        {
            OAuthInformation = new OAuthInfo
            {
                AuthorizationUrl = authorizationUri,
                TokenUrl = tokenUri,
                Key = key,
                Secret = secret,
                RedirectUri = redirectUri,
                Scope = scope
            };            
        }

        public ActionResult OAuthCallback()
        {
            var requestUri = HttpContextFactory.Current.Request.Url;
            var queries = HttpUtility.ParseQueryString(requestUri.Query);
            var code = queries["code"];

            var service = new OAuthService(APIConfiguration);
            var oauthToken = service.GetTokens(code);
            KeyService = new SimpleOAuthKeyService() {OAuthResponse = oauthToken}; 
            

            if (OAuthInformation.Token == null) 
                OAuthInformation.Token = new OAuthToken();
            OAuthInformation.Token.AccessToken = KeyService.OAuthResponse.AccessToken;
            OAuthInformation.Token.RefreshToken = KeyService.OAuthResponse.RefreshToken;
            OAuthInformation.Token.ExpiresIn = KeyService.OAuthResponse.ExpiresIn; 
            //OAuthInformation.RequestAccessToken();
            
            return RedirectToAction("Index", "CommandCentre");
        }

        public JsonResult RefreshToken()
        {
            return Json(OAuthInformation.RefreshToken());
        }
        
        //protected ApiContext ApiContext
        //{
        //    get
        //    {
        //        return new ApiContext(OAuthInformation);
        //    }
        //}
        
        public OAuthInfo OAuthInformation
        {
            get
            {
                var info = HttpContextFactory.Current.Session["OAuthInfo"] as OAuthInfo;

                if (info == null)
                {
                    info = new OAuthInfo
                    {
                        AuthorizationUrl = ConfigurationManager.AppSettings["authorizationUrl"],
                        Key = ConfigurationManager.AppSettings["key"],
                        TokenUrl = ConfigurationManager.AppSettings["tokenUrl"],
                        Secret = ConfigurationManager.AppSettings["secret"],
                        RedirectUri = ConfigurationManager.AppSettings["redirectUrl"],
                        Scope = ConfigurationManager.AppSettings["scope"]
                    };

                    HttpContextFactory.Current.Session["OAuthInfo"] = info;
                }
                return info;
            }
            set
            {
                HttpContextFactory.Current.Session["OAuthInfo"] = value;
            }
        }
    }
}
