<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.cancel" Codebehind="cancel.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<asp:content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	<script language="javascript" type="text/javascript">
	<!--
		resizeTo(1220, 700);
		moveTo((screen.width - 1220) / 2, (screen.height - 700) / 2);
	//-->
	</script>
	
	<div runat="server" id="ButtonBar" class="wizardbuttonbar" style="margin: 0px;">
        <table border="0" cellpadding="0" cellspacing="2" width="100%">
            <tr>
                <td>
                    <input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='<%=Page.ResolveUrl("~/job/job.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%> '+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/driverCommunications.aspx?")%>wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/DriverCallIn/CallIn.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/bookingInPODs.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                </td>
                <td>
                    <input type="button" style="width: 75px; display: <%= JobTypeID == (int)Orchestrator.eJobType.Groupage ? "none" : "" %>"
                        value="Pricing" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/pricing2.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />   
                </td>
                <td width="100%" align="right">
                    
                </td>
            </tr>
        </table>
    </div>
	
	<div align="center">
	<asp:Label id="lblConfirmation" runat="server" CssClass="confirmation"></asp:Label>
		<table width="99%">
			<tr valign="top">
				<td runat="server" id="jobDetailsTD" width="50%" style="text-align: left;">
					<fieldset runat="server" id="jobDetailsFieldset">
						<legend>Job</legend>
						<table>
							<tr>
								<td class="formCellLabel">Job Id:</td>
								<td class="formCellField"><span style="FONT-WEIGHT:bold; font-size:12px"><asp:Label id="lblJobId" runat="server"></asp:Label></span></td>
							</tr>
							<tr>
								<td class="formCellLabel">Job State</td>
								<td class="formCellField"><asp:Label id="lblJobState" runat="server"></asp:Label></td>
							</tr>
							<tr>
								<td class="formCellLabel">Job Type</td>
								<td class="formCellField"><asp:Label id="lblJobType" runat="server"></asp:Label></td>
							</tr>
							<tr id="trCurrentTrafficArea" runat="server">
								<td class="formCellLabel">Current Traffic Area</td>
								<td class="formCellField"><asp:Label id="lblCurrentTrafficArea" runat="server"></asp:Label></td>
							</tr>
							<tr>
								<td class="formCellLabel">Stock Movement Job</td>
								<td class="formCellField"><asp:Label id="lblStockMovement" runat="server"></asp:Label></td>
							</tr>
							<tr>
								<td>&nbsp;</td>
								<td><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand"></td>
							</tr>
						</table>
					</fieldset>
				</td>
				<td runat="server" id="cancellationReasnTD" width="50%">
					<fieldset>
						<legend>Process Cancellation</legend>
						<table runat="server" id="cancellationTable" width="100%">
							<tr>
								<td class="formCellLabel">Reason for Cancellation:</td>
								<td class="formCellField"><asp:TextBox id="txtCancellationReason" runat="server" TextMode="Multiline" Rows="5" Columns="60"></asp:TextBox></td>
								<td class="formCellField"><asp:RequiredFieldValidator id="rfvCancellationReason" runat="server" ControlToValidate="txtCancellationReason" Display="Dynamic" ErrorMessage="Please supply the reason why you are cancelling this job."><img src="../images/error.gif" alt="Please supply the reason why you are cancelling this job." /></asp:RequiredFieldValidator></td>
							</tr>
							<tr>
							    <td colspan="3"><div style="height:18px;"></div></td>
							</tr>
						</table>
						<div class="buttonbar">
							<asp:Button id="btnCancelJob" runat="server" Text="Cancel this Job"></asp:Button>
						</div>
					</fieldset>
				</td>
			</tr>
		</table>
		<div style="height:10px;"></div>
		<table width="99%">
		</table>
	</div>

</asp:content>
