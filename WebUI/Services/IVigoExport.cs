using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Orchestrator.WebUI.Services
{
    // NOTE: If you change the interface name "IVigoExport" here, you must also update the reference to "IVigoExport" in Web.config.
    [ServiceContract]
    public interface IVigoExport
    {
        [OperationContract]
        string[] GetExportMessages(int Rows, ref int LastExportMessageId);

        [OperationContract]
        string[] GetExportIntegrationMessages(string integrationSource, int Rows, ref int LastExportMessageId);

        [OperationContract]
        void SendImportMessages(string[] Messages);

    }
}
