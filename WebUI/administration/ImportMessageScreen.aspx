<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ImportMessageScreen.aspx.cs" Inherits="Orchestrator.WebUI.administration.ImportMessageScreen" %>

<%@ Import Namespace="System.Data" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Import Message Screen</h1>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script type="text/javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script src="/script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
        $(document).ready(function () {

            $('#<%= grdImportMessages.ClientID %>_ctl00 tbody tr:not(.GroupHeader_Orchestrator)').quicksearch({
                position: 'after',
                labelText: '',
                attached: '#grdFilterHolder',
                delay: 100,
                onAfter: function () {
                    StoreSearchText();
                }
            });

            FilterOptionsDisplayHide();
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

        function editImportMessage(importmessageID, importMessagePanelID) {

            var importMessageHtml = $('#' + importMessagePanelID);

            new EditImportMessage(importmessageID, importMessageHtml.text().trim()).sendMessage()
                    .done(function () {
                        alert('The message has been sent.');

                        var url = '/job/job.aspx?wiz=true&jobId=' + jobId + getCSID();

                        location = url;
                    })
                    .fail(function (error) {
                        alert(error);
                    });

        }

        var EditImportMessage = function (importmessageID, importMessage) {
            var _this = this;
            var _deferred;
            var _$sendMessageDialog;
            var _txtContent;

            if ($.isReady) {
                showSendMessageDialog();
            }
            else {
                $(function () {
                    showSendMessageDialog();
                });
            }

            var sendMessageBlockUI = function () {
                if ($.blockUI) {
                    $.blockUI({
                        message: '<h1 style="color: #fff">Sending message</h1>',
                        css: {
                            border: 'none',
                            padding: '15px',
                            backgroundColor: '#444',
                            '-webkit-border-radius': '10px',
                            '-moz-border-radius': '10px',
                            'border-radius': '10px'
                        }
                    });
                }
            };

            var sendMessageUnblockUI = function () {
                if ($.unblockUI) {
                    $.unblockUI();
                }
            };

            function showSendMessageDialog() {
                _$sendMessageDialog = $('#EditImportMessage');

                // Initialise dialog
                _$sendMessageDialog.dialog({ autoOpen: false, modal: true, height: 290, width: 1026, dialogClass: "dialogWithDropShadow" });

                _$sendMessageDialog
                    .off('click', '#btnSendMessage') // Unbind any previously attached handler
                    .on('click', '#btnSendMessage', function () {

                        var message = _txtContent.get_value();
                        var hidImportMessageText = $('#hidImportMessageText');
                        var hidImportMessageID = $('#hidImportMessageID');

                        hidImportMessageID.val(importmessageID);
                        hidImportMessageText.val(message);
                        _$sendMessageDialog.dialog('close');
                        $('#btnHiddenUpdateMessage').click();

                    })
                    .off('click', '#btnClose')
                    .on('click', '#btnClose', function () {
                        _$sendMessageDialog.dialog('close');
                    });

                _this.sendMessage = function () {

                    _deferred = new $.Deferred();

                    _txtContent = $find('txtImportMessageContent');
                    _txtContent.set_value(importMessage);

                    Page_ClientValidate('clearValidation');

                    var dlg = _$sendMessageDialog.dialog('open');

                    return _deferred;
                };

            };

        };

        function StoreSearchText() {

            var searchText = $('.qs_input').val();

            // Update counts
            var importTable = $(document.getElementById("<%=grdImportMessages.ClientID %>"));

            var rowCount = importTable.find("tbody").children("tr:visible").length;

            $('#counts').html('Number of Import Messages: ' + rowCount);
        }

        function updateOrder(orderID) {
            var url = "/Groupage/updateOrder.aspx?wiz=true&oID=" + orderID;
            var randomnumber = Math.floor(Math.random() * 11)
            var wnd = window.open(url, randomnumber, "width=1180, height=900, resizable=1, scrollbars=1");
        };

    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField ID="hidImportMessageID" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hidImportMessageText" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="hidImportMessageCount" runat="server" ClientIDMode="Static" Value="0"/>

    <asp:Button ID="btnHiddenUpdateMessage" runat="server" ClientIDMode="Static" Style="display: none;" />

    <div id="EditImportMessage" style="display: none; background-color: White;" title="Edit Import Message">


        <div class="formCellLabel">
            Message
            <span class="required">*</span>
            <asp:RequiredFieldValidator ID="rfvtxtImportMessageContent" runat="server" ControlToValidate="txtImportMessageContent" ErrorMessage="required"
                Display="Dynamic" ValidationGroup="driverMessaging" />
        </div>
        <telerik:RadTextBox runat="server" ID="txtImportMessageContent" Rows="10" TextMode="MultiLine" Width="1000" ClientIDMode="Static" />


        <div class="buttonBar" style="margin-top: 12px;">
            <input type="button" id="btnSendMessage" value="Send" class="buttonClass" />
            <input type="button" id="btnClose" value="Close" class="buttonClass" />
        </div>
    </div>

    <!--Hidden Filter Options-->
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
        <asp:Panel runat="server" ID="panSearch" DefaultButton="btnViewImportMessages">
            <fieldset>
                <legend>Import Message Selection</legend>
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

                                        <asp:CompareValidator Operator="GreaterThanEqual" ID="cvDateValidator" Type="Date"
                                            ControlToValidate="dteDateTo" ControlToCompare="dteDateFrom"
                                            ToolTip="Date from must be before Date To" runat="server">
                                           <img id="imgDateValidator" runat="server" src="/images/error.png" alt="" />

                                        </asp:CompareValidator>
                                    </td>
                                    <td></td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">From System</td>
                                    <td class="formCellField">
                                        <telerik:RadTextBox Width="100" ID="RadFromSystem" runat="server">
                                        </telerik:RadTextBox>
                                    </td>
                                    <td></td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </fieldset>

            <div class="buttonbar">
                <asp:Button ID="btnViewImportMessages" runat="server" Text="View Import Messages" OnClick="btnViewImportMessages_Click" />
            </div>
        </asp:Panel>
    </div>

    <telerik:RadGrid runat="server" ID="grdImportMessages" AllowSorting="True" Width="100%" AutoGenerateColumns="False" Skin="Office2007" 
        AllowMultiRowSelection="True" ShowHeader="true" ShowFooter="true" OnNeedDataSource="grdImportMessages_NeedDataSource" OnItemDataBound="grdImportMessages_ItemDataBound" OnPreRender="grdImportMessages_PreRender"  >
        <MasterTableView DataKeyNames="ImportMessageID" AllowSorting="true" TableLayout="Fixed" CommandItemDisplay="Top">


            <CommandItemTemplate>
                <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
                <asp:Label ID="lblQF" runat="server" Text="Quick Filter:">
                <div id="grdFilterHolder">
                </div>
                </asp:Label>
                <span id="counts" style="padding-left: 25px; vertical-align: middle;">

                    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
                        Number of Import Messages: <%=NumberOfImportMessages %>&nbsp;
                    </telerik:RadCodeBlock>
                </span>
            </CommandItemTemplate>

            <Columns>
                <telerik:GridHyperLinkColumn HeaderText="ID of Order Created" SortExpression="EntityID" DataNavigateUrlFormatString="javascript:updateOrder({0})" DataNavigateUrlFields="EntityID" DataTextField="EntityID" HeaderStyle-Width="10px"></telerik:GridHyperLinkColumn>
                <telerik:GridBoundColumn HeaderText="Date" HeaderStyle-Width="10px" DataField="CreateDate" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                <telerik:GridBoundColumn HeaderText="Status" HeaderStyle-Width="10px" DataField="MessageStateText" ReadOnly="true" ItemStyle-VerticalAlign="Top" />

                <telerik:GridTemplateColumn HeaderText="Message" HeaderStyle-Width="100px" SortExpression="Message">
                    <ItemTemplate>
                        <asp:Panel ID="panMessage" runat="server">
                            <asp:Literal ID="litMessage" runat="server" Mode="Encode"></asp:Literal>
                        </asp:Panel>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Alert" HeaderStyle-Width="50px" SortExpression="Alert">
                    <ItemTemplate>
                        &nbsp;<asp:Label ID="lblAlert" runat="server" Mode="Encode"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="From System" HeaderStyle-Width="20px" SortExpression="FromSystem">
                    <ItemTemplate>
                        &nbsp;<asp:Label ID="lblFromSystem" runat="server" Mode="Encode"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn HeaderText="Entity Type" HeaderStyle-Width="10px" DataField="EntityTypeText" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                <telerik:GridTemplateColumn UniqueName="EditImportMessage" HeaderStyle-Width="10px">
                    <ItemTemplate>
                        <asp:HyperLink ID="EditLink" runat="server" Text="Edit"></asp:HyperLink>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

</asp:Content>
