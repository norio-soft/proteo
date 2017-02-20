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

namespace Orchestrator.WebUI.GoodsType
{
    public partial class AddUpdateGoodsType : Orchestrator.Base.BasePage
    {
        private int _goodsTypeId = 0;

        protected void Page_Load(object sender, EventArgs e)
        {
            int.TryParse(Request.QueryString["GoodsTypeId"], out _goodsTypeId);

            if (!IsPostBack)
            {
                if (_goodsTypeId > 0)
                    LoadGoodsType();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAdd.Click += new EventHandler(btnAdd_Click);
            btnReturn.Click += new EventHandler(btnReturn_Click);
        }

        private void LoadGoodsType()
        {
            Entities.GoodsType goodsType = RetrieveGoodsType();
            if (goodsType != null)
            {
                txtDescription.Text = goodsType.Description;
                txtShortCode.Text = goodsType.ShortCode;
                chkHazardous.Checked = goodsType.IsHazardous;
                chkDefault.Checked = goodsType.IsDefault;
                btnAdd.Text = "Update Goods Type";
            }
        }

        private Entities.GoodsType PopulateGoodsType()
        {
            Entities.GoodsType goodsType;

            if (_goodsTypeId > 0)
                goodsType = RetrieveGoodsType();
            else
                goodsType = new Entities.GoodsType();

            goodsType.Description = txtDescription.Text;
            goodsType.ShortCode = txtShortCode.Text;
            goodsType.IsDefault = chkDefault.Checked;
            goodsType.IsHazardous = chkHazardous.Checked;
            return goodsType;
        }

        private Entities.GoodsType RetrieveGoodsType()
        {
            Entities.GoodsType goodsType = Facade.GoodsType.GetForGoodsTypeId(_goodsTypeId);
            return goodsType;
        }

        #region Event Handlers

        void btnAdd_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.GoodsType goodsType = PopulateGoodsType();

                if (_goodsTypeId > 0)
                    Facade.GoodsType.Update(goodsType, ((Entities.CustomPrincipal) Page.User).UserName);
                else
                    Facade.GoodsType.Create(goodsType, ((Entities.CustomPrincipal) Page.User).UserName);

                Response.Redirect("ListGoodsTypes.aspx");
            }
        }

        void btnReturn_Click(object sender, EventArgs e)
        {
            Response.Redirect("ListGoodsTypes.aspx");
        }

        #endregion
    }
}