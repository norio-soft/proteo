<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpOrdersReadyToInvoice" Codebehind="wpOrdersReadyToInvoice.ascx.cs" %>
<asp:sqldatasource id="sqlJobsReadyToInvoice" runat="server" DataSourceMode="DataSet" selectCommand="spMI_GetCountOfOrdersReadyToInvoice" ConnectionString="<%$ConnectionStrings:Orchestrator %>" cacheexpirationpolicy="Sliding" enablecaching="true" cacheduration="30"></asp:sqldatasource>
            				                
<asp:gridview id="gvJobsReadyToInvoice" DataSourceID="sqlJobsReadyToInvoice" runat="server" autogeneratecolumns="false" width="100%" enableviewstate="false" cellspacing="0" cellpadding="0" borderwidth="0" cssclass="Grid" showfooter="false" showheader="true" >
    <headerstyle cssclass="HeadingRowLite" height="18" verticalalign="middle" />
    <rowStyle  cssclass="Row" />
    <AlternatingRowStyle  backcolor="WhiteSmoke" />
    <SelectedRowStyle  cssclass="SelectedRow" />
    <footerstyle cssclass="Row" />
    <columns>
            <asp:boundfield datafield="OrganisationName" HeaderText="Client" ItemStyle-Width="32%" ItemStyle-Wrap="false" />
            <asp:boundfield datafield="CountFlagged" HeaderText="" ItemStyle-Width="5%" ItemStyle-HorizontalAlign="Right" />
            <asp:HyperLinkField ItemStyle-HorizontalAlign="Right"  HeaderText="Flagged" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="14%" ItemStyle-Wrap="false" DataTextField="ValueFlagged" DataTextFormatString="{0:C0}" DataNavigateUrlFields="IdentityId,MinFlagged" DataNavigateUrlFormatString="~/invoicing/groupage/InvoiceBatchAndApproval.aspx?identityid={0}&MinDate={1:d}" />
            <asp:boundfield datafield="CountUnFlagged" HeaderText="" ItemStyle-Width="25%" ItemStyle-HorizontalAlign="Right" />
            <asp:HyperLinkField ItemStyle-HorizontalAlign="Right" HeaderText="UnFlagged" HeaderStyle-HorizontalAlign="Left" ItemStyle-Width="14%" ItemStyle-Wrap="false" DataTextField="ValueUnFlagged" DataTextFormatString="{0:C0}" DataNavigateUrlFields="IdentityId,MinUnFlagged" DataNavigateUrlFormatString="~/invoicing/groupage/FlagForInvoicing.aspx?identityid={0}&MinDate={1:d}" />          
    </columns>
 </asp:gridview>