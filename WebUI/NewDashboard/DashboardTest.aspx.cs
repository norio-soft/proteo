using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.WebUI.NewDashboard;
using System.Data;
using System.Web.Security;

namespace Orchestrator.WebUI.NewDashboard
{
    public partial class DashboardTest : System.Web.UI.Page
    {
        public int dashBoardID;
        public int userID;
        public bool home;

        protected void Page_Load(object sender, EventArgs e)
        {
            var user = (Orchestrator.Entities.CustomPrincipal)Page.User;
            userID = user.IdentityId;
            // Get list of dashboards
            DataSet dashboardListDS = DashboardDataAccess.getDashboardsForUser(userID);

            if (dashboardListDS.Tables[0].Rows.Count == 0)
            {
                var dsNewDash = DashboardDataAccess.insertNewDashboard(userID, "Home");
                DashboardDataAccess.makeHomeDashboard(int.Parse(dsNewDash.Tables[0].Rows[0]["newID"].ToString()), userID);
                dashboardListDS = DashboardDataAccess.getDashboardsForUser(userID);
            }

            dashboardList.DataSource = dashboardListDS.Tables[0];
            dashboardList.DataValueField = dashboardListDS.Tables[0].Columns[0].ToString();
            dashboardList.DataTextField = dashboardListDS.Tables[0].Columns[1].ToString();
            DataSet dbWidgets; 
            if (!Page.IsPostBack)
            {
                dashboardList.DataBind();
            }
            
            //Get widgets on home dashboard
            if (Request.QueryString["dashboardID"] == null)
            {
                DataSet dashBoardDetails = DashboardDataAccess.getHomeDashboardDetails(userID);
                home = true;
                dashBoardID = (int)dashBoardDetails.Tables[0].Rows[0]["dashboardId"];
                dbWidgets = DashboardDataAccess.getDBWidgetsForDashboardID(dashBoardID, userID);
                if (!Page.IsPostBack)
                    dashboardList.Items.FindByValue(dashBoardID.ToString()).Selected = true;
            }
            else
            {

                dashBoardID = int.Parse(Request.QueryString["dashboardID"]);
                dbWidgets = DashboardDataAccess.getDBWidgetsForDashboardID(dashBoardID, userID);
                if (dbWidgets.Tables.Count > 0)
                {
                    if (dbWidgets.Tables[1].Rows.Count > 0)
                    {
                        
                        if (!Page.IsPostBack)
                        {
                            updateName.Text = dbWidgets.Tables[1].Rows[0]["dashboardName"].ToString();
                            dashboardList.Items.FindByValue(dashBoardID.ToString()).Selected = true;
                        }  
                    }
                    else
                    {
                        Response.Redirect("~/NewDashboard/DashboardTest.aspx");
                    }
                }
                
            }
            
            if (DashboardDataAccess.homeDashboard(dashBoardID))
            {
                home = true;
                deleteDB.Visible = false;
                lblHomeDashboard.Text = "This is your home dashboard.";
                setHome.Visible = false;
            }
            DataSet widgetListDS = DashboardDataAccess.getWidgetsNotOnDBForDBID(dashBoardID);
            widgetsList.DataSource = widgetListDS.Tables[0];
            widgetsList.DataValueField = widgetListDS.Tables[0].Columns[0].ToString();
            widgetsList.DataTextField = widgetListDS.Tables[0].Columns[1].ToString();
            if (!Page.IsPostBack)
            {
                widgetsList.DataBind();
            }
            string widgetsHtml = "";
            UserControl userControl;
            UserControl userControl2;
            DropDownList widgetModeNames;
            foreach (DataRow row in dbWidgets.Tables[0].Rows)
            {
                //If the widget is not a javascript based chart
                /*
                 * 1 - Table
                 * 2 - Javascript
                 * 3 - Datatable
                 */
                if ((int)row["type"] == 1)
                {
                    userControl = (UserControl)LoadControl("~/UserControls/Webparts/wp" + row["tagName"].ToString() + ".ascx");
                    //userControl = (UserControl)LoadControl("~/NewDashboard/UserControls/" + row["tagName"].ToString() + ".ascx");
                }
                else if ((int)row["type"] == 3)
                {
                    userControl = (UserControl)LoadControl("~/NewDashboard/UserControls/" + row["tagName"].ToString() + ".ascx");
                }
                else
                {
                    //The user control has to be added twice so it resizes without needing to refresh the dashboard
                    userControl = (UserControl)LoadControl("~/NewDashboard/UserControls/" + row["tagName"].ToString() + ".ascx");
                    userControl2 = (UserControl)LoadControl("~/NewDashboard/UserControls/" + row["tagName"].ToString() + ".ascx");
                    widgetsHtml = "<textarea id=\"code" + row["widgetOnDashboardID"] + "\" style=\"display: none;\">";
                    widgetsHtml += "<div id=\"" + row["tagName"] + "\" style=\"width: 100%; height: 100%;\"></div>";
                    widgitPlaceHolder.Controls.Add(new LiteralControl(widgetsHtml));
                    widgitPlaceHolder.Controls.Add(userControl2);
                    widgetsHtml = "</textarea>";
                    widgitPlaceHolder.Controls.Add(new LiteralControl(widgetsHtml));
                    javascriptPlaceHolder.Controls.Add(userControl);
                    userControl2.Attributes.Add("mode", row["mode"].ToString());
                }
                //The control has to be displayed inside the div tags
                widgetsHtml = "<div class=\"dragbox\" id=\"dragbox_" + row["widgetOnDashboardID"]+ "\" style=\"overflow: hidden; position: absolute; z-index: 2; left:"+ row["leftPos"] +"px; top:"+row["topPos"]+"px; width:"+row["width"]+"px; height:"+row["height"]+"px\">\n";
                widgetsHtml += "<div class=\"header\" style=\"text-align:center\">\n";
                widgetsHtml += " <div class=\"headerTitle\">" + row["widgetName"].ToString();
                widgitPlaceHolder.Controls.Add(new LiteralControl(widgetsHtml));
                //If the user is able to select modes for the control then a drop down list is produced
                if (bool.Parse(row["hasModes"].ToString()) == true)
                {
                    widgitPlaceHolder.Controls.Add(new LiteralControl(" For: "));
                    widgetModeNames = new DropDownList();
                    widgetModeNames.ID = "widgetModeNameList_" + row["widgetOnDashboardID"];
                    DataSet widgetModeNamesDS = DashboardDataAccess.getModesForWidget(int.Parse(row["widgetID"].ToString()));
                    widgetModeNames.DataSource = widgetModeNamesDS.Tables[0];
                    widgetModeNames.DataValueField = widgetModeNamesDS.Tables[0].Columns[1].ToString();
                    widgetModeNames.DataTextField = widgetModeNamesDS.Tables[0].Columns[0].ToString();
                    widgetModeNames.DataBind();
                    widgetModeNames.AutoPostBack = true;
                    widgetModeNames.SelectedIndexChanged += new EventHandler(widgetModeNames_SelectedIndexChanged);
                    widgitPlaceHolder.Controls.Add(widgetModeNames);
                    if (!Page.IsPostBack)
                    {
                        widgetModeNames.Items.FindByValue(row["mode"].ToString()).Selected = true;
                    }
                }
                widgetsHtml = "<span>&nbsp;<span style=\"float: right;\">";
                widgitPlaceHolder.Controls.Add(new LiteralControl(widgetsHtml));
                LinkButton deleteLink = new LinkButton();
                deleteLink.ID = "deleteWidgetLink_" + row["widgetOnDashboardID"].ToString();
                deleteLink.CommandArgument = row["widgetOnDashboardID"].ToString();
                deleteLink.Text = "X";
                deleteLink.Click += new EventHandler(deleteLink_Click);
                widgitPlaceHolder.Controls.Add(deleteLink);
                //If the control is not a graph then allow a scroll bar to be visible
                if ((int)row["type"] == 1 || (int)row["type"] == 3)
                {
                    widgetsHtml = "</span></div></div><div class=\"dragbox-content\" id=\"view" + row["widgetOnDashboardID"] + "\" style=\"clear: both; width:" + ((int)row["width"] - 10) + "px; height:" + ((int)row["height"] - 30) + "px;\"><div id=\"" + row["tagName"] + "\" style=\"width: 100%; height: 100%; overflow-y: auto;\">";
                    //widgetsHtml = "</span></div></div><div class=\"dragbox-content\" id=\"view" + row["widgetOnDashboardID"] + "\" style=\"clear: both; width:" + ((int)row["width"] - 10) + "px; height:" + ((int)row["height"] - 30) + "px;\"><div id=\"" + row["tagName"] + "\" style=\"width: 100%; height: 100%;\">";
                }
                else
                {
                    widgetsHtml = "</span></div></div><div class=\"dragbox-content\" id=\"view" + row["widgetOnDashboardID"] + "\" style=\"clear: both; width:" + ((int)row["width"] - 10) + "px; height:" + ((int)row["height"] - 30) + "px;\"><div id=\"" + row["tagName"] + "\" style=\"width: 100%; height: 100%;\">";
                }
                widgitPlaceHolder.Controls.Add(new LiteralControl(widgetsHtml));
                if ((int)row["type"] == 1 || (int)row["type"] == 3)
                {
                    widgitPlaceHolder.Controls.Add(userControl);
                }
                widgitPlaceHolder.Controls.Add(new LiteralControl("</div></div></div>\n\n"));
                /* Current Modes
                 * 1 - Last 30 Days
                 * 2 - This week
                 * 3 - This month
                 * 4 - This year */
                userControl.Attributes.Add("mode", row["mode"].ToString());
                
            }
            
            
        }

        protected void widgetModeNames_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList widgetModeNames = (DropDownList)sender;
            int widgetOnDashboardID = int.Parse(widgetModeNames.ID.Split('_')[1]);
            int modeID = int.Parse(widgetModeNames.SelectedValue);
            DashboardDataAccess.updateWidgetOnDashboardMode(modeID, widgetOnDashboardID);
            Response.Redirect(Request.RawUrl);
        }

        void deleteLink_Click(object sender, EventArgs e)
        {
            var link = ((LinkButton)sender);
            int widgetOnDashboardID = int.Parse(link.CommandArgument);
            DashboardDataAccess.deleteWidgetFromDB(widgetOnDashboardID);
            Response.Redirect(Request.RawUrl);
        }

        protected void addWidgetButton_Click(object sender, EventArgs e)
        {
            DashboardDataAccess.addWidgetToDashboard(int.Parse(widgetsList.SelectedItem.Value), dashBoardID);
            Response.Redirect(Request.RawUrl);
        }

        protected void addNewDashboard_Click(object sender, EventArgs e)
        {
            DataSet newIDDS = DashboardDataAccess.insertNewDashboard(userID, addDashboardName.Text);
            int newID = (int)newIDDS.Tables[0].Rows[0]["newID"];
            Response.Redirect("~/NewDashboard/DashboardTest.aspx?dashboardID=" + newID);
        }

        protected void dbList_Index_Changed(Object sender, EventArgs e)
        {
            var dbList = ((DropDownList) sender);
            Response.Redirect("~/NewDashboard/DashboardTest.aspx?dashboardID=" + dbList.SelectedItem.Value);
        }

        protected void btnUpdateSettings_Click(object sender, EventArgs e)
        {
            DashboardDataAccess.updateDashboardName(updateName.Text, dashBoardID);
            
            if (setHome.Checked == true && home == false)
            {
                DashboardDataAccess.makeHomeDashboard(dashBoardID, userID);
            }
            Response.Redirect(Request.RawUrl);
        }

        protected void deleteDB_Click(object sender, EventArgs e)
        {
                DashboardDataAccess.deleteDashboard(dashBoardID);
                Response.Redirect(Request.RawUrl);
        }

    }
}
