<%@ Control Language="C#" AutoEventWireup="True" Inherits="Orchestrator.UserControls.Order" CodeBehind="order.ascx.cs" %>

<%@ Register TagPrefix="cc1" Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<%@ Register TagPrefix="uc" TagName="Infringements" Src="~/UserControls/businessruleinfringementdisplay.ascx" %>

<script src="/script/toolTipPopUps.js" type="text/javascript"></script>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
    <Scripts>
        <asp:ScriptReference Path="~/script/handlebars-1.0.rc.2.js" />
        <asp:ScriptReference Path="~/script/templates.js" />
    </Scripts>
    <Services>
        <asp:ServiceReference Path="~/Tariff/Tariffs.asmx" />
        <asp:ServiceReference Path="~/Services/Allocation.svc" />
    </Services>
</asp:ScriptManagerProxy>

<style type="text/css">
    .form-value {
        font-size: 11px;
    }

    .tabviewPageViews {
        height: 500px;
        overflow-y: scroll;
    }

    .duplicateOrders {
        position: absolute;
        float: left;
        z-index: 10;
        width: 400px;
        left: 33%;
        top: 33%;
        height: 175px;
        text-align: center;
        font: 12px Verdana, Arial, Helvetica, sans-serif;
        background: url(../images/white.png);
        padding: 12px 10px 12px 2px;
    }

    .masterpagelite_layoutContainer {
        min-width: 1200px;
    }

    .masterpagelite_contentHolder {
        min-height: 400px;
    }
</style>
<cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550"
    Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true">
</cc1:Dialog>
<cc1:Dialog ID="dlgOrderGroup" URL="OrderGroupProfile.aspx" Height="650" Width="1200"
    AutoPostBack="true" runat="server" Mode="Normal" ReturnValueExpected="true">
</cc1:Dialog>

<cc1:Dialog ID="dlgAddUpdateRefusal" URL="/GoodsRefused/AddUpdateGoodsRefused.aspx" Height="700" Width="900"
    AutoPostBack="true" runat="server" Mode="Modal" ReturnValueExpected="true">
</cc1:Dialog>

<cc1:Dialog ID="dlgOrder" URL="/Groupage/ManageOrder.aspx" Height="1400" Width="1200"
    AutoPostBack="true" runat="server" Mode="Modal" ReturnValueExpected="true">
</cc1:Dialog>

<cc1:Dialog ID="dlgExtraWindow" URL="addupdateextra.aspx" Height="270" Width="450"
    AutoPostBack="true" Mode="Modal" ReturnValueExpected="true" runat="server">
</cc1:Dialog>

<cc1:Dialog ID="dlgAllocationHistory" URL="/consortium/orderallocationhistory.aspx" Height="270" Width="700"
    Mode="Modal" runat="server">
</cc1:Dialog>

<asp:Label ID="lblInformation" runat="server" Visible="false"></asp:Label>
<asp:Label ID="lblJobInformation" runat="server"></asp:Label>
<asp:HiddenField ID="hidServiceLevelDays" runat="server" Value="" />
<uc:Infringements ID="ucInfringments" runat="server" />
<asp:ListView ID="lvServiceLevelDays" runat="server" ItemPlaceholderID="itemContainer">
    <LayoutTemplate>

        <script language="javascript" type="text/javascript">
<!--
    var serviceLevelDays = null;
    function populateServiceLevelDays()
    {
        serviceLevelDays = new Array(<asp:PlaceHolder id="itemContainer" runat="server" />);
    }
			
    populateServiceLevelDays();
    //-->
        </script>

    </LayoutTemplate>
    <ItemTemplate>
        [<%# ((int)((System.Data.DataRowView)Container.DataItem)["OrderServiceLevelID"]).ToString() %>,<%# ((System.Data.DataRowView)Container.DataItem)["NoOfDays"] == DBNull.Value ? "-1" : ((int)((System.Data.DataRowView)Container.DataItem)["NoOfDays"]).ToString() %>]
    </ItemTemplate>
    <ItemSeparatorTemplate>
        ,
    </ItemSeparatorTemplate>
    <EmptyDataTemplate>

        <script language="javascript" type="text/javascript">
		<!--
    var serviceLevelDays = new Array();
    //-->
        </script>

    </EmptyDataTemplate>
    <EmptyItemTemplate>

        <script language="javascript" type="text/javascript">
		<!--
    var serviceLevelDays = new Array();
    //-->
        </script>

    </EmptyItemTemplate>
</asp:ListView>
<!-- The .AddMonths(-1) is not a mistype, javascript handles months from 00 i.e. 00 = jan, 01 feb -->
<asp:ListView ID="lvInvalidDates" runat="server" ItemPlaceholderID="itemContainer">
    <LayoutTemplate>

        <script language="javascript" type="text/javascript">
		<!--
    var invalidDates = null;
			
    function populateDates()
    {
        invalidDates = new Array(<asp:PlaceHolder id="itemContainer" runat="server" />);
    }
			
    populateDates();
    //-->
        </script>

    </LayoutTemplate>
    <ItemTemplate>
        { Date : new Date(<%#((DateTime)(Container.DataItem)).ToString("yyyy")%>,<%#((DateTime)(Container.DataItem)).Month == 1 ? "00" : ((DateTime)(Container.DataItem)).AddMonths(-1).ToString("MM")%>,<%#((DateTime)(Container.DataItem)).ToString("dd")%>) }
    </ItemTemplate>
    <ItemSeparatorTemplate>
        ,
    </ItemSeparatorTemplate>
    <EmptyDataTemplate>

        <script language="javascript" type="text/javascript">
		<!--
    var invalidDates = new Array();
    //-->
        </script>

    </EmptyDataTemplate>
    <EmptyItemTemplate>

        <script language="javascript" type="text/javascript">
		<!--
    var invalidDates = new Array();
    //-->
        </script>

    </EmptyItemTemplate>
</asp:ListView>

<div class="buttonbar">
    <input type="button" id="btnCancelOrderTop" runat="server" width="115" value="Cancel Order"
        onclick="javascript:CancelOrder();" />

    <asp:Button ID="btnPILTop" runat="server" Text="PIL" Width="75px" />
    <asp:Button ID="btnSetAsInvoicedTop" runat="server" Text="Set As Invoiced" Visible="true" OnClientClick="if(!confirmSetAsInvoiced()) return false;" ValidationGroup="submit" />
    <asp:Button ID="btnPodLabelTop" Visible="true" runat="server" Text="POD Label" Width="125px" />
    <asp:Button ID="btnCreateDeliveryNoteTop" runat="server" Text="Delivery Note" />
    <asp:Button ID="btnAddGroupedOrderTop" runat="server" Text="Add Grouped Order" ValidationGroup="submit" OnClientClick="if(!orderSubmitCheck('btnAddGroupedOrder')) return false;" />
    <asp:Button ID="btnCopyTop" runat="server" Visible="false" Text="Create a copy" Style="width: 125px;" /><asp:TextBox ID="txtNumberofCopiesTop" runat="server" Width="10" Text="1" Visible="false"></asp:TextBox>
    <asp:Button ID="btnSubmitTop" runat="server" Text="Add Order" ValidationGroup="submit" OnClientClick="if(!orderSubmitCheck('btnSubmit')) return false;" />
    <asp:Button ID="btnAddAndResetTop" runat="server" Text="Add Order & Reset" Style="width: 125px;" ValidationGroup="submit" OnClientClick="if(!orderSubmitCheck('btnAddAndReset')) return false;" />
    <asp:Button ID="btnCreateRunTop" runat="server" Text="Create Run" Visible="false" ValidationGroup="submit" />
</div>

<div style="margin-top: 3px" id="divOrderContanier">

    <div id="divDuplicateOrders" class="duplicateOrders" runat="server" visible="false" orderoptions="Order">
        <table>
            <tr>
                <td>
                    <h1>The below orderIds have been flagged as duplicates. 
                    </h1>
                    <br />
                    <asp:Label ID="lblDuplicateContinue" runat="server" Text='Click "Continue" to add the new order anyway.'></asp:Label>
                    <br />
                    Click "Go Back" to make further changes.                    
                </td>
            </tr>
            <tr>
                <td>
                    <asp:PlaceHolder ID="duplicateOrderIds" runat="server"></asp:PlaceHolder>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button ID="btnGoBack" runat="server" Text="Go Back" Visible="true" OnClientClick="hideDuplicateWindow(); return false;" />
                    <asp:Button ID="btnContinue" runat="server" Text="Continue" Visible="true" ValidationGroup="submit" />
                </td>
            </tr>
        </table>
    </div>

    <table id="addOrder">
        <tr valign="top">
            <td class="neworderOuterCollumnLeft">
                <table cellspacing="0" cellpadding="0">
                    <tr runat="server" id="trOrderIdAndOrderStatus">
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td style="width: 33%;" class="formCellLabel-Horizontal">Order Id
                                    </td>
                                    <td class="formCellLabel-Horizontal">Order Status
                                    </td>
                                    <td class="formCellLabel-Horizontal">Has Problems?
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <asp:Label runat="server" ID="lblOrderId"></asp:Label>
                                    </td>
                                    <td style="width: 33%;" class="formCellField-Horizontal">
                                        <asp:Label runat="server" ID="lblOrderStatus"></asp:Label>
                                    </td>
                                    <td style="width: 33%;" class="formCellField-Horizontal">
                                        <asp:Label ID="lblHasProblems" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcCancellation" runat="server">
                        <tr>
                            <td>
                                <table width="100%">
                                    <tr>
                                        <td style="width: 50%;">
                                            <div class="formCellLabel-Horizontal">
                                                Cancelled By
                                            </div>
                                            <div class="formCellField-Horizontal">
                                                <asp:Label ID="lblCancelledBy" runat="server"></asp:Label>
                                            </div>
                                        </td>
                                        <td style="width: 50%;">
                                            <div class="formCellLabel-Horizontal">
                                                Cancelled At
                                            </div>
                                            <div class="formCellField-Horizontal">
                                                <asp:Label ID="lblCancelledAt" runat="server"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="formCellLabel-Horizontal">
                                    Cancellation Reason
                                </div>
                                <div class="formCellField-Horizontal">
                                    <asp:Label ID="lblCancellationReason" runat="server"></asp:Label>
                                </div>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">Client
                                    </td>
                                    <td class="formCellLabel-Horizontal" id="tdCreateJob">Create Run
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                                            AutoPostBack="true" MarkFirstMatch="true" Height="300px" Overlay="true" CausesValidation="False"
                                            ShowMoreResultsBox="false" Width="350px" AllowCustomText="True" OnClientSelectedIndexChanged="cboClient_OnClientSelectedIndexChanged"
                                            OnClientItemsRequesting="cboClient_itemsRequesting">
                                            <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetClients" />
                                        </telerik:RadComboBox>
                                        <asp:RequiredFieldValidator ID="rfvClient" ValidationGroup="submit" runat="server"
                                            ControlToValidate="cboClient" ErrorMessage="Please select the client this job is for."
                                            Display="dynamic"><img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"  
											Title="Please select the client this job is for." />
                                        </asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="cfvClient" runat="server" ValidationGroup="submit" ControlToValidate="cboClient"
                                            EnableClientScript="false" ErrorMessage="Please select the client this job is for."
                                            Display="Dynamic"><img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" 
											title="Please select the client this job is for." />
                                        </asp:CustomValidator>
                                        <asp:Label ID="lblMissingDocumentsAlert" ForeColor="Red" runat="server" Text="&nbsp;" />
                                    </td>
                                    <td class="formCellField-Horizontal" valign="top">
                                        <asp:CheckBox ID="chkCreateJob" runat="server" Checked="true" />
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <asp:PlaceHolder ID="plcBusinessType" runat="server">
                                        <td style="width: 50%; vertical-align: top;">
                                            <div class="formCellLabel-Horizontal">
                                                Business Type
                                            </div>
                                            <div class="formCellField-Horizontal">
                                                <asp:DropDownList ID="cboBusinessType" runat="server" DataValueField="BusinessTypeID" DataTextField="Description"></asp:DropDownList>
                                                <asp:CustomValidator ID="cfvBusinessType" runat="server" ValidationGroup="submit" ControlToValidate="cboBusinessType" ErrorMessage="Please select a business type for this order." Display="Dynamic" ClientValidationFunction="validateBusinessType">
												<img src="/App_Themes/Orchestrator/Img/MasterPage/icon-warning.png" alt="Please select a business type for this order." />
                                                </asp:CustomValidator>
                                            </div>
                                        </td>
                                    </asp:PlaceHolder>
                                    <td style="width: 50%; vertical-align: top;">
                                        <div class="formCellLabel-Horizontal">Goods Type</div>
                                        <div class="formCellField-Horizontal">
                                            <telerik:RadComboBox ID="cboGoodsType" runat="server" Skin="WindowsXP"></telerik:RadComboBox>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server" id="trWorkType" visible="false">
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">Work Type
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <asp:RadioButtonList ID="rblInvolvement" runat="server" DataTextField="Description" DataValueField="OrderInstructionID" RepeatDirection="Horizontal" RepeatColumns="4"></asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">
                                        <asp:Label ID="lblLoadNumber" Text="Load Number" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <asp:TextBox ID="txtLoadNumber" CssClass="fieldInputBox" runat="server" MaxLength="25"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Panel ID="pnlReferences" runat="server" Width="100%">
                                <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                    <asp:Repeater ID="repReferences" runat="server">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="formCellLabel-Horizontal">
                                                    <%# DataBinder.Eval(Container.DataItem, "Description") %>
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="formCellField-Horizontal">
                                                    <input type="hidden" id="hidOrganisationReferenceId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "OrganisationReferenceId") %>' />
                                                    <asp:PlaceHolder ID="plcHolder" runat="server">
                                                        <asp:TextBox ID="txtReferenceValue" CssClass="fieldInputBox" runat="server" MaxLength="25"></asp:TextBox>
                                                        <asp:RequiredFieldValidator
                                                            ID="rfvReferenceValue"
                                                            runat="server"
                                                            ControlToValidate="txtReferenceValue"
                                                            ValidationGroup="submit"
                                                            EnableClientScript="False"
                                                            Display="Dynamic"
                                                            Enabled='<%#  IsReferencesValidatorEnabled( Convert.ToInt16(DataBinder.Eval(Container.DataItem, "OrganisationReferenceId")) ) %>'
                                                            ErrorMessage='<%# "Please supply a " + DataBinder.Eval(Container.DataItem, "Description") %>'>
                                                            <img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="<%# "Please supply a " + DataBinder.Eval(Container.DataItem, "Description") %>"  />
                                                        </asp:RequiredFieldValidator>
                                                    </asp:PlaceHolder>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </table>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">
                                        <asp:Label ID="lblDocketNumber" Text="Delivery Order Number" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <asp:TextBox runat="server" CssClass="fieldInputBox" ID="txtDeliveryOrderNumber"
                                            MaxLength="25"></asp:TextBox>
                                        <span style="font-size: 11px">(End Customer Ref)</span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">Service
                                    </td>
                                    <td id="tdAllocationLabel" runat="server" class="formCellLabel-Horizontal">Allocated To
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <telerik:RadComboBox ID="cboService" runat="server" Skin="WindowsXP" OnClientSelectedIndexChanged="cboService_SelectedIndexChanged">
                                        </telerik:RadComboBox>
                                    </td>
                                    <td id="tdAllocationField" runat="server" class="formCellField-Horizontal">
                                        <table>
                                            <tr>
                                                <td style="width: 200px;">
                                                    <telerik:RadComboBox ID="cboAllocatedTo" runat="server" Skin="WindowsXP" Width="200" DropDownWidth="300" ShowMoreResultsBox="false" ItemRequestTimeout="500" EnableLoadOnDemand="true" OnClientSelectedIndexChanged="cboAllocatedTo_SelectedIndexChanged" OnClientTextChange="cboAllocatedTo_TextChange">
                                                        <WebServiceSettings Path="~/ws/combostreamers.asmx" Method="GetSubContractors" />
                                                    </telerik:RadComboBox>
                                                    <asp:Label ID="lblAllocatedTo" runat="server" Visible="false" Style="vertical-align: text-top; width: 200px;" />
                                                </td>
                                                <td>
                                                    <asp:Image ID="imgAllocationHistory" runat="server" SkinID="imgIconHistory" Style="cursor: pointer;" ToolTip="Allocation History" Visible="false" />
                                                </td>
                                            </tr>
                                        </table>
                                        <span id="spnAllocationError" class="error"></span>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table width="100%" cellspacing="0" class="neworderTable">
                                <tr>
                                    <td class="formCellLabel-Horizontal">Pallets
                                    </td>
                                    <td class="formCellLabel-Horizontal">&nbsp;
                                    </td>
                                    <td class="formCellLabel-Horizontal">Spaces
                                    </td>
                                    <td class="formCellLabel-Horizontal">&nbsp;
                                    </td>
                                    <td class="formCellLabel-Horizontal">Weight
                                    </td>
                                    <td class="formCellLabel-Horizontal">Cases
                                    </td>
                                    <td class="formCellLabel-Horizontal">Pallet Type
                                    </td>
                                </tr>
                                <tr class="formCellField-Horizontal">
                                    <td>
                                        <telerik:RadNumericTextBox ID="rntxtPallets" runat="server" Type="Number" MinValue="0" MaxValue="999" NumberFormat-DecimalDigits="0" Width="40px" ClientEvents-OnBlur="rntxtPallets_Blur" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ValidationGroup="submit" ID="rfvPallets" runat="server" Style="float: right" ControlToValidate="rntxtPallets" Display="Dynamic" ErrorMessage="Please enter a value for the number of Pallets.">
                                            <img id="Img3" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a value for the number of Pallets." alt="" />
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="rntxtPalletSpaces" runat="server" Type="Number" MinValue="0" MaxValue="999" NumberFormat-DecimalDigits="2" ClientEvents-OnValueChanged="ReRate" Width="45px" />
                                    </td>
                                    <td>
                                        <asp:RequiredFieldValidator ValidationGroup="submit" ID="rfvPalletSpaces" runat="server" ControlToValidate="rntxtPalletSpaces" Display="Dynamic" ErrorMessage="Please enter a value for the number of Pallet Spaces.">
                                            <img id="Img8" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a value for the number of Pallet Spaces." alt="" />
                                        </asp:RequiredFieldValidator>
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="rntxtWeight" runat="server" Type="Number" MinValue="0" MaxValue="99999" NumberFormat-DecimalDigits="0" ClientEvents-OnValueChanged="ReRate" Width="50px" />
                                    </td>
                                    <td>
                                        <telerik:RadNumericTextBox ID="rntxtCartons" runat="server" Type="Number" MinValue="0" MaxValue="99999" NumberFormat-DecimalDigits="0" Width="50px" />
                                    </td>
                                    <td>
                                        <div class="formCellField-Horizontal">
                                            <telerik:RadComboBox ID="cboPalletType" runat="server"></telerik:RadComboBox>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">
                                        <asp:Label ID="lblTrafficNotes" runat="server" Text="Traffic Notes"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <asp:TextBox ID="txtTrafficNotes" runat="server" CssClass="fieldInputBox" MaxLength="15"></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server" id="trInvoiceSeperatley" visible="true">
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">Must Show on Separate Invoice
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal" style="width: 50%;">
                                        <asp:RadioButtonList ID="rblInvoiceSeperatley" runat="server" RepeatDirection="Horizontal">
                                            <asp:ListItem Value="false" Text="No" Selected="True"></asp:ListItem>
                                            <asp:ListItem Value="true" Text="Yes"></asp:ListItem>
                                        </asp:RadioButtonList>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr runat="server" id="trScanLinks">
                        <td>
                            <table cellspacing="0" cellspacing="0" class="neworderTable" style="width: 200px; float: left;">
                                <asp:PlaceHolder ID="phBookingForm" runat="server">
                                    <tr>
                                        <td style="vertical-align: top;" class="formCellLabel-Horizontal">Booking Form Link
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField-Horizontal">
                                            <asp:HyperLink ID="hlBookingFormLink" runat="server" Text="View" Target="_blank" />
                                            <a id="aScanBookingForm" runat="server" href="javascript:OpenBookingFormWindow();">Scan</a>&#160;
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcPOD" runat="server">
                                    <tr>
                                        <td style="vertical-align: top;" class="formCellLabel-Horizontal">POD Link
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField-Horizontal">
                                            <asp:HyperLink ID="hlPODLink" runat="server" Target="_blank" Text="View " />
                                            <a id="aScanPOD" runat="server" href="">Scan</a>&#160;
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                                <asp:PlaceHolder ID="plcDriverCheckIn" runat="server">
                                    <tr>
                                        <td style="vertical-align: top;" class="formCellLabel-Horizontal">Driver Check-In
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField-Horizontal">
                                            <asp:HyperLink ID="hlDriverCheckIn" runat="server" Text="" Target="_blank" />
                                            <a id="aDriverCheckIn" runat="server" href="javascript:OpenDriverCheckIn();">View</a>&#160;
                                        </td>
                                    </tr>
                                </asp:PlaceHolder>
                            </table>
                            <table cellspacing="0" cellspacing="0" class="neworderTable" style="width: 200px; float: left;">
                                <asp:ListView ID="lvSignatures" runat="server" ItemType="Orchestrator.Repositories.DTOs.MWFSignature">
                                    <LayoutTemplate>
                                        <tr>
                                            <td style="vertical-align: top;" class="formCellLabel-Horizontal">Signatures
                                            </td>
                                        </tr>
                                        <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <a class="signatureLink" href="#" data-image-name="<%# Item.ImageName %>" data-signed-by="<%# Item.SignedBy %>" data-comment="<%# Item.Comment %>" data-instruction-complete-date-time="<%# Item.InstructionCompleteDateTime.HasValue ? Item.InstructionCompleteDateTime.Value.ToString("dd/MM/yy hh:mm") : string.Empty %>" data-latitude="<%# Item.Latitude %>" data-longitude="<%# Item.Longitude %>">
                                                    <%# Item.InstructionTypeDescription %>
                                                </a>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <EmptyDataTemplate>
                                    </EmptyDataTemplate>
                                </asp:ListView>
                            </table>
                            <div style="clear: both;"></div>
                        </td>
                    </tr>
                    <tr runat="server" id="trPhotos">
                        <td>
                            <table class="neworderTable" cellpadding="0" cellspacing="0" width="100%">
                                <tr>
                                    <td class="formCellLabel-Horizontal">
                                        <asp:Label ID="Label1" runat="server" Text="Photographs"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellField-Horizontal">
                                        <asp:HyperLink ID="lnkPhotographs" runat="server" Text="Photos belonging to this order" Target="_blank" ToolTip="Click to view the photos belonging to this order."></asp:HyperLink>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="phOrderGroup" runat="server">
                        <tr>
                            <td>
                                <table width="100%" cellspacing="0" cellspacing="0" class="neworderTable">
                                    <tr>
                                        <td class="formCellLabel-Horizontal">Order Group
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField-Horizontal">
                                            <asp:HyperLink ID="lnkOrderGroup" runat="server" ToolTip="Click to view the group this order belongs to."></asp:HyperLink>
                                            <asp:LinkButton ID="lnkCreateGroup" runat="server" ToolTip="Click to create a group containing this order.">Create a Group for this Order</asp:LinkButton>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="phOrderInfo" runat="server">
                        <tr>
                            <td style="vertical-align: top;">
                                <table width="100%" cellspacing="0" cellspacing="0" class="neworderTable">
                                    <tr>
                                        <td style="width: 50%; vertical-align: top;" class="formCellLabel-Horizontal">Created
                                        </td>
                                        <td class="formCellLabel-Horizontal">Last Updated
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellField-Horizontal">
                                            <asp:Label ID="lblCreated" runat="server"></asp:Label>
                                        </td>
                                        <td class="formCellField-Horizontal" style="width: 50%; vertical-align: top;">
                                            <asp:Label ID="lblLastUpdated" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                </table>
            </td>
            <td class="neworderOuterCollumnMid">
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Collect From
                            <asp:Label ID="lblUpdateCollection" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <asp:Panel ID="pnlCollectionPoint" runat="server">
                                <p1:Point runat="server" ID="ucCollectionPoint" ShowFullAddress="true" CanClearPoint="true"
                                    CanUpdatePoint="true" ShowPointOwner="true" Visible="true" IsDepotVisible="true" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Collect When
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" name="collection" runat="server" id="rdCollectionTimedBooking"
                                            onclick="collectionTimedBooking(this);" />Timed Booking
									    <input type="radio" name="collection" runat="server" id="rdCollectionBookingWindow"
                                            onclick="collectionBookingWindow(this);" />Booking Window
									    <input type="radio" name="collection" runat="server" id="rdCollectionIsAnytime" checked="true"
                                            onclick="collectionIsAnytime(this);" />Anytime
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr>
                                    <td class="formCellLabel" style="width: 80px;">Collect from:
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDatePicker Width="100" ID="dteCollectionFromDate" runat="server">
                                            <DateInput runat="server" ClientEvents-OnValueChanged="dteCollectionFromDate_SelectedDateChanged"
                                                DateFormat="dd/MM/yy"
                                                DisplayDateFormat="dd/MM/yy">
                                            </DateInput>

                                        </telerik:RadDatePicker>
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadTimePicker Width="65" ID="dteCollectionFromTime" runat="server">
                                            <DateInput runat="server"
                                                SelectedDate="01/01/01 08:00"
                                                DateFormat="HH:mm"
                                                DisplayDateFormat="HH:mm">
                                            </DateInput>
                                        </telerik:RadTimePicker>
                                    </td>
                                    <td class="formCellInput">
                                        <asp:RequiredFieldValidator ID="rfvCollectionFromDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionFromDate" Display="Dynamic" ErrorMessage="Please enter a collection from date.">
                                            <img id="Img1" alt="" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection from date." />
                                        </asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvCollectionFromTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionFromTime" Display="Dynamic" ErrorMessage="Please enter a collection from time.">
                                            <img id="Img14" alt="" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection from time." />
                                        </asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="CustomValidator1" runat="server" ControlToValidate="dteCollectionFromDate"
                                            Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateCollectionDate"
                                            ErrorMessage="The date cannot be before today." ValidationGroup="submit">
												<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/></asp:CustomValidator>
                                    </td>
                                </tr>
                                <tr runat="server" id="trCollectBy">
                                    <td class="formCellLabel">Collect by:
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadDatePicker Width="100" ID="dteCollectionByDate" runat="server">
                                            <DateInput runat="server" ClientEvents-OnValueChanged="dteCollectionByDate_SelectedDateChanged"
                                                DateFormat="dd/MM/yy"
                                                DisplayDateFormat="dd/MM/yy">
                                            </DateInput>
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td class="formCellInput">
                                        <telerik:RadTimePicker Width="65" ID="dteCollectionByTime" runat="server">
                                            <DateInput
                                                runat="server"
                                                DateFormat="HH:mm"
                                                DisplayDateFormat="HH:mm"
                                                SelectedDate="01/01/01 17:00">
                                            </DateInput>
                                        </telerik:RadTimePicker>
                                    </td>
                                    <td class="formCellInput">
                                        <asp:RequiredFieldValidator ID="rfvCollectionByDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionByDate" Display="Dynamic" ErrorMessage="Please enter a collection by date.">
                                            <img id="Img12" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection by date." />
                                        </asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvCollectionByTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteCollectionByTime" Display="Dynamic" ErrorMessage="Please enter a collection by time.">
                                            <img id="Img13" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a collection by time." />
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Collection Notes (printed on Resource Manifest)
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <asp:TextBox ID="txtCollectionNotes" runat="server" CssClass="fieldInputBox" Width="340"
                                TextMode="MultiLine" Columns="90" Rows="3" MaxLength="500"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Printable Notes (printed on delivery note)
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <asp:TextBox ID="txtNotes" runat="server" Width="340" CssClass="fieldInputBox" TextMode="MultiLine"
                                Columns="100" Rows="3"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:CustomValidator ID="cfvDelivery" runat="server" ControlToValidate="dteDeliveryFromDate"
                                ValidationGroup="submit" EnableClientScript="false" ErrorMessage="">
                                Invalid Collection / Delivery Dates:
							<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" runat="server"
                                id="imgCfvDeliveryWarning" title="" alt="warning icon" />
                            </asp:CustomValidator>
                            <br />
                            <asp:Label runat="server" ForeColor="Red" ID="lblCollectionDeliveryExceptions"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td class="neworderOuterCollumnRight">
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Deliver To
                            <asp:Label ID="lblUpdateDelivery" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <asp:Panel ID="pnlDeliveryPoint" runat="server">
                                <p1:Point runat="server" ID="ucDeliveryPoint" ShowFullAddress="true" CanClearPoint="true"
                                    CanUpdatePoint="true" ShowPointOwner="true" Visible="true" IsDepotVisible="true" />
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Deliver When
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <table>
                                <tr>
                                    <td>
                                        <input type="radio" name="delivery" runat="server" id="rdDeliveryTimedBooking" onclick="deliveryTimedBooking(this);" />Timed
									Booking
									<input type="radio" name="delivery" runat="server" checked="true" id="rdDeliveryBookingWindow"
                                        onclick="deliveryBookingWindow(this);" />Booking Window
									<input type="radio" name="delivery" runat="server" id="rdDeliveryIsAnytime" onclick="deliveryIsAnytime(this);" />Anytime
                                    </td>
                                </tr>
                            </table>
                            <table>
                                <tr runat="server" id="trDeliverFrom">
                                    <td class="formCellLabel">Deliver from:
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDatePicker Width="100" ID="dteDeliveryFromDate" runat="server">
                                            <DateInput runat="server" ClientEvents-OnValueChanged="dteDeliveryFromDate_SelectedDateChanged"
                                                DateFormat="dd/MM/yy"
                                                DisplayDateFormat="dd/MM/yy">
                                            </DateInput>
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadTimePicker Width="65" ID="dteDeliveryFromTime" runat="server" SelectedDate="01/01/01 08:00">
                                            <DateInput runat="server"
                                                DateFormat="HH:mm"
                                                DisplayDateFormat="HH:mm"
                                                SelectedDate="01/01/01 08:00">
                                            </DateInput>
                                        </telerik:RadTimePicker>
                                    </td>
                                    <td class="formCellField">
                                        <asp:RequiredFieldValidator ID="rfvDeliveryFromDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryFromDate" Display="Dynamic" ErrorMessage="Please enter a delivery from date.">
                                            <img id="Img2" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery from date." />
                                        </asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvDeliveryFromTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryFromTime" Display="Dynamic" ErrorMessage="Please enter a delivery from time.">
                                            <img id="Img11" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery from time." />
                                        </asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="formCellLabel" style="width: 80px;">Deliver by:
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadDatePicker Width="100" ID="dteDeliveryByDate" runat="server">
                                            <DateInput runat="server" DateFormat="dd/MM/yy" ClientEvents-OnValueChanged="dteDeliveryByDate_SelectedDateChanged"
                                                DisplayDateFormat="dd/MM/yy">
                                            </DateInput>
                                        </telerik:RadDatePicker>
                                    </td>
                                    <td class="formCellField">
                                        <telerik:RadTimePicker Width="65" ID="dteDeliveryByTime" runat="server" SelectedDate="01/01/01 17:00">
                                            <DateInput runat="server"
                                                DateFormat="HH:mm"
                                                DisplayDateFormat="HH:mm"
                                                SelectedDate="01/01/01 17:00">
                                            </DateInput>
                                        </telerik:RadTimePicker>
                                    </td>
                                    <td class="formCellField">
                                        <asp:RequiredFieldValidator ID="rfvDeliveryByDate" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryByDate" Display="Dynamic" ErrorMessage="Please enter a delivery by date.">
                                            <img id="Img10" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery by date." />
                                        </asp:RequiredFieldValidator>
                                        <asp:RequiredFieldValidator ID="rfvDeliveryByTime" ValidationGroup="submit" runat="server"
                                            ControlToValidate="dteDeliveryByTime" Display="Dynamic" ErrorMessage="Please enter a delivery by time.">
                                            <img id="Img9" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png"
                                                title="Please enter a delivery by time." />
                                        </asp:RequiredFieldValidator>
                                        <asp:CustomValidator ID="CustomValidator3" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                            ValidationGroup="submit" Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate"
                                            ErrorMessage="The date cannot be before today.">
												<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date cannot be before today" alt="warning icon"/></asp:CustomValidator>
                                        <asp:CustomValidator ID="CustomValidator4" runat="server" Enabled="true" ControlToValidate="dteDeliveryByDate"
                                            Display="Dynamic" EnableClientScript="true" ClientValidationFunction="CV_ClientValidateDeliveryDate2"
                                            ErrorMessage="The date you have entered is far in the future. Are you sure it is correct?"
                                            ValidationGroup="submit">
												<img src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="The date entered is far in the future." alt="warning icon"/></asp:CustomValidator>
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Delivery Notes (printed on Resource Manifest)
						<img id="imageCopyToPrintableNotes" alt="Copy to Printable Notes" src="/images/copyicon.png" onclick="CopyDeliveryNotesToPrintableNotes();" width="13" />
                            <img id="imageCopyToSpecialInstructions" style="display: none;" onclick="CopyToSpecialInstructions();" alt="Copy to PalletForce Instructions" src="/images/copyicon.png" width="20" />
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <asp:TextBox ID="txtDeliveryNotes" runat="server" Width="340" CssClass="fieldInputBox"
                                TextMode="MultiLine" Columns="90" Rows="3" MaxLength="500"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr>
                        <td class="formCellLabel-Horizontal">Internal / Confidential Comments
                        </td>
                    </tr>
                    <tr>
                        <td class="formCellField-Horizontal">
                            <asp:TextBox Height="40" ID="txtConfidentialComments" runat="server" Width="340"
                                CssClass="fieldInputBox" TextMode="MultiLine" Columns="100" Rows="3"></asp:TextBox><br />
                            <asp:LinkButton runat="server" ID="lnkUpdateConfidentialComments" Text="Update Comments"></asp:LinkButton>&nbsp;
						<asp:Label runat="server" ID="lblUpdatedConfidentialComments" ForeColor="Red" Visible="false"></asp:Label>
                        </td>
                    </tr>
                </table>
                <table style="width: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                    <tr style="vertical-align: top;">
                        <td style="width: 45%">
                            <div class="formCellLabel-Horizontal">
                                Requires Booking In
                            </div>
                            <div class="formCellField-Horizontal">
                                <asp:CheckBox ID="chkRequiresBookIn" runat="server" Checked="false" Text="" />
                            </div>
                        </td>
                        <td>
                            <div id="bookedInContainer" style="vertical-align: top; vertical-align: text-top;">
                                <div class="formCellLabel-Horizontal">
                                    Booked In
                                </div>
                                <div class="formCellField-Horizontal">
                                    <asp:CheckBox runat="server" ID="chkBookedIn" Checked="false" Text="" />
                                </div>
                                <div class="formCellLabel-Horizontal">
                                    Booked In By
                                </div>
                                <div class="formCellField-Horizontal">
                                    <asp:Label ID="lblBookedInBy" runat="server"></asp:Label>
                                </div>
                                <div class="formCellLabel-Horizontal">
                                    Booked In With
                                </div>
                                <div class="formCellField-Horizontal">
                                    <asp:TextBox ID="txtBookedInWith" runat="server"></asp:TextBox>
                                </div>
                                <div class="formCellLabel-Horizontal">
                                    Booked In References
                                </div>
                                <div class="formCellField-Horizontal">
                                    <asp:TextBox ID="txtBookedInReferences" runat="server"></asp:TextBox>
                                </div>
                                <div id="divDeviationReason">
                                    <div class="formCellLabel-Horizontal">
                                        Deviation Reason
                                    </div>
                                    <div class="formCellField-Horizontal">
                                        <asp:DropDownList ID="cboDeviationReason" runat="server"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div style="width: 1140px; margin: 5px 0;">
        <telerik:RadTabStrip ID="OrderTabs" runat="server" Skin="Vista" MultiPageID="RadMultiPage1"
            Width="1150px">
            <Tabs>
                <telerik:RadTab id="tabPalletForce" runat="server" Text="Palletforce Fields" PageViewID="tabviewPalletForce"></telerik:RadTab>
                <telerik:RadTab id="tabRevenue" runat="server" Text="Revenue" Selected="true" PageViewID="tabviewRevenue"></telerik:RadTab>
                <telerik:RadTab id="tabFinancial" runat="server" Text="Financial" PageViewID="tabviewFinancial"></telerik:RadTab>
                <telerik:RadTab id="tabLegsForOrder" runat="server" Text="Legs for order" PageViewID="tabviewLegsForOrder"></telerik:RadTab>
                <telerik:RadTab id="tabRedeliveriesRefusals" runat="server" Text="Refusals" PageViewID="tabviewRedeliveriesRefusals"></telerik:RadTab>
                <telerik:RadTab id="tabAttemptedCollection" runat="server" Text="Attempted Collection" PageViewID="tabviewAttemptedCollection"></telerik:RadTab>
                <telerik:RadTab id="tabMwfDeliveryDetails" runat="server" Text="Delivery Details" PageViewID="tabviewMwfDeliveryDetails"></telerik:RadTab>
            </Tabs>
        </telerik:RadTabStrip>
    </div>

    <telerik:RadMultiPage ID="RadMultiPage1" runat="server" SelectedIndex="0" ReorderTabsOnSelect="true" Width="1150px">
        <telerik:RadPageView ID="tabviewPalletForce" runat="server" CssClass="tabviewPageViews">
            <table width="1130px">
                <tr id="trPalletForceFields" runat="server" style="">
                    <td>
                        <table id="tblPalletForceFields" cellspacing="0" cellpadding="0">
                            <tr>
                                <td width="442px" valign="top">
                                    <table cellspacing="0" cellpadding="0" class="neworderTable">
                                        <tr>
                                            <td class="formCellLabel-Horizontal">Tracking Number
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <asp:TextBox ID="txtTrackingNumber" runat="server" CssClass="fieldInputBox" Width="435" ReadOnly="true" BackColor="LightGray"
                                                    MaxLength="36"></asp:TextBox><br />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellLabel-Horizontal">Special Instructions
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <asp:TextBox ID="txtPalletForceNotes1" runat="server" CssClass="fieldInputBox" Width="435"
                                                    MaxLength="36"></asp:TextBox><br />
                                                <asp:TextBox ID="txtPalletForceNotes2" runat="server" CssClass="fieldInputBox" Width="435"
                                                    MaxLength="36"></asp:TextBox><br />
                                                <asp:TextBox ID="txtPalletForceNotes3" runat="server" CssClass="fieldInputBox" Width="435"
                                                    MaxLength="36"></asp:TextBox><br />
                                                <asp:TextBox ID="txtPalletForceNotes4" runat="server" CssClass="fieldInputBox" Width="435"
                                                    MaxLength="36"></asp:TextBox>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top">
                                    <table width="99%" cellspacing="0" cellpadding="0" class="neworderTable">
                                        <tr>
                                            <td class="formCellLabel-Horizontal">Trunk Date
                                            </td>
                                            <td class="formCellLabel-Horizontal">Imported Date
                                            </td>
                                            <td class="formCellLabel-Horizontal">Exported Date
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <telerik:RadDateInput Width="65" ID="dteTrunkDate" runat="server" DateFormat="dd/MM/yy"
                                                    DisplayDateFormat="dd/MM/yy">
                                                </telerik:RadDateInput>
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <asp:Label runat="server" ID="lblImportedDate"></asp:Label>
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <% = (this.SavedOrder != null &&  this.SavedOrder.ExportedDate.HasValue) ? this.SavedOrder.ExportedDate.Value.ToString("dd/MM/yy HH:mm") : "" %>
                                                <asp:Label runat="server" ID="lblExportedDate" Visible="false"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                    <table width="99%" cellspacing="0" cellpadding="0" class="neworderTable">
                                        <tr>
                                            <td class="formCellLabel-Horizontal">Full
                                            </td>
                                            <td class="formCellLabel-Horizontal">Half
                                            </td>
                                            <td class="formCellLabel-Horizontal">Qtr
                                            </td>
                                            <td class="formCellLabel-Horizontal">Full Over
                                            </td>
                                            <td class="formCellLabel-Horizontal">Half Over
                                            </td>
                                            <td class="formCellLabel-Horizontal" style="padding-left: 20px">Service
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <telerik:RadNumericTextBox ID="rntPalletForceFullPallets" runat="server" Type="Number"
                                                    MinValue="0" MaxValue="99" NumberFormat-DecimalDigits="0" ClientEvents-OnValueChanged="ReRate"
                                                    Width="40px" />
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <telerik:RadNumericTextBox ID="rntPalletForceHalfPallets" runat="server" Type="Number"
                                                    MinValue="0" MaxValue="99" NumberFormat-DecimalDigits="0" ClientEvents-OnValueChanged="ReRate"
                                                    Width="40px" />
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <telerik:RadNumericTextBox ID="rntPalletForceQtrPallets" runat="server" Type="Number"
                                                    MinValue="0" MaxValue="99" NumberFormat-DecimalDigits="0" ClientEvents-OnValueChanged="ReRate"
                                                    Width="40px" />
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <telerik:RadNumericTextBox ID="rntPalletForceOverPallets" runat="server" Type="Number"
                                                    MinValue="0" MaxValue="99" NumberFormat-DecimalDigits="0" ClientEvents-OnValueChanged="ReRate"
                                                    Width="40px" />
                                            </td>
                                            <td class="formCellField-Horizontal" style="padding-left: 20px">
                                                <telerik:RadComboBox runat="server" ID="cboPalletForceService" Width="60px" />
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                                <td valign="top" style="width: 100%; height: 100%">
                                    <table width="99%" style="height: 100%" cellspacing="0" cellpadding="0" class="neworderTable">
                                        <tr>
                                            <td class="formCellLabel-Horizontal">Surcharges
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal" style="height: 100%">
                                                <div style="height: 78px">
                                                    <asp:CheckBoxList runat="server" RepeatLayout="Flow" ID="cblPalletForceExtraTypes"
                                                        RepeatDirection="Horizontal" />
                                                </div>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="3">
                                    <table width="99.9%" cellspacing="0" cellpadding="0" class="neworderTable">
                                        <tr>
                                            <td style="width: 33%; vertical-align: top;" class="formCellLabel-Horizontal">Requesting Depot
                                            </td>
                                            <td style="width: 33%; vertical-align: top;" class="formCellLabel-Horizontal">Collection Depot
                                            </td>
                                            <td style="width: 33%; vertical-align: top;" class="formCellLabel-Horizontal">Delivery Depot
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <asp:Label runat="server" ID="lblRequestingDepot" ClientIDMode="Static"></asp:Label>
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <asp:Label runat="server" ID="lblCollectionDepot" ClientIDMode="Static"></asp:Label>
                                            </td>
                                            <td class="formCellField-Horizontal">
                                                <asp:Label runat="server" ID="lblDeliveryDepot" ClientIDMode="Static"></asp:Label>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tabviewRevenue" runat="server" Selected="true" CssClass="tabviewPageViews">
            <table>
                <tr>
                    <td valign="top">
                        <div id="divRevenue">
                            <asp:PlaceHolder ID="plcOrderGroupDetail" runat="server">
                                <telerik:RadAjaxPanel ID="pnlOrderGroupInformation" runat="server">
                                    <table style="width: 600px;" cellspacing="0" cellpadding="0" class="neworderTable">
                                        <tr>
                                            <td class="formCellLabel-Horizontal">Order Group Details
                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="formCellField-Horizontal">
                                                <uc:Infringements runat="server" ID="ruleInfringements" Visible="false" EnableViewState="false" />
                                                <table style="font-size: 12px;" cellspacing="0">
                                                    <tr>
                                                        <td class="formCellLabel">Rate
                                                        </td>
                                                        <td class="formCellField-Horizontal">
                                                            <asp:Label ID="lblRate" runat="server"></asp:Label>
                                                            <asp:HyperLink ID="lnkEditRate" runat="server" Text="Edit Grouped Order Rates" CausesValidation="false"
                                                                Visible="false"></asp:HyperLink>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel">Grouped Planning
                                                        </td>
                                                        <td class="formCellField">
                                                            <asp:RadioButtonList ID="cboGroupedPlanning" runat="server" AutoPostBack="true" RepeatDirection="Horizontal"
                                                                RepeatColumns="2">
                                                                <asp:ListItem Value="true" Text="Yes"></asp:ListItem>
                                                                <asp:ListItem Value="false" Text="No"></asp:ListItem>
                                                            </asp:RadioButtonList>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel">Order Count
                                                        </td>
                                                        <td class="formCellField">
                                                            <asp:Label ID="lblOrderCount" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </telerik:RadAjaxPanel>
                            </asp:PlaceHolder>
                            <asp:PlaceHolder ID="plcRate" runat="server">
                                <table style="width: 600px;">
                                    <tr>
                                        <td class="formCellLabel-Horizontal" style="vertical-align: top; text-align: left; width: 175px;">Rate Tariff Card
                                        </td>
                                        <td class="formCellLabel-Horizontal" style="vertical-align: top; text-align: left; width: 100px;">Rate
                                        </td>
                                        <td class="formCellLabel-Horizontal" style="vertical-align: top; text-align: left; width: 80px;">Fuel Surcharge
                                        </td>
                                        <td class="formCellLabel-Horizontal" style="vertical-align: top; text-align: left; width: 80px;">Fuel Surcharge 
                                        </td>
                                    </tr>
                                    <tr id="tr1" runat="server">
                                        <td class="formCellField-Horizontal" style="width: 175px; font-weight: bold; vertical-align: top;">
                                            <asp:TextBox ID="txtRateTariffCard" CssClass="fieldInputBox" runat="server"
                                                Enabled="true" BackColor="#fffedb" Font-Bold="true" Rows="2" TextMode="MultiLine" Columns="31"></asp:TextBox>
                                            <span id="spnRateError" class="error"></span>
                                        </td>
                                        <td class="formCellField-Horizontal" style="width: 100px; vertical-align: top;">
                                            <telerik:RadNumericTextBox ID="rntOrderRate" runat="server" ClientEvents-OnValueChanged="rntOrderRate_OnValueChanged" ClientEvents-OnKeyPress="rntOrderRate_OnKeyPress" NumberFormat-DecimalDigits="2" Type="Currency" Font-Bold="true" Culture="en-GB" Width="90px" />
                                            <asp:LinkButton runat="server" ID="lnkReInstateAutoRating" Style="display: none;" OnClientClick="lnkReinstateAutorating_OnClick(this); return false;" Text="Enable auto-rate"></asp:LinkButton>
                                            <asp:RequiredFieldValidator ID="rfvOrderRate" runat="server" ValidationGroup="submit" ControlToValidate="rntOrderRate" Display="Dynamic" ErrorMessage="Please enter a rate for this Order">
                                                <img id="Img7" runat="server" src="/App_Themes/Orchestrator/img/Masterpage/icon-warning.png" title="Please enter a Rate." alt="" />
                                            </asp:RequiredFieldValidator>
                                        </td>
                                        <td class="formCellField-Horizontal" style="width: 80px; font-weight: bold; vertical-align: top;">
                                            <asp:Label runat="server" ID="lblFuelSurcharge"></asp:Label>
                                        </td>
                                        <td class="formCellField-Horizontal" style="width: 80px; vertical-align: top;">
                                            <telerik:RadNumericTextBox runat="server" Width="75" Type="Percent" MinValue="-100" MaxValue="100" ID="rntFuelSurchargePercentage" ClientEvents-OnValueChanged="rntFuelSurchargePercentage_OnChange"></telerik:RadNumericTextBox>
                                            <asp:CheckBox runat="server" ID="chkOverrideFuelSurcharge" Text="Override" />
                                        </td>
                                    </tr>
                                </table>
                            </asp:PlaceHolder>
                            <table cellspacing="0" cellpadding="0">
                                <tr runat="server" id="trExtras">
                                    <td style="vertical-align: top;">
                                        <table style="width: 800px;" cellspacing="0" cellpadding="0" class="neworderTable">
                                            <tr>
                                                <td class="formCellLabel-Horizontal">
                                                    <asp:Label ID="lblNoExtras" runat="server" Text="There are no Extras" Visible="false"></asp:Label>
                                                    Extras
                                                </td>
                                            </tr>
                                            <tr>
                                                <td class="formCellField-Horizontal">
                                                    <telerik:RadGrid ID="grdExtras" runat="server" AutoGenerateColumns="False" Width="100%"
                                                        AllowMultiRowSelection="True" GridLines="None">
                                                        <ClientSettings Selecting-AllowRowSelect="false">
                                                            <ClientEvents OnRowSelected="CalculateTotal" OnRowDeselected="CalculateTotal" />
                                                        </ClientSettings>
                                                        <MasterTableView DataKeyNames="ExtraTypeId, ExtraId" ClientDataKeyNames="ExtraTypeId, ExtraId, FuelSurchargeApplies">
                                                            <Columns>
                                                                <telerik:GridTemplateColumn HeaderStyle-Width="20" HeaderText="">
                                                                    <HeaderTemplate>
                                                                        <input type="checkbox" onclick="javascript:HandleSelectAll(this);" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSelectExtra" runat="server" onclick="javascript:surchargeExtraChanged();" />
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn HeaderText="Type" ItemStyle-Width="180px">
                                                                    <ItemTemplate>
                                                                        <a runat="server" id="hypUpdateExtra">
                                                                            <asp:Label ID="lblExtraType" runat="server" Text='<%# Eval("ExtraType") %>'></asp:Label></a>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn HeaderText="Amount" UniqueName="ForeignAmount" ItemStyle-Width="100px">
                                                                    <ItemTemplate>
                                                                        <telerik:RadNumericTextBox runat="server" ID="rntExtraForeignAmount" Type="Currency" Width="60px" ClientEvents-OnValueChanged="rntExtraForeignAmount_OnValueChanged" />
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn HeaderText="Fuel Surcharge" UniqueName="FuelSurcharge"
                                                                    ItemStyle-Width="80px">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblExtraFuelSurchargeAmount" runat="server" />
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn HeaderText="State">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblExtraState" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraState").ToString()) %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn HeaderText="Contact">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblClientContact" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ClientContact") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridTemplateColumn Display="False" HeaderText="Custom Description">
                                                                    <ItemTemplate>
                                                                        <asp:Label Visible="true" ID="lblCustomDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomDescription") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                </telerik:GridTemplateColumn>
                                                                <telerik:GridBoundColumn DataField="ExtraId" UniqueName="ExtraId" Display="False">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="ExtraTypeId" UniqueName="ExtraTypeId" Display="False">
                                                                </telerik:GridBoundColumn>
                                                                <telerik:GridBoundColumn DataField="FuelSurchargeApplies" UniqueName="FuelSurchargeApplies"
                                                                    Visible="False">
                                                                </telerik:GridBoundColumn>
                                                            </Columns>
                                                        </MasterTableView>
                                                    </telerik:RadGrid>
                                                    <table>
                                                        <tr id="trOrderTotal" runat="server">
                                                            <td class="formCellField-Horizontal" style="width: 220px; font-weight: bold;">Totals
                                                            </td>
                                                            <td class="formCellField-Horizontal" style="width: 100px; font-weight: bold;">
                                                                <span runat="server" id="spnForeignTotal"></span>
                                                            </td>
                                                            <td class="formCellField-Horizontal" style="width: 80px; font-weight: bold;">
                                                                <span runat="server" id="spnFuelSurchargeTotal"></span>
                                                            </td>
                                                            <td class="formCellField-Horizontal" style="width: 100px; font-weight: bold;">
                                                                <span runat="server" id="spnGrandTotal"></span>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr runat="server" id="trExtras_AddButton">
                                    <td>
                                        <telerik:RadCodeBlock ID="RadCodeBlock3" runat="server">
                                            <div class="buttonbar" style="width: 800px;">
                                                <input type="button" runat="server" id="btnAddExtra" value="Add Extra" />
                                            </div>
                                        </telerik:RadCodeBlock>
                                    </td>
                                </tr>
                                <asp:Panel ID="pnlAttemptedDeliveries" runat="server">
                                    <tr>
                                        <td>
                                            <div class="formCellLabel-Horizontal">
                                                Attempted Delivery Details
                                            </div>
                                            <table width="400">
                                                <tr>
                                                    <td style="padding-top: 5px">
                                                        <telerik:RadGrid ID="grdAttemptedDeliveries" runat="server" Skin="Office2007" AutoGenerateColumns="false">
                                                            <MasterTableView>
                                                                <Columns>
                                                                    <telerik:GridHyperLinkColumn HeaderText="Job ID" DataNavigateUrlFields="JobID" DataNavigateUrlFormatString="javascript:ViewJobDetails({0});"
                                                                        DataTextField="JobID" />
                                                                    <telerik:GridBoundColumn HeaderText="Reason" DataField="Reason" />
                                                                    <telerik:GridBoundColumn HeaderText="Recorded By" DataField="CreateUserId" />
                                                                    <telerik:GridBoundColumn HeaderText="When" DataField="CreateDate" DataFormatString="{0:dd/MM/yy HH:mm}" />
                                                                </Columns>
                                                            </MasterTableView>
                                                        </telerik:RadGrid>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </asp:Panel>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tabviewFinancial" Visible="false" runat="server" CssClass="tabviewPageViews">
            <table>
                <tr runat="server" id="trFinancial">
                    <td style="vertical-align: top;">
                        <div id="divFinancial">
                            <table style="width: 700px;" cellspacing="0" cellpadding="0" class="neworderTable">
                                <tr>
                                    <td class="formCellLabel-Horizontal">Invoice ID
                                    </td>
                                    <td class="formCellLabel-Horizontal">Invoice Separately
                                    </td>
                                    <td class="formCellLabel-Horizontal">Ready To Invoice
                                    </td>
                                    <td class="formCellLabel-Horizontal">Being Invoiced
                                    </td>
                                    <td class="formCellLabel-Horizontal">Nominal Code
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div class="formCellField-Horizontal">
                                            <asp:HyperLink ID="lblInvoiceNumber" runat="server" Target="_blank" Text="View" />
                                        </div>
                                    </td>
                                    <td>
                                        <div class="formCellField-Horizontal">
                                            <asp:Label ID="lblInvoiceSeperatley" runat="server"></asp:Label>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="formCellField-Horizontal">
                                            <asp:Label ID="lblReadyToInvoice" runat="server"></asp:Label>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="formCellField-Horizontal">
                                            <asp:Label ID="lblBeingInvoiced" runat="server"></asp:Label>
                                        </div>
                                    </td>
                                    <td>
                                        <div class="formCellField-Horizontal">
                                            <asp:Label ID="lblNominalCode" runat="server"></asp:Label>
                                        </div>
                                    </td>
                                </tr>
                            </table>
                            <table cellspacing="0" cellpadding="0">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="0">
                                            <tr>
                                                <td>
                                                    <table style="width: 700px;" cellspacing="0" cellpadding="0" class="neworderTable">
                                                        <tr>
                                                            <td style="vertical-align: top;" class="formCellLabel-Horizontal">Subcontractor Details
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formCellField-Horizontal">
                                                                <telerik:RadGrid ID="RadGridForSubby" runat="server" AutoGenerateColumns="false">
                                                                    <MasterTableView>
                                                                        <Columns>
                                                                            <telerik:GridTemplateColumn HeaderText="Name">
                                                                                <ItemTemplate>
                                                                                    <%#((System.Data.DataRowView)Container.DataItem)["DeliveringSubContractor"].ToString()%>
                                                                                </ItemTemplate>
                                                                            </telerik:GridTemplateColumn>
                                                                            <telerik:GridTemplateColumn HeaderText="Rate" UniqueName="ForeignRate">
                                                                                <ItemTemplate>
                                                                                    <asp:Label ID="lblForeignRate" runat="server" />
                                                                                </ItemTemplate>
                                                                            </telerik:GridTemplateColumn>
                                                                            <telerik:GridBoundColumn HeaderText="Rate (£)" DataField="Rate" DataFormatString="{0:C}" />
                                                                            <telerik:GridTemplateColumn HeaderText="Invoice Number">
                                                                                <ItemTemplate>
                                                                                    <a id="lnkViewInvoice" runat="server" target="_blank">
                                                                                        <%#((System.Data.DataRowView)Container.DataItem)["IsInvoiced"].ToString() == "-1" ? "No" : ((System.Data.DataRowView)Container.DataItem)["IsInvoiced"].ToString() %>
                                                                                    </a>
                                                                                </ItemTemplate>
                                                                            </telerik:GridTemplateColumn>
                                                                            <telerik:GridTemplateColumn HeaderText="Pay Method">
                                                                                <ItemTemplate>
                                                                                    <%#((System.Data.DataRowView)Container.DataItem)["SubContractMethod"].ToString()%>
                                                                                </ItemTemplate>
                                                                            </telerik:GridTemplateColumn>
                                                                        </Columns>
                                                                    </MasterTableView>
                                                                </telerik:RadGrid>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                </td>
                                                <td style="vertical-align: top; width: 50%;">
                                                    <asp:Panel ID="pnlNetworkExtras" runat="server">
                                                        <div class="formCellLabel-Horizontal">
                                                            Network Extras
                                                        </div>
                                                        <div id="divNetworkExtras">
                                                            <telerik:RadGrid ID="grdNetworkExtras" runat="server" AutoGenerateColumns="false">
                                                                <MasterTableView>
                                                                    <Columns>
                                                                        <telerik:GridTemplateColumn HeaderText="Type">
                                                                            <ItemTemplate>
                                                                                <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
                                                                                    <a runat="server" id="hypUpdateExtra">
                                                                                        <asp:Label ID="lblExtraType" runat="server" Text='<%# Eval("ExtraType") %>'></asp:Label></a>
                                                                                </telerik:RadCodeBlock>
                                                                            </ItemTemplate>
                                                                        </telerik:GridTemplateColumn>
                                                                        <telerik:GridTemplateColumn Visible="False" HeaderText="Custom Description">
                                                                            <ItemTemplate>
                                                                                <asp:Label Visible="true" ID="lblCustomDescription" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "CustomDescription") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </telerik:GridTemplateColumn>
                                                                        <telerik:GridTemplateColumn HeaderText="State">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblExtraState" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraState").ToString()) %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </telerik:GridTemplateColumn>
                                                                        <telerik:GridTemplateColumn HeaderText="Contact">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblClientContact" runat="server" Text='<%# DataBinder.Eval(Container.DataItem, "ClientContact") %>'></asp:Label>
                                                                            </ItemTemplate>
                                                                        </telerik:GridTemplateColumn>
                                                                        <telerik:GridTemplateColumn HeaderText="Amount">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblExtraForeignAmount" runat="server" />
                                                                            </ItemTemplate>
                                                                        </telerik:GridTemplateColumn>
                                                                    </Columns>
                                                                    <NoRecordsTemplate>
                                                                        There are no Network extras
                                                                    </NoRecordsTemplate>
                                                                </MasterTableView>
                                                            </telerik:RadGrid>
                                                        </div>
                                                    </asp:Panel>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <div runat="server" id="divInvoiceButtons" class="buttonbar">
                                            <asp:Button ID="btnMakeInvoiceable" runat="server" Text="Flag Ready To Invoice" Width="150px" />
                                            <asp:Label ID="lblOnHoldReason" runat="server" Width="600px" Visible="false" />
                                            <asp:Button ID="btnUnFlagForInvoicing" runat="server" Text="Un-flag Ready To Invoice"
                                                Width="150px" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
            </table>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tabviewLegsForOrder" runat="server" CssClass="tabviewPageViews">
            <asp:PlaceHolder runat="server" ID="phLegsForOrder">
                <table>
                    <tr id="trLegsForOrder">
                        <td>
                            <div id="divLegsForOrder" style="width: 800px;">
                                <telerik:RadAjaxPanel ID="rapOrders" runat="server">
                                    <telerik:RadGrid Width="100%" runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true"
                                        EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true">
                                        <MasterTableView Width="100%" DataKeyNames="JobID, CanRemove" ItemStyle-Height="18">
                                            <RowIndicatorColumn Display="false">
                                            </RowIndicatorColumn>
                                            <Columns>
                                                <telerik:GridTemplateColumn HeaderText="Run No">
                                                    <ItemTemplate>
                                                        <a href="javascript:ViewJobDetails(<%#((System.Data.DataRowView)Container.DataItem)["JobID"].ToString()%>)">
                                                            <%#((System.Data.DataRowView)Container.DataItem)["JobID"].ToString()%></a>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="From" SortExpression="StartPoint">
                                                    <ItemTemplate>
                                                        <span id="spnCollectionPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["StartPointID"].ToString() %>);"
                                                            onmouseout="closeToolTip();" class="orchestratorLink">
                                                            <%#((System.Data.DataRowView)Container.DataItem)["StartPoint"].ToString()%></span>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Start" SortExpression="LegPlannedStartDateTime"
                                                    ItemStyle-Width="100" ItemStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["PlannedArrivalDateTime"], false)%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="To" SortExpression="EndPoint">
                                                    <ItemTemplate>
                                                        <span id="spnCollectionPoint" onclick="" onmouseover="ShowPointToolTip(this, <%#((System.Data.DataRowView)Container.DataItem)["EndPointID"].ToString() %>);"
                                                            onmouseout="closeToolTip();" class="orchestratorLink">
                                                            <%#((System.Data.DataRowView)Container.DataItem)["EndPoint"].ToString()%></span>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="At" SortExpression="LegPlannedEndDateTime"
                                                    ItemStyle-Width="100" ItemStyle-Wrap="false">
                                                    <ItemTemplate>
                                                        <%#GetDate((DateTime)((System.Data.DataRowView)Container.DataItem)["PlannedDepartureDateTime"], false)%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Action" ItemStyle-Wrap="false" ItemStyle-Width="80">
                                                    <ItemTemplate>
                                                        <%#DataBinder.Eval(Container.DataItem, "FinishOrderAction").ToString().ToLower() == "default" ? "Deliver" :  DataBinder.Eval(Container.DataItem, "FinishOrderAction")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridTemplateColumn HeaderText="Driver">
                                                    <ItemTemplate>
                                                        <%#DataBinder.Eval(Container.DataItem, "SubContractor") == DBNull.Value ? DataBinder.Eval(Container.DataItem, "FinishFullName") : DataBinder.Eval(Container.DataItem, "SubContractor")%>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                                <telerik:GridBoundColumn HeaderText="Trailer" DataField="FinishTrailerRef" ItemStyle-Wrap="false">
                                                </telerik:GridBoundColumn>
                                                <telerik:GridButtonColumn CommandName="Delete" Text="Remove" UniqueName="Remove" HeaderText="Remove" ButtonType="LinkButton" Visible="true" />
                                                <telerik:GridTemplateColumn UniqueName="callin">
                                                    <ItemTemplate>
                                                        <a runat="server" id="lnkCallIn">call in</a>
                                                    </ItemTemplate>
                                                </telerik:GridTemplateColumn>
                                            </Columns>
                                        </MasterTableView>
                                        <ClientSettings AllowColumnsReorder="true" ReorderColumnsOnClient="true">
                                            <Resizing AllowColumnResize="true" AllowRowResize="false" />
                                            <Selecting AllowRowSelect="true" />
                                        </ClientSettings>
                                    </telerik:RadGrid>
                                </telerik:RadAjaxPanel>
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:PlaceHolder>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tabviewRedeliveriesRefusals" Visible="false" runat="server" CssClass="tabviewPageViews">
            <asp:PlaceHolder runat="server" ID="PlaceHolder1">
                <table>
                    <tr id="tr1">
                        <td>
                            <div id="divRefusalsNewOrder" runat="server" style="width: 1100px;">
                                <h2>Refusals returned on this order</h2>
                                <telerik:RadGrid ID="grdRefusalsNewOrder" runat="server" AutoGenerateColumns="False" Width="100%"
                                    AllowMultiRowSelection="True" GridLines="None">
                                    <MasterTableView DataKeyNames="RefusalId, JobId" ClientDataKeyNames="RefusalId, JobId">
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderText="Goods Id" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a runat="server" id="hypGoodsId">
                                                        <asp:Label ID="lblGoodsId" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalId.ToString()%>'></asp:Label></a>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Job Id" UniqueName="JobId" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a runat="server" id="hypJobId">
                                                        <asp:Label ID="lblJobId" runat="server" Text='<%# ((RefusalRow)Container.DataItem).JobId.ToString()%>'></asp:Label></a>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Original Order Id" UniqueName="OrderId" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a runat="server" id="hypOrderId">
                                                        <asp:Label ID="lblOrderId" runat="server"></asp:Label></a>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Refusal Type" UniqueName="RefusalType" ItemStyle-Width="80px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRefusalType" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalType.ToString()%>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Product" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblProduct" runat="server" Text='<%# ((RefusalRow)Container.DataItem).ProductName.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Code" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCode" runat="server" Text='<%# ((RefusalRow)Container.DataItem).ProductCode.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Receipt No" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReceipt" runat="server" Text='<%# (((RefusalRow)Container.DataItem).RefusalReceiptNumber != null) ? ((RefusalRow)Container.DataItem).RefusalReceiptNumber.ToString() : String.Empty%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Pack Size" ItemStyle-Width="50px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPackSize" runat="server" Text='<%# ((RefusalRow)Container.DataItem).PackSize.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Qty Ordered">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQuantityOrdered" runat="server" Text='<%# ((RefusalRow)Container.DataItem).QuantityOrdered.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Qty Refused">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQuantityRefused" runat="server" Text='<%# ((RefusalRow)Container.DataItem).QuantityRefused.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Return By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTimeFrame" runat="server" Text='<%# ((RefusalRow)Container.DataItem).TimeFrame.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Current Location">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRefusalLocation" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalLocation.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Notes">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNotes" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalNotes.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>
                            <div style="height: 10px;"></div>
                            <div id="divRefusalsOriginalOrder" runat="server" style="width: 1100px;">
                                <h2>Goods refused on this order</h2>
                                <telerik:RadGrid ID="grdRefusalsOriginalOrder" runat="server" AutoGenerateColumns="False" Width="100%"
                                    AllowMultiRowSelection="True" GridLines="None">
                                    <MasterTableView DataKeyNames="RefusalId, JobId" ClientDataKeyNames="RefusalId, JobId">
                                        <Columns>
                                            <telerik:GridTemplateColumn HeaderText="Goods Id" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a runat="server" id="hypGoodsId">
                                                        <asp:Label ID="lblGoodsId" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalId.ToString()%>'></asp:Label></a>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Job Id" UniqueName="JobId" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a runat="server" id="hypJobId">
                                                        <asp:Label ID="lblJobId" runat="server" Text='<%# ((RefusalRow)Container.DataItem).JobId.ToString()%>'></asp:Label></a>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="New Order Id" UniqueName="OrderId" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <a runat="server" id="hypOrderId">
                                                        <asp:Label ID="lblOrderId" runat="server"></asp:Label></a>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Refusal Type" UniqueName="RefusalType" ItemStyle-Width="80px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRefusalType" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalType.ToString()%>' />
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Product" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblProduct" runat="server" Text='<%# ((RefusalRow)Container.DataItem).ProductName.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Code" ItemStyle-Width="60px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblCode" runat="server" Text='<%# ((RefusalRow)Container.DataItem).ProductCode.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Receipt No" ItemStyle-Width="100px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblReceipt" runat="server" Text='<%# (((RefusalRow)Container.DataItem).RefusalReceiptNumber != null) ? ((RefusalRow)Container.DataItem).RefusalReceiptNumber.ToString() : String.Empty%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Pack Size" ItemStyle-Width="50px">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPackSize" runat="server" Text='<%# ((RefusalRow)Container.DataItem).PackSize.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Qty Ordered">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQuantityOrdered" runat="server" Text='<%# ((RefusalRow)Container.DataItem).QuantityOrdered.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Qty Refused">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblQuantityRefused" runat="server" Text='<%# ((RefusalRow)Container.DataItem).QuantityRefused.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Return By">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblTimeFrame" runat="server" Text='<%# ((RefusalRow)Container.DataItem).TimeFrame.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Current Location">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblRefusalLocation" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalLocation.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                            <telerik:GridTemplateColumn HeaderText="Notes">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblNotes" runat="server" Text='<%# ((RefusalRow)Container.DataItem).RefusalNotes.ToString()%>'></asp:Label>
                                                </ItemTemplate>
                                            </telerik:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </telerik:RadGrid>
                            </div>
                        </td>
                    </tr>
                </table>
            </asp:PlaceHolder>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tabviewAttemptedCollection" Visible="false" runat="server" CssClass="tabviewPageViews">
            <asp:PlaceHolder runat="server" ID="phAttemptedColletion">
                <div>
                    <h2>Attempted Collections made for this order</h2>
                    <telerik:RadGrid ID="rgAttemptedCollection" runat="server" AutoGenerateColumns="false" Width="100%" AllowMultiRowSelection="false" GridLines="None">
                        <MasterTableView DataKeyNames="AttemptedCollectionId" ClientDataKeyNames="AttemptedCollectionId">
                            <Columns>
                                <telerik:GridBoundColumn HeaderText="Collection Date" DataField="CollectionDate" />
                                <telerik:GridBoundColumn HeaderText="Collection From" DataField="CollectionFrom" />
                                <telerik:GridBoundColumn HeaderText="Delivery To" DataField="DeliveryTo" />
                                <telerik:GridBoundColumn HeaderText="Reason" DataField="AttemptedCollectionDescription" />
                                <telerik:GridBoundColumn HeaderText="Client Reference" DataField="AttemptedCollectionReference" />
                                <telerik:GridBoundColumn HeaderText="Client Contact" DataField="ClientContact" />
                            </Columns>
                            <NoRecordsTemplate>
                                No Attempted Collection Recoreded.
                            </NoRecordsTemplate>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
            </asp:PlaceHolder>
        </telerik:RadPageView>
        <telerik:RadPageView ID="tabviewMwfDeliveryDetails" Visible="false" runat="server" CssClass="tabviewPageViews">
            <table class="neworderTable">
                <tr>
                    <td class="formCellLabel-Horizontal">
                        <label>Notes</label>
                    </td>
                    <td class="formCellLabel-Horizontal">
                        <label>Qty Del</label>
                    </td>
                    <td class="formCellLabel-Horizontal">
                        <label>Arrival</label>
                    </td>
                    <td class="formCellLabel-Horizontal">
                        <label>Departure</label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellField-Horizontal">
                        <asp:TextBox runat="server" ID="txtMwfDeliveryNotes" CssClass="fieldInputBox" Enabled="false" Width="300px" TextMode="MultiLine" Rows="3" />
                    </td>
                    <td class="formCellField-Horizontal">
                        <asp:TextBox runat="server" ID="txtMwfQuantityDelivered" CssClass="fieldInputBox" Enabled="false" Width="50px" />
                    </td>
                    <td class="formCellField-Horizontal">
                        <asp:TextBox runat="server" ID="txtMwfArrivalDateTime" CssClass="fieldInputBox" Enabled="false" Width="100px" />
                    </td>
                    <td class="formCellField-Horizontal">
                        <asp:TextBox runat="server" ID="txtMwfDepartureDateTime" CssClass="fieldInputBox" Enabled="false" Width="100px" />
                    </td>
                </tr>
            </table>
            <br>
            <telerik:RadGrid ID="rgMwfPallets" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="None" Width="650px">
                <MasterTableView DataKeyNames="OrderMWFPalletID">
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="Pallet" DataField="PalletNumber" HeaderStyle-Width="50px" />
                        <telerik:GridBoundColumn HeaderText="Notes" DataField="DeliveryStatusNote" HeaderStyle-Width="500px" />
                        <telerik:GridBoundColumn HeaderText="Status" DataField="DeliveryStatusName" HeaderStyle-Width="100px" />
                    </Columns>
                    <NoRecordsTemplate>
                        No pallets recorded.
                    </NoRecordsTemplate>
                </MasterTableView>
            </telerik:RadGrid>
        </telerik:RadPageView>
    </telerik:RadMultiPage>

</div>

<div class="buttonbar">
    <asp:ValidationSummary ID="vsSubmit" runat="server" ValidationGroup="submit" />

    <input type="button" id="btnCancelOrder" runat="server" width="115" value="Cancel Order"
        onclick="javascript:CancelOrder();" />

    <asp:Button ID="btnPIL" runat="server" Text="PIL" Width="75px" />
    <asp:Button ID="btnSetAsInvoiced" runat="server" Text="Set As Invoiced" Visible="true" OnClientClick="if(!confirmSetAsInvoiced()) return false;" ValidationGroup="submit" />
    <asp:Button ID="btnPodLabel" Visible="true" runat="server" Text="POD Label" Width="125px" />
    <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Delivery Note" />
    <asp:Button ID="btnAddGroupedOrder" runat="server" Text="Add Grouped Order" ValidationGroup="submit" OnClientClick="if(!orderSubmitCheck('btnAddGroupedOrder')) return false;" />
    <asp:Button ID="btnCopy" runat="server" Visible="false" Text="Create a copy" Style="width: 125px;" /><asp:TextBox ID="txtNumberofCopies" runat="server" Width="10" Text="1" Visible="false"></asp:TextBox>
    <asp:Button ID="btnSubmit" runat="server" Text="Add Order" ValidationGroup="submit" OnClientClick="if(!orderSubmitCheck('btnSubmit')) return false;" />
    <asp:Button ID="btnAddAndReset" runat="server" Text="Add Order & Reset" Style="width: 125px;" ValidationGroup="submit" OnClientClick="if(!orderSubmitCheck('btnAddAndReset')) return false;" />
    <asp:Button ID="btnCreateRun" runat="server" Text="Create Run" Visible="false" ValidationGroup="submit" />

</div>

<input id="hidDeliveryTimingMethod" type="hidden" runat="server" value="anytime" />
<input id="hidCollectionTimingMethod" type="hidden" runat="server" value="anytime" />
<input runat="server" id="hidCollectionFromTimeRestoreValue" type="hidden" />
<input runat="server" id="hidDeliveryFromTimeRestoreValue" type="hidden" />
<input runat="server" id="hidCollectionByTimeRestoreValue" type="hidden" />
<input runat="server" id="hidDeliveryByTimeRestoreValue" type="hidden" />
<input runat="server" id="hidNetworkIdentityId" type="hidden" />

<asp:HiddenField ID="hidExchangeRate" runat="server" Value="1" />
<asp:HiddenField ID="hidOrderGroupID" runat="server" Value="" />
<asp:HiddenField ID="hidGlobalFuelSurchargeAppliesToExtras" runat="server" Value="INVALID" />
<asp:HiddenField ID="hidFuelSurchargeRestoreValue" runat="server" Value="0" />
<asp:HiddenField ID="hidIsForeign" runat="server" Value="false" />
<asp:HiddenField ID="hidPageIsUpdate" runat="server" Value="" />
<asp:HiddenField ID="hidShowConfirmForOrderAfterDays" runat="server" Value="" />
<asp:HiddenField ID="hidTariffRate" runat="server" Value="" />
<asp:HiddenField ID="hidTariffRateOverridden" runat="server" Value="false" />
<asp:HiddenField ID="hidIsAutorated" runat="server" Value="false" />
<asp:HiddenField ID="hidManuallyEnteredRate" runat="server" Value="false" />
<asp:HiddenField ID="hidClientRateTariffDescription" runat="server" Value="" />
<asp:HiddenField ID="hidSurchargeExtraChanged" runat="server" Value="false" />
<asp:HiddenField ID="hidCanCalculateFuelSurcharge" runat="server" Value="true" />
<asp:HiddenField ID="hidReRateNow" runat="server" Value="false" />
<asp:HiddenField ID="hidIsManuallyAllocated" runat="server" Value="false" />
<asp:HiddenField ID="hidCheckSubmit" runat="server" Value="" />

<asp:Panel ID="pnlSetFocus" runat="server">
    <asp:Literal ID="litSetFocus" runat="server" EnableViewState="False"></asp:Literal>
</asp:Panel>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
		
        //Required for the openDialog function called by OpenBookingFormWindow
        var returnUrlFromPopUp = window.location;
        var palletForceTabVisible = false;
		
        function viewOrderProfile(orderID) {
            var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

            var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
        }

        $(document).ready(function() {           
		
            var chkCreateJob = $(":checkbox[id *= 'CreateJob']");
            var chkRequiresBookingIn = $('input[id*=chkRequiresBookIn]');
            var spanBookedIn = $('span[id*=SpanBookedIn]');

            if (chkRequiresBookingIn.prop("checked") == false) {
                spanBookedIn.hide();
            }

            var IsUpdate = ('<%=this.IsUpdate.ToString() %>' == '<%=System.Boolean.TrueString %>');
 
			if(IsUpdate == true)
			{
			    chkCreateJob.parent().css("display", "none");
			    chkCreateJob.prop("checked", false);
			}
			
		    // Resets the Add Order Button submission check on the page load.
			$('input:hidden[id*=hidCheckSubmit]').val("false");

			$('.signatureLink').on('click', signatureLink_click);

		});

        function hideDuplicateWindow()
        {
            var div = $get('<%=this.divDuplicateOrders.ClientID %>');
		    $(div).css("display", "none");  
		}

		function confirmSetAsInvoiced()
		{
		    return confirm("Are you sure you wish to set the Order as invoiced as this cannot be undone? \n\nClick 'Ok' to confirm or click 'Cancel' to leave the order in its current state.");
		}
		
		// Added to get confirmation of a zero rate
		function orderSubmitCheck(btnId)
		{
		    Page_ClientValidate("submit");
			
		    if(Page_IsValid)
		    {
		        var rntOrderRate = $find('<%=this.rntOrderRate.ClientID %>');
			    var orderRate = rntOrderRate.get_value();

			    if ('<%=this.ConfirmZeroRate.ToString() %>' == '<%=System.Boolean.TrueString %>' && orderRate == 0)
				{
				    var userConfirmation = confirm("The Order has a rate of zero, do you wish to continue?");
					
				    if(!userConfirmation)
				        return userConfirmation;
				}
				
				return confirmOrderSubmit(btnId);
            }
            else
                return false;
        }

        // Added to prevent the add order button being clicked more than once.
        function confirmOrderSubmit(btnId)
        {
            if($('input:hidden[id*=hidCheckSubmit]').val() == "true")
                return false;
            else
            {                
                $('input:hidden[id*=hidCheckSubmit]').val("true");
                $('input[id*=' + btnId + ']').val("Please Wait...")
            }

            return true;
        }
		
        function lnkReinstateAutorating_OnClick(lnk) {
            var hidManuallyEnteredRate = $get('<%=this.hidManuallyEnteredRate.ClientID %>');
		    hidManuallyEnteredRate.value = 'false';
		   
		    var hidTariffRateOverridden = $get('<%=this.hidTariffRateOverridden.ClientID %>');
		   hidTariffRateOverridden.value = 'false';
		   
		   var txtRateTariffCard = $get('<%=this.txtRateTariffCard.ClientID %>');
		   var hidManuallyEnteredRate = $get('<%=this.hidManuallyEnteredRate.ClientID %>');
		   
		    txtRateTariffCard.value = ""; // clear the rate, let the auto-rating mechanism populate it.
		   
		    // Hide the link
		    $(lnk).css("display", "none");
		   
		    ReRate();
		}
		
		function rntOrderRate_OnValueChanged(sender, eventArgs) {
		    CalculateTotal();
		}
		
		function rntOrderRate_OnKeyPress(sender, eventArgs)
		{
		    var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
		    var hidIsAutorated = $get('<%=this.hidIsAutorated.ClientID %>');
		    var hidManuallyEnteredRate = $get('<%=this.hidManuallyEnteredRate.ClientID %>');
		   
		    hidManuallyEnteredRate.value = 'true';
		   
		    // The order can only be overridden if it has been autorated
		    if (hidIsAutorated.value == 'true') {
			   
		        var hidTariffRateOverridden = $get('<%=this.hidTariffRateOverridden.ClientID %>');
		       hidTariffRateOverridden.value = "true";
			   
		       var txtRateTariffCard = $get('<%=this.txtRateTariffCard.ClientID %>');
			   txtRateTariffCard.value = "Overridden";

		       // Make the "Re-instate auto-rating" link available.
			   var lnkReInstateAutoRating = $get('<%=this.lnkReInstateAutoRating.ClientID %>');    
			   $(lnkReInstateAutoRating).css("display", "");
           }
       }
		
       function rntFuelSurchargePercentage_OnChange(sender, eventArgs)
       {
           CalculateTotal();
       }
		
       function HandleSelectAll(chk) {
           $(":checkbox[id$=chkSelectExtra]").each(function(chkIndex) { if (this.checked != chk.checked) this.click(); });
       }

       function viewOrderProfile(orderID) {
           var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;
           var randomnumber = Math.floor(Math.random() * 11)
           var wnd = window.open(url, randomnumber, "width=1180, height=900, resizable=1, scrollbars=1");
       }
		
       function HandleSelection(chk, rowIndex) {
           var mtv = $find("<%=grdExtras.ClientID %>").get_masterTableView();
		    var dataItem = mtv.get_dataItems()[rowIndex].get_element();
		    if (chk.checked) {
		        mtv.selectItem(dataItem);
		        dataItem.className = "SelectedRow_Orchestrator";                
		    }
		    else {
		        mtv.deselectItem(dataItem);
		        dataItem.className = "GridRow_Orchestrator";                
		    }

		    CalculateTotal();
		}

		function CloseOnReload() {
		    var oWin = GetRadWindow();
		    if (oWin != null)
		        oWin.Close();
		    else
		        self.close();
		}
		var cboClientInitiated = false;
		function cboClient_OnClientSelectedIndexChanged(sender, eventArgs)
		{
		    try
		    {
		        cboClientInitiated = true;
		        var hidNetworkIdentityId = $get('<%=this.hidNetworkIdentityId.ClientID %>');
				if (eventArgs.get_item().get_value() == hidNetworkIdentityId.value || $('select[id*=cboBusinessType] option:selected').attr("showpalletforcefields") == "true")
				{
				    $("tr[id*=trPalletForceFields]").css("display", "");
				    $("#tabPalletForce").css("display", "");
				    $("#tabviewPalletForce").css("display", "");
				    $('img[id*=imageCopyToSpecialInstructions]').css("display", "");
					
				    palletForceTabVisible = true;

				    var defaultLineTwo = '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions %>';
					if (defaultLineTwo != '')
					    if ($('input[id*=txtPalletForceNotes2]')[0].value == '')
					        $('input[id*=txtPalletForceNotes2]')[0].value = '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions %>';

				    // Blank the trunk date if the business type is changed
                    if ($find("<%=dteTrunkDate.ClientID %>").get_value() == '')
				        $find("<%=dteTrunkDate.ClientID %>").set_selectedDate($find("<%=dteCollectionByDate.ClientID %>").get_selectedDate());

                    if ($find("<%=rntPalletForceFullPallets.ClientID %>").get_value() == '')
				        $find("<%=rntPalletForceFullPallets.ClientID %>").set_value($find("<%=rntxtPallets.ClientID %>").get_value());
                }
                else {
                    $("tr[id*=trPalletForceFields]").css("display", "none");
                    $("#tabPalletForce").css("display", "none");
                    $("#tabviewPalletForce").css("display", "none");
                    $('img[id*=imageCopyToSpecialInstructions]').css("display", "none");
                    palletForceTabVisible = false;
                }

            }
            catch (err) { }
        }
		
	   
        function cboClient_itemsRequesting(sender, eventArgs)
        {
            try
            {
                var context = eventArgs.get_context();
                if ('<%=string.IsNullOrEmpty(Request.QueryString["oid"]) %>' == 'True') {
	                context["DisplaySuspended"] = false;
	            }
	            else {
	                context["DisplaySuspended"] = true;
	            }
            }
            catch (err) { }
        }
		

        function ViewJobDetails(jobID) {
            openResizableDialogWithScrollbars('../Job/Job.aspx?wiz=true&jobId=' + jobID+ getCSID(), '1220', '870');
        }

        function ViewInvoiceDetails(PDFLocation) {
            var url = '<%=this.ResolveUrl("~") %>' + PDFLocation;
		    window.open(url);
		}

		function CancelOrder() {
		    var url = '<%=this.ResolveUrl("~/Groupage/cancelOrder.aspx?orderID=" + OrderID) %>';

		    var wnd = window.radopen("about:blank", "cancelOrderWindow");
		    wnd.SetUrl(url);
		    wnd.SetTitle("Cancel Order");
		}

		function RemoveOrder(JobID, OrderID, UserName) {
		    grdOrders.Callback(JobID, OrderID, UserName);
		}

		function OpenPODWindow(jobId, collectDropId) {
		    var podFormType = 2;
		    var qs = "ScannedFormTypeId=" + podFormType;
		    qs += "&JobId=" + jobId;
		    qs += "&CollectDropId=" + collectDropId;
			
		    <%=dlgDocumentWizard.ClientID %>_Open(qs);
		}

		function OpenPODWindowForEdit(scannedFormId, jobId, collectDropId) {
		
		    var podFormType = 2;
		    var qs = "ScannedFormTypeId=" + podFormType;
		    qs += "&JobId=" + jobId;
		    qs += "&ScannedFormId=" + scannedFormId;
		    qs += "&CollectDropId=" + collectDropId;
			
		    <%=dlgDocumentWizard.ClientID %>_Open(qs);
		}

		function NewBookingForm(orderId) { 
		    var BookingFormType = 3;
		    var qs = "ScannedFormTypeId=" + BookingFormType;
		    qs += "&OrderId=" + orderId;

		    <%=dlgDocumentWizard.ClientID %>_Open(qs);
		}

		function ReDoBookingForm(scannedFormId, orderId) {
		    var BookingFormType = 3;
		    var qs = "ScannedFormTypeId=" + BookingFormType;
		    qs += "&ScannedFormId=" + scannedFormId;
		    qs += "&OrderId=" + orderId;

		    <%=dlgDocumentWizard.ClientID %>_Open(qs);
		}

		var _skipCollectionDateCheck = false;
		var _skipDeliveryDateCheck = false;

		var dteCollectionByTime = $find("<%=dteCollectionByTime.ClientID %>");
		var dteCollectionFromTime = $find("<%=dteCollectionFromTime.ClientID %>");

        function UpdateBookedIn(bookedIn) {
            var chkBookedIn = $('input[id*=chkBookedIn]');
            var chkRequiresBookingIn = $('input[id*=chkRequiresBookIn]');
            var divBookedIn = $('div[id*=bookedInContainer]');

            if (bookedIn.id == chkRequiresBookingIn[0].id)
                if (bookedIn.checked == true) {
                    divBookedIn.show();
                }
                else {
                    chkBookedIn.prop("checked", false);
                    divBookedIn.hide();
                }
        }

        function CV_ClientValidateDeliveryDate(source, args) {
            var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
		    var IsUpdate = $get("<%=hidPageIsUpdate.ClientID%>");

		    if (IsUpdate.value != "True") {
		        var today = new Date();
		        var day_date = today.getDate();
		        var month_date = today.getMonth();
		        var year_date = today.getFullYear();

		        today.setFullYear(year_date, month_date, day_date);

		        var enteredDateParts = dteDateTime.get_dateInput().get_displayValue().split("/");

		        var enteredDate = new Date();
		        enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

		        if (enteredDate >= today) {
		            args.IsValid = true;
		        }
		        else {
		            if (!_skipCollectionDateCheck) {
		                _skipCollectionDateCheck = true;
		                args.IsValid = false;
		                alert("The delivery date entered is in the past - Are you sure?");
		            }
		        }
		    }
		}

		function CV_ClientValidateDeliveryDate2(source, args) {
		    var dteDateTime = $find("<%=dteDeliveryByDate.ClientID%>");
		    var IsUpdate = $get("<%=hidPageIsUpdate.ClientID%>");
		    var hidShowConfirmForOrderAfterDays = $get("<%=hidShowConfirmForOrderAfterDays.ClientID%>");

		    if (IsUpdate.value != "True") {
		        var warningDate = new Date();
		        var day_date = warningDate.getDate();
		        var month_date = warningDate.getMonth();
		        var year_date = warningDate.getFullYear();

		        warningDate.setFullYear(year_date, month_date, day_date);
		        warningDate.setDate(warningDate.getDate() + parseInt(hidShowConfirmForOrderAfterDays.value));

		        var enteredDateParts = dteDateTime.get_dateInput().get_displayValue().split("/");

		        var enteredDate = new Date();
		        enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);



		        if (enteredDate >= warningDate) {
		            if (!_skipCollectionDateCheck) {
		                _skipCollectionDateCheck = true;

		                args.IsValid = true; // do not prevent the order from being created.
		                alert("The delivery date entered is far in the future - Are you sure?");
		            }
		        }
		        else {
		            args.IsValid = true;
		        }
		    }
		}

		function CV_ClientValidateCollectionDate(source, args) {

		    var dteDateTime = $find("<%=dteCollectionFromDate.ClientID%>");
		    var IsUpdate = $get("<%=hidPageIsUpdate.ClientID%>");

		    if (IsUpdate.value != "True") {

		        var today = new Date();
		        var day_date = today.getDate();
		        var month_date = today.getMonth();
		        var year_date = today.getFullYear();

		        today.setFullYear(year_date, month_date, day_date);

		        var enteredDateParts = dteDateTime.get_dateInput().get_displayValue().split("/");

		        var enteredDate = new Date();
		        enteredDate.setFullYear("20" + enteredDateParts[2], enteredDateParts[1] - 1, enteredDateParts[0]);

		        if (enteredDate >= today) {
		            args.IsValid = true;
		        }
		        else {
		            if (!_skipDeliveryDateCheck) {
		                _skipDeliveryDateCheck = true;
		                args.IsValid = false;
		                alert("The collection date entered is in the past - Are you sure?");
		            }
		        }
		    }
		}


		function collectionIsAnytime(rb) {
		    $('tr[id*=trCollectBy]').hide();

		    var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
			if (rb != null) {
			    dteCollectionFromDate.get_dateInput().focus();
			    dteCollectionFromDate.get_dateInput().selectAllText();
			}


			$('input:hidden[id*=hidCollectionTimingMethod]').val('anytime');

			$('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val($find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().get_value());

			$('input:hidden[id*=hidCollectionByTimeRestoreValue]').val($find("<%=dteCollectionByTime.ClientID %>").get_dateInput().get_value());

		    $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value('00:00');
		    $find("<%=dteCollectionFromTime.ClientID %>").set_enabled(false);

		    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value('23:59');
		    $find("<%=dteCollectionByTime.ClientID %>").set_enabled(false);
			
		}

		function deliveryIsAnytime(rb) {
		    $('tr[id*=trDeliverFrom]').hide();

		    var dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
			if (rb != null) {
			    dteDeliveryFromDate.get_dateInput().focus();
			    dteDeliveryFromDate.get_dateInput().selectAllText();
			}


			$('input:hidden[id*=hidDeliveryTimingMethod]').val('anytime');

			$('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().get_value());
			$('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val($find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().get_value());

		    $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value('00:00');
		    $find("<%=dteDeliveryFromTime.ClientID %>").set_enabled(false);

		    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value('23:59');
		    $find("<%=dteDeliveryByTime.ClientID %>").set_enabled(false);
			
		}

		function collectionTimedBooking(rb) {

		    $('tr[id*=trCollectBy]').hide();

            
		    var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
			if (rb != null) {
			    dteCollectionFromDate.get_dateInput().focus();
			    dteCollectionFromDate.get_dateInput().selectAllText();
			}

			var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

			$('input:hidden[id*=hidCollectionTimingMethod]').val('timed');

			if (method == 'window') {
			    $('input:hidden[id*=hidCollectionByTimeRestoreValue]').val($find("<%=dteCollectionByTime.ClientID %>").get_dateInput().get_value());
			}

            $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().get_value());

		    if (method == 'anytime') {
		        if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
		            $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

				} else {
				    $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

                if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
				} else {
				    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
            }
            $find("<%=dteCollectionByTime.ClientID %>").set_enabled(true);
		    $find("<%=dteCollectionFromTime.ClientID %>").set_enabled(true);
		}

		function collectionBookingWindow(rb) {
		    $('tr[id*=trCollectBy]').show();


		    /*var dteCollectionFromDate = $('input[id*=dteCollectionFromDate]');
			if (rb != null) {
				dteCollectionFromDate.focus();
				dteCollectionFromDate.select();
			}*/
		    var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
		    if (rb != null) {
		        dteCollectionFromDate.get_dateInput().focus();
		        dteCollectionFromDate.get_dateInput().selectAllText();
		    }

		    var method = $('input:hidden[id*=hidCollectionTimingMethod]').val();

		    $('input:hidden[id*=hidCollectionTimingMethod]').val('window');

		    if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
		        $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
			}

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionFromTimeRestoreValue]').val());

				} else {
				    $find("<%=dteCollectionFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

                if ($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val() != "") {
                    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidCollectionByTimeRestoreValue]').val());
				} else {
				    $find("<%=dteCollectionByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
            }
							   
            $find("<%=dteCollectionByTime.ClientID %>").set_enabled(true);
		    $find("<%=dteCollectionFromTime.ClientID %>").set_enabled(true);
		}

		function deliveryTimedBooking(rb) {
		    $('tr[id*=trDeliverFrom]').hide();


		    var dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");
			if (rb != null) {
			    dteDeliveryByDate.get_dateInput().focus();
			    dteDeliveryByDate.get_dateInput().selectAllText();
		        
			}
		

			var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

			$('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

			if (method == 'window') {
			    $('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val($find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().get_value());
			}

            $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().get_value());

		    if (method == 'anytime') {
		        if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
		            $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
				} else {
				    $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

                if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
				} else {
				    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").set_enabled(true);
		    $find("<%=dteDeliveryByTime.ClientID %>").set_enabled(true);
		}

		function deliveryBookingWindow(rb) {
		    $('tr[id*=trDeliverFrom]').show();

		    var dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");
			if (rb != null) {
			    dteDeliveryByDate.get_dateInput().focus();
			    dteDeliveryByDate.get_dateInput().selectAllText();
		        
			}

			var method = $('input:hidden[id*=hidDeliveryTimingMethod]').val();

			$('input:hidden[id*=hidDeliveryTimingMethod]').val('window');

			if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
			    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
			}

            if (method == 'anytime') {
                if ($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryFromTimeRestoreValue]').val());
				} else {
				    $find("<%=dteDeliveryFromTime.ClientID %>").get_dateInput().set_value("08:00");
				}

                if ($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val() != "") {
                    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value($('input:hidden[id*=hidDeliveryByTimeRestoreValue]').val());
				} else {
				    $find("<%=dteDeliveryByTime.ClientID %>").get_dateInput().set_value("17:00");
				}
            }

            $find("<%=dteDeliveryFromTime.ClientID %>").set_enabled(true);
		    $find("<%=dteDeliveryByTime.ClientID %>").set_enabled(true);
		}

		function OrderDetail_KeyPress(e) {
		    var keyCode = (e.which) ? e.which : e.keyCode;

		    e.returnValue = (keyCode >= 48 && keyCode <= 57);
		}

		function rntxtPallets_Blur(source) {
		    if (source.get_value() != '') {
				
		        var palletSpaces = $find('<%=this.rntxtPalletSpaces.ClientID %>');
			    var palletForceFullPallets = $find("<%=this.rntPalletForceFullPallets.ClientID %>");

			    if (palletForceFullPallets != null)
			        palletForceFullPallets.set_value(source.get_value());

			    if (palletSpaces.get_value() == '')
			        palletSpaces.set_value(source.get_value());
			}
        }

        // This fires once the ASP.NET Ajax engine has initialised and is available.

        function pageLoad() {

            var collectionMethod = $('input:hidden[id*=hidCollectionTimingMethod]').val();
            var deliveryMethod = $('input:hidden[id*=hidDeliveryTimingMethod]').val();
 
            load();

            //$('input:hidden[id*=hidDeliveryTimingMethod]').val('timed');

            if (deliveryMethod == 'anytime') {
                deliveryIsAnytime(null);
            }

            if (deliveryMethod == 'timed') {
                deliveryTimedBooking(null);
            }

            if (collectionMethod == 'anytime') {
                collectionIsAnytime(null);
            }

            if (collectionMethod == 'timed') {
                collectionTimedBooking(null);
            }

            var cboAllocatedTo = $find("<%= cboAllocatedTo.ClientID %>");
			var lblAllocatedTo = $get("<%= lblAllocatedTo.ClientID %>");
		    if (cboAllocatedTo != null || lblAllocatedTo != null) {
		        hookupAllocationListeners();
		    }
			
		    var rntOrderRate = $find('ctl00_ContentPlaceHolder1_ucOrder_rntOrderRate');

		    if (rntOrderRate != null) {
		        // Only need to hookup the rate listeners if the rate is visible.
		        HookupRateListeners();
		        var currency = rntOrderRate.get_numberFormat().PositivePattern.replace(/\w/g, '');
		        Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol = currency;
		    }

		    // Set the culture information so prices can be manipulated for the correct culture of the client.
		    var grdExtras = $find('<%=this.grdExtras.ClientID %>');
			
		    // If this is a new order and the previous order created had a rate, re rate the page so the surcharges are shown.
			if ('<%=this.IsUpdate.ToString() %>' == '<%=System.Boolean.FalseString %>') {
		        LookupRate();
		        CalculateTotal();
		    }
		    var hidCanCalculateFuelSurcharge = $get('<%=this.hidCanCalculateFuelSurcharge.ClientID %>');                    
		    var txtFuelSurchargePerc = $find('<%=this.rntFuelSurchargePercentage.ClientID %>');                    
		    var chkOverrideFuelSurcharge = $get('<%=this.chkOverrideFuelSurcharge.ClientID %>');                    
			
		    if (hidCanCalculateFuelSurcharge.value == 'true') {
		        if(txtFuelSurchargePerc != null && chkOverrideFuelSurcharge != null)
		        {
		            if(chkOverrideFuelSurcharge.checked == false) {
		                txtFuelSurchargePerc.disable();
		            }
					
		            var chkOverrideFuelSurcharge = $get('<%=this.chkOverrideFuelSurcharge.ClientID %>');
					
					$(chkOverrideFuelSurcharge).click(function() {
					
					    var txtFuelSurchargePerc = $find('<%=this.rntFuelSurchargePercentage.ClientID %>');                    
					    var hidFuelSurchargeRestoreValue = $get('<%=this.hidFuelSurchargeRestoreValue.ClientID %>');
						
					    if(this.checked) {
					        //   User has overridden the fuel surcharge
					        txtFuelSurchargePerc.enable();                    
					        hidFuelSurchargeRestoreValue.value = txtFuelSurchargePerc.get_value();
					    } else {
					        // User has not overridden the fuel surcharge
					        txtFuelSurchargePerc.disable();
					        txtFuelSurchargePerc.set_value(hidFuelSurchargeRestoreValue.value);
								
					    }
					});
                }
            }
        }

        // Hookup the event handlers to the various controls that affect the allocation,
        // any change in any should result in the allocation being looked up with the new information.
        function hookupAllocationListeners() {
            var clientCombo = $find('<%=this.cboClient.ClientID %>');
		    var collectionPointCombo = $find('<%=this.ucCollectionPoint.ComboClientID %>');
		    var collectionFromDateDate = $find('<%=this.dteCollectionFromDate.ClientID %>');
		    var collectionFromDateTime = $find('<%=this.dteCollectionFromTime.ClientID %>');
		    var deliveryPointCombo = $find('<%=this.ucDeliveryPoint.ComboClientID %>');
		    var deliveryDateDate = $find('<%=this.dteDeliveryByDate.ClientID %>');
		    var deliveryDateTime = $find('<%=this.dteDeliveryByTime.ClientID %>');

		    // Remove handlers so we don't get multiple captures.
		    if (clientCombo != null) clientCombo.remove_selectedIndexChanged(reAllocate); // Client
		    if (collectionPointCombo != null) collectionPointCombo.remove_selectedIndexChanged(reAllocate); // Collection Point
		    if (collectionFromDateDate != null) collectionFromDateDate.get_dateInput().remove_valueChanged(reAllocate); // Collection Date Time
		    if (collectionFromDateTime != null) collectionFromDateTime.get_dateInput().remove_valueChanged(reAllocate); // Collection Date Time
		    if (deliveryPointCombo != null) deliveryPointCombo.remove_selectedIndexChanged(reAllocate); // Delivery Point
		    if (deliveryDateDate != null) deliveryDateDate.get_dateInput().remove_valueChanged(reAllocate); // Delivery Date Time
		    if (deliveryDateTime != null) deliveryDateTime.get_dateInput().remove_valueChanged(reAllocate); // Delivery Date Time

		    // Add all the event handlers we need.
		    if (clientCombo != null) clientCombo.add_selectedIndexChanged(reAllocate); // Client
		    if (collectionPointCombo != null) collectionPointCombo.add_selectedIndexChanged(reAllocate); // Collection Point
		    if (collectionFromDateDate != null) collectionFromDateDate.get_dateInput().add_valueChanged(reAllocate); // Collection Date Time
		    if (collectionFromDateTime != null) collectionFromDateTime.get_dateInput().add_valueChanged(reAllocate); // Collection Date Time
		    if (deliveryPointCombo != null) deliveryPointCombo.add_selectedIndexChanged(reAllocate); // Delivery Point
		    if (deliveryDateDate != null) deliveryDateDate.get_dateInput().add_valueChanged(reAllocate); // Delivery Date Time
		    if (deliveryDateTime != null) deliveryDateTime.get_dateInput().add_valueChanged(reAllocate); // Delivery Date Time
		}

		function reAllocate() {
		    var cboAllocatedTo = $find("<%= cboAllocatedTo.ClientID %>");
		    var lblAllocatedTo = $("#<%= lblAllocatedTo.ClientID %>");
		    if (cboAllocatedTo == null && lblAllocatedTo.length == 0) {
		        return;
		    }

		    $("#spnAllocationError").html("");
				
		    // Do not override manually allocated orders
		    var hidIsManuallyAllocated = $("#<%= hidIsManuallyAllocated.ClientID %>");
			var isManuallyAllocated = Boolean.parse(hidIsManuallyAllocated.val());
			
		    // If the allocation has been blanked out or text typed in the box that doesn't match a drop down value, then it shouldn't be considered manually allocated.
			if (cboAllocatedTo != null) {
			    isManuallyAllocated &= cboAllocatedTo.get_value() != "" && (cboAllocatedTo.get_selectedItem() == null || cboAllocatedTo.get_text() == cboAllocatedTo.get_selectedItem().get_text());
			}
			
			if (!isManuallyAllocated) {
			    // Retrieve the various values from all the controls.
			    var clientIdentityID = getClientIdentityID();
			    var collectionPointID = getCollectionPointID();
			    var deliveryPointID = getDeliveryPointID();
			    var collectionDateTime = getCollectionDateTime();
			    var deliveryDateTime = getDeliveryDateTime();

			    // Have all the parameters been supplied?
			    if (clientIdentityID != null && collectionPointID != null && deliveryPointID != null && collectionDateTime != null && deliveryDateTime != null) {
			        try {
			            // Call the service!
			            var orderID = $("#<%= lblOrderId.ClientID %>").text() || null;
					    var orderGroupID = $("#<%= hidOrderGroupID.ClientID %>").val() || null;
					    Orchestrator.WebUI.Services.Allocation.GetConsortiumMemberToAllocate(orderGroupID, orderID, clientIdentityID, collectionPointID, deliveryPointID, collectionDateTime, deliveryDateTime, reAllocate_Success, reAllocate_Failure);
					}
                    catch (e) {
                        if (cboAllocatedTo != null) {
                            cboAllocatedTo.set_value("");
                        }
                        lblAllocatedTo.html("");
                        $("#spnAllocationError").html('This order could not be allocated.<br />' + e.description);
                    }
                }
            }
        }

        function reAllocate_Success(result) {
            var cboAllocatedTo = $find("<%= cboAllocatedTo.ClientID %>");
		    var lblAllocatedTo = $("#<%= lblAllocatedTo.ClientID %>");

		    if (result == null) {
		        if (cboAllocatedTo != null) {
		            cboAllocatedTo.clearSelection();
		            cboAllocatedTo.set_text("");
		        }
		        lblAllocatedTo.html("");
		    }
		    else {
		        var resultSplit = result.split(",");
		        var identityID = parseInt(resultSplit[0], 10);
		        var organisationName = resultSplit[1];
		        if (cboAllocatedTo != null) {
		            cboAllocatedTo.requestItems(organisationName, false);
		            cboAllocatedTo.set_value(result);
		            cboAllocatedTo.set_text(organisationName);
		        }
		        lblAllocatedTo.html(organisationName);
		    }
		}

		function reAllocate_Failure(error) {
		    $("#spnAllocationError").html('This order could not be allocated.<br />' + error.get_message());
		}

		//Tariff Handling
		//#region

		function LookupRate() {
		    var spnRateError = $get('spnRateError');
		    if (spnRateError != null)
		        spnRateError.innerHTML = '';
				
		    // Do not rate manually rated orders
		    var hidTariffRateOverridden = $get('<%=this.hidTariffRateOverridden.ClientID %>');
			if ('<%=this.IsUpdate.ToString() %>' == '<%=System.Boolean.FalseString %>' || hidTariffRateOverridden.value == 'false') {
		        // Retrieve the various values from all the controls.
		        var clientIdentityID = getClientIdentityID();
		        var businessTypeID = getBusinessTypeID();
		        var collectionPointID = getCollectionPointID();
		        var deliveryPointID = getDeliveryPointID();
		        var collectionDateTime = getCollectionDateTime();
		        var deliveryDateTime = getDeliveryDateTime();

		        // Order Instruction ID
		        var orderInstructionID = <%=this.OrderInstructionID %>;

			    // Pallet Type ID
				var palletTypeID = 0;
				var palletTypeCombo = $find('<%=this.cboPalletType.ClientID %>');
				if (palletTypeCombo != null) palletTypeID = palletTypeCombo.get_value();

			    // Pallet Spaces
				var palletSpaces = 0.0;
				var rntxtPalletSpaces = $find('<%=this.rntxtPalletSpaces.ClientID %>');
				if (rntxtPalletSpaces != null) palletSpaces = rntxtPalletSpaces.get_value();
				if (palletSpaces == '') palletSpaces = 0.0;

			    // Weight
				var weight = 0;
				var rntxtWeight = $find('<%=this.rntxtWeight.ClientID %>');
				if (rntxtWeight != null) weight = rntxtWeight.get_value();
				if (weight == '') weight = 0;

			    // Goods Type ID
				var goodsTypeID = 0;
				var goodsTypeCombo = $find('<%=this.cboGoodsType.ClientID %>');
				if (goodsTypeCombo != null) goodsTypeID = goodsTypeCombo.get_value();

			    // Order Service Level ID
				var orderServiceLevelID = 0;
				var serviceLevelCombo = $find('<%=this.cboService.ClientID %>');
				if (serviceLevelCombo != null) orderServiceLevelID = serviceLevelCombo.get_value();

			    // Have all the parameters been supplied?
				if (clientIdentityID != null && businessTypeID != null && orderInstructionID != "" && collectionPointID != null && deliveryPointID != null && collectionDateTime != null && deliveryDateTime != null) {
				    try {
				        // Call the service!
						
				        // If customer identity matches the network identity and the business type isPalletNetwork
				        // then we need to include the vigo fields in the rating calculation.
						
				        var hidNetworkIdentityId = $get('<%=this.hidNetworkIdentityId.ClientID %>');
					    if (
							hidNetworkIdentityId.value == clientIdentityID 
								|| 
							$('select[id*=cboBusinessType] option:selected').attr("showpalletforcefields") == "true"
							) 
					    {
					        var rntPalletForceFullPallets = $find('<%=this.rntPalletForceFullPallets.ClientID %>');
						    var rntPalletForceHalfPallets = $find('<%=this.rntPalletForceHalfPallets.ClientID %>');
						    var rntPalletForceQtrPallets = $find('<%=this.rntPalletForceQtrPallets.ClientID %>');
						    var rntPalletForceOverPallets = $find('<%=this.rntPalletForceOverPallets.ClientID %>');
							
						    var fullPallets = 0;
						    var halfPallets = 0;
						    var quarterPallets = 0;
						    var oversizePallets = 0;
						
						    if (rntPalletForceFullPallets.get_value() != "") {
						        fullPallets = rntPalletForceFullPallets.get_value();
						    }

						    if (rntPalletForceHalfPallets.get_value() != "") {
						        halfPallets = rntPalletForceHalfPallets.get_value();
						    }

						    if (rntPalletForceQtrPallets.get_value() != "") {
						        quarterPallets = rntPalletForceQtrPallets.get_value();
						    }

						    if (rntPalletForceOverPallets.get_value() != "") {
						        oversizePallets = rntPalletForceOverPallets.get_value();
						    }
						
							
						    Orchestrator.WebUI.Tariff.Tariffs.GetRateWithSurchargesForNetwork(clientIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLevelID, collectionDateTime, deliveryDateTime, fullPallets, halfPallets, quarterPallets, oversizePallets, LookupRateSuccess, LookupRateFailure);
						} else {
							
						    Orchestrator.WebUI.Tariff.Tariffs.GetRateWithSurcharges(clientIdentityID, businessTypeID, orderInstructionID, collectionPointID, deliveryPointID, palletTypeID, palletSpaces, weight, goodsTypeID, orderServiceLevelID, collectionDateTime, deliveryDateTime, LookupRateSuccess, LookupRateFailure);
						}
                    }
                    catch (e) {
                        var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
					    hidTariffRate.value = "";
					}
                }
            }
        }
		
        // Hookup the event handlers to the various controls that affect the rate,
        // any change in any should result in the rate being looked up with the new information.
        function HookupRateListeners() {
            var clientCombo = $find('<%=this.cboClient.ClientID %>');
		    var goodsTypeCombo = $find('<%=this.cboGoodsType.ClientID %>');
		    var serviceLevelCombo = $find('<%=this.cboService.ClientID %>');
		    var palletTypeCombo = $find('<%=this.cboPalletType.ClientID %>');
		    var collectionPointCombo = $find('<%=this.ucCollectionPoint.ComboClientID %>');
		    var collectionFromDateDate = $find('<%=this.dteCollectionFromDate.ClientID %>');
		    var collectionFromDateTime = $find('<%=this.dteCollectionFromTime.ClientID %>');
		    var deliveryPointCombo = $find('<%=this.ucDeliveryPoint.ComboClientID %>');
		    var deliveryDateDate = $find('<%=this.dteDeliveryByDate.ClientID %>');
		    var deliveryDateTime = $find('<%=this.dteDeliveryByTime.ClientID %>');
		    var rntOrderRate = $find('<%=this.rntOrderRate.ClientID %>');

		    // Remove handlers so we don't get multiple captures.
		    if (clientCombo != null) clientCombo.remove_selectedIndexChanged(ReRate); // Client

		    // Order Instruction is handled in code behind


		    if (goodsTypeCombo != null) goodsTypeCombo.remove_selectedIndexChanged(ReRate); // Goods Type
		    if (serviceLevelCombo != null) serviceLevelCombo.remove_selectedIndexChanged(ReRate); // Service Level
		    if (palletTypeCombo != null) palletTypeCombo.remove_selectedIndexChanged(ReRate); // Pallet Type
		    if (collectionPointCombo != null) collectionPointCombo.remove_selectedIndexChanged(ReRate); // Collection Point
		    if (collectionFromDateDate != null) collectionFromDateDate.get_dateInput().remove_valueChanged(ReRate); // Collection Date Time
		    if (collectionFromDateTime != null) collectionFromDateTime.get_dateInput().remove_valueChanged(ReRate); // Collection Date Time
		    if (deliveryPointCombo != null) deliveryPointCombo.remove_selectedIndexChanged(ReRate); // Delivery Point
		    if (deliveryDateDate != null) deliveryDateDate.get_dateInput().remove_valueChanged(ReRate); // Delivery Date Time
		    if (deliveryDateTime != null) deliveryDateTime.get_dateInput().remove_valueChanged(ReRate); // Delivery Date Time
		    if (rntOrderRate != null) rntOrderRate.remove_valueChanged(CheckForManualRating); // Rate

		    // Add all the event handlers we need.
		    if (clientCombo != null) clientCombo.add_selectedIndexChanged(ReRate); // Client

		    // Order Instruction is handled in code behind

		    //if (palletSpaceCombo != null) palletSpaceCombo.add_selectedIndexChanged(ReRate); // Pallet Spaces
		    if (goodsTypeCombo != null) goodsTypeCombo.add_selectedIndexChanged(ReRate); // Goods Type
		    if (serviceLevelCombo != null) serviceLevelCombo.add_selectedIndexChanged(ReRate); // Service Level
		    if (palletTypeCombo != null) palletTypeCombo.add_selectedIndexChanged(ReRate); // Pallet Type
		    if (collectionPointCombo != null) collectionPointCombo.add_selectedIndexChanged(ReRate); // Collection Point
		    if (collectionFromDateDate != null) collectionFromDateDate.get_dateInput().add_valueChanged(ReRate); // Collection Date Time
		    if (collectionFromDateTime != null) collectionFromDateTime.get_dateInput().add_valueChanged(ReRate); // Collection Date Time
		    if (deliveryPointCombo != null) deliveryPointCombo.add_selectedIndexChanged(ReRate); // Delivery Point
		    if (deliveryDateDate != null) deliveryDateDate.get_dateInput().add_valueChanged(ReRate); // Delivery Date Time
		    if (deliveryDateTime != null) deliveryDateTime.get_dateInput().add_valueChanged(ReRate); // Delivery Date Time
		    if (rntOrderRate != null) rntOrderRate.add_valueChanged(CheckForManualRating); // Rate
		}

		function ReRate() {
		    checkPalletInput(null, null);
		    LookupRate();
		}

		function CheckForManualRating(sender, eventArgs) {
		    var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
		    tariffRate = hidTariffRate.value;

		    //If the rate has been changed
		    if (eventArgs.get_newValue() != eventArgs.get_oldValue) {
				
		        var hidIsAutorated = $get('<%=this.hidIsAutorated.ClientID %>');
					
			    //If the previous rate was a Tariff rate
			    if (eventArgs.get_oldValue() == tariffRate && hidIsAutorated.value == "true") {
			        var txtRateTariffCard = $get('<%=this.txtRateTariffCard.ClientID %>');
				    txtRateTariffCard.value = "Overridden";
				}
                CalculateTotal();
            }
        }

        function LookupRateSuccess(result) {
            var hidIsAutorated = $get('<%=this.hidIsAutorated.ClientID %>');
		    var hidManuallyEnteredRate = $get('<%=this.hidManuallyEnteredRate.ClientID %>');
		    var hidTariffRateOverridden = $get('<%=this.hidTariffRateOverridden.ClientID %>');
		    var hidTariffRate = $get('<%=this.hidTariffRate.ClientID %>');
		    var rntOrderRate = $find('<%=this.rntOrderRate.ClientID %>');
		    var txtRateTariffCard = $get('<%=this.txtRateTariffCard.ClientID %>');
			
		    if (rntOrderRate != null && txtRateTariffCard != null)
		    {
		        if (result == null || result.Rate == null || result.Rate.Rate == null) {
		            // No rate found for the order - it is not autorated.
		            hidIsAutorated.value = 'false';
		            hidTariffRateOverridden.value = "false";
					
		            // TF 24/02/10: Do not blat the rate when re-rating and no rate found, as per JR request.
		            //hidTariffRate.value = "";
					
		            var hidClientRateTariffDescription = $get('<%=this.hidClientRateTariffDescription.ClientID %>');
					txtRateTariffCard.value = hidClientRateTariffDescription.value;
					
					if (hidManuallyEnteredRate.value == 'false') {
					    // TF 24/02/10: Do not blat the rate when re-rating and no rate found, as per JR request.
					    //var rntOrderRate = $find('<%=this.rntOrderRate.ClientID %>');
					    //rntOrderRate.set_value('');
					    //CalculateTotal();
					} else {
					    // Hide the link to reinstate autorating
					    var lnkReInstateAutoRating = $get('<%=this.lnkReInstateAutoRating.ClientID %>');    
					    $(lnkReInstateAutoRating).css("display", "none");                    
					}
				   
                }
                else {
					
				    // Identify that a rate has been found for the order thus the 
				    // order has been auto-rated (it can still be overridden, but to be overridden the order must be autorated).
                    hidIsAutorated.value = 'true';
					
				    // Set the tariff rate 
                    var orderRate = result.Rate.ForeignRate;
                    hidTariffRate.value = result.Rate.ForeignRate; // So we can determine if the rate has been overridden.
					
                    if (hidTariffRateOverridden.value == "true" || hidManuallyEnteredRate.value == "true") {
                        // Show the link to reinstate autorating
                        var lnkReInstateAutoRating = $get('<%=this.lnkReInstateAutoRating.ClientID %>');    
					    $(lnkReInstateAutoRating).css("display", "");                    

					    var txtRateTariffCard = $get('<%=this.txtRateTariffCard.ClientID %>');
						txtRateTariffCard.value = "Overridden";
						
						return; // do not update the order with autorated values... the rate has been manually entered.
                    }
					
                    rntOrderRate.set_value(orderRate);

				    // Show the user which tariff was used.
                    txtRateTariffCard.value = result.Rate.TariffDescription + ' [' + result.Rate.TariffTableDescription + ']';

                    var grdExtraRows = $find('<%=this.grdExtras.ClientID %>').get_masterTableView().get_dataItems();
					var isForeign = ($get('<%=this.hidIsForeign.ClientID %>').value == 'true');

				    if (result.Surcharges != null && result.Surcharges.length > 0) {
				        $(result.Surcharges).each(function(i) {
				            for (i = 0; i < grdExtraRows.length; i++) {
				                if (grdExtraRows[i].getDataKeyValue("ExtraTypeId") == this.ExtraTypeID && !grdExtraRows[i].get_selected()) {
									
				                    var currency = rntOrderRate.get_numberFormat().PositivePattern.replace(/\w/g, '');
				                    Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol = currency;
									
				                    var rntExtraForeignAmount = grdExtraRows[i].findControl("rntExtraForeignAmount");
				                    rntExtraForeignAmount.get_numberFormat().PositivePattern = currency;
				                    rntExtraForeignAmount.set_value(this.ForeignRate);
				                }
				            }
				        })
				    }
				    CalculateTotal();

				    //                    if (orderRate == 0) {
				    //                        alert("The Order has a Rate of Zero.");
				    //                    }
				}
            }
        }

        function LookupRateFailure(error, userContext, methodName) {
            var spnRateError = $get('spnRateError');
            if (spnRateError != null)
                spnRateError.innerHTML = 'This order could not be rated.<br />' + error.get_message();
        }

        function surchargeExtraChanged() {
            var hidSurchargeExtraChanged = $get('<%=this.hidSurchargeExtraChanged.ClientID %>');
		    hidSurchargeExtraChanged.value = "true";
		}

		function rntExtraForeignAmount_OnValueChanged(source, args) {            
		    surchargeExtraChanged();   
		    CalculateTotal();
		}

		function calculateOrderRateFuelSurcharge() {
	   
		    var rntOrderRate = $find('<%=this.rntOrderRate.ClientID %>');
		    var txtFuelSurchargePerc = $find('<%=this.rntFuelSurchargePercentage.ClientID %>');
		    var lblFuelSurchargeAmount = $get('<%=this.lblFuelSurcharge.ClientID %>')
			
		    if (rntOrderRate == null) return;
		   
		    var rate = rntOrderRate.get_value();
		    var fsAmount = 0;
		    var fsPerc = txtFuelSurchargePerc.get_value(); //.replace("%","");
			
		    fsAmount = (fsPerc/100) * rate;
			
		    // Ensure rounding to 4dp
		    var rounded = Math.round(fsAmount * 10000);
		    fsAmount = rounded / 10000;
			
		    if (fsAmount != NaN) {
		        lblFuelSurchargeAmount.innerText = fsAmount; // String.localeFormat("{0:c}", fsAmount);
		    } else {
		        lblFuelSurchargeAmount.innerText = 0; // String.localeFormat("{0:c}", 0);
		    }
			
		    return fsAmount;
		}

		function CalculateTotal() {
		    var rntOrderRate = $find('<%=this.rntOrderRate.ClientID %>');
		    if (rntOrderRate == null) return;
			
		    var currency = rntOrderRate.get_numberFormat().PositivePattern.replace(/\w/g, '');
		    Sys.CultureInfo.CurrentCulture.numberFormat.CurrencySymbol = currency;
			
		    var foreignTotal = 0;
		    var extraFuelSurchargeTotal = 0;
		    var total = 0;
		    var rate = rntOrderRate.get_value();
			
		    var chkBoxExtras = $("input[id*=chkSelectExtra]");

		    if (rate != '') foreignTotal = rate;
			
		    var extraFuelSurchargeTotal = calculateOrderRateFuelSurcharge();
			
		    var grdExtraRows = $find('<%=this.grdExtras.ClientID %>').get_masterTableView().get_dataItems();

			var globalApplyFuelSurchargeToExtras = false;
		    // Get the global fuel surcharge applies from the organisation defaults settings.

			for (var rowIndex = 0; rowIndex < grdExtraRows.length; rowIndex++) {
			    var chkExtra = $(grdExtraRows[rowIndex].get_element()).find("input[id*='chkSelectExtra']");
			    if (chkExtra != null && chkExtra[0].checked) { 
						
			        var foreignExtraAmount = grdExtraRows[rowIndex].findControl("rntExtraForeignAmount").get_value();
			        var lblExtraFuelSurchargeAmount = $('span[id*=lblExtraFuelSurchargeAmount]')[rowIndex];
			        if (foreignExtraAmount != '') {
			            foreignTotal += foreignExtraAmount;
							
			            var hidGlobalFuelSurchargeAppliesToExtras = $get('<%=this.hidGlobalFuelSurchargeAppliesToExtras.ClientID %>');
							
						if (grdExtraRows[rowIndex].getDataKeyValue("FuelSurchargeApplies") == "True" && hidGlobalFuelSurchargeAppliesToExtras.value == "true") {
								
							var txtFuelSurchargePerc = $find('<%=this.rntFuelSurchargePercentage.ClientID %>');
							var fsPerc = txtFuelSurchargePerc.get_value();
							var fsAmount = 0;
								
							fsAmount = ( (fsPerc/100) * foreignExtraAmount );
								
							if (fsAmount != NaN) {
							    // calculate the fuel surcharge for this extra
									
							    var rounded = Math.round(fsAmount * 10000);
							    fsAmount = rounded / 10000;
									
							    lblExtraFuelSurchargeAmount.innerText = fsAmount; //String.localeFormat("{0:c}", fsAmount);            
									
							    extraFuelSurchargeTotal += fsAmount;
							} else {
							    lblExtraFuelSurchargeAmount.innerText = 0; // String.localeFormat("{0:c}", 0);
							}
						} else {
							// No fuel surcharge applies - indicate this to the user.
							lblExtraFuelSurchargeAmount.innerText = '';
						}
                    }
                }
            }
			
            if (foreignTotal > 0) {
                var exRate = parseFloat($get('<%=this.hidExchangeRate.ClientID %>').value);
			    total = foreignTotal / exRate;
			}

            var spnForeignTotal = $get('<%=this.spnForeignTotal.ClientID %>');
		    spnForeignTotal.innerText = String.localeFormat("{0:c}", foreignTotal);

		    var spnFuelSurchargeTotal = $get('<%=this.spnFuelSurchargeTotal.ClientID %>');
			var spnGrandTotal = $get('<%=this.spnGrandTotal.ClientID %>');
			
		    if (extraFuelSurchargeTotal != NaN) {
		        spnFuelSurchargeTotal.innerText = String.localeFormat("{0:c}", extraFuelSurchargeTotal);            
		        spnGrandTotal.innerText = String.localeFormat("{0:c}", foreignTotal + extraFuelSurchargeTotal);            
		    } else {
		        spnFuelSurchargeTotal.innerText = String.localeFormat("{0:c}", 0);            
		        spnGrandTotal.innerText = String.localeFormat("{0:c}", foreignTotal);            
		    }
		}

		var focusTimeout = -1;
		var focusControl = null;

		function GiveFocus() {
		    clearTimeout(focusTimeout);
		    focusControl.focus();
		    focusControl = null;
		}

		function SetFocus(controlID) {
		    focusControl = null;
		    if (controlID != "") {
		        var control = $get(controlID);
		        if (control != null) {
		            focusControl = control;
		            focusTimeout = setTimeout("GiveFocus()", 500);
		        }
		    }
		}
		
		var cboBusinessType_change = function() {
		    $('select[id*=cboBusinessType] option:selected').each(function() {
		        var chkCreateJob = $(":checkbox[id *= 'CreateJob']");
		        var IsUpdate = ('<%=this.IsUpdate.ToString() %>' == '<%=System.Boolean.TrueString %>');

				if (!IsUpdate && $(this).attr("showcreatejob") == "true" && $("#<%= hidOrderGroupID.ClientID %>").val() == "")
			    {
			        chkCreateJob.parent().css("display", "");
			        chkCreateJob.prop("checked", $(this).attr("createjobchecked") == "true" ? true : false);
			    }
			    else {
			        chkCreateJob.prop("checked", false);
			        chkCreateJob.parent().css("display", "none");
			    }
				
		        // Update the vigo collection depot based on the business type - note this is for UI display only, the same change will be applied when the order is saved.
		        var palletNetworkExportDepotCode = $(this).attr('palletnetworkexportdepotcode');

		        if (palletNetworkExportDepotCode) {
		            $('#lblCollectionDepot,#lblRequestingDepot').text(palletNetworkExportDepotCode);
                }

			    var clientIdentityID = 0;
			    var clientCombo = $find('<%=this.cboClient.ClientID %>');
				if (clientCombo != null) clientIdentityID = clientCombo.get_value();
				var hidNetworkIdentityId = $get('<%=this.hidNetworkIdentityId.ClientID %>');
				
				if ($("select[id*=cboBusinessType] option:selected").attr("showpalletforcefields") == "true" || clientIdentityID == hidNetworkIdentityId.value) {
				    $("tr[id*=trPalletForceFields]").css("display", "");
				    $("#tabPalletForce").css("display", "");
				    $("#tabviewPalletForce").css("display", "");
				    $('img[id*=imageCopyToSpecialInstructions]').css("display", "");
					
				    palletForceTabVisible = true;

				    var defaultLineTwo = '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions %>';
					if(defaultLineTwo != '')
					    if($('input[id*=txtPalletForceNotes2]')[0].value == '')
					        $('input[id*=txtPalletForceNotes2]')[0].value = '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions %>';

				    // Blank the trunk date if the business type is changed
                    if ($find("<%=dteTrunkDate.ClientID %>").get_value() == '')
				        $find("<%=dteTrunkDate.ClientID %>").set_selectedDate($find("<%=dteCollectionByDate.ClientID %>").get_selectedDate());

                    if ($find("<%=rntPalletForceFullPallets.ClientID %>").get_value() == '')
				        $find("<%=rntPalletForceFullPallets.ClientID %>").set_value($find("<%=rntxtPallets.ClientID %>").get_value());
                }
                else {
                    $("tr[id*=trPalletForceFields]").css("display", "none");
                    $("#tabPalletForce").css("display", "none");
                    $("#tabviewPalletForce").css("display", "none");
                    $('img[id*=imageCopyToSpecialInstructions]').css("display", "none");
                    palletForceTabVisible = false;
                }
				
			    ReRate();
			});
			
			
		    shortcut.add("Enter", function(){
		        return false;
		    });
		}

        function load()
        {
            $("#tabPalletForce").css("display", "none");
            $("#tabviewPalletForce").css("display", "none");
			
            palletForceTabVisible = false;
			
            var displayDeviationReason = '<%= Orchestrator.Globals.Configuration.DisplayDeviationCode %>';
			if(displayDeviationReason == 'True')
			    $("#divDeviationReason").css("display", "");
			else
			    $("#divDeviationReason").css("display", "none");
			
			$('select[id*=cboBusinessType]').change(cboBusinessType_change);

			var clientIdentityID = 0;
			var clientCombo = $find('<%=this.cboClient.ClientID %>');
			if (clientCombo != null) clientIdentityID = clientCombo.get_value();
			var hidNetworkIdentityId = $get('<%=this.hidNetworkIdentityId.ClientID %>');
			if ($("select[id*=cboBusinessType] option:selected").attr("showpalletforcefields") == "true" || clientIdentityID == hidNetworkIdentityId.value) {
			    $("#tabPalletForce").css("display", "");
			    $("#tabviewPalletForce").css("display", "");
			    $("tr[id*=trPalletForceFields]").css("display", "");
			    $('img[id*=imageCopyToSpecialInstructions]').css("display", "");
				
			    palletForceTabVisible = true;

			    var defaultLineTwo = '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions %>';
				if(defaultLineTwo != '')
				    if($('input[id*=txtPalletForceNotes2]')[0].value == '')
				        $('input[id*=txtPalletForceNotes2]')[0].value = '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions %>';

            }

            if ($('input:radio[id*=rdCollectionTimedBooking]')[0].checked == true)
                $('tr[id*=trCollectBy]').hide();

            if ($('input:radio[id*=rdCollectionIsAnytime]')[0].checked == true)
                $('tr[id*=trCollectBy]').hide();

            if ($('input:radio[id*=rdCollectionBookingWindow]')[0].checked == true)
                $('tr[id*=trCollectBy]').show();

            if ($('input:radio[id*=rdDeliveryTimedBooking]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').hide();

            if ($('input:radio[id*=rdDeliveryIsAnytime]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').hide();

            if ($('input:radio[id*=rdDeliveryBookingWindow]')[0].checked == true)
                $('tr[id*=trDeliverFrom]').show();

            if ($('#<%=btnAddGroupedOrder.ClientID%>')[0] == null) {
			    //$('#tdCreateJob').css("display", "none");
			    $(":checkbox[id *= 'CreateJob']").prop("checked", false);
			}
			if ($('input[id*=chkRequiresBookIn]').prop("checked") == false) {
			    $('div[id*=bookedInContainer]').hide();
			}
			
			CalculateTotal();
			
			var hidIsAutorated = $get('<%=this.hidIsAutorated.ClientID %>');
			var hidTariffRateOverridden = $get('<%=this.hidTariffRateOverridden.ClientID %>');
			
		    // The order can only be overridden if it has been autorated            
		    if (hidIsAutorated.value == 'true' && hidTariffRateOverridden.value == 'true') {
				
		        try {
					
		            var txtRateTariffCard = $get('<%=this.txtRateTariffCard.ClientID %>');
				    txtRateTariffCard.value = "Overridden";
				    // Make the "Re-instate auto-rating" link available.
				    var lnkReInstateAutoRating = $get('<%=this.lnkReInstateAutoRating.ClientID %>');    
					$(lnkReInstateAutoRating).css("display", "");
					
                } catch (err) { } // If this is an order group then the rate tariff card isn't visible...swallow.
	
            }
        }

        function validateBusinessType(sender, eventArgs) {
            var firstItem = $("#" + '<%=cboBusinessType.ClientID %>');
		    var result = false;
		    var value = parseInt(firstItem.val(), 10);

		    if (!isNaN(value) && value > 0)
		        result = true;

		    eventArgs.IsValid = result;
		    return eventArgs;
		}

		function CopyDeliveryNotesToPrintableNotes() {
		    var notes = $('textarea[id*=txtNotes]').val();
		    notes += $('textarea[id*=txtDeliveryNotes]').val();

		    $('textarea[id*=txtNotes]').val(notes);
		}

		function CopyToSpecialInstructions(sender, eventArgs) {
		    var orderNotesArray = new Array();
		    orderNotesArray[0] = "";
		    orderNotesArray[1] = "";
		    orderNotesArray[2] = "";
		    orderNotesArray[3] = "";
		    var count = 1;
		    var lengthExceeded = false;
		    var space = 0;
		    var availableSpace = 0;
		    var NOTES_MAX_COLUMN_LENGTH = 36;

		    var orderNotes = $('textarea[id*=txtDeliveryNotes]').val(); 

		    orderNotes = orderNotes.replace("\n", " ");
		    orderNotes = orderNotes.replace("\r", " ");

		    // Remove unwanted spaces
		    orderNotes = orderNotes.replace("  ", " ");
		    orderNotes = orderNotes.trim();

		    while (count < 5)
		    {
		        if (orderNotes.length == 0)
		            break;

		        availableSpace = NOTES_MAX_COLUMN_LENGTH;
		        lengthExceeded = false;

		        while (orderNotes.length > 0 && !lengthExceeded)
		        {
		            var nextNotesChunk = "";

		            space = orderNotes.indexOf(" ");
		            if (space == -1)
		            {
		                // No space found
		                if (orderNotes.length <= availableSpace)
		                {
		                    nextNotesChunk = orderNotes;
		                    orderNotes = "";
		                }
		                else
		                {
		                    if (availableSpace > 0)
		                    {
		                        nextNotesChunk = orderNotes.substring(0, availableSpace);
		                        orderNotes = orderNotes.substring(nextNotesChunk.length, orderNotes.length - nextNotesChunk.length);
		                    }
		                    else
		                    {
		                        lengthExceeded = true;
		                    }
		                }
		            }
		            else if (space < availableSpace)
		            {
		                // Get the chunk
		                nextNotesChunk = orderNotes.substring(0, space + 1); // +1 grab the space as well

		                // Trim the chunk of the order notes
		                orderNotes = orderNotes.substring(space + 1, orderNotes.length);

		                // How much space is left in this array val
		                availableSpace -= nextNotesChunk.length;
		            }
		            else
		            {
		                lengthExceeded = true;

		                if (space > NOTES_MAX_COLUMN_LENGTH - 1)
		                {
		                    nextNotesChunk = orderNotes.substring(0, availableSpace);
		                    orderNotes = orderNotes.substring(nextNotesChunk.length, orderNotes.length - nextNotesChunk.length);
		                }
		            }

		            // Add the chunk to the next available array section
		            orderNotesArray[count - 1] = orderNotesArray[count - 1].concat( nextNotesChunk);
		        }

		        count++;
		    }

		    if('<%= Orchestrator.Globals.Configuration.PalletForceSpecialInstructionsEnabled.ToString().ToLower()%>' == 'true'
				&& '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions%>' == '')
			{
			    $(':input[id*=txtPalletForceNotes1]').val(orderNotesArray[0]);
			    $(':input[id*=txtPalletForceNotes2]').val(orderNotesArray[1]);
			    $(':input[id*=txtPalletForceNotes3]').val(orderNotesArray[2]);
			    $(':input[id*=txtPalletForceNotes4]').val(orderNotesArray[3]);
			}
			else if('<%= Orchestrator.Globals.Configuration.PalletForceSpecialInstructionsEnabled.ToString().ToLower()%>' == 'true'
				&& '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions%>' != '')
			{
			    $(':input[id*=txtPalletForceNotes3]').val(orderNotesArray[0]);
			    $(':input[id*=txtPalletForceNotes4]').val(orderNotesArray[1]);
			}
			else if('<%= Orchestrator.Globals.Configuration.PalletForceSpecialInstructionsEnabled.ToString().ToLower()%>' == 'false'
				&& '<%= Orchestrator.Globals.Configuration.DefaultPalletForceSpecialInstructions%>' == '')
			{
			    $(':input[id*=txtPalletForceNotes2]').val(orderNotesArray[0]);
			    $(':input[id*=txtPalletForceNotes3]').val(orderNotesArray[1]);
			    $(':input[id*=txtPalletForceNotes4]').val(orderNotesArray[2]);
			}
			else
			{
			    $(':input[id*=txtPalletForceNotes3]').val(orderNotesArray[0]);
			    $(':input[id*=txtPalletForceNotes4]').val(orderNotesArray[1]);
			} 
}

function CallInThis(jobID, instructionID) {
    openResizableDialogWithScrollbars("/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + jobID + "&instructionid=" + instructionID + getCSID(), 860,790);
}

function cboService_SelectedIndexChanged(sender, eventArgs) {
    var palletForceService = $find("<%=cboPalletForceService.ClientID %>");
	       var orderServiceLevelID = sender.get_selectedItem().get_value();

	       if (palletForceService != null) {
	           var pfItem = palletForceService.findItemByValue(orderServiceLevelID);

	           if (pfItem)
	               pfItem.select();
	       }

	       updateDeliveryDateForServiceLevel(sender);
	   }      
	   function setPalletForceService(orderServiceLevelID){
	       var palletForceService = $find("<%=cboPalletForceService.ClientID %>");
	        if (palletForceService != null) {
	            var pfItem = palletForceService.findItemByValue(orderServiceLevelID);
                
	            if (pfItem)
	                pfItem.select();
	        }
	    }
	    function dteCollectionFromDate_SelectedDateChanged(sender, eventArgs) {
	        var dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID %>");
		    var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");
		    var updateDate = false;

		    if (dteCollectionByDate != null) {
		        if (rdCollectionBookingWindow != null && rdCollectionBookingWindow.prop("checked") != true)
		            updateDate = true;
		        else if (sender.get_selectedDate() > dteCollectionByDate.get_selectedDate())
		            updateDate = true;
		    }

		    if (updateDate) {
		        dteCollectionByDate.set_selectedDate(sender.get_selectedDate());
		    }
		}

		function dteCollectionByDate_SelectedDateChanged(sender, eventArgs) {
		    var serviceLevelCombo = $find("<%=cboService.ClientID %>");
		    var dteCollectionFromDate = $find("<%=dteCollectionFromDate.ClientID %>");
		    var rdCollectionBookingWindow = $("#" + "<%=rdCollectionBookingWindow.ClientID%>");

		    if (dteCollectionFromDate != null && sender.get_selectedDate() < dteCollectionFromDate.get_selectedDate() && rdCollectionBookingWindow != null && rdCollectionBookingWindow.prop("checked") == true)
		        dteCollectionFromDate.set_selectedDate(sender.get_selectedDate());

		    updateDeliveryDateForServiceLevel(serviceLevelCombo);
			
		}

		function dteDeliveryFromDate_SelectedDateChanged(sender, eventArgs) {
		    var dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");

		    if (dteDeliveryByDate != null && sender.get_selectedDate() > dteDeliveryByDate.get_selectedDate())
		        dteDeliveryByDate.set_selectedDate(sender.get_selectedDate());
		}

		function dteDeliveryByDate_SelectedDateChanged(sender, eventArgs) {
		    var dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
		    var rdDeliveryBookingWindow = $("#" + "<%=rdDeliveryBookingWindow.ClientID%>");
		    var updateDate = false;

		    if (dteDeliveryFromDate != null) {
		        if (sender.get_selectedDate() < dteDeliveryFromDate.get_selectedDate())
		            updateDate = true;
		        else if (rdDeliveryBookingWindow != null && rdDeliveryBookingWindow.prop("checked") != true)
		            updateDate = true;
		    }

		    if (updateDate)
		        dteDeliveryFromDate.set_selectedDate(sender.get_selectedDate());
		}

		function updateDeliveryDateForServiceLevel(serviceLevelCombo) {
		    var orderServiceLevelID = -1;
		    var deliveryDate = new Date();
		    var collectionDate = new Date();
		    var noOfDays = null;

		    if(typeof serviceLevelCombo != 'number')
		    {
		        if (serviceLevelCombo != null) orderServiceLevelID = serviceLevelCombo.get_value();
		    }
		    else{
		        orderServiceLevelID = serviceLevelCombo;
		    }

		    var dteCollectionByDate = $find("<%=dteCollectionByDate.ClientID %>");
			if (dteCollectionByDate != null) collectionDate = dteCollectionByDate.get_selectedDate();

			var dteDeliveryFromDate = $find("<%=dteDeliveryFromDate.ClientID %>");
			var dteDeliveryByDate = $find("<%=dteDeliveryByDate.ClientID %>");
		    if (dteDeliveryByDate != null) deliveryDate = dteDeliveryByDate.get_selectedDate();

		    // Gets the number of days set for the specified service level.
		    if (serviceLevelDays != null && orderServiceLevelID != -1)
		        noOfDays = jQuery.grep(serviceLevelDays, function(a) { return a[0] == orderServiceLevelID });

		    if (noOfDays.length > 0 && noOfDays[0][1] != -1) {

		        var selectedNoOfDays = parseInt(noOfDays[0][1], 10);
		        // Calculates the date from collection date with the number of days for that service level.
		        deliveryDate = new Date(collectionDate.getFullYear(), collectionDate.getMonth(), collectionDate.getDate());
		        deliveryDate.setDate(deliveryDate.getDate() + 0);
				
		        var holidayDate = jQuery.grep(invalidDates, function(a) { return a.Date.toLocaleString() == deliveryDate.toLocaleString(); });
		        var dayCounter = 1;
				
		        while (holidayDate.length > 0 || selectedNoOfDays > 0) {
		            deliveryDate = new Date(collectionDate.getFullYear(), collectionDate.getMonth(), collectionDate.getDate());
		            deliveryDate.setDate(deliveryDate.getDate() + dayCounter);
		            holidayDate = jQuery.grep(invalidDates, function(a) { return a.Date.toLocaleString() == deliveryDate.toLocaleString(); });
					
		            if(holidayDate.length == 0)
		                selectedNoOfDays--;
						
		            dayCounter++;
		        }

		        dteDeliveryFromDate.set_selectedDate(deliveryDate);
		        dteDeliveryByDate.set_selectedDate(deliveryDate);
		    }
		}  
			
		function requestStart(sender, eventArgs) {
		}
		
		function responseEnd(sender, eventArgs) {
		    if (cboClientInitiated) {
		        $('select[id*=cboBusinessType]').change(cboBusinessType_change);
		        cboClientInitiated = false;
		        ReRate();
		    }
		}
	   
		function checkPalletInput(sender, eventArgs) {
		    var showPalletAlert = false;

		    if(palletForceTabVisible)
		    {
		        var rntxtPallets = $find('<%=rntxtPallets.ClientID%>');
				
			    var rntPalletForceFullPallets = $find('<%=rntPalletForceFullPallets.ClientID%>');
			    var rntPalletForceHalfPallets = $find('<%=rntPalletForceHalfPallets.ClientID%>');
			    var rntPalletForceQtrPallets = $find('<%=rntPalletForceQtrPallets.ClientID%>');
			    var rntPalletForceOverPallets = $find('<%=rntPalletForceOverPallets.ClientID%>');

			    if (rntxtPallets != null && rntPalletForceFullPallets != null && rntPalletForceHalfPallets != null && rntPalletForceQtrPallets != null && rntPalletForceOverPallets != null) {
			        var palletValue = 0, fullPalletValue = 0, halfPalletValue = 0, qtrPalletValue = 0, osPalletValue = 0;

			        if (rntxtPallets.get_value() != '')
			            palletValue = parseInt(rntxtPallets.get_value(), 10);

			        if (rntPalletForceFullPallets.get_value() != '')
			            fullPalletValue = parseInt(rntPalletForceFullPallets.get_value(), 10);

			        if (rntPalletForceHalfPallets.get_value() != '')
			            halfPalletValue = parseInt(rntPalletForceHalfPallets.get_value(), 10);

			        if (rntPalletForceQtrPallets.get_value() != '')
			            qtrPalletValue = parseInt(rntPalletForceQtrPallets.get_value(), 10);

			        if (rntPalletForceOverPallets.get_value() != '')
			            osPalletValue = parseInt(rntPalletForceOverPallets.get_value(), 10);

			        if ((fullPalletValue + halfPalletValue + qtrPalletValue + osPalletValue) > palletValue)
			            showPalletAlert = true;
			    }
			}
			
            if(showPalletAlert)
                alert("The number of Pallet Force Pallets is greater than the number of Order Pallets");
        }                

        function cboAllocatedTo_SelectedIndexChanged(sender, eventArgs) {
            $("#<%= hidIsManuallyAllocated.ClientID %>").val("<%= true.ToString() %>");
		}

		function cboAllocatedTo_TextChange(sender, eventArgs) {
		    // If the user has typed text that does not match an item in the list then clear the field.
		    var item = sender.get_selectedItem();
		    var text = sender.get_text();
		    if ((item != null && text != item.get_text()) || (item == null && text != "")) {
		        sender.clearSelection();
		        sender.set_text("");
		    }
		}

		function getClientIdentityID() {
		    var cboClient = $find("<%= cboClient.ClientID %>");
		    return cboClient == null ? null : (cboClient.get_value() || null);
		}
		
		function getBusinessTypeID() {
		    var cboBusinessType = $get("<%= cboBusinessType.ClientID %>");
		    if (cboBusinessType.value != '-- Please Select -- ')
		        return cboBusinessType == null ? null : (cboBusinessType.value || null);
		    else
		        return null;

		}
		
		function getCollectionPointID() {
		    var retVal = null;
		    var combo = $find("<%= ucCollectionPoint.ComboClientID %>");
			
			if (combo != null) {
			    // Get the second parameter in the comma delimited list
			    var parts = combo.get_value().split(',');
			    if (parts.length == 2) {
			        retVal = parts[1];
			    }
			}
			
			return retVal;
        }
		
        function getDeliveryPointID() {
            var retVal = null;
            var combo = $find("<%= ucDeliveryPoint.ComboClientID %>");
			
			if (combo != null) {
			    // Get the second parameter in the comma delimited list
			    var parts = combo.get_value().split(',');
			    if (parts.length == 2) {
			        retVal = parts[1];
			    }
			}
			
			return retVal;
        }
		
        function getCollectionDateTime() {
            var retVal = null;
			
            var dteCollectionFromDate = $find("<%= dteCollectionFromDate.ClientID %>");
			var dteCollectionFromTime = $find("<%= dteCollectionFromTime.ClientID %>");
			
		    if (dteCollectionFromDate != null && dteCollectionFromTime != null) {
		        var date = dteCollectionFromDate.get_dateInput().get_selectedDate();
				
		        if (date != null) {
		            retVal = new Date();
		            retVal.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());
					
		            var time = dteCollectionFromTime.get_dateInput().get_selectedDate();
		            if (time == null) {
		                retVal.setHours(23, 59, 59, 0); // Anytime is specified
		            }
		            else {
		                retVal.setHours(time.getHours(), time.getMinutes(), 0, 0);
		            }
		        }
		    }
			
		    return retVal;
		}

		function getDeliveryDateTime() {
		    var retVal = null;
			
		    var dteDeliveryByDate = $find("<%= dteDeliveryByDate.ClientID %>");
			var dteDeliveryByTime = $find("<%= dteDeliveryByTime.ClientID %>");
			
		    if (dteDeliveryByDate != null && dteDeliveryByTime != null) {
		        var date = dteDeliveryByDate.get_selectedDate();
				
		        if (date != null) {
		            retVal = new Date();
		            retVal.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());

		            var time = dteDeliveryByTime.get_selectedDate();
		            if (time == null) {
		                retVal.setHours(23, 59, 59, 0); // Anytime is specified
		            }
		            else {
		                retVal.setHours(time.getHours(), time.getMinutes(), 0, 0);
		            }
		        }
		    }
			
		    return retVal;
		}

		function signatureLink_click() {
		    var tooltip = $find('<%= radToolTip.ClientID %>');
	        tooltip.set_targetControl(this);

	        var data = $(this).data();
	        data.imageBaseUrl = '<%= this.SignatureImageBaseUrl %>';

	        if (data.comment == 'Signed') {
	            data.comment = '';
	        }

	        var compiledTemplate = getTemplate('signature');
	        var html = compiledTemplate(data);

	        tooltip.set_content(html);
	        tooltip.show();
        }


	    function OpenDriverCheckIn()
	    {
	        var orderID = "<%= OrderID %>";
	        var collectionDate = getCollectionDateTime();
	        var deliveryDate = getDeliveryDateTime();
	        if(collectionDate == null) collectionDate = "";
	        else collectionDate = collectionDate.getDate() + "/" + (collectionDate.getMonth() + 1) + "/" + collectionDate.getFullYear();
	        if(deliveryDate == null) deliveryDate = "";
	        else deliveryDate = deliveryDate.getDate() + "/" + (deliveryDate.getMonth() + 1) + "/" + deliveryDate.getFullYear();
	        window.open("/POD/outstandingPods.aspx?orderID=" + orderID + "&startDate=" + collectionDate + "&endDate=" + deliveryDate);
	    }

	</script>
</telerik:RadCodeBlock>

<telerik:RadToolTip runat="server" ID="radToolTip" EnableShadow="true" HideEvent="LeaveTargetAndToolTip"
    ShowEvent="FromCode" Width="500px" RelativeTo="Mouse" Position="MiddleLeft" MouseTrailing="true"
    ShowCallout="false">
</telerik:RadToolTip>

<telerik:RadAjaxManager ID="raxManager" runat="server">
    <ClientEvents OnRequestStart="requestStart" OnResponseEnd="responseEnd" />
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="cboClient">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="lblMissingDocumentsAlert" />
                <telerik:AjaxUpdatedControl ControlID="chkCreateJob" />
                <telerik:AjaxUpdatedControl ControlID="chkOverrideFuelSurcharge" />

                <telerik:AjaxUpdatedControl ControlID="cboPalletType" />
                <telerik:AjaxUpdatedControl ControlID="cboBusinessType" />
                <telerik:AjaxUpdatedControl ControlID="cboGoodsType" />
                <telerik:AjaxUpdatedControl ControlID="cboService" />

                <telerik:AjaxUpdatedControl ControlID="rntxtPallets" />
                <telerik:AjaxUpdatedControl ControlID="rntxtPalletSpaces" />
                <telerik:AjaxUpdatedControl ControlID="rntOrderRate" />
                <telerik:AjaxUpdatedControl ControlID="rntFuelSurchargePercentage" />

                <telerik:AjaxUpdatedControl ControlID="grdExtras" />

                <telerik:AjaxUpdatedControl ControlID="hidExchangeRate" />
                <telerik:AjaxUpdatedControl ControlID="hidIsForeign" />
                <telerik:AjaxUpdatedControl ControlID="hidGlobalFuelSurchargeAppliesToExtras" />
                <telerik:AjaxUpdatedControl ControlID="hidFuelSurchargeRestoreValue" />
                <telerik:AjaxUpdatedControl ControlID="hidClientRateTariffDescription" />

                <telerik:AjaxUpdatedControl ControlID="pnlReferences" />
                <telerik:AjaxUpdatedControl ControlID="pnlSetFocus" />

                <telerik:AjaxUpdatedControl ControlID="txtRateTariffCard" />

                <telerik:AjaxUpdatedControl ControlID="lblFuelSurcharge" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>
