using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

namespace jqWidgetWebPart.WidgetWebPart
{
    [ToolboxItemAttribute(false)]
    [System.Runtime.InteropServices.Guid("d3501e74-ae96-4fbf-9a22-f3fa81c68f2c")]
    public class WidgetWebPart : Microsoft.SharePoint.WebPartPages.WebPart
    {
        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("JQuery UI Widget Name")]
        public string WidgetName { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string WidgetParameters { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string WidgetParameterDefinition { get; set; }

        protected override void CreateChildControls()
        {
            if (string.IsNullOrEmpty(this.WidgetName))
            {
                this.Controls.Add(new LiteralControl("Widget Name is not set. Edit web pat and specify widget."));
            }
            else
            {                
                this.Attributes["data-widget-name"] = this.WidgetName;

                if (!string.IsNullOrEmpty(this.WidgetParameters))
                {
                    var parts = this.WidgetParameters.Split('&');
                    foreach (var part in parts)
                    {
                        var p = part.Split('=');
                        if (p.Length == 2)
                        {
                            this.Attributes["data-" + p[0]] = this.Context.Server.UrlDecode(p[1]);
                        }
                    }
                }
            }
        }

        public override ToolPart[] GetToolParts()
        {
            var tps = base.GetToolParts().ToList();
            tps.Add(new WidgetEditorPart());
            return tps.ToArray();
        }
    }
}
