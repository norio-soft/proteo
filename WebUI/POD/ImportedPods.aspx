<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master"
    CodeBehind="ImportedPods.aspx.cs" Inherits="Orchestrator.WebUI.POD.ImportedPods"
    Title="Haulier Enterprise" %>
    
<%@ Register Assembly="Orchestrator.EF" Namespace="Orchestrator.EF" TagPrefix="cc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <script type="text/javascript" src="/script/jquery.ajaxupload.3.6.js"></script>

    <script type="text/javascript" src="ImportedPods.aspx.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Imported Pods</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="float: left;">
        <h3>
            Imported Pods</h3>
        <table style="text-align: left;" width="35%">
            <tr>
                <td class="formCellLabel" style="width: 6%;">
                    Imported From:
                </td>
                <td class="formCellField" style="width: 5%;">
                    <telerik:RadDatePicker ID="dteImportFromDate" runat="server" Width="100"
                        ToolTip="The imported from date for the pod filter">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
                <td class="formCellLabel" style="width: 8%;">
                    Imported To:
                </td>
                <td class="formCellField" style="width: 5%;">
                    <telerik:RadDatePicker ID="dteImportToDate" runat="server" Width="100" ToolTip="The imported to date for the pod filter">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
            </tr>
        </table>
    </div>
    <div class="clearDiv">
    </div>
    <div class="buttonBar" style="margin: 10px 0px 5px 0px;">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
        <asp:Button ID="btnDelete" runat="server" Text="Delete selected pods" />
        <asp:Button ID="btnUpdateOrderIds" runat="server" Text="Update" />
        <asp:Button ID="btnMatchRoutine" runat="server" Text="Run matching routine" />
    </div>
    <asp:ListView ID="lvItems" runat="server">
        <LayoutTemplate>
            <div class="listViewGrid">
                <table id="orders" cellpadding="0" cellspacing="0">
                    <thead>
                        <tr align="left" class="HeadingRow">
                            <th class="first">
                            </th>
                            <th>
                                <input type="checkbox" id="chkDeleteAll" runat="server" onclick="javascript:HandleSelectAll(this);" />Del
                            </th>
                            <th>
                                Order Id
                            </th>
                            <th>
                                System
                            </th>
                            <th>
                                Image Name
                            </th>
                            <th>
                                Date Received
                            </th>
                            <th>
                                Comments
                            </th>
                            <th>
                                Consignment No.
                            </th>
                            <th>
                                Pod Status
                            </th>
                            <th>
                                Processing Message
                            </th>
                            <th>
                                Last Updated By
                            </th>
                            <th>
                                Last Updated Date
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr id="itemPlaceHolder" runat="server" />
                    </tbody>
                </table>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <tr id="row" runat="server" class="Row">
                <td class="first" runat="server">
                    <asp:HiddenField ID="hidRowDirty" runat="server" Value="false" />
                </td>
                <td id="DeleteCell" runat="server">
                    <input type="checkbox" id="chkDelete" runat="server" />
                </td>
                <td id="OrderCell" runat="server">
                    <asp:TextBox ID="txtOrderId" runat="server" Text='<%# ((ImportedPod)Container.DataItem).OrderId%>' OnChange="txtOrderId_OnTextChanged(this)"></asp:TextBox>
                </td>
                <td id="SystemCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).FromSystem%>
                </td>
                <td id="ImageNameCell" runat="server">
                    <a id="ancImage" runat="server" target="_blank"><%# ((ImportedPod)Container.DataItem).ImageName%></a>
                </td>
                <td id="DateReceivedCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).DateReceived%>
                </td>
                <td id="CommentsCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).Comments%>
                </td>
                <td id="ConNumberCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).ConNumber%>
                </td>
                <td id="PodStatusCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).PodStatus%>
                </td>
                <td id="ProcessingMessage" runat="server">
                    <%# ((ImportedPod)Container.DataItem).ProcessingMessage%>
                </td>
                <td id="LastUpdateUserIdCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).LastUpdatedUserId%>
                </td>
                <td id="LastUpdateDateCell" runat="server">
                    <%# ((ImportedPod)Container.DataItem).LastUpdatedDate%>
                </td>
            </tr>
        </ItemTemplate>
        <EmptyDataTemplate>
            <p>No Imported PODs found for the given date range.</p>
        </EmptyDataTemplate>
    </asp:ListView>
    <div class="buttonBar" id="buttonBarBottom" runat="server" style="margin: 10px 0px 5px 0px;">
        <asp:Button ID="btnRefreshBottom" runat="server" Text="Refresh" CausesValidation="false" />
        <asp:Button ID="btnDeleteBottom" runat="server" Text="Delete selected pods" />
        <asp:Button ID="btnUpdateOrdersIdsBottom" runat="server" Text="Update" />
        <asp:Button ID="btnMatchRoutineBottom" runat="server" Text="Run matching routine" />
    </div>
</asp:Content>
