using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.Reports
{
    public partial class NichollsSubContractorExport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnExport.Click += new EventHandler(btnExport_Click);
        }


        void btnExport_Click(object sender, EventArgs e)
        {
            DateTime startDate = rdiStartDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);

            DateTime endDate = rdiEndDate.SelectedDate.Value;
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));
            
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
            DataSet dsSubbyExport = facJobSubContractor.GetSubbyReportWithLegRate(startDate, endDate);

            if (dsSubbyExport != null && dsSubbyExport.Tables.Count > 0)
            {
                Session["__ExportDS"] = dsSubbyExport.Tables[0];
                Response.Redirect("../Reports/csvexport.aspx?filename=SubContractorReport.csv");
            }
        }
    }
}