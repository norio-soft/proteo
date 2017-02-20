<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Invoicing.Groupage.autorunconfirmation" Title="Invoice Batch Summary Details" Codebehind="autorunconfirmation.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Auto Batch Run Confirmation</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
    <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Auto Run Batch Summary Details</div>
    <p>
        The total number and values of the orders for the batch are display below, this does not include any extras fuel surcharge, VAT etc. 
    </p>
    <p>
        You must provide at least one person to receive notification of when the invoices are ready to approve.</b>
    </p>
    <p>
        If you are happy with this batch, please click <b>Start Creating Batch</b> this will begin the process of creating your batches and will email you when this has been completed.
    </p>
    <p>
        If you want to make changes to this batch please click the <b>Change Batch</b> button.
    </p>
    <table width="800">
        <tr>
            <td class="form-label" width="250">Invoice Date</td>
            <td class="form-value" colspan="2"><asp:label ID="lblInvoicedate" runat="server" Font-Size="Small"></asp:label></td>
        </tr>
        <tr id="trClientInvoiceReference" runat="server">
            <td class="form-label" width="250">Client Invoice Reference</td>
            <td class="form-value" colspan="2"><asp:TextBox ID="txtClientInvoiceReference" runat="server" MaxLength="256" ToolTip="Text entered here will be displayed on all the invoices produced, it can be changed during the approval process."></asp:TextBox></td>
        </tr>
        <tr id="trPOReference" runat="server">
            <td class="form-label" width="250">Purchase Order Reference</td>
            <td runat="server" class="form-value" colspan="2"><asp:TextBox ID="txtPurchaseOrderReference" runat="server" MaxLength="256" ToolTip="Text entered here will be displayed on all the invoices produced, it can be changed during the approval process."></asp:TextBox></td>
        </tr>
        <tr>
            <td class="form-label" valign="top">Notification Recipient(s)</td>
            <td class="form-value" width="500">
                <telerik:RadGrid runat="server" ID="grdNotificationRecipients" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="500" Height="223" ShowFooter="false">
                    <MasterTableView AllowFilteringByColumn="false" DataKeyNames="UserName,EmailAddress,FullName">
                        <RowIndicatorColumn Display="false"></RowIndicatorColumn>
                        <Columns>
                            <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="40" HeaderText=""></telerik:GridClientSelectColumn>
                            <telerik:GridBoundColumn DataField="FullName" HeaderText="User" HeaderStyle-Width="150"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn DataField="EmailAddress" HeaderText="Email Address" HeaderStyle-Width="250"></telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Scrolling UseStaticHeaders="true" AllowScroll="true" ScrollHeight="200" />
                        <Selecting AllowRowSelect="true" />
                    </ClientSettings>
                </telerik:RadGrid>
            </td>
            <td class="form-value" align="left" valign="top" width="50">
            </td>
        </tr>
    </table>
</fieldset>

<telerik:RadGrid runat="server" ID="grdOrders" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="300" ShowFooter="true">
    <MasterTableView Width="100%" >
        <RowIndicatorColumn Display="false"></RowIndicatorColumn>
        <Columns>
            <telerik:GridBoundColumn DataField="OrganisationName" HeaderText="Customer"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="orderCount" DataField="Number of Orders" HeaderText="Number of Orders"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn UniqueName="orderValue" DataField="Value Of Orders" HeaderText="Value" DataFormatString="{0:C}" ></telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
</telerik:RadGrid>

<p></p>

<div style="height:22px; border-bottom:solid 1pt silver;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;">
    <asp:Button ID="btnChangeBatch" runat="server" Text="Change Batch" CausesValidation="false" />
    <asp:Button id="btnCreateBatch" runat="server" Text="Start Creating Batch" />
</div>
</asp:Content>