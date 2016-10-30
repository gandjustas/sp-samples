using Microsoft.SharePoint.WebPartPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace jqWidgetWebPart.WidgetWebPart
{
    public class WidgetEditorPart: ToolPart
    {
        private Dictionary<string, string> parameters;
        private Dictionary<string, string> parameterValues;
        private Dictionary<string, TextBox> inputBoxes = new Dictionary<string,TextBox>();
        
        public WidgetEditorPart()
        {
            this.Title = "Widget Parameters";
        }

        protected WidgetWebPart WebPart
        {
            get
            {
                return this.ParentToolPane.SelectedWebPart as WidgetWebPart;
            }
        }



        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            if(string.IsNullOrEmpty(this.WebPart.WidgetParameterDefinition))
                return;

            this.parameters =
                this.WebPart
                    .WidgetParameterDefinition
                    .Split('&')
                    .Select(p => p.Split('='))
                    .Where(pair => pair.Length > 1)
                    .ToDictionary(pair => pair[0], pair => this.Context.Server.UrlDecode(pair[1]));

            this.parameterValues =
                this.WebPart
                    .WidgetParameters
                    .Split('&')
                    .Select(p => p.Split('='))
                    .Where(pair => pair.Length > 1)
                    .ToDictionary(pair => pair[0], pair => this.Context.Server.UrlDecode(pair[1]));

            var table = new Table();
            foreach (var item in parameters)
            {
                var row = new TableRow();
                var cell = new TableCell();
                row.Cells.Add(cell);
                table.Rows.Add(row);

                var input = new TextBox() { Text = this.parameterValues[item.Key] ?? "" };
                this.inputBoxes.Add(item.Key, input);

                cell.Controls.Add(new Panel()
                {
                    Controls = { new LiteralControl(item.Value) }
                });
                cell.Controls.Add(new Panel()
                {
                    Controls = { input }
                });
            }
            this.Controls.Add(table);
        }

        public override void ApplyChanges()
        {
            EnsureChildControls();

            this.WebPart.WidgetParameters =
                string.Join("&",this.inputBoxes
                                    .Select(p => p.Key + '=' + this.Context.Server.UrlEncode(p.Value.Text))
                                    .ToArray());
                    
        }
    }
}
