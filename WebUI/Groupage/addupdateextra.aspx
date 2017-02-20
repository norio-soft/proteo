<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.OrderExtra" MasterPageFile="~/WizardMasterPage.master" Codebehind="addupdateextra.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add / Update Extra</asp:Content>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:panel ID="pnlError" runat="server" Visible="false">
        <div style="margin-top: 10px;  font-family: Trebuchet MS, Arial, Helvetica;    font-size: 1em;    font-weight: bold;	background-color:#FDE8E9;	border:2px solid #9E0B0E;	padding:2px; 	color: #9E0B0E;">
            <img src="../images/status-red.gif"  align="middle"/><asp:Label ID="lblError" runat="server"></asp:Label>
        </div>
    </asp:panel>
    <div runat="server" id="divInvoicedMessage"><asp:Label ID="lblInvoiceReason" runat="server"></asp:Label></div>
    <table>
        <tr runat="server" id="trClient">
            <td>
                Customer to charge 
            </td>
            <td>
                <telerik:RadComboBox runat="server" ID="cboClient" DataTextField="CustomerOrganisationName" DataValueField="OrderID"></telerik:RadComboBox> 
            </td>
        </tr>
        <tr>
            <td>Extra Type</td>
            <td><asp:DropDownList ID="cboExtraType" runat="server" AutoPostBack="true"></asp:DropDownList></td>
        </tr>
        <tr>
            <td>State</td>
            <td><asp:DropDownList ID="cboExtraState" runat="server" ></asp:DropDownList></td>
        </tr>
        <tr>
            <td>Client Contact</td>
            <td><asp:TextBox ID="txtClientContact" runat="server" Width="280"></asp:TextBox></td>
        </tr>
        <asp:Panel ID="pnlComment" runat="server" Visible="true">
        <tr valign="top">
            <td>Comments</td>
            <td><asp:TextBox ID="txtComment" runat="server" Width="280" Rows="3" TextMode="MultiLine" ></asp:TextBox></td>
        </tr>
        </asp:Panel>
        <tr>
            <td>Amount</td>
            <td>
                <telerik:RadNumericTextBox ID="rntAmount" runat="server" Width="75" Type="Currency" />                 
                <asp:RequiredFieldValidator runat="server" ID="rfvAmount" ControlToValidate="rntAmount" Display="dynamic" ErrorMessage="<img src='../images/error.gif' tooltip='Plese enter a valid money amount.' />" EnableClientScript="true"></asp:RequiredFieldValidator>
            </td>
        </tr>
    </table>
    <div class="wizardbuttonbar">
        <asp:Button ID="btnAddExtra" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnDeleteExtra" runat="server" Text="Delete" Width="75" CausesValidation="False" />
        <input type="button" onclick="window.close();" value="Close" />
    </div>
    <asp:Label ID="lblInjectScript" runat="server"></asp:Label>
</asp:Content>