// -----------------------------------------------------------------------
// <copyright file="TableProviderUtils.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ConnectedWebParts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data;
    using System.ComponentModel;
    using Microsoft.SharePoint.WebControls;
    using System.Collections;
    using System.Web.UI.WebControls.WebParts;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Class provides extension methods to simplify IWebPartTable connections
    /// </summary>
    public static class TableProviderUtils
    {
        /// <summary>
        /// Convert collection of objects to DataTable using PropertyDescriptors.
        /// </summary>
        /// <param name="collection">Collection of objects.</param>
        /// <param name="schema">Schema of objects in collection.</param>
        /// <returns>New DataTable with object data.</returns>
        public static DataTable ToDataTable(this ICollection collection, PropertyDescriptorCollection schema)
        {
            Contract.Requires(schema != null);
            Contract.Requires(collection != null);

            var result = new DataTable();
            result.GenerateDataColumns(schema);
            result.LoadDataTable(collection, schema);
            return result;
        }

        /// <summary>
        /// Loads DataTable from collection of objects.
        /// </summary>
        /// <param name="result">DataTable</param>
        /// <param name="collection">Collection of objects.</param>
        /// <param name="schema">Schema of objects in collection.</param>
        public static void LoadDataTable(this DataTable result, ICollection collection, PropertyDescriptorCollection schema)
        {
            Contract.Requires(result != null);
            Contract.Requires(schema != null);
            Contract.Requires(collection != null);

            foreach (var item in collection)
            {
                var row = result.NewRow();
                foreach (PropertyDescriptor p in schema)
                {
                    row[p.Name] = p.GetValue(item);
                }
                result.Rows.Add(row);
            }
        }

        /// <summary>
        /// Generates DataTable columns from PropertyDescriptorCollection.
        /// </summary>
        /// <param name="result">DataTable</param>
        /// <param name="properties">PropertyDescriptorCollection</param>
        public static void GenerateDataColumns(this DataTable result, PropertyDescriptorCollection properties)
        {
            Contract.Requires(result != null);
            Contract.Requires(properties != null);

            var columns = properties.OfType<PropertyDescriptor>()
                                    .Select(p => new DataColumn(p.Name, p.PropertyType) { Caption = p.DisplayName });
            result.Columns.AddRange(columns.ToArray());
        }

        /// <summary>
        /// Simple way to get data and schema from provider.
        /// </summary>
        /// <param name="provider">IWebPartTable provider</param>
        /// <param name="callback">Callback</param>
        public static void GetTableData(this IWebPartTable provider, Action<ICollection, PropertyDescriptorCollection> callback)        
        {
            if (provider != null)
            {
                provider.GetTableData(d =>
                    {
                        if (d != null && d.Count > 0)
                        {
                            callback(d, provider.Schema);
                        }
                    });
            }
        }


        /// <summary>
        /// Simple way to get data and schema from provider.
        /// </summary>
        /// <param name="provider">IWebPartTable provider</param>
        /// <param name="callback">Callback</param>
        public static void GetDataTable(this IWebPartTable provider, Action<DataTable> callback)
        {
            provider.GetTableData((d, s) => callback(d.ToDataTable(s)));
        }

    }
}
