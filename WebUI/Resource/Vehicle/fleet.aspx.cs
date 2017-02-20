using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;
using System.Data.SqlClient;
using System.Data.Common;
using System.Text;
using System.Web.Services;

namespace Orchestrator.WebUI.Resource.Vehicle
{

    public partial class Fleet : Orchestrator.Base.BasePage
    {

        protected string BlueSphereCustomerID
        { 
            get { return Globals.Configuration.BlueSphereCustomerId; }
        }

        public int? ClientIdentityID { get; set; }

 

        protected void Page_Load(object sender, EventArgs e)
        {
            var principal = (Entities.CustomPrincipal)Page.User;

            if (principal.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                Facade.IUser facUser = new Facade.User();

                using (var reader = facUser.GetRelatedIdentity(principal.UserName))
                {
                    reader.Read();

                    if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
                        this.ClientIdentityID = (int)reader["RelatedIdentityId"];
                    else
                        throw new ApplicationException("User is in Client User role but is not related to a specific client.");
                }
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (IsClientUser)
                Page.MasterPageFile = "~/default_tableless_client.Master";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
        }
      

    }

}
