<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.scan.emailPDF" Codebehind="emailPDF.aspx.cs" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Email PDF</title>
    <link rel="stylesheet" type="text/css" href="../style/styles.css" />
</head>
<body leftmargin="0" topmargin="0" bottommargin="0" rightmargin="0">
    <form id="form1" runat="server">
        <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td class="myHeading" width="100%">
                    <img id="Img3" src="../images/cornerLeftYellow.gif" alt="" border="0" /></td>
                <td>
                    <img id="Img4" src="../images/corner_Right.gif" alt="" border="0" style="width: 10px;" /></td>
            </tr>
            <tr>
                <td colspan="2" width="100%">
                    <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" >
                        <tr>
                            <td colspan="2" class="pageHeadingDefault myTitle" style="border-right: medium none;
                                border-top: medium none; border-left: medium none; border-bottom: medium none">
                               Email PDF
                            </td>
                        </tr>
                        <tr>
                            <td class="greyText" width="100%" valign="top" height="90px">
                                <div style="height: 90px; width: 100%; overflow: auto;">
                                    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
                                        <div class="MessagePanel">
                                            <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /><asp:Label
                                                ID="lblMessage" runat="server">The PDF could not be emailed, please try again.  Make sure the PDF is available and has been uploaded.</asp:Label>
                                        </div>
                                    </asp:Panel>
                                    <table border="0">
                                        <tr valign=top>
                                            <td colspan="2">Please supply the email address you would like this PDF to be emailed to.</td>
                                        </tr>
                                        <tr valign=top height="20">
                                            <td>
                                                Email Address
                                            </td>
                                            <td>
                                                <asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox>
                                                <asp:RequiredFieldValidator ID="rfvEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Please supply a valid email address" Display="Dynamic"><img src="../images/error.gif" alt="Please supply a valid email address." /></asp:RequiredFieldValidator>
                                                <asp:RegularExpressionValidator ID="revEmailAddress" runat="server" ControlToValidate="txtEmailAddress" ErrorMessage="Please supply a valid email address" Display="Dynamic"><img src="../images/error.gif" alt="Please supply a valid email address." /></asp:RegularExpressionValidator>
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
                        <asp:Button ID="btnConfirm" runat="server" Text="Email" Width="75"></asp:Button>
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
                    </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>