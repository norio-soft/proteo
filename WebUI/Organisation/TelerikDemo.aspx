<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="TelerikDemo.aspx.cs" Inherits="Orchestrator.WebUI.Organisation.TelerikDemo" EnableViewState="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Telerik Demo</h1></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <telerik:RadCodeBlock runat="server" ID="RadCodeBlock1">

        <script type="text/javascript">
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

            //select the hovered tree node if the user is dragging a listbox item
            function onTreeViewMouseOver(sender, args) {
                if (listBoxDragInProgress) {
                    args.get_node().select();
                }
            }

            //unselect the item if the user is dragging a node
            function onListBoxMouseOut(sender, args) {
                if (treeViewDragInProgress) {
                    args.get_item().unselect();
                }
            }

            //unselect the node if the user is dragging a listbox item
            function onTreeViewMouseOut(sender, args) {
                if (listBoxDragInProgress) {
                    args.get_node().unselect();
                }
            }

            //indicate that the user started dragging a listbox item
            function onListBoxDragStart(sender, args) {
                listBoxDragInProgress = true;
            }

            //indicate that the user started dragging a tree node
            function onTreeViewDragStart(sender, args) {
                treeViewDragInProgress = true;
            }

            //handle the drop of the listbox item
            function onListBoxDropping(sender, args) {
                //indicate that the user stopped dragging
                listBoxDragInProgress = false;
                document.body.style.cursor = "";
                //restore the cursor to the default state
                document.body.style.cursor = "";

                //get the html element on which the item is dropped
                var target = args.get_htmlElement();

                //if dropped on the listbox itself return.
                if (isOverElement(target, "<%= RadListBox1.ClientID %>")) {
                    return;
                }
                //check if dropped on the treeview
                var target = isOverElement(target, "<%= RadTreeView1.ClientID %>");

                //if not cancel the dropping event so it does not postback
                if (!target) {
                    args.set_cancel(true);
                    return;
                }

                //the item was dropped on the treeview - set the htmlElement. 
                //it can be later accessed via the HtmlElementID property of the RadListBox
                args.set_htmlElement(target);
            }

            //handle the drop of the tree node
            function onTreeViewDropping(sender, args) {
                //indicate that the user stopped dragging
                treeViewDragInProgress = false;

                //restore the cursor to the default state
                document.body.style.cursor = "";

                //get the html element on which the node is dropped
                var target = args.get_htmlElement();

                //if dropped on the treeview itself return.
                if (isOverElement(target, "<%= RadTreeView1.ClientID %>")) {
                    return;
                }
                //check if dropped on the listbox
                var target = isOverElement(target, "<%= RadListBox1.ClientID %>");

                //if not cancel the dropping event so it does not postback
                if (!target) {
                    args.set_cancel(true);
                    return;
                }

                //the node was dropped on the listbox - set the htmlElement. 
                //it can be later accessed via the HtmlElementID property of the RadTreeNodeDragDropEventArgs
                args.set_htmlElement(target);
            }

            //chech if a given html element is a child of an element with the specified id
            function isOverElement(target, id) {
                while (target) {
                    if (target.id == id)
                        break;

                    target = target.parentNode;
                }
                return target;
            }

            function checkDropTargets(target) {
                if (isOverElement(target, "<%= RadListBox1.ClientID %>") || isOverElement(target, "<%= RadTreeView1.ClientID %>")) {
                    //if the mouse is over the treeview or listbox - set the cursor to default
                    document.body.style.cursor = "";
                } else {
                    //else set the cursor to "no-drop" to indicate that dropping over this html element is not supported
                    document.body.style.cursor = "no-drop";
                }
            }

            //update the cursor if the user is dragging the item over supported drop target - listbox or treeview
            function onListBoxDragging(sender, args) {
                checkDropTargets(args.get_htmlElement());
            }

            //update the cursor if the user is dragging the node over supported drop target - listbox or treeview
            function onTreeViewDragging(sender, args) {
                checkDropTargets(args.get_htmlElement());
            }
        </script>

    </telerik:RadCodeBlock>

    <div class="example-panel">
        <telerik:RadAjaxLoadingPanel runat="server" ID="RadAjaxLoadingPanel1" />
        <telerik:RadAjaxManager runat="server" ID="RadAjaxManager1">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="RadListBox1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadListBox1" LoadingPanelID="RadAjaxLoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="RadTreeView1" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="RadTreeView1">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="RadListBox1" LoadingPanelID="RadAjaxLoadingPanel1" />
                        <telerik:AjaxUpdatedControl ControlID="RadTreeView1" LoadingPanelID="RadAjaxLoadingPanel1" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
        <telerik:RadListBox runat="server" ID="RadListBox1" EnableDragAndDrop="True" AllowReorder="true" Height="200px"
            OnClientDropping="onListBoxDropping" OnClientDragStart="onListBoxDragStart" OnClientMouseOver="onListBoxMouseOver"
            OnClientMouseOut="onListBoxMouseOut" OnClientDragging="onListBoxDragging" 
            OnDropped="RadListBox1_ItemDropped">
            <ButtonSettings ShowReorder="false" />
            <Items>
                <telerik:RadListBoxItem Text="Birthday present" />
                <telerik:RadListBoxItem Text="Dry-cleaners" />
                <telerik:RadListBoxItem Text="New tennis racket" />
                <telerik:RadListBoxItem Text="Water the plants" />
            </Items>
        </telerik:RadListBox>
        <telerik:RadTreeView runat="server" ID="RadTreeView1" EnableDragAndDrop="True" OnClientMouseOver="onTreeViewMouseOver"
            OnClientMouseOut="onTreeViewMouseOut" OnClientNodeDragStart="onTreeViewDragStart"
            OnClientNodeDropping="onTreeViewDropping" OnNodeDrop="RadTreeView1_NodeDrop"
            OnClientNodeDragging="onTreeViewDragging">
            <Nodes>
                <telerik:RadTreeNode Text="High priority" AllowDrag="false" Expanded="True" ImageUrl="Images/high.png">
                    <Nodes>
                        <telerik:RadTreeNode Text="Fill tax papers" />
                    </Nodes>
                </telerik:RadTreeNode>
                <telerik:RadTreeNode Text="Normal priority" AllowDrag="false" Expanded="True" ImageUrl="Images/normal.png">
                    <Nodes>
                        <telerik:RadTreeNode Text="Wash the car" />
                    </Nodes>
                </telerik:RadTreeNode>
                <telerik:RadTreeNode Text="Low priority" AllowDrag="false" Expanded="True" ImageUrl="Images/low.png">
                    <Nodes>
                        <telerik:RadTreeNode Text="Paintball with friends" />
                    </Nodes>
                </telerik:RadTreeNode>
            </Nodes>
        </telerik:RadTreeView>
    </div>

</asp:Content>
