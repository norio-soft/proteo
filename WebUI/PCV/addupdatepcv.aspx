<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.PCV.addupdatePCV" Codebehind="addupdatePCV.aspx.cs" %>

<TITLE></TITLE>
<uc1:header id="Header1" title="Add/Update PCV" runat="server" XMLPath="PCVContextMenu.xml"
	SubTitle="Please enter the PCV Details below."></uc1:header>
<form id="Form1" runat="server">
	<asp:label id="lblConfirmation" runat="server" visible="false" cssclass="confirmation" text="The new Trailer has been added successfully.">The new PCV has been added successfully.</asp:label>
	<fieldset>
		<legend>PCV&nbsp;Details</legend>
		<table>
			<TR>
				<TD>
					<asp:Label id="lblJobPCV" runat="server">Job Id</asp:Label></TD>
				<TD width="332">
					<asp:TextBox id="txtJobPCVId" runat="server" Enabled="False"></asp:TextBox></TD>
				<TD></TD>
			</TR>
			<tr>
				<td>Voucher Number</td>
				<td width="332">
					<asp:TextBox id="txtVoucherNo" runat="server"></asp:TextBox></td>
				<TD>
					<asp:requiredfieldvalidator id="rfvTrailerRef" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
						Display="Dynamic" ControlToValidate="txtVoucherNo" ErrorMessage="Please enter a Voucher No.">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter a Voucher No.'
							alt='Field is Required'></asp:requiredfieldvalidator></TD>
			</tr>
			<tr>
				<td>Number of Pallets</td>
				<td width="332">
					<asp:TextBox id="txtNoOfPallets" runat="server" Width="56px"></asp:TextBox></td>
				<TD>
					<asp:requiredfieldvalidator id="rfvNoOfPallets" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
						Display="Dynamic" ControlToValidate="txtNoOfPallets" ErrorMessage="Please enter No Of Pallets.">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter No Of Pallets.'
							alt='Field is Required'></asp:requiredfieldvalidator></TD>
			</tr>
			<TR>
				<TD>Depot Id</TD>
				<TD width="332">
					<asp:TextBox id="txtDepotId" runat="server"></asp:TextBox></TD>
				<TD>
					<asp:requiredfieldvalidator id="rfvDepotId" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
						Display="Dynamic" ControlToValidate="txtDepotId" ErrorMessage="Please enter a Depot Id.">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter a Depot Id.'
							alt='Field is Required'></asp:requiredfieldvalidator></TD>
			</TR>
			<tr>
				<td vAlign="top">
					<P>Delivery Point</P>
					<P>&nbsp;</P>
				</td>
				<TD width="332">
					<P>
						<asp:DataGrid id="dgDeliveryPoint" runat="server" Width="352px" AutoGenerateColumns="False">
							<Columns>
								<asp:BoundColumn DataField="OrganisationName" HeaderText="Client"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="ClientsCustomerIdentityId" HeaderText="ClientId"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="TownId" HeaderText="TownId"></asp:BoundColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Town"></asp:BoundColumn>
								<asp:BoundColumn DataField="PointName" HeaderText="Point"></asp:BoundColumn>
								<asp:BoundColumn Visible="False" DataField="PointId" HeaderText="PointId"></asp:BoundColumn>
								<asp:TemplateColumn HeaderText="Assign PCV to Point">
									<ItemTemplate>
										<uc:RdoBtnGrouper id="selectRadioButton" runat="server" GroupName="Assign" value='<%#DataBinder.Eval(Container.DataItem, "PointId") %>' />
									</ItemTemplate>
								</asp:TemplateColumn>
							</Columns>
						</asp:DataGrid></P>
				</TD>
			</tr>
			<TR>
				<TD height="23">Date Of Issue</TD>
				<TD width="332" height="23">
					<telerik:RadDateInput id="dteDateOfIssue" runat="server"></telerik:RadDateInput></TD>
				</TD>
				<TD height="23">
					<asp:requiredfieldvalidator id="rfvDateOfIssue" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
						Display="Dynamic" ControlToValidate="dteDateOfIssue" ErrorMessage="Please enter a Date Of Issue.">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter a Date Of Issue.'
							alt='Field is Required'></asp:requiredfieldvalidator></TD>
			</TR>
			<TR>
				<TD height="18">Status</TD>
				<TD height="18" width="332">
					<asp:DropDownList id="cboPCVStatus" runat="server" Width="152px"></asp:DropDownList></TD>
				<TD height="18"></TD>
			</TR>
			<TR>
				<TD height="14">Redemption Status</TD>
				<TD width="332" height="14">
					<asp:DropDownList id="cboPCVRedemptionStatus" runat="server" Width="152px"></asp:DropDownList></TD>
				<TD height="14"></TD>
			</TR>
			<TR>
				<TD>No Of Signings</TD>
				<TD width="332">
					<asp:TextBox id="txtSignings" runat="server" Width="128px">1</asp:TextBox></TD>
				<TD>
					<asp:requiredfieldvalidator id="rfvSignings" runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>"
						Display="Dynamic" ControlToValidate="txtSignings" ErrorMessage="Please enter No Of Signings.">
						<img src="../images/Error.gif" height='16' width='16' title='Please enter No Of Signings.'
							alt='Field is Required'></asp:requiredfieldvalidator></TD>
			</TR>
		</table>
	</fieldset>&nbsp;
	<br>
	<asp:panel id="pnlPCVDeleted" runat="server" Visible="False">
		<FIELDSET>
			<LEGEND>
				<P>Is&nbsp;PCV&nbsp;Deleted</P>
			</LEGEND>
			<asp:checkbox id="chkDelete" runat="server" text="This PCV is deleted."></asp:checkbox>
		</FIELDSET>
	</asp:panel><br>
	<div class="buttonbar"><asp:button id="btnAdd" runat="server" text="Add New PCV" onclick="btnAdd_Click"></asp:button>
	</div>
	<asp:validationsummary id="vsAddUpdatePCV" runat="server" ShowMessageBox="True" ShowSummary="False"></asp:validationsummary><br>
</form>
<uc1:footer id="Footer1" runat="server"></uc1:footer>
