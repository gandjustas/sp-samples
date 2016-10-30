using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace SearchDataSource.TasksWebPart
{
    public class SearchDataSource: DataSourceControl
    {
        protected override DataSourceView GetView(string viewName)
        {
            return new SearchDataSourceView(this);
        }

        public string QueryText { get; set; }

        public string SelectProperties { get; set; }     
   
        public string SortOrder { get; set; }     

    }
}
