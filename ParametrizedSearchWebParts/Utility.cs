// -----------------------------------------------------------------------
// <copyright file="Utility.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ParametrizedSearchWebParts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.Office.Server.Search.Query;
    using System.Text.RegularExpressions;
    using System.ComponentModel;
    using System.Collections;
    using System.Globalization;
using System.Web.UI.WebControls.WebParts;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    internal static class Utility
    {
        static Regex parser = new Regex(@"\{(?<name>\w+)\s*:\s*(?<type>[\w\.]+)\s*(?:\(\s*(?<format>.+?)\s*\))?\s*\}", RegexOptions.Compiled);

        public static PropertyDescriptorCollection GetQueryParametersSchema(string queryText)
        {
            var desriptors =
                parser.Matches(queryText)
                      .OfType<Match>()
                      .Select(m =>
                        new CustomPropertyDescriptor(m.Groups["name"].Value, Type.GetType(m.Groups["type"].Value, false)));
            return new PropertyDescriptorCollection(desriptors.ToArray());
        }

        public static string SubstituteParameters(string s, Hashtable parameters)
        {
            return parser.Replace(s, m =>
            {
                var format = m.Groups["format"].Success ? "{0:" + m.Groups["format"].Value + "}" : "{0}";
                return string.Format(CultureInfo.InvariantCulture, format, parameters[m.Groups["name"].Value]);
            });
        }

    }
}
