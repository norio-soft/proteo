<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.Invoicing.SubContractorSB.autorunstart" Title="Sub-contractor Self Bill Auto Batch Run" Codebehind="autorunstart.aspx.cs" %>

<%@ Register TagPrefix="p1" TagName="BusinessTypeCheckList" Src="~/UserControls/BusinessTypeCheckList.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Sub-Contractor Items Ready To Invoice</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <telerik:RadWindowManager ID="rwmSubbyAutoRunStart" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
        <Windows>
            <telerik:RadWindow runat="server" ID="largeWindow" Height="650" Width="970" />
        </Windows>
    </telerik:RadWindowManager>
    
    <asp:Panel id="pnlDefaults" runat="server" DefaultButton="btnRefresh">
        <fieldset>
        <legend>Sub-Contractor Items Ready To Invoice</legend> 
            <table>
                <tr style="vertical-align: top;">
                     <td>
                        <table>
                            <tr>
                                <td class="formCellLabel">Date From</td>
                                <td class="formCellField" style="margin-right: 8px;">
                                    <telerik:RadDatePicker ID="rdiStartDate" runat="server" Width="100" ToolTip="The start date for the filter" >
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Date To</td>
                                <td class="formCellField">
                                    <telerik:RadDatePicker ID="rdiEndDate" runat="server" Width="100" ToolTip="The end date for the filter" >
                                    <DateInput runat="server"
                                    DateFormat="dd/MM/yy">
                                    </DateInput>
                                    </telerik:RadDatePicker>
                                </td>
                                <td class="formCellField"></td>
                            </tr>
                            <tr>
                                <td class="formCellLabel">Sub-Contractor</td>
                                <td class="formCellField"><telerik:RadComboBox ID="cboSubContractor" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                    MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                                    ShowMoreResultsBox="false" Skin="WindowsXP" Width="355px" Height="300px" DataTextField="OrganisationName" DataValueField="IdentityID"></telerik:RadComboBox></td>
                                <td class="formCellField"></td>
                            </tr>
                       </table>
                    </td>

                    <td>
                        <table>
                            <tr>
                                <td class="formCellLabel" rowspan="2">Business Type</td>
                                <td class="formCellField">
                                    <p1:BusinessTypeCheckList ID="chkBusinessType" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    When filtering subcontracted runs or legs by business type, the returned data will only include runs/legs where the business type on every order they include is in the set of business types you select.
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </fieldset>
    </asp:Panel>
    
    <div class="buttonbar" style="margin-bottom:5px;">
        <asp:Button ID="btnRefresh" runat="server" Text="Refresh" CausesValidation="false" />
    </div>

    <asp:Label ID="lblError" runat="server" Font-Bold="true" ForeColor="Red"></asp:Label>

    <div class="buttonbar">
        <asp:Button ID="Button1" runat="server" Text="Create Batch" />
        <asp:Button ID="btnEditRates" runat="server" Text="Edit Rates" CausesValidation="false" />
        <asp:Button ID="btnAddInvoiceReferences" runat="server" Text="Add Invoice References" CausesValidation="false" />
    </div>
    
    <div >
        <table>
            <tr>
                <td>Invoice Date</td>
                <td nowrap="nowrap">
                    <telerik:RadDatePicker ID="rdiInvoiceDate" runat="server" Width="100" ToolTip="The invoice date." >
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
                </td>
                <td><asp:RequiredFieldValidator ID="rfvInvoiceDate" runat="server" ControlToValidate="rdiInvoiceDate" Display="dynamic"><img src="/images/ico_critical_small.gif" height="16" width="16" title="Please supply an invoice date." alt="" /></asp:RequiredFieldValidator></td>
                <asp:CustomValidator ID="cvInvoiceDate" runat="server" Enabled="true" ControlToValidate="rdiInvoiceDate"
	                Display="Dynamic" EnableClientScript="true" ClientValidationFunction="ValidateInvoiceDate"
	                ErrorMessage="This Date may be Wrong">
	            </asp:CustomValidator>
            </tr>
        </table>
    </div>

    <telerik:RadGrid runat="server" ID="grdSubbies" AllowMultiRowEdit="true"
        AllowSorting="True" Skin="Office2007" AutoGenerateColumns="False" AllowMultiRowSelection="True" GridLines="None" >
        <MasterTableView ClientDataKeyNames="JobSubContractID" Width="100%" DataKeyNames="JobSubContractID" EditMode="InPlace" >
            <RowIndicatorColumn Display="false">
                <HeaderStyle Width="20px"></HeaderStyle>
            </RowIndicatorColumn>
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="ClientSelect" HeaderStyle-Width="30px" />
                <telerik:GridBoundColumn HeaderText="Date" SortExpression="Date" DataField="DeliveryDateTime" DataFormatString="{0:dd/MM/yy}" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                <telerik:GridBoundColumn HeaderText="Subcontractor" DataField="SubContractorName" SortExpression="SubContractorName" ReadOnly="true"   />
                <telerik:GridBoundColumn HeaderText="Load Number" SortExpression="CustomerOrderNumbers" DataField="CustomerOrderNumbers" UniqueName="CustomerOrderNumbers" ItemStyle-VerticalAlign="Top" ReadOnly="true" />

                <telerik:GridTemplateColumn HeaderText="Run ID" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top" >
                    <ItemTemplate>
                        <asp:PlaceHolder ID="plcRunID" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                
                <telerik:GridBoundColumn HeaderText="Docket Number" SortExpression="DeliveryOrderNumbers" DataField="DeliveryOrderNumbers" UniqueName="DeliveryOrderNumbers" ItemStyle-VerticalAlign="Top" ReadOnly="true" />
                <telerik:GridBoundColumn HeaderText="Refs" SortExpression="OtherReferences" DataField="OtherReferences" UniqueName="OtherReferences" ItemStyle-VerticalAlign="Top" ReadOnly="true" />
                <telerik:GridTemplateColumn HeaderText="From" HeaderStyle-Width="210px" SortExpression="From" ItemStyle-VerticalAlign="Top">
                    <ItemTemplate>
                        <span id="spnFrom" runat="server" onclick=""></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="To" HeaderStyle-Width="210px" SortExpression="From" ItemStyle-VerticalAlign="Top">
                    <ItemTemplate>
                        <span id="spnTo" runat="server" onclick=""></span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn HeaderText="Pallets" HeaderStyle-Width="35px" SortExpression="Pallets" DataField="PalletCount" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                <telerik:GridBoundColumn HeaderText="Weight" HeaderStyle-Width="35px" SortExpression="Weight" DataField="Weight" DataFormatString="{0:F0}" ReadOnly="true" ItemStyle-VerticalAlign="Top" />
                <telerik:GridTemplateColumn Display="false" UniqueName="InvoiceNo" HeaderText="Invoice No" HeaderStyle-Width="90px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top">
                    <ItemTemplate>
                        <asp:TextBox ID="txtInvoiceNo" runat="server" Width="80px"></asp:TextBox>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Run/Order ID" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top" >
                    <ItemTemplate>
                        <asp:PlaceHolder ID="plcRunOrOrderID" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Rate" HeaderText="Rate" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="80px" ItemStyle-Width="50px" ItemStyle-VerticalAlign="Top">
                    <ItemTemplate>
                        <asp:Label id="lblSubContractRate" runat="server"></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <input ID="txtSubContractRate" style="width:75px" type="text" runat="server" />
                    </EditItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
            <NoRecordsTemplate>
                <div>There are no records to display</div>
            </NoRecordsTemplate>
        </MasterTableView>
        <ClientSettings Selecting-AllowRowSelect="true" AllowColumnsReorder="true" ReorderColumnsOnClient="true">
            <Resizing AllowColumnResize="true" AllowRowResize="false" />
        </ClientSettings>

    </telerik:RadGrid>
    
    <telerik:RadAjaxLoadingPanel Visible="true" ID="LoadingPanel1" IsSticky="false" runat="server">
        <img alt="Loading" src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' />
    </telerik:RadAjaxLoadingPanel>

    <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server" EnableHistory="True">
        <AjaxSettings >
            <telerik:AjaxSetting AjaxControlID="grdSubbies">
             <UpdatedControls>
               <telerik:AjaxUpdatedControl ControlID="grdSubbies" LoadingPanelID="LoadingPanel1" />
             </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnEditRates">
             <UpdatedControls>
               <telerik:AjaxUpdatedControl ControlID="grdSubbies" LoadingPanelID="LoadingPanel1" />
               <telerik:AjaxUpdatedControl ControlID="btnEditRates"/>
               <telerik:AjaxUpdatedControl ControlID="btnEditRatesBottom"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferences"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferencesBottom"/>
             </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnEditRatesBottom">
             <UpdatedControls>
               <telerik:AjaxUpdatedControl ControlID="grdSubbies" LoadingPanelID="LoadingPanel1" />
               <telerik:AjaxUpdatedControl ControlID="btnEditRates"/>
               <telerik:AjaxUpdatedControl ControlID="btnEditRatesBottom"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferences"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferencesBottom"/>
             </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnRefresh">
             <UpdatedControls>
               <telerik:AjaxUpdatedControl ControlID="grdSubbies" LoadingPanelID="LoadingPanel1" />
               <telerik:AjaxUpdatedControl ControlID="btnEditRates"/>
               <telerik:AjaxUpdatedControl ControlID="btnEditRatesBottom"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferences"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferencesBottom"/>
             </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAddInvoiceReferencesBottom">
             <UpdatedControls>
               <telerik:AjaxUpdatedControl ControlID="grdSubbies" LoadingPanelID="LoadingPanel1" />
               <telerik:AjaxUpdatedControl ControlID="btnEditRates"/>
               <telerik:AjaxUpdatedControl ControlID="btnEditRatesBottom"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferences"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferencesBottom"/>
             </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="btnAddInvoiceReferences">
             <UpdatedControls>
               <telerik:AjaxUpdatedControl ControlID="grdSubbies" LoadingPanelID="LoadingPanel1" />
               <telerik:AjaxUpdatedControl ControlID="btnEditRates"/>
               <telerik:AjaxUpdatedControl ControlID="btnEditRatesBottom"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferences"/>
               <telerik:AjaxUpdatedControl ControlID="btnAddInvoiceReferencesBottom"/>
             </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>
    
    <div class="buttonbar">
        <asp:Button ID="btnCreateBatch" runat="server" Text="Create Batch" />
        <asp:Button ID="btnEditRatesBottom" runat="server" Text="Edit Rates" CausesValidation="false" />
        <asp:Button ID="btnAddInvoiceReferencesBottom" runat="server" Text="Add Invoice References" CausesValidation="false" />
    </div>
    
    <telerik:RadCodeBlock runat="server">
        <script language="javascript" type="text/javascript">
        
            function viewJobDetails(jobID)
            {
                var url = "../../job/job.aspx?jobId=" + jobID + getCSID();

                window.open(url);
            }
            
            function viewOrderProfile(orderID)
            {
                var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;

                window.open(url);
            }

            function ValidateInvoiceDate(source, args) {

                var dteDateTime = $find("<%= rdiInvoiceDate.ClientID %>");
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
      
        </script>
    </telerik:RadCodeBlock>
    
</asp:Content>