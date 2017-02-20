<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.Groupage.FlagForInvoicing" MasterPageFile="~/default_tableless.Master" Title="Flag Orders For Invoicing" CodeBehind="FlagForInvoicing.aspx.cs" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Import Namespace="System.Data" %>
<%@ Register TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">

    <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
        Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>

    <script type="text/javascript" language="javascript" src="/script/tooltippopups.js"></script>

    <script language="javascript" type="text/javascript">

        function txtOrderRate_OnChange(sender) {
            var currentItem = $('#' + sender.id);
        }

        var triggerGroupedOrderSelection = true;

        function HandleGroupSelection(orderGroupID) {
            if (!triggerGroupedOrderSelection)
                return;

            triggerGroupedOrderSelection = false;

            $(":checkbox[id$=chkOrder]").each(function (chkIndex) {
                if (!this.checked && this.getAttribute('OrderGroupID') == orderGroupID) {
                    this.click();
                }
            });

            triggerGroupedOrderSelection = true;
        }

        function HandleGroupDeselection(orderGroupID) {
            if (!triggerGroupedOrderSelection)
                return;

            triggerGroupedOrderSelection = false;

            $(":checkbox[id$=chkOrder]").each(function (chkIndex) {
                if (this.checked && this.getAttribute('OrderGroupID') == orderGroupID) {
                    this.click();
                }
            });

            triggerGroupedOrderSelection = true;
        }

        function GetCheckBox(control) {

            if (!control) return;

            for (var i = 0; i < control.childNodes.length; i++) {
                if (!control.childNodes[i].tagName) continue;
                if ((control.childNodes[i].tagName.toLowerCase() == "input") &&
                      (control.childNodes[i].type.toLowerCase() == "checkbox")) {
                    return control.childNodes[i];
                }
            }
        }

        function selectAllCheckboxes(chk) {
            triggerGroupedOrderSelection = false;

            $('table[id*=grdOrders] input:enabled[id*=chkOrder]').each(function () {
                if (this.checked != chk.checked) {
                    this.click();
                }
            });

            triggerGroupedOrderSelection = true;
        };

        function ClickRowCheckBox(chk) {
            //Let RowClick Handle the click.
            chk.checked = !chk.checked;

            //Mock up the sender object.
            var sender = $find(chk.parentElement.parentElement.parentElement.parentElement.parentElement.id);
            var gridDataItem = $find(chk.parentElement.parentElement.id);
            var args = {

                get_itemIndexHierarchical: function () {
                    return chk.attributes["index"].value;
                },
                get_gridDataItem: function(){
                    return gridDataItem;
                }
            };

            RowClick(sender, args);
        }

        function viewJobDetails(jobID) {

            var url = '<%=Page.ResolveUrl("~/job/job.aspx")%>?wiz=true&jobId=' + jobID + getCSID();

            openResizableDialogWithScrollbars(url, '1220', '870');
        }

        function selectAllBusinessTypes(sender) {
            $('input:checkbox[id*=cblBusinessType]').prop('checked', $(sender).prop('checked'));
        }

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

        var clickedItemIndex;
        var selected;

        function GridCreated(sender, args) {
            selected = new Array(sender.get_masterTableView().get_dataItems().length);
        }

        function RowClick(sender, args) {
            if (args.get_gridDataItem().get_element().getAttribute("disabled") !== "disabled") {

                var master = sender.get_masterTableView();
                var index = args.get_itemIndexHierarchical();
                var row = master.get_dataItems()[index];
                selected[index] = !row.get_selected()
                    
                row.set_selected(!row.get_selected());

                var checkbox = row._element.children[0].children[0];
                checkbox.checked = !checkbox.checked;

                if (triggerGroupedOrderSelection) {
                    clickedItemIndex = index;
                    var orderGroupID = checkbox.attributes["orderGroupID"].value;

                    if (orderGroupID > 0) {
                        if (checkbox.checked)
                            HandleGroupSelection(orderGroupID);
                        else
                            HandleGroupDeselection(orderGroupID);
                    }
                }
            }
        }

        function RowDeselecting(sender, args) {
            var index = args.get_itemIndexHierarchical();
            if (clickedItemIndex != index && selected[index]) {
                args.set_cancel(true);
            }
        }

        function RowSelecting(sender, args) {
            var index = args.get_itemIndexHierarchical();

            if (args.get_gridDataItem().get_element().getAttribute("disabled") === "disabled") {
                args.set_cancel(true);
            }
            else
            {
                if (clickedItemIndex == index && !selected[index]) {
                    args.set_cancel(true);
                }
            }
        }

        function OpenPODWindow(orderID) {
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&OrderID=" + orderID;

            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }

    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Flag Orders as Ready To Invoice</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <cc1:Dialog ID="dlgOrder" URL="/groupage/ManageOrder.aspx" Height="900" Width="1200" AutoPostBack="false" runat="server" Mode="Modal" ReturnValueExpected="false"></cc1:Dialog>
    
    <telerik:RadWindowManager ID="rmwFlagForInvoicing" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="600" Width="900" />
        </Windows>
    </telerik:RadWindowManager>

    <asp:Panel ID="pnlDefaults" runat="server" DefaultButton="btnRefresh">
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
                        <asp:Button ID="btnEditRates" runat="server" Text="Edit Rates" CausesValidation="false" />
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <fieldset>
            <legend>Filter Options</legend>
            <asp:RadioButtonList ID="rdoListOrderTypes" runat="server" RepeatDirection="Horizontal"
                RepeatColumns="3">
                <asp:ListItem Text="View Unflagged Orders" Value="Unflagged" Selected="True" />
                <asp:ListItem Text="View Flagged Orders" Value="Flagged" />
            </asp:RadioButtonList>
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions" style="vertical-align: top;">
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100" ToolTip="Please enter a start date for the filter.">
                                        <DateInput runat="server"
                                            DateFormat="dd/MM/yy">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate"
                                        Display="Dynamic" ToolTip="Please enter a start date for the filter.">
                                        <img id="imgReqStartDate" runat="server" src="/images/error.png" alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Date To
                                </td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100" ToolTip="Please enter an end date for the filter." >
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate"
                                        Display="Dynamic" ToolTip="Please enter an end date for the filter.">
                                        <img id="imgReqEndDate" runat="server" src="/images/error.png" alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Client
                                </td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px" DataValueField="IdentityID"
                                        DataTextField="OrganisationName">
                                    </telerik:RadComboBox>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Type
                                </td>
                                <td class="formCellField">
                                    <asp:RadioButtonList ID="rdoListInvoiceType" runat="server" RepeatDirection="Horizontal"
                                        RepeatColumns="3">
                                        <asp:ListItem Text="Normal" Value="NORM" Selected="True" />
                                        <asp:ListItem Text="Self Bill" Value="SELF" />
                                        <asp:ListItem Text="Both" Value="BOTH" />
                                    </asp:RadioButtonList>
                                </td>
                                <td>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Order
                                </td>
                                <td class="formCellField">
                                    <asp:RadioButtonList ID="rblJobs" runat="server" RepeatDirection="Horizontal" AutoPostBack="false">
                                        <asp:ListItem Text="All" Value="0" Selected="True" />
                                        <asp:ListItem Text="With Collection Job" Value="1" />
                                        <asp:ListItem Text="No Collection Job" Value="2" />
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Pricing
                                </td>
                                <td class="formCellField">
                                    <asp:RadioButtonList ID="rblPricing" runat="server" RepeatDirection="Horizontal"
                                        RepeatColumns="4" AutoPostBack="false">
                                        <asp:ListItem Text="All" Value="0" Selected="True" />
                                        <asp:ListItem Text="Auto" Value="1" />
                                        <asp:ListItem Text="Spot" Value="2" />
                                        <asp:ListItem Text="Zero Rated" Value="3" />
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Show Excluded Client's Invoices
                                </td>
                                <td class="formCellField">
                                    <asp:CheckBox runat="server" ID="chkShowExcluded" Checked="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                    <td style="vertical-align: top;">
                        <table>
                            <tr>
                                <td class="formCellLabel">
                                    Business Type
                                </td>
                                <td class="formCellField">
                                    <input type="checkbox" id="chkSelectAllBusinessTypes" onclick="selectAllBusinessTypes(this);" checked='true' /><label for="chkSelectAllBusinessTypes">Select All</label>
                                    <asp:CheckBoxList ID="cblBusinessType" runat="server" RepeatDirection="Horizontal"
                                        RepeatColumns="6" AutoPostBack="false" DataTextField="Description" DataValueField="BusinessTypeID">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Service Level
                                </td>
                                <td class="formCellField">
                                    <asp:CheckBoxList ID="cblServiceLevel" runat="server" RepeatDirection="Horizontal"
                                        RepeatColumns="3" AutoPostBack="false" DataTextField="Description" DataValueField="OrderServiceLevelID">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
                <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" />
        </div>
        </div>

    </asp:Panel>
    
    <asp:Label ID="lblError" runat="server"></asp:Label>
    
    <fieldset style="margin: 10px 0; padding: 2px;">
        <div style="padding: 5px; width: 140px; float: left; text-align: center;">Key</div>
        <div style="padding: 5px; width: 140px; float: left; text-align: center; background-color: #f2dfdf;">Has problem - no POD</div>
        <div style="padding: 5px; width: 300px; float: left; text-align: center; background-color: #ffc2b2;">Client is On Hold and/or Excluded From Invoicing</div>
        <div class="clearDiv"></div>
    </fieldset>
    
    <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" AllowMultiRowEdit="true"
        AutoGenerateColumns="false" AllowMultiRowSelection="true">
        <MasterTableView Width="100%" ClientDataKeyNames="OrderID,OrderGroupID" DataKeyNames="OrderID,OrderGroupID"
            Name="Master" EditMode="InPlace">
            <RowIndicatorColumn Display="false" />
            <Columns>

                <telerik:GridTemplateColumn  UniqueName="chkSelectColumn" HeaderStyle-Width="28" AllowFiltering="false">
                    <HeaderTemplate>
                        <input type="checkbox" id="chkSelectAll" onclick="javascript: selectAllCheckboxes(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <input runat="server" type="checkbox" id="chkOrder" onclick="javascript: ClickRowCheckBox(this);" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn HeaderText="Client" SortExpression="OrganisationName" DataField="OrganisationName" ReadOnly="true"
                    ItemStyle-Wrap="false" HeaderStyle-Width="140" />
                <telerik:GridTemplateColumn HeaderText="ID" SortExpression="OrderID" UniqueName="OrderID" HeaderStyle-Width="55">
                    <ItemTemplate>
                        <a runat="server" id="hypOrder" href="">
                            <%#((System.Data.DataRowView)Container.DataItem)["OrderID"].ToString() %></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Service Level" SortExpression="OrderServiceLevel" ReadOnly="true"
                    DataField="OrderServiceLevel" ItemStyle-Wrap="true" HeaderStyle-Width="70"/>
                <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="Rate"  HeaderStyle-Width="70" UniqueName="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblCharge" runat="server"></asp:Label>
                        &nbsp;<span id="spnCharge" runat="server"><%#((System.Data.DataRowView)Container.DataItem)["Rate"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((System.Data.DataRowView)Container.DataItem)["Rate"]).ToString("C")%></span>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <input ID="txtOrderRate" style="width:75px" type="text" runat="server" onchange="txtOrderRate_OnChange(this);" />
                    </EditItemTemplate>
                    <ItemStyle HorizontalAlign="Right" />
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Pallets" SortExpression="PalletCount" HeaderStyle-Width="100px"
                    ItemStyle-HorizontalAlign="Right">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "PalletCount") %>&nbsp;<%# DataBinder.Eval(Container.DataItem, "PalletTypeDescription") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Pallet Spaces" HeaderStyle-Width="45">
                    <ItemTemplate>
                        &nbsp;<span id="spnPalletSpaces" runat="server"><%# decimal.Parse(((System.Data.DataRowView)Container.DataItem)["PalletSpaces"].ToString()).ToString("0.##") %></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Weight" SortExpression="Weight" HeaderStyle-Width="55">
                    <ItemTemplate>
                        <%# ((decimal)((DataRowView)Container.DataItem)["Weight"]).ToString("F0") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Cases" SortExpression="Cases" DataField="Cases" HeaderStyle-Width="45"  ReadOnly="true"/>
                <telerik:GridTemplateColumn HeaderText="Collection" SortExpression="CollectionPointDescription" HeaderStyle-Width="130">
                    <ItemTemplate>
                        <span id="spnCollectionPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((DataRowView)Container.DataItem)["CollectionPointID"].ToString() %>);"
                            onmouseout="closeToolTip();" class="orchestratorLink"><b>
                                <%#((DataRowView)Container.DataItem)["CollectionPointDescription"].ToString()%></b></span>
                        <br />
                        <%# ((DateTime)((DataRowView)Container.DataItem)["CollectionDateTime"]).ToString("dd/MM/yy") %>
                        &nbsp;
                        <%# ((bool)((DataRowView)Container.DataItem)["CollectionIsAnyTime"]) ? "AnyTime" : ((DateTime)((DataRowView)Container.DataItem)["CollectionDateTime"]).ToString("HH:mm") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Delivery" SortExpression="DeliveryPointDescription" HeaderStyle-Width="130">
                    <ItemTemplate>
                        <span id="spnDeliveryPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((DataRowView)Container.DataItem)["DeliveryPointID"].ToString() %>);"
                            onmouseout="closeToolTip();" class="orchestratorLink"><b>
                                <%#((DataRowView)Container.DataItem)["DeliveryPointDescription"].ToString()%></b></span>
                        <br />
                        <%# ((DateTime)((DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("dd/MM/yy") %>
                        &nbsp;
                        <%# ((bool)((DataRowView)Container.DataItem)["DeliveryIsAnyTime"]) ? "AnyTime" : ((DateTime)((DataRowView)Container.DataItem)["DeliveryDateTime"]).ToString("HH:mm") %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn UniqueName="LoadNo" HeaderText="LoadNo" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber" HeaderStyle-Width="80" ItemStyle-Wrap="false" ReadOnly="true" />
                <telerik:GridBoundColumn UniqueName="DocketNo" HeaderText="DocketNo" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber" HeaderStyle-Width="80" ItemStyle-Wrap="false" ReadOnly="true" />
                <telerik:GridTemplateColumn HeaderText="References" UniqueName="References" HeaderStyle-Width="90px" SortExpression="CustomerOrderNumber" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <asp:Repeater ID="repReferences" runat="server">
                            <ItemTemplate>
                                <span title='<%# ((DataRow) Container.DataItem)["Description"].ToString().Replace("'", "''")%>'>
                                    <%# ((DataRow) Container.DataItem)["Reference"].ToString().Trim().Length > 0 ? ((DataRow) Container.DataItem)["Reference"].ToString() : "<b>Not Supplied</b>" %></span>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <br />
                            </SeparatorTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Notes" SortExpression="OrderNotes" DataField="OrderNotes" ReadOnly="true"
                    ItemStyle-Wrap="true" />
                <telerik:GridTemplateColumn HeaderText="Collection Run ID" SortExpression="CollectionJobID" HeaderStyle-Width="70">
                    <ItemTemplate>
                        <%# ((DataRowView)Container.DataItem)["CollectionJobID"] == DBNull.Value ? "&nbsp;" : "<a href=\"javascript:viewJobDetails(" + ((int)((DataRowView)Container.DataItem)["CollectionJobID"]).ToString() + ")\">" + ((int)((DataRowView)Container.DataItem)["CollectionJobID"]).ToString() + "</a>"%>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridHyperLinkColumn HeaderText="Delivery Run ID" SortExpression="DeliveryJobID"
                    DataTextField="DeliveryJobID" DataNavigateUrlFormatString="javascript:viewJobDetails({0});"
                    DataNavigateUrlFields="DeliveryJobID" HeaderStyle-Width="60">
                </telerik:GridHyperLinkColumn>
                <telerik:GridTemplateColumn HeaderText="Has POD" SortExpression="HasPOD" HeaderStyle-Width="35">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank" Text=""></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Has Extras" SortExpression="HasExtras" HeaderStyle-Width="45">
                    <ItemTemplate>
                        <%# ((bool)((DataRowView)Container.DataItem)["HasExtras"]) ? "Yes" : "No" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Has Un- accepted Extras" SortExpression="HasUnacceptedExtras" HeaderStyle-Width="55">
                    <ItemTemplate>
                        <%# ((bool)((DataRowView)Container.DataItem)["HasUnacceptedExtras"]) ? "Yes" : "No" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <NoRecordsTemplate>
                <div>
                    There are no orders that can be flagged as ready to invoice</div>
            </NoRecordsTemplate>
        </MasterTableView>
        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true" >
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true"  EnableDragToSelectRows="false" />
            <ClientEvents 
               OnGridCreated="GridCreated"
               OnRowClick="RowClick" 
               OnRowSelecting="RowSelecting"
               OnRowDeselecting="RowDeselecting"/>
        </ClientSettings>
    </telerik:RadGrid>
    
    <div class="buttonbar">
        <asp:Button ID="btnSaveChanges" runat="server" Text="Flag Selected Orders" />
    </div>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>
