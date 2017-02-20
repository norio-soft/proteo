using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;
using Orchestrator.WebUI.Controls;

namespace Orchestrator.WebUI.UserControls.comboStreamers
{
    public partial class cboMultiTrunkPoint : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rcbMultiTrunk.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(rcbMultiTrunk_ItemsRequested);
        }

        public void rcbMultiTrunk_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            List<Entities.MultiTrunk> multiTrunks = null;
            Orchestrator.Facade.MultiTrunk facMultiTrunk = new Orchestrator.Facade.MultiTrunk();
            int noOfRowsToReturn = 20;

            multiTrunks = facMultiTrunk.GetForDescriptionFiltered(e.Text, true, noOfRowsToReturn);

            Telerik.Web.UI.RadComboBoxItem rcItem = null;

            foreach (Entities.MultiTrunk mt in multiTrunks)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();

                rcItem.DataItem = mt;
                rcItem.Text = mt.Description;
                rcItem.Value = mt.MultiTrunkId.ToString();

                rcbMultiTrunk.Items.Add(rcItem);
            }
        }
    }
}
