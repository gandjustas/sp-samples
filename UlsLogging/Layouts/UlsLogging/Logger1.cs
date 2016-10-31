using Microsoft.SharePoint.Administration;

namespace UlsLogging.Layouts.UlsLogging
{
    //Simple
    public class Logger1
    {
        public static void Log(string message)
        {
            SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;
            diagSvc.WriteTrace(0,
                new SPDiagnosticsCategory("My Category",
                    TraceSeverity.Monitorable,
                    EventSeverity.Error),
                TraceSeverity.Monitorable,
                message);
        }
    }
}