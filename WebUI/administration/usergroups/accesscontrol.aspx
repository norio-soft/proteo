<%@ Page language="c#" Inherits="Orchestrator.WebUI.administration.usergroups.AccessControl" MasterPageFile="~/default_tableless.Master" Codebehind="AccessControl.aspx.cs" %>
<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Access Control</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Restrict user access to system modules and functions one a per User Group basis below</h2>
	<fieldset>
	    <legend>System module/function</legend>
	    <table>
		    <tr>
			    <td>
				    <asp:DropDownList id="cboSystemPortion" runat="server"></asp:DropDownList>
			    </td>
			    <td>
				    <asp:Button id="btnGetSystemPortion" cssclass="buttonClass" runat="server" Text="Restrict Access"/>
			    </td>
		    </tr>
	    </table>
	</fieldset>
	<asp:Panel id="pnlConfigureRoles" runat="server" Visible="false">
		<table>
			<tr>
				<td colspan="2">User group(s) which can access</td>
				<td>User group(s) which cannot access</td>
			</tr>
			<tr>
				<td><asp:listbox id="lbAssignedRoles" runat="server" width="200px" datatextfield="Name" datavaluefield="Id" selectionmode="multiple"></asp:listbox></td>
				<td>
					<table>
						<tr>
							<td><asp:Button id="btnUnassign" runat="server" Text="Remove >" width="70"/></td>
						</tr>
						<tr>
							<td><asp:Button id="btnAssign" runat="server" Text="Add <" width="70"/></td>
						</tr>
					</table>
				</td>
				<td><asp:listbox id="lbUnassignedRoles" runat="server" datatextfield="Name" width="200px" datavaluefield="Id" selectionmode="multiple" ></asp:listbox></td>
			</tr>
		</table>
	</asp:Panel>
	<div class="buttonbar"><asp:Button id="btnUpdate" runat="server" Text="Update"></asp:Button></div>
</asp:Content>