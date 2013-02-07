//  File:        OAuthServiceHelper.cs
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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Newtonsoft.Json;

namespace CSharpSamples.Common.Helpers
{
    internal static class OAuthServiceHelper
    {
        public static void GetAuthorizationCode(this OAuthInfo info)
        {
            var authorizationParams = string.Format("?client_id={0}&redirect_uri={1}&scope={2}&response_type=code", info.ClientId, info.RedirectUri, info.Scope);

            var authorizationUri = info.AuthorizationUrl + authorizationParams;

            HttpContextFactory.Current.Response.Redirect(authorizationUri);
        }

        public static void RequestAccessToken(this OAuthInfo info)
        {
            var requestUri = HttpContextFactory.Current.Request.Url;
            var queries = HttpUtility.ParseQueryString(requestUri.Query);
            var code = queries["code"];

            if (string.IsNullOrEmpty(code))
            {
                info.Token = new OAuthToken
                {
                    AccessToken = requestUri.Query.TrimStart('?')
                };
                return;
            }
            var accessTokenBody = string.Format("client_id={0}&client_secret={1}&scope={2}&code={3}&redirect_uri={4}&grant_type=authorization_code",
            info.ClientId, info.ClientSecret, info.Scope, code, info.RedirectUri);

            var reply = DoPost(info.TokenUrl, accessTokenBody);
            var tokenJson = JsonConvert.DeserializeObject<dynamic>(reply);

            info.Token = new OAuthToken
            {
                AccessToken = tokenJson.access_token,
                RefreshToken = tokenJson.refresh_token,
                ExpiresIn = tokenJson.expires_in,
                Scope = tokenJson.scope,
                TokenType = tokenJson.token_type
            };
        }
        public static OAuthToken RefreshToken(this OAuthInfo info)
        {
            var accessTokenBody = string.Format("client_id={0}&client_secret={1}&refresh_token={2}&grant_type=refresh_token",
            info.ClientId, info.ClientSecret, info.RefreshToken);

            var reply = DoPost(info.TokenUrl, accessTokenBody);
            var tokenJson = JsonConvert.DeserializeObject<dynamic>(reply);

            info.Token = new OAuthToken
            {
                AccessToken = tokenJson.access_token,
                RefreshToken = tokenJson.refresh_token,
                ExpiresIn = tokenJson.expires_in,
                Scope = tokenJson.scope,
                TokenType = tokenJson.token_type
            };

            return info.Token;
        }
        private static string DoPost(string url, string body)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            var bytes = Encoding.ASCII.GetBytes(body);
            if (bytes.Length > 0)
            {
                request.ContentLength = bytes.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream == null)
                {
                    throw new InvalidOperationException("WebReponse not received.");
                }
                using (var sr = new StreamReader(responseStream))
                {
                    return sr.ReadToEnd();
                }
            }
        }
    }
}
