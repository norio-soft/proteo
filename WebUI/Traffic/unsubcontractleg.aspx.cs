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

public partial class Traffic_unsubcontractleg : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        int instructionID = int.Parse(Request.QueryString["iID"]);
        int jobID = int.Parse(Request.QueryString["jID"]);
        Orchestrator.Facade.IJobSubContractor facJS = new Orchestrator.Facade.Job();
        facJS.UncontractInstruction(jobID, instructionID, this.Page.User.Identity.Name);
        Response.Redirect(Request.QueryString["returnURL"]);
    }
}
