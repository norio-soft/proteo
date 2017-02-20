<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="ConvertInstruction.aspx.cs" Inherits="Orchestrator.WebUI.Job.ConvertInstruction" Title="Convert Instruction" %>

<%@ Register TagPrefix="orchestrator" TagName="point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Change Action</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:MultiView ID="mvConvertInstruction" runat="server">
        <asp:View ID="vwConvertDrop" runat="server">
            <asp:Panel runat="server" DefaultButton="btnCancelDropConversion">
                <p>You are about to change a drop instruction to a cross dock instruction.  Please review the orders shown below and check any messages before supplying the point you wish to cross dock to.</p>
                <fieldset>
                    <legend>Orders to affect</legend>
                    <div style="height: 318px; overflow: auto;" >
                        <telerik:RadGrid id="grdOrdersOnDrop" runat="server" Width="99%" AllowPaging="false" AllowSorting="false" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false">
                            <MasterTableView Width="100%" ClientDataKeyNames="OrderID" DataKeyNames="OrderID">
                                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                                <Columns>
                                    <telerik:GridBoundColumn HeaderText="ID" DataField="OrderID" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription" DataField="BusinessTypeDescription"></telerik:GridBoundColumn>
                                    <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                                    <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                                    <telerik:GridTemplateColumn HeaderText="Collect Details" SortExpression="CollectionPointDescription" ItemStyle-Width="200">
                                        <ItemTemplate>
                                            <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("CollectionPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("CollectionPointDescription")%></span>
                                            <%#GetDate((DateTime)Eval("CollectionDateTime"), (bool)Eval("CollectionIsAnyTime"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Deliver Details" SortExpression="DeliveryPointDescription" ItemStyle-Width="200">
                                        <ItemTemplate>
                                            <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("DeliveryPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("DeliveryPointDescription")%></span>
                                            <%#GetDate((DateTime)Eval("DeliveryDateTime"), (bool)Eval("DeliveryIsAnyTime"))%>
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                                    <telerik:GridBoundColumn HeaderText="Delivering Resource" SortExpression="DeliveringResource" DataField="DeliveringResource" />
                                    <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="ForeignRate">
                                        <ItemTemplate>
                                            <asp:Label ID="lblRate" runat="server"></asp:Label>&nbsp;
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                    <telerik:GridBoundColumn HeaderText="Message" DataField="Message" UniqueName="CustomerID">
                                         <ItemStyle Font-Bold="true" />
                                    </telerik:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                            <ClientSettings AllowDragToGroup="false" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                                <Selecting AllowRowSelect="false" />
                                <Resizing AllowColumnResize="true" AllowRowResize="false" />
                            </ClientSettings>
                        </telerik:RadGrid>
                    </div>
                </fieldset>
                <table class="form-table">
                    <tr>
                        <td class="form-label" valign="top">Cross dock at</td>
                        <td class="form-value"><orchestrator:point ID="ucPoint" runat="server" /></td>
                        <td class="form-label"><asp:CustomValidator ID="cfvPoint" runat="server" EnableClientScript="false" ValidationGroup="vgConvertDrop" Display="Dynamic" ErrorMessage="Please select the point to trunk to."><img src="../../images/ico_critical_small.gif" Title="Please select the point to trunk to." /></asp:CustomValidator></td>
                    </tr>
                </table>
                <div class="buttonBar">
                    <asp:Button ID="btnCancelDropConversion" runat="server" Text="Cancel" CausesValidation="false" />&nbsp;<asp:Button ID="btnConfirmDropConversion" runat="server" Text="Confirm" ValidationGroup="vgConvertDrop" />
                </div>
                <br />
            </asp:Panel>
        </asp:View>
        
        <asp:View id="vwConvertTrunk" runat="server">
            <asp:Panel runat="server" DefaultButton="btnCancelTrunkConversion">
                <p>You are about to change a cross dock instruction to a drop instruction.  Please review the orders shown below and check any messages before confirming the conversion.</p>
                <fieldset>
                    <legend>Orders to affect</legend>
                    <telerik:RadGrid id="grdOrdersOnTrunk" runat="server" Width="99%" AllowPaging="false" AllowSorting="false" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false">
                        <MasterTableView Width="100%" ClientDataKeyNames="OrderID" DataKeyNames="OrderID">
                            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="ID" DataField="OrderID" HeaderStyle-Width="50"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription" DataField="BusinessTypeDescription"></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel" DataField="OrderServiceLevel" />
                                <telerik:GridBoundColumn HeaderText="Order No" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Collect Details" SortExpression="CollectionPointDescription" ItemStyle-Width="200">
                                    <ItemTemplate>
                                        <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("CollectionPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("CollectionPointDescription")%></span>
                                        <%#GetDate((DateTime)Eval("CollectionDateTime"), (bool)Eval("CollectionIsAnyTime"))%>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Deliver Details" SortExpression="DeliveryPointDescription" ItemStyle-Width="200">
                                    <ItemTemplate>
                                        <span onmouseover="javascript:ShowPointToolTip(this,<%#Eval("DeliveryPointID") %>);" onmouseout="closeToolTip();" style="font-weight:bold;"><%#Eval("DeliveryPointDescription")%></span>
                                        <%#GetDate((DateTime)Eval("DeliveryDateTime"), (bool)Eval("DeliveryIsAnyTime"))%>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                                <telerik:GridBoundColumn HeaderText="Delivering Resource" SortExpression="DeliveringResource" DataField="DeliveringResource" />
                                <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="ForeignRate">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRate" runat="server"></asp:Label>&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Message" DataField="Message" ItemStyle-Font-Size="XX-Large">
                                    
                                </telerik:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                        <ClientSettings AllowDragToGroup="false" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                            <Selecting AllowRowSelect="false" />
                            <Resizing AllowColumnResize="true" AllowRowResize="false" />
                        </ClientSettings>
                    </telerik:RadGrid>
                </fieldset>
                <div class="buttonBar">
                    <asp:Button ID="btnCancelTrunkConversion" runat="server" Text="Cancel" CausesValidation="false" />&nbsp;<asp:Button ID="btnConfirmTrunkConversion" runat="server" Text="Confirm" ValidationGroup="vgConvertTrunk" />
                </div>
            </asp:Panel>
        </asp:View>
    </asp:MultiView>
</asp:Content>