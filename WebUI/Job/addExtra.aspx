<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.AddExtra" Codebehind="addExtra.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Add Extra To Job" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add Extra</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   
    <div>
        <asp:panel ID="pnlError" runat="server" Visible="false">
            <div style="margin-top: 10px;  font-family: Trebuchet MS, Arial, Helvetica;    font-size: 1em;    font-weight: bold;	background-color:#FDE8E9;	border:2px solid #9E0B0E;	padding:2px; 	color: #9E0B0E;">
                <img src="../images/status-red.gif"  align="middle"/><asp:Label ID="lblError" runat="server"></asp:Label>
            </div>
        </asp:panel>
        <table>
            <tr>
                <td>Customer to charge</td>
                <td><telerik:RadComboBox ID="cboClient" runat="server" DataTextField="CustomerOrganisationName" DataValueField="OrderID"></telerik:RadComboBox></td> 
            </tr>
            <tr>
                <td>Extra Type</td>
                <td><asp:DropDownList ID="cboExtraType" runat="server" AutoPostBack="true"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>State</td>
                <td><asp:DropDownList ID="cboExtraState" runat="server" AutoPostBack="true"></asp:DropDownList></td>
            </tr>
            <tr>
                <td>Client Contact</td>
                <td><asp:TextBox ID="txtClientContact" runat="server" Width="280"></asp:TextBox></td>
            </tr>
            <asp:Panel ID="pnlCustomExtra" runat="server" Visible="false">
            <tr>
                <td>Custom Description</td>
                <td><asp:TextBox ID="txtCustomDescription" runat="server" Width="200px"></asp:TextBox></td>
            </tr>
            </asp:Panel>
            <asp:Panel ID="pnlDemurrageComments" runat="server" Visible="false">
            <tr valign="top">
                <td>Comments</td>
                <td><asp:TextBox ID="txtDemurrageComments" runat="server" Width="280" Rows="3" TextMode="MultiLine" ></asp:TextBox></td>
            </tr>
            </asp:Panel>
            <tr>
                <td>Amount</td>
                <td><asp:TextBox ID="txtAmount" runat="server" Width="75"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="rfvAmount" ControlToValidate="txtAmount" Display="dynamic" ErrorMessage="<img src='../images/error.gif' tooltip='Plese enter a valid money amount.' />" EnableClientScript="true"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator  runat="server" ID="revAmount" ControlToValidate="txtAmount" ValidationExpression="^(£|-£|£-|-)?([0-9]{1}[0-9]{0,2}(\,[0-9]{3})*(\.[0-9]{0,2})?|[1-9]{1}[0-9]{0,}(\.[0-9]{0,2})?|0(\.[0-9]{0,2})?|(\.[0-9]{1,2})?)$" ErrorMessage="<img src='../images/error.gif' tooltip='Plese enter a valid money amount.' />" EnableClientScript="true" Display="dynamic"></asp:RegularExpressionValidator>
                </td>
            </tr>
        </table>
    </div>

    <div class="buttonbar">
        <asp:Button ID="btnAddExtra" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnDeleteExtra" runat="server" Text="Delete" Width="75" CausesValidation="False" />
        <asp:Button ID="btnClose" runat="server" Text="Cancel" Width="75" visible="true" CausesValidation="false"/>
    </div>
    
</asp:Content>