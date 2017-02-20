<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpClientPalletBalance" Codebehind="wpClientPalletBalance.ascx.cs" %>
<asp:sqldatasource id="sqlClientPalletBalances" runat="server" DataSourceMode="DataSet" selectCommand="spMI_GetClientPalletBalances" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Sliding" enablecaching="true" cacheduration="30"></asp:sqldatasource>

<asp:gridview id="gvClientPalletBalances" DataSourceID="sqlClientPalletBalances" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showheader="false">
    <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <columns>
        <asp:boundfield datafield="OrganisationName" HeaderText="Client" ItemStyle-Width="240" />
        <asp:boundfield datafield="Balance" HeaderText="Balance" />
        <asp:BoundField DataField="PalletTypeDescription" HeaderText="Pallet Type" />
    </columns>
</asp:gridview>