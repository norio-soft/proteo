<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="addMultiDestination.aspx.cs"
    Inherits="Orchestrator.WebUI.Job.addMultiDestination" Title="Add Multiple Destinations"
    MasterPageFile="~/WizardMasterPage.Master" %>

<%@ Register Src="../UserControls/MultiTrunk.ascx" TagName="MultiTrunk" TagPrefix="uc1" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">
    Add Multiple Destinations</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript">
            <!--
        try {
            //resizeTo(730, 500);
            //moveTo(30, 20);
        }
        catch (err) { }

        window.focus();
            -->
    </script>

    <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%">
        <tr>
            <td colspan="2" class="pageHeadingDefault myTitle" style="border-right: medium none;
                border-top: medium none; border-left: medium none; border-bottom: medium none">
                Multi-Destination
            </td>
        </tr>
        <tr>
            <td>
                <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <div style="border-bottom: solid 1pt silver; font-size: 14px; color: #5d7b9d; margin: 5px;">
                    <asp:Label ID="lblTitle" runat="server" Text="Select your multi-trunk"></asp:Label>
                </div>
                <fieldset>
                    <uc1:MultiTrunk ID="ucMultiDestination" runat="server" />
                </fieldset>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td>
                <div class="buttonbar">
                    <asp:Button ID="btnConfirm" runat="server" Text="Add Destinations" Width="125" OnClick="btnConfirm_Click1">
                    </asp:Button>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False"
                        OnClick="btnCancel_Click" />
                </div>
            </td>
        </tr>
    </table>
</asp:Content>
