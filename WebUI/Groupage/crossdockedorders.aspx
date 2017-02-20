<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Groupage_crossdockedorders" Title="Cross Docked Orders" Codebehind="crossdockedorders.aspx.cs" %>

<%@ Register Src="~/UserControls/point.ascx" TagPrefix="uc" TagName="Point" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
     <telerik:RadWindowManager ID="rmwCrossDockedOrders" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="singleton" Width="600px" Height="900px" Modal="true" />
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>
     <h1>Cross Docked Orders</h1>
     <h2></h2>
     <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr valign="top">
                <td width="200"> 
                    <div style="font-weight:bold;">Date Range</div>
                    <table>
                        <tr>
                            <td class="formCellLabel">Date From</td>
                            <td class="formCellField"><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                        </tr>
                        <tr>
                            <td class="formCellLabel">Date To</td>
                            <td class="formCellField"><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Start Date for the filter"></telerik:RadDateInput></td>
                        </tr>
                   </table>
                </td>
                <td>
                    <div style="font-weight:bold;">Cross Dock Location (Leave Empty for all Locations)</div>
                    <uc:Point id="ucPoint" runat="server" CanCreateNewPoint="false"></uc:Point>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:button id="btnRefresh" runat="server" text="Refresh" CausesValidation="false" />    
    </div>
    <telerik:RadGrid runat="server" ID="gvOrders2" DataSourceID="odsOrders" AllowPaging="false" AllowSorting="true" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView Width="100%" DataKeyNames="OrderID">
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="Cross Dock Location" HeaderText="Cross Dock" ItemStyle-Font-Bold="true" ></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Order No" SortExpression="CustomerOrderNumber">
                    <ItemTemplate>
                        <a href="javascript:ViewOrderProfile(<%#((System.Data.DataRowView)Container.DataItem)["OrderID"] %>);"><%#((System.Data.DataRowView)Container.DataItem)["CustomerOrderNumber"] %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" DataField="DeliveryPointDescription" ItemStyle-Wrap="false"></telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false"  >
                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60" />
                <telerik:GridTemplateColumn HeaderText="Collection Job"  ItemStyle-Wrap="false" HeaderStyle-Width="70"  >
                    <ItemTemplate>
                        <a href="javascript:OpenJobDetails(<%#((System.Data.DataRowView)Container.DataItem)["NextPlannedJobID"] %>);"><%#((System.Data.DataRowView)Container.DataItem)["NextPlannedJobID"]%></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
        
    </telerik:RadGrid>
    <div class="buttonbar">
        <asp:button id="btnExport" runat="server" text="Export" CausesValidation="false" />    
    </div>
    <asp:ObjectDataSource runat="server" ID="odsOrders" TypeName="Orchestrator.Facade.Order" SelectMethod="GetCrossDockedOrders">
        <SelectParameters>
            <asp:ControlParameter ControlID="dteStartDate" PropertyName="Date" Name="startDate" />
            <asp:ControlParameter ControlID="dteEndDate" PropertyName="Date"  Name="endDate" />
            <asp:ControlParameter ControlID="ucPoint" PropertyName="PointID" name="PointID" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <script language="javascript" type="text/javascript">
    function ViewOrderProfile(orderID)
    {
        var url = "ManageOrder.aspx";
            url += "?oID=" + orderID;
        
        var wnd = window.radopen("about:blank", "largeWindow");                               
        wnd.SetUrl(url);
        wnd.SetTitle("Add/Update Order");
    }
    
   	function OpenJobDetails(jobId)
	{
   	    if (jobId == 0) return;

   	    var url = '../job/job.aspx?wiz=true&jobId=' + jobId + getCSID();

		openDialogWithScrollbars(url,'600','400');
	}
    </script>
</asp:Content>

