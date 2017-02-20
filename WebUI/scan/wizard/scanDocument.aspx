<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="scanDocument.aspx.cs" Inherits="Orchestrator.WebUI.scan.wizard.scanDocument" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >

<object height="0px" classid = "clsid:5220cb21-c88d-11cf-b347-00aa00a28331" VIEWASTEXT>
	<param name="LPKPath" value="DynamicWebTwain.lpk" />
</object>
<fieldset>
<legend> Place the document into the scanner and click Scan </legend>
<table width="100%" style="height:auto;">
    <tr>
        <td  valign="top" colspan="2">
             <asp:Button runat="server" id="btnBack" text="< Back" causesvalidation="False" />
	        <input type="button" id="btnNext" value="Next >" disabled="disabled" onclick="Save();" causesvalidation="False" />
        </td>
    </tr>
    <tr>
        <td valign="top" colspan="2">
            <div class="toolbar" style="width: 100%;">
                <input id="btnScanButton" onclick="return btnScan_onclick()" type="button" value="Scan" 
                    class="ButtonHeightWidth" />
                <input type="button" id="btnEdit" onclick="ShowImageEditor()" disabled="disabled"
                    value="Show Image Editor" class="ButtonHeightWidth" style="width: 130px;" />

                <asp:CheckBox runat="server" ID="chkUploadNow" Text="Upload scan now. (Please allow upto 5 minutes.)" />
            </div>
        </td>
    </tr>
    <tr valign="top">
        <td style="border: solid 1pt black;" width="50%">
            <center>
                <div id="divPlugin">
                    <table style="display: none" width="100%" id="IsControlInstalled">
                        <tr>
                            <td style="text-align: center; vertical-align: middle;">
                                <a href="DynamicWebTWAINPlugIn.exe"><strong>Download and install the Plug-in Here</strong></a><br>
                                After the installation, please restart your browser.
                            </td>
                        </tr>
                    </table>
                    <embed style="display: block" id="DynamicWebTWAIN" type="Application/DynamicWebTwain-Plugin" onposttransfer="scanner_OnPostTransfer" onpostalltransfers="scanner_OnPostAllTransfers"
                        onmouseclick="scanner_OnMouseClick"
                         pluginspage="DynamicWebTwain.xpi" width="250" height="300">	
                        <param name="_cx" value="847" />
                        <param name="_cy" value="847" />
                        <param name="JpgQuality" value="80" />
                        <param name="Manufacturer" value="DynamSoft Corporation" />
                        <param name="ProductFamily" value="Dynamic Web TWAIN" />
                        <param name="ProductName" value="Dynamic Web TWAIN" />
                        <param name="VersionInfo" value="Dynamic Web TWAIN 5.0" />
                        <param name="TransferMode" value="0" />
                        <param name="BorderStyle" value="0" />
                        <param name="FTPUserName" />
                        <param name="FTPPassword" />
                        <param name="FTPPort" value="21" />
                        <param name="HTTPUserName" />
                        <param name="HTTPPassword" />
                        <param name="HTTPPort" value="80" />
                        <param name="ProxyServer" />
                        <param name="IfDisableSourceAfterAcquire" value="0" />
                        <param name="IfShowUI" value="-1" />
                        <param name="IfModalUI" value="-1" />
                        <param name="IfTiffMultiPage" value="0" />
                        <param name="IfThrowException" value="0" />
                        <param name="MaxImagesInBuffer" value="1" />
                        <param name="TIFFCompressionType" value="0" />
                        <param name="IfFitWindow" value="-1" />
					    </embed>
                </div>
                <div id="divIE">
                    <object classid="clsid:E7DA7F8D-27AB-4EE9-8FC0-3FEC9ECFE758" id="DynamicWebTwain1" width="250" height="300"
                    CodeBase = "DynamicWebTWAIN.cab#version=5,1">
                        <param name="_cx" value="847" />
                        <param name="_cy" value="847" />
                        <param name="JpgQuality" value="80" />
                        <param name="Manufacturer" value="DynamSoft Corporation" />
                        <param name="ProductFamily" value="Dynamic Web TWAIN" />
                        <param name="ProductName" value="Dynamic Web TWAIN" />
                        <param name="VersionInfo" value="Dynamic Web TWAIN 5.0" />
                        <param name="TransferMode" value="0" />
                        <param name="BorderStyle" value="0" />
                        <param name="FTPUserName" />
                        <param name="FTPPassword" />
                        <param name="FTPPort" value="21" />
                        <param name="HTTPUserName" />
                        <param name="HTTPPassword" />
                        <param name="HTTPPort" value="80" />
                        <param name="ProxyServer" />
                        <param name="IfDisableSourceAfterAcquire" value="0" />
                        <param name="IfShowUI" value="-1" />
                        <param name="IfModalUI" value="-1" />
                        <param name="IfTiffMultiPage" value="0" />
                        <param name="IfThrowException" value="0" />
                        <param name="MaxImagesInBuffer" value="1" />
                        <param name="TIFFCompressionType" value="0" />
                        <param name="IfFitWindow" value="-1" />
                    </object>
                </div>
            </center>
        </td>
        <td style="border: solid 1pt black;" width="50%">
            <div id="dvMessages" style="font-family: Verdana; font-size: 11px; color: Green;
                height: 100%; width: 100%; overflow: auto;">
                <div style="font-family: Verdana; font-weight: bold; display: none;" id="divScannerSettings">
                    <span id="lblScannerConnection" style="font-family: Verdana; color: Blue">The Scanner
                        is connected..</span><br/>
                    <br/>
                </div>
                    <a href="#" onclick="SelectDevice()" id="lnkSelectedScanner"
                        style="font-size: 11px;">Click here to change Scanner</a>
            </div>
        </td>
    </tr>
</table>

<div style="text-align: center; font-size: 14pt; font-family: Verdana; display: none;"
    id="divScanSetup">
    You are not able to scan from this computer until the scaning tools have been installed,
    please contact support..
    <div style="margin: 10px; padding: 10px;">
        <input type="button" id="Button1" onclick="CloseWindow()" value="Close" class="ButtonHeightWidth" />
    </div>
</div>
<div style="display: none;">
    <input type="button" id="btnCloseSource" onclick="CloseSource()" value="Close Source" />
    <input type="button" id="Button3" onclick="Initialisation()" value="Initialise" />
</div>
<asp:HiddenField ID="hidSelectedScannerID" runat="server" />
</fieldset>
    <div class="buttonbar">
	    <input type="button" value="Cancel" onclick="window.close()">
          
    
    </div>
<script type="text/javascript" language="javascript" for="DynamicWebTwain1" event="OnMouseClick(index)">
<!-- 
    scanner_OnMouseClick(index)
 -->
</script>

<script language="javascript" for="DynamicWebTwain1" event="OnPostTransfer">
<!-- 
    scanner_OnPostTransfer();
-->
</script>

<script language="javascript" for="DynamicWebTwain1" event="OnPostAllTransfers">
<!-- 
    scanner_OnPostAllTransfers();
-->
</script>


<script language="javascript" type="text/javascript">

    function ExplorerType() {
        var ua = (navigator.userAgent.toLowerCase());       
        if (ua.indexOf("msie") != -1) return "IE";     
    }

    var scanner;
    var ua;
    var hidScannerEl;
    var userName;
    var randomKey;
    var scanDetails;
    var scannerFromCookie = <%=this.ScannerId %>;
    var scanCount = 1;
    var pagesScanned = 0;

    var imageSaveLocation;
    var imageNetworkLocationPath;
    var page = 1;
    var imageCount = 0;

    $(document).ready(function() {   
        ua = (navigator.userAgent.toLowerCase()); 
        hidScannerEl = document.getElementById("<%=hidSelectedScannerID.ClientID %>");
        userName = "<%=this.Page.User.Identity.Name %>";
        randomKey = "<%=this.RandomKey %>";
        scanDetails = userName + "_" + randomKey;
        scannerFromCookie = <%=this.ScannerId %>;
        scanCount = 1;
        pagesScanned = 0;

        imageSaveLocation = "<%=this.ImageSaveLocation %>";
        imageNetworkLocationPath = "<%=this.ScannedDocumentNetworkPath %>";
        page = 1;
        imageCount = 0;

        if (ExplorerType() == "IE") {
            var obj = document.getElementById("divIE");
            obj.style.display = "";
            var obj = document.getElementById("divPlugin");
            obj.style.display = "none";
            scanner = document.getElementById("DynamicWebTwain1");
        }
        else {
            var obj = document.getElementById("divIE");
            obj.style.display = "none";
            var obj = document.getElementById("divPlugin");
            obj.style.display = "";
            scanner = document.embeds[0];
         }

        if(scanner.ErrorCode != "0"){
            if(ua.match(/firefox\/([\d.]+)/)){
	            document.getElementById("IsControlInstalled").style.display = "none";
	            document.getElementById("DynamicWebTWAIN").style.display="block";
            }
            else
	            if(ua.match(/chrome\/([\d.]+)/)||ua.match(/opera.([\d.]+)/)||ua.match(/version\/([\d.]+).*safari/)){
		            document.getElementById("IsControlInstalled").style.display = "block";
		            document.getElementById("DynamicWebTWAIN").style.display="none";
	            }
        }
    });

    function Initialisation() {
        var InitialisationComplete = false;

        if (ExplorerType() == "IE") {
            var obj = document.getElementById("divIE");
            obj.style.display = "";
            var obj = document.getElementById("divPlugin");
            obj.style.display = "none";
            scanner = document.getElementById("DynamicWebTwain1");
        }
        else {
            var obj = document.getElementById("divIE");
            obj.style.display = "none";
            var obj = document.getElementById("divPlugin");
            obj.style.display = "";
            scanner = document.embeds[0];
         }

         ua = (navigator.userAgent.toLowerCase());   

         if(scanner.ErrorCode != "0"){
		    if(ua.match(/firefox\/([\d.]+)/)){
			    document.getElementById("IsControlInstalled").style.display = "none";
			    document.getElementById("DynamicWebTWAIN").style.display="block";
		    }
		    else
			    if(ua.match(/chrome\/([\d.]+)/)||ua.match(/opera.([\d.]+)/)||ua.match(/version\/([\d.]+).*safari/)){
				    document.getElementById("IsControlInstalled").style.display = "block";
				    document.getElementById("DynamicWebTWAIN").style.display="none";
			    }
	    }

        // Check to see if the Scanning Component is Installed
        if (ScanningSetup()) 
        {
        scanner.SelectSourceByIndex(scannerFromCookie)

           // Set the Selected Scanner details 
            if(scanner.OpenSource())
            {
                document.getElementById("lnkSelectedScanner").innerHTML = " Selected Scanner " + scanner.CurrentSourceName;
                document.getElementById("lblScannerConnection").style.display = "none";
                document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = scannerFromCookie;
                scanner.CloseSource();
                InitialisationComplete = true;
            }
            else
            {
                if(SelectDevice() > -1)
                {
                    InitialisationComplete = true;
                }
            }
        }

        document.getElementById("divScannerSettings").style.display = "";
        return InitialisationComplete;
    }


    function TestScannerConnection() {
        if (!scanner.IfDeviceOnline) {
            lblScannerConnection.innerHTML = "The scanner is NOT connected, please check your connection.";
            lblScannerConnection.style.color = "Red";
            lnkSelectedScanner.innerHTML = "Click here to Select Scanner";
        }
        else {
            // Show Connected scanner
            // Enable the scan button
            document.getElementById("lnkSelectedScanner").innerHTML = " Selected Scanner " + scanner.CurrentSourceName;
            document.getElementById("lblScannerConnection").style.display = "none";
        }
    }

    function ScanningSetup() {
        var retVal = true;
        if (scanner == null || scanner == "undefinded") {
            retVal = false;
            // No Scanning Component.
            document.getElementById("divScanSetup").style.display = "";

            // Show the Error
            document.getElementById("lblScannerConnection").innerHTML = "The necessary scanning tools are not installed.";
        }

        //return the result
        return retVal;
    }

    function SelectDevice() {
        // Get the Number of scanners attached to the machine.
        var twainCount = scanner.SourceCount;
        var scanners = "";
        for (var i = 0; i < twainCount; i++) {
            scanners += scanner.SourceNameItems(i) + "\n";
        }

        // Show the Twain selection box.
        scanner.SelectSource();
        scanner.OpenSourceManager();
        twainCount = scanner.SourceCount;
        var selectedID = -1;
        for (var x = 0; x < twainCount; x++) {
            if (scanner.SourceNameItems(x) == scanner.CurrentSourceName) {
                selectedID = x;
                break;
            }
        }

        if (selectedID != -1) {
            document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = selectedID;

            // Check that the selected device is connected to the machine.
            if (scanner.IfDeviceOnline) {
                document.getElementById("lnkSelectedScanner").innerHTML = " Selected Scanner " + scanner.CurrentSourceName;
                document.getElementById("lblScannerConnection").style.display = "none";
            }
            else
            {
                document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = -1;
                lblScannerConnection.innerHTML = "The scanner is offline, please check your connection.";
                lblScannerConnection.style.color = "Red";
                document.getElementById("lnkSelectedScanner").innerHTML = "Click here to Select Scanner";
            }
        }
        else {
            document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = -1;
            lblScannerConnection.innerHTML = "The scanner is NOT connected, please check your connection.";
            lblScannerConnection.style.color = "Red";
            lnkSelectedScanner.innerHTML = "Click here to Select Scanner";
        }
       scanner.CloseSourceManager();
        
        return selectedID;

    }
     
    function scanner_OnPostTransfer() {
        if(scanner.TransferMode == 1) // file transfer 
            scanner.LoadImage('c:\\pdfs\\tmp-' + page + '.jpeg');
    }
    
    function scanner_OnPostAllTransfers() {
        scanner.CloseSource();
        // Enable the Edit Buttons
        document.getElementById("btnEdit").disabled = "";
        document.getElementById("btnScanButton").disabled = "";
            
        pagesScanned = scanner.HowManyImagesInBuffer;
        
        if(pagesScanned > 0)
            document.getElementById("btnNext").disabled = "";
        
        Report(pagesScanned + " pages have been scanned.");
    }

    function scanner_OnMouseClick(imageIndex) {
        scanner.CurrentImageIndexInBuffer = imageIndex;
    }

    function btnScan_onclick() {
        try
        {
            document.getElementById("btnScanButton").disabled = "true";
            var strImageName;
            scanner.RemoveAllImages();
            pagesScanned = 0;
            imageSaveLocation = "<%=this.ImageSaveLocation %>";
            if(Initialisation())
            {
                var scannerTransferMode = <%=this.ScannerTransferMode %> ;

                Report("Scan Started.");
                scanner.IfShowUI = false;
                scanner.OpenSource();
                scanner.MaxImagesInBuffer = 100;
                scanner.IfDisableSourceAfterAquire = true;
                scanner.IfFeederEnabled = true;
                scanner.IfAutoFeed = true;
                scanner.XferCount = -1;
                scanner.Resolution = 150;
                scanner.PixelType = 2;
                scanner.SetViewMode(1, 1);

                // Fix for the Canon DR-M160 which will show 0 pages otherwise.
                if(scanner.CurrentSourceName.search("Canon DR-M160 TWAIN") > -1)
                {
                    scannerTransferMode = 0;
                }

                // Fix for Fujistu Scanners that use PaperStream. User must install the PaperStream IP (TWAIN) Drivers then select the PaperStream option.
                if(scanner.CurrentSourceName.search("PaperStream") > -1)
                {
                    scannerTransferMode = 0;
                }
                
                if(scanner.CurrentSourceName.search("EPSON") > -1)
                {
                    scanner.Resolution = 100;
                    scanner.PixelType = 2;
                    scannerTransferMode = 0;
                }
                
                // Fix for JR A3 Fujitsu scanner. Do not do this for Chilterns though. Horrible hard-coding whilst we develop a Scanner Settings project.
                if(scanner.CurrentSourceName.search("5530C2dj") > -1 && window.location.host != "chiltern.proteoenterprise.co.uk")
                {
                    //scanner.PageSize = 11;
                    // JR use this Scanner for A3 and always have done. Recently, they need to send A4 scans for Smurfit Kappa.
                    // The only way a scan would get picked up as A4 and also allow them to scan A3 when required is to show them the Scanner UI and allow them to select the page size.
                    scanner.IfShowUI = true;
                }

                if(scanner.CurrentSourceName.search("5120Cdj"))
                {
                    scannerTransferMode = 0;
                }
		
                scanner.TransferMode = scannerTransferMode;
                
			    //read back the transfer mode to confirm that it is supported
			    if (scanner.TransferMode == scannerTransferMode)
			    {   
			        if(scanner.TransferMode == 1) // File mode
			        {
			            strImageName = "c:\\pdfs\\tmp-" + page + ".jpeg";
    			        
			             //the source supports the TWSX_FILE transfer mode.
				        scanner.SetFileXferInfo(strImageName, 4);
				    }
		        }
		        else
		            alert("Transfer mode not accepted.")    

                if (scanner.CapSet() == true)
                    scanner.AcquireImage();
            }
        }
        catch(e)
        {
            document.getElementById("btnScanButton").disabled = "";
        }
    }

    function ShowImageEditor() {
        scanner.ShowImageEditor();
    }

    function CloseSource() {
        scanner.CloseSource();
    }

    function Save() {
        var strActionPage;
        var strHostIP;
        var strImageName;
        var CurrentPathName = unescape(location.pathname); // get current PathName in plain ASCII	
        var CurrentPath = CurrentPathName.substring(0, CurrentPathName.lastIndexOf("/") + 1);
        imageSaveLocation = "<%=this.ImageSaveLocation %>";
        
        strActionPage = CurrentPath + "<%=this.ScannerUploadPage %>?ScannedFormId=<%=this.ScannedFormId %>&AppendOrReplace=<%=this.AppendOrReplace %>"; //the ActionPage's file path
        strHostIP = "<%=this.ServerName %>"; //the host's ip or name

        // if the imageSaveLocation is empty we must upload immediately
        if (imageSaveLocation == "") 
        {     
            var now=new Date(); 
            strImageName = now.getFullYear() + "\\" + (now.getMonth() + 1) + "\\" + now.getDate() + "\\" + scanDetails + ".pdf";
            
            // If the pdf can be saved off to a local network location then do so - its much quicker.
            if(imageNetworkLocationPath == "")
            {
                scanner.HTTPPort = "<%=this.ServerPort %>"; 						//the web service's port
                scanner.HTTPUploadAllThroughPostAsPDF(strHostIP, strActionPage, strImageName);
            }
            else
                scanner.SaveAllAsPDF(imageNetworkLocationPath + strImageName);
        }
        else 
        {
            strImageName = scanDetails + ".pdf";
            var chkUpload = document.getElementById("<%=chkUploadNow.ClientID%>");
            if(chkUpload.checked == true)
            {
                imageSaveLocation += "sendnow\\";
            }
            scanner.SaveAllAsPDF(imageSaveLocation + strImageName);
        }
        
        if (scanner.ErrorCode != 0)	
        {				
            //Failed to upload image
            alert(scanner.ErrorString);
            alert(scanner.HTTPPostResponseString);
        }
        else {
            __doPostBack("btnNext", strImageName);
        }
    }

    function Report(message) {
        document.getElementById("dvMessages").innerHTML += "<br/>" + message;
    }
</script>
</asp:Content>
