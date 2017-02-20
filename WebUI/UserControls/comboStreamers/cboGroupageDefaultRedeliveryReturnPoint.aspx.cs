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

using Orchestrator;
using Orchestrator.WebUI.Controls;

public partial class UserControls_comboStreamers_cboGroupageDefaultRedeliveryReturnPoint : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.cboDefaultAttemptedDeliveryReturnPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDefaultAttemptedDeliveryReturnPoint_ItemsRequested);

    }

    public void cboDefaultAttemptedDeliveryReturnPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
    {
        DataSet ds = null;
        Orchestrator.Facade.Point facPoint = new Orchestrator.Facade.Point();
        int noOfRowsToReturn = 20;

        // The search text entered by the user ("e.Text") can be split into two regions delimited by a backslash.
        // Any text to the left of the first backslash (or when there is no backslash) should be used to filter the organisation name.
        // Any text to the right of a backslash should be used to filter the point description.
        char filterChar = (char)92; // Backslash character "\"
        string[] filterString = e.Text.Split(filterChar.ToString().ToCharArray());

        if (string.IsNullOrEmpty(e.Text))
        {
            // Do not filter the point type for the time being - just display 'Any'.
            ds = facPoint.GetAllWithAddress(ePointType.Any, "", "", noOfRowsToReturn);
        }
        else if (filterString.Length == 1)
        {
            // Do not filter the point type for the time being - just display 'Any'.
            ds = facPoint.GetAllWithAddress(ePointType.Any, filterString[0], "", noOfRowsToReturn);
        }
        else if (filterString.Length > 1)
        {
            // Do not filter the point type for the time being - just display 'Any'.
            ds = facPoint.GetAllWithAddress(ePointType.Any, filterString[0], filterString[1], noOfRowsToReturn);
        }

        DataTable dt = ds.Tables[0];
        Telerik.Web.UI.RadComboBoxItem rcItem = null;

        foreach (DataRow row in dt.Rows)
        {
            rcItem = new Telerik.Web.UI.RadComboBoxItem();
            PointComboItem comboItem = new PointComboItem(row);

            rcItem.DataItem = comboItem;
            rcItem.Text = comboItem.SingleLineText;
            rcItem.Value = row["IdentityId"].ToString() + "," + row["PointId"];

            cboDefaultAttemptedDeliveryReturnPoint.Items.Add(rcItem);
        }
    }
}
