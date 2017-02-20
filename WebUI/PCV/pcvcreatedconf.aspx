<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pcvcreatedconf.aspx.cs"   Inherits="Orchestrator.WebUI.PCV.pcvcreatedconf" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <base target="_self" />
    <title>PCV Confirmation</title>
    <script language="javascript" type="text/javascript" src="/script/scripts.js"></script>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        function OpenPCVWindowForEdit() {
            var qs = "ScannedFormTypeId=1&ScannedFormId=<%=this.ScannedFormID %>";
            <%=dlgScanDocument.ClientID %>_Open(qs);
        }
    </script>
    </telerik:RadCodeBlock>
</head>
<body class="masterpagelessPage">
    <form id="form1" runat="server">
        <cc1:Dialog ID="dlgScanDocument" runat="server" URL="/scan/wizard/ScanOrUpload.aspx" Height="550" Width="500" Mode="Modal" AutoPostBack="false" ReturnValueExpected="false" />
    
        <div style="width:100%;">
            <div style="width: 320px; padding: 0 5px; margin:0 auto 0 auto;">
                <div id="OrderID">                    
                    <h3><asp:Label ID="lblClientConfirmationMessage" Visible="false" runat="server" Text="Your pcv has been created." /></h3>
                    <h4>PCV ID: <asp:Label ID="lblPCVID" runat="server"></asp:Label></h4>
                </div>
                <div class="buttonbar">
                    <asp:Button ID="btnScanPCV" runat="server" Text="Scan" CausesValidation="false" Width="100" OnClientClick="javascript:OpenPCVWindowForEdit();" />
                    <asp:Button ID="btnClose" runat="server" Text="Close Window" />
                </div>
            </div>
        </div>
    </form>
</body>
</html>
