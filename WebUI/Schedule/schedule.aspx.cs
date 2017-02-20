using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
namespace Orchestrator.WebUI
{

    public partial class Schedule_schedule : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            Facade.Schedule facSchedule = new Facade.Schedule();
            DataSet ds = facSchedule.GetResourceSchedulesTypeDS(eResourceType.Driver, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);
            ucSchedule.ScheduleStartDate = dteStartDate.SelectedDate.Value;
            ucSchedule.ScheduleEndDate = dteEndDate.SelectedDate.Value;
            ucSchedule.Data = ds;
            ucSchedule.DataBind();
        }
    }
}