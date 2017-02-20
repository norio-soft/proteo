<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Invoicing.SubContractorSB.autorunconfirmation" Title="Sub-contractor Self Bill Invoice Batch Summary Details" Codebehind="autorunconfirmation.aspx.cs" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Auto Run Batch Summary Details</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <h2>The total number and values of the sub-contractor information for the batch are display below.</h2> 
    <div class="infoPanel" style="margin: 0 0 10px 0;">You must provide at least one person to receive notification of when the invoices are ready to approve. If you are happy with this batch, please click <b>Start Creating Batch</b> this will begin the process of creating your batches and will email you when this has been completed. If you want to make changes to this batch please click the <b>Change Batch</b> button.</div>
    <fieldset>
        <table width="800">
            <tr>
                <td class="formCellLabel" width="250">Invoice Date</td>
                <td class="formCellInput" colspan="2"><asp:label ID="lblInvoicedate" runat="server" Font-Size="Small"></asp:label></td>
            </tr>
            <tr id="pnlSubbySelfBill" runat="server" visible="true">
                <td class="formCellLabel" width="250">Self Bill</td>
                <td class="formCellInput" colspan="2"><asp:CheckBox ID="chkSelfBill" runat="server" Checked="true" AutoPostBack="true" /></td>
            </tr>
            <tr id="pnlSubbyInvoiceNo" runat="server" visible="false">
                <td class="formCellLabel" width="250">Sub-Contractor Invoice No</td>
                <td class="formCellInput" colspan="2"><asp:TextBox ID="txtSubbyInvoiceNo" runat="server" MaxLength="255" ></asp:TextBox></td>
            </tr>
            <tr>
                <td class="formCellLabel" valign="top">Notification Recipient(s)</td>
                <td class="formCellInput" width="500">
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
                <td class="formCellInput" align="left" valign="top" width="50">
                    <asp:CustomValidator ID="cfvNotificationPartiesMustBeSpecified" runat="server" ControlToValidate="" Display="dynamic" EnableClientScript="false" ErrorMessage="Please select at least one person to be notified once the invoices have been created."><img src="../../images/ico_critical_small.gif" height="16" width="16" title="Please select at least one person to be notified once the invoices have been created." /></asp:CustomValidator>
                </td>
            </tr>
        </table>
    </fieldset>
    <telerik:RadGrid runat="server" ID="grdSubbies" AllowPaging="false" AllowSorting="true" Skin="Office2007" EnableAJAX="true" AutoGenerateColumns="false" AllowMultiRowSelection="true" Width="300" ShowFooter="true">
        <MasterTableView Width="100%" >
            <RowIndicatorColumn Display="false"></RowIndicatorColumn>
            <Columns>
                <telerik:GridBoundColumn DataField="OrganisationName" HeaderText="Sub-Contractor"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn UniqueName="subbyCount" DataField="Number Of Sub-Contracts" HeaderText="Number Of Sub-Contracts"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn UniqueName="subbyValue" DataField="Value Of Sub-Contracts" HeaderText="Value" DataFormatString="{0:C}" ></telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    <p></p>
    <div class="buttonbar">
        <asp:Button ID="btnChangeBatch" runat="server" Text="Change Batch" CausesValidation="false" />
        <asp:Button id="btnCreateBatch" runat="server" Text="Start Creating Batch" />
    </div>
    
</asp:Content>