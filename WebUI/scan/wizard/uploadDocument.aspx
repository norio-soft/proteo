<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="uploadDocument.aspx.cs" Inherits="Orchestrator.WebUI.scan.wizard.uploadDocument" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <fieldset>
        <legend> Please select a document to upload </legend>

        <telerik:RadProgressManager id="radProgressManager" runat="server" />
        <asp:ValidationSummary id="valSummary" runat="server" />
            
        <table width="100%">
            <tr valign="top">
                <td valign="top" style="margin-top:20px;">
                    <telerik:RadAsyncUpload runat="server" ID="radUpload" ControlObjectsVisibility="None" 
                    MaxFileSize="10000000" 
                    MaxFileInputsCount="1" InputSize="65"
                    AllowedFileExtensions="pdf"
                    OnClientValidationFailed="validationFailed"
                    OnClientFileUploaded="fileuploaded"></telerik:RadAsyncUpload> 
                    <telerik:RadProgressArea id="radProgressArea" runat="server" DisplayCancelButton="true" ProgressIndicators="TotalProgressBar, TotalProgressPercent"  />
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlUpload" runat="server" ScrollBars="Auto" Height="300">
            <div class="buttonbar">
                <asp:Button runat="server" id="btnUpload" text="Upload" style="display:none" />
            </div>
            <div>
                <asp:Label ID="lblError" ForeColor="Red" runat="server"></asp:Label>
                <asp:Label ID="lblConfirmation" runat="server" ></asp:Label>
            </div>
        </asp:Panel>
    </fieldset>
    <div class="buttonbar">
        <asp:Button runat="server" id="btnBack" text="< Back" causesvalidation="False" />
        <asp:Button runat="server" id="btnNext" text="Next >" Enabled="false" causesvalidation="False" />
        &nbsp;&nbsp;
        <input type="button" value="Cancel" onclick="window.close()"/>
    </div>
</div>

<script type="text/javascript">
    function refreshParent() {
        opener.location.href = opener.location.href;
    }

    window.validationFailed = function (radAsyncUpload, args) {

        var $row = $(args.get_row());
        var erorMessage = getErrorMessage(radAsyncUpload, args);
        var span = createError(erorMessage);
        $row.addClass("ruError");
        $row.append(span);
    }

    function fileuploaded(sender, args) {
        var uploadButton = $get("<%= btnUpload.ClientID %>");
        uploadButton.style.display = "block";
    }

    function getErrorMessage(sender, args) {
        var fileExtention = args.get_fileName().substring(args.get_fileName().lastIndexOf('.') + 1, args.get_fileName().length);

        var uploadButton = $get("<%= btnUpload.ClientID %>");
        uploadButton.style.display = "none";

        if (args.get_fileName().lastIndexOf('.') != -1) {//this checks if the extension is correct
            if (sender.get_allowedFileExtensions().indexOf(fileExtention) == -1) {
                return ("You may only upload PDF files.<br />Please press Remove and select a .pdf document to upload.");
            }
            else {
                return ("This file exceeds the maximum allowed size.");
            }
        }
        else {
            return ("You may only upload files with a .pdf extension.<br />Please press Remove and select a .pdf document to upload.");
        }
    }

    function createError(erorMessage) {
        var input = '<br /><span class="ruErrorMessage">' + erorMessage + ' </span>';
        return input;
    }

</script>
</asp:Content>
