using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace Orchestrator.WebUI.Pagers
{
	#region PagingDirectionEnum Enum

	public enum FirstLastPagingDirectionEnum
	{
		First,
		Last
	}
	
	#endregion

	[ToolboxData("<{0}:FirstLastPager runat=server></{0}:FirstLastPager>"), Browsable(true)]
	public class FirstLastPager : PostBackEventHandlerPager
	{
		#region Variables Declartion
		
		// Local variables to hold the page numbers layout & style
		private string				m_Text;
		private string				m_Devider;
		private FirstLastPagingDirectionEnum m_Direction;

		private const string			  cDEF_TEXT	    = "&gt;&gt;";
		private const string			  cDEF_DEVIDER  = "";
		private const string			  cDEF_CSSCLASS = "";
		private const FirstLastPagingDirectionEnum cDEF_PAGINGDIRECTION = FirstLastPagingDirectionEnum.Last;
		#endregion
		
		#region Constructor
		public FirstLastPager()
		{
			m_Text      = cDEF_TEXT;
			m_Devider   = cDEF_DEVIDER;
			m_Direction = cDEF_PAGINGDIRECTION;
		}

		#endregion

		#region Control's State Management
		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState (savedState);

			m_Text          = (string)ViewState["Text"];
			m_Devider       = (string)ViewState["Devider"];
			m_Direction     = (FirstLastPagingDirectionEnum)ViewState["Direction"];
		}
		
		protected override object SaveViewState()
		{
			ViewState["Text"]			  = m_Text;
			ViewState["Devider"]		  = m_Devider;
			ViewState["Direction"]        = m_Direction;
			return base.SaveViewState ();
		}

		#endregion
		
		#region Back/Next Style & Layout properties
		
		/// <summary>
		/// Get/Set the html string to use as the control's text.
		/// </summary>
		[Category("First/Last"),
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
		[Category("First/Last"),
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
		[Category("First/Last"),
		DefaultValue(cDEF_PAGINGDIRECTION),
		Description("Get/Set the Direction of the paging")]
		public FirstLastPagingDirectionEnum PagingDirection
		{
			get{return m_Direction;}
			set{m_Direction = value;}
		}
		#endregion

		#region Rendering methods
		
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			int PageNumber = GetPageNumber();
			
			base.AddAttributesToRender (writer);
			
			if(CurrentPageNumber != PageNumber)
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
				if(m_Direction == FirstLastPagingDirectionEnum.First)
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

		private int GetPageNumber()
		{
			int NextPageNumber = (m_Direction == FirstLastPagingDirectionEnum.First)?1:PageCount;
			
			return NextPageNumber;
		}
		#endregion
	}
}