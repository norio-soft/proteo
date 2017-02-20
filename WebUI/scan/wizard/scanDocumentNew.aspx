<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="scanDocumentNew.aspx.cs" Inherits="Orchestrator.WebUI.scan.wizard.scanDocumentNew" %>


<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script type="text/javascript" src="Resources/dynamsoft.webtwain.initiate.js"> </script>
    <script type="text/javascript" src="Resources/dynamsoft.webtwain.config.js"> </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server" >


<fieldset>
<legend> Place the document into the scanner and click Scan </legend>
<table width="100%" style="height:auto;">
    <tr>
        <td  valign="top" colspan="2">
             <asp:Button runat="server" id="btnBack" text="< Back" causesvalidation="False"/>
	        <input type="button" id="btnNext" value="Next >" disabled="disabled" onclick="Save();" causesvalidation="False"  />
        </td>
    </tr>
    <tr>
        <td valign="top" colspan="2">
            <div class="toolbar" style="width: 100%;">
                <input id="btnScanButton" onclick="return btnScan_onclick()" type="button" value="Scan" disabled="disabled"
                    class="ButtonHeightWidth" />
                <input type="button" id="btnEdit" onclick="ShowImageEditor()" disabled="disabled"
                    value="Show Image Editor" class="ButtonHeightWidth" style="width: 130px;" />
            </div>
        </td>    
    </tr>
     
    <tr>
        <td valign="top" colspan="2">
            <div class="toolbar" style="width: 100%;">
                <asp:CheckBox runat="server" ID="chkUploadNow" Text="Upload scan now. (Please allow upto 5 minutes.)" />
                <input type="checkbox" ID="chkDuplex" disabled="disabled">Duplex<br>
            </div>
        </td>
    </tr>

    <tr valign="top">
        <td style="border: solid 1pt black;" width="50%">
            <center>
                <div id="dwtcontrolContainer" width="250" height="300"> </div>
            </center>
        </td>
        <td style="border: solid 1pt black;" width="50%">
            <div id="dvMessages" style="font-family: Verdana; font-size: 11px; color: Green;
                height: 100%; width: 100%; overflow: auto;">
                <span id="lblDetecting" style="font-family: Verdana;"> Loading scanner components...</span><br/>
                <div style="font-family: Verdana; font-weight: bold; display: none;" id="divScannerSettings">              
                    <span id="lblScannerConnection" style="font-family: Verdana; color: Blue">The scanner is connected...</span><br/>
                </div>
                 <a href="#" onclick="SelectDevice()" id="lnkSelectedScanner"
                    style="font-size: 11px; display: none;">Click here to select scanner</a>
            </div>
        </td>
    </tr>
</table>


<asp:HiddenField ID="hidSelectedScannerID" runat="server" />
</fieldset>

<div class="buttonbar">
	<input type="button" value="Cancel" onclick="window.close()">
</div>



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
    var scanButton;
    var lblScannerConnection;
    var chkDuplex;

    $(document).ready(function() {   
        ua = (navigator.userAgent.toLowerCase()); 
        hidScannerEl = document.getElementById("<%=hidSelectedScannerID.ClientID %>");
        scanButton = document.getElementById('btnScanButton');
        lblScannerConnection = document.getElementById('lblScannerConnection');
        chkDuplex = document.getElementById('chkDuplex');

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

        Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', WebTwainReady);

    });



    function WebTwainReady() {
        if (Initialisation()) {
            scanButton.disabled = '';                
        } 
        
        document.getElementById("lnkSelectedScanner").style.display = "block";
        document.getElementById("lblDetecting").style.display = "none";     
    }

    function Initialisation() {
        var InitialisationComplete = false;

        scanner = Dynamsoft.WebTwainEnv.GetWebTwain('dwtcontrolContainer');

        scanner.IfShowFileDialog = false;

        if(scanner == null || scanner.ErrorCode != "0") {

            // Show the Error
            lblScannerConnection.innerHTML = "The necessary scanning tools are not installed.";
            lblScannerConnection.style.color = "Red";

            return false;
        }

        scanner.ProductKey = "<%=this.DynamicWebTwainLicense %>";

        document.getElementById("lblDetecting").style.display = "none";

        scanner.RegisterEvent('OnPostTransfer', scanner_OnPostTransfer);
        scanner.RegisterEvent('OnPostAllTransfers', scanner_OnPostAllTransfers);
        scanner.RegisterEvent('OnMouseClick', scanner_OnMouseClick);
              

        scanner.SelectSourceByIndex(scannerFromCookie)

        // Set the Selected Scanner details 
        if(scanner.OpenSource())
        {
            document.getElementById("lnkSelectedScanner").innerHTML = " Click here to change scanner (currently: " + scanner.CurrentSourceName + ")";
            document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = scannerFromCookie;
            SetDuplexMode();   
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
        

        document.getElementById("divScannerSettings").style.display = "block";
        return InitialisationComplete;
    }


    function IsScannerConnected() {
        if(scanner.OpenSource()) {
            lblScannerConnection.innerHTML = "The scanner is connected...";
            lblScannerConnection.style.color = "Blue";
            scanButton.disabled = '';
            scanner.CloseSource();
            return true;
        }
        else {
            lblScannerConnection.innerHTML = "The scanner is NOT connected, please check your connection.";
            lblScannerConnection.style.color = "Red";
            lnkSelectedScanner.innerHTML = "Click here to Select Scanner";
            scanButton.disabled = 'disabled';
            return false;
        }
    }

    function SelectDevice() {
        // Get the number of scanners attached to the machine.
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

        var sourceName = scanner.CurrentSourceName;
        if (!sourceName) 
            sourceName = scanner.DefaultSourceName;

        for (var x = 0; x < twainCount; x++) {
            if (scanner.SourceNameItems(x) == sourceName) {
                selectedID = x;
                break;
            }
        }

        if (selectedID != -1) {
            document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = selectedID;
            document.getElementById("lnkSelectedScanner").innerHTML = " Click here to change scanner (currently: " + sourceName + ")";

            // check if scanner is connected
            if(!IsScannerConnected()) {
                selectedID = -1;
            }
            else {
                SetDuplexMode();
            }
         
        }
        else {
            document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = -1;
            lnkSelectedScanner.innerHTML = "Click here to select scanner";
            scanButton.disabled = 'disabled';
        }

        scanner.CloseSourceManager();
        return selectedID;

    }

    function SetDuplexMode() {

        scanner.OpenSource();
        if (scanner.Duplex) {
            chkDuplex.disabled = false;
        }
        else {
            chkDuplex.checked = false;
            chkDuplex.disabled = true;
        }
        scanner.CloseSource();
    }

     
    function scanner_OnPostTransfer() {
        if(scanner.TransferMode == 1) // file transfer 
            scanner.LoadImage('c:\\pdfs\\tmp-' + page + '.jpeg');
    }
    
    function scanner_OnPostAllTransfers() {
        scanner.CloseSource();
        // Enable the Edit Buttons
        document.getElementById("btnEdit").disabled = "";
        scanButton.disabled = "";
            
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

            if (IsScannerConnected()) {

                scanButton.disabled = 'disabled';
                var strImageName;
                pagesScanned = 0;
                imageSaveLocation = "<%=this.ImageSaveLocation %>";

                scanner.RemoveAllImages();
                var scannerTransferMode = <%=this.ScannerTransferMode %> ;

                Report("Scan Started.");
                scanner.IfShowUI = false;
                var open = scanner.OpenSource();

                scanner.IfDuplexEnabled = chkDuplex.checked;

                scanner.JpgQuality = 80;
                scanner.TransferMode = 0;
                scanner.IfTiffMultiPage = false;
                scanner.TIFFCompressionType = 0;
                scanner.IfFitWindow = true;
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

                // Unless the GUI is used, the Scanner demands the ADF is used even when documents are in the Flatbed.
                if(scanner.CurrentSourceName.search("EPSON GT-1500") > -1)
                {
                    scanner.IfShowUI = true;
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
            scanButton.disabled = "";
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
