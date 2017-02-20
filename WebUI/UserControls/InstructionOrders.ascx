<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.UserControls.InstructionOrders" Codebehind="InstructionOrders.ascx.cs" %>
<script type="text/javascript" language="javascript" src='<%=this.ResolveUrl("~/script/tooltippopups.js") %>'></script>
<telerik:RadGrid runat="server" ID="gvOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" AllowFilteringByColumn="false" AllowAutomaticInserts="false">
    <MasterTableView Width="100%" DataKeyNames="OrderID" AllowFilteringByColumn="false" >
       <RowIndicatorColumn Display="false"></RowIndicatorColumn>
        <Columns>
            <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="40" HeaderText=""></telerik:GridClientSelectColumn>
            <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" UniqueName="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false" AllowFiltering="true" DataType="System.String" ></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" AllowFiltering="false" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <span id="spnCollectionPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%# ((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"]%></b></span>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime" AllowFiltering="false">
                <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Order Action" SortExpression="Order Action" DataField="OrderAction"></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="Take To" SortExpression="DeliveryPointDescription" AllowFiltering="false" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <span id="spnTakeToPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><%# ((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"]%></b></span>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Arrive At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false" AllowFiltering="false"  >
                <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
            <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription"  AllowFiltering="false"/>
            <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60"  AllowFiltering="false"/>
            <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60"  AllowFiltering="false"/>
            <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80" AllowFiltering="false">
                <ItemTemplate>
                    <%# ((decimal)((System.Data.DataRowView)Container.DataItem)["Weight"]).ToString("F4")%>
                    <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>            
    </MasterTableView>
    <ClientSettings ApplyStylesOnClient="true" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
        <Resizing AllowColumnResize="true" AllowRowResize="false" />
        <Selecting AllowRowSelect="true" />
        <ClientEvents OnRowSelecting="InstructionOrdersRowSelecting" OnRowDeselecting="InstructionOrdersRowDeselecting" />
    </ClientSettings>
    <FilterMenu CssClass="FilterMenuClass" ></FilterMenu>
</telerik:RadGrid>

<script language="javascript" type="text/javascript">
<!--
    function InstructionOrdersRowSelecting(rowObject)
    {
        var cell = this.GetCellByColumnUniqueName(rowObject, "checkboxSelectColumn");
        return RowSelectingHelper(cell);
    }
    
    function InstructionOrdersRowDeselecting(rowObject)
    {
        var cell = this.GetCellByColumnUniqueName(rowObject, "checkboxSelectColumn");
        return RowDeselectingHelper(cell);
    }
//-->
</script>