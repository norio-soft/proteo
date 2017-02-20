<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="OrderBasedRedelivery.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.JobManagement.OrderBasedRedelivery" MasterPageFile="~/WizardMasterPage.Master" %>

<%@ Register TagPrefix="orchestrator" TagName="feedbackPanel" Src="~/UserControls/feedbackPanel.ascx" %>
<%@ Register TagPrefix="orchestrator" TagName="point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/toolTipPopUps.js"></script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Redelivery of Orders</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog runat="server" ID="dlgOrder" ReturnValueExpected="true" AutoPostBack="true" URL="/groupage/manageorder.aspx" Height="900" Width="1200" Mode="Modal" />

    <h1>Full refusal/redelivery details</h1>
    
    
    <asp:MultiView ID="mv" runat="server">
        <asp:View ID="vwIncorrectDataSupplied" runat="server">
            <orchestrator:feedbackPanel ID="feedbackBadParameters" runat="server" Level="Error"
                Message="This operaion can not be performed on the job you have supplied.  In order to use this functionality the job must be a Groupage job and the action must be a delivery." />
            <div style="height: 22px; width: 100%; border-bottom: solid 1pt silver; color: #ffffff;
                background-color: #99BEDE; text-align: left;">
                <asp:Button ID="btnJobDetails" runat="server" Text="Return to Job Details" />
            </div>
        </asp:View>
        <asp:View ID="vwConfigureRedelivery" runat="server">
            <h2>
                Please specify the following information about the full refusal/redelivery request
                and click <b>Update Orders</b> to update the information in the grid. Then don't
                forget to click Save to permanently save that information.
            </h2>
            <orchestrator:feedbackPanel ID="feedbackInstructions" Visible="false" runat="server"
                Level="Info" Message="" />
            <uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" />
            <div style="font-size: small;">
                <asp:CustomValidator ID="cfvUpdateOrders" runat="server" ValidationGroup="UpdateOrders"
                    Display="Dynamic" Text="Orders with different currency types have been selected." />
            </div>
            <table cellpadding="5" cellspacing="5">
                <tr style="display: none;">
                    <td class="formCellLabel" width="25%" valign="top">
                        Arrival Date Time
                    </td>
                    <td class="formCellField" width="75%" colspan="4">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td>
                                    <telerik:RadDateInput ID="txtArrivalDateTime" runat="server" DateFormat="dd/MM/yy HH:mm"
                                        Width="90px" ToolTip="When the driver arrived at the delivery point." />
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="rfvArrivalDateTime" runat="server" ControlToValidate="txtArrivalDateTime"
                                        Display="Dynamic" ErrorMessage="Please enter the time when the driver arrived at the delivery point."
                                        ValidationGroup="UpdateOrders" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr style="display: none;">
                    <td class="formCellLabel" valign="top">
                        Departure Date Time
                    </td>
                    <td class="formCellField" colspan="4">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td>
                                    <telerik:RadDateInput ID="txtDepartureDateTime" runat="server" DateFormat="dd/MM/yy HH:mm"
                                        Width="90px" ToolTip="When the driver departed the delivery point." />
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="rfvDepartureDateTime" runat="server" ControlToValidate="txtDepartureDateTime"
                                        Display="Dynamic" ErrorMessage="Please enter the time when the driver departed the delivery point."
                                        ValidationGroup="UpdateOrders" />
                                    <asp:CustomValidator ID="cvArrivalDepartureDateTime" runat="server" ControlToValidate="txtDepartureDateTime"
                                        Display="Dynamic" ClientValidationFunction="ValidateDepartureAfterArrival" ErrorMessage="The departure time must be at or after the arrival time."
                                        ValidationGroup="UpdateOrders" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel" valign="top" style="width: 175px;">
                        Order Action
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:RadioButton runat="server" ID="rdOrderAction_Refused" GroupName="gpOrderAction"
                            Text="Refused" />
                        <asp:RadioButton runat="server" ID="rdOrderAction_Delivered" GroupName="gpOrderAction"
                            Text="Delivered" />
                    </td>
                </tr>
                <tr id="trTurnedAway_ResolutionMethod">
                    <td class="formCellLabel" valign="top">
                        Resolution Method
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:RadioButton runat="server" GroupName="gpResolutionMethod" ID="rdResolutionMethod_DontKnow"
                            Text="Dont Know" />
                        <asp:RadioButton runat="server" GroupName="gpResolutionMethod" ID="rdResolutionMethod_AttemptLater"
                            Text="Attempt Later" />
                        <asp:RadioButton runat="server" GroupName="gpResolutionMethod" ID="rdResolutionMethod_Redeliver"
                            Text="Redeliver" />
                    </td>
                </tr>
                <tr id="trTurnedAway_Reason">
                    <td class="formCellLabel" valign="top">
                        Reason for being Turned Away
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:DropDownList ID="cboRedeliveryReason" AppendDataBoundItems="true" DataValueField="RedeliveryReasonId" DataTextField="Description" runat="server" AutoPostBack="false" ToolTip="Please select the reason you need to mark this items for redelivery.">
                            <asp:ListItem Text="--Please Select a Reason--" Value="" />
                        </asp:DropDownList>

                        <asp:RequiredFieldValidator ID="rfvRedeliveryReason" runat="server" ControlToValidate="cboRedeliveryReason" Display="Dynamic" ErrorMessage="" ValidationGroup="UpdateOrders" ToolTip="Please select a redelivery reason.">
                                <img id="img3" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please select a redelivery reason." alt="" />
                            </asp:RequiredFieldValidator>

                        <br />
                        <div style="margin-top:5px;">
                            <asp:TextBox ID="txtCustomReason" runat="server" MaxLength="512" Width="300px" ToolTip="Please provide a custom reason." />
                            <asp:RequiredFieldValidator ID="rfvCustomReason" runat="server" ControlToValidate="txtCustomReason" Display="Dynamic" ErrorMessage="" ValidationGroup="UpdateOrders">
                                <img id="img1" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please specify a custom reason." alt="" />
                            </asp:RequiredFieldValidator>
                        </div>
                    </td>
                </tr>
                <tr id="trTurnedAway_Point">
                    <td class="formCellLabel" valign="top">
                        Points
                    </td>
                    <td class="formCellField" valign="top" style="width: 350px;">
                        <asp:CheckBox runat="server" ID="chkCrossDockGoods" Text="Are you storing goods? (Cross docking)" />
                        <br />
                        <asp:CustomValidator runat="server" ID="cvPoints" ValidationGroup="UpdateOrders" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="validatePoints" ErrorMessage="Please specify either a store point or a new delivery point." ToolTip="Please specify either a store point or a new delivery point.">
                            <img id="imgPointValidationWarningIcon" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please specify either a store point or a new delivery point." alt="Please specify either a store point or a new delivery point." />
                        </asp:CustomValidator>
                        <div id="divCreateOnwardRun">
                            <asp:CheckBox runat="server" ID="chkCreateOnwardRun" Text="Create onward run?" />
                        </div>
                        <div style="height: 15px;"></div>
                        <div id="divCrossDockPoint">
                            <orchestrator:point ID="ucCrossDockPoint" runat="server" CanCreateNewPoint="true" CanChangePoint="true" CanUpdatePoint="false" ValidationGroup="UpdateOrders" />
                        </div>
                    </td>
                    <td class="formCellField" valign="top">
                        <asp:CheckBox ID="chkDeliverGoodsElsewhere" runat="server" Text="Are you delivering goods elsewhere?" />
                        <div style="height: 35px;"></div>
                        <div id="divDeliveryPoint">
                            <orchestrator:point ID="ucNewDeliveryPoint" runat="server" CanCreateNewPoint="true" CanChangePoint="true" CanUpdatePoint="false" ValidationGroup="UpdateOrders" />
                        </div>
                    </td>
                </tr>
                <tr id="trTurnedAway_DeliveryTime">
                    <td class="formCellLabel" valign="top">
                        Deliver by:
                    </td>
                    <td class="formCellField" colspan="4">
                        <table cellpadding="0" cellspacing="0" border="0">
                            <tr>
                                <td colspan="4">
                                    <input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" />Timed
                                    Booking
                                    <input type="radio" name="delivery" runat="server" checked="true" id="rdDeliveryBookingWindow"
                                        onclick="deliveryBookingWindow(this);" />Booking Window
                                    <input type="radio" name="delivery" runat="server" id="rdDeliveryIsAnytime" onclick="deliveryIsAnytime(this);" />Anytime
                                </td>
                            </tr>
                            <tr runat="server" id="trDeliverFrom">
                                <td>
                                    Deliver from:
                                </td>
                                <td>
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryFromDate" runat="server" DateFormat="dd/MM/yy"
                                        DisplayDateFormat="dd/MM/yy">
                                    </telerik:RadDateInput>
                                </td>
                                <td>
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryFromTime" runat="server" DateFormat="HH:mm"
                                        DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00">
                                    </telerik:RadDateInput>
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="submit" runat="server"
                                        ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                        <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                            alt="warning icon" title="Please enter a delivery from date." />
                                    </asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="submit" runat="server"
                                        ControlToValidate="dteDeliveryFromTime" Display="Dynamic" ErrorMessage="Please enter a delivery from time.">
                                        <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                            alt="warning icon" title="Please enter a delivery from time." />
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 80px;">
                                    Deliver by:
                                </td>
                                <td>
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryByDate" runat="server" DateFormat="dd/MM/yy"
                                        DisplayDateFormat="dd/MM/yy" />
                                </td>
                                <td>
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryByTime" runat="server" DateFormat="HH:mm"
                                        DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00" />
                                </td>
                                <td>
                                    <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="submit" runat="server"
                                        ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                        <img id="Img10" runat="server" alt="warning icon" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                            title="Please enter a delivery by date." />
                                    </asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="submit" runat="server"
                                        ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                        <img id="Img9" runat="server" alt="warning icon" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                            title="Please enter a delivery by time." />
                                    </asp:RequiredFieldValidator>
                                    <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                        ValidationGroup="submit" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate"
                                        ErrorMessage="The date cannot be before today.">
                                        <img id="imgic1" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server"
                                            title="The date cannot be before today" alt="warning icon" />
                                    </asp:CustomValidator>
                                    <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                        Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2"
                                        ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?"
                                        ValidationGroup="submit">
                                        <img id="imgic2" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server"
                                            title="The date entered is far in the future." alt="warning icon" />
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr id="trTurnedAway_Charge">
                    <td class="formCellLabel" valign="top">
                        Are you charging for this?
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:CheckBox runat="server" ID="chkCharging" Text="Yes" />
                    </td>
                </tr>
                <tr id="trCharge_Amount">
                    <td class="formCellLabel" valign="top">
                        Amount
                    </td>
                    <td class="formCellField" colspan="4">
                        <telerik:RadNumericTextBox ID="txtExtraAmount" runat="server" MinValue="0" Type="Number">
                        </telerik:RadNumericTextBox>
                    </td>
                </tr>
                <tr id="trCharge_Type">
                    <td class="formCellLabel" valign="top">
                        Type of Extra
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:DropDownList ID="cboExtraType" runat="server" />
                    </td>
                </tr>
                <tr id="trCharge_State">
                    <td class="formCellLabel" valign="top">
                        State of Extra
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:DropDownList ID="cboExtraState" runat="server" AutoPostBack="false" />
                    </td>
                </tr>
                <tr id="trCharge_Client">
                    <td class="formCellLabel" valign="top">
                        Client Contact
                    </td>
                    <td class="formCellField" colspan="4">
                        <asp:TextBox ID="txtClientContact" runat="server" MaxLength="200" Width="200px" />
                        <asp:RequiredFieldValidator ID="rfvClientContact" runat="server" ControlToValidate="txtClientContact"
                            Display="Dynamic" ErrorMessage="Please provide the client contact for the extra."
                            ValidationGroup="UpdateOrders" />
                    </td>
                </tr>
                <tr id="trCharge_ExCustomReason">
                    <td class="formCellLabel" valign="top">
                        Reason
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtExtraCustomReason" runat="server" MaxLength="100" Width="400px" />
                    </td>
                </tr>
            </table>
            <div style="height: 5px;">
            </div>
            
            <div style="padding-top: 5px; padding-bottom: 5px;">
                    Choose the orders in the table below to apply the above settings to by ticking the
                    boxes and clicking "Update Orders":
                </div>
            <telerik:RadAjaxPanel ID="rapRedelivery" runat="server" EnableAJAX="true" ClientEvents-OnRequestStart="rapRedelivery_OnRequestStart" ClientEvents-OnResponseEnd="rapRedelivery_OnResponseEnd">
                <div class="buttonbar">
                    <asp:Button ID="btnUpdateSelectedOrders" runat="server" Text="Update Orders" Width="135px" ValidationGroup="UpdateOrders" ToolTip="Click to update the selected orders in the table below with the setting specified above." />&nbsp;
                </div>
                
                
                
                <telerik:RadGrid runat="server" ID="grdOrders"  AllowPaging="false" AllowSorting="false" Skin="Orchestrator" AutoGenerateColumns="false" AllowMultiRowSelection="true">
                    <MasterTableView Width="100%" DataKeyNames="OrderID,LCID">
                        <RowIndicatorColumn Display="false">
                        </RowIndicatorColumn>
                        <Columns>
                            <telerik:GridClientSelectColumn UniqueName="selectColumn">
                            </telerik:GridClientSelectColumn>
                            <telerik:GridTemplateColumn HeaderText="Order ID">
                                <ItemTemplate>
                                    <a runat="server" id="hypOrderId"><%# Eval("OrderID")%></a>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Client" DataField="Client">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Order No." DataField="CustomerOrderNumber">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Weight" DataField="Weight">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Pallets" DataField="Pallets">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Order Action" DataField="OrderActionLabel">
                            </telerik:GridBoundColumn>
                            <telerik:GridTemplateColumn HeaderText="Resolution">
                                <ItemTemplate>
                                    <%# Eval("ResolutionMethodLabel")%><asp:Label ID="lblReturnToLocationDescription"
                                        runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Deliver to">
                                <ItemTemplate>
                                    <asp:Label ID="lblDeliverToDescription" runat="server"></asp:Label>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Reason">
                                <ItemTemplate>
                                    <asp:Label ID="lblRedeliveryReason" runat="server"></asp:Label>&nbsp;
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn HeaderText="Delivery Date" DataField="RedeliveryByDateTime"
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                            <telerik:GridTemplateColumn HeaderText="Is AnyTime">
                                <ItemTemplate>
                                    <span>
                                        <%# ((bool)((System.Data.DataRowView)Container.DataItem)["RedeliveryIsAnyTime"]) ? "Yes" : "No" %></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Onward Run">
                                <ItemTemplate>
                                    <span>
                                        <%# ((bool)((System.Data.DataRowView)Container.DataItem)["CreateOnwardRun"]) ? "Yes" : "No" %></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn HeaderText="Raise Extra">
                                <ItemTemplate>
                                    <%# Eval("RaiseExtraLabel")%><asp:Label ID="lblExtraRate" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                        <NoRecordsTemplate>
                            <div>
                                There are no orders to display</div>
                        </NoRecordsTemplate>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                </telerik:RadGrid>
            
                <div class="buttonbar">
                    <asp:Button ID="btnSave" runat="server" Enabled="false" Text="Save" Width="95px" ToolTip="Use this button to apply the settings shown above." CausesValidation="true" />&nbsp;
                    <asp:Button ID="btnCancel" runat="Server" Text="Cancel" CausesValidation="false" Width="95px" ToolTip="Return to the Job Management page." />
                </div>
            
            </telerik:RadAjaxPanel>
        </asp:View>
    </asp:MultiView>
    
    
    <div class="masterpagelite_toolTip" id="toolTip" style="display: none;">
        <div id="toolTipInner">
        </div>
    </div>

    <script language="javascript" type="text/javascript">
        var _skipCollectionDateCheck = false;
        var _skipDeliveryDateCheck = false;
        window.focus();

        //$(document).ready(function() {
        window.onload = function() {
            var rdDeliveryTimedBooking = $('input:radio[id*=rdDeliveryTimedBooking]');
            var rdDeliveryIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]');
            var rdDeliveryBookingWindow = $('input:radio[id*=rdDeliveryBookingWindow]');
            var deliverFrom = $('tr[id*=trDeliverFrom]');

            if (rdDeliveryTimedBooking[0] != null && rdDeliveryIsAnytime[0] != null && rdDeliveryBookingWindow[0] != null)
                switch (true) {
                case rdDeliveryTimedBooking[0].checked:
                    deliverFrom.hide();
                    break;

                case rdDeliveryIsAnytime[0].checked:
                    $('input[id*=dteDeliveryFromTime]').prop("disabled", true);
                    $('input[id*=dteDeliveryByTime]').prop("disabled", true);
                    deliverFrom.hide();
                    break;

                case rdDeliveryBookingWindow[0].checked:
                    deliverFrom.show();
                    break;
            }

        };

        function validatePoints(src, args) {

            args.IsValid = true;
            var rdResolutionMethod_Redeliver = $('input:radio[id*=rdResolutionMethod_Redeliver]')[0];

            if (rdResolutionMethod_Redeliver.checked == true) {

                var chkCrossDockGoods = $('input:checkbox[id*=chkCrossDockGoods]')[0];
                var chkDeliverGoodsElsewhere = $('input:checkbox[id*=chkDeliverGoodsElsewhere]')[0];
                args.IsValid = !(chkCrossDockGoods.checked == false && chkDeliverGoodsElsewhere.checked == false);
                if (!args.IsValid) {
                    var imgPointValidationWarningIcon = $('img[id*=imgPointValidationWarningIcon]')[0];
                    $(imgPointValidationWarningIcon).show();
                }
            }
        }

        function initControls() {

            var trTurnedAway_Reason = $('tr[id*=trTurnedAway_Reason]')[0];
            var trTurnedAway_ResolutionMethod = $('tr[id*=trTurnedAway_ResolutionMethod]')[0];
            var trTurnedAway_Point = $('tr[id*=trTurnedAway_Point]')[0];
            var trTurnedAway_DeliveryTime = $('tr[id*=trTurnedAway_DeliveryTime]')[0];
            var trTurnedAway_Charge = $('tr[id*=trTurnedAway_Charge]')[0];
            var cboRedeliveryReason = $get("<%=cboRedeliveryReason.ClientID %>");

            var rfvCustomReason = $get('<%=rfvCustomReason.ClientID %>');
            var rfvClientContact = $get('<%=rfvClientContact.ClientID %>');
            var cvPoints = $get('<%=cvPoints.ClientID %>');
            var imgPointValidationWarningIcon = $('img[id*=imgPointValidationWarningIcon]')[0];

            // TODO: Get rid of hard coded reference
            var ucCrossDockPoint_rfvPoint = document.all ? document.all["ctl00_ContentPlaceHolder1_ucCrossDockPoint_rfvPoint"] : document.getElementById("ctl00_ContentPlaceHolder1_ucCrossDockPoint_rfvPoint");
            var ucNewDeliveryPoint_rfvPoint = document.all ? document.all["ctl00_ContentPlaceHolder1_ucNewDeliveryPoint_rfvPoint"] : document.getElementById("ctl00_ContentPlaceHolder1_ucNewDeliveryPoint_rfvPoint");

            //var trCharge_Amount = $('tr[id*=trCharge_Amount]')[0];
            //var trCharge_Type = $('tr[id*=trCharge_Type]')[0];
            //var trCharge_State = $('tr[id*=trCharge_State]')[0];
            //var trCharge_ExCustomReason = $('tr[id*=trCharge_ExCustomReason]')[0];
            var trCharge_Client = $('tr[id*=trCharge_Client]')[0];
            var cboExtraState = $get("<%=cboExtraState.ClientID %>");
            var chkCharging = $('input:checkbox[id*=chkCharging]')[0];

            var rdResolutionMethod_Redeliver = $('input:radio[id*=rdResolutionMethod_Redeliver]')[0];
            var rdResolutionMethod_AttemptLater = $('input:radio[id*=rdResolutionMethod_AttemptLater]')[0];
            var rdResolutionMethod_DontKnow = $('input:radio[id*=rdResolutionMethod_DontKnow]')[0];

            var rdOrderAction_Refused = $('input:radio[id*=rdOrderAction_Refused]')[0];
            var rdOrderAction_Delivered = $('input:radio[id*=rdOrderAction_Delivered]')[0];

            var chkCrossDockGoods = $('input:checkbox[id*=chkCrossDockGoods]')[0];
            var divCreateOnwardRun = $('div[id*=divCreateOnwardRun]')[0];
            var chkCreateOnwardRun = $('input:checkbox[id*=chkCreateOnwardRun]')[0];
            var chkDeliverGoodsElsewhere = $('input:checkbox[id*=chkDeliverGoodsElsewhere]')[0];

            var divCrossDockPoint = $('div[id*=divCrossDockPoint]')[0];
            var divDeliveryPoint = $('div[id*=divDeliveryPoint]')[0];
            
            var hid = $get("<%=hidPartialPostback.ClientID %>");
            if (hid.value == "0") {

                // Initial page state
                rdOrderAction_Refused.checked = true;
                rdResolutionMethod_AttemptLater.checked = true;
                $('tr[id*=trTurnedAway_]').hide();
                $('tr[id*=trCharge_]').hide();
                $(trTurnedAway_Reason).show();
                $(trTurnedAway_DeliveryTime).show();
                $(trTurnedAway_ResolutionMethod).show();
                $(trTurnedAway_Charge).show();
                $(trCharge_Client).hide();

                // Turn off validation for hidden fields
                ValidatorEnable(rfvCustomReason, false);
                ValidatorEnable(rfvClientContact, false);
                ValidatorEnable(cvPoints, false);
                ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

            } else {

                // Postback page state
                var rdDeliveryTimedBooking = $('input:radio[id*=rdDeliveryTimedBooking]')[0];
                var rdDeliveryIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]')[0];
                var rdDeliveryBookingWindow = $('input:radio[id*=rdDeliveryBookingWindow]')[0];

                if (rdDeliveryTimedBooking.checked == true)
                    deliveryTimedBooking(rdDeliveryTimedBooking);

                if (rdDeliveryBookingWindow.checked == true)
                    deliveryBookingWindow(rdDeliveryBookingWindow);

                if (rdResolutionMethod_Redeliver.checked == true)
                    ValidatorEnable(cvPoints, true);

                if (rdDeliveryIsAnytime.checked == true)
                    deliveryIsAnytime(rdDeliveryIsAnytime);

                if (chkCrossDockGoods.checked == true) {
                    $(divCreateOnwardRun).show();
                    $(divCrossDockPoint).show();
                    ValidatorEnable(ucCrossDockPoint_rfvPoint, true);
                } else {
                    $(divCreateOnwardRun).hide();
                    $(divCrossDockPoint).hide();
                    ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                }

                if (chkDeliverGoodsElsewhere.checked == true) {
                    $(divDeliveryPoint).show();
                    ValidatorEnable(ucNewDeliveryPoint_rfvPoint, true);
                } else {
                    $(divDeliveryPoint).hide();
                    ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
                }

                if (chkCharging.checked == true) {

                    $('tr[id*=trCharge_]').show();
                    if (cboExtraState.value != 'Awaiting Response') {
                        ValidatorEnable(rfvClientContact, true);
                        $(trCharge_Client).show();
                    } else {
                        ValidatorEnable(rfvClientContact, false);
                        $(trCharge_Client).hide();
                    }
                } else {
                    $('tr[id*=trCharge_]').hide();
                }

                if (rdOrderAction_Delivered.checked) {

                    $('tr[id*=trTurnedAway_]').hide();
                    $('tr[id*=trCharge_]').hide();
                    rdResolutionMethod_AttemptLater.checked = true;
                    rdResolutionMethod_DontKnow.checked = false;
                    rdResolutionMethod_Redeliver.checked = false;

                } else {

                    if (rdResolutionMethod_AttemptLater.checked) {

                        $('tr[id*=trTurnedAway_]').hide();
                        $(trTurnedAway_ResolutionMethod).show();
                        $(trTurnedAway_Charge).show();
                        $(trTurnedAway_Reason).show();
                        $(trTurnedAway_DeliveryTime).show();
                        ValidatorEnable(cvPoints, false);
                        ValidatorEnable(rfvClientContact, false);
                        ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

                    }

                    if (rdResolutionMethod_DontKnow.checked) {

                        $('tr[id*=trTurnedAway_]').hide();
                        $('tr[id*=trCharge_]').hide();
                        $(trTurnedAway_ResolutionMethod).show();
                        $(trTurnedAway_Reason).show();
                        chkCharging.checked = false;
                        ValidatorEnable(cvPoints, false);
                        ValidatorEnable(rfvClientContact, false);
                        ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

                    }

                    if (rdResolutionMethod_Redeliver.checked) {

                        $(trTurnedAway_DeliveryTime).show();

                    }
                }
            }
            //

            //-----------------
            // Hook up events
            //-----------------

            // Orcder Actions
            $(rdOrderAction_Refused).click(

                function() {

                    $('tr[id*=trTurnedAway_]').hide();
                    $('tr[id*=trCharge_]').hide();

                    $(trTurnedAway_ResolutionMethod).show();
                    $(trTurnedAway_Charge).show();
                    $(trTurnedAway_Reason).show();
                    $(trTurnedAway_DeliveryTime).show();
                    chkCharging.checked = false;
                }

            );

            $(rdOrderAction_Delivered).click(

                function() {

                    $('tr[id*=trTurnedAway_]').hide();
                    $('tr[id*=trCharge_]').hide();
                    rdResolutionMethod_AttemptLater.checked = true;
                    rdResolutionMethod_DontKnow.checked = false;
                    rdResolutionMethod_Redeliver.checked = false;
                }
            );

            // Resolution methods
            $(rdResolutionMethod_DontKnow).click(

                            function() {

                                $('tr[id*=trTurnedAway_]').hide();
                                $('tr[id*=trCharge_]').hide();
                                $(trTurnedAway_ResolutionMethod).show();
                                $(trTurnedAway_Reason).show();
                                chkCharging.checked = false;
                                ValidatorEnable(cvPoints, false);
                                ValidatorEnable(rfvClientContact, false);
                                ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                                ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);

                            }

                        );

            $(rdResolutionMethod_AttemptLater).click(

                function() {
                    $('tr[id*=trTurnedAway_]').hide();
                    $(trTurnedAway_ResolutionMethod).show();
                    $(trTurnedAway_Charge).show();
                    $(trTurnedAway_Reason).show();
                    $(trTurnedAway_DeliveryTime).show();
                    ValidatorEnable(cvPoints, false);
                    ValidatorEnable(rfvClientContact, false);
                    ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                    ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
                }
            );

            $(rdResolutionMethod_Redeliver).click(

                function() {
                    $(trTurnedAway_Point).show();
                    chkCreateOnwardRun.checked = false;
                    chkCrossDockGoods.checked = false;
                    chkDeliverGoodsElsewhere.checked = false;
                    $(divCrossDockPoint).hide();
                    $(divDeliveryPoint).hide();
                    $(divCreateOnwardRun).hide();
                    $(trTurnedAway_Reason).show();
                    $(trTurnedAway_DeliveryTime).show();
                    ValidatorEnable(cvPoints, true);
                }
            );

            // Store/delivery points
            $(chkDeliverGoodsElsewhere).click(

                                function() {

                                    if ($(this).prop('checked') == true) {
                                        $(imgPointValidationWarningIcon).hide();
                                        $(divDeliveryPoint).show();
                                        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, true);
                                    } else {
                                        $(divDeliveryPoint).hide();
                                        ValidatorEnable(ucNewDeliveryPoint_rfvPoint, false);
                                    }
                                }

                            );

                                $(chkCrossDockGoods).click(

                                function () {
                                    
                                    if ($(this).prop('checked') == true) {
                                        $(imgPointValidationWarningIcon).hide();
                                        $(divCreateOnwardRun).show();
                                        $(divCrossDockPoint).show();
                                        ValidatorEnable(ucCrossDockPoint_rfvPoint, true);
                                    } else {
                                        chkCreateOnwardRun.checked = false;
                                        $(divCreateOnwardRun).hide();
                                        $(divCrossDockPoint).hide();
                                        ValidatorEnable(ucCrossDockPoint_rfvPoint, false);
                                    }
                                }

                            );

            $(chkCharging).click(

                                function() {

                                    if ($(this).prop('checked') == true) {
                                        $('tr[id*=trCharge_]').show();
                                        if (cboExtraState.value != 'Awaiting Response') {
                                            ValidatorEnable(rfvClientContact, true);
                                            $(trCharge_Client).show();
                                        } else {
                                            ValidatorEnable(rfvClientContact, false);
                                            $(trCharge_Client).hide();
                                        }
                                    } else {
                                        $('tr[id*=trCharge_]').hide();
                                    }
                                }

                            );


            $(cboExtraState).change(

                                function() {

                                    if (cboExtraState.value != 'Awaiting Response') {
                                        ValidatorEnable(rfvClientContact, true);
                                        $(trCharge_Client).show();
                                    } else {
                                        ValidatorEnable(rfvClientContact, false);
                                        $(trCharge_Client).hide();
                                    }
                                }

            );

        }

        function ValidateDepartureAfterArrival(sender, args) {
            var arrivalDate = $find('<%=txtArrivalDateTime.ClientID %>');
            var departureDate = $find('<%=txtDepartureDateTime.ClientID %>');

            args.IsValid = arrivalDate.get_displayValue() <= departureDate.get_displayValue();
        }

        function ValidateDeliveryAfterDeparture(sender, args) {
            var itemDepartureDateTime = $find('<%=txtDepartureDateTime.ClientID %>');
            var itemDeliveryDate = $find("<%=dteDeliveryByDate.ClientID%>");
            var itemDeliveryTime = $find("<%=dteDeliveryByTime.ClientID%>");

            if (itemDepartureDateTime != null && itemDeliveryDate != null && itemDeliveryTime != null) {

                var enteredDateParts = itemDeliveryDate.value.split("/");

                var departureDateTime = itemDepartureDateTime.get_displayValue();
                var deliveryDate = new Date();

                deliveryDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

                if (itemDeliveryTime.value.length > 0) {
                    var enteredTimeParts = itemDeliveryTime.value.split(":");
                    deliveryDate.setHours(enteredTimeParts[0], enteredTimeParts[1], 0, 0);
                }
                else {
                    deliveryDate.setHours(23, 59, 0, 0);
                }

                if (deliveryDate > departureDateTime)
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
        }

        function deliveryTimedBooking(rb) {
            $('tr[id*=trDeliverFrom]').hide();

            var dteDeliveryByDate = $('input[id*=dteDeliveryByDate]');
            dteDeliveryByDate.focus();
            dteDeliveryByDate.select();

            var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

            $('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

            if (method == 'window') {
                $('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_value());
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").set_value($find("<%=dteDeliveryByTime.ClientID %>").get_value());

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value("00:00");
                }

                if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value("00:00");
                }
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").enable();
            $find("<%=dteDeliveryByTime.ClientID %>").enable();
        }

        function deliveryBookingWindow(rb) {
            $('tr[id*=trDeliverFrom]').show();
            var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
            dteDeliveryFromDate.focus();
            dteDeliveryFromDate.select();

            var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

            $('input:hidden[id*=hidDeliveryTimingMethod]').val('window');

            if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val()) {
                $find("<%=dteDeliveryFromTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
            }

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value("00:00");
                }

                if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value("00:00");
                }
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").enable();
            $find("<%=dteDeliveryByTime.ClientID %>").enable();
        }

        function deliveryIsAnytime(rd) {
            $('tr[id*=trDeliverFrom]').hide();

            var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
            dteDeliveryFromDate.focus();
            dteDeliveryFromDate.select();

            //                    $('input:hidden[id*=hidDeliveryTimingMethod]').val('anytime');

            //                    $('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_value());
            //                    $('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val($find("<%=dteDeliveryByTime.ClientID %>").get_value());

            $find("<%=dteDeliveryFromTime.ClientID %>").set_value('00:00');
            $find("<%=dteDeliveryFromTime.ClientID %>").disable();

            $find("<%=dteDeliveryByTime.ClientID %>").set_value('23:59');
            $find("<%=dteDeliveryByTime.ClientID %>").disable();
        }

        function CV_ClientValidateDeliveryDate(source, args) {
            var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
            var today = new Date();
            var day_date = today.getDate();
            var month_date = today.getMonth();
            var year_date = today.getFullYear();

            today.setFullYear(year_date, month_date, day_date);

            var enteredDateParts = dteDateTime.get_displayValue().split("/");
            var enteredDate = new Date();

            enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

            if (enteredDate >= today) {
                args.IsValid = true;
            }
            else {
                if (!_skipCollectionDateCheck) {
                    _skipCollectionDateCheck = true;
                    args.IsValid = false;
                    alert("The date entered is in the past - Are you sure?");
                }
            }
        }

        function CV_ClientValidateDeliveryDate2(source, args) {
            var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
            var warningDate = new Date();
            var day_date = warningDate.getDate();
            var month_date = warningDate.getMonth();
            var year_date = warningDate.getFullYear();

            warningDate.setFullYear(year_date, month_date, day_date);

            var enteredDateParts = dteDateTime.get_displayValue().split("/");
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

        var rdDeliveryTimedBooking = $('input:radio[id*=rdDeliveryTimedBooking]');
        var rdDeliveryIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]');
        var rdDeliveryBookingWindow = $('input:radio[id*=rdDeliveryBookingWindow]');

        if (rdDeliveryTimedBooking[0] != null && rdDeliveryTimedBooking[0].checked == true)
            $('tr[id*=trDeliverFrom]').hide();

        if (rdDeliveryIsAnytime[0] != null && rdDeliveryIsAnytime[0].checked == true)
            $('tr[id*=trDeliverFrom]').hide();

        if (rdDeliveryBookingWindow[0] != null && rdDeliveryBookingWindow[0].checked == true)
            $('tr[id*=trDeliverFrom]').show();

        function ViewOrder(orderID) {
            var url = "/Groupage/ManageOrder.aspx";
            url += "?oID=" + orderID;

            var wnd = window.radopen("about:blank", "largeWindow");
            wnd.SetUrl(url);
            wnd.SetTitle("Add/Update Order");
        }

        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var _panels, _count;
        prm.add_pageLoaded(pageLoaded);

        function pageLoaded(sender, args) {
            if (_panels != undefined && _panels.length > 0) {
                for (i = 0; i < _panels.length; i++)
                    _panels[i].dispose();
            }

            var panels = args.get_panelsUpdated();

            if (panels.length > 0) {
                if (rdDeliveryTimedBooking[0] != null && rdDeliveryTimedBooking[0].checked == true)
                    $('tr[id*=trDeliverFrom]').hide();

                if (rdDeliveryIsAnytime[0] != null && rdDeliveryIsAnytime[0].checked == true)
                    $('tr[id*=trDeliverFrom]').hide();

                if (rdDeliveryBookingWindow[0] != null && rdDeliveryBookingWindow[0].checked == true)
                    $('tr[id*=trDeliverFrom]').show();
            }

            //
            // Setting the hidden field value to "1" indicates to the initControls method
            // that the call isnt the first.
            var hid = $get("<%=hidPartialPostback.ClientID %>");
            initControls();
            hid.value = '1';
        }

        function rapRedelivery_OnRequestStart(sender, eventArgs) {
            showLoading("Updating...");
        }

        function rapRedelivery_OnResponseEnd(sender, eventArgs) {
            hideLoading();
        }

        function showLoading(messageContent) {
            $.blockUI({
                message: '<div style="margin-left:30px;"><span id="UpdatableMessage">' + messageContent + '</span></div>',
                css: {
                    border: 'none',
                    padding: '15px',
                    backgroundColor: '#000',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: '.5',
                    color: '#fff'
                }
            });
        }

        function hideLoading() {
            $.unblockUI();
        }

    </script>

    <input type="hidden" runat="server" value="0" id="hidPartialPostback" />
</asp:Content>