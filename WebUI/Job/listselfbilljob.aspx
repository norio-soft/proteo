<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.listselfbilljob" Codebehind="listselfbilljob.aspx.cs" %>

<uc1:header id="Header1" title="Find Self Bill Remainder Job" runat="server" SubTitle="Find a Self Bill Remainder Job by completing any of the information below."
	XMLPath="InvoicingContextMenu.xml"></uc1:header>
<BODY>
	<form id="Form1" runat="server">
		<asp:label id="lblConfirmation" runat="server" visible="false" cssclass="confirmation"></asp:label>
		<fieldset style="TEXT-ALIGN: left">
			<legend>
				<P><STRONG>Self Bill Remainder Job Filter</STRONG></P>
			</legend>
			<table>
				<tr>
					<td>Client</td>
					<td>
                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="true" ShowMoreResultsBox="true" Width="355px">
                        </telerik:RadComboBox>
					</td>
				</tr>
				<TR>
					<TD>
						<P>Date From</P>
					</TD>
					<TD vAlign="top" height="28">
						<P><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></P>
					</TD>
				</TR>
				<TR>
					<TD vAlign="top">Date To</TD>
					<TD><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput>
						<P><asp:button id="btnDates" runat="server" Text="Un/Populate Dates"></asp:button></P>
					</TD>
				</TR>
			</table>
		</fieldset>
		<DIV class="buttonbar"><asp:datagrid id="dgJob" runat="server" ShowFooter="True" AutoGenerateColumns="False" AllowPaging="True"
				pagesize="20" width="100%" cellpadding="2" backcolor="White" border="0" PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right"
				OnPageIndexChanged="dgJob_Page" cssclass="DataGridStyle">
				<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
				<ItemStyle CssClass="DataGridListItem"></ItemStyle>
				<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
				<Columns>
					<asp:BoundColumn DataField="OrganisationName" HeaderText="Client" readonly="True"  ></asp:BoundColumn>
					<asp:BoundColumn DataField="CreateUserId" HeaderText="Created By" readonly="True"  ></asp:BoundColumn>
					<asp:BoundColumn DataField="CreateDate" DataFormatString="{0:dd/MM/yy HH:mm}" HeaderText="Created Date" readonly="True"  ></asp:BoundColumn>
					<asp:TemplateColumn HeaderText="Link To Invoice Id">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "InvoiceId")%>' id="lblInvoiceId">
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Amount">
						<ItemTemplate>
							<%#DataBinder.Eval(Container.DataItem, "ChargeAmount")%>
						</ItemTemplate>
						<EditItemTemplate>
							<table>
							<tr>
							<td>
								<asp:TextBox id="txtChargeAmount" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "ChargeAmount")%>'>
								</asp:TextBox>
							</td>
							<td>
							<asp:requiredfieldvalidator id=rfvAmount runat="server" text="<img src='Images/Error.gif' height='16' width='16' title='Field is Required' alt='Field is Required'>" ErrorMessage="Please enter an Amount." ControlToValidate="txtChargeAmount" Display="Dynamic">
									<img src="../images/Error.gif" height='16' width='16' title='Please enter an Amount.'
										alt='Field is Required'></asp:requiredfieldvalidator>
							</td>
							</tr>
							</table>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Job Id">
						<ItemTemplate>
							<asp:Label runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "JobId")%>' id="lblJobId">
							</asp:Label>
						</ItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Whom">
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Whom")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id="txtWhom" runat="server" Text='<%#DataBinder.Eval(Container.DataItem, "Whom")%>'>
							</asp:TextBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="State">
						<ItemTemplate>
							<%# DataBinder.Eval(Container.DataItem, "Description")%>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:DropDownList id="cboJobSelfBillState" runat="server" DataSource='<%#SelfBillJobState%>' SelectedValue='<%# DataBinder.Eval(Container.DataItem, "Description")%>'>
							</asp:DropDownList>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:EditCommandColumn ButtonType="LinkButton" UpdateText="Update" HeaderText="Edit Item" CancelText="Cancel"
						EditText="Edit"></asp:EditCommandColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" CssClass="DataGridListPagerStyle" Mode="NumericPages"></PagerStyle>
			</asp:datagrid></DIV>
		<DIV class="buttonbar" align="left"><asp:button id="btnSearch" runat="server" cssclass="Button" text="Search"></asp:button><INPUT type="reset" value="Reset">
			<asp:Button id="btnReport" runat="server" Text="Generate Report" visible="false"></asp:Button></DIV>
		<DIV class="buttonbar"><uc1:footer id="Footer1" runat="server"></uc1:footer>
		</DIV>
	</form>
</BODY>
