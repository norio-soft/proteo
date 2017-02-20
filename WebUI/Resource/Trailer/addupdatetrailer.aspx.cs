using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.Globals;



namespace Orchestrator.WebUI.Resource.Trailer
{
	/// <summary>
	/// Summary description for addupdatetrailer.
	/// </summary>
	public partial class addupdatetrailer : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_RESOURCE_ID = "ResourceId";
		
		#endregion

		#region Form Elements
		

		#endregion

		#region Page Elements

		private bool						m_isUpdate = false;
		private int							m_resourceId = 0;
		private Entities.Trailer			trailer;
		//private	eResourceType				m_resourceType;
		//private	DateTime					m_startDate;

		protected int m_organisationId = 0;
		protected string m_startTown = String.Empty;
		protected int m_startTownId = 0;
		protected int m_pointId = 0;
		#endregion 

		#region Page Init/Load
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditResource);
			btnAdd.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditResource);

            if (Request.QueryString["rcbID"] == null)
            {
                m_resourceId = GetResourceId();

                if (m_resourceId > 0)
                    m_isUpdate = true;

                if (!IsPostBack)
                {
                    PopulateStaticControls();

                    if (m_isUpdate)
                        LoadTrailer();
                }

                infringementDisplay.Visible = false;
            }

            this.thirdPartyIntegrationIDRangeValidator.MaximumValue = Int64.MaxValue.ToString();
        }

		private void addupdatetrailer_Init(object sender, EventArgs e)
		{
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			this.cboPoint.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboPoint_ItemsRequested);
            this.cboOrganisation.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboOrganisation_ItemsRequested);
            btnCancel.Click += new EventHandler(btnCancel_Click);
            cfvPoint.ServerValidate += new ServerValidateEventHandler(cfvPoint_ServerValidate);
		}

        void cfvPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboPoint.SelectedValue, 1, true);
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
        }
		
		#endregion 

		private int GetResourceId()
		{
			int retVal = Convert.ToInt32(Request.QueryString["resourceId"]);
			if (retVal > 0)
			{
				// Store in ViewState
				ViewState[C_RESOURCE_ID] = retVal;
			}
			else
			{
				// Attempt from ViewState
				retVal = Convert.ToInt32(ViewState[C_RESOURCE_ID]);
			}

			return retVal;
		}

		#region Populate Static Controls
		///	<summary> 
		/// Populate Static Controls
		///	</summary>
		private void PopulateStaticControls()
		{
			// Load the Classes Dropdown
			Facade.ITrailer facResource = new Facade.Resource();
			DataSet dsTrailerTypes = facResource.GetAllTrailerTypes();
			cboTrailerType.DataSource = dsTrailerTypes;
			cboTrailerType.DataTextField = "Description";
			cboTrailerType.DataValueField = "TrailerTypeId";
			cboTrailerType.DataBind(); 
			cboTrailerType.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "")); 

			// Load the Manufacturers dropdown
			DataSet dsTrailerManufacturers = facResource.GetAllTrailerManufacturers();
			cboTrailerManufacturer.DataSource = dsTrailerManufacturers;
			cboTrailerManufacturer.DataTextField = "Description";
			cboTrailerManufacturer.DataValueField = "TrailerManufacturerId";
			cboTrailerManufacturer.DataBind();
			cboTrailerManufacturer.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "")); 

			// Load the Trailer Description dropdown
            cboTrailerDescription.DataSource = facResource.GetAllTrailerDescriptions();
            cboTrailerDescription.DataTextField = "Description";
            cboTrailerDescription.DataValueField = "TrailerDescriptionId";
			cboTrailerDescription.DataBind();
			cboTrailerDescription.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

			Facade.IControlArea facControlArea = new Facade.Traffic();
			cboControlArea.DataSource = facControlArea.GetAll();
			cboControlArea.DataTextField = "Description";
			cboControlArea.DataValueField = "ControlAreaId";
			cboControlArea.DataBind();
			cboControlArea.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

			Facade.ITrafficArea facTrafficArea = (Facade.ITrafficArea) facControlArea;
			cboTrafficArea.DataSource = facTrafficArea.GetAll();
			cboTrafficArea.DataTextField = "Description";
			cboTrafficArea.DataValueField = "TrafficAreaId";
			cboTrafficArea.DataBind();
			cboTrafficArea.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));
		}


		#endregion
		
		#region Load/Add/Update/Populate Trailer
		///	<summary> 
		/// Load Trailer
		///	</summary>
		private void LoadTrailer()
		{
			if (ViewState["trailer"]==null)
			{
				Facade.ITrailer facTrailer = new Facade.Resource();
				trailer = facTrailer.GetForTrailerId(m_resourceId, true);
				ViewState["trailer"] = trailer;
			}
			else
				trailer = (Entities.Trailer)ViewState["trailer"];

			if (trailer != null)
			{
				txtTrailerRef.Text = trailer.TrailerRef;
			
				cboTrailerType.Items.FindByValue(trailer.TrailerTypeId.ToString()).Selected = true;
				cboTrailerManufacturer.Items.FindByValue(trailer.TrailerManufacturerId.ToString()).Selected = true;

                Facade.IPoint facPoint = new Facade.Point();
				Entities.Point point = facPoint.GetPointForPointId(trailer.HomePointId);

				cboOrganisation.Text = point.OrganisationName;
				cboOrganisation.SelectedValue = point.IdentityId.ToString();

				m_organisationId = point.IdentityId;

				m_startTown = point.PostTown.TownName;
				m_startTownId = point.PostTown.TownId;

                if (trailer.VehicleResourceID.HasValue)
                    lblVehicle.Text = trailer.VehicleResource;

                txtGPSUnitID.Text = trailer.GPSUnitID;
				cboPoint.Text = point.Description;
				cboPoint.SelectedValue = point.PointId.ToString();
				m_pointId = point.PointId;

                cboTrailerDescription.SelectedIndex = trailer.TrailerDescriptionId +1;

				if (trailer.ResourceStatus== eResourceStatus.Deleted)
					chkDelete.Checked = true;

				Entities.ControlArea ca = null;
				Entities.TrafficArea ta = null;

				using (Facade.IResource facResource = new Facade.Resource())
					facResource.GetControllerForResourceId(trailer.ResourceId, ref ca, ref ta);

				if (ca != null && ta != null)
				{
					cboControlArea.ClearSelection();
					cboControlArea.Items.FindByValue(ca.ControlAreaId.ToString()).Selected = true;
					cboTrafficArea.ClearSelection();
					cboTrafficArea.Items.FindByValue(ta.TrafficAreaId.ToString()).Selected = true;
				}

				chkDelete.Visible = true;
				pnlTrailerDeleted.Visible = true;

                txtThirdPartyIntegrationID.Text = (trailer.ThirdPartyIntegrationID.HasValue) ? trailer.ThirdPartyIntegrationID.ToString() : string.Empty;

			}

			btnAdd.Text = "Update";
		}
		
		///	<summary> 
		/// Populate Trailer
		///	</summary>
		private void populateTrailer()
		{
	
			if (ViewState["trailer"] ==null)
			{
				trailer = new Entities.Trailer();
				if (m_resourceId > 0)
					trailer.ResourceId = m_resourceId;
			}
			else
				trailer = (Entities.Trailer)ViewState["trailer"];
			
			trailer.TrailerRef = txtTrailerRef.Text;
			trailer.TrailerTypeId = Convert.ToInt32(cboTrailerType.Items[cboTrailerType.SelectedIndex].Value);
			trailer.TrailerManufacturerId = Convert.ToInt32(cboTrailerManufacturer.Items[cboTrailerManufacturer.SelectedIndex].Value);	
			trailer.TrailerDescriptionId = cboTrailerDescription.SelectedIndex;
			trailer.HomePointId = Convert.ToInt32(cboPoint.SelectedValue);

			trailer.ResourceType = eResourceType.Trailer;
            trailer.GPSUnitID = txtGPSUnitID.Text;
			if (chkDelete.Checked)
			{
				trailer.ResourceStatus= eResourceStatus.Deleted;
			}
			else
			{
				trailer.ResourceStatus = eResourceStatus.Active;
			}

            trailer.ThirdPartyIntegrationID = (string.IsNullOrEmpty(txtThirdPartyIntegrationID.Text)) ? (long?)null : (long?)long.Parse(txtThirdPartyIntegrationID.Text);

		}

		
		///	<summary> 
		/// Update Trailer
		///	</summary>
		private bool UpdateTrailer()
		{
			Facade.ITrailer facResource = new Facade.Resource();
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;
			
			Entities.FacadeResult result = facResource.Update(trailer, userName);

            if (result.Success)
		    {
                if (Configuration.IsMwfCustomer && !string.IsNullOrEmpty(Configuration.BlueSphereCustomerId))
                {
                    try
                    {
                        MobileWorkerFlow.MWFServicesCommunication.Client.UpdateTrailerInBlueSphere(trailer, trailer.ResourceId.ToString());
                    }
                    catch (Exception ex)
                    {
                        // MM (2nd April 2012): Not ideal but solution for now, if fail to get through to bluesphere then show error to user
                        lblConfirmation.Text = "There was an error updating the trailer: " + ex.Message;
                        lblConfirmation.Visible = true;
                        lblConfirmation.ForeColor = Color.Red;
                        result = new Entities.FacadeResult(false);
                        return false;
                    }
                }
		    }
		    else
		    {
		        infringementDisplay.Infringements = result.Infringements;
		        infringementDisplay.DisplayInfringments();
		    }

		    return result.Success;
		}


		///	<summary> 
		/// Add Trailer
		///	</summary>
		private bool AddTrailer()
		{
			Facade.ITrailer facResource = new Facade.Resource();
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Entities.FacadeResult result = facResource.Create(trailer, userName);

            if (result.Success)
			{
				// Store the new id in the viewstate
				trailer.ResourceId = result.ObjectId;
				ViewState[C_RESOURCE_ID] = result.ObjectId;

                if (Configuration.IsMwfCustomer && !string.IsNullOrEmpty(Configuration.BlueSphereCustomerId))
			    {
			        try
			        {
			            MobileWorkerFlow.MWFServicesCommunication.Client.AddNewTrailerToBlueSphere(trailer,
			                                                                                       result.ObjectId.ToString());
			        }
			        catch (Exception ex)
			        {
			            // MM (2nd April 2012): Not ideal but solution for now, if fail to get through to bluesphere then show error to user
			            lblConfirmation.Text = "There was an error adding the trailer: " + ex.Message;
			            lblConfirmation.Visible = true;
			            lblConfirmation.ForeColor = Color.Red;
			            result = new Entities.FacadeResult(false);

			        }
			    }
			}
			else
			{
				infringementDisplay.Infringements = result.Infringements;
				infringementDisplay.DisplayInfringments();
			}

			return result.Success;
		}


		#endregion 

		#region Events & Methods
		///	<summary> 
		/// Button Add_Click
		///	</summary>
		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid)
			{
				bool retVal = false;
                lblConfirmation.Text = "";
				lblConfirmation.Visible = true;

				populateTrailer();

				if (m_isUpdate) 
					retVal = UpdateTrailer();
				else
					retVal = AddTrailer();

				if (retVal)
				{
					if (m_isUpdate) 
						lblConfirmation.Text = "The trailer has been updated successfully.";
					else
					{
						lblConfirmation.Text = "The trailer has been added successfully.";
						btnAdd.Text = "Update";
					}

					if (cboControlArea.SelectedValue != String.Empty && cboTrafficArea.SelectedValue != String.Empty)
					{
						using (Facade.IResource facResource = new Facade.Resource())
							facResource.AssignToArea(Convert.ToInt32(cboControlArea.SelectedValue), Convert.ToInt32(cboTrafficArea.SelectedValue), trailer.ResourceId, "Trailer has been updated", ((Entities.CustomPrincipal) Page.User).UserName);
					}

                    // refresh the page we came from if it is expecting a call back
                    this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack", "__dialogCallBack(window, 'refresh');", true);

                    this.ReturnValue = lblConfirmation.Text;
            			
                    this.Close();
                }					
				else
				{
                    if (string.IsNullOrWhiteSpace(lblConfirmation.Text))
						lblConfirmation.Text = m_isUpdate ? "The trailer has not been updated successfully." : "The trailer has not been added successfully.";

                    this.ReturnValue = lblConfirmation.Text;
				}
			}
		}

		#endregion
		
		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init +=new EventHandler(addupdatetrailer_Init);
		}
		#endregion

		#region DBCombo's Server Methods and Initialisation		
	
        void cboOrganisation_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboOrganisation.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);

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
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboOrganisation.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
        void cboPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboPoint.Items.Clear();
            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString() != "")
            {
                string[] values = e.Context["FilterString"].ToString().Split(';');
                try { identityId = int.Parse(values[0]); }
                catch { }
                if (values.Length > 1 && values[1] != "false" && !string.IsNullOrEmpty(values[1]))
                {
                    searchText = values[1];
                }
                else if (!string.IsNullOrEmpty(e.Text))
                    searchText = e.Text;
            }
            else
                searchText = e.Context["FilterString"].ToString();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Any, 0, searchText);
            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

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
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                cboPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

		#endregion
	}
}
