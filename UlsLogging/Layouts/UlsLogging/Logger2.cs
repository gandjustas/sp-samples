using System;
using System.Collections.Generic;
using System.ComponentModel;
using Microsoft.SharePoint.Administration;

namespace UlsLogging.Layouts.UlsLogging
{
    //DO NOT USE THIS
    public class Logger2 : SPDiagnosticsServiceBase
    {
        private static readonly SPDiagnosticsCategory category = new SPDiagnosticsCategory("MyCategory", TraceSeverity.Verbose, EventSeverity.Error);
        private static readonly SPDiagnosticsArea area = new SPDiagnosticsArea("MyArea", new[] {category});
    

        private static Logger2 _current;

        public static Logger2 Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new Logger2();
                }

                return _current;
            }
        }

        private Logger2()
            : base("My Logging Service", SPFarm.Local)
        {

        }

        protected override IEnumerable<SPDiagnosticsArea> ProvideAreas()
        {
            yield return area;
        }

        public static void Log(string message)
        {
            Logger2.Current.WriteTrace(0, category, TraceSeverity.Unexpected, message);
        }
    }

}
