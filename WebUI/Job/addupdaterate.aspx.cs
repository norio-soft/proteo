using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator.WebUI.UserControls;

namespace Orchestrator.WebUI.Job
{
	/// <summary>
	/// Summary description for addupdaterate.
	/// </summary>
	public partial class addupdaterate : Orchestrator.Base.BasePage
	{
		#region Properties

		private bool IsUpdate
		{
			set
			{
				m_isUpdate = true;
				btnSubmit.Text = "Update.";
			}
		}

		#endregion

		#region Page Variables

		private int m_rateId;
		private bool m_isUpdate;

		#endregion

		#region Form Elements


		#endregion

		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditRate);

			m_rateId = Convert.ToInt32(Request.QueryString["rateId"]);
				
			if (m_rateId > 0)
			{
				IsUpdate = true;
				
				if (!IsPostBack)
					PopulateControls();
			}

		}


        
		#endregion

		#region DBCombo's Server Methods and Initialisation


        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

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
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboDeliveryPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboDeliveryPoint.Items.Clear();
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
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Deliver, 0, searchText);
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
                cboDeliveryPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboDelivery_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboDelivery.Items.Clear();

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
                cboDelivery.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboCollectionPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboCollectionPoint.Items.Clear();
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
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Collect, 0, searchText);
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
                cboCollectionPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
        
        void cboCollection_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboCollection.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

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
                cboCollection.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
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
			this.Init +=new EventHandler(addupdaterate_Init);
		}
		#endregion

		#region Event Handlers

		private void addupdaterate_Init(object sender, EventArgs e)
		{
            cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboCollection.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboCollection_ItemsRequested);
            this.cboCollectionPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboCollectionPoint_ItemsRequested);
            this.cboDelivery.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDelivery_ItemsRequested);
            this.cboDeliveryPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDeliveryPoint_ItemsRequested);

            btnSubmit.Click +=new EventHandler(btnSubmit_Click);

            cfvClient.ServerValidate += new ServerValidateEventHandler(cfvClient_ServerValidate);
            cfvCollection.ServerValidate += new ServerValidateEventHandler(cfvCollection_ServerValidate);
            cfvCollectionPoint.ServerValidate += new ServerValidateEventHandler(cfvCollectionPoint_ServerValidate);
            cfvDelivery.ServerValidate += new ServerValidateEventHandler(cfvDelivery_ServerValidate);
            cfvDeliveryPoint.ServerValidate += new ServerValidateEventHandler(cfvDeliveryPoint_ServerValidate);
		}

        void cfvClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDeliveryPoint.SelectedValue, 1, true);
        }

        void cfvDeliveryPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDeliveryPoint.SelectedValue, 1, true);
        }

        void cfvDelivery_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDelivery.SelectedValue, 1, true);
        }

        void cfvCollectionPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboCollectionPoint.SelectedValue, 1, true);
        }

        void cfvCollection_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboCollection.SelectedValue, 1, true);
        }
		
		private void btnSubmit_Click(object sender, EventArgs e)
		{
			if (Page.IsValid)
			{
				Facade.IJobRate facJobRate = new Facade.Job();
				Entities.JobRate jobRate = new Entities.JobRate();
				SetEntityProperties(jobRate);

				int rateId = facJobRate.Create(jobRate, ((Entities.CustomPrincipal) Page.User).UserName);
				if (rateId > 0)
				{	
					lblConfirmation.Text = "The rate was added successfully";
					lblConfirmation.Visible = true;
				}
			}
		}

		#endregion

		#region Methods

		private void SetEntityProperties(Entities.JobRate jobRate)
		{
            jobRate.IdentityId = Convert.ToInt32(cboClient.SelectedValue);

            jobRate.CollectionPointId = int.Parse(cboCollectionPoint.SelectedValue);
			jobRate.DeliveryPointId = int.Parse(cboDeliveryPoint.SelectedValue);
			
            jobRate.FullLoadRate = Decimal.Parse(txtFullLoadRate.Text, System.Globalization.NumberStyles.Currency);
			jobRate.MultiDropRate = Decimal.Parse(txtMultiDropRate.Text, System.Globalization.NumberStyles.Currency);
            jobRate.PartLoadRate = Decimal.Parse(txtPartLoadRate.Text, System.Globalization.NumberStyles.Currency);

            // The start of the start date
            jobRate.StartDate = dteStartDate.SelectedDate.Value;
            jobRate.StartDate = jobRate.StartDate.Subtract(jobRate.StartDate.TimeOfDay);

            if (!dteEndDate.SelectedDate.HasValue)
                // Insert null into EndDate in tblJobRate if date equal to
                // '01/01/1753', indicating rate never expires.
                jobRate.EndDate = (DateTime)SqlDateTime.MinValue;
            else
            {
                // The end of the end date
                jobRate.EndDate = dteEndDate.SelectedDate.Value;
                jobRate.EndDate = jobRate.EndDate.Subtract(jobRate.EndDate.TimeOfDay);
                jobRate.EndDate = jobRate.EndDate.AddDays(1);
                jobRate.EndDate = jobRate.EndDate.AddMinutes(-1);
            }
		}

		private void PopulateControls()
		{
			Facade.IJobRate facJobRate = new Facade.Job();
			Entities.JobRate jobRate = facJobRate.GetRateForRateId(m_rateId);

			if (jobRate != null)
			{

				// Delivery point
				Facade.IPoint facPoint = new Facade.Point();
				Entities.Point point = facPoint.GetPointForPointId(jobRate.DeliveryPointId);
				cboDeliveryPoint.Text = point.Description;
				cboDeliveryPoint.SelectedValue = jobRate.DeliveryPointId.ToString();
				cboDelivery.Text = point.OrganisationName;
				cboDelivery.SelectedValue = point.IdentityId.ToString();

				// Collection point
				point = facPoint.GetPointForPointId(jobRate.CollectionPointId);
				cboCollectionPoint.Text = point.Description;
				cboCollectionPoint.SelectedValue = jobRate.CollectionPointId.ToString();
				cboCollection.Text = point.OrganisationName;
				cboCollection.SelectedValue = point.IdentityId.ToString();

				txtFullLoadRate.Text = jobRate.FullLoadRate.ToString("C");
				txtMultiDropRate.Text = jobRate.MultiDropRate.ToString("C");
				dteStartDate.SelectedDate = jobRate.StartDate;
				dteStartDate.Text = jobRate.StartDate.ToString();
				if (!(jobRate.EndDate.ToString() == "01/01/1753 00:00:00"))
				{
					dteEndDate.SelectedDate = jobRate.EndDate;
					dteEndDate.Text = jobRate.EndDate.ToString();
				}
			}
		}
		#endregion
	}
}
