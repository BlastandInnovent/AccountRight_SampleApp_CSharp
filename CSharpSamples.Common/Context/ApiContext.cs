//  File:        ApiContext.cs
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
using System.Net;
using System.Text;
using CSharpSamples.Common.Exceptions;
using CSharpSamples.Common.Helpers;
using Newtonsoft.Json;

namespace CSharpSamples.Common.Context
{
    public class ApiContext
    {
        private HttpWebRequest CreateHttpRequest(string url, string method, bool isCloud, string accessToken)
        {
            var req = (HttpWebRequest)WebRequest.Create(url);
            req.KeepAlive = true;
            req.Method = method;
            req.ContentType = "application/json; charset=utf-8";
            req.Accept = "text/json";
            SetHeader(req, isCloud, accessToken);

            return req;
        }

        protected virtual void SetAuthorizationHeader(HttpWebRequest req, bool isCloud, string accessToken) { }
        protected virtual void AfterResponse(HttpWebResponse rep) { }

        private void SetHeader(HttpWebRequest req, bool isCloud, string accessToken)
        {
            if (isCloud)
            {
                req.Headers.Add(HttpRequestHeader.Authorization, "Basic " + accessToken);
            }

            SetAuthorizationHeader(req, isCloud, accessToken);
        }

        protected void Save(object obj, string method, string url, bool isCloud, string accessToken)
        {
            try
            {
                var req = CreateHttpRequest(url, method, isCloud, accessToken);

                var json = SerializeToJson(obj);
                var buff = Encoding.ASCII.GetBytes(json);
                req.ContentLength = buff.Length;

                var postData = req.GetRequestStream();
                postData.Write(buff, 0, buff.Length);
                postData.Close();

                var postResponse = (HttpWebResponse)req.GetResponse();
                AfterResponse(postResponse);
                using (var respStream = postResponse.GetResponseStream())
                {
                    if (respStream != null)
                        using (var respStreamReader = new StreamReader(respStream))
                        {
                            var responseString = respStreamReader.ReadToEnd();
                        }
                }
            }
            catch (WebException wex)
            {
                HanldeException(wex);
            }
        }

        protected void Delete(string url, bool isCloud, string accessToken)
        {
            try
            {
                var req = CreateHttpRequest(url, "DELETE", isCloud, accessToken);

                var postResponse = (HttpWebResponse)req.GetResponse();
                AfterResponse(postResponse);
                using (var respStream = postResponse.GetResponseStream())
                {
                    if (respStream != null)
                        using (var respStreamReader = new StreamReader(respStream))
                        {
                            var responseString = respStreamReader.ReadToEnd();
                        }
                }
            }
            catch (WebException wex)
            {
                HanldeException(wex);
            }
        }

        internal T Get<T>(string webApiUrl, bool isCloud, string accessToken,
            IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null, int? pageCount = null)
        {
            var apiUrl = webApiUrl.Combine(searches, logical, sorting, pageCount);

            return Get<T>(apiUrl, isCloud, accessToken);
        }

        internal T Get<T>(string url, bool isCloud, string accessToken)
        {
            try
            {
                var req = CreateHttpRequest(url, "GET", isCloud, accessToken);

                var resp = (HttpWebResponse)req.GetResponse();
                AfterResponse(resp);
                using (var respStream = resp.GetResponseStream())
                {
                    using (var respStreamReader = new StreamReader(respStream))
                    {
                        return DeserializeJson<T>(respStreamReader.ReadToEnd());
                    }
                }
            }
            catch (WebException wex)
            {
                HanldeException(wex);
            }

            return default(T);
        }


        private string SerializeToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        protected TS DeserializeJson<TS>(string jsonString)
        {
            //DataContractJsonSerializer from .net 4.0 library is not working fine for date format
            //error occur when try deserialize date "start with '\/Date(' and end with ')\/' as required for JSON"
            //so use Json.Net instead
            return JsonConvert.DeserializeObject<TS>(jsonString);
        }

        private void HanldeException(WebException wex)
        {
            if (wex.Status == WebExceptionStatus.ConnectFailure)
                throw new Exception(string.Format("Connection failed, please make sure the target server {0} is on.", ""), wex);

            using (var resp = (HttpWebResponse)wex.Response)
            {
                if (resp == null)
                    throw wex;

                using (var respStream = resp.GetResponseStream())
                {
                    using (var respStreamReader = new StreamReader(respStream))
                    {
                        var responseString = respStreamReader.ReadToEnd();

                        //  Account Right validation message comes here, customize your own exception
                        if (resp.StatusCode == HttpStatusCode.BadRequest)
                            throw new Exception(string.Format("AccountRight validation message - {0}", responseString));

                        if (resp.StatusCode == HttpStatusCode.NotFound)
                            throw new RecordDoesNotExistsException(wex);

                        if (resp.StatusCode == HttpStatusCode.Unauthorized)
                            throw new UnauthorizedException(responseString);

                        throw new Exception(string.Format("{0} - {1} : {2}", resp.StatusCode.GetHashCode(), resp.StatusDescription, responseString));
                    }
                }
            }
        }
    }
}
