using System;

namespace Orchestrator.WebUI.Pagers
{
	#region PageNumberChanged delegate & PageNumberChangedEventArgs Class 
	
	/// <summary>
	/// This class provides the event argument for a page number change event.
	/// </summary>
	public class PageNumberChangedEventArgs : EventArgs
	{
		#region Variables Declaration

		int m_OldPageNumber;
		int m_NewPageNumber;
		
		#endregion
		
		#region Constructor

		public PageNumberChangedEventArgs(int OldPageNumber, int NewPageNumber)
		{
			m_OldPageNumber = OldPageNumber;
			m_NewPageNumber = NewPageNumber;
		}
		
		#endregion
		
		#region Properties
		
		/// <summary>
		/// Get the page number before the user changed the page
		/// </summary>
		public int OldPageNumber
		{
			get{return m_OldPageNumber;}
		}
		
		/// <summary>
		/// Get the new page number
		/// </summary>
		public int NewPageNumber
		{
			get{return m_NewPageNumber;}
		}

		#endregion
	}
	
	public delegate void PageNumberChanged(object sender, PageNumberChangedEventArgs e);
	#endregion
}