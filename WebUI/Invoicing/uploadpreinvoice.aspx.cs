using System;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Collections;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
                        
using Orchestrator.Entities;
using Orchestrator.Globals;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for Invoice Preparation.
	/// </summary>
	public partial class UploadPreInvoice: Orchestrator.Base.BasePage
	{	
		#region Pre Invoice Discrepancy Class
		[Serializable]				  
			private class PreInvoiceDiscrepancy
		{
			#region Private Fields
			private int			m_jobId = 0;
			private decimal		m_preInvoiceJobCost = 0;
			private decimal		m_hMSJobCost = 0;
			private string		m_shipmentNo = string.Empty;
			private string		m_discrepancyNote = string.Empty;
			private string		m_preInvoicePONumber = string.Empty;
			private string		m_preInvoicePickUpLocation = string.Empty;
			private DateTime	m_preInvoiceApprovedDate = DateTime.MinValue;
			#endregion

			#region Constructors
			public PreInvoiceDiscrepancy(){}
			public PreInvoiceDiscrepancy(int jobId, decimal preInvoiceJobCost, decimal hMSJobCost, string shipmentNo,  string discrepancyNote, string preInvoicePONumber, string preInvoicePickUpLocation, DateTime preInvoiceApprovedDate)
			{
				m_jobId = jobId; 
				m_preInvoiceJobCost	= preInvoiceJobCost; 
				m_hMSJobCost = hMSJobCost; 
				m_shipmentNo = shipmentNo;
				m_discrepancyNote = discrepancyNote;
				m_preInvoicePONumber = preInvoicePONumber;
				m_preInvoicePickUpLocation = preInvoicePickUpLocation;
				m_preInvoiceApprovedDate = preInvoiceApprovedDate;
			}
			#endregion
		
			#region Public Properties
		
			public int JobId
			{
				get {return m_jobId;}
				set {m_jobId = value;}
			}

			public decimal PreInvoiceJobCost
			{
				get {return m_preInvoiceJobCost;}
				set {m_preInvoiceJobCost  = value;}
			}

			public decimal HMSJobCost
			{
				get {return m_hMSJobCost;}
				set {m_hMSJobCost  = value;}
			}

			public string ShipmentNo
			{
				get {return m_shipmentNo;}
				set {m_shipmentNo  = value;}
			}

			public string DiscrepancyNote
			{
				get {return m_discrepancyNote;}
				set {m_discrepancyNote= value;}
			}

			public string PreInvoicePONumber
			{
				get {return m_preInvoicePONumber;}
				set {m_preInvoicePONumber= value;}
			}
			public string PreInvoicePickUpLocation
			{
				get {return m_preInvoicePickUpLocation;}
				set {m_preInvoicePickUpLocation= value;}
			}

			public DateTime PreInvoiceApprovedDate
			{
				get {return m_preInvoiceApprovedDate;}
				set {m_preInvoiceApprovedDate= value;}
			}
			#endregion 
		}	   
		#endregion
               	
		#region Constants & Enums
		private const int C_HOURSPREVIOUS = 4;	// Past hours to show
		private const int C_HOURSFOLLOWING = 4;	// Following hours to show
		private const int C_HOURSTOTAL = 36;	// Total hours to show
		private const int C_HOURSMOVE = 6;		// Hours to move when going back / forward
		
		private const string C_HOURS_PREVIOUS_VS = "C_HOURS_PREVIOUS_VS";
		private const string C_HOURS_FOLLOWING_VS = "C_HOURS_FOLLOWING_VS";

		private static readonly TimeSpan C_TIMESPAN = new TimeSpan(0, 0, 30, 0, 0);
		private static readonly TimeSpan C_MOVEONREFRESH = C_TIMESPAN;



		private const string C_BASE_LOCATION_VS = "BaseLocation";
		private const string C_FILE_NAME_ON_SERVER_VS = "FileNameOnServer";
		#endregion

		#region Page Variables
		private string				m_connection = string.Empty;
		private	ArrayList			m_arrJobId = new ArrayList();
		private DataSet				m_dsPrevious = null;
		private string				m_fileNameOnServer = string.Empty;
		private string				m_baseLocation = string.Empty;
		#endregion

		#region Property Interfaces
		private int HoursPrevious
		{
			get
			{
				if (ViewState[C_HOURS_PREVIOUS_VS] == null)
				{
					HoursPrevious = 6;
					return 6;
				}
				else
					return (int) ViewState[C_HOURS_PREVIOUS_VS];
			}
			set
			{
				ViewState[C_HOURS_PREVIOUS_VS] = value;
			}
		}

		private int HoursFollowing
		{
			get
			{
				if (ViewState[C_HOURS_FOLLOWING_VS] == null)
				{
					HoursFollowing = 3;
					return 3;
				}
				else
					return (int) ViewState[C_HOURS_FOLLOWING_VS];
			}
			set
			{
				ViewState[C_HOURS_FOLLOWING_VS] = value;
			}
		}

		#endregion

		#region Form Elements


		#endregion

		#region Page/Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

			if (!IsPostBack)
			{
				// Check whether the user has any previous pre-invoices to sort
				string userName = ((Entities.CustomPrincipal)Page.User).UserName;

				Facade.IInvoice facInv = new Facade.Invoice();
				
				m_dsPrevious = facInv.GetPreviousPreInvoiceForUserName(userName);

				if (m_dsPrevious.Tables[0].Rows != null)
				{
					if (m_dsPrevious.Tables[0].Rows.Count != 0)
					{
						btnReload.Visible = true; 

						ViewState[C_BASE_LOCATION_VS] = m_dsPrevious.Tables[0].Rows[0]["FileLocation"].ToString();
						ViewState[C_FILE_NAME_ON_SERVER_VS] = m_dsPrevious.Tables[0].Rows[0]["FileName"].ToString();
						
						string filename = m_dsPrevious.Tables[0].Rows[0]["FileName"].ToString();
						cboClient.Text = m_dsPrevious.Tables[0].Rows[0]["OrganisationName"].ToString();
						cboClient.SelectedValue = m_dsPrevious.Tables[0].Rows[0]["ClientId"].ToString();

						m_dsPrevious = LoadFileToDataSet(m_dsPrevious.Tables[0].Rows[0]["FileLocation"].ToString(), m_dsPrevious.Tables[0].Rows[0]["FileName"].ToString());

						if (m_dsPrevious != null)
							CompareFileWithHMS(m_dsPrevious); 

						if (dgJobDiscrepancies.Items.Count !=0)
							txtOutput.InnerHtml = "The previous invoice (<b>" + filename.ToString() + "</b>) has been loaded as the discrepancies required attention.";
					}
				}
			}
			else
			{
				m_baseLocation = (string) ViewState[C_BASE_LOCATION_VS];
				m_fileNameOnServer = (string) ViewState[C_FILE_NAME_ON_SERVER_VS];
			}
		}
	
		protected void UploadPreInvoice_Init(object sender, EventArgs evArgs)
		{
			this.dgJobDiscrepancies.ItemDataBound += new System.Web.UI.WebControls.DataGridItemEventHandler(this.dgJobDiscrepancies_ItemDataBound);
			this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
			this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
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
			this.Init += new System.EventHandler(this.UploadPreInvoice_Init);

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
	
		#region Methods and Functions
		
		#region Button & Link Events
		private void btnUpload_Click(object sender, EventArgs e)
		{
			// If file name incorrect ignore the rest
			dgJobDiscrepancies.CurrentPageIndex = 0;
			
			DataSet ds = UploadFileToHMS();
			
			if (ds != null)
			{
				if (ds.Tables.Count != 0)
					CompareFileWithHMS(ds);
				else
					txtOutput.InnerHtml = "Upload unsuccessful ... please ensure that the file is a CSV and that the location of the file is correct.";
			}
		}
		
		private void btnReload_Click(object sender, System.EventArgs e)
		{
			if (m_dsPrevious != null)
			{
				DataSet ds = LoadFileToDataSet(m_dsPrevious.Tables[0].Rows[0]["FileLocation"].ToString() , m_dsPrevious.Tables[0].Rows[0]["FileName"].ToString());
				CompareFileWithHMS(ds);
			}
			else
			{
				txtOutput.InnerHtml = "Unable to find relevant Invoice to upload!"; 	
				btnReload.Visible = false;
			}
		}
		
		private void btnSave_Click(object sender, System.EventArgs e)
		{
			SavePreInvoice();
		}	
		
		protected void lnkJobId_Click(object sender, EventArgs e)
		{
			SavePreInvoice();
															 
			DataGridItem parent = (DataGridItem)((LinkButton) sender).Parent.Parent;

			// Hyperlink to Pricing using JobId
			string jobId = ((HtmlInputHidden) (parent.FindControl("hidJobId"))).Value;
			
			// If no job id don't go to job
			if (jobId != "0")
				Response.Redirect("../job/job.aspx?wiz=true&jobId="+jobId+"&csid="+this.CookieSessionID);
			else
				 txtOutput.InnerHtml = "Unable to link to the Job.";
		}

		#endregion 
		private bool CheckFileExistance(string fileName)
		{
			bool fileExists = false;

			Facade.IInvoice facInv = new Facade.Invoice();
			fileExists = facInv.CheckFile(fileName);

			return fileExists; 
		}

		protected void SavePreInvoice()
		{
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

			Facade.IInvoice facProgress = new Facade.Invoice();
		
			m_baseLocation = (string) ViewState[C_BASE_LOCATION_VS];
			m_fileNameOnServer = (string) ViewState[C_FILE_NAME_ON_SERVER_VS];
			
			int clientId = 0;
			
			if (cboClient.SelectedValue != string.Empty)
				clientId = Convert.ToInt32(cboClient.SelectedValue);

			bool saved = false;
			
			// Check for file existance
			bool fileExists = false;

			if (m_fileNameOnServer != null)
				fileExists = CheckFileExistance(m_fileNameOnServer);

			if (!fileExists)
			{
				saved = facProgress.SavePreInvoice(m_baseLocation, m_fileNameOnServer, userName, clientId); 
		
				if (saved)
					lblConfirmation.Text = "The file " + m_fileNameOnServer.ToString() + " was saved successfully";
				else
					lblConfirmation.Text = "The " + m_fileNameOnServer.ToString() + " file was not saved!";

					lblConfirmation.Visible = true;
			}
			else
			  txtOutput.InnerHtml = "The file has already been saved.";
		}

		private DataSet UploadFileToHMS()
		{
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
			
			if (uplTheFile.Value  == string.Empty) 
			{
				txtOutput.InnerHtml = "Error - a file name must be specified.";
				return null;
			}

			// Check the fule extension
			if (uplTheFile.Value.Substring (uplTheFile.Value.Length - 4, 4)!= ".csv")
			{
				txtOutput.InnerHtml = "Error - a file extension must be [NAME OF FILE].csv.";
				return null;
			}
			  
			m_fileNameOnServer = userName + DateTime.UtcNow.ToString("ddMMyyyyHHmmss") + ".csv"; //txtServername.Value;
			m_baseLocation = Server.MapPath("./PreInvoiceStorage/");
			
			if ("" == m_fileNameOnServer) 
			{
				txtOutput.InnerHtml = "Error - a file name must be specified.";
				return null;
			}

			if (null != uplTheFile.PostedFile) 
			{
				try 
				{
					uplTheFile.PostedFile.SaveAs(m_baseLocation+m_fileNameOnServer);
					
					txtOutput.InnerHtml = "File <b>" + 
						m_baseLocation+m_fileNameOnServer+"</b> uploaded successfully";
				}
				catch (Exception error) 
				{
					txtOutput.InnerHtml = "Error saving <b>" + 
						m_baseLocation+m_fileNameOnServer+"</b><br>"+ error.ToString();
				}
			}
			
			ViewState[C_FILE_NAME_ON_SERVER_VS] = m_fileNameOnServer;
			ViewState[C_BASE_LOCATION_VS] = m_baseLocation;

			return LoadFileToDataSet(m_baseLocation, m_fileNameOnServer);
		}

		private DataSet LoadFileToDataSet(string baseLocation, string fileNameOnServer)
		{
			if (baseLocation != "" && fileNameOnServer != "")
			{
				// Create connection string variable. Modify the "Data Source"
				// parameter as appropriate for your environment.
				m_connection = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" + baseLocation + ";Extended Properties=Text"; 
			
				// Create connection object by using the preceding connection string.
				OleDbConnection objConn = new OleDbConnection(m_connection);

				// The code to follow uses a SQL SELECT command to display the data from the worksheet.
				OleDbCommand ExcelCommand = new OleDbCommand(@"SELECT * FROM " + fileNameOnServer,objConn);

				OleDbDataAdapter ExcelAdapter = new OleDbDataAdapter(ExcelCommand);
			
				// Open connection with the database.
				objConn.Open();

				DataSet ExcelDataSet = new DataSet();

				try
				{
					ExcelAdapter.Fill(ExcelDataSet);
				}
				catch
				{
					txtOutput.InnerHtml = "Unable to find " + fileNameOnServer + " Invoice to upload!"; 
				}
				// Close connection with the database.
				objConn.Close();  
 
				return ExcelDataSet; 
			}
				return null;
		}

		private void CompareFileWithHMS(DataSet ExcelDataSet)
		{
			if (ExcelDataSet.Tables.Count == 0)
			{
				btnReload.Visible = false;
				return;
			}

			int jobCountPreInvoice = 0;
			int totalRowsPreInvoice = ExcelDataSet.Tables[0].Rows.Count - 2;
			int totalDiscrepanies = 0; 
			int clientId = 0; 

			Facade.IJob facJob = new Facade.Job();
			
			DataSet ds = null; 
			
			StringBuilder sb = new StringBuilder();	 
			
			ArrayList arrDiscrepancy = new ArrayList();

			// Client
			if (cboClient.Text != "")
				clientId = Convert.ToInt32 (cboClient.SelectedValue);
			
			foreach (DataRow row in ExcelDataSet.Tables[0].Rows)
			{
				// Test format of row Approved Date by trying to convert the date ...
				// thats if it is a date.  If not that row is not a job!
				try
				{
					// Checking whether the column is a date format 
					Convert.ToDateTime(row[3].ToString());
										
					string shipmentNo = string.Empty;
					
					shipmentNo = row[0].ToString();  
					
					ds = facJob.GetJobForLoadNo (shipmentNo, clientId); // Shipment No is actually Load No
				
					if (ds.Tables[0].Rows.Count == 0)
					{
						// Found Discrepancy with Shipment No either not in system or incorrect shipment no
						PreInvoiceDiscrepancy pid = new PreInvoiceDiscrepancy();
						
						pid.JobId = 0;	 
						pid.ShipmentNo = shipmentNo.ToString(); // Displayed Pre-Invoice No	
						pid.PreInvoicePONumber =  row[1].ToString();
						pid.PreInvoicePickUpLocation =  row[2].ToString();
						pid.PreInvoiceApprovedDate =  Convert.ToDateTime(row[3]);
						pid.PreInvoiceJobCost = Convert.ToDecimal(row[4]);
						pid.HMSJobCost = decimal.Parse("£0.00", NumberStyles.Currency);
						pid.DiscrepancyNote = "Discrepancy with the Shipment No, not found in Database or incorrectly given, please investigate.";

						arrDiscrepancy.Add(pid);

						totalDiscrepanies++;
					}
					else
					{
						// Check cost of job with the pre-invoice cost
						if (decimal.Parse (ds.Tables[0].Rows[0]["ChargeAmount"].ToString(), NumberStyles.Currency) !=  decimal.Parse ( row[4].ToString(), NumberStyles.Currency ))
						{
							// Found Discrepancy with cost, add to list and highlight cost
							PreInvoiceDiscrepancy pid = new PreInvoiceDiscrepancy();
                             							
							pid.JobId = Convert.ToInt32(ds.Tables[0].Rows[0]["JobId"]);	
							pid.ShipmentNo = ds.Tables[0].Rows[0]["LoadNo"].ToString(); // Display HMS Load No
							pid.PreInvoicePONumber =  row[1].ToString();
							pid.PreInvoicePickUpLocation =  row[2].ToString();
							pid.PreInvoiceApprovedDate =  Convert.ToDateTime(row[3]);
							pid.PreInvoiceJobCost = Convert.ToDecimal(row[4]);						
							pid.HMSJobCost =  Convert.ToDecimal(ds.Tables[0].Rows[0]["ChargeAmount"]);
							pid.DiscrepancyNote = "Discrepancy with the Amount";
							
							arrDiscrepancy.Add(pid);

							totalDiscrepanies++;
						}
					   
						// Check State of job within HMS
						if ((eJobState)Enum.Parse(typeof(eJobState), (Convert.ToInt32( ds.Tables[0].Rows[0]["JobStateId"])).ToString()) != eJobState.ReadyToInvoice)
						{
							// Found Discrepancy with state of the job, add to list and highlight job state
							PreInvoiceDiscrepancy pid = new PreInvoiceDiscrepancy();
 							
							pid.JobId = Convert.ToInt32(ds.Tables[0].Rows[0]["JobId"]);	
							pid.ShipmentNo = ds.Tables[0].Rows[0]["LoadNo"].ToString(); // Display HMS Load No	
							pid.PreInvoicePONumber =  row[1].ToString();
							pid.PreInvoicePickUpLocation =  row[2].ToString();
							pid.PreInvoiceApprovedDate =  Convert.ToDateTime(row[3]);
							pid.PreInvoiceJobCost = decimal.Parse ( row[4].ToString(), NumberStyles.Currency ); //Convert.ToDecimal(row[4]);
							pid.HMSJobCost = Convert.ToDecimal (ds.Tables[0].Rows[0]["ChargeAmount"]);
							pid.DiscrepancyNote = "Discrepancy with the state of the Job, please review.";
							
							arrDiscrepancy.Add(pid);

							totalDiscrepanies++;
						}

						// Add JobId to array list 
						m_arrJobId.Add(ds.Tables[0].Rows[0]["JobId"].ToString());
					}
					// Count how many jobs are in this Pre Invoice
					jobCountPreInvoice++;
				}					
				catch{}
				finally	{}
			}  
			
			string totalCostPreInvoice = string.Empty;
			
			try
			{
				// Row count and - 2 to get Total Cost (Don't Ask Why Minus 2 rows)
				totalCostPreInvoice = decimal.Parse ( ExcelDataSet.Tables[0].Rows[totalRowsPreInvoice][4].ToString(), NumberStyles.Currency ).ToString ();
			}
			catch
			{
				// Row count and - 3 to get Total Cost (Don't Ask Why Minus 3 rows)
				totalCostPreInvoice = decimal.Parse ( ExcelDataSet.Tables[0].Rows[totalRowsPreInvoice - 1][4].ToString(), NumberStyles.Currency ).ToString ();
			}

			// Apply Discrepancies to Datagrid
			if (arrDiscrepancy.Count != 0)
			{
				dgJobDiscrepancies.DataSource = arrDiscrepancy;
				dgJobDiscrepancies.DataBind(); 
				btnSave.Visible = true;
			}
			else
			{
				if (m_arrJobId.Count != 0)
				{
                    //#15867 J.Steele 
                    //Clear the Invoice Session variables before setting the specific ones
                    Utilities.ClearInvoiceSession();

					Session["ClientId"] = Convert.ToInt32 (cboClient.SelectedValue); 
					Session["ClientName"] = cboClient.Text;
					Session["JobIds"] =	m_arrJobId;
					Session["FileLocation"] = m_baseLocation;
					Session["FileName"] = m_fileNameOnServer;
					Server.Transfer("addupdateinvoice.aspx");
				}
			}
			
			ViewState[C_BASE_LOCATION_VS] = m_baseLocation;
			ViewState[C_FILE_NAME_ON_SERVER_VS] = m_fileNameOnServer;
					
			lblPreInvoiceOverview.Text = "There are " + jobCountPreInvoice + " jobs for this pre invoice and " + totalDiscrepanies + " job discrepancies. The total amount for this pre invoice is £" + totalCostPreInvoice.ToString(); 
		}

     
		#region Grid Events
		protected void dgJobDiscrepancies_Page(Object sender, DataGridPageChangedEventArgs e) 
		{
			dgJobDiscrepancies.CurrentPageIndex = e.NewPageIndex;   

			dgJobDiscrepancies.DataBind();
		}

		protected void dgJobDiscrepancies_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if(e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				if (e.Item.Cells[1].Text == "&nbsp;")
				{
					e.Item.Cells[1].Text = "PO Number incorrect format.";
					e.Item.Cells[1].ForeColor = Color.Red;
				}
				
				if (e.Item.Cells[5].Text == "Discrepancy with the Shipment No, not found in Database or incorrectly given, please investigate.")
					e.Item.Cells[0].ForeColor = Color.Red;
				else if (e.Item.Cells[5].Text == "Discrepancy with the Amount")
				{											 
					e.Item.Cells[2].ForeColor = Color.Red;
					e.Item.Cells[3].ForeColor = Color.Red;
				}
				else if (e.Item.Cells[5].Text == "Discrepancy with Job State, please review.")
					e.Item.Cells[0].ForeColor = Color.Red;
			}
		}

		#endregion		
		#endregion
	}
}
