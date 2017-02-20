<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Pallet.ListPalletTypes" MasterPageFile="~/default_tableless.Master" Codebehind="ListPalletTypes.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List Pallet Types</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <h2>Allows you to view a list of the pallet types configured in the system</h2>
    <asp:GridView ID="gvPalletTypes" runat="server" AllowPaging="true" AllowSorting="false" AutoGenerateColumns="false" PageSize="25" cellspacing="0" cellpadding="3" borderwidth="1" cssclass="Grid" width="650">
        <HeaderStyle cssclass="HeadingRowLite" height="22" verticalalign="middle" HorizontalAlign="left" />
        <RowStyle cssclass="Row" />
        <AlternatingRowStyle backcolor="WhiteSmoke" />
        <SelectedRowStyle cssclass="SelectedRow" />
        <Columns>
            <asp:HyperLinkField HeaderText="Description" DataTextField="Description" DataNavigateUrlFields="PalletTypeId" DataNavigateUrlFormatString="AddUpdatePalletType.aspx?palletTypeId={0}" />
            <asp:TemplateField HeaderText="Track Pallets by Default" ItemStyle-Width="150">
                <ItemTemplate>
                    <%# ((bool)((System.Data.DataRowView) Container.DataItem)["TrackByDefault"]) ? "yes" : "no" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Active (For New Clients)" ItemStyle-Width="170">
                <ItemTemplate>
                    <%# ((bool)((System.Data.DataRowView) Container.DataItem)["ForNewClientsByDefault"]) ? "yes" : "no" %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Default Pallet Type (For new Clients)" ItemStyle-Width="170">
                <ItemTemplate>
                    <%# ((bool)((System.Data.DataRowView) Container.DataItem)["IsDefault"]) ? "yes" : "no" %>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <div class="whitepacepusher"></div>
    <div class="buttonbar">
        <asp:Button id="btnAddPalletType" runat="server" Text="Add Pallet Type" />
    </div>
</asp:Content>
