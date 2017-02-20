using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.UserControls.WebParts
{
    public class OrgUnitEditorPart : EditorPart
    {
        private Label OrgUnitsLabel;
        private DropDownList OrgUnitsDropDownList;

        public OrgUnitEditorPart(string WebPartId)
        {
            //To allow multiple instances of a web part, make the editor unique for the webpart
            this.ID    = WebPartId + "_OrgUnitEditorPart";
            this.Title = "Grouping";
            this.Description = "Defines the grouping level to use for displaying the data";
        }

        protected override void CreateChildControls()
        {
            OrgUnitsLabel = new Label();
            Controls.Add(OrgUnitsLabel);
            OrgUnitsLabel.Text = "Grouping";

            OrgUnitsDropDownList = new DropDownList();
            Controls.Add(OrgUnitsDropDownList);
            OrgUnitsDropDownList.DataValueField = "OrgUnitId";
            OrgUnitsDropDownList.DataTextField = "Name";
            OrgUnitsDropDownList.DataSource = EF.DataContext.Current.OrgUnits;
            OrgUnitsDropDownList.DataBind();
            OrgUnitsDropDownList.Items.Insert(0, new ListItem("ALL", string.Empty));
            
        }

        public override bool ApplyChanges()
        {
            EnsureChildControls();

            WebUI.WebParts.GenericSilverlightWithOrgUnit silverlightWebPart = GetSilverlightWebPart();

            if (silverlightWebPart != null)
            {
                if (OrgUnitsDropDownList.SelectedIndex <= 0)
                    silverlightWebPart.OrgUnitId = null;
                else
                    silverlightWebPart.OrgUnitId = int.Parse(OrgUnitsDropDownList.SelectedValue);
            }
            else
            {
                return false;
            }
            return true;
        }

        public override void SyncChanges()
        {
            EnsureChildControls();

            WebUI.WebParts.GenericSilverlightWithOrgUnit silverlightWebPart = GetSilverlightWebPart();

            if (silverlightWebPart != null)
            {
                int? orgUnitId = silverlightWebPart.OrgUnitId;

                if (orgUnitId.HasValue)
                {
                    string orgUnitIdString = orgUnitId.Value.ToString();
                    if (OrgUnitsDropDownList.Items.FindByValue(orgUnitIdString) != null)
                    {
                        OrgUnitsDropDownList.SelectedValue = orgUnitIdString;
                    }
                }
                else
                    OrgUnitsDropDownList.SelectedIndex = 0;
            }
        }

        private WebUI.WebParts.GenericSilverlightWithOrgUnit GetSilverlightWebPart()
        {
            WebUI.WebParts.GenericSilverlightWithOrgUnit silverlightWebPart = null;

            if (this.WebPartToEdit.Controls.Count > 0)
            {
                IWebPart webPart = (IWebPart)this.WebPartToEdit as IWebPart; 
                silverlightWebPart = this.WebPartToEdit.Controls[0] as WebUI.WebParts.GenericSilverlightWithOrgUnit;
            }

            return silverlightWebPart;
        }
}
}