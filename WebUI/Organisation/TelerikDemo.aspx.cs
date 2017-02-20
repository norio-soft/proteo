using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Organisation
{
    public partial class TelerikDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void RadTreeView1_NodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
        {
            e.SourceDragNode.Remove();

            if (e.HtmlElementID == RadListBox1.ClientID)
            {
                RadListBoxItem item = new RadListBoxItem();
                item.Text = e.SourceDragNode.Text;

                if (RadListBox1.SelectedIndex > -1)
                {
                    RadListBox1.Items.Insert(RadListBox1.SelectedIndex + 1, item);
                }
                else
                {
                    RadListBox1.Items.Add(item);
                }
            }
            else
            {
                if (e.DestDragNode.Level == 0)
                {
                    e.DestDragNode.Nodes.Add(e.SourceDragNode);
                }
                else
                {
                    e.DestDragNode.InsertAfter(e.SourceDragNode);
                }
            }
        }
        protected void RadListBox1_ItemDropped(object sender, RadListBoxDroppedEventArgs e)
        {
            if (e.HtmlElementID == RadTreeView1.ClientID)
            {
                if (RadTreeView1.SelectedNode != null)
                {
                    foreach (RadListBoxItem item in e.SourceDragItems)
                    {
                        RadTreeNode node = new RadTreeNode(item.Text);
                        if (RadTreeView1.SelectedNode.Level == 0)
                        {
                            RadTreeView1.SelectedNode.Nodes.Add(node);
                        }
                        else
                        {
                            RadTreeView1.SelectedNode.InsertAfter(node);
                        }

                        item.Remove();
                    }
                }
            }
        }
    }
}
