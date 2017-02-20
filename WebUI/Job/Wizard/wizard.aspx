<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.wizard" EnableEventValidation="false" Codebehind="wizard.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
	<head runat="server">
		<title>Add/Update Job Wizard</title>
		<link href="../../style/styles.css" type="text/css" rel="stylesheet"/>
		<script src="../../script/scripts.js" type="text/javascript"></script>
        <script src="../../script/cookie-session-id.js" type="text/javascript"></script>
        
		<script language="JavaScript" type="text/javascript">
		    var width = 628;
			var height = 508;
			
			// Close the dialog
			function closeme()
			{
				window.close()
			}

			function initWizardSize() 
			{
				resizeTo(width, height);
				self.moveTo(screen.width/2 - width/2, 20)
			}
		</script>
	</head>

	<body bgcolor="#ece9d8" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0" onload="initWizardSize()">
		<form id="Form1" method="post" runat="server">
            <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true" EnablePartialRendering="true">
            </asp:ScriptManager>
			<!-- Begin Main Content -->
			<asp:panel id="pnlContent" runat="server"></asp:panel>
			<asp:Panel id="pnlCancel" Runat="server" Visible="False">
				<script language="javascript">
					closeme();
				</script>
			</asp:Panel>
			<!-- End Main Content -->
		</form>
	</body>
</html>
