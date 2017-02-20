using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Pallet
{
    public partial class AuditPalletBalance : System.Web.UI.Page
    {
        #region properties

        private const string vs_palletBalanceType = "vs_isClievs_palletBalanceTypent";
        protected ePalletBalanceType PalletBalanceType
        {
            get { return ViewState[vs_palletBalanceType] != null ? (ePalletBalanceType)ViewState[vs_palletBalanceType] : ePalletBalanceType.NotSet; }
            set { ViewState[vs_palletBalanceType] = value; }
        }

        private const string vs_ID = "vs_ID";
        protected int currentID
        {
            get {return ViewState[vs_ID] != null ? (int)ViewState[vs_ID] : -1;}
            set { ViewState[vs_ID] = value; }
        }

        private const string vs_palletTypeID = "vs_palletTypeID";
        protected int PalletTypeID
        {
            get { return ViewState[vs_palletTypeID] != null ? (int)ViewState[vs_palletTypeID] : -1; }
            set { ViewState[vs_palletTypeID] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnBack.Click += new EventHandler(btnBack_Click);
        }

        #region Private Methods

        private void ConfigureDisplay()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["cID"]))
            {
                PalletBalanceType = ePalletBalanceType.ClientPalletBalance;
                currentID = int.Parse(Request.QueryString["cID"]);
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["pID"]))
            {
                PalletBalanceType = ePalletBalanceType.PointPalletBalance;
                currentID = int.Parse(Request.QueryString["pID"]);
            }
            else if (!string.IsNullOrEmpty(Request.QueryString["tID"]))
            {
                PalletBalanceType = ePalletBalanceType.TrailerPalletBalance;
                currentID = int.Parse(Request.QueryString["tID"]);
            }

            if (!string.IsNullOrEmpty(Request.QueryString["ptID"]))
                PalletTypeID = int.Parse(Request.QueryString["ptID"]);
            else
                PalletBalanceType = ePalletBalanceType.NotSet;

            //switch (PalletBalanceType)
            //{
            //    case ePalletBalanceType.ClientPalletBalance:

            //        break;
            //    case ePalletBalanceType.PointPalletBalance:
                    
            //        break;
            //    case ePalletBalanceType.TrailerPalletBalance:
                    
            //        break;
            //    case ePalletBalanceType.NotSet:
                    
            //        break;
            //}

            rcbPalletType.DataSource = Facade.PalletType.GetAllPalletTypes();
            rcbPalletType.DataBind();

            if (PalletTypeID > 0)
                rcbPalletType.SelectedValue = PalletTypeID.ToString();

            rdiStartDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day).AddMonths(-1);
            rdiEndDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);

            switch (PalletBalanceType)
            {
                case ePalletBalanceType.ClientPalletBalance:
                    LoadClientAuditTrail();
                    break;
                case ePalletBalanceType.PointPalletBalance:
                    LoadPointAuditTrail();
                    break;
                case ePalletBalanceType.TrailerPalletBalance:
                    LoadTrailerAuditTrail();
                    break;
                case ePalletBalanceType.NotSet:
                    rcbPalletType.DataSource = Facade.PalletType.GetAllPalletTypes();
                    PalletBalanceNotSet();
                    break;
            }
        }

        private void LoadClientAuditTrail()
        {
            pnlClient.Visible = true;

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Entities.Organisation currentOrg = facOrganisation.GetForIdentityId(currentID);

            lblPalletBalanceType.Text = "Client Pallet Audit Trail : " + currentOrg.OrganisationName;

            Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
            lvClientAudit.DataSource = facPalletBalance.GetClientAuditTrail(currentID, int.Parse(rcbPalletType.SelectedValue), rdiStartDate.SelectedDate.Value, rdiEndDate.SelectedDate.Value);
            lvClientAudit.DataBind();
        }

        private void LoadPointAuditTrail()
        {
            pnlPoint.Visible = true;

            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point currentPoint = facPoint.GetPointForPointId(currentID);

            lblPalletBalanceType.Text = "Point Pallet Audit Trail : " + currentPoint.Description;

            Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
            lvPointAudit.DataSource = facPalletBalance.GetPointAuditTrail(currentID, int.Parse(rcbPalletType.SelectedValue), rdiStartDate.SelectedDate.Value, rdiEndDate.SelectedDate.Value);
            lvPointAudit.DataBind();
        }

        private void LoadTrailerAuditTrail()
        {
            pnlTrailer.Visible = true;

            Facade.ITrailer facTrailer = new Facade.Resource();
            Entities.Trailer currentTrailer = facTrailer.GetForTrailerId(currentID);

            lblPalletBalanceType.Text = "Trailer Pallet Audit Trail : " + currentTrailer.TrailerRef;

            Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
            lvTrailerAudit.DataSource = facPalletBalance.GetTrailerAuditTrail(currentID, int.Parse(rcbPalletType.SelectedValue), rdiStartDate.SelectedDate.Value, rdiEndDate.SelectedDate.Value);
            lvTrailerAudit.DataBind();
        }

        private void PalletBalanceNotSet()
        {
            lblPalletBalanceType.Text = "Pallet Balance Type Not Set";
        }

        #endregion

        #region Events

        void btnRefresh_Click(object sender, EventArgs e)
        {
            switch (PalletBalanceType)
            {
                case ePalletBalanceType.ClientPalletBalance:
                    LoadClientAuditTrail();
                    break;
                case ePalletBalanceType.PointPalletBalance:
                    LoadPointAuditTrail();
                    break;
                case ePalletBalanceType.TrailerPalletBalance:
                    LoadTrailerAuditTrail();
                    break;
                case ePalletBalanceType.NotSet:
                    break;
            }
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("ViewPalletBalances.aspx");
        }

        #endregion

    }
}
