// -----------------------------------------------------------------------
// <copyright file="OrganizationTree.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ConnectedWebParts
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint;
    using Microsoft.SharePoint.WebControls;
    using Microsoft.Office.Server.UserProfiles;
    using System.Data;
    using System.Runtime.InteropServices;
    using Microsoft.SharePoint.WebPartPages;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// Class repesents SharePoint Web Part. 
    /// Web part displays organization tree from organization profiles.
    /// </summary>
    [ToolboxItemAttribute(false)]
    [Guid("6e5df2d4-5c9d-4fcf-b32f-22bec808bd18")]
    public class OrganizationTree : Microsoft.SharePoint.WebPartPages.WebPart, IWebPartTable, IWebEditable
    {
        private const string PropertyNamesDelimeter = ";#";

        private TreeView tree;
        private OrganizationProfileManager opm;
        PropertyDescriptorCollection schema;


        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Async load nodes")]
        [WebDescription("Load organization nodes in background on demand")]
        [DefaultValue(true)]
        public bool PopulateNodesFromClient { get; set; }

        [WebBrowsable(true)]
        [Personalizable(PersonalizationScope.Shared)]
        [WebDisplayName("Expand depth")]
        [WebDescription("Maximum level of expanded organizations")]
        [DefaultValue(2)]
        public int ExpandDepth { get; set; }

        [WebBrowsable(false)]
        [Personalizable(PersonalizationScope.Shared)]
        public string ProfileProperties { get; set; }

        public HashSet<string> ProfilePropertyNames
        {
            get
            {
                return new HashSet<string>((ProfileProperties ?? "").Split(new[] { PropertyNamesDelimeter }, StringSplitOptions.None));
            }
            set
            {
                if (value != null)
                {
                    ProfileProperties = string.Join(PropertyNamesDelimeter, value.ToArray());
                }
            }
        }

        /// <summary>
        /// Gets current OrganizationProfileManager from context.
        /// </summary>
        protected OrganizationProfileManager OrganizationProfileManager
        {
            get
            {
                if (this.opm == null)
                {
                    this.opm = new OrganizationProfileManager(SPServiceContext.Current);
                }
                return this.opm;
            }
        }



        #region Build Tree

        protected override void CreateChildControls()
        {
            Contract.Ensures(this.tree != null);
            Contract.Assume(this.OrganizationProfileManager.RootOrganization != null);

            this.tree = new TreeView();
            this.tree.EnableClientScript = true;
            this.tree.PopulateNodesFromClient = this.PopulateNodesFromClient;
            this.tree.ExpandDepth = this.ExpandDepth;
            this.tree.TreeNodePopulate += new TreeNodeEventHandler(tree_TreeNodePopulate);
            this.tree.Nodes.Add(ToTreeNode(this.OrganizationProfileManager.RootOrganization));
            Controls.Add(this.tree);
        }


        void tree_TreeNodePopulate(object sender, TreeNodeEventArgs e)
        {
            Contract.Assume(e.Node != null);
            Contract.Assume(e.Node.Value != null);

            var recordId = int.Parse(e.Node.Value);
            var profile = this.OrganizationProfileManager.GetOrganizationProfile(recordId);

            Contract.Assume(profile.GetChildren() != null);
            foreach (var child in profile.GetChildren())
            {
                e.Node.ChildNodes.Add(ToTreeNode(child));
            }

        }

        private static TreeNode ToTreeNode(OrganizationProfile child)
        {
            Contract.Requires(child != null);
            Contract.Ensures(Contract.Result<TreeNode>() != null);

            return new TreeNode(child.DisplayName, child.RecordId.ToString())
                   {
                       PopulateOnDemand = child.HasChildren,
                   };
        }

        #endregion

        #region Table provider

        /// <summary>
        /// IWebPartTable provider method.
        /// </summary>
        /// <returns>IWebPartTable provider.</returns>
        [ConnectionProvider("Users")]
        public IWebPartTable SendUsersFromSelectedNode()
        {
            return this;
        }

        public void GetTableData(TableCallback callback)
        {
            if (callback != null)
            {
                EnsureChildControls();
                if (this.tree.SelectedNode != null)
                {
                    long recordId = 0;

                    if (long.TryParse(this.tree.SelectedValue, out recordId))
                    {
                        var profiles = this.OrganizationProfileManager
                                           .GetOrganizationProfile(recordId)
                                           .GetImmediateMembers();
                        callback(profiles);
                    }
                }
            }
        }

        public PropertyDescriptorCollection Schema
        {
            get
            {
                if (this.schema == null)
                {

                    var names = this.ProfilePropertyNames;
                    var upm = new UserProfileManager(SPServiceContext.Current);

                    Contract.Assume(upm.DefaultProfileSubtypeProperties != null);
                    //Filter section headers from property list
                    var props = from prop in upm.DefaultProfileSubtypeProperties
                                where !prop.IsSection
                                where names.Contains(prop.Name)
                                orderby prop.DisplayOrder
                                select new UserProfilePropertyDescriptor(prop);
                    this.schema = new PropertyDescriptorCollection(props.ToArray());
                }
                return this.schema;
            }
        }
        #endregion

        public override ToolPart[] GetToolParts()
        {
            var tps = base.GetToolParts().ToList();
            tps.Add(new OrganizationTreeToolPart());
            return tps.ToArray();
        }
    }
}

