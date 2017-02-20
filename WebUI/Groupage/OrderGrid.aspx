<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.OrderGrid"  MasterPageFile="~/default_tableless.master" Title="Groupage - Orders" Codebehind="OrderGrid.aspx.cs" %>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadWindowManager ID="rmwFlagForInvoicing" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>
    <h1>Groupage Orders</h1>
    <h2></h2>
     <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr valign="top">
                <td>
                     <table>
                        <tr>
                            <td class="formCellLabel">Order Status</td>
                            <td class="formCellField"><asp:checkboxlist runat="server" id="cblOrderStatus" repeatdirection="horizontal" ></asp:checkboxlist></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Planned For Delivery</td>
                            <td class="formCellField" style="padding-left: 3px;"><asp:checkbox id="chkPlannedForDelivery" text="Planned For Delivery" runat="server"></asp:checkbox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Planned For Collection</td>
                            <td class="formCellField" style="padding-left: 3px;"><asp:checkbox id="chkPlannedForCollection" text="Planned For Collection" runat="server"></asp:checkbox></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Order Actions</td>
                            <td class="formCellField" style="padding-left: 3px;">
                                <asp:checkbox id="chkCrossDock" runat="server" text="Cross Docked"></asp:checkbox>
                                <asp:checkbox id="chkTransShipped" runat="server" text="Cross Shipment"></asp:checkbox>
                                <asp:checkbox id="chkLeaveOnTrailer" runat="server" text="Leave On Trailer"></asp:checkbox>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Business Type</td>
                            <td class="formCellField">
                                <asp:CheckBoxList ID="cboBusinessType" runat="server" DataValueField="BusinessTypeID" DataTextField="Description" RepeatDirection="Horizontal"></asp:CheckBoxList>
                            </td>
                        </tr>
                    </table>
                </td>
                <td style="border-left:solid 1pt black;" runat="server" id="tdDateOptions" visible="false">
                    <table>
                    <tr>
                        <td class="formCellLabel">Date From</td>
                        <td class="formCellField"><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Date To</td>
                        <td class="formCellField"><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                        <td class="formCellField"></td>
                    </tr>
                   </table>
                </td>
            </tr>
        </table>
    </fieldset>        
    
    <div class="buttonBar">
        <asp:button id="btnRefresh" runat="server" text="Refresh" />    
    </div>
    
    <div style="text-align:right;">
        <span id="dvSelected" style="background-color:#3d3d3d; color:white;padding-left:3px;padding-right:3px; font-weight:bold;height:20px; padding-top:3px;">Number of Orders : <asp:label id="lblOrderCount" runat="server"></asp:label></span>
    </div>
    <telerik:RadGrid runat="server" ID="gvOrders2" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView Width="100%" ClientDataKeyNames="OrderID,NoPallets" DataKeyNames="OrderID">
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign=Left HeaderText="" ></telerik:GridClientSelectColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Order No" SortExpression="CustomerOrderNumber">
                    <ItemTemplate>
                        <a href="javascript:ViewOrderProfile(<%#((System.Data.DataRowView)Container.DataItem)["OrderID"] %>);"><%#((System.Data.DataRowView)Container.DataItem)["CustomerOrderNumber"] %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" DataField="CollectionPointDescription"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" DataField="DeliveryPointDescription"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Last Planned Action" DataField="LastPlannedAction"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" />
                <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" />
                <telerik:GridBoundColumn HeaderText="Pallet Type" SortExpression="PalletTypeDescription" DataField="PalletTypeDescription" />
                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight">
                    <ItemTemplate>
                        <%# ((decimal)((System.Data.DataRowView)Container.DataItem)["Weight"]).ToString("F0")%>
                        <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                    </ItemTemplate>
               </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false"/>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
            <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" />
        </ClientSettings>
        
    </telerik:RadGrid>

    <div class="buttonBar">
        <input type="button" id="btnAddOrder" runat="server" value="Add Order" onclick="javascript: AddOrder();" />
        <asp:Button id="btnConfirmOrders" runat="server" Text="Confirm Orders" visible="false" />
    </div>
    
    <script type="text/javascript" language="javascript">
        var totalWeight = 0;
        var totalPallets = 0;
        function RowSelected(sender, eventArgs) {
            totalPallets += parseFloat(eventArgs.getDataKeyValue("NoPallets"));
        }

        function RowDeSelected(sender, eventArgs)
        {

            totalPallets -= parseFloat(eventArgs.getDataKeyValue("NoPallets"));
        }
        
        function showTotals()
        {
           var el = document.getElementById("dvSelected"); 
           el.innerHtml = "<b>Pallets : " + totalPallets + "</b>";
        }
        
        
    </script>
    <script language="javascript" type="text/javascript">
    function ViewOrderProfile(orderID)
    {
        var url = "ManageOrder.aspx";
            url += "?oID=" + orderID;
        
        var wnd = window.radopen("about:blank", "largeWindow");                               
        wnd.SetUrl(url);
        wnd.SetTitle("Add/Update Order");
    }
    </script>
</asp:Content>
