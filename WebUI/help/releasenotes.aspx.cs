using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

namespace Orchestrator.WebUI.help
{
    public partial class releasenotes : System.Web.UI.Page
    {
        protected string path = "releaseNotes.html"; //specify the path to your file


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {

                //set the external file content in the editor
                NotesEditor.Content = ReadFile(Server.MapPath(path));
                NotesEditor.EditModes = Telerik.Web.UI.EditModes.Preview;
            }

            HttpContext httpContext = HttpContext.Current;

            Facade.ISecurity facSecurity = new Facade.Security();
            Facade.IUser facUser = new Facade.User();

            string[] userRoleString = (((Entities.CustomPrincipal)httpContext.User).UserRole.Split(new char[] { ',' }));
            eUserRole[] userRole = new eUserRole[userRoleString.Length];

            for (int i = 0; i < userRoleString.Length; i++)
                userRole[i] = (eUserRole)int.Parse(userRoleString[i]);

            bool canEdit = false;
            foreach (eUserRole r in userRole)
                if (r == eUserRole.SystemAdministrator)
                    canEdit = true;
            if (!canEdit)
                Server.Transfer("relnotes.aspx");

            pnldmin.Visible = canEdit;
        
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.btnEdit.Click += new EventHandler(btnEdit_Click);
        }

        void btnEdit_Click(object sender, EventArgs e)
        {
            HttpContext httpContext = HttpContext.Current;

            Facade.ISecurity facSecurity = new Facade.Security();
            Facade.IUser facUser = new Facade.User();

            string[] userRoleString = (((Entities.CustomPrincipal)httpContext.User).UserRole.Split(new char[] { ',' }));
            eUserRole[] userRole = new eUserRole[userRoleString.Length];

            for (int i = 0; i < userRoleString.Length; i++)
                userRole[i] = (eUserRole)int.Parse(userRoleString[i]);

            bool canEdit = false;
            foreach (eUserRole r in userRole)
                if (r == eUserRole.SystemAdministrator)
                    canEdit = true;

            if (canEdit)
            {
                NotesEditor.EditModes = Telerik.Web.UI.EditModes.All;
            }
            else
            {
                NotesEditor.EditModes = Telerik.Web.UI.EditModes.Preview;

            }
        }


        protected string ReadFile(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return string.Empty;
            }

            using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
            {
                return sr.ReadToEnd();
            }
        }

        protected void Button1_Click1(object sender, EventArgs e)
        {

            //Open file for writing and write content
            using (StreamWriter externalFile = new StreamWriter(this.MapPath(path), false, Encoding.UTF8))
            {
                externalFile.Write(NotesEditor.Content);

            }            
        }

    }
}