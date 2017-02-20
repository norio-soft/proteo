<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Point.getLegPointDataForJobhtml" Codebehind="getLegPointDataForJobhtml.aspx.cs" %>
<%@ OutputCache Duration="360" VaryByParam="InstructionId" %>
<html>
<head id="Head1" runat="server">
</head>
<body>
<table width="200" cellspacing="0" cellpadding="3">
    <tr valign="top">
        <td><asp:label id="lblAddress" runat="server"></asp:label></td>
    </tr>
        <td><asp:label id="lblBookedDateTime" runat="server"></asp:label> </td>
    </tr>
    <tr>
        <td><asp:label id="lblPlannedTimes" runat="server"></asp:label></td>
    </tr>
    <tr>
        <td colspan="2"><asp:label id="lblInstructionType" runat="server"></asp:label></td>
    </tr>
</table>
</body>
</html>
