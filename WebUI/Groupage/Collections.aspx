<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.Collections" MasterPageFile="~/default_tableless.Master" Title="Groupage - Collection Run" EnableEventValidation="false" Codebehind="Collections.aspx.cs" %>


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
            
                var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
                var rows = mtv.get_dataItems();
                
                for (var rowIndex = 0; rowIndex < rows.length; rowIndex++)
                    try
                    {
                        if (rows[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID)
                        {
                            var gridRowTable = rows[rowIndex].get_owner();
                            var cell = gridRowTable.getCellByColumnUniqueName(rows[rowIndex], "chkSelectColumn"); 
                            if(!cell) return;
                        
                            var chkOrderID = GetCheckBox(cell);
                            if(!chkOrderID) return;
                            
                            chkOrderID.checked = true;
                        }
                    }
                    catch (error)
                    {
                    }

            groupHandlingIsActive = false;
        }
        
        function GetCheckBox(control) { 

            if (!control) return; 

            for (var i = 0; i < control.childNodes.length; i++) { 
                if (!control.childNodes[i].tagName) continue; 
                    if ((control.childNodes[i].tagName.toLowerCase() == "input") && 
                    (control.childNodes[i].type.toLowerCase() == "checkbox")) { 
                        return control.childNodes[i]; 
                    } 
            } 
        } 
        
        function HandleGroupDeselection(orderGroupID)
        {
            if (groupHandlingIsActive)
                return;
                
            groupHandlingIsActive = true;
            
            if (confirm("Do you wish to also deselect other orders in this order's group?"))
            {
                var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
                var rows = mtv.get_dataItems();
                
                for (var rowIndex = 0; rowIndex < rows.length; rowIndex++)
                    try
                    {
                        if (rows[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID)
                        {
                            var gridRowTable = rows[rowIndex].get_owner();
                            var cell = gridRowTable.getCellByColumnUniqueName(rows[rowIndex], "chkSelectColumn"); 
                            if(!cell) return;
                        
                            var chkOrderID = GetCheckBox(cell);
                            if(!chkOrderID) return;
                            
                            chkOrderID.checked = false;
                        }
                    }
                    catch (error)
                    {
                    }
            }

            groupHandlingIsActive = false;
        }
        
        function ChangeList(e, src, orderID, noPallets, orderGroupID, orderGroupGroupedPlanning)
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
            
                RowSelected(noPallets);
                
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

                RowDeSelected(noPallets);
                
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
         var btn = document.getElementById('<%= grdOrders.MasterTableView.GetItems(Telerik.Web.UI.GridItemType.CommandItem)[0].FindControl("btnSearch").ClientID %>');

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

        if (evt.stopPropagation)
        {
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
     <telerik:RadWindowManager ID="rmwCollections" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>
    <h1>Collections</h1>
    <h2></h2>
    <fieldset class="collapsingFieldset">
        <legend>Filter Options</legend>
        <div class="collapsingFieldsetInner">
            <table>
                <tr>
                    <td class="formCellLabel">Date From</td>
                    <td class="formCellField"><telerik:RadDateInput id="dteStartDate" runat="server" Width="75" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                    <td class="formCellLabel">Date To</td>
                    <td class="formCellField"><telerik:RadDateInput id="dteEndDate" runat="server" Width="75" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                    <td><asp:CheckBox ID="chkShowCrossDockedOnly" runat="server" Text="Only Show Orders that have been planned for Cross Docking." /></td>
                    <td><asp:CheckBox ID="chkShowUnPlannedOnly" runat="server" Text="Only Show Orders that have <b>NOT</b> been planned for collection." /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Collection Area</td>
                    <td class="formCellField" colspan="99">
                        <asp:CheckBox ID="chkSelectAllTrafficAreas" runat="server" Text="Select all Traffic Areas" style="padding-left:3px;" />
                        <asp:CheckBoxList ID="cblTrafficAreas" runat="server" DataValueField="TrafficAreaID" DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="9"></asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Business Type</td>
                    <td class="formCellField" colspan="99">
                        <asp:CheckBoxList ID="cblBusinessTypes" runat="server" DataValueField="BusinessTypeID" DataTextField="Description" RepeatDirection="Horizontal"></asp:CheckBoxList>
                    </td>
                </tr>
            </table>
        </div>
    </fieldset>
    <div class="buttonbar">            
        <asp:button id="btnRefresh" runat="server" text="Refresh" />
    </div>
    <div style="text-align:right;">
        <span id="dvSelected" style="background-color:#5D7B9D; color:white;padding-left:3px;padding-right:3px; font-weight:bold;height:20px; padding-top:3px;">Number of Pallets : 0</span>
    </div>
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
        
    <div class="buttonBar" id="div1" style="margin-top:10px;" runat="server">
       <asp:Button id="btnCreateJobTop" runat="server" Text="Create Collection Job" PostBackUrl="CollectionJob.aspx" disabled />
    </div>
       <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" >
        
        <MasterTableView Width="100%" ClientDataKeyNames="OrderID,OrderGroupID,OrderGroupGroupedPlanning" DataKeyNames="OrderID,OrderGroupID,OrderGroupGroupedPlanning" CommandItemDisplay="top" GroupLoadMode="Client" >
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
             <CommandItemTemplate>
                        <div style="float: right">
                            <asp:Label ID="Label50" runat="server" Text="Filter:"></asp:Label>
                               <asp:DropDownList ID="cboField" runat="server"><asp:ListItem Text="Town" Selected=True Value="Town" /><asp:ListItem Text="Point Name" Value="CollectionPointDescription" /></asp:DropDownList>
                            <asp:TextBox ID="txtSearch" onkeypress="doFilter(this, event, true)" runat="server"></asp:TextBox> 
                            <asp:Button ID="btnSearch" runat="server" CommandName="search" Text="Filter"  Width="75"/> 
                            <asp:Button ID="btnShowAll" runat="server" CommandName="showall" Text="Show All"  Width="75"/> 
                        </div>
                   </CommandItemTemplate>
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <DetailTables>
                <telerik:GridTableView ClientDataKeyNames="OrderID" DataKeyNames="OrderID"  AutoGenerateColumns="false"  >
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="OrderID" MasterKeyField="OrderID" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="D/O Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                        <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                        <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                    </Columns>
                </telerik:GridTableView>       
            </DetailTables>
            <Columns>
                
                <telerik:GridTemplateColumn UniqueName="chkSelectColumn"  ItemStyle-Width="18" HeaderStyle-Width="24">
                    <ItemTemplate>
                        <asp:CheckBox id="chkOrderID" runat="server"  />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="ID" SortExpression="OrderID" UniqueName="OrderID" HeaderStyle-Width="60">
                    <ItemTemplate>
                        <span>
                            <a onclick='javascript:viewOrderProfile(<%# ((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString() %>);'
                                target="_blank" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                <%#((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString()%>
                            </a>
                            <img style="padding-left:2px;" runat="server" alt="" id="imgOrderCollectionDeliveryNotes" src="~/images/postit_small.gif" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Collect From">
                    <ItemTemplate>
                         <span id="spnCollectionPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryPointID"].ToString() %>);" onMouseOut="closeToolTip();" class="orchestratorLink"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn HeaderText="Town" SortExpression="Town" >
                    <ItemTemplate>
                        <b> <%# (string)((System.Data.DataRowView)Container.DataItem)["Town"]%></b>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionRunDeliveryDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridHyperLinkColumn HeaderText="Client" SortExpression="CustomerOrganisationName" UniqueName="CustomerOrganisationName" DataTextField="CustomerOrganisationName" DataNavigateUrlFormatString="javascript:viewOrderProfile({0});" DataNavigateUrlFields="OrderID"></telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription" DataField="BusinessTypeDescription" />
                <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                    <ItemTemplate>
                         <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="closeToolTip();" class="orchestratorLink"><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Destination" SortExpression="Destination Point" HeaderStyle-Wrap="false" >
                    <ItemTemplate>
                        <span id="spnDestinationPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["Destination PointID"].ToString() %>);" onMouseOut="closeToolTip();" class="orchestratorLink"><%#((System.Data.DataRowView)Container.DataItem)["Destination Point"].ToString()%></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Last Planned Action" SortExpression="LastPlannedOrderAction" DataField="LastPlannedOrderAction" />                
                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="40" UniqueName="NoPallets" />
                <telerik:GridTemplateColumn HeaderText="Pallet Spaces" HeaderStyle-Width="50">
                    <ItemTemplate>
                        <span id="spnPalletSpaces" runat="server"><%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["PalletSpaces"].ToString()).ToString("0.##") %></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80">
                    <ItemTemplate>
                        <%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["Weight"].ToString()).ToString("N2")%>
                        <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>                      
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="true" HeaderStyle-Width="200" />
                <telerik:GridBoundColumn HeaderText="IsPlanned" Visible="false" DataField="PlannedForCollection" UniqueName="PlannedForCollection"/>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" AllowGroupExpandCollapse="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" OnRowContextMenu="RowContextMenu" />
        </ClientSettings>
        </telerik:RadGrid>
        <telerik:RadContextMenu ID="RadMenu1" IsContext="True" runat="server" OnClientItemClicked="OnClick" ContextMenuElementID="none">
        </telerik:RadContextMenu>
      
        <div class="buttonBar" id="divCreateCollectionButtonBar" style="margin-top:10px;" runat="server">
           <asp:Button id="btnCreateCollection" runat="server" Text="Create Collection Job" PostBackUrl="CollectionJob.aspx" disabled />
        </div>

     <script language="javascript" type="text/javascript">
            var totalWeight = 0;
            var totalPallets = <%=NoPallets.ToString() %>;
            
            if (totalPallets > 0)
                showTotals();
                
            function RowSelected(i)
            {
                totalPallets += parseInt(i);
                showTotals();
            }
            
            function RowDeSelected(i)
            {
                totalPallets -= parseInt(i);
                showTotals();
            }
            
            function showTotals()
            {
               var el = document.getElementById("dvSelected"); 
               el.innerHTML = "Number of Pallets : " + totalPallets ;
               document.getElementById("<%=btnCreateCollection.ClientID%>").disabled=false;
               $get("<%=btnCreateJobTop.ClientID %>").disabled = false;
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
