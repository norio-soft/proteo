using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Diagnostics;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;
using Orchestrator.Entities;

public partial class Resource_managedrivertype : Orchestrator.Base.BasePage
{
    int _driverTypeID = -1;

    private Orchestrator.Entities.DriverType _DriverType;

    public Orchestrator.Entities.DriverType VS_DriverType
    {
        get { return (Orchestrator.Entities.DriverType)this.ViewState["_DriverType"]; }
        set { this.ViewState["_DriverType"] = value; }
    }
	

    protected void Page_Load(object sender, EventArgs e)
    {
       
        if (!IsPostBack)
        {
            if (Request.QueryString["dt"] != null)
            {
                if (int.TryParse(Request.QueryString["dt"], out _driverTypeID))
                    LoadDriverType();
            }

        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.btnOK.Click += new EventHandler(btnOK_Click);
    }

    void btnOK_Click(object sender, EventArgs e)
    {
        Orchestrator.Entities.DriverType dt = null;
        if (this.VS_DriverType != null)
            dt = this.VS_DriverType;
        else
            dt = new DriverType();

        dt.Description = txtDescription.Text;
        dt.Monday = chkMonday.Checked;
        dt.Tuesday = chkTuesday.Checked;
        dt.Wednesday = chkWednesday.Checked;
        dt.Thursday = chkThursday.Checked;
        dt.Friday = chkFriday.Checked;
        dt.Saturday = chkSaturday.Checked;
        dt.Sunday = chkSunday.Checked;
        dt.StartTime = rtpStartTime.SelectedDate.Value;
        dt.FinishTime = rtpFinishTime.SelectedDate.Value;

        Orchestrator.Facade.IDriver facDriver = new Orchestrator.Facade.Resource();
        int retVal = facDriver.UpdateDriverType(dt, this.Page.User.Identity.Name);

        if (retVal > 0)
        {
            InjectScript.Text = @"<script>RefreshParentPage();</script>";
            return;
        }

        lblError.Visible = true;
        lblError.Text = "There was an error please try again.";

    }

    private void LoadDriverType()
    {
        Orchestrator.Facade.IDriver facDriver = new Orchestrator.Facade.Resource();
        Orchestrator.Entities.DriverType dt = facDriver.GetDriverType(_driverTypeID);

        this.VS_DriverType = dt;
        txtDescription.Text = dt.Description;
        chkMonday.Checked = dt.Monday;
        chkTuesday.Checked = dt.Tuesday;
        chkWednesday.Checked = dt.Wednesday;
        chkThursday.Checked = dt.Thursday;
        chkFriday.Checked = dt.Friday;
        chkSaturday.Checked = dt.Saturday;
        chkSunday.Checked = dt.Sunday;
        rtpStartTime.SelectedDate = dt.StartTime == DateTime.MinValue ? DateTime.Now.Date : dt.StartTime;
        rtpFinishTime.SelectedDate = dt.FinishTime == DateTime.MinValue ? DateTime.Now.Date : dt.FinishTime;
    }
}
