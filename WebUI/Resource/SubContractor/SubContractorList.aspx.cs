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

using P1TP.Components.Web.UI;

namespace Orchestrator.WebUI.Resource.SubContractor
{
	public partial class SubContractorList : Orchestrator.Base.BasePage
	{
		#region Constants

		private const string C_SORT_COLUMN_VS = "C_SORT_COLUMN_VS";
		private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";

		#endregion

		#region Form Elements


		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

			if (!IsPostBack)
			{
				PopulateSubContractors();
			}
		}

		private void SubContractorList_Init(object sender, EventArgs e)
		{
			btnAddSubContractor.Click += new EventHandler(btnAddSubContractor_Click);

			dgSubContractors.PageIndexChanged += new DataGridPageChangedEventHandler(dgSubContractors_PageIndexChanged);
			dgSubContractors.ItemCommand += new DataGridCommandEventHandler(dgSubContractors_ItemCommand);
			dgSubContractors.ItemDataBound += new DataGridItemEventHandler(dgSubContractors_ItemDataBound);
            this.btnExport.Click += new EventHandler(btnExport_Click);
		}

        void btnExport_Click(object sender, EventArgs e)
        {
            Session["__ExportDS"] = GetSubContractors().Tables[0];
            Response.Redirect("../../reports/csvexport.aspx?filename=subcontractors");
        }

		#endregion

		#region Populate

		private DataSet GetSubContractors()
		{
			Facade.IOrganisation facOrganisation = new Facade.Organisation();
			DataSet dsSubContractors = facOrganisation.GetAllForType((int) eOrganisationType.SubContractor);

			return dsSubContractors;
		}

		private void PopulateSubContractors()
		{
			dgSubContractors.DataSource = GetSubContractors();
			dgSubContractors.DataBind();
		}

		#endregion

		#region Event Handlers

		#region DataGrid Event Handlers

		private void dgSubContractors_ItemCommand(object source, DataGridCommandEventArgs e)
		{
			switch (e.CommandName.ToLower())
			{
				case "select":
					Response.Redirect("../../Organisation/addupdateorganisation.aspx?type=" + ((int) eOrganisationType.SubContractor).ToString() + "&IdentityId=" + ((HtmlInputHidden) e.Item.FindControl("hidIdentityId")).Value);
					break;
			}
		}

		private void dgSubContractors_ItemDataBound(object sender, DataGridItemEventArgs e)
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

		private void dgSubContractors_PageIndexChanged(object source, DataGridPageChangedEventArgs e)
		{
			dgSubContractors.CurrentPageIndex = e.NewPageIndex;
			PopulateSubContractors();
		}

		#endregion

		private void btnAddSubContractor_Click(object sender, EventArgs e)
		{
			// Redirect the user to the create organiastion page - specifying the organisation type as SubContractor
			Response.Redirect("../../Organisation/addupdateorganisation.aspx?type=" + ((int) eOrganisationType.SubContractor).ToString());
		}

		#endregion

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			PageNumbersPager1.DataGrid = dgSubContractors;
			NextBackPager1.DataGrid = dgSubContractors;
			NextBackPager2.DataGrid = dgSubContractors;
			Firstlastpager1.DataGrid = dgSubContractors;
			Firstlastpager2.DataGrid = dgSubContractors;
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init += new EventHandler(SubContractorList_Init);
		}
		#endregion
	}
}
