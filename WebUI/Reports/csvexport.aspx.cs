using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Reports
{
	/// <summary>
	/// Summary description for csvexport.
	/// </summary>
	public partial class csvexport : Orchestrator.Base.BasePage
	{
		private DataView exportDS;
        private string filename = "JobExport.csv";

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (null!=Session["__ExportDS"])
				exportDS = new DataView((DataTable)Session["__ExportDS"]);

			if (Request.QueryString["filename"] != null)
				filename = Request.QueryString["filename"];
			// Clean the Ds
			CleanDS();

			if (exportDS != null)
				exportToCSV();
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
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
		}
		#endregion

		private void CleanDS()
		{
			DataRow row = null;
			for(int i = 0; i <exportDS.Table.Rows.Count; i++)
			{
				row = exportDS.Table.Rows[i];
				if (row[0].ToString() == "__SubHeading")
				{
					exportDS.Delete(i);
				}
			}
		}

		private void exportToCSV()
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			for (int i = 0; i <= exportDS.Table.Columns.Count - 1; i++)
			{
				if (i < exportDS.Table.Columns.Count -1) 
					sb.Append ("\"" + exportDS.Table.Columns[i].ColumnName + "\"" + ",");
				else
					sb.Append ("\"" + exportDS.Table.Columns[i].ColumnName + "\"" + "\n");
			}

			foreach(DataRow row in exportDS.Table.Rows)
			{
				for (int i = 0; i <= exportDS.Table.Columns.Count -1; i++)
				{
					if (i < exportDS.Table.Columns.Count -1)
						sb.Append("\"" + row[i].ToString() + "\"" + ",");
					else
						sb.Append("\""  + row[i].ToString() + "\"" + "\n");
				}
			}
			WriteToHTTPStream(sb);
		}
		

		private void WriteToHTTPStream(System.Text.StringBuilder sb)
		{
            Response.ContentType = "text/comma-separated-values";
			Response.AddHeader("content-disposition", @"attachment; filename =""" + filename + "\"") ;
			Response.Write(sb.ToString());
			Response.Flush();
			Response.End();

		}
	}
}
