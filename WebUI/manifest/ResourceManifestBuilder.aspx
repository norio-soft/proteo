<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="True" CodeBehind="ResourceManifestBuilder.aspx.cs"
    Inherits="Orchestrator.WebUI.manifest.ResourceManifestBuilder" Title="Resource Manifest" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript">
    <!--
        function AutoName(dteOrderManifestDate, args) {

            var txtManifestName = $find("<%=txtManifestName.ClientID %>");
            var hidSelectedDriver = $get("<%=hidSelectedDriver.ClientID %>");

            var driverName = hidSelectedDriver.value;

            // Try and parse driver name to swap first name and surname
            try {

                var firstSpaceIndex = driverName.indexOf(" ");
                if (firstSpaceIndex > 0) {
                    var names = driverName.split(" ");

                    if (names.length > 0) {
                        var lastName = names[0];
                        var firstName = driverName.substr(firstSpaceIndex + 1, driverName.length - (firstSpaceIndex + 1));
                        driverName = firstName + ' ' + lastName;
                    }
                }
            } catch (err) { }

            if (driverName.length == 0) {
                driverName = hidSelectedDriver.value;
            }

            txtManifestName.set_value(driverName + ' - ' + args.get_newValue());
        }
        
    //-->
    </script>

    <input runat="server" type="hidden" id="hidSelectedDriver" />
    <div class="buttonBar">
        <input type="button" value="Go Back to Manifest List" onclick="javascript:window.location = 'ResourceManifestList.aspx';" />&nbsp;
    </div>
    <h1>
        Resource Manifest Builder</h1>
    <!--<h2>Description here</h2>-->
    <fieldset>
        <legend>Manifest Details</legend>
        <table>
            <tr>
                <td class="formCellLabel">
                    Manifest Name
                </td>
                <td class="formCellField">
                    <telerik:RadTextBox runat="server" ID="txtManifestName" Width="300" Enabled="true">
                    </telerik:RadTextBox>
                </td>
                <td class="formCellLabel">
                    Manifest Date
                </td>
                <td class="formCellField">
                    <telerik:RadDateInput DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="50"
                        runat="server" ID="dteManifestDate" OnClientDateChanged="AutoName">
                        <ClientEvents OnValueChanged="AutoName" />
                    </telerik:RadDateInput>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset runat="server" id="fieldsetone">
        <legend>Filter Options</legend>
        <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
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
                    <td class="formCellLabel">
                        Type
                    </td>
                    <td class="formCellField">
                        <asp:RadioButtonList ID="rblType" AutoPostBack="true" runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Text="Driver" Value="1" Selected="True"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Driver Category
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboDriverType" runat="server" Enabled="true" DataTextField="Description"
                            DataValueField="DriverTypeID">
                        </asp:DropDownList>
                    </td>
                    <td class="formCellLabel">
                        Depot
                    </td>
                    <td class="formCellField">
                        <asp:DropDownList ID="cboDepot" runat="server" DataTextField="OrganisationLocationName"
                            DataValueField="OrganisationLocationID">
                        </asp:DropDownList>
                    </td>
                </tr>
            </table>
        </telerik:RadAjaxPanel>
    </fieldset>
    <div class="buttonbar">
        <asp:Button ID="btnGetInstructions" runat="server" Text="Get Data" />
    </div>
    <telerik:RadGrid runat="server" ID="grdManifests" Skin="Orchestrator" AutoGenerateColumns="false"
        EnableAJAX="true">
        <MasterTableView DataKeyNames="JobID" AllowSorting="true">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Driver Manifest" ItemStyle-Width="30" ItemStyle-HorizontalAlign="Left"
                    DataField="DriverResourceID">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkDriverManifest" runat="server" />
                        <asp:HiddenField ID="hidResourceId" runat="server" Value='<%#Eval("DriverResourceID") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="PIL's" ItemStyle-Width="75" DataField="DriverIdentityID">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkPIL" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn SortExpression="DriverName" HeaderText="Driver" ItemStyle-Width="150"
                    ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <a href="drivermanifest.aspx?jID=<%#(Eval("JobID") == DBNull.Value) ? -1 : Eval("JobID")%>&drID=<%# Eval("DriverResourceID")%>&sd=<%=dteStartdate.SelectedDate %>&ed=<%=dteEndDate.SelectedDate %>"
                            target="_blank">
                            <%#Eval("DriverName") %>
                        </a>
                        <input type="hidden" runat="server" id="hidDriverName" value='<%#Eval("DriverName") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn SortExpression="ResourceManifestId" HeaderText="Manifest"
                    ItemStyle-Width="150" ItemStyle-VerticalAlign="Middle">
                    <ItemTemplate>
                        <a id="hypManifest" runat="server"></a>
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
                <telerik:GridTemplateColumn HeaderText="Instructions" ItemStyle-VerticalAlign="Top">
                    <ItemTemplate>
                        <span style="font-size: 11px;">
                            <%#Eval("Instructions") %></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn>
                    <ItemTemplate>
                        <asp:HiddenField ID="hidOrderIDs" runat="server" Value='<%#Eval("OrderIDs") %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Order Manifest" ItemStyle-Width="75" DataField="DriverIdentityID"
                    Display="false">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkOrderManifest" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <SelectFields>
                        <telerik:GridGroupByField FieldName="DriverType" HeaderText="Driver Type" />
                    </SelectFields>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldName="DriverType" HeaderText="Driver Type" />
                    </GroupByFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>
            <SortExpressions>
                <telerik:GridSortExpression FieldName="DriverName" SortOrder="Ascending" />
                <telerik:GridSortExpression FieldName="EarliestDateTime" SortOrder="Ascending" />
            </SortExpressions>
        </MasterTableView>
    </telerik:RadGrid>
    <div class="whiteSpacePusher">
    </div>
    <div class="buttonbar">
        <asp:Button ID="btnCreatePIL" runat="server" Text="Create PIL" />
        <asp:Button ID="btnDriverManifests" runat="server" Text="Create Driver Manifests" />
    </div>

    <script type="text/javascript">
        function openJobDetailsWindow(jobId) {
            var url = '../Job/Job.aspx?wiz=true';
            url += ('&jobId=' + jobId) + getCSID();
            openDialogWithScrollbars(url, '1220', '870');
        }

        // Disable driver manifest check boxes for rows that have a manifest.
        $('table[id*=grdManifests] a[id*=hypManifest]:not(:empty)').each(
            function(index, el) {
                $(this).parent().parent().find('input[id*=chkDriverManifest]').each(
                    function(index, el) { el.disabled = 'disabled'; }
                );
            }
        );

        function driverCheckChange(driverResourceId, checkbox) {

            var checkedStatus = checkbox.checked;
            
            if (checkbox.checked == true) {

                // Set the selected driver hidden field
                var driverName = $(checkbox).parent().parent().find('input[id*=hidDriverName]').val();
                var manifestDate = $find("<%=dteManifestDate.ClientID %>");
                var hidSelectedDriver = $('#<%=hidSelectedDriver.ClientID %>');
                hidSelectedDriver.val('');
                hidSelectedDriver.val(driverName);
                
                var txtManifestName = $find("<%=txtManifestName.ClientID %>");

                // Try and parse driver name to swap first name and surname
                try {
                
                    var firstSpaceIndex = driverName.indexOf(" ");
                    if (firstSpaceIndex > 0) {
                        var names = driverName.split(" ");

                        if (names.length > 0) {
                            var lastName = names[0];
                            var firstName = driverName.substr(firstSpaceIndex + 1, driverName.length - (firstSpaceIndex + 1));
                            driverName = firstName + ' ' + lastName;
                        }
                    }
                } catch (err) { }

                txtManifestName.set_value(driverName + ' - ' + manifestDate.get_value());

                $('table[id*=grdManifests] input[id*=hidResourceId][value=' + driverResourceId + ']').parent().find('input[id*=chkDriverManifest]').each(
                   function(index, chk) {
                       if (chk.disabled != true) {
                           chk.checked = checkedStatus;
                       }
                   }
                );

                $('table[id*=grdManifests] input[id*=hidResourceId][value!=' + driverResourceId + ']').parent().find('input[id*=chkDriverManifest]').each(
                   function(index, chk) {
                       chk.checked = false;
                   }
                );

                return false;
            }
        }
        
    </script>

</asp:Content>
