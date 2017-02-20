<%@ Page language="c#" Inherits="Orchestrator.WebUI.GoodRefused.ListGoodsRefused" MasterPageFile="~/default_tableless.master" Codebehind="ListGoodsRefused.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="chklstVal" Namespace="P1TP.Components.Web.CheckBoxListValidator" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="p1" TagName="Resource" Src="~/UserControls/resource.ascx" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript" src="/script/jquery-ui-1.9.2.min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.fixedheader.js"></script>
    <script type="text/javascript" language="javascript" src="/script/jquery.quicksearch-1.3.1.js" ></script>
    <script type="text/javascript" language="javascript" src="ListGoodsRefused.js"></script>

    <script language="javascript" type="text/javascript">
        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
        }

  
</script>

    <style type="text/css">
        h3
        {
            clear: left;
            font-size: 12px;
            font-weight: normal;
            padding: 0 0 1em;
            margin: 0;
        }
        
        /*.masterpage_layout {width: 1700px;}*/.RadGrid_Orchestrator *
        {
            font-family: Verdana !important;
            font-size: 10px !important;
        }
        
        .overlayedDataBox
        {
            width: 330px !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List Goods Refused</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <telerik:RadWindowManager ID="rmwCollections" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>
    
    <cc1:Dialog ID="dlgRun" runat="server" Mode="Modal" Resizable="false" URL="/Job/Job.aspx" Width="1200" Height="900" UseCookieSessionID="true"/>
    <cc1:Dialog ID="dlgOrder" URL="/Groupage/ManageOrder.aspx" Height="1400" Width="1000" AutoPostBack="true" runat="server" Mode="Modal" ReturnValueExpected="true"></cc1:Dialog>
    <cc1:Dialog ID="dlgRefusal" ReturnValueExpected="true" AutoPostBack="true" runat="server" Mode="Modal" Resizable="false" URL="Addupdategoodsrefused.aspx" Width="900" Height="760" />
    <cc1:Dialog ID="dlgResource" runat="server" Mode="Modal" Resizable="true" URL="/Resource/Future.aspx" Width="1100" Height="560" />
    <cc1:Dialog ID="dlgAddUpdatePointGeofence" runat="server" Width="1034" Height="600" Mode="Normal" URL="/Point/Geofences/GeofenceManagement.aspx"></cc1:Dialog>
    <cc1:Dialog ID="dlgManifestGeneration" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" OnClientDialogCallBack="manifestGeneration_CallBack" ReturnValueExpected="true" ></cc1:Dialog>
    <cc1:Dialog ID="dlgLoadingSheet" runat="server" Width="1200" Height="900" Mode="Normal" URL="/Reports/Reportviewer.aspx?wiz=true" ReturnValueExpected="false" ></cc1:Dialog>

    <h2>Use this screen to create orders and runs for your refused goods.</h2>
    <p>Please note that you can only Load Build an onwards run if you have selected a Client, if only the "Outstanding" status is selected and if only the "At Store" location is selected.</p>
    <p>If the Goods are On-Trailer and not Returned, editing the Goods Refusal and selecting Off-Trailer will make them available for onwards run.</p>
    <p>If the Goods are With Driver or Unknown, editing the Goods Refusal and checking the At Store option will make them available for onwards run.</p>
    
    

                            <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <asp:Button id="btnReport" visible="false" text="Generate Report" runat="server" CausesValidation="false"></asp:Button>
            <div id="dvSelected" style="float:right; text-align:right; color:white; font-weight:bold; padding: 3px 3px 3px 3px; margin: 3px 0px 3px 0px;"></div>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
        <table width="100%">
        <tr valign="top">
            <td valign="top">
        <div style="float:left; margin-right: 60px">
                <fieldset>
                    <legend>Filter Options</legend>
                    <table>
                        <tr style="display:none">
                            <td class="formCellLabel">Refused Goods</td>
                            <td class="formCellField" colspan="3">
                                <p><asp:radiobuttonlist id="rdoGoodsRefusedFilterType" runat="server" RepeatDirection="Horizontal" AutoPostBack="True"></asp:radiobuttonlist></p>
                            </td>
                        </tr>
                        <tr>
                            <td class="formCellLabel"><asp:label id="lblClient" runat="server">Client</asp:label></td>
                            <td class="formCellField" colspan="3">
                                <telerik:RadComboBox ID="cboClient" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="false" AllowCustomText="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px"></telerik:RadComboBox>
                                <asp:LinkButton ID="lnkbtnClearClient" runat="server" OnClientClick="if(!clearClient()) return false;" Text="Clear" />
                            </td>
                        </tr>
                        <asp:panel id="pnlGoodsRefusedListBoxes1" runat="server" Visible="True">
                            <tr>
                                <td class="formCellLabel">Status</td>
                                <td class="formCellField" colspan="3">
                                    <asp:CheckBoxList id="chkGoodsRefusedStatus" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    <chklstVal:RequiredFieldValidatorForCheckBoxLists Display="Dynamic" id="rfvGoodsRefusedStatus" EnableClientScript=false runat="server" ErrorMessage="Please select at least one good refused status..." ControlToValidate="chkGoodsRefusedStatus"><img src="/images/Error.gif" height='16' width='16' title='Please select at least one good refused status...'></chklstVal:RequiredFieldValidatorForCheckBoxLists>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Current Location</td>
                                <td class="formCellField" colspan="3">
                                    <asp:CheckBoxList id="chkLocation" runat="server" RepeatDirection="Horizontal"></asp:CheckBoxList>
                                    <chklstVal:RequiredFieldValidatorForCheckBoxLists Display="Dynamic" id="rfvLocation" EnableClientScript=false runat="server" ErrorMessage="Please select at least one location..." ControlToValidate="chkLocation"><img src="/images/Error.gif" height='16' width='16' title='Please select at least one location...'></chklstVal:RequiredFieldValidatorForCheckBoxLists>
                                </td>
                            </tr>
                        </asp:panel>
                        <tr>
                            <td class="formCellLabel">Date From</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiStartDate" Width="100" runat="server">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                        </tr>    
                        <tr>
                            <td class="formCellLabel">Date To</td>
                            <td class="formCellField">
                                <telerik:RadDatePicker ID="rdiEndDate" Width="100" runat="server">
                                <DateInput runat="server"
                                DateFormat="dd/MM/yy">
                                </DateInput>
                                </telerik:RadDatePicker>
                            </td>
                        </tr>
                    </table>
                   
                </fieldset>
                </div>
                  <div style="float:left">
                           <table style="width:100%;" cellpadding="0" cellspacing="0">
                        <tr>
                            <td>
                                <fieldset>
                                    <legend>Store Location</legend>
                                    <table width="100%">
                                        <tr>
                                            <td class="formCellLabel">Client</td>
                                            <td>
                                                <telerik:RadComboBox ID="cboStoreOrganisation" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false" AllowCustomText="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" OnClientSelectedIndexChanged="StoreLocationClientSelectedIndexChanged"></telerik:RadComboBox>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel">Point</td>
                                            <td>
                                                <telerik:RadComboBox ID="cboStorePoint" runat="server" EnableLoadOnDemand="true" ShowMoreResultsBox="false" AllowCustomText="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="355px" OnClientItemsRequesting="StorePointRequesting"></telerik:RadComboBox>
                                            </td>
                                        </tr>
                                    </table>
                                    <div class="buttonBar">
                                        <asp:Button ID="btnClearStoreLocation" runat="server" Text="Clear" OnClientClick="if(!clearStoreLocation()) return false;" />
                                    </div>
                                </fieldset>
                            </td>
                        </tr>
                    </table>
                    </div>
                    </table>
                    <div class="buttonbar" style="margin-bottom:5px;">
	    <asp:Button ID="btnFind" runat="server" Text="Search" Width="75px" />
	    
    </div>
    
                    </div>
              
                                    
                
     
    

                
           <asp:panel id="pnlGoodsRefusedListBoxes2" runat="server" Visible="True">
                </asp:panel>     

   



    <div class="overlayedClearDataBox" id="overlayedClearDataBox" style="display: none;">
        <div id="tabs">
            <ul>
                <li><a href="#tabs-1">Load Builder</a></li>
                <li><a href="#tabs-2">Collect / Deliver</a></li>
                <li><a href="#tabs-3">Run</a></li>
                <li id="dragLI" class="moveTab"></li>
                <li id="resetBox" class="resetTab" onclick="resetBox();"></li>
                <li id="expandBox" class="expandTab" onclick="expandBox();"></li>
                <li id="detractBox" class="detractTab" onclick="detractBox();" style="display: none;">
                </li>
            </ul>
            <div id="tabs-1">
                <div id="loadBuilder" class="loadBuilder" style="height: 355px; overflow-x: hidden; overflow-y: auto;">
                    <div class="loadBuilderInner">
                        <fieldset style="margin: 0 0 5px 0; display:none;">
                            <legend>Runs</legend>To add an order to an existing run either click on a Coll Run
                            ID in the Grid or click here:
                            <input type="button" value="Add To Existing Run" class="buttonClass" onclick="ShowUpdateJob();" />
                        </fieldset>
                        <fieldset style="margin: 0 0 5px 0;">
                            <legend>BusinessType</legend>
                            <table>
                                <tr>
                                    <td class="formCellLabel">Selected</td>
                                    <td class="formCellInput"><telerik:RadComboBox ID="rcbBusinessType" runat="server" Width="200px" DataTextField="Description" DataValueField="BusinessTypeID" /></td>
                                </tr>
                            </table>
                        </fieldset>
                        <table id="tblLoadBuilderOrders" class="DataGridStyle" style="width: 315px; font-weight: normal;
                            display: none;" cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    LO
                                </th>
                                <th style="width: 30px;">
                                    ID
                                </th>
                                <th style="width: 100px;">
                                    From
                                </th>
                                <th style="display: none;">
                                    To
                                </th>
                                <th>
                                    Spcs
                                </th>
                                <th>
                                    Kg
                                </th>
                                <th>
                                    &#160;
                                </th>
                            </tr>
                            <tr class="DataGridListItem">
                                <td style="width: 20px;">
                                    <img src="/App_Themes/Orchestrator/img/masterpage/icon-cross.png" id="img2" style="display: none;
                                        cursor: pointer; margin: 2px 2px 0 0;" /><input type="text" id="txtOrder" style="width: 15px;
                                            color: Black;" />
                                </td>
                                <td style="width: 15px;" class="orderID">
                                    <span id="refusalID">refusalID</span>
                                </td>
                                <td style="width: 135px;">
                                    <span id="collectionPoint">collectionpoint</span>
                                </td>
                                <td style="display: none;">
                                    <span id="deliveryPoint">deliverypoint</span>
                                </td>
                                <td style="width: 18px;">
                                    <img alt="Remove order from Load" src="/App_Themes/Orchestrator/img/masterpage/icon-cross.png"
                                        id="imgRemove" style="cursor: pointer; margin: 2px 2px 0 0;" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div class="buttonBar">
                        <input type="button" class="buttonClassSmall" value="Re-Order" onclick="ReOrder();" />
                    </div>
                    <div style="min-height: 20px">
                        <div id="divBusinessTypes">
                        </div>
                    </div>
                </div>
            </div>
            <div id="tabs-2">
                <div class="loadBuilder" style="height: 355px; overflow-x: hidden; overflow-y: auto;">
                    <div>
                        <fieldset>
                            <legend>Collect</legend>
                            <table>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <table>
                                            <tr>
                                                <td>
                                                    <input type="radio" name="collection" runat="server" id="rdCollectionTimedBooking" checked="true" onclick="collectionTimedBooking(this);" />Timed Booking
                                                    <input type="radio" name="collection" runat="server" id="rdCollectionBookingWindow" onclick="collectionBookingWindow(this);" />Booking Window
                                                    <input type="radio" name="collection" runat="server" id="rdCollectionIsAnytime" onclick="collectionIsAnytime(this);" />Anytime
                                                </td>
                                            </tr>
                                        </table>
                                        <table>
                                            <tr>
                                                <td class="formCellLabel" style="width: 80px;">
                                                    Collect from:
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadDateInput Width="65" ID="dteCollectionFromDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionFromDate_SelectedDateChanged" />
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadDateInput Width="65" ID="dteCollectionFromTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" />
                                                </td>
                                                <td class="formCellInput">
                                                    <asp:RequiredFieldValidator ID="rfvCollectionFromDate" ValidationGroup="createReturns" runat="server" ControlToValidate="dteCollectionFromDate" Display="Dynamic" ErrorMessage="Please enter a collection from date.">
                                                        <img id="Img3" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection from date." />
                                                    </asp:RequiredFieldValidator>
                                                    <asp:RequiredFieldValidator ID="rfvCollectionFromTime" ValidationGroup="createReturns" runat="server" ControlToValidate="dteCollectionFromTime" Display="Dynamic" ErrorMessage="Please enter a collection from time.">
                                                        <img id="Img14" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection from time." />
                                                    </asp:RequiredFieldValidator>
                                                    <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="dteCollectionFromDate" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateCollectionDate" ErrorMessage="The date cannot be before today." ValidationGroup="createReturns">
                                                        <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon" />
                                                    </asp:CustomValidator>
                                                </td>
                                            </tr>
                                            <tr runat="server" id="trCollectBy">
                                                <td class="formCellLabel">
                                                    Collect by:
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadDateInput Width="65" ID="dteCollectionByDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteCollectionByDate_SelectedDateChanged" />
                                                </td>
                                                <td class="formCellInput">
                                                    <telerik:RadDateInput Width="65" ID="dteCollectionByTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00" />
                                                </td>
                                                <td class="formCellInput">
                                                    <asp:RequiredFieldValidator ID="rfvCollectionByDate" ValidationGroup="createReturns" runat="server" ControlToValidate="dteCollectionByDate" Display="Dynamic" ErrorMessage="Please enter a collection by date.">
                                                        <img id="Img12" alt="" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection by date." />
                                                    </asp:RequiredFieldValidator>
                                                    <asp:RequiredFieldValidator ID="rfvCollectionByTime" ValidationGroup="createReturns" runat="server" ControlToValidate="dteCollectionByTime" Display="Dynamic" ErrorMessage="Please enter a collection by time.">
                                                        <img id="Img13" alt="" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a collection by time." />
                                                    </asp:RequiredFieldValidator>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </div>
                    <div>
                        <fieldset style="margin-bottom: 5px;">
                            <legend>Deliver Refusals To</legend>
                            <table style="width: 315px;">
                                <tr>
                                    <td>
                                        <input type="checkbox" id="chkDeliveryPoint" onclick="javascript:ChangeDelivery(this);" value="0" /><label for="chkDeliveryPoint">Deliver to this point</label>
                                    </td>
                                </tr>
                                <tr id="trDeliveryPoint" style="display:none;">
                                    <td>
                                        <table>
                                            <tr>
                                                <td colspan="2">
                                                    <p1:Point runat="server" ID="ucDeliveryPoint" ShowFullAddress="true" CanChangePoint="true" CanCreateNewPoint="false" CanUpdatePoint="false" Width="300" />
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="formCellField-Horizontal">
                                                    <table>
                                                        <tr>
                                                            <td>
                                                                <input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" checked="true" />Timed Booking
                                                                <input type="radio" name="delivery" runat="server" id="rdDeliveryBookingWindow" onclick="deliveryBookingWindow(this);" />Booking Window
                                                                <input type="radio" name="delivery" runat="server" id="rdDeliveryIsAnytime" onclick="deliveryIsAnytime(this);" />Anytime
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
                                                                <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                                                    <img id="Img8" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from date.">
                                                                </asp:RequiredFieldValidator>
                                                                <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryFromTime" Display="Dynamic" ErrorMessage="Please enter a delivery from time.">
                                                                    <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery from time.">
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
                                                                <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                                                    <img id="Img10" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by date." alt="">
                                                                </asp:RequiredFieldValidator>
                                                                <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="submit" runat="server" ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                                                    <img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a delivery by time."  alt="">
                                                                </asp:RequiredFieldValidator>
                                                                <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate" ValidationGroup="submit" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate" ErrorMessage="The date cannot be before today.">
                                                                    <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/>
                                                                </asp:CustomValidator>
                                                                <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2" ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?" ValidationGroup="submit">
                                                                    <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date entered is far in the future." alt="warning icon"/>
                                                                </asp:CustomValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                        <div id="deliveryPointText" class="infoPanel" style="font-weight: normal;">
                            Unless a delivery point is specified here, the selected orders will be taken to their own delivery point (which may be different to the goods return point in cases where the goods record has been updated after creating the refusal order). 
                        </div>
                    </div>
                </div>
            </div>
            <div id="tabs-3">
                <div class="loadBuilder" style="height: 355px; overflow-x:hidden; overflow-y:auto;">
                    <fieldset>
                        <legend>Run</legend>
                        <input type="checkbox" id="chkCreateRun" value="createRun" /><label for="chkCreateRun">Create Refusal Return Run</label>
                    </fieldset>
                    <div id="runOptions" style="display:none;">
                        <fieldset>
                            <legend>Goods Action</legend>
                            <input type="checkbox" id="chkLeaveGoods" value="leaveGoods" /><label for="chkLeaveGoods">Leave goods at destination</label>
                            <div id="dvCrossDockLocation" style="display:none;">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <p1:Point runat="server" ID="ucLeaveLocation" ShowFullAddress="true" CanChangePoint="true" CanCreateNewPoint="false" CanUpdatePoint="false" Width="300" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField-Horizontal">
                                            <table>
                                                <tr>
                                                    <td>
                                                        <input type="radio" name="leave" runat="server" id="rdLeaveTimedBooking" onclick="leaveTimedBooking(this);" />Timed Booking
                                                        <input type="radio" name="leave" runat="server" id="rdLeaveBookingWindow" onclick="leaveBookingWindow(this);" />Booking Window
                                                        <input type="radio" name="leave" runat="server" id="rdLeaveIsAnytime" checked="true" onclick="leaveIsAnytime(this);" />Anytime
                                                    </td>
                                                </tr>
                                            </table>
                                            <table>
                                                <tr>
                                                    <td class="formCellLabel" style="width: 80px;">
                                                        Leave from:
                                                    </td>
                                                    <td class="formCellInput">
                                                        <telerik:RadDateInput Width="65" ID="dteLeaveFromDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteLeaveFromDate_SelectedDateChanged" />
                                                    </td>
                                                    <td class="formCellInput">
                                                        <telerik:RadDateInput Width="65" ID="dteLeaveFromTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 08:00" />
                                                    </td>
                                                    <td class="formCellInput">
                                                        <asp:RequiredFieldValidator ID="rfvLeaveFromDate" ValidationGroup="createReturns" runat="server" ControlToValidate="dteLeaveFromDate" Display="Dynamic" ErrorMessage="Please enter a Leave from date.">
                                                            <img id="Img4" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a Leave from date." />
                                                        </asp:RequiredFieldValidator>
                                                        <asp:RequiredFieldValidator ID="rfvLeaveFromTime" ValidationGroup="createReturns" runat="server" ControlToValidate="dteLeaveFromTime" Display="Dynamic" ErrorMessage="Please enter a Leave from time.">
                                                            <img id="Img5" runat="server" alt="" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a Leave from time." />
                                                        </asp:RequiredFieldValidator>
                                                        <asp:CustomValidator ID="cfvLeaveFromDate" runat="server" ControlToValidate="dteLeaveFromDate" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateLeaveDate" ErrorMessage="The date cannot be before today." ValidationGroup="createReturns">
                                                            <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon" />
                                                        </asp:CustomValidator>
                                                    </td>
                                                </tr>
                                                <tr runat="server" id="trLeaveBy">
                                                    <td class="formCellLabel">
                                                        Leave by:
                                                    </td>
                                                    <td class="formCellInput">
                                                        <telerik:RadDateInput Width="65" ID="dteLeaveByDate" runat="server" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="dteLeaveByDate_SelectedDateChanged" />
                                                    </td>
                                                    <td class="formCellInput">
                                                        <telerik:RadDateInput Width="65" ID="dteLeaveByTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" SelectedDate="01/01/01 17:00" />
                                                    </td>
                                                    <td class="formCellInput">
                                                        <asp:RequiredFieldValidator ID="rfvLeaveByDate" ValidationGroup="createReturns" runat="server" ControlToValidate="dteLeaveByDate" Display="Dynamic" ErrorMessage="Please enter a Leave by date.">
                                                            <img id="Img6" alt="" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a Leave by date." />
                                                        </asp:RequiredFieldValidator>
                                                        <asp:RequiredFieldValidator ID="rfvLeaveByTime" ValidationGroup="createReturns" runat="server" ControlToValidate="dteLeaveByTime" Display="Dynamic" ErrorMessage="Please enter a Leave by time.">
                                                            <img id="Img7" alt="" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a Leave by time." />
                                                        </asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </div>
                        </fieldset>
                        <fieldset>
                            <legend>Resource this</legend>
                            <input type="radio" value="0" name="rdoresource" id="rbOwnResource" checked="checked" /><label for="rbOwnResource">use own resource</label>
                            <input type="radio" value="1" name="rdoresource" id="rbSubContractor" /><label for="rbSubContractor">Use Subcontractor</label>
                            <div id="pnlOwnresource">
                                <p1:Resource runat="server" ID="ucResource"></p1:Resource>
                            </div>
                            <div id="pnlSubContractor" style="display: none;">
                                <table>
                                    <tr>
                                        <td colspan="2">
                                            <telerik:RadComboBox ID="cboSubContractor" runat="server" AllowCustomText="true" ShowMoreResultsBox="false" MarkFirstMatch="true" ItemRequestTimeout="500" Width="100%" EnableLoadOnDemand="true" >
                                                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetSubContractors" />
                                            </telerik:RadComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label for="rbWholeJob">Subcontract whole run</label>
                                            <input type="radio" id="rbWholeJob" value="0" name="SubContractOption" checked="checked" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <label for="rbPerOrder">Subcontract per order</label>
                                            <input type="radio" id="rbPerOrder" value="1" name="SubContractOption" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">
                                            Subcontract for
                                        </td>
                                        <td class="formCellInput">
                                            <telerik:RadNumericTextBox ID="txtSubbyRate" runat="server" Type="Currency" MinValue="0" Value="0" Width="80" />
                                        </td>
                                    </tr>
                                </table>
                            </div>
                            <div>
                                <input type="checkbox" id="chkCreateManifest" value="createManifest" /><label for="chkCreateManifest">Create and show the manifest</label>
                                <br />
                                <input type="checkbox" id="chkShowJobOnCreation" value="showJobOnCreation" /><label for="chkShowJobOnCreation">Show the run details when created</label>
                                <br />
                                <input type="checkbox" id="chkCreateLoadingSheet" value="createLoadingSheet" /><label for="chkCreateLoadingSheet">Create and show the loading sheet</label>
                                <br />
                                <input type="checkbox" id="chkShowInProgress" value="2" checked="checked" /><label for="chkShowInProgress">Show as communicated</label>
                            </div>
                        </fieldset>
                    </div>
                    <div id="manifestOptions" style="display:none;">
                        <fieldset>
                            <legend>Manifest options</legend>
                            <asp:RadioButtonList ID="rblOrdering" runat="server" RepeatDirection="Horizontal" Style="display:none;">
                                <asp:ListItem Text="Order by Planned Times" Value="0" />
                                <asp:ListItem Text="Order by Run Booked Times" Value="1" Selected="True" />
                            </asp:RadioButtonList>
                            <input type="checkbox" id="chkExcludeFirstRow" value="1" checked="checked" /><label for="chkExcludeFirstRow">Exclude the first row from the Manifest</label>
                            <br />
                            <input type="checkbox" id="chkShowFullAddress" value="1" /><label for="chkShowFullAddress">Show the full address on the manifest</label>
                            <br />
                            <span style="width: 80px;">Manifest Date:</span><telerik:RadDateInput ID="dteManifestDate" runat="server" OnClientDateChanged="ManifestDate_OnDateChanged"></telerik:RadDateInput>
                            <br />
                            <span style="width: 80px;">Manifest Title :</span><input type="text" id="txtManifestTitle" style="width: 250px;" />
                            <br />
                            Number of blank rows to include:<input type="text" id="txtExtraRows" value="0" style="width: 25px;" />
                        </fieldset>
                    </div>
                    <div class="buttonbar" style="margin-bottom: 1px; margin-top: 3px; display: none;" id="createCollectionJob">
                        <input id="btnCreateCollectionJob" type="button" value="Submit" style="font-size: 11px;" onclick="javascript:CreateReturns();" validationgroup="createReturns" />
                        <input id="btnCancelCreateCollectionJob" type="button" value="Cancel" style="font-size: 11px;" onclick="javascript:CancelAndResetScreen();" />
                    </div>
                </div>
            </div>
        </div>
        <div id="tabs2" style="display: none;">
            <ul>
                <li><a href="#tabs-4">Instructions</a></li>
                <li><a href="#tabs-5">Pick up point</a></li>
                <li><a href="#tabs-6">Existing Orders</a></li>
                <li id="dragLI" class="moveTab"></li>
                <li id="resetBox" class="resetTab" onclick="resetBox();"></li>
                <li id="expandBox" class="expandTab" onclick="expandBox();"></li>
                <li id="detractBox" class="detractTab" onclick="detractBox();" style="display: none;">
                </li>
            </ul>
            <div id="tabs-4">
                <div class="jobView">
                    <div>
                        Instructions for the run :
                        <input type="text" id="txtJobID" style="width: 65px;" />
                        &nbsp;
                        <input type="button" id="btnFindJob" value="Find" onclick="FindJob();" />
                    </div>
                    <div style="height: 255px; overflow-x: hidden; overflow-y: auto;">
                        <table id="tblInstructions" class="DataGridStyle" style="width: 95%; font-weight: normal;"
                            cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    &nbsp;
                                </th>
                                <th>
                                    Where
                                </th>
                                <th>
                                    When
                                </th>
                            </tr>
                        </table>
                        <input type="checkbox" id="chkLoadAndGo" checked="checked" value="0" onclick="SelectLoadAndGo(this);" /><label id="lblLoadAndGo" for="chkLoadAndGo">Add selected orders as load and go.</label>
                        <br />
                        <input type="checkbox" id="chkUpdateManifest" value="updateManifest" checked="true" /><label id="lblUpdateManifest" for="chkUpdateManifest">Update and show the manifest</label>
                    </div>
                    <div class="buttonbar" style="margin-bottom: 1px; margin-top: 3px; text-align: center;">
                        <input type="button" value="Add Order(s) to Run" onclick="UpdateCollectionJob();" />
                        <input type="button" value="Cancel" onclick="CancelAndResetScreen();" />
                    </div>
                </div>
            </div>
            <div id="tabs-5" class="jobView">
            </div>
            <div id="tabs-6">
                <div class="jobView">
                    <div style="height: 315px; overflow-x: hidden; overflow-y: auto;">
                        <table id="tblExistingOrders" class="DataGridStyle" style="width: 95%; font-weight: normal;
                            display: none;" cellpadding="2" cellspacing="0">
                            <tr class="DataGridListHeadSmall">
                                <th>
                                    LO
                                </th>
                                <th style="width: 30px;">
                                    ID
                                </th>
                                <th style="width: 100px;">
                                    From
                                </th>
                                <th style="display: none;">
                                    To
                                </th>
                                <th>
                                    Spcs
                                </th>
                                <th>
                                    Kg
                                </th>
                            </tr>
                            <tr class="DataGridListItem">
                                <td style="width: 40px;">
                                    <img src="/App_Themes/Orchestrator/img/masterpage/icon-cross.png" id="img1" style="cursor: pointer;
                                        margin: 2px 2px 0 0;" /><input type="text" id="Text2" value="0" style="width: 15px;
                                            color: Black;" />
                                </td>
                                <td style="width: 15px;" class="orderID">
                                    <span id="Span2">OrderID</span>
                                </td>
                                <td style="width: 135px;">
                                    <span id="Span3">collectionpoint</span>
                                </td>
                                <td style="display: none;">
                                    <span id="Span4">deliverypoint</span>
                                </td>
                                <td style="width: 15px;">
                                    <span id="Span5">palletspaces</span>
                                </td>
                                <td style="width: 15px;">
                                    <span id="Span6">weight</span>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div id="dvColumnDisplay" class="ui-tabs ui-corner-all">
            <fieldset>
                <legend>Display Columns</legend>
                <asp:CheckBoxList ID="cblGridColumns" runat="server">
                    <asp:ListItem Text="Collection Vehicle" Value="CollectionVehicle" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Driver" Value="CollectionDriver" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Client" Value="Customer" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Delivery Point" Value="DeliveryPoint" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Delivery Town" Value="DeliveryTown" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Final Destination Point" Value="DestinationPoint" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Final Destination Town" Value="DestinationTown" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Run" Value="CollectionJob" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collect Order" Value="CollectionOrder" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Customer" Value="CollectionCustomer" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Point" Value="CollectionPoint" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Address Line 1" Value="CollectionAddress1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Address Line 2" Value="CollectionAddress2" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Address Lines" Value="CollectionAddressLines" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="No Pallets" Value="NoPallets" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Pallet Spaces" Value="Spcs" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Weight" Value="Weight" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Collection Notes" Value="CollectionNotes" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Surcharges" Value="Surcharges" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Service Level" Value="OrderServiceLevelShortCode" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="Requesting Depot" Value="RequestingDepot" Selected="True"></asp:ListItem>
                </asp:CheckBoxList>
            </fieldset>
            <div class="buttonBar">
                <asp:Button ID="btnChangeColumns" Text="Save Columns" runat="server"/>
                <input type="button" id="Button1" runat="server" value="Cancel and close" onclick="ColumnDisplayHide()" />
            </div>
        </div>
    </div>
    
    <asp:Panel ID="pnlResourceManifestLinkPopup" runat="server" style="width: 400px; background-color: white; border-width: 2px; border-color: black; border-style: solid; padding: 20px; color: Silver; display: none;">
        <h2>Resource Manifest</h2>
        <p>
            The Resource Manifest could not be generated.
            <br />
            Please follow the link below to manually create / update.
        </p>
        <p>
            <a href="javascript:window.open('/manifest/resourcemanifestlist.aspx'); NextAction();">Create Resource Manifest</a>
        </p>
        <div class="buttonBar">
            <asp:Button ID="btnClose" runat="server" Text="Ok" />
        </div>
    </asp:Panel>
    
    <asp:Button ID="btnTest" Text="Test Button" runat="server" Style="display: none;" />
    <ajaxToolkit:ModalPopupExtender runat="server" ID="extener" BehaviorID="programmaticModalPopupBehavior" TargetControlID="btnTest" BackgroundCssClass="modalBackground" PopupControlID="pnlResourceManifestLinkPopup" OkControlID="btnClose" OnOkScript="javascript:NextAction();"></ajaxToolkit:ModalPopupExtender>   
        
    <div>
        <telerik:RadGrid runat="server" ID="grdRefusals" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false">
            <MasterTableView Width="100%" ClientDataKeyNames="RefusalId" DataKeyNames="RefusalId">
                <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                <Columns>
                    <telerik:GridTemplateColumn headertext="Goods ID">
                        <itemtemplate>
                            <a href="javascript:OpenGoodsRefusedForEdit(<%# DataBinder.Eval(Container.DataItem, "InstructionId") %>,<%# DataBinder.Eval(Container.DataItem, "RefusalId") %>)"><%# DataBinder.Eval(Container.DataItem, "RefusalId") %></a>
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn datafield="OrganisationName" headertext="Client" />
                    <telerik:GridBoundColumn datafield="ProductName" headertext="Name" />
                    <telerik:GridBoundColumn datafield="TimeFrame" headertext="Time Frame" dataformatstring="{0:dd/MM/yy}" />
                    <telerik:GridBoundColumn datafield="RefusalType" headertext="Type" />
                    <telerik:GridTemplateColumn HeaderText="Order">
                        <ItemTemplate>
                            <a runat="server" id="hypOriginalOrder"></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn datafield="ReturnLocation" headertext="Return To" />
                    <telerik:GridBoundColumn datafield="StoreLocation" headertext="Stored at" />
                    <telerik:GridTemplateColumn headertext="Located">
                        <itemtemplate>
                            <asp:label id="lblGoodLocation" runat="server" text='<%# DataBinder.Eval(Container.DataItem, "GoodLocation") %>' ></asp:label>
                            <a id="lnkResource" runat="server"></a>
                            <input type="hidden" id="Hidden1" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "RefusalId") %>' />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn headertext="Run Id">
                        <itemtemplate>
                            <a href="javascript:OpenRunDetails(<%# DataBinder.Eval(Container.DataItem, "JobId") %>);"><%# DataBinder.Eval(Container.DataItem, "JobId") %></a>
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn datafield="LoadNo" headertext="Load No" />
                    <telerik:GridBoundColumn datafield="DocketNumber" headertext="Docket No" />
                    <telerik:GridBoundColumn datafield="CollectDropDateTime" headertext="Date Refused" dataformatstring="{0:dd/MM/yy}" />
                    <telerik:GridBoundColumn datafield="OriginatingPoint" headertext="Originating" />
                    <telerik:GridTemplateColumn HeaderText="Return Order">
                        <ItemTemplate>
                            <a runat="server" id="hypNewOrder"></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn headertext="Return Run">
                        <itemtemplate>
                            <a href="javascript:OpenRunDetails(<%# DataBinder.Eval(Container.DataItem, "ReturnJobID") == DBNull.Value ? -1 : DataBinder.Eval(Container.DataItem, "ReturnJobID")  %>);"><%# DataBinder.Eval(Container.DataItem, "ReturnJobID")%></a>
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn headertext="Inc." UniqueName="SelectColumn">
                        <itemtemplate>
                            <asp:checkbox id="chkInclude" runat="server" AutoPostBack="false" />
                        </itemtemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                <Resizing AllowColumnResize="true" AllowRowResize="false" />
            </ClientSettings>
        </telerik:RadGrid>
    </div>
	
	<div class="buttonbar" align="left">
		<asp:Button ID="btnFindBottom" runat="server" Text="Search" Width="75px" />
		<asp:Button id="btnReportBottom" Text="Generate Report" visible="false" runat="server" CausesValidation="false"></asp:Button>
	</div>
	
	<uc1:reportviewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:reportviewer>
	
	<asp:HiddenField ID="hidShowConfirmForOrderAfterDays" runat="server" Value="" />
	
	<input id="hidCollectionTimingMethod" type="hidden" runat="server" />
	<input id="hidDeliveryTimingMethod" type="hidden" runat="server" />
	<input id="hidLeaveTimingMethod" type="hidden" runat="server" />
	<input runat="server" id="hidCollectionFromTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidCollectionByTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
	<input runat="server" id="hidLeaveFromTimeRestoreValue" type="hidden" />
    <input runat="server" id="hidLeaveByTimeRestoreValue" type="hidden" />
    
    <script language="javascript" type="text/javascript">
    <!--   
        function ShowFuture(resourceId, resourceTypeId, fromDate)
	    {
	        var qs = "wiz=true&resourceId=" + resourceId + "&resourceTypeId=" + resourceTypeId + "&fromDateTime=" + fromDate;
		    <%=dlgResource.ClientID %>_Open(qs);
	    }
	    
        function StoreLocationClientSelectedIndexChanged(item)
        {
            var pointCombo = $find("<%=cboStorePoint.ClientID %>");
            pointCombo.set_text("");
            pointCombo.requestItems(item.get_value(),false);
        }

        function StorePointRequesting(sender, eventArgs)
        {
            var storeLocationClientCombo = $find("<%=cboStoreOrganisation.ClientID %>");

            var context = eventArgs.get_context();
            context["FilterString"] = storeLocationClientCombo.get_value() + ";" + sender.get_text();
        }   
            
        function CountGoodsRefused(gridItem, index, checkBoxElement)
        {
            refusalId = gridItem.Cells[18].Value;
            hidSelectedRefusals.value = "";
            
            // Check the current CheckBox that we have just dealt with 
            if (checkBoxElement.checked)
                hidSelectedRefusals.value = hidSelectedRefusals.value + refusalId + ",";

            // Do the rest of the CheckBoxes
            var gridItem1;
            var itemIndex = 0;
            
            while(gridItem1 = dgRefusedGoods.Table.GetRow(itemIndex))
            {
                checked = gridItem1.Cells[17].Value;
                refusalId = gridItem1.Cells[18].Value;
                
                if(checked) // If checked
                    hidSelectedRefusals.value = hidSelectedRefusals.value + refusalId + ",";
               
               itemIndex++;
            }
          
            return true;
        }
        
        function OpenGoodsRefusedForEdit(instructionId, refusalId)
        {
	        var qs = "InstructionId=" + instructionId + "&RefusalId=" + refusalId + "&isWindowed=1" + "&showRP=1";
	        <%=dlgRefusal.ClientID %>_Open(qs);	        
        }
        
        function OpenRunDetails(runID) 
        {
            var qs = "jobId=" + runID + "&wiz=true";
	        <%=dlgRun.ClientID %>_Open(qs);	            
        }
        
        function clearClient()
        {
            var cboClient = $find("<%=cboClient.ClientID %>");
            cboClient.trackChanges();
            cboClient.clearSelection();
            cboClient.clearItems();
            cboClient.commitChanges();
            
            return false;
        }
        
        function clearStoreLocation()
        {
            var cboStoreOrganisation = $find("<%=cboStoreOrganisation.ClientID %>");
            var cboStorePoint = $find("<%=cboStorePoint.ClientID %>");

            cboStorePoint.trackChanges();        
            cboStorePoint.clearSelection();
            cboStorePoint.clearItems();
            cboStorePoint.commitChanges();

            cboStoreOrganisation.trackChanges();        
            cboStoreOrganisation.clearSelection();
            cboStoreOrganisation.clearItems();
            cboStoreOrganisation.commitChanges();
            
            return false;
        }
        
    //-->
    </script>

    <script language="javascript" type="text/javascript">
        var refusals = "";
    
        var orders = "";
        var ordersSelectedOnDeliveryJob = "";
        var ordersOnDeliveryJobCrossDockPointIds = "";
        
        var defaultGroupageCollectionRunDeliveryPointId = "<%=(GroupageCollectionRunDeliveryPoint == null) ? "" : GroupageCollectionRunDeliveryPoint.PointId.ToString()%>";
        var defaultGroupageCollectionRunDeliveryPointDescription = "<%=GroupageCollectionRunDeliveryPoint.Description %>";
        
        var hidShowConfirmForOrderAfterDays = $get("<%=hidShowConfirmForOrderAfterDays.ClientID %>");
        
        var hidPostBackOnSuccess = $get("<%=btnFind.ClientID %>");
        
        var jobs = "";
        var isUpdating = false;
        var groupHandlingIsActive = false;
        
        var mtv = null;
        var menu = null;
        
        var rdCollectionIsAnytime = null, rdLeaveIsAnytime = null, rdDeliveryIsAnytime = null;
        var dteCollectionFromDate = null, dteCollectionFromTime = null, dteCollectionByDate = null, dteCollectionByTime = null;
        var dteLeaveFromDate = null, dteLeaveFromTime = null, dteLeaveByDate = null, dteLeaveByTime = null;
        var dteDeliveryFromDate = null, dteDeliveryFromTime = null, dteDeliveryByDate = null, dteDeliveryByTime = null;
        
        var dteManifestDate = null;
        var rcbBusinessType = null;
        var cboSubContractor = null;
        var txtSubContractRate = -1;
        var cboDriver = null;
        
        var totalWeight = <%=Weight.ToString() %>;
        var totalPallets = <%=NoPallets.ToString() %>;
        var totalSpaces = <%=NoPalletSpaces.ToString() %>;              
        var userID = <%=((Page.User) as Orchestrator.Entities.CustomPrincipal).IdentityId %>;
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        
        function DriverOnClientBlur(sender, eventArgs) {
            $('#txtManifestTitle').val(sender.get_text() + " - " + dteManifestDate.get_displayValue());
        }
        
        function CollectionPointAlterPosition(pointId) {
            var qs = "pointId=" + pointId;
            <%=dlgAddUpdatePointGeofence.ClientID %>_Open(qs);
        }
   
        function pageLoad()
        {
            mtv = $find("<%=grdRefusals.ClientID %>").get_masterTableView();
            
            dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID%>");
            dteCollectionFromTime = $find("<%=dteCollectionFromTime.ClientID%>");
            dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID%>");
            dteCollectionByTime = $find("<%=dteCollectionByTime.ClientID%>");
            
            dteLeaveFromDate = $find("<%=dteLeaveFromDate.ClientID%>");
            dteLeaveFromTime = $find("<%=dteLeaveFromTime.ClientID%>");
            dteLeaveByDate = $find("<%=dteLeaveByDate.ClientID%>");
            dteLeaveByTime = $find("<%=dteLeaveByTime.ClientID%>");
            
            dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
            dteDeliveryFromTime = $find("<%=dteDeliveryFromTime.ClientID %>");
            dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");
            dteDeliveryByTime = $find("<%=dteDeliveryByTime.ClientID %>");
            
            rdDeliveryIsAnytime = $('#' + '<%=rdDeliveryIsAnytime.ClientID %>');
            rdLeaveIsAnytime = $('#' + '<%=rdLeaveIsAnytime.ClientID %>');
            rdCollectionIsAnytime = $('#' + '<%=rdCollectionIsAnytime.ClientID %>');
            
            dteManifestDate = $find("<%=dteManifestDate.ClientID%>");
            
            rcbBusinessType = $find("<%=rcbBusinessType.ClientID %>");
            
            cboSubContractor = $find("<%=cboSubContractor.ClientID %>");
            
            txtSubContractRate = $find("<%=txtSubbyRate.ClientID %>");
            
            cboDriver = $find('ctl00_ContentPlaceHolder1_ucResource_cboDriver');
            cboDriver.add_selectedIndexChanged(DriverOnClientBlur);
            
            var collectionMethod = $('input:hidden[id*=hidCollectionTimingMethod]').val();
            var collectionMethod = $('input:hidden[id*=hidLeaveTimingMethod]').val();
            
            if (collectionMethod == 'anytime') {
                collectionIsAnytime(null);
            }

            if (collectionMethod == 'timed') {
                collectionTimedBooking(null);
            }
            
            if (collectionMethod == 'anytime') {
                collectionIsAnytime(null);
            }

            if (collectionMethod == 'timed') {
                collectionTimedBooking(null);
            }
        }
        
        function ChangeDelivery(chk) {
            if (chk.checked == true)
                $('[id*=ucDeliveryPoint]').prop("disabled", false);
            else 
                $('[id*=ucDeliveryPoint]').prop("disabled", true);
        }
        
        function manifestGeneration_CallBack(object, value)
        {
            if (value.length > 0 && $('#chkCreateLoadingSheet')[0].checked) {
                PageMethods.GenerateAndShowLoadingSheet(value, LoadingSheet_Success, LoadingSheet_Failure);
            }
            else
                hidPostBackOnSuccess.click();
        }
        
        function ManifestGeneration_Success(result) {
            var parts = result.toString().split(",");
            var qs = "jID=" + parts[0];
            <%=dlgManifestGeneration.ClientID %>_Open(qs);
        }

        function ManifestGeneration_Failure(error) {
            alert("Something went wrong when creating the Manifest.");
        }
        
        function LoadingSheet_Success(result) {
            <%=dlgLoadingSheet.ClientID %>_Open();
            hidPostBackOnSuccess.click();
        }

        function LoadingSheet_Failure(error) {
            alert("Something went wrong when creating the Loading Sheet.");
        }
        
        function DisplayManifestLinkWindow(jobID, rmID, excludeFirstRow, usePlannedTimes, extraRows, showFullAddress)
        {
            var modalPopupBehavior = $find('programmaticModalPopupBehavior');
            modalPopupBehavior.show();
        }
        
        function NextAction() {
            hidPostBackOnSuccess.click();
        }
        
        $(document).ready(function() {
            $('#<%=grdRefusals.ClientID %>_ctl00 tbody tr:not(.rgGroupHeader)').quicksearch({
                position: 'after',
                labelText: '',
                attached: '#grdFilterHolder',
                delay: 100
            });
            
            var checked = $(':checkbox[id*=chkShowAll]').prop("checked");
            $(':checkbox[id*=chkIncludeOrdersNotPlannedForCollection]').prop("disabled", checked);
            $(':checkbox[id*=chkIncludeOrdersXDockedAtOwnCompany]').prop("disabled", checked);
            $(':checkbox[id*=chkIncludeOrdersXDockedAtOtherCompany]').prop("disabled", checked);
            
            $(':checkbox[id*=chkShowAll]').click(function (index, ele) {
                var checked = $(this).prop("checked");
                $(':checkbox[id*=chkIncludeOrdersNotPlannedForCollection]').prop("disabled", checked);
                $(':checkbox[id*=chkIncludeOrdersXDockedAtOwnCompany]').prop("disabled", checked);
                $(':checkbox[id*=chkIncludeOrdersXDockedAtOtherCompany]').prop("disabled", checked);
            });
                
            if ($('input:radio[id*=rdCollectionTimedBooking]')[0].checked == true)
                $('tr[id*=trCollectBy]').hide();

            if ($('input:radio[id*=rdCollectionIsAnytime]')[0].checked == true)
                $('tr[id*=trCollectBy]').hide();

            if ($('input:radio[id*=rdCollectionBookingWindow]')[0].checked == true)
                $('tr[id*=trCollectBy]').show();
                
            if ($('input:radio[id*=rdLeaveTimedBooking]')[0].checked == true)
                $('tr[id*=trLeaveBy]').hide();

            if ($('input:radio[id*=rdLeaveIsAnytime]')[0].checked == true)
                $('tr[id*=trLeaveBy]').hide();

            if ($('input:radio[id*=rdLeaveBookingWindow]')[0].checked == true)
                $('tr[id*=trLeaveBy]').show();
                
            if ($('input:radio[id*=rdDeliveryTimedBooking]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').hide();

            if ($('input:radio[id*=rdDeliveryIsAnytime]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').hide();

            if ($('input:radio[id*=rdDeliveryBookingWindow]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').show();
            
            showTotals(false);
        });
        
        function manifestGeneration_CallBack(object, value)
        {
            if (value.length > 0 && $('#chkCreateLoadingSheet')[0].checked) {
                PageMethods.GenerateAndShowLoadingSheet(value, LoadingSheet_Success, LoadingSheet_Failure);
            }
            else
                hidPostBackOnSuccess.click();
        }
        
        function ManifestGeneration_Success(result) {
            var parts = result.toString().split(",");
            var qs = "jID=" + parts[0];
            <%=dlgManifestGeneration.ClientID %>_Open(qs);
        }

        function ManifestGeneration_Failure(error) {
            alert("Something went wrong when creating the Manifest.");
        }
        
        function LoadingSheet_Success(result) {

            <%=dlgLoadingSheet.ClientID %>_Open('rpk=' + result);
            hidPostBackOnSuccess.click();
        }

        function LoadingSheet_Failure(error) {
            alert("Something went wrong when creating the Loading Sheet.");
        }
                    
    </script>
    
    <script type="text/javascript" language="javascript">
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
            var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
            var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");

            if (dteCollectionFromDate != null && sender.get_selectedDate() < dteCollectionFromDate.get_selectedDate() && rdCollectionBookingWindow != null && rdCollectionBookingWindow.prop("checked"))
                dteCollectionFromDate.set_selectedDate(sender.get_selectedDate());
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

        function leaveIsAnytime(rb) {
            $('tr[id*=trLeaveBy]').hide();

            var dteCollectionFromDate = $('input[id*=dteLeaveFromDate]');
            if (rb != null) {
                dteCollectionFromDate.focus();
                dteCollectionFromDate.select();
            }

            $('input:hidden[id*=hidLeaveTimingMethod]').val('anytime');

            $('input:hidden[id*=hidLeaveFromTimeRestoreValue]').val($find("<%=dteLeaveFromTime.ClientID %>").get_value());
            $('input:hidden[id*=hidLeaveByTimeRestoreValue]').val($find("<%=dteLeaveByTime.ClientID %>").get_value());

            $find("<%=dteLeaveFromTime.ClientID %>").set_value('00:00');
            $find("<%=dteLeaveFromTime.ClientID %>").disable();

            $find("<%=dteLeaveByTime.ClientID %>").set_value('23:59');
            $find("<%=dteLeaveByTime.ClientID %>").disable();
        }

        function leaveTimedBooking(rb) {
            $('tr[id*=trLeaveBy]').hide();

            var dteLeaveFromDate = $('input[id*=dteLeaveFromDate]');
            if (rb != null) {
                dteLeaveFromDate.focus();
                dteLeaveFromDate.select();
            }

            var method = $('input:hidden[id*=hidLeaveTimingMethod]').val();

            $('input:hidden[id*=hidLeaveTimingMethod]').val('timed');

            if (method == 'window') {
                $('input:hidden[id*=hidLeaveByTimeRestoreValue]').val($find("<%=dteLeaveByTime.ClientID %>").get_value());
            }

            $find("<%=dteLeaveByTime.ClientID %>").set_value($find("<%=dteLeaveFromTime.ClientID %>").get_value());

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidLeaveFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteLeaveFromTime.ClientID %>").set_value($('input:hidden[id*=hidLeaveFromTimeRestoreValue]').val());

                } else {
                $find("<%=dteLeaveFromTime.ClientID %>").set_value("08:00");
                }

                if ($('input:hidden[id*=hidLeaveByTimeRestoreValue]').val() != "") {
                    $find("<%=dteLeaveByTime.ClientID %>").set_value($('input:hidden[id*=hidLeaveByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteLeaveByTime.ClientID %>").set_value("17:00");
                }
            }
            $find("<%=dteLeaveByTime.ClientID %>").enable();
            $find("<%=dteLeaveFromTime.ClientID %>").enable();
        }

        function leaveBookingWindow(rb) {
            $('tr[id*=trLeaveBy]').show();
            var dteLeaveFromDate = $('input[id*=dteLeaveFromDate]');
            if (rb != null) {
                dteLeaveFromDate.focus();
                dteLeaveFromDate.select();
            }

            var method = $('input:hidden[id*=hidLeaveTimingMethod]').val();

            $('input:hidden[id*=hidLeaveTimingMethod]').val('window');

            if ($('input:hidden[id*=hidLeaveByTimeRestoreValue]').val() != "") {
                $find("<%=dteLeaveByTime.ClientID %>").set_value($('input:hidden[id*=hidLeaveByTimeRestoreValue]').val());
            }

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidLeaveFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteLeaveFromTime.ClientID %>").set_value($('input:hidden[id*=hidLeaveFromTimeRestoreValue]').val());

                } else {
                    $find("<%=dteLeaveFromTime.ClientID %>").set_value("08:00");
                }

                if ($('input:hidden[id*=hidLeaveByTimeRestoreValue]').val() != "") {
                    $find("<%=dteLeaveByTime.ClientID %>").set_value($('input:hidden[id*=hidLeaveByTimeRestoreValue]').val());
                } else {
                    $find("<%=dteLeaveByTime.ClientID %>").set_value("17:00");
                }
            }

            $find("<%=dteLeaveByTime.ClientID %>").enable();
            $find("<%=dteLeaveFromTime.ClientID %>").enable();
        }

        function dteLeaveFromDate_SelectedDateChanged(sender, eventArgs) {
            var dteLeaveFromDate = $find("<%=dteLeaveFromDate.ClientID %>");
            var rdLeaveBookingWindow = $("#" + "<%=rdLeaveBookingWindow.ClientID%>");
            var updateDate = false;

            if (dteLeaveFromDate != null) {
                if (rdLeaveBookingWindow != null && !rdLeaveBookingWindow.prop("checked"))
                    updateDate = true;
                else if (sender.get_selectedDate() > dteLeaveByDate.get_selectedDate())
                    updateDate = true;
            }

            if (updateDate) {
                dteLeaveByDate.set_selectedDate(sender.get_selectedDate());
            }
        }

        function dteLeaveByDate_SelectedDateChanged(sender, eventArgs) {
            var dteLeaveFromDate = $find("<%=dteLeaveFromDate.ClientID %>");
            var rdLeaveBookingWindow = $("#" + "<%=rdLeaveBookingWindow.ClientID%>");

            if (dteLeaveFromDate != null && sender.get_selectedDate() < dteLeaveFromDate.get_selectedDate() && rdLeaveBookingWindow != null && rdLeaveBookingWindow.prop("checked"))
                dteLeaveFromDate.set_selectedDate(sender.get_selectedDate());
        }

        function CV_ClientValidateLeaveDate(source, args) {
            var dteDateTime = $find("<%=dteLeaveFromDate.ClientID%>");
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

        FilterOptionsDisplayHide();
    </script>
        
</asp:Content>