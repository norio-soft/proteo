<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpOrdersByState" Codebehind="wpOrdersByState.ascx.cs" %>
<table width="100%">
    <tr>
        <td>
            <asp:gridview id="gvOrdersByState" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showheader="false">
                <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
                <rowStyle  cssclass="Row" />
                <AlternatingRowStyle  backcolor="WhiteSmoke" />
                <SelectedRowStyle  cssclass="SelectedRow" />
                <columns>
                    <asp:boundfield datafield="Description" HeaderText="State" ItemStyle-Width="240" />
                    <asp:boundfield datafield="Total" HeaderText="Total" />
                </columns>
            </asp:gridview>
        </td>
    </tr>
</table>