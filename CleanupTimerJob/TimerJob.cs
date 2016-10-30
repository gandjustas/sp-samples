using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint;
using Microsoft.Office.Server.Utilities;

namespace CleanupTimerJob
{
    public class TimerJob : SPAllSitesJobDefinition
    {
        TimerJobUtility tju;

        public TimerJob(): base() { }

        public TimerJob(SPWebApplication webApp)
            : base(Constants.TimerJobName, webApp)
        {
            this.Title = "Folder cleanup job";
        }

        public override void ProcessSite(SPSite site, SPJobState jobState)
        {
            tju = new TimerJobUtility(Constants.TimerJobName, jobState);
            tju.DisableEventFiring = false;
            tju.CancellationGranularity = IterationGranularity.List;
            tju.ResumeGranularity = IterationGranularity.List;
            tju.ProcessSite(site, ProcessSite);
        }

        private void ProcessSite(SPSite site)
        {
            tju.ProcessSite(site, ProcessWeb, null);
        }

        private void ProcessWeb(SPWeb web)
        {
            if (Convert.ToBoolean(web.Properties[Constants.FlagPropertyName]))
            {
                tju.ProcessLists(web.Lists, CleanupUtility.CleanupList, null);
            }
        }

    }
}
