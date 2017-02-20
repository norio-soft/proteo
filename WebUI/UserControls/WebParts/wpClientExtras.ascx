<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpClientExtras" Codebehind="wpClientExtras.ascx.cs" %>
<asp:sqldatasource id="sqlClientExtras" runat="server" DataSourceMode="DataSet" selectCommand="spMI_GetCountOfExtrasByClient" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Sliding" enablecaching="true" cacheduration="30"></asp:sqldatasource>
            				                
<asp:gridview id="gvExtrasByClient" DataSourceID="sqlClientExtras" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" borderwidth="0" cssclass="Grid" showfooter="false">
    <headerstyle CssClass="webpartHeadingRow" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <footerstyle cssclass="Row" />
    <columns>
        <asp:templatefield headertext="Client" ItemStyle-Width="40%" ItemStyle-Wrap="false">
            <ItemTemplate>
                <a href="/invoicing/invoiceextrapreparation.aspx?identityid=<%# Eval("IdentityId" ) %>"><%# Eval("OrganisationName") %></a>
            </ItemTemplate>
        </asp:templatefield>
        <asp:boundfield datafield="AwaitingResponse" HeaderText="Awaiting" ItemStyle-Width="30" />
        <asp:boundfield datafield="Accepted" HeaderText="Accepted" ItemStyle-Width="30" />
    </columns>
 </asp:gridview>