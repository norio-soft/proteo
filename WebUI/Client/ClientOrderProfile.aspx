<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientOrderProfile.aspx.cs"
    Inherits="Orchestrator.WebUI.Client.ClientOrderProfile" MasterPageFile="~/WizardMasterPage.Master"
    Title="Order Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript" src="../script/scripts.js"></script>

    <script language="javascript" type="text/javascript" src="../script/tooltippopups.js"></script>

    <h1>
        <asp:Label runat="server" Font-Size="Large" ID="lblOrderHeading"></asp:Label></h1>
    <table width="100%" cellpadding="2px">
        <tr valign="top">
            <td>
                <div style="border-bottom: solid 1pt silver; padding: 4px; color: #ffffff; background-color: #99BEDE;
                    text-align: left;">
                    <asp:Button ID="btnPIL2" runat="server" Text="Pallet Identification Labels" Width="160px"
                        Height="25" />
                    <asp:Button ID="btnCreateDeliveryNote2" runat="server" Text="Delivery Note" Height="25" />
                    <input type="button" id="btnClose2" value="Close Window" onclick="CloseOnReload()"
                        style="width: 125px; height: 25px;" />
                </div>
                <div style="height: 10px;">
                </div>
                <h2>
                    Order Details</h2>
                <table class="form-value" width="100%" style="font-size: 12px;" cellspacing="2px"
                    cellpadding="3px">
                    <tr>
                        <td>
                            Business Type
                        </td>
                        <td>
                            <asp:Label ID="lblBusinessType" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Order Service Level
                        </td>
                        <td>
                            <asp:Label ID="lblOrderServiceLevel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 18px">
                            Order Status
                        </td>
                        <td style="height: 18px">
                            <b>
                                <asp:Label ID="lblOrderStatus" runat="server"></asp:Label></b>
                        </td>
                    </tr>
                    <tr id="trInvoiceId" runat="server">
                        <td>
                            Invoice ID
                        </td>
                        <td>
                            <asp:HyperLink ID="lblInvoiceNumber" runat="server" Target="_blank" Text="View" />
                        </td>
                    </tr>
                    <asp:PlaceHolder ID="plcBooking" runat="server">
                        <tr>
                            <td style="height: 18px">
                                Booking Form Link
                            </td>
                            <td style="height: 18px">
                                <asp:HyperLink ID="hlBookingFormLink" runat="server" Text="View" Target="_blank" />
                                <a id="aScanBookingForm" runat="server" href="javascript:OpenBookingFormWindow();">Scan</a>&#160;
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcPOD" runat="server">
                        <tr>
                            <td class="form-label">
                                POD Link
                            </td>
                            <td>
                                <asp:HyperLink ID="hlPODLink" runat="server" Target="_blank" Text="View " />
                                <a id="aScanPOD" runat="server" href="">Scan</a>&#160;
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <asp:PlaceHolder ID="plcCancellation" runat="server">
                        <tr>
                            <td>
                                Cancellation Reason
                            </td>
                            <td>
                                <asp:Label ID="lblCancellationReason" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Cancelled By
                            </td>
                            <td>
                                <asp:Label ID="lblCancelledBy" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                Cancelled At
                            </td>
                            <td>
                                <asp:Label ID="lblCancelledAt" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </asp:PlaceHolder>
                    <tr>
                        <td>
                             <asp:Label ID="lblCustomerOrderNumberText" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblLoadNumber" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                           <asp:Label ID="lblDeliveryOrderNumberText" runat="server"></asp:Label>
                        </td>
                        <td>
                            <asp:Label ID="lblDeliveryOrderNo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <asp:Repeater ID="repReferences" runat="server" Visible="false">
                        <ItemTemplate>
                            <tr>
                                <td valign="top" style="width: 170px;">
                                    <span>
                                        <%# DataBinder.Eval(Container.DataItem, "OrganisationReference.Description")%></span>
                                </td>
                                <td valign="top">
                                    <%# DataBinder.Eval(Container.DataItem, "Reference")%>
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                    <tr>
                        <td>
                            Collect From
                        </td>
                        <td>
                            <asp:Label ID="lblCollectionPoint" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Collect When
                        </td>
                        <td>
                            <asp:Label ID="lblCollectDateTime" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Deliver To
                        </td>
                        <td>
                            <asp:Label ID="lblDeliverTo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Deliver When
                        </td>
                        <td>
                            <asp:Label ID="lblDeliveryDateTime" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Pallets
                        </td>
                        <td>
                            <asp:Label ID="lblPallets" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Pallet Spaces
                        </td>
                        <td>
                            <asp:Label ID="lblPalletSpaces" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Type Of Goods
                        </td>
                        <td>
                            <asp:Label ID="lblGoodsType" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Weight
                        </td>
                        <td>
                            <asp:Label ID="lblWeight" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            No. of Cartons
                        </td>
                        <td>
                            <asp:Label ID="lblCartons" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr id="trRate" runat="server">
                        <td>
                            Rate
                        </td>
                        <td style="font-weight: bolder">
                            <asp:Label ID="lblRate" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td valign="top">
                            Notes
                        </td>
                        <td>
                            <asp:Label ID="lblNotes" runat="server"></asp:Label>&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td>
                            Created
                        </td>
                        <td>
                            <asp:Label ID="lblCreated" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table runat="server" id="tblSubbyRate" style="width: 700px;" cellspacing="0" cellpadding="0" class="neworderTable">
        <tr>
            <td class="formCellField-Horizontal">
                <telerik:RadGrid ID="RadGridForSubby" runat="server" AutoGenerateColumns="false">
                    <MasterTableView>
                        <Columns>
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
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
    <table>
        <tr>
            <td>
                <div style="height: 10px;">
                </div>
                <h2>
                    Extras</h2>
                <table width="100%">
                    <tr>
                        <td style="padding-top: 5px">
                            <asp:Label ID="lblNoExtras" runat="server" Text="There are no Extras" Visible="false"></asp:Label>
                            <telerik:RadGrid ID="grdExtras" runat="server" Skin="Office2007" AutoGenerateColumns="false">
                                <MasterTableView>
                                    <Columns>
                                        <telerik:GridTemplateColumn HeaderText="Type">
                                            <ItemTemplate>
                                                <asp:Label ID="lblExtraType" runat="server" Text='<%# Orchestrator.WebUI.Utilities.UnCamelCase(DataBinder.Eval(Container.DataItem, "ExtraType").ToString()) %>'></asp:Label>
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
                                        <telerik:GridTemplateColumn HeaderText="Amount (£)">
                                            <ItemTemplate>
                                                <asp:Label ID="lblExtraAmount" runat="server" />
                                            </ItemTemplate>
                                        </telerik:GridTemplateColumn>
                                        <telerik:GridHyperLinkColumn HeaderText="Job ID" DataNavigateUrlFields="JobID" DataNavigateUrlFormatString="javascript:ViewJobDetails({0});"
                                            DataTextField="JobID">
                                        </telerik:GridHyperLinkColumn>
                                    </Columns>
                                </MasterTableView>
                            </telerik:RadGrid>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div style="height: 28px; margin-top: 5px; padding: 4px; color: #ffffff; background-color: #99BEDE;
        text-align: left;">
        <asp:Button ID="btnPIL" runat="server" Text="Pallet Identification Labels" Width="160px"
            Height="25" />
        <asp:Button ID="btnCreateDeliveryNote" runat="server" Text="Delivery Note" Height="25" />
        <input type="button" id="btnClose" value="Close Window" onclick="CloseOnReload()"
            style="width: 125px; height: 25px;" />
    </div>

    <script language="javascript" type="text/javascript">
        //Required for the openDialog function called by OpenBookingFormWindow
        var returnUrlFromPopUp = window.location;

        function CloseOnReload() {
            var oWin = GetRadWindow();
            if (oWin != null)
                oWin.Close();
            else
                self.close();
        }

        function ViewInvoiceDetails(PDFLocation) {
            var url = '<%=this.ResolveUrl("~") %>' + PDFLocation;
            window.open(url);
        }

        function OpenPODWindow(jobId, collectDropId) {

            var podFormType = 2;
            var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
            url += "?ScannedFormTypeId=" + podFormType;
            url += "&JobId=" + jobId;
            url += "&CollectDropId=" + collectDropId;

            openResizableDialogWithScrollbars(url, 550, 500);
        }

        function OpenPODWindowForEdit(scannedFormId, jobId, collectDropId) {
            var podType = 2;
            var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
            url += "?ScannedFormTypeId=" + podType;
            url += "&JobId=" + jobId;
            url += "&ScannedFormId=" + scannedFormId;
            url += "&CollectDropId=" + collectDropId;

            openResizableDialogWithScrollbars(url, 550, 500);
        }

        function NewBookingForm(orderId) {
            var BookingFormType = 3;
            var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
            url += "?ScannedFormTypeId=" + BookingFormType;
            url += "&OrderId=" + orderId;

            openResizableDialogWithScrollbars(url, 550, 500);
        }

        function ReDoBookingForm(scannedFormId, orderId) {
            var BookingFormType = 3;
            var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
            url += "?ScannedFormTypeId=" + BookingFormType;
            url += "&ScannedFormId=" + scannedFormId;
            url += "&OrderId=" + orderId;

            openResizableDialogWithScrollbars(url, 550, 500);
        }
    </script>

</asp:Content>
