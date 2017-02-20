<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.changeBookedTimes"
    CodeBehind="changeBookedTimes.aspx.cs" MasterPageFile="~/wizardmasterpage.Master" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>

    <script language="javascript" src="/script/tooltippopups.js" type="text/javascript"></script>

    <script language="javascript" src="/script/jquery.dataTables.min.js" type="text/javascript"></script>
    <script language="javascript" src="/script/FixedHeader.min.js" type="text/javascript"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <script type="text/javascript">
        var jobID = parseInt('<%= this.JobID %>', 10);
    </script>

    <script type="text/javascript" src="changeBookedTimes.aspx.js?ver=201300206"></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="PageTitlePlaceHolder1">Manage Booked Times</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="margin-bottom: 2px;">
        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
        <p>
            When using the <span style="font-style:italic; font-weight:bold;"> Simple </span> option this will use the specified date for ALL orders on this Instruction.
        </p>
        <table id="BookedTimes" cellpadding="0" border="0" cellspacing="0" style="border-top: 0px;  ">
            <thead  style="height:22px;">
                <tr class="HeadingRow">
                    <th>
                        Location
                    </th>
                    <th>
                        Action
                    </th>
                    <th>
                        Date
                    </th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="repBookedDateTimes" runat="server">
                    <ItemTemplate>
                        <tr id="rowInstruction" runat="server">
                            <td valign="top" width="250">
                                <div onmouseover="ShowPointToolTip(this, <%# Eval("PointId") %>);" onmouseout="closeToolTip();">
                                    <b><%# Eval("PointName") %></b>
                                </div>
                                <input type="hidden" id="hidInstructionId" runat="server" value='<%# Eval("InstructionId") %>' />
                                <input type="hidden" id="hidInstructionTypeID" runat="server" value='<%# Eval("InstructionTypeId") %>' />
                                <input type="hidden" id="hidPointID" runat="server" value='<%# Eval("PointID") %>' />
                                <asp:Label ID="lblInstructionDateTime" runat="server"></asp:Label>
                            </td>
                            <td valign="top" width="100">
                                <%# Orchestrator.WebUI.Utilities.UnCamelCase((Eval("InstructionType")).ToString())%>
                            </td>
                            <td valign="top">
                                <asp:Panel ID="pnlTrunkInstruction" runat="server">
                                    <table border="0" cellpadding="0" cellspacing="0">
                                        <tr>
                                            <td>
                                                <telerik:RadDatePicker Width="100" ID="dteBookedDate" runat="server"> 
                                                    <DateInput runat ="server"
                                                    DateFormat="dd/MM/yy"
                                                    DisplayDateFormat="dd/MM/yy">
                                                        </DateInput>
                                                </telerik:RadDatePicker>
                                            </td>
                                            <td>
                                                <telerik:RadTimePicker Width="65" ID="dteBookedTime" runat="server"> 
                                                    <DateInput runat="server"
                                                    DateFormat="HH:mm"
                                                    EmptyMessage="AnyTime" DisplayDateFormat="HH:mm">
                                                        </DateInput>
                                                </telerik:RadTimePicker>
                                            </td>
                                            <td width="20">
                                                <asp:RequiredFieldValidator ID="rfvBookedDate" runat="server" ControlToValidate="dteBookedDate"
                                                    Display="Dynamic" ErrorMessage="Please enter a Booked Date and Time.">
			                                        <img src="../images/error.gif" Title="Please enter a Booked Date and Time.">
                                                </asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>
                                <asp:Panel ID="pnlOrderInstruction" runat="server">
                                    <asp:HiddenField ID="hidView" runat="server" Value="simple" />
                                    <asp:Panel ID="pnlSwitcher" runat="server">
                                        <div class="buttonbar" style="height:18px;">
                                            <asp:Button runat="server" id="btnSimple" style="height:20px;font-size:8px;" OnClientClick="ShowSimple(this); return false;" Text="Simple" />
                                            <asp:Button runat="server" id="btnAdvanced" style="height:20px;font-size:8px;" OnClientClick="ShowAdvanced(this); return false;" Text="Advanced" />
                                        </div>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlSimple" runat="server" style="display:none;">
                                        <table>
                                            <tr>
                                                <td>
                                                    <input type="radio" name="collection" runat="server" id="rdCollectionTimedBooking"
                                                        onclick="collectionTimedBooking(this);" />Timed Booking
                                                    <input type="radio" name="collection" runat="server" id="rdCollectionBookingWindow"
                                                        onclick="collectionBookingWindow(this);" />Booking Window
                                                    <input type="radio" name="collection" runat="server" id="rdCollectionIsAnytime"
                                                        onclick="collectionIsAnytime(this);" />Anytime
                                                </td>
                                            </tr>
                                        </table>
                                        <table>
                                            <tr>
                                                <td class="formCellLabel" style="width: 80px;">
                                                    From:
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadDatePicker Width="100" ID="dteCollectionFromDate" runat="server"> 
                                                        <DateInput runat="server"
                                                        DateFormat="dd/MM/yy"
                                                        DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionFromDate_SelectedDateChanged">
                                                            </DateInput>
                                                    </telerik:RadDatePicker>
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadTimePicker Width="65" ID="dteCollectionFromTime" runat="server"> 
                                                        <DateInput runat="server"
                                                            DateFormat="HH:mm"
                                                        DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" OnClientDateChanged="dteCollectionFromTime_SelectedDateChanged">
                                                        </DateInput>
                                                        
                                                    </telerik:RadTimePicker>
                                                </td>
                                                <td class="formCellInput">
                                                    <asp:RequiredFieldValidator ID="rfvCollectionFromDate" ValidationGroup="submit" runat="server"
                                                        ControlToValidate="dteCollectionFromDate" Display="Dynamic" ErrorMessage="Please enter a collection from date.">
                                                        <img id="Img1" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                            title="Please enter a collection from date."></asp:RequiredFieldValidator>
                                                    <asp:RequiredFieldValidator ID="rfvCollectionFromTime" ValidationGroup="submit" runat="server"
                                                        ControlToValidate="dteCollectionFromTime" Display="Dynamic" ErrorMessage="Please enter a collection from time.">
                                                        <img id="Img14" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                            title="Please enter a collection from time."></asp:RequiredFieldValidator>
                                                  
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trCollectBy">
                                                <td class="formCellLabel">
                                                    To:
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadDatePicker Width="100" ID="dteCollectionByDate" runat="server" DateFormat="dd/MM/yy">
                                                        <DateInput runat="server" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionByDate_SelectedDateChanged">
                                                            </DateInput>
                                                    </telerik:RadDatePicker>
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadTimePicker Width="65" ID="dteCollectionByTime" runat="server">
                                                        <DateInput runat="server" DisplayDateFormat ="HH:mm" SelectedDate="01/01/01 17:00" OnClientDateChanged="dteCollectionFromTime_SelectedDateChanged" >
                                                            </DateInput>
                                                    </telerik:RadTimePicker>
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
                                        <input id="hidCollectionTimingMethod" type="hidden" runat="server" value="anytime" />
                                        <input runat="server" id="hidCollectionFromTimeRestoreValue" type="hidden" />
                                        <input runat="server" id="hidCollectionByTimeRestoreValue" type="hidden" />
                                    </asp:Panel>
                                    <asp:Panel ID="pnlAdvanced" runat="server" >
                                        <telerik:RadGrid runat="server" ID="grdOrders" AllowFilteringByColumn="false" AllowMultiRowSelection="true"
                                            AllowSorting="False" AutoGenerateColumns="false" EnableAjaxSkinRendering="true">
                                            <MasterTableView DataKeyNames="OrderID">
                                                <Columns>
                                                    <telerik:GridTemplateColumn UniqueName="OrderID" HeaderText="ID" HeaderStyle-Width="55px">
                                                        <ItemTemplate>
                                                            <a href="" onclick='viewOrderProfile(<%#Eval("OrderID") %>); return false;' target="_blank">
                                                                <%#Eval("OrderID") %></a>
                                                            <input type="hidden" runat="server" id="hidOrderId" />
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridTemplateColumn HeaderText="Refs">
                                                        <ItemTemplate>
                                                            Load No:
                                                            <%# Eval("CustomerOrderNumber") %><br />
                                                            Delivery Order No:
                                                            <%# Eval("DeliveryOrderNumber") %>
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn UniqueName="LoadNo" DataField="CustomerOrderNumber" HeaderText="Load No"
                                                        AllowFiltering="true" />
                                                    <telerik:GridBoundColumn UniqueName="CollectFrom" HeaderText="From" DataField="CollectionPointDescription" Visible="false">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridTemplateColumn UniqueName="CollectAt" HeaderText="Collect At" ItemStyle-Wrap="false"
                                                        HeaderStyle-Width="120px">
                                                        <ItemTemplate>
                                                            <div id="collectionTime">
                                                                <asp:RadioButtonList ID="rblCollectionTimeOptions" runat="server" EnableViewState="false"
                                                                    RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                    <asp:ListItem Text="W" Value="0"></asp:ListItem>
                                                                    <asp:ListItem Text="T" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="A" Value="2"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                                <br />
                                                                <asp:TextBox ID="txtCollectAtDate" runat="server" Width="60px" EnableViewState="false" />
                                                                <asp:TextBox ID="txtCollectAtTime" runat="server" Width="40px" EnableViewState="false" /><br />
                                                                <asp:TextBox ID="txtCollectByDate" runat="server" Width="60px" EnableViewState="false" />
                                                                <asp:TextBox ID="txtCollectByTime" runat="server" Width="40px" EnableViewState="false" />
                                                            </div>
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                    <telerik:GridBoundColumn UniqueName="DeliverTo" HeaderText="To" DataField="DeliveryPointDescription" Visible="false">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridTemplateColumn UniqueName="DeliverAt" HeaderText="Deliver At" ItemStyle-Wrap="false"
                                                        HeaderStyle-Width="120px">
                                                        <ItemTemplate>
                                                            <div id="deliveryTime">
                                                                <asp:RadioButtonList ID="rblDeliveryTimeOptions" runat="server" EnableViewState="false"
                                                                    RepeatDirection="Horizontal" RepeatLayout="Flow">
                                                                    <asp:ListItem Text="W" Value="0"></asp:ListItem>
                                                                    <asp:ListItem Text="T" Value="1"></asp:ListItem>
                                                                    <asp:ListItem Text="A" Value="2"></asp:ListItem>
                                                                </asp:RadioButtonList>
                                                                <br />
                                                                <asp:TextBox ID="txtDeliverAtFromDate" runat="server" Width="60px" EnableViewState="false" />
                                                                <asp:TextBox ID="txtDeliverAtFromTime" runat="server" Width="40px" EnableViewState="false" /><br />
                                                                <asp:TextBox ID="txtDeliverByFromDate" runat="server" Width="60px" EnableViewState="false" />
                                                                <asp:TextBox ID="txtDeliverByFromTime" runat="server" Width="40px" EnableViewState="false" />
                                                            </div>
                                                        </ItemTemplate>
                                                    </telerik:GridTemplateColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </asp:Panel>
                                </asp:Panel>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="3">
                        <asp:CheckBox ID="chkAlterLegsToReflectNewTimes" runat="server" Text="Alter planned times to match"
                            TextAlign="Left" Checked="true" />
			            (note: only takes effect for changes made in "Simple" mode)
                        <br />
                        If any of the booked date times are listed as AnyTime they will be planned to happen
                        up until 23:59 of that day, unless other instructions force them to occur earlier.
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
    <asp:HiddenField ID="hidSimpleInstructions" runat="server" />
    <asp:HiddenField ID="hidOrderIDs" runat="server" />
    <asp:HiddenField ID="hidInstructionIDs" runat="server" />
    <div class="buttonbar">
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
    <p>
        If there are multiple Orders on a Load or Drop, then the date that will be displayed on the Traffic Sheet will be the most restrictive. Therefore, a Timed Booking is more restrictive than a Booking Window.
    </p>
    <div id="divPointAddress" style="z-index: 5; display: none; background-color: Wheat;
        padding: 2px 2px 2px 2px;">
        <table style="background-color: white; border: solid 1pt black;" cellpadding="2">
            <tr>
                <td>
                    <span id="spnPointAddress"></span>
                </td>
            </tr>
        </table>
    </div>
    <telerik:RadInputManager ID="rimApproveOrder" runat="server">
        <telerik:DateInputSetting BehaviorID="DateInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" EmptyMessage="Enter a valid Date" DateFormat="dd/MM/yy" SelectionOnFocus="SelectAll"
            EnabledCssClass="DateControl" FocusedCssClass="DateControl_Focused" HoveredCssClass="DateControl_Hover"
            InvalidCssClass="DateControl_Error" DisplayDateFormat="dd/MM/yy">
        </telerik:DateInputSetting>
        <telerik:DateInputSetting BehaviorID="TimeInputBehaviour" InitializeOnClient="false  "
            Culture="en-GB" EmptyMessage="Anytime" DateFormat="HH:mm" DisplayDateFormat="HH:mm"
            EnabledCssClass="TimeControl" FocusedCssClass="TimeControl_Focused" HoveredCssClass="TimeControl_Hover"
            InvalidCssClass="TimeControl_Error">
        </telerik:DateInputSetting>
        <telerik:NumericTextBoxSetting BehaviorID="NumericInputBehaviour" InitializeOnClient="false"
            Culture="en-GB" Type="Currency" DecimalDigits="2" MinValue="0" EnabledCssClass="TextControl"
            FocusedCssClass="TextControl_Focused" HoveredCssClass="TextControl_Hover" InvalidCssClass="TextControl_Error">
        </telerik:NumericTextBoxSetting>
    </telerik:RadInputManager>

    <script type="text/javascript">

        //Global variables used in the code behind
        var dteDateTime = null;
        var dteCollectionByDate = null;
        var rdCollectionBookingWindow = null;
        var dteCollectionFromDate = null;
        var dteCollectionByTime = null;
        var dteCollectionFromTime = null;
        var dteDeliveryFromTime = null;
        var dteDeliveryByTime = null;
        var dteDeliveryByDate = null;
        var dteDeliveryFromDate = null;
        var rdDeliveryBookingWindow = null;

        var timerID = null;
        function pageLoad() {


            $(':radio[id *= rblCollectionTimeOptions]').bind("click", ChangeCollectionTiming);
            $(':radio[id *= rblDeliveryTimeOptions]').bind("click", ChangeDeliveryTiming);

            $(':text[id*=txtCollectByDate]').bind("change", setDirty);
            $(':text[id*=txtCollectByTime]').bind("change", setDirty);
            $(':text[id*=txtCollectAtDate]').bind("change", setDirty);
            $(':text[id*=txtCollectAtTime]').bind("change", setDirty);

            $(':text[id*=txtDeliverByFromDate]').bind("change", setDirty);
            $(':text[id*=txtDeliverByFromTime]').bind("change", setDirty);
            $(':text[id*=txtDeliverAtFromDate]').bind("change", setDirty);
            $(':text[id*=txtDeliverAtFromTime]').bind("change", setDirty);

            timerID = window.setTimeout(InitialiseOrchestratorPage, 300);
        }
   
    </script>

    <script language="javascript" type="text/javascript">
        <!--
        $(document).ready(function() {
            //  var oTable = $('#BookedTimes').dataTable();
            //$.fn.dataTableExt.FixedHeader(oTable);
            dirtyorderIDs = "";
            dirtyInstructionIDs = "";

            timerID = window.setTimeout(InitialiseOrchestratorPage, 300);
        })
        //-->

        
    </script>

</asp:Content>
