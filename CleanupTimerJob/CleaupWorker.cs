// -----------------------------------------------------------------------
// <copyright file="CleaupWorker.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace CleanupTimerJob
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CleaupWorker : SPWorkItemJobDefinition
    {
        TimerJobUtility tju;
        WorkItemTimerJobState wiJobState  = new WorkItemTimerJobState(true);

        public CleaupWorker() : base() { }

        public CleaupWorker(SPWebApplication webApp)
            : base(Constants.WorkerJobName, webApp)
        {
            this.Title = "Folder cleanup worker";
        }

        public override Guid WorkItemType()
        {
            return Constants.WorkItemType;
        }

        protected override bool ProcessWorkItem(SPContentDatabase contentDatabase, SPWorkItemCollection workItems, SPWorkItem workItem, SPJobState jobState)
        {
            tju = new TimerJobUtility(Constants.TimerJobName, jobState);
            return tju.ProcessWorkItem(workItems, workItem, wiJobState, ProcessWorkItemCore);
        }

        private void ProcessWorkItemCore(SPWorkItem wi, WorkItemTimerJobState timerJobstate)
        {
            var list = timerJobstate.Web.Lists[wi.ParentId];
            CleanupUtility.CleanupList(list);
        }
    }
}
