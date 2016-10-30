// -----------------------------------------------------------------------
// <copyright file="CleanupUtility.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupTimerJob
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.SharePoint;
    using Microsoft.Office.Server.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class CleanupUtility
    {
        public static void CleanupList(SPList list)
        {
            if (!list.Hidden && list is SPDocumentLibrary)
            {
                DeleteEmptyFolders(new SPFolderHierarchy(list));
            }
        }

        private static void DeleteEmptyFolders(SPFolderHierarchy h)
        {
            foreach (SPFolder folder in (h as IEnumerable<SPFolder>))
            {
                if (folder.Item != null)
                {
                    DeleteEmptyFolders(h.GetSubFolders(folder.ServerRelativeUrl));
                    if (folder.ItemCount == 0)
                    {
                        folder.Delete();
                    }
                }
            }
        }

        public static void AddCleanupWorkitem(SPWeb web, Guid listId)
        {
            if (web.UserIsSiteAdmin)
            {
                AddCleanupWorkitemAux(web.Site, web, listId);
            }
            else
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var site = new SPSite(web.Site.ID))
                    {
                        AddCleanupWorkitemAux(site, web, listId);
                    }
                });
            }
        }

        private static void AddCleanupWorkitemAux(SPSite site, SPWeb web, Guid listId)
        {
            site.AddWorkItem(
                new Guid(), DateTime.UtcNow, Constants.WorkItemType,
                web.ID, listId, -1,
                true, new Guid(), web.ID,
                web.CurrentUser.ID, null, null, new Guid());
        }

    }
}
