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

using System.Data.Sql;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace Orchestrator.WebUI.Traffic
{
    public partial class Resource : Orchestrator.Base.BasePage
    {
        #region Private fields

        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";

        private SqlDataReader _rdrMyResource = null;
        private SqlDataReader _rdrMyResourceFuture = null;

        #endregion

        protected Entities.TrafficSheetFilter _filter = null;
        bool cookieGenerated = false;

        #region Public Properties
        protected string ControlAreaId
        {
            get
            {
                return _filter.ControlAreaId.ToString(); ;
            }
        }

        protected string StartDateString
        {
            get { return _filter.FilterStartDate.ToString("ddMMyyyy") + "0000"; }
        }

        protected string TrafficAreaIds
        {
            get { return Entities.Utilities.CommaSeparatedIDs(_filter.TrafficAreaIDs); }
        }

        protected string StartDate
        {
            get
            {
                return _filter.FilterStartDate.ToString("dd/MM/yyyy");
            }
        }

        protected string EndDate
        {
            get
            {
                return _filter.FilterEnddate.ToString("dd/MM/yyyy");
            }
        }

        protected int DepotId
        {
            get { return _filter.DepotId; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            if (!((ComponentArt.Web.UI.CallBack)RadPanelbar1.Items[3].Items[0].FindControl("cbJobsComingIn")).IsCallback)
            {
                if (IsPostBack)
                {
                    if (MyClientSideAnchor.OutputData == "")
                    {
                        GetValuesFromCookie();
                    }
                }
                if (!IsPostBack)
                {
                    UseDefaultFilter();

                    // Hook ethe PreRenderComplete Event for the Databinding
                    this.PreRenderComplete += new EventHandler(Traffic_TrafficSheet4_PreRenderComplete);

                    //register the Async Load Methods
                    AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsyncGetMyResource), new EndEventHandler(EndAsyncGetMyResource));
                    AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsyncGetMyResourceFuture), new EndEventHandler(EndAsyncGetMyResourceFuture));
                }
            }
            else
            {
                UseDefaultFilter();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.MyClientSideAnchor.OnWindowClose += new Codesummit.WebModalAnchor.OnWindowCloseEventHandler(MyClientSideAnchor_OnWindowClose);

            if (RadPanelbar1.FindControl("cbJobsComingIn") != null)
                ((ComponentArt.Web.UI.CallBack)RadPanelbar1.FindControl("cbJobsComingIn")).Callback += new ComponentArt.Web.UI.CallBack.CallbackEventHandler(cbJobsComingIn_Callback);
            if (RadPanelbar1.FindControl("cbDriversComingIn") != null)
                ((ComponentArt.Web.UI.CallBack)RadPanelbar1.FindControl("cbDriversComingIn")).Callback += new ComponentArt.Web.UI.CallBack.CallbackEventHandler(cbDriversComingIn_Callback);
            if (RadPanelbar1.FindControl("cbVehiclesComingIn") != null)
                ((ComponentArt.Web.UI.CallBack)RadPanelbar1.FindControl("cbVehiclesComingIn")).Callback += new ComponentArt.Web.UI.CallBack.CallbackEventHandler(cbVehiclesComingIn_Callback);
            if (RadPanelbar1.FindControl("cbTrailersComingIn") != null)
                ((ComponentArt.Web.UI.CallBack)RadPanelbar1.FindControl("cbTrailersComingIn")).Callback += new ComponentArt.Web.UI.CallBack.CallbackEventHandler(cbTrailersComingIn_Callback);
            if (RadPanelbar1.FindControl("cbMyVehicles") != null)
                ((ComponentArt.Web.UI.CallBack)RadPanelbar1.FindControl("cbMyVehicles")).Callback += new ComponentArt.Web.UI.CallBack.CallbackEventHandler(cbMyVehicles_Callback);
            if (RadPanelbar1.FindControl("cbMyTrailers") != null)
                ((ComponentArt.Web.UI.CallBack)RadPanelbar1.FindControl("cbMyTrailers")).Callback += new ComponentArt.Web.UI.CallBack.CallbackEventHandler(cbMyTrailers_Callback);


        }


        #region My Resource Filter Options
        protected void ShowDriverReload_CheckedChanged(object sender, EventArgs e)
        {
            this.PreRenderComplete += new EventHandler(Traffic_TrafficSheet4_PreRenderComplete);
            AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsyncGetMyResource), new EndEventHandler(EndAsyncGetMyResource));
            AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsyncGetMyResourceFuture), new EndEventHandler(EndAsyncGetMyResourceFuture));
        }

        protected void ShowEmptyTrailers_CheckedChanged(object sender, EventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).DataSource = GetMyTrailers();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).DataBind();
            lblInjectScript.Text = "<script type=\"text/javascript\">ExpandTrailers();</script>";

        }

        protected void SortTrailers_CheckedChanged(object sender, EventArgs e)
        {
            DataTable dt = GetMyTrailers();
            DataView dv = dt.DefaultView;
            CheckBox chkSortTrailers = ((CheckBox)RadPanelbar1.FindControl("chkSortTrailersByLastLocation"));
            if (chkSortTrailers.Checked)
                dv.Sort = "LastLocation";

            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).DataSource = dv;

            if (chkSortTrailers.Checked)
                ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).Sort = "LastLocation";
            else
                ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).Sort = String.Empty;

            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).DataBind();
            lblInjectScript.Text = "<script type=\"text/javascript\">ExpandTrailers();</script>";

        }

        protected void ShowVehiclesReload_CheckedChanged(object sender, EventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyVehicles")).DataSource = GetMyVehicles();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyVehicles")).DataBind();
            lblInjectScript.Text = "<script type=\"text/javascript\">ExpandVehicles();</script>";
        }
        #endregion

        void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        {
            UseDefaultFilter();

            // Hook ethe PreRenderComplete Event for the Databinding
            this.PreRenderComplete += new EventHandler(Traffic_TrafficSheet4_PreRenderComplete);

            //register the Async Load Methods
            AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsyncGetMyResource), new EndEventHandler(EndAsyncGetMyResource));
            AddOnPreRenderCompleteAsync(new BeginEventHandler(BeginAsyncGetMyResourceFuture), new EndEventHandler(EndAsyncGetMyResourceFuture));
        }

        #region Callback Handlers
        void cbJobsComingIn_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdJobsComingIn")).DataSource = GetJobsComingIn();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdJobsComingIn")).DataBind();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdJobsComingIn")).RenderControl(e.Output);
        }

        void cbDriversComingIn_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdDriversComingIn")).DataSource = GetDriversComingIn();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdDriversComingIn")).DataBind();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdDriversComingIn")).RenderControl(e.Output);
        }

        void cbVehiclesComingIn_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdVehiclesComingIn")).DataSource = GetVehiclesComingIn();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdVehiclesComingIn")).DataBind();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdVehiclesComingIn")).RenderControl(e.Output);
        }

        void cbTrailersComingIn_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdTrailersComingIn")).DataSource = GetTrailersComingIn();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdTrailersComingIn")).DataBind();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdTrailersComingIn")).RenderControl(e.Output);
        }

        void cbMyVehicles_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyVehicles")).DataSource = GetMyVehicles();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyVehicles")).DataBind();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyVehicles")).RenderControl(e.Output);
        }

        void cbMyTrailers_Callback(object sender, ComponentArt.Web.UI.CallBackEventArgs e)
        {
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).DataSource = GetMyTrailers();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).DataBind();
            ((ComponentArt.Web.UI.Grid)RadPanelbar1.FindControl("grdMyTrailers")).RenderControl(e.Output);
        }
        #endregion

        #region Async Begin Event Handlers
        IAsyncResult BeginAsyncGetMyResource(object sender, EventArgs e, AsyncCallback cb, object state)
        {
            if (_filter.FilterStartDate == DateTime.MinValue)
                UseDefaultFilter();

            string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;

            SqlConnection conMyResource = new SqlConnection(connectionString);
            conMyResource.Open();

            SqlCommand cmdMyResource = new SqlCommand("spResource_GetDepotCodeResources");

            cmdMyResource.Parameters.Add(new SqlParameter("@ControlAreaId", _filter.ControlAreaId));
            cmdMyResource.Parameters.Add(new SqlParameter("@TrafficAreaIds", TrafficAreaIds));
            cmdMyResource.Parameters.Add(new SqlParameter("@StartDate", _filter.FilterStartDate));
            cmdMyResource.Parameters.Add(new SqlParameter("@EndDate", _filter.FilterEnddate));
            cmdMyResource.Parameters.Add(new SqlParameter("@OnlyShowAvailable", false));
            cmdMyResource.Parameters.Add(new SqlParameter("@ResourceTypeId", 3));
            cmdMyResource.Parameters.Add(new SqlParameter("@HomeDepotId", _filter.DepotId));
            cmdMyResource.Parameters.Add(new SqlParameter("@DriverRequestsHoursAhead", Orchestrator.Globals.Configuration.DriverRequestHoursAhead));
            cmdMyResource.Parameters.Add(new SqlParameter("@OnlyShowIfNoFuture", _filter.OnlyShowDriversWithoutReloads));
            cmdMyResource.CommandType = CommandType.StoredProcedure;
            cmdMyResource.Connection = conMyResource;

            return cmdMyResource.BeginExecuteReader(cb, cmdMyResource, CommandBehavior.CloseConnection);

        }

        IAsyncResult BeginAsyncGetMyResourceFuture(object sender, EventArgs e, AsyncCallback cb, object state)
        {
            string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;
            SqlConnection conMyResourceFuture = new SqlConnection(connectionString);
            conMyResourceFuture.Open();

            SqlCommand cmdMyResourceFuture = new SqlCommand("spResource_GetAllResourceFutures");

            cmdMyResourceFuture.CommandType = CommandType.StoredProcedure;
            cmdMyResourceFuture.Parameters.Add(new SqlParameter("@ResourceTypeId", 3));
            cmdMyResourceFuture.Connection = conMyResourceFuture;

            return cmdMyResourceFuture.BeginExecuteReader(cb, cmdMyResourceFuture, CommandBehavior.CloseConnection);

        }

        #endregion

        #region Async End Event Handlers
        void EndAsyncGetMyResource(IAsyncResult ar)
        {
            SqlCommand cmdMyResource = (SqlCommand)ar.AsyncState;
            _rdrMyResource = cmdMyResource.EndExecuteReader(ar);
        }

        void EndAsyncGetMyResourceFuture(IAsyncResult ar)
        {
            SqlCommand cmdMyResourceFuture = (SqlCommand)ar.AsyncState;
            _rdrMyResourceFuture = cmdMyResourceFuture.EndExecuteReader(ar);
        }
        #endregion

        #region Async Page Methods for DataBinding
        void Traffic_TrafficSheet4_PreRenderComplete(object sender, EventArgs e)
        {
            DataReaderAdapter dar = new DataReaderAdapter();

            #region My Resource
            DataTable dtMyDrivers = new DataTable();
            DataTable dtMyTrailers = new DataTable();
            DataTable dtMyVehicles = new DataTable();
            DataTable dtMyResourceFuture = new DataTable();
            CheckBox chkShowDriversWithoutReload = ((CheckBox)RadPanelbar1.FindControl("chkShowDriversWithoutReloads"));

            try
            {
                if (_rdrMyResource != null)
                {
                    dar.FillFromReader(dtMyDrivers, _rdrMyResource);

                    if (_rdrMyResourceFuture != null)
                    {
                        dar.FillFromReader(dtMyResourceFuture, _rdrMyResourceFuture);
                        if (dtMyResourceFuture != null)
                        {
                            DataView dv = null;
                            dv = dtMyResourceFuture.DefaultView;


                            //Fill My Drivers Grid
                            if (dtMyDrivers != null && dtMyDrivers.Columns.Count > 0)
                            {
                                DataView driversWithFuture = dtMyDrivers.DefaultView;
                                //if (_filter.OnlyShowDriversWithoutReloads == false)
                                //{
                                // BUG: t.lunken this will restrict to only people who have got a future booking (Doh!)
                                //driversWithFuture.RowFilter = "HasFuture = 1";

                                PopulateDriverFutures(driversWithFuture, dv);
                                //}

                                ((ComponentArt.Web.UI.Grid)RadPanelbar1.Items[0].Items[0].FindControl("grdMyDrivers")).ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(TSResource_ItemContentCreated);
                                ((ComponentArt.Web.UI.Grid)RadPanelbar1.Items[0].Items[0].FindControl("grdMyDrivers")).DataSource = driversWithFuture;
                                ((ComponentArt.Web.UI.Grid)RadPanelbar1.Items[0].Items[0].FindControl("grdMyDrivers")).DataBind();
                            }
                        }
                    }
                }
            }
            finally
            {
                if (_rdrMyResource != null)
                    _rdrMyResource.Dispose();

                if (_rdrMyResourceFuture != null)
                    _rdrMyResourceFuture.Dispose();
            }

            #endregion
        }


        #region Resource Future Loading
        private void PopulateVehicleFutures(DataView vehiclesWithFuture, DataView dv)
        {
            StringBuilder sbNow = new StringBuilder(string.Empty);
            StringBuilder sbTomorrow = new StringBuilder(string.Empty);

            string linkOpenJobDetails = "<A onmouseover=\"showHelpTipUrl(event,'../point/getLegPointDataForJobHTML.aspx?PointId={0}&amp;InstructionId={1}&amp;InstructionTypeId={2}&amp;BookedTime={3}&amp;IsAnyTime={4}&amp;LegPlannedStart={5}&amp;LegPlannedEnd={6}');\" onmouseout=\"hideHelpTip(this);\" class=\"helpLink\" href=\"javascript:OpenJobDetails({7});\">{8}</A>";
            string linkOpenHoliday = "<span>{0}</span>";
            foreach (DataRow vehicle in vehiclesWithFuture.Table.Rows)
            {
                //if (vehicle["HasFuture"].ToString() == "1")
                //{
                dv.RowFilter = "ResourceId = " + vehicle["ResourceId"].ToString();
                sbNow = new StringBuilder();
                sbTomorrow = new StringBuilder();

                foreach (DataRowView row in dv)
                {
                    if (row["GroupName"].ToString() == "next")
                    {
                        if (row["FutureType"].ToString() == "Leg")
                        {
                            sbNow.Append(string.Format(linkOpenJobDetails, row["PointId"].ToString(),
                                                                           row["InstructionId"].ToString(),
                                                                           row["InstructionTypeId"].ToString(),
                                                                           row["BookedDateTime"].ToString(),
                                                                           row["AnyTime"].ToString() == "True" ? "1" : "0",
                                                                           row["StartDateTime"].ToString(),
                                                                           row["EndDateTime"].ToString(),
                                                                           row["JobId"].ToString(),
                                                                           row["Description"].ToString()));
                        }
                        else
                        {
                            sbNow.Append(string.Format(linkOpenHoliday, row["Description"].ToString() + " from " + row["StartDateTime"].ToString() + " to " + row["EndDateTime"].ToString()));
                        }
                        sbNow.Append("<br/>");
                    }
                    else
                    {
                        if (row["FutureType"].ToString() == "Leg")
                        {
                            sbTomorrow.Append(string.Format(linkOpenJobDetails,
                                                                           row["PointId"].ToString(),
                                                                           row["InstructionId"].ToString(),
                                                                           row["InstructionTypeId"].ToString(),
                                                                           row["BookedDateTime"].ToString(),
                                                                           row["AnyTime"].ToString() == "True" ? "1" : "0",
                                                                           row["StartDateTime"].ToString(),
                                                                           row["EndDateTime"].ToString(),
                                                                           row["JobId"].ToString(),
                                                                           row["Description"].ToString()));
                        }
                        else
                        {
                            sbTomorrow.Append(string.Format(linkOpenHoliday, row["Description"].ToString() + " from " + row["StartDateTime"].ToString() + " to " + row["EndDateTime"].ToString()));
                        }
                        sbTomorrow.Append("<br/>");
                    }
                }

                vehicle["Now"] = sbNow.ToString();
                vehicle["Next"] = sbTomorrow.ToString();
                //}
            }
        }

        private void PopulateTrailerFutures(DataView trailersWithFuture, DataView dv)
        {
            StringBuilder sbNow = new StringBuilder(string.Empty);
            StringBuilder sbTomorrow = new StringBuilder(string.Empty);
            string linkOpenJobDetails = "<A onmouseover=\"showHelpTipUrl(event,'../point/getLegPointDataForJobHTML.aspx?PointId={0}&amp;InstructionId={1}&amp;InstructionTypeId={2}&amp;BookedTime={3}&amp;IsAnyTime={4}&amp;LegPlannedStart={5}&amp;LegPlannedEnd={6}');\" onmouseout=\"hideHelpTip(this);\" class=\"helpLink\" href=\"javascript:OpenJobDetails({7});\">{8}</A>";
            string linkOpenHoliday = "<span>{0}</span>";
            foreach (DataRow trailer in trailersWithFuture.Table.Rows)
            {
                //if (trailer["HasFuture"].ToString() == "1")
                //{
                dv.RowFilter = "ResourceId = " + trailer["TrailerResourceId"].ToString();
                sbNow = new StringBuilder();
                sbTomorrow = new StringBuilder();

                foreach (DataRowView row in dv)
                {


                    if (row["GroupName"].ToString() == "next")
                    {
                        if (row["FutureType"].ToString() == "Leg")
                        {
                            sbNow.Append(string.Format(linkOpenJobDetails, row["PointId"].ToString(),
                                                                           row["InstructionId"].ToString(),
                                                                           row["InstructionTypeId"].ToString(),
                                                                           row["BookedDateTime"].ToString(),
                                                                           row["AnyTime"].ToString() == "True" ? "1" : "0",
                                                                           row["StartDateTime"].ToString(),
                                                                           row["EndDateTime"].ToString(),
                                                                           row["JobId"].ToString(),
                                                                           row["Description"].ToString()));
                        }
                        else
                        {
                            sbNow.Append(string.Format(linkOpenHoliday, row["Description"].ToString() + " from " + row["StartDateTime"].ToString() + " to " + row["EndDateTime"].ToString()));
                        }
                        sbNow.Append("<br/>");
                    }
                    else
                    {
                        if (row["FutureType"].ToString() == "Leg")
                        {
                            sbTomorrow.Append(string.Format(linkOpenJobDetails,
                                                                           row["PointId"].ToString(),
                                                                           row["InstructionId"].ToString(),
                                                                           row["InstructionTypeId"].ToString(),
                                                                           row["BookedDateTime"].ToString(),
                                                                           row["AnyTime"].ToString() == "True" ? "1" : "0",
                                                                           row["StartDateTime"].ToString(),
                                                                           row["EndDateTime"].ToString(),
                                                                           row["JobId"].ToString(),
                                                                           row["Description"].ToString()));
                        }
                        else
                        {
                            sbTomorrow.Append(string.Format(linkOpenHoliday, row["Description"].ToString() + " from " + row["StartDateTime"].ToString() + " to " + row["EndDateTime"].ToString()));
                        }
                        sbTomorrow.Append("<br/>");
                    }
                }

                trailer["Now"] = sbNow.ToString();
                trailer["Next"] = sbTomorrow.ToString();
                //}
            }
        }

        private void PopulateDriverFutures(DataView driversWithFuture, DataView dv)
        {
            StringBuilder sbNow = new StringBuilder(string.Empty);
            StringBuilder sbTomorrow = new StringBuilder(string.Empty);
            string linkOpenJobDetails = "<span onmouseover=\"showHelpTipUrl(event,'../point/getLegPointDataForJobHTML.aspx?PointId={0}&amp;InstructionId={1}&amp;InstructionTypeId={2}&amp;BookedTime={3}&amp;IsAnyTime={4}&amp;LegPlannedStart={5}&amp;LegPlannedEnd={6}');\" onmouseout=\"hideHelpTip(this);\" class=\"helpLink\" onclick=\"javascript:OpenJobDetails({7});\">{8}</span>";
            string linkOpenHoliday = "<span>{0}</span>";
            foreach (DataRow driver in driversWithFuture.Table.Rows)
            {
                //if (driver["HasFuture"].ToString() == "1")
                //{
                dv.RowFilter = "ResourceId = " + driver["DriverResourceId"].ToString();
                sbNow = new StringBuilder();
                sbTomorrow = new StringBuilder();

                foreach (DataRowView row in dv)
                {

                    if (row["GroupName"].ToString() == "next")
                    {
                        if (row["FutureType"].ToString() == "Leg")
                        {
                            sbNow.Append(string.Format(linkOpenJobDetails, row["PointId"].ToString(),
                                                                           row["InstructionId"].ToString(),
                                                                           row["InstructionTypeId"].ToString(),
                                                                           row["BookedDateTime"].ToString(),
                                                                           row["AnyTime"].ToString() == "True" ? "1" : "0",
                                                                           row["StartDateTime"].ToString(),
                                                                           row["EndDateTime"].ToString(),
                                                                           row["JobId"].ToString(),
                                                                           row["Description"].ToString()));
                        }
                        else
                        {
                            sbNow.Append(string.Format(linkOpenHoliday, row["Description"].ToString() + " from " + row["StartDateTime"].ToString() + " to " + row["EndDateTime"].ToString()));
                        }
                        sbNow.Append("<br/>");
                    }
                    else
                    {
                        if (row["FutureType"].ToString() == "Leg")
                        {
                            sbTomorrow.Append(string.Format(linkOpenJobDetails,
                                                                           row["PointId"].ToString(),
                                                                           row["InstructionId"].ToString(),
                                                                           row["InstructionTypeId"].ToString(),
                                                                           row["BookedDateTime"].ToString(),
                                                                           row["AnyTime"].ToString() == "True" ? "1" : "0",
                                                                           row["StartDateTime"].ToString(),
                                                                           row["EndDateTime"].ToString(),
                                                                           row["JobId"].ToString(),
                                                                           row["Description"].ToString()));
                        }
                        else
                        {
                            sbTomorrow.Append(string.Format(linkOpenHoliday, row["Description"].ToString() + " from " + row["StartDateTime"].ToString() + " to " + row["EndDateTime"].ToString()));
                        }
                        sbTomorrow.Append("<br/>");
                    }


                    driver["Now"] = sbNow.ToString();
                    driver["Next"] = sbTomorrow.ToString();
                }
                //}
            }
        }
        #endregion

        #endregion

        #region Cookie Handling

        private bool GetValuesFromCookie()
        {

            _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);

            // 01/05/08 : t.lunken - EH Nichols Addition
            CheckBox chkShowDriversWithoutReloads = ((CheckBox)this.RadPanelbar1.FindControl("chkShowDriversWithoutReloads"));
            CheckBox chkOnlyShowEmptyTrailers = ((CheckBox)this.RadPanelbar1.FindControl("chkOnlyShowEmptyTrailers"));
            CheckBox chkSortTrailersByLastLocation = ((CheckBox)this.RadPanelbar1.FindControl("chkSortTrailersByLastLocation"));
            CheckBox chkShowEmptyVehiclesOnly = ((CheckBox)this.RadPanelbar1.FindControl("chkShowEmptyVehiclesOnly"));

            if (_filter.OnlyShowDriversWithoutReloads != chkShowDriversWithoutReloads.Checked)
            {
                _filter.OnlyShowDriversWithoutReloads = chkShowDriversWithoutReloads.Checked;
                SetCookie(_filter);
            }
            if (_filter.OnlyShowEmptyTrailers != chkOnlyShowEmptyTrailers.Checked)
            {
                _filter.OnlyShowEmptyTrailers = chkOnlyShowEmptyTrailers.Checked;
                SetCookie(_filter);
            }
            if (_filter.SortTrailersByLastLocation != chkSortTrailersByLastLocation.Checked)
            {
                _filter.SortTrailersByLastLocation = chkSortTrailersByLastLocation.Checked;
                SetCookie(_filter);
            }
            if (_filter.OnlyShowEmptyVehicles != chkShowEmptyVehiclesOnly.Checked)
            {
                _filter.OnlyShowEmptyVehicles = chkShowEmptyVehiclesOnly.Checked;
                SetCookie(_filter);
            }

            chkShowDriversWithoutReloads.Checked = _filter.OnlyShowDriversWithoutReloads;
            chkOnlyShowEmptyTrailers.Checked = _filter.OnlyShowEmptyTrailers;
            chkSortTrailersByLastLocation.Checked = _filter.SortTrailersByLastLocation;
            chkShowEmptyVehiclesOnly.Checked = _filter.OnlyShowEmptyVehicles;

            return true;
        }

        private void SetCookie(Entities.TrafficSheetFilter ts)
        {
            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, ts);
        }


        private void UseDefaultFilter()
        {

            try
            {
                _filter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
                // Set the Filters
                ((CheckBox)this.RadPanelbar1.FindControl("chkShowDriversWithoutReloads")).Checked = _filter.OnlyShowDriversWithoutReloads;
                ((CheckBox)this.RadPanelbar1.FindControl("chkOnlyShowEmptyTrailers")).Checked = _filter.OnlyShowEmptyTrailers;
                ((CheckBox)this.RadPanelbar1.FindControl("chkSortTrailersByLastLocation")).Checked = _filter.SortTrailersByLastLocation;
                ((CheckBox)this.RadPanelbar1.FindControl("chkShowEmptyVehiclesOnly")).Checked = _filter.OnlyShowEmptyVehicles;

                // To ensure that the dates are always valid check here
                if (_filter.FilterStartDate == DateTime.MinValue)
                    GenerateCookie();
            }
            catch
            {
                // invalid or old values stored in the cookie simply regenerate.
                GenerateCookie();
            }
        }

        private void GenerateCookie()
        {
            Facade.ITrafficSheetFilter facTrafficSheetFilter = new Facade.Traffic();
            _filter = facTrafficSheetFilter.GetDefault(((Entities.CustomPrincipal)Page.User).IdentityId);

            // 02/05/08 : t.lunken : EH Nicols Additions
            _filter.OnlyShowDriversWithoutReloads = false;
            _filter.OnlyShowEmptyTrailers = false;
            _filter.SortTrailersByLastLocation = false;
            _filter.OnlyShowEmptyVehicles = false;

            #region // Configure the default dates.
            // Default dates are from the start of today until:
            //   1) On a Saturday, until the end of Monday.
            //   2) On any other day, until the end of tomorrow.
            DateTime startOfToday = DateTime.Now;
            startOfToday = startOfToday.Subtract(startOfToday.TimeOfDay);
            DateTime endOfTomorrow = startOfToday.Add(new TimeSpan(1, 23, 59, 59));
            _filter.FilterStartDate = startOfToday;

            //ViewState["m_startDate"] = startOfToday;
            if (startOfToday.DayOfWeek == DayOfWeek.Saturday)
            {
                DateTime endOfMonday = startOfToday.Add(new TimeSpan(2, 23, 59, 59));
                //ViewState["m_endDate"] = endOfMonday;
                _filter.FilterEnddate = endOfMonday;
            }
            else
                _filter.FilterEnddate = endOfTomorrow;
            //ViewState["m_endDate"] = endOfTomorrow;

            #endregion

            Utilities.SetTrafficSheetCookie(this.CookieSessionID, this.Response, _filter);

            cookieGenerated = true;
        }

        #endregion

        #region CallBack Related Calls for Grid Loading
        int jobsComingInErrorCount = 0;
        DataTable GetJobsComingIn()
        {

            try
            {

                string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;
                using (SqlConnection conJobsCominIn = new SqlConnection(connectionString))
                {
                    conJobsCominIn.Open();

                    SqlCommand cmdJobsComingIn = new SqlCommand("spJob_GetJobsComingIntoArea");

                    cmdJobsComingIn.CommandType = CommandType.StoredProcedure;
                    cmdJobsComingIn.Connection = conJobsCominIn;
                    cmdJobsComingIn.Parameters.Clear();
                    cmdJobsComingIn.Parameters.Add(new SqlParameter("@TrafficAreas", TrafficAreaIds));

                    SqlDataReader dr = cmdJobsComingIn.ExecuteReader();
                    if (dr != null)
                    {
                        DataReaderAdapter dar = new DataReaderAdapter();
                        DataTable dt = new DataTable();
                        dar.FillFromReader(dt, dr);
                        if (dt != null && dt.Columns.Count > 0)
                        {
                            conJobsCominIn.Close();

                            return dt;
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                if (Utilities.MapSqlException(sqle) && jobsComingInErrorCount < 5)
                {
                    jobsComingInErrorCount++;
                    return GetJobsComingIn();

                }
                else
                    throw;

            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        int getDriversComingInErrorCount = 0;
        DataTable GetDriversComingIn()
        {
            try
            {

                string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;
                using (SqlConnection conDriversCominIn = new SqlConnection(connectionString))
                {
                    conDriversCominIn.Open();

                    SqlCommand cmdDriversComingIn = new SqlCommand("spDriver_GetLastPosition");

                    cmdDriversComingIn.CommandType = CommandType.StoredProcedure;
                    cmdDriversComingIn.Connection = conDriversCominIn;
                    cmdDriversComingIn.Parameters.Add(new SqlParameter("@TrafficAreas", TrafficAreaIds));

                    SqlDataReader dr = cmdDriversComingIn.ExecuteReader();

                    if (dr != null)
                    {
                        DataReaderAdapter dar = new DataReaderAdapter();
                        DataTable dt = new DataTable();
                        dar.FillFromReader(dt, dr);
                        if (dt != null && dt.Columns.Count > 0)
                        {
                            conDriversCominIn.Close();

                            return dt;
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                if (Utilities.MapSqlException(sqle) && getDriversComingInErrorCount < 5)
                {
                    getDriversComingInErrorCount++;
                    return GetDriversComingIn();

                }
                else
                    throw;

            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        int getVehiclesComingInErrorCount = 0;
        DataTable GetVehiclesComingIn()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;
                using (SqlConnection conVehiclesCominIn = new SqlConnection(connectionString))
                {
                    conVehiclesCominIn.Open();

                    SqlCommand cmdVehiclesComingIn = new SqlCommand("spVehicle_GetLastPosition");

                    cmdVehiclesComingIn.CommandType = CommandType.StoredProcedure;
                    cmdVehiclesComingIn.Connection = conVehiclesCominIn;
                    cmdVehiclesComingIn.Parameters.Add(new SqlParameter("@TrafficAreas", TrafficAreaIds));

                    SqlDataReader dr = cmdVehiclesComingIn.ExecuteReader();

                    if (dr != null)
                    {
                        DataReaderAdapter dar = new DataReaderAdapter();
                        DataTable dt = new DataTable();
                        dar.FillFromReader(dt, dr);
                        if (dt != null && dt.Columns.Count > 0)
                        {
                            conVehiclesCominIn.Close();

                            return dt;
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                if (Utilities.MapSqlException(sqle) && getVehiclesComingInErrorCount < 5)
                {
                    getVehiclesComingInErrorCount++;
                    return GetVehiclesComingIn();

                }
                else
                    throw;

            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        int getTrailersComingInErrorCount = 0;
        DataTable GetTrailersComingIn()
        {

            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;
                using (SqlConnection conTrailersCominIn = new SqlConnection(connectionString))
                {
                    conTrailersCominIn.Open();

                    SqlCommand cmdTrailersComingIn = new SqlCommand("spTrailer_GetLastPosition");

                    cmdTrailersComingIn.CommandType = CommandType.StoredProcedure;
                    cmdTrailersComingIn.Connection = conTrailersCominIn;
                    cmdTrailersComingIn.Parameters.Add(new SqlParameter("@TrafficAreas", TrafficAreaIds));

                    SqlDataReader dr = cmdTrailersComingIn.ExecuteReader();

                    if (dr != null)
                    {
                        DataReaderAdapter dar = new DataReaderAdapter();
                        DataTable dt = new DataTable();
                        dar.FillFromReader(dt, dr);
                        if (dt != null && dt.Columns.Count > 0)
                        {
                            conTrailersCominIn.Close();

                            return dt;
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                if (Utilities.MapSqlException(sqle) && getTrailersComingInErrorCount < 5)
                {
                    getTrailersComingInErrorCount++;
                    return GetTrailersComingIn();

                }
                else
                    throw;

            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        int getMyVehiclesErrorCount = 0;
        DataTable GetMyVehicles()
        {
            try
            {
                string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;

                using (SqlConnection conMyResource = new SqlConnection(connectionString))
                {
                    conMyResource.Open();

                    SqlCommand cmdMyResource = new SqlCommand("spResource_GetDepotCodeResources");

                    cmdMyResource.Parameters.Add(new SqlParameter("@ControlAreaId", _filter.ControlAreaId));
                    cmdMyResource.Parameters.Add(new SqlParameter("@TrafficAreaIds", TrafficAreaIds));
                    cmdMyResource.Parameters.Add(new SqlParameter("@StartDate", _filter.FilterStartDate));
                    cmdMyResource.Parameters.Add(new SqlParameter("@EndDate", _filter.FilterEnddate));
                    cmdMyResource.Parameters.Add(new SqlParameter("@OnlyShowAvailable", false));
                    cmdMyResource.Parameters.Add(new SqlParameter("@ResourceTypeId", 1));
                    cmdMyResource.Parameters.Add(new SqlParameter("@HomeDepotId", _filter.DepotId));
                    cmdMyResource.Parameters.Add(new SqlParameter("@DriverRequestsHoursAhead", Orchestrator.Globals.Configuration.DriverRequestHoursAhead));
                    cmdMyResource.Parameters.Add(new SqlParameter("@OnlyShowIfNoFuture", _filter.OnlyShowEmptyVehicles));

                    cmdMyResource.CommandType = CommandType.StoredProcedure;
                    cmdMyResource.Connection = conMyResource;

                    DataReaderAdapter dar = new DataReaderAdapter();
                    SqlDataReader dr = cmdMyResource.ExecuteReader();

                    if (dr != null)
                    {
                        DataTable dtMyVehicles = new DataTable();
                        dar.FillFromReader(dtMyVehicles, dr);
                        if (dtMyVehicles != null && dtMyVehicles.Columns.Count > 0)
                        {
                            using (SqlConnection conMyResourceFuture = new SqlConnection(connectionString))
                            {
                                conMyResourceFuture.Open();

                                SqlCommand cmdMyResourceFuture = new SqlCommand("spResource_GetAllResourceFutures");

                                cmdMyResourceFuture.CommandType = CommandType.StoredProcedure;
                                cmdMyResourceFuture.Parameters.Add(new SqlParameter("@ResourceTypeId", 1));
                                cmdMyResourceFuture.Connection = conMyResourceFuture;

                                _rdrMyResourceFuture = cmdMyResourceFuture.ExecuteReader();

                                if (_rdrMyResourceFuture != null)
                                {
                                    DataTable dtMyResourceFuture = new DataTable();
                                    dar.FillFromReader(dtMyResourceFuture, _rdrMyResourceFuture);
                                    if (dtMyResourceFuture != null)
                                    {
                                        DataView dv = null;
                                        dv = dtMyResourceFuture.DefaultView;


                                        DataView vehiclesWithFuture = dtMyVehicles.DefaultView;
                                        vehiclesWithFuture.RowFilter = "HasFuture = 1";
                                        PopulateVehicleFutures(vehiclesWithFuture, dv);
                                        dtMyVehicles.DefaultView.RowFilter = "";
                                        conMyResource.Close();

                                        return dtMyVehicles;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                if (Utilities.MapSqlException(sqle) && getMyVehiclesErrorCount < 5)
                {
                    getMyVehiclesErrorCount++;
                    return GetMyVehicles();
                }
                else
                    throw;

            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }

        int getMyTrailersErrorCount = 0;
        DataTable GetMyTrailers()
        {
            try
            {
               

                string connectionString = WebConfigurationManager.ConnectionStrings["Orchestrator"].ConnectionString;

                using (SqlConnection conMyResource = new SqlConnection(connectionString))
                {
                    conMyResource.Open();

                    SqlCommand cmdMyResource = new SqlCommand("spResource_GetDepotCodeResources");

                    cmdMyResource.Parameters.Add(new SqlParameter("@ControlAreaId", _filter.ControlAreaId));
                    cmdMyResource.Parameters.Add(new SqlParameter("@TrafficAreaIds", TrafficAreaIds));
                    cmdMyResource.Parameters.Add(new SqlParameter("@StartDate", _filter.FilterStartDate));
                    cmdMyResource.Parameters.Add(new SqlParameter("@EndDate", _filter.FilterEnddate));
                    cmdMyResource.Parameters.Add(new SqlParameter("@OnlyShowAvailable", false));
                    cmdMyResource.Parameters.Add(new SqlParameter("@ResourceTypeId", 2));
                    cmdMyResource.Parameters.Add(new SqlParameter("@HomeDepotId", _filter.DepotId));
                    cmdMyResource.Parameters.Add(new SqlParameter("@DriverRequestsHoursAhead", Orchestrator.Globals.Configuration.DriverRequestHoursAhead));
                    cmdMyResource.Parameters.Add(new SqlParameter("@OnlyShowIfNoFuture", _filter.OnlyShowEmptyTrailers));

                    cmdMyResource.CommandType = CommandType.StoredProcedure;
                    cmdMyResource.Connection = conMyResource;

                    DataReaderAdapter dar = new DataReaderAdapter();
                    SqlDataReader dr = cmdMyResource.ExecuteReader();

                    if (dr != null)
                    {
                        DataTable dtMyTrailers = new DataTable();
                        dar.FillFromReader(dtMyTrailers, dr);

                        if (dtMyTrailers != null && dtMyTrailers.Columns.Count > 0)
                        {
                            using (SqlConnection conMyResourceFuture = new SqlConnection(connectionString))
                            {
                                conMyResourceFuture.Open();

                                SqlCommand cmdMyResourceFuture = new SqlCommand("spResource_GetAllResourceFutures");

                                cmdMyResourceFuture.CommandType = CommandType.StoredProcedure;
                                cmdMyResourceFuture.Parameters.Add(new SqlParameter("@ResourceTypeId", 2));
                                cmdMyResourceFuture.Connection = conMyResourceFuture;

                                _rdrMyResourceFuture = cmdMyResourceFuture.ExecuteReader();

                                if (_rdrMyResourceFuture != null)
                                {
                                    DataTable dtMyResourceFuture = new DataTable();
                                    dar.FillFromReader(dtMyResourceFuture, _rdrMyResourceFuture);
                                    if (dtMyResourceFuture != null)
                                    {
                                        DataView dv = null;
                                        dv = dtMyResourceFuture.DefaultView;


                                        DataView trailersWithFuture = dtMyTrailers.DefaultView;
                                        trailersWithFuture.RowFilter = "HasFuture = 1";
                                        PopulateTrailerFutures(trailersWithFuture, dv);
                                        dtMyTrailers.DefaultView.RowFilter = "";
                                        conMyResource.Close();

                                        return dtMyTrailers;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (SqlException sqle)
            {
                if (Utilities.MapSqlException(sqle) && getMyTrailersErrorCount < 5)
                {
                    getMyTrailersErrorCount++;
                    return GetMyTrailers();
                }
                else
                    throw;

            }
            catch (Exception e)
            {
                throw e;
            }

            return null;
        }


        #endregion

        #region Context Menu Creation

        void TSResource_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            string jobId = e.Item["JobId"].ToString();
            string legId = e.Item["LegId"].ToString();

            //string rowId = e.Content.ClientID;

            //string functionName = "ShowDriverContextMenu('{0}', '{1}', '{2}',{3});";
            //functionName = string.Format(functionName, jobId, legId, "0", rowId);
            //e.Content. .Add("onmousedown", functionName);
        }
        #endregion

    }

   
}
