using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace Orchestrator.WebUI.Pagers
{
	[ToolboxData("<{0}:PageNumbersPager runat=server></{0}:PageNumbersPager>"), Browsable(true)]
	public class PageNumbersPager : PostBackEventHandlerPager
	{
		#region Variables Declartion

		// Local variables to hold the page numbers layout & style
		private int            m_Columns;
		private int            m_MaxAtOnce;
		private string         m_PageNumbersCSSClass;
		private string         m_PageNumbersCurrentPageCSSClass;


		private const int cDEF_COLUMNS      = 0;
		private const int cDEF_MAXATONCE    = 0;
		private const string cDEF_PAGENUMBERS_CSSCLASS  = "";
		private const string cDEF_PAGENUMBERS_CURRENTPAGE_CSSCLASS  = "";
		
	#endregion
		
		#region Constructor
		
		public PageNumbersPager()
		{
			m_Columns						 = cDEF_COLUMNS;
			m_PageNumbersCSSClass			 = cDEF_PAGENUMBERS_CSSCLASS;
			m_PageNumbersCurrentPageCSSClass = cDEF_PAGENUMBERS_CURRENTPAGE_CSSCLASS;
			m_MaxAtOnce						 = cDEF_MAXATONCE;
		}

		#endregion

		#region Control's State Management

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState (savedState);

			m_Columns						  = (int)ViewState["PageNumbersColumns"];
			m_MaxAtOnce						  = (int)ViewState["PageNumbersMaxAtOnce"];
			m_PageNumbersCSSClass			  = (string)ViewState["PageNumbersCSSClass"];
			m_PageNumbersCurrentPageCSSClass  = (string)ViewState["PageNumbersCurrentPageCSSClass"];
		}
		
		protected override object SaveViewState()
		{
			ViewState["PageNumbersColumns"]				 = m_Columns;
			ViewState["PageNumbersMaxAtOnce"]			 = m_MaxAtOnce;
			ViewState["PageNumbersCSSClass"]			 = m_PageNumbersCSSClass;
			ViewState["PageNumbersCurrentPageCSSClass"]  = m_PageNumbersCurrentPageCSSClass;

			return base.SaveViewState ();
		}

#endregion
		
		#region Page Numbers Style & Layout properties
		
		[Category("Page Numbers"),
		DefaultValue(cDEF_COLUMNS),
		Description("Get/Set the numbers of columns to use in each row(0 - unlimited).")]
		public int PageNumbersColumns
		{
			get{return m_Columns;}
			set{m_Columns = value;}
		}
		
		/*
		[Category("Page Numbers"),
		DefaultValue(cDEF_MAXATONCE),
		Description("Get/Set the maximum page numbers to display at once.")]
		public int PageNumbersMaxAtOnce
		{
			get{return m_MaxAtOnce;}
			set{m_MaxAtOnce = value;}
		}
		*/
		[Category("Page Numbers"),
		DefaultValue(cDEF_PAGENUMBERS_CSSCLASS),
		Description("Get/Set the CSS class of the page numbers")]
		public string PageNumbersCSSClass
		{
			get{return m_PageNumbersCSSClass;}
			set{m_PageNumbersCSSClass = value;}
		}

		[Category("Page Numbers"),
		DefaultValue(cDEF_PAGENUMBERS_CURRENTPAGE_CSSCLASS),
		Description("Get/Set the CSS class of the select page number")]
		public string PageNumbersCurrentPageCSSClass
		{
			get{return m_PageNumbersCurrentPageCSSClass;}
			set{m_PageNumbersCurrentPageCSSClass = value;}
		}
		#endregion
	
		#region Rendering methods
		
		public override void RenderBeginTag(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.RenderBeginTag(HtmlTextWriterTag.Table);
		}
		
		public override void RenderEndTag(HtmlTextWriter writer)
		{
			writer.RenderEndTag();
		}
		
		protected override void RenderContents(HtmlTextWriter writer)
		{
			int Cols = 0;

			writer.RenderBeginTag(HtmlTextWriterTag.Tr);
			for(int i=1; i<=this.PageCount; i++)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				
				if(i == CurrentPageNumber && m_PageNumbersCurrentPageCSSClass.Length > 0)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, m_PageNumbersCurrentPageCSSClass);
				}
				else if(m_PageNumbersCSSClass.Length > 0)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Class, m_PageNumbersCSSClass);
				}
				writer.AddAttribute(HtmlTextWriterAttribute.Href, String.Format("javascript:{0}", GetPostBackHrefString(i)));
				writer.RenderBeginTag(HtmlTextWriterTag.A);
				writer.Write(i);
				writer.RenderEndTag();
				writer.RenderEndTag();

				if(++Cols == m_Columns)
				{
					Cols = 0;
					writer.RenderEndTag();
					writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				}
			}
			writer.RenderEndTag();
		}
		#endregion
	}
}