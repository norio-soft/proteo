using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

using System.Collections.Generic;

namespace Orchestrator.WebUI.Client
{
    public partial class ClientPointList : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            GetClientId();

            if (!this.IsPostBack)
                this.grdPoints.Rebind();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdPoints.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdPoints_NeedDataSource);
            //this.grdPoints.ItemDataBound += new GridItemEventHandler(grdPoints_ItemDataBound);

            this.btnRefreshTop.Click += new EventHandler(btnRefresh_Click);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
        }

        //void grdPoints_ItemDataBound(object sender, GridItemEventArgs e)
        //{
        //if (e.Item.ItemType == GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.Item)
        //{
        //    Telerik.WebControls.GridItem item = e.Item as Telerik.WebControls.GridItem;
        //}
        //}

        private int _clientIdentityId = 0;
        private void GetClientId()
        {
            Entities.CustomPrincipal cp = Page.User as Entities.CustomPrincipal;

            if (cp.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                Facade.IUser facUser = new Facade.User();
                SqlDataReader reader = facUser.GetRelatedIdentity(((Entities.CustomPrincipal)Page.User).UserName);
                reader.Read();

                if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
                {
                    int RelatedIdentityID = int.Parse(reader["RelatedIdentityId"].ToString());
                    _clientIdentityId = RelatedIdentityID;
                }
                else
                {
                    throw new ApplicationException("User is not a client user.");
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            this.grdPoints.Rebind();
        }

        private void grdPoints_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.grdPoints.DataSource = null;
            Facade.IPoint facPoint = new Facade.Point();
            DataSet dsPoints = facPoint.GetAllWithAddressForClient(_clientIdentityId);
            this.grdPoints.DataSource = dsPoints;
        }
    }
}
