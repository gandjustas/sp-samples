using System;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.Office.Server.Search.WebControls;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.InteropServices;

namespace ParametrizedSearchWebParts.AppendSearchQueryWebPart
{
    [ToolboxItemAttribute(false)]
    [Guid("6ba2a020-2ec0-46b4-b876-1cbc7fb34220")]
    public class AppendSearchQueryWebPart : WebPart
    {
        private Hashtable parameters = new Hashtable();
        private PropertyDescriptorCollection schema;
        public AppendSearchQueryWebPart()
        {
            this.ChromeType = PartChromeType.None;
        }

        [WebBrowsable(true)]
        [WebDisplayName("Query id")]
        [Personalizable(PersonalizationScope.Shared)]
        public int QueryId { get; set; }

        [WebBrowsable(true)]
        [WebDisplayName("Query text")]
        [Personalizable(PersonalizationScope.Shared)]
        public string QueryToAppend { get; set; }

        [ConnectionConsumer("Query parameters", AllowsMultipleConnections = true)]
        public void SetQueryParametersProvider(IWebPartParameters provider)
        {
            this.schema = this.schema ?? Utility.GetQueryParametersSchema(this.QueryToAppend ?? "");
            if (provider != null)
            {
                provider.SetConsumerSchema(this.schema);
                provider.GetParametersData(ParametersCallback);
            }
        }

        protected void ParametersCallback(IDictionary data)
        {
            foreach (PropertyDescriptor pd in this.schema)
            {
                pd.SetValue(parameters, data[pd.Name]);
            }

            if (ParametersSatisfied())
            {
                AppendQuery();
            }
        }

        private bool ParametersSatisfied()
        {
            return this.schema
                       .OfType<PropertyDescriptor>()
                       .All(p => this.parameters.ContainsKey(p.Name));
        }

        protected void AppendQuery()
        {
            if (this.QueryToAppend != null)
            {
                var qm = SharedQueryManager.GetInstance(this.Page, (QueryId)this.QueryId).QueryManager;
                if (qm.UserQuery != "")
                {
                    qm.UserQuery = "(" + qm.UserQuery + ") ";
                }
                qm.UserQuery += Utility.SubstituteParameters(this.QueryToAppend, parameters);
            }
        }

    }
}
