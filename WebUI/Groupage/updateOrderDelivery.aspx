<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" CodeBehind="updateOrderDelivery.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.updateOrderDelivery" %>

<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1 style="display:none;"><asp:Label runat="server" ID="lblHeader"></asp:Label></h1>
    <input id="hidDeliveryTimingMethod" type="hidden" runat="server" value="anytime" />
    <input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
    <asp:HiddenField ID="hidShowConfirmForOrderAfterDays" runat="server" Value="" />
        <div>
            <h1>The orders below, all belong to the same drop instruction on Run Id <asp:HyperLink ID="hypRunTitle" runat="server"></asp:HyperLink>, delivering to <asp:Label ID="lblTitleDelivery" runat="server" ForeColor="Green"></asp:Label></h1>
        </div>
        <div>
            <h2>Enter the new delivery details, select the orders you wish to update and click the Update Orders button.</h2>
        </div>    
        <fieldset>
            <table>
            <tr>
                <td>Deliver Where:</td>
                <td>Deliver When:</td>
            </tr>
                <tr>
                    <td>
                        <p1:Point runat="server" ID="cboNewDeliveryPoint" CanChangePoint="true" CanClearPoint="true" CanCreateNewPoint="true" 
                        CanUpdatePoint="true" PointSelectionRequired="true" PointType="Deliver" ShowFullAddress="true" />
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td class="formCellField-Horizontal">
                                    <table>
                                        <tr>
                                            <td>
                                                <input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" />Timed
                                                Booking
                                                <input type="radio" name="delivery" runat="server" id="rdDeliveryBookingWindow"
                                                    onclick="deliveryBookingWindow(this);" />Booking Window
                                                <input type="radio" name="delivery" runat="server" id="rdDeliveryIsAnytime" checked="true" onclick="deliveryIsAnytime(this);" />Anytime
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
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblError" runat="server" ForeColor="Red"></asp:Label>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonBar">
            <asp:Button runat="server" ID="btnUpdateOrders" text="Update Order(s)" />
            <asp:Button runat="server" ID="btnRefresh" text="Refresh" />
        </div>
    
        <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" ShowGroupPanel="false" ShowFooter="false" AllowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="100%" >
        <MasterTableView ClientDataKeyNames="OrderID" DataKeyNames="OrderID" NoMasterRecordsText="Please click search to find orders" CommandItemDisplay="Top" GroupLoadMode="Client" >
            <CommandItemTemplate>
            </CommandItemTemplate>
            <RowIndicatorColumn Display="false">
            </RowIndicatorColumn>
            <Columns>
                <telerik:GridTemplateColumn DataField="InstructionId" UniqueName="InstructionId" Display="false">
                    <ItemTemplate>
                        <asp:Label ID="lblInstructionId" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderStyle-Width="20" HeaderText="">
                    <HeaderTemplate>
                        <input type="checkbox" onClick="javascript:HandleSelectAll(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderID" HeaderStyle-Width="60px">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypUpdateOrder" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Run&nbsp;Id" SortExpression="JobId" HeaderStyle-Width="50px" >
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypRun" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" HeaderStyle-Width="200px" SortExpression="CustomerOrganisationName" DataField="CustomerOrganisationName" UniqueName="Customer">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Business Type" HeaderStyle-Width="100px" SortExpression="BusinessTypeDescription" DataField="BusinessTypeDescription" UniqueName="BusinessType">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Deliver Where" SortExpression="DeliveryPointDescription" HeaderStyle-Width="160px"
                    DataField="DeliveryPointDescription" ItemStyle-Wrap="true" UniqueName="DeliveryPointDescription" >
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver When" SortExpression="DeliveryDateTime" UniqueName="DeliveryDateTime" HeaderStyle-Width="120px">
                    <ItemTemplate>
                        <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber" HeaderStyle-Width="120px"
                    DataField="DeliveryOrderNumber" UniqueName="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription" HeaderStyle-Width="160px"
                    DataField="CollectionPointDescription" ItemStyle-Wrap="true" UniqueName="CollectionPointDescription">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect When" SortExpression="CollectionDateTime" UniqueName="CollectionDateTime" HeaderStyle-Width="120px">
                    <ItemTemplate>
                        <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="ForeignRate" HeaderStyle-Width="50px" UniqueName="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblRate" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowDragToGroup="false" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
             <Scrolling UseStaticHeaders="true" ScrollHeight="500" AllowScroll="true" />
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true"  />
        </ClientSettings>
    </telerik:RadGrid>
    
    <telerik:RadInputManager ID="rimApproveOrder" runat="server">
        <telerik:DateInputSetting BehaviorID="DateInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" EmptyMessage="Enter a valid Date" DateFormat="dd/MM/yy" SelectionOnFocus="SelectAll"
            EnabledCssClass="DateControl" FocusedCssClass="DateControl_Focused" HoveredCssClass="DateControl_Hover"
            InvalidCssClass="DateControl_Error" DisplayDateFormat="dd/MM/yy">
        </telerik:DateInputSetting>
        <telerik:DateInputSetting BehaviorID="TimeInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" EmptyMessage="Anytime" DateFormat="HH:mm" DisplayDateFormat="HH:mm"
            EnabledCssClass="TimeControl" FocusedCssClass="TimeControl_Focused" HoveredCssClass="TimeControl_Hover"
            InvalidCssClass="TimeControl_Error">
        </telerik:DateInputSetting>
        <telerik:NumericTextBoxSetting BehaviorID="NumericInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" Type="Currency" DecimalDigits="2" MinValue="0" EnabledCssClass="TextControl"
            FocusedCssClass="TextControl_Focused" HoveredCssClass="TextControl_Hover" InvalidCssClass="TextControl_Error">
        </telerik:NumericTextBoxSetting>
    </telerik:RadInputManager>
    <script language="javascript" type="text/javascript">
        var _skipCollectionDateCheck = false;
        var _skipDeliveryDateCheck = false;

        function ViewOrderProfile(orderID) {
            var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

            var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
        }

        function ViewRun(runId) {
            var url = "/job/job.aspx?jobId=" + runId + getCSID();

            window.open(url);
        }
    
        function HandleSelectAll(chk) {
            $(":checkbox[id$=chkSelectOrder]").each(function(chkIndex) { if (this.checked != chk.checked) this.click(); });
        }

        function pageLoad() {
            var deliveryMethod = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

            //$('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

            if (deliveryMethod == 'anytime') {
                deliveryIsAnytime(null);
            }

            if (deliveryMethod == 'timed') {
                deliveryTimedBooking(null);
            }

            if (deliveryMethod == 'window') {
                deliveryBookingWindow(null);
            }
        }

        function HandleSelection(chk, rowIndex) {
            var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            var dataItem = mtv.get_dataItems()[rowIndex].get_element();

            if (chk.checked) {
                mtv.selectItem(dataItem);
                dataItem.className = "SelectedRow_Orchestrator";
            }
            else {
                mtv.deselectItem(dataItem);
                dataItem.className = "GridRow_Orchestrator";
            }
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

            if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val()) {
                $find("<%=dteDeliveryFromTime.ClientID %>").set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
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
                        alert("The date entered is far in the future - Are you sure?");
                    }
                }
                else {
                    args.IsValid = true;
                }
        }
    </script>
</asp:Content>
