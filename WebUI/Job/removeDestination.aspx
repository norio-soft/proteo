<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.removeDestination" Codebehind="removeDestination.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Remove Destination</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <base target="_self" />
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="height: 182; width: 100%; overflow: auto;">
        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
        <table width="99%">
            <tr>
                <td align="left" valign="top">
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td height="50">&nbsp;</td>
            </tr>
        </table>
    </div>

    <div class="buttonbar">
        <asp:Button ID="btnRemoveDestination" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
                    
</asp:Content>