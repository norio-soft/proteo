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
using System.Text;

using P1TP.Components.Web.Validation;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for ListDeliveryAndReturnsLog.
	/// </summary>
	public partial class ListDeliveryAndReturnsLog : Orchestrator.Base.BasePage
	{
		#region Constants

		// Number of client references to include
		private const int C_REFERENCE_COUNT_VS = 2;

		#endregion
		
		#region Form Elements

		protected System.Web.UI.WebControls.RequiredFieldValidator	rfvClient;
		protected System.Web.UI.WebControls.RequiredFieldValidator	rfvDateFrom;
		protected System.Web.UI.WebControls.RequiredFieldValidator	rfvDateTo;

		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				//dteStartDate.Date = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
				//dteEndDate.Date = DateTime.Today;
				
				// TODO:	1.	Work out what log frequency to use.
				//			2.	Check whether the log has been sent.  By checking the mail and fax tables
				//			3.	If not highlight that this hasn't and show the time in hours before it is due.
				//			(Show for all clients initially for this particular day)
				//			Also check whether the client has had any deliverys and returns for that timeframe given
				//			if not do not highlight within grid.
			
				// TODO:	Time frames required to be added to the calculation.
				// TODO:	The generate button/client drop not required on first access only when drilled down.

				LoadGrid();
			}
		}

		protected void DeliveryAndReturnsLog_Init(object sender, System.EventArgs e)
		{
			this.dgLogs.ItemDataBound +=new DataGridItemEventHandler(dgLogs_ItemDataBound);
			this.btnGenerate.Click +=new EventHandler(btnGenerate_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
		}
		#endregion

		#region Delivery And Returns Logs Grid
		protected void dgLogs_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgLogs.CurrentPageIndex = e.NewPageIndex;   
		}

		private void LoadGrid()
		{
			Facade.IReferenceData facRef = new Facade.ReferenceData();
			
			DataSet dsLogs = facRef.GetUnsentLogs();

			if (dsLogs != null)
			{
				if (dsLogs.Tables[0].Rows.Count == 0)
				{
					lblError.Text = "No logs require to be sent.";
					
					lblError.Visible = true;
					pnlLogs.Visible = false;
				}
				else
				{	
					DataView dv = new DataView(dsLogs.Tables[0]);
					
					if (cboClient.SelectedValue != string.Empty)
					{
                        if ((dteStartDate.SelectedDate != DateTime.MinValue) || (dteEndDate.SelectedDate != DateTime.MinValue))
						{
                            dv.RowFilter = "IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + " AND DateTimeFrom >= '" + dteStartDate.SelectedDate.Value.ToString("dd/MMM/yyyy HH:mm") + "' AND DateTimeTo <= '" + dteEndDate.SelectedDate.Value.ToString("dd/MMM/yyyy HH:mm") + "'";
							
							dgLogs.DataSource = dv;
							dgLogs.DataBind();
							
							if(dgLogs.Items.Count == 0)
							{
                                lblError.Text = "No logs for client " + cboClient.Text + " for period " + dteStartDate.SelectedDate.Value.ToString("dd/MM/yy") + " to " + dteEndDate.SelectedDate.Value.ToString("dd/MM/yy");
								
								lblError.Visible = true;
								pnlLogs.Visible = false;
							}
							else
							{
								lblError.Visible = false;
								pnlLogs.Visible = true;
							}
						}
						else
						{
							dv.RowFilter =  "IdentityId=" + Convert.ToInt32(cboClient.SelectedValue);
						
							dgLogs.DataSource = dv;
							dgLogs.DataBind();

							if(dgLogs.Items.Count == 0)
							{
								lblError.Text = "No logs for client " + cboClient.Text + ".";
								
								lblError.Visible = true;
								pnlLogs.Visible = false;
							}
							else
							{
								lblError.Visible = false;
								pnlLogs.Visible = true;
							}
						}
					}
					else
					{
                        if ((dteStartDate.SelectedDate != DateTime.MinValue) || (dteEndDate.SelectedDate != DateTime.MinValue))
						{
                            dv.RowFilter = "DateTimeFrom >= '" + dteStartDate.SelectedDate.Value.ToString("dd/MMM/yyyy HH:mm") + "' AND DateTimeTo <= '" + dteEndDate.SelectedDate.Value.ToString("dd/MMM/yyyy HH:mm") + "'";
							
							dgLogs.DataSource = dv;
							dgLogs.DataBind();
							
							if(dgLogs.Items.Count == 0)
							{
                                lblError.Text = "No logs for period " + dteStartDate.SelectedDate.Value.ToString("dd/MM/yy") + " to " + dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"); ;
								
								lblError.Visible = true;
								pnlLogs.Visible = false;
							}
							else
							{
								lblError.Visible = false;
								pnlLogs.Visible = true;
							}
						}
						else
						{
							dgLogs.DataSource = dsLogs;
							dgLogs.DataBind();
							
							lblError.Visible = false;
							pnlLogs.Visible = true;
						}
					}
				}
			}
		}

		private void dgLogs_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				if (Convert.ToDateTime(e.Item.Cells[4].Text) <= DateTime.UtcNow)
				{
					// Need To Check Whether It Is 1 Hour Late Or More If So The Log Is Overdue
					if (Convert.ToDateTime (e.Item.Cells[4].Text) <= DateTime.UtcNow.AddHours(-1)) 
					{
						for (int i = 0; i <= e.Item.Cells.Count - 1; i++)
						{
							e.Item.Cells[i].BackColor = Color.Salmon; // salmon
						}

						e.Item.Cells[5].Text = "Overdue Log";
					}
					// If It Is Due Within 1 Hour Then The Log Is Due To Client
					else
					{
						for (int i = 0; i <= e.Item.Cells.Count - 1; i++)
						{
							e.Item.Cells[i].BackColor = Color.PeachPuff;
						}

						e.Item.Cells[5].Text = "Due To Client";
					}
                }
				else
					e.Item.Cells[5].Text = "Normal";
			}
		}

		#endregion 

		#region DBCombo's Server Methods and Initialisation 

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
			this.Init += new System.EventHandler(this.DeliveryAndReturnsLog_Init);

		}
		#endregion

		#region Event Handlers
		
		private void btnGenerate_Click(object sender, EventArgs e)
		{
			LoadGrid();
		}

		#endregion
	}
}
