<%@ Page language="c#" Inherits="Orchestrator.WebUI.scan.viewer" Codebehind="viewer.aspx.cs" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
  <HEAD>
		<title><%= FormType %> viewer</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="Visual Basic .NET 7.1" name="CODE_LANGUAGE">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href='<%=Page.ResolveUrl("~/style/hms.css")%>' type="text/css" rel="stylesheet">
		<script language="vbscript" src="../script/pegasuskeys.vbs"></script>
		<script language="javascript"><!--
			var skipcycle = false

			function fcsOnMe(){
			if (!skipcycle){
			window.focus(); 
			}
			mytimer = setTimeout('fcsOnMe()', 500);
			}
			//-->
			
			var myTimer;
			
			function checkFocus()
			{
				try
				{
						clearTimeout(myTimer);
						self.focus();
				}
				catch(e)
				{clearTimeout(myTimer); 
				 self.focus();}
				
			}
		</script>
</HEAD>
	<body bottomMargin="0" leftMargin="0" topMargin="0" rightMargin="0" scroll="no" onload="javascript:myTimer = setTimeout('checkFocus()', 600);" >
		<form id="Form1" method="post" runat="server">
			<div class="buttonbar" style="TEXT-ALIGN: left"><input class="button" id="btnPrevPage" style="WIDTH: 88px; HEIGHT: 24px" type="button"
					value="< Prev Page"> <input class="button" id="btnNextPage" style="WIDTH: 88px; HEIGHT: 24px" type="button"
					value="Next Page >">&nbsp;<asp:textbox id="txtCurrentPage" runat="server" Width="56px" BackColor="White" ReadOnly="True">0</asp:textbox>&nbsp;<input class="button" id="btnPrint" type="button" value="Print...">&nbsp;
				<input class="button" id="btnClose" onclick="self.close()" type="button" value="Close">
				<select id="cboZoom" name="cboZoom">
					<option value="0">Full Width</option>
					<option value="1">Full Height</option>
					<option value="2" selected>Best Fit</option>
				</select>
				<input class="button" id="btnRotateLeft" style="WIDTH: 88px; HEIGHT: 24px" type="button" value="Rotate Left">
				<input class="button" id="btnRotateRight" style="WIDTH: 88px; HEIGHT: 24px" type="button" value="Rotate Right">
				<asp:textbox id="txtFirstFormPageID" style="DISPLAY: none" runat="server" BorderColor="Yellow"></asp:textbox><asp:textbox id="txtTotalPages" style="DISPLAY: none" runat="server" Width="80px" BorderColor="#80FF80">0</asp:textbox>
				<asp:TextBox id="txtFileName" style="DISPLAY: none" runat="server"></asp:TextBox></div>
			<object classid="clsid:5220cb21-c88d-11cf-b347-00aa00a28331" VIEWASTEXT>
							<PARAM NAME="LPKPath" VALUE="ImagXpress7.lpk">
						</object>
	
<OBJECT id=PrintPRO1 codeBase=../CAB/PrintPro3.cab#Version=3,0,8,0 
classid=clsid:66157B4F-9E4A-488C-92A4-4434A16FCBF2 VIEWASTEXT>
	<PARAM NAME="_ExtentX" VALUE="953">
	<PARAM NAME="_ExtentY" VALUE="794">
	<PARAM NAME="ErrStr" VALUE="C33D27897EEBCA0E0FB71358D5B7BF01">
	<PARAM NAME="ErrCode" VALUE="955868777">
	<PARAM NAME="ErrInfo" VALUE="-1067866244">
	<PARAM NAME="_cx" VALUE="953">
	<PARAM NAME="_cy" VALUE="794">
	<PARAM NAME="AddCR" VALUE="-1">
	<PARAM NAME="Alignment" VALUE="0">
	<PARAM NAME="BackColor" VALUE="16777215">
	<PARAM NAME="BackStyle" VALUE="0">
	<PARAM NAME="BMargin" VALUE="240">
	<PARAM NAME="CurrentX" VALUE="240">
	<PARAM NAME="CurrentY" VALUE="240">
	<PARAM NAME="DocName" VALUE="PrintPRO Document">
	<PARAM NAME="DrawMode" VALUE="13">
	<PARAM NAME="DrawStyle" VALUE="1">
	<PARAM NAME="DrawWidth" VALUE="1">
	<PARAM NAME="FillColor" VALUE="0">
	<PARAM NAME="FillStyle" VALUE="1">
	<PARAM NAME="ForeColor" VALUE="0">
	<PARAM NAME="LMargin" VALUE="240">
	<PARAM NAME="ScaleMode" VALUE="1">
	<PARAM NAME="TMargin" VALUE="240">
	<PARAM NAME="WordWrap" VALUE="-1">
	<PARAM NAME="Font" VALUE="MS Sans Serif">
	<PARAM NAME="AutoMargin" VALUE="1">
	<PARAM NAME="OutputFileName" VALUE="">
	<PARAM NAME="PicTransparent" VALUE="0">
	<PARAM NAME="PicTransparentColor" VALUE="0">
	<PARAM NAME="UseDefaultPrinter" VALUE="-1">
	<PARAM NAME="PrintPreviewScale" VALUE="1">
	<PARAM NAME="PrintPreviewOnly" VALUE="0">
	<PARAM NAME="OwnDIB" VALUE="0">
	<PARAM NAME="PrintToFile" VALUE="0">
	<PARAM NAME="SaveFileType" VALUE="4">
	<PARAM NAME="EvalMode" VALUE="1">
	</OBJECT>
			<div style="DISPLAY: none">
<OBJECT id=IX1 style="WIDTH: 96px; HEIGHT: 32px" 
codeBase=../CAB/ImagXpress7.cab#Version=7,0,27,0 
classid=clsid:6D3CF4F3-C2F3-46E7-A126-3E53102A6B91 VIEWASTEXT>
	<PARAM NAME="_ExtentX" VALUE="2540">
	<PARAM NAME="_ExtentY" VALUE="847">
	<PARAM NAME="ErrStr" VALUE="D9EAA52EFA74D7FAB3067744B35291A8">
	<PARAM NAME="ErrCode" VALUE="955868777">
	<PARAM NAME="ErrInfo" VALUE="-1067866244">
	<PARAM NAME="Persistence" VALUE="-4863">
	<PARAM NAME="_cx" VALUE="2540">
	<PARAM NAME="_cy" VALUE="847">
	<PARAM NAME="AutoSize" VALUE="5">
	<PARAM NAME="Font" VALUE="MS Sans Serif">
	<PARAM NAME="SaveTransparencyColor" VALUE="0">
	<PARAM NAME="OLEDropMode" VALUE="0">
	<PARAM NAME="SaveTIFFCompression" VALUE="0">
	<PARAM NAME="SaveTransparent" VALUE="0">
	<PARAM NAME="SaveJPEGProgressive" VALUE="0">
	<PARAM NAME="SaveJPEGGrayscale" VALUE="0">
	<PARAM NAME="SaveJPEGLumFactor" VALUE="10">
	<PARAM NAME="SaveJPEGChromFactor" VALUE="10">
	<PARAM NAME="SaveJPEGSubSampling" VALUE="2">
	<PARAM NAME="ViewAntialias" VALUE="-1">
	<PARAM NAME="BorderType" VALUE="1">
	<PARAM NAME="ViewDithered" VALUE="-1">
	<PARAM NAME="AlignH" VALUE="1">
	<PARAM NAME="AlignV" VALUE="1">
	<PARAM NAME="LoadRotated" VALUE="0">
	<PARAM NAME="JPEGEnhDecomp" VALUE="-1">
	<PARAM NAME="WMFConvert" VALUE="0">
	<PARAM NAME="ProcessImageID" VALUE="1">
	<PARAM NAME="OwnDIB" VALUE="-1">
	<PARAM NAME="FileTimeout" VALUE="10000">
	<PARAM NAME="AsyncPriority" VALUE="0">
	<PARAM NAME="LZWPassword" VALUE="">
	<PARAM NAME="ViewUpdate" VALUE="-1">
	<PARAM NAME="TWAINProductName" VALUE="">
	<PARAM NAME="TWAINProductFamily" VALUE="">
	<PARAM NAME="TWAINManufacturer" VALUE="">
	<PARAM NAME="TWAINVersionInfo" VALUE="">
	<PARAM NAME="ViewProgressive" VALUE="0">
	<PARAM NAME="SaveTIFFByteOrder" VALUE="0">
	<PARAM NAME="FTPUserName" VALUE="">
	<PARAM NAME="FTPPassword" VALUE="">
	<PARAM NAME="ProxyServer" VALUE="">
	<PARAM NAME="SaveEXIFThumbnailSize" VALUE="0">
	<PARAM NAME="SaveLJPPrediction" VALUE="1">
	<PARAM NAME="PDFSwapBlackandWhite" VALUE="0">
	<PARAM NAME="SaveTIFFRowsPerStrip" VALUE="0">
	<PARAM NAME="TIFFIFDOffset" VALUE="0">
	<PARAM NAME="ViewGrayMode" VALUE="0">
	<PARAM NAME="SaveWSQQuant" VALUE="1">
	<PARAM NAME="DisplayError" VALUE="0">
	<PARAM NAME="EvalMode" VALUE="1">
	</OBJECT>
			</div>
<OBJECT id=ix7 style="WIDTH: 100%; HEIGHT: 89%" codeBase=../CAB/ImagXpress7.cab#Version=7,0,27,0 
classid=clsid:6D3CF4F3-C2F3-46E7-A126-3E53102A6B91 VIEWASTEXT>
	<PARAM NAME="_ExtentX" VALUE="22701">
	<PARAM NAME="_ExtentY" VALUE="16219">
	<PARAM NAME="ErrStr" VALUE="D9EAA52EFA74D7FAB3067744B35291A8">
	<PARAM NAME="ErrCode" VALUE="955868777">
	<PARAM NAME="ErrInfo" VALUE="-1067866244">
	<PARAM NAME="Persistence" VALUE="-4863">
	<PARAM NAME="_cx" VALUE="22701">
	<PARAM NAME="_cy" VALUE="16219">
	<PARAM NAME="AutoSize" VALUE="5">
	<PARAM NAME="Font" VALUE="MS Sans Serif">
	<PARAM NAME="SaveTransparencyColor" VALUE="0">
	<PARAM NAME="OLEDropMode" VALUE="0">
	<PARAM NAME="SaveTIFFCompression" VALUE="0">
	<PARAM NAME="SaveTransparent" VALUE="0">
	<PARAM NAME="SaveJPEGProgressive" VALUE="0">
	<PARAM NAME="SaveJPEGGrayscale" VALUE="0">
	<PARAM NAME="SaveJPEGLumFactor" VALUE="10">
	<PARAM NAME="SaveJPEGChromFactor" VALUE="10">
	<PARAM NAME="SaveJPEGSubSampling" VALUE="2">
	<PARAM NAME="ViewAntialias" VALUE="-1">
	<PARAM NAME="BorderType" VALUE="1">
	<PARAM NAME="ViewDithered" VALUE="-1">
	<PARAM NAME="AlignH" VALUE="1">
	<PARAM NAME="AlignV" VALUE="1">
	<PARAM NAME="LoadRotated" VALUE="0">
	<PARAM NAME="JPEGEnhDecomp" VALUE="-1">
	<PARAM NAME="WMFConvert" VALUE="0">
	<PARAM NAME="ProcessImageID" VALUE="2">
	<PARAM NAME="OwnDIB" VALUE="-1">
	<PARAM NAME="FileTimeout" VALUE="10000">
	<PARAM NAME="AsyncPriority" VALUE="0">
	<PARAM NAME="LZWPassword" VALUE="">
	<PARAM NAME="ViewUpdate" VALUE="-1">
	<PARAM NAME="TWAINProductName" VALUE="">
	<PARAM NAME="TWAINProductFamily" VALUE="">
	<PARAM NAME="TWAINManufacturer" VALUE="">
	<PARAM NAME="TWAINVersionInfo" VALUE="">
	<PARAM NAME="ViewProgressive" VALUE="0">
	<PARAM NAME="SaveTIFFByteOrder" VALUE="0">
	<PARAM NAME="FTPUserName" VALUE="">
	<PARAM NAME="FTPPassword" VALUE="">
	<PARAM NAME="ProxyServer" VALUE="">
	<PARAM NAME="SaveEXIFThumbnailSize" VALUE="0">
	<PARAM NAME="SaveLJPPrediction" VALUE="1">
	<PARAM NAME="PDFSwapBlackandWhite" VALUE="0">
	<PARAM NAME="SaveTIFFRowsPerStrip" VALUE="0">
	<PARAM NAME="TIFFIFDOffset" VALUE="0">
	<PARAM NAME="ViewGrayMode" VALUE="0">
	<PARAM NAME="SaveWSQQuant" VALUE="1">
	<PARAM NAME="DisplayError" VALUE="0">
	<PARAM NAME="EvalMode" VALUE="0">
	</OBJECT>
		</form>
		<script language="vbscript">
		dim CurrentPage
		dim rotationChanged 
		
		rotationChanged = false
		
	sub window_onload
		dim IsLocal
		
		IsLocal=<%=IsLocal%>
		
		if IsLocal then 
			document.all.btnPrint.style.width=0
			'document.all.btnPrint.style.
		end if			
		
		'License Keys for HMS
		dim webKey
		webKey = "<%=Orchestrator.WebUI.Utilities.GetPegasusLicenseKey()%>"
		document.all.IX7.IntegratorWeb webkey
		document.all.IX1.IntegratorWeb webKey
		document.all.PrintPRO1.IntegratorWeb webKey
		
		
		CurrentPage  = 0
		document.all("<%=txtCurrentPage.ClientID%>").value = CurrentPage +1
		if document.all("<%=txtFileName.ClientID%>").value = "" then
			document.all.IX7.ProgressEnabled = True
			document.all.IX7.Async = 1 ' TURNED OFF BELOW WHEN FINISHED LOADING
			document.all.IX7.Timer = 50 ' 1000's of a second, 50 = 1/20th
			document.all.IX7.filename = "<%=Orchestrator.Globals.Configuration.WebServer%>/scan/getscan.aspx?ScannedFormId=" & "<%=scannedFormId%>" & "&" & "PageNumber=0"
		else
			document.all.IX7.filename = document.all("<%=txtFileName.ClientID%>").value & document.all("<%=txtCurrentPage.ClientID%>").value & ".jpg"
		end if
		
		document.all.IX7.AutoSize = 6
		document.all.cboZoom.options(0).selected = true
		
		
		resizeTo 640,480 
		document.all.IX7.Scrollbars = 3
		document.all.IX7.Async = 0
	end sub 
	
	'Sub IX7_Progress(ImageId, OperationId, BytesProcessed, TotalBytes, pctDone, bDone, dAsync, Error)
'		if bDone then 
'			msgbox(	"TotalBytes: " & TotalBytes & vbcrlf & "Error: " & Error )
'		end if
'	End Sub
	
	 
	sub btnNextPage_onClick
		If (CINT(document.all("<%=txtCurrentPage.ClientID%>").value) + 1 > CINT(document.all("<%=txtTotalPages.ClientID%>").value)) Then
			msgbox "There are no further pages in this file to view.", vbinformation, "ImagXpress"
		else

			document.all("<%=txtCurrentPage.ClientID%>").value  = cint(document.all("<%=txtCurrentPage.ClientID%>").value) + 1
				
			dim fName
			if document.all("<%=txtFileName.ClientID%>").value = "" then
				fName = "<%=Orchestrator.Globals.Configuration.WebServer%>/scan/getscan.aspx?ScannedFormId=" & "<%=scannedFormId%>" & "&" & "PageNumber=" & clng(document.all("<%=txtFirstFormPageID.ClientID%>").value) + CINT(document.all("<%=txtCurrentPage.ClientID%>").value) - 1
			else
				fName = document.all("<%=txtFileName.ClientID%>").value & document.all("<%=txtCurrentPage.ClientID%>").value & ".jpg"
			end if
			'document.all.ix7.PageNbr = CurrentPage
			document.all.IX7.Async = 1 ' TURNED OFF BELOW WHEN FINISHED LOADING
			'document.all.IX7.ProgressEnabled = True
			document.all.IX7.Timer = 50 '
			document.all.ix7.FileName=fName
		while document.all.IX7.idle = false
		wend
		document.all.IX7.Refresh
		end if
	end sub
	
	sub btnPrevPage_onClick
		If (document.all("<%=txtCurrentPage.ClientID%>").value <= 1) Then
			msgbox "There are no previous pages in this file to view.", vbinformation, "ImagXpress"
		else
			document.all("<%=txtCurrentPage.ClientID%>").value  = document.all("<%=txtCurrentPage.ClientID%>").value - 1		
			dim fName
			if document.all("<%=txtFileName.ClientID%>").value = "" then
				fName = "<%=Orchestrator.Globals.Configuration.WebServer%>/scan/getscan.aspx?ScannedFormId=" & "<%=scannedFormId%>" & "&" & "PageNumber=" & clng(document.all("<%=txtFirstFormPageID.ClientID%>").value) + CINT(document.all("<%=txtCurrentPage.ClientID%>").value) - 1
			else
				fName = document.all("<%=txtFileName.ClientID%>").value & document.all("<%=txtCurrentPage.ClientID%>").value & ".jpg"
			end if
			document.all.IX7.Async = 1 ' TURNED OFF BELOW WHEN FINISHED LOADING
			'document.all.IX7.ProgressEnabled = True
			document.all.IX7.Timer = 50 '
			document.all.ix7.FileName=fName
			while document.all.IX7.idle = false
			wend
			document.all.IX7.Refresh
		end if
	end sub
	
	' RUN THE PRINT DIALOG BOX AND GENERATE OUTPUT
	sub btnPrint_OnClick()
			Dim a 
			Dim b 
			
			Dim counter
			For counter = 1 To CINT(document.all("<%=txtTotalPages.ClientID%>").value) 
				
				If counter > 1 Then document.all.PrintPRO1.NewPage
		
				if document.all("<%=txtFileName.ClientID%>").value = "" then
					document.all.IX1.filename = "<%=Orchestrator.Globals.Configuration.WebServer%>/scan/getscan.aspx?ScannedFormId=" & "<%=scannedFormId%>" & "&PageNumber=" & clng(document.all("<%=txtFirstFormPageID.ClientID%>").value) + cint(counter) - 1
				else
					document.all.ix1.filename = document.all("<%=txtFileName.ClientID%>").value & counter & ".jpg"
				end if
				
				while document.all.IX1.idle = false
				wend
				'the following Wait call was added because of difficulties printing multi-page tiffs.
				'the call allows the document to be loaded before printing is performed.
				'Setting async to false on the ix control did not work.
				'If printing problems are encountered on site then the wait parameter may need increasing.
				
				document.all.ix1.CtlFontSize = 18
				document.all.ix1.DrawTextString 10,10,"This is a COPY Document - Printed By " & "<%=userFullName%>" &  " on " & now(),DefaultTextColor
				document.all.ix1.DrawTextString 0,1720,"This is a COPY Document - Printed By " & "<%=userFullName%>" &  " on " & now(),DefaultTextColor
				
				while document.all.IX1.idle = false
				wend
				
		
				document.all.PrintPRO1.hDib = document.all.IX1.hDib
				
				document.all.PrintPRO1.PrintDialog
				document.all.PrintPRO1.StartPrintDoc
				
			    a = document.all.PrintPRO1.ScaleWidth - 1 - document.all.PrintPRO1.LMargin
				b = document.all.PrintPRO1.ScaleHeight - 1 - document.all.PrintPRO1.TMargin - document.all.PrintPRO1.BMargin
				

				document.all.PrintPRO1.PrintDIB document.all.PrintPRO1.LMargin, document.all.PrintPRO1.TMargin, a, b, 0, 0, 0, 0, True
				'WaterMark


	
			Next 
			
			document.all.PrintPRO1.EndPrintDoc
	end sub
	
	Private Sub Wait(secs)
		Dim enter_time, leave_time

		enter_time = Timer
		leave_time = Timer

		While (enter_time + secs > leave_time)
			leave_time = Timer
		Wend
	End Sub	
	
	Sub cboZoom_onChange()
		Dim zoomLevel
		document.all.IX7.AutoSize = 5
		zoomLevel = document.all.cboZoom.value
		ChangeZoom zoomLevel 
	End Sub
		
		sub window_onresize()
			zoomLevel = document.all.cboZoom.value
			ChangeZoom zoomLevel 
		end sub
	sub ChangeZoom(ZoomLevel)
			if ZoomLevel<3 then 
			'document.all.ix7.Zoom 5
			'document.all.ix7.ScrollBars = 3
			document.all.ix7.ZoomToFit cint(ZoomLevel)
			'document.all.ix7.MenuSelect 1,0,400,Zoomlevel, 0,20
			'msgbox zoomlevel		
		else
			document.all.ix7.Zoom ZoomLevel/100
		end if
	
	end sub
	
	Sub IX7_MenuSelect(menuType,tool,topMenuID,subMenuID,user1,user2)
		'msgbox "MenuType: " & menuType & vbcrlf & "Tool: " & tool & vbcrlf & "TopMenuId: " & topMenuID & vbcrlf & "subMenuId: " & subMenuID & vbcrlf & "User1: " & user1 & vbcrlf & "User 2" & user2
		
	End Sub

	Sub btnRotateLeft_OnClick()
		document.all.IX7.Rotate(90)	
		if not rotationChanged then  
			window.resizeBy 1,1
			rotationChanged = true
		end if
	End Sub
		
	Sub btnRotateRight_OnClick()
		document.all.IX7.Rotate(-90)
		if not rotationChanged then  
			window.resizeBy 1,1
			rotationChanged = true
		end if
		
	End Sub

		</script>
	</body>
</HTML>
