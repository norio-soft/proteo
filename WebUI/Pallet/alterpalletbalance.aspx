<%@ Page language="c#" Inherits="Orchestrator.WebUI.Pallet.AlterPalletBalance" MasterPageFile="~/WizardMasterPage.Master" Title="Alter Pallet Balance" Codebehind="AlterPalletBalance.aspx.cs" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="uc1" TagName="point" Src="~/usercontrols/point.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="alterpalletbalance.aspx.js"></script>
    
    <script type="text/javascript" language="javascript">
        var leavePalletTypeID = <%=((int)Orchestrator.eInstructionType.LeavePallets) %>;
        var dehirePalletTypeID = <%=((int)Orchestrator.eInstructionType.DeHirePallets) %>;
        var userID = <%=((Page.User) as Orchestrator.Entities.CustomPrincipal).IdentityId %>;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var _skipCollectionDateCheck = false;
        
        // This fires once the ASP.NET Ajax engine has initialised and is available.
        function pageLoad() {
            var deliveryMethod = $('input:hidden[id*=hidDeliveryTimingMethod]').val();
            
            var rfvDeHireReceipt = $("span[id*='rfvDeHireReceipt']");
            var txtDeHireReceipt = $("input[id*='txtDeHireReceipt']");;
            ValidatorEnable(rfvDeHireReceipt[0], false);
            txtDeHireReceipt.prop('disabled', true).val("none_available");
            
            if (deliveryMethod == 'anytime') {
                $('input:radio[id*=rdDeliveryIsAnytime]')[0].checked = true;
                deliveryIsAnytime(null);
            }

            if (deliveryMethod == 'timed') {
                deliveryTimedBooking(null);
            }
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
            
        }
        
        function leavePallets()
        {
            palletActionButtonClicked(leavePalletTypeID)
        }
        
        function dehirePallets()
        {
            palletActionButtonClicked(dehirePalletTypeID)
        }
        
        function palletActionButtonClicked(palletAction)
        {
            if(palletAction == leavePalletTypeID)
                showLoading("Leaving Pallets");
            else
                showLoading("Dehiring Pallets");
                
            var ramAlterPalletBalance = $find("<%=ramAlterPalletBalance.ClientID %>");
            ramAlterPalletBalance.ajaxRequest(palletAction);
            
            return false;
        }
        
        function ramAlterPalletBalance_OnResponseEnd(sender, eventArgs)
        {
            resetTabs();
            hideLoading();
        }
        
        function setNotAvailableValidation(sender)
        {
            var rfvDeHireReceipt = $("span[id*='rfvDeHireReceipt']");
            var txtDeHireReceipt = $("input[id*='txtDeHireReceipt']");;
            
            if(sender.checked)
            {
                ValidatorEnable(rfvDeHireReceipt[0], false);
                txtDeHireReceipt.prop('disabled', true).val("none_available");
            }
            else
            {
                ValidatorEnable(rfvDeHireReceipt[0], true);
                txtDeHireReceipt.removeAttr('disabled').val("");
            }
        }
        
        function resetTabs()
        {
                                
        }        
    
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Alter Pallet Balance</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="divNoDetails" runat="server">
        <div style="padding:5px 0px 10px 10px;">
            <h3>No Details Found</h3>
            <p>
                <asp:Label ID="lblNoDetails" runat="server" Text="Please try re-opening the page as the current details failed to load." />
            </p>
        </div>
    </div>
    
    <div id="divPalletHandling" runat="server">
        <div style="padding:5px 0px 10px 10px;">
            <h3>Trailer Pallet Information</h3>
            <table>
                <tr>
                    <td class="formCellLabel">Pallets From Run Id</td>
                    <td class="formCellField"><asp:Label ID="lblRunID" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Pallet Type</td>
                    <td class="formCellField"><asp:Label ID="lblPalletType" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">No Of Pallets</td>
                    <td class="formCellField"><asp:Label ID="lblNoOfPallets" runat="server" /></td>
                </tr>
            </table>
        </div>

        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Leave Pallets</a></li>
                <li><a href="#tabs-2">De-Hire Pallets</a></li>
            </ul>
            <div id="tabs-1">
                <div style="margin:5px 0px 10px 10px;">
                    <div>
                        <h3>Leave Pallet Details</h3>
                        <table>
                            <tr>
                                <td class="formCellLabel">Destination</td>
                                <td class="formCellField">
                                    <uc1:point ID="ucLeavePoint" runat="server" CanClearPoint="true" EditMode="false" IsDepotVisible="false" PointSelectionRequired="true" ShowFullAddress="true" ValidationGroup="leavePallets" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">No Of Pallets</td>
                                <td class="formCellField">
                                    <telerik:RadNumericTextBox ID="rntNoLeavePallets" runat="server" MinValue="0" Type="Number" NumberFormat-DecimalDigits="0" Width="50px" />
                                    <asp:RequiredFieldValidator ID="rfvNoLeavePallets" runat="server" ControlToValidate="rntNoLeavePallets" Display="Dynamic" ValidationGroup="leavePallets" ErrorMessage="Please enter the number of pallets to leave.">
                                        <img id="Img4" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter the number of pallets to leave.">
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        <div class="buttonbar" style="margin-top:10px;">
                            <asp:Button ID="btnLeavePallets" runat="server" Text="Leave Pallets" ValidationGroup="leavePallets" />
                        </div>
                    </div>
                </div>
            </div>
            <div id="tabs-2">
                <div style="margin:5px 0px 10px 10px;">
                    <div>
                        <h3>De-Hire Pallet Details</h3>
                        <table>
                            <tr id="deHireRow">
                                <td class="formCellLabel">DeHire Pallets For</td>
                                <td class="formCellField" colspan="2">
                                    <telerik:RadComboBox ID="rcbDeHireOrganisation" runat="server" width="342px" DataTextField="OrganisationName" DataValueField="IdentityId" EnableLoadOnDemand="true" ShowMoreResultsBox="false" Height="150px" Overlay="true" AllowCustomText="false" MarkFirstMatch="true" ItemRequestTimeout="500" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Destination</td>
                                <td class="formCellField" colspan="2">
                                    <uc1:point ID="ucDeliveryPoint" runat="server" CanClearPoint="true" EditMode="false" IsDepotVisible="false" PointSelectionRequired="true" ShowFullAddress="true" ValidationGroup="dehirePallets" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">De-hire When</td>
                                <td class="formCellField" colspan="2">
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
                                                <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="dehirePallets" runat="server"
                                                    ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                                    <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                        title="Please enter a delivery from date."></asp:RequiredFieldValidator>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="dehirePallets" runat="server"
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
                                                <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="dehirePallets" runat="server"
                                                    ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                                    <img id="Img10" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                        title="Please enter a delivery by date."></asp:RequiredFieldValidator>
                                                <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="dehirePallets" runat="server"
                                                    ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                                    <img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                        title="Please enter a delivery by time."></asp:RequiredFieldValidator>
                                                <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                                    ValidationGroup="dehirePallets" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate"
                                                    ErrorMessage="The date cannot be before today.">
                                                            <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/></asp:CustomValidator>
                                                <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                                    Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2"
                                                    ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?"
                                                    ValidationGroup="dehirePallets">
                                                            <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date entered is far in the future." alt="warning icon"/></asp:CustomValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">De-hire Receipt Number</td>
                                <td class="formCellField" style="width:180px;">
                                    <asp:TextBox ID="txtDeHireReceipt" runat="server"></asp:TextBox>
                                    <asp:RequiredFieldValidator ID="rfvDeHireReceipt" runat="server" ErrorMessage="Please enter the de-hire receipt number" ControlToValidate="txtDeHireReceipt" Display="Dynamic" ValidationGroup="dehirePallets" EnableClientScript="True">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please enter the de-hire receipt number" />
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td align="left">
                                    <asp:CheckBox ID="chkNotAvailble" runat="server" Checked="true" Text="Not Availble" onclick="javascript:setNotAvailableValidation(this);" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Receipt Type</td>
                                <td class="formCellField" colspan="2">
                                    <telerik:RadComboBox ID="rcbDehireRecieptType" runat="server" Width="142px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">No Of Pallets</td>
                                <td class="formCellField" colspan="2">
                                    <telerik:RadNumericTextBox ID="rntNoDehirePallets" runat="server" MinValue="0" Type="Number" NumberFormat-DecimalDigits="0" Width="50px" />
                                    <asp:RequiredFieldValidator ID="rfvNoDehirePallets" runat="server" ControlToValidate="rntNoDehirePallets" Display="Dynamic" ValidationGroup="dehirePallets" ErrorMessage="Please enter the number of pallets to dehire.">
                                        <img id="Img3" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter the number of pallets to dehire.">
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Charge</td>
                                <td class="formCellField" colspan="2">
                                    <telerik:RadNumericTextBox ID="rntPalletDeliveryCharge" runat="server" Type="Currency" NumberFormat-DecimalDigits="2" MinValue="0" Width="50px" />
                                    <asp:RequiredFieldValidator ID="rfvPDC" runat="server" ControlToValidate="rntPalletDeliveryCharge" ValidationGroup="dehirePallets"  Display="Dynamic" ErrorMessage="Please enter the value of the pallets to deliver.">
                                        <img id="Img1" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter the value of the pallets to deliver.">
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                        </table>
                        <div class="buttonbar" style="margin-top:10px;">
                            <asp:Button ID="btnDeHirePallets" runat="server" Text="De-hire Pallets" ValidationGroup="dehirePallets" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
        
        <div>
            <input id="hidDeliveryTimingMethod" type="hidden" runat="server" value="anytime" />
            <input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
            <input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
            <asp:HiddenField ID="hidShowConfirmForOrderAfterDays" runat="server" Value="" />
        </div>
    </div>
    
    <div class="buttonbar" style="margin-top:10px;">
        <asp:Button ID="btnClose" runat="server" Text="Close" />
    </div>
    
    <telerik:RadAjaxManager ID="ramAlterPalletBalance" runat="server" ClientEvents-OnResponseEnd="ramAlterPalletBalance_OnResponseEnd">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ramAlterPalletBalance">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lblNoOfPallets" />

                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
</asp:Content>
