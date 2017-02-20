<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.Deliveries" EnableViewStateMac="false" MasterPageFile="~/default_tableless.Master" Title="Groupage - Deliveries" Codebehind="Deliveries.aspx.cs" %>


<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" language="javascript" src="../script/tooltippopups.js"></script>
      <script type="text/javascript">
        var orders = "";
        var groupHandlingIsActive = false;
        
        function HandleGroupSelection(orderGroupID)
        {
            if (groupHandlingIsActive)
                return;
                
            groupHandlingIsActive = true;
            
            var mtv= $find("<%=grdDeliveries.ClientID %>").get_masterTableView();
            var dataItems = mtv.get_dataItems();
            
            for (var rowIndex = 0; rowIndex < dataItems.length; rowIndex++)
            {
                try
                {
                    if (dataItems[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID)
                    {
                        var chkOrderID = dataItems[rowIndex].get_element().childNodes[1].childNodes[0];
                        // If the checkbox has been found, and is not selected - tick the checkbox.
                        if (chkOrderID != null && !chkOrderID.checked)
                            chkOrderID.click();
                    }
                }
                catch (error)
                {
                }
            }

            groupHandlingIsActive = false;
        }
        
        function HandleGroupDeselection(orderGroupID)
        {
            if (groupHandlingIsActive)
                return;
                
            groupHandlingIsActive = true;
            
            var mtv= $find("<%=grdDeliveries.ClientID %>").get_masterTableView();
            var dataItems = mtv.get_dataItems();
            
            for (var rowIndex = 0; rowIndex < dataItems.length; rowIndex++)
            {
                try
                {
                    if (dataItems[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID)
                    {
                        var chkOrderID = dataItems[rowIndex].get_element().childNodes[1].childNodes[0];
                        // If the checkbox has been found, and is selected - untick the checkbox.
                        if (chkOrderID != null && chkOrderID.checked)
                            chkOrderID.click();
                    }
                }
                catch (error)
                {
                }
            }

            groupHandlingIsActive = false;
        }

        
        function ChangeList(e, src, orderID, noPallets, orderGroupID, orderGroupGroupedPlanning, palletSpaces, weight)
        {

            
            var gridRow;

            if(e.target)
            {
                gridRow = e.target.parentNode.parentNode;
            }
            else
            {
                gridRow = e.srcElement.parentNode.parentNode;
            }
            
            if (src.checked)
            {
                // Add to the list
                if(orders.length > 0)
                orders += ",";
                orders += orderID; 
                gridRow.className= "SelectedRow_Orchestrator";

                RowSelected(noPallets, palletSpaces, weight);
                
                // Is the order part of a group that is grouped planning enabled?
                // Automatically select the other orders in the grid that belong to this group.
                if (orderGroupID > 0 && orderGroupGroupedPlanning)
                {
                    HandleGroupSelection(orderGroupID);
                }
            }
            else
            {
                // remove from the list
                orders = orders.replace(orderID + ",", "");
                orders = orders.replace("," + orderID, "");
                orders = orders.replace(orderID, "");
                gridRow.className= "GridRow_Orchestrator";

                RowDeSelected(noPallets, palletSpaces, weight);
                
                // Is the order part of a group that is grouped planning enabled?
                // Prompt to see if the user wishes to uncheck those orders also.
                if (orderGroupID > 0 && orderGroupGroupedPlanning)
                {
                    HandleGroupDeselection(orderGroupID);
                }
            }
        }
     
     function doFilter(sender, e)
     {
       if(e.keyCode == 13)
       {
         var btn = document.getElementById('<%= grdDeliveries.MasterTableView.GetItems(Telerik.Web.UI.GridItemType.CommandItem)[0].FindControl("btnSearch").ClientID %>');

          if(btn != null)
         {
            e.cancelBubble = true;
            e.returnValue = false;

             if (e.preventDefault)
            {
                e.preventDefault();
            }

            btn.click();
        }
       }
     }
     
    var _orderID = 0;
    function RowContextMenu(sender, eventArgs)
    {
        var menu = $find("<%=RadMenu1.ClientID %>");
        var evt = eventArgs.get_domEvent();

        var index = eventArgs.get_itemIndexHierarchical();
        document.getElementById("radGridClickedRowIndex").value = index;

        menu.show(evt);

        evt.cancelBubble = true;
        evt.returnValue = false;

        if (evt.stopPropagation) {
            evt.stopPropagation();
            evt.preventDefault();
        }

        sender.get_masterTableView().selectItem(sender.get_masterTableView().get_dataItems()[index].get_element(), true);
        _orderID = sender.get_masterTableView().get_dataItems()[index].getDataKeyValue("OrderID");
    }
    
    function OnClick(sender, eventArgs)
    {
        var mnuCall = eventArgs.get_item().get_value();
        eventArgs.get_item().get_menu().hide();

        var businessTypeID = mnuCall;
        var returnURL = location.href;

        if (orders == "")
            orders = _orderID;
            
        if (orders.indexOf(",") == -1 || confirm("This will change the business type for multiple orders, are you sure?"))
            location.href = "changebusinesstype.aspx?oID=" + orders + "&BT=" + businessTypeID + "&returnUrl=" + returnURL;
    }
    </script>
    <style type="text/css">
        .masterpage_layout 
        {
            width: 1700px;
        }
    </style>
        <telerik:RadWindowManager ID="rmwDeliveries" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
            <Windows>
                <telerik:RadWindow runat="server" ID="largeWindow" Height="650" Width="970" />
            </Windows>
        </telerik:RadWindowManager>
      
    <h1>Deliveries Filters</h1>
    <h2>
        You can view all of the orders that you need to plan into delivery jobs. You can choose which orders to see by delivery date and then you can further filter this view by entering information in the Filter text box at the top of the table of orders.
    </h2>
    <fieldset class="collapsingFieldset">
        <legend>Filter Options</legend>
        <div class="collapsingFieldsetInner">
            <table>
                <tr>
                    <td class="formCellLabel">Date From</td>
                    <td colspan="3" align="left">
                        <table>
                            <tr>
                                <td class="formCellField"><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField"><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                                <td class="formCellLabel">Date Filters on</td>
                                <td class="formCellField" colspan="3">
                                <asp:RadioButtonList ID="rblDateFiltering" runat="server" RepeatDirection="Horizontal"></asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Delivery Area</td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBox ID="chkSelectAllTrafficAreas" runat="server" Text="Select all Traffic Areas" style="padding-left:3px;" />
                        <asp:CheckBoxList ID="cblTrafficAreas" runat="server" DataValueField="TrafficAreaID" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="8"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Business Type</td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBoxList ID="cblBusinessTypes" runat="server" DataValueField="BusinessTypeID" DataTextField="Description" RepeatDirection="Horizontal"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Service Level</td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBoxList ID="cblServiceLevel" runat="server" DataValueField="OrderServiceLevelID" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Surcharges</td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBoxList id="cblSurcharges" runat="server" DataValueField="ExtraTypeID" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="7"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Booked In</td>
                    <td class="formCellField" colspan="3">
                        <asp:CheckBox id="chkBookedInYes" Text="Yes" Checked="true" runat="server"></asp:CheckBox>
                        <asp:CheckBox id="chkBookedInNo" Text="No" Checked="true" runat="server"></asp:CheckBox>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Trunk Date From</td>
                    <td class="formCellField"><asp:CheckBox id="chkNoTrunkDate" Text="Show rows with no trunk date" Checked="true" runat="server"></asp:CheckBox></td>
                </tr>
            </table>
        </div>
    </fieldset>
    <div class="buttonbar">            
        <asp:button id="btnRefresh" runat="server" text="Refresh" Width="75" />
    </div>
    <div style="text-align:left;">
        <span id="Span1" style="background-color:Purple; color:white;padding-left:3px;padding-right:3px; font-weight:bold;height:20px; padding-top:3px; padding-bottom:3px;">Exported</span>
    </div>
    <div class="overlayedDataBox">
        <h1>Load</h1>
        <span id="dvSelected">Number of Pallets : 0</span>
        <div class="buttonbar" style="margin-bottom:1px;margin-top:3px;"><asp:button ID="btnCreateJobShortcut" runat="server" Text="Create Delivery Job" style=" font-size:11px;" PostBackUrl=<%="deliveryjob.aspx?" + this.CookieSessionID  %> /></div>
    </div>  
        <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
        <telerik:RadGrid runat="server" ID="grdDeliveries" AllowPaging="false" AllowSorting="true" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true"  >
            <MasterTableView Width="100%" DataKeyNames="Weight,PalletSpaces" ClientDataKeyNames="OrderID,OrderGroupID,OrderGroupGroupedPlanning" CommandItemDisplay="Top" GroupLoadMode="Client">
                 <GroupByExpressions>
                    <telerik:GridGroupByExpression>
                        <GroupByFields>
                            <telerik:GridGroupByField FieldName="TrafficAreaShortName" />
                            
                        </GroupByFields>
                        <SelectFields>
                            <telerik:GridGroupByField FieldName="TrafficAreaShortName" FieldAlias="TrafficAreaShortName"  />
                            <telerik:GridGroupByField FieldName="OrderID" Aggregate="Count" FieldAlias="Count" />
                        </SelectFields>
                    </telerik:GridGroupByExpression>
                </GroupByExpressions>
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <CommandItemTemplate>
                    <div style="text-align:right;">
                        <asp:Label ID="Label50" runat="server" Text="Filter:"></asp:Label>
                           <asp:DropDownList ID="cboField" runat="server"><asp:ListItem Text="Town" Selected=True Value="Town" /><asp:ListItem Text="Point Name" Value="CollectionPointDescription" /></asp:DropDownList>
                        <asp:TextBox ID="txtSearch" onkeypress="doFilter(this, event, true)" runat="server"></asp:TextBox> 
                        <asp:Button ID="btnSearch" runat="server" CommandName="search" Text="Filter"  Width="75"/> 
                        <asp:Button ID="btnShowAll" runat="server" CommandName="showall" Text="Show All"  Width="75"/> 
                    </div>
                </CommandItemTemplate>   
                
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="chkSelectColumn" HeaderStyle-Width="25">
                        <ItemTemplate>
                            <asp:CheckBox id="chkOrderID" runat="server"  />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Post Code" SortExpression="DeliveryPostCode" UniqueName="DeliveryPostCode" DataField="DeliveryPostCode" ItemStyle-Wrap="false" ></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="ID" SortExpression="OrderID" UniqueName="OrderID" HeaderStyle-Width="60">
                        <ItemTemplate>
                            <span>
                                <a onclick='javascript:viewOrderProfile(<%# ((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString() %>);'
                                    target="_blank" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                    <%#((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString()%>
                                </a>
                                <img style="padding-left:2px;" runat="server" id="imgOrderBookedIn" src="/App_Themes/Orchestrator/Img/MasterPage/icon-tick-small.png" alt="Booked In" />
                            </span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Trunk Date" DataField="TrunkDate" SortExpression="TrunkDate" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Collect From" SortExpression="CollectionPointDescription">
                        <ItemTemplate>
                             <span id="spnCollectionPOint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryRunCollectionPointID"].ToString() %>);" onMouseOut="closeToolTip();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="DeliveryRunCollectionDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false">
                        <ItemTemplate>
                             <asp:Label runat="server" ID="lblCollectAt"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription" DataField="BusinessTypeDescription" />
                    <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                        <ItemTemplate>
                             <span id="spnDeliveryPOint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="closeToolTip();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                     <telerik:GridTemplateColumn HeaderText="Town" SortExpression="Town" >
                        <ItemTemplate>
                            <b> <%# (string)((System.Data.DataRowView)Container.DataItem)["Town"]%></b>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                        <ItemTemplate>
                             <asp:Label runat="server" ID="lblDeliverAt"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="Rate" HeaderStyle-Width="80">
                        <ItemTemplate>
                            <%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["Rate"].ToString()).ToString("C")%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="40" UniqueName="NoPallets" />
                    <telerik:GridTemplateColumn HeaderText="Pallet Spaces" UniqueName="NoPalletSpaces">
                        <ItemTemplate>
                            <span id="spnPalletSpaces" runat="server" ><%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["PalletSpaces"].ToString()).ToString("0.##") %></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80" UniqueName="Weight">
                        <ItemTemplate>
                            <%# decimal.Parse(Eval("Weight").ToString()).ToString("F0")%>
                            <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                    <telerik:GridTemplateColumn HeaderText="Surcharges">
                        <ItemTemplate>
                            <asp:Label ID="lblSurcharge" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Delivery Notes" DataField="DeliveryNotes" ItemStyle-Wrap="true" HeaderStyle-Width="200" />
                    <telerik:GridBoundColumn HeaderText="Collection Notes" DataField="CollectionNotes" ItemStyle-Wrap="true" HeaderStyle-Width="200" />
                    <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="true" HeaderStyle-Width="200" />
                    <telerik:GridBoundColumn DataField="Weight" Visible="false" UniqueName="_weight"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="PalletSpaces" Visible="false" UniqueName="_palletspaces"></telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" AllowGroupExpandCollapse="true" >
                <Resizing AllowColumnResize="true" AllowRowResize="false" />
                <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" OnRowContextMenu="RowContextMenu"/>
            </ClientSettings>
            
        </telerik:RadGrid>
         <telerik:RadContextMenu ID="RadMenu1" IsContext="True" runat="server" Skin="Outlook" OnClientItemClicked="OnClick" ContextMenuElementID="none"></telerik:RadContextMenu>

        
        <div class="buttonbar">
            <asp:Button ID="btnExportOrder" runat="server" Text="Export" 
                onclick="btnExportOrder_Click" />
            <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Generate Delivery Note" />            
            <asp:Button id="btnCreateDelivery" runat="server" Text="Create Delivery Job" PostBackUrl="deliveryjob.aspx" /><span style="width:50px;">&nbsp;</span><asp:Button ID="btnSaveGridSettings" runat="server" Text="Save Grid Layout" />
        </div>
    
    <script type="text/javascript" language="javascript">
        var totalWeight = <%=Weight.ToString() %>;
        var totalPallets = <%=NoPallets.ToString() %>;
        var totalSpaces = <%=NoPalletSpaces.ToString() %>;
        
            
        showTotals(false);
            
        function RowSelected(pallets, palletSpaces, weight)
        {
            totalPallets += parseInt(pallets);
            totalSpaces  += parseFloat(palletSpaces);
            totalWeight  += parseFloat(weight);
            showTotals(true);     
        }
        
        function RowDeSelected(pallets, palletSpaces, weight)
        {
            totalPallets -= parseInt(pallets);
            totalSpaces  -= parseFloat(palletSpaces);
            totalWeight  -= parseFloat(weight);
            showTotals(true);
        }
            
        
        function showTotals(changed)
        {
        
            var el = document.getElementById("dvSelected"); 
            el.innerHTML = "Number of Pallets : " + totalPallets ;
            el.innerHTML += "<br/>Number of Pallet Spaces : " + totalSpaces;
            el.innerHTML += "<br/>Total weight: " + totalWeight;

            if (changed)
            {
                document.getElementById("<%=btnCreateDelivery.ClientID%>").disabled=false;
                document.getElementById("<%=btnCreateDeliveryNote.ClientID%>").disabled=false;
            }
        }
        
        function AddOrder()
        {
            location.href="manageorder.aspx";
        }

        function viewOrderProfile(orderID)
        {
            var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;
            
            var wnd = window.radopen("about:blank", "largeWindow");                               
            wnd.SetUrl(url);
            wnd.SetTitle("Add/Update Order");
        }
        
        $(document).ready(function() {
            $(":checkbox[id*='chkSelectAllTrafficAreas']").click(function() {
                var checked_status = this.checked;
                $(":checkbox[id*='cblTrafficAreas']").each(function() {
                    this.checked = checked_status;

                });
            });
        });

    </script>
</asp:Content>
