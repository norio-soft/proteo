<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.UserControls.DateAndTime" Codebehind="DateAndTime.ascx.cs" %>

<table cellpadding="0" cellspacing="0">
    <tr>
        <td><telerik:RadDateInput id="dteDate" runat="server" dateformat="dd/MM" Width="32px" Font-Size="11px" ></telerik:RadDateInput></td><td><telerik:RadDateInput id="dteTime" runat="server"  dateformat="HH:mm" Width="32px" Font-Size="11px"></telerik:RadDateInput></td>
    </tr>
</table>