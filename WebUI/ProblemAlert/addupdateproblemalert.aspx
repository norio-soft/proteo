<%@ Page language="c#" Inherits="Orchestrator.WebUI.ProblemAlert.AddUpdateProblemAlert" Codebehind="AddUpdateProblemAlert.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add/Update Problem Alert</asp:Content>
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

</asp:Content> 

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<h3>Enter New Problem Alert</h3>

<div style="margin-bottom:10px;">
    <asp:Label id="lblMessage" runat="server" CssClass="confirmation" />
    <br />
    <table style="width:800px;">
	    <tr>
	        <td class="formCellLabel" style="width:30%;">Is Problem Resolved</td>
		    <td class="formCellField" style="width:70%;"><asp:checkbox id="chkIsResolved" runat="server" /></td>
	    </tr>
		<tr>
		    <td class="formCellLabel">Estimated Time of Arrival (ETA)</td>
		    <td class="formCellField">
			    <telerik:RadDateInput runat="server" ID="dteETA" DateFormat="dd/MM/yy"></telerik:RadDateInput>
			    <asp:RequiredFieldValidator id="rfvETA" runat="server" ErrorMessage="<img src='../images/error.gif' title='Please enter the ETA for this Order'/>" ControlToValidate="dteETA"></asp:RequiredFieldValidator>
		    </td>
	    </tr>
	    <tr>
	        <td class="formCellLabel">Problem Description</td>
		    <td class="formCellField">
			    <asp:textbox id="txtProblemDescription" runat="server" textmode="multiline" cols="100" rows="10" Width="90%"></asp:textbox>
			    <asp:RequiredFieldValidator id="rfvDescription" runat="server" ErrorMessage="<img src='../images/error.gif' title='PLease Enter a description of the problem'/>" ControlToValidate="txtProblemDescription"></asp:RequiredFieldValidator>
		    </td>
		</tr>
	</table>
</div>

<div class="buttonbar">
	<input type="button" onclick="window.location='../job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%> '+ getCSID();" value="Back to Details"/>
	<asp:button id="btnSubmit" runat="server" text="Post Alert"></asp:button>
</div>

</asp:Content>