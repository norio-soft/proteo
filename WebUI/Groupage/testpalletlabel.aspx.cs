using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Groupage
{
    public partial class testpalletlabel : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //-------------------------------------------------------------------------------------	
            //									Load Report Section 
            //-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PalletforceLabel;
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = null;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = null;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

            // Show the user control
            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
        }
    }
}
