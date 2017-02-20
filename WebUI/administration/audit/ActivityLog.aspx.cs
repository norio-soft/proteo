using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator.Globals;
using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.administration.audit
{
	/// <summary>
	/// Summary description for ActivityLog.
	/// </summary>
	public partial class ActivityLog : Orchestrator.Base.BasePage
	{
		#region Constants and Enums

		private const string C_ACTIVITY_LOG_DATA_VS = "ActivityLog";
		private enum eDataGridColumns {CreateDate, UserName, Description, ObjectId, EmailRecipient, EmailReportType, FaxNumber, FaxReportType, SMSRecipient, SMSMessageText, OrganisationName};
		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.SystemUsage);

			if (!IsPostBack)
			{
				PopulateStaticControls();
				ViewState["CurrentPageIndex"] = 0;
                LoadGrid();// LoadActivityLog(GetActivityLogDataSet());
			}
			else
			{
				Page.Validate();
				if (Page.IsValid)
                    LoadGrid();  //LoadActivityLog(GetActivityLogDataSet());
			}
		}

		#endregion

		#region Populate Controls

		private void PopulateStaticControls()
		{
			cboAction.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eAuditEvent)));
			cboAction.DataBind();
			cboAction.Items.Insert(0, new ListItem("", ""));
		}

		#endregion

		#region Load Activity Log

        private void LoadGrid()
        {
            Facade.IAuditLog facAuditLog = new Facade.Audit();
            DataSet dsActivityLog;

            bool noCriteria = false;


            if (txtUser.Text == "" && cboAction.SelectedValue == "" && txtJob.Text == "" && dteDateFrom.DateInput.Text == "" && dteDateTo.DateInput.Text == "")
                noCriteria = true;


            if (noCriteria)
            {
                dsActivityLog = facAuditLog.GetAll(0, 500); // Convert.ToInt32(ViewState["CurrentPageIndex"]), Convert.ToInt32(dgActivityLog.PageSize));

                dgActivityLog.DataSource = dsActivityLog;
                dgActivityLog.DataBind();

                dgActivityLog.Levels[0].Columns["ObjectId"].Visible = false; //	dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].Visible = false;				
            }
            else
            {
                // Username
                string userName = null;
                userName = txtUser.Text == "" ? "" : txtUser.Text;

                // AuditEventId
                int auditEventId;
                auditEventId = cboAction.SelectedValue == "" ? -1 : (int)(eAuditEvent)Enum.Parse(typeof(eAuditEvent), (cboAction.SelectedValue.Replace(" ", "")));

                // JobId
                int jobId;
                if (txtJob.Text == "")
                    jobId = -1;
                else
                    jobId = Convert.ToInt32(txtJob.Text);

                // DateFrom
                DateTime dateFrom = DateTime.Today;

                if (!dteDateFrom.SelectedDate.HasValue || dteDateFrom.SelectedDate.Value == DateTime.MinValue)
                    dateFrom = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
                else
                    dateFrom = dteDateFrom.SelectedDate.Value;

                // DateTo
                DateTime dateTo = DateTime.Now;
                if (!dteDateTo.SelectedDate.HasValue || dteDateTo.SelectedDate.Value == DateTime.MinValue)
                    dateTo = Convert.ToDateTime(Convert.ToString(SqlDateTime.MinValue));
                else
                    dateTo = dteDateTo.SelectedDate.Value.AddDays(1);

                dsActivityLog = facAuditLog.GetWithParams(userName, auditEventId, jobId, dateFrom, dateTo, 0,500);//Convert.ToInt32(ViewState["CurrentPageIndex"]), dgActivityLog.PageSize);
                dgActivityLog.DataSource = dsActivityLog;
                dgActivityLog.DataBind();

                // If a job Id is entered, or if an audit event id is entered and it relates to an event
                // raised by job activity, set the ObjectId BoundColumn, which will contain the job ID,
                // to Visible. 
                if (jobId != -1 || auditEventId != -1 && (auditEventId == (int)eAuditEvent.JobMarkedForCancellation || auditEventId == (int)eAuditEvent.JobReinstatedUnmarked || auditEventId == (int)eAuditEvent.JobSetToReadyToInvoice))
                    dgActivityLog.Levels[0].Columns["ObjectId"].HeadingText = "Job Id"; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].HeaderText = "Job Id";
                else if (auditEventId != -1 && auditEventId == (int)eAuditEvent.LegDeleted)
                    dgActivityLog.Levels[0].Columns["ObjectId"].HeadingText = "Leg Id"; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].HeaderText = "Leg Id";
                else if (auditEventId != -1 && auditEventId == (int)eAuditEvent.LegPointDeleted)
                    dgActivityLog.Levels[0].Columns["ObjectId"].HeadingText = "Leg Point Id";// dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].HeaderText = "Leg Point Id";
                else if (auditEventId != -1 && auditEventId == (int)eAuditEvent.EmailSent || auditEventId == (int)eAuditEvent.UserEmailedPalletLog)
                {
                    dgActivityLog.Levels[0].Columns["ObjectId"].Visible = false; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].Visible = false;
                    dgActivityLog.Levels[0].Columns["EmailRecipient"].Visible = true; // dgActivityLog.Columns[(int)eDataGridColumns.EmailRecipient].Visible = true;
                    dgActivityLog.Levels[0].Columns["EmailReportType"].Visible = true; //dgActivityLog.Columns[(int)eDataGridColumns.EmailReportType].Visible = true;
                }
                else if (auditEventId != -1 && auditEventId == (int)eAuditEvent.FaxSent)
                {
                    dgActivityLog.Levels[0].Columns["ObjectId"].Visible = false; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].Visible = false;
                    dgActivityLog.Levels[0].Columns["FaxNumber"].Visible = true; // dgActivityLog.Columns[(int)eDataGridColumns.FaxNumber].Visible = true;
                    dgActivityLog.Levels[0].Columns["FaxReportType"].Visible = true; //    dgActivityLog.Columns[(int)eDataGridColumns.FaxReportType].Visible = true;
                }
                else if (auditEventId != -1 && auditEventId == (int)eAuditEvent.SMSSent)
                {
                    dgActivityLog.Levels[0].Columns["ObjectId"].Visible = false; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].Visible = false;
                    dgActivityLog.Levels[0].Columns["SMSRecipient"].Visible = true; // dgActivityLog.Columns[(int)eDataGridColumns.SMSRecipient].Visible = true;
                    dgActivityLog.Levels[0].Columns["SMSMessageText"].Visible = true; // dgActivityLog.Columns[(int)eDataGridColumns.SMSMessageText].VidgActivityLog.Levels[0].Columns["SMSRecipient"].Visible = true; sible = true;
                }
                else if (auditEventId != -1 && auditEventId == (int)eAuditEvent.PalletBalanceAltered)
                {
                    dgActivityLog.Levels[0].Columns["ObjectId"].Visible = false; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].Visible = false;
                    dgActivityLog.Levels[0].Columns["OrganisationName"].Visible = true; // dgActivityLog.Columns[(int)eDataGridColumns.OrganisationName].Visible = true;
                }
                else
                    dgActivityLog.Levels[0].Columns["ObjectId"].Visible = false; // dgActivityLog.Columns[(int)eDataGridColumns.ObjectId].Visible = false;
            }

            if (dgActivityLog.RecordCount == 0)
            {
                lblError.Visible = true;
                dgActivityLog.Visible = false;
            }
            else
            {
                lblError.Visible = false;
                dgActivityLog.Visible = true;
            }
        }

		private void LoadActivityLog(DataSet dsActivityLog)
		{
			if (dsActivityLog.Tables[0].Rows.Count == 0)
			{
				lblError.Visible = true;
				dgActivityLog.Visible = false;
			}
			else
			{
				lblError.Visible = false;
                dgActivityLog.DataBind();
				dgActivityLog.Visible = true;
			}
		}

		#endregion

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
			this.Init +=new EventHandler(ActivityLog_Init);
		}
		#endregion

		#region Event Handlers

		private void ActivityLog_Init(object sender, EventArgs e)
		{
			btnFilter.Click +=new EventHandler(btnFilter_Click);
			cfvDateFrom.ServerValidate +=new ServerValidateEventHandler(cfvStartDate_ServerValidate);
			cfvJob.ServerValidate +=new ServerValidateEventHandler(cfvJob_ServerValidate);
		}

		private void btnFilter_Click(object sender, EventArgs e)
		{
		}

		#endregion

		#region Validation

		private void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = false;

            if (dteDateFrom.SelectedDate <= dteDateTo.SelectedDate)
				args.IsValid = true;
		}

		private void cfvJob_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
		}

		#endregion

	}
}
