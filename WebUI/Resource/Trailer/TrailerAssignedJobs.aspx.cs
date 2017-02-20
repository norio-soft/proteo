using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.Trailer
{

    //----------------------------------------------------------------------------

    public partial class TrailerAssignedJobs : Orchestrator.Base.BasePage
    {

        //----------------------------------------------------------------------------

        private IList<string> jobIds = new List<string>();

        //----------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.PopulateTrailers();

            this.jobIds.Clear();
        }

        //----------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rgJobsAssigned.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(rgJobsAssigned_NeedDataSource);
            this.btnRefreshBottom.Click += new EventHandler(btnRefresh_Click);
            this.btnRefreshTop.Click += new EventHandler(btnRefresh_Click);
            this.rgJobsAssigned.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(rgJobsAssigned_ItemDataBound);
            this.cboTrailer.SelectedIndexChanged += new EventHandler(cboTrailer_SelectedIndexChanged);
        }

        //----------------------------------------------------------------------------

        protected void cboTrailer_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.jobIds.Clear();
            this.rgJobsAssigned.Rebind();
        }

        //----------------------------------------------------------------------------

        private void PopulateTrailers()
        {
            Facade.ITrailer facTrailers = new Facade.Resource();
            DataSet dsTrailers = facTrailers.GetAll(false, false);
            cboTrailer.DataSource = dsTrailers;
            cboTrailer.DataTextField = "TrailerRef";
            cboTrailer.DataValueField = "ResourceId";
            cboTrailer.DataBind();
            cboTrailer.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "-1"));

            if (Request.QueryString["resourceId"] != null)
                this.cboTrailer.SelectedValue = Request.QueryString["resourceId"].ToString();
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
            Facade.ITrailer facTrailer = new Facade.Resource();
            DataSet jobsAssigned = null;

            if (!String.IsNullOrEmpty(this.cboTrailer.SelectedValue))
                jobsAssigned = facTrailer.GetJobsAssignedForTrailer(int.Parse(this.cboTrailer.SelectedValue));

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
