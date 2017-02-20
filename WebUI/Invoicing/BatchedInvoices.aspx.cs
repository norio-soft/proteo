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

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for BatchedInvoices.
	/// </summary>
	public partial class BatchedInvoices : Orchestrator.Base.BasePage
	{
		#region Form Elements
		

		#endregion 
		
		#region Page Variables		
		
		//private NameValueCollection		m_batches = null;
		
		#endregion

		#region Page Load/Init
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

			LoadBatchInvoices();
		}

		private void BatchedInvoices_Init(object sender, EventArgs e)
		{
			this.dgInvoiceBatch.ItemCommand +=new DataGridCommandEventHandler(dgInvoiceBatch_ItemCommand);
		}

		#endregion 

		#region Batch Invoice Grid
			
		protected void dgInvoiceBatch_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgInvoiceBatch.CurrentPageIndex = e.NewPageIndex;   
		}

		private void dgInvoiceBatch_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "create":
					// TODO: NEED TO DELETE ONCE CREATED 

					break;
				case "view":
					HtmlInputHidden clientId = (HtmlInputHidden) e.Item.FindControl("hidIdentityId");
					
					string jobIds = e.Item.Cells[1].Text;
					int batchId = Convert.ToInt32(e.Item.Cells[0].Text);
					
					ArrayList selectedJobs = new ArrayList();
						
					string[] job = jobIds.Split(',');
				
					for (int i = 0; i < job.Length; i++)
					{
						selectedJobs.Add(job[i].ToString());
					}

                    //#15867 J.Steele 
                    //Clear the Invoice Session variables before setting the specific ones
                    Utilities.ClearInvoiceSession();
					Session["JobIds"] = selectedJobs;
					Session["ClientId"] = Convert.ToInt32 (clientId.Value); 
					Session["BatchId"] = batchId;
					Server.Transfer("addupdateinvoice.aspx"); 
					break;
			}
		}
		#endregion

		#region Add Invoice
		///	<summary> 
		/// Add Invoice
		///	</summary>
		private int AddInvoice()
		{
			int invoiceId = 0;
			Facade.IInvoice facInvoice = new Facade.Invoice();

			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
			
			// m_jobIdCSV = (string) ViewState[C_JOBIDCSV_VS]; 

			// invoiceId = facInvoice.Create(m_Invoice, userName);
            
			if (invoiceId == 0)
			{
				lblNote.Text = "There was an error adding the Invoice, please try again.";
				lblNote.Visible = true;
				lblNote.ForeColor = Color.Red;
			}

			return invoiceId;
		}
		#endregion

		#region Load Batch Invoices

		private void LoadBatchInvoices()
		{
			Facade.IInvoiceBatches facInv = new Orchestrator.Facade.Invoice();
			
			DataSet dsInvoice = facInv.GetAllBatches ();
			
			dgInvoiceBatch.DataSource = dsInvoice;

			dgInvoiceBatch.DataBind();

			if (dsInvoice.Tables[0].Rows.Count > 0)
			{
				dgInvoiceBatch.Visible = true;
				pnlInvoiceBatch.Visible = true;
			}
			else
			{
				lblNote.Text = "No invoice batches to invoice";
				lblNote.ForeColor = Color.Red;
				lblNote.Visible = true;
				dgInvoiceBatch.Visible = false;
				pnlInvoiceBatch.Visible = true;
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
			this.Init +=new EventHandler(BatchedInvoices_Init);

		}
		#endregion
	}
}
