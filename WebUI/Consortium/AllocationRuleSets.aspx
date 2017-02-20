<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="AllocationRuleSets.aspx.cs" Inherits="Orchestrator.WebUI.Consortium.AllocationRuleSets" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Allocation Rule Sets</h1>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 500px;">
        <telerik:RadGrid ID="grdRuleSets" runat="server" AutoGenerateColumns="false">
            <MasterTableView DataKeyNames="AllocationRuleSetID">
                <Columns>
                    <telerik:GridHyperLinkColumn DataTextField="Description" DataNavigateUrlFormatString="allocationrulesets.aspx?rsid={0}" DataNavigateUrlFields="AllocationRuleSetID" HeaderText="Description" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        
        <asp:Panel ID="pnlRuleSet" runat="server" Visible="false">
            <table>
                <tr>
                    <td class="formCellLabel" style="width: 140px;">
                        Description
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtDescription" runat="server" />
                    </td>
                </tr>
            </table>
            
            <asp:ListView ID="lvPointTables" runat="server" DataKeyNames="AllocationRulePointTableID">
                <LayoutTemplate>
                    <br />
                    <h2>Point Tables</h2>
                    <table>
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="formCellLabel" style="width: 140px;"><%# Eval("Description") %></td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboPointTable" runat="server" DataValueField="AllocationPointTableID" DataTextField="Description" Width="250" AppendDataBoundItems="true">
                                <Items>
                                    <telerik:RadComboBoxItem Value="" Text="- select -" />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:RequiredFieldValidator ID="rfvPointTable" runat="server" ControlToValidate="cboPointTable" InitialValue="- select -" Display="Dynamic">&bull;</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
            
            <asp:ListView ID="lvZoneTables" runat="server" DataKeyNames="AllocationRuleZoneTableID">
                <LayoutTemplate>
                    <br />
                    <h2>Zone Tables</h2>
                    <table>
                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="formCellLabel" style="width: 140px;"><%# Eval("Description") %></td>
                        <td class="formCellField">
                            <telerik:RadComboBox ID="cboZoneTable" runat="server" DataValueField="AllocationZoneTableID" DataTextField="Description" Width="250" AppendDataBoundItems="true">
                                <Items>
                                    <telerik:RadComboBoxItem Value="" Text="- select -" />
                                </Items>
                            </telerik:RadComboBox>
                            <asp:RequiredFieldValidator ID="rfvZoneTable" runat="server" ControlToValidate="cboZoneTable" InitialValue="- select -" Display="Dynamic">&bull;</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>

            <br />
            <div class="buttonBar">
                <asp:Button ID="btnSave" runat="server" CausesValidation="true" Text="Update" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
