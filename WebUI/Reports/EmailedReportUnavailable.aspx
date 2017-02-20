<%@ Page language="c#" Inherits="Orchestrator.WebUI.Reports.EmailedReportUnavailable" Codebehind="EmailedReportUnavailable.aspx.cs" EnableTheming="true"  %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!-- Running on :<%=HttpContext.Current.Server.MachineName%>.  -->
 <html>
	<head runat="server">
		<title>Orchestrator: report unavailable</title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR">
		<meta content="C#" name="CODE_LANGUAGE">
		<meta content="JavaScript" name="vs_defaultClientScript">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link href="/style/Styles.css" type="text/css" rel="stylesheet">
		<link href="/style/newMasterpage.css" type="text/css" rel="stylesheet">
	</head>
<body class="errorpageBodyClass">

<form id="Form1" method="post" runat="server">
	  		
		
	<div class="errorContainerOuter">	
		
		<div class="errorTopHeader">
            <IMG src="/images/newLogin/orch-logo.png" border="0" alt="Logo">
        </div>

        <div class="errorContainer">

	        <div class="errorInnerHeader">
                &nbsp;
	        </div>

	        <div class="errorInnerContent">
                <table id="ControlPanelLayout">
				    <tr>
					    <td>
						    <table cellspacing="0" cellspadding="0">
							    <tr>
								    <td align="center" width="165"></td>
								    <td align="left"></td>
								    <td vAlign="middle">
									    <div id="PageMenu1_maxMenu" style="padding: 0px; margin:0px;"></div>
								    </td>
							    </tr>
						    </table>
					    </td>
				    </tr>
				    <tr>
					    <td vAlign="top">
						    <table cellSpacing="0" cellPadding="0" width="100%">
							    <tr>
								    <td id="ContentPane" vAlign="top">
									    <!-- Begin Content -->
								        <h1>Report Unavailable</h1>								            
								        <asp:label runat="server" id="lblTitle" cssclass="ControlTitleHeader">The report you have requested is currently unavailable.</asp:label>
								        <div class="errorContentBox">
								            <asp:Label id="lblError" Runat="server"></asp:Label>
								            <p>This has been logged and the Orchestrator support team have been notified. Please try again later.</p>
								            <p>If the problem persists please contact the Orchestrator Service Desk on:</p>
								            <h3>0845 643 18 30</h3>
								            <p>Please accept our apologies for this inconvenience.</p>
								        </div>
								        <!--
								        <h2>Error Details</h2>
								        <div style="color:red; font-family:verdana;">
    											
										        <asp:PlaceHolder Id="errorDetails" Runat="server"></asp:PlaceHolder>
										        <BR>
										        <asp:DataGrid id="dgServerVariables" runat="server" >
											        <AlternatingItemStyle CssClass="resultscolor2"></AlternatingItemStyle>
											        <ItemStyle Height="22px" CssClass="resultscolor1"></ItemStyle>
											        <HeaderStyle Font-Bold="True" CssClass="resultstableheader" VerticalAlign="Bottom"></HeaderStyle>
										        </asp:DataGrid>
										        <BR>
    										
								        </div> -->
							        <!-- End Content -->
								    </td>
							    </tr>
						    </table>
					    </td>
				    </tr>
			    </table>
	        </div>

	        <div class="errorInnerFooter">
                &nbsp;
	        </div>

        </div>

        <div class="errorBottomFooter">
            Application Provided and supported by <a href="http://www.orchestrator.co.uk/">Orchestrator Ltd</a>
        </div>
        
    </div>
		
		
		
		
        </form>
	</body>
</HTML>
	
  
