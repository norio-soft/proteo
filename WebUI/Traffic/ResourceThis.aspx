<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.ResourceThis" EnableEventValidation="false" MasterPageFile="~/WizardMasterPage.Master" CodeBehind="ResourceThis.aspx.cs" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script language="javascript" type="text/javascript" src="/script/scripts.js" ></script>
    <script language="javascript" type="text/javascript" src="/script/popAddress.js"></script>
    <script language="javascript" type="text/javascript" src="/script/tooltippopups.js"></script>
    <script language="javascript" type="text/javascript" src="/script/ajaxwindowscripts.js"></script>
    <script language="javascript" type="text/javascript" src="/script/jquery.fixedheader.js"></script>
    
    <script type="text/javascript" language="javascript">

        

    <!--
        //Client Side Fields
        var HasChanges, inputs, lastChanged, editedRow;
        var subbedLegCheckedCount = 0;
        
        var driverTempValue = '';
        var vehicleTempValue = '';
        
        var chkAllLegs = null;
        var chkCommAllLegs = null;

        //////////////////////////////////////////////////////////////////////////////////////////
        // Fix up combo box height when rendering as disabled.
        // see: : http://www.telerik.com/support/kb/article/b454K-mgh-b454T-bmc-b454c-bmc.aspx
        // var combo =<%=cboDriver.ClientID%>;
        // combo.FixUp(combo.InputDomElement,true);
        // combo =<%=cboVehicle.ClientID%>;
        // combo.FixUp(combo.InputDomElement,true);
        //////////////////////////////////////////////////////////////////////////////////////////
        
        var preLoadedDriver = <%= driverID %>;
        var preLoadedVehicle = <%= vehicleID %>;
        var preLoadedTrailer = <%= trailerID %>;

        var checkResourceAvailable = false;
            
        <% if(resourceIsAvailableCheck){ %>
        checkResourceAvailable= true;
            <%}%>
        function ajaxRequestEnd(sender, eventsArgs)
        {
            if($(".infringmentDisplayWrapper")[0] != null)
            {
                ShowUpdateLegs();
            }
        }
        
        function disableTrailer(sender, eventArgs)
        {
           var cboTrailer = $find("<%= cboTrailer.ClientID %>");
           cboTrailer.disable();
        }
                      
        var preselectedresourceloaded = false;
        $(document).ready(function() {
            checkForSubbedLegs();

            var legPlanHeight = $('#Legs').height() + 2;

            if (legPlanHeight > 250)
                legPlanHeight = 250;

            $("#Legs").fixedHeader({
                width: $('#Legs').width(),
                height: legPlanHeight
            });

            $("#pcvs").fixedHeader({
                width: $('#pcvs').width(),
                height: 100
            });

            $("#refusedGoods").fixedHeader({
                width: $('#refusedGoods').width(),
                height: 100
            });

            $("#" + "<%=btnCancel.ClientID%>").focus();

            chkAllLegs = $("input:checkbox[id*='chkAllLegs']");
            chkCommAllLegs = $("input:checkbox[id*='chkCommAllLegs']");

            if ($('.chkLegSelect :checkbox:enabled').length == 0)
                chkAllLegs.prop('disabled', true);

            var existingDrivers = $("span[id*=lblDriver][HasContent=true]");
            if (existingDrivers.length > 0)
                existingDrivers.parent().parent().find('input:checkbox[id*=chkCommunicateLeg]').prop('disabled', false);

            if ($('.chkCommLeg :checkbox:enabled').length == 0) {
                chkCommAllLegs.prop('disabled', true);
                $('.chkCommLeg :checkbox').prop("disabled", true);
            }

            if ($('.chkLegSelect :checkbox:enabled').length == $('.chkLegSelect :checkbox:enabled:checked').length)
                chkAllLegs.prop('checked', true);

            if ($('.chkCommLeg :checkbox:enabled').length == $('.chkCommLeg :checkbox:enabled:checked').length)
                chkCommAllLegs.prop('checked', true);

            ShowUpdateLegs();

            if ("<%=PendingUpdates %>" == "True")
                $('#' + "<%=btnConfirm.ClientID %>").show()
            else
                $('#' + "<%=btnConfirm.ClientID %>").hide()
        });
        
        function setFocus() {
            // Sets the focus to the first empty input.
            $('#tblInputs input:visible').filter(function() {
                if ($(this).val() == '') {
                   return true;
                }
            }).first().focus();
        }

        function openAlterPalletBalance(jobID, palletTypeID, palletCount, resourceID) {
            var qs = "JobId=" + jobID + "&rID=" + resourceID + "&PTId=" + palletTypeID + "&noP=" + palletCount;
            <%=dlgAlterPalletBalance.ClientID %>_Open(qs);
        }
        
        function openGoodsRefusal(refusalID, instructionID) {
            var qs = "RefusalID=" + refusalID + "&InstructionId=" + instructionID + "&isWindowed=1";
            <%=dlgGoodsRefused.ClientID %>_Open(qs);
        }
        
        function openGoodsJob(jobID, instructionID) {
            //var url = "/Traffic/JobManagement/DriverCallIn/tabReturns.aspx?wiz=true&jobId=" + jobID + "&instructionId=" + instructionID;
            //openResizableDialogWithScrollbars(url, 1024, 720);
            var qs = "wiz=true&jobId=" + jobID + "&instructionId=" + instructionID;
            <%=dlgGoodsRefusalJob.ClientID %>_Open(qs);
        }
        
        function openJob(jobID)
        {
            var qs = "wiz=true&jobId=" + jobID;
            <%=dlgJob.ClientID %>_Open(qs);
        }
        
        function checkForSubbedLegs()
        {
            var hidNumberOfSelectedLegsSubbedOut = $get("<%=hidNumberOfSelectedLegsSubbedOut.ClientID %>");
            subbedLegCheckedCount = hidNumberOfSelectedLegsSubbedOut.value;

            // You cannot assign driver and vehicle resources to subbed out legs, so if any subbed out legs are checked
            // on page load - disable the driver and vehicle combos.
            if(subbedLegCheckedCount > 0)
            {
                // Disable the driver and vehicle combo
                //THIS DOESNT WORK AS THE OBJECTS HAVE NOT BEEN CREATED YET
                var _cboDriver = $find("<%= cboDriver.ClientID %>");
                var _cboVehicle = $find("<%= cboVehicle.ClientID %>");
                
                _cboDriver.set_emptyMessage("-Unavailable-");
                _cboVehicle.set_emptyMessage("-Unavailable-");
                
                _cboDriver.disable();                        
                _cboVehicle.disable();
            }
        }

        //Is this used?
        function subbedLegCheckedChange(chkBox)
        {
            // This function ensures that the driver and vehicle combo boxes are
            // disabled whenever a leg with a subby on it is selected. The reason for 
            // this is that subbed legs cannot have driver and vehicle resources assigned to them.
                            
            if (chkBox != null)
            {
                if (chkBox.checked == true) 
                {
                    driverTempValue = _cboDriver.get_value();
                    vehicleTempValue = _cboVehicle.get_value();
                    
                    // Disable the driver and vehicle combo
                    
                    _cboDriver.set_emptyMessage("-Unavailable-");
                    _cboVehicle.set_emptyMessage("-Unavailable-");

                    _cboDriver.clearSelection();
                    _cboVehicle.clearSelection();
                    
                    _cboDriver.disable();
                    _cboVehicle.disable();
                    
                    // Increment the count of legs with subby's that are checked
                    subbedLegCheckedCount++;
                    
                    // Finally, check the text box.
                    chkBox.checked = true;
                }
                else
                {
                    // Enable the driver and vehicle combo
                    if(subbedLegCheckedCount > 0)
                    {
                        // If the count is greater than zero, decrement the number of 
                        // legs with subby's on (that are checked) by one.
                        subbedLegCheckedCount--;    
                    }
                    
                    if(subbedLegCheckedCount == 0)
                    {
                        // There are no legs with subby's on that are checked. It is
                        // therefore ok to re-enable the driver and vehicle combos.
                        _cboDriver.enable();                        
                        _cboVehicle.enable();
                        
                        // Restore the text values of both combos back to what they were
                        // before a subbed leg was selected.
                        _cboDriver.set_value(driverTempValue);
                        _cboVehicle.set_value(vehicleTempValue);    
                    }
                    
                    // Finally, uncheck the text box.
                    chkBox.checked = false;
                }
            }
        }
        
        //Is this used?
        function callUpdate(id, index)
        {
            updateRecord(index);
        }

        //Is this used?
        function updateRecord(index)
        {
            var ctlId = index;// + 2;
            if (ctlId.toString().length == 1)
                ctlId = "0" + ctlId;
            var VehicleCtlName =  "radGrid1_ctl01_ctl" + ctlId + "_cboVehicle";
            var DriverCtlName =  "radGrid1_ctl01_ctl" + ctlId + "_cboDriver";
            var TrailerCtlName =  "radGrid1_ctl01_ctl" + ctlId + "_cboTrailer";
                        
            var _cboDriver  = null;
            var _cboVehicle = null;
            var _cboTrailer = null;
           
            _cboDriver  = eval(DriverCtlName);
            _cboVehicle = eval(VehicleCtlName);
            _cboTrailer = eval(TrailerCtlName);
                                                                       
            var updateParamaters = "Update:" + _cboDriver.get_value() + ":" + _cboVehicle.get_value() + ":" + _cboTrailer.get_value();
            alert(updateParamaters);
        }
        
        //Is this used?
        function GetCurrentElement(e)
        {
            if(!e)
                var e = window.event;

            if (e.srcElement)
            {
                return e.srcElement;
            }

            if(e.target)
            {
                return e.target;
            }
        }


        function cboDriver_OnClientSelectedIndexChanged(sender, eventArgs) 
        {
            if(checkResourceAvailable)
            {
                if(preLoadedDriver == -1)
                {
                    var reID = sender.get_value();
                    var jID = "<%= JobId %>";
                    var startD = new Date(<%= startDate %>);
                    var endD = new Date(<%= endDate %>);
                    $.ajax({
                        dataType: "json",
                        url: '/Api/ResourceIsAvailable',
                        data: {resourceID: reID, jobID:jID, fromDate: startD.toJSON(), toDate: endD.toJSON()}

                    }).done(function(data){
                        if(data)
                        {
                            $('#<%=driverResourceError.ClientID%>').html("");
                        }
                        else
                        {
                            $('#<%=driverResourceError.ClientID%>').html("The selected driver has already been resourced on this day.<br/>");
                        }
                    });
                }
                else
                {
                    preLoadedDriver = -1;
                }
            }
            if(chkCommAllLegs == null)
                chkCommAllLegs = $("input:checkbox[id*='chkCommAllLegs']");
        
            var item = eventArgs.get_item();
            if (item != null && item.get_text() != "" && item.get_value() != "-1")
            {
                if(chkCommAllLegs == null)
                {
                    chkCommAllLegs.prop('checked', false);
                    chkCommAllLegs.prop('disabled', false); 
                }
                
                $('.chkLegSelect :checkbox:enabled:checked').parent().parent().parent().find('input:checkbox[id*=chkCommLeg]')
                    .prop('disabled', false);

                LoadUsualVehicle();
            }
            else
            {
                if (chkCommAllLegs != null)
                    chkCommAllLegs.prop('disabled', true);
                
                $('.chkLegSelect :checkbox:enabled:checked').parent().parent().parent().find('input:checkbox[id*=chkCommLeg]')
                    .prop('checked', false)
                    .prop('disabled', true);
            }
        }
        
        function ValidateClientSideClosing(item)
        {
            if (item != null)
                if (item.get_text().length > 0 && item.get_value().length <1)
                    return false;
        }
       
        var pickingUsualVehicle = false;

        function LoadUsualVehicle() 
        {
           
            var _cboVehicle = $find("<%= cboVehicle.ClientID %>");
            
            pickingUsualVehicle = true;
            
            _cboVehicle.trackChanges();
            _cboVehicle.clearSelection(); 
            _cboVehicle.clearSelection();   
            _cboVehicle.clearItems();
            _cboVehicle.Text = "";
            _cboVehicle.requestItems("", false);
            _cboVehicle.commitChanges();             
        }

        function cboDriver_OnClientItemsRequesting(sender, eventArgs) 
        {
            var context = eventArgs.get_context();
            context["FilterString"] = "<%=GetClientDataString()%>";
        }

        function cboTrailer_OnClientItemsRequesting(sender, eventArgs)
        {
            var context = eventArgs.get_context();
            context["FilterString"] = "<%=GetClientDataString()%>";
        }
        
        function cboTrailer_OnClientTextChange(sender, eventArgs)
        {
            if(sender.get_originalText() != sender.get_text() && sender.get_value() == "")
            {
                sender.set_originalText(sender.get_text());
                var comboItem = new Telerik.Web.UI.RadComboBoxItem();
        
                comboItem.set_text(sender.get_text());
                comboItem.set_value("-2");
                
                sender.trackChanges();
                sender.clearSelection();   
                sender.clearItems();
                sender.get_items().add(comboItem);
                comboItem.select();
                sender.commitChanges();
                
                triggerThirdPartyTrailerNotification(sender);                
            }
        }
        
        function cboTrailer_OnClientBlur(sender, eventArgs)
        {
            triggerThirdPartyTrailerNotification(sender);
        }
        
        function cboVehicle_OnClientSelectedIndexChanged(sender, eventArgs)
        {
            if(checkResourceAvailable)
            {
                if(preLoadedVehicle == -1)
                {
                    var reID = sender.get_value();
                    var jID = "<%= JobId %>";
                    var startD = new Date(<%= startDate %>);
                    var endD = new Date(<%= endDate %>);
        $.ajax({
            dataType: "json",
            url: '/Api/ResourceIsAvailable',
            data: {resourceID: reID, jobID:jID, fromDate: startD.toJSON(), toDate: endD.toJSON()}

        }).done(function(data){
            if(data)
            {
                $('#<%=vehicleResourceError.ClientID%>').html("");
            }
            else
            {
                $('#<%=vehicleResourceError.ClientID%>').html("The selected vehicle has already been resourced on this day.<br/>");
            }
        });
                }
                else
                {
                    preLoadedVehicle = -1;
                }
            }
        }

        function cboTrailer_OnClientSelectedIndexChanged(sender, eventArgs)
        {
            if(checkResourceAvailable)
            {
                if(preLoadedTrailer == -1)
                {
                    var reID = sender.get_value();
                    var jID = "<%= JobId %>";
                    var startD = new Date(<%= startDate %>);
                    var endD = new Date(<%= endDate %>);
        $.ajax({
            dataType: "json",
            url: '/Api/ResourceIsAvailable',
            data: {resourceID: reID, jobID:jID, fromDate: startD.toJSON(), toDate: endD.toJSON()}

        }).done(function(data){
            if(data)
            {
                $('#<%=trailerResourceError.ClientID%>').html("");
            }
            else
            {
                $('#<%=trailerResourceError.ClientID%>').html("The selected trailer has already been resourced on this day.");
            }
        });
                }
                else
                {
                    preLoadedTrailer = -1;
                }
            }
            triggerThirdPartyTrailerNotification(sender);

            var item = eventArgs.get_item();
            var resourceId = item.get_value();
            var qs = "trailer|" + resourceId;
            
            var ramResourceThis = $find("<%=ramResourceThis.ClientID %>");
            ramResourceThis.ajaxRequest(qs);
        }
        
        function triggerThirdPartyTrailerNotification(sender)
        {
            var thirdPartyTrailerNote = $('#thirdPartyTrailerNote');
            var trThirdPartyOrganisation = $('#' + '<%=trThirdPartyOrganisation.ClientID %>');
            var rfvThirdPartyTrailerOwner = $get('<%=rfvThirdPartyTrailerOwner.ClientID %>');
            
            if(sender.get_value() == "-2")
            {
                thirdPartyTrailerNote.show();
                trThirdPartyOrganisation.show();
                ValidatorEnable(rfvThirdPartyTrailerOwner, true);
            }
            else
            {
                thirdPartyTrailerNote.hide();
                trThirdPartyOrganisation.hide();
                ValidatorEnable(rfvThirdPartyTrailerOwner, false);
            }
        }

        function cboVehicle_OnClientItemsRequesting(sender, eventArgs) 
        {
            
            var _cboDriver = $find("<%=cboDriver.ClientID %>");
            var _cboVehicle = $find("<%=cboVehicle.ClientID %>");
            var context = eventArgs.get_context();
            //Pass the selected Driver Id if we are picking the usual vehicle.
            //pickingUsualVehicle = false;
            if (pickingUsualVehicle)
            {
                context["FilterString"] = "true" + ":" + _cboDriver.get_value();
                }
            else
            {
                context["FilterString"] = "<%=GetClientDataString()%>";
                }
        }

        function cboVehicle_OnClientItemsRequested(sender, eventArgs) 
        {
            var items = sender.get_items();

            
                if (pickingUsualVehicle)
            {
                if (items.get_count() > 0) 
                    items.getItem(0).select();

                // Only set focus if picking usual vehicle.
                // setFocus();
            }
        
            pickingUsualVehicle = false;
        }
        
        function closeCheckForPendingUpdates()
        {
            var hdnUpdatesPending = $('#' + "<%=hdnUpdatesPending.ClientID %>");
            
            if(hdnUpdatesPending.val().toLowerCase() == "true")
            {
                var result = confirm("There are currently pending changes that will be lost if you close this window, do you wish to continue?");
                if(result)
                    return true;
                else
                    return false;
            }
            else
                return true;
        }
        
        function chkHeaderSelectedAll(chkHeaderSelect, checkBoxClass)
        {
            var filterString = chkHeaderSelect.checked ? ":not(checked)" : ":checked";
            $('.' + checkBoxClass + ' :checkbox:enabled' + filterString).prop("checked", chkHeaderSelect.checked);
             
            ShowUpdateLegs();
        }
        
        function chkLegSelected(chkLegSelect, checkBoxClass)
        {
            if(chkAllLegs == null)
                chkAllLegs = $("input:checkbox[id*='chkAllLegs']");
        
            if(chkCommAllLegs == null)
                chkCommAllLegs = $("input:checkbox[id*='chkCommAllLegs']");
        
            var chkHeader = checkBoxClass == "chkCommLeg" ? chkCommAllLegs : chkAllLegs;
            
            var legs = $('.' + checkBoxClass + ' :checkbox:enabled').length;
            var checkedLegs = $('.' + checkBoxClass + ' :checkbox:enabled:checked').length;

            ShowUpdateLegs();
            
            if(!chkLegSelect.checked || checkedLegs == 0)
                chkHeader.prop("checked", false);
                
            if(legs == checkedLegs)
                chkHeader.prop("checked", true);
        }
        
        function SelectUnassigned(comboSelector)
        {
            var comboId = "";
            
            switch(comboSelector)
            {
                case 0:
                    comboId = <%=cboDriver.ClientID %>;
                    break;
                case 1:
                    comboId = <%=cboVehicle.ClientID %>;
                    break;
                case 2:
                    comboId = <%=cboTrailer.ClientID %>;
                    break;
            }
            
            var combo = $find(comboId.id);
            var comboItem = new Telerik.Web.UI.RadComboBoxItem();
            
            combo.clearSelection();   
            combo.clearItems();
            
            comboItem.set_text("Unassign");
            comboItem.set_value("-1");
            
            combo.trackChanges();
            combo.get_items().add(comboItem);
            comboItem.select();
            combo.commitChanges();     
            
            return false;
        }
        
        function SelectClear(comboSelector)
        {
            var comboId = "";
            
            switch(comboSelector)
            {
                case 0:
                    comboId = <%=cboDriver.ClientID %>;
                    break;
                case 1:
                    comboId = <%=cboVehicle.ClientID %>;
                    break;
                case 2:
                    comboId = <%=cboTrailer.ClientID %>;
                    $('#' + "<%=lblTrailerDescription.ClientID %>").text('');
                    $('#' + "<%=pnlTrailerContents.ClientID %>").hide();                    
                    break;
            }
            
            var combo = $find(comboId.id);
            /*var comboItem = new Telerik.Web.UI.RadComboBoxItem();
            
            comboItem.set_text("");
            comboItem.set_value("");*/
            
            combo.trackChanges();
            /*combo.get_items().add(comboItem);
            comboItem.select();*/
            combo.clearSelection();   
            combo.clearItems();
            combo.Text = "";
            combo.commitChanges();
            return false;
        }
        
        function PreSelectedResource()
        {
            if (preselectedresourceloaded)
                return;
            
            if ('<%=this.DriverResourceId %>' != '0')
            {
                 var cboDriver = $find("<%=cboDriver.ClientID %>");
                 var cbiDriver = new Telerik.Web.UI.RadComboBoxItem();
            
                cbiDriver.set_text("<%=this.Driver %>");
                cbiDriver.set_value("<%=this.DriverResourceId %>");
                
                cboDriver.trackChanges();
                cboDriver.get_items().add(cbiDriver);
                cbiDriver.select();
                cboDriver.commitChanges();
            }
            if ('<%=this.VehicleResourceId %>' != '0')
            {
                 var cboVehicle = $find("<%=cboVehicle.ClientID %>");
                 var cbiVehicle = new Telerik.Web.UI.RadComboBoxItem();
            
                cbiVehicle.set_text("<%=this.RegNo %>");
                cbiVehicle.set_value("<%=this.VehicleResourceId %>");
                
                cboVehicle.trackChanges();
                cboVehicle.get_items().add(cbiVehicle);
                cbiVehicle.select();
                cboVehicle.commitChanges();
            }
            if ('<%=this.TrailerResourceId %>' != '0')
            {
                 var cboTrailer = $find("<%=cboTrailer.ClientID %>");
                 var cbiTrailer = new Telerik.Web.UI.RadComboBoxItem();
            
                cbiTrailer.set_text("<%=this.TrailerRef %>");
                cbiTrailer.set_value("<%=this.TrailerResourceId %>");
                
                cboTrailer.trackChanges();
                cboTrailer.get_items().add(cbiTrailer);
                cbiTrailer.select();
                cboTrailer.commitChanges();
            }
            
            preselectedresourceloaded = true;
            setFocus();
        }
        
        function ShowUpdateLegs()
        {
            var checkedSelectedLegs = $('.chkLegSelect :checkbox:enabled:checked').length;
            var checkedCommunicatedLegs = $('.chkCommLeg :checkbox:enabled:checked').length;
            var showButtonBar = (checkedCommunicatedLegs != 0 || checkedSelectedLegs != 0);
            
            var btnUpdateRows = $('#' + '<%=btnUpdateAndConfirm.ClientID %>');
        
            if(showButtonBar && $(".infringmentDisplayWrapper").css("display") == "none")
            {
                //$("#updateRowContainer").css("display", "");
                $("#updateRowContainer").show();
                btnUpdateRows.show();
            }
            else
            {
                //$("#updateRowContainer").css("display", "none");
                $("#updateRowContainer").hide();
                btnUpdateRows.hide();
            }
        }
        
        
        // Load the Traffic Areas based on the Control Area.
    //-->
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Resource This</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgJob" runat="server" URL="/job/Job.aspx" Height="720" Width="1024" Mode="Normal" AutoPostBack="false" ReturnValueExpected="false" UseCookieSessionID="true"/>
    <cc1:Dialog ID="dlgGoodsRefusalJob" runat="server" URL="/Traffic/JobManagement/DriverCallIn/tabReturns.aspx" Height="720" Width="1024" Mode="Normal" AutoPostBack="false" ReturnValueExpected="false" UseCookieSessionID="true"/>
    <cc1:Dialog ID="dlgAlterPalletBalance" runat="server" URL="/Pallet/AlterPalletBalance.aspx" Width="600" Height="700" Mode="Normal" ReturnValueExpected="true" AutoPostBack="true" />
    <cc1:Dialog ID="dlgGoodsRefused" runat="server" URL="/GoodsRefused/addupdategoodsrefused.aspx" Width="900" Height="700" Mode="Modal" ReturnValueExpected="true" AutoPostBack="true" />
    
    <asp:ScriptManagerProxy runat="server">
        <Services>
        <asp:ServiceReference Path="/ws/resourcethis.asmx" />
        </Services>
    </asp:ScriptManagerProxy>
    <asp:HiddenField ID="hdnUpdatesPending" runat="server" />  
    <div>
        <div style="float:left; width:450px;">
            <fieldset style="min-height: 196px">
                <h3>Select Resources</h3>
                <div style="color:red">
                    <asp:Label ID="driverResourceError" runat="server"></asp:Label>
                    <asp:Label ID="vehicleResourceError" runat="server"></asp:Label>
                    <asp:Label ID="trailerResourceError" runat="server"></asp:Label>
                </div>
                <table id="tblInputs" order="0" style="width:100%;">
                    <tr valign="top">
                        <td class="formCellLabel" style="width:25%;">
                            Driver
                        </td>
                        <td class="formCellInput" style="width:75%;">
                            <div style="display:inline;">
                                <telerik:RadComboBox ID="cboDriver" runat="server" EnableLoadOnDemand="True" ItemRequestTimeout="500"
                                    OnClientSelectedIndexChanged="cboDriver_OnClientSelectedIndexChanged" OnClientDropDownClosing="ValidateClientSideClosing" 
                                    OnClientItemsRequesting="cboDriver_OnClientItemsRequesting" DataTextField="Description" DataValueField="ResourceId"
                                    MarkFirstMatch="true" ShowMoreResultsBox="True" AllowCustomText="true" Width="155px" Overlay="true" Height="250px" EnableVirtualScrolling="False" ItemsPerRequest="-1" EnableAutomaticLoadOnDemand="False" AutoPostBack="false"/>
                            </div>
                            <div style="display:inline; margin-left:10px; width:100%;">
                                <asp:Button ID="btnDriverUnassign" runat="server" Text="Unassign" OnClientClick='if(!SelectUnassigned(0)) return false;' />
                                <input type="button" id="btnDriverClear" value="Clear" onclick="if(!SelectClear(0)) return false;" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">
                            Vehicle
                        </td>
                        <td class="formCellInput">
                            <div style="display:inline;">
                                <telerik:RadComboBox ID="cboVehicle" runat="server" EnableLoadOnDemand="True" ItemRequestTimeout="500"
                                    OnClientItemsRequested="cboVehicle_OnClientItemsRequested" OnClientDropDownClosing="ValidateClientSideClosing"
                                    OnClientItemsRequesting="cboVehicle_OnClientItemsRequesting" DataTextField="Description" DataValueField="ResourceId"
                                    MarkFirstMatch="true" ShowMoreResultsBox="True" AllowCustomText="true" Width="155px" Overlay="true" Height="250px" EnableVirtualScrolling="False" ItemsPerRequest="-1" EnableAutomaticLoadOnDemand="False" AutoPostBack="false" OnClientSelectedIndexChanged="cboVehicle_OnClientSelectedIndexChanged" />
                            </div>
                            <div style="display:inline; margin-left:10px; width:100%;">
                                <asp:Button ID="btnVehicleUnassign" runat="server" Text="Unassign" OnClientClick='if(!SelectUnassigned(1)) return false;' />
                                <input type ="button" id="btnVehicleClear" value="Clear" onclick="if(!SelectClear(1)) return false;" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellLabel">
                            Trailer
                        </td>
                        <td class="formCellInput">
                            <div style="display:inline;">
                                <telerik:RadComboBox ID="cboTrailer" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" AutoPostBack="false"
                                    OnClientItemsRequesting="cboTrailer_OnClientItemsRequesting" OnClientDropDownClosing="ValidateClientSideClosing"
                                    OnClientTextChange="cboTrailer_OnClientTextChange" OnClientBlur="cboTrailer_OnClientBlur"
                                    OnClientSelectedIndexChanged="cboTrailer_OnClientSelectedIndexChanged"
                                    DataTextField="Description" DataValueField="ResourceId"
                                    MarkFirstMatch="true" ShowMoreResultsBox="True" AllowCustomText="true" Width="155px" Overlay="true" Height="250px" EnableVirtualScrolling="False" ItemsPerRequest="-1" EnableAutomaticLoadOnDemand="false"/>
                            </div>
                            <div style="display:inline; margin-left:10px; width:100%;">
                                <asp:Button ID="btnTrailerUnassign" runat="server" Text="Unassign" OnClientClick='if(!SelectUnassigned(2)) return false;' />
                                <input type="button" id="btnTrailerClear" value="Clear" onclick="if(!SelectClear(2)) return false;" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>&nbsp;</td>
                        <td><div><asp:Label ID="lblTrailerDescription" runat="server"></asp:Label></div></td>
                    </tr>
                    <tr id="trThirdPartyOrganisation" runat="server" style="display:none;">
                        <td class="formCellLabel" style="width:25%;">Organisation</td>
                        <td class="formCellInput">
                            <telerik:RadComboBox ID="cboThirdPartyTrailerOwner" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false" AutoPostBack="false"
                                                 DataTextField="OrganisationName" DataValueField="IdentityId"
                                                 MarkFirstMatch="true" AllowCustomText="false" ItemRequestTimeout="500" Width="155px" ValidationGroup="grpResourceThis" EnableVirtualScrolling="True">
                            </telerik:RadComboBox>
                            <asp:RequiredFieldValidator ID="rfvThirdPartyTrailerOwner" runat="server" Enabled="false" ControlToValidate="cboThirdPartyTrailerOwner" ErrorMessage="Please indicate which company owns this trailer." EnableClientScript="True"  ValidationGroup="grpResourceThis" >
                                <img src="/images/error.png" Title="Please indicate which company owns this trailer." alt="" />
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <table>
                    <tr>
                        <td colspan="2">
                            <span id="divPCVS" runat="server" style="font-family: tahoma; color: Red; padding: 2px">(<asp:Label ID="lblPCVs" runat="server">Please Inform Driver There are PCV's To Take</asp:Label>)</span>
                        </td>
                    </tr>
                 </table>
                <table>
                    <tr valign="top">
                        <td colspan="3">
                            <b>NB:</b>
                            <asp:Label ID="lblDrawingResourceFrom" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </fieldset>
        </div>
        <div style="float:left; margin-left:10px;">
            <asp:Panel ID="pnlTrailerContents" runat="server" >
                <div style="float:left;">
                <asp:ListView ID="lvPalletBalances" runat="server">
                    <LayoutTemplate>
                        <fieldset style="border-color: #CCC;">
                            <h3>Empty Pallets</h3>
                            <table class="Grid" style="width: 100%;" cellpadding="0" cellspacing="0">
                                <thead><tr class="HeadingRow"><th>Pallets</th><th>Pallet Type</th><th>RunID</th></tr></thead>
                                <tbody>
                                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                </tbody>
                            </table>
                        </fieldset>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr class="Row">
                            <td><a href='javascript:openAlterPalletBalance(<%# ((System.Data.DataRowView)Container.DataItem)["JobID"].ToString() %>, <%# ((System.Data.DataRowView)Container.DataItem)["PalletTypeID"].ToString() %>, <%# ((System.Data.DataRowView)Container.DataItem)["PalletCount"].ToString()%>, <%# ((System.Data.DataRowView)Container.DataItem)["ResourceID"].ToString()%> );'><%# ((System.Data.DataRowView)Container.DataItem)["PalletCount"].ToString()%></a></td>
                            <td><%# ((System.Data.DataRowView)Container.DataItem)["PalletType"].ToString()%></td>
                            <td><a href='javascript:openJob(<%# ((System.Data.DataRowView)Container.DataItem)["JobID"].ToString() %>);'><%# ((System.Data.DataRowView)Container.DataItem)["JobID"].ToString() %></a></td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
                </div>
                <div style="float:left; margin-left:5px;">
                <asp:ListView ID="lvGoodsRefused" runat="server">
                    <LayoutTemplate>
                        <fieldset style="border-color: #CCC;">
                            <h3>Refused Goods</h3>
                            <table class="Grid" style="width: 100%;" cellpadding="0" cellspacing="0">
                                <thead><tr class="HeadingRow"><th>Goods ID</th><th>Run ID</th></tr></thead>
                                <tbody>
                                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                </tbody>
                            </table>
                        </fieldset>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr class="Row">
                            <td><a href='javascript:openGoodsRefusal(<%# ((System.Data.DataRowView)Container.DataItem)["RefusalID"].ToString() %>, <%# ((System.Data.DataRowView)Container.DataItem)["RefusedInstructionID"].ToString() %>);'><%# ((System.Data.DataRowView)Container.DataItem)["RefusalID"].ToString()%></a></td>
                            <td><a href='javascript:openJob(<%# ((System.Data.DataRowView)Container.DataItem)["RefusalJobId"].ToString() %>, <%# ((System.Data.DataRowView)Container.DataItem)["RefusedInstructionID"].ToString() %>);'><%# ((System.Data.DataRowView)Container.DataItem)["RefusalJobId"].ToString()%></a></td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
                </div>
            </asp:Panel>
        </div>
    </div>
    
    <div class="clearDiv"></div>
    
    <div style="height:50px;">
        <div id="thirdPartyTrailerNote" style="display:none;">
            The trailer you have selected is a third party trailer.
            <br />
            Please indicate which organisation owns this trailer so that pallet balances and refused goods can be recovered.
            <br />
            <p style="font-weight:bold;">IMPORTANT! : Do not use this method to create your own company Trailers. Own company Trailers must be created from the Trailer screen.</p>
        </div>
        <div style="margin-left:27px; padding-top:5px; display:none;" id="wrapperInfringementDisplay" runat="server" class="infringmentDisplayWrapper">
            <uc1:infringementDisplay runat="server" ID="idErrors" />
        </div>
    </div>
    
    <div>
        <fieldset>
            <div>
                <div style="float:left;">
                    <h3>Run</h3>
                </div>
                <div style="float:right; vertical-align:middle;">
                    <div style="border:solid 1px black; height:20px; width:20px; background-color:#FFA; float:right; margin-left:5px; margin-right:5px;">
                    </div>
                    <div style="padding-top:4px; float:right;">
                        <asp:Label ID="lblPendingChanges" runat="server" Font-Bold="true" Text="Pending Changes" />
                    </div>
                </div>
                <div class="clearDiv"></div>            
            </div>
           
            <div>
                <telerik:RadGrid ID="grdRun" runat="server" Width="940px" Height="200px" ShowFooter="false" GridLines="Horizontal" AllowSorting="false" AutoGenerateColumns="false">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridTemplateColumn UniqueName="chkSelectLeg" HeaderStyle-Width="3%" ItemStyle-Width="3%">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkAllLegs" CssClass="selectAllRunLegs" runat="server" onclick="javascript:chkHeaderSelectedAll(this, 'chkLegSelect');" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:Checkbox ID="chkSelectLeg" runat="server" CssClass="chkLegSelect" onclick="javascript:chkLegSelected(this, 'chkLegSelect');" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="CollectionPoint" HeaderText="From" HeaderStyle-Width="25%" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <span onmouseover="ShowPointToolTip(this, <%# ((Orchestrator.Entities.LegView) Container.DataItem).StartLegPoint.Point.PointId.ToString() %>);" onmouseout="javascript:closeToolTip();">
                                        <b><%# ((Orchestrator.Entities.LegView)Container.DataItem).StartLegPoint.Point.Description%></b>
                                    </span>
                                    <input type="hidden" id="hidInstructionId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "InstructionID" ) %>' />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="DeliveryPoint" HeaderText="To" HeaderStyle-Width="25%" ItemStyle-Width="25%">
                                <ItemTemplate>
                                    <span onmouseover="ShowPointToolTip(this, <%# ((Orchestrator.Entities.LegView) Container.DataItem).EndLegPoint.Point.PointId.ToString() %>);" onmouseout="javascript:closeToolTip();">
                                        <b><%# ((Orchestrator.Entities.LegView)Container.DataItem).EndLegPoint.Point.Description %></b>
                                    </span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Driver" HeaderText="Driver" HeaderStyle-Width="12%" ItemStyle-Width="12%">
                                <ItemTemplate>
                                    <span id="spnDriver" runat="server"><asp:Label ID="lblDriver" runat="server" /></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Vehicle" HeaderText="Vehicle" HeaderStyle-Width="10%" ItemStyle-Width="10%">
                                <ItemTemplate>
                                    <span id="spnVehicle" runat="server"><asp:Label ID="lblVehicle" runat="server" /></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Trailer" HeaderText="Trailer" HeaderStyle-Width="7%" ItemStyle-Width="7%">
                                <ItemTemplate>
                                    <span id="spnTrailer" runat="server"><asp:Label ID="lblTrailer" runat="server" /></span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="Comm" HeaderText="Comm" HeaderStyle-Width="3%" ItemStyle-Width="3%">
                                <HeaderTemplate>
                                    <asp:CheckBox ID="chkCommAllLegs" CssClass="communicateAllLegs" runat="server" ToolTip="Communicate all legs" onclick="javascript:chkHeaderSelectedAll(this, 'chkCommLeg');" />
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <asp:CheckBox ID="chkCommunicateLeg" runat="server" CssClass="chkCommLeg" ToolTip="Communicate leg" onclick="javascript:chkLegSelected(this, 'chkCommLeg');" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="CA" HeaderText="CA" HeaderStyle-Width="100" ItemStyle-Width="100">
                                <ItemTemplate>
                                    <span id="spnCA" runat="server">
                                        <asp:DropDownList ID="cboControlArea" runat="server"></asp:DropDownList>
                                        <ajaxToolkit:CascadingDropDown ID="CascadingDropDown2" runat="server" Category="ControlArea" TargetControlID="cboControlArea"  LoadingText="Loading CA.." ServicePath="/ws/resourcethis.asmx" ServiceMethod="GetControlAreas"></ajaxToolkit:CascadingDropDown>
                                        <asp:Label Visible="false" ID="lblControlArea" runat="server" />
                                    </span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn UniqueName="TA" HeaderText="TA" HeaderStyle-Width="100" ItemStyle-Width="100">
                                <ItemTemplate>                                   
                                    <span id="spnTA" runat="server">
                                        <asp:DropDownList ID="cboTrafficArea" runat="server"></asp:DropDownList> 
                                        <ajaxToolkit:CascadingDropDown ID="CascadingDropDown1" runat="server" Category="TrafficArea" TargetControlID="cboTrafficArea" ParentControlID="cboControlArea" LoadingText="Loading TA.." ServicePath="/ws/resourcethis.asmx" ServiceMethod="GetTrafficAreas"></ajaxToolkit:CascadingDropDown>
                                        <asp:Label Visible="false" ID="lblTrafficArea" runat="server" />
                                    </span>
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Scrolling AllowScroll="true" UseStaticHeaders="true" SaveScrollPosition="true" />
                    </ClientSettings>
                </telerik:RadGrid>
            </div>
            
            <div class="buttonbar" style="margin-top:5px;" id="updateRowContainer">
                <asp:Button ID="btnUpdateRows" runat="server" Text="Update Legs" ValidationGroup="grpResourceThis" />
            </div>
        </fieldset>
    </div>
    
    
    <div style="margin-bottom:5px;">
        <b>NB:</b>
       You may only communicate legs that have already been assigned a driver. Only legs selected for communication will be communicated.
    </div>
        
    <div class="whitespacepusher"></div>
    
    <div class="buttonbar">
        <asp:Button ID="btnRemoveResource" runat="server" Text="Remove Resources" />
        <asp:Button ID="btnUncommunicate" runat="server" Text="Uncommunicate" />
        <asp:Button ID="btnShowLoadOrder" runat="server" Text="Show Load Order" />
        <asp:Button ID="btnCancel" runat="server" Text="Close" Width="75" CausesValidation="False" OnClientClick="if(!closeCheckForPendingUpdates()) return false;" />
        <asp:Button ID="btnConfirm" runat="server" Text="Update Run" Width="85" />
        <asp:Button ID="btnUpdateAndConfirm" runat="server" Text="Update & Confirm" Width="125px" />
    </div>
    
    <div id="divPointAddress" style="z-index:5; display: none; padding: 2px 2px 2px 2px;">
        <table style="background-color: white; border: solid 1pt black;" cellpadding="2">
            <tr>
                <td>
                    <span id="spnPointAddress"></span>
                </td>
            </tr>
        </table>
    </div>    
    
    <input type="hidden" runat="server" id="hidNumberOfSelectedLegsSubbedOut" />
    
    <telerik:RadAjaxManager ID="ramResourceThis" runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="ramResourceThis" EventName="AjaxRequest">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="pnlTrailerContents" />
                    <telerik:AjaxUpdatedControl ControlID="lvPalletBalances" />
                    <telerik:AjaxUpdatedControl ControlID="lvGoodsRefused" />                  
                    <telerik:AjaxUpdatedControl ControlID="wrapperInfringementDisplay" />
                    <telerik:AjaxUpdatedControl ControlID="lblTrailerDescription" />
                    <%--<telerik:AjaxUpdatedControl ControlID="cboTrailer" UpdatePanelRenderMode="Inline" />--%>
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
        <ClientEvents OnResponseEnd="ajaxRequestEnd" />
    </telerik:RadAjaxManager>
    
    <asp:Label ID="InjectScript" runat="server"></asp:Label>
   <script type="text/javascript">
       Sys.Application.add_load(PreSelectedResource);
   </script>

    
</asp:Content>
