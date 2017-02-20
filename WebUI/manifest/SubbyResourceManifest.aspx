<%@ Page MasterPageFile="~/default_tableless.master" Language="C#" AutoEventWireup="true" CodeBehind="SubbyResourceManifest.aspx.cs" Inherits="Orchestrator.WebUI.manifest.SubbyResourceManifest " %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" language="javascript">

        $(document).ready(function() {
            ResetResourceManifestJobOrders();
        });

        function ResetResourceManifestJobOrders() {
            var i = 0;

            $('input:hidden[id*=hidJobOrder]').each(function() {
                this.value = i++;
            });
        }
        
        function manifestChanged(input, args) {
            $('input[id*=hidManifestChanged]').val('true');
        }
        function openJobDetailsWindow(jobId) {

            var url = '/job/job.aspx?wiz=true&jobId=' + jobId + getCSID();

            openResizableDialogWithScrollbars(url, '1220', '870');
        }

        showHideMoveButtons();
        $('table[id*=grdResourceManifestJobs] span[id*=spnInstructions]:contains("Job removed")').each(
            function(index, el) {
                $(this).parent().parent().each(
                    function(index, el) {
                        $(this).find('td').each(function(input, el) { el.style["backgroundColor"] = '#FA8072'; });
                    }
                );
            }
        );

        function showHideMoveButtons() {

            // Show all buttons
            $('table[id*=grdResourceManifestJobs] img[name*=move]').show();

            // Hide up button for first row 
            $('table[id*=grdResourceManifestJobs] tr:eq(1) img[name*=moveUpButton]').hide();

            // Hide down button for last row
            $('table[id*=grdResourceManifestJobs] tr:last img[name*=moveDownButton]').hide();
        }

        function up(butt, jobId, resourceManifestJobId) {

            var currentRow = $(butt).parent().parent();
            var previousRow = $(butt).parent().parent().prev();

            if (previousRow != null) {
                currentRow.insertBefore(previousRow);
                $('input[id*=hidManifestChanged]').val('true');
            }

            showHideMoveButtons();
        }

        function down(butt, jobId, resourceManifestJobId) {

            var currentRow = $(butt).parent().parent();
            var nextRow = $(butt).parent().parent().next();

            if (nextRow != null) {
                currentRow.insertAfter(nextRow);
                $('input[id*=hidManifestChanged]').val('true');
            }

            showHideMoveButtons();
        }

        function selectAllCheckboxes(chk) {
            $('table[id*=grdResourceManifestAddJobs] input[id*=chkDriverManifest]').prop('checked', chk.checked);
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Subby Manifest</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="buttonBar">
        <input type="button" value="Go Back to Manifest List" onclick="javascript:window.location = 'ResourceManifestList.aspx';" />&nbsp;
        <input type="button" value="Create a New Manifest" onclick="javascript:window.location = 'SubbyResourceManifestBuilder.aspx';" />
    </div>
    
    <fieldset>
        <legend>Manifest Details</legend>
        <table>
            <tr>
                <td class="formCellLabel">
                    Manifest No.
                </td>
                <td class="formCellField">
                    <asp:Label runat="server" ID="lblManifestNumber"></asp:Label>
                </td>
                <td class="formCellLabel">
                    Manifest Name
                </td>
                <td class="formCellField">
                    <telerik:RadTextBox runat="server" ClientEvents-OnValueChanging="manifestChanged" ID="txtManifestName"
                        Width="300" Enabled="true">
                    </telerik:RadTextBox>
                </td>
                <td class="formCellLabel">
                    Manifest Date
                </td>
                <td class="formCellField">
                    <telerik:RadDateInput DateFormat="dd/MM/yy" ClientEvents-OnValueChanging="manifestChanged"
                        DisplayDateFormat="dd/MM/yy" Width="50" runat="server" ID="dteManifestDate">
                    </telerik:RadDateInput>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <asp:Panel runat="server" ID="pnlExistingManifest">
        <telerik:RadGrid runat="server" ID="grdResourceManifestJobs" Skin="Orchestrator"
            AutoGenerateColumns="false" EnableAJAX="true">
            <MasterTableView DataKeyNames="JobID" AllowSorting="true">
                <Columns>
                    <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <img name="moveUpButton" src="../images/newMasterPage/icon-arrow-up.png" onclick="javascript:up(this, <%#Eval("JobId") %>, <%#Eval("ResourceManifestJobId") %>);" />&nbsp;
                            <img name="moveDownButton" src="../images/newMasterPage/icon-arrow-down.png" onclick="javascript:down(this, <%#Eval("JobId") %>, <%#Eval("ResourceManifestJobId") %>);" />
                            <input type="hidden" runat="server" id="hidJobId" value='<%#Eval("JobId") %>' />
                            <input type="hidden" runat="server" id="hidJobOrder" value="" />
                            <input type="hidden" runat="server" id="hidResourceManifestJobId" value='<%#Eval("ResourceManifestJobId") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Subby Manifest" ItemStyle-Width="30" ItemStyle-HorizontalAlign="Left"
                        DataField="SubContractorID">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkDriverManifest" runat="server" Checked='<%# Eval("Removed").ToString().ToLower() == "true" ? false : true %>' />
                            <asp:HiddenField ID="hidResourceId" runat="server" Value='<%#Eval("SubcontractorIdentityID") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="PIL's" ItemStyle-Width="75" DataField="SubContractorID">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPIL" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn SortExpression="DriverName" HeaderText="Driver" ItemStyle-Width="150"
                        ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <%#Eval("OrganisationName") %>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="JobID" ItemStyle-Width="75" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <asp:Label ID="availableLabel" runat="server" Text="Available"></asp:Label>
                            <a ID="jobIdLink" runat="server" ></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="Earliest Date Time" ItemStyle-Width="130" DataField="EarliestDateTime">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn HeaderText="Instructions" ItemStyle-VerticalAlign="Middle">
                        <ItemTemplate>
                            <span id="spnInstructions" style="font-size: 11px;">
                                <%#Eval("Instructions") %></span>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn>
                        <ItemTemplate>
                            <asp:HiddenField ID="hidOrderIDs" runat="server" Value='<%#Eval("OrderIDs") %>' />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
        <input type="hidden" runat="server" id="hidManifestChanged" value="false" />
        <fieldset>
        <legend>Options</legend>
            <table>
                <tr>
                    <td>
                        Add additional blank rows:&nbsp;<telerik:RadNumericTextBox runat="server" ID="txtExtraRowCount" MinValue="0" MaxValue="20" Width="40px" Type="Number" > </telerik:RadNumericTextBox>
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkExcludeFirstRow" Text="Exclude first row on report" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkUsePlannedTimes" Text="Use Planned Times" />
                    </td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkShowFullAddress" Text="Show Full Address" />
                    </td>
                </tr>
            </table>
        </fieldset>
        <div class="buttonBar">
            <asp:Button runat="server" ID="btnDisplayManifest" OnClientClick="javascript:ResetResourceManifestJobOrders();" Text="Save & Display Manifest" />&nbsp;&nbsp;
            <asp:Button ID="btnCreatePIL" runat="server" Text="Create PIL" />&nbsp;&nbsp;
            <asp:Button runat="server" ID="btnAddJobsToManifest" Text="Add Jobs To Manifest" />
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" ID="pnlAddJobs" Visible="false">
        <fieldset runat="server" id="fieldsetone">
            <legend>Filter Options</legend>
            <table>
                <tr>
                    <td class="formCellLabel">
                        Date From
                    </td>
                    <td class="formCellField">
                        <telerik:RadDateInput runat="server" CssClass="dateInputBox" ID="dteStartdate">
                        </telerik:RadDateInput>
                    </td>
                    <td class="formCellLabel">
                        Date To
                    </td>
                    <td class="formCellField">
                        <telerik:RadDateInput runat="server" CssClass="dateInputBox" ID="dteEndDate">
                        </telerik:RadDateInput>
                    </td>
                </tr>
                <tr>
                    <td colspan="6">
                    </td>
                </tr>
            </table>
            <div class="buttonbar">
                <asp:Button ID="btnGetInstructions" runat="server" Text="Get Data" />
            </div>
            <telerik:RadGrid runat="server" ID="grdResourceManifestAddJobs" Skin="Orchestrator"
                AutoGenerateColumns="false" EnableAJAX="false">
                <MasterTableView DataKeyNames="JobID" AllowSorting="true">
                    <Columns>
                        <telerik:GridTemplateColumn>
                            <HeaderTemplate>
                                <input type="checkbox" checked="checked" ID="chkSelectAll" onclick="javascript:selectAllCheckboxes(this);" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkDriverManifest" runat="server" Checked="true" />
                                <asp:HiddenField ID="hidResourceId" runat="server" Value='<%#Eval("SubcontractorIdentityID") %>' />
                                <input type="hidden" runat="server" id="hidJobId" value='<%#Eval("JobId") %>' />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn SortExpression="OrganisationName" HeaderText="Subcontractor" ItemStyle-Width="150"
                            ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <%#Eval("OrganisationName") %>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="JobID" ItemStyle-Width="75" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <asp:Label ID="availableLabel" runat="server" Text="Available"></asp:Label>
                                <asp:HyperLink ID="jobIdLink" runat="server" Text="" NavigateUrl="" Visible="false"></asp:HyperLink>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridBoundColumn HeaderText="Earliest Date Time" ItemStyle-Width="130" DataField="EarliestDateTime">
                        </telerik:GridBoundColumn>
                        <telerik:GridTemplateColumn HeaderText="Instructions" ItemStyle-VerticalAlign="Middle">
                            <ItemTemplate>
                                <span style="font-size: 11px;">
                                    <%#Eval("Instructions") %></span>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Order Manifest" ItemStyle-Width="75" DataField="SubContractorId"
                            Display="false">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkOrderManifest" runat="server" />
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </fieldset>
        <div class="buttonBar">
            <asp:Button runat="server" ID="btnAddSelectedJob" Visible="false" Text="Add selected jobs to manifest" />
            <asp:Button runat="server" ID="btnCancelAddJobs" Text="Cancel" />
        </div>
    </asp:Panel>

</asp:Content>