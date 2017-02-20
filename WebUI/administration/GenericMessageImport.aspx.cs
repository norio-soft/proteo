using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.administration
{
    public partial class GenericMessageImport : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnSubmit.Click += new EventHandler(btnSubmit_Click);
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            int importMessageId = 0;

            StreamReader messageReader = null;

            if (rbCopyPaste.Checked)
            {
                // Copy paste
                if (this.txtMessage.Text.Length > 0)
                {
                    //Convert the string to a stream so that it can be read line by line
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(txtMessage.Text);
                    MemoryStream stream = new MemoryStream(bytes); 
                    messageReader = new StreamReader(stream);
                }
            }
            else
            {
                messageReader = new StreamReader(this.upload1.FileContent);
            }

            try
            {
                if (messageReader != null && !messageReader.EndOfStream)
                {
                    BusinessLogicLayer.ImportMessage busImportMessage = new Orchestrator.BusinessLogicLayer.ImportMessage();


                    //Create a messsage for each line
                    int count = 0;
                    while (!messageReader.EndOfStream)
                    {
                        string message = messageReader.ReadLine();

                        importMessageId = busImportMessage.Create(txtFromSystem.Text, eMessageState.Unprocessed, message, this.Page.User.Identity.Name);
                        count++;
                    }

                    this.txtMessage.Text = string.Empty;
                    lblResult.Text = string.Format("{0} messages have successfully been added.", count);
                }
                else
                {
                    lblResult.Text = "Messages were not supplied so none have been added.";
                }
            }
            catch 
            {
                throw;
            }
            finally
            {
                if (messageReader != null)
                    messageReader.Dispose();
            }

            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                string fromSystem = string.Empty;
                fromSystem = Request.QueryString["system"];

                if (!string.IsNullOrEmpty(fromSystem))
                {
                    this.txtFromSystem.Text = fromSystem;
                    this.txtFromSystem.ReadOnly = true;
                }
                else
                {
                    this.txtFromSystem.Text = "";
                    this.txtFromSystem.ReadOnly = false;
                }
            }
        }


    }
}
