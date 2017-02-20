<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Client.ClientManageOrder" MasterPageFile="~/default_tableless_client.master" CodeBehind="ClientManageOrder.aspx.cs" %>

<%@ Register  TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>

<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="uc" TagName="Infringements" Src="~/UserControls/businessruleinfringementdisplay.ascx" %>


<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    <base target="_self" />

    <style type="text/css">
        .form-value
        {
            font-size: 11px;
        }
    
        .AlignRight
        {
            text-align:right;
        }
        in
        h2
        {
            font-weight:bold !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Order Entry</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<cc1:Dialog ID="dlgOrderConfirmation" runat="server" URL="/groupage/ordercreatedconf.aspx?" Width="650" Height="195" AutoPostBack="true" ReturnValueExpected="true" Mode="Modal" />
	
<asp:HiddenField ID="pageIsUpdate" runat="server" Value="" />
<asp:HiddenField ID="hidShowConfirmForOrderAfterDays" runat="server" Value="" />
<asp:HiddenField ID="hidServiceLevelDays" runat="server" Value="" />

<asp:ListView ID="lvServiceLevelDays" runat="server" ItemPlaceholderID="itemContainer">
    <LayoutTemplate>
        <script language="javascript" type="text/javascript">
            var serviceLevelDays = null;
            function populateServiceLevelDays()
            {
                serviceLevelDays = new Array(<asp:PlaceHolder id="itemContainer" runat="server" />);
            }
            
            populateServiceLevelDays();
        </script>
    </LayoutTemplate>
    <ItemTemplate>[<%# ((int)((System.Data.DataRowView)Container.DataItem)["OrderServiceLevelID"]).ToString() %>,<%# ((System.Data.DataRowView)Container.DataItem)["NoOfDays"] == DBNull.Value ? "-1" : ((int)((System.Data.DataRowView)Container.DataItem)["NoOfDays"]).ToString() %>]</ItemTemplate>
    <ItemSeparatorTemplate>,</ItemSeparatorTemplate>
    <EmptyDataTemplate>
        <script language="javascript" type="text/javascript">
            var serviceLevelDays = new Array();
        </script>
    </EmptyDataTemplate>
    <EmptyItemTemplate>
        <script language="javascript" type="text/javascript">
            var serviceLevelDays = new Array();
        </script>
    </EmptyItemTemplate>
</asp:ListView>
<!-- The .AddMonths(-1) is not a mistype, javascript handles months from 00 i.e. 00 = jan, 01 feb -->
<%-- Call Ref: 66260. Above logic fails if there is a holiday/weekend in January.
     E.G. 17-JAN-2012 is 17/01/2012. The AddMonths (on the month element only) converts this to 17/12/2012 (and NOT to 17/00/2012).
     As 12 is not a valid month in Java, this works out to 17/01/2013!--%>
<asp:ListView ID="lvInvalidDates" runat="server" ItemPlaceholderID="itemContainer">
    <LayoutTemplate>
        <script language="javascript" type="text/javascript">
            var invalidDates = null;
            
            function populateDates()
            {
                invalidDates = new Array(<asp:PlaceHolder id="itemContainer" runat="server" />);
            }
            
            populateDates();
        </script>
    </LayoutTemplate>
<%--Call Ref: 66260 see above.    <ItemTemplate>{ Date : new Date(<%#((DateTime)(Container.DataItem)).ToString("yyyy")%>,<%#((DateTime)(Container.DataItem)).AddMonths(-1).ToString("MM")%>,<%#((DateTime)(Container.DataItem)).ToString("dd")%>) }</ItemTemplate>--%>
    <ItemTemplate>{ Date : new Date(<%#((DateTime)(Container.DataItem)).ToString("yyyy")%>,<%#Convert.ToString((Convert.ToInt32(((DateTime)(Container.DataItem)).ToString("MM"))) - 1)%>,<%#((DateTime)(Container.DataItem)).ToString("dd")%>) }</ItemTemplate>
    <ItemSeparatorTemplate>,</ItemSeparatorTemplate>
    <EmptyDataTemplate>
        <script language="javascript" type="text/javascript">
            var invalidDates = new Array();
        </script>
    </EmptyDataTemplate>
    <EmptyItemTemplate>
        <script language="javascript" type="text/javascript">
            var invalidDates = new Array();
        </script>
    </EmptyItemTemplate>
</asp:ListView>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Services>
        <asp:ServiceReference Path="~/Tariff/Tariffs.asmx" />
    </Services>
</asp:ScriptManagerProxy>

<div style="text-align: center;">
	<asp:Label Visible="false" runat="server" ID="lblCancelledIndicator1" Font-Size="Large" ForeColor="Red" Text="<div style='height:20px;'></div>This order has been cancelled <a href='ClientOrderList.aspx'>Click here to return to your list of orders</a>"></asp:Label>
</div>
<p>Enter information into the following four sections and click one of the "Add
							Order" buttons at the bottom of the form to submit your order.</p>
<div runat="server" id="divOuter" style="padding-bottom: 15px;margin-left: 5%;margin-right: 5%;width: 1024px;" >
	
				<div runat="server" visible="false" id="divInformation" style="height: 20px; width: 813px; padding: 4px; color: #000; background-color: #f5f5f5; text-align: left; position:relative;">
					<asp:Label ID="lblInformation" runat="server" Visible="false"></asp:Label>
					<uc:Infringements ID="ucInfringments" runat="server"></uc:Infringements>
				</div>
				<div style="height: 10px;"></div>
				<div style=" border: solid 1px #000; padding: 8px 8px 8px 8px; height:750px; min-width:1250px;">

					<div style="height: 10px;"></div>
					<div style="float: left; background-color: #FFFCED; margin-right: 5px; height: 520px;" >
						<h2>1 - Order Information</h2>
						<table class="form-value" width="100%" cellpadding="5px">
							<tr runat="server" id="tblRowOrderId">
								<td>
									<div>OrderId</div>
								</td>
								<td>
									<asp:Label runat="server" ID="lblOrderId"></asp:Label>
								</td>
							</tr>
							<tr>
								<td style="width: 140px;">
									<div>
										<asp:Label runat="server" Text="Load Number" ID="lblLoadNumber"></asp:Label>
									</div>
								</td>
								<td>
									<asp:TextBox ID="txtLoadNumber" Width="156px" runat="server" MaxLength="25"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									<div>
										<asp:Label runat="server" Text="Delivery Order Number" ID="lblDocketNumber"></asp:Label>
									</div>
								</td>
								<td>
									<asp:TextBox runat="server" Width="156px" ID="txtDeliveryOrderNumber" MaxLength="25"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									<div>Service</div>
								</td>
								<td>
									<telerik:RadComboBox ID="cboService" runat="server" OnClientSelectedIndexChanged="cboService_SelectedIndexChanged" ></telerik:RadComboBox>
								</td>
							</tr>
							<tr runat="server" id="trNoPallets">
								<td>
									<div>Number of Pallets</div>
								</td>
								<td>
									<asp:TextBox ID="txtNoPallets" Width="156px" style="margin-right: 15px; height: 17px;" runat="server"></asp:TextBox>
									<telerik:RadComboBox ID="cboPalletType" runat="server"></telerik:RadComboBox>
									<asp:RequiredFieldValidator ID="rfvNoPallets" 
                                        SkinID="NonSkinnedValidator"
                                        runat="server" 
                                        ControlToValidate="txtNoPallets"
										Display="Dynamic" 
                                        ErrorMessage="Please enter the number of pallets.">
										<img id="Img3" runat="server" src="~/images/icon-warning.png" title="Please enter the number of pallets." />
									</asp:RequiredFieldValidator>
									<asp:RegularExpressionValidator 
                                        ID="revPallets" 
                                        runat="server" 
                                        ControlToValidate="txtNoPallets"
										Display="Dynamic" 
                                        ValidationExpression="^[0-9]*" 
                                        ErrorMessage="Please enter a valid Number.">
										<img id="Img4" runat="server" src="~/images/icon-warning.png" title="Please enter a valid number." />
									</asp:RegularExpressionValidator>
									<asp:CustomValidator 
                                        ID="cfvNoPallets" 
                                        runat="server" 
                                        SkinID="NonSkinnedValidator" 
                                        ControlToValidate="txtNoPallets"
										ErrorMessage="The number of Pallets must be between 1 and 999." 
                                        ClientValidationFunction="validatePallets"
										Display="Dynamic">
                                        <img src="../images/icon-warning.png" title="The number of Pallets must be between 1 and 999." />
									</asp:CustomValidator>
								</td>
							</tr>
							<tr runat="server" id="trPalletSpaces">
								<td>
									<div>Pallet Spaces</div>
								</td>
								<td>
									<telerik:RadNumericTextBox ID="rntxtPalletSpaces" runat="server" Type="Number" Width="75px" />
									<asp:RequiredFieldValidator 
                                        ID="rfvPalletSpaces" 
                                        runat="server" 
                                        ControlToValidate="rntxtPalletSpaces" 
                                        SkinID="NonSkinnedValidator"
										Display="Dynamic" 
                                        ErrorMessage="Please enter a value for the number of Pallet Spaces.">
										<img id="Img8" runat="server" src="~/images/icon-warning.png" title="Please enter a value for the number of Pallet Spaces." />
									</asp:RequiredFieldValidator>
                                    <asp:CustomValidator 
                                        ID="cfvPalletSpaces" 
                                        runat="server" 
                                        ControlToValidate="rntxtPalletSpaces" 
                                        ClientValidationFunction="validateSpaces" 
                                        SkinID="NonSkinnedValidator"
                                        Display="Dynamic" 
                                        ErrorMessage="Please provide the number of Pallet Spaces between 0 and 9,999.">
                                        <img src="/images/icon-warning.png" height="16" width="16" title="Please provide the number of Pallet Spaces between 0 and 999." />
                                    </asp:CustomValidator>
									
								</td>
							</tr>
							<tr runat="server" id="trPalletNetworkFields">
								<td>
									<div>Pallets</div>
								</td>
								<td>
									Full:&nbsp;<telerik:RadNumericTextBox ID="rntxtFullPallets" NumberFormat-DecimalDigits="0" runat="server" Type="Number" Width="42px" />
									Half:&nbsp;<telerik:RadNumericTextBox ID="rntxtHalfPallets" NumberFormat-DecimalDigits="0" runat="server" Type="Number" Width="42px" />
									Qtr:&nbsp;<telerik:RadNumericTextBox ID="rntxtQtrPallets" NumberFormat-DecimalDigits="0" runat="server" Type="Number" Width="42px" />
									Over:&nbsp;<telerik:RadNumericTextBox ID="rntxtOverSizePallets" NumberFormat-DecimalDigits="0" runat="server" Type="Number" Width="42px" />
								</td>
							</tr>
							<tr>
								<td>
									<div>Goods Type</div>
								</td>
								<td>
									<telerik:RadComboBox ID="cboGoodsType" runat="server"></telerik:RadComboBox>
								</td>
							</tr>
							<tr>
								<td>
									<div>Weight (kg)</div>
								</td>
								<td>
									<asp:TextBox ID="txtWeight" Width="156px" runat="server">0</asp:TextBox>
									<asp:RegularExpressionValidator 
                                        ID="revWeight" 
                                        runat="server" 
                                        ControlToValidate="txtWeight"
										Display="Dynamic" 
                                        ValidationExpression="^[0-9]*" 
                                        ErrorMessage="Please enter a valid Number.">
									    <img id="Img5" runat="server" src="~/images/icon-warning.png" title="Please enter a valid number."/>
									</asp:RegularExpressionValidator>
									<asp:RequiredFieldValidator 
                                        ID="rfvWeight" 
                                        runat="server" 
                                        SkinID="NonSkinnedValidator" 
                                        ControlToValidate="txtWeight"
										Display="Dynamic" 
                                        ErrorMessage="Please enter a value for the weight.">
										<img id="img15" runat="server" src="/images/icon-warning.png" title="Please enter a value for the weight."/>
									</asp:RequiredFieldValidator>
                                    <asp:CustomValidator 
                                        ID="cfvWeight" 
                                        runat="server" 
                                        SkinID="NonSkinnedValidator" 
                                        ControlToValidate="txtWeight"
										ErrorMessage="The weight must be between 1 and 99,999kgs." 
                                        ClientValidationFunction="validateWeight"
										Display="Dynamic">
                                        <img src="../images/icon-warning.png" title="The weight must be between 0 and 99,999kgs." />
                                    </asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td>
									<div>Cases</div>
								</td>
								<td>
									<asp:TextBox ID="txtCases" Width="156px" runat="server">0</asp:TextBox>
									<asp:RegularExpressionValidator 
                                        ID="revCases" 
                                        runat="server" 
                                        ControlToValidate="txtCases"
										Display="Dynamic" 
                                        ValidationExpression="^[0-9]*" 
                                        ErrorMessage="Please enter a valid Number.">
										<img id="Img6" runat="server" src="~/images/icon-warning.png" title="Please enter a valid number." alt="" />
									</asp:RegularExpressionValidator>
                                    <asp:CustomValidator 
                                        ID="cfvCases" 
                                        runat="server" 
                                        SkinID="NonSkinnedValidator" 
                                        ControlToValidate="txtCases"
										ErrorMessage="The number of cases must be between 1 and 99,999." 
                                        ClientValidationFunction="validateCases"
										Display="Dynamic">
                                        <img src="../images/icon-warning.png" title="The number of cases must be between 0 and 99,999." />
                                    </asp:CustomValidator>
								</td>
							</tr>
							<tr>
								<td valign="top">
									<div>Notes</div>
								</td>
								<td colspan="2">
									<asp:TextBox ID="txtNotes" runat="server" TextMode="MultiLine" Columns="50" Rows="5"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td colspan="3">
									<asp:Panel ID="pnlReferences" runat="server">
										<asp:Repeater ID="repReferences" runat="server">
											<HeaderTemplate>
												<table class="form-value" style="width: 500px;">
													<tr>
														<td colspan="2">
															<br />
															<b>Additional References:</b><br />
															<br />
														</td>
													</tr>
											</HeaderTemplate>
											<ItemTemplate>
												<tr>
													<td style="width: 145px;" valign="top">
														<div>
															<%# DataBinder.Eval(Container.DataItem, "Description") %></div>
														<input type="hidden" id="hidOrganisationReferenceId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "OrganisationReferenceId") %>' />
													</td>
													<td>
														<asp:PlaceHolder ID="plcHolder" runat="server">
															<asp:TextBox ID="txtReferenceValue" runat="server"></asp:TextBox>
															<asp:RequiredFieldValidator ID="rfvReferenceValue" runat="server" ControlToValidate="txtReferenceValue"
																EnableClientScript="False" Display="Dynamic" ErrorMessage='Please supply a <%# DataBinder.Eval(Container.DataItem, "Description") %>.'><img src="../images/error.png"  Title='Please supply a <%# DataBinder.Eval(Container.DataItem, "Description") %>.'></asp:RequiredFieldValidator>
														</asp:PlaceHolder>
													</td>
												</tr>
											</ItemTemplate>
											<FooterTemplate>
												</table>
											</FooterTemplate>
										</asp:Repeater>
									</asp:Panel>
								</td>
							</tr>
						</table>
					</div>
					
					
					<div style="float: left; background-color: #F2F8F2; margin-bottom: 5px;" >
						<h2>2 - Collect From</h2>
						<table class="form-value" width="100%">
							<tr>
								<td>
									<asp:Panel ID="pnlCollectionPoint" runat="server">
										<p1:Point runat="server" ID="ucCollectionPoint" ShowFullAddress="true" CanClearPoint="true"
											Visible="true" RequireAddressLine1="true" CanUpdatePoint="false" Width="473px" />
									</asp:Panel>
								</td>
							</tr>
						</table>
						<div class="formCellField-Horizontal">
							<table class="form-value" width="100%">
								<tr>
									<td>
										<div>Collect When</div>
									</td>
								</tr>
								<tr>
									<td>
										<input type="radio" name="collection" runat="server" id="rdCollectionTimedBooking"
											onclick="collectionTimedBooking(this);" />Timed Booking
										<input type="radio" name="collection" runat="server" checked="true" id="rdCollectionBookingWindow"
											onclick="collectionBookingWindow(this);" />Booking Window
										<input type="radio" name="collection" runat="server" id="rdCollectionIsAnytime" onclick="collectionIsAnytime(this);" />Anytime
									</td>
								</tr>
							</table>
							<table class="form-value" width="100%" cellpadding="5px">
								<tr>
									<td style="width: 137px;">
										Collect from:
									</td>
									<td>
										<telerik:RadDatePicker Width="100" ID="dteCollectionFromDate" runat="server" >
										<DateInput runat="server" ClientEvents-OnValueChanged="dteCollectionFromDate_SelectedDateChanged" 
                                            DateFormat="dd/MM/yy" 
                                            DisplayDateFormat="dd/MM/yy">
                                        </DateInput>
                                        </telerik:RadDatePicker>
										<telerik:RadTimePicker Width="65" ID="dteCollectionFromTime" runat="server" >
                                            <DateInput runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00">
                                            </DateInput>
										</telerik:RadTimePicker>
									</td>
									<td>
										<asp:RequiredFieldValidator 
                                            ID="rfvCollectionFromDate" 
                                            runat="server" 
                                            ControlToValidate="dteCollectionFromDate"
											Display="Dynamic" 
                                            ErrorMessage="Please enter a collection from date."
                                            SkinID="NonSkinnedValidator" >
										    <img id="Img1" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection from date."/>
										</asp:RequiredFieldValidator>
										<asp:RequiredFieldValidator 
                                            ID="rfvCollectionFromTime" 
                                            runat="server" 
                                            ControlToValidate="dteCollectionFromTime"
											Display="Dynamic" 
                                            ErrorMessage="Please enter a collection from time."
                                            SkinID="NonSkinnedValidator" >
										    <img id="Img14" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection from time."/>
										</asp:RequiredFieldValidator>
										<asp:CustomValidator 
                                            ID="CustomValidator1" 
                                            runat="server" 
                                            ControlToValidate="dteCollectionFromDate"
											Display="Dynamic" 
                                            EnableClientScript="true" 
                                            ClientValidationFunction="CV_ClientValidateCollectionDate"
											ErrorMessage="The date cannot be before today."
                                            SkinID="NonSkinnedValidator" >
											<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/>
										</asp:CustomValidator>
									</td>
								</tr>
								<tr runat="server" id="trCollectBy">
									<td>
										Collect by:
									</td>
									<td>
										<telerik:RadDatePicker Width="100" ID="dteCollectionByDate" runat="server" >
                                        <DateInput runat="server" ClientEvents-OnValueChanged="dteCollectionByDate_SelectedDateChanged" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                        </DateInput>
                                        </telerik:RadDatePicker>
										<telerik:RadTimePicker Width="65" ID="dteCollectionByTime" runat="server">
                                        <DateInput runat="server" 
                                            DateFormat="HH:mm" 
                                            DisplayDateFormat="HH:mm"
                                            SelectedDate="01/01/01 17:00">
                                        </DateInput>
                                        </telerik:RadTimePicker>
									</td>
									<td>
										<asp:RequiredFieldValidator 
                                            ID="rfvCollectionByDate" 
                                            runat="server" 
                                            ControlToValidate="dteCollectionByDate" 
                                            Display="Dynamic" 
                                            OnClientDateChanged="dteCollectionByDate_SelectedDateChanged" 
                                            ErrorMessage="Please enter a collection by date."
                                            SkinID="NonSkinnedValidator" >
										    <img id="Img12" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection by date." alt=""/>
										</asp:RequiredFieldValidator>
										<asp:RequiredFieldValidator 
                                            ID="rfvCollectionByTime" 
                                            runat="server" 
                                            ControlToValidate="dteCollectionByTime" 
                                            Display="Dynamic" 
                                            ErrorMessage="Please enter a collection by time."
                                            SkinID="NonSkinnedValidator" >
										    <img id="Img13" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection by time." alt=""/>
										</asp:RequiredFieldValidator>
									</td>
								</tr>
							</table>
						</div>
						<div style="height: 10px;"></div>
						<table>
							<tr>
								<td colspan="3">
									<asp:CustomValidator ID="cfvDelivery" runat="server" ControlToValidate="dteDeliveryFromDate" EnableClientScript="false" ErrorMessage="">Invalid Collection / Delivery Dates:
										<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server" id="imgCfvDeliveryWarning" title="" alt="warning icon" />
									</asp:CustomValidator>
									<br />
									<asp:Label runat="server" ForeColor="Red" ID="lblCollectionDeliveryExceptions"></asp:Label>
								</td>
							</tr>
						</table>
					</div>
					

					<div style=" float:left; position:relative; background-color:#F5FBFF; margin-bottom: 5px">
						<h2>3 - Deliver To</h2>
						<table class="form-value" width="100%">
							<tr>
								<td>
									<asp:Panel ID="pnlDeliveryPoint" runat="server">
										<p1:Point runat="server" ID="ucDeliveryPoint" ShowFullAddress="true" RequireAddressLine1="true" CanClearPoint="true" CanUpdatePoint="false" Width="473px" />
									</asp:Panel>
								</td>
							</tr>
						</table>
						<table class="form-value" width="100%">
							<tr>
								<td>
									<div>Deliver When</div>
								</td>
							</tr>
							<tr>
								<td>
									<input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" />Timed Booking
									<input type="radio" name="delivery" runat="server" checked="true" id="rdDeliveryBookingWindow" onclick="deliveryBookingWindow(this);" />Booking Window
									<input type="radio" name="delivery" runat="server" id="rdDeliveryIsAnytime" onclick="deliveryIsAnytime(this);" />Anytime
								</td>
							</tr>
						</table>
						<table class="form-value" width="100%" cellpadding="5px">
							<tr runat="server" id="trDeliverFrom">
								<td style="width: 140px;">
									Deliver from:
								</td>
								<td>
									<telerik:RadDatePicker Width="100" ID="dteDeliveryFromDate" runat="server">
                                        <DateInput runat="server" ClientEvents-OnValueChanged="dteDeliveryFromDate_SelectedDateChanged" 
                                            DateFormat="dd/MM/yy" 
                                            DisplayDateFormat="dd/MM/yy">
                                        </DateInput>
									</telerik:RadDatePicker>
									<telerik:RadTimePicker Width="65" ID="dteDeliveryFromTime" runat="server" SelectedDate="01/01/01 08:00">
                                        <DateInput runat="server" 
                                            DateFormat="HH:mm" 
                                            DisplayDateFormat="HH:mm" 
                                            SelectedDate="01/01/01 08:00">
                                        </DateInput>
									</telerik:RadTimePicker>
								</td>
								<td>
									<asp:RequiredFieldValidator ID="rfvDeliveryFromDate" 
                                        runat="server" 
                                        ControlToValidate="dteDeliveryFromDate"
										Display="Dynamic" 
                                        ErrorMessage="Please enter a delivery from date."
                                        SkinID="NonSkinnedValidator" >
									    <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from date."/>
									</asp:RequiredFieldValidator>
									<asp:RequiredFieldValidator 
                                        ID="rfvDeliveryFromTime" 
                                        runat="server" 
                                        ControlToValidate="dteDeliveryFromTime"
										Display="Dynamic" 
                                        ErrorMessage="Please enter a delivery from time."
                                        SkinID="NonSkinnedValidator" >
									    <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from time."/>
									</asp:RequiredFieldValidator>
								</td>
							</tr>
							<tr>
								<td style="width: 140px;">
									Deliver by:
								</td>
								<td>
									<telerik:RadDatePicker Width="100" ID="dteDeliveryByDate" runat="server"> 
                                        <DateInput runat="server" DateFormat="dd/MM/yy" ClientEvents-OnValueChanged="dteDeliveryByDate_SelectedDateChanged"
                                            DisplayDateFormat="dd/MM/yy">
                                            </DateInput>
                                            </telerik:RadDatePicker>
									<telerik:RadTimePicker Width="65" ID="dteDeliveryByTime" runat="server" SelectedDate="01/01/01 17:00">
                                        <DateInput runat="server"
                                            DateFormat="HH:mm"
                                            DisplayDateFormat="HH:mm"
                                            SelectedDate="01/01/01 17:00">
                                        </DateInput>
									</telerik:RadTimePicker>
								</td>
								<td>
									<asp:RequiredFieldValidator 
                                        ID="rfvDeliveryByDate" 
                                        runat="server" 
                                        ControlToValidate="dteDeliveryByDate"
										Display="Dynamic" 
                                        ErrorMessage="Please enter a delivery by date."
                                        SkinID="NonSkinnedValidator" >
										<img id="Img10" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by date." alt="" />
									</asp:RequiredFieldValidator>
									<asp:RequiredFieldValidator 
                                        ID="rfvDeliveryByTime" 
                                        runat="server" 
                                        ControlToValidate="dteDeliveryByTime"
										Display="Dynamic" 
                                        ErrorMessage="Please enter a delivery by time."
                                        SkinID="NonSkinnedValidator" >
										<img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by time." alt="" />
									</asp:RequiredFieldValidator>
									<asp:CustomValidator 
                                        ID="CustomValidator3" 
                                        runat="server" 
                                        Enabled="true" 
                                        ControlToValidate="dteDeliveryByDate"
										Display="Dynamic" 
                                        EnableClientScript="true" 
                                        ClientValidationFunction="CV_ClientValidateDeliveryDate"
										ErrorMessage="The date cannot be before today."
                                        SkinID="NonSkinnedValidator" >
										<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/>
									</asp:CustomValidator>
									<asp:CustomValidator 
                                        ID="CustomValidator4" 
                                        runat="server" 
                                        Enabled="true" 
                                        ControlToValidate="dteDeliveryByDate"
										Display="Dynamic" 
                                        EnableClientScript="true" 
                                        ClientValidationFunction="CV_ClientValidateDeliveryDate2"
										ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?"
                                        SkinID="NonSkinnedValidator" >
										<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date entered is far in the future." alt="warning icon"/>
									</asp:CustomValidator>
								</td>
							</tr>
						</table>
					</div>
				

					<div style="background-color:#FFF0F0; width: 485px">
						<h2>4 - Rate</h2>
						<table class="form-value" cellpadding="5px">
							<tr>
								<td style="width: 140px; vertical-align: top;">
									<div>Rate:</div>
								</td>
								<td>
									<div style="float:left;">
										<telerik:RadNumericTextBox ID="rntRate" runat="server" Type="Currency" CssClass="AlignRight" Font-Bold="true" Culture="en-GB" Width="65px" />
										<asp:RequiredFieldValidator 
                                            ID="rfvRate" 
                                            runat="server" 
                                            ControlToValidate="rntRate" 
                                            Display="Dynamic" 
                                            ErrorMessage="Please enter a rate for this Order"
                                            SkinID="NonSkinnedValidator" >
											<img id="Img7" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a Rate." alt="" />
										</asp:RequiredFieldValidator>
										<asp:HiddenField ID="hidTariffRateDescription" runat="server" Value="" />
										<asp:HiddenField ID="hidTariffRate" runat="server" Value="" />
										<div>
											<span id="spnRateError" class="error"></span>
										</div>
									</div>
									<div style="clear"></div>
								</td>
							</tr>
							<tr id="trSurcharges" runat="server" style="display: none;">
								<td style="width: 75px; vertical-align: top;">
									<div>Surcharges</div>
								</td>
								<td style="width: 250px;">
									<div style="float:right; margin-right:5px;">
										<table id="tblSurcharges" cellpadding="0" cellspacing="0" style="text-align:left;">
											<tbody id="bSurcharges"></tbody>
										</table>
										<asp:HiddenField ID="hidSelectedSurchargeExtraTypeIDs" runat="server" />
									</div>
									<div style="clear"></div>
								</td>
							</tr>
							<tr>
								<td style="width: 75px; vertical-align: top;">
									<div>Order Total</div>
								</td>
								<td style="width: 250px;">
									<div style="float:right; margin-right:5px;">
										<b><span id="spnTotal"></span></b>
									</div>
									<div style="clear"></div>
								</td>
							</tr>
						</table>
						<table>
							<tr>
								<td>
									<asp:CustomValidator ID="cfvOrderDetailValidation" runat="server" ControlToValidate="txtLoadNumber" EnableClientScript="false" ErrorMessage="">
										Invalid Collection / Delivery Details:
										<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server" id="img16" title="" alt="warning icon" />
									</asp:CustomValidator>
									<br />
									<ul><asp:Literal ID="litPointExistingValidatorDisplay" runat="server" /></ul>
								</td>
							</tr>
						</table>
					</div>
				</div>
</div>

<div class="buttonbar" style="margin-left: 5%; margin-right:5%; width:1013px; height: 30px;">
					<table cellpadding="1px">
						<tr>
							<td>
								<asp:Button ID="btnSubmit" runat="server" Text="Add Order" Style="width: 130px; height: 25px;" OnClientClick="if (!validateAddedNewPoint()) return false;" ToolTip="Click to submit your order." />
								<asp:Button ID="btnUpdate" runat="server" Text="Update Order" Style="width: 130px; height: 25px;" OnClientClick="if (!validateAddedNewPoint()) return false;" OnClick="btnUpdate_Click" />
							</td>

							<td>
								<asp:Button ID="btnAddAndReset" runat="server" Text="Add Order & Reset" Style="width: 130px; height: 25px;" OnClientClick="if (!validateAddedNewPoint()) return false;" ToolTip="Click to submit your order and reset all the field values back to their default settings." />
								<input type="button" value="Back To List" runat="server" id="btnBack" onclick="javascript:window.location = 'ClientOrderList.aspx';" style="width: 130px; height: 25px;" />
							</td>
						</tr>
						<tr runat="server" id="tblRowCancelOrder">
							<td>
								<input type="button" id="btnCancel" runat="server" value="Cancel Order" style="width: 130px; height: 25px;" onclick="javascript:if(!confirm('Are you sure you want to cancel the order?')){return;}; cancelClick = true;" />
							</td>
							<td>
								<b>Click to cancel your order.</b>
							</td>
						</tr>
					</table>
					<br />
				</div>

<div style="text-align: center;">
	<asp:Label Visible="false" runat="server" ID="lblCancelledIndicator2" Font-Size="Large" ForeColor="Red" Text="This order has been cancelled <a href='ClientOrderList.aspx'>Click here to return to your list of orders</a><div style='height: 40px;'></div>"></asp:Label>
</div>

<input id="hidDeliveryTimingMethod" type="hidden" runat="server" value="window" />
<input id="hidCollectionTimingMethod" type="hidden" runat="server" value="window" />
<input runat="server" id="hidCollectionFromTimeRestoreValue" type="hidden" />
<input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
<input runat="server" id="hidCollectionByTimeRestoreValue" type="hidden" />
<input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
<input runat="server" id="hidShowPalletNetworkFields" type="hidden" />

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
	<script type="text/javascript" language="javascript">

	    // Fix for the Modal Dialog box functionality being removed from Chrome 37.
        // This code can be removed once the Modal Dialog Boxes have been removed.
	    if (!window.showModalDialog) {
	        window.showModalDialog = function (arg1, arg2, arg3) {

	            var w;
	            var h;
	            var resizable = "no";
	            var scroll = "no";
	            var status = "no";

	            // get the modal specs
	            var mdattrs = arg3.split(";");
	            for (i = 0; i < mdattrs.length; i++) {
	                var mdattr = mdattrs[i].split("=");

	                var n = mdattr[0];
	                var v = mdattr[1];
	                if (n) { n = n.trim().toLowerCase(); }
	                if (v) { v = v.trim().toLowerCase(); }

	                if (n == "dialogheight") {
	                    h = v.replace("px", "");
	                } else if (n == "dialogwidth") {
	                    w = v.replace("px", "");
	                } else if (n == "resizable") {
	                    resizable = v;
	                } else if (n == "scroll") {
	                    scroll = v;
	                } else if (n == "status") {
	                    status = v;
	                }
	            }

	            var left = window.screenX + (window.outerWidth / 2) - (w / 2);
	            var top = window.screenY + (window.outerHeight / 2) - (h / 2);
	            var targetWin = window.open(arg1, arg1, 'toolbar=no, location=no, directories=no, status=' + status + ', menubar=no, scrollbars=' + scroll + ', resizable=' + resizable + ', copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
	            targetWin.focus();
	        };
	    }

        var focusTimeout = -1;
		var focusControl = null;

        function validateSpaces(sender, eventArgs) {
            eventArgs.IsValid = false;

            if (eventArgs.Value < 0 || eventArgs.Value > 999)
                eventArgs.IsValid = false;
            else
                eventArgs.IsValid = true;
        }

        function validatePallets(sender, eventArgs) {
            eventArgs.IsValid = false;

            if (eventArgs.Value < 0 || eventArgs.Value > 999)
                eventArgs.IsValid = false;
            else
                eventArgs.IsValid = true;
        }

        function validateWeight(sender, eventArgs) {
            eventArgs.IsValid = false;

            if (eventArgs.Value < 0 || eventArgs.Value > 99999)
                eventArgs.IsValid = false;
            else
                eventArgs.IsValid = true;
        }

        function validateCases(sender, eventArgs) {
            eventArgs.IsValid = false;

            if (eventArgs.Value < 0 || eventArgs.Value > 99999)
                eventArgs.IsValid = false;
            else
                eventArgs.IsValid = true;
        }

		function validateAddedNewPoint() {
			var collectionPoint = <%=ucCollectionPoint.ClientID %>_checkAddedNewPoint();
			var deliveryPoint = <%=ucDeliveryPoint.ClientID %>_checkAddedNewPoint();

			if(!collectionPoint || !deliveryPoint)
				alert("Please ensure that you have clicked \"Create Point\" before adding your order.");

			return collectionPoint && deliveryPoint;
		}

		function GiveFocus()
		{
			clearTimeout(focusTimeout);
			focusControl.focus();
			focusControl = null;
		}

		function SetFocus(controlID)
		{
			focusControl = null;
			if (controlID != "")
			{
				focusControl = $get(controlID);
				if (focusControl != null)
				{
					focusTimeout = setTimeout("GiveFocus()", 500);
				}
			}
		}
	
		// This fires once the ASP.NET Ajax engine has initialised and is available.
		function pageLoad() {
			
			var collectionMethod = $('input:hidden[id*=hidCollectionTimingMethod]').val();
			var deliveryMethod = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

			//$('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

			if (deliveryMethod == 'anytime') {
				deliveryIsAnytime(null);
			}
			
			if (deliveryMethod == 'timed') {
				deliveryTimedBooking(null);
			}
			
			if (collectionMethod == 'anytime') {
				collectionIsAnytime(null);
			}
		
			if (collectionMethod == 'timed') {
				collectionTimedBooking(null);
			}
		
			var rntRate = $find('ctl00_ContentPlaceHolder1_ucOrder_rntRate');

			if (rntRate != null) {
				// Only need to hookup the rate listeners if the rate is visible.
				HookupRateListeners();
			}
			
			// Set the culture information so prices can be manipulated for the correct culture of the client.
			if (rntRate != null) {
				Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol = rntRate.get_numberFormat().PositivePattern.replace(/\w/g, '');
			}

			// If this is a new order and the previous order created had a rate, re rate the page so the surcharges are shown.
			if ('<%=this.IsUpdate.ToString() %>' == '<%=System.Boolean.FalseString %>') {
				LookupRate();
			}

			var serviceLevelCombo = $find("<%=cboService.ClientID %>");
            updateDeliveryDateForServiceLevel(serviceLevelCombo);

			//CalculateTotal();
		}

		function LookupRate() {
			var problemEncountered = false;
			var spnRateError = $get('spnRateError');
			if (spnRateError != null)
				spnRateError.innerHTML = '';
				
			// Do not rate manually rated orders
			if ('<%=this.IsUpdate.ToString() %>' == '<%=System.Boolean.FalseString %>' || '<%=this.SavedOrder == null ? System.Boolean.FalseString : this.SavedOrder.IsTariffOverride.ToString() %>' == '<%=System.Boolean.FalseString %>') {
				// Retrieve the various values from all the controls.
				// Client Identity ID
				var clientIdentityID = <%=this.ClientIdentityID.ToString() %>;

				// Business Type ID
				var businessTypeID = <%=this.BusinessTypeID.ToString() %>;
				
				// Order Instruction ID
				var orderInstructionID = <%=this.OrderInstructionID.ToString() %>;

				// Collection Point ID
				var collectionPointID = 0;
				var collectionPointCombo = $find('<%=this.ucCollectionPoint.ComboClientID %>');
				if (collectionPointCombo != null) {
					// Get the second parameter in the comma delimited list
					var collectionPointParts = collectionPointCombo.get_value().split(',');
					if (collectionPointParts.length == 2)
						collectionPointID = collectionPointParts[1];
				}

				// Delivery Point ID
				var deliveryPointID = 0;
				var deliveryPointCombo = $find('<%=this.ucDeliveryPoint.ComboClientID %>');
				if (deliveryPointCombo != null) {
					// Get the second parameter in the comma delimited list
					var deliveryPointParts = deliveryPointCombo.get_value().split(',');
					if (deliveryPointParts.length == 2)
						deliveryPointID = deliveryPointParts[1];
				}

				// Pallet Type ID
				var palletTypeID = 0;
				var palletTypeCombo = $find('<%=this.cboPalletType.ClientID %>');
				if (palletTypeCombo != null) palletTypeID = palletTypeCombo.get_value();

				// Pallet Count
				var palletCount = 0;
				var palletCountInput = $get('<%=this.txtNoPallets.ClientID %>');
			    if (palletCountInput != null) {
			        if (palletCountInput.value != "") palletCount = palletCountInput.value;
			    }

				// Pallet Space
				var palletSpaceID = 1;
				
				// Weight
				var weight = $('#' + '<%=this.txtWeight.ClientID %>').val();                

				// Goods Type ID
				var goodsTypeID = 0;
				var goodsTypeCombo = $find('<%=this.cboGoodsType.ClientID %>');
				if (goodsTypeCombo != null) goodsTypeID = goodsTypeCombo.get_value();

				// Order Service Level ID
				var orderServiceLevelID = 0;
				var serviceLevelCombo = $find('<%=this.cboService.ClientID %>');
				if (serviceLevelCombo != null) orderServiceLevelID = serviceLevelCombo.get_value();

				// Collection Date
				var collectionDate = new Date();
				var collectionFromDateDate = $find('<%=this.dteCollectionFromDate.ClientID %>');
				var collectionByDateDate = $find('<%=this.dteCollectionByDate.ClientID %>');
				var collectionFromDateTime = $find('<%=this.dteCollectionFromTime.ClientID %>');
				if (collectionFromDateDate != null && collectionFromDateTime != null) {
					var date = collectionFromDateDate.get_selectedDate();
					if (date != null) {
						var time = collectionFromDateTime.get_selectedDate();
						collectionDate.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());
						if (time == null) {
							// Anytime is specified
							collectionDate.setHours(23, 59, 59, 0);
						}
						else {
							collectionDate.setHours(time.getHours(), time.getMinutes(), 0, 0);
						}
					}
					else {
						problemEncountered = true;
					}
				}

				// Delivery Date
				var deliveryDate = new Date();
				var deliveryDateDate = $find('<%=this.dteDeliveryByDate.ClientID %>');
				var deliveryDateTime = $find('<%=this.dteDeliveryByTime.ClientID %>');
				if (deliveryDateDate != null && deliveryDateTime != null) {
					var date = deliveryDateDate.get_selectedDate();
					if (date != null) {
						var time = deliveryDateTime.get_selectedDate();
						deliveryDate.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());
						if (time == null) {
							// Anytime is specified
							deliveryDate.setHours(23, 59, 59, 0);
						}
						else {
							deliveryDate.setHours(time.getHours(), time.getMinutes(), 0, 0);
						}
					}
					else {
						problemEncountered = true;
					}
				}
				
				// Have no problems been encountered and all the parameters been supplied yet...
				if (!problemEncountered && clientIdentityID != "" && orderInstructionID != "" && collectionPointID > 0 && deliveryPointID > 0) {
				
					try {
					
					// If customer identity matches the network identity and the business type isPalletNetwork
						// then we need to include the vigo fields in the rating calculation.
						
						var hidShowPalletNetworkFields = $get('<%=this.hidShowPalletNetworkFields.ClientID %>');
						if (hidShowPalletNetworkFields.value == 'true') 
						{
							var rntPalletForceFullPallets = $find('<%=this.rntxtFullPallets.ClientID %>');
							var rntPalletForceHalfPallets = $find('<%=this.rntxtHalfPallets.ClientID %>');
							var rntPalletForceQtrPallets = $find('<%=this.rntxtQtrPallets.ClientID %>');
							var rntPalletForceOverPallets = $find('<%=this.rntxtOverSizePallets.ClientID %>');
							
							var fullPallets = 0;
							var halfPallets = 0;
							var quarterPallets = 0;
							var oversizePallets = 0;
						
							if (rntPalletForceFullPallets.get_value() != "") {
								fullPallets = rntPalletForceFullPallets.get_value();
							}

							if (rntPalletForceHalfPallets.get_value() != "") {
								halfPallets = rntPalletForceHalfPallets.get_value();
							}

							if (rntPalletForceQtrPallets.get_value() != "") {
								quarterPallets = rntPalletForceQtrPallets.get_value();
							}

							if (rntPalletForceOverPallets.get_value() != "") {
								oversizePallets = rntPalletForceOverPallets.get_value();
							}
						
							palletSpaces = fullPallets + halfPallets + quarterPallets + oversizePallets;
							Orchestrator.WebUI.Tariff.Tariffs.GetRateWithClientSurchargesForNetwork(clientIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletCount, weight, goodsTypeID, orderServiceLevelID, collectionDate, deliveryDate, fullPallets, halfPallets, quarterPallets, oversizePallets, LookupRateSuccess, LookupRateFailure);
						} else {
							
							// Call the service!
							Orchestrator.WebUI.Tariff.Tariffs.GetRatewithClientSurcharges(clientIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletCount, weight, goodsTypeID, orderServiceLevelID, collectionDate, deliveryDate, LookupRateSuccess, LookupRateFailure);
						}
					}
					catch (e) {
						var hidTariffRateDescription = $get('<%=this.hidTariffRateDescription.ClientID %>');
						hidTariffRateDescription.value = "";
						var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
						hidTariffRate.value = "";
						var trSurcharges = $get('<%=this.trSurcharges.ClientID %>');
						trSurcharges.style.display = 'none';
						var hidSelectedSurchargeExtraTypeIDs = $get('<%=this.hidSelectedSurchargeExtraTypeIDs.ClientID %>');
						hidSelectedSurchargeExtraTypeIDs.value = '';

						// Hide surcharges.
						var trSurcharges = $get('<%=this.trSurcharges.ClientID %>');
						if (trSurcharges != null) {
							trSurcharges.style.display = 'none';
						}
					}
				}
				else {
					// Hide surcharges.
					var trSurcharges = $get('<%=this.trSurcharges.ClientID %>');
					if (trSurcharges != null) {
						trSurcharges.style.display = 'none';
					}
				}
			}
		}

		// Hookup the event handlers to the various controls that affect the rate,
		// any change in any should result in the rate being looked up with the new information.
		function HookupRateListeners() {
			
			var txtNoPallets = $get('<%=this.txtNoPallets.ClientID %>');
			var goodsTypeCombo = $find('<%=this.cboGoodsType.ClientID %>');
			var serviceLevelCombo = $find('<%=this.cboService.ClientID %>');
			var palletTypeCombo = $find('<%=this.cboPalletType.ClientID %>');
			var collectionPointCombo = $find('<%=this.ucCollectionPoint.ComboClientID %>');
			var collectionFromDateDate = $find('<%=this.dteCollectionFromDate.ClientID %>');
			var collectionFromDateTime = $find('<%=this.dteCollectionFromTime.ClientID %>');
			var deliveryPointCombo = $find('<%=this.ucDeliveryPoint.ComboClientID %>');
			var deliveryDateDate = $find('<%=this.dteDeliveryByDate.ClientID %>');
			var deliveryDateTime = $find('<%=this.dteDeliveryByTime.ClientID %>');
			var rntRate = $find('<%=this.rntRate.ClientID %>');

			var rntPalletNetworkFullPallets = $find('<%=this.rntxtFullPallets.ClientID %>');
			var rntPalletNetworkHalfPallets = $find('<%=this.rntxtHalfPallets.ClientID %>');
			var rntPalletNetworkQtrPallets = $find('<%=this.rntxtQtrPallets.ClientID %>');
			var rntPalletNetworkOverPallets = $find('<%=this.rntxtOverSizePallets.ClientID %>');

			// Remove handlers so we don't get multiple captures.
			// Order Instruction is handled in code behind
			
//            try {
//                $removeHandler($get('<%=this.txtNoPallets.ClientID %>'), 'blur', ReRate); // Pallet Count
//            }
//            catch (ex) { }

			if (rntPalletNetworkFullPallets != null) rntPalletNetworkFullPallets.remove_valueChanged(ReRate);
			if (rntPalletNetworkHalfPallets != null) rntPalletNetworkHalfPallets.remove_valueChanged(ReRate);
			if (rntPalletNetworkQtrPallets != null) rntPalletNetworkQtrPallets.remove_valueChanged(ReRate);
			if (rntPalletNetworkOverPallets != null) rntPalletNetworkOverPallets.remove_valueChanged(ReRate);
			if (txtNoPallets != null) $(txtNoPallets).unbind('blur', ReRate); 
			if (goodsTypeCombo != null) goodsTypeCombo.remove_selectedIndexChanged(ReRate); // Goods Type
			if (serviceLevelCombo != null) serviceLevelCombo.remove_selectedIndexChanged(ReRate); // Service Level
			if (palletTypeCombo != null) palletTypeCombo.remove_selectedIndexChanged(ReRate); // Pallet Type
			if (collectionPointCombo != null) collectionPointCombo.remove_selectedIndexChanged(ReRate); // Collection Point
			if (collectionFromDateDate != null) collectionFromDateDate.remove_valueChanged(ReRate); // Collection Date Time
			if (collectionFromDateTime != null) collectionFromDateTime.remove_valueChanged(ReRate); // Collection Date Time
			if (deliveryPointCombo != null) deliveryPointCombo.remove_selectedIndexChanged(ReRate); // Delivery Point
			if (deliveryDateDate != null) deliveryDateDate.remove_valueChanged(ReRate); // Delivery Date Time
			if (deliveryDateTime != null) deliveryDateTime.remove_valueChanged(ReRate); // Delivery Date Time
			if (rntRate != null) rntRate.remove_valueChanged(CheckForManualRating); // Rate
			
			// Add all the event handlers we need.
			// Order Instruction is handled in code behind
			
			//$addHandler($get('<%=this.txtNoPallets.ClientID %>'), 'blur', ReRate); // Pallet Count
			if (rntPalletNetworkFullPallets != null) rntPalletNetworkFullPallets.add_valueChanged(ReRate);
			if (rntPalletNetworkHalfPallets != null) rntPalletNetworkHalfPallets.add_valueChanged(ReRate);
			if (rntPalletNetworkQtrPallets != null) rntPalletNetworkQtrPallets.add_valueChanged(ReRate);
			if (rntPalletNetworkOverPallets != null) rntPalletNetworkOverPallets.add_valueChanged(ReRate);
			if (txtNoPallets != null) { $(txtNoPallets).bind('blur', ReRate); } // Pallet Count
			if (goodsTypeCombo != null) goodsTypeCombo.add_selectedIndexChanged(ReRate); // Goods Type
			if (serviceLevelCombo != null) serviceLevelCombo.add_selectedIndexChanged(ReRate); // Service Level
			if (palletTypeCombo != null) palletTypeCombo.add_selectedIndexChanged(ReRate); // Pallet Type
			if (collectionPointCombo != null) collectionPointCombo.add_selectedIndexChanged(ReRate); // Collection Point
			if (collectionFromDateDate != null) collectionFromDateDate.add_valueChanged(ReRate); // Collection Date Time
			if (collectionFromDateTime != null) collectionFromDateTime.add_valueChanged(ReRate); // Collection Date Time
			if (deliveryPointCombo != null) deliveryPointCombo.add_selectedIndexChanged(ReRate); // Delivery Point
			if (deliveryDateDate != null) deliveryDateDate.add_valueChanged(ReRate); // Delivery Date Time
			if (deliveryDateTime != null) deliveryDateTime.add_valueChanged(ReRate); // Delivery Date Time
			if (rntRate != null) rntRate.add_valueChanged(CheckForManualRating); // Rate
		}

		function ReRate() {
			LookupRate();
		}

		function CheckForManualRating(sender, eventArgs) {
			var hidTariffRateDescription = $get('<%=this.hidTariffRateDescription.ClientID %>');
			var tariffRateDescription = hidTariffRateDescription.value;
			var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
			tariffRate = hidTariffRate.value;

			// If the user has made the value different from the autorate (if one exists) then flag as overridden.
			if ( tariffRate && eventArgs.get_newValue() != eventArgs.get_oldValue) {
				// The user has overridden the value.
				// Set new total.
				var total = GetTotal();
				total -= eventArgs.get_oldValue();
				total += eventArgs.get_newValue();
				SetTotal(total);
			}
		}
		
		function LookupRateSuccess(result) {
			
			if (result == null || result.Rate == null || result.Rate.Rate == null) {
				var hidTariffRateDescription = $get('<%=this.hidTariffRateDescription.ClientID %>');
				hidTariffRateDescription.value = "";
				var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
				hidTariffRate.value = "";
				var trSurcharges = $get('<%=this.trSurcharges.ClientID %>');
				trSurcharges.style.display = 'none';
				var hidSelectedSurchargeExtraTypeIDs = $get('<%=this.hidSelectedSurchargeExtraTypeIDs.ClientID %>');
				hidSelectedSurchargeExtraTypeIDs.value = '';
			}
			else {
				// Set the rate and rate id.
				var rntRate = $find('<%=this.rntRate.ClientID %>');
				rntRate.set_value(result.Rate.Rate);
				var hidTariffRateDescription = $get('<%=this.hidTariffRateDescription.ClientID %>');
				hidTariffRateDescription.value = result.Rate.TariffDescription; // So we can record the rate used.
				var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
				hidTariffRate.value = result.Rate.Rate; // So we can determine if the rate has been overridden.

				// Start calculating the total.
				var total = result.Rate.Rate;

				var trSurcharges = $get('<%=this.trSurcharges.ClientID %>');
				var hidSelectedSurchargeExtraTypeIDs = $get('<%=this.hidSelectedSurchargeExtraTypeIDs.ClientID %>');
				// Only allow surcharge options for new orders.
				if (result.Surcharges != null && result.Surcharges.length > 0 && '<%=(this.SavedOrder == null).ToString() %>' == '<%=System.Boolean.TrueString %>') {
					// Provide the surcharges as options for the user.
					// Each surcharge is presented as a checkbox that when the checked status changes will result in the
					// hidden fields being updated accordingly.
					
					// Clear out existing Surcharges.
					var newSelection = '';
					var bSurcharges = $('#' + trSurcharges.id).find('#bSurcharges');
					bSurcharges.empty();
					
					$(result.Surcharges).each(function(i) {
						var matchSelection = new RegExp(String.format('((^{0}-)|(\\|{0}-))', this.ExtraTypeID));
						var thisExtraWasSelected = matchSelection.test(hidSelectedSurchargeExtraTypeIDs.value);

						if (thisExtraWasSelected) {
							total += this.Rate;
							if (newSelection.length > 0)
								newSelection += '|';
							newSelection += this.ExtraTypeID + '-' + this.Rate;
						}
						
						bSurcharges.append('<tr><td><input class="selectedSurcharge" type="checkbox" ExtraTypeId="' + this.ExtraTypeID + '" onclick="javascript:handleSurcharge(this.checked, ' + this.ExtraTypeID + ', ' + this.Rate + ');" /></td><td style="padding-left:8px;">' + this.Description + '</td><td style="padding-left:25px;"><b>' + this.DisplayRate + '</b></td></tr>');
					});
					
					if('<%=DisplaySurcharges %>' == '<%=System.Boolean.TrueString %>')
						trSurcharges.style.display = '';
									   
					hidSelectedSurchargeExtraTypeIDs.value = newSelection;
					
					// Ensure that previously selected surcharges are selected.
					if (hidSelectedSurchargeExtraTypeIDs.value != '') {
						var extraTypes = hidSelectedSurchargeExtraTypeIDs.value.split("|");
						
						$('input:checkbox[ExtraTypeId]').prop('checked', false);
						for (var i = 0; i < extraTypes.length; i++)
						{
							var extraTypeID = extraTypes[i].split("-")[0];
						
							$('input:checkbox[ExtraTypeId*=' + extraTypeID + ']')[0].checked = true;
						}
					}
				}
				else {
					// No surcharges to show.
					trSurcharges.style.display = 'none';
					hidSelectedSurchargeExtraTypeIDs.value = '';
				}

				// Update the total.
				SetTotal(total);

				// Tell the user that a zero rate has been supplied.
				if (result.Rate.Rate.Value == 0) {
					alert("The Order has a Rate of Zero.");
				}
			}
		}

		function cboService_SelectedIndexChanged(sender, eventArgs) {
			updateDeliveryDateForServiceLevel(sender);
		}   
		
		function dteCollectionFromDate_SelectedDateChanged(sender, eventArgs) {
			var dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID %>");
			var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");
			var updateDate = false;

			if (dteCollectionByDate != null) {
				if (rdCollectionBookingWindow != null && !rdCollectionBookingWindow.prop("checked"))
					updateDate = true;
				else if (sender.get_selectedDate() > dteCollectionByDate.get_selectedDate())
					updateDate = true;
			}

			if (updateDate) {
				dteCollectionByDate.set_selectedDate(sender.get_selectedDate());
			}
		}

		function dteCollectionByDate_SelectedDateChanged(sender, eventArgs) {
			var serviceLevelCombo = $find("<%=cboService.ClientID %>");
			var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
			var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");

			if (dteCollectionFromDate != null && sender.get_selectedDate() < dteCollectionFromDate.get_selectedDate() && rdCollectionBookingWindow != null && rdCollectionBookingWindow.prop("checked"))
				dteCollectionFromDate.set_selectedDate(sender.get_selectedDate());

			updateDeliveryDateForServiceLevel(serviceLevelCombo);
			
		}

		function dteDeliveryFromDate_SelectedDateChanged(sender, eventArgs) {
			var dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");

			if (dteDeliveryByDate != null && sender.get_selectedDate() > dteDeliveryByDate.get_selectedDate())
				dteDeliveryByDate.set_selectedDate(sender.get_selectedDate());
		}

		function dteDeliveryByDate_SelectedDateChanged(sender, eventArgs) {
			var dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
			var rdDeliveryBookingWindow = $("#" + "<%=rdDeliveryBookingWindow.ClientID%>");
			var updateDate = false;

			if (dteDeliveryFromDate != null) {
				if (sender.get_selectedDate() < dteDeliveryFromDate.get_selectedDate())
					updateDate = true;
				else if (rdDeliveryBookingWindow != null && !rdDeliveryBookingWindow.prop("checked"))
					updateDate = true;
			}

			if (updateDate)
				dteDeliveryFromDate.set_selectedDate(sender.get_selectedDate());
	   }

		function updateDeliveryDateForServiceLevel(serviceLevelCombo) {
			var orderServiceLevelID = -1;
			var deliveryDate = new Date();
			var collectionDate = new Date();
			var noOfDays = null;

			if (serviceLevelCombo != null) orderServiceLevelID = serviceLevelCombo.get_value();

			var dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID %>");
			if (dteCollectionByDate != null) collectionDate = dteCollectionByDate.get_selectedDate();

			var dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
			var dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");
			if (dteDeliveryByDate != null) deliveryDate = dteDeliveryByDate.get_selectedDate();

			// Gets the number of days set for the specified service level.
			if (serviceLevelDays != null && orderServiceLevelID != -1)
				noOfDays = jQuery.grep(serviceLevelDays, function(a) { return a[0] == orderServiceLevelID });

			if (noOfDays.length > 0 && noOfDays[0][1] != -1) {

				var selectedNoOfDays = parseInt(noOfDays[0][1], 10);
				// Calculates the date from collection date with the number of days for that service level.
				deliveryDate = new Date(collectionDate.getFullYear(), collectionDate.getMonth(), collectionDate.getDate());
				deliveryDate.setDate(deliveryDate.getDate() + 0);
				
				//var holidayDate = jQuery.grep(invalidDates, function(a) { return Date.parse(a.Date.toLocaleString()) == Date.parse(deliveryDate.toLocaleString()); });
				var holidayDate = jQuery.grep(invalidDates, function(a) { return a.Date.toLocaleString() == deliveryDate.toLocaleString(); });
				var dayCounter = 1;
				
				while (holidayDate.length > 0 || selectedNoOfDays > 0) {
					deliveryDate = new Date(collectionDate.getFullYear(), collectionDate.getMonth(), collectionDate.getDate());
					deliveryDate.setDate(deliveryDate.getDate() + dayCounter);
				    //holidayDate = jQuery.grep(invalidDates, function(a) { return Date.parse(a.Date.toLocaleString()) == Date.parse(deliveryDate.toLocaleString()); });
					holidayDate = jQuery.grep(invalidDates, function(a) { return a.Date.toLocaleString() == deliveryDate.toLocaleString(); });
					
					if(holidayDate.length == 0)
						selectedNoOfDays--;
						
					dayCounter++;
				}

				dteDeliveryFromDate.set_selectedDate(deliveryDate);
				dteDeliveryByDate.set_selectedDate(deliveryDate);
			}
		}  

		function handleSurcharge(isChecked, extraTypeId, rate) {
			var hidSelectedSurchargeExtraTypeIDs = $get('<%=this.hidSelectedSurchargeExtraTypeIDs.ClientID %>');

			var total = GetTotal();

			if (isChecked) {
				if (hidSelectedSurchargeExtraTypeIDs.value.length > 0) {
					hidSelectedSurchargeExtraTypeIDs.value += '|';
				}

				hidSelectedSurchargeExtraTypeIDs.value += extraTypeId + '-' + rate;
				total += rate;
			}
			else {
				var removeSelection = new RegExp(String.format('((^{0}-{1}$)|(^{0}-{1}\\|)|(\\|{0}-{1}\\|)|(\\|{0}-{1}$))', extraTypeId, rate));
				hidSelectedSurchargeExtraTypeIDs.value = hidSelectedSurchargeExtraTypeIDs.value.replace(removeSelection, function(theMatch) { if (theMatch.startsWith('|') && theMatch.endsWith('|')) return '|'; else return ''; });
				total -= rate;
			}

			SetTotal(total);
		}

		function LookupRateFailure(error, userContext, methodName) {
			
			// Hide surcharges.
			var trSurcharges = $get('<%=this.trSurcharges.ClientID %>');
			if (trSurcharges != null) {
				trSurcharges.style.display = 'none';
			}

			var spnRateError = $get('spnRateError');
			if (spnRateError != null)
				spnRateError.innerHTML = 'This order could not be rated.<br />' + error.get_message();
		}

		function GetTotal() {
			// Retrieve the total.
			var spnTotal = $get('spnTotal');
			return Number.parseLocale(StripCurrency(spnTotal.innerHTML));
		}

		function SetTotal(val) {
			// Update the total.
			var spnTotal = $get('spnTotal');
			
			spnTotal.innerHTML = String.localeFormat("{0:c}", val);
		}

		function StripCurrency(val) {
			stripped = val.replace(new RegExp('\\' + Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol), '');
			if (val == stripped)
				stripped = val.replace(new RegExp(Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol), '');
			return stripped;
		}
			
//        function formatCurrency(el)
//        {
//            var charge = new NumberFormat(el.value) 
//            
//            charge.setCurrencyPrefix('£');
//            charge.setCurrency(true);
//            el.value = charge.toFormatted();
//        }
//        
		var _skipCollectionDateCheck = false;
		var _skipDeliveryDateCheck = false;

		var dteCollectionByTime = $find("<%=dteCollectionByTime.ClientID %>");
		var dteCollectionFromTime = $find("<%=dteCollectionFromTime.ClientID %>");
		 
		function CV_ClientValidateDeliveryDate(source, args) {
			var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
			var IsUpdate = $get("<%=pageIsUpdate.ClientID%>");

			if (IsUpdate.value != "True") {
				var today = new Date();
				var day_date = today.getDate();
				var month_date = today.getMonth();
				var year_date = today.getFullYear();

				today.setFullYear(year_date, month_date, day_date);

				var enteredDateParts = dteDateTime.get_dateInput().get_displayValue().split("/");

				var enteredDate = new Date();
				enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

				if (enteredDate >= today) {
					args.IsValid = true;
				}
				else {
					if (!_skipCollectionDateCheck) {
						_skipCollectionDateCheck = true;
						args.IsValid = false;
						//alert("The date entered is in the past - Are you sure?");
					}
				}
			}
		}

		function CV_ClientValidateDeliveryDate2(source, args) {
			var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
			var IsUpdate = $get("<%=pageIsUpdate.ClientID%>");
			var hidShowConfirmForOrderAfterDays = $get("<%=hidShowConfirmForOrderAfterDays.ClientID%>");

			if (IsUpdate.value != "True") {
				var warningDate = new Date();
				var day_date = warningDate.getDate();
				var month_date = warningDate.getMonth();
				var year_date = warningDate.getFullYear();

				warningDate.setFullYear(year_date, month_date, day_date);
				warningDate.setDate(warningDate.getDate() + parseInt(hidShowConfirmForOrderAfterDays.value));

				var enteredDateParts = dteDateTime.get_dateInput().get_displayValue().split("/");

				var enteredDate = new Date();
				enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);



				if (enteredDate >= warningDate) {
					if (!_skipCollectionDateCheck) {
						_skipCollectionDateCheck = true;

						args.IsValid = true; // do not prevent the order from being created.
						alert("The date entered is far in the future - Are you sure?");
					}
				}
				else {
					args.IsValid = true;
				}
			}
		}

		function CV_ClientValidateCollectionDate(source, args) {

			var dteDateTime = $find("<%=dteCollectionFromDate.ClientID%>");
			var IsUpdate = $get("<%=pageIsUpdate.ClientID%>");

			if (IsUpdate.value != "True" && dteDateTime.get_dateInput() !="") {

				var today = new Date();
				var day_date = today.getDate();
				var month_date = today.getMonth();
				var year_date = today.getFullYear();

				today.setFullYear(year_date, month_date, day_date);

				var enteredDateParts = dteDateTime.get_dateInput().get_displayValue().split("/");

				var enteredDate = new Date();
				enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

				if (enteredDate >= today) {
					args.IsValid = true;

				}
				else {
					if (!_skipDeliveryDateCheck) {
						_skipDeliveryDateCheck = true;
						args.IsValid = false;
						//alert("The date entered is in the past - Are you sure?");
					}
				}
			}
		}
		function collectionIsAnytime(rd) {
			$('tr[id*=trCollectBy]').hide();

			//var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
			//dteCollectionFromDate.focus();
		    //dteCollectionFromDate.select();

			var dteCollectionFromDate = $find("<%= dteCollectionFromDate.ClientID %>");
		    dteCollectionFromDate.get_dateInput().focus();

		    var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

		    if (method == "anytime")
		    {
		        if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") 
		        {
		            $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value('00:00');
		            $find("<%=dteCollectionFromTime.ClientID %>").set_enabled(false);
		            return;
		        }
		    }

			$('input:hidden[id*=hidCollectionTimingMethod]').val('anytime');

			$('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val($find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().get_value());
			$('input:hidden[id*=hidCollectionByTimeRestoreValue]').val($find("<%=dteCollectionByTime.ClientID %>").get_dateInput().get_value());

			$find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value('00:00');
			$find("<%=dteCollectionFromTime.ClientID %>").set_enabled(false);

			$find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value('23:59');
			$find("<%=dteCollectionByTime.ClientID %>").set_enabled(false);
		}

		function deliveryIsAnytime(rd) {
			$('tr[id*=trDeliverFrom]').hide();

			//var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
			//dteDeliveryFromDate.focus();
		    //dteDeliveryFromDate.select();

			dteDeliveryByDate = $find("<%= dteDeliveryByDate.ClientID %>");
		    dteDeliveryByDate.get_dateInput().focus();

		    var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

		    if (method == "anytime")
		    {
		        if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") 
		        {
		            $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value('23:59');
		            $find("<%=dteDeliveryByTime.ClientID %>").set_enabled(false);
		            return;
		        }
		    }

			$('input:hidden[id*=hidDeliveryTimingMethod]').val('anytime');

			$('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().get_value());
			$('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val($find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().get_value());

			$find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value('00:00');
			$find("<%=dteDeliveryFromTime.ClientID %>").set_enabled(false);

			$find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value('23:59');
			$find("<%=dteDeliveryByTime.ClientID %>").set_enabled(false);
		}

		function collectionTimedBooking(rb) {

			$('tr[id*=trCollectBy]').hide();

			//var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
			//dteCollectionFromDate.focus();
			//dteCollectionFromDate.select();

			var dteCollectionFromDate = $find("<%= dteCollectionFromDate.ClientID %>");
		    dteCollectionFromDate.get_dateInput().focus();

			var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

			$('input:hidden[id*=hidCollectionTimingMethod]').val('timed');

			if (method == 'window') {
				$('input:hidden[id*=hidCollectionByTimeRestoreValue]').val($find("<%=dteCollectionByTime.ClientID %>").get_dateInput().get_value());
			}

			$find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().get_value());

			if (method == 'anytime') {
				if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
				    $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

				} else {
					$find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

				if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
				    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
				} else {
				    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
			}

			$find("<%=dteCollectionByTime.ClientID %>").set_enabled(true);
			$find("<%=dteCollectionFromTime.ClientID %>").set_enabled(true);
		}

		function collectionBookingWindow(rb) {
			$('tr[id*=trCollectBy]').show();
			//var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
			//dteCollectionFromDate.focus();
		    //dteCollectionFromDate.select();

			var dteCollectionFromDate = $find("<%= dteCollectionFromDate.ClientID %>");
		    dteCollectionFromDate.get_dateInput().focus();

			var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

			$('input:hidden[id*=hidCollectionTimingMethod]').val('window');

			if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
				$find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
			}

			if (method == 'anytime') {
				if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
					$find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

				} else {
					$find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

				if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
					$find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
				} else {
					$find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
			}

			$find("<%=dteCollectionByTime.ClientID %>").set_enabled(true);
			$find("<%=dteCollectionFromTime.ClientID %>").set_enabled(true);
		}

		function deliveryTimedBooking(rb) {
			$('tr[id*=trDeliverFrom]').hide();

			//var dteDeliveryByDate = $('input[id*=dteDeliveryByDate]');
			//dteDeliveryByDate.focus();
		    //dteDeliveryByDate.select();

			var dteDeliveryByDate = $find("<%= dteDeliveryByDate.ClientID %>");
		    dteDeliveryByDate.get_dateInput().focus();

			var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

			$('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

			if (method == 'window') {
				$('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().get_value());
			}

			$find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().get_value());

			if (method == 'anytime') {
				if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
				    $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
				} else {
				    $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

				if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
				    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
				} else {
				    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
			}

		    $find("<%=dteDeliveryFromTime.ClientID %>").set_enabled(true);
		    $find("<%=dteDeliveryByTime.ClientID %>").set_enabled(true);
		}

		function deliveryBookingWindow(rb) {
			$('tr[id*=trDeliverFrom]').show();
			//var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
			//dteDeliveryFromDate.focus();
			//dteDeliveryFromDate.select();

			var dteDeliveryFromDate = $find("<%= dteDeliveryFromDate.ClientID %>");
		    dteDeliveryFromDate.get_dateInput().focus();

			var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

			$('input:hidden[id*=hidDeliveryTimingMethod]').val('window');

			if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val()) {
				$find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
			}

			if (method == 'anytime') {
				if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
					$find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
				} else {
					$find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

				if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
					$find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
				} else {
					$find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
			}

		    $find("<%=dteDeliveryFromTime.ClientID %>").set_enabled(true);
		    $find("<%=dteDeliveryByTime.ClientID %>").set_enabled(true);
		}
	//-->
	</script>
</telerik:RadCodeBlock>

<asp:Panel ID="pnlSetFocus" runat="server">
	<asp:Literal ID="litSetFocus" runat="server" EnableViewState="False"></asp:Literal>
</asp:Panel>

<telerik:RadAjaxManager ID="raxManager" runat="server">
	<AjaxSettings>
		<telerik:AjaxSetting AjaxControlID="ucCollectionPoint">
			<UpdatedControls>
				<telerik:AjaxUpdatedControl ControlID="pnlSetFocus" />
			</UpdatedControls>
		</telerik:AjaxSetting>
		<telerik:AjaxSetting AjaxControlID="ucDeliveryPoint">
			<UpdatedControls>
				<telerik:AjaxUpdatedControl ControlID="pnlSetFocus" />
			</UpdatedControls>
		</telerik:AjaxSetting>
	</AjaxSettings>
</telerik:RadAjaxManager>

</asp:Content>