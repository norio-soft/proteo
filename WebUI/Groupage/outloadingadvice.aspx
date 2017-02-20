<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.OutloadingAdvice" Title="Produce Outloading Advice Sheet" Codebehind="outloadingadvice.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h1>Collections Points With Orders</h1>
    <h2>The points below have orders that are for collection on the date specified, to generate an Outloading advice sheet for a collection point, tick the checkbox and click on Generate Report</h2>
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr>
                <td class="formCellLabel">Collection Date</td>
                <td class="formCellField"><telerik:RadDateInput id="dteCollectionDate" Width="75" runat="server" dateformat="dd/MM/yy" ToolTip="The Collection Date"></telerik:RadDateInput></td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonBar">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75" />
     </div>
 <telerik:RadGrid runat="server" ID="grdOrders" Width="600px"  DataSourceID="odsOrder" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView ClientDataKeyNames="PointID" DataKeyNames="PointID">
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign=Left HeaderStyle-Width="40" HeaderText="" ></telerik:GridClientSelectColumn>
                <telerik:GridBoundColumn HeaderText="ID" DataField="PointID" Visible="false"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Description" SortExpression="Description" DataField="Description" HeaderStyle-Wrap="false" />                
                <telerik:GridBoundColumn HeaderText="Number of Orders" SortExpression="Number Of Orders" DataField="Number Of Orders" HeaderStyle-Width="60" />            
            </Columns>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </telerik:RadGrid>
    <div class="buttonBar">
        <asp:Button ID="btnGenerate" Text="Generate Report" runat="server" />
    </div>        
    <asp:ObjectDataSource runat="server" ID="odsOrder" TypeName="Orchestrator.Facade.Order" SelectMethod="GetOrdersForOutloading" >
        <SelectParameters>
            <asp:ControlParameter ControlID="dteCollectiondate" PropertyName="Date"  Name="collectionDate" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

