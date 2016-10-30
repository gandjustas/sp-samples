// -----------------------------------------------------------------------
// <copyright file="GridTableConsumer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ConnectedWebParts
{
    using System;
    using System.Linq;
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.WebControls;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Runtime.InteropServices;
    using Microsoft.SharePoint.WebPartPages;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Class repesents SharePoint Web Part. 
    /// Web part displays grid for data from connected web part.
    /// </summary>
    [ToolboxItemAttribute(false)]
    [Guid("c9dc2060-a825-4f03-b1ce-9bb476080604")]
    public class GridTableConsumer : System.Web.UI.WebControls.WebParts.WebPart
    {
        private const string FilterExpression = "FilterExpression";

        SPGridView grid;
        ObjectDataSource ds;
        IWebPartTable provider;
        PropertyDescriptorCollection schema;
        ICollection data;
        SPGridViewPager pager;

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Page size")]
        [DefaultValue(30)]
        public int PageSize { get; set; }

        protected override void CreateChildControls()
        {
            Contract.Ensures(this.grid != null);
            Contract.Ensures(this.ds != null);
            Contract.Ensures(this.pager != null);

            this.ds = new ObjectDataSource(typeof(GridTableConsumer).AssemblyQualifiedName, "SelectData")
                {
                    ID = "gridDS",
                    EnableCaching = false
                };
            this.ds.ObjectCreating += (sender, e) => e.ObjectInstance = this;
            this.ds.Filtering += (sender, e) =>
                {
                    ViewState[FilterExpression] = this.ds.FilterExpression;
                };
            this.Controls.Add(this.ds);

            this.grid = new SPGridView
                {
                    ID = "grid",
                    AutoGenerateColumns = false,
                    EnableViewState = false,
                    DataSourceID = this.ds.ID,
                    AllowSorting = true,
                    AllowFiltering = true,
                    FilteredDataSourcePropertyName = FilterExpression,
                    FilteredDataSourcePropertyFormat = "{1} = '{0}'",
                    AllowPaging = true,
                    PageSize = this.PageSize
                };
            this.grid.Sorting += (sender, e) =>
                {
                    this.ds.FilterExpression = (string)ViewState[FilterExpression] ?? "";
                };
            this.grid.RowDataBound += new GridViewRowEventHandler(grid_RowDataBound);
            this.Controls.Add(grid);

            this.pager = new SPGridViewPager()
            {
                GridViewId = "grid",
            };
            this.Controls.Add(this.pager);
        }

        #region Loading Data

        public DataTable SelectData()
        {
            EnsureDataAndSchema(); //for filters

            DataTable result = null;
            if (this.data != null && this.schema != null)
            {
                result = this.data.ToDataTable(this.schema);
            }
            return result;
        }


        protected override void OnPreRender(EventArgs e)
        {
            Contract.Assume(this.grid != null);

            base.OnPreRender(e);

            EnsureDataAndSchema();
            GenerateGridColumns(this.grid, this.schema);
            this.grid.DataBind();
        }

        private void EnsureDataAndSchema()
        {
            if (this.data == null)
            {
                this.provider.GetTableData((d, s) =>
                {
                    this.data = d;
                    this.schema = s;
                });
            }
        }



        private static void GenerateGridColumns(SPGridView grid, PropertyDescriptorCollection properties)
        {
            Contract.Requires(grid != null);

            grid.Columns.Clear();
            if (properties != null)
            {
                var fields = properties.OfType<PropertyDescriptor>()
                                       .Select(p => new SPBoundField
                                       {
                                           DataField = p.Name,
                                           HeaderText = p.DisplayName,
                                           SortExpression = p.Name
                                       })
                                       .ToList();
                fields.ForEach(grid.Columns.Add);
                grid.FilterDataFields = string.Join(",", fields.Select(f => f.DataField).ToArray());
            }
        }
        #endregion


        /// <summary>
        /// IWebPartTable consumer method.
        /// </summary>
        /// <param name="provider">IWebPartTable provider.</param>
        [ConnectionConsumer("Table")]
        public void GetTableProvider(IWebPartTable provider)
        {
            this.provider = provider;
        }

        #region Sorting and Fltering Data

        protected sealed override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);

            if (Context.Request.Form["__EVENTARGUMENT"] != null
                && Context.Request.Form["__EVENTARGUMENT"].EndsWith("__ClearFilter__")
                && Context.Request.Form["__EVENTTARGET"].StartsWith(this.ClientID))
            {
                // Clear FilterExpression
                ViewState.Remove(FilterExpression);
            }
        }

        private void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (sender == null || e.Row.RowType != DataControlRowType.Header)
            {
                return;
            }

            SPGridView grid = sender as SPGridView;

            Contract.Assume(grid != null);

            if (String.IsNullOrEmpty(grid.FilterFieldName))
            {
                return;
            }

            // Show icon on filtered column
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                DataControlField field = grid.Columns[i];

                Contract.Assume(field != null);
                if (field.SortExpression == grid.FilterFieldName)
                {
                    Image filterIcon = new Image();
                    filterIcon.ImageUrl = "/_layouts/images/filter.gif";
                    filterIcon.Style[HtmlTextWriterStyle.MarginLeft] = "2px";

                    // If we simply add the image to the header cell it will
                    // be placed in front of the title, which is not how it
                    // looks in standard SharePoint. We fix this by the code
                    // below.
                    Literal headerText = new Literal();
                    headerText.Text = field.HeaderText;

                    PlaceHolder panel = new PlaceHolder();
                    panel.Controls.Add(headerText);
                    panel.Controls.Add(filterIcon);

                    e.Row.Cells[i].Controls[0].Controls.Add(panel);

                    break;
                }
            }
        }
        #endregion
    }
}
