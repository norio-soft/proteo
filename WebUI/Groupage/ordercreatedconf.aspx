<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Groupage_ordercreatedconf"   Codebehind="ordercreatedconf.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Order Confirmation</title>
    <script language="javascript" type="text/javascript" src="../script/scripts.js"></script>
    <telerik:RadCodeBlock runat="server">
        <script type="text/javascript">
            //Required for the openDialog function called by OpenBookingFormWindow
            var returnUrlFromPopUp = window.location;
            
            function closeWindow()
            {
               CloseOnReload()
            }
            function GetRadWindow()
            {
                var oWindow = null;
                try
                {
                    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
                    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
                }
                catch(err)
                {
                    oWindow = null;
                }
                return oWindow;
            }

            function CloseOnReload()
            {
                var oWin = GetRadWindow();
                if (oWin != null)
                    oWin.Close();
                else
                    self.close();
                    
            }
            
            function OpenBookingFormUpload()
	        {
	            var url = '<%=this.ResolveUrl("/document/upload.aspx?oid=" + OrderID)%>' + "&type=BookingForm";
	            openDialog(url, 405,388,null);
	        }

	        function NewBookingForm() {
	            var BookingFormType = 3;
	            var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
	            url += "?ScannedFormTypeId=" + BookingFormType;
	            url += "&OrderId=<%=this.OrderID %>";

	            openResizableDialogWithScrollbars(url, 550, 500);
	        }
        </script>
    </telerik:RadCodeBlock>
</head>
<body class="masterpagelessPage">
    <form id="form1" runat="server">
        <div style="margin-left:auto; margin-right:auto; width:98%;">
            <div id="OrderID">                    
                <h3><asp:Label ID="lblClientConfirmationMessage" Visible="false" runat="server" Text="Your order has been created and submitted for approval." /></h3>
                <h4>Order ID: <asp:Label ID="lblOrderID" runat="server"></asp:Label></h4>
            </div>
            <div class="buttonbar" style="margin:0 2px 0 2px;">
                <asp:Button ID="btnBookingForm" runat="server" Text="Attach Booking Form" style="width:25%;" />    
                <asp:Button ID="btnPodLabel" runat="server" Text="POD Label" style="width:16%;" CausesValidation="false"  />
                <asp:Button ID="btnCreatePIL" runat="server" Text="Pallet Id Label" CausesValidation="false" style="width:19%;" />
                <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Delivery Note" CausesValidation="false" style="width:19%;" />
                <input type="button" class="Button" value="Close Window" onclick="javascript:closeWindow();" style="width:18%;" />
            </div>
        </div>
    </form>
</body>
</html>
