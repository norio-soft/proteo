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

public partial class UserControls_comboStreamers_cboTown : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.cboTown.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboTown_ItemsRequested);
    }

    void cboTown_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
    {
        //cboTown.Items.Clear();

        Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
        DataSet ds = facRefData.GetTownForTownName(e.Text);

        int itemsPerRequest = 20;
        int itemOffset = e.NumberOfItems;
        int endOffset = itemOffset + itemsPerRequest;
        if (endOffset > ds.Tables[0].Rows.Count)
            endOffset = ds.Tables[0].Rows.Count;

        DataTable dt = ds.Tables[0];
        Telerik.Web.UI.RadComboBoxItem rcItem = null;
        for (int i = itemOffset; i < endOffset; i++)
        {
            rcItem = new Telerik.Web.UI.RadComboBoxItem();
            rcItem.Text = dt.Rows[i]["Description"].ToString();
            rcItem.Value = dt.Rows[i]["TownId"].ToString();
            cboTown.Items.Add(rcItem);
        }

        if (dt.Rows.Count > 0)
        {
            e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }
    }
}
