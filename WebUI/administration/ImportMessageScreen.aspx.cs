using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.administration
{
    public partial class ImportMessageScreen : Orchestrator.Base.BasePage
    {
        private const string FRIENDLY_FROMSYSTEM_NAME_CONST = "FriendlyFromSystemName";

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.ImportMessageManagement);

            if (!IsPostBack)
                GetImportMessages();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnHiddenUpdateMessage.Click += new EventHandler(btnHiddenUpdateMessage_Click);
        }

        private void btnHiddenUpdateMessage_Click(object sender, EventArgs e)
        {
            var newImportMessageContent = this.hidImportMessageText.Value;
            int oldImportMessageID = 0;

            int.TryParse(this.hidImportMessageID.Value, out oldImportMessageID);

            Facade.ImportMessage facImportMes = new Facade.ImportMessage();

            var oldImportMessage = facImportMes.GetForImportMessageId(oldImportMessageID);

            var newImportMessage = new Entities.ImportMessage
                (
                    oldImportMessage.FileName,
                    oldImportMessage.FromSystem,
                    eMessageState.Unprocessed,
                    newImportMessageContent,
                    oldImportMessage.EntityTypeId,
                    oldImportMessage.EntityId
                );
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            facImportMes.Create(newImportMessage, userName);
        }

        protected void grdImportMessages_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            GetImportMessages();

        }

        protected void grdImportMessages_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView dr = (DataRowView)(e.Item.DataItem);

                Panel panMessage = (Panel)e.Item.FindControl("panMessage");
                Literal litMessage = (Literal)e.Item.FindControl("litMessage");

                Label lblAlert = (Label)e.Item.FindControl("lblAlert");
                Label lblFromSystem = (Label)e.Item.FindControl("lblFromSystem");
                HyperLink editLink = (HyperLink)e.Item.FindControl("EditLink");

                var message = dr["Message"].ToString();
                string fromSystem = dr["FromSystem"].ToString();

                litMessage.Text = message;

                if(dr["Alert"].ToString().Length > 200)
                {
                    lblAlert.Text = dr["Alert"].ToString().Substring(0, 200) + "...";
                    lblAlert.ToolTip = dr["Alert"].ToString();
                }
                else
                {
                    lblAlert.Text = dr["Alert"].ToString();
                }

                //single quotes on their own cause the model to not appear.
                editLink.Attributes["href"] = "javascript:void(0);";
                editLink.Attributes["onclick"] = "editImportMessage('" + dr["ImportMessageID"] + "' , '" + panMessage.ClientID + "');";

                var friendlyFromSystemNames = Facade.IntegrationMapping.GetMappings(FRIENDLY_FROMSYSTEM_NAME_CONST, FRIENDLY_FROMSYSTEM_NAME_CONST);

                var intMapping = friendlyFromSystemNames.FirstOrDefault(fs => fs.FromValue == fromSystem);

                lblFromSystem.Text = (intMapping != null) ? intMapping.ToValue : fromSystem;
                
            }
            else if(e.Item is GridCommandItem)
            {
                
            }
        }

        private int m_numberOfImportMessages = 0;

        protected int NumberOfImportMessages
        {
            get { return m_numberOfImportMessages; }
        }

        protected void btnViewImportMessages_Click(object sender, EventArgs e)
        {
            grdImportMessages.Rebind();
        }

        private void GetImportMessages()
        {
            if (dteDateFrom.SelectedDate.HasValue && dteDateTo.SelectedDate.HasValue)
            {

                var dateFrom = dteDateFrom.SelectedDate.Value;
                var dateTo = dteDateTo.SelectedDate.Value;

                TimeSpan toTime = new TimeSpan(23, 59, 0);
                dateTo = dateTo + toTime;

                Facade.ImportMessage facImportMes = new Facade.ImportMessage();
                var ds = facImportMes.GetAllForView(dateFrom, dateTo, RadFromSystem.Text);
                m_numberOfImportMessages = ds.Tables[0].Rows.Count;
                grdImportMessages.DataSource = ds;
            }
            else
            {
                grdImportMessages.DataSource = string.Empty;

            }
        }

        protected void grdImportMessages_PreRender(object sender, EventArgs e)
        {
            RadGrid rg = sender as RadGrid;
            m_numberOfImportMessages = rg.Items.Count;
        }
    }
}