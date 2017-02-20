using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.Reports
{
	public partial class CdrReport : System.Web.UI.Page
	{
        protected void Page_Load(object sender, EventArgs e)
        { }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnExport.Click += new EventHandler(btnExport_Click);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int clientId = 0;
            int.TryParse(this.cboClient.SelectedValue, out clientId);

            Facade.GenericReport facGenReport = new Orchestrator.Facade.GenericReport();
            DataSet ds = facGenReport.GenericReport_CdrReportInSpreadsheet(clientId, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value, Page.User.Identity.Name);

            Session["__ExportDS"] = ds.Tables[0];
            Response.Redirect("../reports/csvexport.aspx?filename=CdrReport.CSV");
        }
	}
}