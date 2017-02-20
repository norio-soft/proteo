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

namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for EnterDriverStartTimes.
	/// </summary>
	public partial class EnterDriverStartTimes : Orchestrator.Base.BasePage
	{
		#region Page Load/Init/Error

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Plan, eSystemPortion.AddEditResource);

			if (!IsPostBack)
			{
				if (Request.QueryString["resourceId"] != null && Request.QueryString["date"] != null  && Request.QueryString["date"] != "undefined" )
				{
					int resourceId = Convert.ToInt32(Request.QueryString["resourceId"]);
					Facade.IDriver facDriver = new Facade.Resource();
					cboDriver.Text = facDriver.GetDriverForResourceId(resourceId).Individual.FullName;
					cboDriver.SelectedValue = resourceId.ToString();
					
					string date = Request.QueryString["date"];
                    rdiDate.SelectedDate = new DateTime(Convert.ToInt32(date.Substring(6, 4)), Convert.ToInt32(date.Substring(3, 2)), Convert.ToInt32(date.Substring(0, 2)));

					// Get the start time for the driver for this date and populate the time control.
                    DataSet dsStartTime = facDriver.GetStartTimesForDateAndDriver(resourceId, rdiDate.SelectedDate.Value);
					if (dsStartTime.Tables[0].Rows.Count > 0)
					{
                        rdiStartTime.SelectedDate = (DateTime)dsStartTime.Tables[0].Rows[0]["StartDateTime"];
						txtNotes.Text = dsStartTime.Tables[0].Rows[0]["Notes"].ToString ();
					}
				}
				else
                    rdiDate.SelectedDate = DateTime.UtcNow;
			}
		}

        protected override void  OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnSet.Click += new EventHandler(btnSet_Click);
            this.btnClose.Click += new EventHandler(btnClose_Click);

            this.cboDriver.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDriver_ItemsRequested);

            this.cfvDriver.ServerValidate += new ServerValidateEventHandler(cfvDriver_ServerValidate);
        }

		#endregion

		#region DBCombo's Server Methods and Initialisation

        void cboDriver_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver, false);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboDriver.Items.Clear();
            cboDriver.DataSource = boundResults;
            cboDriver.DataBind();

            if (boundResults.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

		#endregion

        #region Validation

        void cfvDriver_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboDriver.SelectedValue, 1, true);
        }

        #endregion

        #region Event Handlers

        void btnClose_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Resource/Driver/ViewDriverStartTimes.aspx");           
        }

		private void btnSet_Click(object sender, EventArgs e)
		{
			int resourceId = Convert.ToInt32(cboDriver.SelectedValue);
            DateTime startDateTime = Convert.ToDateTime(rdiDate.SelectedDate.Value.ToString("dd/MM/yy") + " " + rdiStartTime.SelectedDate.Value.ToString("T"));
			string notes = txtNotes.Text;

			Facade.IDriver facDriver = new Facade.Resource();

            int iRetVal = facDriver.CreateStartDateTime(resourceId, startDateTime, notes);
				
            if (iRetVal > 0)
            {
                this.ReturnValue = string.Format("{0} {1}", rdiDate.SelectedDate.Value.ToString("dd/MM"), startDateTime.ToString("HH:mm"));
                
                this.Close();

                string str = "<script>opener.reload();</script>";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "Script", str, false);
            }
		}

		#endregion
	}
}
