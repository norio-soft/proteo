<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.setBatchInvoiceDate" Codebehind="setBatchInvoiceDate.aspx.cs" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Set Batch Invoice Date</title>
    <link rel="stylesheet" type="text/css" href="../style/styles.css" />
</head>
<body leftmargin="0" topmargin="0" bottommargin="0" rightmargin="0">
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td class="myHeading" width="100%">
                    <img id="Img3" src="../images/cornerLeftYellow.gif" alt="" border="0" /></td>
                <td>
                    <img id="Img4" src="../images/corner_Right.gif" alt="" border="0" style="width: 10px;" /></td>
            </tr>
            <tr>
                <td colspan="2" width="100%">
                    <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" height="150">
                        <tr>
                            <td colspan="2" class="pageHeadingDefault myTitle" style="border-right: medium none;
                                border-top: medium none; border-left: medium none; border-bottom: medium none">
                                Set Batch Invoice Date
                            </td>
                        </tr>
                        <tr>
                            <td class="greyText" width="100%" valign="top">
                                <div style="height: 150px; width: 100%; overflow: auto;">
                                    <table width="99%">
                                        <tr>
                                            <td width="25%" valign="middle">Invoice Date:</td>
                                            <td>
                                                <telerik:RadDateInput id="dteInvoiceDate" runat="server" dateformat="dd/MM/yy" ToolTip="The invoice date." Width="50px" Nullable="False"></telerik:RadDateInput>
                                            </td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="rfvInvoiceDate" runat="server" ControlToValidate="dteInvoiceDate" ErrorMessage="Please supply a valid invoice date."><img src="../images/error.gif" alt="Please supply a valid invoice date." /></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                                <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false">
                                </cs:WebModalWindowHelper>
                            </td>
                        </tr>
                     </table>
                    </td>
                   </tr>
                    <tr>
                        <td colspan="2">
                            <div class="wizardbuttonbar">
                                <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
                            </div>
                        </td>
                    </tr>
                </table>
    </form>
</body>
</html>
