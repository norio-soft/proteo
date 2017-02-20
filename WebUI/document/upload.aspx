<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="upload.aspx.cs" Inherits="Orchestrator.WebUI.document.upload" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Upload Document</title>
    <link id="Link1" href="~/style/Styles.css" type="text/css" rel="stylesheet" runat="server" />
    <style type="text/css">
        body{margin:0;}
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server" ID="scriptManager"></asp:ScriptManager>
    <div>
        <table cellpadding="0" cellspacing="0" width="100%">
            <tr style="background-image: url(<%=Page.ResolveUrl("~/images/header_wizard_bg.gif")%>);
                background-repeat: repeat-x; height: 30px;">
                <td style="width: 32px;">
                    <img id="Img2" runat="server" src="~/images/monogram.gif" /></td>
                <td align="left" style="font-size: 12pt; color: White;">
                    <asp:Label ID="lblWizardTitle" runat="server">Job Details</asp:Label></td>
            </tr>
        </table>
            
        </div>
        <fieldset style="font-size:11px;margin-top:5px">
        <asp:Label runat="server" ID="lblDescription" >Please check/enter the details and select the file that you would like to upload - you can only upload PDF files.</asp:Label>
        </fieldset>
        <telerik:RadProgressManager id="radProgressManager" runat="server" />
        <asp:ValidationSummary  id="valSummary" runat="server" />
        
        <asp:Label ID="lblError" CssClass="Error" runat="server" Visible="false"></asp:Label>
        
        <table style="margin-top:20px;">
            <tr>
                <td><asp:label ID="lblTicketRef" runat="server" Text="Ticket/Reference No"></asp:label></td><td> <asp:TextBox ID="txtTicketNo" runat="server" Width="150"></asp:TextBox><asp:RequiredFieldValidator ID="rfvTicketNo" runat="server" ErrorMessage="Please enter the ticket/reference number." ControlToValidate="txtTicketNo" Display="None"></asp:RequiredFieldValidator></td>
            </tr>
            <tr>
                <td><asp:label ID="lblSignatureDate" runat="server" Text="Signature Date"></asp:label></td><td><telerik:RadDateInput runat="server" id="dteSignatureDate" Width="150"></telerik:RadDateInput></td>
            </tr>
            <tr>
                <td colspan="2" >
                     <telerik:RadUpload runat="server" ID="radUpload"  ControlObjectsVisibility="None" 
                    MaxFileSize="10000000" 
                    TargetFolder="~/files" MaxFileInputsCount="1" InputSize="48" />
                    <telerik:RadProgressArea id="radProgressArea" runat="server" Skin="Vista" DisplayCancelButton="true" ProgressIndicators="TotalProgressBar, TotalProgressPercent"  />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="wizardbuttonbar" style="text-align: left;">
                        <asp:Button ID="btnUpload" runat="server" Text="Upload File" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClientClick="window.close(); return false;" />
                    </div>
                </td>
            </tr>
        </table>
        
       
       
     <script type="text/javascript">
        function refreshParent()
        {
            opener.location.href = opener.location.href;
        }
    </script>
    </form>
</body>
</html>
