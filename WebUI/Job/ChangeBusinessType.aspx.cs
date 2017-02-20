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

namespace Orchestrator.WebUI.Job
{
    /// <summary>
    /// Shamelessly copied from Job/changenominalCode.aspx
    /// </summary>
    public partial class ChangeBusinessType : Orchestrator.Base.BasePage
    {
        #region Properties

        protected int CurrentBusinessTypeID
        {
            get { return this.ViewState["_currentBusinessTypeID"] == null ? 0 : (int)this.ViewState["_currentBusinessTypeID"]; }
            set { this.ViewState["_currentBusinessTypeID"] = value; }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadJob();
                this.Page.Form.DefaultButton = this.btnCancel.UniqueID;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnOK.Click += new EventHandler(btnOK_Click);
        }

        #endregion

        #region Event Handlers

        #region Beutton Events
        void btnOK_Click(object sender, EventArgs e)
        {
            if (cboBusinessType.SelectedValue != this.CurrentBusinessTypeID.ToString())
            {
                int jobID = int.Parse(Request.QueryString["jID"]);
                Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();
                bool retVal = facJob.UpdateBusinessTypeID(jobID, int.Parse(cboBusinessType.SelectedValue), this.Page.User.Identity.Name);
                if (retVal)
                {
                    InjectScript.Text = @"<script>RefreshParentPage()</script>";
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.InjectScript.Text = @"<script>CloseOnReload()</script>";
        }

        #endregion

        #endregion

        private void LoadJob()
        {
            int jobID = int.Parse(Request.QueryString["jID"]);

            Orchestrator.Entities.Job job = null;
            Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();
            job = facJob.GetJob(jobID);

            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            Entities.BusinessType businessType = facBusinessType.GetForBusinessTypeID(job.BusinessTypeID);

            if (businessType != null)
                lblBusinessType.Text = businessType.Description;
            else
                lblBusinessType.Text = "No Business Type Set";

            DataSet dsTypes = facBusinessType.GetAll();
            cboBusinessType.DataSource = dsTypes;
            cboBusinessType.DataTextField = "Description";
            cboBusinessType.DataValueField = "BusinessTypeID";
            cboBusinessType.DataBind();

            if (businessType != null)
            {
                cboBusinessType.FindItemByValue(businessType.BusinessTypeID.ToString()).Selected = true;
                this.CurrentBusinessTypeID = businessType.BusinessTypeID;
            }
        }
    }
}