<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.updateResourceLocations" Codebehind="updateResourceLocations.aspx.cs" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Update Resource Locations</title>
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
                    <table cellspacing="2" class="greyBorder myTrafficDesk" width="100%" height="150">
                        <tr>
                            <td colspan="2" class="pageHeadingDefault myTitle" style="border-right: medium none; border-top: medium none; border-left: medium none; border-bottom: medium none">Update Resource Locations</td>
                        </tr>
                        <tr>
                            <td class="greyText" width="100%" valign="top">
                                <div style="height: 220px; width: 100%; overflow: auto;">
                                    <table height="120" border="0">
                                        <tr runat="server" visible="false" id="trErrors">
                                            <td colspan="99"><uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" /></td>
                                        </tr>
                                        <tr valign=top>
                                            <td colspan="2">This will update the following resources' last locations as being at the following point.</td>
                                        </tr>
                                        <tr valign="top">
                                            <td width="50%"><b>Resources Affected</b></td>
                                            <td width="50%"><b>Point</b></td>
                                        </tr>
                                        <tr valign="top">
                                            <td>
                                                <table width="100%" border="0" cellpadding="0" cellspacing="1">
                                                    <tr>
                                                        <td>Driver:</td>
                                                        <td id="tdDriver" runat="server"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Vehicle:</td>
                                                        <td id="tdVehicle" runat="server"></td>
                                                    </tr>
                                                    <tr>
                                                        <td>Trailer:</td>
                                                        <td id="tdTrailer" runat="server"></td>
                                                    </tr>
                                                </table>
                                            </td>
                                            <td id="tdPoint" runat="server"></td>
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
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" />
                    </div>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>