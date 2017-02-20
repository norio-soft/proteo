<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ApprovalProcess.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.Invoicing.ApprovalProcess" %>
<asp:RadioButtonList runat="server" ID="rblFilterInvoices" AutoPostBack="true" RepeatDirection="Horizontal">
    <asp:ListItem Text="Mine Only" Value="0" Selected="True" />
    <asp:ListItem Text="All" Value="1" />
</asp:RadioButtonList>

<table>
    <tr>
        <td class="formCellLabel">Created By
        </td>
        <td class="formCellField" colspan="4">
            <input type="checkbox" runat="server" ID="chkSelectAllCreatedBy" class="chkSelectAllCreatedBy" checked='true' onclick="selectAllCreatedBy(this);" /><label for="chkSelectAllCreatedBy">Select All</label>
            <asp:CheckBoxList runat="server" ID="cblCreatedBy" RepeatDirection="Horizontal" RepeatColumns="6"></asp:CheckBoxList>
        </td>
    </tr>
</table>

<div class="buttonbar">
    <asp:Button ID="btnApplyTop" runat="server" Text="Apply" CausesValidation="false" />
    <asp:Button ID="btnRefreshTop" runat="server" Text="Refresh PreInvoices" CausesValidation="false" />
</div>

<br />

<telerik:RadGrid runat="server" ID="gvPreInvoices" AllowPaging="false" AllowSorting="false" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false" AllowFilteringByColumn="false" AllowAutomaticInserts="false" BackColor="White">
    <mastertableview name="PreInvoices" datakeynames="PreInvoiceID,InvoiceTypeID,LCID,PreInvoiceBatchID,InvoiceGenerationParametersID" ClientDataKeyNames="PreInvoiceID" allowfilteringbycolumn="false" commanditemdisplay="None" editmode="InPlace">
        <RowIndicatorColumn Display="false"></RowIndicatorColumn>
        <DetailTables >
            <telerik:GridTableView Name="Items" DataKeyNames="PreInvoiceID" AutoGenerateColumns="false"  EditMode="InPlace" Width="600" CommandItemDisplay="Top">
                <ParentTableRelation>
                    <telerik:GridRelationFields DetailKeyField="PreInvoiceID" MasterKeyField="PreInvoiceID" />
                </ParentTableRelation>
                <Columns>
                    <telerik:GridTemplateColumn UniqueName="OrderId" HeaderText="Id" HeaderStyle-Width="200">
                        <ItemTemplate>
                            <a id="lnkOrderId" runat="server"></a>
                            <asp:Label ID="lblJobSubcontractId" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn HeaderText="ID" SortExpression="ID" DataField="ItemID" ReadOnly="true" HeaderStyle-Width="200" Display="false" />
                    <telerik:GridBoundColumn HeaderText="Reference" SortExpression="Reference" DataField="Reference" ReadOnly="true" HeaderStyle-Width="200" />
                    <telerik:GridTemplateColumn HeaderText="Pallet Spaces">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblPalletSpaces"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn HeaderText="Rate" ItemStyle-HorizontalAlign="Right">
                        <ItemTemplate>
                            <asp:HiddenField runat="server" ID="hidOldRate" />
                            <asp:TextBox ID="txtRate" runat="server" Width="50" onchange="javascript:txtRate_OnClientValueChanged(this);"></asp:TextBox>
                            <asp:HiddenField ID="hidInvoiceItemIsDirty" runat="server" Value="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn>

                        <ItemTemplate>
                            <asp:Button ID="btnDelete" runat="server" Text="Delete" CommandName="remove" OnClientClick="javascript:preInvoiceOrder_Remove(this); return false" Visible="true"></asp:Button>
                            <asp:HiddenField ID="hidInvoiceItemPendingDelete" runat="server" Value="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
                <CommandItemTemplate>
                <div style="float:right;">
                    <asp:Button ID="btnExport" runat="server" Text="Order Export" CssClass="buttonClassSmall" CommandName="Export"/>
                </div>
                </CommandItemTemplate>
            </telerik:GridTableView>     
        </DetailTables>
        <Columns>
            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="30">
                <ItemTemplate>
                    <span id="spnPreInvoice" name="spnPreInvoice" style="text-align:center;">
                        <asp:CheckBox runat="server" id="chkPreInvoice"/>
                    </span>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="BatchLabel" HeaderText="Batch" HeaderStyle-Width="75" ReadOnly="true" />
            <telerik:GridBoundColumn DataField="OrganisationName" HeaderText="Client" HeaderStyle-Width="200" ReadOnly="true" />
            <telerik:GridTemplateColumn HeaderText="Invoice Date" HeaderStyle-Width="80" ReadOnly="false" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <telerik:RadDateInput ID="rdiInvoiceDate" runat="server" EmptyMessage="None Provided" Culture="en-GB" DateFormat="dd/MM/yy" DisplayDateFormat="dd/MM/yy" Width="70" ClientEvents-OnValueChanged="rdiInvoiceDate_OnClientValueChanged" />
                    <asp:RequiredFieldValidator ID="rfvInvoiceDate" runat="server" ControlToValidate="rdiInvoiceDate" Display="Dynamic" EnableClientScript="false">
                        <img src="/images/ico_critical_small.gif" height="16" width="16" title="Please supply an invoice date." alt="" />
                    </asp:RequiredFieldValidator>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Tax Rate" HeaderStyle-Width="90">
                <ItemTemplate>
                    <asp:DropDownList ID="cboTaxRate" runat="server" DataTextField="Description" DataValueField="VatNo" Width="80" AutoPostBack="false" onchange="javascript:cboTaxRate_OnClientSelectedIndexChanged(this);" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Client Ref" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="100" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <telerik:RadTextBox ID="rtClientReference" runat="server" Width="90" MaxLength="256" EmptyMessage="None Provided" ClientEvents-OnValueChanged="rtClientReference_OnClientValueChanged" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="PO Ref" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="100" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <telerik:RadTextBox ID="rtPurchaseOrderReference" runat="server" Width="90" MaxLength="256" EmptyMessage="None Provided" ClientEvents-OnValueChanged="rtPurchaseOrderReference_OnClientValueChanged" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Total Net" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblNetAmount" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Total On Extras" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblExtraAmount" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Total Fuel" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblFuelSurchargeAmount" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Total Tax" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblTaxAmount" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Grand Total" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70" ItemStyle-Wrap="false">
                <ItemTemplate>
                    <asp:Label ID="lblTotalAmount" runat="server" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderStyle-Width="40">
                <ItemTemplate>
                    <a href="<%=Orchestrator.Globals.Configuration.WebServer %><%# DataBinder.Eval(Container.DataItem, "PDFLocation") %>" title="View Invoice" target="_blank">View</a>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" HeaderStyle-Width="100" ReadOnly="true" />
            <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created On" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="70" ReadOnly="true" />
            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="40">
                <HeaderTemplate>
                    Auto Email
                </HeaderTemplate>
                <ItemTemplate>
                    <span id="spnAutoEmail" name="spnAutoEmail" style="text-align:center;" >
                        <input type="checkbox" id="chkAutoEmail" checked="<%# Eval("AutoEmailInvoices") %>" disabled="true" style="display:<%# ((bool)Eval("AutoEmailInvoices"))?"":"none" %>;" />
                    </span>
            </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="110" ItemStyle-Wrap="false" >
                <HeaderTemplate>
                    <asp:RadioButton ID="rdoDoNothing" runat="server" Text="Do Nothing"/>
                </HeaderTemplate>
                <ItemTemplate>
                    <span id="spnDoNothing" name="spnDoNothing">
                        <input type="radio" id="rdoDoNothing" runat="server" name="rdoItemGroup" />
                    </span>
                    <asp:Label ID="lblPendingChanges" runat="server" Text="" Font-Bold="true"></asp:Label>
                    <asp:HiddenField ID="hidIsDirty" runat="server" Value="False" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60" HeaderText="Reject">
                <ItemTemplate>
                    <span id="spnReject" name="spnReject">
                        <input type="radio" id="rdoReject" runat="server" name="rdoItemGroup" />
                    </span>
                </ItemTemplate>
            </telerik:GridTemplateColumn>

            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60">
                <HeaderTemplate>
                    <asp:RadioButton ID="rdoApprove" runat="server" text="Approve" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span id="spnApprove" name="spnApprove">
                        <input type="radio" id="rdoApprove" runat="server" name="rdoItemGroup" />
                    </span>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60">
                <HeaderTemplate>
                    <asp:RadioButton ID="rdoApproveAndPost" runat="server" text="Approve and Post" />
                </HeaderTemplate>
                <ItemTemplate>
                    <span id="spnApproveAndPost" name="spnApproveAndPost">
                        <input type="radio" id="rdoApproveAndPost" runat="server" name="rdoItemGroup" />
                    </span>
            </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="60">
                <HeaderTemplate>                    
                    <HeaderTemplate>                        
                        <input type="checkbox" id="chkAllUseHeadedPaper" onclick="javascript:selectAllCheckboxes(this);" />
                        <span>Use Headed Paper</span>
                    </HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <input type="checkbox" id="chkUseHeadedPaper" runat="server" />
            </ItemTemplate>
            </telerik:GridTemplateColumn>
        </Columns>            
    </mastertableview>
    <ClientSettings Selecting-AllowRowSelect="true">
        <ClientEvents OnRowSelected="gvPreInvoices_OnClientRowSelected" />        
        <Scrolling AllowScroll="true" UseStaticHeaders="true" />
    </ClientSettings>
    <filtermenu cssclass="FilterMenuClass"></filtermenu>
</telerik:RadGrid>

<br />

<div class="buttonbar">
    <asp:Button ID="btnApply" runat="server" Text="Apply" CausesValidation="false" />
    <asp:Button ID="btnRefresh" runat="server" Text="Refresh PreInvoices" CausesValidation="false" />
</div>

<telerik:RadAjaxManager ID="ramApprovalProcess" runat="server" EnableAJAX="false" >
    <AjaxSettings>
        <telerik:AjaxSetting AjaxControlID="gvPreInvoices">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="gvPreInvoices" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnApply">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="gvPreInvoices" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="btnApplyTop">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="gvPreInvoices" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="gvPreInvoices">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="gvPreInvoices" />
            </UpdatedControls>
        </telerik:AjaxSetting>
        <telerik:AjaxSetting AjaxControlID="rblFilterInvoices">
            <UpdatedControls>
                <telerik:AjaxUpdatedControl ControlID="gvPreInvoices" />
            </UpdatedControls>
        </telerik:AjaxSetting>
    </AjaxSettings>
</telerik:RadAjaxManager>

<telerik:RadCodeBlock ID="rcbApprovalProcess" runat="server">
<script type="text/javascript">
    var ramApprovalProcess = null;

    Sys.Application.add_load(function () {

        ramApprovalProcess = $find("<%=ramApprovalProcess.ClientID %>");

        var invoiceItems = $("input[id*=hidInvoiceItemPendingDelete]");
        invoiceItems.each(function (index, element) {
            if (element.value.toLowerCase() === 'true') {
                setInvoiceItemToPendingDelete($(this));

                var invoiceDateItemChildRow = $('#' + this.id);

                var invoiceRow = null;
                var invoiceChildRowId = invoiceDateItemChildRow.parent().parent().parent().parent().attr("id");
                var invoiceChildRow = $find(invoiceChildRowId);

                if (invoiceChildRow != null) {
                    invoiceRow = invoiceChildRow.get_parentRow();

                    var jQueryInvoiceRow = $('#' + invoiceRow.id);
                    pendingChanges(jQueryInvoiceRow);
                }
            }
        });
    });

   

    function viewOrder(orderID) {
        var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

        var wnd = window.open(url, "OrderProfile", "Width=1180, height=900, scrollbars=1, resizable=1");
    }

    function selectAllCheckboxes(chk) {
        $('table[id*=gvPreInvoices] input:enabled[id*=chkUseHeadedPaper]').each(function () {
            this.click();
        });
    }

    function selectAllCreatedBy(sender) {
        $('input:checkbox[id*=cblCreatedBy]').prop('checked', $(sender).prop('checked'));
    }

    </script>
</telerik:RadCodeBlock>
