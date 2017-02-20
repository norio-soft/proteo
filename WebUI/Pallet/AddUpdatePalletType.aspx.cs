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

using Orchestrator.Globals;
using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Pallet
{
    public partial class AddUpdatePalletType : Orchestrator.Base.BasePage
    {
        private int _palletTypeId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            int.TryParse(Request.QueryString["PalletTypeId"], out _palletTypeId);

            if (!IsPostBack)
            {
                if (_palletTypeId > 0)
                    LoadPalletType();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnRemoveFromAllClients.Click += new EventHandler(btnRemoveFromAllClients_Click);
            btnReturnToList.Click += new EventHandler(btnReturnToList_Click);
        }

        private void LoadPalletType()
        {
            Entities.PalletType palletType = RetrievePalletType();
            if (palletType != null)
            {
                txtDescription.Text = palletType.Description;
                chkTrack.Checked = palletType.TrackByDefault;
                chkForNewClients.Checked = palletType.ActiveForNewClients;
                chkIsDefault.Checked = palletType.IsDefaultForNewClients;

                btnAdd.Text = "Update Pallet Type";
                btnRemoveFromAllClients.Visible = true;
            }
        }

        private Entities.PalletType PopulatePalletType()
        {
            Entities.PalletType palletType;

            if (_palletTypeId > 0)
                palletType = RetrievePalletType();
            else
                palletType = new Entities.PalletType();

            palletType.Description = txtDescription.Text;
            palletType.TrackByDefault = chkTrack.Checked;
            palletType.ActiveForNewClients = chkForNewClients.Checked;
            palletType.IsDefaultForNewClients = chkIsDefault.Checked;

            return palletType;
        }

        private Entities.PalletType RetrievePalletType()
        {
            Entities.PalletType palletType = Facade.PalletType.GetForPalletTypeId(_palletTypeId);
            return palletType;
        }

        #region Event Handlers

        #region Button Events

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.PalletType palletType = PopulatePalletType();

                if (_palletTypeId > 0)
                    Facade.PalletType.Update(palletType, ((Entities.CustomPrincipal)Page.User).UserName);
                else
                    Facade.PalletType.Create(palletType, ((Entities.CustomPrincipal)Page.User).UserName);

                Response.Redirect("ListPalletTypes.aspx");
            }
        }

        void btnRemoveFromAllClients_Click(object sender, EventArgs e)
        {
            Facade.PalletType.RemoveFromAllClients(_palletTypeId);

            Response.Redirect("ListPalletTypes.aspx");
        }

        void btnReturnToList_Click(object sender, EventArgs e)
        {
            Response.Redirect("ListPalletTypes.aspx");
        }

        #endregion

        #endregion
    }
}
