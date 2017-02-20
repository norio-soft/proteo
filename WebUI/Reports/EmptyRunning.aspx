<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" CodeBehind="EmptyRunning.aspx.cs" Inherits="Orchestrator.WebUI.Reports.EmptyRunning" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="chklstVal" Namespace="P1TP.Components.Web.CheckBoxListValidator" Assembly="P1TP.Components"%>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Empty Running</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script language="javascript" type="text/javascript">
        // Function to show the filter options overlay box
        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });
            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "block";
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
            var fr = document.getElementById ("overlayedIframe");
            fr.style.display ="none";

        }

        $(document).ready(function () {
            var width = $("#overlayedClearFilterBox").width();
            var height = $("#overlayedClearFilterBox").height();
            var position = $("#overlayedClearFilterBox").position();
            $("#overlayedIframe").css("width", width);
            $("#overlayedIframe").css("height", height + 25);
            $("#overlayedIframe").css("top", position.top);
            $("#overlayedIframe").css("left", position.left);
            FilterOptionsDisplayHide();
        });

        /*Fix that allows menu bars to overlay reports when using Adobe PDF plugin for Google Chrome */
        window.onload = function () {
            changeIframeToObject();
        }

        function changeIframeToObject() {
            $("iframe[src*=\"/Reports\"]").replaceWith("<object data=\"/Reports/ActiveReportPdf.ashx\" type=\"application/pdf\" width=\"100%\" height=\"1000\" style=\"z-index: 1;\"></object>")
        }

        Telerik.Web.UI.Overlay.IsSupported = function () {
            return true;
        }

    </script>
    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">
        <h1>Empty Running</h1>
        <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
            <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick=" FilterOptionsDisplayShow() " style="display: none;">Show filter Options</div>
            <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick=" FilterOptionsDisplayHide() ">Close filter Options</div>
            <asp:Button ID="btnExport" runat="server" Text="Export" 
                ValidationGroup="grpRefresh" CausesValidation="true" 
                onclick="btnExport_Click" />
            <asp:Button ID="btnExportSummary" runat="server" Text="Export Summary" 
                ValidationGroup="grpRefresh" CausesValidation="true" 
                onclick="btnExportSummary_Click" />
        </div>
        <!--Hidden Filter Options-->
        <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
            <fieldset>
                <legend>Filter Options</legend>
                <table>
                    <tr>
                        <td runat="server" id="tdDateOptions">
                            <table>
                                <tr>                               
                                    <td class="formCellLabel" style="width: 100px;">Date From</td>
                                    <td class="formCellField">
                                        <telerik:RadDatePicker id="dteStartDate" runat="server" ToolTip="The Start Date for the filter" Width="100" TabIndex="1">
                                        <DateInput runat="server"
                                        dateformat="dd/MM/yy">
                                        </DateInput>
                                        </telerik:RadDatePicker>
                                        <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter a Start Date">
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td class="formCellLabel" style="width: 100px;">Date To</td>
                                    <td class="formCellField">
                                        <telerik:RadDatePicker id="dteEndDate" runat="server" ToolTip="The Start Date for the filter" Width="100" TabIndex="2">
                                        <DateInput runat="server"
                                        dateformat="dd/MM/yy">
                                        </DateInput>
                                        </telerik:RadDatePicker>
                                        <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ValidationGroup="grpRefresh" ErrorMessage="Please enter an End Date" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">
                                        Clients
                                    </td>
                                    <td>
                                        <telerik:RadListBox runat="server" ID="lbAvailableClients" Width="400" EnableDragAndDrop="true"
                                                            AllowTransfer="true" TransferToID="lbSelectedClients" AllowTransferDuplicates="false"
                                                            Height="400" MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                                                            SelectionMode="Multiple"  DataValueField="IdentityId" 
                                                            DataTextField="OrganisationName" AllowReorder="False" TabIndex="3" EnableMarkMatches="True" ButtonSettings-RenderButtonText="true" ButtonSettings-AreaHeight="200" ButtonSettings-AreaWidth="120"/>
                                    </td>
                                    <td class="formCellLabel">
                                        Selected Clients
                                    </td>
                                    <td>
                                        <telerik:RadListBox runat="server" ID="lbSelectedClients" Width="300" EnableDragAndDrop="true"
                                                            Height="400" SelectionMode="Multiple"  DataValueField="IdentityId" 
                                                            DataTextField="OrganisationName" AllowReorder="False" TabIndex="4" EnableMarkMatches="true"/>
                                       <asp:CustomValidator runat="server" ID="cvSelectedClients" ControlToValidate="lbSelectedClients" ClientValidationFunction="validateSelectedClients" ValidationGroup="grpRefresh" ErrorMessage="Please select one or more clients" ValidateEmptyText="True"></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel">
                                        Traffic Area
                                    </td>
                                    <td class="formCellField" colspan="3">
                                        <asp:CheckBox ID="chkSelectAllTrafficAreas" runat="server" Checked="false" Text="Select all Traffic Areas"
                                                      Style="padding-left: 3px;" />
                                        <asp:CheckBoxList ID="cblTrafficAreas" runat="server" DataValueField="TrafficAreaID"
                                                          DataTextField="Description" RepeatDirection="Horizontal" RepeatColumns="8">
                                                          
                                        </asp:CheckBoxList>
                                        <asp:CustomValidator runat="server" ID="cvtrafficarea" ValidationGroup="grpRefresh"
                                    ClientValidationFunction="ValidateTrafficList"
                                    ErrorMessage="Please Select at least one Traffic Area" ></asp:CustomValidator>

                                        </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel" style="width: 100;">Grouping</td>
                                    <td class="formCellField" style="width: 50;">
                                        <telerik:RadButton ID="radGrouping1" runat="server" Text="Traffic Area, Client" GroupName="ReportGrouping" ToggleType="Radio" AutoPostBack="False" ButtonType="ToggleButton" Checked="True"></telerik:RadButton>
                                        <telerik:RadButton ID="radGrouping2" runat="server" Text="Client, Traffic Area" GroupName="ReportGrouping" ToggleType="Radio" AutoPostBack="False" ButtonType="ToggleButton"></telerik:RadButton>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </fieldset>
            <div class="buttonbar">
                <asp:Button ID="btnRefresh" runat="server" Text="Refresh" 
                    ValidationGroup="grpRefresh" CausesValidation="true" TabIndex="5" 
                    onclick="btnRefresh_Click"/>
            </div>
        </div>
        <div>
            <uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:ReportViewer>
        </div>
    </asp:Panel>
    <script type="text/javascript">
        $(document).ready(function() {
            // Select all  traffic areas when select all is clicked.
            $(":checkbox[id*='chkSelectAllTrafficAreas']").click(function() {
                var checked_status = this.checked;
                $(":checkbox[id*='cblTrafficAreas']").each(function() {
                    this.checked = checked_status;
                });
            });
        });

        function cboClient_itemsRequesting(sender, eventArgs) {
            try {
                var context = eventArgs.get_context();
                if ('<%= string.IsNullOrEmpty(Request.QueryString["oid"]) %>' == 'True') {
                    context["DisplaySuspended"] = false;
                } else {
                    context["DisplaySuspended"] = true;
                }
            } catch(err) {
            }
        }

        

        function onTrafficAreaChecked() {
            allInCheckboxListChecked($(":checkbox[id*='cblTrafficAreas']"), $(":checkbox[id*='chkSelectAllTrafficAreas']"));
        }

        function allInCheckboxListChecked(chkboxlist, chkbox) {
            var allChecked = true;
            chkboxlist.each(function() {
                if (!this.checked)
                    allChecked = false;
            });

            chkbox[0].checked = allChecked;
        }

        function validateSelectedClients(source, eventArgs) {
            var list = $find("<%= lbSelectedClients.ClientID %>");
            eventArgs.IsValid = list.get_items().get_count() > 0;
        }

        function ValidateTrafficList(source, args) {

            var chkListModules = document.getElementById('<%= cblTrafficAreas.ClientID %>');
            
            var chkListinputs = chkListModules.getElementsByTagName("input");
            for (var i = 0; i < chkListinputs.length; i++) {
                if (chkListinputs[i].checked) {
                    
                    args.IsValid = true;
                    return;
                }
            }
            
            args.IsValid = false;
        }

    </script>
</asp:Content>