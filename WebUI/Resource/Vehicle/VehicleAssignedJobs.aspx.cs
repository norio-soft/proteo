using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.Vehicle
{

    //----------------------------------------------------------------------------

    public partial class VehicleAssignedJobs : Orchestrator.Base.BasePage
    {

        //----------------------------------------------------------------------------

        private IList<string> jobIds = new List<string>();

        //----------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
                this.PopulateVehicles();
        }

        //----------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rgJobsAssigned.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(rgJobsAssigned_NeedDataSource);
            this.btnRefreshBottom.Click += new EventHandler(btnRefresh_Click);
            this.btnRefreshTop.Click += new EventHandler(btnRefresh_Click);
            this.rgJobsAssigned.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(rgJobsAssigned_ItemDataBound);
            this.cboVehicle.SelectedIndexChanged += new EventHandler(cboVehicle_SelectedIndexChanged);
        }

        //----------------------------------------------------------------------------

        protected void cboVehicle_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.jobIds.Clear();
            this.rgJobsAssigned.Rebind();
        }

        //----------------------------------------------------------------------------

        private void PopulateVehicles()
        {
            Facade.IVehicle facVehicle = new Facade.Resource();
            DataSet dsVehicles = facVehicle.GetAllVehicles();
            cboVehicle.DataSource = dsVehicles;
            cboVehicle.DataTextField = "RegNo";
            cboVehicle.DataValueField = "ResourceId";
            cboVehicle.DataBind();
            cboVehicle.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "-1"));

            if (Request.QueryString["resourceId"] != null)
                cboVehicle.SelectedValue = Request.QueryString["resourceId"].ToString();
        }

        //----------------------------------------------------------------------------

        public void rgJobsAssigned_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView dataRow = (DataRowView)e.Item.DataItem;
                if (!this.jobIds.Contains(dataRow["JobId"].ToString()))
                {
                    HtmlAnchor job = (HtmlAnchor)e.Item.FindControl("lnkJob");
                    job.HRef = "javascript:ViewJob('" + dataRow["JobId"].ToString() + "');";
                    job.InnerText = dataRow["JobId"].ToString();

                    this.jobIds.Add(dataRow["JobId"].ToString());
                }
            }
        }

        //----------------------------------------------------------------------------

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            this.jobIds.Clear();
            this.rgJobsAssigned.Rebind();
        }

        //----------------------------------------------------------------------------

        public void rgJobsAssigned_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.rgJobsAssigned.DataSource = this.GetJobsAssigned();
        }

        //----------------------------------------------------------------------------

        public DataSet GetJobsAssigned()
        {
            Facade.IVehicle facVehicle = new Facade.Resource();
            DataSet jobsAssigned = null;

            if (String.IsNullOrEmpty(cboVehicle.SelectedValue) || cboVehicle.SelectedValue != "-1")
                jobsAssigned = facVehicle.GetJobsAssignedForVehicle(int.Parse(cboVehicle.SelectedValue));

            return jobsAssigned;
        }

        //----------------------------------------------------------------------------

        public string GetInstructionTypeImage(int instructionTypeID)
        {
            if (instructionTypeID == 1 || instructionTypeID == 5)
                return "loadfinal.png";
            else if (instructionTypeID == 7)
                return "trunk.gif";
            else if (instructionTypeID == 8)
                return "dropturnedaway.png";
            else
                return "dropfinal.png";
        }

        //----------------------------------------------------------------------------

    }

    //----------------------------------------------------------------------------

}
