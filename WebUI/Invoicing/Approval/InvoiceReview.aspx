<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.Approval.InvoiceReview" MasterPageFile="~/default_tableless.master" Title="Invoice Review" CodeBehind="InvoiceReview.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
<!--
        function SelectAllItems(itemName) {
            var parents = document.getElementsByName(itemName);
            for (var index = 0; index < parents.length; index++) {
                var inputs = parents[index].getElementsByTagName("input");
                if (inputs.length > 0)
                    if (inputs[0].style.display != 'none')
                    inputs[0].checked = true;
            }
        }

        function HideOption(clientID, substituteID) {
            var item = document.getElementById(clientID);
            if (item != null) {
                if (item.checked) {
                    var substituteItem = document.getElementById(substituteID);
                    if (substituteItem != null)
                        substituteItem.checked = true;
                }

                item.style.display = 'none';
            }
        }
//-->
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Invoice Review</h1></asp:Content>

<asp:Content ID="content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <fieldset style="padding: 0px; margin-top: 5px; margin-bottom: 5px;">
        <p>
            Below are a list of invoices that need to be approved before they can be finalised.
        </p>
        <p>
            You can specify which invoices you wish to approve, regenerate or reject by selecting
            the appropriate actions.
        </p>
    </fieldset>
    <asp:Label runat="server" ID="lblSavedMessage" ForeColor="Red"></asp:Label><div style="height: 10px;"></div>
    <telerik:RadGrid runat="server" ID="gvPreInvoices" AllowPaging="false" AllowSorting="false"
        Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="false"
        AllowFilteringByColumn="false" AllowAutomaticInserts="false">
        <mastertableview width="100%" name="PreInvoices" datakeynames="PreInvoiceID,InvoiceTypeID,LCID"
            allowfilteringbycolumn="false" commanditemdisplay="None" editmode="InPlace">
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <DetailTables>
                <telerik:GridTableView Name="Items" DataKeyNames="PreInvoiceID" AutoGenerateColumns="false" EditMode="InPlace" Width="600">
                    <ParentTableRelation>
                        <telerik:GridRelationFields DetailKeyField="PreInvoiceID" MasterKeyField="PreInvoiceID" />
                    </ParentTableRelation>
                    <Columns>
                        <telerik:GridBoundColumn HeaderText="ID" SortExpression="ID" DataField="ItemID" ReadOnly="true" HeaderStyle-Width="200" />
                        <telerik:GridBoundColumn HeaderText="Reference" SortExpression="Reference" DataField="Reference" ReadOnly="true" HeaderStyle-Width="200" />
                        <telerik:GridTemplateColumn HeaderText="Pallet Spaces">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblPalletSpaces"></asp:Label>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn HeaderText="Rate" ItemStyle-HorizontalAlign="Right">
                            <ItemTemplate>
                                <asp:HiddenField runat="server" ID="hidOldRate" />
                                <asp:TextBox ID="txtRate" runat="server" Width="50"></asp:TextBox>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                        <telerik:GridTemplateColumn>
                            <ItemTemplate>
                                <asp:Button ID="btnSave" runat="server" Text="Save" CommandName="update"></asp:Button>
                            </ItemTemplate>
                        </telerik:GridTemplateColumn>
                    </Columns>
                </telerik:GridTableView>       
            </DetailTables>
            <Columns>
                <telerik:GridBoundColumn DataField="OrganisationName" HeaderText="Client" HeaderStyle-Width="250" ReadOnly="true" />
                <telerik:GridBoundColumn DataField="InvoiceDate" HeaderText="Invoice Date" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="100" ReadOnly="true" />
                <telerik:GridTemplateColumn HeaderText="Tax Rate">
                    <ItemTemplate>
                        <asp:DropDownList ID="cboTaxRate" runat="server" DataTextField="Description" DataValueField="VatNo" OnSelectedIndexChanged="cboTaxRate_SelectedIndexChanged" AutoPostBack="true" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Client Reference" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="200">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkChangeClientReference" runat="server" Text="" CommandName="editClientReference"></asp:LinkButton><asp:TextBox ID="txtClientReference" runat="server" Width="60" MaxLength="256"></asp:TextBox>&nbsp;<asp:LinkButton ID="lnkSaveClientReference" runat="server" Text="Save" CommandName="updateClientReference"></asp:LinkButton>&nbsp;<asp:LinkButton ID="lnkCancelClientReference" runat="server" Text="Cancel" CommandName="cancelClientReference" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="PO Reference" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="200">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkChangePurchaseOrderReference" runat="server" Text="" CommandName="editPurchaseOrderReference"></asp:LinkButton><asp:TextBox ID="txtPurchaseOrderReference" runat="server" Width="60" MaxLength="256"></asp:TextBox>&nbsp;<asp:LinkButton ID="lnkSavePurchaseOrderReference" runat="server" Text="Save" CommandName="updatePurchaseOrderReference"></asp:LinkButton>&nbsp;<asp:LinkButton ID="lnkCancelPurchaseOrderReference" runat="server" Text="Cancel" CommandName="cancelPurchaseOrderReference" CausesValidation="false"></asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total Net" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70">
                    <ItemTemplate>
                        <asp:Label ID="lblNetAmount" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total On Extras" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70">
                    <ItemTemplate>
                        <asp:Label ID="lblExtraAmount" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total Fuel Surcharge" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70">
                    <ItemTemplate>
                        <asp:Label ID="lblFuelSurchargeAmount" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Total Tax" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70">
                    <ItemTemplate>
                        <asp:Label ID="lblTaxAmount" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Grand Total" ItemStyle-HorizontalAlign="Right" HeaderStyle-Width="70">
                    <ItemTemplate>
                        <asp:Label ID="lblTotalAmount" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn>
                    <ItemTemplate>
                        <a href="<%=Orchestrator.Globals.Configuration.WebServer %><%# DataBinder.Eval(Container.DataItem, "PDFLocation") %>" title="View Invoice" target="_blank">View</a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By" HeaderStyle-Width="100" ReadOnly="true" />
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created On" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="70" ReadOnly="true" />
                <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100" >
                    <HeaderTemplate>
                        <asp:RadioButton ID="rdoDoNothing" runat="server" Text="Do Nothing"/>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="spnDoNothing" name="spnDoNothing">
                            <input type="radio" id="rdoDoNothing" runat="server" name="rdoItemGroup" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100">
                    <HeaderTemplate>
                        <asp:RadioButton ID="rdoReject" runat="server" text="Reject" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="spnReject" name="spnReject">
                            <input type="radio" id="rdoReject" runat="server" name="rdoItemGroup" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100">
                    <HeaderTemplate>
                        <asp:RadioButton ID="rdoRegenerate" runat="server" text="Regenerate" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="spnRegenerate" name="spnRegenerate">
                            <input type="radio" id="rdoRegenerate" runat="server" name="rdoItemGroup" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="100">
                    <HeaderTemplate>
                        <asp:RadioButton ID="rdoApprove" runat="server" text="Approve" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="spnApprove" name="spnApprove">
                            <input type="radio" id="rdoApprove" runat="server" name="rdoItemGroup" />
                        </span>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="140">
                    <HeaderTemplate>
                        <asp:RadioButton ID="rdoApproveAndPost" runat="server" text="Approve and Post" />
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="spnApproveAndPost" name="spnApproveAndPost">
                            <input type="radio" id="rdoApproveAndPost" runat="server" name="rdoItemGroup" />
                        </span>
                </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="140">
                    <HeaderTemplate>
                            Use Headed Paper
                    </HeaderTemplate>
                    <ItemTemplate>
                        <span id="spnUseHeadedPaper" name="spnUseHeadedPaper">
                            <input type="checkbox" id="chkUseHeadedPaper" runat="server" name="chkUseHeadedPaper" />
                        </span>
                </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>            
        </mastertableview>
        <filtermenu cssclass="FilterMenuClass"></filtermenu>
    </telerik:RadGrid>
    <br />
    <div class="buttonbar">
        <asp:Button ID="btnApply" runat="server" Text="Apply" CausesValidation="false" />
    </div>
</asp:Content>