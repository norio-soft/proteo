using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI
{
    public delegate void SelectedPointChangedEventHandler(object sender, SelectedPointChangedEventArgs e);

    public class SelectedPointChangedEventArgs : EventArgs
    {
        private Entities.Point _selectedPoint = null;

        public SelectedPointChangedEventArgs(Entities.Point selectedPoint)
        {
            _selectedPoint = selectedPoint;
        }

        public Entities.Point SelectedPoint
        {
            get { return _selectedPoint; }
        }
    }

}