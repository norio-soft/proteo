using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Code.components
{
    //Tree node type
    public enum TreeDataNodeType { NotSet, OrgUnit, Vehicle, Driver }

    //Class to hold the data for a tree node
    public class TreeDataNode
    {
        public int? OrgUnitId { get; set; }
        public int? ParentOrgUnitId { get; set; }
        public int? ResourceId { get; set; }
        public TreeDataNodeType Type { get; set; }
        public string Text { get; set; }
        public bool Checked { get; set; }
    }
}