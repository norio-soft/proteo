<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Organisation.Organisation_uninvoicedForClient" Title="All Uninvoiced Work for Client" Codebehind="uninvoicedForClient.aspx.cs" %>

<%@ Import namespace="System.Data"%>

<%@ Register TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>All Uninvoiced work for Client</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">

        <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
    Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>



	<script src="/script/jquery.qtip-1.0.0-rc3.min.js" type="text/javascript"></script>
	<script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>
	
	<script language="javascript" type="text/javascript">

	    function OpenPODWindow(orderID) {
	        var podFormType = 2;
	        var qs = "ScannedFormTypeId=" + podFormType;
	        qs += "&OrderID=" + orderID;

	        <%=dlgDocumentWizard.ClientID %>_Open(qs);
	    }

		function viewOrderProfile(orderID) {
			var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;
			var wnd = window.open(url, "Order", "width=1080, height=900, resizabled=1, scrollbars=1");
		}

		function viewJobDetails(jobID) {

		    var url = '../job/job.aspx?jobId=' + jobID + getCSID();

			openResizableDialogWithScrollbars(url, '1100', '900');
}

// Function to show the filter options overlay box
function FilterOptionsDisplayShow() {
    $("#overlayedClearFilterBox").css({ 'display': 'block' });
    $("#filterOptionsDiv").css({ 'display': 'none' });
    $("#filterOptionsDivHide").css({ 'display': 'block' });
}

function FilterOptionsDisplayHide() {
    $("#overlayedClearFilterBox").css({ 'display': 'none' });
    $("#filterOptionsDivHide").css({ 'display': 'none' });
    $("#filterOptionsDiv").css({ 'display': 'block' });
}

	</script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

	<telerik:RadWindowManager ID="rwmAllUninvoicedWorkForClient" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
		<Windows>
			<telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
		</Windows>
	</telerik:RadWindowManager>

	<asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
            <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
		<fieldset>
			<legend>Filter Options</legend>
			<table>
				<tr>
					 <td runat="server" id="tdDateOptions" >
						<table>
							<tr>
								<td class="formCellLabel">Date From</td>
								<td class="formCellField"><telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100" ToolTip="The start date for the filter." >
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
								<td class="formCellField"><asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter a Start Date"></asp:RequiredFieldValidator></td>
							</tr>
							<tr>
								<td class="formCellLabel">Date To</td>
								<td class="formCellField"><telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100" ToolTip="The end date for the filter." >
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker></td>
								<td class="formCellField"><asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter an End Date"></asp:RequiredFieldValidator></td>
							</tr>
							<tr>
								<td class="formCellLabel">Client</td>
								<td class="formCellField"><telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
									MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
									DataTextField="OrganisationName" DataValueField="IdentityID"
									ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px"></telerik:RadComboBox>
								</td>
							</tr>
							<tr>
								<td class="formCellLabel">Resource</td>
								<td class="formCellField">
									<asp:RadioButtonList ID="cboSearchAgainstWorker" runat="server" RepeatDirection="horizontal" RepeatColumns="4">
										<asp:ListItem Text="In House" Value="HOUSE"></asp:ListItem>
										<asp:ListItem Text="Sub Contracted" Value="SUB"></asp:ListItem>
										<asp:ListItem Text="Unresourced" Value="NONE"></asp:ListItem>
										<asp:ListItem Text="All" Value="ALL" Selected="true"></asp:ListItem>
								   </asp:RadioButtonList>
								</td>
							</tr>
					   </table>
					</td>
				</tr>
			</table>
		</fieldset>
        		<div class="buttonBar">
			<asp:Button ID="btnRefresh" runat="server" Text="Refresh" ValidationGroup="grpRefresh" CausesValidation="true" />
		</div>   
        </div>
     
	</asp:Panel>

	<table id="tblLegend" class="colouredKey" style="margin: 0 0 10px 0;" runat="server" border="0" cellpadding="2" cellspacing="1" width="100%">
		<tr height="20">
			<td align="center" bgcolor="white" width="11%" >Booked</td>
			<td align="center" bgcolor="#CCFFCC" width="11%">Planned</td>
			<td align="center" bgcolor="#99FF99" width="11%">In Progress</td>
			<td align="center" bgcolor="LightBlue" width="11%">Completed</td>
			<td align="center" bgcolor="MistyRose" width="12%">Booking In Incomplete</td>
			<td align="center" bgcolor="PaleVioletRed" width="11%">Booking In Complete</td>
			<td align="center" bgcolor="Yellow" width="11%">Ready To Invoice</td>
			<td align="center" bgcolor="Gold" width="11%">Invoiced</td>
			<td align="center" bgcolor="Khaki" width="11%">Cancelled</td>
		</tr>
	</table>
	
	<table id="tblSummary" runat="server" style="display:none; " cellpadding="0" cellspacing="0"> 
		<tbody>
			<tr>
				<td>
					<h3><asp:Label ID="lblSummary" runat="server" Text="Summary" /></h3>
				</td>
			</tr>
			<tr>
				<td>
					<telerik:RadGrid ID="grdSummary" runat="server" AllowPaging="false" ShowGroupPanel="true" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
						<MasterTableView DataKeyNames="Client" allowSorting="true" >
							<Columns>
								<telerik:GridBoundColumn HeaderText="Client" DataField="Client" HeaderStyle-Width="200"></telerik:GridBoundColumn>
								<telerik:GridBoundColumn HeaderText="Run Type" DataField="Job Type" HeaderStyle-Width="200"></telerik:GridBoundColumn>
								<telerik:GridTemplateColumn HeaderText="Run Count" HeaderStyle-Width="80">
									<ItemTemplate>
										<%# ((int)((DataRowView)Container.DataItem)["CountOfJobs"]).ToString() %>
									</ItemTemplate>
									<FooterTemplate>
										<asp:Label ID="lblTotalCountOfJobs" runat="server"></asp:Label>
									</FooterTemplate>
									<ItemStyle HorizontalAlign="right" />
									<FooterStyle HorizontalAlign="right" />
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn HeaderText="Extra Count" HeaderStyle-Width="80">
									<ItemTemplate>
										<%# ((int)((DataRowView)Container.DataItem)["CountOfExtras"]).ToString() %>
									</ItemTemplate>
									<FooterTemplate>
										<asp:Label ID="lblTotalCountOfExtras" runat="server"></asp:Label>
									</FooterTemplate>
									<ItemStyle HorizontalAlign="right" />
									<FooterStyle HorizontalAlign="right" />
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn HeaderText="Total Charge" HeaderStyle-Width="100">
									<ItemTemplate>
										<%# ((decimal)((DataRowView)Container.DataItem)["Total Charge Amount"]).ToString("C") %>
									</ItemTemplate>
									<FooterTemplate>
										<asp:Label ID="lblTotalRate" runat="server"></asp:Label>
									</FooterTemplate>
									<ItemStyle HorizontalAlign="right" />
									<FooterStyle HorizontalAlign="right" />
								</telerik:GridTemplateColumn>
							</Columns>
						</MasterTableView>
					</telerik:RadGrid>
				</td>
			</tr>
		</tbody>
	</table>
	
	<div id="divUnInvoicedExtras" runat="server" style=" display:none; width:100%">
	<h3><asp:Label ID="lblTitle" runat="server" Text="Uninvoiced Extras" /></h3>
		<telerik:RadGrid ID="grdUninvoiceExtras" runat="server" AllowPaging="false" ShowGroupPanel="true"  allowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
			<MasterTableView width="100%" DataKeyNames="OrderID">
				<Columns>
					<telerik:GridHyperLinkColumn HeaderText="Order ID" SortExpression="OrderID" DataNavigateUrlFormatString="javascript:viewOrderProfile({0})" DataNavigateUrlFields="OrderID" DataTextField="OrderID" HeaderStyle-Width="50"></telerik:GridHyperLinkColumn>
					<telerik:GridBoundColumn HeaderText="Order State" DataField="OrderStatus" HeaderStyle-Width="135"></telerik:GridBoundColumn>
					<telerik:GridTemplateColumn HeaderText="Extra Amount" HeaderStyle-Width="85">
						<ItemTemplate>
							<%#((System.Data.DataRowView)Container.DataItem)["ForeignAmount"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((System.Data.DataRowView)Container.DataItem)["ForeignAmount"]).ToString("C")%>
						</ItemTemplate>
						<FooterTemplate>
							<asp:Label ID="lblTotalUninvoicedExtras" runat="server" />
						</FooterTemplate>
						<ItemStyle HorizontalAlign="Left" />
						<FooterStyle HorizontalAlign="Left" />
					</telerik:GridTemplateColumn>
					<telerik:GridBoundColumn HeaderText="Client" DataField="OrganisationName" HeaderStyle-Width="135"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="On Hold" DataField="OnHold" HeaderStyle-Width="60"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="Extra Type" DataField="ExtraType" HeaderStyle-Width="175"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="Description" DataField="CustomDescription" HeaderStyle-Width="175"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="Extra State" DataField="ExtraState" HeaderStyle-Width="150"></telerik:GridBoundColumn>
					<telerik:GridBoundColumn HeaderText="Create Date" DataField="CreateDate" HeaderStyle-Width="150"></telerik:GridBoundColumn>
				</Columns>
			</MasterTableView>
		</telerik:RadGrid>
	</div>
	
	<table id="tblNormal" runat="server" style="width:100%; display:none" cellpadding="0" cellspacing="0"> 
		<tbody>
			<tr>
				<td>
					<h3><asp:Label ID="lblNormalTitle" runat="server" Text="Normal" /></h3>
				</td>
			</tr>
			<tr>
				<td>
					<telerik:RadGrid ID="grdNormal" runat="server" AllowPaging="false" ShowGroupPanel="true"  allowSorting="true" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
					   <MasterTableView width="100%" DataKeyNames="JobID">
							<Columns>
								<telerik:GridHyperLinkColumn HeaderText="Run ID" SortExpression="JobID" DataNavigateUrlFormatString="javascript:viewJobDetails({0})" DataNavigateUrlFields="JobID" DataTextField="JobID" HeaderStyle-Width="95"></telerik:GridHyperLinkColumn>
								<telerik:GridTemplateColumn HeaderText="Job Charge" HeaderStyle-Width="135">
									<ItemTemplate>
										&nbsp;<span id="spnCharge" runat="server"><%#((System.Data.DataRowView)Container.DataItem)["Rate"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((System.Data.DataRowView)Container.DataItem)["Rate"]).ToString("C")%></span>
									</ItemTemplate>
									<FooterTemplate>
										<asp:Label ID="lblTotalUninvoiced" runat="server" />
									</FooterTemplate>
									<ItemStyle HorizontalAlign="Right" />
									<FooterStyle HorizontalAlign="Right" />
								</telerik:GridTemplateColumn>
								<telerik:GridBoundColumn HeaderText="Load Number" DataField="CustomerOrderNumber" HeaderStyle-Width="100"></telerik:GridBoundColumn>
								<telerik:GridBoundColumn HeaderText="Docket Number" DataField="DeliveryOrderNumber" HeaderStyle-Width="100"></telerik:GridBoundColumn>
									<telerik:GridTemplateColumn HeaderText="References">
										<ItemTemplate>
											<asp:Repeater ID="repReferences" runat="server">
												<ItemTemplate>
													<%# ((System.Data.DataRow)Container.DataItem)["ReferenceDescription"].ToString() %>:&nbsp;<%# ((System.Data.DataRow)Container.DataItem)["Reference"].ToString() %>
												</ItemTemplate>
												<SeparatorTemplate>
													<br />
												</SeparatorTemplate>
											</asp:Repeater>
											&nbsp;
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									 <telerik:GridTemplateColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" HeaderStyle-Width="100">
									<ItemTemplate>
										 <span id="spnCollectionPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" HeaderStyle-Width="100">
									<ItemTemplate>
										 <span id="spnDeliveryPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridBoundColumn HeaderText="Client" DataField="Client" HeaderStyle-Width="175"></telerik:GridBoundColumn>
								<telerik:GridTemplateColumn HeaderText="Delivery" HeaderStyle-Width="100">
									<ItemTemplate>
										<span id="spnDelivery"><%#((bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnytime"]) ? ((DateTime)((System.Data.DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy") : ((DateTime)((System.Data.DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy HH:mm")%></span>                                    
									</ItemTemplate>
								</telerik:GridTemplateColumn>
								<telerik:GridBoundColumn HeaderText="Driver" DataField="Driver" HeaderStyle-Width="150"></telerik:GridBoundColumn>
								<telerik:GridTemplateColumn HeaderText="Trailer" SortExpression="Trailer" HeaderStyle-Width="75">
									<ItemTemplate>
										<span id="spnTrailer"><%#((System.Data.DataRowView)Container.DataItem)["Trailer"].ToString().Length < 2 ? "&nbsp;" : ((System.Data.DataRowView)Container.DataItem)["Trailer"].ToString()%></span>
									</ItemTemplate>
								</telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Has POD" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank" Text=""></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
							</Columns>
					   </MasterTableView>
					</telerik:RadGrid>
				</td>
			</tr>
		</tbody>
	</table>
	
	<asp:Repeater ID="repBusinessType" runat="server" Visible="true">
		<HeaderTemplate>
			<table id="tblBusinessType" style="width:100%"> 
				<tbody>    
		</HeaderTemplate>
			<ItemTemplate>
				<tr id="rowTitle" runat="server" style="display:none">
					<td>
						<h3><asp:Label ID="lblTitle" runat="server" Text="" /></h3>
						<asp:HiddenField ID="hidBusinessTypeID" runat="server" />
					</td>
				</tr>
				<tr id="rowGrid" runat="server" style="display:none">
					<td>
						<telerik:RadGrid ID="grdBusinessType" runat="server" AllowPaging="false" ShowGroupPanel="true"  allowSorting="true" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" OnNeedDataSource="grd_NeedDataSource" OnItemDataBound="grd_ItemDataBound" ShowFooter="true">
						   <MasterTableView width="100%" DataKeyNames="JobID">
								<Columns>
									<telerik:GridTemplateColumn HeaderText="Run ID" SortExpression="JobID" HeaderStyle-Width="50">
										<ItemTemplate>
											<a href="javascript:viewJobDetails(<%#((System.Data.DataRowView)Container.DataItem)["JobID"].ToString() %>)"><%#((System.Data.DataRowView)Container.DataItem)["JobID"].ToString() == string.Empty ? "&nbsp;" : ((System.Data.DataRowView)Container.DataItem)["JobID"].ToString()%></a>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridHyperLinkColumn HeaderText="Order ID" SortExpression="OrderID" DataNavigateUrlFormatString="javascript:viewOrderProfile({0})" DataNavigateUrlFields="OrderID" DataTextField="OrderID" HeaderStyle-Width="50"></telerik:GridHyperLinkColumn>
									<telerik:GridTemplateColumn HeaderText="Order Charge" HeaderStyle-Width="85">
										<ItemTemplate>
											&nbsp;<span id="spnCharge"><%#((System.Data.DataRowView)Container.DataItem)["Rate"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((System.Data.DataRowView)Container.DataItem)["Rate"]).ToString("C")%></span>
										</ItemTemplate>
										<FooterTemplate>
											<asp:Label ID="lblTotalUninvoiced" runat="server" />
										</FooterTemplate>
										<ItemStyle HorizontalAlign="Right" />
										<FooterStyle HorizontalAlign="Right" />
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn HeaderText="Pallet Spaces" HeaderStyle-Width="50">
										<ItemTemplate>
											&nbsp;<span id="spnPalletSpaces" runat="server"><%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["PalletSpaces"].ToString()).ToString("0.##") %></span>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridBoundColumn HeaderText="Customer Order Number" DataField="CustomerOrderNumber" HeaderStyle-Width="135"></telerik:GridBoundColumn>
									<telerik:GridBoundColumn HeaderText="Delivery Order Number" DataField="DeliveryOrderNumber" HeaderStyle-Width="135"></telerik:GridBoundColumn>
									<telerik:GridTemplateColumn HeaderText="References" HeaderStyle-Width="150">
										<ItemTemplate>
											<asp:Repeater ID="repReferences" runat="server">
												<ItemTemplate>
													<%# ((System.Data.DataRow)Container.DataItem)["ReferenceDescription"].ToString() %>:&nbsp;<%# ((System.Data.DataRow)Container.DataItem)["Reference"].ToString() %>
												</ItemTemplate>
												<SeparatorTemplate>
													<br />
												</SeparatorTemplate>
											</asp:Repeater>
											&nbsp;
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									 <telerik:GridTemplateColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" HeaderStyle-Width="100">
										<ItemTemplate>
											 <span id="spnCollectionPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridTemplateColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription" HeaderStyle-Width="100">
										<ItemTemplate>
											 <span id="spnDeliveryPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>"><b><%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridBoundColumn HeaderText="Client" DataField="Client" HeaderStyle-Width="175"></telerik:GridBoundColumn>
									<telerik:GridTemplateColumn HeaderText="Delivery" HeaderStyle-Width="75">
										<ItemTemplate>
											<span id="spnDelivery"><%#((bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnytime"]) ? ((DateTime)((System.Data.DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy") : ((DateTime)((System.Data.DataRowView)Container.DataItem)["Planned Date Time"]).ToString("dd/MM/yyyy HH:mm")%></span>                                    
										</ItemTemplate>
									</telerik:GridTemplateColumn>
									<telerik:GridBoundColumn HeaderText="Driver" DataField="Driver" HeaderStyle-Width="150"></telerik:GridBoundColumn>
									<telerik:GridTemplateColumn HeaderText="Trailer" SortExpression="Trailer" HeaderStyle-Width="75">
										<ItemTemplate>
											<span id="spnTrailer"><%#((System.Data.DataRowView)Container.DataItem)["Trailer"].ToString().Length < 2 ? "&nbsp;" : ((System.Data.DataRowView)Container.DataItem)["Trailer"].ToString()%></span>
										</ItemTemplate>
									</telerik:GridTemplateColumn>
                                    <telerik:GridTemplateColumn HeaderText="Has POD" HeaderStyle-Width="50">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank"></asp:HyperLink>
                                    </ItemTemplate>
                                    </telerik:GridTemplateColumn>
								</Columns>
						   </MasterTableView>
						</telerik:RadGrid>
					</td>
				</tr>
			</ItemTemplate>
		<FooterTemplate>
				</tbody>
			</table>
		</FooterTemplate>
	</asp:Repeater>
	<script type="text/javascript" >
		$('.ShowPointTooltip').each(function (i, item) {
			$(item).qtip({
				style: { name: 'dark',
					width: { min: 176 }
				},
				position: { adjust: { screen: true} },
				content: {
					url: $(item).attr('rel'),
					data: { pointId: $(item).attr('pointid') },
					method: 'get'
				}
			}


				);
});
FilterOptionsDisplayHide();
	</script>

</asp:Content>