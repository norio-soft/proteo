using System;
using System.Data;
using System.Web.UI.WebControls.WebParts;


public partial class Default2 :Orchestrator.Base.BasePage
{

    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        if (IsClientUser)
            Page.MasterPageFile = "default_tableless_client.Master";
        else if (IsSubConUser)
            Page.MasterPageFile = "default_tableless_SubCon.Master";
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (IsClientUser)
        {
            pnlWebParts.Visible = false;
            pnlClient.Visible = true;
            pnlSubCon.Visible = false;
        }
        else if (IsSubConUser)
        {
            pnlWebParts.Visible = false;
            pnlClient.Visible = false;
            pnlSubCon.Visible = true;
        }
        else
        {
            pnlWebParts.Visible = true;
            pnlClient.Visible = false;
            pnlSubCon.Visible = false;
        }

        if (pnlClient.Visible)
        {
            // remove all the contols in the normal view to prevent them running and therefore save time;
            pnlWebParts.Controls.Clear();

            DateTime endDate = DateTime.Today.AddMonths(-1);
            endDate = new DateTime(endDate.Year, endDate.Month, System.Globalization.CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(endDate.Year, endDate.Month));
            lblUnbilledEndDate.Text = endDate.ToString("MMM yy");

            Orchestrator.Facade.User facUser = new Orchestrator.Facade.User();
            DataSet dsUser = facUser.GetOrganisationForUser(((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);

            int clientIdentityID = (int)dsUser.Tables[0].Rows[0]["RelatedIdentityID"];

            slOrdersForWeek.InitParameters = slOrdersForWeek.InitParameters + ",ClientIdentityID=" + clientIdentityID.ToString();
            slInvoicedRevenue.InitParameters += ",ClientIdentityID=" + clientIdentityID.ToString();
            slOutstandingPODs.InitParameters += ",ClientIdentityID=" + clientIdentityID.ToString();
            slUninvoicedWork.InitParameters += ",ClientIdentityID=" + clientIdentityID.ToString();
        }

        // check to see if the user has got FleetMetrk installed and active.


        if (!Orchestrator.Globals.Configuration.ShowFleetMetrik)
        {
            radTabs.Tabs[1].NavigateUrl = "nofleetmetrik.aspx";
        }

        // If this is FleetMetrik then we do not want to show the first Tab at all
        if (Page.Theme == "FleetMetrik" || Orchestrator.Globals.Configuration.FleetMetrikInstance)
        {
            Server.Transfer("fmwebparts2.aspx");
        }

    }

    protected void wpmManager_AuthorizeWebPart(object sender, WebPartAuthorizationEventArgs e)
    {
        // Hide the Unallocated Orders web part if allocation is not enabled
        if (e.Path.EndsWith("wpUnallocatedOrders.ascx", StringComparison.CurrentCultureIgnoreCase))
            e.IsAuthorized = Orchestrator.WebUI.Utilities.IsAllocationEnabled();
    }

}