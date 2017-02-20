<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.Docket" Codebehind="Docket.ascx.cs" %>
<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px"><STRONG>
							<asp:Label id=lblCollectDrop runat="server" Font-Size="Medium"></asp:Label></STRONG>
						</div>
			            <div style="PADDING-LEFT: 20px; FONT-WEIGHT: bold; PADDING-TOP: 5px">Enter Docket information</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Please enter the docket information for this
							<asp:Label ID="lblCollection" Runat="server">Collection</asp:Label><asp:Label ID="lblDelivery" Runat="server">Delivery</asp:Label>.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>

	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
			width="100%">
			<div>
				<table width="100%" border="0" cellpadding="0" cellspacing="0">
					<tr>
						<td width="100" valign="top">
							<span>Booked Date and Time</span>
						</td>
						<td>
							<table cellpadding="0" cellspacing="0" border="0">
								<tr>
									<td nowrap=nowrap><telerik:RadDateInput id="dteBookedDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Booked Date"></telerik:RadDateInput><asp:CustomValidator ID="cfvValidDateRange" Runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="The Date must be no more than 1 day before today and no more than 3 days ahead of today."><img src="../../images/error.png" Title="The Date must be no more than 1 day before today and no more than 3 days ahead of today."></asp:CustomValidator></td>
									<td></td>
								</tr>
								<tr>
									<td>
									    <telerik:RadDateInput runat="server" ID="dteBookedTime" DateFormat="t"></telerik:RadDateInput>
									   </td>
									<td>
										<asp:requiredfieldvalidator id="rfvBookedDate" runat="server" ControlToValidate="dteBookedDate" Display="Dynamic"
											ErrorMessage="Please enter a Booked Date and Time.">
											<img src="../../images/error.png" Title="Please enter a Booked Date and Time."></asp:requiredfieldvalidator>
										<asp:CustomValidator ID="cfvBookedDate" Runat="server" ControlToValidate="dteBookedDate" Display="Dynamic"
											EnableClientScript="False" ErrorMessage="Booked Dates for collections must occur before the first delivery, and for deliveries the booked date must occur after the last collection.">
											<img src="../../images/error.png" Title="Booked Dates for collections must occur before the first delivery, and for deliveries the booked date must occur after the last collection."></asp:CustomValidator>
										
									</td>
								</tr>
							</table>
						</td>
						<td rowspan="7" valign =top >
							<asp:Label id="lblAddedDockets" Font-Bold="true" Runat="server" Text="Existing Dockets" ></asp:Label>
							<br />
							<asp:Repeater ID="repDockets" runat="server" Visible="True">
								<ItemTemplate>
									<asp:Label id="lblDocket" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Docket") %>'></asp:Label>
									<br>
								</ItemTemplate>
							</asp:Repeater>
						</td>
						<td rowspan="7" valign =top>
						    &nbsp;
						</td>
					</tr>
					<tr id="trSelectDocket" runat="server">
						<td>
							<asp:Label ID="lblSelectDocket" runat="server" Text="Select Docket Number"></asp:Label>
						</td>
						<td>
							<asp:DropDownList ID="cboDockets" Runat="server" AutoPostBack="True"></asp:DropDownList>
							<asp:RequiredFieldValidator id="rfvDockets" runat="server" ControlToValidate="cboDockets" InitialValue="-- Please select a Docket Number --"
								Display="Dynamic" ErrorMessage="Please select a Docket Number.">
								<img src="../../images/error.png" Title="Please select a Docket Number."></asp:RequiredFieldValidator>
						</td>

					</tr>
					<tr>
						<td>
							<asp:Label ID="lblDocketNumber" Runat="server" Text="Docket Number"></asp:Label>
						</td>
						<td>
							<asp:TextBox ID="txtDocket" Runat="server"></asp:TextBox>
							<asp:RequiredFieldValidator ID="rfvDocket" Runat="server" ControlToValidate="txtDocket" Display="Dynamic" ErrorMessage="Please enter a Docket Number.">
								<img src="../../images/error.png" Title="Please enter a Docket Number."></asp:RequiredFieldValidator>
							<asp:CustomValidator ID="cfvDocket" Runat="server" ControlToValidate="txtDocket" Display="Dynamic" ErrorMessage="Docket Numbers must be unique for this job.">
								<img src="../../images/error.png" Title="Docket Numbers must be unique for this job."></asp:CustomValidator>
						</td>
						</tr>						
					<tr>
						<td colspan="2">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<tr>
						<td>
							<span>Quantity Ordered (Cases)</span>
						</td>
						<td>
							<asp:TextBox ID="txtQuantityCases" runat="server"></asp:TextBox>
							<asp:CustomValidator ID="cfvQuantityCases" Runat="server" ControlToValidate="txtQuantityCases" Display="Dynamic"
								ErrorMessage="Please enter a positive whole number.">
								<img src="../../images/error.png" Title="Please enter a positive whole number."></asp:CustomValidator>
						</td>
					</tr>
					<tr>
						<td>
							<span>Number of Pallets</span>
						</td>
						<td>
							<asp:TextBox ID="txtPallets" Runat="server"></asp:TextBox>
							<asp:CustomValidator ID="cfvPallets" Runat="server" ControlToValidate="txtPallets" Display="Dynamic"
								ErrorMessage="Please enter a positive whole number.">
								<img src="../../images/error.png" Title="Please enter a positive whole number."></asp:CustomValidator>
						</td>
					</tr>
					<tr>
						<td>
							<span>Weight</span>
						</td>
						<td>
							<asp:TextBox ID="txtWeight" Runat="server"></asp:TextBox>
							<asp:CustomValidator ID="cfvWeight" Runat="server" ControlToValidate="txtWeight" Display="Dynamic" ErrorMessage="Please enter a valid weight.">
								<img src="../../images/error.png" Title="Please enter a valid weight."></asp:CustomValidator>
						</td>
					</tr>
					<tr>
					    <td>
					        <span>Type of Goods</span>
					    </td>
					    <td>
					        <asp:DropDownList ID="cboGoodsType" runat="server" DataValueField="GoodsTypeId" DataTextField="Description"></asp:DropDownList>
					    </td>
					</tr>
					<tr>
					<td valign="top">
							<span>Notes</span>
						</td>
						<td colspan="3">
							<asp:TextBox ID="txtNotes" Runat="server" TextMode="multiline" Rows="3" Columns="50"></asp:TextBox>
						</td>
					</tr>
					<tr>
						<td colspan="4">
							<div class="spacer" style="HEIGHT:10px"></div>
						</td>
					</tr>
					<tr>
						<td colspan="4">
							<table width="100%">
								<tr>
									<td align="left">
										<asp:CheckBox ID="chkAddAnotherDocket" Runat="server" Text="Add another docket number to this "
											Checked="False"></asp:CheckBox>
									</td>
									<td align="right">
										<asp:Button ID="btnRemoveDocket" runat="server" Text="Remove" CausesValidation="False"></asp:Button>
									</td>
								</tr>
							</table>
						</td>
					</tr>
					<tr>
					</tr>
				</table>
			</div>
		</td>
	</tr>

	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:Button ID="btnBack" Runat="server" CausesValidation="False" Text="< Back"></asp:Button>
			<asp:button id="btnNext" runat="server" CausesValidation="True" Text="Next >"></asp:button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>
<script language="javascript" type="text/javascript">
<!--
	function HidePage()
	{
		if (typeof(Page_ClientValidate) == 'function')
		{
			if (Page_ClientValidate())
			{
				document.getElementById("tblMain").style.display = "none";
				document.getElementById("tblRotate").style.display = "block";
			}
		}
		else
		{
			document.getElementById("tblMain").style.display = "none";
			document.getElementById("tblRotate").style.display = "block";
		}
	}
//-->
</script>
<table width="596" height="472" cellpadding="0" cellspacing="0" border="0" id="tblRotate"
	style="DISPLAY: none">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px">
							Processing
						</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Your actions are being processed, please wait.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50">
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px"
			width="100%"></td>
	</tr>
	<tr height="2" bgcolor="#aca899">
		<td colspan="2" height="3" style="BORDER-TOP:#aca899 1pt solid; BORDER-BOTTOM:white 1pt solid"></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px"></td>
	</tr>
</table>


