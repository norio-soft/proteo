<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.OrderGroupProfile"
    MasterPageFile="~/WizardMasterPage.master" Title="Haulier Enterprise"
    CodeBehind="OrderGroupProfile.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>
<%@ Import Namespace="System.Data" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">
    Order Group Details</asp:Content>
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

    <script language="javascript" type="text/javascript" src="/script/scripts.js"></script>

    <script language="javascript" type="text/javascript" src="/script/tooltippopups.js"></script>

    <style type="text/css">
        td.form-label
        {
            font-size: 10px;
            font-weight: bold;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgOrder" runat="server" Width="1220" Height="790" Mode="Normal" URL="manageorder.aspx" AutoPostBack="true" ReturnValueExpected="true" />
    <cc1:Dialog ID="dlgAssociateOrder" runat="server" URL="AssociateOrderWithGroup.aspx" Height="500" Width="800" Mode="Normal" AutoPostBack="true" ReturnValueExpected="true" />
    <fieldset>
        <legend>Order Group Details</legend>
        <uc1:infringementDisplay runat="server" ID="ruleInfringements" Visible="false" EnableViewState="false" />
        <table style="font-size: 12px;" cellspacing="0">
            <tr>
                <td class="formCellLabel">
                    Rate
                </td>
                <td class="formCellField" colspan="2">
                    <asp:Label ID="lblRate" runat="server"></asp:Label>
                </td>
                <td rowspan="5" valign="top" style="padding-left:10px;">
                    <asp:TextBox ID="txtRateTariffCard" CssClass="fieldInputBox" ReadOnly="true" runat="server" BackColor="#fffedb" Font-Bold="true" Rows="5" TextMode="MultiLine" Columns="45"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Grouped Planning
                </td>
                <td class="formCellField">
                    <asp:RadioButtonList ID="cboGroupedPlanning" runat="server" AutoPostBack="true" RepeatDirection="Horizontal"
                        RepeatColumns="2">
                        <asp:ListItem Value="true" Text="Yes"></asp:ListItem>
                        <asp:ListItem Value="false" Text="No"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    Order Count
                </td>
                <td class="formCellField">
                    <asp:Label ID="lblOrderCount" runat="server"></asp:Label>
                </td>
            </tr>
            <tr id="trAllocation" runat="server">
                <td class="formCellLabel">
                    Allocated To
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboAllocatedTo" runat="server" Skin="WindowsXP" Width="200"
                        DropDownWidth="300" ShowMoreResultsBox="false" ItemRequestTimeout="500" EnableLoadOnDemand="true"
                        OnClientTextChange="cboAllocatedTo_TextChange" AutoPostBack="true">
                        <WebServiceSettings Path="~/ws/combostreamers.asmx" Method="GetSubContractors" />
                    </telerik:RadComboBox>
                    <asp:Label ID="lblAllocatedTo" runat="server" Visible="false" />
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">
                    <asp:Label ID="lblLoadNoText" runat="server"></asp:Label>
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtLoadNumber" runat="server"></asp:TextBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <legend>Grouped Orders</legend>
        <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" AutoGenerateColumns="false">
            <MasterTableView Width="100%" DataKeyNames="OrderID" ItemStyle-Height="18" Name="Master" EnableColumnsViewState="false">
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <DetailTables>
                    <telerik:GridTableView DataKeyNames="OrderID" AutoGenerateColumns="false" Name="Child">
                        <ParentTableRelation>
                            <telerik:GridRelationFields DetailKeyField="OrderID" MasterKeyField="OrderID" />
                        </ParentTableRelation>
                        <Columns>
                            <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                            <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription" />
                            <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false" />
                       </Columns>
                    </telerik:GridTableView>
                </DetailTables>
                <Columns>
                    <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName"></telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="ID">
                        <ItemTemplate>
                            <a id="hypOrder" href="javascript:<%# dlgOrder.GetOpenDialogScript(string.Format("OID={0}", ((DataRowView)Container.DataItem)["OrderID"])) %>">
                                <%# ((DataRowView)Container.DataItem)["OrderID"] %></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="runId" ItemStyle-Width="10" HeaderText="Run Id">
                        <ItemTemplate>
                            <a id="hypRun" runat="server"></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Customer Order Number" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber" UniqueName="CustomerOrderNumber" />
                    <telerik:GridBoundColumn HeaderText="DeliveryOrder Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" UniqueName="DeliveryOrderNumber"/>
                    <telerik:GridTemplateColumn HeaderText="Collect From">
                        <ItemTemplate>
                            <span id="spnCollectionPoint" onmouseover="ShowPointToolTip(this, <%#((DataRowView)Container.DataItem)["CollectionRunDeliveryPointID"].ToString() %>);" onmouseout="closeToolTip();" class="orchestratorLink">
                                <b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b>
                            </span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionRunDeliveryDateTime" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionRunDeliveryIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription">
                        <ItemTemplate>
                            <span id="spnDeliveryPoint" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onmouseout="closeToolTip();" class="orchestratorLink">
                                <b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b>
                            </span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60" />
                    <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60" UniqueName="NoPallets" />
                    <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80">
                        <ItemTemplate>
                            <%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["Weight"].ToString()).ToString("N2")%>
                            <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Rate" UniqueName="Rate" ItemStyle-Width="80px">
                        <ItemTemplate>
                            <telerik:RadNumericTextBox Width="60px" Type="Currency" runat="server" ID="rntForeignRate"></telerik:RadNumericTextBox>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridButtonColumn HeaderText="" ButtonType="PushButton" CommandName="RemoveOrder" Text="Remove"></telerik:GridButtonColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </fieldset>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <div class="buttonbar">
            <asp:Button ID="btnCreateJob" runat="server" Text="Create Run" ToolTip="Create a job to manage the collection and delivery of these orders." CausesValidation="false" />
            <asp:Button ID="btnProducePILs" runat="server" Text="Produce PILs" ToolTip="Produce all the PILs for this order group." CausesValidation="false" />
            <asp:Button ID="btnRemoveAll" runat="server" Text="Remove All" ToolTip="Remove all the orders from this group." />
            <asp:Button ID="btnAssociateOrder" runat="server" Text="Associate Order" OnClientClick="associateOrder();return false;" CausesValidation="false" />
            <asp:Button ID="btnAddNewOrder" runat="server" Text="Add New Order" OnClientClick="addNewOrder();return false;" CausesValidation="false" />
            <asp:Button ID="btnUpdateOrders" runat="server" Text="Update order(s)" CausesValidation="false" ToolTip="Save all orders with updated information." />
        </div>

        <script language="javascript" type="text/javascript">
    <!--
        var _orderGroupID = <%= OrderGroup != null ? OrderGroup.OrderGroupID : 0 %>;
        
        function windowClosed()
        {
            location.href = <%=GetRefreshPageUrl() %>;
        }

        function addNewOrder()
        {
            if (_orderGroupID > 0)
            {
                var qs = "ogid=" + _orderGroupID;
                <%=dlgOrder.ClientID %>_Open(qs);
            }
        }
        
        function ViewJob(jobID) {
            var url = "/job/job.aspx?jobId=" + jobID+ getCSID();

            window.open(url);
        }

        function associateOrder()
        {
            if (_orderGroupID > 0)
            {
                var qs = "ogid=" + _orderGroupID;
                <%=dlgAssociateOrder.ClientID %>_Open(qs);
            }
        }

        function cboAllocatedTo_TextChange(sender, eventArgs) {
            // If the user has typed text that does not match an item in the list then clear the field.
            var item = sender.get_selectedItem();
            var text = sender.get_text();
            if ((item != null && text != item.get_text()) || (item == null && text != "")) {
                sender.clearSelection();
                sender.set_text("");
            }
        }
        
    //-->
        </script>

    </telerik:RadCodeBlock>
</asp:Content>
