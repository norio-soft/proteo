using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class DepotList : Orchestrator.Base.BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadNetworks();
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            this.Title = "Orchestrator - Depots";

            grdDepots.NeedDataSource += new GridNeedDataSourceEventHandler(grdDepots_NeedDataSource);
            grdDepots.InsertCommand += new GridCommandEventHandler(grdDepots_InsertCommand);
            grdDepots.UpdateCommand += new GridCommandEventHandler(grdDepots_UpdateCommand);
            grdDepots.DeleteCommand += new GridCommandEventHandler(grdDepots_DeleteCommand);
            grdDepots.ItemDataBound += new GridItemEventHandler(grdDepots_ItemDataBound);

            cboNetworks.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboNetworks_SelectedIndexChanged);
        }

        #region Private Functions

        private void LoadNetworks()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var depotRepo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);

                var networks =
                    from n in depotRepo.GetAllNetworks()
                    orderby n.Organisation.OrganisationName
                    select new { n.IdentityID, n.Organisation.OrganisationName };

                cboNetworks.DataValueField = "IdentityId";
                cboNetworks.DataTextField = "OrganisationName";
                cboNetworks.DataSource = networks.ToList();
                cboNetworks.DataBind();
            }

        }

        private void LoadDepots()
        {
            if (cboNetworks.SelectedIndex == -1)
                grdDepots.DataSource = null;
            else
            {
                //Get the IdentityId of the Network from the combo
                int identityId = int.Parse(cboNetworks.SelectedValue);

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var depotRepo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);

                    grdDepots.DataSource =
                        (from d in depotRepo.GetAll()
                         where d.NetworkIdentityID == identityId
                         select new
                         {
                             d.DepotID,
                             d.Code,
                             PointDescription = d.Point.Description,
                             d.HubIdentifier,
                             d.PrintOnLabel,
                         }).ToList();
                }
            }
        }

        #endregion

        #region Events

        #region Grid Events

        void grdDepots_DeleteCommand(object source, GridCommandEventArgs e)
        {
            //Get the DepotId of the grid row
            int depotId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["DepotId"];
            
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var depotRepo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);

                var companyNetwork = (from d in depotRepo.GetAllNetworks()
                                      where d.CompanyDepotID == depotId
                                      select 1).Count();

                if (companyNetwork > 0)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "Network Depot", "alert('You cannot delete the Company Depot for this Network.');", true);
                    e.Canceled = true;
                }
                else
                {
                    var depotPostCodeAreas = (from d in depotRepo.GetAllDepotPostcodeAreas()
                                              where d.DepotID == depotId
                                              select d).ToList();

                    foreach (Models.DepotPostcodeArea dpca in depotPostCodeAreas)
                        depotRepo.DeleteDepotPostcodeArea(dpca);

                    // Do an initial save in case we have removed any Depot Post Codes or there will be an FK SQL Error
                    uow.SaveChanges();

                    depotRepo.Remove(depotId);
                    uow.SaveChanges();
                }
            }
        }

        void grdDepots_UpdateCommand(object source, GridCommandEventArgs e)
        {
            // Ensure that all points are created.
            Page.Validate("PointSavedValidation");
            bool pointsValid = Page.IsValid;

            if (pointsValid)
            {
                // Get all the information out of the Grid Row
                Orchestrator.WebUI.Controls.Point ucPoint = (Orchestrator.WebUI.Controls.Point)e.Item.FindControl("ucPoint");
                TextBox txtCode = (TextBox)e.Item.FindControl("txtCode");
                TextBox txtHubIdentifier = (TextBox)e.Item.FindControl("txtHubIdentifier");
                RadComboBox cboPrintOnLabel = (RadComboBox)e.Item.FindControl("cboPrintOnLabel");
                Label txtCodeProblem = (Label)e.Item.FindControl("txtCodeProblem");
                int identityId = int.Parse(cboNetworks.SelectedValue);
                int depotId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["DepotId"];

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var depotRepo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);
                    var depot = depotRepo.Find(depotId);

                    // Check if there is a Depot which already has this code for this Network (and which isn't THIS record!).
                    var depotCodeCount = depotRepo.GetAll().Count(d => d.NetworkIdentityID == identityId
                                                                        && d.Code == txtCode.Text
                                                                        && d.DepotID != depot.DepotID);

                    if (depotCodeCount > 0)
                    {
                        // Code already in use, show the error on-screen.
                        txtCodeProblem.Visible = true;
                        e.Canceled = true;
                    }
                    else
                    {
                        txtCodeProblem.Visible = false;
                        depot.Code = txtCode.Text;
                        depot.PointID = ucPoint.PointID;
                        depot.HubIdentifier = txtHubIdentifier.Text;
                        depot.PrintOnLabel = cboPrintOnLabel.Text == "Yes";
                        uow.SaveChanges();
                    }
                }
            }
            else
                e.Canceled = true;
        }

        void grdDepots_InsertCommand(object source, GridCommandEventArgs e)
        {
            // Ensure that all points are created.
            Page.Validate("PointSavedValidation");
            bool pointsValid = Page.IsValid;

            if (pointsValid)
            {
                //Find the Point control and Code textbox within the grid row
                Orchestrator.WebUI.Controls.Point ucPoint = (Orchestrator.WebUI.Controls.Point)e.Item.FindControl("ucPoint");
                TextBox txtCode = (TextBox)e.Item.FindControl("txtCode");
                TextBox txtHubIdentifier = (TextBox)e.Item.FindControl("txtHubIdentifier");
                RadComboBox cboPrintOnLabel = (RadComboBox)e.Item.FindControl("cboPrintOnLabel");
                Label txtCodeProblem = (Label)e.Item.FindControl("txtCodeProblem");
                int identityId = int.Parse(cboNetworks.SelectedValue);

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var depotRepo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);
                    var depot = new Models.Depot();
                    
                    // Check if any Depots already have this Depot Code for this Network
                    var depotCodeCount = depotRepo.GetAll().Count(d => d.NetworkIdentityID == identityId
                                                                        && d.Code == txtCode.Text);

                    if (depotCodeCount > 0)
                    {
                        // Code already exists, inform the user.
                        txtCodeProblem.Visible = true;
                        e.Canceled = true;
                    }
                    else
                    {
                        txtCodeProblem.Visible = false;
                        depot.Code = txtCode.Text;
                        depot.PointID = ucPoint.PointID;
                        depot.HubIdentifier = txtHubIdentifier.Text;
                        depot.PrintOnLabel = cboPrintOnLabel.Text == "Yes";
                        depot.NetworkIdentityID = identityId;

                        depotRepo.Add(depot);
                        uow.SaveChanges();
                    }
                }
            }
            else
                e.Canceled = true;
        }

        void grdDepots_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            LoadDepots();
        }

        protected void grdDepots_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //If the Grid Item is the EditForm and it is being shown
            if (e.Item.ItemType == GridItemType.EditFormItem && e.Item.IsInEditMode)
            {
                //Get the Point and  Code controls from the EditForm
                Orchestrator.WebUI.Controls.Point ucPoint = (Orchestrator.WebUI.Controls.Point)e.Item.FindControl("ucPoint");
                TextBox txtCode = (TextBox)e.Item.FindControl("txtCode");
                TextBox txtHubIdentifier = (TextBox)e.Item.FindControl("txtHubIdentifier");
                RadComboBox cboPrintOnLabel = (RadComboBox)e.Item.FindControl("cboPrintOnLabel");

                //If the EditForm is being shown for a new Depot default the values
                if (e.Item.OwnerTableView.IsItemInserted)
                {
                    //Set the default values for a new Depot
                    txtCode.Text = string.Empty;
                    txtHubIdentifier.Text = string.Empty;
                    cboPrintOnLabel.SelectedIndex = 1;
                    ucPoint.Reset();
                }
                else
                {
                    int depotId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["DepotId"];

                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var depotRepo = DIContainer.CreateRepository<Repositories.IDepotRepository>(uow);
                        var depot = depotRepo.Find(depotId);

                        Facade.Point facPoint = new Orchestrator.Facade.Point();
                        Entities.Point point = facPoint.GetPointForPointId(depot.PointID);
                        ucPoint.SelectedPoint = point;

                        txtCode.Text = depot.Code;
                        txtHubIdentifier.Text = depot.HubIdentifier;

                        if (depot.PrintOnLabel == null || depot.PrintOnLabel == false)
                            cboPrintOnLabel.SelectedIndex = 1;
                        else
                            cboPrintOnLabel.SelectedIndex = 0;
                    }
                }

            }
        }

        #endregion

        #region ComboBox Events

        void cboNetworks_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //A different pallet network has been selected so load the Depots for it
            LoadDepots();
        }

        #endregion

        #endregion
    }
}
