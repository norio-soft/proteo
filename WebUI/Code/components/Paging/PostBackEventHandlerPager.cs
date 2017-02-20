using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;

namespace Orchestrator.WebUI.Pagers
{
	public abstract class PostBackEventHandlerPager : PagerBase, IPostBackEventHandler
	{
		#region Process Postback events
		public virtual void RaisePostBackEvent(string eventArgument)
		{
			if(Convert.ToInt32(eventArgument) != CurrentPageNumber)
				OnPageNumberChanged(new PageNumberChangedEventArgs(CurrentPageNumber, Convert.ToInt32(eventArgument)));
		}
		#endregion
	}
}
