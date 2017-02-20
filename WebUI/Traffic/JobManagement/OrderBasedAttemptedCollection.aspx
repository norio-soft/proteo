<%@ Page Title="Haulier Enterprise" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true" CodeBehind="OrderBasedAttemptedCollection.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.JobManagement.OrderBasedAttemptedCollection" %>

<%@ Register TagPrefix="orchestrator" TagName="feedbackPanel" Src="~/UserControls/feedbackPanel.ascx" %>
<%@ Register TagPrefix="orchestrator" TagName="point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

    <script type="text/javascript" language="javascript">
        var _panels, _count;
        var _skipCollectionDateCheck = false;
        var _skipDeliveryDateCheck = false;
        window.focus();

        var rdDeliveryTimedBooking = null, rdDeliveryIsAnytime = null, rdDeliveryBookingWindow = null;

        var dteCollectionFromDate = dteCollectionByDate = null;
        var dteDeliveryFromDate = dteDeliveryByDate = null;
        
        var dteCollectionFromTime = dteCollectionByTime = null;
        var dteDeliveryFromTime = dteDeliveryByTime = null;

        var chkDeliverGoodsElsewhere = chkCollectGoodsElsewhere = null;
        
        var trAttemptedCollection_Reference = trAttemptedCollection_ClientContact = null;
        var trAttemptedCollection_ResolutionMethod = trAttemptedCollection_Point = trAttemptedCollection_DeliveryTime = trAttemptedCollection_ChangeDeliveryDate = trAttemptedCollection_Charge = trAttemptedCollection_Reason = null;

        var imgPointValidationWarningIcon = null;

        // TODO: Get rid of hard coded reference
        var ucNewDeliveryPoint_rfvPoint = ucNewCollectionPoint_rfvPoint = null;
        var trCharge_Amount = trCharge_Type = trCharge_State = trCharge_Client = null;
        var chkChangeDeliveryDate = chkCharging = null;

        var rdResolutionMethod_AttemptLater = null;
        var rdResolutionMethod_Cancel = null;

        var rdOrderAction_NotCollected = null;
        var rdOrderAction_Collected = null;

        var divCreateOnwardRun = chkCreateOnwardRun = chkDeliverGoodsElsewhere = null;
        var divCrossDockPoint = divDeliveryPoint = divCollectionPoint = divCustomReason = null;
        var cboRedeliveryReason = rfvCustomReason = rfvClientContact = cboExtraState = null;

        function pageLoad(sender, args) {
            rdDeliveryTimedBooking = $('input:radio[id*=rdDeliveryTimedBooking]');
            rdDeliveryIsAnytime = $('input:radio[id*=rdDeliveryIsAnytime]');
            rdDeliveryBookingWindow = $('input:radio[id*=rdDeliveryBookingWindow]');

            trAttemptedCollection_Reason = $('tr[id*=trAttemptedCollection_Reason]')
            trAttemptedCollection_Reference = $('tr[id*=trAttemptedCollection_Reference]');
            trAttemptedCollection_ClientContact = $('tr[id*=trAttemptedCollection_ClientContact]');
            trAttemptedCollection_ResolutionMethod = $('tr[id*=trAttemptedCollection_ResolutionMethod]');
            trAttemptedCollection_Point = $('tr[id*=trAttemptedCollection_Point]');
            trAttemptedCollection_DeliveryTime = $('tr[id*=trAttemptedCollection_DeliveryTime]');
            trAttemptedCollection_ChangeDeliveryDate = $('tr[id*=trAttemptedCollection_ChangeDeliveryDate]');
            trAttemptedCollection_Charge = $('tr[id*=trAttemptedCollection_Charge]');

            imgPointValidationWarningIcon = $('img[id*=imgPointValidationWarningIcon]');

            // TODO: Get rid of hard coded reference
            ucNewDeliveryPoint_rfvPoint = document.all ? document.all["ctl00_ContentPlaceHolder1_ucNewDeliveryPoint_rfvPoint"] : document.getElementById("ctl00_ContentPlaceHolder1_ucNewDeliveryPoint_rfvPoint");
            ucNewCollectionPoint_rfvPoint = document.all ? document.all["ctl00_ContentPlaceHolder1_ucNewCollectionPoint_rfvPoint"] : document.getElementById("ctl00_ContentPlaceHolder1_ucNewCollectionPoint_rfvPoint");

            trCharge_Amount = $('tr[id*=trCharge_Amount]');
            trCharge_Type = $('tr[id*=trCharge_Type]');
            trCharge_State = $('tr[id*=trCharge_State]');
            trCharge_Client = $('tr[id*=trCharge_Client]');

            chkCharging = $('input:checkbox[id*=chkCharging]');
            chkChangeDeliveryDate = $('input:checkbox[id*=chkChangeDeliveryDate]');

            chkDeliverGoodsElsewhere = $('input:checkbox[id*=chkDeliverGoodsElsewhere]');
            chkCollectGoodsElsewhere = $('input:checkbox[id*=chkCollectGoodsElsewhere]');

            rdResolutionMethod_AttemptLater = $('input:radio[id*=rdResolutionMethod_AttemptLater]');
            rdResolutionMethod_Cancel = $('input:radio[id*=rdResolutionMethod_Cancel]');

            rdOrderAction_NotCollected = $('input:radio[id*=rdOrderAction_NotCollected]');
            rdOrderAction_Collected = $('input:radio[id*=rdOrderAction_Collected]');

            divCreateOnwardRun = $('div[id*=divCreateOnwardRun]');
            chkCreateOnwardRun = $('input:checkbox[id*=chkCreateOnwardRun]');
            chkDeliverGoodsElsewhere = $('input:checkbox[id*=chkDeliverGoodsElsewhere]');

            divCrossDockPoint = $('div[id*=divCrossDockPoint]');
            divDeliveryPoint = $('div[id*=divDeliveryPoint]');
            divCollectionPoint = $('div[id*=divCollectionPoint]');
            divCustomReason = $('div[id*=divCustomReason]');
        
            cboRedeliveryReason = $get("<%=cboRedeliveryReason.ClientID %>");
            rfvCustomReason = $get('<%=rfvCustomReason.ClientID %>');
            rfvClientContact = $get('<%=rfvClientContact.ClientID %>');
            cboExtraState = $get("<%=cboExtraState.ClientID %>");

            dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
            dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID %>");

            dteCollectionFromTime = $find("<%=dteCollectionFromTime.ClientID %>");
            dteCollectionByTime = $find("<%=dteCollectionByTime.ClientID %>");

            dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
            dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");

            dteDeliveryFromTime = $find("<%=dteDeliveryFromTime.ClientID %>");
            dteDeliveryByTime = $find("<%=dteDeliveryByTime.ClientID %>");

            // Setting the hidden field value to "1" indicates to the initControls method
            // that the call isnt the first.
            var hid = $get("<%=hidPartialPostback.ClientID %>");
            initControls(hid);
            hid.value = '1';
        }
    </script>

    <script type="text/javascript" language="javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/toolTipPopUps.js"></script>
    <script type="text/javascript" language="javascript" src="OrderBasedAttemptedCollection.aspx.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Attempted Collection</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog runat="server" ID="dlgOrder" ReturnValueExpected="true" AutoPostBack="true" URL="/groupage/manageorder.aspx" Height="900" Width="1200" Mode="Modal" />
    <cc1:Dialog runat="server" ID="dlgRun" ReturnValueExpected="false" AutoPostBack="false" URL="/Job/Job.aspx" Height="900" Width="1200" Mode="Normal" UseCookieSessionID="true"/>

    <h1>Attempted Collection</h1>

    <h2>
        Please specify the following information about the full refusal/redelivery request
        and click <b>Update Orders</b> to update the information in the grid. Then don't
        forget to click Save to permanently save that information.
    </h2>
    
    <orchestrator:feedbackPanel ID="feedbackInstructions" Visible="false" runat="server" Level="Info" Message="" />
    
    <uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" />
    
    <div style="font-size: small;">
        <asp:CustomValidator ID="cfvUpdateOrders" runat="server" ValidationGroup="UpdateOrders" Display="Dynamic" Text="Orders with different currency types have been selected." />
    </div>
    
    <div id="divNoDetails" runat="server" style="display:none;">
        <orchestrator:feedbackPanel ID="feedbackBadParameters" runat="server" Level="Error" Message="This operaion can not be performed on the job you have supplied.  Please cancel and try again." />
        <div class="buttonbar">
            <asp:Button ID="btnJobDetails" runat="server" Text="Return to Job Details" />
        </div>
    </div>
    <div id="divAttemptedCollection" runat="server">
        <table cellpadding="5" cellspacing="5">
        <tr style="display:none;">
            <td class="formCellLabel" valign="top" style="width: 175px;">
                Order Action
            </td>
            <td class="formCellField">
                <asp:RadioButton runat="server" ID="rdOrderAction_NotCollected" GroupName="gpOrderAction" Text="Not Collected" onclick="javascript:rdOrderAction_NotCollected_onClick(this);" />
                <asp:RadioButton runat="server" ID="rdOrderAction_Collected" GroupName="gpOrderAction" Text="Collected" onclick="javascript:rdOrderAction_Collected_onClick(this);" />
            </td>
        </tr>
        <tr id="trCollection_ResolutionMethod">
            <td class="formCellLabel" valign="top">
                Resolution Method
            </td>
            <td class="formCellField">
                <asp:RadioButton runat="server" GroupName="gpResolutionMethod" ID="rdResolutionMethod_Cancel" Text="Cancelled" onclick="javascript:rdResolutionMethod_Cancel_onClick(this);" />
                <asp:RadioButton runat="server" GroupName="gpResolutionMethod" ID="rdResolutionMethod_AttemptLater" Text="Attempt Later" onclick="javascript:rdResolutionMethod_AttemptLater_onClick(this);" />
            </td>
        </tr>
        <tr id="trCollection_Reason">
            <td class="formCellLabel" valign="top">
                Reason for being Turned Away
            </td>
            <td class="formCellField">
                <asp:DropDownList ID="cboRedeliveryReason" DataValueField="RedeliveryReasonId" DataTextField="Description" runat="server" AutoPostBack="false" ToolTip="Please select the reason you need to mark this items for redelivery." onchange="javascript:cboRedeliveryReason_onChange(this);" />
                <div id="divCustomReason">
                    <br />
                    <asp:TextBox ID="txtCustomReason" runat="server" MaxLength="512" Width="300px" ToolTip="Please provide a custom reason." />
                    <asp:RequiredFieldValidator ID="rfvCustomReason" runat="server" ControlToValidate="txtCustomReason" Display="Dynamic" ErrorMessage="" ValidationGroup="UpdateOrders">
                        <img id="img1" src="/App_Themes/Orchestrator/img/MasterPage/icon-warning.png" runat="server" title="Please specify a custom reason." alt="" />
                    </asp:RequiredFieldValidator>
                </div>
            </td>
        </tr>
        <tr id="trCollection_Reference">
            <td class="formCellLabel" valign="top">
                Attempted Collection Reference
            </td>
            <td class="formCellField">
                <asp:TextBox ID="txtAttemptedCollectionReference" runat="server" MaxLength="250" Width="300px" ToolTip="Enter a reference value here." />
            </td>
        </tr>
        <tr id="trCollection_ClientContact">
            <td class="formCellLabel" valign="top">
                Client Contact
            </td>
            <td class="formCellField">
                <asp:TextBox ID="txtAttemptedClientContact" runat="server" MaxLength="45" Width="300px" ToolTip="Capture who issued the reference here." />
            </td>
        </tr>
        <tr id="trAttemptedCollection_CollectionDateTime">
            <td class="formCellLabel" valign="top">
                Collect When
            </td>
            <td class="formCellField" valign="top">
                <table>
                    <tr>
                        <td colspan="4">
                            <input type="radio" name="collection" runat="server" id="rdCollectionTimedBooking" checked="true" onclick="collectionTimedBooking(this);" />Timed Booking
                            <input type="radio" name="collection" runat="server" id="rdCollectionBookingWindow" onclick="collectionBookingWindow(this);" />Booking Window
                            <input type="radio" name="collection" runat="server" id="rdCollectionIsAnytime" onclick="collectionIsAnytime(this);" />Anytime
                        </td>
                    </tr>
                
                    <tr>
                        <td class="formCellLabel">
                            Collect from:
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteCollectionFromDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionFromDate_SelectedDateChanged" />
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteCollectionFromTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" />
                        </td>
                        <td class="formCellInput">
                            <asp:RequiredFieldValidator ID="rfvCollectionFromDate" ValidationGroup="collection" runat="server" ControlToValidate="dteCollectionFromDate" Display="Dynamic" ErrorMessage="Please enter a collection from date.">
                                <img id="Img3" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection from date." />
                            </asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvCollectionFromTime" ValidationGroup="collection" runat="server" ControlToValidate="dteCollectionFromTime" Display="Dynamic" ErrorMessage="Please enter a collection from time.">
                                <img id="Img14" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection from time." />
                            </asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="dteCollectionFromDate" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateCollectionDate" ErrorMessage="The date cannot be before today." ValidationGroup="submit">
                                <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon" />
                            </asp:CustomValidator>
                        </td>
                    </tr>
                    <tr runat="server" id="trCollectBy">
                        <td class="formCellLabel">
                            Collect by:
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteCollectionByDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" />
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteCollectionByTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" />
                        </td>
                        <td class="formCellInput">
                            <asp:RequiredFieldValidator ID="rfvCollectionByDate" ValidationGroup="collection" runat="server" ControlToValidate="dteCollectionByDate" Display="Dynamic" ErrorMessage="Please enter a collection by date.">
                                <img id="Img12" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection by date." />
                            </asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvCollectionByTime" ValidationGroup="collection" runat="server" ControlToValidate="dteCollectionByTime" Display="Dynamic" ErrorMessage="Please enter a collection by time.">
                                <img id="Img13" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection by time." />
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>                        
            </td>
        </tr>
        <tr id="trAttemptedCollection_ChangeDeliveryDate">
            <td class="formCellLabel" valign="top">
                Is the Delivery Date Changing?
            </td>
            <td class="formCellField" valign="middle">
                <div>
                    <div style="float:left;"><asp:CheckBox ID="chkChangeDeliveryDate" runat="server" AutoPostBack="false" Text="Yes" onclick="javascript:chkChangeDeliveryDate_onClick(this);" /></div>
                    <div style="float:left; padding-left:10px; padding-top:5px;"><b>NB</b>: This will not affect the cross-dock date.</div>
                    <div class="clearDiv"></div>
                </div>
            </td>
        </tr>
        <tr id="trAttemptedCollection_DeliveryTime">
            <td class="formCellLabel" valign="top">
                Deliver by:
            </td>
            <td class="formCellField">
                <table cellpadding="0" cellspacing="0" border="0">
                    <tr>
                        <td colspan="4">
                            <input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" />Timed Booking
                            <input type="radio" name="delivery" runat="server" checked="true" id="rdDeliveryBookingWindow" onclick="deliveryBookingWindow(this);" />Booking Window
                            <input type="radio" name="delivery" runat="server" id="rdDeliveryIsAnytime" onclick="deliveryIsAnytime(this);" />Anytime
                        </td>
                    </tr>
                    <tr runat="server" id="trDeliverFrom">
                        <td class="formCellLabel">
                            Deliver from:
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteDeliveryFromDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy"></telerik:RadDateInput>
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteDeliveryFromTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00"></telerik:RadDateInput>
                        </td>
                        <td class="formCellInput">
                            <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" alt="warning icon" title="Please enter a delivery from date." />
                            </asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryFromTime" Display="Dynamic" ErrorMessage="Please enter a delivery from time.">
                                <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" alt="warning icon" title="Please enter a delivery from time." />
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">
                            Deliver by:
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteDeliveryByDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteDeliveryByDate_SelectedDateChanged" />
                        </td>
                        <td class="formCellInput">
                            <telerik:RadDateInput Width="65" ID="dteDeliveryByTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00" />
                        </td>
                        <td class="formCellInput">
                            <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                <img id="Img10" runat="server" alt="warning icon" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by date." />
                            </asp:RequiredFieldValidator>
                            <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                <img id="Img9" runat="server" alt="warning icon" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by time." />
                            </asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate" ValidationGroup="submit" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate" ErrorMessage="The date cannot be before today.">
                                <img id="imgic1" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server" title="The date cannot be before today" alt="warning icon" />
                            </asp:CustomValidator>
                            <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2" ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?" ValidationGroup="submit">
                                <img id="imgic2" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server" title="The date entered is far in the future." alt="warning icon" />
                            </asp:CustomValidator>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trAttemptedCollection_Point">
            <td class="formCellLabel" valign="top">
                Points
            </td>
            <td style="padding:0 0 0 0; margin:0 0 0 0;">
                <table cellpadding="0" cellspacing="0">
                    <tr>
                        <td class="formCellField" valign="top" style="width:350px;">
                            <asp:CheckBox ID="chkCollectGoodsElsewhere" runat="server" Text="Is the Collection Point Changing?" onclick="javascript:chkCollectGoodsElsewhere_onClick(this);" />
                            <div style="height: 30px;"></div>
                            <div id="divCollectionPoint">
                                <orchestrator:point ID="ucNewCollectionPoint" runat="server" CanCreateNewPoint="true" CanChangePoint="true" CanUpdatePoint="false" CanClearPoint="true" ValidationGroup="UpdateOrders" />
                            </div>
                        </td>
                        <td class="formCellField" valign="top" style="width:350px;">
                            <asp:CheckBox ID="chkDeliverGoodsElsewhere" runat="server" Text="Are you delivering goods elsewhere?" onclick="javascript:chkDeliverGoodsElsewhere_onClick(this);" />
                            <div style="height: 30px;"><b>NB</b>: This will only change the orders delivery point, it will not change any cross dock information.</div>
                            <div id="divDeliveryPoint">
                                <orchestrator:point ID="ucNewDeliveryPoint" runat="server" CanCreateNewPoint="true" CanChangePoint="true" CanUpdatePoint="false" CanClearPoint="true" ValidationGroup="UpdateOrders" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr id="trAttemptedCollection_CreateNewRun">
            <td class="formCellLabel" valign="top">
                Create New Run
            </td>
            <td class="formCellField">
                <asp:CheckBox runat="server" ID="chkCreateOnwardRun" Text="Yes" />
            </td>
        </tr>
        <tr id="trAttemptedCollection_Charge">
            <td class="formCellLabel" valign="top">
                Are you charging for this?
            </td>
            <td class="formCellField">
                <asp:CheckBox runat="server" ID="chkCharging" Text="Yes" onclick="javascript:chkCharging_onClick(this);" />
            </td>
        </tr>
        <tr id="trCharge_Amount">
            <td class="formCellLabel" valign="top">
                Amount
            </td>
            <td class="formCellField">
                <telerik:RadNumericTextBox ID="txtExtraAmount" runat="server" MinValue="0" Type="Number">
                </telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr id="trCharge_Type">
            <td class="formCellLabel" valign="top">
                Type of Extra
            </td>
            <td class="formCellField">
                <asp:DropDownList ID="cboExtraType" runat="server" />
            </td>
        </tr>
        <tr id="trCharge_State">
            <td class="formCellLabel" valign="top">
                State of Extra
            </td>
            <td class="formCellField">
                <asp:DropDownList ID="cboExtraState" runat="server" AutoPostBack="false" onchange="javascript:cboExtraState_onChange(this);" />
            </td>
        </tr>
        <tr id="trCharge_Client">
            <td class="formCellLabel" valign="top">
                Client Contact
            </td>
            <td class="formCellField">
                <asp:TextBox ID="txtClientContact" runat="server" MaxLength="200" Width="200px" />
                <asp:RequiredFieldValidator ID="rfvClientContact" runat="server" ControlToValidate="txtClientContact" Display="Dynamic" ErrorMessage="Please provide the client contact for the extra." ValidationGroup="UpdateOrders">
                    <img id="Img4" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please provide the client contact for the extra." />
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr id="trCharge_ExCustomReason">
            <td class="formCellLabel" valign="top">
                Reason
            </td>
            <td class="formCellField">
                <asp:TextBox ID="txtExtraCustomReason" runat="server" MaxLength="100" Width="200px" />
            </td>
        </tr>
    </table>
    
        <div class="buttonbar">
            <asp:Button ID="btnSave" runat="server" Text="Save" Width="95px" ToolTip="Use this button to apply the settings shown above." CausesValidation="true" ValidationGroup="collection" />&nbsp;
            <asp:Button ID="btnCancel" runat="Server" Text="Cancel" CausesValidation="false" Width="95px" ToolTip="Return to the Job Management page." />
        </div>
    </div>
        
    <div class="masterpagelite_toolTip" id="toolTip" style="display: none;">
        <div id="toolTipInner"></div>
    </div>
    
    <input type="hidden" runat="server" value="0" id="hidPartialPostback" />
    <input id="hidDeliveryTimingMethod" type="hidden" runat="server" value="anytime" />
    <input id="hidCollectionTimingMethod" type="hidden" runat="server" value="anytime" />
    <input runat="server" id="hidCollectionFromTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidCollectionByTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
</asp:Content>