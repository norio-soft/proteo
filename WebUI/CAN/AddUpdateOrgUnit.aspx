<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="True" CodeBehind="AddUpdateOrgUnit.aspx.cs" Inherits="Orchestrator.WebUI.CAN.AddUpdateOrgUnit" EnableViewState="true" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Grouping</h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadCodeBlock runat="server" ID="RadCodeBlock1">
        <script type="text/javascript">
        //<!--
        //indicates whether the user is currently dragging a listbox item
        var listBoxDragInProgress = false;

        //indicates whether the user is currently dragging a tree node
        var treeViewDragInProgress = false;

        //select the hovered listbox item if the user is dragging a node
        function onListBoxMouseOver(sender, args) {
            if (treeViewDragInProgress) {

                args.get_item().select();
            }
        }

     
     
    function onClientContextMenuShowing(sender, args) {
        var treeNode = args.get_node();
        treeNode.set_selected(true);
        //enable/disable menu items
        setMenuItemsState(args.get_menu().get_items(), treeNode);
    }

    function onClientContextMenuItemClicking(sender, args) {
        var menuItem = args.get_menuItem();
        var treeNode = args.get_node();
        menuItem.get_menu().hide();

        switch (menuItem.get_value()) {
            case "New":
                break;
            case "Rename":
                treeNode.startEdit();
                break;
            case "Delete":
                var result = confirm('Are you sure you want to delete "' + treeNode.get_text() + '"?');
                args.set_cancel(!result);
                if (result)
                {
                    $("input[id $='btnDeleteNode'").click();
                }
                break;
        }
    }



    //this method disables the appropriate context menu items
    function setMenuItemsState(menuItems, treeNode) {
        for (var i = 0; i < menuItems.get_count(); i++) {
            var menuItem = menuItems.getItem(i);
            switch (menuItem.get_value()) {
                case "New":
                    menuItem.set_text(String.format('New unit in "{0}"', treeNode.get_text()));
                    break;
                case "Rename":
                    menuItem.set_text(String.format('Rename "{0}"', treeNode.get_text()));
                    break;
                case "Delete":
                    if (treeNode.get_parent() ==  treeNode.get_treeView()) {
                        menuItem.set_enabled(false);
                        menuItem.set_text('Delete');
                    }
                    else {
                        menuItem.set_enabled(true);
                        menuItem.set_text(String.format('Delete "{0}"', treeNode.get_text()));
                    }
                    break;
            }
        }
    }

    // Without this, anything other than 100% Zoom on Chrome will crash the screen on Post-Back
    // This will be fixed in Q3 2014 SP1 and can be removed from this solution when upgraded
    // http://www.telerik.com/forums/system-formatexception-input-string-was-not-in-a-correct-format-thrown-on-chrome-when-browser-is-zoomed-in-out
    Telerik.Web.UI.RadTreeView.prototype.saveClientState = function () {
        return "{\"expandedNodes\":" + this._expandedNodesJson +
        ",\"collapsedNodes\":" + this._collapsedNodesJson +
        ",\"logEntries\":" + this._logEntriesJson +
        ",\"selectedNodes\":" + this._selectedNodesJson +
        ",\"checkedNodes\":" + this._checkedNodesJson +
        ",\"scrollPosition\":" + Math.round(this._scrollPosition) + "}";
    }

    Telerik.Web.UI.RadListBox.prototype.saveClientState = function () {
        return "{" +
        "\"isEnabled\":" + this._enabled +
        ",\"logEntries\":" + this._logEntriesJson +
        ",\"selectedIndices\":" + this._selectedIndicesJson +
        ",\"checkedIndices\":" + this._checkedIndicesJson +
        ",\"scrollPosition\":" + Math.round(this._scrollPosition) +
        "}";
    }

    //-->
    </script>
    </telerik:RadCodeBlock>

    <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" >
        <img src="/images/postbackLoading.gif" alt="Loading" />
    </telerik:RadAjaxLoadingPanel>
    
    <div>
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server" EnableAJAX="true">
        
        <div style="float:left; padding:0px 40px 40px 40px;" >
        <table>
                    <tr>
                        <th>
                            Grouping
                        </th>
                        <th>
                            <asp:Label ID="txtResourceType" runat ="server" Text ="Vehicles not in this Group" />
                        </th>
                        <th>
                            <asp:Label ID="txtResourceTypeList" runat ="server" Text="Vehicles in this Group"  />
                        </th>
                    </tr>
                    <tr style="vertical-align: top;">
                        <td>
                            <asp:TextBox ID="txtNodeName" runat="server"></asp:TextBox>
                            <asp:Button ID="btnDeleteNode" runat="server" Text="Remove Selected Group" Visible="true" />
                            <asp:Button ID="btnAddNode" runat="server" Text="Add Group"/>

                             <telerik:RadTreeView ID="OrgUnitTree" runat="server" MultipleSelect="false"  
                                AllowNodeEditing="True" EnableDragAndDrop="true" EnableDragAndDropBetweenNodes="true" 
                                OnClientContextMenuItemClicking="onClientContextMenuItemClicking"
                                OnClientContextMenuShowing="onClientContextMenuShowing" 
                                DataTextField="Text" DataFieldID="OrgUnitId" DataFieldParentID="ParentOrgUnitId" 
                                >
                                <ContextMenus>
                                    <telerik:RadTreeViewContextMenu ID="MainContextMenu" runat="server">
                                        <Items>
                                            <telerik:RadMenuItem Value="Rename" Text="Rename" PostBack="false" >
                                            </telerik:RadMenuItem>
                                            <telerik:RadMenuItem Value="Delete" Text="Delete" >
                                            </telerik:RadMenuItem>
                                        </Items>
                                        <CollapseAnimation Type="none" />
                                    </telerik:RadTreeViewContextMenu>
                                </ContextMenus>
                            </telerik:RadTreeView>
                        </td>
                        <td>
                            <telerik:RadListBox runat="server" ID="lbResourcesNotInGroup" Width="282" EnableDragAndDrop="true"
                                AllowTransfer="true" TransferToID="lbResourcesInGroup" AllowTransferDuplicates="false"
                                Height="400" MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                                SelectionMode="Multiple"  DataValueField="ResourceId" DataTextField="Description"/>
                        </td>
                        <td>
                            <telerik:RadListBox runat="server" ID="lbResourcesInGroup" Width="250" EnableDragAndDrop="true"
                                Height="400" SelectionMode="Multiple"  DataValueField="ResourceId" DataTextField="Description"/>
                        </td>
                    </tr>
                </table>
        </div>
        
        <div style="float:left; width:500px; padding:0px 40px 40px 0px" >
            <p>
                Select which type of Resource to view.
            </p>
            <p>
                <asp:RadioButton ID="VehiclesRadioButton" runat="server" Checked="true"  Text="Vehicles" GroupName="ResourceTypeGroup" AutoPostBack="true" />
                <asp:RadioButton ID="DriversRadioButton" runat="server" Text="Drivers"  GroupName="ResourceTypeGroup" AutoPostBack="true" />
            </p>
            <p>
                <asp:CheckBox ID="cbOnlyOrphans" runat ="server" Text="In the left-hand pane, only show Resources which are not currently in any Group." AutoPostBack="true" />
            </p>
            <p>
            The tree on the far left represents your organisation's structure. You can edit the tree by right-clicking items on it and selecting from the pop-up menu.
            </p>
            <p>
            When you have finished editing the tree, drag and drop items from the list on to the appropiate tree item. You can drag and drop multiple items by holding down the Ctrl keyboard button while you are selecting them.
            </p>
            <p>
            Click the Save Changes button to keep the changes you have made (changes will be lost if you choose a different item from the menu or change the Vehicles or Drivers filter).
            </p>
            <p>* = the vehicle has an associated GPS Unit Identity.</p>
        </div>
        </telerik:RadAjaxPanel>
    <div class="buttonbar" style="clear:left;">
        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" Width="100" />
        <div runat="server" class="errorPanel" id="errorText" visible = "false"></div>

    </div>
    </div>
   
</asp:Content>