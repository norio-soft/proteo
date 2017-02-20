using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Globalization;
using Orchestrator.Facade;

namespace Orchestrator.WebUI
{
    public partial class OrderExtra : Orchestrator.Base.BasePage
    {
        public bool IsUpdate
        {
            get
            {
                return m_extraId > 0;
            }
        }

        #region Fields

        private int m_jobId = 0;
        private int m_extraId = 0;
        private bool m_demurrage = false;
        private int m_instructionId = 0;
        private int m_orderID = 0;
        private int m_lcid = -1;
        private int? m_exchangeRateID = null;
        private decimal m_fuelSurchargePercentage;

        #endregion

        public int OrderID
        {
            get { return m_orderID; }
        }

        protected int InstructionID
        {
            get
            {
                return m_instructionId;
            }
        }

        #region Page Loading/Setup/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["jobId"] != null)
                m_jobId = int.Parse(Request.QueryString["jobId"]);

            if (Request.QueryString["OID"] != null)
                m_orderID = int.Parse(Request.QueryString["OID"]);

            if (Request.QueryString["extraId"] != null)
                m_extraId = int.Parse(Request.QueryString["extraId"]);

            //If an InstructionId has been passed then it must be for Demurrage
            if (Request.QueryString["instructionId"] != null)
            {
                m_instructionId = int.Parse(Request.QueryString["instructionId"]);
                m_demurrage = true;
            }

            if (Request.QueryString["lcID"] != null)
                m_lcid = int.Parse(Request.QueryString["lcID"]);

            if (!string.IsNullOrEmpty(Request.QueryString["erID"]) && Request.QueryString["erID"] != "-1")
                m_exchangeRateID = int.Parse(Request.QueryString["erID"]);

            if (!IsPostBack)
            {
                PopulateStaticControls();
                if (m_extraId > 0)
                    LoadExtra();
            }
        }

        private void PopulateStaticControls()
        {
            if (m_instructionId > 0)
            {
                Facade.IOrder facOrder = new Facade.Order();
                DataSet dsclients = facOrder.GetOrdersForInstructionID(this.InstructionID);
                cboClient.DataSource = dsclients;
                cboClient.DataBind();

                List<String> Clients = new List<string>();

                foreach (DataRow row in dsclients.Tables[0].Rows)
                {
                    if (!Clients.Contains(row["CustomerOrganisationName"].ToString()))
                        Clients.Add(row["CustomerOrganisationName"].ToString());
                }

                if (Clients.Count == 1)
                {
                    cboClient.Enabled = false;
                    cboClient.Items[0].Selected = true;
                }
            }
            else
            {
                trClient.Visible = false;
            }


            Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            bool? getActiveExtraTypes = true;
            cboExtraType.DataSource = facExtraType.GetForIsEnabled(getActiveExtraTypes);
            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";

            cboExtraState.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eExtraState)));

            // If culture set, set culture of amount textbox, otherwise, set to system default.
            if (m_lcid > 0)
                rntAmount.Culture = new CultureInfo(m_lcid);
            else
                rntAmount.Culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            Page.DataBind();

            //If its for Demurrage then pre-select the demurrage extra
            if (m_demurrage)
                cboExtraType.SelectedValue = ((int)eExtraType.Demurrage).ToString();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnDeleteExtra.Click += new EventHandler(btnDeleteExtra_Click);
            this.btnAddExtra.Click += new EventHandler(btnAddExtra_Click);
            this.cboExtraType.SelectedIndexChanged += new EventHandler(cboExtraType_SelectedIndexChanged);
            this.rntAmount.PreRender += new EventHandler(rntAmount_PreRender);
        }

        void rntAmount_PreRender(object sender, EventArgs e)
        {
            if (!IsUpdate && this.OrderID > 0)
            {
                eExtraType extraType = (eExtraType)Enum.Parse(typeof(eExtraType), cboExtraType.SelectedValue.Replace(" ", ""));

                // If the order has a tariff rate id, then see if this extra type has a specific amount configured.
                Facade.IOrder facOrder = new Facade.Order();
                Entities.Order order = facOrder.GetForOrderID(this.OrderID);

                if (order.IsAutorated)
                {
                    IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;

                    try
                    {
                        Repositories.DTOs.RateInformation rateInfo = facOrder.GetRate(order, true, out surcharges, false);
                    }
                    catch (ApplicationException)
                    {
                        // Swallow application exceptions as get rate can fall over if a rate cannot be
                        // found for a given postcode. If this happens then we don't really care.
                    }

                    if (surcharges != null)
                    {
                        var rs = (from s in surcharges
                                  where s.ExtraTypeID == (int)extraType
                                  select s).FirstOrDefault();

                        if (rs != null)
                            rntAmount.Value = (double?)rs.ForeignRate;
                    }
                }
            }
        }

        void cboExtraType_SelectedIndexChanged(object sender, EventArgs e)
        {
            eExtraType extraType = (eExtraType)Enum.Parse(typeof(eExtraType), cboExtraType.SelectedValue.Replace(" ", ""));
            
            if (this.OrderID > 0)
            {
                Facade.IOrder facOrder = new Facade.Order();
                Entities.Order order = facOrder.GetForOrderID(this.OrderID);
                IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;

                try
                {
                    Repositories.DTOs.RateInformation rateInfo = facOrder.GetRate(order, true, out surcharges);
                }
                catch (ApplicationException)
                {
                    // Swallow application exceptions as get rate can fall over if a rate cannot be
                    // found for a given postcode. If this happens then we don't really care.
                }

                if (surcharges != null)
                {
                    var rs = (from s in surcharges
                              where s.ExtraTypeID == (int)extraType
                              select s).FirstOrDefault();

                    if (rs != null)
                        rntAmount.Value = (double?)rs.ForeignRate;
                }
            }
        }

        #endregion

        #region Control Event Handlers

        void btnDeleteExtra_Click(object sender, EventArgs e)
        {
            Facade.IJobExtra facExtra = new Facade.Job();
            bool retVal = facExtra.DeleteExtra(m_extraId);
            if (retVal)
            {
                this.ReturnValue = "refresh";
                this.Close();
            }
            else
            {
                lblError.Text = "There was a problem trying to remove this extra.";
                pnlError.Visible = true;
            }
        }

        void btnAddExtra_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
            {
                Facade.IJobExtra facExtra = new Facade.Job();
                Entities.Extra extra = new Orchestrator.Entities.Extra();

                if ((eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedValue.Replace(" ", "")) == eExtraState.Accepted && txtClientContact.Text == "")
                {
                    pnlError.Visible = true;
                    lblError.Text = "If the Extra has been accepted please state whom by.";
                    return;
                }

                if (m_extraId > 0)
                    UpdateExtra();
                else
                    CreateExtra();

                this.ReturnValue = "refresh";
                this.Close();
            }
        }

        #endregion

        #region Data Loading/Manipulation

        private void LoadExtra()
        {
            Facade.IJobExtra facExtra = new Facade.Job();
            Entities.Extra extra = facExtra.GetExtraForExtraId(m_extraId);
            string isBeingInvoiced = facExtra.IsExtraBeingInvoiced(m_extraId);

            cboExtraState.ClearSelection();
            cboExtraState.Items.FindByValue(Utilities.UnCamelCase(extra.ExtraState.ToString())).Selected = true;
            txtClientContact.Text = extra.ClientContact;

            rntAmount.Text = extra.ForeignAmount.ToString();
            m_exchangeRateID = extra.ExchangeRateID;

            txtComment.Text = extra.CustomDescription;
            
            cboExtraType.ClearSelection();
            cboExtraType.Items.FindByValue(((int)extra.ExtraType).ToString()).Selected = true;

            cboExtraType.Enabled = extra.ExtraState != eExtraState.Invoiced && String.IsNullOrEmpty(isBeingInvoiced);
            cboExtraState.Enabled = String.IsNullOrEmpty(isBeingInvoiced);
            rntAmount.Enabled = extra.ExtraState != eExtraState.Invoiced && String.IsNullOrEmpty(isBeingInvoiced);

            btnAddExtra.Visible = String.IsNullOrEmpty(isBeingInvoiced);
            btnDeleteExtra.Visible = extra.ExtraState != eExtraState.Invoiced && String.IsNullOrEmpty(isBeingInvoiced);

            divInvoicedMessage.Visible = !String.IsNullOrEmpty(isBeingInvoiced);
            lblInvoiceReason.Text = "This Extra cannot be altered as it is currently on " + isBeingInvoiced;


            Session["_extra"] = extra;
        }

        //private void UpdateVigoNotes1(int orderId)
        //{
        //    // Update the order palletforce notes 1 field to reflect the addition of the extra
        //    Facade.IVigoOrder facVigoOrder = new Orchestrator.Facade.Order();
        //    Entities.VigoOrder vigoOrder = facVigoOrder.GetForOrderId(m_orderID);
        //    Facade.IOrderExtra facExtra = new Facade.Order();

        //    Facade.IOrder facOrder = new Facade.Order();
        //    Entities.Order order = facOrder.GetForOrderID(m_orderID);

        //    Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
        //    List<Entities.ExtraType> ets = facExtraType.GetForIsEnabled(new bool?());
        //    string vigoNotes1_ForSurchargeDescriptions = string.Empty;

        //    if (vigoOrder != null)
        //    {
        //        DataSet dsExtras = facExtra.GetExtrasForOrderID(m_orderID);
        //        if (dsExtras != null && dsExtras.Tables[0] != null)
        //        {
        //            if (dsExtras.Tables[0].Rows.Count > 0)
        //            {
        //                foreach (DataRow row in dsExtras.Tables[0].Rows)
        //                {
        //                    if (row["ExtraAppliesToID"].ToString() == "1") // These are client extras not network extras.
        //                    {
        //                        Entities.ExtraType extraType = ets.Find(et => et.ExtraTypeId == int.Parse(row["ExtraTypeId"].ToString()));
        //                        if (extraType != null)
        //                        {
        //                            //Some hard coded nastiness that will need to be changed at some point...

        //                            //BW    Delivery between [deliver from time] – [deliver by time] hrs.
        //                            //BI    Please book in.
        //                            //AM    AM delivery.
        //                            //TB    Deliver at [deliver by time] hrs. or if not A service: Deliver on [delivery date] at [deliver by time] hrs.
        //                            //EB    Deliver on [delivery date].
        //                            //TL    Tail-lift delivery.
        //                            //SA    SATURDAY AM DELIVERY.
        //                            //TA    Deliver by 10am.

        //                            string textToAppend = string.Empty;
        //                            switch (extraType.ShortDescription.ToUpper())
        //                            {
        //                                case "BW":
        //                                    textToAppend = string.Format("Delivery between {0} – {1} hrs", order.DeliveryFromDateTime.ToString("hh:mm"), order.DeliveryDateTime.ToString("hh:mm"));
        //                                    break;

        //                                case "BI":
        //                                    textToAppend = "Please book in";
        //                                    break;

        //                                case "AM":
        //                                    textToAppend = "AM dellivery";
        //                                    break;

        //                                case "TB":
        //                                    textToAppend = string.Format("Deliver on {0} at {1} hrs", order.DeliveryDateTime.ToString("dd/MM/yy"), order.DeliveryDateTime.ToString("hh:mm"));
        //                                    break;

        //                                case "EB":
        //                                    textToAppend = string.Format("Deliver on {0}", order.DeliveryDateTime.ToString("dd/MM/yy"));
        //                                    break;

        //                                case "SA":
        //                                    textToAppend = "SATURDAY AM DELIVERY";
        //                                    break;

        //                                case "TA":
        //                                    textToAppend = "Deliver by 10am";
        //                                    break;

        //                                default:
        //                                    break;
        //                            }

        //                            if (textToAppend != string.Empty)
        //                                vigoNotes1_ForSurchargeDescriptions = AppendToVigoNotes(vigoNotes1_ForSurchargeDescriptions, textToAppend, ":");
        //                        }
        //                    }
        //                }

        //                // Update the vigo order with the new notes 1 information
        //                if (vigoOrder.Note1 != vigoNotes1_ForSurchargeDescriptions)
        //                {
        //                    vigoOrder.Note1 = vigoNotes1_ForSurchargeDescriptions;
        //                    facVigoOrder.CreateOrUpdate(vigoOrder, this.User.Identity.Name);
        //                }
        //            }
        //            else
        //            {
        //                // Update the vigo order with the new notes 1 information (which should be blank as there are no surcharges).
        //                if (vigoOrder.Note1 != "")
        //                {
        //                    vigoOrder.Note1 = "";
        //                    facVigoOrder.CreateOrUpdate(vigoOrder, this.User.Identity.Name);
        //                }
        //            }
        //        }
        //    }
        //}

        private void CreateExtra()
        {
            Entities.Extra extra = new Orchestrator.Entities.Extra();

            extra.JobId = m_jobId;
            if (m_instructionId > 0)
            {
                m_orderID = int.Parse(cboClient.SelectedValue);
            }
            

            extra.OrderID = m_orderID;

            extra = PopulateExtra(extra);

            //if (extra.ExtraType == eExtraType.Demurrage)
            //{
            //    extra.DemurrageComments = txtDemurrageComments.Text;
            //    extra.InstructionId = m_instructionId;
            //}

            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            Facade.IOrderExtra facExtra = new Facade.Order();
            int extraId = facExtra.Create(extra, this.Page.User.Identity.Name);
            if (extraId > 0)
            {
                //lblInjectScript.Text = "<script>window.close()</script>";

                // If this is an order extra....
                //if (m_orderID != 0)
                //    UpdateVigoNotes1(m_orderID);
            }
            else
            {
                lblError.Text = "There was a problem trying to Add this extra.";
                pnlError.Visible = true;
            }
        }

        private string AppendToVigoNotes(string notes, string textToAppend, string delimiter)
        {
            if (textToAppend.Length > 36)
                return notes;

            if (notes.Length == 0)
                return textToAppend;

            if (string.Format("{0}{1} {2}", notes, delimiter, textToAppend).Length > 36)
                return notes;

            return string.Format("{0}{1} {2}", notes, delimiter, textToAppend);
        }

        private void UpdateExtra()
        {
            Entities.Extra extra = (Entities.Extra)Session["_extra"];

            extra = PopulateExtra(extra);

            //if (extra.ExtraType == eExtraType.Demurrage)
            //    extra.DemurrageComments = txtDemurrageComments.Text;

            Facade.IOrderExtra facExtra = new Facade.Order();
            if (facExtra.Update(extra, ((Entities.CustomPrincipal)Page.User).UserName))
            {
               // lblInjectScript.Text = "<script>window.close()</script>";
            }
            else
            {
                lblError.Text = "There was a problem trying to update this extra.";
                pnlError.Visible = true;
            }
        }

        private Entities.Extra PopulateExtra(Entities.Extra extra)
        {
            decimal rate = 0m;
            decimal fuelSurchargeRate = 0m;

            extra.ExtraType = (eExtraType)Enum.Parse(typeof(eExtraType), cboExtraType.SelectedValue.Replace(" ", ""));
            extra.ExtraState = (eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedValue.Replace(" ", ""));
            extra.ClientContact = txtClientContact.Text;

            
            if (rntAmount.Value.HasValue)
            {
                rate = (decimal)rntAmount.Value.Value;

                if (ApplyFuelSurcharge())
                {  
                    fuelSurchargeRate = Math.Round(rate * (m_fuelSurchargePercentage/100.0m), 4,MidpointRounding.AwayFromZero);
                }
            }

            extra.ForeignAmount = rate;
            extra.FuelSurchargeForeignAmount = fuelSurchargeRate;

            if (m_exchangeRateID != null && extra.ForeignAmount > 0)
            {
                Facade.IExchangeRates facER = new Facade.ExchangeRates();
                extra.ExtraAmount = facER.GetConvertedRate((int)m_exchangeRateID, extra.ForeignAmount);
                extra.FuelSurchargeAmount = facER.GetConvertedRate((int)m_exchangeRateID,
                                                                   extra.FuelSurchargeForeignAmount);
                extra.ExchangeRateID = m_exchangeRateID;
            }
            else
            {
                extra.ExtraAmount = extra.ForeignAmount;
                extra.FuelSurchargeAmount = extra.FuelSurchargeForeignAmount;
            }

            extra.CustomDescription = txtComment.Text;

            return extra;
        }

        private bool ApplyFuelSurcharge()
        {
            Facade.IOrder facadeOrder = new Facade.Order();
            var order = facadeOrder.GetForOrderID(this.OrderID);

            Facade.Organisation facOrg = new Facade.Organisation();
            var defaults = facOrg.GetDefaultsForIdentityId(order.CustomerIdentityID);

            Facade.ExtraType facExtraType = new Facade.ExtraType();
            var extraType = facExtraType.GetForExtraTypeID(int.Parse(cboExtraType.SelectedValue));

            m_fuelSurchargePercentage = order.FuelSurchargePercentage;
            bool fuelSurchargeAppliesToExtras = bool.Parse(defaults.Tables[0].Rows[0]["FuelSurchargeOnExtras"].ToString());

            return fuelSurchargeAppliesToExtras && extraType.FuelSurchargeApplies;
        }

        #endregion
    }
}
