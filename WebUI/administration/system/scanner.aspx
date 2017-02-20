<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.SetUp.scanner" MasterPageFile="~/default_tableless.Master" Codebehind="scanner.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript">
        if (!window.ActiveXObject)
            window.location = "/UseIE.htm?" + window.location.pathname.substr(1);
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Set-Up Scanning Components</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <object classid = "clsid:5220cb21-c88d-11cf-b347-00aa00a28331" VIEWASTEXT>
	    <param name="LPKPath" value="DynamicWebTwain.lpk" />
    </object>
    
    <h3>Setting up the ActiveX Control</h3>
    
    <p><b>The following applies to Internet Explorer 6 and above *ONLY*.</b></p>
    
    <ul style="list-style-type:circle;">
        <li><p>When first navigating to this page, you will be prompted to install an ActiveX Control (either via toolbar at the top of the page, or by pop-up window).</p></li>
        <li><p>Please 'Install the following Add-on' from the toolbar at the top (if prompted) and then click 'Install' or 'Run' when the pop-up window appears. This will install it for your browser.</p></li>
        <li><p>Please then make sure that the scanner you intend to use is installed, connected and switched on. If not, please do that before proceeding any further.</p></li>
        <li><p>Once done please insert a single piece of A4 paper prefrably with some writing or a picture on it and press the 'Scan' button.</p></li>
        <li><p>The sheet should be scanned and a thumbnail of the image should appear in the small window on the left. This will confirm a sucessful setup and scan.</p></li>
    </ul>
    
    <div style="width:50%;">
        <div>
            <div style="float:left; border:solid 1px black; padding:0 5 0 5; height:310px;">
                <object classid="clsid:E7DA7F8D-27AB-4EE9-8FC0-3FEC9ECFE758" id="DynamicWebTwain1" width="250" height="300" CodeBase="DynamicWebTWAIN.cab#version=5,1">
                    <param name="_cx" value="847" />
                    <param name="_cy" value="847" />
                    <param name="JpgQuality" value="100" />
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
            <div style="float:left; border:solid 1px black; padding:0 5 0 5; height:310px;">
                <div id="dvMessages" style="font-family: Verdana; font-size: 11px; color: Green; height: 100%; width: 100%; overflow: auto;">
                    <div style="font-family: Verdana; font-weight: bold; display: none;" id="divScannerSettings">
                        <span id="lblScannerConnection" style="font-family: Verdana; color: Blue">The Scanner is connected..</span>
                        <br/>
                        <br/>
                    </div>
                    <a href="#" onclick="SelectDevice()" id="lnkSelectedScanner" style="font-size: 11px;">Click here to change Scanner</a>
                </div>
            </div>
            <div class="clearDiv"></div>
        </div>

        <div style="text-align: center; font-size: 14pt; font-family: Verdana; display: none;"
            id="divScanSetup">
            You are not able to scan from this computer until the scaning tools have been installed,
            please contact support..
            <div style="margin: 10px; padding: 10px;">
                <input type="button" id="Button2" onclick="CloseWindow()" value="Close" class="ButtonHeightWidth" />
            </div>
        </div>
        
        <div style="display: none;">
            <input type="button" id="btnCloseSource" onclick="CloseSource()" value="Close Source" />
            <input type="button" id="Button3" onclick="Initialisation()" value="Initialise" />
        </div>
        
        <div class="buttonbar">
            <input id="btnScanButton" onclick="return btnScan_onclick()" type="button" value="Scan" class="ButtonHeightWidth" />
        </div>
        
        <asp:HiddenField ID="hidSelectedScannerID" runat="server" />
    </div>
    
    <script type="text/javascript" language="javascript" for="DynamicWebTwain1" event="OnMouseClick(index)">
        scanner_OnMouseClick(index)
    </script>

    <script language="javascript" type="text/javascript">
        var scanner = document.getElementById("DynamicWebTwain1");

        var hidScannerEl = document.getElementById("<%=hidSelectedScannerID.ClientID %>");
        var userName = "<%=this.Page.User.Identity.Name %>";
        var randomKey = "<%=this.RandomKey %>";
        var scanDetails = userName + "_" + randomKey;
        var scannerFromCookie = <%=this.ScannerId %>;
        var scanCount = 1;
        var pagesScanned = 0;

        var imageSaveLocation = "<%=this.ImageSaveLocation %>";
        var imageNetworkLocationPath = "<%=this.ScannedDocumentNetworkPath %>";
        var page = 1;

        var addListener = function() {
            if (window.addEventListener) {
                return function(el, type, fn) {
                    el.addEventListener(type, fn, false);
                };
            }
            else if (window.attachEvent) {
                return function(el, type, fn) {
                    var f = function() {
                        fn.call(el, window.event);
                    };
                    el.attachEvent(type, f);
                };
            }
            else {
                return function(el, type, fn) {
                    element[type] = fn;
                }
            }
        } ();

        var imageCount = 0;

        addListener(scanner, "OnPostAllTransfers", scanner_OnPostAllTransfers);
        addListener(scanner, "OnPostTransfer", scanner_OnPostTransfer);

        function Initialisation() {
            var InitialisationComplete = false;

            scanner = document.getElementById("DynamicWebTwain1");

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
            var twainCount = document.getElementById("DynamicWebTwain1").SourceCount;
            var scanners = "";
            for (var i = 0; i < twainCount; i++) {
                scanners += document.getElementById("DynamicWebTwain1").SourceNameItems(i) + "\n";
            }

            // Show the Twain selection box.
            document.getElementById("DynamicWebTwain1").SelectSource();
            document.getElementById("DynamicWebTwain1").OpenSourceManager();
            twainCount = document.getElementById("DynamicWebTwain1").SourceCount;
            var selectedID = -1;
            for (var x = 0; x < twainCount; x++) {
                if (document.getElementById("DynamicWebTwain1").SourceNameItems(x) == document.getElementById("DynamicWebTwain1").CurrentSourceName) {
                    selectedID = x;
                    break;
                }
            }

            if (selectedID != -1) {
                document.getElementById("<%=hidSelectedScannerID.ClientID%>").value = selectedID;

                // Check that the selected device is connected to the machine.
                if (document.getElementById("DynamicWebTwain1").IfDeviceOnline) {
                    document.getElementById("lnkSelectedScanner").innerHTML = " Selected Scanner " + document.getElementById("DynamicWebTwain1").CurrentSourceName;
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
            document.getElementById("DynamicWebTwain1").CloseSourceManager();
            
            return selectedID;

        }
         
        function scanner_OnPostTransfer() {
            if(scanner.TransferMode == 1) // file transfer 
                scanner.LoadImage('c:\\pdfs\\tmp-' + page + '.jpeg');
        }
        
        function scanner_OnPostAllTransfers() {
            scanner.CloseSource();
            // Enable the Edit Buttons
            document.getElementById("btnScanButton").disabled = "";
            pagesScanned = scanner.HowManyImagesInBuffer;
            Report(pagesScanned + " pages have been scanned.");
        }

        function scanner_OnMouseClick(imageIndex) {
            scanner.CurrentImageIndexInBuffer = imageIndex;
        }

        function btnScan_onclick() {
            try
            {
                var strImageName;
            
                document.getElementById("btnScanButton").disabled = "true";
                scanner.RemoveAllImages();
                pagesScanned = 0;
                imageSaveLocation = "<%=this.ImageSaveLocation %>";

                if(Initialisation())
                {
                
                    Report("Scan Started.");
                    scanner.IfShowUI = false;
                    scanner.OpenSource();
                    scanner.TransferMode = <%=this.ScannerTransferMode %>
                    scanner.MaxImagesInBuffer = 100;
                    scanner.IfDisableSourceAfterAquire = true;
                    scanner.IfFeederEnabled = true;
                    scanner.IfAutoFeed = true;
                    scanner.XferCount = -1;
                    scanner.Resolution = 150;
                    scanner.PixelType = 2;
                    scanner.SetViewMode(1, 1);
                    
			        //read back the transfer mode to confirm that it is supported
			        if (scanner.TransferMode == <%=this.ScannerTransferMode %>)
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

        function Report(message) {
            document.getElementById("dvMessages").innerHTML += "<br/>" + message;
        }
    </script>
</asp:Content>