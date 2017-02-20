using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.administration.DeliveryWindow
{
    public partial class dragDropControl : System.Web.UI.UserControl
    {
        public String Label { get { return lblRegion.Text; } set { lblRegion.Text = value; } }
        public String Region { get { return region.Text; } set { region.Text = value; } }
        string mapItem = "<li id=\"{0}\" draggable=\"true\"  ondragstart=\"drag(event)\">{1}</li>";
        public String Items { get { return region.Text; } }
        public String Keys { get { return keys.Text; } }

        public ListBox ListCodes { get { return lstPostcodes; } set { lstPostcodes = value; } }

        public Int32 ZoneId { set { ViewState["ZoneID"] = value; } get { return  ViewState["ZoneID"]==null ? 0 : (Int32)ViewState["ZoneID"]; } }

        public delegate void ChangedEventHandler(List<ListItem> items);
        public delegate List<ListItem> AddedEventHandler();

        public event ChangedEventHandler Add;
        public event ChangedEventHandler Remove;
        public event AddedEventHandler AddedItems;

        public void Showbuttons(Boolean show)
        {
            btnAddSingle.Visible = show;
            btnRemove.Visible = show;
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected virtual void OnAdd(List<ListItem> items)
        {
            if (Add != null)
                Add(items);
        }

        protected virtual List<ListItem> OnAddedItems()
        {
            List<ListItem> v = new List<ListItem>();

            if (AddedItems != null)
                v= AddedItems();

            return v;
        }

        // Invoke the Changed event; called whenever list changes
        protected virtual void OnRemove(List<ListItem> items)
        {
            if (Remove != null)
                Remove(items);
        }



        public void AddItem(string key, string value)
        {
            Region += String.Format(mapItem, key, value);
            keys.Text += key + ";";


            ListItem item = new ListItem();
            item.Text = value;
            item.Value = key;

            lstPostcodes.Items.Add(item);

           
        }

        public void AddItem(DataColumn key, DataColumn value)
        {
            AddItem(key.ToString(), value.ToString());
        }


        public void AddItems(DataTable dt)
        {

            if (dt == null) return;

            if (dt.Rows == null) return;

            if (dt.Rows.Count == 0) return;

            foreach (DataRow dr in dt.Rows)
            {
                AddItem(dr["PostCodeRegionId"].ToString() , dr["Description"].ToString());
            }


        }

        protected void btnAddSingle_Click(object sender, EventArgs e)
        {
            List<ListItem> SelectedItems = new List<ListItem>();
           
            SelectedItems = OnAddedItems();

            foreach (ListItem li in SelectedItems)
            {
                lstPostcodes.Items.Add(li);
            }

            // get a LINQ-enabled list of the list items
            List<ListItem> list = new List<ListItem>(lstPostcodes.Items.Cast<ListItem>());
            // use LINQ to Objects to order the items as required
            list = list.OrderBy(li => li.Text).ToList<ListItem>();
            // remove the unordered items from the listbox, so we don't get duplicates
            lstPostcodes.Items.Clear();
            // now add back our sorted items
            lstPostcodes.Items.AddRange(list.ToArray<ListItem>());

        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            List<ListItem> SelectedItems = new List<ListItem>();

            foreach (ListItem listItem in lstPostcodes.Items)
            {
                if (listItem.Selected)
                {
                    //listItem.Value contains the value of the selected item
                    //listItem.Text is the text displayed in the listbox of the selected item
                    SelectedItems.Add(listItem);
                }
            }

            RemoteItems(SelectedItems);

            Remove(SelectedItems);

           
        }

        public void RemoteItems(List<ListItem> DelItems)
        {
            foreach (ListItem li in DelItems)
            {
                lstPostcodes.Items.Remove (li);
            }

        }

        public List<ListItem> GetSelectedItems()
        {
            List<ListItem> SelectedItems = new List<ListItem>();

            foreach (ListItem listItem in lstPostcodes.Items)
            {
                if (listItem.Selected)
                {
                    //listItem.Value contains the value of the selected item
                    //listItem.Text is the text displayed in the listbox of the selected item
                    SelectedItems.Add(listItem);
                }
            }

            return SelectedItems;
        }

        public List<String> GetItems()
        {


            List<String> SelectedItems = new List<String>();

            foreach (ListItem listItem in lstPostcodes.Items)
            {
                    SelectedItems.Add(listItem.Value);
                
            }

            return SelectedItems;
        }

    }
}