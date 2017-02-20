using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Text;
using P1TP.Components.Web.UI;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Integration
{
	/// <summary>
	/// Summary description for IntegratePoints.
	/// </summary>
	public partial class IntegratePoints : Orchestrator.Base.BasePage
	{
		private int integrationPointId;
		protected int m_clientId = 0;
		protected string m_clientName = "";
		protected int m_pointTownId = 0;
		protected string m_pointTownName = "";
		protected int m_pointId = 0;
		protected string m_pointName = "";

        #region Page 

        protected void Page_Load(object sender, System.EventArgs e)
		{
            btnRemoveIntegration.Attributes.Add("onclick", "return confirm('This is an Irreversible Action, are you sure you wish to proceed?')");
            if (!this.IsPostBack)
            {
                this.pointsToIntegrateGrid.Rebind();
                if (this.pointsToIntegrateGrid.Items.Count > 0)
                {
                    pnlIntegratePoint.Visible = true;
                    this.pointsToIntegrateGrid.Items[0].Selected = true;
                    this.pointsToIntegrateGrid_SelectedIndexChanged(this.pointsToIntegrateGrid, null);
                    this.pointsToIntegrateGrid.Items[0].Selected = true;
                }
            }

			if (ViewState["IntegrationPointId"] != null)
				integrationPointId = Convert.ToInt32(ViewState["IntegrationPointId"]);
		}

        override protected void OnInit(EventArgs e)
        {
            
            base.OnInit(e);
            this.btnRemoveIntegration.Click += new EventHandler(btnRemoveIntegration_Click);
            this.btnIntegratePoint.Click += new EventHandler(btnIntegratePoint_Click);
            this.RemoveIntegrationGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(RemoveIntegrationGrid_NeedDataSource);

            this.pointsToIntegrateGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(pointsToIntegrateGrid_NeedDataSource);
            this.pointsToIntegrateGrid.SelectedIndexChanged += new EventHandler(pointsToIntegrateGrid_SelectedIndexChanged);

            this.newPoint.SelectedPointChanged += new SelectedPointChangedEventHandler(newPoint_SelectedPointChanged);
            this.newPoint.NewPointSelected += new EventHandler(newPoint_NewPointSelected);
            
            this.existingPoint.SelectedPointChanged += new SelectedPointChangedEventHandler(existingPoint_SelectedPointChanged);
        }

       

       

        #endregion

        #region Page Methods

        private int GetPointId()
        {
            int pointId = -1;

            if (newPoint.SelectedPoint != null && newPoint.SelectedPoint.PointId > 0)
                pointId = newPoint.SelectedPoint.PointId;

            if (existingPoint.SelectedPoint != null && existingPoint.SelectedPoint.PointId > 0)
                pointId = existingPoint.SelectedPoint.PointId;

            return pointId;
        }

        private void UpdatePointControl(string controlName, string fieldName)
        {
            if (this.pointsToIntegrateGrid.SelectedItems.Count > 0)
            {
                GridDataItem dataItem = this.pointsToIntegrateGrid.SelectedItems[0] as GridDataItem;

                if (dataItem != null)
                {
                    TextBox PointTextbox = (TextBox)this.newPoint.FindControl(controlName);
                    if (!String.IsNullOrEmpty(dataItem[fieldName].Text) && dataItem[fieldName].Text != "&nbsp;")
                        PointTextbox.Text = dataItem[fieldName].Text.Trim();
                    else
                        PointTextbox.Text = String.Empty;
                }
            }
        }

        #endregion

        #region Events

        #region Grid

        public void pointsToIntegrateGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {

            Facade.Point facPoint = new Facade.Point();
            pointsToIntegrateGrid.DataSource = facPoint.GetPointsRequiringIntegration();
        }

        
        public void RemoveIntegrationGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            string postcode;
            GridDataItem dataItem = null;
            
            if (this.pointsToIntegrateGrid.SelectedItems.Count > 0)
                dataItem = this.pointsToIntegrateGrid.SelectedItems[0] as GridDataItem;

            if (dataItem != null && GetDataItemValue(dataItem, "PostCode") != "")
            {
                ClientScript.RegisterStartupScript(GetType(), "Error", "document.getElementById('RemoveIntegrationID').style.visibility = 'visible';", true);
                postcode = GetDataItemValue(dataItem, "PostCode");
                Facade.ImportMessage facImportMessage = new Facade.ImportMessage();
                RemoveIntegrationGrid.DataSource = facImportMessage.GetForIntegrationMapping(postcode);
            }
            else
            {
                ClientScript.RegisterStartupScript(GetType(), "Error", "document.getElementById('RemoveIntegrationID').style.visibility = 'hidden';", true); 
                postcode = null;
            }
        }

        public void pointsToIntegrateGrid_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridDataItem dataItem = this.pointsToIntegrateGrid.SelectedItems[0] as GridDataItem;

            if (dataItem != null)
            {
                string externalPointRef = GetDataItemValue(dataItem, "ExternalPointRef");
                string addressLine1 = GetDataItemValue(dataItem, "AddressLine1");
                string addressLine2 = GetDataItemValue(dataItem, "AddressLine2");
                string addressLine3 = GetDataItemValue(dataItem, "AddressLine3");
                string postTown = GetDataItemValue(dataItem, "PostTown");
                string county = GetDataItemValue(dataItem, "County");
                string postcode = GetDataItemValue(dataItem, "PostCode");
                
                string dropDownText = string.Concat(
                    externalPointRef.Substring(0, Math.Min(externalPointRef.Length, 5)).TrimEnd(),
                    "\\",
                    string.IsNullOrEmpty(postTown) ? postcode : postTown);

                var addressFields = new[] { addressLine1, addressLine2, addressLine3, postTown, county, postcode };
                string integratingPoint = "<b>{0}</b><br>Supplied as:<br>{1}";

                lblIntegratingPoint.Text = string.Format(integratingPoint, externalPointRef, string.Join(",<br />", addressFields));

                integrationPointId = int.Parse(dataItem["IntegrationPointId"].Text);
                ViewState["IntegrationPointId"] = integrationPointId;

                pnlIntegratePoint.Visible = true;
                m_clientId = 0;
                m_pointTownId = 0;
                m_pointId = 0;

                this.UpdatePointControl("txtAddressLine1", "AddressLine1");
                this.UpdatePointControl("txtAddressLine2", "AddressLine2");
                this.UpdatePointControl("txtAddressLine3", "AddressLine3");
                this.UpdatePointControl("txtPostTown", "PostTown");
                this.UpdatePointControl("txtCounty", "County");
                this.UpdatePointControl("txtPostCode", "PostCode");

                RadComboBox existingPoints = this.existingPoint.FindControl("cboPoint") as RadComboBox;
                
                if (existingPoints != null)
                    existingPoints.Text = dropDownText;

                RemoveIntegrationGrid.Rebind();
            }
        }

        private string GetDataItemValue(GridDataItem dataItem, string columnName)
        {
            string retVal = dataItem[columnName].Text.Trim();
            if (retVal == "&nbsp;")
                retVal = string.Empty;
            return retVal;
        }

        private List<int> GetImportMessageIDsForRemoval()
        {
            List<int> importMessageIDs = new List<int>();

            foreach (GridDataItem i in RemoveIntegrationGrid.MasterTableView.Items)
	        {
                if (i.OwnerTableView.Name == RemoveIntegrationGrid.MasterTableView.Name)
                {
                    CheckBox chk = (CheckBox)i["CheckboxSelectColumn"].Controls[0];
                    if (chk.Checked == true)
                    {
                        importMessageIDs.Add(Convert.ToInt32(GetDataItemValue(i, "ImportMessageID")));
                    }
                }
	        }

            return importMessageIDs;
        }

        #endregion

        #region Button
        private void btnRemoveIntegration_Click(object sender, EventArgs e)
        {

            
                GridDataItem dataItem = this.pointsToIntegrateGrid.SelectedItems[0] as GridDataItem;
                int IntegrationPointId = Convert.ToInt32(GetDataItemValue(dataItem, "IntegrationPointId"));

                if (IntegrationPointId > 0)
                {
                    string ImportMessageIDCSV;
                    List<int> importMessageIDs = new List<int>();
                    importMessageIDs = GetImportMessageIDsForRemoval();
                    if (importMessageIDs.Count > 0)
                    {
                        ImportMessageIDCSV = String.Join(",", importMessageIDs);
                    }
                    else
                    {
                        ImportMessageIDCSV = "";
                    }
                    bool success = false;
                    Facade.IPoint facPoint = new Facade.Point();
                    success = facPoint.DeletePointForIntegration(IntegrationPointId, ImportMessageIDCSV);

                    if (success == true)
                    {
                        Response.Redirect(Request.RawUrl);
                        
                    }
                    else
                    {
                        ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Unable to delete the Point, please try again');", true);
                    }
                }
            
          

            
        }

        private void btnIntegratePoint_Click(object sender, EventArgs e)
        {
            int pointId = this.GetPointId();

            if (pointId > 0)
            {
                bool success = false;
                this.lblNewPointValidation.Visible = false;
                this.lblNewPointValidation.Text = String.Empty;
                this.lblExistingPointValidation.Visible = false;
                this.lblExistingPointValidation.Text = String.Empty;

                string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                Facade.IPoint facPoint = new Facade.Point();
                success = facPoint.IntegratePoint(integrationPointId, pointId, userId);

                if (success)
                {
                    pnlIntegratePoint.Visible = false;
                    ViewState["IntegrationPointId"] = 0;

                    existingPoint.Reset();
                    newPoint.Reset();
                    //this.newPoint.EditMode = true;

                    this.pointsToIntegrateGrid.Rebind();
                    if (this.pointsToIntegrateGrid.Items.Count > 0)
                    {
                        pnlIntegratePoint.Visible = true;
                        this.pointsToIntegrateGrid.Items[0].Selected = true;
                        this.pointsToIntegrateGrid_SelectedIndexChanged(this.pointsToIntegrateGrid, null);
                        this.pointsToIntegrateGrid.Items[0].Selected = true;
                    }
                }
            }
        }

        #endregion

        #region Combobox

        public void existingPoint_SelectedPointChanged(object sender, SelectedPointChangedEventArgs e)
        {
            this.lblExistingPointValidation.Visible = false;
        }

        public void newPoint_SelectedPointChanged(object sender, SelectedPointChangedEventArgs e)
        {
            this.lblNewPointValidation.Visible = false;
        }

        public void newPoint_NewPointSelected(object sender, EventArgs e)
        {
            this.UpdatePointControl("txtAddressLine1", "AddressLine1");
            this.UpdatePointControl("txtAddressLine2", "AddressLine2");
            this.UpdatePointControl("txtAddressLine3", "AddressLine3");
            this.UpdatePointControl("txtPostTown", "PostTown");
            this.UpdatePointControl("txtCounty", "County");
            this.UpdatePointControl("txtPostCode", "PostCode");
        }

        #endregion

        #endregion
    }
}
