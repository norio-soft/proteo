<%@ Page Language="C#" AutoEventWireup="True" Inherits="Orchestrator.WebUI.Traffic.JobManagement.addupdatepallethandling" Codebehind="addupdatepallethandling.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="uc1" TagName="point" Src="~/usercontrols/point.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <style type="text/css">
        .toolbar li {
        display:inline;
        background-color:#eee;
        border:1px solid;
        border-color:#f3f3f3 #bbb #bbb #f3f3f3;
        margin:0;
        padding:2px;
        }
        
        .orderedList 
        {
            margin-left:0px;
            list-style-type:none;
            padding-left:0px !important;
        }
        
        .orderedList li
        {
        	margin:0;
        	padding:2px;
        	list-style-type:none;
        }
        
        .orderedListSelected
        {
        	margin:0;
        	padding:2px;
        	background-color:#FFADAD;
        	list-style-type:none;
        }
        
        .inlineList
        {
        	display:inline;
        	margin-left:15px;
            list-style-type:none;
        }
        
        .inlineList li
        {
        	list-style-type:none;
        	display:inline block;
        	float:left;
        	margin:0;
        	padding:2px;
        }
    </style>    

    <link rel="stylesheet" type="text/css" href="/style/styles.css" />

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/tooltippopups.js" type="text/javascript"></script>
    
    <script type="text/javascript" language="javascript">
    <!--
        var resourceCheckBoxSelected = false;
        var isChecked = false;
        var allChecked = false;
        var table = null;
        
        $(document).ready(function() {
            table = $get('existingPalletHandling');
            
        });
        
        function realPostBack(eventTarget, eventArgument)
        {
            $find("<%= ramPalletHandling.ClientID %>").__doPostBack(eventTarget, eventArgument);
        }
        
        function PalletHandlingResourceSelected(chkResourceSelected)
        {
            var selectedDiv = $("#" + chkResourceSelected.id).closest("div");
            var allDivs = $(".PalletResourceWrapperClass");
            
            for (var i = 0; i < allDivs.length; i++)
            {
                if (allDivs[i].id != selectedDiv.attr("id") )
                {
                    var currentDiv = $("#" + allDivs[i].id);
                    currentDiv.find("*").prop("disabled", chkResourceSelected.checked);
                }
            }
                
            resourceCheckBoxSelected = chkResourceSelected.checked;
            DisplayButton(selectedDiv);
        }
        
        function UpdatePalletTotal(object, sender)
        {
            // Find the parent table of this numeric text box, then get all child textboxes for that table.
            var TextBoxes = $("#" + object.get_id()).closest("table").find("input:text:not(:hidden)");
            var currentCheckBox = $("#" +  object.get_id()).closest("div").find(":checkbox:not(:disabled)");
            var palletCount = 0;
            
            // Go through the text boxes and retrieve they're values to put in the footer control.
            for( var i = 0; i < TextBoxes.length; i++)
            {
                var currentValue = parseInt(TextBoxes[i].value, 10);
                var textBoxID = TextBoxes[i].id.substring(0, TextBoxes[i].id.indexOf("_text"));
                var inputBox = $find(textBoxID);
                
                if(!isNaN(currentValue))
                {
                    if(inputBox.get_minValue() <= currentValue)
                    {
                        if(currentValue > inputBox.get_maxValue())
                            currentValue = inputBox.get_maxValue();
                    }
                    else
                        currentValue = 0;
                    
                    palletCount += currentValue;
                }
                else
                   $("#" + TextBoxes[i].id).val(0);
            }
            
            var palletWrapperDiv = $("#" + object.get_id()).closest("div");
            palletWrapperDiv.find("#palletTotal").text(palletCount);
            resourceCheckBoxSelected = currentCheckBox.prop("checked");
            DisplayButton(palletWrapperDiv);
        }
        
        function toggleGroup(img, numberOfRows) {
            //  get a reference to the row and table
            var tr = img.parentNode.parentNode;
            var src = img.src;

            //  do some simple math to determine how many
            //  rows we need to hide/show
            var startIndex = tr.rowIndex + 1;
            var stopIndex = startIndex + parseInt(numberOfRows);

            //  if the img src ends with plus, then we are expanding the
            //  rows.  go ahead and remove the hidden class from the rows
            //  and update the image src
            if (src.endsWith('topItem_exp.gif')) {
                for (var i = startIndex; i < stopIndex; i++) {
                    Sys.UI.DomElement.removeCssClass(table.rows[i], 'hidden');
                }

                src = src.replace('topItem_exp.gif', 'topItem_col.gif');
            }
            else {
                for (var i = startIndex; i < stopIndex; i++) {
                    Sys.UI.DomElement.addCssClass(table.rows[i], 'hidden');
                }

                src = src.replace('topItem_col.gif', 'topItem_exp.gif');
            }

            //  update the src with the new value
            img.src = src;
        }
    
        function DisplayButton(palletWrapperDiv)
        {
            var palletButtonBar = $("#" + "<%=palletButtonBar.ClientID %>");
            var btnCancelUpdate = $("#" + "<%=btnCancelUpdate.ClientID %>");
            var btnGenerate = $("#" + "<%=btnGenerate.ClientID %>");
            var totalPallets = parseInt($("#" + palletWrapperDiv.attr("id")).find("#palletTotal").text(), 10);
            
            totalPallets > 0 && resourceCheckBoxSelected ? btnGenerate.show() : btnGenerate.hide();
            
            if( (btnGenerate.css("display") == "none") && (btnCancelUpdate.css("display") == "none") )
                palletButtonBar.css("display", "none");
            else
                palletButtonBar.css("display", "");
        }

         //Window Code
        function _showModalDialog(url, width, height, windowTitle)
        {
            MyClientSideAnchor.WindowHeight= height + "px";
            MyClientSideAnchor.WindowWidth= width + "px";
            
            MyClientSideAnchor.URL = url;
            MyClientSideAnchor.Title = windowTitle;
            var returnvalue = MyClientSideAnchor.Open();
            if (returnvalue == true)
            {
                document.all.Form1.submit();
	        }
            return true;	        
        }    

        function ShowPleaseWait()
        {
            document.getElementById("divPleaseWait").style.display="";
            return true;
        }
        
        function showDocketWindow(instructionId)
        {
            var qs = "instructionId=" + instructionId;
            <%=dlgDockets.ClientID %>_Open(qs);
        }
        
        function ShowLoadOrder(instructionID)
        {
            var qs = "instructionID=" + instructionID;
            <%=dlgLoadOrder.ClientID %>_Open(qs);
        }
    -->    
    </script>
    
    <script type="text/javascript" language="javascript">
        var rcbPalletHandlingAction = null;
        
        var leavePalletTypeID = <%=((int)Orchestrator.eInstructionType.LeavePallets) %>;
        var dehirePalletTypeID = <%=((int)Orchestrator.eInstructionType.DeHirePallets) %>;
        var userID = <%=((Page.User) as Orchestrator.Entities.CustomPrincipal).IdentityId %>;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var _skipCollectionDateCheck = false;
        
        $(document).ready(function() {
            $('#' + '<%=deHireRow.ClientID %>').hide();

            if ($('input:radio[id*=rdDeliveryTimedBooking]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').hide();

            if ($('input:radio[id*=rdDeliveryIsAnytime]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').hide();

            if ($('input:radio[id*=rdDeliveryBookingWindow]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').show();
                
            var rfvDeHireOrganisation = $("span[id*='rfvDeHireOrganisation']");
            ValidatorEnable(rfvDeHireOrganisation[0], false);
        });
        
        // This fires once the ASP.NET Ajax engine has initialised and is available.
        function pageLoad() {
            var deliveryMethod = $('input:hidden[id*=hidDeliveryTimingMethod]').val();
            
            rcbPalletHandlingAction = $find("<%=rcbPalletHandlingAction.ClientID %>");

            if (deliveryMethod == 'anytime') {
                $('input:radio[id*=rdDeliveryIsAnytime]')[0].checked = true;
                deliveryIsAnytime(null);
            }

            if (deliveryMethod == 'timed') {
                deliveryTimedBooking(null);
            }
            
            dehireSelectedChange();
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

            if (dteDeliveryFromDate != null) {
                if (sender.get_selectedDate() < dteDeliveryFromDate.get_selectedDate())
                    updateDate = true;
                else if (rdDeliveryBookingWindow != null && !rdDeliveryBookingWindow.prop("checked"))
                    updateDate = true;
            }

            if (updateDate)
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

        function rcbPalletHandlingAction_OnClientSelectedIndexChanged(sender, eventArgs) {
            dehireSelectedChange();
        }
        
        function dehireSelectedChange()
        {
            var deHireRow = $('#' + '<%=deHireRow.ClientID %>');
            var rfvDeHireOrganisation = $("span[id*='rfvDeHireOrganisation']");
        
            if (rcbPalletHandlingAction.get_value() == dehirePalletTypeID)
            {
                ValidatorEnable(rfvDeHireOrganisation[0], true);
                deHireRow.show();
            }
            else
            {
                ValidatorEnable(rfvDeHireOrganisation[0], false);
                deHireRow.hide();
            }
        }
        
        function pendingActionsWarning()
        {
            alert("There are currently pending actions that need to be removed before this action can be undone.");
        }
    
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Control Pallet Handling on Run</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgDockets" runat="server" URL="/Job/UpdateDockets.aspx" Width="620" Height="320" AutoPostBack="true" ReturnValueExpected="true" Mode="Modal" />
    <cc1:Dialog ID="dlgLoadOrder" runat="server" Mode="Modal" URL="/Groupage/LoadOrder.aspx" Width="800" Height="400" Scrollbars="true"></cc1:Dialog>
    
    <div class="MessagePanel" id="messagePanel" runat="server" style="display:none;">
        <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /><asp:Label ID="lblConfirmation" runat="server"></asp:Label>
    </div>

    <uc1:infringementDisplay runat="server" ID="idErrors" Visible="false" />
    
    <div style="float:left; width:475px;">
        <fieldset>
            <legend>Pallet Delivery</legend>
            <table border="0">
                <tr>
                    <td class="formCellLabel">Action</td>
                    <td colspan="2">
                        <telerik:RadComboBox ID="rcbPalletHandlingAction" runat="server" width="342px" ValidationGroup="grpAddPalletHandling" OnClientSelectedIndexChanged="rcbPalletHandlingAction_OnClientSelectedIndexChanged" />
                    </td>
                </tr>
                <tr id="deHireRow" runat="server">
                    <td class="formCellLabel">DeHire Pallets For</td>
                    <td colspan="2">
                        <telerik:RadComboBox ID="rcbDeHireOrganisation" runat="server" width="342px" ValidationGroup="grpAddPalletHandling" DataTextField="OrganisationName" DataValueField="IdentityId" EnableLoadOnDemand="true" ShowMoreResultsBox="false" Height="150px" Overlay="true" AllowCustomText="false" MarkFirstMatch="true" ItemRequestTimeout="500" />
                        <asp:RequiredFieldValidator ID="rfvDeHireOrganisation"  ValidationGroup="grpAddPalletHandling" runat="server" ControlToValidate="rcbDeHireOrganisation" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                            <img id="Img3" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from date." alt="" />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Destination</td>
                    <td colspan="2">
                        <uc1:point ID="ucDeliveryPoint" runat="server" CanClearPoint="true" EditMode="false" ValidationGroup="grpAddPalletHandling" IsDepotVisible="false" PointSelectionRequired="true" ShowFullAddress="true" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Deliver When
                    </td>
                    <td class="formCellField" colspan="2" >
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
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryFromDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteDeliveryFromDate_SelectedDateChanged" SelectionOnFocus="SelectAll" />
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryFromTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" SelectionOnFocus="SelectAll" />
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="grpAddPalletHandling" runat="server" ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                        <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from date." alt="" />
                                    </asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="grpAddPalletHandling" runat="server" ControlToValidate="dteDeliveryFromTime" Display="Dynamic" ErrorMessage="Please enter a delivery from time.">
                                        <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from time." alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel" style="width: 80px;">
                                    Deliver by:
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryByDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteDeliveryByDate_SelectedDateChanged" SelectionOnFocus="SelectAll" />
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDateInput Width="65" ID="dteDeliveryByTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00" SelectionOnFocus="SelectAll" />
                                </td>
                                <td class="formCellField">
                                    <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="grpAddPalletHandling" runat="server" ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                        <img id="Img10" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by date." alt="" />
                                    </asp:RequiredFieldValidator>
                                    <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="grpAddPalletHandling" runat="server" ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                        <img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by time." alt="" />
                                    </asp:RequiredFieldValidator>
                                    <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate" ValidationGroup="grpAddPalletHandling" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate" ErrorMessage="The date cannot be before today.">
                                        <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon" alt="" />
                                    </asp:CustomValidator>
                                    <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2" ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?" ValidationGroup="grpAddPalletHandling">
                                        <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date entered is far in the future." alt="warning icon" alt="" />
                                    </asp:CustomValidator>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">Charge</td>
                    <td colspan="2">
                        <telerik:RadNumericTextBox ID="rntPalletDeliveryCharge" runat="server" Type="Currency" ValidationGroup="grpAddPalletHandling" NumberFormat-DecimalDigits="2" MinValue="0" Width="50px" />
                        <asp:RequiredFieldValidator ID="rfvPDC" runat="server" ControlToValidate="rntPalletDeliveryCharge" ValidationGroup="grpAddPalletHandling"  Display="Dynamic" ErrorMessage="Please enter the value of the pallets to deliver." alt="">
                            <img id="Img1" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from date." alt="" />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </fieldset>
    </div>
    
    <div style="float:left; margin-left:10px; width:600px;">
        <telerik:RadCodeBlock ID="rcbEmptyPallets" runat="server">
            <div>
                <fieldset>
                    <legend>Returned Empty Pallets</legend>
                    <p>
                        Please select a resource to plan its pallet handling, then choose how many pallets you wish to plan.
                        <br />
                    </p>
                    
                    <asp:ListView ID="lvEmptyPallets" runat="server">
                        <LayoutTemplate>
                            <ul class="orderedList">
                                <li>
                                    <tr id="itemPlaceHolder" runat="server" />
                                </li>
                            </ul>
                        </LayoutTemplate>
                        <ItemTemplate>
                            <div id="PalletResourceWrapper" runat="server" class="PalletResourceWrapperClass">
                                <ul class="inlineList">
                                    <li style="margin-top:1px;">
                                        <h3><%# Eval("Resources.PalletContainer")%></h3>
                                        <asp:HiddenField ID="hdnResource" runat="server" Value='<%# Eval("Resources.PalletContainer") %>' />
                                    </li>
                                    <li>
                                        <asp:CheckBox ID="chkResourceSelected" runat="server" onclick="javascript:PalletHandlingResourceSelected(this);" style="margin-left:10px;" />
                                        <asp:HiddenField ID="hidResourceID" runat="server" Value='<%# Eval("Resources.ResourceID") %>' />
                                        <asp:HiddenField ID="hidIsFixedUnit" runat="server" Value='<%# Eval("Resources.IsFixedUnit") %>' />
                                    </li>
                                    <li>
                                        <asp:Panel ID="pnlUpdateResource" runat="server">
                                            <ul class="inlineList">
                                                <li><telerik:RadComboBox ID="rcbAlternateResources" runat="server" AutoPostBack="false" DataTextField="Value" DataValueField="Key" style="width:100px; margin-left:150px;" /></li>
                                                <li><asp:Button ID="btnPalletMove" runat="server" Text="Re-allocate Pallets" OnClick="btnPalletMove_Click" /></li>
                                            </ul>
                                        </asp:Panel>
                                    </li>
                                </ul>
                                
                                <div class="clearDiv"></div>
                                
                                <asp:ListView ID="lvItems" runat="server" DataSource='<%# Eval("Items") %>'>
                                    <LayoutTemplate>
                                        <table id="palletTypeTable">
                                            <thead>
                                                <tr>
                                                    <th>PalletType</th>
                                                    <th>Empty Pallets</th>
                                                    <th>Pallets Selected</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr id="itemPlaceHolder" runat="server" />
                                            </tbody>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr class="Row">
                                            <td class="formCellLabel" style="width:250px;">
                                                <%# ((System.Data.DataRow)Container.DataItem)["PalletType"].ToString() %>
                                                <asp:HiddenField ID="hdnPalletType" runat="server" Value='<%# ((System.Data.DataRow)Container.DataItem)["PalletType"].ToString() %>' />
                                                <asp:HiddenField ID="hdnPalletTypeID" runat="server" Value='<%# ((System.Data.DataRow)Container.DataItem)["PalletTypeID"].ToString() %>' />
                                            </td>
                                            <td class="formCellField" style="width:100px;" id="NoOfPalletsWrapper" runat="server">
                                                <asp:Label ID="lblNoOfPallets" runat="server" Text='<%# ((System.Data.DataRow)Container.DataItem)["NoOfPallets"].ToString() %>' />
                                                <asp:HiddenField ID="hdnNoOfPallets" runat="server" Value='<%# ((System.Data.DataRow)Container.DataItem)["NoOfPallets"].ToString() %>' />
                                            </td>
                                            <td class="formCellField" style="width:100px;" id="SelectedPalletsWrapper" runat="server">
                                                <telerik:RadNumericTextBox ID="rntNoOfPallets" Width="20" runat="server" Type="Number" NumberFormat-DecimalDigits="0" Value='<%# decimal.Parse(((System.Data.DataRow)Container.DataItem)["HandledPallets"].ToString()) %>' ClientEvents-OnValueChanged="UpdatePalletTotal" NegativeStyle-ForeColor="Red"  MaxValue='<%# int.Parse(((System.Data.DataRow)Container.DataItem)["NoOfPallets"].ToString()) %>' />
                                                <asp:RequiredFieldValidator ID="rfvNoOfPallets" runat="server" ControlToValidate="rntNoOfPallets" Display="Dynamic">
                                                    <img src="/images/error.png" alt="Please enter a number of pallets." />
                                                </asp:RequiredFieldValidator>
                                                <asp:HiddenField ID="hdnIdentifier" runat="server" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:ListView>
                                
                                <ul>
                                    <li>
                                        <div id="palletTotal" style="text-align:right; width:385px; ">
                                            <asp:Label ID="lblPalletTotal" runat="server" Font-Bold="true" style="border-top: solid 1 black;" Text="0" />
                                        </div>
                                    </li>
                                </ul>
                            </div>
                        </ItemTemplate>
                        <EmptyDataTemplate>
                            <div>
                                There are currently no empty pallets on this run.
                            </div>
                        </EmptyDataTemplate>
                    </asp:ListView>

                    <div class="buttonBar" id="palletButtonBar" runat="server" style="display:none;">
                        <asp:Button ID="btnGenerate" runat="server" Text="Generate" Width="75" ValidationGroup="grpAddPalletHandling"></asp:Button>
                        <asp:Button ID="btnCancelUpdate" runat="server" Text="Cancel" Width="75" style="display:none;" ></asp:Button>
                    </div>
                </fieldset>
            </div>
        </telerik:RadCodeBlock>
    </div>
    
    <div class="clearDiv"></div>
    
    <div style="width:1085px;">
        <fieldset>
            <legend>Existing Pallet Handling On Run</legend>
            <asp:ListView ID="lvExisingPalletHandling" runat="server" >
                <LayoutTemplate>
                    <table id="existingPalletHandling" cellpadding="0" cellspacing="0" style="width:100%;">
                        <thead class="HeadingRow">
                            <tr>
                                <th>OrderID</th>
                                <th>Action</th>
                                <th>No Of Pallets</th>
                                <th>Pallet Type</th>
                                <th>Destination</th>
                                <th>Delivery Date</th>
                                <th>Resource</th>
                                <th>Charge</th>
                                <th></th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr id="itemPlaceHolder" runat="server" />
                        </tbody>
                    </table>
                </LayoutTemplate>
                <ItemTemplate>
                    <tr class="row">
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).OrderID %></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).ToBeRemoved ? "Remove" : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).PalletActionDescription %></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).UpdatedNoPallets > 0 ? ((Orchestrator.Entities.PalletDelivery)Container.DataItem).UpdatedNoPallets.ToString() : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).NoOfPallets.ToString() %></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).ToBeRemoved ? "" : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).PalletType%></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).ToBeRemoved ? "" : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).Destination%></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).ToBeRemoved ? "" : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).DeliveryDateFormatted%></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).ToBeRemoved ? "" : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).Resource%></td>
                        <td><%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).ToBeRemoved ? "" : ((Orchestrator.Entities.PalletDelivery)Container.DataItem).Charge%></td>
                        <td>
                            <asp:LinkButton ID="lbSelect" runat="server" Text="Update" CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).Identifier %>' OnClick="lbSelect_Click" />
                            <span style="width:5px;" /> 
                            <asp:LinkButton ID="lbRemove" runat="server" Text="Remove" CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).Identifier %>' OnClick="lbRemove_Click" />
                            <span style="width:5px;" />
                            <asp:LinkButton ID="lbUndo" runat="server" Text="Undo" CausesValidation="false" CommandArgument='<%# ((Orchestrator.Entities.PalletDelivery)Container.DataItem).Identifier %>' OnClick="lbUndo_Click" />
                        </td>
                    </tr>
                </ItemTemplate>
                <EmptyDataTemplate>
                    There are currently no Pallet Handling Instructions on this Job.
                </EmptyDataTemplate>
            </asp:ListView>
        </fieldset>
    </div>
    
    <div class="buttonbar" style="width:1075px;">
        <asp:Button ID="btnUpdate" runat="server" Text="Update" CausesValidation="false" />
        <asp:Button ID="btnGenerateUpdate" runat="server" Text="Generate & Update" ValidationGroup="grpAddPalletHandling" />
        <asp:Button ID="btnCancel" runat="server" Text="Close" Width="75" CausesValidation="False" />
    </div>
        
    <div id="divPointAddress" style="z-index=5; display:none; background-color:Wheat; padding:2px 2px 2px 2px;">
	    <table style="background-color: white; border:solid 1pt black;" cellpadding="2">
		    <tr>
			    <td><span id=""></span></td>
		    </tr>
	    </table>
    </div>
	    
    <div style="position:absolute; width:100%; left:0px; top:10px; height:300px; display:none; text-align:center;" id="divPleaseWait">
        <table width="465" cellspacing="0" cellpadding="0" border="1" style="height:480; background-color:#ffffff; border-collapse:collapse;border:solid 1pt black;">
            <tr>
                <td align="center">
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td style="font-size:10px;">Please wait Configuring Pallet Handling... </td>
                            <td><img src="/images/spinner.gif" width="16" height="16" border="0"></td>
                        </tr>
                    </table>      
                </td>
            </tr>
        </table>
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
    
    <telerik:RadAjaxManager ID="ramPalletHandling" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="pnlExistingPalletHandling">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lvEmptyPallets" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
</asp:Content>