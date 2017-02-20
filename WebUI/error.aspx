<%@ Page language="c#" Inherits="Orchestrator.WebUI.error" Codebehind="error.aspx.cs" EnableTheming="true"   %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<!-- Running on :<%=HttpContext.Current.Server.MachineName%>.  -->
 <html>
	<head runat="server">
		<title><%= Orchestrator.Globals.Configuration.WebUITitle %></title>
		<meta content="Microsoft Visual Studio .NET 7.1" name="GENERATOR" />
		<meta content="C#" name="CODE_LANGUAGE" />
		<meta content="JavaScript" name="vs_defaultClientScript" />
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema" />
		<link href="/style/Styles.css" type="text/css" rel="stylesheet"/>
		<link href="/style/newMasterpage.css" type="text/css" rel="stylesheet"/>
	</head>
<body class="errorpageBodyClass">

<form id="Form1" method="post" runat="server">
	  		
		
	<div class="errorContainerOuter">	
		
		<div class="errorTopHeader">
            <a href="/default.aspx"><IMG src="/App_Themes/Orchestrator/img/Login/he-logo.png" border="0" alt="Return to the Homepage" /></a>
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
								        <h1>Error</h1>								            
								        <asp:label runat="server" id="lblTitle" cssclass="ControlTitleHeader">Haulier Enterprise has encountered a problem processing your request.</asp:label>
								        <div class="errorContentBox">
								            <asp:Label id="lblError" Runat="server"></asp:Label>
								            <p>The problem has been logged and the Haulier Enterprise support team have been notified. Please try and repeat your last action.</p>
								            <p>If the problem persists please contact the Haulier Enterprise Service Desk on:</p>
								            <h3>0845 644 37 20</h3>
								            <p>Please accept our apologies for this inconvenience.</p>
								        </div>
								        <div class="errorButtonBar"><input onclick="parent.location='default.aspx'" type="button" value="Return to homepage" class="buttonclass" /></div>
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
            Application Provided and supported by <a href="http://www.proteo.co.uk/">Proteo</a>
        </div>
        
    </div>
		
		
		
		
        </form>
	</body>
</HTML>
	
  
