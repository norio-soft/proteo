<%@ Control Language="C#" AutoEventWireup="true" Inherits=" Orchestrator.WebUI.orders" Codebehind="orders.ascx.cs" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>


<style type="text/css">
.collapsePanel {
	
	background-color:white;
	overflow:hidden;
}

.collapsePanelHeader{	
	width:100%;		
	height:22px; 
	border-bottom:solid 1pt silver;
	color:#ffffff; 
	background-color:#196AAC;
	cursor: pointer; 
	font-family:verdana;
	font-size:11px;
	font-weight:bold;
	vertical-align:middle;
}
.FilterMenuClass
 {
    z-index:1000;
    text-wrap:true;
 }      

</style>
 

    <asp:Panel ID="pnlOptionsHeader" runat="server" CssClass="collapsePanelHeader"> 
            <div style="float: left; padding:3px; ">Filter Options</div>
            <div style="float: left; margin-left: 20px;padding:3px;">
                <asp:Label ID="Label1" runat="server">(Click to Show Details...)</asp:Label>
            </div>
            <div style="float: right; vertical-align: middle;padding:3px;">
                <asp:Image ID="Image1" runat="server" ImageUrl="~/images/expand.jpg"/>
            </div>
    </asp:Panel>
   
    <asp:Panel ID="pnlOptionsContent" runat="server" Height="0" CssClass="collapsePanel" >
        <table>
            <tr valign="top">
                <td width="600">
                     <table>
                        <tr>
                            <td>Order Status</td>
                            <td><asp:checkboxlist runat="server" id="cblOrderStatus" repeatdirection="horizontal" ></asp:checkboxlist></td>
                        </tr>
                        <tr>
                            <td>Planned For Delivery</td>
                            <td><asp:checkbox id="chkPlannedForDelivery" text="Planned For Delivery" runat="server"></asp:checkbox></td>
                        </tr>
                        <tr>
                            <td>Planned For Collection</td>
                            <td><asp:checkbox id="chkPlannedForCollection" text="Planned For Collection" runat="server"></asp:checkbox></td>
                        </tr>
                        <tr>
                            <td>Order Actions</td>
                            <td>
                                <asp:checkbox id="chkCrossDock" runat="server" text="Cross Docked"></asp:checkbox>
                                <asp:checkbox id="chkTransShipped" runat="server" text="Cross Shipment"></asp:checkbox>
                                <asp:checkbox id="chkLeaveOnTrailer" runat="server" text="Leave On Trailer"></asp:checkbox>
                            </td>
                        </tr>
                    </table>
                </td>
                <asp:Panel ID="pnlDateFilter" runat="server" Visible="false">
                <td style="border-left:solid 1pt black;">
                    <table>
                    <tr>
                        <td>Date From</td>
                        <td><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                    </tr>
                    <tr>
                        <td>Date To</td>
                        <td><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                        <td></td>
                    </tr>
                   </table>
                </td>
                </asp:Panel>
                </tr>
        </table>
        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">
            <asp:button id="btnRefresh" runat="server" text="Refresh" CausesValidation="False" />    
        </div>
    </asp:Panel>
        
     
    <div style="text-align:right;padding-top:5px;">
        <span id="dvSelected" style="display:none; background-color:#5D7B9D; color:white;padding-left:3px;padding-right:3px; font-weight:bold;height:20px; padding-top:3px;">Number of Orders : <asp:label id="lblOrderCount" runat="server"></asp:label></span>
    </div>
    <telerik:RadGrid runat="server" ID="gvOrders2" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" AllowFilteringByColumn="true" AllowAutomaticInserts="false" >
        <MasterTableView DataKeyNames="OrderID" AllowFilteringByColumn="true"  >
           <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign=Left HeaderStyle-Width="40" HeaderText=""  ></telerik:GridClientSelectColumn>
                <telerik:GridHyperLinkColumn HeaderText="Client" SortExpression="CustomerOrganisationName" UniqueName="CustomerOrganisationName" DataTextField="CustomerOrganisationName" DataNavigateUrlFormatString="javascript:viewOrderProfile({0});" DataNavigateUrlFields="OrderID"></telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"  AllowFiltering="false"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel"  AllowFiltering="true"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Last Planned Action" SortExpression="LastPlannedOrderAction" DataField="LastPlannedOrderAction"  AllowFiltering="false"></telerik:GridBoundColumn>                
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" DataField="CollectionPointDescription" ItemStyle-Wrap="false" AutoPostBackOnFilter="true" UniqueName="CollectionPointDescription"> </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime" AllowFiltering="false">
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" DataField="DeliveryPointDescription" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false" AllowFiltering="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="DON" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription"  AllowFiltering="false"/>
                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60"  AllowFiltering="false"/>
                <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60"  AllowFiltering="false"/>
                <telerik:GridBoundColumn HeaderText="Pallet Type" SortExpression="PalletTypeDescription" DataField="PalletTypeDescription" HeaderStyle-Width="80"  AllowFiltering="false" Visible="false"/>
                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80" AllowFiltering="false">
                    <ItemTemplate>
                        <%# ((decimal)((System.Data.DataRowView)Container.DataItem)["Weight"]).ToString("F4")%>
                        <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                    </ItemTemplate>
               </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false"  AllowFiltering="false" Visible="false"/>
            </Columns>            
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <ClientEvents OnRowSelecting="Orders2RowSelecting" OnRowDeselecting="Orders2RowDeselecting" OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" />
        </ClientSettings>
        <FilterMenu CssClass="FilterMenuClass" ></FilterMenu>
    </telerik:RadGrid>

    <div class="buttonbar">
        <input type="button" id="btnAddOrder" runat="server" value="Add Order" onclick="javascript:AddOrder()"; />
        <asp:Button id="btnSelect" runat="server" Text="Select Orders" />
        <asp:Button id="btnClearFilters" runat="server" Text="Clear Filters" />
    </div>
   
        <cc1:collapsiblepanelextender id="CollapsiblePanelExtender1" runat="server" TargetControlID="pnlOptionsContent"
        ExpandControlID="pnlOptionsHeader"
        CollapsedSize="0"
        CollapseControlID="pnlOptionsHeader" 
        Collapsed="true"
        TextLabelID="Label1"
        ExpandedText="(Click to Hide Details...)"
        CollapsedText="(Click to Show Details...)"
        ImageControlID="Image1"
        ExpandedImage="~/images/collapse.jpg"
        CollapsedImage="~/images/expand.jpg"
        SuppressPostBack="true" />
       
    <script type="text/javascript" language="javascript">
        var totalWeight = 0;
        var totalPallets = 0;
        function RowSelected(row)
        {
            totalPallets += row.KeyValues["NoPallets"];
        }
        
        function RowDeSelected(row)
        {
            
            totalPallets -= row.KeyValues["NoPallets"];
        }

        function Orders2RowSelecting(rowObject)
        {
            var cell = this.GetCellByColumnUniqueName(rowObject, "checkboxSelectColumn");
            return RowSelectingHelper(cell);
        }

        function Orders2RowDeselecting(rowObject)
        {
            var cell = this.GetCellByColumnUniqueName(rowObject, "checkboxSelectColumn");
            return RowDeselectingHelper(cell);
        }
        
        function showTotals()
        {
           var el = document.getElementById("dvSelected"); 
           el.innerHtml = "<b>Pallets : " + totalPallets + "</b>";
        }
    </script>
     <script language="javascript" type="text/javascript">
        function viewOrderProfile(orderID)
        {
            var oManager = GetRadWindowManager();
               
            var oWnd = oManager.GetWindowByName("singleton");
            var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;
                
            oWnd.SetUrl(url);
            oWnd.SetTitle("Add/Update Order");
            oWnd.Show();
        }
      </script>