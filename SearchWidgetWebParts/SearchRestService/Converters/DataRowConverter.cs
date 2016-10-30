using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace SearchRestService
{
    internal class DataRowConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get { return new Type[] { typeof(DataRow) }; }
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type,
                                           JavaScriptSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            DataRow dataRow = obj as DataRow;

            return dataRow != null
                   ? dataRow.Table.Columns.OfType<DataColumn>().ToDictionary(c => c.ColumnName, c => dataRow[c])
                   : new Dictionary<string, object>();
        }
    }
}
