using ClosedXML.Excel;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ExportToExcel.Control
{
    [System.Runtime.InteropServices.GuidAttribute("7FD7C6F0-4EDA-48CE-AC8F-AA9F9D2666AC")]
    public class ExportToSpreadsheetControl : System.Web.UI.Control
    {
        protected override void OnLoad(EventArgs e) { 
            if (this.Page.Request["__EVENTTARGET"] == Constants.ExportToSpreadsheet) 
            {
                var spContext  = SPContext.Current;
                var web = spContext.Web;

                var pair = this.Page.Request["__EVENTARGUMENT"].Split('|');
                var list = web.Lists[new Guid(pair[0])];
                var view = list.Views[new Guid(pair[1])];
                var title = string.IsNullOrEmpty(view.Title) ? list.Title : (list.Title + " - " + view.Title);
                ExportData(title, GetDataTable(list, view));
            } 
        }

        private void ExportData(string title, System.Data.DataTable table)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add(title);
            ws.Cell(1, 1).InsertTable(table);

            var response = this.Page.Response;
            response.Clear();

            response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var filename = title+".xlsx";
            response.AddHeader("content-disposition", GetContentDisposition(filename));

            // Flush the workbook to the Response.OutputStream
            using (var memoryStream = new MemoryStream())
            {
                wb.SaveAs(memoryStream);
                memoryStream.WriteTo(response.OutputStream);
            }
            response.End();
        }


        //http://stackoverflow.com/questions/93551/how-to-encode-the-filename-parameter-of-content-disposition-header-in-http
        private string GetContentDisposition(string filename)
        {
            var request = this.Page.Request;
            string contentDisposition;
            if (request.Browser.Browser == "IE" && (request.Browser.Version == "7.0" || request.Browser.Version == "8.0"))
                contentDisposition = "attachment; filename=" + Uri.EscapeDataString(filename);
            else if (request.UserAgent != null && request.UserAgent.ToLowerInvariant().Contains("android")) // android built-in download manager (all browsers on android)
                contentDisposition = "attachment; filename=\"" + MakeAndroidSafeFileName(filename) + "\"";
            else
                contentDisposition = "attachment; filename=\"" + filename + "\"; filename*=UTF-8''" + Uri.EscapeDataString(filename);
            return contentDisposition;
        }

        private static readonly Dictionary<char, char> AndroidAllowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ._-+,@£$€!½§~'=()[]{}0123456789".ToDictionary(c => c);
        private string MakeAndroidSafeFileName(string fileName)
        {
            char[] newFileName = fileName.ToCharArray();
            for (int i = 0; i < newFileName.Length; i++)
            {
                if (!AndroidAllowedChars.ContainsKey(newFileName[i]))
                    newFileName[i] = '_';
            }
            return new string(newFileName);
        }

        private static System.Data.DataTable GetDataTable(SPList list, SPView view)
        {
            var query = new SPQuery(view);
            SPListItemCollectionPosition position;
            var flags = SPListGetDataTableOptions.UseBooleanDataType | SPListGetDataTableOptions.UseCalculatedDataType;
            var result = list.GetDataTable(query, flags, out position);
            while (position != null)
            {
                query.ListItemCollectionPosition = position;
                list.AppendDataTable(query, flags, result, out position);
            }
            return result;
        }
    }
}
