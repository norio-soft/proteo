using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Specialized;

namespace Orchestrator.WebUI.Groupage
{
    public partial class TransshippingSheet : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            reportViewer.Visible = false;

            int jobID = 0;
            if (int.TryParse(Request.QueryString["JobID"], out jobID))
            {
                ProduceReport(jobID);
            }
        }

        private void ProduceReport(int jobID)
        {
            Facade.IOrder facOrder = new Facade.Order();
            DataSet sheetData = facOrder.GetTransShippedOrdersOnJob(jobID);

            if (sheetData != null && sheetData.Tables.Count == 1)
            {
                DataTable instructionData = new DataTable();
                instructionData.Columns.Add(new DataColumn("Instruction"));
                DataRow row = instructionData.NewRow();
                row["Instruction"] = txtInstruction.Text;
                instructionData.Rows.Add(row);
                sheetData.Tables.Add(instructionData);

                sheetData.AcceptChanges();

                NameValueCollection reportParams = new NameValueCollection();
                reportParams.Add("JobID", jobID.ToString());

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.TransshippingSheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = sheetData;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                reportViewer.Visible = true;
            }
        }
    }
}