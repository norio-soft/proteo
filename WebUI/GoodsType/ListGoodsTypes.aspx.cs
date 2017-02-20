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
    public partial class ListGoodsTypes : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvGoodsTypes.DataSource = Facade.GoodsType.GetAllActiveGoodsTypes();
                gvGoodsTypes.DataBind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnAdd.Click += new EventHandler(btnAdd_Click);
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddUpdateGoodsType.aspx");
        }
    }
}