<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage.cancelOrder" MasterPageFile="~/WizardMasterPage.master" Title="Cancel Order" Codebehind="cancelOrder.aspx.cs" %>
<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script lanuage="javascript" type="text/javascript" src="../script/scripts.js"></script>
    <style type="text/css">
        td.form-label {font-size:10px; font-weight:bold;}
    </style>

    <asp:Panel ID="pnlCancellationMessage" runat="server" CssClass="warningPanel">
        <asp:Label ID="lblCancellationMessage" runat="server" />
    </asp:Panel>
    
    <asp:Panel ID="pnlCancellationReason" runat="server">
        <fieldset style="padding:0px; margin-top:5px; margin-bottom:5px;">
            <div style="height:20px; border-bottom:solid 1pt silver;padding-top:4px;padding-left:2px;margin-bottom:0px; color:#ffffff; background-color:#5d7b9d;font-weight:bold;">Cancellation Reason</div>
            <table style="font-size:12px;" cellspacing="0">
                <tr>
                    <td class="form-value">
                        <asp:DropDownList ID="cboCancellationReason" runat="server" AppendDataBoundItems="true" Width="250px">
                            <asp:ListItem Value="" Text="- select -" />
                        </asp:DropDownList>
                        <asp:TextBox ID="txtCancellationReason" runat="server" TextMode="MultiLine" MaxLength="4000" Rows="6" Columns="60" />
                    </td>
                    <td valign="top">
                        <asp:CustomValidator ID="cvCancellationReason" runat="server" ClientValidationFunction="validateCancellationReason" Display="Dynamic" ErrorMessage="Please supply a cancellation reason" />
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    
    <div style="height:22px; margin-top:5px;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">
        <input type="button" id="btnClose" value="Close" onclick="CloseOnReload()" style="width:75px;" />
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75px" />
    </div>
    
    <script language="javascript" type="text/javascript">
        function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow; //IE (and Moz az well) 
            return oWindow;
        }

        function CloseOnReload() {
            var oWindow = GetRadWindow();
            oWindow.BrowserWindow.location.reload();
            oWindow.Close();
        }

        function validateCancellationReason(sender, eventArgs) {
            eventArgs.IsValid = $("#<%= cboCancellationReason.ClientID %>").val() || $("#<%= txtCancellationReason.ClientID %>").val();
        }
    </script>
</asp:Content>