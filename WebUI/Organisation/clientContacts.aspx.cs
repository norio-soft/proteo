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
using Telerik.Web.UI;

using Orchestrator.Entities;

namespace Orchestrator.WebUI.Organisation
{
    public partial class clientContacts : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //  set the isClient property of the lookup control from the query string, if it is not supplied 
            //  then default to true. Also set the isCalledFromHotKey from the query string if it exists, if not supplied
            //  default to false
            if (!this.IsPostBack)
            {
                if (Request.QueryString["isCalledFromHotKey"] != null)
                {
                    bool isCalledFromHotKey = Convert.ToBoolean(Request.QueryString["isCalledFromHotKey"]);

                    this.contactLookUp1.IsCalledFromHotKey = isCalledFromHotKey;
                }
                else
                    this.contactLookUp1.IsCalledFromHotKey = false;

                if (Request.QueryString["isClient"] != null)
                {
                    bool isClient = Convert.ToBoolean(Request.QueryString["isClient"]);

                    this.contactLookUp1.IsClient = isClient;
                }
                else
                    this.contactLookUp1.IsClient = true;
            }
        }
    }
}
