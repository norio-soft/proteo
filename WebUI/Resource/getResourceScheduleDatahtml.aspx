<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Resource.getResourceScheduleDatahtml" Codebehind="getResourceScheduleDatahtml.aspx.cs" %>
<%@ OutputCache Duration="360" VaryByParam="ResourceId" %>
<html>
<head id="Head1" runat="server" title="Holiday">
</head>
<body>
<table width="200" cellspacing="0" cellpadding="3">
    <tr valign="top"><td><asp:label id="lblResourceScheduleDescription" runat="server"></asp:label></td></tr>
    <tr>
        <td><asp:Label runat="server"><b>From: </b></asp:Label></td>
        <td><asp:label id="lblFromDate" runat="server"></asp:label></td>
    </tr>
    <tr>
        <td><asp:Label ID="Label1" runat="server"><b>To: </b></asp:Label></td>
        <td><asp:label id="lblToDate" runat="server"></asp:label></td>
    </tr>
</table>
</body>
</html>