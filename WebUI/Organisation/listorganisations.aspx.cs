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
using Orchestrator.WebUI.UserControls;

using Orchestrator.Globals;

using System.Text;

using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Organisation
{
	/// <summary>
	/// Summary description for listorganisations.
	/// </summary>
	public partial class listorganisations : Orchestrator.Base.BasePage
	{
		#region Form Elements




		#endregion

		#region Page_Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
            if (!IsPostBack)
			    PopulateClients();
		}

		#endregion

		#region Populate DataGrid

		private void PopulateClients()
		{
			using (Facade.Organisation facOrganisation = new Facade.Organisation())
				
			if (Request.QueryString["incomplete"] != null)
			{
				pageHeader.Title = "Incomplete Clients";
				pageHeader.SubTitle = "A list of clients for which no information has been entered is listed below. Please enter this information as soon as possible.";
				dgClients.DataSource = facOrganisation.GetIncomplete();
			}
			else
				dgClients.DataSource = facOrganisation.GetAllForType((int) eOrganisationType.Client);

			dgClients.DataBind();
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
            this.btnExport.Click += new EventHandler(btnExport_Click);
		}

        void btnExport_Click(object sender, EventArgs e)
        {
            DataSet ds = null;
            using (Facade.Organisation facOrganisation = new Facade.Organisation())

                if (Request.QueryString["incomplete"] != null)
                {
                    pageHeader.Title = "Incomplete Clients";
                    pageHeader.SubTitle = "A list of clients for which no information has been entered is listed below. Please enter this information as soon as possible.";
                    ds = facOrganisation.GetIncomplete();
                }
                else
                    ds = facOrganisation.GetAllForType((int)eOrganisationType.Client);
            Session["__ExportDS"] = ds.Tables[0];
            Response.Redirect("../reports/csvexport.aspx?filename=AllClients");
        }

        
		#endregion


		private void dgClients_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "select":
					Response.Redirect("addupdateorganisation.aspx?identityId=" + ((HtmlInputHidden) e.Item.FindControl("hidIdentityId")).Value);
					break;
			}
		}

		private void dgClients_ItemDataBound(object sender, DataGridItemEventArgs e)
		{
			if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
			{
				DataRowView rv = (DataRowView) e.Item.DataItem;

				if (rv["AddressLine1"] != DBNull.Value && ((string) rv["AddressLine1"]).Length > 0)
				{
					StringBuilder sb = new StringBuilder();

					sb.Append((string) rv["AddressLine1"]);
					sb.Append("<br>");
				
					if (rv["AddressLine2"] != DBNull.Value && ((string) rv["AddressLine2"]).Length > 0)
					{
						sb.Append((string) rv["AddressLine2"]);
						sb.Append("<br>");
					}

					if (rv["AddressLine3"] != DBNull.Value && ((string) rv["AddressLine3"]).Length > 0)
					{
						sb.Append((string) rv["AddressLine3"]);
						sb.Append("<br>");
					}

					if (rv["PostTown"] != DBNull.Value && ((string) rv["PostTown"]).Length > 0)
					{
						sb.Append((string) rv["PostTown"]);
						sb.Append("<br>");
					}

					if (rv["County"] != DBNull.Value && ((string) rv["County"]).Length > 0)
					{
						sb.Append((string) rv["County"]);
						sb.Append("<br>");
					}

					if (rv["PostCode"] != DBNull.Value && ((string) rv["PostCode"]).Length > 0)
					{
						sb.Append((string) rv["PostCode"]);
						sb.Append("<br>");
					}

					((Label) e.Item.FindControl("lblAddress")).Text = sb.ToString();
				}
			}
		}

		
	}
}
