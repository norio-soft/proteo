<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.ManageBusinessType" Title="Manage Business Type" Codebehind="managebusinesstype.aspx.cs" %>
<%@ Register Src="~/UserControls/businessruleinfringementdisplay.ascx" TagPrefix="uc" TagName="InfringementDisplay" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1><asp:Label ID="lblTitle" runat="server" Font-Bold="false" Font-Size="Small" Text="Manage Business Type"></asp:Label></h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>From here you can add or update the settings for a Business Type</h2>
    <uc:InfringementDisplay runat="server" ID="ucInfringements" />
    <fieldset>
        <table>
            <tr>
                <td>Description:</td>
                <td><asp:TextBox ID="txtDescription" runat="server" Width="400"></asp:TextBox><asp:RequiredFieldValidator ID="rfvDescription" runat="server" ControlToValidate="txtDescription" ErrorMessage="Please enter a description for the Order Type"><img runat="server" src="~/images/error.png" title="Please enter a description." /></asp:RequiredFieldValidator></td>            
            </tr>
            <tr>
                <td>Do you want to capture Driver Debriefs for this Business Type?</td>
                <td><asp:RadioButtonList ID="rblDeBreifRequired" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes" Selected="True"></asp:ListItem><asp:ListItem Value="False" Text="No"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>Show the Create Job checkbox on the Add Order Screen?</td>
                <td><asp:RadioButtonList ID="rblShowCreateJob" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes" Selected="True"></asp:ListItem><asp:ListItem Value="False" Text="No"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>Default the Create Job checkbox to Checked?</td>
                <td><asp:RadioButtonList ID="rblCreateJobChecked" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes" Selected="True"></asp:ListItem><asp:ListItem Value="False" Text="No"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr id="trConsortium" runat="server" visible="false">
                <td>Exclude this business type from consortium member file export?</td>
                <td><asp:RadioButtonList ID="rblExcludeFromConsortiumMemberFileExport" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <table>
            <tr>
                <td>Minimum Pallets</td>
                <td><asp:TextBox runat="server" ID="txtPalletThresholdMin" Width="25"></asp:TextBox><asp:RequiredFieldValidator ID="rfvPalletThresholdMin" runat="server" ControlToValidate="txtPalletThresholdMin" ErrorMessage="Please enter the minimum number of pallets"><img runat="server" src="~/images/error.gif" title="Please enter the minimum number of pallets" /></asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="revPalletThresholdMin" runat="server" ControlToValidate="txtPalletThresholdMin" ValidationExpression="\d*"><img runat="server" src="~/images/error.gif" title="Please enter a valid number between 1 and 60" /></asp:RegularExpressionValidator> </td>
            </tr>
            <tr>
                <td>Maximum Pallets</td>
                <td><asp:TextBox runat="server" ID="txtPalletThresholdMax" Width="25"></asp:TextBox><asp:RequiredFieldValidator ID="rfvPalletThresholdMax" ControlToValidate="txtPalletThresholdMax" runat="server" ErrorMessage="Please enter the minimum number of pallets"><img id="Img1" runat="server" src="~/images/error.gif" title="Please enter the minimum number of pallets" /></asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="revPallethThresholdMax" ControlToValidate="txtPalletThresholdMax" runat="server" ValidationExpression="\d*"><img id="Img2" runat="server" src="~/images/error.gif" title="Please enter a valid number between 1 and 60" /></asp:RegularExpressionValidator> </td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <table>
            <tr>
                <td colspan="2"><asp:CheckBox runat="server" ID="chkPalletNetwork" Text="Pallet network?" /></td>
            </tr>
            <tr>
                <td>Pallet network outbound Requesting and Collection depot</td>
                <td><asp:TextBox runat="server" ID="txtPalletNetworkExportDepotCode" Width="30"></asp:TextBox></td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <table>
            <tr>
                <td>Default Nominal Code</td>
                <td><telerik:RadComboBox ID="cboDefaultNominalCode" runat="server" Skin="WindowsXP" HighlightTemplatedItems="true" Width="230px">
                    <HeaderTemplate>
                        <table style="width:230px; text-align:left;">
                            <tr><td style="width:80px;">Code</td><td style="width:150px;">Description</td></tr>
                        </table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table>
                            <tr><td style="width:80px;"><%#DataBinder.Eval(Container.DataItem, "NominalCode")%></td><td style="width:150px;"><%#DataBinder.Eval(Container.DataItem, "Description") %></td></tr>
                        </table>
                    </ItemTemplate>
                </telerik:RadComboBox></td>
            </tr>
            <tr>
                <td>Sub-contract Nominal Code</td>
                <td><telerik:RadComboBox ID="cboSubContractNominalCode" runat="server" Skin="WindowsXP" HighlightTemplatedItems="true" Width="230px">
                    <HeaderTemplate>
                        <table style="width:230px; text-align:left;">
                            <tr><td style="width:80px;">Code</td><td style="width:150px;">Description</td></tr>
                        </table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table>
                            <tr><td style="width:80px;"><%#DataBinder.Eval(Container.DataItem, "NominalCode")%></td><td style="width:150px;"><%#DataBinder.Eval(Container.DataItem, "Description") %></td></tr>
                        </table>
                    </ItemTemplate>
                </telerik:RadComboBox></td>
            </tr>
            <tr>
                <td>Sub-contract Cost (Self Bill) Nominal Code </td>
                <td><telerik:RadComboBox ID="cboSubContractSelfBillNominalCode" runat="server" Skin="WindowsXP" HighlightTemplatedItems="true" Width="230px">
                    <HeaderTemplate>
                        <table style="width:230px; text-align:left;">
                            <tr><td style="width:80px;">Code</td><td style="width:150px;">Description</td></tr>
                        </table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <table>
                            <tr><td style="width:80px;"><%#DataBinder.Eval(Container.DataItem, "NominalCode")%></td><td style="width:150px;"><%#DataBinder.Eval(Container.DataItem, "Description") %></td></tr>
                        </table>
                    </ItemTemplate>
                </telerik:RadComboBox></td>
            </tr>
        </table>
    </fieldset>
    <fieldset>
        <table style="vertical-align: text-top;">
            <tr>
                <td colspan="5">Mobile Worker Flow Settings</td>
            </tr>
            <tr>
                <td>Scan barcodes on delivery</td>
                <td><asp:RadioButtonList ID="rblIsBarcodeScannedOnDelivery" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>Capture customer name on collection</td>
                <td><asp:RadioButtonList ID="rblIsCustomerNameCapturedOnCollection" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
                <td>&nbsp;</td>
                <td>Capture customer name on delivery</td>
                <td><asp:RadioButtonList ID="rblIsCustomerNameCapturedOnDelivery" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>Capture customer signature on collection</td>
                <td><asp:RadioButtonList ID="rblIsCustomerSignatureCapturedOnCollection" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
                <td>&nbsp;</td>
                <td>Capture customer signature on delivery</td>
                <td><asp:RadioButtonList ID="rblIsCustomerSignatureCapturedOnDelivery" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>Bypass clean/claused screen</td>
                <td><asp:RadioButtonList ID="rblMwfBypassCleanClausedScreen" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
                <td>&nbsp;</td>
                <td>Bypass comments screen</td>
                <td><asp:RadioButtonList ID="rblMwfBypassCommentsScreen" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
            </tr>
            <tr>
                <td>Confirm Call In Times</td>
                <td><asp:RadioButtonList ID="rblMwfConfirmCallIn" runat="server" RepeatDirection="Horizontal"><asp:ListItem Value="True" Text="Yes"></asp:ListItem><asp:ListItem Value="False" Text="No" Selected="True"></asp:ListItem></asp:RadioButtonList></td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </fieldset>
    <div class="buttonbar">
        <asp:Button ID="btnUpdate" runat="server" Text="Add" Width="75" />
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="false" />
        <asp:Button ID="btnList" runat="server" Text="Business Type List" OnClientClick="location.href='businesstypes.aspx'; return false;" />
    </div>
</asp:Content>