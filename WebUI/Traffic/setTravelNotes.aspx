<%@ Page language="c#" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.Traffic.setTravelNotes" Codebehind="setTravelNotes.aspx.cs" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Set Travel Notes</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        
    <div style="height: 220px; width: 100%; overflow: auto;">
        <table width="99%">
            <tr>
                <td>Set travel notes for <asp:Label ID="lblDriverName" runat="server"></asp:Label> to:</td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox ID="txtTravelNotes" runat="server" TextMode="MultiLine" Rows="10" Columns="50"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
                
    <div class="buttonbar">
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
                
</asp:Content>