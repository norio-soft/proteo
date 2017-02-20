<%@ Page Title="Merge another Run" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="mergerun.aspx.cs" Inherits="Orchestrator.WebUI.Job.mergerun" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadWindowManager ID="rmwOpenOrderWindow" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="1180" Width="900" />
        </Windows>
    </telerik:RadWindowManager>

    <script language="javascript" src="../script/tooltippopups.js" type="text/javascript"></script>
    <cc1:Dialog ID="dlgAddOrder" URL="./addorder.aspx" Width="1180" Height="900" AutoPostBack="true" Mode="Modal"
        runat="server" ReturnValueExpected="true">
    </cc1:Dialog>

    <h1>Run Details</h1>
    <h2>
        <asp:Label ID="lblRunInformation" runat="server"></asp:Label></h2>
    <asp:Label ID="lblError" runat="server" CssClass="errorMessage" Visible="false" EnableViewState="false"></asp:Label>
    <div>
        <table>
            <tr>
                <td>Search:</td>
                <td colspan="3">
                    <asp:TextBox ID="txtSearch" runat="server" Width="125"></asp:TextBox>
            </tr>
        </table>
    </div>
    <div class="buttonbar">
        <asp:Button ID="btnFindRun" runat="server" Text="Find Run" OnClick="btnFindRun_Click" />
    </div>
    <div style="height: 320px; overflow: scroll;">
        <telerik:RadGrid runat="server" ID="grdOrders" Width="97%" AllowPaging="false" ShowGroupPanel="true" AllowSorting="true"
            Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
            <MasterTableView Width="100%" ClientDataKeyNames="OrderID,CanAddThisOrder" DataKeyNames="OrderID,CanAddThisOrder">
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <Columns>
                    <%--<telerik:GridClientSelectColumn UniqueName="bob" ItemStyle-Width="25" HeaderStyle-Width="25"></telerik:GridClientSelectColumn>--%>
                    <telerik:GridBoundColumn HeaderText="Order ID" DataField="OrderID" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Client" SortExpression="Customer" DataField="Customer"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessType" DataField="BusinessType"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                    <%--<telerik:GridTemplateColumn HeaderText="Collect Details" SortExpression="CollectionPointDescription" ItemStyle-Width="200">
                        <ItemTemplate>
                            <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("CollectionPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("CollectionPointDescription")%></span>
                            <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Deliver Details" SortExpression="DeliveryPointDescription" ItemStyle-Width="200">
                        <ItemTemplate>
                            <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("DeliveryPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("DeliveryPointDescription")%></span>
                            <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>--%>
                    <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                    <telerik:GridBoundColumn Display="false" Visible="false" DataField="CanAddThisOrder" />
                    <telerik:GridButtonColumn UniqueName="actionCol" Text="Add Order" ButtonCssClass="buttonClass" ButtonType="PushButton" CommandName="addOrder" />
                    <telerik:GridBoundColumn HeaderText="Status" SortExpression="Status" DataField="Status" />
                </Columns>
            </MasterTableView>
            <ClientSettings AllowDragToGroup="false" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                <Selecting AllowRowSelect="true" />
                <Resizing AllowColumnResize="true" AllowRowResize="false" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
    <div class="buttonbar">
        <asp:Button ID="btnMerge" runat="server" Text="Cancel Run and Close" OnClick="btnMerge_Click" />
    </div>

</asp:Content>
