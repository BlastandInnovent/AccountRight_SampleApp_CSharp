//  File:        QueryStringHelper.cs
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
using System.Linq;
using System.Text;

namespace CSharpSamples.Common.Helpers
{
    public static class QueryStringHelper
    {
        public static string ConstructOrderByQueryString(this IEnumerable<SortDescription> sorting)
        {
            if (sorting == null) return string.Empty; 
            var sorts = sorting.Where(x => !string.IsNullOrEmpty(x.Field)).ToList();

            if (sorts == null || !sorts.Any()) return string.Empty;
            
            return "$orderby=" + string.Join(",", sorting.Select(s => s.Field + " " + s.Direction.ToString()));
        }

        public static string ConstructFilterQueryString(this IEnumerable<SearchCriteria> searches, LogicalOperator logical)
        {
            if (searches == null || !searches.Any()) return string.Empty;

            var queryString = new StringBuilder();
            for (var i = 0; i < searches.Count(); i++)
            {
                var criteria = searches.ElementAt(i);

                var filterString = ConstructUriFilterString(criteria);

                queryString.Append(i > 0 ? string.Format(" {0} {1}", logical.ToString(), filterString) : filterString);
            }

            var query = string.Format("$filter={0}", queryString);

            return query;
        }

        private static string ConstructUriFilterString(SearchCriteria criteria)
        {
            return string.Format("{0} {1} '{2}'", criteria.Field, GetOperand(criteria.OperandType), UriLiteral(criteria.FieldType, criteria.SearchText));
        }


        private static string GetOperand(OperandType type)
        {
            switch (type)
            {
                case OperandType.Equal:
                    return "eq";
                case OperandType.GreaterThan:
                    return "gt";
                case OperandType.GreaterThanOrEqual:
                    return "ge";
                case OperandType.LessThan:
                    return "lt";
                case OperandType.LessThanOrEqual:
                    return "le";
            }

            throw new Exception("Operand parsing failed");
        }

        private static string UriLiteral(Type type, string text)
        {
            if (type == typeof(bool) || type== typeof(int))
                return text;

            if (type == typeof(DateTime))
                return string.Format("datetime'{0}'", text);

            if (type == typeof(Guid))
                return string.Format("guid'{0}'", text);

            if (type == typeof(decimal))
                return string.Format("{0}m", text.Trim());
            
            return string.Format("'{0}'", text);
        }

        private static string GetPagingQueryString(int pageCount)
        {
            return string.Format("$top={0}&$skip=0", pageCount);
        }

        
        public static string CombineQuery(IEnumerable<SearchCriteria> searches, LogicalOperator logical, IEnumerable<SortDescription> sorting, int? pageCount)
        {
            var queries = new List<string>();
            if (pageCount.HasValue)
                queries.Add(GetPagingQueryString(pageCount.Value));
            if (searches != null && searches.Count() > 0)
                queries.Add(searches.ConstructFilterQueryString(logical));
            if (sorting != null && sorting.Count() > 0)
                queries.Add(sorting.ConstructOrderByQueryString());

            return CombineQuery(queries);
        }
        
        public static string CombineQuery(IEnumerable<string> queries)
        {
            var queriesToConstruct = queries.Where(q => !string.IsNullOrEmpty(q)).ToList();
            var query = ""; 

            for (var i = 0; i < queriesToConstruct.Count(); i++)
            {
                if (string.IsNullOrEmpty(query))
                    query = queriesToConstruct[i];
                else
                    query += "&" + queriesToConstruct[i];
            }

            return query; 
        }



        [Obsolete("Will be removed after refactor for .Net SDK")]
        public static string Combine(this string url, IEnumerable<SearchCriteria> searches, LogicalOperator logical, IEnumerable<SortDescription> sorting, int? pageCount)
        {
            var queries = new List<string>();
            if (pageCount.HasValue)
                queries.Add(GetPagingQueryString(pageCount.Value));
            if (searches != null && searches.Count() > 0)
                queries.Add(searches.ConstructFilterQueryString(logical));
            if (sorting != null && sorting.Count() > 0)
                queries.Add(sorting.ConstructOrderByQueryString());

            return url.Combine(queries);
        }

        [Obsolete("Will be removed after refactor for .Net SDK")]
        public static string Combine(this string url, IEnumerable<string> queries)
        {
            var builder = new UriBuilder(url);

            var queriesToConstruct = queries.Where(q => !string.IsNullOrEmpty(q)).ToList();
            var query = builder.Query.TrimStart('?');

            for (var i = 0; i < queriesToConstruct.Count(); i++)
            {
                if (string.IsNullOrEmpty(query))
                    query = queriesToConstruct[i];
                else
                    query += "&" + queriesToConstruct[i];
            }
            builder.Query = query;
            return builder.Uri.ToString();
        }
        
        //public static string GetUrl(this string url, bool isCloud, OAuthInfo oAuthInfo)
        //{
        //    if (isCloud)
        //    {
        //        //var builder = new UriBuilder(url);

        //        //if (!string.IsNullOrEmpty(builder.Query))
        //        //    builder.Query.Insert(0, string.Format("key={0}&", oAuthInfo.DevKey));
        //        //else
        //        //    builder.Query = string.Format("key={0}&", oAuthInfo.DevKey);

        //        //return builder.Uri.ToString();
        //    }

        //    return url;
        //}
    }
}
