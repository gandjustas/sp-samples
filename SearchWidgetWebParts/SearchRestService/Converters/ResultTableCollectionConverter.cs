using Microsoft.Office.Server.Search.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SearchRestService
{
    internal class ResultTableCollectionConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(ResultTableCollection) }; }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
                                           JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var resultTableCollection = obj as ResultTableCollection;
            Dictionary<string, object> propValues = new Dictionary<string, object>();

            if (resultTableCollection != null)
            {
                if (resultTableCollection.Exists(ResultType.RelevantResults))
                {
                    var resultTable = resultTableCollection[ResultType.RelevantResults];
                    propValues.Add("TotalResults", resultTable.TotalRows);
                    propValues.Add("Results", resultTable.Table.Rows.OfType<DataRow>());
                }
                if (resultTableCollection.Exists(ResultType.RefinementResults))
                {
                    var refinersTable = resultTableCollection[ResultType.RefinementResults];
                    propValues.Add("TotalRefiners", refinersTable.TotalRows);
                    propValues.Add("Refiners", refinersTable.Table.Rows.OfType<DataRow>());
                }
            }

            return propValues;
        }
    }
}
