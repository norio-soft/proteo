<%@ Page Language="C#" AutoEventWireup="true" Inherits="Traffic_subcontractleg" MasterPageFile="~/WizardMasterPage.master" Title="SubContract Leg or Job" Codebehind="subcontractleg.aspx.cs" %>
<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:Panel ID="pnlSubContract" runat="server">
        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
        <table width="99%">
            <tr >
                <td width="25%">
                    Sub-Contract to:</td>
                <td>
                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false" 
                                         MarkFirstMatch="true" Skin="WindowsXP" AllowCustomText="false" ItemRequestTimeout="500" Width="155px" >
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvSubContractor" runat="server" ControlToValidate="cboSubContractor"
                                                ErrorMessage="<img src='../images/error.png' Title='Please select a Sub-Contractor. alt='Error''>" 
                                                EnableClientScript="True">
                    </asp:RequiredFieldValidator>
                    <asp:CustomValidator ID="cfvSubContrator" runat="server" ControlToValidate="cboSubContractor"
                                         ErrorMessage="<img src='../images/error.png' title='Please select a Sub-Contractor.' alt='Error' />" 
                                         EnableClientScript="False">
                    </asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td>
                    Sub-Contract Rate</td>
                <td>
                    <asp:TextBox ID="txtSubContractRate" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvSubContractorRate" runat="server" ControlToValidate="txtSubContractRate"
                                                ErrorMessage="Please supply a rate."><img src="../images/Error.gif" height="16" 
                                                width="16" title="Please supply a rate." alt="Error" />
                    </asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="revSubContractorRate" runat="server" ControlToValidate="txtSubContractRate"
                                                    ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$"
                                                    ErrorMessage="Please enter a valid currency value for the rate.">
                                                    <img src="../../images/Error.gif" height="16" width="16" alt="Error"
                                                    title="Please enter a valid currency value for the rate." />
                    </asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td nowrap="nowrap">
                    Use Sub-Contractor's Trailer</td>
                <td>
                    <asp:CheckBox ID="chkUseSubContractorTrailer" runat="server" Checked="True" AutoPostBack="True"></asp:CheckBox>
                </td>
            </tr>
            <tr id="trHaulierTrailer" runat="server">
                <td nowrap="nowrap">
                    Use your Trailer
                </td>
                <td>
                    <telerik:RadComboBox ID="cboTrailer" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false" 
                    MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="155px" Height="100px" AllowCustomText="false">
                    </telerik:RadComboBox>
                    <asp:RequiredFieldValidator ID="rfvTrailer" runat="server" ControlToValidate="cboTrailer" 
                    ErrorMessage="<img src='../images/error.png' Title='Please select a Trailer.'>" EnableClientScript="True">
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="3" height="50">&nbsp;</td>
            </tr>
        </table>
        
        <table>
            <tr>
                <td> SubContract Whole Job</td>
            </tr>
        </table>
        <table>
            <tr>
                <td>Legs for Job</td>
            </tr>
        </table>
    </asp:Panel>
    <telerik:RadAjaxManager ID="raxmanager" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="chkUseSubContractorTrailer">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="trHailerTrailer" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>