using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

namespace Orchestrator.WebUI.UserControls
{
    public partial class resource : System.Web.UI.UserControl
    {
         //Provide every page with an easy way to instaitate the DataContext
        //private EF.DataContext _DataContext = null;
        private EF.DataContext DataContext
        {
            get { return Orchestrator.EF.DataContext.Current; }
        }

        protected bool IsDriverTimeEnabled
        {
            get { return Orchestrator.Globals.Configuration.IsDriverTimeEnabled; }
        }

        #region Cookie Handling

        private string _cookieSessionID = string.Empty;
        public string CookieSessionID
        {
            get
            {
                if (String.IsNullOrEmpty(Request.QueryString["csid"]))
                {
                    _cookieSessionID = Utilities.GetRandomString(6);
                }
                else
                {
                    _cookieSessionID = Request.QueryString["csid"];
                }

                return _cookieSessionID;
            }
            set
            {
                _cookieSessionID = value;
            }
        }

        #endregion

        #region Control properties

        public int DepotID
        {
            get
            {
                Entities.TrafficSheetFilter filter = Utilities.GetFilterFromCookie(this.CookieSessionID, this.Request);
                int depotID = 0;

                if (filter != null)
                    depotID = filter.DepotId;
                // TODO : Set this to a value if ther eis nothing in the cookie.#

                if (depotID == 0)
                {
                    // we need to get the default depot for the company
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    Entities.Organisation org = facOrg.GetForIdentityId(Globals.Configuration.IdentityId);
                    foreach (Entities.OrganisationLocation ol in org.Locations)
                    {
                        if (ol.OrganisationLocationType == eOrganisationLocationType.Depot)
                        {
                            depotID = ol.OrganisationLocationId;
                            break;
                        }
                    }

                    // if the dpot id is still 0 then they have NO depot so use the head office location as this will be
                    // the default home point for the drivers etc
                    if (depotID == 0)
                    {
                        foreach (Entities.OrganisationLocation ol in org.Locations)
                        {
                            if (ol.OrganisationLocationType == eOrganisationLocationType.HeadOffice)
                            {
                                depotID = ol.OrganisationLocationId;
                                break;
                            }
                        }
                    }
                }
                return depotID;
            }
            set
            {
                this.ViewState["DepotID"] = value;
            }

        }

        public int DriverResourceID
        {
            get
            {
                int driverResourceID = 0;
                int.TryParse(cboDriver.SelectedValue, out driverResourceID);
                return driverResourceID;
            }

            set
            {

                Facade.IDriver facDriver = new Facade.Resource();
                Orchestrator.Entities.Driver driver = facDriver.GetDriverForResourceId(value);
                Telerik.Web.UI.RadComboBoxItem item = new Telerik.Web.UI.RadComboBoxItem(driver.Individual.FullName, driver.ResourceId.ToString());
                cboDriver.Items.Add(item);
                item.Selected = true;
            }
        }

        public int VehicleResourceID
        {
            get
            {
                int vehicleResourceID = 0;
                int.TryParse(cboVehicle.SelectedValue, out vehicleResourceID);
                return vehicleResourceID;
            }
            set
            {
                Facade.IVehicle facVehicle = new Facade.Resource();
                Orchestrator.Entities.Vehicle vehicle = facVehicle.GetForVehicleId(value);
                Telerik.Web.UI.RadComboBoxItem item = new Telerik.Web.UI.RadComboBoxItem(vehicle.DisplayValue, vehicle.ResourceId.ToString());
                cboVehicle.Items.Add(item);
                item.Selected = true;
            }
        }

        public int TrailerResourceID
        {
            get
            {
                int trailerResourceID = 0;
                int.TryParse(cboTrailer.SelectedValue, out trailerResourceID);
                return trailerResourceID;
            }
            set
            {
                Facade.ITrailer facTrailer = new Facade.Resource();
                Orchestrator.Entities.Trailer trailer = facTrailer.GetForTrailerId(value);
                Telerik.Web.UI.RadComboBoxItem item = new Telerik.Web.UI.RadComboBoxItem(trailer.DisplayValue, trailer.ResourceId.ToString());
                cboTrailer.Items.Add(item);
                item.Selected = true;
            }

        }

        public int PlanningCategoryID
        {
            get
        {
            int planningCategoryID = 0;
            int.TryParse(cboPlanningCategory.SelectedValue, out planningCategoryID);
            return planningCategoryID;
        }
            set
            {
                cboPlanningCategory.FindItemByValue(value.ToString()).Selected = true;
            }
        }



        public string DriverName
        {
            get
            {
                return cboDriver.SelectedItem.Text;
            }
        }

        public string TrailerRef
        {
            get
            {
                return cboTrailer.SelectedItem.Text;
            }
        }

        public string VehicleReg
        {
            get
            {
                return cboVehicle.SelectedItem.Text;
            }
        }

        public bool ShowTrailerType
        {
            get
            {
                return trTrailerType.Visible;
            }
            set
            {
                trTrailerType.Visible = value;
            }
        }

        #endregion

        #region Control Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (ShowTrailerType)
                {
                    
                    cboPlanningCategory.DataSource = this.DataContext.PlanningCategorySet.OrderBy(pc=>pc.DisplayShort);
                    cboPlanningCategory.DataBind();

                    // Add an empty value
                    cboPlanningCategory.Items.Add(new Telerik.Web.UI.RadComboBoxItem("", "0"));
                    cboPlanningCategory.Items.FindItemByValue("0").Selected = true;
                }
            }
        }
        #endregion
    }
}