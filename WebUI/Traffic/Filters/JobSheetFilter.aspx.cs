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

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Orchestrator.WebUI.Traffic.Filters
{
    //TODO: If there is no default filter for an identity create one.
    public partial class JobSheetFilter : Orchestrator.Base.BasePage
    {
        #region Constants

        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";
        #endregion

        #region Private  Fields
        private Entities.TrafficSheetFilter m_trafficSheetFilter = null;

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.CanAccess(Orchestrator.eSystemPortion.GeneralUsage);

            if (!IsPostBack)
            {
                PopulateStaticControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboFilters.SelectedIndexChanged += new EventHandler(cboFilters_SelectedIndexChanged);

            btnSaveFilter.Click += new EventHandler(btnSaveFilter_Click);
            btnFilter.Click += new EventHandler(btnFilter_Click);
            btnClose.Click += new EventHandler(btnClose_Click);
        }

        #endregion

        #region Utility Methods

        private void BindDefaultFilter()
        {
            m_trafficSheetFilter = GetDefaultFilter();

            if (m_trafficSheetFilter != null)
                BindTrafficSheetFilter(m_trafficSheetFilter);
        }

        private void BindTrafficSheetFilter(Entities.TrafficSheetFilter filter)
        {
            txtFilterName.Text = filter.FilterName;
            SelectItem(cboFilters.Items, filter.FilterId);

            cboControlArea.ClearSelection();
            SelectItem(cboControlArea.Items, filter.ControlAreaId);

            cboTrafficAreas.ClearSelection();
            foreach (var taid in filter.TrafficAreaIDs)
            {
                SelectItem(cboTrafficAreas.Items, taid);
            }

            cboDepot.ClearSelection();
            cboDepot.Items.FindByValue(filter.DepotId.ToString()).Selected = true;

            chkOnlyShowJobsWithDemurrage.Checked = filter.OnlyShowJobsWithDemurrage;
            chkOnlyShowJobsWithDemurrageAwaitingAcceptance.Checked = filter.OnlyShowJobsWithDemurrageAwaitingAcceptance;
            chkOnlyShowJobsWithPCVs.Checked = filter.OnlyShowJobsWithPCVs;
            
            for (int i = 0; i < filter.JobStates.Count; i++)
            {
                chkJobStates.Items.FindByValue(Utilities.UnCamelCase(Enum.Parse(typeof(eJobState), filter.JobStates[i].ToString()).ToString())).Selected = true;
            }

            cblBusinessType.ClearSelection();
            foreach (int i in filter.BusinessTypes)
            {
                cblBusinessType.Items.FindByValue(i.ToString()).Selected = true;
            }

        }

        private Entities.TrafficSheetFilter GetDefaultFilter()
        {
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            Entities.TrafficSheetFilter ts =  facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);
           
            return ts;
        }

        /// <summary>
        /// Formats the filter data into an xml string and returns it to the parent window.
        /// </summary>
        /// <returns>Filter data.</returns>
        private string RecoverFilterData()
        {

            string retVal = m_trafficSheetFilter.ToXML();
            return retVal;
        }

        private void SelectItem(ListItemCollection items, string value)
        {
            ListItem selectingItem = items.FindByValue(value);
            if (selectingItem != null)
                selectingItem.Selected = true;
        }

        private void SelectItem(ListItemCollection items, int value)
        {
            SelectItem(items, value.ToString());
        }

        #endregion

      
        private void PopulateFilterList()
        {
            int identityId = ((Entities.CustomPrincipal)Page.User).IdentityId;
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            cboFilters.DataSource = facTrafficSheetFilter.GetForUser(identityId, false);
            cboFilters.DataBind();
            cboFilters.Items.Insert(0, new ListItem("-- Blank Filter --", "0"));
        }

        private void SetFilter()
        {

            m_trafficSheetFilter = Utilities.GetFilterFromCookie(this.CookieSessionID, this.Request);

            if (m_trafficSheetFilter == null)
            {
                Facade.ITrafficSheetFilter facFilter = new Facade.Traffic();
                m_trafficSheetFilter = facFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);
            }

            // Configure the filter
            m_trafficSheetFilter.ControlAreaId = Convert.ToInt32(cboControlArea.SelectedValue);
            m_trafficSheetFilter.TrafficAreaIDs.Clear();

            foreach (ListItem item in cboTrafficAreas.Items)
            {
                if (item.Selected)
                    m_trafficSheetFilter.TrafficAreaIDs.Add(Convert.ToInt32(item.Value));
            }

            m_trafficSheetFilter.FilterOutExcludedPoints = false;
            m_trafficSheetFilter.JobStates.Clear();

            foreach (ListItem item in chkJobStates.Items)
            {
                if (item.Selected)
                    m_trafficSheetFilter.JobStates.Add(Enum.Parse(typeof(eJobState), item.Value.Replace(" ", "")));
            }

            m_trafficSheetFilter.OnlyShowMyJobs = false;
            m_trafficSheetFilter.OnlyShowJobsWithDemurrage = chkOnlyShowJobsWithDemurrage.Checked;
            m_trafficSheetFilter.OnlyShowJobsWithDemurrageAwaitingAcceptance = chkOnlyShowJobsWithDemurrageAwaitingAcceptance.Checked;
            m_trafficSheetFilter.FilterStartDate = (DateTime)dteStartDate.SelectedDate;
            m_trafficSheetFilter.FilterEnddate = ((DateTime)dteEndDate.SelectedDate).Add(new TimeSpan(23, 59, 59));
            m_trafficSheetFilter.DepotId = int.Parse(cboDepot.SelectedValue);

            m_trafficSheetFilter.BusinessTypes.Clear();
            foreach (ListItem li in cblBusinessType.Items)
                if (li.Selected)
                    m_trafficSheetFilter.BusinessTypes.Add(int.Parse(li.Value));

            SetCookie(m_trafficSheetFilter);

        }

        private void SetCookie(Entities.TrafficSheetFilter ts)
        {
            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, ts);
        }
        private void PopulateStaticControls()
        {
            #region // Populate Control Areas
            Facade.IControlArea facControlArea = new Facade.Traffic();
            cboControlArea.DataSource = facControlArea.GetAll();
            cboControlArea.DataBind();
            cboControlArea.Items[0].Selected = true;
            #endregion 

            #region // Populate Traffic Areas
            Facade.ITrafficArea facTrafficArea = (Facade.ITrafficArea)facControlArea;
            cboTrafficAreas.DataSource = facTrafficArea.GetAll();
            cboTrafficAreas.DataBind();
            #endregion

            #region // Populate Depots
            Facade.IOrganisationLocation facOrganisationLocation = new Facade.Organisation();
            cboDepot.DataSource = facOrganisationLocation.GetAllDepots(Orchestrator.Globals.Configuration.IdentityId);
            cboDepot.DataBind();
            cboDepot.Items.Insert(0, new ListItem("Use Control Area and Traffic Areas to determine resource pool", "0"));
            #endregion

            #region // Populate Filters for this user
            PopulateFilterList();
            #endregion

            #region // Configure the default dates.
            // Default dates are from the start of today until:
            //   1) On a Saturday, until the end of Monday.
            //   2) On any other day, until the end of tomorrow.
            DateTime startOfToday = DateTime.Now;
            startOfToday = startOfToday.Subtract(startOfToday.TimeOfDay);
            DateTime endOfTomorrow = startOfToday.Add(new TimeSpan(1, 23, 59, 59));

            DateTime startDate = startOfToday;
            DateTime endDate = endOfTomorrow;
            if (startOfToday.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime endOfMonday = startOfToday.Add(new TimeSpan(2, 23, 59, 59));
                endDate = endOfMonday;
            }
            #endregion

            #region // Cause the job states to be displayed
            chkJobStates.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobState)));
            chkJobStates.DataBind();
            #endregion

            #region // Load the Job Types
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();

            cblBusinessType.DataSource = facBusinessType.GetAll();
            cblBusinessType.DataTextField = "Description";
            cblBusinessType.DataValueField = "BusinessTypeID";
            cblBusinessType.DataBind();
            #endregion

            #region React to supplied values

            bool haveReacted = false;

            try
            {
                Entities.TrafficSheetFilter m_trafficSheetFilter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);

                if (m_trafficSheetFilter == null)
                {
                    m_trafficSheetFilter = GetDefaultFilter();
                    m_trafficSheetFilter.FilterStartDate = startDate;
                    m_trafficSheetFilter.FilterEnddate = endDate;
                }

                cboControlArea.ClearSelection();
                SelectItem(cboControlArea.Items, m_trafficSheetFilter.ControlAreaId);
                haveReacted = true;

                foreach (var taid in m_trafficSheetFilter.TrafficAreaIDs)
                {
                    SelectItem(cboTrafficAreas.Items, taid);
                }

                for (int i = 0; i < m_trafficSheetFilter.JobStates.Count; i++)
                {
                    chkJobStates.Items.FindByValue(Utilities.UnCamelCase(Enum.Parse(typeof(eJobState), m_trafficSheetFilter.JobStates[i].ToString()).ToString())).Selected = true;
                }

                cboDepot.ClearSelection();
                cboDepot.Items.FindByValue(m_trafficSheetFilter.DepotId.ToString()).Selected = true;
                dteStartDate.SelectedDate = m_trafficSheetFilter.FilterStartDate;
                dteEndDate.SelectedDate= m_trafficSheetFilter.FilterEnddate;

              

                cblBusinessType.ClearSelection();
                foreach (int i in m_trafficSheetFilter.BusinessTypes)
                    cblBusinessType.Items.FindByValue(i.ToString()).Selected = true;
            }
            catch (Exception eX)
            {
                Entities.TrafficSheetFilter ts = GetDefaultFilter();
                ts.FilterStartDate = startDate;
                dteStartDate.SelectedDate = startDate;
                ts.FilterEnddate = endDate;
                dteEndDate.SelectedDate = endDate;
                if (Request.Cookies[this.CookieSessionID] == null)
                {
                    SetCookie(ts);
                }

                BindDefaultFilter();
            }
            finally
            {
                // Apply the dates.
                
            }
            #endregion

        }

        #region Event Handlers

        /// <summary>
        /// The user has selected one of their previously saved filters.
        /// </summary>
        void cboFilters_SelectedIndexChanged(object sender, EventArgs e)
        {
            int trafficSheetFilterId = Convert.ToInt32(cboFilters.SelectedValue);

            if (trafficSheetFilterId == 0)
            {
                // This is the "blank filter" setting.
                cboControlArea.ClearSelection();
                cboTrafficAreas.ClearSelection();
                cboDepot.SelectedIndex = 0;
                txtFilterName.Text = string.Empty;
                cblBusinessType.ClearSelection();
            }
            else
            {
                // Load the filter and configure the textbox to allow saving a new version
                Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
                Entities.TrafficSheetFilter selectedFilter = facTrafficSheetFilter.GetForTrafficSheetFilterId(trafficSheetFilterId);

                m_trafficSheetFilter = selectedFilter;
                BindTrafficSheetFilter(selectedFilter);

            }
        }

        /// <summary>
        /// The user wishes to apply the filter configuration.
        /// </summary>
        void btnFilter_Click(object sender, EventArgs e)
        {
            btnFilter.DisableServerSideValidation();

            if (Page.IsValid)
            {
                SetFilter();

                this.ReturnValue = "refresh";
                this.Close();
            }
        }

        /// <summary>
        /// The users wishes to save the filter.
        /// </summary>
        void btnSaveFilter_Click(object sender, EventArgs e)
        {
            btnSaveFilter.DisableServerSideValidation();

            if (Page.IsValid)
            {
                Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
                Entities.CustomPrincipal user = (Entities.CustomPrincipal) Page.User;
                Entities.TrafficSheetFilter filter = null;
                bool isUpdate = false;
                if (dteStartDate.SelectedDate != null && dteEndDate.SelectedDate == null)
                    dteEndDate.SelectedDate = ((DateTime)dteStartDate.SelectedDate).AddDays(1).Add(new TimeSpan(23, 59, 59));

                if (cboFilters.SelectedValue != "0" && cboFilters.SelectedItem.Text == txtFilterName.Text)
                {
                    // We are going to update the old version of the filter.
                    isUpdate = true;
                    filter = facTrafficSheetFilter.GetForTrafficSheetFilterId(Convert.ToInt32(cboFilters.SelectedValue));
                }
                else
                {
                    filter = new Entities.TrafficSheetFilter(user.IdentityId);
                    filter.FilterName = txtFilterName.Text;
                }
                filter.FilterStartDate = (DateTime) dteStartDate.SelectedDate;
                filter.FilterEnddate = (DateTime)dteEndDate.SelectedDate;

                // Configure the filter
                filter.ControlAreaId = Convert.ToInt32(cboControlArea.SelectedValue);
                filter.TrafficAreaIDs.Clear();

                foreach (ListItem item in cboTrafficAreas.Items)
                {
                    if (item.Selected)
                        filter.TrafficAreaIDs.Add(Convert.ToInt32(item.Value));
                }

                filter.FilterOutExcludedPoints = false;
                filter.TrafficSheetGrouping = eTrafficSheetGrouping.None;
                filter.TrafficSheetSorting = eTrafficSheetSorting.EarliestLeg;
                filter.JobStates.Clear();

                foreach (ListItem item in chkJobStates.Items)
                    if (item.Selected)
                        filter.JobStates.Add(Enum.Parse(typeof(eJobState), item.Value.Replace(" ", "")));

                filter.BusinessTypes.Clear();
                foreach (ListItem li in cblBusinessType.Items)
                    if (li.Selected)
                        filter.BusinessTypes.Add((int)int.Parse(li.Value));

                filter.DepotId = int.Parse(cboDepot.SelectedValue);
                
                filter.OnlyShowJobsWithDemurrage = chkOnlyShowJobsWithDemurrage.Checked;
                filter.OnlyShowJobsWithDemurrageAwaitingAcceptance = chkOnlyShowJobsWithDemurrageAwaitingAcceptance.Checked;
                filter.OnlyShowJobsWithPCVs = chkOnlyShowJobsWithPCVs.Checked;
                filter.OnlyShowMyJobs = false;

                Facade.ITrafficArea facTrafficArea = new Facade.Traffic();

                if (isUpdate)
                    ((Facade.ITrafficSheetFilter)facTrafficArea).Update(filter, user.UserName);
                else
                    filter.FilterId = ((Facade.ITrafficSheetFilter)facTrafficArea).Create(filter, user.UserName);

                SetFilter();
                PopulateFilterList();
                cboFilters.ClearSelection();
                SelectItem(cboFilters.Items, filter.FilterId);

                this.ReturnValue = "refresh";
                this.Close();

                
            }
        }

        /// <summary>
        /// The user wishes to close the window without making any filter changes.
        /// </summary>
        void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
    }
}