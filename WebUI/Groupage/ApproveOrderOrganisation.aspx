<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveOrderOrganisation.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.ApproveOrderOrganisation" MasterPageFile="~/WizardMasterPage.master" Title="Approve Order Point" %>

<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadCodeBlock runat="server">
    <script language="javascript" type="text/javascript" src="/script/scripts.js"></script>
    <script language="javascript" type="text/javascript" src="/script/tooltippopups.js"></script>
</telerik:RadCodeBlock>
    <table>
        <tr>
            <td>
                <asp:Panel runat="server" ID="pnlApproveOrganisation">
                    <table style="height: 135px; width: 530px;" class="form-value" cellpadding="5px">
                        <tr>
                            <td>
                                <div style="border-bottom: solid 1pt silver; padding: 4px; color: #ffffff; background-color: #99BEDE;
                                    text-align: left;">
                                    <asp:Label runat="server" ID="lblUnnaprovedOrganisation"></asp:Label>
                                </div>
                                <asp:Panel ID="pnlFullAddress" runat="server" Visible="true">
                                    <div style="padding: 4px; background-color: white; width: 510px; text-align: left;
                                        font-size: 12px">
                                        <asp:Label ID="lblNameAndStatus" runat="server" Visible="True" Text="Organisation and Status" />
                                    </div>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="buttonbar">
                                    <asp:Button Width="150px" runat="server" ID="btnApproveOrganisation" Text="Approve Organisation" OnClick="btnApproveOrganisation_Click" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" />
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <div class="buttonBar">
                    <asp:Button runat="server" ID="btnCloseForm" Text="Cancel" OnClick="btnCloseForm_Click" />
                </div>
            </td>
        </tr>
    </table>
   
</asp:Content>
