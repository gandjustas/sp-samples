// -----------------------------------------------------------------------
// <copyright file="XsltTableConsumer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ConnectedWebParts
{
    using System;
    using System.ComponentModel;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.WebControls;
    using System.Runtime.InteropServices;
    using System.Data;
    using System.Xml.Xsl;
    using System.IO;
    using System.Collections.Generic;
    using System.Xml;
    using Microsoft.SharePoint.Utilities;

    /// <summary>
    /// Class repesents SharePoint Web Part. 
    /// Web part displays data from connected web part by applying xsl transform.
    [ToolboxItemAttribute(false)]
    [Guid("b914adea-e076-4392-9c8e-f426c58e5ea7")]
    public class XsltTableConsumer : WebPart
    {
        IWebPartTable provider;

        [WebBrowsable(true)]
        [Category("Custom")]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Xsl Link")]
        [WebDescription("Link to Xslt file")]
        public string XslLink { get; set; }

        [WebBrowsable(true)]
        [Category("Custom")]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Xsl")]
        [WebDescription("Xsl Transformation")]
        public string Xsl { get; set; }

        [ConnectionConsumer("Table")]
        public void GetTableProvider(IWebPartTable provider)
        {
            this.provider = provider;
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            if (this.provider != null)
            {
                provider.GetDataTable(d =>
                    {
                        d.TableName = "Row";
                        var ds = new DataSet("Rows") { Tables = { d } };
                        ds.SchemaSerializationMode = SchemaSerializationMode.IncludeSchema;

                        var stream = new MemoryStream();
                        ds.WriteXml(stream, XmlWriteMode.WriteSchema);
                        stream.Position = 0;

                        if (!string.IsNullOrEmpty(this.XslLink))
                        {
                            XslCompiledTransform t;
                            if (!transformCache.TryGetValue(this.XslLink, out t))
                            {
                                t = new XslCompiledTransform();
                                t.Load(this.XslLink);
                                transformCache[this.XslLink] = t;
                            }
                            t.Transform(XmlReader.Create(stream), XmlWriter.Create(writer.InnerWriter));
                        }
                        else if (string.IsNullOrEmpty(this.Xsl))
                        {
                            var t = new XslCompiledTransform();
                            t.Load(this.Xsl);
                            t.Transform(XmlReader.Create(stream), XmlWriter.Create(writer.InnerWriter));
                        }
                        else
                        {
                            var reader = new StreamReader(stream);
                            writer.RenderBeginTag("xmp");
                            writer.Write(reader.ReadToEnd());
                            writer.RenderEndTag();
                        }
                    });
            }
        }

        private static readonly Dictionary<string, XslCompiledTransform> transformCache = new Dictionary<string, XslCompiledTransform>();
    }
}
