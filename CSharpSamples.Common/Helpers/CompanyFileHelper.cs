using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpSamples.Common.Models;

namespace CSharpSamples.Common.Helpers
{
    public static class CompanyFileHelper
    {
        public static string GetUrl(this CompanyModel company, OAuthInfo info)
        {
            return company.Uri.GetUrl(company.IsCloud, info.ClientId);
        }

        public static string GetResourceUri(this CompanyResourceModel companyResource, string resource, OAuthInfo info, int? pageCount = null)
        {
            return GetResourceUri(companyResource, resource).GetUrl(companyResource.CompanyFile.IsCloud, info.ClientId);
        }

        public static string GetResourceByIdUri(this CompanyResourceModel companyResource, string resource, string id, OAuthInfo info)
        {
            var resourceByIdUri = GetResourceUri(companyResource, resource) + "/" + id;

            return resourceByIdUri.GetUrl(companyResource.CompanyFile.IsCloud, info.ClientId);
        }

        private static string GetResourceUri(CompanyResourceModel companyResource, string resource)
        {
            var resourceUri = companyResource.Resources.FirstOrDefault(r =>
            {
                var segments = new Uri(r).Segments;
                return segments[segments.Length - 1].Trim('/') == resource;
            });

            return resourceUri.TrimEnd('/');
        }
        
    }
}
