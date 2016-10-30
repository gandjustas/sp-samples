using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CleanupTimerJob
{
    public static class Constants
    {
        public const string TimerJobName = "CleanupJob";
        public const string WorkerJobName = "CleanupWorker";
        public const string FlagPropertyName = "CleanupEnabled";
        public const string CleanupPostBackEvent = "CleanupPostBackEvent";

        public static readonly Guid WorkItemType = new Guid("{CAB189CA-5589-4387-96F6-3E2DFE6F257C}");
    }
}
