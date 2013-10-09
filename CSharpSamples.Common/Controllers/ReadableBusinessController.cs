using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CSharpSamples.Common.Helpers;
using MYOB.AccountRight.SDK.Contracts;
using MYOB.AccountRight.SDK.Contracts.Version2;
using MYOB.AccountRight.SDK.Services;

namespace CSharpSamples.Common
{
    public class ReadableBusinessController<T, S> :  ControllerBase, IBusinessController
        where T : BaseEntity
        where S : ReadableService<T>
    {

        private S _service;
        protected S Service
        {
            get
            {
                if (_service == null)
                {
                    _service = (S) Activator.CreateInstance(typeof(S), APIConfiguration, null, KeyService);
                }
                return _service;
            }
        }

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
            var pageItem = Service.GetRange(CompanyFile, "", null);

            return Json(pageItem);
        }
        public JsonResult GetPrevPage(string prevPageLink)
        {
            var pageItem = Service.GetRange(CompanyFile, "", null);
            return Json(pageItem);
        }

        protected PagedCollection<T> GetAll(int? pageCount)
        {
            return GetPageItem(null, LogicalOperator.and, null, pageCount);
        }

        protected IEnumerable<T> GetAll(IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null)
        {            
            var result = GetPageItem(searches, logical, null).Items.ToList(); //  due some issue from ui side, set sorting to null first.
            return result;
        }

        protected PagedCollection<T> GetPageItem(IEnumerable<SearchCriteria> searches, LogicalOperator logical = LogicalOperator.and, IEnumerable<SortDescription> sorting = null, int? pageCount = null)
        {
            var query = QueryStringHelper.CombineQuery(searches, logical, sorting, null);
            var result = Service.GetRange(CompanyFile, query, CompanyCredential);
            return result;
        }

        protected T GetById(string id)
        {
            return Service.Get(CompanyFile, new Guid(id), CompanyCredential);
        }

        protected IEnumerable<T> OrderBy(string field, SortDirection direction)
        {
            var sorting = new[] { new SortDescription { Field = field, Direction = direction } };
            var pageItems = GetPageItem(null, LogicalOperator.and, sorting);

            return pageItems.Items;
        }       
    }
}
