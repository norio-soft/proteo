using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	
	/// <summary>
	/// Summary description for tabReturns.
	/// </summary>
	public partial class tabReturns : Orchestrator.Base.BasePage
	{

		private const string C_JOB_VS = "C_JOB_VS";
		private const string C_POINT_ID_VS = "C_POINT_ID_VS";

		#region Fields
		
		#region Private 

		#endregion

		#region Protected
		protected	Entities.Job			m_job				= null;
		protected	int						m_jobId				= 0;	// The id of the job we are currently manipulating.
		protected	int						m_instructionId		= 0;	// The id of the job we are currently manipulating.
		protected	int						m_pointId			= 0;	// The id of the point visited in the job we are currently manipulating.
		

		#endregion

		#endregion
	
		#region Page Load/Init
		protected void Page_Load(object sender, System.EventArgs e)
		{
			
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);
			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);
			if (!IsPostBack)
			{
				// Bind the job information.
				BindJob();
				BindInstruction();
				BindGoods();

				dgGoods.ItemDataBound += new DataGridItemEventHandler(dgGoods_ItemDataBound);
				dgGoods.ItemCommand += new DataGridCommandEventHandler(dgGoods_ItemCommand);
			}
			else
			{
				m_job = (Entities.Job) ViewState[C_JOB_VS];
				m_pointId = (int) ViewState[C_POINT_ID_VS];
			}

            if (((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
                this.buttonBar.Visible = false;
		}
		#endregion

		#region Data Loading.Binding

		private void dgGoods_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView row = (DataRowView) e.Item.DataItem;

                ////////////////////////////////////////////////////////
                int refusalId = (int)row["RefusalId"];
                eGoodsRefusedType type = (eGoodsRefusedType)row["RefusalTypeId"];
                HtmlAnchor hypRefusal = (HtmlAnchor)e.Item.FindControl("hypRefusal");
                if (type == eGoodsRefusedType.Shorts || type == eGoodsRefusedType.OverAndAccepted)
                {
                    hypRefusal.HRef = "shortage.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&refusalId=" + refusalId + "&t=2" + "&csid=" + this.CookieSessionID;
                    hypRefusal.InnerText = refusalId.ToString();
                }
                else
                {
                    hypRefusal.HRef = "refusal.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&refusalId=" + refusalId + "&t=1" + this.CookieSessionID;
                    hypRefusal.InnerText = refusalId.ToString();
                }

                ///////////////////////////////////////////////////////

				int storePointId = 0;
				if (row["StorePointId"] != DBNull.Value)
					storePointId = (int) row["StorePointId"];
				int returnPointId = 0;
				if (row["ReturnPointId"] != DBNull.Value)
					returnPointId = (int) row["ReturnPointId"];

				if (storePointId != 0 || returnPointId != 0)
				{
                    Facade.IPoint facPoint = new Facade.Point();
					if (storePointId != 0)
					{
						Label lblStoreAt = (Label) e.Item.FindControl("lblStoreAt");

						Entities.Point storePoint = facPoint.GetPointForPointId(storePointId);
						lblStoreAt.Text = storePoint.OrganisationName + ", " + storePoint.PostTown.TownName;

                        Uri pointAddressUri = new Uri(Request.Url, Page.ResolveUrl("~/Point/GetPointAddressHtml.aspx"));
						lblStoreAt.Attributes.Add("onMouseOver", "ShowPoint('" + pointAddressUri.ToString() + "', '" + storePointId.ToString() + "')");
						lblStoreAt.Attributes.Add("onMouseOut", "HidePoint()");
					}

					if (returnPointId != 0)
					{
						Label lblReturnTo = (Label) e.Item.FindControl("lblReturnTo");

						Entities.Point returnPoint = facPoint.GetPointForPointId(returnPointId);
						lblReturnTo.Text = returnPoint.OrganisationName + ", " + returnPoint.PostTown.TownName;

                        Uri pointAddressUri = new Uri(Request.Url, Page.ResolveUrl("~/Point/GetPointAddressHtml.aspx"));
						lblReturnTo.Attributes.Add("onMouseOver", "ShowPoint('" + pointAddressUri.ToString() + "', '" + returnPointId.ToString() + "')");
						lblReturnTo.Attributes.Add("onMouseOut", "HidePoint()");
					}
				}

				// Only allow goods of status Outstanding to be edited or deleted (unless this is a return job).
				if (row["RefusalStatusId"] != DBNull.Value)
				{
					eGoodsRefusedStatus status = (eGoodsRefusedStatus) row["RefusalStatusId"];

                    //if ((status != eGoodsRefusedStatus.Outstanding && type != eGoodsRefusedType.Shorts && type != eGoodsRefusedType.OverAndAccepted) && m_job.JobType != eJobType.Return)
                    //{
                    //    // This has been commented out following a desire by Tony to be able to view the details - instead the Update Goods button is disabled.
                    //    //((Button) e.Item.Cells[8].Controls[0]).Enabled = false;
                    //    ((Button) e.Item.Cells[8].Controls[0]).Text = "View";
                    //    ((Button) e.Item.Cells[10].Controls[0]).Enabled = false;
                    //}
				}
			}
		}

		private void dgGoods_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				// The id of the goods refusal we are working with.
				int refusalId = Convert.ToInt32(e.Item.Cells[1].Text);

				switch (e.CommandName.ToLower())
				{
					case "edit":
						// Check the refusal type.
						eGoodsRefusedType type = (eGoodsRefusedType) Enum.Parse(typeof(eGoodsRefusedType), e.Item.Cells[1].Text.Replace(" ", ""));

						if (type == eGoodsRefusedType.Shorts || type == eGoodsRefusedType.OverAndAccepted)
						{
                            Response.Redirect("shortage.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&refusalId=" + refusalId + "&csid=" + this.CookieSessionID);
						}
						else
						{
                            Response.Redirect("refusal.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&refusalId=" + refusalId + "&csid=" + this.CookieSessionID);
						}
						break;
					case "delete":
						Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.TakeCallIn);
						bool success = false;

						using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
							success = facGoodsRefusal.Delete(refusalId, this.Page.User.Identity.Name);

						if (success)
						{
							BindGoods();
						}
						break;
				}
			}
		}

		/// <summary>
		/// Retrieves the goods that have been returned/refused for this point and populates the goods datagrid.
		/// </summary>
		private void BindGoods()
		{
			using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
			{
				DataSet dsGoods = facGoodsRefusal.GetRefusalsForInstructionId(m_instructionId);
				dgGoods.DataSource = dsGoods;
				dgGoods.DataBind();

				int goodsCount = 0;
				foreach (DataGridItem item in dgGoods.Items)
					if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
						goodsCount++;

				if (goodsCount == 0)
				{
					lblGoodsCount.Text = "There are no goods stored.";
					dgGoods.Visible = false;
				}
				else
				{
					lblGoodsCount.Text = "There are " + goodsCount.ToString() + " refused or returned goods stored.";
					dgGoods.Visible = true;
				}
			}
		}

		private void BindJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				LoadJob();				
			}
		}

		private void LoadJob()
		{
			using (Facade.IJob facJob = new Facade.Job())
			{
				m_job = facJob.GetJob(m_jobId, true);

				if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);
			}

			ViewState[C_JOB_VS] = m_job;
		}

		private void BindInstruction()
		{
			Entities.Instruction instruction = null;

			// Get the instruction to work with.
			using (Facade.IInstruction facInstruction = new Facade.Instruction())
			{
				if (m_instructionId == 0)
				{
					// Get the job's next instruction.
					instruction = facInstruction.GetNextInstruction(m_jobId);
				}
				else
				{
					// Get the specific instruction.
					instruction = facInstruction.GetInstruction(m_instructionId);
				}
			}

			if (instruction != null)
			{
				m_pointId = instruction.PointID;
				ViewState[C_POINT_ID_VS]  = m_pointId;
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
			this.dgGoods.ItemCommand += new DataGridCommandEventHandler(dgGoods_ItemCommand);
			this.dgGoods.ItemDataBound +=new DataGridItemEventHandler(dgGoods_ItemDataBound);

		}
		#endregion
	}
}
