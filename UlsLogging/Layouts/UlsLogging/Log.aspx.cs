using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace UlsLogging.Layouts.UlsLogging
{
    public partial class Log : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Button1_OnClick(object sender, EventArgs e)
        {
            //Simple
            Logger1.Log("Hello ULS");
        }

        protected void Button2_OnClick(object sender, EventArgs e)
        {
            //DO NOT USE THIS
            Logger2.Log("Hello ULS");
        }

        protected void Button3_OnClick(object sender, EventArgs e)
        {
            //FULL BLOWN LOGGING
            Logger3.Log("Hello ULS");
        }
    }
}
