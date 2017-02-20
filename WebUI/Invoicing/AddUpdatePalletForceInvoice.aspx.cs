using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using Telerik.Web.UI;
using System.Collections.Generic;

namespace Orchestrator.WebUI.Invoicing
{
    public partial class AddUpdatePalletForceInvoice : Orchestrator.Base.BasePage
    {
        #region Structs

        protected struct InvoiceItemTotals
        {
            public int? ImportedInvoiceItems;
            public int? SystemItems;
            public decimal? ImportedInvoiceTotal;
            public decimal? DifferenceTotal;
            public int LCID;

            public InvoiceItemTotals(int importedInvoiceItems, int systemItems, decimal importedInvoiceTotal, decimal differenceTotal, int lcid)
            {
                ImportedInvoiceItems = importedInvoiceItems;
                SystemItems = systemItems;
                ImportedInvoiceTotal = importedInvoiceTotal;
                DifferenceTotal = differenceTotal;
                LCID = lcid;
            }

            public CultureInfo InvoiceCulture
            {
                get
                {
                    return new CultureInfo(LCID);
                }
            }
        }

        #endregion

        #region Properties

        protected int UnMatchedItems = 0;
        CultureInfo ci = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

        private static string vs_uploadedFileName = "vs_uploadedFileName";
        public string UploadedFileName
        {
            get { return Session[vs_uploadedFileName] == null ? string.Empty : (string)Session[vs_uploadedFileName]; }
            set { Session[vs_uploadedFileName] = value; }
        }

        private static string vs_uploadedFileExtension = "vs_uploadedFileExtension";
        public string UploadedFileExtension
        {
            get { return Session[vs_uploadedFileExtension] == null ? string.Empty : (string)Session[vs_uploadedFileExtension]; }
            set { Session[vs_uploadedFileExtension] = value; }
        }

        private static string vs_uploadedFilePath = "vs_uploadedFilePath";
        public string UploadedFilePath
        {
            get { return Session[vs_uploadedFilePath] == null ? string.Empty : (string)Session[vs_uploadedFilePath]; }
            set { Session[vs_uploadedFilePath] = value; }
        }

        private static string vs_preInvoiceID = "vs_preInvoiceID";
        public int PreInvoiceID
        {
            get { return Session[vs_preInvoiceID] == null ? -1 : (int)Session[vs_preInvoiceID]; }
            set { Session[vs_preInvoiceID] = value; }
        }

        private static string vs_preInvoiceNo = "vs_preInvoiceNo";
        public string preInvoiceNo
        {
            get { return Session[vs_preInvoiceNo] == null ? string.Empty : (string)Session[vs_preInvoiceNo]; }
            set { Session[vs_preInvoiceNo] = value; }
        }

        private static string vs_preInvoiceDate = "vs_preInvoiceDate";
        public DateTime? preInvoiceDate
        {
            get { return Session[vs_preInvoiceDate] == null ? null : (DateTime?)Session[vs_preInvoiceDate]; }
            set { Session[vs_preInvoiceDate] = value; }
        }

        private static string vs_preInvoiceType = "vs_preInvoiceType";
        public eInvoiceType? preInvoiceType
        {
            get { return Session[vs_preInvoiceDate] == null ? null : (eInvoiceType?)Session[vs_preInvoiceType]; }
            set { Session[vs_preInvoiceType] = value; }
        }

        private static string vs_userID = "vs_userID";
        public string UserID
        {
            get { return Session[vs_userID] == null ? string.Empty : Session[vs_userID].ToString(); }
            set { Session[vs_userID] = value; }
        }

        private static string vs_pfImportedInvoice = "vs_pfImportedInvoice";
        public Entities.PalletForceImportedInvoice PFImportedInvoice
        {
            get { return Session[vs_pfImportedInvoice] == null ? null : (Entities.PalletForceImportedInvoice)Session[vs_pfImportedInvoice]; }
            set { Session[vs_pfImportedInvoice] = value; }
        }

        protected List<InvoiceItemTotals> SummaryDetails = null;

        #endregion

        #region Exposed Methods

        protected CultureInfo GetCulture(int? lcid)
        {
            if (!lcid.HasValue)
                return new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
            else
                return new CultureInfo((int)lcid);
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && HttpContext.Current.Request.Files.Count > 0)
                GetAsyncUploadedFile(HttpContext.Current);

            if (!IsPostBack || (IsPostBack && dlgMatchOrder.ReturnValue == bool.TrueString))
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnBack.Click += new EventHandler(btnGeneric_Click);
            btnBack1.Click += new EventHandler(btnGeneric_Click);

            hdnRemoveButton.Click += new EventHandler(btnRemove_Click);
            hdnApproveButton.Click += new EventHandler(btnGeneric_Click);

            ramPreInvoice.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(ramPreInvoice_AjaxRequest);
        }

        #region Private Methods

        private void ConfigureDisplay()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["pfID"]))
                PreInvoiceID = int.Parse(Request.QueryString["pfID"]);

            if (PreInvoiceID > 0)
            {
                Facade.IPalletForceImportPreInvoice facPFiii = new Facade.PreInvoice();
                Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();

                Entities.PalletForceImportedInvoice pfImportedInvoice = facPFiii.GetImportedInvoiceForPreInvoiceID(PreInvoiceID);
                Entities.PreInvoice preInvoice = facPreInvoice.GetPreInvoice(PreInvoiceID);

                preInvoiceDate = pfImportedInvoice.InvoiceDate;
                preInvoiceNo = pfImportedInvoice.InvoiceNo;
                preInvoiceType = pfImportedInvoice.InvoiceType;

                lblSystemItemTotal.Text = preInvoice.NetAmount.ToString("C", ci);

                UploadedFileExtension = Path.GetExtension(pfImportedInvoice.Filename);
                UploadedFileName = Path.GetFileName(pfImportedInvoice.Filename);
                UploadedFilePath = pfImportedInvoice.Filename;

                UploadedFileDetails.Style["display"] = "";
                UploadInvoiceFile.Style["display"] = "none";
            }

            DisplayFileDetails();

            RebindGrid();
            UserID = ((Entities.CustomPrincipal)Page.User).UserName;

            // Define button visibilty.
            // If the pre-invoice has been created allow removal.
            btnRemove.Visible = btnRemove1.Visible = PreInvoiceID > 0;
            // If the pre-invoice has been created and all items are matched, allow creation.
            btnApprove.Visible = btnApprove1.Visible = PreInvoiceID > 0 && UnMatchedItems == 0;
        }

        private void DisplayFileDetails()
        {
            if (preInvoiceDate.HasValue)
                lblInvoiceDate.Text = preInvoiceDate.Value.ToLongDateString();

            if (preInvoiceType.HasValue)
                lblInvoiceType.Text = preInvoiceType.Value.ToString().Replace('_', ' ');

            lblInvoiceNo.Text = preInvoiceNo;
            lblFileName.Text = UploadedFileName;
        }

        private void GetAsyncUploadedFile(HttpContext currentContext)
        {
            UploadedFileName = Path.GetFileName(currentContext.Request.Files[0].FileName);
            UploadedFileExtension = Path.GetExtension(currentContext.Request.Files[0].FileName).ToLower();
            UploadedFilePath = currentContext.Server.MapPath(Orchestrator.Globals.Configuration.PalletForceUploadedInvoiceCSVPath) + UploadedFileName;

            currentContext.Request.Files[0].SaveAs(UploadedFilePath);
        }

        private void RebindGrid()
        {
            // A function to return the value of difference amount from the group of orders found.
            Func<IEnumerable<DataRow>, decimal?> getImportedItemDifferenceAmount = drs =>
                drs.Any(dr => dr.Field<int?>("PFInvoiceItemID").HasValue) ? drs.First(dr => dr.Field<int?>("PFInvoiceItemID").HasValue).Field<decimal?>("DifferenceAmount").Value : (decimal?)null;

            // Get Counts of Invoice Items
            Func<IEnumerable<DataRow>, int?> getImportedItemCount = drs => drs.Where(dr => dr.Field<int?>("PFInvoiceItemID").HasValue).Count();

            // Get Invoice Totals
            //Func<IEnumerable<DataRow>, decimal?> getImportedItemValue = drs => drs.Where(dr => dr.Field<int?>("OrderID").HasValue).Sum(dr => dr.Field<decimal?>("Cost").Value);
            Func<IEnumerable<DataRow>, decimal?> getDifferenceItemValue = drs => drs.Where(dr => dr.Field<int?>("PFInvoiceItemID").HasValue).Sum(dr => dr.Field<decimal?>("DifferenceAmount").Value);

            Facade.IPalletForceImportPreInvoice facPFPreInvoice = new Facade.PreInvoice();
            DataSet ds = null;

            switch (preInvoiceType)
            {
                case eInvoiceType.PFDepotCharge:
                    ds = facPFPreInvoice.GetImportedDepotChargePreInvoice(PreInvoiceID);
                    break;
                case eInvoiceType.PFHubCharge:
                    ds = facPFPreInvoice.GetImportedHubChargePreInvoice(PreInvoiceID);
                    break;
                case eInvoiceType.PFSelfBillDeliveryPayment:
                    ds = facPFPreInvoice.GetImportedSelfBillDeliveryPreInvoice(PreInvoiceID);
                    break;
            }

            if (ds != null)
            {
                int? unMatchedItems = 0;
                var queryInvoiceItems = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();
                var resultRows = from row in queryInvoiceItems
                                 group row by row["OrderID"] into g
                                 select new
                                 {
                                     OrderID = g.Key,
                                     DifferenceAmount = getImportedItemDifferenceAmount(g),
                                     OrderCount = g.Count(),
                                     Items = g
                                 };

                lvPreInvoiceItems.DataSource = resultRows;
                lvPreInvoiceItems.DataBind();

                // Get any unmatched palletforce invoice items.
                if (ds.Tables.Count > 1)
                {
                    var queryUnmatchedItems = ds.Tables[1].Rows.Cast<DataRow>().AsEnumerable();
                    lvPreInvoiceUnMatchedItems.DataSource = queryUnmatchedItems;
                    lvPreInvoiceUnMatchedItems.DataBind();

                    unMatchedItems = queryUnmatchedItems.Count();
                }

                lblInvoiceItems.Text = getImportedItemCount(queryInvoiceItems).Value.ToString();
                lblMatchedItems.Text = unMatchedItems.Value.ToString();

                if (unMatchedItems.HasValue && unMatchedItems.Value > 0)
                {
                    lblMatchedItems.ForeColor = System.Drawing.Color.Red;
                    UnMatchedItems = unMatchedItems.Value;
                    lblUnMatchedItems.Text = unMatchedItems.Value.ToString();
                }
                else
                {
                    lblMatchedItems.ForeColor = System.Drawing.Color.Black;
                    lblUnMatchedItems.Text = "0";
                }

                //lblSystemItemTotal.Text = getImportedItemValue(queryInvoiceItems).Value.ToString("C", ci);
                lblDifferenceValue.Text = getDifferenceItemValue(queryInvoiceItems).Value.ToString("C", ci);
            }
        }

        private void ResetSessionVaribles()
        {
            UserID = UploadedFileName = UploadedFileExtension = UploadedFilePath = preInvoiceNo = null;
            preInvoiceDate = null;
            preInvoiceType = null;
            PreInvoiceID = -1;
        }

        #endregion

        #region Events

        #region Ajax Requests

        void ramPreInvoice_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            ConfigureDisplay();
        }

        #endregion

        #region Buttons

        void btnGeneric_Click(object sender, EventArgs e)
        {
            ResetSessionVaribles();
            Response.Redirect("PalletForceInvoiceList.aspx");
        }

        void btnRemove_Click(object sender, EventArgs e)
        {
            Facade.IPalletForceImportPreInvoice facPFImportPreInvoice = new Facade.PreInvoice();
            Entities.PalletForceImportedInvoice pfImportedInvoice = facPFImportPreInvoice.GetImportedInvoiceForPreInvoiceID(PreInvoiceID);

            bool retVal = facPFImportPreInvoice.DeleteImportedPreInvoice(PreInvoiceID, pfImportedInvoice.Filename);

            if (retVal)
                btnGeneric_Click(null, null);
        }

        #endregion

        #region Web Methods

        [System.Web.Services.WebMethod]
        public static string ImportFile(string fileName)
        {
            string retVal = string.Empty;
            string uploadedFilePath = string.Empty;
            eInvoiceType invoiceType;
            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;

            //Replace with system defined regular expression
            if (fileName.ToLower().Contains("hubinv"))
                invoiceType = eInvoiceType.PFHubCharge;
            else if (fileName.ToLower().Contains("chargeinv"))
                invoiceType = eInvoiceType.PFDepotCharge;
            else
                invoiceType = eInvoiceType.PFSelfBillDeliveryPayment;

            //Check to see if the file already exists.
            Facade.IPalletForceImportPreInvoice facPFImport = new Facade.PreInvoice();
            bool fileAlreadyImported = facPFImport.CheckImportedFiles(fileName);

            if (currentSession[vs_uploadedFilePath] != null && currentSession[vs_userID] != null && !fileAlreadyImported)
            {
                #region Import File

                uploadedFilePath = currentSession[vs_uploadedFilePath].ToString();

                if (File.Exists(uploadedFilePath))
                {
                    try
                    {
                        currentSession[vs_pfImportedInvoice] = facPFImport.ImportPalletForceInvoice(uploadedFilePath, invoiceType, currentSession[vs_userID].ToString());
                        retVal = bool.TrueString;
                    }
                    catch (Exception ex)
                    {
                        #region File Removal on Error
                        retVal = FileRemovalOnError(uploadedFilePath, ex);
                        #endregion
                    }
                }
                else
                    retVal = string.Format("{0}|{1}", 4, -1);

                #endregion
            }
            else
            {
                #region Remove and send Error Message

                if (currentSession[vs_uploadedFilePath] != null)
                {
                    try
                    {
                        File.Delete(currentSession[vs_uploadedFilePath].ToString());
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        retVal = fileAlreadyImported ?  string.Format("{0}|{1}", 3, -1) : string.Format("{0}|{1}", 2, -1);
                    }
                }
                else
                    retVal = string.Format("{0}|{1}", 2, -1);

                StaticResetSessionVaribles(currentSession);

                #endregion
            }

            return retVal.ToString();
        }

        [System.Web.Services.WebMethod]
        public static string ValidFileImport()
        {
            string retVal = string.Empty;
            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;

            if (currentSession[vs_pfImportedInvoice] != null)
            {
                #region Import File

                Entities.PalletForceImportedInvoice pfii = (Entities.PalletForceImportedInvoice)currentSession[vs_pfImportedInvoice];

                try
                {
                    Facade.IPalletForceImportPreInvoice facPFImport = new Facade.PreInvoice();
                    currentSession[vs_pfImportedInvoice] = facPFImport.ValidatePFImportedItems(pfii, currentSession[vs_userID].ToString());
                    retVal = bool.TrueString;
                }
                catch (Exception ex)
                {
                    #region File Removal on Error
                    retVal = FileRemovalOnError(currentSession[vs_uploadedFilePath].ToString(), ex);
                    #endregion
                }
                #endregion
            }

            return retVal;
        }

        [System.Web.Services.WebMethod]
        public static string CreatePalletForcePreInvoice()
        {
            int preInvoiceID = -1;
            string retVal = string.Empty;

            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;

            //Check to see if the file already exists.
            Facade.IPalletForceImportPreInvoice facPFImport = new Facade.PreInvoice();
            string uploadedFilePath = currentSession[vs_uploadedFilePath].ToString();

            if (currentSession[vs_pfImportedInvoice] != null)
            {
                #region Import File

                Entities.PalletForceImportedInvoice pfii = (Entities.PalletForceImportedInvoice)currentSession[vs_pfImportedInvoice];

                try
                {
                    preInvoiceID = facPFImport.CreatePalletForcePreInvoice(pfii, uploadedFilePath, currentSession[vs_userID].ToString());
                    retVal = preInvoiceID.ToString();
                }
                catch (Exception ex)
                {
                    #region File Removal on Error
                    retVal = FileRemovalOnError(uploadedFilePath, ex);
                    #endregion
                }
                finally
                {
                    currentSession[vs_preInvoiceID] = preInvoiceID;
                }

                #endregion
            }

            return retVal;
        }

        [System.Web.Services.WebMethod]
        public static string RestoreOriginalValues()
        {
            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;

            string retVal = string.Empty;
            string userID = !string.IsNullOrEmpty(currentSession[vs_userID].ToString()) ? currentSession[vs_userID].ToString() : string.Empty;
            int? preInvoiceID = (int?)currentSession[vs_preInvoiceID];
            eInvoiceType? invoiceType = (eInvoiceType?)currentSession[vs_preInvoiceType];

            if (preInvoiceID != null && preInvoiceID > 0 && invoiceType != null && userID != string.Empty)
            {
                try
                {
                    Facade.IPalletForceImportPreInvoice facPFImport = new Facade.PreInvoice();
                    facPFImport.RestoreOriginalValues((int)preInvoiceID, (eInvoiceType)invoiceType, userID);
                }
                catch (Exception ex)
                {
                    retVal = RetrieveErrorMessages(ex);
                }
            }
            else
                retVal = string.Format("{0}|{1}", 1, "There was an error retrieving the preinvoice details. Please reload the page and try again.");

            ////Pause the app for 10 seconds.
            //DateTime dt = DateTime.Now.AddSeconds(10);
            //DateTime newdate = DateTime.Now;

            //do { newdate = DateTime.Now; }
            //while (newdate < dt);

            return retVal;
        }

        [System.Web.Services.WebMethod]
        public static string CreatePalletForceInvoice()
        {
            System.Web.SessionState.HttpSessionState currentSession = HttpContext.Current.Session;

            string retVal = string.Empty;
            string userID = !string.IsNullOrEmpty(currentSession[vs_userID].ToString()) ? currentSession[vs_userID].ToString() : string.Empty;
            int invoiceID = -1;
            int? preInvoiceID = (int?)currentSession[vs_preInvoiceID];
            eInvoiceType? invoiceType = (eInvoiceType?)currentSession[vs_preInvoiceType];

            if (preInvoiceID.HasValue && preInvoiceID.Value > 0 && invoiceType != null && userID != string.Empty)
            {
                Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();

                List<int> preInvoiceIDs = new List<int>();
                preInvoiceIDs.Add(preInvoiceID.Value);

                List<Entities.PreInvoice> preInvoices = facPreInvoice.GetPreInvoices(preInvoiceIDs);

                if (preInvoices != null && preInvoices.Count > 0)
                    try
                    {
                        Entities.PreInvoice currentPreInvoice = preInvoices[0];
                        invoiceID = facPreInvoice.CreateInvoice(currentPreInvoice, userID);

                        if (invoiceID > 0)
                            facPreInvoice.DeletePreInvoice(currentPreInvoice.PreInvoiceID);
                    }
                    catch (Exception ex)
                    {
                        retVal = RetrieveErrorMessages(ex);
                    }

            }
            else
                retVal = string.Format("{0}|{1}", 1, "There was an error retrieving the preinvoice details. Please reload the page and try again.");

            return retVal;
        }

        #region web method static functions

        public static void StaticResetSessionVaribles(System.Web.SessionState.HttpSessionState currentSession)
        {
            currentSession[vs_userID] = currentSession[vs_uploadedFileName] = currentSession[vs_uploadedFileExtension] = currentSession[vs_uploadedFilePath] = currentSession[vs_preInvoiceNo] = null;
            currentSession[vs_preInvoiceID] = -1;
            currentSession[vs_preInvoiceDate] = null;
            currentSession[vs_preInvoiceType] = null;
        }

        public static string FileRemovalOnError(string uploadedFilePath, Exception currentException)
        {
            string retVal = string.Empty;

            try
            {
                File.Delete(uploadedFilePath);
            }
            catch (Exception unused) { } // Swallow this, as the file may have manually been removed.
            finally
            {
                #region Error Message Handling
                retVal = RetrieveErrorMessages(currentException);
                #endregion
            }

            return retVal;
        }

        public static string RetrieveErrorMessages(Exception currentException)
        {
            string retVal = string.Empty;
            StringBuilder sb = new StringBuilder();

            while (currentException.InnerException != null)
            {
                if (sb.Length > 0)
                    sb.Append(",");

                sb.Append(currentException.Message);
                currentException = currentException.InnerException;
            }

            if (sb.Length > 0)
                sb.Append(",");

            sb.Append(currentException.Message);
            //retVal = string.Format("{0}|{1}", 1, string.Empty);

//#if DEBUG
            retVal = string.Format("{0}|{1}", 1, sb.ToString());
//#endif
            return retVal;
        }

        #endregion

        #endregion

        #endregion
    }
}