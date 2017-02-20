using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace Orchestrator.WebUI.Pagers
{
	#region PagingDirectionEnum Enum

	public enum PagingDirectionEnum
	{
		Next,
		Back
	}
	
	#endregion

	[ToolboxData("<{0}:NextBackPager runat=server></{0}:NextBackPager>"), Browsable(true)]
	public class NextBackPager : PostBackEventHandlerPager
	{
		#region Variables Declartion
		
		// Local variables to hold the page numbers layout & style
		private string				m_Text;
		private string				m_Devider;
		private PagingDirectionEnum m_Direction;
		private int					m_StepSize;
		private bool                m_Cyclic;

		private const string			  cDEF_TEXT	    = "Next&gt;";
		private const string			  cDEF_DEVIDER  = "";
		private const string			  cDEF_CSSCLASS = "";
		private const PagingDirectionEnum cDEF_PAGINGDIRECTION = PagingDirectionEnum.Next;
		private const int				  cDEF_STEPSIZE = 1;
		private const bool                cDEF_CYCLIC   = true;
		#endregion
		
		#region Constructor
		public NextBackPager()
		{
			m_Text      = cDEF_TEXT;
			m_Devider   = cDEF_DEVIDER;
			m_Direction = cDEF_PAGINGDIRECTION;
			m_StepSize  = cDEF_STEPSIZE;
			m_Cyclic    = cDEF_CYCLIC;
		}

		#endregion

		#region Control's State Management
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState (savedState);

			m_Text          = (string)ViewState["Text"];
			m_Devider       = (string)ViewState["Devider"];
			m_Direction     = (PagingDirectionEnum)ViewState["Direction"];
			m_StepSize      = (int)ViewState["StepSize"];
			m_Cyclic      = (bool)ViewState["Cyclic"];
		}
		
		protected override object SaveViewState()
		{
			ViewState["Text"]			  = m_Text;
			ViewState["Devider"]		  = m_Devider;
			ViewState["Direction"]        = m_Direction;
			ViewState["StepSize"]         = m_StepSize;
			ViewState["Cyclic"]           = m_Cyclic;
			return base.SaveViewState ();
		}

		#endregion
		
		#region Back/Next Style & Layout properties
		
		/// <summary>
		/// Get/Set the html string to use as the control's text.
		/// </summary>
		[Category("Next/Back"),
		DefaultValue(cDEF_TEXT),
		Description("Get/Set the html string to use as the control's text.")]
		public string Text
		{
			get{return m_Text;}
			set{m_Text = value;}
		}
		
		/// <summary>
		/// Get/Set the devider between the next/back captions. if the KeepTogether property is set to false then this devider will be added at the right to the back caption and at the left of the next caption. The devider is treated as html string to allow using images instead of text
		/// </summary>
		[Category("Next/Back"),
		DefaultValue(cDEF_DEVIDER),
		Description("Get/Set the html string to use as the devider between the text and the previous/next control.")]
		public string Devider
		{
			get{return m_Devider;}
			set{m_Devider = value;}
		}
		
		/// <summary>
		/// Get/Set the Direction of the paging
		/// </summary>
		[Category("Next/Back"),
		DefaultValue(cDEF_PAGINGDIRECTION),
		Description("Get/Set the Direction of the paging")]
		public PagingDirectionEnum PagingDirection
		{
			get{return m_Direction;}
			set{m_Direction = value;}
		}

		/// <summary>
		/// Get/Set the size of the paging (i.e. 3 - 3 pages forward/backward)
		/// </summary>
		[Category("Next/Back"),
		DefaultValue(cDEF_STEPSIZE),
		Description("Get/Set the paging size(i.e. 1 Page, 2 pages,... on each click")]
		public int PagingStep
		{
			get{return m_StepSize;}
			set{m_StepSize = value;}
		}

		/// <summary>
		/// Get/Set the flag that indicates if the paging is cyclic
		/// </summary>
		[Category("Next/Back"),
		DefaultValue(cDEF_CYCLIC),
		Description("Get/Set the flag that indicates if the paging is cyclic.")]
		public bool IsCyclic
		{
			get{return m_Cyclic;}
			set{m_Cyclic = value;}
		}
		#endregion

		#region Rendering methods
		
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			int PageNumber = GetNextPageNumber();
			
			base.AddAttributesToRender (writer);
			
			if(this.Enabled)
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Href,String.Format("javascript:{0}",base.GetPostBackHrefString(PageNumber)));
			}
			else
			{
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled,"1");
			}
		}

		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			this.AddAttributesToRender(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.A);
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			if(Devider.Length > 0)
			{
				if(m_Direction == PagingDirectionEnum.Back)
				{
					writer.Write(Text);
					writer.Write(Devider);
				}
				else
				{
					writer.Write(Devider);
					writer.Write(Text);
				}
			}
			else
			{
				writer.Write(Text);
			}
		}
		
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			writer.RenderEndTag();
		}

		private int GetNextPageNumber()
		{
			int NextPageNumber = (m_Direction == PagingDirectionEnum.Next)?CurrentPageNumber + m_StepSize:CurrentPageNumber - m_StepSize;
			this.Enabled = true;

			if(NextPageNumber < 1)
			{
				if(m_Cyclic)
				{
					NextPageNumber = PageCount + NextPageNumber;
				}
				else
				{
					this.Enabled = false;
				}
			}
			else if(NextPageNumber > PageCount)
			{
				if(m_Cyclic)
				{
					NextPageNumber -= PageCount;
				}
				else
				{
					this.Enabled = false;
				}
			}
			
			return NextPageNumber;
		}
		#endregion
	}
}