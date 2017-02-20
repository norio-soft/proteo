<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpOSCallins" Codebehind="wpOSCallins.ascx.cs" %>
 <asp:sqldatasource id="sqlOSCallIns" runat="server" DataSourceMode="DataSet" selectCommand="spMI_OutstandingCallIns" ConnectionString="<%$ConnectionStrings:Orchestrator %>" ></asp:sqldatasource>
<asp:gridview id="gvOSCallIns" DataSourceID="sqlOSCallIns" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showfooter="false" showheader="false">
    <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <footerstyle cssclass="Row" />
    <columns>
        <asp:templatefield headertext="Control Area">
            <itemstyle Width="240" />
            <itemtemplate>
                <a href="job/callinlog.aspx?ca=<%# DataBinder.Eval(Container.DataItem, "ControlAreaId")%>"><%# DataBinder.Eval(Container.DataItem, "ControlArea")%></a>
            </itemtemplate>
        </asp:templatefield>    
        <asp:BoundField DataField="OSCallIns" HeaderText="" />
        </columns>
 </asp:gridview>