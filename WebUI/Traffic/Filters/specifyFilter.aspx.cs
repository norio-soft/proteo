using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Linq;
using Orchestrator.Models;

namespace Orchestrator.WebUI.Traffic.Filters
{

    public partial class specifyFilter : Orchestrator.Base.BasePage
    {

        private readonly IDictionary<Entities.TrafficSheetFilter.eInProgressSubState, string> InProgressSubStateDescriptions = new Dictionary<Entities.TrafficSheetFilter.eInProgressSubState, string>
        {
            { Entities.TrafficSheetFilter.eInProgressSubState.NonMWF, "Non MWF" },
            { Entities.TrafficSheetFilter.eInProgressSubState.MWFCommunicated, "MWF Communicated" },
            { Entities.TrafficSheetFilter.eInProgressSubState.MWFStarted, "MWF Started" },
        };

        private Entities.TrafficSheetFilter Filter
        {
            get { return (Entities.TrafficSheetFilter)this.ViewState["_filter"]; }
            set { this.ViewState["_filter"] = value; }
        }

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.CanAccess(eSystemPortion.GeneralUsage);

            if (!IsPostBack)
            {
                PopulateStaticControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboFilters.SelectedIndexChanged += new EventHandler(cboFilters_SelectedIndexChanged);
            cblInstructionStates.DataBound += cblInstructionStates_DataBound;

            btnSaveFilter.Click += new EventHandler(btnSaveFilter_Click);
            btnFilter.Click += new EventHandler(btnFilter_Click);
            btnClose.Click += new EventHandler(btnClose_Click);
        }

        #endregion

        #region Utility Methods

        private void BindDefaultFilter()
        {
            Entities.TrafficSheetFilter defaultFilter = GetDefaultFilter();

            if (defaultFilter != null)
                BindTrafficSheetFilter(defaultFilter);
        }

        private void BindTrafficSheetFilter(Entities.TrafficSheetFilter filter)
        {
            txtFilterName.Text = filter.FilterName;
            SelectItem(cboFilters.Items, filter.FilterId);

            cboControlArea.ClearSelection();
            SelectItem(cboControlArea.Items, filter.ControlAreaId);

            cboTrafficAreas.ClearSelection();
            foreach (var trafficAreaID in filter.TrafficAreaIDs)
                SelectItem(cboTrafficAreas.Items, trafficAreaID);

            cboDepot.ClearSelection();
            cboDepot.Items.FindByValue(filter.DepotId.ToString()).Selected = true;

            chkShowStockMovementJobs.Checked = filter.ShowStockMovementJobs;
            chkShowPlanned.Checked = filter.ShowPlannedLegs ;

            cblBusinessType.ClearSelection();
            foreach (int i in filter.BusinessTypes)
            {
                cblBusinessType.Items.FindByValue(i.ToString()).Selected = true;
            }

            cblInstructionStates.ClearSelection();
            foreach (int i in filter.InstructionStates)
            {
                cblInstructionStates.Items.FindByValue(((eInstructionState)i).ToString()).Selected = true;
            }

            cblInProgressSubStates.ClearSelection();
            if (!filter.InProgressSubStates.Any())
            {
                foreach (ListItem i in cblInProgressSubStates.Items)
                {
                    i.Selected = true;
                }
            }
            else
            {
                foreach (int i in filter.InProgressSubStates)
                {
                    cblInProgressSubStates.Items.FindByValue(((int)i).ToString()).Selected = true;
                }
            }

            chkSortbyEvent.Checked = filter.SortbyEvent;

            // MWF Comms States
            cblMWFCommsStates.ClearSelection();
            var states = MWF_Instruction.GetTrafficSheetStatesFromCommunicationStatesFilter(filter.MWFCommsStates);
            foreach (int i in states)
            {
                cblMWFCommsStates.Items.FindByValue(i.ToString()).Selected = true;
            }

            // MWF Instruction States
            cblMWFInstructionStates.ClearSelection();
            foreach (int i in filter.MWFInstructionStates)
            {
                cblMWFInstructionStates.Items.FindByValue(i.ToString()).Selected = true;
            }
        }

        private Entities.TrafficSheetFilter GetDefaultFilter()
        {
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            return facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);
        }

        private void SetCookie()
        {
            if (Filter == null)
                Filter = new Entities.TrafficSheetFilter();

            Filter  = this.PopulateFilter(Filter);

            Utilities.SetTrafficSheetCookie(this.CookieSessionID, Response, Filter);
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

        private void PopulateStaticControls()
        {
            #region // Configure the min and max dates.
            dteStartDate.MinDate = new DateTime(2000, 1, 1);
            dteEndDate.MaxDate  = new DateTime(2036, 12, 31);
            #endregion

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

            if (cboTrafficAreas.Items.Count > 8)
            {
                divTrafficAreas.Attributes.Add("class", "overflowHandler");
                divTrafficAreas.Attributes.Add("style", "height:100px;");
            }
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

            #region // Populate the Business Types
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            cblBusinessType.DataSource = facBusinessType.GetAll();
            cblBusinessType.DataTextField = "Description";
            cblBusinessType.DataValueField = "BusinessTypeID";
            cblBusinessType.DataBind();
            #endregion

            #region // Populate the Instruction States
            
            cblInstructionStates.DataSource = Enum.GetNames(typeof(eInstructionState));
            cblInstructionStates.DataBind();
            
            #endregion

            #region // Populate the In Progress Sub-States

            cblInProgressSubStates.DataSource = this.InProgressSubStateDescriptions.ToDictionary(kvp => (int)kvp.Key, kvp => kvp.Value);
            cblInProgressSubStates.DataBind();

            #endregion

            #region // Populate the Planning Categories Types

            cblPlanningCategory.DataSource = DataContext.PlanningCategorySet.OrderBy(pc => pc.DisplayShort);
            cblPlanningCategory.DataBind();
            var liAll = new ListItem("All", "-1") {Selected = true};

            cblPlanningCategory.Items.Add(liAll);
            
            #endregion

            #region // Populate the Instruction Types
            
            cblInstructionType.Items.Clear();
            
            var liMWF = new ListItem("MWF", "1");
            liMWF.Attributes.Add("onclick", "onInstructionTypeChecked();");
            var liNonMWF = new ListItem("Non-MWF", "2");
            liNonMWF.Attributes.Add("onclick", "onInstructionTypeChecked();");

            cblInstructionType.Items.Add(liMWF);
            cblInstructionType.Items.Add(liNonMWF);

            #endregion

            #region // Populate the MWF Comms Status

            var list = Enum.GetValues(typeof (MWFCommunicationStatusForTrafficSheetEnum)).Cast<object>().ToDictionary(v => (int) v, v => MWF_Instruction.GetCommunicationStatusForTrafficSheetDescription((int) v));

            cblMWFCommsStates.DataSource = list;
            cblMWFCommsStates.DataBind();
            
            #endregion

            #region // Populate the MWF Instruction States

            // Only adding the states that we use for now
            var list2 = new Dictionary<int, string>
                {
                    {(int) MWFStatusEnum.Drive, "Drive"},
                    {(int) MWFStatusEnum.OnSite, "On Site"},
                    {(int) MWFStatusEnum.Suspend, "Suspend"},
                    {(int) MWFStatusEnum.Resume, "Resume"},
                    {(int) MWFStatusEnum.Complete, "Complete"}
                };

            cblMWFInstructionStates.DataSource = list2;
            cblMWFInstructionStates.DataBind();

            #endregion

            if (Filter == null)
                Filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);



            if (Filter == null)
            {
                Filter = GetDefaultFilter();

                // Configure the default dates.
                // Default dates are from the start of today until:
                //   1) On a Saturday, until the end of Monday.
                //   2) On any other day, until the end of tomorrow.
                Filter.FilterStartDate = startOfToday;
                if (startOfToday.DayOfWeek == DayOfWeek.Saturday)
                {
                    DateTime endOfMonday = startOfToday.Add(new TimeSpan(2, 23, 59, 59));
                    Filter.FilterEnddate = endOfMonday;
                }
                else
                    Filter.FilterEnddate = endOfTomorrow;
            }


                cboControlArea.ClearSelection();
                SelectItem(cboControlArea.Items, Filter.ControlAreaId);

                foreach (var taid in Filter.TrafficAreaIDs)
                    SelectItem(cboTrafficAreas.Items, taid);

                startDate = Filter.FilterStartDate;
                endDate = Filter.FilterEnddate;
                chkShowPlanned.Checked = Filter.ShowPlannedLegs;
                chkShowStockMovementJobs.Checked = Filter.ShowStockMovementJobs;

                cboDepot.ClearSelection();
                cboDepot.Items.FindByValue(Filter.DepotId.ToString()).Selected = true;

                foreach (var i in Filter.BusinessTypes)
                {
                    try
                    {
                        // P1 use if we try to work on different client's version of Orchestrator
                        // the same cookie is used (domain).
                        cblBusinessType.Items.FindByValue(i.ToString()).Selected = true;
                    }
                    catch { }
                }

                foreach (int i in this.Filter.InstructionStates)
                {
                    cblInstructionStates.Items.FindByValue(((eInstructionState)i).ToString()).Selected = true;
                }

                foreach (var ipss in this.Filter.InProgressSubStates)
                {
                    cblInProgressSubStates.Items.FindByValue(((int)ipss).ToString()).Selected = true;
                }

                if (this.Filter.PlanningCategories.Count > 0)
                {
                    foreach (ListItem li in cblPlanningCategory.Items)
                    {
                        li.Selected = this.Filter.PlanningCategories.Contains(int.Parse(li.Value));
                    }
                }

                this.chkSortbyEvent.Checked = Filter.SortbyEvent;
                this.chkCollapseRuns.Checked = this.Filter.CollapseRuns;

                liMWF.Selected = Filter.IncludeMWFInstructions;
                liNonMWF.Selected = Filter.IncludeNonMWFInstructions;

                chkSelectAllInstructionTypes.Checked = (liMWF.Selected && liNonMWF.Selected);

                bool allSelected = false;
                if (Filter.MWFCommsStates.Count > 0)
                {
                    allSelected = true;
                    foreach (ListItem li in cblMWFCommsStates.Items)
                    {
                        li.Selected = Filter.MWFCommsStates.Contains(int.Parse(li.Value));
                        if (!li.Selected)
                            allSelected = false;
                    }
                }

                chkSelectAllMWFCommsStates.Checked = allSelected;


                var selected = false;
                if (Filter.MWFInstructionStates.Count > 0)
                {
                    selected = true;
                    foreach (ListItem li in cblMWFInstructionStates.Items)
                    {
                        li.Selected = Filter.MWFInstructionStates.Contains(int.Parse(li.Value));
                        if (!li.Selected)
                            selected = false;
                    }
                }

                chkSelectAllMWFInstructionStates.Checked = selected;
                

                #region // Apply the dates.
                dteStartDate.SelectedDate = startDate;
                dteEndDate.SelectedDate = endDate;
                #endregion

        }

        private string GetCommunicationStatusImageFor(int value)
        {
            switch (value)
            {
                case (int)MWFCommunicationStatusForTrafficSheetEnum.NotSent:
                    return "exclamation.png";
                case (int)MWFCommunicationStatusForTrafficSheetEnum.Sent:
                    return "tlRed.gif";
                case (int)MWFCommunicationStatusForTrafficSheetEnum.ReceivedOnDevice:
                    return "tlAmber.gif";
                case (int)MWFCommunicationStatusForTrafficSheetEnum.DriverAcknowledged:
                    return "tlGreen.gif";
                default:
                    return "";
            }
        }

        private Entities.TrafficSheetFilter PopulateFilter(Entities.TrafficSheetFilter _filter)
        {
            Entities.TrafficSheetFilter filter = null;

            if (_filter == null)
                filter = new Entities.TrafficSheetFilter();
            else
                filter = _filter;

            filter.FilterStartDate = dteStartDate.SelectedDate.Value;
            filter.FilterEnddate = dteEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));
            filter.ShowPlannedLegs = chkShowPlanned.Checked;
            filter.ShowStockMovementJobs = chkShowStockMovementJobs.Checked;
            filter.DepotId = int.Parse(cboDepot.SelectedValue);
            filter.ControlAreaId = int.Parse(cboControlArea.SelectedValue);
            filter.SortbyEvent = chkSortbyEvent.Checked;
            filter.CollapseRuns = chkCollapseRuns.Checked;

            filter.TrafficAreaIDs.Clear();
            foreach (ListItem li in cboTrafficAreas.Items)
            {
                if (li.Selected)
                    filter.TrafficAreaIDs.Add(int.Parse(li.Value));
            }

            filter.BusinessTypes.Clear();
            foreach (ListItem li in cblBusinessType.Items)
            {
                if (li.Selected)
                    filter.BusinessTypes.Add(int.Parse(li.Value));
            }

            filter.InstructionStates.Clear();
            foreach (ListItem li in cblInstructionStates.Items)
            {
                if (li.Selected)
                    filter.InstructionStates.Add((int)Enum.Parse(typeof(eInstructionState), li.Value));
            }

            filter.InProgressSubStates.Clear();
            foreach (ListItem li in cblInProgressSubStates.Items)
            {
                if (li.Selected)
                    filter.InProgressSubStates.Add((Entities.TrafficSheetFilter.eInProgressSubState)int.Parse(li.Value));
            }

            filter.PlanningCategories.Clear();
            foreach (ListItem li in cblPlanningCategory.Items)
            {
                if (li.Selected)
                    filter.PlanningCategories.Add(int.Parse(li.Value));
            }

            if (chkSelectAllInstructionTypes.Checked)
            {
                filter.IncludeMWFInstructions = true;
                filter.IncludeNonMWFInstructions = true;
            }
            else
            {
                foreach (ListItem li in cblInstructionType.Items)
                {
                    switch (li.Text)
                    {
                        case "MWF":
                            filter.IncludeMWFInstructions = li.Selected;
                            break;
                        case "Non-MWF":
                            filter.IncludeNonMWFInstructions = li.Selected;
                            break;
                    }
                }
            }

            filter.MWFCommsStates.Clear();
            foreach (ListItem item in cblMWFCommsStates.Items.Cast<ListItem>().Where(item => item.Selected))
            {
                filter.MWFCommsStates.Add(int.Parse(item.Value));
            }

            filter.MWFInstructionStates.Clear();
            foreach (ListItem item in cblMWFInstructionStates.Items.Cast<ListItem>().Where(item => item.Selected))
            {
                filter.MWFInstructionStates.Add(int.Parse(item.Value));
            }

            return filter;
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
            }
            else
            {
                // Load the filter and configure the textbox to allow saving a new version
                Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
                Entities.TrafficSheetFilter selectedFilter = facTrafficSheetFilter.GetForTrafficSheetFilterId(trafficSheetFilterId);

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
                SetCookie(); 
                //mwhelper.OutputData = RecoverFilterData();
               // mwhelper.CloseForm = true;
                //mwhelper.CausePostBack = true;
                this.ReturnValue = "filterupdated";
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
                var user = (Entities.CustomPrincipal) Page.User;
                Entities.TrafficSheetFilter filter = null;
                var isUpdate = false;

                if (cboFilters.SelectedValue != "0" && cboFilters.SelectedItem.Text == txtFilterName.Text)
                {
                    // We are going to update the old version of the filter.
                    isUpdate = true;
                    filter = facTrafficSheetFilter.GetForTrafficSheetFilterId(Convert.ToInt32(cboFilters.SelectedValue));
                }
                else
                {
                    filter = new Entities.TrafficSheetFilter(user.IdentityId) {FilterName = txtFilterName.Text};
                }

                filter.FilterOutExcludedPoints = false;
                filter.TrafficSheetGrouping = eTrafficSheetGrouping.None;
                filter.TrafficSheetSorting = eTrafficSheetSorting.EarliestLeg;

                this.PopulateFilter(filter);

                Facade.ITrafficArea facTrafficArea = new Facade.Traffic();

                if (isUpdate)
                    ((Facade.ITrafficSheetFilter)facTrafficArea).Update(filter, user.UserName);
                else
                    filter.FilterId = ((Facade.ITrafficSheetFilter)facTrafficArea).Create(filter, user.UserName);

                PopulateFilterList();
                cboFilters.ClearSelection();
                SelectItem(cboFilters.Items, filter.FilterId);

                Filter = filter;
                SetCookie();
                ReturnValue = "filterupdated";
            }
        }

        /// <summary>
        /// The user wishes to close the window without making any filter changes.
        /// </summary>
        void btnClose_Click(object sender, EventArgs e)
        {
            mwhelper.CloseForm = true;
            mwhelper.CausePostBack = false;
            ReturnValue = "";
        }

        /// <summary>
        /// Adds the icon to the comms status when databound
        /// </summary>
        protected void cblMWFCommsStatus_DataBound(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBoxList;
            if (checkBox != null)
            {
                foreach (ListItem listItem in checkBox.Items)
                {
                    listItem.Text = string.Format("<img src='/images/icons/{1}' />&nbsp{0}&nbsp", listItem.Text,
                                                  GetCommunicationStatusImageFor(int.Parse(listItem.Value)));
                    listItem.Attributes.Add("onclick", "onMWFCommsStatusChecked();");
                }
            }
        }

        protected void cblMWFInstructionStates_DataBound(object sender, EventArgs e)
        {
            var checkBox = sender as CheckBoxList;
            if (checkBox != null)
            {
                foreach (ListItem listItem in checkBox.Items)
                {
                   listItem.Attributes.Add("onclick", "onMWFInstructionStatusChecked();");
                }
            }
        }

        private void cblInstructionStates_DataBound(object sender, EventArgs e)
        {
            var inProgressListItem = cblInstructionStates.Items.FindByValue(eInstructionState.InProgress.ToString());
            inProgressListItem.Attributes["class"] = "inProgressInstructionState";
        }

        #endregion
    }

}