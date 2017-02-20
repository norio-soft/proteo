using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;


using P1TP.Components.Web.Validation;
using ComponentArt.Web.UI;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
	/// <summary>
	/// Summary description for driverCallIn.
	/// </summary>
	public partial class driverCallIn : Orchestrator.Base.BasePage
	{
		private const string C_JOB_VS = "C_JOB_VS";
		private const string C_PCV_VS = "C_PCV_VS";
		private const string C_POINT_ID_VS = "C_POINT_ID_VS";

		private enum eCreatingPointFor {PalletHandling, GoodsReturn, GoodsStore};

		#region Form Elements

		#region Record the driver's call-in

		protected	Panel				pnlResources;

		#endregion

		#region Goods
		
		#endregion

		#region Shortages

		#endregion

		#region PCVs

		#endregion

		#region Job Progress

		#endregion

		#endregion

		#region Page Variables

		#region Protected

		protected	Entities.Job			m_job				= null;
		protected	Entities.PCV			m_PCV				= null;
		protected	int	m_jobId		= 0;	// The id of the job we are currently manipulating.
		protected	int	m_pointId	= 0;	// The id of the point visited in the job we are currently manipulating.

		protected	int		m_PalletIdentityId	= 0;			// The id of the organisation we are going to send the pallets to.
		protected	string	m_PalletTown		= String.Empty;	// The description of the town we are going to send the pallets to.
		protected	int		m_PalletTownId		= 0;			// The id of the town we are going to send the pallets to.
		protected	int		m_PalletPointId		= 0;			// The id of the point we are going to send the pallets to.

		protected	int		m_GoodsStoreIdentityId	= 0;			// The id of the organisation we are going to store the goods at.
		protected	string	m_GoodsStoreTown		= String.Empty;	// The description of the town we are going to store the goods at.
		protected	int		m_GoodsStoreTownId		= 0;			// The id of the town we are going to store the goods at.
		protected	int		m_GoodsStorePointId		= 0;			// The id of the point we are going to store the goods at.

		protected	int		m_GoodsReturnIdentityId	= 0;			// The id of the organisation we are going to return the goods to.
		protected	string	m_GoodsReturnTown		= String.Empty;	// The description of the town we are going to return the goods to.
		protected	int		m_GoodsReturnTownId		= 0;			// The id of the town we are going to return the goods to.
		protected	int		m_GoodsReturnPointId	= 0;			// The id of the point we are going to return the goods to.

		#endregion

		#region Private

		private		int						m_instructionId		= 0;		// The id of the instruction that is being called-in.	
		
		#endregion

		#endregion
        		
        protected void Page_Load(object sender, System.EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);

            Response.Redirect("DriverCallIn/CallIn.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&csid=" + this.CookieSessionID);
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
	}
}
