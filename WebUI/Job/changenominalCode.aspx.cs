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

public partial class Job_changenominalCode : Orchestrator.Base.BasePage
{

    #region Properties
    protected int CurrentNominalCodeID
    {
        get { return this.ViewState["_currentNominalCodeID"] == null ? 0 : (int)this.ViewState["_currentNominalCodeID"]; }
        set { this.ViewState["_currentNominalCodeID"] = value; }
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
        if (cboNominalCode.SelectedValue != this.CurrentNominalCodeID.ToString())
        {
            int jobID = int.Parse(Request.QueryString["jID"]);
            Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();
            bool retVal = facJob.UpdateNominalCode(jobID, int.Parse(cboNominalCode.SelectedValue), this.Page.User.Identity.Name);
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

        if (job.NominalCode != null)
            lblNominalCode.Text = job.NominalCode.ShortCode;
        else
            lblNominalCode.Text = "No Nominal Code Set";

        Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
        DataSet dsCodes = facNominalCode.GetAllActive();
        cboNominalCode.DataSource = dsCodes;
        cboNominalCode.DataTextField = "NominalCode";
        cboNominalCode.DataValueField="NominalCodeID";
        cboNominalCode.DataBind();

        if (job.NominalCode != null)
        {
            cboNominalCode.FindItemByValue(job.NominalCode.NominalCodeID.ToString()).Selected = true;
            this.CurrentNominalCodeID = job.NominalCode.NominalCodeID;
        }

    }
}
