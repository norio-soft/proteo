<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.RemoveTrunk" Codebehind="removetrunk.aspx.cs" Title="Haulier Enterprise" MasterPageFile="~/WizardMasterPage.Master" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Remove Trunk</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <base target="_self" />

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/tooltippopups.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="divPointAddress" style="z-index: 5; display: none; background-color: Wheat; padding: 2px 2px 2px 2px;">
        <table style="background-color: white; border: solid 1pt black;" cellpadding="2">
            <tr>
                <td>
                    <span id="spnPointAddress"></span>
                </td>
            </tr>
        </table>
    </div>
    
    <div style="height: 250; width: 100%; overflow: auto;">
        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
        
        <asp:Panel id="pnlConfirmation" runat="server">
            <table class="Grid" cellpadding="3" cellspacing="0" width="100%">
                <tr class="HeadingRow">
                    <td class="HeadingCellText">From</td>
                    <td class="HeadingCellText">To</td>
                </tr>
                <tr>
                <telerik:RadCodeBlock id="codeBlock2" runat="server">
                    <td><span onmouseover='ShowPointToolTip(this, "<%=StartPointId %>");' onmouseout="closeToolTip();"><%=LegStart%></span></td>  
                    <td><span onmouseover='ShowPointToolTip(this, "<%=EndPointId %>");' onmouseout="closeToolTip();"><%=LegEnd%></span></td>  
                </telerik:RadCodeBlock>
                </tr>
            </table>
            <br />
            <p>Please confirm that you wish to remove this Trunk Leg.</p>
        </asp:Panel>
        
        <asp:Panel ID="pnlCannotRemoveTrunk" runat="server">
            <p>This trunk point is involved in the handling of orders on this groupage job, to change this job's journey please edit the job.</p>
        </asp:Panel>
    </div>
                    
    <div class="buttonbar">
        <asp:Button ID="btnRemoveTrunk" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
    
</asp:Content>