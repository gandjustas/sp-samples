// -----------------------------------------------------------------------
// <copyright file="OrganizationTreeToolpart.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ConnectedWebParts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.UI.WebControls;
    using Microsoft.Office.Server.UserProfiles;
    using Microsoft.SharePoint;
    using System.Web.UI;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Class represents ToolPart for select Organization Tree profile property names
    /// </summary>
    public class OrganizationTreeToolPart : ToolPart
    {
        ListBox list;
        public OrganizationTreeToolPart()
        {
            this.Title = "Profile properties";
        }

        protected OrganizationTree WebPart
        {
            get
            {
                Contract.Assume(this.ParentToolPane != null);
                return this.ParentToolPane.SelectedWebPart as OrganizationTree;
            }
        }

        protected override void CreateChildControls()
        {
            Contract.Ensures(this.list != null);

            this.list = new ListBox()
            {
                SelectionMode = ListSelectionMode.Multiple,
                Height = Unit.Pixel(200)
            };
            var names = this.WebPart.ProfilePropertyNames;
            var upm = new UserProfileManager(SPServiceContext.Current);

            Contract.Assume(upm.DefaultProfileSubtypeProperties != null);
            var items = from p in upm.DefaultProfileSubtypeProperties
                        where !p.IsSection
                        orderby p.DisplayOrder
                        select new ListItem(
                            string.Format("{0} ({1})", p.DisplayName, p.Name),
                            p.Name)
                            {
                                Selected = names.Contains(p.Name)
                            };
            this.list.Items.AddRange(items.ToArray());
            this.Controls.Add(this.list);

        }

        public override void ApplyChanges()
        {
            EnsureChildControls();

            var set = new HashSet<string>(this.list
                        .GetSelectedIndices()
                        .Select(i => this.list.Items[i].Value));
            this.WebPart.ProfilePropertyNames = set;
        }
    }
}
