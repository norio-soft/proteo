<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApproveOrderPoint.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.ApproveOrderPoint" MasterPageFile="~/WizardMasterPage.master" Title="Approve Order Point" %>

<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>
<%@ Register TagPrefix="uc" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<telerik:RadCodeBlock runat="server">
    <script language="javascript" type="text/javascript" src="/script/scripts.js"></script>
    <script language="javascript" type="text/javascript" src="/script/tooltippopups.js"></script>
<%--    <script language="javascript" type="text/javascript">
        function RequestStart(sender, args) {
            if (hidePageShowLoading != null) {
                hidePageShowLoading();
            }
            alert(RequestStart);
        }

        function RequestEnd() {
            if (showPageHideLoading != null) {
                showPageHideLoading();
            }
        }
    </script>--%>
</telerik:RadCodeBlock>
    <table>
        <tr>
            <td valign="top">
                <asp:Panel runat="server" ID="pnlApprovePoint">
                    <table style="height: 135px; width: 530px;" class="form-value" cellpadding="5px">
                        <tr>
                            <td valign="top">
                                <div style="border-bottom: solid 1pt silver; padding: 4px; color: #ffffff; background-color: #99BEDE;
                                    text-align: left;">
                                    <asp:Label runat="server" ID="lblProposedPoint"></asp:Label>
                                </div>
                                <asp:Panel ID="pnlFullAddress" runat="server" Visible="true">
                                    <div style="padding: 4px; background-color: white; width: 510px; text-align: left;
                                        font-size: 12px">
                                        <asp:Label ID="lblFullAddress" runat="server" Visible="True" Text="Full Address" />
                                    </div>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td valign="bottom">
                                <div class="buttonbar">
                                    <asp:Button Width="100px" runat="server" ID="btnApprovePoint" Text="Approve point" OnClick="btnApprovePoint_Click" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlRejectPoint" Visible="true">
                    <table style="width: 530px; height: 340px;" class="form-value" visible="false" cellpadding="5px">
                        <tr>
                            <td valign="top">
                                <div style="border-bottom: solid 1pt silver; padding: 4px; color: #ffffff; background-color: #99BEDE;
                                    text-align: left;">
                                    <asp:Label runat="server" ID="lblAlternativePoint"></asp:Label>
                                </div>
                                <div style="width: 400px;">
                                    <uc:Point ID="ucPoint" runat="server" CanChangePoint="true" CanClearPoint="true"
                                        CanCreateNewPoint="true" ShowFullAddress="true" PointSelectionRequired="false" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td valign="bottom">
                                <div class="buttonbar">
                                    <asp:Button runat="server" ID="btnSubmitAlternative" Text="Submit alternative" OnClick="btnSubmitAlternative_Click" />
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
                    <asp:Button runat="server" ID="btnCloseForm" Text="Cancel - Close Form" OnClick="btnCloseForm_Click" />
                </div>
            </td>
        </tr>
    </table>
   
</asp:Content>
