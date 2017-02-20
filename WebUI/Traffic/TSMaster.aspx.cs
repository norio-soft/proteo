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
using Orchestrator;
using Orchestrator.Entities;

public partial class Traffic_TSMaster : Page
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadMenu();

        // Set the default search based on Settings
        cblSearchType.Items.FindByValue(Orchestrator.Globals.Configuration.DefaultSearch.ToLower()).Selected = true;

    }

    private void LoadMenu()
        {
            //Get the theme name if it isn't Orchestrator so that it can be used to select the menu
            string themeName = Page.Theme;
            if (themeName.Equals("Orchestrator", StringComparison.CurrentCultureIgnoreCase))
                themeName = string.Empty;

            if (!Page.IsPostBack)
                RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Default.xml", themeName));
        }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (Orchestrator.Globals.Configuration.DefaultSearch.ToLower() == "orders")
            this.cblSearchType.SelectedValue = "orders";
        else if (Orchestrator.Globals.Configuration.DefaultSearch.ToLower() == "runs")
            this.cblSearchType.SelectedValue = "runs";

        btnSearch.ServerClick += new EventHandler(btnSearch_ServerClick);
    }

    void btnSearch_ServerClick(object sender, EventArgs e)
    {
        if (cblSearchType.SelectedValue == "orders")
            Response.Redirect("/groupage/findorder.aspx?ss=" + txtSearchString.Text);
        else
            Response.Redirect("/job/jobsearch.aspx?searchString=" + txtSearchString.Text);
    }

    private string _cookieSessionID = string.Empty;
    public string CookieSessionID
    {
        get
        {
            if (string.IsNullOrEmpty(_cookieSessionID))
            {
                _cookieSessionID = Orchestrator.WebUI.Utilities.GetRandomString(6);
            }

            return _cookieSessionID;
        }
        set
        {
            _cookieSessionID = value;
        }
    }

}
