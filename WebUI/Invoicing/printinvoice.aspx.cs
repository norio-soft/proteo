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
using System.Xml;  
using System.IO;
using System.Globalization;
using System.Text;

using Orchestrator.WebUI.UserControls;
using Orchestrator.Entities; 
using Orchestrator.Globals;
using Orchestrator.WebUI.Reports;
using Orchestrator.Reports;

using P1TP.Components.Web.Validation;
using Orchestrator.WebUI.Security;
using Orchestrator.Logging;


using DataDynamics.ActiveReports;
using DataDynamics.ActiveReports.Document;
using DataDynamics.ActiveReports.Export.Pdf;
using DataDynamics.ActiveReports.Export.Xls;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for printinvoice.
	/// </summary>
	public partial class printinvoice : Orchestrator.Base.BasePage
	{	
		#region Constant & Enums
        private const string C_JOBIDCSV_VS = "JobIdCSV";
		#endregion

		#region Page Variables
		private bool				m_isUpdate = false;
		private string				m_extraIdCSV = null;
		private int					m_InvoiceNo = 0;
		private Entities.Invoice	m_Invoice;		
		private string				m_jobIdCSV = String.Empty;
		private int					m_clientId = 0;
        private string              m_invoiceIdCSV = String.Empty;
		#endregion 

		#region Form Elements

		protected System.Web.UI.WebControls.Panel					pnlPCVDeleted;
		protected System.Web.UI.WebControls.CheckBox				chkDiscrepancies;
		protected System.Web.UI.WebControls.Label					lblDatePosted;
		protected System.Web.UI.WebControls.Label					lblDatePrinted;
		
		protected System.Web.UI.WebControls.TextBox					txtVAT;
		protected System.Web.UI.WebControls.TextBox					txtAmount;
		protected System.Web.UI.WebControls.TextBox					txtReason;
		protected System.Web.UI.WebControls.TextBox					txtTotalAmount;
		#endregion
	
		#region Page/Load/Init/Error
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);
           
            try
            {
                m_invoiceIdCSV = Request.QueryString["InvoiceIdCSV"].ToString();
            }
            catch { }

            if (m_invoiceIdCSV != String.Empty)
            {
                int counter = 0;

                //if (m_invoiceIdCSV.Length > 0)
                //    m_invoiceIdCSV = m_invoiceIdCSV.Substring(0, m_invoiceIdCSV.Length - 1);

                string[] invoiceIdCSV = m_invoiceIdCSV.Split(',');

                Orchestrator.Reports.ReportBase mainReport = null;

                mainReport = new rptMain();

                // Loop around and view and add invoice to the cache and then generate the main invoice
                foreach (string invoiceId in invoiceIdCSV)
                {
                    m_InvoiceNo = Convert.ToInt32(invoiceId);
                    m_isUpdate = true;

                    // Invoice Sort Type
                    rdoSortType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceSortType)));
                    rdoSortType.DataBind();
                    rdoSortType.Items[0].Selected = true;
                   
                    if (mainReport != null)
                    {
                        LoadInvoice();

                        Orchestrator.Reports.ReportBase tempReport = LoadReport();

                        for (int a = 0; a <= tempReport.Document.Pages.Count - 1; a++)
                        {
                            mainReport.Document.Pages.Add(tempReport.Document.Pages[a]);
                        }
                    }
                }

                Response.Clear();
                Response.ClearContent();
                Response.ClearHeaders();
                Response.Buffer = true;
                Response.ContentType = "application/pdf";

                // IE & Acrobat seem to require "content-disposition" header being in the response.
                // If you don't add it, the doc still works most of the time, but not always.
                // this makes a new window appear: Response.AddHeader("content-disposition","attachment; filename=MyPDF.PDF");
                Response.AddHeader("content-disposition", "inline; filename=MyPDF.PDF");

                // Create the PDF export object
                PdfExport pdf = new PdfExport();
                // Create a new memory stream that will hold the pdf output
                System.IO.MemoryStream memStream = new System.IO.MemoryStream();
                // Export the report to PDF:
                pdf.Export(mainReport.Document, memStream);
                // Write the PDF stream out
                Response.BinaryWrite(memStream.ToArray());
                // Send all buffered content to the client
                Response.End();
            }
		}

		protected void addupdateinvoice_Init(object sender, EventArgs e)
		{

		}
	    #endregion

		#region Load Invoice(s)
		///	<summary> 
		/// Load Invoice
		///	</summary>
		private void LoadInvoice()
		{
            //if (ViewState["invoice"]==null)
            //{
				Facade.IInvoice facInvoice = new Facade.Invoice();
				Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
				m_Invoice = facInvoice.GetForInvoiceId(m_InvoiceNo);
				
				if (m_Invoice.InvoiceType == eInvoiceType.SelfBill)
				{
					m_Invoice.Extras = facInvoiceExtra.GetExtraCollectionForInvoiceId(m_Invoice.InvoiceId);
				}

				ViewState["invoice"] = m_Invoice;
            //}
            //else
            //    m_Invoice = (Entities.Invoice)ViewState["invoice"];

			// Load the report with the relevant details
			if (m_Invoice != null)
			{

				lblInvoiceNo.Text = m_Invoice.InvoiceId.ToString();  
				lblInvoiceNo.ForeColor = Color.Black;
				
				lblInvoiceType.Text = m_Invoice.InvoiceType.ToString();
				
				if (m_Invoice.InvoiceType == eInvoiceType.SelfBill)
				{
					lblClientInvoiceSelfBillAmount.Visible = true;
					lblClientSelfBillInvoiceNumber.Visible = true; 
					txtClientSelfBillAmount.Visible = true;
					txtClientSelfBillAmount.Text = m_Invoice.ClientInvoiceAmount.ToString("C");
					txtClientSelfBillInvoiceNumber.Visible = true;
					txtClientSelfBillInvoiceNumber.Text = m_Invoice.SelfBillInvoiceNumber; 
					divClientSelfBillAmount.Visible = true;
					chkSelfBillRemainder.Visible = true;
					lblRemainder.Visible = true;
				}

				if (m_Invoice.OverrideReason != string.Empty)
				{
					pnlOverride.Visible = true;
					chkOverride.Checked = true;
					txtOverrideReason.Text = m_Invoice.OverrideReason.ToString();
					txtOverrideGrossAmount.Text = m_Invoice.OverrideTotalAmountGross.ToString("C");
					txtOverrideNetAmount.Text = m_Invoice.OverrideTotalAmountNet.ToString("C");
					txtOverrideVAT.Text = m_Invoice.OverrideTotalAmountVAT.ToString("C"); 
				}

				// Display the invoice date, but only allow the date to be altered if the invoice has not been posted.
				dteInvoiceDate.SelectedDate = m_Invoice.InvoiceDate;
				dteInvoiceDate.Enabled = !m_Invoice.Posted;

				lblDateCreated.Text = m_Invoice.CreatedDate.ToShortDateString(); 
				lblDateCreated.ForeColor = Color.Black; 
				
				txtInvoiceNotes.Text = m_Invoice.InvoiceDetails; 
				
				chkIncludePODs.Checked = m_Invoice.IncludePODs;

				chkIncludeReferences.Checked = m_Invoice.IncludeReferences;
				
				if (m_Invoice.IncludeDemurrage)
				{
					chkIncludeDemurrage.Checked = true;
					rdoDemurrageType.SelectedIndex = Convert.ToInt32(m_Invoice.DemurrageType); 
					rdoDemurrageType.Visible = true;
				}
				else
				{
					chkIncludeDemurrage.Visible = false;
					lblNoDemurrage.Visible = true;
					rdoDemurrageType.Visible = false;
				}

				if (m_Invoice.IncludeFuelSurcharge)
				{
					chkIncludeFuelSurcharge.Checked = true;
					txtFuelSurchargeRate.Text = m_Invoice.FuelSurchargeRate.ToString(); 
					rdoFuelSurchargeType.SelectedIndex = Convert.ToInt32(m_Invoice.FuelSurchargeType); 
					rdoFuelSurchargeType.Visible = true;
					divFuelSurcharge.Visible = true;
				}
				else
				{
					divFuelSurcharge.Visible = false;
					chkIncludeFuelSurcharge.Checked = false;
					rdoFuelSurchargeType.Visible = false; 
				}
					
				chkJobDetails.Checked = m_Invoice.IncludeJobDetails;
  
				chkExtraDetails.Checked = m_Invoice.IncludeExtraDetails;  

				rdoSortType.SelectedIndex = Convert.ToInt32 (m_Invoice.InvoiceSortingType) - 1;

				ViewState[C_JOBIDCSV_VS] = m_Invoice.JobIdCSV;
	
				if (m_Invoice.InvoiceType == eInvoiceType.SelfBill && m_Invoice.Extras != null)
				{
					
					if (m_Invoice.Extras.Count != 0) 
					{
						pnlExtras.Visible = true;
						dgExtras.DataSource = m_Invoice.Extras;
						dgExtras.DataBind();	


					
						m_extraIdCSV = "";
						foreach (Entities.Extra extra in m_Invoice.Extras)
						{
							if (m_extraIdCSV.Length > 0)
								m_extraIdCSV += ",";
							m_extraIdCSV += extra.ExtraId;
						}

						ViewState["ExtraIdCSV"] = m_extraIdCSV;
						chkExtraDetails.Visible = true;
					}
					else
					{
						pnlExtras.Visible = false; 
					}
				}
				   
				if (m_isUpdate)
				{
					if (m_Invoice.ForCancellation)
					{
						btnAdd.Visible = false;
						btnSendToAccounts.Visible = false;
						chkPostToExchequer.Visible = false;
						chkDelete.Checked = true;
					}
					else
					{
						if (!chkPostToExchequer.Checked)
						{
							btnAdd.Visible = true;
							btnSendToAccounts.Visible = true;
							chkPostToExchequer.Visible = true;
							chkDelete.Checked = false;						
						}
					}
				}
				else
					chkPostToExchequer.Visible = true;

				if (m_Invoice.Posted)
				{
					btnAdd.Visible = false;
					btnSendToAccounts.Visible = false; 
					chkPostToExchequer.Checked = true;
					chkPostToExchequer.Visible = true;
					pnlInvoiceDeleted.Visible = false;
					chkDelete.Visible = false;
					pnlExtras.Enabled = false;
					pnlIncludes.Enabled = false;
					pnlOverride.Enabled = false;    
					chkOverride.Enabled = false;
					pnlSelfRemainder.Enabled = false;
					txtInvoiceNotes.Enabled = false;
					rdoSortType.Enabled = false;
					txtClientSelfBillAmount.Enabled = false;
					chkSelfBillRemainder.Enabled = false;
					txtClientSelfBillInvoiceNumber.Enabled = false; 
					btnSendToAccounts.Visible = false;
					btnViewInvoice.Visible = false;
					dteInvoiceDate.Enabled = false;
				}
				else
				{
					btnAdd.Visible = true;
					btnSendToAccounts.Visible = true; 
					chkPostToExchequer.Checked = false;
					pnlInvoiceDeleted.Visible = true; 
					chkDelete.Visible = true;
				}
			}	 	

			Header1.Title = "Update Invoice";
			Header1.subTitle = "Please make any changes neccessary.";
			btnAdd.Text = "Update";	
		}
		
        /// <summary>
        /// Load Report Depending On Invoice Type
        /// </summary>
        private ReportBase LoadReport()
        {
            // Configure the Session variables used to pass data to the report
            NameValueCollection reportParams = new NameValueCollection();

            //-------------------------------------------------------------------------------------	
            //						Job/Collect-Drops/References/Demurrages Section
            //-------------------------------------------------------------------------------------	
            Facade.IInvoice facInv = new Facade.Invoice();

            DataSet dsInv = null;
            if (m_isUpdate)
                dsInv = facInv.GetJobsForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));
            else
                dsInv = facInv.GetJobsToInvoice(m_jobIdCSV);

            reportParams.Add("JobIds", m_jobIdCSV);

            reportParams.Add("ExtraIds", m_extraIdCSV);

            //-------------------------------------------------------------------------------------	
            //									Param Section
            //-------------------------------------------------------------------------------------	
            // Fuel Type & Rate 
            eInvoiceDisplayMethod fuelType;
            decimal newRate = 0;

            if (chkIncludeFuelSurcharge.Checked)
            {
                reportParams.Add("Fuel", "Include");

                // Pass The New Rate To Report if so...
                newRate = Convert.ToDecimal(txtFuelSurchargeRate.Text);
                reportParams.Add("FuelRate", newRate.ToString());

                // Pass FuelSurchargeType
                fuelType = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), rdoFuelSurchargeType.SelectedValue);
                reportParams.Add("FuelType", fuelType.ToString());
            }

            // Override
            if (chkOverride.Checked)
            {
                reportParams.Add("Override", "Include");
                reportParams.Add("OverrideVAT", txtOverrideVAT.Text);
                reportParams.Add("OverrideNET", txtOverrideNetAmount.Text);
                reportParams.Add("OverrideGross", txtOverrideGrossAmount.Text);
                reportParams.Add("OverrideReason", txtOverrideReason.Text);
            }

            // PODs
            if (chkIncludePODs.Checked)
                reportParams.Add("PODs", "Include");

            // References
            if (chkIncludeReferences.Checked)
                reportParams.Add("References", "Include");

            // Job 
            if (chkJobDetails.Checked)
            {
                reportParams.Add("JobDetails", "Include");

                // Demuragge 
                if (chkIncludeDemurrage.Checked)
                {
                    reportParams.Add("Demurrage", "Include");

                    // Demurrage Type
                    try
                    {
                        eInvoiceDisplayMethod demurrageType = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), rdoDemurrageType.SelectedValue);
                        reportParams.Add("DemurrageType", demurrageType.ToString());
                    }
                    catch (Exception) { }
                }
            }

            if (pnlExtras.Visible)
            {
                reportParams.Add("ExtraIds", Convert.ToString(ViewState["ExtraIdCSV"]));
                reportParams.Add("Extras", "Include");
            }

            // Extra details
            if (chkExtraDetails.Visible && chkExtraDetails.Checked)
                reportParams.Add("ExtraDetail", "include");

            // Self Bill Invoice Number
            if ((eInvoiceType)Enum.Parse(typeof(eInvoiceType), lblInvoiceType.Text) == eInvoiceType.SelfBill)
            {
                reportParams.Add("InvoiceType", "SelfBill");
                reportParams.Add("SelfBillInvoiceNumber", txtClientSelfBillInvoiceNumber.Text);
            }
            else
                reportParams.Add("InvoiceType", "Normal");

            // Client Name & Id			
            if (m_isUpdate)
            {
                Facade.IInvoice facClient = new Facade.Invoice();

                DataSet ds = facClient.GetClientForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));

                try
                {
                    reportParams.Add("Client", Convert.ToString(ds.Tables[0].Rows[0]["Client"]));
                    reportParams.Add("ClientId", Convert.ToString(ds.Tables[0].Rows[0]["ClientId"]));

                    m_clientId = int.Parse(ds.Tables[0].Rows[0]["ClientId"].ToString());

                    if (!chkPostToExchequer.Checked)
                    {
                        btnSendToAccounts.Visible = true;
                        pnlInvoiceDeleted.Visible = true;
                    }
                }
                catch
                {
                }
            }
            else
            {
                if (Convert.ToString(Session["ClientName"]) != "")
                    reportParams.Add("Client", Convert.ToString(Session["ClientName"]));

                if (Convert.ToString(Session["ClientId"]) != "")
                    reportParams.Add("ClientId", Convert.ToString(Session["ClientId"]));

                if (m_clientId == 0)
                    m_clientId = int.Parse(Session["ClientId"].ToString());
                else
                {
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    Entities.Organisation enOrg = new Entities.Organisation();
                    enOrg = facOrg.GetForIdentityId(m_clientId);
                    reportParams.Add("Client", enOrg.OrganisationName.ToString());
                    reportParams.Add("ClientId", m_clientId.ToString());
                }
            }

            // Date Range
            if (Convert.ToDateTime(Session["StartDate"]).Date != DateTime.MinValue)
                reportParams.Add("startDate", Convert.ToDateTime(Session["StartDate"]).ToString("dd/MM/yy"));

            if (Convert.ToDateTime(Session["EndDate"]).Date != DateTime.MinValue)
                reportParams.Add("endDate", Convert.ToDateTime(Session["EndDate"]).ToString("dd/MM/yy"));

            // Invoice Id
            if (lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
                reportParams.Add("invoiceId", lblInvoiceNo.Text);
            else
                reportParams.Add("invoiceId", "0");

            // Posted To Accounts
            if (chkPostToExchequer.Checked)
                reportParams.Add("Accounts", "true");

            int vatNo = 0;
            decimal vatRate = 0.00M;
            // VAT Rate 
            facInv.GetVatRateForVatType(eVATType.Standard, dteInvoiceDate.SelectedDate.Value, out vatNo, out vatRate);
            reportParams.Add("VATrate", vatRate.ToString());

            // Invoice Date
            reportParams.Add("InvoiceDate", dteInvoiceDate.SelectedDate.Value.ToShortDateString());

            //-------------------------------------------------------------------------------------	
            //									Load Report Section 
            //-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.Invoice;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsInv;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = rdoSortType.SelectedItem.Text.Replace(" ", "");
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

            Orchestrator.Reports.ReportBase activeReport = null;
            eReportType reportType = 0;

            reportType = (eReportType)Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable];
            try
            {
                activeReport = new Orchestrator.Reports.Invoicing.rptInvoice();
            }
            catch (NullReferenceException e)
            {
                Response.Write(e.Message);
                Response.Flush();
                return null;
            }

            if (activeReport != null)
            {
                try
                {
                    ApplicationLog.WriteTrace("Getting Data From Session Variable");
                    DataView view = null;
                    DataSet ds = (DataSet)Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable];

                    string sortExpression = (string)Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable];
                    ApplicationLog.WriteTrace("sortExpression ::" + sortExpression);
                    try
                    {
                        if (sortExpression.Length > 0)
                        {
                            view = new DataView(ds.Tables[0]);
                            view.Sort = sortExpression;
                            activeReport.DataSource = view;
                        }
                        else
                            activeReport.DataSource = ds;

                        activeReport.DataMember = (string)Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable];
                    }
                    catch (Exception eX)
                    {
                        this.Trace.Write("Err1:" + eX.Message);
                        ApplicationLog.WriteTrace("GetReport :: Err1:" + eX.Message);
                    }
                    activeReport.Document.Printer.PrinterName = string.Empty;
                    activeReport.Document.Printer.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    //activeReport.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4;
                    //activeReport.PrintWidth = activeReport.PageSettings.PaperWidth - (activeReport.PageSettings.Margins.Right + activeReport.PageSettings.Margins.Left);
                    activeReport.Run(false);
                }
                catch (Exception e)
                {
                    Response.Write(e.Message);
                    Response.Flush();
                    return null;
                }
            }

            Session[Orchestrator.Globals.Constants.ReportSessionVariable] = activeReport;

            return activeReport;
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
			this.Init += new System.EventHandler(this.addupdateinvoice_Init);

		}
		#endregion
	}
}
