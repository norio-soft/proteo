using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;
using Orchestrator;
using Orchestrator.WebUI.Controls;

public partial class UserControls_comboStreamers_cboPoint : Orchestrator.Base.BasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.cboPoint.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboPoint_ItemsRequested);
    }

    public void cboPoint_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
    {
        DataSet ds = null;
        Orchestrator.Facade.Point facPoint = new Orchestrator.Facade.Point();
        int noOfRowsToReturn = 20;

        // The search text entered by the user ("e.Text") can be split into two regions delimited by a backslash.
        // Any text to the left of the first backslash (or when there is no backslash) should be used to filter the organisation name.
        // Any text to the right of a backslash should be used to filter the point description.
        char filterChar = (char)92; // Backslash character "\"
        string[] filterString = e.Text.Split(filterChar.ToString().ToCharArray());

        bool isClient = false;
        Orchestrator.Entities.CustomPrincipal cp = Page.User as Orchestrator.Entities.CustomPrincipal;
        isClient = cp.IsInRole(((int)eUserRole.ClientUser).ToString());
        int clientIdentityId = 0;

        if (isClient)
        {
            // Get the clients related organisation identityId.

            //Get the user's Identity row
            Orchestrator.Facade.IUser facUser = new Orchestrator.Facade.User();
            SqlDataReader reader = facUser.GetRelatedIdentity(((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);
            reader.Read();

            //Is the User a Client User
            if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
            {
                clientIdentityId = (int)reader["RelatedIdentityId"];
            }
            reader.Close();

            // If clientIdentityId = 0 then throw error?

            if (string.IsNullOrEmpty(e.Text))
                ds = facPoint.GetAllWithAddressForClientFiltered(clientIdentityId, ePointType.Any, "", noOfRowsToReturn);
            else
                ds = facPoint.GetAllWithAddressForClientFiltered(clientIdentityId, ePointType.Any, e.Text, noOfRowsToReturn);
        }
        else
        {
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
        }

        DataTable dt = ds.Tables[0];
        RadComboBoxItem rcItem = null;

        foreach (DataRow row in dt.Rows)
        {
            rcItem = new RadComboBoxItem();
            PointComboItem comboItem = new PointComboItem(row);

            rcItem.DataItem = comboItem;
            rcItem.Text = comboItem.SingleLineText;
            rcItem.Value = row["IdentityId"].ToString() + "," + row["PointId"];

            cboPoint.Items.Add(rcItem);
        }
    }
}


