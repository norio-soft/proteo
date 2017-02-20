using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace Orchestrator.WebUI.Pagers
{
	public abstract class PagerBase : System.Web.UI.WebControls.WebControl
	{
		#region Variables Declartion
		
		// Local variable to hold the pager global properties
		private DataGrid         m_DataGrid;
		private int              m_PagesCount;
		private int              m_CurrentPage;
		private bool			 m_RecieveSync;
		private bool			 m_CauseSync;

		//Default Properties values
		private const int            cDEF_PAGECOUNT   = 3;
		private const int            cDEF_CURRENTPAGE = 1;
		private const bool			 cDEF_RECIEVE_SYNC = true;
		private const bool			 cDEF_CAUSE_SYNC   = true;

		// Holds all pager controls. Used to syncronize all pagers that the RecieveSync is set to true
		private static ArrayList m_Pagers = new ArrayList();
		#endregion
		
		#region Events Declaration
		
		/// <summary>
		/// This event is reaised when the page number is changed
		/// </summary>
		public event PageNumberChanged PageNumberChanged;

		#endregion
		
		#region Constructor

		public PagerBase()
		{
			// Initialize the properties values
			m_CurrentPage = cDEF_CURRENTPAGE;
			m_PagesCount  = cDEF_PAGECOUNT;
			m_DataGrid    = null;
			m_CauseSync   = cDEF_CAUSE_SYNC;
			m_RecieveSync = cDEF_RECIEVE_SYNC;
			
			// Add this control to the static arraylist member
			m_Pagers.Add(this);
		}

		#endregion

		#region Control's State Management
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState (savedState);

			m_PagesCount  = (int)ViewState["PagesCount"];
			m_CurrentPage = (int)ViewState["CurrentPage"];
			m_CauseSync   = (bool)ViewState["CauseSync"];
			m_RecieveSync = (bool)ViewState["RecieveSync"];
		}
		
		protected override object SaveViewState()
		{
			ViewState["PagesCount"]  = m_PagesCount;
			ViewState["CurrentPage"] = m_CurrentPage;
			ViewState["CauseSync"]   = m_CauseSync;
			ViewState["RecieveSync"] = m_RecieveSync;

			return base.SaveViewState ();
		}

		#endregion

		#region Global properties
		
		/// <summary>
		/// <p>
		///		Get/Set the DataGrid control that the pager should bind to.
		/// </p>
		/// <p>
		///		Setting this property, will override the PageCount property if set manually. For example
		///		if the PageCount was set by code to 5 and later in the code the DataGrid property
		///		was set then the Control will contain reference to the DataGrid and the number of pages
		///		of it (not necessarily 5).
		/// </p>
		/// </summary>
		[Category("DataBind"),
		 Description("Get/Set the DataGrid control that the pager should bind to.")
		]
		public System.Web.UI.WebControls.DataGrid DataGrid
		{
			get{return m_DataGrid;}
			set{m_DataGrid = value;}
		}

		/// <summary>
		/// <p>
		/// Get/Set the number of pages to display.
		/// </p>
		/// 
		/// <p>
		///		This property can be set only if the DataGrid proeprty is set to null, otherwise its value will not be changed.
		/// </p>
		/// </summary>
		[Category("Misc"),
		DefaultValue(cDEF_PAGECOUNT),
		Description("Get/Set the number of pages to display in the pager. This property has no meaning when binding the control to a DataGrid")
		]
		public int PageCount
		{
			get{return m_PagesCount;}
			set
			{
				m_PagesCount = value;
				if(m_CurrentPage > m_PagesCount)
					m_CurrentPage = m_PagesCount;
			}
		}
		
		/// <summary>
		/// <p>
		/// Get the current page number.
		/// </p>
		/// </summary>
		[Browsable(false),
		Category("Misc"),
		DefaultValue(cDEF_CURRENTPAGE),
		Description("Get/Set the current page number.")
		]
		public int CurrentPageNumber
		{
			get{return m_CurrentPage;}
			set
			{
				if(value > m_PagesCount || value <= 0)
				{
					throw(new ArgumentOutOfRangeException("The value of CurrentPageNumber should be in the range of 1-PageCount"));
				}
				m_CurrentPage = value;
			}
		}
		
		[Browsable(true),
		Category("Misc"),
		DefaultValue(cDEF_RECIEVE_SYNC),
		Description("Get/Set a flag indicates if the CurrentPageNumber property should be syncronized with other pagers on this page that has CauseSync set to true.")
		]
		public bool RecieveSync
		{
			get{return m_RecieveSync;}
			set{m_RecieveSync = value;}
		}

		[Browsable(true),
		Category("Misc"),
		DefaultValue(cDEF_CAUSE_SYNC),
		Description("Get/Set a flag indicates if this pager syncronizes other pagers controls that when the event of PageNumberChanged is raised.")
		]
		public bool CauseSync
		{
			get{return m_CauseSync;}
			set{m_CauseSync = value;}
		}
		#endregion

		#region Protected methods
		
		/// <summary>
		/// This methods return the post back event string that contains the 
		/// new page number
		/// </summary>
		/// <param name="PageNumber">The new page number</param>
		/// <returns>The client script post back string</returns>
		protected string GetPostBackHrefString(int PageNumber)
		{
			return Page.GetPostBackEventReference(this, PageNumber.ToString());
		}
		
		/// <summary>
		/// Handles the Page number changed event
		/// </summary>
		protected virtual void OnPageNumberChanged(PageNumberChangedEventArgs e)
		{
			// Get the new page number from the event arguments
			CurrentPageNumber = e.NewPageNumber;

			// If the control binded to grid, update the grid's page number
			if(DataGrid != null)
			{
				DataGrid.CurrentPageIndex = e.NewPageNumber-1;
				DataGrid.DataBind();
			}
			
			// If this control should syncronize the rest of the pagers then do it!
			if(m_CauseSync)
				Sync();
			
			// Call the eventhandler
			if(PageNumberChanged != null)
			{
				PageNumberChanged(this, e);
			}
		}

		#endregion
		
		#region Override methods
		protected override void OnPreRender(EventArgs e)
		{
			// If the control is binded to data grid
			if(m_DataGrid != null)
			{	
				// If the data grid not allow paging, hide the control
				if(!m_DataGrid.AllowPaging)
				{
					this.Visible = false;
				}

				// Set the page count and current page according the data grid settings
				PageCount     = m_DataGrid.PageCount;
				m_CurrentPage = m_DataGrid.CurrentPageIndex+1;
			}
		}

		#endregion

		#region Private methods

		/// <summary>
		/// This method syncronize all the pagers with the current page number of this control
		/// </summary>
		private void Sync()
		{
			foreach(PagerBase p in m_Pagers)
			{
				if(p.m_RecieveSync) p.m_CurrentPage = this.m_CurrentPage;
			}
		}
		#endregion
	}
}
