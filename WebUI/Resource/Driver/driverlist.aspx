<%@ Page Language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.DriverList" CodeBehind="DriverList.aspx.cs" MasterPageFile="~/default_tableless.Master"   Title="Driver List" %>

<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>
<%@ Register TagPrefix="uc" TagName="MwfDriverMessaging" Src="~/UserControls/mwf/DriverMessaging.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Driver List</h1></asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script src="/script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js" type="text/javascript"></script>    

    <script type="text/javascript">
        function ShowFuture(resourceId, resourceTypeId, fromDate) {
            var url = '../../Resource/Future.aspx?wiz=true&resourceId=' + resourceId + '&resourceTypeId=' + resourceTypeId + '&fromDateTime=' + fromDate;
            openResizableDialogWithScrollbars(url, 1050, 502);
        }

        function SetAvailability(resourceId, chkBox) {
            // Update the resource availability using AJAX for speed.

            var pageUrl = webserver + "/Resource/UpdateResourceAvailability.aspx?resourceId=" + resourceId + "&resourceTypeId=3&resourceStatusId=";
            if (chkBox.checked)
                pageUrl += "1";
            else
                pageUrl += "4";

            var xmlRequest = new XMLHttpRequest();

            xmlRequest.open("POST", pageUrl, false);
            xmlRequest.setRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            xmlRequest.send(null);

            if (xmlRequest.responseText.toLowerCase() == "failed") {
                chkBox.checked = !chkBox.checked;
                alert("The resource was not updated.");
            }
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="lblNote" runat="server" Text=""></asp:Label>
    <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox">
        <fieldset>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Last Name
                    </td>
                    <td class="formCellField">
                        <asp:TextBox runat="server" id="txtFilterLastName"></asp:TextBox>
                    </td>
                    <td class="formCellLabel">
                        First Name
                    </td>
                    <td class="formCellField">
                        <asp:TextBox runat="server" ID="txtFilterFirstName"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonbar">
            <asp:Button ID="btnRefresh" runat="server" Text="Refresh" Width="75" />
            <input type="button" id="Button2" runat="server" value="Cancel and close" onclick="FilterOptionsDisplayHide()" />
        </div>
    </div>
    
    <h2>Please choose a driver from the list below.</h2>
    
    <cc1:Dialog ID="dlgAddUpdateDriver" URL="addupdatedriver.aspx" Width="600" Height="720"
        AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" Scrollbars="false">
    </cc1:Dialog>
    
    <telerik:RadGrid PageSize="100" EnableViewState="false" ID="grdDrivers" runat="server" AllowSorting="True" AllowMultiRowSelection="true">
        <ClientSettings>
              <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />  
              <ClientEvents OnRowSelecting="grid_rowSelecting" OnGridCreated="grid_created" />
        </ClientSettings>
        <MasterTableView ClientDataKeyNames="IdentityId, ResourceID" DataKeyNames="IdentityId, ResourceID" AutoGenerateColumns="False" CommandItemDisplay="Top" GroupLoadMode="Client" CommandItemStyle-BackColor="White">
            <RowIndicatorColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>
            <ExpandCollapseColumn>
                <HeaderStyle Width="20px"></HeaderStyle>
            </ExpandCollapseColumn>
            <CommandItemTemplate>
                <input type="button" runat="server" class="buttonClassSmall" value="Send Message" id="btnSendDriverMessage" onclick="sendDriverMessage()" />
                <input runat="server" type="button" class="buttonClassSmall" value="Add Driver" id="btnAddDriver" />
                <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none">
                    Show filter Options</div>
                <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">
                    Close filter Options</div>
                <asp:Label ID="lblQF" runat="server" Text="Quick Filter:" />
                <div id="grdFilterHolder">
                </div>
            </CommandItemTemplate>
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn" />
                <telerik:GridBoundColumn HeaderText="Identity Id" DataField="IdentityId" UniqueName="IdentityId" DataType="System.Int" ReadOnly="True" SortExpression="IdentityId" Visible="false">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Full Name">
                    <ItemTemplate>
                        <a runat="server" id="hypViewDriver"></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Usual Vehicle" DataField="RegNo" UniqueName="RegNo"
                    ReadOnly="True" SortExpression="RegNo">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Availability" UniqueName="Avail">
                    <ItemTemplate>
                        <input type="checkbox" id="chkAvailability" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Is Agency Driver">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblIsAgencyDriver"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Owned By" DataField="OrganisationLocationName"
                    UniqueName="OrganisationLocationName" ReadOnly="True" SortExpression="OrganisationLocationName">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Mobile" DataField="MobilePhone" UniqueName="MobilePhone"
                    ReadOnly="True" SortExpression="MobilePhone">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Digital Tacho" DataField="DigitalTachoCardId" UniqueName="Tacho" ReadOnly="True" SortExpression="RegNo" Visible="false"/>
                <telerik:GridBoundColumn HeaderText="Phone" DataField="HomePhone" UniqueName="HomePhone"
                    ReadOnly="True" SortExpression="HomePhone">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Personal Mobile" DataField="PersonalMobile" UniqueName="PersonalMobile"
                    ReadOnly="True" SortExpression="PersonalMobile">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Current Controller" DataField="DepotCode" UniqueName="DepotCode"
                    ReadOnly="True" SortExpression="DepotCode">
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Current Location" DataField="CurrentLocation"
                    UniqueName="CurrentLocation" ReadOnly="True" SortExpression="CurrentLocation">
                </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Default Communication" DataField="CommunicationType" UniqueName="CommunicationType" ReadOnly="True" SortExpression="CommunicationType">
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn HeaderText="Show Future" UniqueName="ShowFuture">
                    <ItemTemplate>
                        <a runat="server" id="hypShowFuture"></a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <FilterMenu EnableTheming="True">
            <CollapseAnimation Type="OutQuint" Duration="200"></CollapseAnimation>
        </FilterMenu>
    </telerik:RadGrid>
    
    <div class="whitepacepusher"></div>

    <uc:MwfDriverMessaging runat="server" ID="mwfDriverMessaging" />

    <script type="text/javascript">
        var selected = {};
        
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

        $(document).ready(function() {
            FilterOptionsDisplayHide();
            $('#<%= grdDrivers.ClientID %>_ctl00 tbody tr:not(.GroupHeader_Orchestrator)').quicksearch({
                position: 'after',
                labelText: '',
                attached: '#grdFilterHolder',
                delay: 100
            });

        });

        var sendDriverMessage = function () {
            var grdDrivers = $find('<%= grdDrivers.ClientID %>');
                var selectedDriverIDs = $.map(grdDrivers.get_selectedItems(), function (i) { return i.getDataKeyValue('ResourceID'); });

                if (selectedDriverIDs.length > 0) {
                    new MwfDriverMessaging(selectedDriverIDs).sendMessage()
                        .done(function() {
                            alert('The message has been sent.');
                            $('[id$=btnRefresh]').trigger('click');
                        })
                        .fail(function (error) {
                            alert(error);
                        });
                }
                else {
                    alert('Please first select one or more drivers to send a message to');
                }
        }

        function grid_rowSelecting(sender, args) {
            // Client select checkboxes are disabled for drivers with no passcode, but clicking the "Select All" checkbox will still cause these rows to become selected.
            // This stops that from happening.
            var isSelectDisabled = $(args.get_gridDataItem().get_element()).find('[id$=ClientSelectColumnSelectCheckBox]').prop('disabled');
            
            if (isSelectDisabled) {
                args.set_cancel(true);
            }
        }

        function grid_created(sender, eventArgs) {
            var masterTable = sender.get_masterTableView(),
              headerCheckBox = $telerik.$(masterTable.HeaderRow).find(":checkbox")[0];

            if (headerCheckBox) {
                headerCheckBox.checked = masterTable.get_selectedItems().length == masterTable.get_pageSize();
            }
        }

    </script>

</asp:Content>