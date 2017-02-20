<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.addjob" Codebehind="addjob.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="header1" runat="server" Title="Add Job" SubTitle="Select the type of job to add"></uc1:header>

<form id="Form1" method="post" runat="server">
	<br>
	<strong>Select the type of job to add</strong>
	<asp:DropDownList id="cboJobType" runat="server"></asp:DropDownList>
	<div class="buttonbar">
		<asp:Button id="btnAddJob" runat="server" Text="Add Job"></asp:Button>
	</div>
	
	<asp:Panel id="pnlOpenWizard" runat="server" visible="false">
	<script language="javascript" type="text/javascript">
	<!--
		javascript:openResizableDialogWithScrollbars('wizard/wizard.aspx', '623', '508');
	//-->
	</script>
	</asp:Panel>
</form>

<uc1:footer id="footer1" runat="server"></uc1:footer>