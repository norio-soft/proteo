<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpMessagesAwaiting" Codebehind="wpMessagesAwaiting.ascx.cs" %>
<asp:sqldatasource id="sqlNewJobsAwaitingIntegration" runat="server" DataSourceMode="DataSet" selectCommand="spMI_GetCountOfNewJobsAwaitingIntegration" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Sliding" enablecaching="true" cacheduration="30"></asp:sqldatasource>

<asp:gridview id="gvNewJobsAwaitingIntegration" DataSourceID="sqlNewJobsAwaitingIntegration" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showfooter="false" showheader="false" >
    <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <footerstyle cssclass="Row" />
    <columns>
        <asp:templatefield headertext="Source">
            <itemstyle Width="240" />
            <itemtemplate>
                <%# Orchestrator.WebUI.Utilities.UnCamelCase((string) DataBinder.Eval(Container.DataItem, "Description"))%>
            </itemtemplate>
        </asp:templatefield>    
        <asp:templatefield headertext="" >
            <itemstyle Horizontalalign="left" />
            <itemtemplate>
                <%#DataBinder.Eval(Container.DataItem, "Total") %>
            </itemtemplate>
        </asp:templatefield>    
        </columns>
 </asp:gridview>