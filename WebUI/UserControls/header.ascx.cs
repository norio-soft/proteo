namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	using System.Xml;
	using System.Xml.XPath;
	using System.Xml.Xsl;
    using System.Configuration;

	/// <summary>
	///		Summary description for header.
	/// </summary>
	public partial class header : System.Web.UI.UserControl
	{
        public static string defaultPageTitle = "Orchestrator" + (Orchestrator.Globals.Configuration.InstallationCompanyName.Length > 0 ? " - " + Orchestrator.Globals.Configuration.InstallationCompanyName : "");
        public string pageTitle = defaultPageTitle;
		public		string title			= string.Empty;
		public		string subTitle			= string.Empty;
		

		public		int	  helpId			= 0;

		protected	PlaceHolder ContextMenu;
		protected	string		m_onLoadScript		= string.Empty;
		protected	bool		m_showLeftMenu		= true;
		protected	string		m_leftMenuDisplay	=string.Empty;
		protected	bool		m_isWizard			= false;
		protected	bool		m_isReport			= false;
        protected bool m_showMenu = true;
        protected bool m_showSearch = true;

		private		string		m_XMLPath = string.Empty;

		private		int			m_jobId				= 0;

		Entities.CustomPrincipal user;

		protected void Page_Load(object sender, System.EventArgs e)
		{
			// Put user code to initialize the page here
            if (Request.QueryString["jobId"] != null)
                int.TryParse(Request.QueryString["jobId"], out m_jobId);

            user = (Entities.CustomPrincipal)Page.User;
			if (Request["wiz"]!=null && Request["wiz"].ToString()=="true")
			{
				m_isWizard = true;
                pnlWizardHeading.Visible = true;
                pnl1.Visible = false;
				ShowLeftMenu = false;
				RadMenu1.Visible = false;
			}

            if (Request.QueryString["wizard"] != null)
            {
                pnlDialogHeading.Visible = false;
            }

			LoadMenu();

		}

		protected void Page_Init(object sender, EventArgs e)
		{
           
		}

		private void SetContextMenu()
		{
			string XSLPath = Server.MapPath(Page.ResolveUrl("~/xsl/contextmenu.xsl"));
			string contextMenu = string.Empty;
			if (Cache[m_XMLPath] == null)
			{
				System.Web.Caching.CacheDependency dep = new System.Web.Caching.CacheDependency(m_XMLPath); 
				contextMenu = TransformXML(m_XMLPath,XSLPath);
				Cache.Insert(m_XMLPath, contextMenu,dep);
			}
			else
			{
				contextMenu = (string)Cache[m_XMLPath];
			}
			ContextMenu.Controls.Add(new System.Web.UI.LiteralControl(contextMenu));
			ContextMenu.Visible = true;

		}

		/// <summary>
		/// Depending on the user's role we can/will use different menus
		/// Also we will load the users own personalised Launch pad.
		/// </summary>
		private void LoadMenu()
        {
            //Get the theme name if it isn't Orchestrator so that it can be used to select the menu
            string themeName = Page.Theme;
            if (themeName.Equals("Orchestrator", StringComparison.CurrentCultureIgnoreCase))
                themeName = string.Empty;

            if (user.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                if (!Page.IsPostBack)
                    RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Client.xml", themeName));

                WhiteLabelHeader();
            }
            else
                if (!Page.IsPostBack)
                    RadMenu1.LoadContentFile(string.Format("~/UserControls/menu{0}Default.xml", themeName));
        }

        private void WhiteLabelHeader()
        {
            HtmlLink css = new HtmlLink();
            css.Href = "/style/whitelabel.css";
            css.Attributes["rel"] = "stylesheet";
            css.Attributes["type"] = "text/css";
            css.Attributes["media"] = "all";
            hdr.Controls.Add(css); ;

            //// set the Header Background
            //tdHeaderBackground.Style.Add("background-image", string.Format("url({0}", ConfigurationManager.AppSettings["WhiteLabel.CustomerUser.HeaderBackground"]));
            //tdHeaderBackground.Style.Add("background-repeat", "repeat-x");
            //tdMenuBar1.Attributes.Remove("background");
            //tdMenuBar1.Style.Add("background-color", "#484747");
            //tdMenuBar2.Attributes.Remove("background");
            //tdMenuBar2.Style.Add("background-color", "#484747");
            //tdMenuBar3.Attributes.Remove("background");
            //tdMenuBar3.Style.Add("background-color", "#484747");
            //tdMenuBar4.Attributes.Remove("background");
            //tdMenuBar4.Style.Add("background-color", "#484747");
            //tdMenuBar5.Attributes.Remove("background");
            //tdMenuBar5.Style.Add("background-color", "#484747");

            //// set the Left Logo
            //imgLeftLogo.Src = ConfigurationManager.AppSettings["WhiteLabel.CustomerUser.LeftLogo"];


            //litWhiteLabelmenu.Text = "<style type=\"text/css\">.TopMenuItem {color: #FFF;height:32px;background-image: url('/images/newMasterPage/nav-seperatorbg.jpg');background-position: center right;background-repeat: no-repeat;} .TopMenuItemHover, .TopMenuItemActive {background-image: url('/images/newMasterPage/nav-seperatorbg.jpg');background-position: center right;background-repeat: no-repeat;background-color: #484747;color: #8cc7f1;height:32px;} .MenuGroup {background-color: #484747;border-right: 1px solid #000;border-bottom: 1px solid #000;border-left: 1px solid #000;border-top: 1px solid #484747;} .MenuItem {background-color: #484747;border: 1px solid #484747;} .MenuItem nobr {color: #FFF;} .MenuItemHover{background-color: #c2c2c2;background-image: url('/images/newMasterPage/menu-hoverbg.jpg');background-repeat:repeat-x;border: 1px solid #a5cae4;} .MenuItemHover nobr {color: #000;} .MenuBreak {background-color: #3e3e3e;border-bottom: 1px dotted #000;}</style>";
            //// Hide the Seach Panel
            //pnlSearch.Visible = false;

            //// Show the powered by logo
            //imgPoweredByLogo.Visible = true;
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
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Init += new EventHandler(Page_Init);
		}
		#endregion

		public string PageTitle
		{
			get { return pageTitle; }
			set { pageTitle = value + " - " + defaultPageTitle; }
		}

		public string Title
		{
			get{return title;}
			set 
			{
				title = value;
			}
		}

		public string SubTitle
		{
			get {return subTitle;}
			set {subTitle = value;}
		}

		public int HelpId
		{
			get {return helpId;}
			set {helpId = value;}
		}

		public string XMLPath
		{
			get {return m_XMLPath;}
			set {m_XMLPath = Server.MapPath(value);}
		}

		public string OnLoadScript
		{
			get {return m_onLoadScript;}
			set {m_onLoadScript = value;}
		}

		public bool ShowLeftMenu
		{
			get {return m_showLeftMenu;}
			set {m_showLeftMenu = value;}
		}

		public string LeftMenuDisplay
		{
			get 
			{
				if (ShowLeftMenu)
				{
					return string.Empty;
				}
				else
					return "display:none;";
			}
		}

		public string HasMenu
		{
			get 
			{
				if (m_isWizard || m_isReport)
					return "none";
				else
					return String.Empty;
			}
		}

		public string IsWizard
		{
			get 
			{
				if (m_isWizard)
					return "none";
				else
					return String.Empty;
			}
		}

		public string IsReport
		{
			get { return m_isReport.ToString(); }
			set
			{
				m_isReport = value.ToLower() == "true";
			}
		}

		public string PageWidth
		{
			get 
			{
				// 2005-03-08 : Stephen Newman
				// Removed so as to allow for the pop up job management
				//if (m_isWizard && !m_isReport)
				//	return "460";
				//else
					return "100%";
			}
		}

        public bool ShowMenu
        {
            get { return m_showMenu; }
            set { m_showMenu = value; }
        }

        public bool ShowSearch
        {
            get { return m_showSearch; }
            set { m_showSearch = value; }
        }

		public int JobId
		{
			get { return m_jobId; }
			set { m_jobId = value; }
		}

		private string TransformXML(string sXML , string XSLPath ) 
		{
			XmlDataDocument oXML = new XmlDataDocument();
			//XPathDocument oXSL = new XPathDocument(XSLPath);
            
            XslCompiledTransform oTransform = new XslCompiledTransform();
			XsltArgumentList xslArg = new XsltArgumentList();
			string sMessage;
			//oXML.Load(new XmlTextReader(sXML, XmlNodeType.Document, null));
			oXML.Load(sXML);
            XPathNavigator nav = oXML.CreateNavigator();
			oTransform.Load(XSLPath);
            
			try
			{
				System.IO.MemoryStream oText = new System.IO.MemoryStream();
				XmlTextWriter xmlWrite = new XmlTextWriter(oText, System.Text.Encoding.UTF8);
				oTransform.Transform(nav, xmlWrite);
				xmlWrite.Flush();
				oText.Flush();
				oText.Position = 1;
				System.IO.StreamReader  sr = new System.IO.StreamReader(oText);
				sMessage = sr.ReadToEnd();
				oText.Close();
				return sMessage;
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}

        
	}

}
