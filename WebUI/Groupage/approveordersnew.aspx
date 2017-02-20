<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="approveordersnew.aspx.cs"
    Inherits="Orchestrator.WebUI.Groupage.approveordersnew" MasterPageFile="~/default_tableless.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script type="text/javascript" src="approveordersnew.aspx.js"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <style type="text/css">
        .DeliveryNotes
        {
            width: 100%;
            height: 100%;
            font-size: 10px;
            border: 0px;
        }
        .bookInWindow
        {
            position: absolute;
            z-index: 10;
            width: 390px;
            height: 150px;
            text-align: center;
            font: 14px Verdana, Arial, Helvetica, sans-serif;
            background: url(../images/white.png);
            padding: 12px 10px 12px 17px;
            display: none;
        }
        .titleBar
        {
            text-align: left;
            font-weight: bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="HeaderPlaceHolder1">
    <h1>Approve orders</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <fieldset>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Date From
                    </td>
                    <td class="formCellInput">
                        <telerik:RadDatePicker  Width="100"
                            runat="server" ID="dteStartDate">
                            <DateInput runat="server"
                            DateFormat="dd/MM/yy" 
                            DisplayDateFormat="dd/MM/yy">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Date To
                    </td>
                    <td class="formCellInput">
                        <telerik:RadDatePicker  Width="100"
                            runat="server" ID="dteEndDate">
                            <DateInput runat="server"
                            DateFormat="dd/MM/yy" 
                            DisplayDateFormat="dd/MM/yy">
                            </DateInput>
                        </telerik:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Client
                    </td>
                    <td class="formCellInput" colspan="2">
                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                            MarkFirstMatch="false" AllowCustomText="False" ShowMoreResultsBox="false" Width="355px"
                            Height="300px" OnClientItemsRequesting="cboClient_itemsRequesting">
                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                        </telerik:RadComboBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnSearch" runat="server" Text="Search" />
            <input type="button" id="btnCancelSearch" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide();" />
        </div>
    </div>
    
    <div id="divRejectReason" class="overlayedRejectBox" style="display: none;">
        <fieldset>
            <legend>Reject orders</legend>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Reason for rejection
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboRejectionReason" runat="server" AppendDataBoundItems="true" Width="250px">
                            <asp:ListItem Value="" Text="- select -" />
                        </asp:DropDownList>
                        <asp:TextBox ID="txtRejectionReason" runat="server" TextMode="MultiLine" Style="width: 250px" Rows="3" />
                        <div>
                            <asp:CustomValidator ID="cvRejectionReason" runat="server" ValidationGroup="vgRejectOrders" ClientValidationFunction="validateRejectionReason" Display="Dynamic" ErrorMessage="Please supply a rejection reason" />
                        </div>
                    </td>
                </tr>
            </table>
        </fieldset>
        
        <div class="buttonBar">
            <asp:Button ID="btnRejectOrders" runat="server" Text="Reject Orders" CausesValidation="true" ValidationGroup="vgRejectOrders" OnClientClick="rejectOrders(); return false;" />
            <asp:Button ID="btnHiddenRejectOrders" runat="server" style="display: none;" CausesValidation="true" ValidationGroup="vgRejectOrders" />
            <input type="button" value="Cancel" onclick="$('#divRejectReason').hide(); return false;" causesvalidation="false" />
        </div>
        <br />
    </div>
    
    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
        <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
        <div class="overlayedApproveIcon">
            <asp:Button ID="btnConfirmOrders" runat="server" Text="Approve Order(s)" CausesValidation="false" /></div>
        <div class="overlayedRejectIcon">
            <input type="button" value="Reject Order(s)" onclick="ShowRejectReason();" /></div>
        <div class="overlayedSaveIcon">
            <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" />
        </div>
        <div class="commandDataRow">
            <span class="commandDataLabel">Number of Orders:</span> <span class="commandDataField">
                <asp:Label ID="lblOrderCount" runat="server"></asp:Label></span> <span class="commandDataLabel">
                    Total Pallet Count:</span> <span class="commandDataField">
                        <asp:Label ID="lblPalletCount" runat="server"></asp:Label>
                        (<span id="spnSelectedPallets">0</span>)</span> <span class="commandDataLabel">Dirty
                            Rows:</span> <span class="commandDataField" id="spnDirtyOrders">0</span>
            <span class="commandDataLabel">Orders to Approve:</span> <span class="commandDataField"
                id="spnOrdersToApprove">0</span> <span id="spnSaving" style="display: none;">
                    <img src="/images/spinner.gif" alt="Saving changes" />
                    Saving your changes.</span>
        </div>
    </div>
    
    <telerik:RadGrid runat="server" ID="grdOrders" AllowFilteringByColumn="false" AllowMultiRowSelection="true" AllowSorting="False"
        AutoGenerateColumns="false" EnableAjaxSkinRendering="true">
        <MasterTableView DataKeyNames="OrderID,OrderGroupID">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="chkSelectColumn" HeaderStyle-Width="2%" AllowFiltering="false">
                    <HeaderTemplate>
                        <input type="checkbox" id="chkSelectAll" onclick="javascript:selectAllCheckboxes(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                        <input type="hidden" runat="server" id="hidOrderId" />
                        <asp:HiddenField runat="server" ID="hidOrderChanged" Value="false" />
                        <asp:HiddenField ID="hidOrderGroupId" runat="server" Value='<%#Eval("OrderGroupID") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="Client" HeaderStyle-Width="7%" DataField="CustomerOrganisationName"
                    HeaderText="Client" AllowFiltering="true" />
                <telerik:GridTemplateColumn UniqueName="OrderID" HeaderText="ID" HeaderStyle-Width="3%">
                    <ItemTemplate>
                        <a href="#" onclick='viewOrderProfile(<%#Eval("OrderID") %>); return false;' target="_blank">
                            <%#Eval("OrderID") %>
                        </a>
                        <img style="padding-left: 2px;" runat="server" id="imgOrderBookedIn" visible="false"
                            src="/App_Themes/Orchestrator/Img/MasterPage/icon-tick-small.png" alt="Booked In" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="LoadNo" HeaderStyle-Width="7%" DataField="CustomerOrderNumber" HeaderText="Load No"
                    AllowFiltering="true" />
                <telerik:GridTemplateColumn UniqueName="RequiresBookingIn" HeaderText="RB" HeaderStyle-Width="2%">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkSetRequireBookIn" runat="server" Text="Set" NavigateUrl="#"></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="CollectFromOrganisation" HeaderStyle-Width="7%" HeaderText="Collect From Client"
                    AllowFiltering="true">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblCollectFromOrganisation"></asp:Label>
                        <asp:PlaceHolder ID="litViewCollectionOrganisation" runat="server"></asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="CollectFrom" HeaderStyle-Width="8%" HeaderText="Collect From">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="litViewCollectionPoint" runat="server"></asp:PlaceHolder>
                        <asp:Label runat="server" ID="lblCollectFromPoint"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="CollectAt1" HeaderStyle-Width="8%" HeaderText="Collect At" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <div id="collectionTime">
                            <asp:RadioButtonList ID="rblCollectionTimeOptions" runat="server" EnableViewState="false"
                                RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Text="W" Value="0"></asp:ListItem>
                                <asp:ListItem Text="T" Value="1"></asp:ListItem>
                                <asp:ListItem Text="A" Value="2"></asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <asp:TextBox ID="txtCollectAtDate" runat="server" Width="45%" EnableViewState="false" />
                            <asp:TextBox ID="txtCollectAtTime" runat="server" Width="45%" EnableViewState="false" /><br />
                            <asp:TextBox ID="txtCollectByDate" runat="server" Width="45%" EnableViewState="false" />
                            <asp:TextBox ID="txtCollectByTime" runat="server" Width="45%" EnableViewState="false" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="DeliverToOrganisation" HeaderStyle-Width="7%" HeaderText="Deliver To Client"
                    AllowFiltering="true">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblDeliverToOrganisation"></asp:Label>
                        <asp:PlaceHolder ID="litViewDeliveryOrganisation" runat="server"></asp:PlaceHolder>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="DeliverTo" HeaderText="Deliver To"  HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <asp:PlaceHolder ID="litViewDeliveryPoint" runat="server"></asp:PlaceHolder>
                        <asp:Label runat="server" ID="litDeliverTo"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="DeliverAt" HeaderText="Deliver At" ItemStyle-Wrap="false"
                     HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <div id="deliveryTime">
                            <asp:RadioButtonList ID="rblDeliveryTimeOptions" runat="server" EnableViewState="false"
                                RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Text="W" Value="0"></asp:ListItem>
                                <asp:ListItem Text="T" Value="1"></asp:ListItem>
                                <asp:ListItem Text="A" Value="2"></asp:ListItem>
                            </asp:RadioButtonList>
                            <br />
                            <asp:TextBox ID="txtDeliverAtFromDate" runat="server" Width="45%" EnableViewState="false" />
                            <asp:TextBox ID="txtDeliverAtFromTime" runat="server" Width="45%" EnableViewState="false" /><br />
                            <asp:TextBox ID="txtDeliverByFromDate" runat="server" Width="45%" EnableViewState="false" />
                            <asp:TextBox ID="txtDeliverByFromTime" runat="server" Width="45%" EnableViewState="false" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="DeliveryOrderNumber" HeaderStyle-Width="7%" DataField="DeliveryOrderNumber"
                    HeaderText="Del Num" AllowFiltering="false" />
                <telerik:GridBoundColumn UniqueName="NoOfPallets" DataField="NoPallets" HeaderText="Plts"
                    ItemStyle-HorizontalAlign="Right" AllowFiltering="false" HeaderStyle-Width="2%" />
                <telerik:GridBoundColumn UniqueName="Weight" HeaderText="Kg" DataField="Weight" DataFormatString="{0:N0}"
                    ItemStyle-HorizontalAlign="Right" SortExpression="Weight" HeaderStyle-Width="2%"
                    AllowFiltering="false" />
                <telerik:GridTemplateColumn UniqueName="ForiegnRate1" HeaderText="Rate" ItemStyle-HorizontalAlign="Right"
                    SortExpression="ForeignRate" AllowFiltering="false" HeaderStyle-Width="4%">
                    <ItemTemplate>
                        <asp:TextBox ID="txtRate" runat="server" Width="50px" EnableViewState="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Delivery Notes" HeaderStyle-Width="8%">
                    <ItemTemplate>
                        <asp:TextBox ID="txtDeliveryNotes" runat="server" TextMode="MultiLine" Rows="4" CssClass="DeliveryNotes"
                            Text='<%#Eval("DeliveryNotes") %>'></asp:TextBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="RequestingDepot" HeaderText="Req Depot" HeaderStyle-Width="3%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn UniqueName="OrderServiceLevelShortCode" DataField="OrderServiceLevelShortCode"
                    HeaderText="SL" HeaderStyle-Width="3%" AllowFiltering="false" />
                <telerik:GridBoundColumn UniqueName="Surcharges" DataField="Surcharges" HeaderText="SC"
                    AllowFiltering="false" AllowSorting="false" HeaderStyle-Width="6%">
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
            <Scrolling AllowScroll="true" ScrollHeight="510" UseStaticHeaders="true" />
            <Resizing AllowColumnResize="true" AllowRowResize="false" EnableRealTimeResize="false" ResizeGridOnColumnResize="true" ClipCellContentOnResize="true" />
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

    <asp:HiddenField ID="hidOrderIDs" runat="server" />
    <asp:HiddenField ID="hidApproveOrderIDs" runat="server" />

    <div id="bookInWindow" class="bookInWindow">
        <table style="text-align: left;">
            <tr>
                <td colspan="2">
                    <div class="titleBar">
                        Book in details</div>
                </td>
            </tr>
            <tr>
                <td style="width: 120px;">
                    Booked In With
                </td>
                <td style="text-align: left;">
                    <input type="text" style="width: 150px;" id="txtBookedInWith" />
                </td>
            </tr>
            <tr>
                <td>
                    References
                </td>
                <td style="text-align: left;">
                    <input type="text" style="width: 200px;" id="txtBookedInReferences" />
                </td>
            </tr>
            <tr>
                <td>
                    Booked in for
                </td>
                <td>
                    <div id="bookedInTime">
                        <input type="radio" id="optTimeW" name="BookInTime" value="0" checked="checked" />Window
                        <input type="radio" id="optTimeT" name="BookInTime" value="1" />Timed
                        <br />
                        <asp:TextBox ID="txtBookInFromDate" runat="server" Width="70px" EnableViewState="false" />
                        <asp:TextBox ID="txtBookInFromTime" runat="server" Width="40px" EnableViewState="false" />
                        <asp:TextBox ID="txtBookInByFromDate" runat="server" Width="70px" EnableViewState="false" />
                        <asp:TextBox ID="txtBookInByFromTime" runat="server" Width="40px" EnableViewState="false" />
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="buttonbar">
                        <input type="button" value="Save" id="btnSaveBookIn" onclick="BookIn(this);" />
                        <input type="button" value="Cancel" onclick="$('#bookInWindow').hide();" />
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
        var hidApproveOrderIDs = "#<%=hidApproveOrderIDs.ClientID %>";
        var dteBookInFromDate = "#<%=txtBookInFromDate.ClientID %>";
        var dteBookInFromTime = "#<%=txtBookInFromTime.ClientID %>";
        var dteBookInByFromDate = "#<%=txtBookInByFromDate.ClientID %>";
        var dteBookInByFromTime = "#<%=txtBookInByFromTime.ClientID %>";
        var cboRejectionReason = "#<%=cboRejectionReason.ClientID %>";
        var txtRejectionReason = "#<%=txtRejectionReason.ClientID %>";
    </script>

</asp:Content>
