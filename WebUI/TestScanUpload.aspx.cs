using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Data;
using System.Reflection;
using System.IO;
using System.Net.Mail;
using System.Net;

namespace Orchestrator.WebUI
{
    public partial class TestScanUpload : Orchestrator.Base.BasePage
    {
        private DataRow _dataRow = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            this.btnTestEmail.Click += new EventHandler(btnTestEmail_Click);
        }

        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            Facade.EmailTemplate facEmailTemplate = new Facade.EmailTemplate();
            Entities.EmailTemplate emailTemplate = facEmailTemplate.GetForEmailTemplateId(Globals.Configuration.PodEmailTemplateId);

            // Get data to be used to populate placeHolders
            Facade.IPOD facPod = new Facade.POD();
            facPod.SendClientPod(113530);

        }
    }
}
