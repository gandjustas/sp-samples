using System;
using System.Collections.Generic;
using Microsoft.SharePoint.Administration;

namespace UlsLogging.Layouts.UlsLogging
{
    //FULL BLOWN LOGGING
    public class Logger3 : SPDiagnosticsServiceBase
    {
        public const string ServiceName = "My Logging Service2";
        public const string DiagnosticAreaName = "My";
        public const string DiagnosticCategoryName = "Default";
        private static Logger3 _current;

        public static Logger3 Current
        {
            get
            {
                if (_current == null)
                {
                    _current = SPFarm.Local.GetChild<Logger3>(ServiceName) ?? new Logger3(ServiceName);
                }

                return _current;
            }
        }

        public Logger3():base()
        {
            
        }
        private Logger3(string name)
            : base(name, SPFarm.Local)
        {

        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            List<SPDiagnosticsArea> areas = new List<SPDiagnosticsArea>
            {
                new SPDiagnosticsArea(DiagnosticAreaName, new List<SPDiagnosticsCategory>
                {
                    new SPDiagnosticsCategory(DiagnosticCategoryName, TraceSeverity.Verbose, EventSeverity.Error)
                })
            };

            return areas;
        }

        public static void Log(string messae)
        {
            SPDiagnosticsCategory category =
                Logger3.Current.Areas[DiagnosticAreaName].Categories[DiagnosticCategoryName];
            Logger3.Current.WriteTrace(0, category, TraceSeverity.Unexpected, messae);
        }
    }
}
