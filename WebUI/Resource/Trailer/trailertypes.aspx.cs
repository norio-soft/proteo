using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Resource.Trailer
{
    public partial class trailertypes : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnAdd.Click += new EventHandler(btnAdd_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnUpdate.Click += new EventHandler(btnUpdate_Click);
            this.grdTrailerTypes.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdTrailerTypes_NeedDataSource);
            this.grdTrailerTypes.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(grdTrailerTypes_ItemCommand);
        }

        void grdTrailerTypes_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "select")
            {
                // Load the Trailer Type
                this.hidTrailerTypeID.Value = grdTrailerTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["TrailerTypeId"].ToString();
                this.txtDescription.Text = grdTrailerTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["Description"].ToString();
                this.lblCreated.Text = string.Format("Created By {0} at {1}", grdTrailerTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["CreateUserID"].ToString(), ((DateTime)grdTrailerTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["CreateDate"]).ToString("dd/MM/yy"));
                this.lblUpdated.Text = string.Format("LastUpdated Created By {0} at {1}", grdTrailerTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["LastUpdateUserID"].ToString(), ((DateTime)grdTrailerTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["LastUpdateDate"]).ToString("dd/MM/yy"));

                this.fsTrailerType.Visible = true;
                this.fsTrailerTypes.Visible = false;
            }
        }

        void grdTrailerTypes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.ITrailer facTrailer = new Facade.Resource();
            this.grdTrailerTypes.DataSource = facTrailer.GetAllTrailerTypes();
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            Facade.ITrailer facTrailer = new Facade.Resource();
            int trailerTypeID = 0;
            int.TryParse(hidTrailerTypeID.Value, out trailerTypeID);
            int retVal = facTrailer.UpdateTrailerType(trailerTypeID, txtDescription.Text, Page.User.Identity.Name);
            if (retVal > 0)
            {
                fsTrailerType.Visible = false;
                fsTrailerTypes.Visible = true;
                grdTrailerTypes.Rebind();
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.fsTrailerType.Visible = false;
            this.fsTrailerTypes.Visible = true;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            this.hidTrailerTypeID.Value = "0";
            this.txtDescription.Text = String.Empty;
            this.lblCreated.Text = String.Empty;
            this.lblUpdated.Text = String.Empty;
            this.fsTrailerType.Visible = true;
            this.fsTrailerTypes.Visible = false;
        }
    }
}
