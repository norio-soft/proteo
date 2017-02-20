<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Vehicle.addupdatevehiclekey" Codebehind="addupdatevehiclekey.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add/Update Vehicle Key</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<asp:validationsummary id="vsAddUpdateVehicleKey" runat="server" ShowMessageBox="True" ShowSummary="False"></asp:validationsummary>
	<asp:label id="lblConfirmation" runat="server" visible="false" cssclass="confirmation" text="The new vehicle key has been added successfully."></asp:label>
	<asp:label id="lblReturnLink" style="PADDING-BOTTOM: 5px" runat="server" visible="false"></asp:label>
	
	<fieldset>
		<legend>Vehicle Key Details</legend>
		<table>
			<tr>
				<td>Key Type</td>
				<td style="width:159px;">
				    <asp:DropDownList id="ddlKeyTypes" runat="server" />
				</td>
				<td>
				    <asp:requiredfieldvalidator id="rfvKeyTypes" runat="server" ErrorMessage="Please specify a Key Type." Display="Dynamic" ControlToValidate="ddlKeyTypes">
				        <img src="/images/Error.gif" height='16' width='16' title='Please specify a Key Type.' alt="" />
				    </asp:requiredfieldvalidator>
				</td>
			</tr>
			<tr>
				<td style="height:25px;">Serial</td>
				<td style="width:159px;">
				    <asp:textbox id="txtSerial" runat="server" MultiLine="true" MaxLength="100" />
				</td>
				<td>
				    <asp:requiredfieldvalidator id="rfvSerial" runat="server" ErrorMessage="Please enter a Serial." ControlToValidate="txtSerial">
				        <img src="/images/Error.gif" height='16' width='16' title='Please enter a Serial.' alt="" />
				    </asp:requiredfieldvalidator>
				</td>
			</tr>
		</table>
	</fieldset> 
	
	<div class="buttonbar"><asp:button id="btnAdd" runat="server" text="Add Vehicle Key"></asp:button></div>
</asp:Content>