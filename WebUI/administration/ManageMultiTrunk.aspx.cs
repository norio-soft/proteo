using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Generic;

namespace Orchestrator.WebUI.Traffic
{

    //--------------------------------------------------------------------------------------------------------

    public partial class ManageMultiTrunk : Orchestrator.Base.BasePage
    {

        //--------------------------------------------------------------------------------------------------------
        
        protected void Page_Load(object sender, EventArgs e)
        {
            //this.multiTrunksRadGrid.DataBind();
        }

        //--------------------------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.multiTrunksRadGrid.NeedDataSource += 
                new Telerik.Web.UI.GridNeedDataSourceEventHandler(multiTrunksRadGrid_NeedDataSource);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);

        }

        //--------------------------------------------------------------------------------------------------------

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            //this.LoadMultiTrunks();
            this.multiTrunksRadGrid.Rebind();
        }

        //--------------------------------------------------------------------------------------------------------

        public void multiTrunksRadGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.LoadMultiTrunks();
        }

        //--------------------------------------------------------------------------------------------------------

        public void LoadMultiTrunks()
        {
            Facade.MultiTrunk facMultiTrunk = new Orchestrator.Facade.MultiTrunk();
            List<Entities.MultiTrunk> multiTrunks = facMultiTrunk.GetAll();

            this.multiTrunksRadGrid.DataSource = multiTrunks;
        }

        //--------------------------------------------------------------------------------------------------------

    }

    //--------------------------------------------------------------------------------------------------------

}
