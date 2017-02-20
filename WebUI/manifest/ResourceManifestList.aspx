<%@ Page MasterPageFile="~/default_tableless.master" Language="C#" AutoEventWireup="true" CodeBehind="ResourceManifestList.aspx.cs" Inherits="Orchestrator.WebUI.manifest.ResourceManifestList" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Driver Manifests <asp:Label ID="lblTitle" runat="server" Font-Bold="false" Font-Size="Small" ></asp:Label></h1>
</asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript">
        function ScanManifest(manifestId) {
            var url = "/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=5&ManifestId=" + manifestId;
            openResizableDialogWithScrollbars(url, 550, 500);
        }
        function ScanExistingManifest(manifestId, scannedFormId) {
            var url = "/scan/wizard/ScanOrUpload.aspx?ScannedFormTypeId=5&ManifestId=" + manifestId + "&ScannedFormId=" + scannedFormId;
            openResizableDialogWithScrollbars(url, 550, 500);
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
        
        function PrintManifests() {
            var ids = '';
            for (var k in selected) {
                if (selected.hasOwnProperty(k)){
                    if (selected[k]) {
                        if (ids.length > 0)
                            ids += ",";

                        ids += k;
                    }
                }
            }
            if (ids.length > 0) {
                var url = "printmultiplemanifests.aspx?generate&i=" + ids;
                window.open(url);
            }
            else {
                alert("No Manifests ticked.");
            }
            
        }

        var selected = {};
        var manifests = new Object();
        function RadGrid1_RowSelected(sender, args) {
            var manifestID = args.getDataKeyValue("ResourceManifestId");
            if (!selected[manifestID]) {
                selected[manifestID] = true;
            }
        }

        function RadGrid1_RowDeselected(sender, args) {
            var manifestID = args.getDataKeyValue("ResourceManifestId");
            if (selected[manifestID]) {
                selected[manifestID] = null;
            }
        }

        function RadGrid1_RowCreated(sender, args) {
            var manifestID = args.getDataKeyValue("ResourceManifestId");
            if (selected[manifestID]) {
                args.get_gridDataItem().set_selected(true);
            }
        }

        function GridCreated(sender, eventArgs) {
            var masterTable = sender.get_masterTableView(),
              headerCheckBox = $telerik.$(masterTable.HeaderRow).find(":checkbox")[0];

            if (headerCheckBox) {
                headerCheckBox.checked = masterTable.get_selectedItems().length == masterTable.get_pageSize();
            }
        }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    
    <telerik:RadAjaxPanel ID="radPanel" runat="server">    
        <h2>
            All manifests for the specified date range, you can change which manifests
            you see by changing the dates and clicking on Get Manifests
        </h2>
        <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
                        <input style="width: 200px;" value="Create New Driver Manifest" type="button" onclick="javascript:window.location = 'ResourceManifestBuilder.aspx';" />
            <input style="width: 200px;" value="Create New Subby Manifest" type="button" onclick="javascript:window.location = 'SubbyResourceManifestBuilder.aspx';" />

            <input type="button" value="Print Selected" class="butonClass" onclick="PrintManifests();" />
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
                <fieldset>
            <legend>Dates</legend>
        <div style="font-size: 210%">
            <asp:ValidationSummary runat="server" />
            <span>
                From
                <telerik:RadDatePicker ID="dteStartDate" EmptyMessage="Enter Date" Width="100" runat="server">
                <DateInput runat="server"
                 DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy">
                 </DateInput>
                </telerik:RadDatePicker>
                <asp:RequiredFieldValidator ID="rfvStartDate" ControlToValidate="dteStartDate" runat="server" ErrorMessage="You must give a start date" Display="None"></asp:RequiredFieldValidator>
            </span>
            <span>
                To
                <telerik:RadDatePicker ID="dteEndDate" runat="server" Width="100" EmptyMessage="Enter Date">
                <DateInput runat="server"
                DisplayDateFormat="dd/MM/yy" DateFormat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker>
                <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="You must give an end date" Display="None"></asp:RequiredFieldValidator>
            </span>
            <asp:CustomValidator ID="cvDateRange" runat="server" ErrorMessage="The date range is too large." Display="None" ControlToValidate="dteEndDate"></asp:CustomValidator>
        </div>
        </fieldset>
                <div class="buttonbar" style="margin: 10px 0px 5px 0px;">
                <asp:Button ID="btnGetManifests" runat="server" CssClass="buttonClass" Text="Get Manifests" />
                
                
        </div>
        </div>
        
        <telerik:RadGrid runat="server" ID="grdResourceManifestList" Skin="Orchestrator"
            AllowFilteringByColumn="false" AllowPaging="true" AllowSorting="true" AllowMultiRowSelection="true"
            EnableAJAX="true" AutoGenerateColumns="false" ShowStatusBar="false" PageSize="250">
            <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
                 <ClientEvents OnRowCreated="RadGrid1_RowCreated" OnRowSelected="RadGrid1_RowSelected"
          OnRowDeselected="RadGrid1_RowDeselected" OnGridCreated="GridCreated" />
            </ClientSettings>
           
            <PagerStyle NextPageText="Next &amp;gt;" PrevPageText="&amp;lt; Prev" Mode="NextPrevAndNumeric">
            </PagerStyle>
            <MasterTableView DataKeyNames="ResourceManifestId" ClientDataKeyNames="ResourceManifestId">
            
                <RowIndicatorColumn Display="false">
                </RowIndicatorColumn>
                <Columns>
                    <telerik:GridClientSelectColumn></telerik:GridClientSelectColumn>
                    <telerik:GridTemplateColumn UniqueName="ResourceManifestId" HeaderText="Manifest No."
                        DataField="ResourceManifestId" SortExpression="ResourceManifestId" AllowFiltering="true">
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="hypResourceManifestId"></asp:HyperLink>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="ViewOrScan" HeaderText="Scan" HeaderStyle-Width="100px"
                        DataField="ScannedFormId" SortExpression="ScannedFormId" AllowFiltering="false">
                        <ItemTemplate>
                            <asp:HyperLink runat="server" ID="hypViewScan" Visible="false" Target="_blank"></asp:HyperLink>
                            <a runat="server" id="hypScan"></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="Description" DataField="Description" HeaderText="Name"
                        AllowFiltering="true" />
                    <telerik:GridTemplateColumn UniqueName="ManifestDate" HeaderText="Manifest Date"
                        AllowFiltering="true">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblManifestDate"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="CreateDate" DataField="CreateDate" HeaderText="Created"
                        AllowFiltering="true" />
                        <telerik:GridTemplateColumn UniqueName="ResourceName" headertext="Who" AllowFiltering="true">
                            <ItemTemplate><%# string.IsNullOrEmpty(Eval("ResourceName").ToString()) ? Eval("OrganisationName") : Eval("ResourceName") %></ItemTemplate>
                        </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn UniqueName="CreateUserId" DataField="CreateUserId" HeaderText="Created By"
                        AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="LastUpdateDate" DataField="LastUpdateDate" HeaderText="Last Updated"
                        AllowFiltering="true" />
                    <telerik:GridBoundColumn UniqueName="LastUpdateUserId" DataField="LastUpdateUserId"
                        HeaderText="Last Updated By" AllowFiltering="true" />
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
     
    </telerik:RadAjaxPanel>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>