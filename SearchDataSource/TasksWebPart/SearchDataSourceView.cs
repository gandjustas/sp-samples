using Microsoft.Office.Server.Search.Query;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace SearchDataSource.TasksWebPart
{
    class SearchDataSourceView: DataSourceView
    {
        SearchDataSource dataSource;

        public SearchDataSourceView(SearchDataSource dataSource): base(dataSource,"")
        {
            this.dataSource = dataSource;
        }

        public override bool CanPage
        {
            get
            {
                return true;
            }
        }

        public override bool CanSort
        {
            get
            {
                return true;
            }
        }

        public override bool CanRetrieveTotalRowCount
        {
            get
            {
                return true;
            }
        }

        protected override System.Collections.IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            var query = GetQuery(string.IsNullOrEmpty(arguments.SortExpression)?dataSource.SortOrder:arguments.SortExpression);

            query.StartRow = arguments.StartRowIndex;
            query.RowLimit = arguments.MaximumRows;

            var results = query.Execute()[ResultType.RelevantResults];
            arguments.TotalRowCount = results.TotalRows;
            return results.Table.DefaultView;
        }

        private KeywordQuery GetQuery(string sortExpression)
        {
            var query = new KeywordQuery(SPContext.Current.Site);
            query.QueryText = this.dataSource.QueryText;
            query.ResultTypes = ResultType.RelevantResults;

            if (!string.IsNullOrEmpty(this.dataSource.SelectProperties))
            {
                query.SelectProperties.Clear();
                foreach (var property in this.dataSource.SelectProperties.Split(','))
                {
                    query.SelectProperties.Add(property.Trim());
                }
            }

            if (!string.IsNullOrEmpty(sortExpression))
            {
                query.SortList.Clear();
                foreach (var sort in sortExpression.Split(','))
                {
                    if (sort.EndsWith(" DESC"))
                    {
                        query.SortList.Add(sort.Substring(0, sort.IndexOf(" DESC")), SortDirection.Descending);
                    }
                    else
                    {
                        query.SortList.Add(sort, SortDirection.Ascending);
                    }
                }
            }

            return query;
        }
    }
}
