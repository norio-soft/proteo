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
using Orchestrator.WebUI.Controls;
using P1TP.Components.Web.Validation;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for LoadReleaseNotification.
	/// </summary>
	public partial class LoadReleaseNotification : Orchestrator.Base.BasePage
	{

		#region Constants and Enums
	
		private enum eDataGridColumns {
            JobId, 
            LoadNo, 
            Client, 
            Destination, 
            DeliveryTime,
            Driver, 
            RegNo,
            TrailerRef
        };

		#endregion

		#region Private Fields

		private int m_instructionId;
		private int m_jobId;

		#endregion

		#region Form Elements


		#endregion

		#region Page Init/Load/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			if (!IsPostBack)
			{
				PopulateDataGrid();

                reportViewer.Visible = false;
			}
		}

		#endregion

		#region Populate Data Grid

		private void PopulateDataGrid()
		{
			Facade.ReferenceData facReferenceData = new Facade.ReferenceData();
			dgJobCollections.DataSource = facReferenceData.GetCollectionsForJobId(m_jobId);
			dgJobCollections.DataBind();
		}

		private void GetDocketNumbers(int instructionId)
		{
			using (Facade.IInstruction facInstruction = new Facade.Instruction())
			{
				Entities.Instruction selected = facInstruction.GetInstruction(instructionId);

				string docketCollection = string.Empty;

				if (selected != null)
					foreach (Entities.CollectDrop cd in selected.CollectDrops)
					{
						if (docketCollection.Length > 0)
							docketCollection += "/";
						docketCollection += cd.Docket;
					}

				txtRef.Text = docketCollection;
			}
		}

		#endregion

		#region DataGrid Event Handlers

		protected void rbgCollection_CheckedChanged(object sender, EventArgs e)
		{
			RdoBtnGrouper grouper = (RdoBtnGrouper) sender;

			if (grouper.Checked)
			{
				btnReport.Enabled = true;
				pnlEnterInformation.Visible = true;
				DataGridItem dgi = (DataGridItem) grouper.Parent.Parent;
				ViewState["DataGridItemIndex" ] = dgi.ItemIndex;

				// Get the docket numbers being collected at this point.
				int instructionId = Convert.ToInt32(dgi.Cells[9].Text);
				GetDocketNumbers(instructionId);
			}
		}

		protected void dgJobCollections_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			// If any null values within DataGrid's DataSource, DataGrid will change these
			// to &nbsp. Since passing data to the report, set any such cells' Text
			// property to an empty string, "", so &nbsp is not displayed in the report.
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				for (int i=0; i < e.Item.Cells.Count; i++)
				{
					if (e.Item.Cells[i].Text == "&nbsp;")
						e.Item.Cells[i].Text = "";
				}
			}

			if ((e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem) && m_instructionId == int.Parse(e.Item.Cells[9].Text))
			{
				pnlEnterInformation.Visible = true;
				btnReport.Enabled = true;
				RdoBtnGrouper rbgCollection = (RdoBtnGrouper) e.Item.FindControl("rbgCollection");
				if (rbgCollection != null)
				{
					ViewState["DataGridItemIndex"] = e.Item.ItemIndex;
					rbgCollection.Checked = true;

					// Get the docket numbers being collected at this point.
					GetDocketNumbers(m_instructionId);
				}
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
			this.Init +=new EventHandler(LoadReleaseNotification_Init);
			this.dgJobCollections.ItemDataBound +=new DataGridItemEventHandler(dgJobCollections_ItemDataBound);
		}
		#endregion

		#region Event Handlers

		private void LoadReleaseNotification_Init(object sender, EventArgs e)
		{
			btnReport.Click +=new EventHandler(btnReport_Click);
		}

		private void btnReport_Click(object sender, EventArgs e)
		{
            btnReport.DisableServerSideValidation();

            if (Page.IsValid)
        		LoadReport();
		}

		#endregion

		#region ActiveReport

		private void LoadReport()
		{
			int itemIndex = Convert.ToInt32(ViewState["DataGridItemIndex"]);
			
			NameValueCollection reportParams = new NameValueCollection();
			reportParams.Add("LoadNo", dgJobCollections.Items[itemIndex].Cells[(int)eDataGridColumns.LoadNo].Text);
			reportParams.Add("Destination", dgJobCollections.Items[itemIndex].Cells[(int)eDataGridColumns.Destination].Text);
			reportParams.Add("Driver", dgJobCollections.Items[itemIndex].Cells[(int)eDataGridColumns.Driver].Text);
			reportParams.Add("VehicleReg", dgJobCollections.Items[itemIndex].Cells[(int)eDataGridColumns.RegNo].Text);
            reportParams.Add("TrailerRef", dgJobCollections.Items[itemIndex].Cells[(int)eDataGridColumns.TrailerRef].Text);
            reportParams.Add("DeliveryTime", dgJobCollections.Items[itemIndex].Cells[(int)eDataGridColumns.DeliveryTime].Text);
            reportParams.Add("To", txtTo.Text);
			reportParams.Add("From", txtFrom.Text);
			reportParams.Add("Ref", txtRef.Text);

			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
			Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.LoadReleaseFax;
			Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

			// Show the user control
			reportViewer.Visible = true;
			using (Facade.IJob facJob = new Facade.Job())
				reportViewer.IdentityId = facJob.GetJob(m_jobId).IdentityId;
		}

		#endregion
	}
}
