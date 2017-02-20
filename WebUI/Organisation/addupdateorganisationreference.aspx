<%@ Reference Page="~/organisation/addupdateorganisation.aspx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Organisation.addupdateorganisationreference" Codebehind="addupdateorganisationreference.aspx.cs" MasterPageFile="~/default_tableless.Master"   %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:validationsummary id="valSum" runat="server" ShowSummary="False" ShowMessageBox="True"></asp:validationsummary><br>
	
	<asp:label ID="lblConfirmation" runat="server" Visible="false" CssClass="confirmation" Text="The new reference has been added successfully."></asp:label>
	<asp:label id="lblReturnLink" style="PADDING-BOTTOM: 5px" runat="server" visible="false"></asp:label>
	
	<fieldset>
		<legend>Reference Capturing Information</legend>
		<table>
			<tr>
				<td>Description</td>
				<td><asp:textbox id="txtDescription" runat="server"></asp:textbox><asp:requiredfieldvalidator id="rfvDescription" runat="server" ErrorMessage="Please provide a description for this reference."
						ControlToValidate="txtDescription" Display="Dynamic">
						<img src="../images/Error.gif" height="16" width="16" title="Please provide a description for this reference." /></asp:requiredfieldvalidator></td>
			</tr>
			<tr>
				<td>Capture Data As</td>
				<td><asp:dropdownlist id="cboDataType" runat="server"></asp:dropdownlist></td>
			</tr>
			<tr>
				<td>Reference Status</td>
				<td><asp:dropdownlist id="cboStatus" runat="server"></asp:dropdownlist></td>
			</tr>
            <tr>
                <td>Can Display on Invoice?</td>
                <td>
                    <asp:CheckBox ID="chkCanDisplayOnInvoice" runat="server"></asp:CheckBox>
                </td>
            </tr>
            <tr>
                <td>Is Mandatory for an Order?</td>
                <td>
                    <asp:CheckBox ID="chkIsMandatoryOnOrder" runat="server"></asp:CheckBox>
                </td>
            </tr>
		</table>
	</fieldset>
	
	<br />
	
	
	<div class="buttonbar"><asp:button id="btnAdd" runat="server" text="Add" onclick="btnAdd_Click"></asp:button></div>

</asp:Content>
