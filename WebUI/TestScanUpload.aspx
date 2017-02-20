<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestScanUpload.aspx.cs" Inherits="Orchestrator.WebUI.TestScanUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <table>
        <tr>
            <td>
                <input type="button" ID="btnScanNewPod" value="Scan New Pod" runat="server" onclick="ScanPodNew();">
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnScanPodAndReplace" value="Scan Pod Replace Existing" runat="server" onclick="ScanPodExisting();">
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnScanPodAppendToExisting" value="Scan Pod Append to Existing" runat="server" onclick="ScanPodExisting();">
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnUploadPod" value="Upload New Pod" runat="server" onclick="UploadPodNew();">
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnUploadAndReplace" value="Upload And Replace Existing Pod" runat="server" onclick="UploadPodExisting();">
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnScanBookingForm" value="Scan New Booking Form" runat="server" onclick="ScanNewBookingForm();" />
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnReScanBookingForm" value="Re-Scan Booking Form" runat="server" onclick="ReScanBookingForm();" />
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnScanPCV" value="Scan PCV" runat="server" onclick="ScanPCV();" />
            </td>
        </tr>
        <tr>
            <td>
                <input type="button" ID="btnReScan" value="Re-Scan PCV" runat="server" onclick="ReScanPCV();" />
            </td>
        </tr>
                <tr>
            <td>
                <asp:Button runat="server" ID="btnTestEmail" Text="Test Email" />
            </td>
        </tr>
    </table>
    </div>
    </form>
</body>

<script>

    var returnUrlFromPopUp = window.location;

    function ScanPodNew()
    {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=2&JobId=27988&CollectDropId=117128") %>';
        openDialog(url, 510, 455, null);
    }

    function ScanPodExisting() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=2&JobId=27937&ScannedFormId=113334") %>';
        openDialog(url, 510, 500, null);
    }

    function UploadPodNew() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=2&JobId=27988&CollectDropId=117128") %>';
        openDialog(url, 510, 500, null);
    }

    function UploadPodExisting() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=2&JobId=27937&ScannedFormId=113334") %>';
        openDialog(url, 510, 500, null);
    }

    function ScanNewBookingForm() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=3&OrderId=51211") %>';
        openDialog(url, 510, 500, null);
    }

    function ReScanBookingForm() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=3&OrderId=51211&ScannedFormId=113340") %>';
        openDialog(url, 510, 500, null);
    }
    
    function ScanPCV() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=1&JobId=27937&ScannedFormId=113364") %>';
        openDialog(url, 510, 500, null);
    }
    
    function ReScanPCV() {
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=1&JobId=27937&ScannedFormId=113364") %>';
        openDialog(url, 510, 500, null);
    }
    
</script>
   
</html>
