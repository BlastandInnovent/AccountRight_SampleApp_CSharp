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
using System.Web.Mvc;
using System.IO;
using System.Net;
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
        public JsonResult CompanyLogon(CompanyModel companyFile, string userId, string password)
        {
            try
            {
                var context = new CompanyFileContext(OAuthInformation);
                context.Login(companyFile, userId, password);

                Context = context;
            }
            catch (Exception ex)
            {
                return Json(new LogonResult { ok = false, message = ex.Message });
            }

            return Json(new LogonResult { ok = true });
        }

        public JsonResult CompanyLogOut()
        {
            Context = null;
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

            CloudApi = cloudApi;
        }

        public ActionResult OAuthCallback()
        {
            OAuthInformation.RequestAccessToken();

            return RedirectToAction("Index", "CommandCentre");
        }

        public JsonResult RefreshToken()
        {
            return Json(OAuthInformation.RefreshToken());
        }


        
        protected ApiContext ApiContext
        {
            get
            {
                return new ApiContext(OAuthInformation);
            }
        }

        protected CompanyFileContext Context
        {
            get
            {
                return HttpContextFactory.Current.Session["CompanyFileContext"] as CompanyFileContext;
            }
            private set
            {
                HttpContextFactory.Current.Session["CompanyFileContext"] = value;
            }
        }
        public string CloudApi
        {
            get
            {
                var apiUrl = HttpContextFactory.Current.Session["CloudWebApi"] as string;

                if (string.IsNullOrEmpty(apiUrl))
                {
                    apiUrl = ConfigurationManager.AppSettings["cloudApiUrl"];
                    HttpContextFactory.Current.Session["CloudWebApi"] = apiUrl;
                }

                return apiUrl;
            }
            set
            {
                HttpContextFactory.Current.Session["CloudWebApi"] = value;
            }
        }
        public string NetworkApi
        {
            get
            {
                var apiUrl = HttpContextFactory.Current.Session["NetworkWebApi"] as string;

                if (string.IsNullOrEmpty(apiUrl))
                {
                    apiUrl = ConfigurationManager.AppSettings["networkApiUrl"];
                    HttpContextFactory.Current.Session["NetworkWebApi"] = apiUrl;
                }

                return apiUrl;
            }
            set
            {
                HttpContextFactory.Current.Session["NetworkWebApi"] = value;
            }
        }

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
