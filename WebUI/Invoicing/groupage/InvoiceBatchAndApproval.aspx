<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true"
    CodeBehind="InvoiceBatchAndApproval.aspx.cs" Title="Haulier Enterprise"
    Inherits="Orchestrator.WebUI.Invoicing.groupage.InvoiceBatchAndApproval" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="dlg" %>
<%@ Register TagPrefix="orc" TagName="ApprovalProcess" Src="~/UserControls/Invoicing/ApprovalProcess.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script type="text/javascript" language="javascript" src="/script/jquery-ui-min.js"></script>

    <script type="text/javascript" language="javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <script type="text/javascript" language="javascript" src="InvoiceBatchAndApproval.aspx.js"></script>

    <script type="text/javascript" language="javascript" src="/UserControls/Invoicing/ApprovalProcess.ascx.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Orders Ready To Invoice</h1>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <dlg:Dialog ID="dlgOrder" runat="server" URL="/Invoicing/printmultipleinvoices.aspx"
        Width="1080" Height="900" AutoPostBack="false" ReturnValueExpected="false" Mode="Normal" />
    <asp:ListView ID="lvExistingBatches" runat="server" ItemPlaceholderID="itemContainer">
        <LayoutTemplate>

            <script language="javascript" type="text/javascript">
            <!--
    var existingBatchLabels = null;
                
    function populateExistingBatches()
    {
        existingBatchLabels = new Array(<asp:PlaceHolder id="itemContainer" runat="server" />);
    }
                
    populateExistingBatches();
    //-->
            </script>

            <!-- Stop IE from stretching the RadDatePickers to 100% Width -->
            <style type="text/css">
            	.RadPicker
		        {
			        width: 100px !important;
			        display: inline-block !important;
   	            }
	        </style>

        </LayoutTemplate>
        <ItemTemplate>
            [<%# ((int)((System.Data.DataRow)Container.DataItem)["IdentityId"]).ToString() %>,
            "<%# ((string)((System.Data.DataRow)Container.DataItem)["BatchLabel"]).ToString() %>"]</ItemTemplate>
        <ItemSeparatorTemplate>
            ,</ItemSeparatorTemplate>
        <EmptyDataTemplate>

            <script language="javascript" type="text/javascript">
            <!--
    var existingBatchLabels = new Array();
    //-->
            </script>

        </EmptyDataTemplate>
        <EmptyItemTemplate>

            <script language="javascript" type="text/javascript">
            <!--
    var existingBatchLabels = new Array();
    //-->
            </script>

        </EmptyItemTemplate>
    </asp:ListView>
    <fieldset>
        <span style="vertical-align: top;">
            <img align="middle" id="imgExpColBatches" alt="Expand-Collapse" src="/images/topItem_col.gif"
                onclick="javascript: showHideDiv('divap','imgExpColBatches', 'images/topItem_col.gif','images/topItem_exp.gif');" />&nbsp;<a
                    name="Legs" style="color: Black; text-decoration: none;"><b>Current PreInvoices</b></a>
        </span>
        <div id="divap" style="display: inline;">
            <orc:ApprovalProcess ID="approvalProcess" runat="server"></orc:ApprovalProcess>
        </div>
    </fieldset>
    <asp:Label ID="lblError" runat="server"></asp:Label>
    <fieldset>
        <span style="vertical-align: top;">
            <img align="middle" id="img1" alt="Expand-Collapse" src="/images/topItem_col.gif"
                onclick="javascript: showHideDiv('divcb','imgExpColBatches', 'images/topItem_col.gif','images/topItem_exp.gif');" />&nbsp;<a
                    name="Legs" style="color: Black; text-decoration: none;"><b>Ready To Invoice</b></a>
        </span>
        <div id="divcb" style="display: inline;">
            <table>
                <tr>
                    <td runat="server" id="tdDateOptions">
                        <telerik:RadAjaxPanel runat="server" ID="radPanel">
                            <table>
                                <tr>
                                    <td class="formCellLabel" >
                                        Client
                                    </td>
                                    <td class="formCellField" colspan="4">
                                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true"
                                            ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                                            AllowCustomText="False" ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px"
                                            Height="300px" SelectOnTab="true" DataTextField="OrganisationName" DataValueField="IdentityID">
                                        </telerik:RadComboBox>
                                    </td>
                                    </tr>
                                    <tr>
                                    <td class="formCellLabel">
                                        Date From
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100" ToolTip="The start date for the filter" >
                                        <DateInput ID="DateInput1" runat="server"
                                        DateFormat="dd/MM/yy">
                                        </DateInput>
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td class="formCellLabel">
                                        Date To
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100" ToolTip="The end date for the filter" >
                                        <DateInput ID="DateInput2" runat="server"
                                        DateFormat="dd/MM/yy">
                                        </DateInput>
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td class="formCellField">
                                        <asp:RadioButtonList ID="cboSearchAgainstDate" runat="server" RepeatDirection="horizontal">
                                            <asp:ListItem Text="Collection Date" Value="COL"></asp:ListItem>
                                            <asp:ListItem Text="Delivery Date" Value="DEL"></asp:ListItem>
                                            <asp:ListItem Text="Collection and Delivery Dates" Selected="true" Value="BOTH"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                  
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td class="formCellLabel">
                                        Business Type
                                    </td>
                                    <td class="formCellField" colspan="4">
                                        <input type="checkbox" id="chkSelectAllBusinessTypes" onclick="selectAllBusinessTypes(this);" checked='true' /><label for="chkSelectAllBusinessTypes">Select All</label>
                                        <asp:CheckBoxList runat="server" ID="cblBusinessType" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">
                                        Invoice Type
                                    </td>
                                    <td class="formCellField" colspan="4">
                                        <asp:RadioButtonList ID="rdoListInvoiceType" runat="server" RepeatDirection="Horizontal"
                                            RepeatColumns="3">
                                            <asp:ListItem Text="Normal" Value="NORM" Selected="True" />
                                            <asp:ListItem Text="Self Bill" Value="SELF" />
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                        </telerik:RadAjaxPanel>
                    </td>
                </tr>
            </table>
            <div>
                Pending Changes can be shown if there are orders or extras on a pre-invoice which are no longer in the system.
            </div>
            <div class="buttonbar" style="margin-bottom: 10px; float:left; border-right-style:none; border-bottom-style:none">
                <table>
                    <tr>
                        <td>
                            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
                        </td>
                        <td>
                            <asp:Button ID="btnAutoBatch" runat="server" Text="Auto Batch" CausesValidation="false" Enabled="false" />
                            <asp:Button ID="btnCreateBatch" runat="server" Text="Create Batch" />
                            <asp:Button ID="btnAlterBatch" runat="server" Text="Add to Batch" CausesValidation="false" />
                            <asp:Button ID="btnUnflagOrder" runat="server" Text="Unflag Orders" />
                        </td>
                        
                    </tr>
                </table>
                </div>
                <div style="border: 1px solid black; background-color: whitesmoke; padding: 10px; text-align: left; margin-bottom: 10px;">Invoice Date
                            <telerik:RadDatePicker ID="rdiBatchInvoiceDate" runat="server" Width="100px"
                                ToolTip="The invoice date for the batch." Calendar-Visible="True">
                                <DateInput ID="DateInput3" runat="server"
                                DateFormat="dd/MM/yy"
                                OnClientDateChanged="BatchInvoiceDateTop_SyncInvoiceDate">
                                </DateInput>
                                </telerik:RadDatePicker></div>
            
            <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" AllowMultiRowEdit="true"
                AutoGenerateColumns="false" AllowMultiRowSelection="true">
                <MasterTableView Width="100%" ClientDataKeyNames="OrderID,OrderGroupID" DataKeyNames="OrderID,OrderGroupID,CustomerOrganisationName,CustomerIdentityID,ForeignRate,LCID,IsFlaggedForInvoicing"
                    Name="Master">
                    <RowIndicatorColumn Display="false">
                    </RowIndicatorColumn>
                    <Columns>

                        <telerik:GridTemplateColumn  UniqueName="chkSelectColumn" HeaderStyle-Width="28" AllowFiltering="false">
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelectAll" onclick="javascript:selectAllCheckboxes(this);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <input runat="server" type="checkbox" id="chkOrder" onclick="javascript:ClickRowCheckBox(this);" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <%-- 
                        <telerik:GridTemplateColumn ItemStyle-Width="28" HeaderStyle-Width="28">
                            <ItemTemplate>
                                <input runat="server" type="checkbox" id="chkOrder" onclick="javascript:SelectOrder(this);" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn> 
                        --%>
                        <telerik:GridTemplateColumn UniqueName="OrderId" HeaderText="Order Id" HeaderStyle-Width="55">
                            <ItemTemplate>
                                <a id="lnkOrderId" runat="server"></a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn UniqueName="JobId" HeaderText="Run Id" HeaderStyle-Width="55">
                            <ItemTemplate>
                                <a id="lnkJobId" runat="server"></a>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn UniqueName="DeliveryOrderNumber" HeaderText="Delivery Order Number"
                            HeaderStyle-Width="100" SortExpression="DeliveryOrderNumber" DataField="DeliveryOrderNumber">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Load Number" HeaderStyle-Width="100" SortExpression="CustomerOrderNumber"
                            DataField="CustomerOrderNumber">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Client" HeaderStyle-Width="120" SortExpression="CustomerOrganisationName"
                            DataField="CustomerOrganisationName" ItemStyle-Wrap="false">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Collect From" HeaderStyle-Width="140" SortExpression="CollectionPointDescription"
                            DataField="CollectionPointDescription" ItemStyle-Wrap="false">
                        </telerik:GridBoundColumn>
                        <telerik:GridBoundColumn HeaderText="Deliver To" HeaderStyle-Width="140" SortExpression="DeliveryPointDescription"
                            DataField="DeliveryPointDescription" ItemStyle-Wrap="false">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Deliver At" HeaderStyle-Width="50" SortExpression="DeliveryDateTime"
                            ItemStyle-Width="150" ItemStyle-Wrap="false">
                            <ItemTemplate>
                                <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Rate" HeaderStyle-Width="60">
                            <ItemTemplate>
                                <asp:Label ID="lblCharge" runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="References" HeaderStyle-Width="80">
                            <ItemTemplate>
                                <asp:Label ID="lblReferences" runat="server" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Batch Id" HeaderStyle-Width="100" UniqueName="batchId">
                            <ItemTemplate>
                                <asp:TextBox ID="txtBatchId" runat="server" Width="98%" />
                            </ItemTemplate>
                            <ItemStyle HorizontalAlign="Right" />
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="Is Flagged For Invoicing" HeaderStyle-Width="140" SortExpression="IsFlaggedForInvoicing"
                            DataField="IsFlaggedForInvoicing" ItemStyle-Wrap="false" UniqueName="IsFlaggedForInvoicing" Display="false">
                        </telerik:GridBoundColumn>
                    </Columns>
                    <NoRecordsTemplate>
                        <div>
                            There are no records to display</div>
                    </NoRecordsTemplate>
                </MasterTableView>
                <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                    <Resizing AllowColumnResize="true" AllowRowResize="false" />
                    <Selecting AllowRowSelect="true" EnableDragToSelectRows="false" />
                    <ClientEvents 
                        OnGridCreated="GridCreated"
                        OnRowClick="RowClick" 
                        OnRowSelecting="RowSelecting"
                        OnRowDeselecting="RowDeselecting"
                        OnRowMouseOver="RowMouseOver"/>
                </ClientSettings>
            </telerik:RadGrid>
            <b>Grouped Order Count:&nbsp;<asp:Label runat="server" ID="lblOrderCount" Text="0"></asp:Label></b><br />
            <b>Total Rate:&nbsp;<asp:Label runat="server" ID="lblRunningTotal" Text="0"></asp:Label></b>
            <div style="height: 10px;">
            </div>
            <div class="buttonbar" style="margin-bottom: 10px; float:left; border-right-style:none; border-bottom-style:none" id="createBatch2">
                <asp:Button ID="Button1" runat="server" Text="Create Batch" />
                
            </div>
            <div style="border: 1px solid black; background-color: whitesmoke; padding: 10px; text-align: left; margin-bottom: 10px;">Invoice Date
                <telerik:RadDatePicker ID="rdiBatchInvoiceDateBottom" runat="server" Width="100"
                    ToolTip="The invoice date for the batch.">
                    <DateInput ID="DateInput4" runat="server"
                    DateFormat="dd/MM/yy" OnClientDateChanged="BatchInvoiceDateBottom_SyncInvoiceDate">
                    </DateInput>
                    </telerik:RadDatePicker></div>
        </div>

        
            <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="rdiBatchInvoiceDateBottom"
	                Display="Dynamic" EnableClientScript="true" ClientValidationFunction="ValidateInvoiceDate"
	                ErrorMessage="This Date may be Wrong">
	        </asp:CustomValidator>

    </fieldset>
    <asp:HiddenField ID="txt" runat="server" />
    <asp:Panel ID="pnlBatches" runat="server">
        <div style="background-color: White; border: solid 1pt black">
            <fieldset>
                <legend>Details of the Batches Generated</legend>
                <telerik:RadGrid ID="grdBatches" Width="600" runat="server" AllowPaging="false" AllowSorting="true"
                    AutoGenerateColumns="false" AllowMultiRowSelection="true">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridBoundColumn DataField="Customer" HeaderText="Customer">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="BatchName" HeaderText="Batch ID">
                            </telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="Total" HeaderText="Total">
                            </telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
                <asp:Button ID="btnOK" runat="server" Text="Close" CssClass="buttonClass" />
            </fieldset>
        </div>
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender runat="server" PopupControlID="pnlBatches" OkControlID="btnOK"
        ID="popBatches" TargetControlID="txt">
    </ajaxToolkit:ModalPopupExtender>
    <asp:HiddenField runat="server" ID="hidUserName" />
    <asp:HiddenField runat="server" ID="hidBatchIndicator" />

    <script type="text/javascript" language="javascript">

        function selectAllCheckboxes(chk) {
            $('table[id*=grdOrders] input:enabled[id*=chkOrder]').each(function () {
                if (this.checked != chk.checked) {
                    this.click();
                }
            });
           
        }

        function ValidateInvoiceDate(source, args)  {
      
            var dteDateTime = $find("<%= rdiBatchInvoiceDateBottom.ClientID %>");
            var dteDateTimeValue = dteDateTime.get_dateInput().get_selectedDate();

            //  Create Today's date
            var today = new Date();
            var upperBound = today.setDate(today.getDate() + 30);
            var today = new Date();
            var lowerBound = today.setDate(today.getDate() - 30);

            if (dteDateTimeValue > upperBound) {
                var r = confirm('Warning. The selected Invoice date is more than 30 days in the future.');
                if (r == true)
                    args.IsValid = true;
                else {
                    args.IsValid = false;
                }
            }

            if (dteDateTimeValue < lowerBound) {
                var r = confirm('Warning. The selected Invoice date is more than 30 days in the past.');
                if (r == true)
                    args.IsValid = true;
                else {
                    args.IsValid = false;
                }
            }
        }


        var groupHandlingIsActive = false;
        var createBatch1 = null, createBatch2 = null;
        var launch = false;
        $(document).ready(function() {
            createBatch1 = $("input[id*=btnCreateBatch]");
            createBatch2 = $("div[id*=createBatch2]");
            validateBatchLabels();

        });

        function BatchInvoiceDateBottom_SyncInvoiceDate(sender, eventArgs) {
            var rdiTop = $find("<%= rdiBatchInvoiceDate.ClientID %>");
            var rdiTopDate = rdiTop.get_dateInput().get_selectedDate().toLocaleDateString("en-GB");

            var rdiBottom = $find("<%= rdiBatchInvoiceDateBottom.ClientID %>");
            var rdiBottomDate = rdiBottom.get_dateInput().get_selectedDate().toLocaleDateString("en-GB");

            if (rdiTopDate != rdiBottomDate)
                rdiTop.get_dateInput().set_value(rdiBottomDate);
        }

        function BatchInvoiceDateTop_SyncInvoiceDate(sender, eventArgs) {

            var rdiTop = $find("<%= rdiBatchInvoiceDate.ClientID %>");
            var rdiTopDate = rdiTop.get_dateInput().get_selectedDate().toLocaleDateString("en-GB");

            var rdiBottom = $find("<%= rdiBatchInvoiceDateBottom.ClientID %>");
            var rdiBottomDate = rdiBottom.get_dateInput().get_selectedDate().toLocaleDateString("en-GB");

            if (rdiTopDate != rdiBottomDate)
                rdiBottom.get_dateInput().set_value(rdiTopDate);

        }

        function ClickRowCheckBox(chk)
        {
            //Let RowClick Handle the click.
            chk.checked = !chk.checked;

            //Mock up the sender object.
            var sender = $find(chk.parentElement.parentElement.parentElement.parentElement.parentElement.id);

            var args = {
		
                get_itemIndexHierarchical : function() {
                    return chk.attributes["index"].value;
                }
            };

            RowClick(sender, args);
        }

        
        function SelectOrder(chk) 
        {

            if(chk ==null) return;

            // Update the running total.
            var lblRunningTotal = $('span[id*=lblRunningTotal]')[0];
            var grid = $get("<%= grdOrders.ClientID %>");
            var invTotal = 0;

            if (chk.checked) {
                
                // Highlight the selected row.
                var selectedRowIndex = $(chk).attr('Index');

                $('table[id*=grdOrders] tr:gt(0)').each(function(index, ele) {
                    $(ele)[0].style.backgroundColor = '#ffffff';
                });

                $(chk).parent().parent('tr')[0].style.backgroundColor = '#ffff99';

                var batchRef = $('input:hidden[id*=hidUserName]')[0].value + '-' + $('input:hidden[id*=hidBatchIndicator]')[0].value;

                // Tick all the orders in the group.
                var orderGroupId = $(chk).attr('OrderGroupId');
                if (orderGroupId != null && orderGroupId > 0) {
                    $('input:checkbox[id*=chkOrder][OrderGroupId=' + orderGroupId + ']').prop('checked', true);

                    // Get all the batch ref text boxes for the orders in the group
                    $('input[id*=txtBatchId][OrderGroupId=' + orderGroupId + ']').attr('value', batchRef);

                } else {

                    // Auto insert the batch reference if there isn't one already
                    var txtBatchId = $(chk).parent().parent().find('input[id*=txtBatchId]')[0];

                    if (txtBatchId.value == "") {
                        txtBatchId.value = batchRef;
                    }
                }

            } else {

                $('table[id*=grdOrders] tr:gt(0)').each(function(index, ele) {
                    $(ele)[0].style.backgroundColor = '#ffffff';
                });
                document.getElementById("chkSelectAll").checked = false;
                // Un-tick all the orders in the group.
                var orderGroupId = $(chk).attr('OrderGroupId');
                if (orderGroupId != null && orderGroupId > 0) {
                    $('input:checkbox[id*=chkOrder][OrderGroupId=' + orderGroupId + ']').prop('checked', false);

                    // Get all the batch ref text boxes for the orders in the group
                    $('input[id*=txtBatchId][OrderGroupId=' + orderGroupId + ']').attr('value', '');

                } else {

                    // Wipe the batch reference 
                    var txtBatchId = $(chk).parent().parent().find('input[id*=txtBatchId]')[0];
                    txtBatchId.value = "";
                }
                
            }
          
           // Count all the orders not in group
           // Count the unique number of groups ticked
            var uniqueGroupIdsTicked = new Array();
            var arrayCounter = 0;
            $('input:checkbox:checked[id*=chkOrder][OrderGroupId!=0]').each(function(ix, ele) {
                var orderGroupIdToFind = $(ele).attr('OrderGroupId');
                var orderGroupRateToFind = $(ele).attr('Rate');
                var og = jQuery.grep(uniqueGroupIdsTicked, function(a) { return a == orderGroupIdToFind });
                if (og > 0) {
                    // order group id is already in the array, do not readd
                } else {
                    uniqueGroupIdsTicked[arrayCounter] = orderGroupIdToFind;
                    //invTotal += parseFloat(orderGroupRateToFind.split(',').join(''));
                    arrayCounter++;
                }
            });

            $('input:checkbox:checked[id*=chkOrder]').each(function(ix, ele) {
                var orderGroupRateToFind = $(ele).attr('Rate');
                invTotal += parseFloat(orderGroupRateToFind.split(',').join(''));
            });
            
            var countOfOrderGroupsWithASelectOrder = uniqueGroupIdsTicked.length;
            var countOfOrdersNotInGroup = $('input:checkbox:checked[id*=chkOrder][OrderGroupId=0]').length;
            var totalselectedCount = countOfOrderGroupsWithASelectOrder + countOfOrdersNotInGroup;

           // Update the order count
            var lblOrderCount = $('span[id*=lblOrderCount]')[0];
            lblOrderCount.innerHTML = totalselectedCount;
            lblRunningTotal.innerHTML = invTotal.toFixed(2);
                       
        }

        function HandleGroupSelection(e, src, orderGroupID, orderId) {
            validateBatchLabel(src);

            if (orderGroupID == "0")
                return;

            var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            var columns = mtv.get_columns();
            var rows = mtv.get_dataItems();

            var batchId = "";
            var currentTextBox = $("#" + src.id);

            for (var rowIndex = 0; rowIndex < rows.length; rowIndex++) {
                try {
                    if (rows[rowIndex].getDataKeyValue("OrderID") == orderId) {
                        var gridRowTable = rows[rowIndex].get_owner();
                        var cell = gridRowTable.getCellByColumnUniqueName(rows[rowIndex], "batchId");
                        if (!cell) return;

                        var txtBatchId = GetTextBox(cell);
                        if (txtBatchId != null)
                            batchId = txtBatchId.value;

                        break;
                    }
                }
                catch (error) { }
            }

            for (var groupRowIndex = 0; groupRowIndex < rows.length; groupRowIndex++)
                try {
                    if (rows[groupRowIndex].getDataKeyValue("OrderGroupID") == orderGroupID && rows[groupRowIndex].getDataKeyValue("OrderID") != orderId) {
                        var groupGridRowTable = rows[groupRowIndex].get_owner();
                        var otherCell = groupGridRowTable.getCellByColumnUniqueName(rows[groupRowIndex], "batchId");
                        if (!otherCell) continue;
                        else {
                            var txtGroupBatchId = GetTextBox(otherCell);
                            if (txtGroupBatchId != null) {
                                txtGroupBatchId.value = batchId;
                                $('#' + txtGroupBatchId.id).css("background-color", currentTextBox.css("background-color"));
                            }
                        }
                    }
                } catch (error) { }

        }

        function cboClient_itemsRequesting(sender, eventArgs) {
            try {
                var context = eventArgs.get_context();
            }
            catch (err) { }
        }

        function viewOrderProfile(orderID) {
            var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

            var wnd = window.open(url, "OrderProfile", "Width=1180, height=900, scrollbars=1, resizable=1");
        }

        function GetTextBox(control) {

            if (!control) return;

            for (var i = 0; i < control.childNodes.length; i++) {
                if (!control.childNodes[i].tagName) continue;
                if ((control.childNodes[i].tagName.toLowerCase() == "input") &&
                  (control.childNodes[i].type.toLowerCase() == "text")) {
                    return control.childNodes[i];
                }
            }
        }

        function GroupHandling(orderGroupID, selectRow) {
            if (groupHandlingIsActive)
                return;

            groupHandlingIsActive = true;

            var mtv = $find("<%=grdOrders.ClientID %>").get_masterTableView();
            var rows = mtv.get_dataItems();

            for (var rowIndex = 0; rowIndex < rows.length; rowIndex++)
                try {
                    if (rows[rowIndex].getDataKeyValue("OrderGroupID") == orderGroupID) {
                        var gridRowTable = rows[rowIndex].get_owner();
                        var cell = gridRowTable.getCellByColumnUniqueName(rows[rowIndex], "CheckColumn");
                        if (!cell) return;

                        var chkOrderID = GetCheckBox(cell);
                        if (!chkOrderID) return;

                        chkOrderID.checked = selectRow;
                    }
                }
                catch (error) {
                }

            groupHandlingIsActive = false;
        }

        function HandleSelection(e, src, orderGroupID) {
            var gridRow;

            if (e.target) {
                gridRow = e.target.parentNode.parentNode;
            }
            else {
                gridRow = e.srcElement.parentNode.parentNode;
            }

            if (src.checked)
                gridRow.className = "SelectedRow_Orchestrator";
            else
                gridRow.className = "GridRow_Orchestrator";

            // Is the order part of a group that is grouped planning enabled?
            // Automatically select the other orders in the grid that belong to this group.
            if (orderGroupID > 0)
                GroupHandling(orderGroupID, src.checked);
        }


        function showBatchPanel() {
            launch = true;
        }

        var refreshPreInvoices = "<%=RefreshPreInvoices %>";

        function pageLoad() {
            if (launch) {
                $find('<%=popBatches.ClientID %>').show();
            }
            if (refreshPreInvoices == "true")
                $('#ctl00$ContentPlaceHolder1$approvalProcess$btnRefresh').click();
        }

        function selectAllBusinessTypes(sender) {
            $('input:checkbox[id*=cblBusinessType]').prop('checked', $(sender).prop('checked'));
        }
        
    </script>

</asp:Content>
