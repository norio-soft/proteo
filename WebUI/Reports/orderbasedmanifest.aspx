<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Traffic.OrderBasedManifest" CodeBehind="orderbasedmanifest.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>

<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Order Manifest</h1></asp:Content> 

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
    <!--
        function AutoName(dteOrderManifestDate, args) {
            var txtOrderManifestName = $find("<%=txtOrderManifestName.ClientID %>");

            try {
                var cboSubContractor = $find("<%=cboSubContractor.ClientID%>");
                var cboResource = $find("<%=cboResource.ClientID%>");
            } catch (err) { /* The combo's will not evaluate when they are hidden */ }

            if (cboSubContractor != null) {
                if (cboSubContractor.get_text() != "") {
                    txtOrderManifestName.set_value(cboSubContractor.get_text() + ' - ' + args.get_newValue());
                }
                else {
                    txtOrderManifestName.set_value(cboResource.get_text() + ' - ' + args.get_newValue());
                }
            }
            else {
                var lblResourceOrSubcontractorName = document.getElementById("<%=lblResourceOrSubcontractorName.ClientID%>");
                txtOrderManifestName.set_value(lblResourceOrSubcontractorName.innerText + ' - ' + args.get_newValue());
            }
        }
        
    //-->
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table width="100%" border="0" cellpadding="3px" cellspacing="0">
        <tr>
            <td>
                <div class="buttonBar" style="height: 30px; background-color: #99BEDE;">
                    <input style="width: 175px;" value="Go Back to Manifest List" type="button" onclick="javascript:window.location = 'orderbasedmanifestlist.aspx';" />
                    <input style="width: 175px;" value="Create New Order Manifest" type="button" onclick="javascript:window.location = 'orderbasedmanifest.aspx';" />
                </div>
            </td>
        </tr>
    </table>
    
    <fieldset>
        <legend>Manifest Details</legend>
        <table cellpadding="3px">
            <tr>
                <td>
                    <div>
                        <table cellpadding="5px;">
                            <tr runat="server" id="trResourceSubcontractorComboSection">
                                <td colspan="2" width="45%">
                                    Resource:<div>
                                    </div>
                                    <telerik:RadComboBox ID="cboResource" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                        ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px" OnClientSelectedIndexChanged="cboResource_ClientSelectedIndexChanged">
                                    </telerik:RadComboBox>
                                </td>
                                <td colspan="2" width="50%">
                                    Sub-Contractor:<div>
                                    </div>
                                    <telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true"
                                        ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                                        AllowCustomText="False" ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px"
                                        Height="300px" OnClientSelectedIndexChanged="cboSubContractor_ClientSelectedIndexChanged">
                                    </telerik:RadComboBox>
                                </td>
                            </tr>
                            <tr runat="server" id="trManifestNumber" visible="true">
                                <td valign="top">
                                    <b>Manifest No.</b>
                                </td>
                                <td style="width: 350px;">
                                    <asp:Label runat="server" ID="lblOrderManifestNumber" Text="Pending..."></asp:Label>
                                </td>
                                <td style="width: 15%;">
                                    <b>Manifest&nbsp;Date:</b>
                                </td>
                                <td align="left">
                                    <telerik:RadDatePicker  Width="100"
                                        runat="server" ID="dteOrderManifestDate">
                                        <DateInput runat="server"
                                        DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" OnClientDateChanged="AutoName">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                            </tr>
                            <tr>
                                <td align="left" width="50px">
                                    <b>Manifest&nbsp;Name:</b>
                                </td>
                                <td valign="top" align="left">
                                    <telerik:RadTextBox runat="server" ID="txtOrderManifestName" Width="300" Enabled="true">
                                    </telerik:RadTextBox>
                                </td>
                                <td>
                                    <asp:Label Font-Bold="true" runat="server" Text="Resource/Subcontractor:" ID="lblResourceOrSubcontractorNameLabel"></asp:Label>
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblResourceOrSubcontractorName"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        <legend>Current Orders</legend>
        <table cellpadding="3px">
            <tr>
                <td>
                    <div style="height: 10px;">
                    </div>
                    <asp:Panel runat="server" ID="pnlOrdersCurrentlyOnManifest" Visible="false">
                        To remove an order from the manifest simply unselect it then click the &quot;Save
                        and Display Manifest&quot; button below.
                        <br />
                        <asp:CheckBox runat="server" ID="chkRemoveFromJob" Text=" Remove order from job when removing from manifest."
                            Checked="true" /><div style="height: 10px;">
                            </div>
                    </asp:Panel>
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid runat="server" ID="grdManifest" AllowFilteringByColumn="false" Skin="Orchestrator"
                        AllowSorting="true" AllowMultiRowSelection="true" EnableAJAX="true"
                        AutoGenerateColumns="false" ShowStatusBar="false">
                        <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
                        </ClientSettings>
                        <PagerStyle NextPageText="Next &amp;gt;" PrevPageText="&amp;lt; Prev" Mode="NextPrevAndNumeric">
                        </PagerStyle>
                        <MasterTableView DataKeyNames="OrderId">
                            <RowIndicatorColumn Display="false">
                            </RowIndicatorColumn>
                            <Columns>
                                <telerik:GridTemplateColumn UniqueName="OrderId" HeaderText="" DataField="OrderId"
                                    SortExpression="OrderId">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkOrderID" runat="server"></asp:CheckBox>
                                        <asp:HiddenField ID="hidOrderId" runat="server"></asp:HiddenField>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderId">
                                    <ItemTemplate>
                                        <a id="hypOrderId" runat="server">
                                            <%# ((System.Data.DataRowView)Container.DataItem)["OrderId"].ToString()%></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Customer" SortExpression="Customer">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Start From" SortExpression="StartFrom">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartFrom" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Start At" SortExpression="StartAt">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartAt" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Destination" HeaderText="Destination" SortExpression="Destination" />
                                <telerik:GridBoundColumn DataField="ArriveAtDisplay" HeaderText="Arrive At" SortExpression="ArriveAt" />
                                <telerik:GridBoundColumn DataField="Items" HeaderText="Items" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="Items" />
                                <telerik:GridBoundColumn DataField="Weight" HeaderText="Weight" ItemStyle-HorizontalAlign="Right"
                                    DataFormatString="{0:F0}" SortExpression="Weight" />
                                <telerik:GridBoundColumn DataField="Reference" HeaderText="Reference" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="Reference" />
                                <telerik:GridBoundColumn DataField="DeliveryOrderNumber" HeaderText="Order" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="DeliveryOrderNumber" />
                                <telerik:GridBoundColumn DataField="OrderSubconRate" HeaderText="Order Sub Rate" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="OrderSubconRate" DataFormatString="{0:N2}" />
                                <telerik:GridBoundColumn DataField="Comments" HeaderText="Comments" SortExpression="Comments" />
                                <telerik:GridTemplateColumn HeaderText="Job" SortExpression="JobId">
                                    <ItemTemplate>
                                        <a id="hypJobId" runat="server"></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        <legend>Search for orders to add</legend>
        <table cellpadding="3px">
            <tr>
                <td colspan="2">
                    <table border="0">
                        <tr>
                            <td align="left" style="width: 320px;">
                                <span>Display orders planned to start after:
                                    <telerik:RadDatePicker ID="dteStartDateTime" runat="server" Width="100px">
                                        <DateInput runat="server"
                                         DateFormat="dd/MM/yy"
                                        DisplayDateFormat="dd/MM/yy">
                                        </DateInput>
                                    </telerik:RadDatePicker>
                                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ErrorMessage="Please enter a valid start date"
                                        ControlToValidate="dteStartDateTime" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid start date" /></asp:RequiredFieldValidator></span>
                                &nbsp;&nbsp; <span>Include planned work
                                    <asp:CheckBox ID="chkIncludePlannedWork" runat="server" Text="" text-align="Right"
                                        Checked="true" /></span>
                            </td>
                            <td style="vertical-align: top; padding-left: 5px; padding-top: 5px;" valign="top"
                                rowspan="2">
                                <b>Additional Orders:</b>&nbsp;To add orders to the manifest, select the resource
                                or subcontractor above (if not previously selected), fill in the search criteria
                                in this section and then click the &quot;Search for orders&quot; button to retrieve
                                a list of additional orders. Next, simply select the orders you wish to add from
                                the list presented and click the &quot;Save &amp; Display Manifest&quot; button
                                further below.
                            </td>
                        </tr>
                    </table>
                    <div style="height: 10px;">
                    </div>
                    <div class="buttonBar" style="height: 30px; padding: 4px; background-color: #99BEDE;">
                        <asp:Button ID="btnGetOrders" runat="server" Text="Search for orders" />
                    </div>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="lblOrderCount" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    <telerik:RadGrid runat="server" ID="grdManifestAddOrders" Visible="false" AllowFilteringByColumn="false"
                        Skin="Orchestrator" AllowPaging="false" AllowSorting="true" AllowMultiRowSelection="true"
                        EnableAJAX="true" AutoGenerateColumns="false" ShowStatusBar="false" PageSize="25">
                        <ClientSettings AllowDragToGroup="true" AllowColumnsReorder="true">
                            <Selecting AllowRowSelect="True" EnableDragToSelectRows="true"></Selecting>
                        </ClientSettings>
                        <PagerStyle NextPageText="Next &amp;gt;" PrevPageText="&amp;lt; Prev" Mode="NextPrevAndNumeric">
                        </PagerStyle>
                        <MasterTableView DataKeyNames="OrderId">
                            <RowIndicatorColumn Display="false">
                            </RowIndicatorColumn>
                            <Columns>
                                <telerik:GridTemplateColumn>
                                    <HeaderTemplate>
                                        <input type="checkbox" checked="checked" onclick="javascript:selectAllCheckboxes(this);" id="chkCheckAll" />
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkOrderID" runat="server"></asp:CheckBox>
                                        <asp:HiddenField ID="hidOrderId" runat="server"></asp:HiddenField>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Order&nbsp;Id" SortExpression="OrderID">
                                    <ItemTemplate>
                                        <a id="hypOrderId" runat="server">
                                            <%# ((System.Data.DataRowView)Container.DataItem)["OrderId"].ToString()%></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Customer" SortExpression="Customer">
                                    <ItemTemplate>
                                        <asp:Label ID="lblCustomer" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Start From" SortExpression="StartFrom">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartFrom" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Start At" SortExpression="StartAt">
                                    <ItemTemplate>
                                        <asp:Label ID="lblStartAt" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Destination" HeaderText="Destination" SortExpression="Destination" />
                                <telerik:GridBoundColumn DataField="ArriveAtDisplay" HeaderText="Arrive At" SortExpression="ArriveAt" />
                                <telerik:GridBoundColumn DataField="Items" HeaderText="Items" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="Items" />
                                <telerik:GridBoundColumn DataField="Weight" HeaderText="Weight" ItemStyle-HorizontalAlign="Right"
                                    DataFormatString="{0:F0}" SortExpression="Weight" />
                                <telerik:GridBoundColumn DataField="Reference" HeaderText="Reference" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="Reference" />
                                <telerik:GridBoundColumn DataField="DeliveryOrderNumber" HeaderText="Order" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="DeliveryOrderNumber" />
                                <telerik:GridBoundColumn DataField="OrderSubconRate" HeaderText="Order Sub Rate" ItemStyle-HorizontalAlign="Right"
                                    SortExpression="OrderSubconRate" DataFormatString="{0:N2}" />
                                <telerik:GridBoundColumn DataField="Comments" HeaderText="Comments" SortExpression="Comments" />
                                <telerik:GridBoundColumn DataField="DriverName" HeaderText="Driver" SortExpression="DriverName" />
                                <telerik:GridBoundColumn DataField="RegNo" HeaderText="Vehicle" SortExpression="RegNo" />
                                <telerik:GridBoundColumn DataField="TrailerRef" HeaderText="Trailer" SortExpression="TrailerRef" />
                                <telerik:GridTemplateColumn HeaderText="Job" SortExpression="JobId">
                                    <ItemTemplate>
                                        <a id="hypJobId" runat="server"></a>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <fieldset>
        <legend>View the Manifest &amp; PILS Report</legend>
        <table width="100%" border="0" cellpadding="3px" cellspacing="0">
            <tr>
                <td>
                    <asp:Panel runat="server" ID="pnlSaveAndDisplay" Visible="false">
                        Add additional blank rows:&nbsp;
                        <telerik:RadNumericTextBox runat="server" ID="txtExtraRowCount" MinValue="0" MaxValue="20" Value="2" Width="40px">
                        </telerik:RadNumericTextBox>&nbsp;<asp:CheckBox ID="chkIncludeFullAddresses" runat="server"
                            Text="Show Full Addresses" text-align="Right" Checked="true" />
                        <asp:CheckBox ID="chkShowCollectionDetails" runat="server" Text="Show Collection Details"
                            text-align="right" Checked="true" />
                        <div class="buttonBar" style="height: 30px; margin-top: 20px; padding: 4px; color: #ffffff;
                            background-color: #99BEDE;">
                            <nfvc:NoFormValButton ID="btnDisplayManifest" runat="server" Text="Save &amp; Display Manifest"
                                CausesValidation="False" />&nbsp;&nbsp;<asp:Button ID="btnProducePILs" runat="server"
                                    Text="Produce PILs" CausesValidation="False" />
                        </div>
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </fieldset>
    
    <uc1:ReportViewer ID="reportViewer" runat="server" Visible="False"></uc1:ReportViewer>

    <telerik:RadWindowManager ID="rmwAdmin" runat="server" Modal="true" ShowContentDuringLoad="true" KeepInScreenBounds="true" VisibleStatusbar="false"></telerik:RadWindowManager> 
    
    <script language="javascript" type="text/javascript">
    <!--
        
        function ViewJobDetails(jobID)
        {
            var url = '../Job/Job.aspx?wiz=true&jobId=' + jobID + getCSID();

            openResizableDialogWithScrollbars(url,'1220','870');
        }
    
        function cboResource_ClientSelectedIndexChanged(source, args)
        {
            var dteOrderManifestDate = $find("<%=dteOrderManifestDate.ClientID%>");
            
            var cboSubContractor = $find("<%=cboSubContractor.ClientID%>");
            cboSubContractor.clearSelection();
            
            var txtOrderManifestName = $find("<%=txtOrderManifestName.ClientID %>");
            txtOrderManifestName.set_value(source.get_text() + ' - ' + dteOrderManifestDate.get_dateInput().get_displayValue());
        }
    
        function cboSubContractor_ClientSelectedIndexChanged(source, args)
        {
            var dteOrderManifestDate = $find("<%=dteOrderManifestDate.ClientID%>");
        
            var cboResource = $find("<%=cboResource.ClientID%>");
            cboResource.clearSelection();
            
            var txtOrderManifestName = $find("<%=txtOrderManifestName.ClientID %>");
            txtOrderManifestName.set_value(source.get_text() + ' - ' + dteOrderManifestDate.get_dateInput().get_displayValue());
        }
    
        function ChangeList(e,src)
        {
            var gridRow;

            if(e.target)
            {
                gridRow = e.target.parentNode.parentNode;
            }
            else
            {
                gridRow = e.srcElement.parentNode.parentNode;
            }

            if (src.checked)
            {
                gridRow.className= "SelectedRow_Office2007";
            }
            else
            {
                gridRow.className= "GridRow_Office2007";
            }
        }
    
        function selectAllCheckboxes(chk)
        {
            if (chk.checked != true)
            {
                $('table[id*=grdManifestAddOrders] input[id*=chkOrderID]').prop('checked',false);
            }else{
                $('table[id*=grdManifestAddOrders] input[id*=chkOrderID]').prop('checked',true);
            }
        }
        
        function viewOrderProfile(orderID)
        {
            var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;
            
            var wnd = window.radopen("about:blank", "largeWindow");                               
            wnd.SetUrl(url);
            wnd.SetTitle("Add/Update Order");
        }
        
        function viewJobDetails(jobID)
        {
            var url = '../Job/Job.aspx?wiz=true&jobId=' + jobID + getCSID();

    		openResizableDialogWithScrollbars(url,'600','400');
        }       

    //-->
    </script>

</asp:Content>