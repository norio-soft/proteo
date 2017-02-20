<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpTop10PCVLocations" Codebehind="wpTop10PCVLocations.ascx.cs" %>
<asp:sqldatasource id="sqlPCVS" runat="server" DataSourceMode="DataSet" selectCommand="spMI_Top10PCVLocations" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Sliding" enablecaching="true" cacheduration="30"></asp:sqldatasource>                                            
<asp:gridview id="gvPCVS" DataSourceID="sqlPCVS" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showheader="true">
    <headerstyle CssClass="webpartHeadingRow" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <columns>
        <asp:TemplateField HeaderText="Location" ItemStyle-Width="240">
            <ItemTemplate>
                <a href="/traffic/jobmanagement/takePCVs.aspx?pointID=<%#Eval("PointID") %>"><%#Eval("Description") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:boundfield datafield="NoPallets" HeaderText="Pallets" />
        <asp:boundfield datafield="NoPCVS" HeaderText="PCVs" />
    </columns>
</asp:gridview>
