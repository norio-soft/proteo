<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Resource.SetLocation" Codebehind="setLocation.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Subcontract Job</title>
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
                    <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" height="250">
                        <tr>
                            <td colspan="2" class="pageHeadingDefault myTitle" style="border-right: medium none;
                                border-top: medium none; border-left: medium none; border-bottom: medium none">
                                Set Location
                            </td>
                        </tr>
                        <tr>
                            <td class="greyText" width="100%" valign="top">
                                <div style="height: 220px; width: 100%; overflow: auto;">
                                    <asp:Panel ID="pnlSubContract" runat="server">
                                        
                                    </asp:Panel>
                                </div>
                                
                            </td>
                        </tr>
                     </table>
                    </td>
                   </tr>
                    <tr>
                        <td colspan="2">
                            <div class="wizardbuttonbar">
                                <asp:Button ID="btnSetLocation" runat="server" Text="Confirm" Width="75"></asp:Button>
                                <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
                            </div>
                        </td>
                    </tr>
                </table>
    </form>
</body>
</html>