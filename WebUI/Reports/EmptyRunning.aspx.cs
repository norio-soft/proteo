using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Data;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Reports
{
    public partial class EmptyRunning : Orchestrator.Base.BasePage
    {
        private List<int> TrafficAreaIDs
        {
            get
            {

                var trafficAreaIDs = new List<int>();
                if (chkSelectAllTrafficAreas.Checked)
                {
                    int identityId = ((Entities.CustomPrincipal)Page.User).IdentityId;
                    Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
                    var trafficareas = facTrafficArea.GetAll();
                    foreach (DataRow dr in trafficareas.Tables[0].Rows)
                        trafficAreaIDs.Add((int)dr["TrafficAreaId"]);
                    return trafficAreaIDs;
                }

                    

                
                int holding;
                foreach (ListItem li in cblTrafficAreas.Items)
                    if (li.Selected && int.TryParse(li.Value, out holding) && !trafficAreaIDs.Contains(holding))
                        trafficAreaIDs.Add(holding);

                return trafficAreaIDs;
            }
        }

        private List<int> ClientIDs
        {
            get { 
                var clientIDs = new List<int>();
                foreach (RadListBoxItem selectedClient in lbSelectedClients.Items)
                {
                    clientIDs.Add(int.Parse(selectedClient.Value));
                }
                return clientIDs;
            }
        }

        private eEmptyRunningGrouping ReportGrouping
        {
            get
            {
                return radGrouping1.Checked
                           ? eEmptyRunningGrouping.TrafficAreaThenClient
                           : eEmptyRunningGrouping.ClientThenTrafficArea;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                LoadFilterData();

            foreach (ListItem item in cblTrafficAreas.Items)
            {
                item.Attributes.Add("onclick", "onTrafficAreaChecked();");
            }
        }

        private void LoadFilterData()
        {
            // Load the clients
            var orgFacade = new Facade.Organisation();
            var orgs = orgFacade.GetAllForType(2);

            lbAvailableClients.DataSource = orgs.Tables[0];
            lbAvailableClients.DataBind();

            // Load the traffic areas
            Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
            cblTrafficAreas.DataSource = facTrafficArea.GetAll();
            cblTrafficAreas.DataBind();
        }
        
        protected void btnExport_Click(object sender, EventArgs e)
        {
            var dsEmptyRunning = GetReportDataSource();
            Session["__ExportDS"] = dsEmptyRunning.Tables[0];
            Response.Redirect("../reports/csvexport.aspx?filename=EmptyRunningByDateRange"+ String.Format("{0:yyyyMMdd}",dteStartDate.SelectedDate) + "_" +String.Format("{0:yyyyMMdd}",dteEndDate.SelectedDate) + ".CSV");
        }

        protected void btnExportSummary_Click(object sender, EventArgs e)
        {
            var dsEmptyRunning = GetSummaryDataSource();

            Session["__ExportDS"] = dsEmptyRunning.Tables[0];
            Response.Redirect("../reports/csvexport.aspx?filename=EmptyRunningSummaryByDateRange"+ String.Format("{0:yyyyMMdd}",dteStartDate.SelectedDate) + "_" +String.Format("{0:yyyyMMdd}",dteEndDate.SelectedDate) + ".CSV");
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            var dsEmptyRunning = GetReportDataSource();

            var reportParams = new NameValueCollection
                {
                    {"StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy")},
                    {"EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy")},
                    {"GroupingOption", ReportGrouping.ToString()}
                };

            // Configure the Session variables used to pass data to the report
            Session[Globals.Constants.ReportTypeSessionVariable] = eReportType.EmptyRunning;
            Session[Globals.Constants.ReportDataSessionTableVariable] = dsEmptyRunning;
            Session[Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Globals.Constants.ReportParamsSessionVariable] = reportParams;

            // Show the user control
            reportViewer.Visible = true;
        }

        /// <summary>
        /// Returns the correct dataset depending on the filters.
        /// </summary>
        /// <returns></returns>
        private DataSet GetReportDataSource()
        {
            var facEmptyRunning = new Facade.EmptyRunning();
            return facEmptyRunning.GetEmptyRunningForDateRangeAndClientsAndTrafficAreas(ClientIDs, TrafficAreaIDs, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value, ReportGrouping);
        } 

        private DataSet GetSummaryDataSource()
        {
            var facEmptyRunning = new Facade.EmptyRunning();
            return facEmptyRunning.GetSummaryForDateRangeAndClientsAndTrafficAreas(ClientIDs, TrafficAreaIDs, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);
        }
        
        //---------------------------------------------------------------------------------------

    }
}
