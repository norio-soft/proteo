<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="WorkForDriver.aspx.cs" Inherits="Orchestrator.WebUI.Organisation.WorkForDriver" Title ="All Work For Drivers" %>

<%@ Import Namespace="System.Data" %>

<%@ Register TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>All Work For Drivers</h1>
</asp:Content>



<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
    Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>

    <script type="text/javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script src="/script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {

            FilterOptionsDisplayHide();

            //Re-add the options to the select clients list because they aren't stored server side
            var oldValues = document.getElementById("<%=hidSelectedDriversValues.ClientID%>").value;
            var oldText = document.getElementById("<%=hidSelectedDriversText.ClientID%>").value;

            var valuesArray = oldValues.split(',');
            var textArray = oldText.split('|');


            for (var i = 0; i < valuesArray.length; i++) {
                //The split creates an empty string at the end of the array
                if (valuesArray[i] !== "") {
                    $('.seSelectedDrivers')
                        .append($('<option>', { value: valuesArray[i] })
                        .text(textArray[i]));
                    FilterOptionsDisplayHide();
                }
            }

        });

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

        function AddDriverToSelected() {

            var selectedDriver = $find('<%=cboDriver.ClientID%>');
            var value = selectedDriver._value;
            var text = selectedDriver._text;


            if (value != "") {
                var exists = false;
                $('.seSelectedDrivers option').each(function () {
                    if (this.value == value) {
                        exists = true;
                        return false;
                    }
                });

                if (!exists) {
                    $('.seSelectedDrivers')
                 .prepend($('<option>', { value: value })
                 .text(text));

                    //store both value and text
                    var oldValues = document.getElementById("<%=hidSelectedDriversValues.ClientID%>").value;
                    var newValues = value + ',' + oldValues;

                    var oldText = document.getElementById("<%=hidSelectedDriversText.ClientID%>").value;
                    var newText = text + '|' + oldText;

                    document.getElementById("<%=hidSelectedDriversValues.ClientID%>").value = newValues;
                    document.getElementById("<%=hidSelectedDriversText.ClientID%>").value = newText;
                }
            }

            return false;

        }

        function RemoveDriverToSelected() {


            var selectedOption = $('.seSelectedDrivers option:selected');
            var indexToRemove = selectedOption.index();
            if (selectedOption.val()) {
                selectedOption.remove();

                //Update hidden field for values
                var oldValues = document.getElementById("<%=hidSelectedDriversValues.ClientID%>").value;
                var arrayValues = oldValues.split(',');

                arrayValues.splice(indexToRemove, 1);
                var newValues = arrayValues.join(',');

                document.getElementById("<%=hidSelectedDriversValues.ClientID%>").value = newValues;

                //Update hidden field for text
                var oldText = document.getElementById("<%=hidSelectedDriversText.ClientID%>").value;
                var arrayText = oldText.split('|');

                arrayText.splice(indexToRemove, 1);
                var newText = arrayText.join('|');

                document.getElementById("<%=hidSelectedDriversText.ClientID%>").value = newText;

            }

            return false;
        }

        function viewJobDetails(jobID) {
            var url = "/job/job.aspx?jobId=" + jobID + getCSID();

            openResizableDialogWithScrollbars(url, '1100', '800');
        }

        function updateOrder(orderID) {
            var url = "/Groupage/updateOrder.aspx?wiz=true&oID=" + orderID;
            var randomnumber = Math.floor(Math.random() * 11)
            var wnd = window.open(url, randomnumber, "width=1180, height=900, resizable=1, scrollbars=1");
        }

        function CheckSelectAll(item) {

            $(item.parentElement.parentElement.children).each(function () {
                this.children[0].checked = item.checked;
            });
        }

        function CheckSelectBox(item) {

            var selected = true;

            $(item.parentElement.parentElement.children).each(function () {
                if ($(this.children[1]).text() != "All" && !this.children[0].checked)
                 selected = false;
            });

            $(item.parentElement.parentElement.children).each(function () {
                if ($(this.children[1]).text() == "All")
                    this.children[0].checked = selected;
            });
        }

        function OpenPODWindow(orderID) {
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&OrderID=" + orderID;

            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
        <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
        <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
        <nfvc:NoFormValButton id="btnExportToCSV" runat="server" text="Export To CSV"></nfvc:NoFormValButton>
    </div>

    <!--Hidden Filter Options-->
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <fieldset>
            <legend>Driver Work Items</legend>
            <table>
                <tr>
                    <td>
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker Width="100" ID="dteDateFrom" runat="server">
                                        <DateInput runat="server"
                                            DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvDateFrom" runat="server" ControlToValidate="dteDateFrom"
                                        Display="Dynamic" ToolTip="Please enter a start date for the Report.">
                                        <img id="imgReqStartDate" runat="server" src="/images/error.png" alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker Width="100" ID="dteDateTo" runat="server">
                                        <DateInput runat="server"
                                            DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvDateTo" runat="server" ControlToValidate="dteDateTo"
                                        Display="Dynamic" ToolTip="Please enter a Emd date for the Report.">
                                        <img id="imgReqEndDate" runat="server" src="/images/error.png" alt="" />
                                    </asp:RequiredFieldValidator>
                                </td>
                                <td></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    <asp:Label ID="lblMode" runat="server"></asp:Label></td>
                                <td colspan="1">
                                    <telerik:RadComboBox ID="cboDriver" runat="server" DataTextField="Description" DataValueField="ResourceId" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                        ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="200px" Height="300px">
                                    </telerik:RadComboBox>

                                    <asp:RequiredFieldValidator ID="rfvcboDriver" runat="server" ControlToValidate="cboDriver" ValidationGroup="grpRefresh">
                                        <img src="/images/Error.gif" height="16" width="16" title="Please select a Client or Sub-Contractor." alt="" />
                                    </asp:RequiredFieldValidator>


                                </td>
                                <td>
                                    <asp:Button runat="server" type="button" ID="btnAddcboDriver" name="add" OnClientClick="javascript:AddDriverToSelected(); return false;" UseSubmitBehavior="false" Text="Add" />
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    <asp:Label ID="lblSelectedDrivers" runat="server" Text="Selected Drivers"></asp:Label></td>

                                <td colspan="1">
                                    <select id="seSelectedDrivers" class="seSelectedDrivers" name="seSelectedDrivers" multiple="true" runat="server" style="width: 200px">
                                    </select>
                                    <asp:HiddenField ID="hidSelectedDriversValues" runat="server" />
                                    <asp:HiddenField ID="hidSelectedDriversText" runat="server" />
                                </td>
                                <td>
                                    <asp:Button runat="server" type="button" ID="btnRemoveSelectedDrivers" name="remove" OnClientClick="javascript:RemoveDriverToSelected(); return false;" UseSubmitBehavior="false" Text="Remove" />
                                </td>

                            </tr>

                           <tr>
                             <td class="formCellLabel">Select All Drivers</td>
                             <td><asp:CheckBox runat="server" ID="cbSelectedAllDrivers" cssClass="checkboxList"></asp:CheckBox></td> 
                            </tr>
                            
                        </table>
                    </td>
                </tr>


                <tr>
                    <td>
                        <table>
                            <tr>
                                <td class="formCellLabel">Job Status
                                </td>
                                <td class="formCellInput">
                                    <asp:CheckBoxList runat="server" ID="cblJobStatus" RepeatDirection="horizontal" CssClass="checkboxList">
                                    </asp:CheckBoxList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>

        <div class="buttonbar">
            <asp:Button ID="btnGetData" runat="server" Text="Get Data" />
        </div>
    </div>

    <table id="tblSummary" runat="server" style="display: none" cellpadding="0" cellspacing="0">
        <tbody>
            <tr>
                <td>
                    <h3>
                        <asp:Label ID="lblSummary" runat="server" Text="Summary" /></h3>
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid ID="grdSummary" runat="server" AllowPaging="false" ShowGroupPanel="true" Skin="Orchestrator" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" ShowFooter="true">
                        <MasterTableView DataKeyNames="OrganisationName" Width="100%" AllowSorting="true">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Client" DataField="OrganisationName" HeaderStyle-Width="200"></telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Delivery Run Count" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((int)((DataRowView)Container.DataItem)["CountOfRuns"]).ToString() %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalCountOfDeliveryRuns" runat="server"></asp:Label>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Order Count" HeaderStyle-Width="80">
                                    <ItemTemplate>
                                        <%# ((int)((DataRowView)Container.DataItem)["CountOfOrders"]).ToString() %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalCountOfOrders" runat="server"></asp:Label>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Rate" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((decimal)((DataRowView)Container.DataItem)["TotalRate"]).ToString("C") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalRate" runat="server"></asp:Label>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Extra Charges" HeaderStyle-Width="155">
                                    <ItemTemplate>
                                        <%#((DataRowView)Container.DataItem)["NumberOfExtras"].ToString() %> extras worth <%#((decimal)((DataRowView)Container.DataItem)["TotalValueOfExtras"]).ToString("C") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblExtrasBreakdown" runat="server"></asp:Label>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Fuel Surcharges" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((decimal)((DataRowView)Container.DataItem)["TotalFuelSurcharge"]).ToString("C") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalOfFuelSurchargeAmount" runat="server"></asp:Label>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Total Weight" HeaderStyle-Width="100">
                                    <ItemTemplate>
                                        <%# ((decimal)((DataRowView)Container.DataItem)["TotalWeight"]).ToString("F0") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalWeight" runat="server"></asp:Label>
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </tbody>
    </table>

    <asp:Repeater ID="repDrivers" runat="server" Visible="true">
        <HeaderTemplate>
            <table style="width: 100%" cellpadding="0" cellspacing="0">
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <h3>
                        <asp:Label ID="lblTitle" runat="server" Text="" /></h3>
                    <asp:HiddenField ID="hidDriverID" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid runat="server" ID="grdDrivers" AllowSorting="True" AutoGenerateColumns="False" AllowMultiRowSelection="True" ShowFooter="true" OnNeedDataSource="grd_NeedDataSource" OnItemDataBound="grd_ItemDataBound">
                        <MasterTableView DataKeyNames="OrderID" AllowSorting="true">

                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Driver" HeaderStyle-Width="30px" DataField="FullName" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                                <telerik:GridHyperLinkColumn HeaderText="Order ID" SortExpression="OrderID" DataNavigateUrlFormatString="javascript:updateOrder({0})" DataNavigateUrlFields="OrderID" DataTextField="OrderID" HeaderStyle-Width="30px"></telerik:GridHyperLinkColumn>
                                <telerik:GridHyperLinkColumn HeaderText="Run ID" SortExpression="JobID" DataNavigateUrlFormatString="javascript:viewJobDetails({0})" DataNavigateUrlFields="JobID" DataTextField="JobID" HeaderStyle-Width="40"></telerik:GridHyperLinkColumn>
                                <telerik:GridBoundColumn HeaderText="Customer Order Number" HeaderStyle-Width="30px" DataField="CustomerOrderNumber" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                                <telerik:GridBoundColumn HeaderText="Delivery Order Number" HeaderStyle-Width="30px" DataField="DeliveryOrderNumber" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                                <telerik:GridBoundColumn HeaderText="Client" HeaderStyle-Width="30px" DataField="OrganisationName" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                                <telerik:GridTemplateColumn HeaderText="Rate" HeaderStyle-Width="30px" SortExpression="Rate">
                                    <ItemTemplate>
                                        <%#((DataRowView)Container.DataItem)["Rate"].ToString().Length < 2 ? "&nbsp;" : ((decimal)((DataRowView)Container.DataItem)["Rate"]).ToString("C")%>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalRate" runat="server" />
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Extras" HeaderStyle-Width="50px" SortExpression="NumberOfExtras,TotalValueOfExtras">
                                    <ItemTemplate>
                                        <%#((DataRowView)Container.DataItem)["NumberOfExtras"].ToString() %> extras worth <%#((decimal)((DataRowView)Container.DataItem)["TotalValueOfExtras"]).ToString("C") %>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblTotalExtras" runat="server" />
                                    </FooterTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridBoundColumn HeaderText="Collection Point" HeaderStyle-Width="30px" DataField="CollectionPointDescription" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                                <telerik:GridBoundColumn HeaderText="Delivery Point" HeaderStyle-Width="30px" DataField="DeliveryPointDescription" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                                <telerik:GridBoundColumn HeaderText="Post Town" HeaderStyle-Width="20px" DataField="DeliveryPostTown" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                                <telerik:GridBoundColumn HeaderText="Delivery Date" HeaderStyle-Width="30px" DataField="DeliveryDateTime" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                                <telerik:GridTemplateColumn HeaderText="No Pallets" HeaderStyle-Width="50" SortExpression="NoPallets">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnNoPallets" runat="server"><%#((DataRowView)Container.DataItem)["NoPallets"].ToString().Length < 1 ? "&nbsp;" : ((int)((DataRowView)Container.DataItem)["NoPallets"]).ToString()%></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn HeaderText="Pallet Spaces" HeaderStyle-Width="30" SortExpression="PalletSpaces">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnPalletSpaces" runat="server"><%#((DataRowView)Container.DataItem)["PalletSpaces"].ToString().Length < 2 ? "&nbsp;" : Math.Round(((decimal)((DataRowView)Container.DataItem)["PalletSpaces"]),2).ToString()%></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridTemplateColumn HeaderText="Weight" HeaderStyle-Width="30" SortExpression="Weight">
                                    <ItemTemplate>
                                        &nbsp;<span id="spnWeight" runat="server"><%#((DataRowView)Container.DataItem)["Weight"].ToString().Length < 2 ? "&nbsp;" : Math.Round(((decimal)((DataRowView)Container.DataItem)["Weight"]),2).ToString()%></span>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>

                                <telerik:GridBoundColumn HeaderText="Trailer" DataField="Trailer" HeaderStyle-Width="50"></telerik:GridBoundColumn>

                                <telerik:GridTemplateColumn HeaderText="Has POD" HeaderStyle-Width="30" SortExpression="HasPOD">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="lnkPOD" runat="server" NavigateUrl="" ForeColor="black" Target="_blank" Text=""></asp:HyperLink>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

</asp:Content>
