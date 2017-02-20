<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpUnallocatedOrders.ascx.cs" Inherits="Orchestrator.WebUI.WebParts.wpUnallocatedOrders" %>

<asp:SqlDataSource ID="sqlUnallocatedOrders" runat="server" DataSourceMode="DataSet" SelectCommand="spMI_GetCountOfUnallocatedOrdersByClient" ConnectionString="<%$ ConnectionStrings:Orchestrator %>" />

<asp:GridView ID="gvUnallocatedOrders" runat="server" DataSourceID="sqlUnallocatedOrders"
    AutoGenerateColumns="false" Width="100%" EnableViewState="false" CellSpacing="0"
    CellPadding="0" BorderWidth="0" CssClass="Grid" ShowFooter="true" ShowHeader="false">
    <HeaderStyle CssClass="HeadingRowLite" Height="22" VerticalAlign="middle" />
    <RowStyle CssClass="Row" />
    <AlternatingRowStyle BackColor="WhiteSmoke" />
    <SelectedRowStyle CssClass="SelectedRow" />
    <FooterStyle CssClass="Row" />
    <Columns>
        <asp:TemplateField HeaderText="Unallocated Orders">
            <ItemStyle Width="240" />
            <ItemTemplate>
                <a href="/groupage/findorder.aspx?uoiid=<%# Eval("IdentityID") %>">
                    <%# Eval("OrganisationName") %>
                </a>
            </ItemTemplate>
            <FooterStyle Font-Bold="true" />
            <FooterTemplate>Total</FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%# Eval("Count") %>
            </ItemTemplate>
            <FooterStyle Font-Bold="true" />
            <FooterTemplate><%= this.Total %></FooterTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
