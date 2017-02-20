using System.Collections;
using System.Collections.Specialized;
using System.Security.AccessControl;
using System.Text;
using Orchestrator.Entities;
using Orchestrator.Globals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.SafetyChecks
{
    public partial class Search : System.Web.UI.Page
    {
        #region Constants
        private const string DROP_DOWN_FILTER_ALL_OPTION_TEXT = "All";
        private const string DROP_DOWN_FILTER_ALL_OPTION_VALUE = "";
        #endregion

        protected void Page_Init(object sender, EventArgs e) 
        {
            this.grdResults.NeedDataSource += new GridNeedDataSourceEventHandler(grdResults_NeedDataSource);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            lastResultSet = new List<SafetyCheckCombinedResults>();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
            PopulateSearchCriteriaFields(!IsPostBack);
        }

        List<SafetyCheckCombinedResults> lastResultSet;

        #region Search Criteria

        #region Data driven dropdown helper stuff
        private IDictionary<string, string> _driverList { get; set; }
        private IDictionary<string, string> _vehicleList { get; set; }
        private IDictionary<string, string> _trailerList { get; set; }
        private IDictionary<string, string> _profileList { get; set; }
        private IDictionary<string, string> _statusList { get; set; }

        private void getDriverList(Action callback)
        {
            bool filterShowDeletedDrivers = true;

            _driverList = new Dictionary<string, string>();
            Facade.IDriver facDrivers = new Facade.Resource();
            DataSet dsDrivers = facDrivers.GetAllDrivers(filterShowDeletedDrivers);

            var drivers =
                from dr in dsDrivers.Tables[0].AsEnumerable()
                orderby
                    dr.Field<string>("FullName"),
                    dr.Field<int>("ResourceStatusID") == 2 ? 1 : 0 // Non-deleted driver before deleted one, so if there are duplicate names we include the non-deleted one
                select new
                {
                    DriverTitle = dr.Field<string>("DriverTitle"),
                    FullName = dr.Field<string>("FullName"),
                };

            foreach (var driver in drivers)
            {
                // If the DriverTitle isn't in the form of a Guid, we simply cannot link it to a driver in BS(!!)
                // I'm going to exlucde them from the filter here, although that may result in shouts of "where is my driver?!?"
                // in which case this check will have to be moved to the point when users click search
                Guid driverTitleAsGuid;

                if (Guid.TryParse(driver.DriverTitle, out driverTitleAsGuid))
                    if (!_driverList.ContainsKey(driver.FullName))
                        _driverList.Add(driver.FullName, driverTitleAsGuid.ToString().Trim());
                    else
                        AddHTMLDebugComment("cboDriver: Driver name already in dictionary (" + driver.FullName + "), skipping drop down option.");
                else
                    AddHTMLDebugComment("cboDriver: Unlinkable driver detected (" + driver.FullName + "), skipping drop down option.");
            }
            callback();
        }
        private void fillDriverList() { populateRadComboBox(cboDriver, _driverList, true, true); }

        private void getVehicleList(Action callback)
        {
            var customerId = new Guid(Configuration.BlueSphereCustomerId);
            _vehicleList = new Dictionary<string, string>();
            foreach (KeyValuePair<string, Guid> kvp in MobileWorkerFlow.MWFServicesCommunication.Client.GetVehiclesAssignedToProfile(customerId, null, true, false, false))
            {
                _vehicleList.Add(kvp.Key, kvp.Value.ToString());
            }
            callback();
        }
        private void fillVehicleList() { populateRadComboBox(cboVehicle, _vehicleList, true, true); }

        private void getTrailerList(Action callback)
        {
            var customerId = new Guid(Configuration.BlueSphereCustomerId);
            _trailerList = new Dictionary<string, string>();
            foreach (KeyValuePair<string, Guid> kvp in MobileWorkerFlow.MWFServicesCommunication.Client.GetVehiclesAssignedToProfile(customerId, null, false, true, false))
            {
                _trailerList.Add(kvp.Key, kvp.Value.ToString());
            }
            callback();
        }
        private void fillTrailerList() { populateRadComboBox(cboTrailer, _trailerList, true, true); }

        private void getProfileList(Action callback)
        {
            var customerId = new Guid(Configuration.BlueSphereCustomerId);
            var profiles = MobileWorkerFlow.MWFServicesCommunication.Client.GetAllSafetyCheckProfilesList(customerId);
            // It is possible for there to be multiple profiles with the same title, so as a workaround group and only include the first
            _profileList = profiles.GroupBy(p => p.ProfileTitle, p => p.ProfileId).ToDictionary(group => group.Key, group => group.First().ToString());
            callback();
        }
        private void fillProfileList() { populateRadComboBox(cboProfile, _profileList, true, false); }

        private void getSafetyCheckStatusList(Action callback)
        {
            _statusList = new Dictionary<string, string>();
            _statusList.Add("Failed", "-1");
            _statusList.Add("Passed", "1");
            _statusList.Add("Passed (Discretionary)", "0");
            callback();
        }
        private void fillSafetyCheckStatusList() { populateRadComboBox(cboStatus, _statusList, true, false); }

        /// <summary>
        /// Populates the given RadComboBox with the given Dictionary<string, string> (key = display name, value = value)
        /// </summary>
        /// <param name="targetDropDown">The target RadComboBox dropdown you want to fill</param>
        /// <param name="srcItems">Dictionary<string, string> list of options</param>
        /// <param name="includeAllOption">Should an "all" option be included as the first entry?</param>
        /// <param name="sortByTextAlphabetically">If true then the items will be sorted alphabetically by display text in the list</param>
        private void populateRadComboBox(RadComboBox targetDropDown, IDictionary<string, string> srcItems, bool includeAllOption, bool sortByTextAlphabetically)
        {
            targetDropDown.Items.Clear();
            targetDropDown.Text = "";

            if (includeAllOption)
                targetDropDown.Items.Add(new RadComboBoxItem(DROP_DOWN_FILTER_ALL_OPTION_TEXT, DROP_DOWN_FILTER_ALL_OPTION_VALUE));

            if (sortByTextAlphabetically)
                targetDropDown.Items.AddRange(srcItems.OrderBy(kvp => kvp.Key).Select(kvp => new RadComboBoxItem(kvp.Key.ToString(), kvp.Value.ToString())));
            else
                targetDropDown.Items.AddRange(srcItems.Select(kvp => new RadComboBoxItem(kvp.Key.ToString(), kvp.Value.ToString())));
        }

        #endregion

        /// <summary>
        /// Populate the search criteria form with initial data
        /// </summary>
        /// <param name="refreshFromDb">Should dynamic drop down data be refreshed from the DB? Use sparingly</param>
        public void PopulateSearchCriteriaFields(bool refreshFromDb)
        {
            if (refreshFromDb)
            {   // On initial load refresh drop down data
                AddHTMLDebugComment("Starting to populate drop downs");
                getDriverList(fillDriverList);
                getVehicleList(fillVehicleList);
                getTrailerList(fillTrailerList);
                getProfileList(fillProfileList);
                getSafetyCheckStatusList(fillSafetyCheckStatusList);
                AddHTMLDebugComment("Finished populating drop downs");
            }
        }

        #endregion

        /// <summary>
        /// Invoked when user clicks 'Search'
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            grdResults.Rebind();
        }

        /// <summary>
        /// Invoked when results grid needs a data source
        /// Reads current search criteria values, performs the search, the binds result into grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdResults_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack)
            {
                var customerId = new Guid(Configuration.BlueSphereCustomerId);
                DateTime? selectedFromDate = null;
                DateTime? selectedToDate = null;
                Guid? selectedDriverId = null;
                string selectedVehicleId = null;
                string selectedTrailerId = null;
                Guid? selectedProfileId = null;
                int? selectedStatusId = null;

                // RadDatePicker will default the timestamp portion of a date to midnight.
                // If a toDate has been set then we override the timestamp portion to 23:59:59.999 so we get ALL results up TO and INCLUDING that date
                // Annoyingly I found in testing that this despite my preciseness in setting that timestamp, it always seems to hit SQL having been rounded 1ms up to midnight tomorrow
                // I couldn't find where this was happening and its the most minor of bugs i'll be impressed if they find it in testing ;)
                if (txtEffectiveDateTo.SelectedDate != null) selectedToDate = txtEffectiveDateTo.SelectedDate.Value.AddHours(23.0d).AddMinutes(59.0d).AddSeconds(59.0d).AddMilliseconds(999.0d);
                // Do nothign with the from date leave as is, it makes sens to start searching FROM midnight:
                selectedFromDate = txtEffectiveDateFrom.SelectedDate;

                if (cboDriver.SelectedIndex > 0) selectedDriverId = Guid.Parse(cboDriver.SelectedItem.Value);
                if (cboVehicle.SelectedIndex > 0) selectedVehicleId = cboVehicle.SelectedItem.Value;
                if (cboTrailer.SelectedIndex > 0) selectedTrailerId = cboTrailer.SelectedItem.Value;
                if (cboProfile.SelectedIndex > 0) selectedProfileId = Guid.Parse(cboProfile.SelectedItem.Value);
                if (cboStatus.SelectedIndex > 0) selectedStatusId = int.Parse(cboStatus.SelectedItem.Value);

                List<SafetyCheckCombinedResults> results = MobileWorkerFlow.MWFServicesCommunication.Client.SearchSafetyCheckResults(customerId, selectedFromDate, selectedToDate, selectedVehicleId, selectedTrailerId, selectedDriverId, selectedProfileId, selectedStatusId);
                grdResults.DataSource = results;
                prepareForExport(results);
            }
        }

        /// <summary>
        /// Method to output a string onto the page, wrapped in HTML comment tags.
        /// Will only write comments in DEBUG configuration only.
        /// </summary>
        /// <param name="comment">The string to write; this should be HTML comment safe (ie. must contain no HTML comment tags)</param>
        private void AddHTMLDebugComment(string comment)
        {
            #if DEBUG
                System.Text.StringBuilder htmlComment = new StringBuilder();
                htmlComment.Append("<!--DBG: ");
                htmlComment.Append(comment);
                htmlComment.Append("-->\r\n");
                Page.Header.Controls.Add(new LiteralControl(htmlComment.ToString()));
            #endif   
        }

        /// <summary>
        /// Export the last batch of search results:
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, EventArgs e)
        {
            Response.Redirect("../../reports/csvexport.aspx?filename=SafetyCheckResultsExport.CSV");
        }

        /// <summary>
        /// Sets the session variable for CSV export ready for user to invoke
        /// </summary>
        /// <param name="resultSet"></param>
        private void prepareForExport(List<SafetyCheckCombinedResults> resultSet)
        {
            Session["__ExportDS"] = ToDataSet(resultSet).Tables[0]; // TODO: Verify on no results
        }


        /// <summary>
        /// Convert the results List into a DataSet
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private DataSet ToDataSet(List<SafetyCheckCombinedResults> list)
        {
            Type elementType = typeof(SafetyCheckCombinedResults);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            foreach (var propInfo in elementType.GetProperties())
            {
                if (isExportableColumn(propInfo.Name))
                {
                    Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
                    t.Columns.Add(translateColumnName(propInfo.Name), ColType);
                }
            }

            foreach (SafetyCheckCombinedResults item in list)
            {
                DataRow row = t.NewRow();
                foreach (var propInfo in elementType.GetProperties())
                {
                    if (isExportableColumn(propInfo.Name)) row[translateColumnName(propInfo.Name)] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }
                t.Rows.Add(row);
            }

            return ds;
        }

        /// <summary>
        /// Should the given column name be included in the export?
        /// </summary>
        /// <param name="columnName">Column name</param>
        /// <returns>True if it should be included</returns>
        private bool isExportableColumn(string columnName)
        {
            bool result = false;
            string columnUC = columnName.ToUpper();
            switch (columnUC)
            {
                case "DRIVERTITLE":
                case "VEHICLETITLE":
                case "VEHICLESAFETYCHECKDATE":
                case "VEHICLESAFETYCHECKPROFILETITLE":
                case "VEHICLESAFETYCHECKRESULTTERM":    // User friendly result, Pass, Fail or Discretionary
                case "TRAILERTITLE":
                case "TRAILERSAFETYCHECKDATE":
                case "TRAILERSAFETYCHECKPROFILETITLE":
                case "TRAILERSAFETYCHECKRESULTTERM":    // User friendly result, Pass, Fail or Discretionary
                    result = true;
                    break;
                default:
                    // Note that there ARE also some additional columns I deemed useless for export not listed here that hit the default case, mostly from ObjectBase:
                    result = false;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Translates the SQL result set column name to match the on screen table column
        /// </summary>
        private string translateColumnName(string resultsSetColumnName)
        {
            string result = String.Empty;
            string columnUC = resultsSetColumnName.ToUpper();
            switch (columnUC)
            {
                case "DRIVERTITLE":
                    result = "Driver";
                    break;
                case "VEHICLETITLE":
                    result = "Vehicle";
                    break;
                case "VEHICLESAFETYCHECKDATE":
                    result = "Vehicle Checked";
                    break;
                case "VEHICLESAFETYCHECKPROFILETITLE":
                    result = "Vehicle Profile";
                    break;
                case "VEHICLESAFETYCHECKRESULTTERM":
                    result = "Vehicle Status";
                    break;
                case "TRAILERTITLE":
                    result = "Trailer";
                    break;
                case "TRAILERSAFETYCHECKDATE":
                    result = "Trailer Checked";
                    break;
                case "TRAILERSAFETYCHECKPROFILETITLE":
                    result = "Trailer Profile";
                    break;
                case "TRAILERSAFETYCHECKRESULTTERM":
                    result = "Trailer Status";
                    break;
            }
            return result;
        }
    }
}