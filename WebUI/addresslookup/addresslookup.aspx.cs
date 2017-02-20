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

namespace Orchestrator.WebUI.addresslookup
{
	/// <summary>
	/// Summary description for addresslookup.
	/// </summary>
	public partial class addresslookup : Orchestrator.Base.BasePage
	{
	
		protected string	m_townName;
		protected int		m_townId;
		protected string	m_countyName;
		protected bool		m_oneRow;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.TakeCallIn, eSystemPortion.AddEditPoints, eSystemPortion.AddEditOrganisations);

			if (!IsPostBack)
			{
				m_townName = Request["townName"];
				DisplayTowns();
			}
		}

		private void DisplayTowns()
		{
			DataSet dsTowns;
			Facade.IReferenceData facRefData = new Facade.ReferenceData();
			dsTowns = facRefData.GetTownForTownName(m_townName);

			if (dsTowns.Tables[0].Rows.Count > 1)
			{
				lstTowns.DataSource = dsTowns.Tables[0];
				lstTowns.DataBind();
				lstTowns.Visible = true;
			}
			else
			{
				m_townId = Convert.ToInt32(dsTowns.Tables[0].Rows[0]["TownId"]);
				DisplayCounty(m_townId);
				txtTown.Text = dsTowns.Tables[0].Rows[0]["Description"].ToString();
				m_oneRow = true;
				m_townName = txtTown.Text;
			}
			
		}

		private void DisplayCounty(int townId)
		{
			
			Facade.IReferenceData facRefData = new Facade.ReferenceData();
			DataSet dsCounty = facRefData.GetCountyForTownId(townId);

			txtCounty.Text = dsCounty.Tables[0].Rows[0]["Description"].ToString();
			lblTownId.Text = townId.ToString();
			m_townId = townId;
			m_countyName = txtCounty.Text;
		}

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

		}
		#endregion

		protected void lstTowns_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			txtTown.Text = lstTowns.SelectedItem.Text;
			m_townName = txtTown.Text;
			DisplayCounty(Convert.ToInt32(lstTowns.SelectedItem.Value));
		}
	}
}
