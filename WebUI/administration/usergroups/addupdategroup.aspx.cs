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

namespace Orchestrator.WebUI.administration.usergroups
{
	/// <summary>
	/// Summary description for addupdategroup.
	/// </summary>
	public partial class addupdategroup : Orchestrator.Base.BasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditUser);

			// Put user code to initialize the page here
			if (!IsPostBack) 
				BindGrid();
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
			this.Init +=new EventHandler(addupdategroup_Init);
		}
		#endregion

		private void BindGrid()
		{
			Facade.UserAdmin facUserAdmin = new Facade.UserAdmin();
			Facade.IReferenceData refData = new Facade.ReferenceData();
			DataSet dsUserGroups = refData.GetAllRoles();
			dgUserGroups.DataSource = dsUserGroups;
			dgUserGroups.DataBind();
		}

		private void addupdategroup_Init(object sender, EventArgs e)
		{
			btnAdd.Click +=new EventHandler(btnAdd_Click);

            dgUserGroups.UpdateCommand += new ComponentArt.Web.UI.Grid.GridItemEventHandler(dgUserGroups_UpdateCommand);
            dgUserGroups.NeedRebind += new ComponentArt.Web.UI.Grid.NeedRebindEventHandler(dgUserGroups_NeedRebind);
            dgUserGroups.NeedDataSource += new ComponentArt.Web.UI.Grid.NeedDataSourceEventHandler(dgUserGroups_NeedDataSource);
		}

        private void UpdateUser(ComponentArt.Web.UI.GridItem item, string command)
        {
            switch (command)
            {
                case "UPDATE":
                    string description = item["Description"].ToString();
                    int roleId = Convert.ToInt32(item["RoleId"]);

                    Facade.IRole facRole = new Facade.Security();

                    facRole.UpdateRole(roleId, description);

                   // BindGrid();

                    //int extraId = Convert.ToInt32(item["ExtraId"].ToString());

                    //Facade.IJobExtra facJobExtra = new Facade.Job();

                    //Entities.Extra updatingExtra = facJobExtra.GetExtraForExtraId(extraId);

                    //updatingExtra.ExtraState = (eExtraState)Enum.Parse(typeof(eExtraState), item["ExtraState"].ToString());
                    //updatingExtra.ExtraAmount = Decimal.Parse(item["ExtraAmount"].ToString(), NumberStyles.Currency);
                    //updatingExtra.ClientContact = item["ClientContact"].ToString();

                    //facJobExtra.UpdateExtra(updatingExtra, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
            }
        }

        void dgUserGroups_NeedDataSource(object sender, EventArgs e)
        {
            BindGrid();
        }

        void dgUserGroups_NeedRebind(object sender, EventArgs e)
        {
            dgUserGroups.DataBind();
        }

        void dgUserGroups_UpdateCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            UpdateUser(e.Item, "UPDATE");
        }

        private void btnAdd_Click(object sender, EventArgs e)
		{

			if (Page.IsValid)
			{
				bool alreadyExists = false;

				foreach (ComponentArt.Web.UI.GridItem dgItem in dgUserGroups.Items)
				{
                    if (dgItem["Description"].ToString() == txtDescription.Text)
						alreadyExists = true;
				}

				if (alreadyExists)
				{
					lblError.Text = "User group already added with that name.";
					lblError.Visible = true;
				}
				else if (txtDescription.Text == "")
				{
					lblError.Text = "Please specify a name for the user group.";
					lblError.Visible = true;
				}
				else
				{
					Facade.IRole facRole = new Facade.Security();
					facRole.AddRole(txtDescription.Text);
					BindGrid();
				}
			}
		}
	}
}
