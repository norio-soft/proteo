//====================================================================
// This file is generated as part of Web project conversion.
// The extra class 'GoodsFilterOptions' in the code behind file in 'goodsrefused\ListGoodsRefused.aspx.cs' is moved to this file.
//====================================================================


using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.WebUI.Security;
using Orchestrator.Globals;
using P1TP.Components.Web.Validation;


namespace Orchestrator.WebUI.GoodRefused
 {


	[Serializable]
	public class GoodsFilterOptions
	{
		private int			m_clientId;
		private string		m_statusIdCSV;
		private string		m_locationIdCSV;
		private DateTime	m_dateFrom;
		private DateTime	m_dateTo;

		public int ClientId
		{
			get { return m_clientId; }
			set { m_clientId = value; }
		}

		public string StatusIdCSV
		{
			get { return m_statusIdCSV; }
			set { m_statusIdCSV = value; }
		}

		public string LocationIdCSV
		{
			get { return m_locationIdCSV; }
			set { m_locationIdCSV = value; }
		}

		public DateTime DateFrom
		{
			get { return m_dateFrom; }
			set { m_dateFrom = value; }
		}

		public DateTime DateTo
		{
			get { return m_dateTo; }
			set { m_dateTo = value; }
		}
	}

}