using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Web.Caching;
using System.Threading;
using System.Diagnostics;
using Orchestrator.Models;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Traffic
{
    public partial class TSTrafficSheet : Orchestrator.Base.BasePage
    {

        private enum TrafficSheetGridCells
        {
            InstructionID,
            HiddenKeys,
            DraggableLeg,
            LegImage,
            JobID,
            Notes,
            DepotCode,
            LoadNumber,
            Customer,
            StartInstructionType,
            LoadBooked,
            PlannedStartTime,
            StartPoint,
            EndInstructionType,
            Booked,
            PlannedEndTime,
            EndPoint,
            Weight,
            Pallets,
            PlanningCategory,
            Resource,
            Links,
            PlanningCategoryID,
            MwfCommunicationStatus,
        };

        #region Data Members

        private int[] m_trafficAreaIds = { 24 };
        private int[] m_jobTypes = { 1, 2, 3, 4 };
        private DateTime m_startDate = DateTime.MinValue;
        private DateTime m_endDate = DateTime.MaxValue;
        private int m_numberOfJobs = 0;
        private int m_numberofLegs = 0;
        private bool m_showPlannedLegs = true;
        private bool m_showStockMovemementJobs = true;

        private List<int> currentJobs = new List<int>();

        private DataSet _currentTrafficData = null; // This is used for configuration of collapse / expand boxes.
        private Entities.TrafficSheetFilter __filter = null;

        #endregion

        #region Property Interface

        private Entities.TrafficSheetFilter _filter
        {
            get
            {
                if (__filter == null)
                    __filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
                return __filter;
            }
            set
            {
                __filter = value;
            }

        }

        protected string ControlAreaId
        {
            get
            {
                return _filter.ControlAreaId.ToString(); ;
            }
        }

        protected string TrafficAreaIds
        {
            get { return Entities.Utilities.CommaSeparatedIDs(_filter.TrafficAreaIDs); }
        }

        protected string JobTypes
        {
            get
            {
                string _jobTypes = string.Empty;
                foreach (int i in m_jobTypes)
                {
                    _jobTypes += Utilities.UnCamelCase(((eJobType)i).ToString());
                    _jobTypes += ",";
                }
                if (_jobTypes.EndsWith(",")) _jobTypes = _jobTypes.Substring(0, _jobTypes.Length - 1);
                return _jobTypes;
            }

        }

        protected string StartDate
        {
            get
            {
                return m_startDate.ToString("dd/MM/yyyy");
            }
        }

        protected string EndDate
        {
            get
            {
                return m_endDate.ToString("dd/MM/yyyy");
            }
        }

        protected string ShowPlannedLegs
        {
            get { return m_showPlannedLegs.ToString().ToLower(); }
        }

        protected string ShowStockMovementJobs
        {
            get { return m_showStockMovemementJobs.ToString().ToLower(); }
        }

        protected string DepotId
        {
            get { return _filter.DepotId.ToString(); }
        }

        protected bool CollapseRuns
        {
            get { return _filter.CollapseRuns; }
        }

        #endregion

        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtGridSearch.Attributes.Add("onkeydown", "if (event.keyCode == 13){document.getElementById('" + imbRefresh.ClientID + "').click();}");

                UseDefaultFilter();
            }
            else
            {
                // Check to see if the user has requested to uncontract a leg
                string target = Request.Params["__EVENTTARGET"];
                string args = Request.Params["__EVENTARGUMENT"];

                if (!string.IsNullOrEmpty(target))
                {
                    if (target.ToLower() == "uncontractleg" && !string.IsNullOrEmpty(args))
                    {
                        // extract the jobid and the instructionid
                        string[] values = args.Split(new char[] { ',' });
                        UncontractLeg(Convert.ToInt32(values[0]), Convert.ToInt32(values[1]));
                    }
                }
            }

            pnlResetResourceOptions.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            objJobs.Selecting += new ObjectDataSourceSelectingEventHandler(objJobs_Selecting);
            objJobs.Selected += new ObjectDataSourceStatusEventHandler(objJobs_Selected);

            gvTrafficSheet.Sorting += new GridViewSortEventHandler(gvTrafficSheet_Sorting);
            gvTrafficSheet.RowDataBound += new GridViewRowEventHandler(gvTrafficSheet_RowDataBound);
            gvTrafficSheet.RowCreated += new GridViewRowEventHandler(gvTrafficSheet_RowCreated);
            gvTrafficSheet.PreRender += new EventHandler(gvTrafficSheet_PreRender);

            imbRefresh.Click += new ImageClickEventHandler(imbRefresh_Click);

            this.dlgFilter.DialogCallBack += new EventHandler(dlgFilter_DialogCallBack);
            this.dlgTrailerType.DialogCallBack += new EventHandler(dlgTrailerType_DialogCallBack);
            this.dlgResourceThis.DialogCallBack += new EventHandler(dlgResourceThis_DialogCallBack);
            this.dlgTrunk.DialogCallBack += new EventHandler(dlgTrunk_DialogCallBack);
            this.dlgSubcontract.DialogCallBack += new EventHandler(dlgSubcontract_DialogCallBack);
            this.dlgRemoveTrunk.DialogCallBack += new EventHandler(dlgRemoveTrunk_DialogCallBack);
            this.dlgMultiTrunk.DialogCallBack += new EventHandler(dlgMultiTrunk_DialogCallBack);
            this.dlgBookedTimes.DialogCallBack += new EventHandler(dlgBookedTimes_DialogCallBack);
            this.dlgPlannedTimes.DialogCallBack += new EventHandler(dlgPlannedTimes_DialogCallBack);
            this.dlgAddMultipleDestination.DialogCallBack += new EventHandler(dlgAddMultipleDestination_DialogCallBack);
            this.dlgCommunicate.DialogCallBack += (s, ev) => { BindData(); };
            this.dlgChangePlanningDepot.DialogCallBack += dlgChangePlanningDepot_DialogCallBack;
            this.RadMenu1.PreRender += new EventHandler(RadMenu1_PreRender);
        }

        void dlgChangePlanningDepot_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void dlgAddMultipleDestination_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void RadMenu1_PreRender(object sender, EventArgs e)
        {
            Telerik.Web.UI.RadMenuItem rmi = RadMenu1.Items.FindItemByText("Quick Communicate This");
            if (rmi != null)
                rmi.Visible = Orchestrator.Globals.Configuration.ShowQuickCommunicateThis;
        }

        void dlgPlannedTimes_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void dlgBookedTimes_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void dlgMultiTrunk_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void dlgRemoveTrunk_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void dlgSubcontract_DialogCallBack(object sender, EventArgs e)
        {
            this.BindData();
        }

        void dlgTrunk_DialogCallBack(object sender, EventArgs e)
        {
            pnlResetResourceOptions.Visible = true;

            this.BindData();
        }

        void dlgResourceThis_DialogCallBack(object sender, EventArgs e)
        {
            pnlResetResourceOptions.Visible = true;


            this.BindData();
        }
        #endregion

        void dlgTrailerType_DialogCallBack(object sender, EventArgs e)
        {
            if (dlgTrailerType.ReturnValue == "changetrailertype")
                this.BindData();
        }

        void gvTrafficSheet_PreRender(object sender, EventArgs e)
        {
            GridView gv = sender as GridView;
            m_numberofLegs = gv.Rows.Count;
            m_numberOfJobs = currentJobs.Count;
        }

        void imbRefresh_Click(object sender, ImageClickEventArgs e)
        {
            GetValuesFromCookie();
            BindData();
        }

        void gvTrafficSheet_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                AddGlyph(gvTrafficSheet, e.Row);
        }

        void gvTrafficSheet_Sorting(object sender, GridViewSortEventArgs e)
        {
            e.SortExpression = e.SortExpression.Replace("LegPlannedStartDateTime", "StartTimeSortColumn");
            e.SortExpression = e.SortExpression.Replace("LegPlannedEndDateTime", "EndTimeSortColumn");

            if (e.SortExpression.IndexOf("ASC") > 0)
                e.SortExpression = e.SortExpression.Replace("ASC", ",JobId, LegOrder ASC");
            else
            {
                if (e.SortExpression.IndexOf("ASC") == -1 && e.SortExpression.IndexOf("DESC") == -1)
                {
                    e.SortExpression += " ,JobId, LegOrder ASC";
                }
                else
                {
                    e.SortExpression = e.SortExpression.Replace("DESC", ",JobId, LegOrder DESC");
                }

            }
        }

        /// <summary>
        /// Gets a column from the traffic sheet by name. Uses the sort expression column name
        /// which is a more robust name than the header text.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
 
        private DataControlField GetColumnByName(string name)
        {
            return gvTrafficSheet.Columns.Cast<DataControlField>().First(c => c.SortExpression == name);
        }

        void objJobs_Selected(object sender, ObjectDataSourceStatusEventArgs e)
        {
            DataSet dsJobs = (DataSet)e.ReturnValue;
            if (dsJobs != null)
            {
                _currentTrafficData = dsJobs;
                GetColumnByName("BookedDateFrom").Visible = Orchestrator.Globals.Configuration.ShowBookedDeliveryTimeOnTrafficSheet;
                GetColumnByName("LegPlannedStartDateTime").Visible = Orchestrator.Globals.Configuration.ShowPlannedStartTimeOnTrafficSheet;
                GetColumnByName("LegPlannedEndDateTime").Visible = Orchestrator.Globals.Configuration.ShowPlannedFinishTimeOnTrafficSheet;

                GetColumnByName("Weight").Visible = Orchestrator.Globals.Configuration.ShowWeightOnTrafficSheet;
                GetColumnByName("NoPallets").Visible = Orchestrator.Globals.Configuration.ShowPalletsOnTrafficSheet;

                m_numberOfJobs = (int)dsJobs.Tables[1].Rows[0]["NumberOfJobs"];
                m_numberofLegs = dsJobs.Tables[0].Rows.Count;

                lblDateRange.Text = "From : " + _filter.FilterStartDate.ToString("dd/MM/yy HH:mm") + "   To : " + _filter.FilterEnddate.ToString("dd/MM/yy HH:mm");
                string filterExpression = string.Empty;
                string filter = string.Empty;
                bool collectionSearch = false;
                if (txtGridSearch.Text.ToLower().StartsWith("col:"))
                {
                    filterExpression = "(StartInstructionTypeId=1 AND StringStartDateTime Like '%{0}%')";
                    collectionSearch = true;
                    lblCollectionFilter.Text = "Col. Filter : " + txtGridSearch.Text.ToLower().Replace("col:", "");

                }
                else
                {
                    filterExpression = "(Load_Number Like '%{0}%') OR (OrganisationName Like '%{0}%') OR (StartPointDisplay Like '%{0}%') OR (EndPointDisplay Like '%{0}%') OR (FullName Like '%{0}%') OR (RegNo Like '%{0}%') OR (TrailerRef Like '%{0}%' OR StartingClients Like '%{0}%' OR EndingClients Like '%{0}%')";
                    lblCollectionFilter.Text = string.Empty;
                }

                // Add the filtering for the spcific Trailer Types here?
                if (txtGridSearch.Text.Length > 0)
                {
                    if (!string.IsNullOrEmpty(filter))
                        filter += " AND ";

                    if (collectionSearch)
                        filter = string.Format(filterExpression, txtGridSearch.Text.ToLower().Replace("col:", ""));
                    else
                        filter = string.Format(filterExpression, txtGridSearch.Text);
                }
                else
                {
                    lblCollectionFilter.Text = string.Empty;
                }


                if (!string.IsNullOrEmpty(filter))
                {
                    //dsJobs.Tables[ 0 ].DefaultView.RowFilter = filter;
                    objJobs.FilterExpression = filter;
                    txtGridSearch.Text = string.Empty;
                }
            }
        }

        void gvTrafficSheet_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;

                HtmlImage imgInstructionOrderNotes = e.Row.FindControl("imgInstructionOrderNotes") as HtmlImage;
                HtmlImage imgHasPCV = e.Row.FindControl("imgHasPCV") as HtmlImage;

                string jobId = drv["JobId"].ToString();
                string instructionID = drv["InstructionID"].ToString();
                string driver = drv["FullName"].ToString();
                string driverResourceId = drv["DriverResourceId"] == DBNull.Value ? "0" : drv["DriverResourceId"].ToString();
                string subContractorId = drv["SubContractorIdentityId"] == DBNull.Value ? "0" : drv["SubContractorIdentityId"].ToString();
                string regNo = drv["RegNo"].ToString();
                string vehicleResourceId = drv["VehicleResourceId"] == DBNull.Value ? "0" : drv["VehicleResourceId"].ToString();
                string trailerRef = drv["TrailerRef"].ToString();
                string trailerResourceId = drv["TrailerResourceId"] == DBNull.Value ? "0" : drv["TrailerResourceId"].ToString();
                DateTime bookedDelDate = (DateTime)drv["BookedDateFrom"];
                string legPlannedStart = ((DateTime)drv["LegPlannedStartDateTime"]).ToString();
                string legPlannedEnd = ((DateTime)drv["LegPlannedEndDateTime"]).ToString();
                string depotCode = drv["DepotCode"].ToString();
                string lastUpdateDate = DateTime.Parse(drv["LastUpdateDate"].ToString()).ToString("dd/MM/yy HH:mm:ss");
                string instructionStateID = drv["InstructionStateId"].ToString();
                string startInstructionStateID = drv["StartInstructionStateID"].ToString();
                string endInstructionStateID = drv["EndInstructionStateID"].ToString();
                string startInstructionID = drv["StartInstructionId"].ToString();
                string endInstructionID = drv["EndInstructionId"].ToString();
                bool startIsAnyTime = drv["StartIsAnyTime"] == DBNull.Value ? false : Convert.ToBoolean(drv["StartIsAnyTime"]);
                bool endIsAnyTime = drv["EndIsAnyTime"] == DBNull.Value ? false : Convert.ToBoolean(drv["EndIsAnyTime"]);
                bool bookedIsAnyTime = drv["BookedIsAnytime"] == DBNull.Value ? false : Convert.ToBoolean(drv["BookedIsAnytime"]);
                string legOrder = drv["LegOrder"].ToString();
                string rowId = e.Row.ClientID;
                bool displayOrderNotePopup = drv["DisplayOrderNotePopup"] == DBNull.Value ? false : Convert.ToBoolean(drv["DisplayOrderNotePopup"]);
                string legPosition = drv["LegPosition"] == DBNull.Value ? string.Empty : drv["LegPosition"].ToString();
                var allowTTCommunication = drv.Row.Field<bool>("AllowTTCommunication");

                if (startInstructionStateID != endInstructionStateID && int.Parse(legOrder) == 0)
                {
                    // determine the correct instructionid to use.
                    if (int.Parse(startInstructionStateID) < int.Parse(endInstructionStateID))
                    {
                        if (!string.IsNullOrEmpty(startInstructionID))
                        {
                            instructionID = startInstructionID;
                            instructionStateID = startInstructionStateID;
                        }
                        else
                            instructionID = endInstructionID;
                    }
                }

                Label lblHiddenKeys = (Label)e.Row.FindControl("lblHiddenKeys");

                lblHiddenKeys.Text = "";
                const string SEPARATOR = "|";

                StringBuilder sb = new StringBuilder();
                sb.Append(jobId);
                sb.Append(SEPARATOR);
                sb.Append(instructionID);
                sb.Append(SEPARATOR);
                sb.Append(driver);
                sb.Append(SEPARATOR);
                sb.Append(driverResourceId);
                sb.Append(SEPARATOR);
                sb.Append(regNo);
                sb.Append(SEPARATOR);
                sb.Append(vehicleResourceId);
                sb.Append(SEPARATOR);
                sb.Append(trailerRef);
                sb.Append(SEPARATOR);
                sb.Append(trailerResourceId);
                sb.Append(SEPARATOR);
                sb.Append(legPlannedStart);
                sb.Append(SEPARATOR);
                sb.Append(legPlannedEnd);
                sb.Append(SEPARATOR);
                sb.Append(depotCode);
                sb.Append(SEPARATOR);
                sb.Append(lastUpdateDate);
                sb.Append(SEPARATOR);
                sb.Append(instructionStateID);
                sb.Append(SEPARATOR);
                sb.Append(startInstructionID);
                sb.Append(SEPARATOR);
                sb.Append(startInstructionStateID);

                lblHiddenKeys.Text = sb.ToString();


                trailerRef = trailerRef.Replace("'", "\\'");
                driver = driver.Replace("'", "\\'");

                string functionName = "sCML(event, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}');";
                functionName = string.Format(functionName, jobId, instructionID, driver, driverResourceId, subContractorId, regNo, vehicleResourceId, trailerRef, trailerResourceId,
                                                           legPlannedStart, legPlannedEnd, depotCode, lastUpdateDate, instructionStateID, allowTTCommunication, rowId);

                e.Row.Attributes.Add("oncontextmenu", functionName);

                if (Orchestrator.Globals.Configuration.ShowBookedDeliveryTimeOnTrafficSheet && bookedIsAnyTime)
                    e.Row.Cells[(int)TrafficSheetGridCells.StartInstructionType].Style.Add("font-weight", "bold");

                if (Orchestrator.Globals.Configuration.ShowPlannedStartTimeOnTrafficSheet && startIsAnyTime)
                    e.Row.Cells[(int)TrafficSheetGridCells.PlannedStartTime].Style.Add("font-weight", "bold");

                if (Orchestrator.Globals.Configuration.ShowPlannedStartTimeOnTrafficSheet && endIsAnyTime)
                    e.Row.Cells[(int)TrafficSheetGridCells.Booked].Style.Add("font-weight", "bold");

                try
                {
                    if (bookedDelDate.Date > DateTime.Today.Date)
                        e.Row.Cells[(int)TrafficSheetGridCells.StartInstructionType].Style.Add("font-style", "italic");

                    if (DateTime.Parse(legPlannedStart).Date > DateTime.Today.Date)
                        e.Row.Cells[(int)TrafficSheetGridCells.PlannedStartTime].Style.Add("font-style", "italic");
                }
                catch { }

                foreach (TableCell cell in e.Row.Cells)
                {
                    cell.CssClass = "DataCell";
                }

                if (imgInstructionOrderNotes != null)
                {
                    if (displayOrderNotePopup)
                    {
                        imgInstructionOrderNotes.Attributes.Add("onmouseover", "javascript:ShowCollectionDeliveryNotesForInstructions(this," + startInstructionID.ToString() + ",'" + endInstructionID.ToString() + "');");
                        imgInstructionOrderNotes.Attributes.Add("onmouseout", "javascript:closeToolTip();");
                        imgInstructionOrderNotes.Attributes.Add("class", "orchestratorLink");
                    }

                    imgInstructionOrderNotes.Visible = displayOrderNotePopup;
                }

                imgHasPCV.Visible = (bool)drv["HasPCVAttached"];

                switch ((eInstructionState)int.Parse(instructionStateID))
                {
                    case eInstructionState.Planned:
                        e.Row.CssClass = "statePlanned";
                        break;
                    case eInstructionState.InProgress:
                        e.Row.CssClass = "stateInProgress";
                        break;
                    case eInstructionState.Completed:
                        e.Row.CssClass = "stateCompleted";
                        break;
                }

                Literal litLegImage = e.Row.FindControl("litLegImage") as Literal;

                if (litLegImage != null)
                {

                    const string leadRunLegRow = @"<table style=""width:100%;"">
                                                    <tr>
                                                        <td align=""center"">
                                                            <img src=""{0}"" height=""11"" width=""14"" alt="""" JobId=""{1}"" InstructionId=""{2}"" class=""tsToggleImage"" onclick=""toggleJobs(this, {1}, {2});"" />
                                                        </td>
                                                        <td align=""center"" style=""width:30%;"">
                                                            {3}
                                                        </td>
                                                    </tr>
                                                   </table>";

                    const string followingRunLegRow = @"<table style=""width:100%;"">
                                                            <tr>
                                                                <td>
                                                                    <span id=""spnJobSelector"" style=""display:none"" JobId=""{0}""></span>
                                                                </td>
                                                                <td align=""center"" style=""width:30%;"">
                                                                    {1}
                                                                </td>
                                                            </tr>
                                                        </table>";

                    string retVal = string.Empty, legPositionImage = string.Empty;
                    int jID, iID;
                    int.TryParse(jobId, out jID);
                    int.TryParse(instructionID, out iID);
                    string rowDisplay = _filter.CollapseRuns ? "display:none;" : "";

                    if (_currentTrafficData != null && _currentTrafficData.Tables.Count > 0 && _currentTrafficData.Tables[0].Rows.Count > 0)
                    {
                        var queryTrafficItems = _currentTrafficData.Tables[0].Rows.Cast<DataRow>().AsEnumerable();
                        var foundLegs = from row in queryTrafficItems
                                        where row.Field<int>("JobId") == jID
                                        orderby row.Field<int>("LegOrder") ascending
                                        select row;

                        if (foundLegs != null && foundLegs.Count() > 0)
                        {
                            string imagePath = _filter.CollapseRuns ? Page.ResolveClientUrl("~/images/exp.gif") : Page.ResolveClientUrl("~/images/col.gif");

                            switch (legPosition)
                            {
                                case "First":
                                case "Middle":
                                case "Last":
                                    break;
                                default:
                                    legPosition = "spacer";
                                    break;
                            }

                            if (foundLegs.Count() == 1 && legPosition == "spacer")
                            {
                                legPositionImage = LegPositionImage(legPosition, instructionID, jobId, true);
                                retVal = legPositionImage;
                            }
                            else if (foundLegs.Count() == 1 && legPosition != "spacer")
                            {
                                legPositionImage = LegPositionImage(legPosition, instructionID, jobId, true);
                                retVal = string.Format(followingRunLegRow, jobId, legPositionImage);
                            }
                            else if (foundLegs.Count() > 1 && foundLegs.First().Field<int>("InstructionID") == iID)
                            {
                                legPositionImage = LegPositionImage(legPosition, instructionID, jobId, !_filter.CollapseRuns);
                                retVal = string.Format(leadRunLegRow, imagePath, jobId, instructionID, legPositionImage);
                            }
                            else
                            {
                                legPositionImage = LegPositionImage(legPosition, instructionID, jobId, true);
                                retVal = string.Format(followingRunLegRow, jobId, legPositionImage);

                                if (_filter.CollapseRuns)
                                    e.Row.Style.Add("display", "none");
                            }
                        }
                        else
                            retVal = legPositionImage;
                    }
                    else
                        retVal = legPositionImage;

                    litLegImage.Text = retVal;
                }

                if (!currentJobs.Contains(int.Parse(jobId)))
                    currentJobs.Add(int.Parse(jobId));

            }
        }

        private string LegPositionImage(string legPosition, string instructionId, string jobId, bool displayImage)
        {
            const string legImage = @"<img src=""{0}"" style=""{1}"" height=""20"" width=""5"" class=""tsLegImage"" JobId=""{2}"" InstructionId=""{3}"" alt="""" />";
            const string spacer = @"<img src=""{0}"" style=""{0}"" height=""20"" width=""5"" alt="""" />";

            string legPositionImage = string.Empty;
            string rowDisplay = displayImage ? "" : "display:none";
            switch (legPosition)
            {
                case "First":
                    legPositionImage = string.Format(legImage, Page.ResolveClientUrl("~/images/legTop.gif"), rowDisplay, jobId, instructionId);
                    break;
                case "Middle":
                    legPositionImage = string.Format(legImage, Page.ResolveClientUrl("~/images/legMiddle.gif"), rowDisplay, jobId, instructionId);
                    break;
                case "Last":
                    legPositionImage = string.Format(legImage, Page.ResolveClientUrl("~/images/legBottom.gif"), rowDisplay, jobId, instructionId);
                    break;
                default:
                    legPositionImage = string.Format(spacer, Page.ResolveClientUrl("~/images/spacer.gif"));
                    break;
            }

            return legPositionImage;
        }

        void objJobs_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            if (_filter == null)
                UseDefaultFilter();

            // this is required fpr when sorting is applied to the grid
            if (_filter.FilterStartDate == DateTime.MinValue)
                if (!GetValuesFromCookie())
                {
                    e.Cancel = true;
                    return;
                }

            e.InputParameters.Add("controlAreaId", _filter.ControlAreaId);
            e.InputParameters.Add("trafficAreaIds", _filter.TrafficAreaIDs.ToArray());
            e.InputParameters.Add("enforceUserCollectionPreferences", false);
            e.InputParameters.Add("startDate", _filter.FilterStartDate);
            e.InputParameters.Add("endDate", _filter.FilterEnddate);
            e.InputParameters.Add("showPlanned", _filter.ShowPlannedLegs);
            e.InputParameters.Add("ShowStockMovementJobs", _filter.ShowStockMovementJobs);
            e.InputParameters.Add("identityid", ((Entities.CustomPrincipal)Page.User).IdentityId);
            e.InputParameters.Add("businessTypes", _filter.BusinessTypes);
            e.InputParameters.Add("instructionStates", _filter.InstructionStates);

            // In Progress Sub-States
            var includeInProgress = _filter.InstructionStates.Contains((int)eInstructionState.InProgress);
            var subStatesFiltered = _filter.InProgressSubStates.Any();
            e.InputParameters.Add("excludeInProgressNonMWF", includeInProgress && subStatesFiltered && !_filter.InProgressSubStates.Contains(Entities.TrafficSheetFilter.eInProgressSubState.NonMWF));
            e.InputParameters.Add("excludeInProgressMWFCommunicated", includeInProgress && subStatesFiltered && !_filter.InProgressSubStates.Contains(Entities.TrafficSheetFilter.eInProgressSubState.MWFCommunicated));
            e.InputParameters.Add("excludeInProgressMWFStarted", includeInProgress && subStatesFiltered && !_filter.InProgressSubStates.Contains(Entities.TrafficSheetFilter.eInProgressSubState.MWFStarted));

            // HETTI Parameters
            e.InputParameters.Add("includeMWFInstructions", _filter.IncludeMWFInstructions);
            e.InputParameters.Add("includeNonMWFInstructions", _filter.IncludeNonMWFInstructions);

            // Get List of selected MWF Comms filter states and convert to actual MWF instruction states.
            var comms = MWF_Instruction.GetCommunicationStatesFromTrafficSheetFilter(_filter.MWFCommsStates);
            e.InputParameters.Add("mwfCommsStates", comms);
            e.InputParameters.Add("mwfInstructionStates", _filter.MWFInstructionStates);

            // Moved this from the filter to an installation setting as this is only used by richards
            e.InputParameters.Add("sortbyEvent", Globals.Configuration.SortTrafficSheetByEvent);
        }

        /// <summary>
        /// Triggered when a modal window is closed.
        /// </summary>
        void dlgFilter_DialogCallBack(object sender, EventArgs e)
        {
            if (this.dlgFilter.ReturnValue.ToLower() == "filterupdated")
            {
                BindData();
                // Reload the resource window
                Page.ClientScript.RegisterStartupScript(typeof(Page), "reloadResource", "top.document.getElementById('tsResource').contentWindow.location.href='TSResource.aspx?csid=" + this.CookieSessionID + "';", true);
            }
        }

        private void UncontractLeg(int jobId, int instructionID)
        {
            Orchestrator.Facade.IJobSubContractor facJS = new Orchestrator.Facade.Job();
            facJS.UncontractInstruction(jobId, instructionID, this.Page.User.Identity.Name);
        }

        private void BindData()
        {
            gvTrafficSheet.DataBind();
        }

        private bool GetValuesFromCookie()
        {
            try
            {
                _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);

                return true;
            }
            catch
            {
                pnlConfirmation.Visible = true;
                // Generate Default cookie as there is a now a proplem
                GenerateCookie();
                return false;
            }
        }

        private void UseDefaultFilter()
        {

            try
            {
                // Deserialize the Filter 
                _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, this.Request);

                if (_filter == null || _filter.FilterStartDate == DateTime.MinValue)
                    _filter = GenerateCookie();

            }
            catch (Exception ex)
            {
                // invalid or old values stored in the cookie simply regenerate.
                Debug.Write(ex.Message);
                _filter = GenerateCookie();
            }

        }


        private Entities.TrafficSheetFilter GenerateCookie()
        {

            return Utilities.GenerateCookie(this.CookieSessionID, this.Response, ((Entities.CustomPrincipal)Page.User).IdentityId);
        }

        protected string GetBookedDateTime(DateTime from, DateTime to, bool isTimed, bool isAnyTime, int instructionTypeID)
        {
            string retVal = string.Empty;

            // Based on Conervsation with Tony Richards @19/11 17:05
            if (isAnyTime)
            {
                retVal = from.Date.ToString("dd/MM") + " Anytime";
            }
            if (isTimed)
            {
                retVal = from.ToString("dd/MM HH:mm");
            }

            if (instructionTypeID == (int)eInstructionType.Trunk)
            {
                retVal = from.ToString("dd/MM HH:mm");
            }
            else if (!isAnyTime && !isTimed)
            {

                if (from.Date != to.Date)
                {
                    retVal = string.Format("{0} to {1}", from.ToString("dd @ HH:mm"), to.ToString("dd @ HH:mm"));
                }
                else
                {
                    retVal = string.Format("{0} {1} to {2}", from.ToString("dd/MM"), from.ToString("HH:mm"), to.ToString("HH:mm"));
                }
            }

            return retVal;
        }

        protected string GetInstructionTypeImage(int instructionTypeID)
        {
            if (instructionTypeID == 1 || instructionTypeID == 5)
                return "loadfinal.png";
            else if (instructionTypeID == 7)
                return "trunk.gif";
            else
                return "dropfinal.png";
        }

        protected string GetLinkImage(string hasLinks)
        {
            if (hasLinks != "0")
                return "<span onmouseover='showHelpTipUrl(event,\"~/job/getJobDataForJobPopup.aspx?LegId=" + hasLinks + "\");' onmouseout=\"hideHelpTip(this);\" class=\"helpLink\"><img src=\"../images/jobLink.gif\" border=\"0\" height=\"18\" width=\"15\" /></span>";
            else
                return string.Empty;
        }

        protected string GetLoadLinks(string loadLinks)
        {
            string linkTemplate = "<span id=\"spnLoadNumber\"  onMouseOver=\"sONTT(this, {0});\" onMouseOut=\"closeToolTip();\" class=\"orchestratorLink\">{1}</span>";
            string[] references = loadLinks.Split('/');
            string[] parts;
            string lnk = string.Empty;
            string retVal = string.Empty;
            foreach (string link in references)
            {
                parts = link.Split('|');
                if (parts.Length > 1 && (!retVal.Contains(parts[0].Trim())))
                {
                    lnk = String.Format(linkTemplate, parts[1].Trim(), parts[0].Trim());
                    if (retVal.Length > 0)
                        retVal += " / ";
                    retVal += lnk;
                }
            }
            if (retVal.Length == 0)
                retVal = loadLinks;

            return retVal;
        }

        protected string GetResourceLink(DataRowView data)
        {
            #region The Constants used for the links
            string lnkResourceThis = @"<a href=""javascript:oRW({0},'{1}', {2}, '{3}', {4}, '{5}', {6} , '{7}', '{8}', '{9}', '{10}', '{11}' )"">{12}</a>";
            string lnkCallInThis = @"<a href=""#"" onclick=""javascript:window.open('/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid={0}&instructionid={1}&csid={2}');"">{3}</a>";

            #endregion
            string retVal = string.Empty;
            DataRow row = data.Row;

            // Detrmine the correct instruction and state we are looking at here.
            string startInstructionStateID = row["StartInstructionStateID"].ToString();
            string endInstructionStateID = row["EndInstructionStateID"].ToString();
            string instructionID = "";
            string startInstructionID = row["StartInstructionID"].ToString();
            string endInstructionID = row["EndInstructionID"].ToString();
            string instructionStateID = row["InstructionStateId"].ToString();
            string legOrder = row["LegOrder"].ToString();
            if (startInstructionStateID != endInstructionStateID && int.Parse(legOrder) == 0)
            {
                // determine the correct instructionid to use.
                if (int.Parse(startInstructionStateID) < int.Parse(endInstructionStateID))
                {
                    if (!string.IsNullOrEmpty(startInstructionID))
                    {
                        instructionID = startInstructionID;
                        instructionStateID = startInstructionStateID;
                    }
                    else
                        instructionID = endInstructionID;
                }
            }

            if (string.IsNullOrEmpty(instructionID))
                instructionID = row["InstructionId"].ToString();

            eInstructionState instructionState = (eInstructionState)int.Parse(instructionStateID);

            string divWrapper = "<div style='float:left;width:25%;'>{0}</div>";

            string resourceName = string.Format("<div style='float:left;width:50%;'>{0}</div>", row["FullName"].ToString().Length > 0 ? row["FullName"].ToString() : row["SubContractorName"].ToString().Length > 0 ? row["SubContractorName"].ToString() : "Resource This");
            if (row["RegNo"].ToString().Length > 0)
            {
                if (resourceName.Contains("Resource This"))
                    resourceName += string.Format("<div style='float:left;width:25%;'>{0}</div>", row["RegNo"].ToString());
                else
                    resourceName += string.Format("<div style='float:left;width:25%;'>{0}</div>", row["RegNo"].ToString());
            }
            else
            {
                resourceName += string.Format("<div style='float:left;width:25%;'>{0}</div>", "&nbsp;");
            }

            if (row["TrailerRef"].ToString().Length > 0)
            {
                if (resourceName.Contains("Resource This"))
                    resourceName += string.Format("<div style='float:left;width:25%;'>{0}</div>", row["TrailerRef"].ToString());
                else
                    resourceName += string.Format("<div style='float:left;width:25%;'>{0}</div>", row["TrailerRef"].ToString());
            }

            #region // If the Instruction is Booked show the resource this link
            if (instructionState == eInstructionState.Planned || instructionState == eInstructionState.Booked)
            {


                retVal = string.Format(lnkResourceThis, instructionID.ToString(),
                                                        row["FullName"].ToString(),
                                                        row["DriverResourceId"] == DBNull.Value ? "0" : row["DriverResourceId"].ToString(),
                                                        row["RegNo"].ToString(),
                                                        row["VehicleResourceId"] == DBNull.Value ? "0" : row["VehicleResourceId"].ToString(),
                                                        row["TrailerRef"].ToString(),
                                                        row["TrailerResourceId"] == DBNull.Value ? "0" : row["TrailerResourceId"].ToString(),
                                                        row["LegPlannedStartDateTime"].ToString(),
                                                        row["LegPlannedEndDateTime"].ToString(),
                                                        row["DepotCode"].ToString(),
                                                        row["LastUpdateDate"].ToString(),
                                                        row["JobId"].ToString(),
                                                        resourceName);
            }
            #endregion

            // If the instruction is in porgress show the resource and the call in link
            if (instructionState == eInstructionState.InProgress)
            {
                retVal = string.Format(lnkCallInThis, row["JobId"], instructionID, CookieSessionID, resourceName);
            }

            #region // if the instruction is complete show the resource and no links
            if (instructionState == eInstructionState.Completed)
            {

                retVal = resourceName;
            }
            #endregion

            return retVal;
        }

        protected int NumberOfLegsToPlan
        {
            get { return m_numberofLegs; }
        }

        protected int NumberOfJobsToPlan
        {
            get { return m_numberOfJobs; }
        }

        private void AddGlyph(GridView grid, GridViewRow item)
        {
            Label glyph = new Label();
            glyph.EnableTheming = false;
            glyph.Font.Name = "webdings";
            glyph.Font.Size = FontUnit.XSmall;
            glyph.Text = (grid.SortDirection == System.Web.UI.WebControls.SortDirection.Ascending ? " 5" : " 6");

            // Find the column sorted by
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                string colExpr = grid.Columns[i].SortExpression;
                if (colExpr != "" && colExpr == grid.SortExpression)
                {
                    item.Cells[i].Controls.Add(glyph);
                }
            }
        }

        protected string GetCustomerName(int startInstructionTypeID, int endInstructionTypeID, string startingCustomers, string endingCustomers)
        {
            string LINK = @"<span id=""spnStartPointDisplay"" onmouseover=""sODTT(this, {0});"" onmouseout=""closeToolTip();"" class=""orchestratorLink"">{1}</span>";
            string retVal = string.Empty;
            string[] _startingClients = startingCustomers.Split(',');
            string[] _endingClients = endingCustomers.Split(',');
            List<string> clients = new List<string>();

            // this has been simplified as it appears that the Traffic sheet is only used for Full Load work.
            if (startingCustomers.Length > 0)
            {
                foreach (string client in _startingClients)
                {
                    if (clients.IndexOf(string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0])) == -1)
                        if (clients.Count > 0)
                            clients.Add("<br/>" + string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0]));
                        else
                            clients.Add(string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0]));
                }
            }
            if (endingCustomers.Length > 0)
            {
                foreach (string client in _endingClients)
                {
                    if (clients.IndexOf(string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0])) == -1)
                        if (clients.Count > 0)
                            clients.Add("<br/>" + string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0]));
                        else
                            clients.Add(string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0]));
                }
            }
            foreach (string client in clients)
                retVal += client;

            // if there is only 1 starting and 1 ending client and they are the same just return this one.
            /*
            if (_startingClients.Length == _endingClients.Length)
                if (_startingClients[0] == _endingClients[0])
                    retVal = string.Format(LINK, (_startingClients[0].Split('|'))[1], (_startingClients[0].Split('|'))[0]);
                else
                {
                    // We could be doing a load to load
                    if (startInstructionTypeID == 1)
                        retVal = string.Format(LINK, (_startingClients[0].Split('|'))[1], (_startingClients[0].Split('|'))[0]);
                    else
                        retVal += "<br/>" + string.Format(LINK, (_endingClients[0].Split('|'))[1], (_endingClients[0].Split('|'))[0]);
                }
            else
            {
                // there is more than 1 client involved, depending on the instruction ype display the names for the start or the end
                if (startInstructionTypeID == 1 || startInstructionTypeID == 5)
                {
                    // Show Loading client
                    foreach (string client in _startingClients)
                    {
                        retVal += string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0]);
                    }
                }
                else
                {
                    // Show Delivery client
                    foreach (string client in _endingClients)
                    {
                        retVal += string.Format(LINK, (client.Split('|'))[1], (client.Split('|'))[0]);
                    }
                }
            }

            */
            return retVal;
        }

        int rowNumber = 0;
        protected int GetRowNumber()
        {
            rowNumber++;
            return rowNumber;
        }

        [System.Web.Services.WebMethod]
        public static bool UnCommunicate(int jobID, int instructionID)
        {
            try
            {
                Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
                return facDriverCommunication.RemoveDriverCommunication(jobID, instructionID);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [System.Web.Services.WebMethod]
        public static bool Communicate(int instructionID, string driver, int driverResourceId, int vehicleResourceId, int subContractorId, int jobID, string userId)
        {
            try
            {
                bool retVal = false;

                Entities.DriverCommunication communication = new Entities.DriverCommunication();
                communication.Comments = "Communicated via Run Details";
                communication.DriverCommunicationStatus = eDriverCommunicationStatus.Accepted;

                communication.NumberUsed = "unknown";

                if (subContractorId > 0)
                {
                    Facade.IJobSubContractor facSubContractor = new Facade.Job();
                    communication.DriverCommunicationType = eDriverCommunicationType.Manifest;
                    communication.DriverCommunicationId = facSubContractor.CreateCommunication(jobID, instructionID, subContractorId, communication, userId);
                }
                else
                {
                    Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
                    communication.DriverCommunicationType = facDriverCommunication.GetDefaultCommunicationType(driverResourceId, vehicleResourceId);
                    communication.DriverCommunicationId = facDriverCommunication.Create(jobID, driverResourceId, communication, userId, instructionID);

                    int controlArea = 0;
                    int trafficArea = 0;

                    // determine the destination CA and TA for the Instruction to be communicated.
                    Entities.InstructionCollection _instructions = null;
                    using (Facade.IInstruction facInstrucion = new Facade.Instruction())
                    {
                        _instructions = facInstrucion.GetForJobId(jobID);
                    }
                    int lastInstructionID = -1;
                    for (int instructionIndex = _instructions.Count - 1; instructionIndex >= 0; instructionIndex--)
                    {
                        if (_instructions[instructionIndex].Driver != null && _instructions[instructionIndex].Driver.ResourceId == driverResourceId)
                        {
                            lastInstructionID = _instructions[instructionIndex].InstructionID;
                            controlArea = _instructions[instructionIndex].Point.Address.TrafficArea.ControlAreaId;
                            trafficArea = _instructions[instructionIndex].Point.Address.TrafficArea.TrafficAreaId;
                            break;
                        }
                    }

                    if (controlArea == 0 || trafficArea == 0)
                        throw new Exception("Unable to communicate this Run. There may have been some changes since you last Refreshed the screen. Please press Refresh and try again.");

                    // by defaul this will need to give the resource as well to maintain consistency.
                    facDriverCommunication.Create(jobID, driverResourceId, communication, userId, instructionID, lastInstructionID, controlArea, trafficArea);
                }

                return retVal;
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        [System.Web.Services.WebMethod]
        public static int LoadRunIntoCache(int jobID)
        {

            //Facade.IJob facJob = new Facade.Job();
            //Entities.Job job = facJob.GetJob(jobID, true);
            //System.Web.HttpContext.Current.Cache.Add("JobEntityForJobId" + jobID.ToString(),
            //            job,
            //            null,
            //            Cache.NoAbsoluteExpiration,
            //            TimeSpan.FromMinutes(5),
            //            CacheItemPriority.Normal,
            //            null);

            return jobID;
        }

        [System.Web.Services.WebMethod]
        public static bool CreateACopyOfRun(int jobID, string userID)
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = facJob.GetJob(jobID, true);

            System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    facJob.Copy(job, userID);
                }
                catch (Exception ex)
                {
                    Global.UnhandledException(ex);
                }
            });

            return true;
        }

        #region TT Integration

        // used for getting status information about the job


        public static string GetTTCommunicationStatusTextWS()
        {
            string LINK = @" onmouseover=""sODTT(this, {0});"" onmouseout=""closeToolTip();"" class=""orchestratorLink"">";

            LINK = string.Format(LINK, "FOO");

            return LINK;
        }


        protected string GetTTCommunicationStatusImage(object communicationStatusID, bool allowTTCommunication, string startInstructionID, string endInstructionID)
        {
            if (communicationStatusID == null || string.IsNullOrEmpty(communicationStatusID.ToString()))
            {
                return allowTTCommunication ? "exclamation.png" : string.Empty;
            }

            if (communicationStatusID is int)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    
                    var instRepo = DIContainer.CreateRepository<IInstructionRepository>(uow);
                    var mwfInstRepo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);

                    var startInstruction = instRepo.Find(int.Parse(startInstructionID));

                    Models.MWF_Instruction mwfStartInstruction;

                    if (startInstruction.InstructionOrder == 0)
                        mwfStartInstruction = mwfInstRepo.GetForHaulierEnterpriseInstruction(int.Parse(startInstructionID));
                    else
                        mwfStartInstruction = mwfInstRepo.GetProceedFromInstructionForHEOriginInstruction(int.Parse(startInstructionID));

                    var mwfEndInstruction = mwfInstRepo.GetForHaulierEnterpriseInstruction(int.Parse(endInstructionID));

                    var hasLegGotMwfCommError =
                ((mwfStartInstruction != null && mwfStartInstruction.DriveDateTime == null && mwfStartInstruction.CommunicationStatus != MWFCommunicationStatusEnum.AcknowledgedOnDevice) ||
                mwfEndInstruction != null && mwfEndInstruction.DriveDateTime == null && mwfEndInstruction.CommunicationStatus != MWFCommunicationStatusEnum.AcknowledgedOnDevice);

                    if (hasLegGotMwfCommError)
                        return "tlRed.gif";

                    return "tlGreen.gif";
                }
            }
            else
            {
                throw new InvalidCastException("GetTTCommunicationStatusImage - CommunicationStatusID is not an int, unable to get image name.");
            }
        }

        protected string GetTTCommunicationStatusText(object communicationStatusID, bool allowTTCommunication)
        {
            if (communicationStatusID == null || string.IsNullOrEmpty(communicationStatusID.ToString()))
            {
                return allowTTCommunication ? "Not Communicated" : string.Empty;
            }

            if (communicationStatusID is int)
            {
                switch ((MWFCommunicationStatusEnum)communicationStatusID)
                {
                    case MWFCommunicationStatusEnum.NotReadyToBeCommunicated:
                        return string.Empty;
                    case MWFCommunicationStatusEnum.NewInformationNeedsCommunicating:
                        return "Communication Sent";
                    case MWFCommunicationStatusEnum.UpdatedInformationNeedsCommunicating:
                        return "Communication Sent";
                    case MWFCommunicationStatusEnum.DeletedInformationNeedsCommunicating:
                        return "Communication Sent";
                    case MWFCommunicationStatusEnum.CommunicatedSuccessfully:
                        return "Communication Sent";
                    case MWFCommunicationStatusEnum.ReceivedOnDevice:
                        return "Communication Received On Device";
                    case MWFCommunicationStatusEnum.UnassignNeedsCommunicating:
                        return "Communication Sent";
                    case MWFCommunicationStatusEnum.AcknowledgedOnDevice:
                        return "Driver Acknowledged";
                    default:
                        return string.Empty;
                }
            }
            else
            {
                throw new InvalidCastException("GetTTCommunicationStatusText - CommunicationStatusID is not an int, unable to get status text.");
            }
        }

        #endregion
    }
}
