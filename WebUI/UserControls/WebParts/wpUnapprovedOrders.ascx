<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="wpUnapprovedOrders.ascx.cs"
    Inherits="Orchestrator.WebUI.WebParts.wpUnapprovedOrders" %>
<asp:SqlDataSource ID="sqlUnapprovedOrders" runat="server" DataSourceMode="DataSet"
    SelectCommand="spMI_GetCountOfUnapprovedOrdersByClient" ConnectionString="<%$ConnectionStrings:Orchestrator %>">
</asp:SqlDataSource>
<asp:GridView ID="gvUnapprovedOrders" DataSourceID="sqlUnapprovedOrders" runat="server"
    AutoGenerateColumns="false" Width="100%" EnableViewState="false" CellSpacing="0"
    CellPadding="0" BorderWidth="0" CssClass="Grid" ShowFooter="true" ShowHeader="false">
    <HeaderStyle CssClass="HeadingRowLite" Height="22" VerticalAlign="middle" />
    <RowStyle CssClass="Row" />
    <AlternatingRowStyle BackColor="WhiteSmoke" />
    <SelectedRowStyle CssClass="SelectedRow" />
    <FooterStyle CssClass="Row" />
    <Columns>
        <asp:TemplateField HeaderText="Unapproved Orders">
            <ItemStyle Width="240" />
            <ItemTemplate>
                <a href="groupage/approveordersnew.aspx?iId=<%# DataBinder.Eval(Container.DataItem, "IdentityId")%>">
                    <%# DataBinder.Eval(Container.DataItem, "OrganisationName")%></a>
            </ItemTemplate>
            <FooterStyle Font-Bold="true" />
            <FooterTemplate>Total</FooterTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <%# DataBinder.Eval(Container.DataItem, "Count")%>
            </ItemTemplate>
            <FooterStyle Font-Bold="true" />
            <FooterTemplate><%=Total%></FooterTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
