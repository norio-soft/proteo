<%@ Page Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="AddUpdatePalletForceInvoice.aspx.cs" Inherits="Orchestrator.WebUI.Invoicing.AddUpdatePalletForceInvoice" Title="Haulier Enterprise" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="dlg" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" src="/script/jquery-ui-min.js"></script>
    <script type="text/javascript" src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script type="text/javascript" src="/script/jquery.ajaxupload.3.6.js"></script>
    <script type="text/javascript" src="AddUpdatePalletForceInvoice.aspx.js"></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Add/Update PalletForce Invoice</h1></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <dlg:Dialog ID="dlgOrder" runat="server" URL="/Groupage/ManageOrder.aspx" Width="1080" Height="900" AutoPostBack="false" ReturnValueExpected="false" Mode="Modal" />
    <dlg:Dialog ID="dlgMatchOrder" runat="server" URL="MatchOrder.aspx" Width="800" Height="600" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />

    <div style="float:left; display:none;" id="UploadedFileDetails" runat="server">
        <div style="float:left;">
            <h3>Uploaded File Details</h3>
            <table>
                <tr>
                    <td class="formCellLabel">File Name</td>
                    <td class="formCellField"><asp:Label ID="lblFileName" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Invoice Type</td>
                    <td class="formCellField"><asp:Label ID="lblInvoiceType" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Invoice No</td>
                    <td class="formCellField"><asp:Label ID="lblInvoiceNo" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Invoice Date</td>
                    <td class="formCellField"><asp:Label ID="lblInvoiceDate" runat="server" /></td>
                </tr>
            </table>
        </div>
        <div style="float:left;">
            <h3>Summary Details</h3>
            <table>
                <tr>
                    <td class="formCellLabel">Invoice Items</td>
                    <td class="formCellField"><asp:Label ID="lblInvoiceItems" runat="server" /></td>
                    
                </tr>
                <tr>
                    <td class="formCellLabel">Unmatched Items</td>
                    <td class="formCellField"><asp:Label ID="lblMatchedItems" runat="server" /></td>
                </tr>                
                <tr>
                    <td class="formCellLabel">Orchestrator Item Total</td>
                    <td class="formCellField"><asp:Label ID="lblSystemItemTotal" runat="server" /></td>
                </tr>
                <tr>
                    <td class="formCellLabel">Difference Value</td>
                    <td class="formCellField"><asp:Label ID="lblDifferenceValue" runat="server" /></td>
                </tr>
            </table>
        </div>
    </div>
    
    <div style="float:left; widows:300px;" id="UploadInvoiceFile" runat="server">
        <h3>Submit For Processing</h3>
        <table>
            <tr>
                <td class="formCellLabel" style="vertical-align:middle;">File</td>
                <td class="formCellField">
                    <input type="text" disabled="disabled" id="inpFileName" />
                    <input type="button" value="Browse..." id="btnChoose" />
                    <input type="button" id="btnUpload" title="Upload" value="Upload" onclick="javascript:btnSubmit_OnClientAdded();" />
                </td>
            </tr>
        </table>
    </div>
    
    <div class="clearDiv"></div>
    
    <div class="buttonBar" style="margin: 10px 0px 5px 0px;">
        <asp:Panel ID="pnlTopButtonBar" runat="server" >
            <asp:Button ID="btnApprove" runat="server" Text="Approve" OnClientClick="if(!approvePreInvoice()) return false;" />
            <asp:Button ID="btnRemove" runat="server" Text="Remove" OnClientClick="if(!btnRemove_OnClick()) return false;" />
            <asp:Button ID="btnBack" runat="server" Text="Back" />
        </asp:Panel>
    </div>
    
    <div id="tabs">
        <ul>
            <li><a href="#tabs-1">Invoice Items</a></li>
            <li><a href="#tabs-2"><span id="spnUnMatchedTitle">Unmatched Items</span></a></li>
        </ul>
        <div id="tabs-1">
            <div style="margin:5px 0px 10px 0px;">
                <h3>Matched Items</h3>
                <asp:ListView ID="lvPreInvoiceItems" runat="server">
                    <LayoutTemplate>
                        <div class="listViewGrid">
                            <table id="orders" cellpadding="0" cellspacing="0">
                                <thead>
                                    <tr class="HeadingRow">
                                        <th class="first"></th>
                                        <th>OrderID</th>
                                        <th>JobID</th>
                                        <th>Depot</th>
                                        <th>Date</th>
                                        <th>Full</th>
                                        <th>Half</th>
                                        <th>Qtr</th>
                                        <th>O/S</th>
                                        <th>Weight</th>
                                        <th>Service</th>
                                        <th>Surcharge</th>
                                        <th>PostCode</th>
                                        <th>PostTown</th>
                                        <th>Cost</th>
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
                            <td class="first">
                                <img src="/images/lines/plus.gif" alt='Group: <%# Eval("OrderID")%>' onclick="toggleGroup(this, '<%# Eval("OrderCount") %>');" />
                            </td>
                            <th colspan="14"><%# Eval("OrderID")%> <%# ((decimal)Eval("DifferenceAmount")) > 0 ? "Difference Amount : " + ((decimal)Eval("DifferenceAmount")).ToString("C", GetCulture(2057)) : "" %> </th>
                        </tr>
                        <asp:ListView ID="lvItems" runat="server" DataSource='<%# Eval("Items") %>' >
                            <LayoutTemplate>
                                <tr runat="server" id="itemPlaceHolder" />
                            </LayoutTemplate>
                            <ItemTemplate>
                                <tr id="row" runat="server" class="Row hidden">
                                    <td class="first"></td>
                                    <td><%# string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["PFInvoiceItemID"].ToString()) ? "<a id=\"orderRef\" href=\"javascript:viewOrderProfile(" + ((System.Data.DataRow)Container.DataItem)["OrderID"].ToString() +");\">" + ((System.Data.DataRow)Container.DataItem)["OrderID"].ToString() + "</a>" : "Imported" %></td>
                                    <td>&nbsp;</td>
                                    <td><%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["DeliveryDepot"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["DeliveryDepot"].ToString() : "&nbsp;" %></td>
                                    <td><%# ((DateTime)((System.Data.DataRow)Container.DataItem)["DeliveryDate"]).ToString("dd/MM/yyyy")%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["FullPallets"].ToString()%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["HalfPallets"].ToString()%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["QtrPallets"].ToString()%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["OSPallets"].ToString()%></td>
                                    <td><%# ((decimal)((System.Data.DataRow)Container.DataItem)["Weight"]).ToString("F2")%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["PFServiceLevel"].ToString()%></td>
                                    <td><%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["PFSurcharge"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["PFSurcharge"].ToString() : "&nbsp;" %></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["PostCode"].ToString()%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["PostTown"].ToString()%></td>
                                    <td><%# ((System.Data.DataRow)Container.DataItem)["Cost"] != DBNull.Value ? ((decimal?)((System.Data.DataRow)Container.DataItem)["Cost"]).Value.ToString("C", GetCulture(((int?)((System.Data.DataRow)Container.DataItem)["LCID"]))) : "&nbsp;" %></td>
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
        <div id="tabs-2">
            <div style="margin:5px 0px 10px 0px;">
                <h3>Unmatched Items</h3>
                <asp:ListView ID="lvPreInvoiceUnMatchedItems" runat="server">
                    <LayoutTemplate>
                        <div class="listViewGrid">
                            <table id="orders" cellpadding="0" cellspacing="0">
                                <thead>
                                    <tr class="HeadingRow">
                                        <th></th>
                                        <th>Consignment No</th>
                                        <th>Ref 2</th>
                                        <th>Depot</th>
                                        <th>Date</th>
                                        <th>Full</th>
                                        <th>Half</th>
                                        <th>Qtr</th>
                                        <th>O/S</th>
                                        <th>Weight</th>
                                        <th>Service</th>
                                        <th>Surcharge</th>
                                        <th>PostCode</th>
                                        <th>PostTown</th>
                                        <th>Cost</th>
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
                            <td><%# "<a href=\"javascript:viewMatchOrder('" + PreInvoiceID + "','" + ((System.Data.DataRow)Container.DataItem)["PFInvoiceItemID"].ToString() + "','" + ((System.Data.DataRow)Container.DataItem)["PFRef1"].ToString() + "')\">Find Order</a>"%></td>
                            <td><%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["PFRef1"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["PFRef1"].ToString() : "&nbsp;" %></td>
                            <td><%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["PFRef2"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["PFRef2"].ToString() : "&nbsp;" %></td>
                            <td><%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["DeliveryDepot"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["DeliveryDepot"].ToString() : "&nbsp;" %></td>
                            <td><%# ((DateTime)((System.Data.DataRow)Container.DataItem)["DeliveryDate"]).ToString("dd/MM/yyyy")%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["FullPallets"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["HalfPallets"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["QtrPallets"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["OSPallets"].ToString()%></td>
                            <td><%# ((decimal)((System.Data.DataRow)Container.DataItem)["Weight"]).ToString("F2")%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["PFServiceLevel"].ToString()%></td>
                            <td><%# !string.IsNullOrEmpty(((System.Data.DataRow)Container.DataItem)["PFSurcharge"].ToString()) ? ((System.Data.DataRow)Container.DataItem)["PFSurcharge"].ToString() : "&nbsp;" %></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["PostCode"].ToString()%></td>
                            <td><%# ((System.Data.DataRow)Container.DataItem)["PostTown"].ToString()%></td>
                            <td><%# ((decimal)((System.Data.DataRow)Container.DataItem)["Cost"]).ToString("C", GetCulture(((int?)((System.Data.DataRow)Container.DataItem)["LCID"])))%></td>
                        </tr>
                    </ItemTemplate>
                    <EmptyDataTemplate>
                        <div>
                            There are no records to display
                        </div>
                    </EmptyDataTemplate>
                </asp:ListView>
            </div>
        </div>
    </div>
    
    <div class="buttonBar" style="margin-top:10px;">
        <asp:Panel ID="pnlBottomPanelBar" runat="server">
            <asp:Button ID="btnApprove1" runat="server" Text="Approve" OnClientClick="if(!approvePreInvoice()) return false;" />
            <asp:Button ID="btnRemove1" runat="server" Text="Cancel" OnClientClick="if(!btnRemove_OnClick()) return false;" />
            <asp:Button ID="btnBack1" runat="server" Text="Back" />
        </asp:Panel>
    </div>
    
    <div>
        <asp:Button ID="hdnRemoveButton" runat="server" style="display:none;" />
        <asp:Button ID="hdnApproveButton" runat="server" style="display:none;" />
        <asp:Label ID="lblUnMatchedItems" runat="server" style="display:none;" />
    </div>
    
    <telerik:RadCodeBlock ID="rcbPreInvoice" runat="server" >
    
        <telerik:RadAjaxManager ID="ramPreInvoice" runat="server" ClientEvents-OnResponseEnd="ramPreInvoiceAjaxRequest_OnResponseEnd">
            <AjaxSettings>
                <telerik:AjaxSetting AjaxControlID="ramPreInvoice">
                    <UpdatedControls>
                        <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceItems" />
                        <telerik:AjaxUpdatedControl ControlID="lvPreInvoiceUnMatchedItems" />
                        <telerik:AjaxUpdatedControl ControlID="UploadedFileDetails" />
                        <telerik:AjaxUpdatedControl ControlID="pnlTopButtonBar" />
                        <telerik:AjaxUpdatedControl ControlID="pnlBottomPanelBar" />
                        <telerik:AjaxUpdatedControl ControlID="lblUnMatchedItems" />
                    </UpdatedControls>
                </telerik:AjaxSetting>
            </AjaxSettings>
        </telerik:RadAjaxManager>
    
        <script type="text/javascript" language="javascript">
            /*<![CDATA[*/
            var UploadInvoiceFile = $("#" + "<%=UploadInvoiceFile.ClientID %>");
            var UploadedFileDetails = $("#" + "<%=UploadedFileDetails.ClientID %>");
            var hdnRemoveButton = document.getElementById("<%=hdnRemoveButton.ClientID %>");
            var hdnApproveButton = document.getElementById("<%=hdnApproveButton.ClientID %>");
            var fileToUpload = null;
            var btnChoose = $("#btnChoose");
            var txtFileName = $("#inpFileName");
            var table = $get('orders');
            var unMatchedItemsID = "<%=lblUnMatchedItems.ClientID %>";
            
            function createPalletForcePreInvoice_Success(result) {
                var parts = result.toString().split("|");

                if (parts.length == 1) {
                    updateLoadingMessage('Loading Grid');
                    fileToUpload = null;
                    var ramPreInvoice = $find("<%=ramPreInvoice.ClientID %>");
                    ramPreInvoice.ajaxRequest();
                }
                else {
                    var errorCodeID = parseInt(parts[0], 10);

                    switch (errorCodeID) {
                        case 1:
                            var errorMessage = "";
                            if (parts[1].length > 1);
                                errorMessage = " Error Content : " + parts[1];
                            alert("There was an error uploading the file." + errorMessage);
                            //alert("There was an error creating the Preinvoice, please check none of the items have not already been invoiced");
                            break;
                        default:
                            break;
                    }

                    enableProcessing();
                    hideLoading();
                }
            }
            
            function viewOrderProfile(orderID) {
                var qs = "oID=" + orderID;
                <%=dlgOrder.ClientID %>_Open(qs);
            }
            
            function viewMatchOrder(preInvoiceID, pfInvoiceItemID, consignmentNo)
            {
                var qs = "piID=" + preInvoiceID + "&pfID=" + pfInvoiceItemID + "&cNo=" + consignmentNo;
                <%=dlgMatchOrder.ClientID %>_Open(qs);
            }
            
            /*]]>*/
        </script>
        
    </telerik:RadCodeBlock>

</asp:Content>