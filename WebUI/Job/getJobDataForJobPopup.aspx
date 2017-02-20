<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.GetJobDataForJobPopUp" Codebehind="getJobDataForJobPopup.aspx.cs" %>
<%@ OutputCache Duration="1800" VaryByParam="LegId" %>
<table  cellspacing="0" cellpadding="3">
    <tr valign="top">
        <td colspan="2"><asp:label id="lblJobId" runat="server"></asp:label></td>
        <td>
            <table cellpadding="3" cellspacing="0">
                <tr>
                    <td style="font-weight:bold">FROM</td>
                    <td style="font-weight:bold">TO</td>
                </tr>
                <asp:repeater id="rptLegs" runat="server">
                    <ItemTemplate>
                    <tr>
                        <td><asp:label id="lblFromOrganisation" runat="server"></asp:label><br />
                            <b><asp:label id="lblFromPoint" runat="server"></asp:label></b><br />
                            <asp:label id="lblFromDateTime" runat="server"></asp:label> </td>
                        <td><asp:label id="lblToOrganisation" runat="server"></asp:label><br />
                            <b><asp:label id="lblToPoint" runat="server"></asp:label></b><br />
                            <asp:label id="lblToDateTime" runat="server"></asp:label> </td>
                    </tr>        
                    </ItemTemplate>
                </asp:repeater>
            </table>
        </td>
    </tr>
</table>