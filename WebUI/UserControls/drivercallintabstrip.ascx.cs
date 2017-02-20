namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
    using System.Linq;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

    using Telerik.Web.UI;

	/// <summary>
	///		Summary description for DriverCallInTabStrip.
	/// </summary>
	public partial class DriverCallInTabStrip : System.Web.UI.UserControl
	{
        private static string vs_selectedTabId = "vs_selectedTabId";

        private		int			m_jobId = 0;
		private		int			m_instructionId = 0;
        private string[]        m_tabTexts = new String[] { "Call In", "Add Refusal", "Add Shorts/Overs", "Returned/Refused Goods", "PCVs", "Progress" };

		protected int? SelectedTab
		{
            get { return ViewState[vs_selectedTabId] == null ? 0 : (int)ViewState[vs_selectedTabId]; }
            set {
                if (value != null && rtsCallIn.Tabs.FindTabByText(m_tabTexts[value.Value]) != null)
                    rtsCallIn.Tabs.FindTabByText(m_tabTexts[value.Value]).Selected = true;
             
                ViewState[vs_selectedTabId] = value; 
            }
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{


			if (!IsPostBack)
                Populate();
        }

        public void Populate()
        {
            GetSessionIDFromQueryString();

            rtsCallIn.Tabs.Clear();

			m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
			m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);

			// Get the instruction to work with.
			if (m_instructionId == 0)
				using (Facade.IInstruction facInstruction = new Facade.Instruction())
				{
					Entities.Instruction instruction = facInstruction.GetNextInstruction(m_jobId);
					if (instruction != null)
						m_instructionId = instruction.InstructionID;
				}

            rtsCallIn.Tabs.Add(CreateTab(m_tabTexts[0], "CallIn", 0));
            
            var instructionType = (from i in EF.DataContext.Current.InstructionSet
                     where i.InstructionId == m_instructionId
                     select i.InstructionType.InstructionTypeId).FirstOrDefault();

            if (instructionType == 2)
            {
                rtsCallIn.Tabs.Add(CreateTab(m_tabTexts[1], "Refusal", 1));
                rtsCallIn.Tabs.Add(CreateTab(m_tabTexts[2], "Shortage", 2));
            }

            rtsCallIn.Tabs.Add(CreateTab(m_tabTexts[3], "tabReturns", 3));

            if (!((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
            {
                rtsCallIn.Tabs.Add(CreateTab(m_tabTexts[4], "tabPCVS", 4));
                rtsCallIn.Tabs.Add(CreateTab(m_tabTexts[5], "tabProgress", 5));
            }

            int selectedTabID = 0;
            
            int.TryParse(Request.QueryString["t"], out selectedTabID);
            if (selectedTabID > -1 && rtsCallIn.Tabs.FindTabByText(m_tabTexts[selectedTabID]) != null)
                rtsCallIn.Tabs.FindTabByText(m_tabTexts[selectedTabID]).Selected = true;
    	}

        private RadTab CreateTab(string displayValue, string pageName, int tabID)
        {
            RadTab newTab = new RadTab();
            newTab.Attributes.Add("TargetPage", Page.ResolveUrl("~/Traffic/JobManagement/DriverCallIn/" + pageName + ".aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&t=" + tabID + "&csid=" + this.CookieSessionID));
            newTab.Text = displayValue;

            return newTab;
        }

        #region Cookie Handling

        private string _cookieSessionID = string.Empty;
        public string CookieSessionID
        {
            get
            {
                if (string.IsNullOrEmpty(_cookieSessionID))
                {
                    _cookieSessionID = Utilities.GetRandomString(6);
                }

                return _cookieSessionID;
            }
            set
            {
                _cookieSessionID = value;
            }
        }

        private void GetSessionIDFromQueryString()
        {
            if (!String.IsNullOrEmpty(Request.QueryString["csid"]))
            {
                _cookieSessionID = Request.QueryString["csid"];
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

            rtsCallIn.TabClick += new RadTabStripEventHandler(rtsCallIn_TabClick);
		}

        void rtsCallIn_TabClick(object sender, RadTabStripEventArgs e)
        {
            SelectedTab = e.Tab.Index;
            Response.Redirect(e.Tab.Attributes["TargetPage"]);
        }
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}
		#endregion
	}
}
