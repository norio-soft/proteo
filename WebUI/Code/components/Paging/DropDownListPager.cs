using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace Orchestrator.WebUI.Pagers
{
	[ToolboxData("<{0}:DropDownListPager runat=server></{0}:DropDownListPager>"), Browsable(true)]
	public class DropDownListPager : PagerBase, IPostBackDataHandler
	{
		#region Variables Declartion
		private int  m_NewPageNumber = 0;
		#endregion
		
		#region Constructor

		public DropDownListPager()
		{
		}

		#endregion

		#region Control's State Management

		#endregion
		
		#region Combo Style & Layout properties

		#endregion

		#region Rendering methods
		
		protected override void Render(HtmlTextWriter writer)
		{
			base.AddAttributesToRender(writer);
			writer.AddAttribute(HtmlTextWriterAttribute.Name, this.UniqueID);
            //GREG:: Depreciated.
			//writer.AddAttribute(HtmlTextWriterAttribute.Onchange, Page.GetPostBackClientEvent(this,""));
			writer.AddAttribute(HtmlTextWriterAttribute.Size, "1");
			writer.RenderBeginTag(HtmlTextWriterTag.Select);
			for(int i=1; i<=PageCount; i++)
			{
				if(i == CurrentPageNumber)
				{
					writer.AddAttribute(HtmlTextWriterAttribute.Selected,"");

				}
				writer.AddAttribute(HtmlTextWriterAttribute.Value, i.ToString());
				writer.RenderBeginTag(HtmlTextWriterTag.Option);
				writer.Write(i);
				writer.RenderEndTag();
			}
			writer.RenderEndTag();
		}
		#endregion

		#region IPostBackDataHandler Members
		
		public void RaisePostDataChangedEvent()
		{
			OnPageNumberChanged(new PageNumberChangedEventArgs(CurrentPageNumber, m_NewPageNumber));
		}

		public bool LoadPostData(string postDataKey, System.Collections.Specialized.NameValueCollection postCollection)
		{
			if(postCollection[postDataKey] != CurrentPageNumber.ToString())
			{
				m_NewPageNumber = Convert.ToInt32(postCollection[postDataKey]);
				return true;
			}
			return false;
		}

		#endregion
	}
}