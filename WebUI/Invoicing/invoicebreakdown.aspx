<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>

<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Invoicing.InvoiceBreakdown" CodeBehind="InvoiceBreakdown.aspx.cs" %>

<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>

<asp:content contentplaceholderid="ContentPlaceHolder1" runat="server">
    <asp:label id="lblSubTitle" runat="server" Text="The Invoice Breakdown summary is below."></asp:label>

	<asp:label id="lblJobCount" runat="server" Font-Size="Medium" Width="100%"></asp:label>
	<asp:label id="lblDetails" runat="server" Width="100%" visible="False"></asp:label>
	
	<input type="hidden" id="hidJobCount" runat="server" value="0"/>
	<input type="hidden" id="hidJobTotal" runat="server" value="0"/>
		<FIELDSET>
			<P><LEGEND><STRONG>Invoice Type</STRONG></LEGEND></P>
			<TABLE>
				<TR>
					<TD>
					    Please select type of invoice to breakdown
					</TD>
					<TD>
					    <asp:RadioButtonList id="rdoInvoiceType" runat="server" RepeatColumns="3" repeatDirection="horizontal" AutoPostBack="True"></asp:RadioButtonList>
					</td> 
					<td>
						<asp:RequiredFieldValidator id="rfvInvoiceType" runat="server" Display="Dynamic" ControlToValidate="rdoInvoiceType"
							ErrorMessage="Please select invoice type.">
						<img src="../images/Error.gif" height='16' width='16' title='Please select invoice type.'></asp:RequiredFieldValidator></TD>
					</td>
				</TR>
				<TR>
					<TD>
					
					</TD>
				</TR>
			</TABLE>
		</FIELDSET>
		
		<asp:panel id="pnlFilter" runat="server" visible="false">
		<TABLE id="Table3" width="100%">
			<TBODY>
				<TR>
					<TD>
						<FIELDSET>
							<P><LEGEND><STRONG>Customer</STRONG></LEGEND></P>
							<P>
								<TABLE id="Table1">
									<TR>
										<TD vAlign="top">
											<asp:Label id="lblClient" Runat="server" Text="Client"></asp:Label></TD>
										<TD>
											<radC:RadComboBox ID="cboClient" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px"></radC:RadComboBox>
										<TD>
										</TD>
									</TR>
									<tr>
										<td>
											<asp:Label id="lblSubContractor" Runat="server" Text="Sub Contractor" visisble="false"></asp:Label>
										</td>
										<td>
											<radC:RadComboBox ID="cboSubContractor" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px" ></radC:RadComboBox>
											
										</td>
									</tr>
									<TR>
										<TD>
											<asp:Label id="lblJobState" Runat="server" Text="Client">Job State</asp:Label></TD>
										<TD>
											<asp:dropdownlist id="cboJobState" runat="server"></asp:dropdownlist></TD>
										<TD>
											<asp:RequiredFieldValidator id="rfvJobState" runat="server" Display="Dynamic" ControlToValidate="cboJobState"
												ErrorMessage="Please select a job state.">
												<img src="../images/Error.gif" height='16' width='16' title='Please select a job state.'></asp:RequiredFieldValidator></TD>
									</TR>
									<tr>
										<td>
											<asp:Label id="lblInvoiceId" Runat="server" Text="Invoice Id"></asp:Label>
										</td>
										<td>
											<asp:TextBox id=txtInvoiceId runat="server"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td>
											<asp:Label id="lblJobId" Runat="server" Text="Job Id"></asp:Label> (There may be more jobs attached to the invoice!)
										</td>
										<td>
											<asp:TextBox id="txtJobId" runat="server"></asp:TextBox>
										</td>
									</tr>
									<tr>
										<td>
											<asp:Label id="lblClientInvoiceNumber" Runat="server" Text="Client Invoice Number"></asp:Label>
										</td>
										<td>
											<asp:TextBox id="txtClientInvoiceNumber" runat="server"></asp:TextBox>	
										</td>
									</tr>
								</TABLE>
							</P>
						</FIELDSET>
			<FIELDSET>
			<P><LEGEND><STRONG>Date</STRONG></LEGEND></P>
			<TABLE>
				<TR>
					<TD>Date From
					</TD>
					<TD>
						<telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></TD>
				</TR>
				<TR>
					<TD>Date To</TD>
					<TD>
						<telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></TD>
				</TR>
			</TABLE>
		</FIELDSET>
		</asp:panel>
	<asp:panel id="pnlSort" runat="server" visible="false">
		<FIELDSET>
			<P><LEGEND><STRONG>Sort</STRONG></LEGEND></P>
			<TABLE>
				<TR>
					<TD vAlign="top">Sort By
					</TD>
					<TD>
						<asp:RadioButtonList id="rdoSortType" runat="server" RepeatColumns="3" repeatDirection="horizontal" AutoPostBack="True"></asp:RadioButtonList></TD>
				</TR>
			</TABLE>
		</FIELDSET>
	</asp:panel>
		<asp:label id="lblOnHold" runat="server" visible="false" text="" cssclass="confirmation"></asp:label>
		
	<asp:panel id="pnlNormalJob" runat="server" visible="false">
		<FIELDSET>
			<P><LEGEND><STRONG>Jobs Invoiced (Normal/Self Bill)</STRONG></LEGEND></P>
			<asp:datalist id="dlJob" runat="server" width="100%" BorderColor="Black" CellPadding="2" GridLines="Both"
				ShowFooter="False" BorderStyle="Solid" BorderWidth="1px">
				<HeaderTemplate>
					<TR bgcolor="Silver">
						<TD><STRONG>Job Id</STRONG></TD>
						<TD><STRONG>Invoice Id</STRONG></TD>
						<TD><STRONG>Customer</STRONG></TD>
						<td ><strong>Collection Point</strong></td>
						<TD><STRONG>Delivery Point</STRONG></TD>
						<TD><STRONG>Date Completed</STRONG></TD>
						<TD><STRONG>Charge</STRONG></TD>
						<TD><STRONG>References</STRONG></TD>
					</TR>
				</HeaderTemplate>
				<ItemTemplate>
					<TR>
						<TD valign="top">
							<asp:LinkButton id=lnkJobId onclick=lnkJobId_Click runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "JobId") %>' CausesValidation="False">
							</asp:LinkButton></TD>
							<TD valign="top">
							<asp:LinkButton id="lnkInvoiceId" onclick=lnkInvoiceId_Click runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "InvoiceId") %>' CausesValidation="False">
							</asp:LinkButton></TD>
						<TD valign="top">
							<asp:Label id=lblOrganisationName Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrganisationName") %>' Visible="True">
							</asp:Label>
						</TD>
						<td align="top">
							<asp:Label id="lblCollectionPoint" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CollectionPoint") %>'/>
						</td>
						<TD valign="top">
							<asp:Repeater id="repCustomers" Runat="server">
								<ItemTemplate>
									<asp:Label id="lblCustomerName" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName") %>'>
									</asp:Label>
									<br>
								</ItemTemplate>
							</asp:Repeater></TD>
						<TD valign="top">
							<asp:Label id=lblCompleteDate Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CompleteDate", "{0:dd/MM/yy HH:mm}") %>'>
							</asp:Label></TD>
						<TD valign="top" align="right">
							<asp:Label id=lblChargeAmount Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ChargeAmount","{0:C}") %>'>
							</asp:Label></TD>
						<TD valign="top" align="left">
							<asp:Repeater id="repReferences" Runat="server">
								<ItemTemplate>
										<i>
										<asp:Label ID=lblDescription Runat=server text='<%# DataBinder.Eval(Container.DataItem, "OrganisationReference.Description") %>'>
										</asp:Label></i>:- (
										<asp:Label ID=lblValue Runat=server text='<%# DataBinder.Eval(Container.DataItem, "Value") %>'>
									</asp:Label>
										)<br>
									</ItemTemplate>
							</asp:Repeater></TD>
						</TR>
				</ItemTemplate>
			</asp:datalist>
		</FIELDSET>
	</asp:panel>

	</asp:panel><asp:panel id="pnlSubJob" runat="server" visible="false">
		<FIELDSET>
			<P><LEGEND><STRONG>Jobs Invoiced (Sub Contract)</STRONG></LEGEND></P>
			<asp:label id="lblOnHoldSub" runat="server" visible="false" text="" cssclass="confirmation"></asp:label>
			<asp:datalist id="dlJobSub" runat="server" width="100%" BorderColor="Black" CellPadding="2" GridLines="Both"
				ShowFooter="False" BorderStyle="Solid" BorderWidth="1px">
				<HeaderTemplate>
					<TR bgcolor="Silver">
						<TD><STRONG>Job Id</STRONG></TD>
						<TD><STRONG>Invoice Id</STRONG></TD>
						<TD><STRONG>Client Invoice Number</STRONG></TD>
						<TD><STRONG>Customer</STRONG></TD>
						<td ><strong>Collection Point</strong></td>
						<TD><STRONG>Delivery Point</STRONG></TD>
						<TD><STRONG>Date Completed</STRONG></TD>
						<TD><STRONG>Charge</STRONG></TD>
						<TD><STRONG>Rate</STRONG></TD>
						<TD><STRONG>References</STRONG></TD>
					</TR>
				</HeaderTemplate>
				<ItemTemplate>
					<TR>
						<TD valign="top">
							<asp:LinkButton id="lnkJobIdSub" onclick=lnkJobId_Click runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "JobId") %>' CausesValidation="False">
							</asp:LinkButton></TD>
						<TD valign="top">
							<asp:LinkButton id="lnkInvoiceIdSub" onclick=lnkInvoiceId_Click runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "InvoiceId") %>' CausesValidation="False">
							</asp:LinkButton></TD>
						<TD valign="top">
							<%# DataBinder.Eval(Container.DataItem, "ClientInvoiceNumber") %>
							</TD>
						<TD valign="top">
							<asp:Label id="lblOrganisationNameSub" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "OrganisationName") %>' Visible="True">
							</asp:Label>
						</TD>
						<td align="top">
							<asp:Label id="lblCollectionPointSub" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CollectionPoint") %>'/>
						</td>
						<TD valign="top">
							<asp:Repeater id="repCustomersSub" Runat="server">
								<ItemTemplate>
									<asp:Label id="lblCustomerNameSub" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomerName") %>'>
									</asp:Label>
									<br>
								</ItemTemplate>
							</asp:Repeater></TD>
						<TD valign="top">
							<asp:Label id="lblCompleteDateSub" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CompleteDate", "{0:dd/MM/yy HH:mm}") %>'>
							</asp:Label></TD>
						<TD valign="top" align="right">
							<asp:Label id="lblChargeAmountSub" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ChargeAmount","{0:C}") %>'>
							</asp:Label></TD>
						<TD valign="top" align="right">
							<asp:Label id="lblRate" Runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "Rate","{0:C}") %>'>
							</asp:Label></TD>
						<TD valign="top" align="left">
							<asp:Repeater id="repReferencesSub" Runat="server">
								<ItemTemplate>
										<i>
										<asp:Label ID="lblDescriptionSub" Runat=server text='<%# DataBinder.Eval(Container.DataItem, "OrganisationReference.Description") %>'>
										</asp:Label></i>:- (
										<asp:Label ID="lblValueSub" Runat=server text='<%# DataBinder.Eval(Container.DataItem, "Value") %>'>
									</asp:Label>
										)<br>
									</ItemTemplate>
							</asp:Repeater></TD>
						</TR>
				</ItemTemplate>
			</asp:datalist>
		</FIELDSET>
	</asp:panel

	<DIV class="buttonbar">
		<nfvc:noformvalbutton id="btnFilter" runat="server" text="Generate Invoiced Jobs" ServerClick="btnFilter_Click"></nfvc:noformvalbutton>
		<nfvc:noformvalbutton id="btnClear" runat="server" text="Clear" ServerClick="btnClear_Click"></nfvc:noformvalbutton>
	</DIV>
<script language="javascript">
<!--


    var message = "You have selected "
    var message1 = " job(s), and the total amount is £"

    function TotalAmount(sender, amount) {
        var count = parseInt(document.getElementById("hidJobCount").value, 10);
        var total = parseInt(document.getElementById("hidJobTotal").value, 10);

        if (sender.checked) {
            count = count + 1;
            total = total + amount;
        }
        else {
            count = count - 1;
            total = total - amount;
        }

        document.getElementById("hidJobCount").value = count;
        document.getElementById("hidJobTotal").value = total;

        var text = message + count + message1 + total;

        setLabelText("lblDetails", text);
    }
    function setLabelText(ID, Text) {
        document.getElementById(ID).innerHTML = Text;
    }


	-->
</script>
</asp:content>
