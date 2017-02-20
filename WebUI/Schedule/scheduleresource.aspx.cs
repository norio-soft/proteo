using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;


namespace Orchestrator.WebUI.Schedule
{
    public partial class scheduleresourcenew : System.Web.UI.Page
    {

        protected bool showDriverTypes = true;
        protected bool showDepots = true;


        protected override void EnsureChildControls()
        {
             this.RadScheduler1.AppointmentComparer = new ProteoAppointmentComparer();

            base.EnsureChildControls();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            // Set the current user id as the default value for parameters
            SchedulerDataSource.UpdateParameters["UserId"].DefaultValue = SchedulerDataSource.InsertParameters["UserId"].DefaultValue = Page.User.Identity.Name;
            if (!IsPostBack)
            {
                PopulateFilter();
            }

            setUpFilters();

            this.RadScheduler1.AppointmentComparer = new ProteoAppointmentComparer();


        }

        private void PopulateFilter()
        {
            Facade.Resource facResource = new Facade.Resource();
            Facade.IOrganisationLocation facOrganiastionLocation = new Facade.Organisation();

            //resource types
            var resourceTypes = facResource.GetAllResourceTypes();
            cboResourceTypes.DataSource = resourceTypes.Tables[0];
            cboResourceTypes.DataTextField = "Description";
            cboResourceTypes.DataValueField = "ResourceTypeId";
            cboResourceTypes.DataBind();

            string driverTypeName = Enum.GetName(typeof(eResourceType), eResourceType.Driver);
            cboResourceTypes.Items.FindByText(driverTypeName).Selected = true;

            //driver types
            var driverTypes = facResource.GetAllDriverTypes();
            cboDriverTypes.DataSource = driverTypes.Tables[0];
            cboDriverTypes.DataTextField = "Description";
            cboDriverTypes.DataValueField = "DriverTypeID";
            cboDriverTypes.DataBind();
            cboDriverTypes.Items.Insert(0, new ListItem("-- all --", "0"));

            //depots
            cboDepots.DataSource = facOrganiastionLocation.GetAllDepots(Orchestrator.Globals.Configuration.IdentityId);
            cboDepots.DataValueField = "OrganisationLocationId";
            cboDepots.DataTextField = "OrganisationLocationName";
            cboDepots.DataBind();
            cboDepots.Items.Insert(0, new ListItem("-- all --", "0"));
        }

        private void setUpFilters()
        {
            string driverTypeName = Enum.GetName(typeof(eResourceType), eResourceType.Driver);
            if (cboResourceTypes.SelectedItem != null && cboResourceTypes.SelectedItem.Text.ToLower() != driverTypeName.ToLower())
            {
                showDriverTypes = false;
                cboDriverTypes.ClearSelection();
                cboDriverTypes.Items.FindByValue("0").Selected = true;
            }


            string trailerTypeName = Enum.GetName(typeof(eResourceType), eResourceType.Trailer);

            if (cboResourceTypes.SelectedItem != null && cboResourceTypes.SelectedItem.Text.ToLower() == trailerTypeName.ToLower())
            {
                showDepots = false;
                cboDepots.ClearSelection();
                cboDepots.Items.FindByValue("0").Selected = true;
            }
        }
    }
}

