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
using System.Text;

namespace Orchestrator.WebUI.Point
{
    /// <summary>
    /// Summary description for GetLowerPointAddressHtml.
    /// </summary>
    public partial class GetLowerPointAddress: Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            int pointId = Convert.ToInt32(Request.QueryString["pointId"]);
            Entities.Point point = null;

            if (pointId > 0)
            {
                string cacheName = "_pointAddress" + pointId.ToString();
                if (Cache[cacheName] == null)
                {
                    Facade.IPoint facPoint = new Facade.Point();
                    point = facPoint.GetPointForPointId(pointId);
                }
                else
                {
                    point = (Entities.Point)Cache["_pointAddress" + pointId.ToString()];
                }
            }

            if (point != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(point.Address.AddressLine1);
                sb.Append("<br>");

                if (point.Address.AddressLine2.Length > 0)
                {
                    sb.Append(point.Address.AddressLine2);
                    sb.Append("<br>");
                }
                if (point.Address.AddressLine3.Length > 0)
                {
                    sb.Append(point.Address.AddressLine3);
                    sb.Append("<br>");
                }
                if (point.Address.PostTown.Length > 0)
                {
                    sb.Append(point.Address.PostTown);
                    sb.Append("<br>");
                }
                if (point.Address.County.Length > 0)
                {
                    sb.Append(point.Address.County);
                    sb.Append("<br>");
                }
                if (point.Address.PostCode.Length > 0)
                {
                    sb.Append(point.Address.PostCode);
                    sb.Append("<br>");
                }
                if (point.Address.CountryDescription.Length > 0)
                {
                    sb.Append(point.Address.CountryDescription);
                }
         
                Response.Write(sb.ToString());
                Response.Flush();
                Response.End();
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
