<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.setTrafficArea" MasterPageFile="~/WizardMasterPage.master" Title="Change Traffic Area" Codebehind="setTrafficArea.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Set Traffic Area</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table height="120" border="0">
        <tr runat="server" visible="false" id="trErrors">
            <td colspan="99"><uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" /></td>
        </tr>
        <tr valign=top height="20">
            <td colspan="2">Set the control area and traffic area of this leg to make it another planner's responsibility. If you wish this leg and all following legs to also be set to the new area, check the checkbox.</td>
        </tr>
        <tr valign=top height="20">
            <td width="30%">Control&nbsp;Area</td>
            <td width="70%"><asp:DropDownList ID="cboControlArea" runat="server" DataValueField="ControlAreaId" DataTextField="Description"></asp:DropDownList></td>
        </tr>
        <tr valign=top height="20">
            <td>Traffic&nbsp;Area</td>
            <td><asp:DropDownList ID="cboTrafficArea" runat="server" DataValueField="TrafficAreaId" DataTextField="Description"></asp:DropDownList></td>
        </tr>
        <tr valign=top height="20">
            <td colspan="2"><asp:CheckBox ID="chkApplyToAllFollowingInstructions" runat="server" Text="Apply to all following legs also" /></td>
        </tr>
    </table>              
    <div class="buttonbar">
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" />
    </div>       
</asp:Content>