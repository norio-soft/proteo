<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="InvoicingImport.aspx.cs" Inherits="Orchestrator.WebUI.Invoicing.InvoicingImport"
    Title="Haulier Enterprise" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="dlg" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>

    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>

    <script type="text/javascript" src="/script/jquery.ajaxupload.3.6.js"></script>

    <script type="text/javascript" src="InvoicingImport.aspx.js"></script>

</asp:Content>
<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Invoicing Import</h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <dlg:Dialog ID="dlgOrder" runat="server" URL="/Groupage/ManageOrder.aspx" Width="1080"
        Height="900" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    <dlg:Dialog ID="dlgMatchOrder" runat="server" URL="MatchOrder.aspx" Width="800" Height="600"
        AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    <div style="float: left;" id="UploadedFileDetails" runat="server">
        <div style="float: left;">
            <h3>
                Imported File Details</h3>
            <table style="text-align: left;" width="65%">
                <tr>
                    <td class="formCellLabel" style="width: 5%;">
                        File:
                    </td>
                    <td class="formCellField" style="width: 15%;">
                        <telerik:RadComboBox ID="cboFile" AutoPostBack="true" Overlay="true" SelectOnTab="false"
                            runat="server" EnableLoadOnDemand="false" AllowCustomText="false" ShowMoreResultsBox="false"
                            MarkFirstMatch="false" ItemRequestTimeout="500" Width="400px" Height="300px" TabIndex="1" CausesValidation="false">
                        </telerik:RadComboBox>
                    </td>
                    <td class="formCellLabel" style="width: 5%;">
                        Imported From:
                    </td>
                    <td class="formCellField" style="width: 5%;">
                        <telerik:RadDateInput ID="dteImportFromDate" runat="server" DateFormat="dd/MM/yy"
                            ToolTip="The imported from date for the file filter">
                        </telerik:RadDateInput>
                    </td>
                    <td class="formCellLabel" style="width: 5%;">
                        Imported To:
                    </td>
                    <td class="formCellField" style="width: 5%;">
                        <telerik:RadDateInput ID="dteImportToDate" runat="server" DateFormat="dd/MM/yy" ToolTip="The imported tod date for the file filter">
                        </telerik:RadDateInput>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Folder Name
                    </td>
                    <td class="formCellField">
                        <asp:Label ID="lblFromSystem" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Batch Ref
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtBatchRef" runat="server" />
                        <asp:RequiredFieldValidator ID="rfvBatchRef" runat="server" ControlToValidate="txtBatchRef"
                            Display="Dynamic" ErrorMessage="Please enter a batch ref."><img src="../../images/error.gif" alt="Please enter a batch ref." /></asp:RequiredFieldValidator>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="clearDiv">
    </div>
    <div class="buttonBar" style="margin: 10px 0px 5px 0px;">
        <asp:Panel ID="pnlTopButtonBar" runat="server">
            <asp:Button ID="btnRefreshFiles" runat="server" Text="Refresh Files" CausesValidation="false"/>
            <asp:Button ID="btnDelete" runat="server" Text="Delete File" CausesValidation="false" OnClientClick='return confirm("Are you sure you wish to delete the selected file and its contents?");' />
        </asp:Panel>
    </div>
    <div class="buttonBar" style="margin: 10px 0px 5px 0px;">
        <asp:Panel ID="Panel1" runat="server">
            <asp:Button ID="btnRefreshInvoices" runat="server" Text="Refresh Invoices" CausesValidation="false" />
            <asp:Button ID="btnExport" runat="server" Text="Export" />
            <asp:Button ID="btnCreatePreInvoice" runat="server" Text="Create Pre Invoice" />
        </asp:Panel>
    </div>
    <fieldset>
        <table>
            <tr>
                <td>
                    <img align="middle" id="imgExpColImportedInvoices" alt="Expand-Collapse" src="/images/topItem_col.gif"
                        onclick="javascript: showHideDiv('divImportedInvoices','imgExpColImportedInvoices', 'images/topItem_col.gif','images/topItem_exp.gif');" />
                </td>
                <td>
                    <h3>
                        Imported Invoice Items</h3>
                </td>
            </tr>
        </table>
        <div id="divImportedInvoices" style="display: inline;">
            <div style="margin: 5px 0px 10px 0px;">
                <div>
                    Business Type
                    <asp:DropDownList ID="cboBusinessType" runat="server" AutoPostBack="true" DataValueField="BusinessTypeID"
                        DataTextField="Description">
                    </asp:DropDownList>
                    <asp:Label ID="lblStatus" Test="" runat="server" ForeColor="Red"></asp:Label>
                    <br />
                </div>
                <br />
                <asp:ListView ID="lvPreInvoiceItems" runat="server">
                    <LayoutTemplate>
                        <div class="listViewGrid">
                            <table id="orders" cellpadding="0" cellspacing="0">
                                <thead>
                                    <tr align="left" class="HeadingRow">
                                        <th class="first">
                                        </th>
                                        <th>
                                            <input type="checkbox" id="chkExcludeAll" runat="server" onclick="javascript:HandleSelectAll(this);" />Excl
                                        </th>
                                        <th>
                                            ?
                                        </th>
                                        <th>
                                            Business Type
                                        </th>
                                        <th>
                                            Order Status
                                        </th>
                                        <th>
                                            OrderID
                                        </th>
                                        <th>
                                            Docket No
                                        </th>
                                        <th>
                                            Load No
                                        </th>
                                        <th>
                                            Shipment
                                        </th>
                                        <th>
                                            Weight
                                        </th>
                                        <th>
                                            Collection Date
                                        </th>
                                        <th>
                                            Delivery Description
                                        </th>
                                        <th>
                                            Delivery Post code
                                        </th>
                                        <th>
                                            Delivery Date
                                        </th>
                                        <th>
                                            Cost
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr id="itemPlaceHolder" runat="server" />
                                </tbody>
                            </table>
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="row" runat="server" class="group">
                            <td id="Td1" class="first" runat="server">
                                <img id="imgExpand" src="/images/lines/minus.gif" alt='Group: <%# Eval("OrderID")%>'
                                    onclick="toggleGroup(this, '<%# Eval("OrderCount") %>');" />
                            </td>
                            <td id="Td2" class="first" runat="server">
                                <asp:CheckBox ID="chkExclude" runat="server" Checked="false" />
                            </td>
                            <th colspan="14">
                                <%# (Convert.ToInt32(Eval("OrderID")) == -1) ? "" : "OrderId - " + Eval("OrderID") + "   "%>
                                <%# Eval("Ref1") != null && !String.IsNullOrEmpty(Eval("Ref1").ToString()) ? "Ref1  -  " + Eval("Ref1").ToString().Trim() + "   " : String.Empty %>
                                <%# Eval("Ref2") != null && !String.IsNullOrEmpty(Eval("Ref2").ToString()) ? "Ref2  -  " + Eval("Ref2").ToString().Trim() + "   " : String.Empty%>
                                <%# Eval("Ref3") != null && !String.IsNullOrEmpty(Eval("Ref3").ToString()) ? "Ref3  -  " + Eval("Ref3").ToString().Trim() + "   " : String.Empty%>
                                <%# Convert.ToDecimal(Eval("DifferenceAmount")) != 0 ? "Cost Difference  -  " + Convert.ToDecimal(Eval("DifferenceAmount")).ToString("C", GetCulture(2057)) : ""%>
                                <%# Convert.ToDecimal(Eval("DifferenceWeightAmount")) != 0 ? "Weight Difference  -  " + Convert.ToDecimal(Eval("DifferenceWeightAmount")).ToString() : ""%>
                            </th>
                        </tr>
                        <asp:ListView ID="lvItems" runat="server" DataSource='<%# Eval("Items") %>' OnItemDataBound="lvItems_ItemDataBound">
                            <LayoutTemplate>
                                <tr runat="server" id="itemPlaceHolder" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr id="row" runat="server" class="Row">
                                    <td class="first">
                                    </td>
                                    <td class="first">
                                    </td>
                                    <td id="StatusCell" runat="server">
                                        -
                                    </td>
                                    <td id="BusinessTypeCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["BusinessType"].ToString()%>
                                    </td>
                                    <td id="OrderStatusCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["OrderStatus"].ToString()%>
                                    </td>
                                    <td id="OrderIdsCell" runat="server">
                                        <a id="lnkOrder" runat="server"></a>
                                    </td>
                                    <td id="Ref1sCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["Ref1"].ToString() %>
                                    </td>
                                    <td id="Ref2sCell" runat="server">
                                        <%# (((System.Data.DataRow)Container.DataItem)["Ref2"]).ToString()%>
                                    </td>
                                    <td id="Ref3sCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["Ref3"].ToString()%>
                                    </td>
                                    <td id="WeightsCell" runat="server">
                                        <%# ((decimal)((System.Data.DataRow)Container.DataItem)["Weight"]).ToString("F2")%>
                                    </td>
                                    <td id="CollectionDatesCell" runat="server">
                                        <%#  ((System.Data.DataRow)Container.DataItem)["CollectionDate"] != DBNull.Value ? ((DateTime)((System.Data.DataRow)Container.DataItem)["CollectionDate"]).ToString("dd/MM/yyyy") : "&nbsp;"%>
                                    </td>
                                    <td id="DeliveryDescriptionsCell" runat="server">
                                        <%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["DeliveryDescription"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["DeliveryDescription"].ToString() : "&nbsp;"%>
                                    </td>
                                    <td id="DeliveryPostCodesCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["DeliveryPostCode"].ToString()%>
                                    </td>
                                    <td id="DeliveryDatesCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["DeliveryDate"] != DBNull.Value ? ((DateTime)((System.Data.DataRow)Container.DataItem)["DeliveryDate"]).ToString("dd/MM/yyyy") : "&nbsp;"%>
                                    </td>
                                    <td id="ChargesCell" runat="server">
                                        <%# ((System.Data.DataRow)Container.DataItem)["Charge"] != DBNull.Value ? ((decimal?)((System.Data.DataRow)Container.DataItem)["Charge"]).Value.ToString("C", GetCulture(((int?)((System.Data.DataRow)Container.DataItem)["LCID"]))) : "&nbsp;"%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:ListView>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div>
                            There are no records to display
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </div>
    </fieldset>
    <fieldset>
        <table>
            <tr>
                <td>
                    <img align="middle" id="imgExpColAdditionalOrders" alt="Expand-Collapse" src="/images/topItem_col.gif"
                        onclick="javascript: showHideDiv('divAdditionalOrders','imgExpColAdditionalOrders', 'images/topItem_col.gif','images/topItem_exp.gif');" />
                </td>
                <td>
                    <h3>
                        Additional orders not included in the file with a matching load number</h3>
                </td>
            </tr>
        </table>
        <div id="divAdditionalOrders" style="display: inline;">
            <div style="margin: 5px 0px 10px 0px;">
                <asp:ListView ID="lvAdditional" runat="server">
                    <LayoutTemplate>
                        <div class="listViewGrid">
                            <table id="orders" cellpadding="0" cellspacing="0">
                                <thead>
                                    <tr align="left" class="HeadingRow">
                                        <th class="first">
                                        </th>
                                        <th>
                                            <input type="checkbox" id="chkExcludeAllAdditional" runat="server" onclick="javascript:HandleSelectAllAdditional(this);" />Excl
                                        </th>
                                        <th>
                                            ?
                                        </th>
                                        <th>
                                            Business Type
                                        </th>
                                        <th>
                                            Order Status
                                        </th>
                                        <th>
                                            OrderID
                                        </th>
                                        <th>
                                            Docket No
                                        </th>
                                        <th>
                                            Load No
                                        </th>
                                        <th>
                                            Shipment
                                        </th>
                                        <th>
                                            Weight
                                        </th>
                                        <th>
                                            Collection Date
                                        </th>
                                        <th>
                                            Delivery Description
                                        </th>
                                        <th>
                                            Delivery Post code
                                        </th>
                                        <th>
                                            Delivery Date
                                        </th>
                                        <th>
                                            Cost
                                        </th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr id="itemPlaceHolder" runat="server" />
                                </tbody>
                            </table>
                        </div>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <tr id="row" runat="server" class="Row">
                            <td class="first">
                            </td>
                            <td id="chkRowAdditional" runat="server" align="left">
                                <asp:CheckBox ID="chkExcludeAdditional" runat="server" Checked="false" />
                            </td>
                            <td id="StatusCell" runat="server">
                            </td>
                            <td id="BusinessTypeCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["BusinessType"].ToString()%>
                            </td>
                            <td id="OrderStatusCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["OrderStatus"].ToString()%>
                            </td>
                            <td id="OrderIdsCell" runat="server">
                                <a href="javascript:viewOrderProfile(<%# ((System.Data.DataRow)Container.DataItem)["OrderId"].ToString() %>)">
                                    <%# ((System.Data.DataRow)Container.DataItem)["OrderId"].ToString() %></a>
                            </td>
                            <td id="Ref1sCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["Ref1"].ToString() %>
                            </td>
                            <td id="Ref2sCell" runat="server">
                                <%# (((System.Data.DataRow)Container.DataItem)["Ref2"]).ToString()%>
                            </td>
                            <td id="Ref3sCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["Ref3"].ToString()%>
                            </td>
                            <td id="WeightsCell" runat="server">
                                <%# ((decimal)((System.Data.DataRow)Container.DataItem)["Weight"]).ToString("F2")%>
                            </td>
                            <td id="CollectionDatesCell" runat="server">
                                <%#  ((System.Data.DataRow)Container.DataItem)["CollectionDate"] != DBNull.Value ? ((DateTime)((System.Data.DataRow)Container.DataItem)["CollectionDate"]).ToString("dd/MM/yyyy") : "&nbsp;"%>
                            </td>
                            <td id="DeliveryDescriptionsCell" runat="server">
                                <%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["DeliveryDescription"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["DeliveryDescription"].ToString() : "&nbsp;"%>
                            </td>
                            <td id="DeliveryPostCodesCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["DeliveryPostCode"].ToString()%>
                            </td>
                            <td id="DeliveryDatesCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["DeliveryDate"] != DBNull.Value ? ((DateTime)((System.Data.DataRow)Container.DataItem)["DeliveryDate"]).ToString("dd/MM/yyyy") : "&nbsp;"%>
                            </td>
                            <td id="ChargesCell" runat="server">
                                <%# ((System.Data.DataRow)Container.DataItem)["Charge"] != DBNull.Value ? ((decimal?)((System.Data.DataRow)Container.DataItem)["Charge"]).Value.ToString("C", GetCulture(((int?)((System.Data.DataRow)Container.DataItem)["LCID"]))) : "&nbsp;"%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:ListView>
            </div>
        </div>
    </fieldset>
    <div class="buttonBar" style="margin: 10px 0px 5px 0px;">
        <asp:Panel ID="Panel2" runat="server">
            <asp:Button ID="btnRefreshInvoiceBottom" runat="server" Text="Refresh Invoices" CausesValidation="false" />
            <asp:Button ID="btnExportBottom" runat="server" Text="Export" />
            <asp:Button ID="btnCreatePreInvoiceBottom" runat="server" Text="Create Pre Invoice" />
        </asp:Panel>
    </div>
    <telerik:RadCodeBlock ID="rcbPreInvoice" runat="server">
        <%--        <telerik:RadAjaxManager ID="ramPreInvoice" runat="server" ClientEvents-OnResponseEnd="ramPreInvoiceAjaxRequest_OnResponseEnd">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="ramPreInvoice">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceItems" />
                        <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceUnMatchedItems" />
                        <telerik:AjaxUpdatedControl ControlID="UploadedFileDetails" />
                        <telerik:AjaxUpdatedControl ControlID="pnlTopButtonBar" />
                        <telerik:AjaxUpdatedControl ControlID="pnlBottomPanelBar" />
                        <telerik:AjaxUpdatedControl ControlID="lblUnMatchedItems" />
                        <telerik:AjaxUpdatedControl ControlID="cboFile" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
                <telerik:AjaxSetting AjaxControlID="cboFile">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="cboFile" />
                        <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceItems" />
                        <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceUnMatchedItems" />
                        <telerik:AjaxUpdatedControl ControlID="UploadedFileDetails" />
                        <telerik:AjaxUpdatedControl ControlID="pnlTopButtonBar" />
                        <telerik:AjaxUpdatedControl ControlID="pnlBottomPanelBar" />
                        <telerik:AjaxUpdatedControl ControlID="lblUnMatchedItems" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>--%>

        <script type="text/javascript" language="javascript">
            /*<![CDATA[*/
            var btnChoose = $("#btnChoose");
            var table = $get('orders');

            
//            function createPreInvoice_Success(result) {
//                var parts = result.toString().split("|");

//                if (parts.length == 1) {
//                    updateLoadingMessage('Loading Grid');
//                    fileToUpload = null;
//                    var ramPreInvoice = $find("ramPreInvoice.ClientID ");
//                    ramPreInvoice.ajaxRequest();
//                }
//                else {
//                    var errorCodeID = parseInt(parts[0], 10);

//                    switch (errorCodeID) {
//                        case 1:
//                            var errorMessage = "";
//                            if (parts[1].length > 1);
//                                errorMessage = " Error Content : " + parts[1];
//                            alert("There was an error uploading the file." + errorMessage);
//                            //alert("There was an error creating the Preinvoice, please check none of the items have not already been invoiced");
//                            break;
//                        default:
//                            break;
//                    }

//                    enableProcessing();
//                    hideLoading();
//                }
//            }
            
            function viewOrderProfile(orderID) {
                var qs = "oID=" + orderID;
                <%=dlgOrder.ClientID %>_Open(qs);
            }
            
            function matchOrder(importedInvoiceItemId, loadNo)
            {
                var qs = "importedInvoiceItemID=" + importedInvoiceItemId + "&loadNo=" +loadNo ;
                <%=dlgMatchOrder.ClientID %>_Open(qs);
            }
            
            /*]]>*/
        </script>

    </telerik:RadCodeBlock>
</asp:Content>
