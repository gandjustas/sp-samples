using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SearchDataSource.TasksWebPart
{
    [ToolboxItemAttribute(false)]
    public class TasksWebPart : WebPart
    {
        GridView gridview;
        SearchDataSource dataSource;
        protected override void CreateChildControls()
        {
            dataSource = new SearchDataSource()
            {
                QueryText = "contentclass:spspeople"
            };
            dataSource.ID = "datasource";
            this.Controls.Add(dataSource);

            gridview = new GridView();
            gridview.DataSourceID = dataSource.ID;
            gridview.AllowPaging = true;
            gridview.AllowSorting = true;
            gridview.AutoGenerateColumns = true;
            this.Controls.Add(gridview);
            
        }
    }
}
