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

using System.Collections.Specialized;

namespace Orchestrator.WebUI
{
    public partial class Groupage_ordercreatedconf : Orchestrator.Base.BasePage
    {

        #region Properties
        public int OrderID
        {
            get
            {
                int orderID = -1;
                if (!string.IsNullOrEmpty(Request.QueryString["OID"]))
                    orderID = int.Parse(Request.QueryString["OID"]);
                return orderID;
            }
        }
        #endregion

        #region Page Load / Init
        protected void Page_Load(object sender, EventArgs e)
        {
            //bool performAction = !(this.Request.QueryString["action"] == null);
            //if (performAction)
            //{
            //    string action = this.Request.QueryString["action"].ToString();
            //    switch (action)
            //    {
            //        case "dn": // delivery note
            //            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');", true);
            //            break;

            //        case "bf": // booking form
            //            Page.ClientScript.RegisterStartupScript(this.GetType(), "bookingform", "NewBookingForm();", true);
            //            break;

            //        case "pil": // pallet identification label
            //            Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');", true);
            //            break;

            //        default:
            //            break;
            //    }
            //}

            Entities.CustomPrincipal cp = Page.User as Entities.CustomPrincipal;
            if (cp.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                // Hide scan booking form button for client users, only allow upload.
                lblClientConfirmationMessage.Visible = true;
            }

            ShowOptions();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnCreatePIL.Click += new EventHandler(btnCreatePIL_Click);
            this.btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnBookingForm.Click += new EventHandler(btnBookingForm_Click);
            this.btnPodLabel.Click += new EventHandler(btnPodLabel_Click);
        }

        void btnPodLabel_Click(object sender, EventArgs e)
        {
            //Use the Pub-Sub service to Print the POD Labels
            Facade.PODLabelPrintingService podLabelPrintingService = new Facade.PODLabelPrintingService();
            bool isPrinted = false;

            try
            {
                isPrinted = podLabelPrintingService.PrintPODLabel(this.OrderID);
            }
            catch (Exception ex) 
            {
                isPrinted = false; // The call has failed, gracefully let the user know.
            }

            string message;
            if (isPrinted)
                message = "Your POD label has been sent to the Printer";
            else
                message = "The POD Label Printing Service is not available. Please restart the service on your print server and try again.";

            this.MessageBox(message);
        }
        #endregion

        private void ShowOptions()
        {
            lblOrderID.Text = OrderID.ToString();
            btnPodLabel.Visible = Globals.Configuration.PodLabelsEnabled;
        }

        #region Button Events

        void btnCreatePIL_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (OrderID != -1)
            {
                #region Pop-up Report
                eReportType reportType = eReportType.PIL;
                dsPIL = facLoadOrder.GetPILData(OrderID.ToString());
                DataView dvPIL;

                if ((bool)dsPIL.Tables[0].Rows[0]["IsPalletNetwork"])
                {
                    reportType = Globals.Configuration.PalletNetworkLabelID;

                    //Need to duplicate the rows for the Pallteforce labels
                    dsPIL.Tables[0].Merge(dsPIL.Tables[0].Copy(), true);
                    dvPIL = new DataView(dsPIL.Tables[0], string.Empty, "OrderId, PalletCount", DataViewRowState.CurrentRows);
                }
                else
                {
                    dvPIL = new DataView(dsPIL.Tables[0]);
                }
                
                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = reportType;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dvPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                //string url = this.Request.Url.AbsoluteUri;
                //if (url.LastIndexOf("&action") > 0)
                //    url = url.Remove(url.LastIndexOf("&action"));
                
                //Response.Redirect(url + "&action=pil");

                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');", true);

                #endregion
            }
        }

        void btnCreateDeliveryNote_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsDelivery = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (OrderID != -1)
            {
                dsDelivery = facOrder.GetDeliveryNoteDataForOrderIDs(OrderID.ToString());

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DeliveryNote;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsDelivery;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                //string url = this.Request.Url.AbsoluteUri;
                //if (url.LastIndexOf("&action") > 0)
                //    url = url.Remove(url.LastIndexOf("&action"));    
                //Response.Redirect(url + "&action=dn");

                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');", true);
            }
        }
        #endregion

        protected void btnBookingForm_Click(object sender, EventArgs e)
        {
            //string url = this.Request.Url.AbsoluteUri;
            //if (url.LastIndexOf("&action") > 0)
            //    url = url.Remove(url.LastIndexOf("&action"));
            //Response.Redirect(url + "&action=bf");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "bookingform", "NewBookingForm();", true);
                        
        }

    }
}
