using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Point
{
	/// <summary>
	/// Summary description for GetPointAddressHtml.
	/// </summary>
	public partial class GetPointAddressHtml : Orchestrator.Base.BasePage
	{
		protected void Page_Load(object sender, System.EventArgs e)
		{
			int pointId = Convert.ToInt32(Request.QueryString["pointId"]);
            int instructionId = Convert.ToInt32(Request.QueryString["iID"]);

            Entities.Instruction instruction = null;
            Entities.Point point = null;

            if (instructionId > 0)
            {
                string cacheName = "_pointAddressWithNotes" + instructionId.ToString();

                if (Cache[cacheName] == null)
                {
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    instruction = facInstruction.GetInstruction(instructionId);

                    if (instruction != null)
                        Cache.Add(cacheName, instruction, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                {
                    instruction = (Entities.Instruction)Cache[cacheName];
                }

                if (instruction != null)
                    point = instruction.Point;
            }
            
            if (pointId > 0)
			{
                string cacheName = "_pointAddress" + pointId.ToString();
                if (Cache[cacheName] == null)
                {
                    Facade.IPoint facPoint = new Facade.Point();
                    point = facPoint.GetPointForPointId(pointId);
                    if (point != null)
                        Cache.Add(cacheName, point, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(0, 20, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                {
                    point = (Entities.Point)Cache["_pointAddress" + pointId.ToString()];
                }
            }

			if (point != null)
			{
				// A leg has been selected for viewing so display it.
                XslCompiledTransform transformer = new XslCompiledTransform();
				transformer.Load(Server.MapPath(@"..\xsl\instructions.xsl"));

				XmlUrlResolver resolver = new XmlUrlResolver();
				XPathNavigator navigator = point.ToXml().CreateNavigator();

				// Populate the Point.
				StringWriter sw = new StringWriter();
				transformer.Transform(navigator, null, sw);
				Response.Write(sw.GetStringBuilder().ToString());

                // If instruction notes are present, attach them to the response.
                if (instruction != null && instruction.Note.Length > 0)
                    Response.Write("<br>" + instruction.Note);
                
                Response.Write(string.Format("<br>tel: {0}", point.PhoneNumber));

                if (point.PointNotes != string.Empty)
                    Response.Write("<br>" + point.PointNotes.Replace("\r\n", "</br>"));

                if (Response.IsClientConnected)
                {
                    Response.Flush();
                    Response.End();
                }

				return;
			}
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
	}
}
