namespace Orchestrator.WebUI.UserControls
{
	using System;
	using System.Data;
	using System.Drawing;
	using System.Web;
	using System.Web.UI.WebControls;
	using System.Web.UI.HtmlControls;

	/// <summary>
	///		Summary description for CallInSelector.
	/// </summary>
	public partial class CallInSelector : System.Web.UI.UserControl
	{
		#region Form Elements


		#endregion

		#region Page Variables

		private int		m_jobId = 0;
		private bool	m_hidePendingCallIns = true;

		#endregion

		#region Property Interfaces

		protected int JobId
		{
			get { return m_jobId; }
			set { m_jobId = value; }
		}

		protected bool HidePendingCallIns
		{
			get { return m_hidePendingCallIns; }
			set { m_hidePendingCallIns = true; }
		}

		#endregion

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				m_jobId = Convert.ToInt32(Request.QueryString["JobId"]);
			}
			catch {}

			if (!IsPostBack)
			{
				cboCallIns.Attributes.Add("onChange", "javascript:SelectCallIn()");

				if (m_jobId > 0)
				{
					// Retrieve and populate the call in selector.
					using (Facade.IInstruction facInstruction = new Facade.Instruction())
					{
						DataSet instructions = facInstruction.GetInstructionsForCallInSelection(m_jobId);
						if (instructions.Tables[0].Rows.Count == 0)
							cboCallIns.Visible = false;
						else
						{
							DataView data = new DataView(instructions.Tables[0]);
							if (m_hidePendingCallIns)
								data.RowFilter = "InstructionActualId IS NOT NULL";

							cboCallIns.DataValueField = "InstructionId";
							cboCallIns.DataTextField = "Description";
							cboCallIns.DataSource = data;
							cboCallIns.DataBind();

							cboCallIns.Items.Insert(0, new ListItem("-- Please select --", "0"));
							if (cboCallIns.Items.Count == 1)
								cboCallIns.Visible = false;
						}
					}
				}
				else
					cboCallIns.Visible = false;
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
