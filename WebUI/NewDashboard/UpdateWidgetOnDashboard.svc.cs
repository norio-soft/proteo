using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;

namespace Orchestrator.WebUI.NewDashboard
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [ServiceContract(Namespace = "")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class UpdateWidgetOnDashboard
    {
        // To use HTTP GET, add [WebGet] attribute. (Default ResponseFormat is WebMessageFormat.Json)
        // To create an operation that returns XML,
        //     add [WebGet(ResponseFormat=WebMessageFormat.Xml)],
        //     and include the following line in the operation body:
        //         WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
        [OperationContract]
        public void updateWidgetPosition(int widgetOnDashboardID, int top, int left)
        {
            DashboardDataAccess.updateWidgetPosition(widgetOnDashboardID, top, left);
        }

        [OperationContract]
        public void updateWidgetSize(int widgetOnDashboardID, int height, int width)
        {
            DashboardDataAccess.updateWidgetSize(widgetOnDashboardID, height, width);
        }

        // Add more operations here and mark them with [OperationContract]
    }
}
