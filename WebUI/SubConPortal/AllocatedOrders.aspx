<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless_SubCon.Master"
    CodeBehind="AllocatedOrders.aspx.cs" Inherits="Orchestrator.WebUI.SubConPortal.AllocatedOrders" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Your Allocated Orders</h1>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <script language="javascript" type="text/javascript">
        var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";

        // Attach a handler to the load event.
        Sys.Application.add_load(applicationLoadHandler);

        function applicationLoadHandler() {

            $('input[id*=<%=txtSearch.ClientID%>]').blur(
                function() {
                    // When the focus is lost, if no search text is specified then set some default search dates.
                    if ($get('<%=txtSearch.ClientID%>').value == '') {
                        // Get the date controls and default the values
                        $find('<%=dteStartDate.ClientID %>').set_value($('input:hidden[id*=hidStartDate]').val());
                        $find('<%=dteEndDate.ClientID %>').set_value($('input:hidden[id*=hidEndDate]').val());
                    }
                }
            );
        }

        function showLoading() {
            $.blockUI({ css: {
                border: 'none',
                padding: '15px',
                backgroundColor: '#000',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: '.5',
                color: '#fff'
            }
            });
        }

        function hideLoading() {
            $.unblockUI();
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

        // Function to display the column configure box 
        function ColumnDisplayShow() {
            $("#tabs").css({ 'display': 'none' });
            $("#dvColumnDisplay").css({ 'display': 'block' });
        }

        // Function to hide the column configure box 
        function ColumnDisplayHide() {
            $("#tabs").css({ 'display': 'block' });
            $("#dvColumnDisplay").css({ 'display': 'none' });
        }
    </script>

    <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
        Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>
    <cc1:Dialog ID="dlgSubbyOrderProfile" URL="/client/ClientOrderProfile.aspx" Width="750"
        Height="900" AutoPostBack="false" Mode="Modal" runat="server" ReturnValueExpected="true">
    </cc1:Dialog>
    <cc1:Dialog ID="dlgCallIn" URL="/Traffic/JobManagement/DriverCallIn/CallIn.aspx"
        Width="850" Height="800" AutoPostBack="true" Mode="Normal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true">
    </cc1:Dialog>
    <div id="divRejectReason" class="overlayedRejectBox" style="display: none;">
        <fieldset>
            <legend>Reject orders</legend>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Reason for rejection
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboRejectionReason" runat="server" AppendDataBoundItems="true"
                            Width="250px">
                            <asp:ListItem Value="" Text="- select -" />
                        </asp:DropDownList>
                        <asp:TextBox ID="txtRejectionReason" runat="server" TextMode="MultiLine" Style="width: 250px"
                            Rows="3" />
                        <div>
                            <asp:CustomValidator ID="cvRejectionReason" runat="server" ValidationGroup="vgRejectOrders"
                                ClientValidationFunction="validateRejectionReason" Display="Dynamic" ErrorMessage="Please supply a rejection reason" />
                        </div>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonBar">
            <asp:Button ID="btnRejectOrders" runat="server" Text="Reject Orders" CausesValidation="true"
                ValidationGroup="vgRejectOrders" />
            <asp:Button ID="btnHiddenRejectOrders" runat="server" Style="display: none;" CausesValidation="true"
                ValidationGroup="vgRejectOrders" />
            <input type="button" value="Cancel" onclick="$('#divRejectReason').hide(); return false;"
                causesvalidation="false" />
        </div>
        <br />
    </div>

        <asp:Panel ID="pnlConfirmation" runat="server" Visible="false" EnableViewState="false">
        <div class="MessagePanel" style="vertical-align: middle;">
            <table>
                <tr>
                    <td>
                        <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" />
                    </td>
                    <td>
                        <asp:Label CssClass="ControlErrorMessage" ID="lblNote" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <input type="hidden" id="hidRecordIds" runat="server" />
    <input type="hidden" id="hidStartDate" runat="server" />
    <input type="hidden" id="hidEndDate" runat="server" />
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <fieldset>
            <table width="100%">
                <tr>
                    <td class="formCellLabel">
                        Search for
                    </td>
                    <td class="formCellInput">
                        <table>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                                </td>
                                <td>
                                    <asp:RadioButtonList ID="rblSearchFor" runat="server" RepeatDirection="horizontal">
                                        <asp:ListItem Text="All" Value="ALL" Selected="true"></asp:ListItem>
                                        <asp:ListItem Text="OrderId" Value="ORDER"></asp:ListItem>
                                        <asp:ListItem Text="Customer Order Number (load No)" Value="LOADNO"></asp:ListItem>
                                        <asp:ListItem Text="Delivery Order Number (Docket No)" Value="DELNO"></asp:ListItem>
                                        <asp:ListItem Text="Custom Refs" Value="CUSTOM"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Date From
                    </td>
                    <td class="formCellInput">
                        <telerik:RadDateInput DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="65"
                            runat="server" ID="dteStartDate">
                        </telerik:RadDateInput>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Date To
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td class="formCellInput">
                                    <telerik:RadDateInput DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="65"
                                        runat="server" ID="dteEndDate">
                                    </telerik:RadDateInput>
                                </td>
                                <td class="formCellInput">
                                    <asp:RadioButtonList ID="cboSearchAgainstDate" runat="server" RepeatDirection="horizontal">
                                        <asp:ListItem Text="Collection Date" Value="COL"></asp:ListItem>
                                        <asp:ListItem Text="Delivery Date" Value="DEL"></asp:ListItem>
                                        <asp:ListItem Text="Collection and Delivery Dates" Selected="true" Value="BOTH"></asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Order Status
                    </td>
                    <td class="formCellInput" colspan="2">
                        <asp:CheckBoxList runat="server" ID="cblOrderStatus" RepeatDirection="horizontal">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Business Type
                    </td>
                    <td class="formCellField" colspan="2">
                        <asp:CheckBoxList runat="server" ID="cblBusinessType" RepeatDirection="Horizontal">
                        </asp:CheckBoxList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        
                    </td>
                    <td class="formCellInput">
                        <asp:RadioButtonList ID="rblAllocated" runat="server" RepeatDirection="horizontal">
                            <asp:ListItem Text="Allocated Only" Value="AL"></asp:ListItem>
                            <asp:ListItem Text="Subbed Only" Value="SUB"></asp:ListItem>
                            <asp:ListItem Text="Both" Selected="true" Value="BOTH"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Un-invoiced Only
                    </td>
                    <td class="formCellInput">
                        <asp:CheckBox runat="server" ID="chkUnInvoicedOnly" Checked="false" AutoPostBack="false" />
                    </td>
                </tr>
                <tr>
                    <td colspan="3">
                        <table>
                            <tr>
                                <td class="formCellLabel">
                                    Client
                                </td>
                                <td class="formCellInput">
                                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="false" AllowCustomText="False" ShowMoreResultsBox="false" Width="355px"
                                        Height="300px" OnClientItemsRequesting="cboClient_itemsRequesting">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Delivery Point
                                </td>
                                <td class="formCellInput">
                                    <telerik:RadComboBox ID="cboDeliveryPointFilter" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="false" AutoPostBack="false" ShowMoreResultsBox="false"
                                        Width="355px" Height="300px" Overlay="true" AllowCustomText="True" HighlightTemplatedItems="true"
                                        CausesValidation="false" OnClientDropDownClosed="Point_CombBoxClosing">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPointsIncludeDeleted" />
                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellLabel">
                                    Collection Point
                                </td>
                                <td class="formCellInput">
                                    <telerik:RadComboBox ID="cboCollectionPointFilter" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="false" AutoPostBack="false" ShowMoreResultsBox="false"
                                        Width="355px" Height="300px" Overlay="true" AllowCustomText="True" HighlightTemplatedItems="true"
                                        CausesValidation="false" OnClientDropDownClosed="Point_CombBoxClosing">
                                        <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetPointsIncludeDeleted" />
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">
                                    Goods Type
                                </td>
                                <td>
                                    <telerik:RadComboBox ID="cboGoodsType" runat="server" Skin="WindowsXP">
                                    </telerik:RadComboBox>
                                </td>
                                <td class="formCellLabel">
                                    Service Level
                                </td>
                                <td class="formCellField">
                                    <telerik:RadComboBox ID="cboService" runat="server">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnSearch" runat="server" Text="Search" />
            <input type="button" id="Button1" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide();" />
        </div>
    </div>
    <input type="hidden" id="radGridClickedRowIndex" name="radGridClickedRowIndex" />
    <telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" ShowGroupPanel="false"
        ShowFooter="false" AllowSorting="true" AutoGenerateColumns="false" AllowMultiRowSelection="true"
        Width="100%">
        <MasterTableView ClientDataKeyNames="OrderID" DataKeyNames="OrderID" NoMasterRecordsText="Please click search to find orders"
            CommandItemDisplay="Top" GroupLoadMode="Client">
            <CommandItemTemplate>
                <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()"
                    style="display: none;">
                    Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">
                    Close filter Options</div>
                <asp:Button ID="btnRefreshTop" runat="server" Text="Search" CssClass="buttonClassSmall"
                    OnClick="btnSearch_Click" />
                <asp:Button ID="btnRejectOrders" CssClass="buttonClassSmall" runat="server" Text="Reject Order(s)"
                    OnClientClick="ShowRejectReason();return false;" />
            </CommandItemTemplate>
            <RowIndicatorColumn Display="false">
            </RowIndicatorColumn>
            <Columns>
                <telerik:GridTemplateColumn HeaderText="">
                    <HeaderTemplate>
                        <input type="checkbox" onclick="javascript:HandleSelectAll(this);" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <asp:CheckBox ID="chkSelectOrder" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderID">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="hypUpdateOrder" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Call In">
                    <ItemTemplate>
                        <a runat="server" id="hypCallIn"></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Pod">
                    <ItemTemplate>
                        <a runat="server" id="hypAddPod"></a><a runat="server" id="hypViewPod"></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Client" SortExpression="CustomerOrganisationName"
                    DataField="CustomerOrganisationName" UniqueName="Customer" HeaderStyle-Width="10%">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Customer Order No" Aggregate="Count" FooterText="# Orders<br/>"
                    FooterStyle-Font-Bold="true" SortExpression="CustomerOrderNumber" DataField="CustomerOrderNumber"
                    UniqueName="CustomerOrderNumber">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Delivery Order Number" SortExpression="DeliveryOrderNumber"
                    DataField="DeliveryOrderNumber" UniqueName="DeliveryOrderNumber" />
                <telerik:GridBoundColumn HeaderText="Business Type" SortExpression="BusinessTypeDescription"
                    DataField="BusinessTypeDescription" UniqueName="BusinessType">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Rate" SortExpression="ForeignRate" UniqueName="Rate">
                    <ItemTemplate>
                        <asp:Label ID="lblRate" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Weight" HeaderText="Kgs" Aggregate="Sum" FooterStyle-Wrap="false"
                    DataFormatString="{0:0}" FooterStyle-Font-Bold="true" FooterAggregateFormatString="Kgs<br/>{0:0}"
                    UniqueName="Weight">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Collect From" SortExpression="CollectionPointDescription"
                    HeaderStyle-Width="12%" DataField="CollectionPointDescription" ItemStyle-Wrap="false"
                    UniqueName="CollectionPointDescription">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Collect At" SortExpression="CollectionDateTime"
                    UniqueName="CollectionDateTime">
                    <ItemTemplate>
                        <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["CollectionDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["CollectionIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Deliver To" SortExpression="DeliveryPointDescription"
                    HeaderStyle-Width="12%" DataField="DeliveryPointDescription" ItemStyle-Wrap="false"
                    UniqueName="DeliveryPointDescription">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Deliver At" SortExpression="DeliveryDateTime"
                    UniqueName="DeliveryDateTime">
                    <ItemTemplate>
                        <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["DeliveryDateTime"], (bool)((System.Data.DataRowView)Container.DataItem)["DeliveryIsAnyTime"])%></ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Status" SortExpression="Status"
                    DataField="Status" UniqueName="Status" HeaderStyle-Width="10%"/>
                <telerik:GridBoundColumn HeaderText="Service" SortExpression="OrderServiceLevel"
                    HeaderStyle-Width="5%" DataField="OrderServiceLevel" UniqueName="OrderServiceLevel" />
                <telerik:GridBoundColumn DataField="NoPallets" Aggregate="Sum" FooterText="Plts<br/>"
                    FooterStyle-Wrap="false" HeaderText="Plts" FooterStyle-Font-Bold="true" UniqueName="NoPallets">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="PalletSpaces" Aggregate="Sum" FooterAggregateFormatString="Spaces<br/>{0:0}"
                    DataFormatString="{0:0}" FooterStyle-Wrap="false" FooterStyle-Font-Bold="true"
                    HeaderText="Spaces" UniqueName="Spaces">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Goods Type" SortExpression="GoodsTypeDescription"
                    DataField="GoodsTypeDescription" UniqueName="GoodsTypeDescription" />
                <telerik:GridBoundColumn HeaderText="Surcharges" SortExpression="Surcharges" DataField="Surcharges"
                    UniqueName="Surcharges" />
            </Columns>
        </MasterTableView>
        <ClientSettings AllowGroupExpandCollapse="true" AllowDragToGroup="false" AllowColumnsReorder="true"
            ReorderColumnsOnClient="true">
            <Scrolling UseStaticHeaders="true" ScrollHeight="600" AllowScroll="true" />
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </telerik:RadGrid>

    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">

        <script type="text/javascript">
            function cboClient_itemsRequesting(sender, eventArgs) {
                try {
                    var context = eventArgs.get_context();
                    context["DisplaySuspended"] = true;
                }
                catch (err) { }
            }
            
            function ViewPod(podPath) {
                window.open(podPath);
            }
            
            function ShowRejectReason() {
                $('#divRejectReason').show();
                $('#divRejectReason').css('top', $(window).height() / 2 - $('#divRejectReason').height() / 2);
                $('#divRejectReason').css('left', $(window).width() / 2 - $('#divRejectReason').width() / 2);
                
                $(':textarea[id*=txtRejectionReason]').focus();
            }
            
            function OpenPODWindow(jobId, collectDropId) {
                var podFormType = 2;
                var qs = "ScannedFormTypeId=" + podFormType;
                qs += "&JobId=" + jobId;
                qs += "&CollectDropId=" + collectDropId;
                
                <%=dlgDocumentWizard.ClientID %>_Open(qs);
            }
            
            function HandleSelectAll(chk) 
            {
                $(":checkbox[id$=chkSelectOrder]").each(function(chkIndex) {
                    if (this.checked != chk.checked) {
                        this.click();
                    }
                });
            }
            
            function HandleSelection(chk, rowIndex, orderGroupID) 
            {
                var check = chk.checked;
                
                if (orderGroupID > 0) {
                    $(":checkbox[id$=chkSelectOrder]").each(function(chkIndex) {
                        if (this.id != chk.id && this.parentElement.getAttribute('OrderGroupID') == orderGroupID) {
                            this.checked = check;
                        }
                    });
                }
            }
            
            var cboRejectionReason;
            var txtRejectionReason;
            
            function pageLoad()
            {
                cboRejectionReason = "#<%=cboRejectionReason.ClientID %>";
                txtRejectionReason = "#<%=txtRejectionReason.ClientID %>";
            }

            function OpenPODWindowForEdit(scannedFormId, jobId, collectDropId) {
            
                var podFormType = 2;
                var qs = "ScannedFormTypeId=" + podFormType;
                qs += "&JobId=" + jobId;
                qs += "&ScannedFormId=" + scannedFormId;
                qs += "&CollectDropId=" + collectDropId;
                
                <%=dlgDocumentWizard.ClientID %>_Open(qs);
            }
            
            function CallInThis(jobID, instructionID) {
                var qs = "jobid=" + jobID;
                qs += "&instructionid=" + instructionID;
                
                <%=dlgCallIn.ClientID %>_Open(qs);
            }

            function ViewOrder(orderId) {

                var qs = "wiz=true";
                qs += "&Oid=" + orderId;
                
                <%=dlgSubbyOrderProfile.ClientID %>_Open(qs);
            }
            
            function validateRejectionReason(sender, eventArgs) {
                eventArgs.IsValid = $(cboRejectionReason).val() || $(txtRejectionReason).val();
            }

            // The Combostreamers returns HTML which formats the Points in the drop-down. But when selected, the HTML itself will show.
            // Strip the HTML and other address information out leaving only the Point Name/Description.
            function Point_CombBoxClosing(sender, eventArgs) {

                try {
                    var itemText = sender.get_selectedItem().get_text();

                    if (itemText.indexOf('</td><td>') > 0) {
                        // remove any html characters from this. and Show the Point Name only
                        var pointName = itemText.split('</td><td>')[0];

                        pointName = pointName.replace(/&(lt|gt);/g, function (strMatch, p1) {
                            return (p1 == "lt") ? "<" : ">";
                        });
                        pointName = pointName.replace(/<\/?[^>]+(>|$)/g, "");
                        sender.set_text(pointName);
                    }

                }
                catch (e) { }

            }
        </script>

    </telerik:RadCodeBlock>
</asp:Content>
