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

namespace Orchestrator.WebUI.Resource.Driver
{
    public partial class DriverContactPopUp : Orchestrator.Base.BasePage
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["identityId"] == string.Empty || Request.QueryString["identityId"] == null) return;
                string identityId = Request.QueryString["identityId"].ToString();

                if (!string.IsNullOrEmpty(identityId))
                {
                    // Split string to see if we need to get details for an individual or an organisation

                    string[] s = identityId.Split(":".ToCharArray());
                    string driverType = s[0].ToLower().Trim();

                    switch (driverType)
                    {
                        case "organisation":
                            tblSubContractorIndividual.Visible = false;

                            int organisationId = Convert.ToInt32(s[1]);
                            BusinessLogicLayer.Organisation busOrg = new BusinessLogicLayer.Organisation();
                            Entities.Organisation org = busOrg.GetForIdentityId(organisationId);

                            Entities.OrganisationLocation headOffice = null;
                            if (org.Locations != null)
                                headOffice = org.Locations.GetHeadOffice();

                            lblOrganisationFullName.Text = org.OrganisationDisplayName;
                            lblOrganisationMainTelephone.Text = org.Locations[0].TelephoneNumber;

                            if (headOffice != null && headOffice.Point != null)
                            {
                                lblOrganisationAddressLine1.Text = headOffice.Point.Address.AddressLine1;
                                lblOrganisationAddressLine2.Text = headOffice.Point.Address.AddressLine2;
                                lblOrganisationAddressLine3.Text = headOffice.Point.Address.AddressLine3;
                                lblOrganisationPostTown.Text = headOffice.Point.Address.PostTown;
                                lblOrganisationCounty.Text = headOffice.Point.Address.County;
                                lblOrganisationPostCode.Text = headOffice.Point.Address.PostCode;
                            }

                            break;
                        case "individual":

                            tblSubContractorOrganisation.Visible = false;

                            int individualIdentityId = Convert.ToInt32(s[1]);
                            DataAccess.IIndividual dacIndividual = new DataAccess.Individual();
                            DataSet dsIndividual = dacIndividual.GetContactDetails(individualIdentityId);

                            lblIndividualFullName.Text = dsIndividual.Tables[0].Rows[0]["FullName"].ToString();
                            lblIndiviualHomePhone.Text = dsIndividual.Tables[0].Rows[0]["HomePhone"].ToString();
                            lblIndividualMobilePhone.Text = dsIndividual.Tables[0].Rows[0]["MobilePhone"].ToString();
                            lblIndividualPersonalMobile.Text = dsIndividual.Tables[0].Rows[0]["PersonalMobile"].ToString();
                            lblIndividualAddressLine1.Text = dsIndividual.Tables[0].Rows[0]["AddressLine1"].ToString();
                            lblIndividualAddressLine2.Text = dsIndividual.Tables[0].Rows[0]["AddressLine2"].ToString();
                            lblIndividualAddressLine3.Text = dsIndividual.Tables[0].Rows[0]["AddressLine3"].ToString();
                            lblIndividualPostTown.Text = dsIndividual.Tables[0].Rows[0]["PostTown"].ToString();
                            lblIndividualCounty.Text = dsIndividual.Tables[0].Rows[0]["County"].ToString();
                            lblIndividualPostCode.Text = dsIndividual.Tables[0].Rows[0]["PostCode"].ToString();

                            break;
                        default:

                            break;
                    }
                }
            }
        }
    }
}
