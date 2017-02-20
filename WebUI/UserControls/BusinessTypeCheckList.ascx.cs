using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.UserControls
{
    public partial class BusinessTypeCheckList : System.Web.UI.UserControl
    {

        private IDictionary<int, string> _allBusinessTypes = null;
        private IDictionary<int, string> AllBusinessTypes
        {
            get
            {
                if (_allBusinessTypes == null)
                {
                    Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                    var dsBusinessTypes = facBusinessType.GetAll();
                    this._allBusinessTypes = dsBusinessTypes.Tables[0].AsEnumerable().ToDictionary(dr => dr.Field<int>("BusinessTypeID"), dr => dr.Field<string>("Description"));
                }

                return this._allBusinessTypes;
            }
        }

        private IEnumerable<int> AllBusinessTypeIDs
        {
            get { return this.AllBusinessTypes.Select(kvp => kvp.Key); }
        }

        public int ItemCountPerRow
        {
            get { return lvBusinessType.GroupItemCount; }
            set { lvBusinessType.GroupItemCount = value; }
        }

        public IEnumerable<int> SelectedBusinessTypeIDs
        {
            get { return hidBusinessTypeIDs.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s)); }
            set { hidBusinessTypeIDs.Value = string.Join(",", value.Select(i => i.ToString()).ToArray()); }
        }

        public bool AllBusinessTypesSelected
        {
            get { return !this.AllBusinessTypeIDs.Except(this.SelectedBusinessTypeIDs).Any(); }
            set { this.SelectedBusinessTypeIDs = this.AllBusinessTypeIDs; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                lvBusinessType.DataSource = this.AllBusinessTypes.Select(kvp => new { BusinessTypeID = kvp.Key, Description = kvp.Value });
                lvBusinessType.DataBind();
            }
        }

    }
}