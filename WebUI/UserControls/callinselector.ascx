<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.CallInSelector" Codebehind="CallInSelector.ascx.cs" %>
<div align="right">
	<asp:DropDownList ID="cboCallIns" Runat="server"></asp:DropDownList>
</div>
<script language="javascript" src="<%=Page.ResolveUrl("~/script/scripts.js")%>" type="text/javascript"></script>
<script language="javascript" src="<%=Page.ResolveUrl("~/script/cookie-session-id.js")%>" type="text/javascript"></script>
<script language="javascript" type="text/javascript">
<!--
function SelectCallIn()
{
	// Get the selected item.
	var jobId = '<%=JobId%>';
	var instructionId = document.getElementById('<%=cboCallIns.ClientID%>').value;

	if (instructionId != '0')
	{
	    var url = webserver + "/traffic/jobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobid=" + jobId + "&instructionid=" + instructionId + getCSID();

		if (window.parent == null)
			window.location = url;
		else
			window.parent.location = url;
	}
}
//-->
</script>