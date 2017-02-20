using Orchestrator.Repositories;
using Orchestrator.Repositories.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Reports
{
    public partial class AgencyDriver : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            grdAgencyDriver.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdAgencyDriver_NeedDataSource);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnExport.Click += new EventHandler(btnExport_Click);

        }

        private void PopulateStaticControls()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var organisationsRepo = DIContainer.CreateRepository<IOrganisationRepository>(uow);
                var depots = organisationsRepo.GetOrganisationDepots();
                foreach(var depot in depots)
                {
                    var rcItem = new Telerik.Web.UI.RadComboBoxItem();
                    rcItem.Text = depot.OrganisationLocationName;
                    rcItem.Value = depot.OrganisationLocationID.ToString();
                    cboDepot.Items.Add(rcItem);
                }
            }

            dteStartDate.SelectedDate = Utilities.StartOfWeek(DateTime.Now, DayOfWeek.Monday);
            dteEndDate.SelectedDate = dteStartDate.SelectedDate.Value.AddDays(7);
        }
        private IEnumerable<DriverAgencyReportRow> GetData()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var driverRepo = DIContainer.CreateRepository<IDriverRepository>(uow);
                var fromDate = dteStartDate.SelectedDate;
                var toDate = dteEndDate.SelectedDate;
                int? depotId = null;
                if(!chkAllDepots.Checked)
                    depotId = int.Parse(cboDepot.SelectedValue);
                var reportRows = driverRepo.GetNumberOfJobsResourcedToAgencyDrivers(fromDate.Value,toDate.Value,depotId);

                return reportRows.ToList();
            }
        }

        private void grdAgencyDriver_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var data = GetData();
            grdAgencyDriver.DataSource = data;
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if(Page.IsValid)
            {
                grdAgencyDriver.Rebind();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            if(Page.IsValid)
            {
                // Clear anything that might have been written by the aspx page.
                Response.ClearContent();
                Response.ClearHeaders();

                string rangeText = string.Format("{0} to {1}", dteStartDate.SelectedDate.Value.ToString("d"), dteEndDate.SelectedDate.Value.ToString("d"));

                // Add the appropriate headers
                string filename = string.Format("Agency_Driver_Report_{0}.csv", rangeText).Replace('/', '-');
                Response.AddHeader("content-disposition", string.Format("attachement; filename={0}", filename));
                // Add the right content type
                Response.ContentType = "application/msexcel";

                var data = GetData();
                string csv = "";

                    csv += GetCSVHeader() + "\n";
                    foreach(var darr in data)
                    {
                       csv += GetCSVRow(darr) +"\n";
                    }
                    Response.Write(csv);


                Response.Flush();
                Response.End();
            }
        }

        private string GetCSVHeader()
        {
            return "Agency,Driver Name,Payroll No,Depot,Number of Jobs";
        }

        private string GetCSVRow(DriverAgencyReportRow darr)
        {
            return "\""+ darr.AgencyName + "\",\"" + darr.FullName + "\",\"" + darr.PayrollNo + "\",\"" + darr.DepotName + "\"," + darr.JobCount;
        }
    }
}