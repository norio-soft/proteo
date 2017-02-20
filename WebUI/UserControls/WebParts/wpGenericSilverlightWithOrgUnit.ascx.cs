using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;
using System.Web.Caching;

using Telerik.Web.UI;

using Telerik.Charting;

using System.Drawing;


namespace Orchestrator.WebUI.WebParts
{
    public partial class GenericSilverlightWithOrgUnit : System.Web.UI.UserControl, IWebPart, IWebEditable //, IWebActionable
    {
        
        #region IWebPart Members

        public string CatalogIconImageUrl { get; set; }
        public string Description { get; set; }
        public string Subtitle { get { return string.Empty; } }
        public string Title { get; set; }
        public string TitleIconImageUrl { get; set; }
        public string TitleUrl { get; set; }

        #endregion

        #region IWebEditable Members
        //This is used to enable the OrgUnit to be selected from a drop down list
        //of OrgUnits from the database

        EditorPartCollection IWebEditable.CreateEditorParts()
        {
            List<EditorPart> editors = new List<EditorPart>();
            editors.Add(new WebUI.UserControls.WebParts.OrgUnitEditorPart(this.ID));
            return new EditorPartCollection(editors);
        }

        object IWebEditable.WebBrowsableObject
        {
            get { return this; }
        }

        #endregion

        #region Additional Properties

        //These properties are initially set by attributes in the markup
        //but then have to be persisted in the personalisation properties
        [Personalizable(PersonalizationScope.Shared)]
        public String Xap { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public String Xaml { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public int Width { get; set; }

        [Personalizable(PersonalizationScope.Shared)]
        public int Height { get; set; }

        #endregion

        //This setting has its own editor OrgUnitEditorPart which allows the user to set it
        [Personalizable(true)]
        public int? OrgUnitId { get; set; }

        //public GetOrgUnitQueryString()
        //{
        //    if (OrgUnitId.HasValue)
        //    {
            
        //    }
        //}


        public String CacheProofXap 
        { 
            get
            {
                //Get the file creation date of the xap
                string xapPath = HttpContext.Current.Server.MapPath("~/ClientBin/" + this.Xap);
                DateTime xapCreationDate = System.IO.File.GetLastWriteTime(xapPath);

                //Use it to generate a version number
                long version = xapCreationDate.Ticks % 99999;

                //Add it to the xap path to force a download if the xap has been changed
                return this.Xap + "?v=" + version.ToString();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }


    }
}
