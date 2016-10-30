using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Client.Services;
using Microsoft.SharePoint.Utilities;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Activation;
using System.Web;
using System.Web.Script.Serialization;

namespace SearchRestService
{
    [BasicHttpBindingServiceMetadataExchangeEndpoint]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Search : ISearch
    {

        //IMPORTANT
        //When a web application is configured to use claims authentication (Windows claims, form-based authentication claims, or SAML claims), 
        //the IIS website is always configured to have anonymous access turned on. Your custom SOAP and WCF endpoints may receive requests 
        //from anonymous users. If you have code in your WCF service that calls the RunWithElevatedPrivileges method to access information 
        //without first checking whether the call is from an authorized user or an anonymous user, you risk returning protected SharePoint 
        //data to any anonymous user for some of your functions that use that approach.

        public System.IO.Stream Query(string q, int top, int skip, string select, string orderBy,
                            bool includeRefiners, string r) 
        {
            using (new SPMonitoredScope("Execute Query Method"))
            {
                var site = SPContext.Current.Site;
                var result = GetSearchResults(site, q, top, skip, select, orderBy, includeRefiners, r);
                return ToJson(result);
            }
        }

        private static ResultTableCollection GetSearchResults(SPSite site, string q, int top, int skip, string select, string orderBy, bool includeRefiners, string r)
        {
            var query = new KeywordQuery(site);
            query.QueryText = q;
            query.StartRow = skip;
            if (top > 0)
            {
                query.RowLimit = top;
            }

            FillSelectProperties(select, query);

            FillSortList(orderBy, query);

            query.ResultTypes = ResultType.RelevantResults;
            if (includeRefiners)
            {
                query.ResultTypes |= ResultType.RefinementResults;
                query.Refiners = r;
            }
            return query.Execute();
        }

        private static void FillSortList(string orderBy, KeywordQuery query)
        {
            if (!string.IsNullOrEmpty(orderBy))
            {
                var orderByParts = orderBy.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                query.SortList.Clear();
                foreach (var part in orderByParts)
                {
                    var pair = part.Split(' ');
                    if (pair.Length > 1 && string.Compare(pair[1], "desc", System.StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        query.SortList.Add(pair[0], SortDirection.Descending);
                    }
                    else
                    {
                        query.SortList.Add(pair[0], SortDirection.Ascending);
                    }
                }
            }
        }

        private static void FillSelectProperties(string select, KeywordQuery query)
        {
            if (!string.IsNullOrEmpty(select))
            {
                var properties = select.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);
                query.SelectProperties.Clear();
                query.SelectProperties.AddRange(properties);
            }
        }

        private static Stream ToJson(ResultTableCollection value)
        {
            JavaScriptSerializer ser = new JavaScriptSerializer();

            List<JavaScriptConverter> converters = new List<JavaScriptConverter>();

            converters.Add(new DataRowConverter());
            converters.Add(new ResultTableCollectionConverter());
            ser.RegisterConverters(converters);

            var resultStream = new MemoryStream();
            var writer = new StreamWriter(resultStream);
            writer.Write(ser.Serialize(value));

            writer.Flush();
            resultStream.Position = 0;

            return resultStream;
        }
    }
}
