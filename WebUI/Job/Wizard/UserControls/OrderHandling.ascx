<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.OrderHandling" Codebehind="OrderHandling.ascx.cs" %>
<%@ Register TagPrefix="uc" TagName="Point" Src="~/UserControls/point.ascx" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<script type="text/javascript" language="javascript" src="../../script/tooltippopups.js"></script>
<script language="javascript" type="text/javascript">
<!--
    width = 950;
    height = 750;

    var totalWeight = 0;
    var totalPallets = 0;
    function RowSelected(row)
    {
        totalPallets += row.KeyValues["NoPallets"];
    }
    
    function RowDeSelected(row)
    {
        
        totalPallets -= row.KeyValues["NoPallets"];
    }
    
    function showTotals()
    {
       var el = document.getElementById("dvSelected"); 
       el.innerHtml = "<b>Pallets : " + totalPallets + "</b>";
    }
//-->
</script>

<table width="100%" height="472" cellpadding="0" cellspacing="0" border="0" id="tblMain">
	<tr height="56">
		<td bgcolor="white" align="right" style="PADDING-RIGHT:10px">
			<table width="100%" cellpadding="0" cellspacing="0" align="right">
				<tr>
					<td width="100%" valign="top" align="left">
						<div style="PADDING-LEFT:20px; FONT-WEIGHT:bold; PADDING-TOP:5px"><STRONG>
							<asp:Label id=lblCollectDrop runat="server" Font-Size="Medium"></asp:Label></STRONG>
						</div>
			            <div style="PADDING-LEFT: 20px; FONT-WEIGHT: bold; PADDING-TOP: 5px">Configure Orders Handling</div>
						<div style="PADDING-LEFT:35px;PADDING-TOP:2px">
							Specify what you intend to do with the orders you've selected for collection.
						</div>
					</td>
					<td align="right">
						<img src="../../images/p1logo.gif" width="50" height="50" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr height="1" bgcolor="#aca899">
		<td colspan="2" height="1"></td>
	</tr>
	<tr height="305" bgcolor="#ece9d8">
		<td style="PADDING-RIGHT:10px;PADDING-LEFT:35px;VERTICAL-ALIGN:top;PADDING-TOP:10px" width="100%">
            <uc1:infringementDisplay id="idErrors" runat="server" visible="false" />
            <div style="display:<%=CurrentInstruction.InstructionID > 0 ? "none" : "inline"%>">
                <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Order Collection</div>
                    <p>You have configured this collection with a booked date/time of <asp:Label ID="lblBookedDateTime" runat="server" />.</p>
                    <asp:PlaceHolder ID="plcSubstitute" runat="server">
                        <p>You can <asp:LinkButton ID="lnkUseEarliestTime" runat="server" CausesValidation="false">substitute this time for the earliest</asp:LinkButton> booked date/time from the orders you have selected.</p>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcUseInstruction" runat="server">
                        <p>There is already a collection on this job that you could add these orders to, <asp:LinkButton ID="lnkUseCollection" runat="server" CausesValidation="false">click to add orders to existing instruction.</asp:LinkButton><asp:HiddenField id="hidUseInstructionID" runat="server" Value="0" /></p>
                    </asp:PlaceHolder>
                </fieldset>
            </div>
			<div>
                <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Order Handling</div>
                    <p>You can override the default handling of one or more of the orders shown below by providing a new location, time and action using this panel.  Select the orders to affect in the grid below and click the "Update Order Handling" button to process your change.</p>
		            <table width="100%" border="0" cellpadding="2" cellspacing="0">
		                <tr>
		                    <td colspan="2" width="50%">
		                        <uc:Point id="ucPoint" runat="server" CanCreateNewPoint="True" CanChangePoint="True" />
		                    </td>
		                    <td></td>
		                </tr>
		                <tr>
				            <td valign="top" colspan="3">
					            <table width="500" cellpadding="0" cellspacing="0" border="0">
						            <tr>
						                <td nowrap="nowrap"><span>Booked Date and Time</span>&nbsp;</td>
							            <td nowrap="nowrap"><telerik:RadDateInput id="dteBookedDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Booked Date"></telerik:RadDateInput><asp:CustomValidator ID="cfvValidDateRange" Runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="The Date must be no more than 1 day before today and no more than 3 days ahead of today."><img src="../../images/error.png" Title="The Date must be no more than 1 day before today and no more than 3 days ahead of today."></asp:CustomValidator></td>
									    <td width="110"><telerik:RadDateInput id="dteBookedTime" runat="server" dateformat="t" DataMode="EditModeText" NullText="AnyTime" Nullable="True"></telerik:RadDateInput></td>
									    <td width="280"><asp:requiredfieldvalidator id="rfvBookedDate" runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" ErrorMessage="Please enter a Booked Date and Time."><img src="../../images/error.png" Title="Please enter a Booked Date and Time."></asp:requiredfieldvalidator><asp:CustomValidator ID="cfvBookedDate" Runat="server" ControlToValidate="dteBookedDate" Display="Dynamic" EnableClientScript="False" ErrorMessage="Booked Dates for collections must occur before the first delivery, and for deliveries the booked date must occur after the last collection."><img src="../../images/error.png" Title="Booked Dates for collections must occur before the first delivery, and for deliveries the booked date must occur after the last collection."></asp:CustomValidator></td>
						            </tr>
					            </table>
				            </td>
		                </tr>
		                <tr>
		                    <td colspan="3">
		                        Order Handling: <asp:RadioButtonList ID="rdoOrderAction" runat="server" RepeatDirection="horizontal" RepeatColumns="6" />
		                    </td>
		                </tr>
		                <tr>
		                    <td colspan="3" align="right">
                                <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">
    		                        <asp:Button ID="btnUpdateOrderHandling" runat="server" Text="Update Order Handling" />
		                        </div>
		                    </td>
		                </tr>
			            <tr>
			                <td colspan="2">
			                    <asp:Repeater ID="repValidationMessages" runat="server" EnableViewState="False">
			                        <HeaderTemplate>
			                            <b>Your changes may not be valid, please review the following messages and try again.</b>
			                            <table width="500" cellpadding="2" cellspacing="1" border="0">
			                        </HeaderTemplate>
			                        <ItemTemplate>
			                            <tr>
			                                <td width="35" align="center" valign="top"><asp:Image ID="imgIcon" runat="server" /></td>
			                                <td width="465" align="left" valign="middle"><asp:Literal ID="litText" runat="server" /></td>
			                            </tr>
			                        </ItemTemplate>
			                        <FooterTemplate>
			                            </table>
			                        </FooterTemplate>
			                    </asp:Repeater>
			                </td>
			            </tr>
		            </table>
	            </fieldset>
                <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Current Order Handling</div>
                    <telerik:RadCodeBlock runat="server">
                    <telerik:RadGrid runat="server" ID="gvOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="false" AutoGenerateColumns="false" AllowMultiRowSelection="true" AllowFilteringByColumn="false" AllowAutomaticInserts="false">
                        <MasterTableView Width="100%" DataKeyNames="OrderID" AllowFilteringByColumn="false" >
                           <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                            <Columns>
                                <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="40" HeaderText=""></telerik:GridClientSelectColumn>
                                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName" UniqueName="CustomerOrganisationName" DataField="CustomerOrganisationName" ItemStyle-Wrap="false" AllowFiltering="true" DataType="System.String" ></telerik:GridBoundColumn>
                                <telerik:GridBoundColumn HeaderText="Order Action" SortExpression="Order Action" DataField="OrderAction"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Take To" SortExpression="DeliveryPointDescription" HeaderStyle-Width="150" AllowFiltering="false">
                                    <ItemTemplate>
                                        <span id="spnDeliveryPoint" onClick="" onMouseOver="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);" onMouseOut="hideAd();" class="orchestratorLink"><b><asp:LinkButton ID="lnkDeliveryPoint" runat="server" CausesValidation="false" CommandName="PopulatePoint"><%# ((System.Data.DataRowView)Container.DataItem)["DeliveryPointDescription"]%></asp:LinkButton></b></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Arrive At" SortExpression="DeliveryDateTime" ItemStyle-Width="150" ItemStyle-Wrap="false" AllowFiltering="false"  >
                                    <ItemTemplate><%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="DON" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" />
                                <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription" DataField="GoodsTypeDescription"  AllowFiltering="false"/>
                                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="60"  AllowFiltering="false"/>
                                <telerik:GridBoundColumn HeaderText="Pallets" SortExpression="NoPallets" DataField="NoPallets" HeaderStyle-Width="60"  AllowFiltering="false"/>
                                <telerik:GridBoundColumn HeaderText="Pallet Type" SortExpression="PalletTypeDescription" DataField="PalletTypeDescription" HeaderStyle-Width="80"  AllowFiltering="false" Visible="false"/>
                                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="80" AllowFiltering="false">
                                    <ItemTemplate>
                                        <%# ((decimal)((System.Data.DataRowView)Container.DataItem)["Weight"]).ToString("F4")%>
                                        <%# (string)((System.Data.DataRowView)Container.DataItem)["WeightShortCode"]%>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Delivery" SortExpression="DeliveryPointDescription" HeaderStyle-Width="150" AllowFiltering="false">
                                    <ItemTemplate>
                                        <span id="spnFinalDeliveryPoint" runat="server" onClick="" onMouseOut="hideAd();" class="orchestratorLink"><b><asp:LinkButton ID="lnkFinalDeliveryPoint" runat="server" CausesValidation="false" CommandName="PopulateDeliveryPoint"></asp:LinkButton></b></span>
                                        <br />
                                        <asp:Label ID="lblDeliveryDateTime" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn HeaderText="Notes" DataField="Notes" ItemStyle-Wrap="false"  AllowFiltering="false" Visible="false"/>
                                <telerik:GridBoundColumn HeaderText="IsDirty" DataField="IsDirty" Visible="false" />
                            </Columns>            
                        </MasterTableView>
                        <ClientSettings ApplyStylesOnClient="true" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                            <Resizing AllowColumnResize="true" AllowRowResize="false" />
                            <Selecting AllowRowSelect="true" />
                            <ClientEvents OnRowSelecting="OrdersRowSelecting" OnRowDeselecting="OrdersRowDeselecting" />
                        </ClientSettings>
                        <FilterMenu CssClass="FilterMenuClass" ></FilterMenu>
                    </telerik:RadGrid>
                    </telerik:RadCodeBlock>
                </fieldset>
                <asp:PlaceHolder ID="plcLegTimeAlteringMode" runat="server">
                    <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
                        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Leg Time Alteration Method</div>
                        <p>
	                        The actions you are taking may require the leg times to be altered, you can control the level the leg times are changed by selecting one of the following options. 
	                        <asp:RadioButtonList ID="rdoLegTimeAlteringMode" runat="server" RepeatDirection="Horizontal" RepeatColumns="6" />
                        </p>
                    </fieldset>
                </asp:PlaceHolder>
			</div>
		</td>
	</tr>
	<tr height="1" bgcolor="#aca899">
		<td colspan="2" height="1" ></td>
	</tr>
	<tr height="46" bgcolor="#ece9d8">
		<td colspan="2" align="right" height="46" style="PADDING-RIGHT:10px">
			<asp:Button ID="btnBack" Runat="server" CausesValidation="False" Text="< Back"></asp:Button>
			<asp:button id="btnNext" runat="server" CausesValidation="False" Text="Next >"></asp:button>
			&nbsp;&nbsp;
			<asp:Button ID="btnCancel" Runat="server" CausesValidation="False" Text="Cancel"></asp:Button>
		</td>
	</tr>
</table>

<script language="javascript" type="text/javascript">
<!--
    function OrdersRowSelecting(rowObject)
    {
        var cell = this.GetCellByColumnUniqueName(rowObject, "checkboxSelectColumn");
        return RowSelectingHelper(cell);
    }
    
    function OrdersRowDeselecting(rowObject)
    {
        var cell = this.GetCellByColumnUniqueName(rowObject, "checkboxSelectColumn");
        return RowDeselectingHelper(cell);
    }
//-->
</script>