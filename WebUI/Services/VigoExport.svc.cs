using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Linq;

namespace Orchestrator.WebUI.Services
{
    // NOTE: If you change the class name "VigoExport" here, you must also update the reference to "VigoExport" in Web.config.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class VigoExport : IVigoExport
    {
        public const string INTEGRATION_SOURCE_NAME = "VIGO";
        public const string USERNAME = "Vigo Integration";

        [Obsolete("Use GetExportIntegrationMessages instead.")]
        public string[] GetExportMessages(int Rows, ref int LastExportMessageId)
        {
            return GetExportIntegrationMessages(INTEGRATION_SOURCE_NAME, Rows, ref LastExportMessageId);
        }

        public string[] GetExportIntegrationMessages(string IntegrationSource, int Rows, ref int LastExportMessageId)
        {
            Facade.ExportMessage facExportMessage = new Facade.ExportMessage();
            List<Entities.ExportMessage> exportMessages = facExportMessage.GetToProcess(
                    IntegrationSource, LastExportMessageId, Rows);

            //Set the LastExportMesageId to the greatest LastExportMessageId
            //This will be passed in next time to determine which rows need to be returned then
            if (exportMessages.Count > 0)
            {
                LastExportMessageId = exportMessages.Max(m => m.ExportMessageId);

                string[] messageStrings = (from m in exportMessages
                                           select m.Message).ToArray();

                return messageStrings;
            }
            else
            {
                return new string[] { };
            }
        }

        public void SendImportMessages(string[] Messages)
        {
            Facade.ImportMessage facImportMessage = new Facade.ImportMessage();

            foreach (string message in Messages)
            {
                facImportMessage.Create(
                    INTEGRATION_SOURCE_NAME,
                    eMessageState.Unprocessed,
                    message,
                    USERNAME);
            }
        }

        //public CompositeType GetDataUsingDataContract(CompositeType composite)
        //{
        //    if (composite.BoolValue)
        //    {
        //        composite.StringValue += "Suffix";
        //    }
        //    return composite;
        //}

    }
}
