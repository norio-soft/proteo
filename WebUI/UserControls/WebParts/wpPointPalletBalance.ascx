<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpPointPalletBalance" Codebehind="wpPointPalletBalance.ascx.cs" %>
<asp:sqldatasource id="sqlPointPalletBalances" runat="server" DataSourceMode="DataSet" selectCommand="spMI_GetPointPalletBalances" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Sliding" enablecaching="true" cacheduration="30"></asp:sqldatasource>                                            
<asp:gridview id="gvPointPalletBalances" DataSourceID="sqlPointPalletBalances" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showheader="false">
    <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <columns>
        <asp:templatefield headertext="Description" itemstyle-width="240">
            <itemtemplate>
                <%# DataBinder.Eval(Container.DataItem, "Description")%>
            </itemtemplate>
        </asp:templatefield>
        <asp:boundfield datafield="Balance" HeaderText="Balance" />
        <asp:BoundField DataField="PalletTypeDescription" HeaderText="Pallet Type" />
    </columns>
</asp:gridview>