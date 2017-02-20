using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Orchestrator;

namespace Orchestrator.WebUI
{
    public delegate void SelectedMultiTrunkChangedEventHandler(object sender, SelectedMultiTrunkChangedEventArgs e);

    public class SelectedMultiTrunkChangedEventArgs : EventArgs
    {
        private Entities.MultiTrunk _selectedMultiTrunk = null;

        public SelectedMultiTrunkChangedEventArgs(Entities.MultiTrunk selectedMultiTrunk)
        {
            _selectedMultiTrunk = selectedMultiTrunk;
        }

        public Entities.MultiTrunk SelectedMultiTrunk
        {
            get { return _selectedMultiTrunk; }
        }
    }
}

