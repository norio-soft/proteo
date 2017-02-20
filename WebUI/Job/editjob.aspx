<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.editjob" Codebehind="editjob.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="header1" runat="server" Title="Edit Job" SubTitle="Select the type of job to add"></uc1:header>

<form id="Form1" method="post" runat="server">

	<asp:Panel id="pnlOpenWizard" runat="server" visible="False">
	<script language="javascript" type="text/javascript">
	<!--
		openResizableDialogWithScrollbars('wizard/wizard.aspx?jobId=<%=m_jobId%>', '623', '508');
	//-->
	</script>
	</asp:Panel>
</form>

<uc1:footer id="footer1" runat="server"></uc1:footer>