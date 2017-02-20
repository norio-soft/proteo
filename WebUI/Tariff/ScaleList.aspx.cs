using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Humanizer;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class ScaleList : Orchestrator.Base.BasePage
    {

        protected void Page_Init(object sender, EventArgs e)
        {
            this.Title = "Orchestrator - Scales";
            btnAdd.Click += btnAdd_Click;
            chkEnabledOnly.CheckedChanged += chkEnabledOnly_CheckedChanged;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadScales();
            }
        }

        void chkEnabledOnly_CheckedChanged(object sender, EventArgs e)
        {
            LoadScales();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/AddScale.aspx");
        }

        private void LoadScales()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IScaleRepository>(uow);

                //Get the Scales 
                var scales =
                    from s in repo.GetAll()
                    where s.IsEnabled || !chkEnabledOnly.Checked
                    orderby s.Description
                    select new
                    {
                        s.ScaleID,
                        s.Description,
                        s.Metric,
                        s.IsEnabled,
                    };

                //and bind the result to the grid
                grdScales.DataSource = scales.ToList();
                grdScales.DataBind();
            }
        }
    
    }

}
