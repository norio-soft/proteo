using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;

namespace Orchestrator.WebUI.Services
{
    public static class WcfServiceError
    {

        private const string SOURCE = "Orchestrator WebUI WCF Service";
        private const string LOG = "Application";

        public static void LogException(Exception ex)
        {
            if (!EventLog.SourceExists(SOURCE))
                EventLog.CreateEventSource(SOURCE, LOG);

            EventLog.WriteEntry(SOURCE, ex.ToString(), EventLogEntryType.Error);
        }

    }
}
