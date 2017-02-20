using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Data.SqlTypes;

using P1TP.Components.Web.Validation;
using Orchestrator.Globals;
using System.Web.Configuration;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for CallInLog.
	/// </summary>
	public partial class CallInLog : Orchestrator.Base.BasePage
	{
		protected string m_popupJobId = string.Empty;

        #region Protected Properties
        protected string SortExpression
        {
            get { return this.ViewState["_sortExpression"] == null ? "BookedDateTime" : this.ViewState["_sortExpression"].ToString(); }
            set { this.ViewState["_sortExpression"] = value; }
        }

        protected string SortDirection
        {
            get { return this.ViewState["_sortDirection"] == null ? "ASC" : this.ViewState["_sortDirection"].ToString(); }
            set { this.ViewState["_sortDirection"] = value; }
        }
        #endregion

        #region Form Elements

        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            if (IsPostBack)
                return;

			if (!IsPostBack || cboControlArea.Items.Count == 0)
				PopulateStaticControls();

			// Put user code to initialize the page here
			lblError.Visible = false;

            if (Request.QueryString["ca"] != null && !IsPostBack)
            {
                cboControlArea.ClearSelection();
                cboControlArea.Items.FindByValue(Request.QueryString["ca"]).Selected = true;
                LoadReport(string.Empty);
            }
            else
            {
                if (!IsPostBack && Request.QueryString["rcbId"] == null)
                    LoadDefaultView();
            }

                if (Request.QueryString["rcbId"] == null)
		  	    LoadReport(string.Empty);

            
		}


        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

  
		private void PopulateStaticControls()
		{
			// Bind the control areas
			using (Facade.IControlArea facControlArea = new Facade.Traffic())
				cboControlArea.DataSource = facControlArea.GetAll();
			cboControlArea.DataBind();
			cboControlArea.Items.Insert(0, new ListItem("All", "0"));

			// Pre-populate fields if qs elements present
			if (Request.QueryString["prepop"] == "true")
			{
				m_popupJobId = Request.QueryString["jid"];

				cboControlArea.SelectedIndex = Convert.ToInt32(Request.QueryString["ca"]);
				if (Request.QueryString["oi"] != string.Empty)
				{
					using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
					{
						Entities.Organisation client = facOrganisation.GetForIdentityId(Convert.ToInt32(Request.QueryString["oi"]));
						cboClient.Text = client.OrganisationName;
						cboClient.SelectedValue = client.IdentityId.ToString();
					}
				}
			}
		}

		private DateTime ReconstituteDate(string text)
		{
			int day = Convert.ToInt32(text.Substring(0, 2));
			int month = Convert.ToInt32(text.Substring(3, 2));
			int year = Convert.ToInt32(text.Substring(6, 4));

			return new DateTime(year, month, day);
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init +=new EventHandler(CallInLog_Init);
		}
		#endregion

		#region Event Handlers

		private void CallInLog_Init(object sender, EventArgs e)
		{
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
			this.btnFilter.Click +=new EventHandler(btnFilter_Click);
            this.gvJobs.Sorting += new GridViewSortEventHandler(gvJobs_Sorting);
       	}

        void gvJobs_Sorting(object sender, GridViewSortEventArgs e)
        {
            
            if (e.SortExpression != this.SortExpression)
                this.SortExpression = e.SortExpression;
        
            if (this.SortDirection == "ASC")
                this.SortDirection = "DESC";
            else
                this.SortDirection = "ASC";

            LoadReport(e.SortExpression);
            
        }

       

		private void btnFilter_Click(object sender, EventArgs e)
		{
            this.SortExpression = string.Empty;
			LoadReport(string.Empty);
		}

        private void btnPrintReport_Click(object sender, EventArgs e)
        {
                
        }

		#endregion

		#region Active Report

		private void LoadReport(string sortExpression)
		{
            Facade.Job facJob = new Facade.Job();

            int identityId = 0;

            if (cboClient.SelectedValue != "")
                identityId = int.Parse(cboClient.SelectedValue);

            DataSet dsCallInLog = facJob.GetCallInLog(Convert.ToInt32(cboControlArea.SelectedValue), identityId);

            DataView dv = dsCallInLog.DefaultViewManager.CreateDataView(dsCallInLog.Tables[0]);
            if (this.SortExpression != string.Empty)
                dv.Sort = this.SortExpression + " " + this.SortDirection;

            this.gvJobs.DataSource = dv;
            this.gvJobs.DataBind();

            bool gpsActive = bool.Parse(System.Web.Configuration.WebConfigurationManager.AppSettings["GPS.Active"]);
            if (!gpsActive)
                gvJobs.Columns[12].Visible = false;
            if (dsCallInLog.Tables[0].Rows.Count > 0)
            {
                //// Configure the report settings collection
                NameValueCollection reportParams = new NameValueCollection();
                reportParams.Add("selectUrl", "CallInLog.aspx?prepop=true&ca=" + cboControlArea.SelectedValue + "&oi=" + cboClient.SelectedValue);

                // Configure the Session variables used to pass data to the report
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.CallInLog;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsCallInLog;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                // Show the user control
                //reportViewer.Visible = true;

                //reportViewer.IdentityId = identityId;
                //dgJobs.DataSource = dsCallInLog.Tables[0];
                //dgJobs.DataBind();
                lblCallInCount.Text = dsCallInLog.Tables[0].Rows.Count.ToString();
            }
            else
            {
                lblError.Text = "No runs found";
                lblError.Visible = true;
                //reportViewer.Visible = false;
                //dgJobs.Visible = false;
            }
		}

        DateTime m_startDate = DateTime.Now;
        DateTime m_endDate = DateTime.Now;
        int m_controlAreaId = 0;

        private void LoadDefaultView()
        {
            bool cookieExists = GetValuesFromCookie();
            if (cookieExists)
            {
                cboControlArea.Items.FindByValue(m_controlAreaId.ToString()).Selected = true;
                //LoadReport();
            }
        }

        private bool GetValuesFromCookie()
        {
            try
            {
                Entities.TrafficSheetFilter filter = Utilities.GetFilterFromCookie(this.CookieSessionID, this.Request);
                if (filter != null)
                    m_controlAreaId = filter.ControlAreaId;
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected string GetOrderIDs(string orderIDs)
        {
            string retVal = string.Empty;
            string lnk = "<a href='javascript:viewOrder({0});'>{0}</a>";
            foreach (string order in orderIDs.Split('/'))
            {
                if (retVal.Length > 0)
                    retVal += " / ";
                retVal += string.Format(lnk, order);
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
		#endregion

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this.Page);
            }
        }
	}
}
