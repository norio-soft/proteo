<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="True" CodeBehind="PalletHandlingWizard.aspx.cs" Inherits="Orchestrator.WebUI.Pallet.PalletHandlingWizard" Title="Untitled Page" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="uc1" TagName="point" Src="~/usercontrols/point.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="PalletHandlingWizard.aspx.js"></script>

    <script language="javascript" type="text/javascript">
        var leavePalletTypeID = <%=((int)Orchestrator.eInstructionType.LeavePallets) %>;
        var dehirePalletTypeID = <%=((int)Orchestrator.eInstructionType.DeHirePallets) %>;
        var userID = <%=((Page.User) as Orchestrator.Entities.CustomPrincipal).IdentityId %>;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var _skipCollectionDateCheck = false;
        
        // This fires once the ASP.NET Ajax engine has initialised and is available.
        function pageLoad() {
            var collectionMethod = $('input:hidden[id*=hidCollectionTimingMethod]').val();
            var deliveryMethod = $('input:hidden[id*=hidDeliveryTimingMethod]').val();
            
            if (collectionMethod == 'anytime') {
                $('input:radio[id*=rdCollectionIsAnytime]')[0].checked = true;
                collectionIsAnytime(null);
            }

            if (collectionMethod == 'timed') {
                collectionTimedBooking(null);
            }
            
            if (deliveryMethod == 'anytime') {
                $('input:radio[id*=rdDeliveryIsAnytime]')[0].checked = true;
                deliveryIsAnytime(null);
            }

            if (deliveryMethod == 'timed') {
                deliveryTimedBooking(null);
            }
        }
        
        function rcbSelectedPalletType_OnClientSelectedIndexChanged(sender, eventArgs){
            var selectedValue = parseInt(sender.get_value(), 10);
            var rntSelectedPallets = $find("<%=rntSelectedPallets.ClientID %>");
        
            if(selectedValue > 0)
            {
                var palletTypePlannedBalance = $("input:hidden[id*='palletTypePlannedBalance'][PalletTypeID*='" + selectedValue + "']");
                var palletBalance = palletTypePlannedBalance.attr("Balance");
                var plannedPalletBalance = palletTypePlannedBalance.attr("PlannedBalance");
                var remainingBalance = palletBalance - plannedPalletBalance;
                rntSelectedPallets.set_maxValue(remainingBalance);
            }
            else
                rntSelectedPallets.set_maxValue(0);
        }
        
        function collectionIsAnytime(rb) {
            $('tr[id*=trCollectBy]').hide();

            var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
            if (rb != null) {
                dteCollectionFromDate.focus();
                dteCollectionFromDate.select();
            }

            $('input:hidden[id*=hidCollectionTimingMethod]').val('anytime');

            $('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val($find("<%=dteCollectionFromTime.ClientID %>").get_value());
            $('input:hidden[id*=hidCollectionByTimeRestoreValue]').val($find("<%=dteCollectionByTime.ClientID %>").get_value());

            $find("<%=dteCollectionFromTime.ClientID %>").set_value('00:00');
            $find("<%=dteCollectionFromTime.ClientID %>").disable();

            $find("<%=dteCollectionByTime.ClientID %>").set_value('23:59');
            $find("<%=dteCollectionByTime.ClientID %>").disable();
            
        }
        
        function collectionTimedBooking(rb) {

            $('tr[id*=trCollectBy]').hide();

            var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
            if (rb != null) {
                dteCollectionFromDate.focus();
                dteCollectionFromDate.select();
            }

            var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

            $('input:hidden[id*=hidCollectionTimingMethod]').val('timed');

            if (method == 'window') {
                $('input:hidden[id*=hidCollectionByTimeRestoreValue]').val($find("<%=dteCollectionByTime.ClientID %>").get_value());
            }

            $find("<%=dteCollectionByTime.ClientID %>").set_value($find("<%=dteCollectionFromTime.ClientID %>").get_value());

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionFromTime.ClientID %>").set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

                } else {
                    $find("<%=dteCollectionFromTime.ClientID %>").set_value("08:00");
                }

                if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionByTime.ClientID %>").set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteCollectionByTime.ClientID %>").set_value("17:00");
                }
            }
            $find("<%=dteCollectionByTime.ClientID %>").enable();
            $find("<%=dteCollectionFromTime.ClientID %>").enable();
        }

        function collectionBookingWindow(rb) {
            $('tr[id*=trCollectBy]').show();
            var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
            if (rb != null) {
                dteCollectionFromDate.focus();
                dteCollectionFromDate.select();
            }

            var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

            $('input:hidden[id*=hidCollectionTimingMethod]').val('window');

            if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
                $find("<%=dteCollectionByTime.ClientID %>").set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
            }

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionFromTime.ClientID %>").set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

                } else {
                    $find("<%=dteCollectionFromTime.ClientID %>").set_value("08:00");
                }

                if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionByTime.ClientID %>").set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteCollectionByTime.ClientID %>").set_value("17:00");
                }
            }
                               
            $find("<%=dteCollectionByTime.ClientID %>").enable();
            $find("<%=dteCollectionFromTime.ClientID %>").enable();
        }

        function deliveryTimedBooking(rb) {
            $('tr[id*=trDeliverFrom]').hide();

            var dteDeliveryByDate = $('input[id*=dteDeliveryByDate]');
            if (rb != null) {
                dteDeliveryByDate.focus();
                dteDeliveryByDate.select();
            }

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
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value("08:00");
                }

                if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value("17:00");
                }
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").enable();
            $find("<%=dteDeliveryByTime.ClientID %>").enable();
        }
        
        function deliveryBookingWindow(rb) {
            $('tr[id*=trDeliverFrom]').show();

            var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
            if (rb != null) {
                dteDeliveryFromDate.focus();
                dteDeliveryFromDate.select();
            }

            var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

            $('input:hidden[id*=hidDeliveryTimingMethod]').val('window');

            if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                $find("<%=dteDeliveryByTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
            }

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryFromTime.ClientID %>").set_value("08:00");
                }

                if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteDeliveryByTime.ClientID %>").set_value("17:00");
                }
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").enable();
            $find("<%=dteDeliveryByTime.ClientID %>").enable();
        }
        
        function deliveryIsAnytime(rb) {
            $('tr[id*=trDeliverFrom]').hide();
            var dteDeliveryFromDate = $('input[id*=dteDeliveryFromDate]');
            if (rb != null) {
                dteDeliveryFromDate.focus();
                dteDeliveryFromDate.select();
            }

            $('input:hidden[id*=hidDeliveryTimingMethod]').val('anytime');

            $('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_value());
            $('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val($find("<%=dteDeliveryByTime.ClientID %>").get_value());

            $find("<%=dteDeliveryFromTime.ClientID %>").set_value('00:00');
            $find("<%=dteDeliveryFromTime.ClientID %>").disable();

            $find("<%=dteDeliveryByTime.ClientID %>").set_value('23:59');
            $find("<%=dteDeliveryByTime.ClientID %>").disable();
            
        }
        
        function dteCollectionFromDate_SelectedDateChanged(sender, eventArgs) {
            var dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID %>");
            var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");
            var updateDate = false;

            if (dteCollectionByDate != null)
            { 
                if(rdCollectionBookingWindow != null && !rdCollectionBookingWindow.prop("checked"))
                    updateDate = true;
                else if(sender.get_selectedDate() > dteCollectionByDate.get_selectedDate())
                    updateDate = true;
            }

            if (updateDate) {
                dteCollectionByDate.set_selectedDate(sender.get_selectedDate());
            }
        }

        function dteCollectionByDate_SelectedDateChanged(sender, eventArgs) {
            var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
            var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");

            if (dteCollectionFromDate != null && sender.get_selectedDate() < dteCollectionFromDate.get_selectedDate() && rdCollectionBookingWindow != null && rdCollectionBookingWindow.prop("checked"))
                dteCollectionFromDate.set_selectedDate(sender.get_selectedDate());
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

            if (dteDeliveryFromDate != null)
            {
                if( sender.get_selectedDate() < dteDeliveryFromDate.get_selectedDate())
                    updateDate = true;
                else if(rdDeliveryBookingWindow != null && !rdDeliveryBookingWindow.prop("checked"))
                    updateDate = true;
            }
            
            if(updateDate)
                dteDeliveryFromDate.set_selectedDate(sender.get_selectedDate());
        }
        
        function CV_ClientValidateCollectionDate(source, args) {

            var dteDateTime = $find("<%=dteCollectionFromDate.ClientID%>");
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
                if (!_skipDeliveryDateCheck) {
                    _skipDeliveryDateCheck = true;
                    args.IsValid = false;
                    alert("The collection date entered is in the past - Are you sure?");
                }
            }

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
                    alert("The delivery date entered is in the past - Are you sure?");
                }
            }
        }
        
        function CV_ClientValidateDeliveryDate2(source, args) {
            var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
            var hidShowConfirmForOrderAfterDays = $get("<%=hidShowConfirmForOrderAfterDays.ClientID%>");
            var warningDate = new Date();
            var day_date = warningDate.getDate();
            var month_date = warningDate.getMonth();
            var year_date = warningDate.getFullYear();

            warningDate.setFullYear(year_date, month_date, day_date);
            warningDate.setDate(warningDate.getDate() + parseInt(hidShowConfirmForOrderAfterDays.value));

            var enteredDateParts = dteDateTime.get_displayValue().split("/");

            var enteredDate = new Date();
            enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

            if (enteredDate >= warningDate) {
                if (!_skipCollectionDateCheck) {
                    _skipCollectionDateCheck = true;

                    args.IsValid = true; // do not prevent the order from being created.
                    alert("The delivery date entered is far in the future - Are you sure?");
                }
            }
            else {
                args.IsValid = true;
            }
        }
        
        function loadRunWindow(runID)
        {
            var qs = "JobId=" + runID;
            <%=dlgRun.ClientID %>_Open(qs);
        }
        
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Pallet Run Wizard</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgRun" runat="server" URL="/Job/Job.aspx" Height="900" Width="1024" Mode="Normal" AutoPostBack="true" ReturnValueExpected="true" UseCookieSessionID="true"/>

    <div>
        <div class="whitepacepusher"></div>
        
        <div style="float:left; margin-left:5px; height:540px;">
            <h3>Pallet Delivery</h3>
            <div>
                <table>
                    <tr>
                        <td class="formCellLabel">Select Pallet Type</td>
                        <td>
                            <telerik:RadComboBox ID="rcbSelectedPalletType" runat="server" DataValueField="PalletTypeID" DataTextField="PalletDescription" width="250px" OnClientSelectedIndexChanged="rcbSelectedPalletType_OnClientSelectedIndexChanged" />
                            <asp:RequiredFieldValidator ID="rfvPalletType" ValidationGroup="submit" runat="server"
                                ControlToValidate="rcbSelectedPalletType" Display="Dynamic" ErrorMessage="Please select a pallet type.">
                                <img id="Img5" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                            title="Please select a pallet type."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Action</td>
                        <td>
                            <telerik:RadComboBox ID="rcbPalletHandlingAction" runat="server" width="250px" OnClientSelectedIndexChanged="rcbPalletHandlingAction_OnClientSelectedIndexChanged" />
                        </td>
                    </tr>
                    <tr id="deHireRow">
                        <td class="formCellLabel">DeHire Pallets For</td>
                        <td >
                            <telerik:RadComboBox ID="rcbDeHireOrganisation" runat="server" width="250px" DataTextField="OrganisationName" DataValueField="IdentityId" EnableLoadOnDemand="true" ShowMoreResultsBox="false" Height="150px" Overlay="true" AllowCustomText="false" MarkFirstMatch="true" ItemRequestTimeout="500" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Destination</td>
                        <td>
                            <uc1:point ID="ucDeliveryPoint" runat="server" CanClearPoint="true" EditMode="false" IsDepotVisible="false" PointSelectionRequired="true" ShowFullAddress="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">
                            Collect When
                        </td>
                        <td class="formCellField">
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" name="collection" runat="server" id="rdCollectionTimedBooking" onclick="collectionTimedBooking(this);" />Timed Booking
                                        <input type="radio" name="collection" runat="server"  id="rdCollectionBookingWindow" onclick="collectionBookingWindow(this);" />Booking Window
                                        <input type="radio" name="collection" runat="server" id="rdCollectionIsAnytime" checked="true" onclick="collectionIsAnytime(this);" />Anytime
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td class="formCellLabel" style="width: 80px;">
                                        Collect from:
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDateInput Width="65" ID="dteCollectionFromDate" runat="server" DateFormat="dd/MM/yy"
                                            DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionFromDate_SelectedDateChanged">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDateInput Width="65" ID="dteCollectionFromTime" runat="server" DateFormat="HH:mm"
                                            DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellInput">
                                        <asp:RequiredFieldValidator ID="rfvCollectionFromDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionFromDate" Display="Dynamic" ErrorMessage="Please enter a collection from date.">
                                            <img id="Img3" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection from date."></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvCollectionFromTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionFromTime" Display="Dynamic" ErrorMessage="Please enter a collection from time.">
                                            <img id="Img14" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection from time."></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="dteCollectionFromDate"
                                            Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateCollectionDate"
                                            ErrorMessage="The date cannot be before today." ValidationGroup="submit">
                                                    <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr runat="server" id="trCollectBy">
                                    <td class="formCellLabel">
                                        Collect by:
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDateInput Width="65" ID="dteCollectionByDate" runat="server" DateFormat="dd/MM/yy"
                                            DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionByDate_SelectedDateChanged">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDateInput Width="65" ID="dteCollectionByTime" runat="server" DateFormat="HH:mm"
                                            DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellInput">
                                        <asp:RequiredFieldValidator ID="rfvCollectionByDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionByDate" Display="Dynamic" ErrorMessage="Please enter a collection by date.">
                                            <img id="Img12" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection by date."></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvCollectionByTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionByTime" Display="Dynamic" ErrorMessage="Please enter a collection by time.">
                                            <img id="Img13" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection by time."></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">
                            Deliver When
                        </td>
                        <td class="formCellField">
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" />Timed Booking
                                        <input type="radio" name="delivery" runat="server" id="rdDeliveryBookingWindow" onclick="deliveryBookingWindow(this);" />Booking Window
                                        <input type="radio" name="delivery" runat="server" checked="true" id="rdDeliveryIsAnytime" onclick="deliveryIsAnytime(this);" />Anytime
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr runat="server" id="trDeliverFrom">
                                    <td class="formCellLabel">
                                        Deliver from:
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDateInput Width="65" ID="dteDeliveryFromDate" runat="server" DateFormat="dd/MM/yy"
                                            DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteDeliveryFromDate_SelectedDateChanged" SelectionOnFocus="SelectAll">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDateInput Width="65" ID="dteDeliveryFromTime" runat="server" DateFormat="HH:mm"
                                            DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" SelectionOnFocus="SelectAll">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellField">
                                        <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                            <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery from date."></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryFromTime" Display="Dynamic" ErrorMessage="Please enter a delivery from time.">
                                            <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery from time."></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel" style="width: 80px;">
                                        Deliver by:
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDateInput Width="65" ID="dteDeliveryByDate" runat="server" DateFormat="dd/MM/yy"
                                            DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteDeliveryByDate_SelectedDateChanged" SelectionOnFocus="SelectAll">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDateInput Width="65" ID="dteDeliveryByTime" runat="server" DateFormat="HH:mm"
                                            DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00" SelectionOnFocus="SelectAll">
                                        </telerik:RadDateInput>
                                    </td>
                                    <td class="formCellField">
                                        <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                            <img id="Img10" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery by date."></asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                            <img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery by time."></asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                            ValidationGroup="submit" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate"
                                            ErrorMessage="The date cannot be before today.">
                                                    <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/></asp:CustomValidator>
                                        <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                            Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2"
                                            ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?"
                                            ValidationGroup="submit">
                                                    <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date entered is far in the future." alt="warning icon"/></asp:CustomValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">Charge</td>
                        <td>
                            <telerik:RadNumericTextBox ID="rntPalletDeliveryCharge" runat="server" Type="Currency" NumberFormat-DecimalDigits="2" MinValue="0" Width="50px" />
                            <asp:RequiredFieldValidator ID="rfvPDC" runat="server" ControlToValidate="rntPalletDeliveryCharge" ValidationGroup="submit"  Display="Dynamic" ErrorMessage="Please enter the value of the pallets to deliver.">
                                <img id="Img1" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from date.">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">No Of Pallets</td>
                        <td>
                            <telerik:RadNumericTextBox ID="rntSelectedPallets" runat="server" Type="Number" Width="25px" NumberFormat-DecimalDigits="0" MinValue="0" />
                            <asp:RequiredFieldValidator ID="rfvSelectedPallets" runat="server" ControlToValidate="rntSelectedPallets" ValidationGroup="submit"  Display="Dynamic" ErrorMessage="Please enter the number of pallets to deliver.">
                                <img id="Img4" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter the number of pallets to deliver.">
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <div class="buttonbar" style="margin-top:10px;">
                    <asp:Button ID="btnCreatePalletDelivery" runat="server" Text="Create Pallet Delivery" ValidationGroup="submit" />
                </div>
            </div>
        </div>
        
        <div style="float:left; margin-left:30px;">
            <fieldset>
            <h3>Available Pallets</h3>
            <asp:ListView ID="lvSelectedPalletDetails" runat="server">
                <LayoutTemplate>
                    <table cellpadding="0" cellspacing="0">
                        <tbody>
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr>
                        <td class="formCellLabel" ><%# Eval("PalletDescription") %></td>
                        <td class="formCellInput" >
                            <%# Eval("PBalance")%> <b>(<%# Eval("PlannedBalance")%>)</b>
                            <input type="hidden" id="palletTypePlannedBalance" PalletTypeID='<%# Eval("PalletTypeID") %>' Balance='<%# Eval("PBalance") %>' PlannedBalance='<%# Eval("PlannedBalance") %>' />
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    No Pallets Selected
                </EmptyDataTemplate>
            </asp:ListView>
            </fieldset>
        </div>
        
        <div style="float:left; margin-left:30px;">
            <fieldset>
            <h3>Current Pallet Deliveries</h3>
            <asp:ListView ID="lvPalletDeliveries" runat="server">
                <LayoutTemplate>
                    <table cellpadding="0" cellspacing="0">
                        <thead><tr class="HeadingRow"><td>Destination</td><td>Action</td><td>Pallet Type</td><td>No of Pallets</td></tr></thead>
                        <tbody>
                            <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="Row">
                        <td><%# ((PalletDelivery)Container.DataItem).Destination %></td>
                        <td><%# ((PalletDelivery)Container.DataItem).PalletActionDescription %></td>
                        <td><%# ((PalletDelivery)Container.DataItem).PalletType %></td>
                        <td><%# ((PalletDelivery)Container.DataItem).PalletOrder.NoPallets %></td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    <table cellpadding="0" cellspacing="0">
                        <thead><tr class="HeadingRow"><td>Destination</td><td>Action</td><td>Pallet Type</td><td>No of Pallets</td></tr></thead>
                        <tbody>
                            <tr>
                                <td colspan="4">
                                    No Current Pallet Deliveries
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </EmptyDataTemplate>
            </asp:ListView>
            </fieldset>
        </div>
        
        <div class="clearDiv" ></div>
        
        <div class="buttonbar">
            <asp:Button ID="btnCreatePalletRun" runat="server" Text="Create Pallet Run" OnClientClick="if(!createPalletReturnRun()) return false;" />
            <asp:Button ID="btnClose" runat="server" Text="Close" />
        </div>
        
    </div>

    <div>
        <input id="hidCollectionTimingMethod" type="hidden" runat="server" value="anytime" />
        <input id="hidDeliveryTimingMethod" type="hidden" runat="server" value="anytime" />
        <input runat="server" id="hidCollectionFromTimeRestoreValue" type="hidden" />
        <input runat="server" id="hidCollectionByTimeRestoreValue" type="hidden" />
        <input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
        <input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
        <asp:HiddenField ID="hidShowConfirmForOrderAfterDays" runat="server" Value="" />
    </div>
    
</asp:Content>