using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.Point
{
    public partial class addupdatepoint2 : System.Web.UI.Page
    {
        #region Private Fields
        int _pointID            = -1;
        int _identityID         = -1;
        string _clientName      = string.Empty;
        Entities.Point _point   = null;
        bool allowclose         = true;
        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            int.TryParse(Request.QueryString["pointId"], out _pointID);
            int.TryParse(Request.QueryString["identityId"], out _identityID);
            _clientName = Request.QueryString["clientName"];


           
            
            btnClose.Visible = allowclose;

            if (!IsPostBack)
            {
                Facade.IPoint facPoint = new Facade.Point();
                _point = facPoint.GetPointForPointId(_pointID);
                ucPointCtl.SelectedPoint = _point;
                if (_identityID > 0)
                {
                    ucPointCtl.NewPointOwnerIdentityID = _identityID;
                    ucPointCtl.NewPointOwnerDescription = _clientName;
                }
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            bool.TryParse(Request.QueryString["allowclose"], out allowclose);
            if (!allowclose)
                this.MasterPageFile = "~/default_tableless.Master";

        }

        
        #endregion
    }
}