<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Job.bulkchangerate" CodeBehind="bulkchangerate.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>Bulk Rate Change</h1>
    <h2><asp:Label ID="lblTitle" runat="server" Text="Makes a bulk change of rates to all active rates."></asp:Label></h2>
    <asp:Label ID="lblConfirmation" runat="server" Text="" CssClass="confirmation" Visible="false"></asp:Label> 
    <fieldset>
        <p>This operation will adjust <b>all</b> or <b>one client's</b> currently active rates
         by the percentage you supply in the text box below, and these changes will be applied immediately.</p>
        <table>
            <tr>
                <td class="formCellLabel">Client</td>
                <td class="formCellInput">
                    <telerik:radcombobox id="cboOrganisation" runat="server" enableloadondemand="true" itemrequesttimeout="500"
                        markfirstmatch="true" radcontrolsdir="~/script/RadControls/" allowcustomtext="true"
                        showmoreresultsbox="false" skin="WindowsXP" width="355px" overlay="true" zindex="50"
                        height="275px">
                    </telerik:radcombobox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Increase Rates by</td>
                <td class="formCellInput">
                    <table cellpadding="0" cellspacing="0">
                        <tr>
                            <td align="left">
                                <asp:TextBox ID="txtIncreaseBy" runat="server" Width="50px"></asp:TextBox>%
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="rfvIncreaseBy" runat="server" ControlToValidate="txtIncreaseBy"
                                    ErrorMessage="Please supply a percentage to increase the rates by."><img src="../images/Error.gif" height="16" width="16" title="Please supply a percentage to increase the rates by." /></asp:RequiredFieldValidator>
                                <asp:CustomValidator ID="cfvIncreaseBy" runat="server" ControlToValidate="txtIncreaseBy"
                                    EnableClientScript="False" ErrorMessage="Please supply a percentage to increase the rates by."><img src="../images/error.gif" title="Please supply a percentage to increase the rates by." /></asp:CustomValidator>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button ID="btnSubmit" runat="server" Text="Apply Rate Increase"></asp:Button>
    </div>
</asp:Content>
