using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI
{
    public partial class nofleetmetrik : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            

        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Masterpage_Tableless master = this.Master as Masterpage_Tableless;
            master.MainMenu.LoadContentFile("/usercontrols/menuDefault.xml");

        }
    }
}