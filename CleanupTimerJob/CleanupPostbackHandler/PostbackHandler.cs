using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Runtime.InteropServices;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace CleanupTimerJob.CleanupPostbackHandler
{
    [ToolboxItem(false)]
    [Guid("0a3b8df1-0a43-485d-be83-983c1df8b30d")]
    public class PostbackHandler : WebControl
    {
        protected override void OnLoad(EventArgs e)
        {
            if (this.Page.Request["__EVENTTARGET"] == Constants.CleanupPostBackEvent)
            {
                var listId = new Guid(this.Page.Request["__EVENTARGUMENT"]);
                var web = SPContext.Current.Web;

                CleanupUtility.AddCleanupWorkitem(web, listId);

                ScriptLink.RegisterScriptAfterUI(this.Page, "sp.js", false, true);
                var scriptText = ScriptLink.BuildDelayedExecutionScript("sp.js", 
                    "SP.UI.Notify.addNotification", 
                    "'Library submitted to cleanup'", "false");
                
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),
                            Constants.CleanupPostBackEvent,
                            "function showPostBackNotification() {" +scriptText + "} _spBodyOnLoadFunctionNames.push('showPostBackNotification');",
                            true);              
            }
        }

    }
}
