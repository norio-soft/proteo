<%@ Page language="c#" Inherits="Orchestrator.WebUI.PCV.ListPCVsRequiringScan" Codebehind="ListPCVsRequiringScan.aspx.cs" MasterPageFile="~/default_tableless.Master" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>PCVs Requiring Scan</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        
        <h2>Please select a PCV listed below to scan</h2>
        
		<asp:Datagrid id="dgRequiringScan" runat="server" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True"
					  pagesize="15" width="100%" cellpadding="2" backcolor="White" border="1" cssclass="DataGridStyle"
					  PagerStyle-Mode="NumericPages" PagerStyle-HorizontalAlign="Right">
				<AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
				<ItemStyle CssClass="DataGridListItem"></ItemStyle>
				<HeaderStyle CssClass="DataGridListHead"></HeaderStyle>	
				<Columns>
					<asp:HyperLinkColumn Text="Voucher No" DataNavigateUrlField="ScannedFormId" DataNavigateUrlFormatString="javascript:OpenPCVWindowForEdit({0});"
										 DataTextField="VoucherNo" SortExpression="VoucherNo" HeaderText="Voucher Number"></asp:HyperLinkColumn>
					<asp:BoundColumn DataField="VoucherNo" SortExpression="VoucherNo" HeaderText="Voucher Number" Visible="False"></asp:BoundColumn>
					<asp:BoundColumn DataField="NoOfPalletsReceived" SortExpression="NoOfPalletsReceived" HeaderText="Number of Pallets Received"></asp:BoundColumn>
					<asp:BoundColumn DataField="NoOfPalletsReturned" SortExpression="NoOfPalletsReturned" HeaderText="Number of Pallets Returned"></asp:BoundColumn>
					<asp:BoundColumn DataField="NoOfPalletsOutstanding" SortExpression="NoOfPalletsOutstanding" HeaderText="Number of Pallets Outstanding"></asp:BoundColumn>
					<asp:BoundColumn DataField="Status" SortExpression="Status" HeaderText="Status"></asp:BoundColumn>
					<asp:BoundColumn DataField="RedemptionStatus" SortExpression="RedemptionStatus" HeaderText="Redemption Status"></asp:BoundColumn>
					<asp:BoundColumn DataField="ReasonForIssuing" SortExpression="ReasonForIssuing" HeaderText="Reason For Issuing"></asp:BoundColumn>
					<asp:BoundColumn DataField="RedemptionStatus" SortExpression="RedemptionStatus" HeaderText="Redemption Status"></asp:BoundColumn>
					<asp:BoundColumn DataField="DateOfIssue" SortExpression="DateOfIssue" HeaderText="Date Of Issue" DataFormatString="{0:dd/MM/yy HH:mm}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Description" SortExpression="Description" HeaderText="Delivery Point"></asp:BoundColumn>
					<asp:BoundColumn DataField="OrganisationName" SortExpression="OrganisationName" HeaderText="Organisation"></asp:BoundColumn>
				</Columns>
				<PagerStyle visible="false"/>
		</asp:Datagrid>
			<table align="center">
		<tr>
			<td>
				<cc2:FirstLastPager id="Firstlastpager2" runat="server" Text="&amp;lt;&amp;lt;" PagingDirection="First"
					Devider="|" CssClass="PageNumbers"></cc2:FirstLastPager></td>
			<td><cc2:nextbackpager id="NextBackPager2" runat="server" PageCount="10" CssClass="PageNumbers" IsCyclic="False"
					PagingDirection="Back" Text="&amp;lt;Back"></cc2:nextbackpager></td>

			<td><cc2:pagenumberspager id="PageNumbersPager1" runat="server" PageCount="5" PageNumbersCSSClass="PageNumbers"
					PageNumbersCurrentPageCSSClass="CurrentPage"></cc2:pagenumberspager></td>

			<td><cc2:nextbackpager id="NextBackPager1" runat="server" PageCount="10" CssClass="PageNumbers"></cc2:nextbackpager></td>
			<td><cc2:FirstLastPager id="Firstlastpager1" runat="server" CssClass="PageNumbers" Devider="|"></cc2:FirstLastPager></td>
		</tr>
	</table>
<script language="javascript" type="text/javascript">
<!--
    function OpenPCVWindowForEdit(scannedFormId) {
        var pcvType = 1;
        var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
        url += "?ScannedFormTypeId=" + pcvType;
        url += "&ScannedFormId=" + scannedFormId;
        openResizableDialogWithScrollbars(url, 550, 500);
    }
//-->
</script>

</asp:Content>

