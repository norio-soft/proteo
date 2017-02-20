<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.SubContractor.SubContractorList" Codebehind="SubContractorList.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<uc1:header id="Header1" runat="server" Title="Sub-Contractor List" SubTitle="Please choose a sub-contractor from the list below."></uc1:header>
<style>
		.PageNumbers { FONT-SIZE: 10pt; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: none }
		.CurrentPage { FONT-WEIGHT: bold; FONT-SIZE: 10pt; COLOR: red; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: underline }
	</style>
<form id="Form1" method="post" runat="server">
	<br>
	<P1:PrettyDataGrid id="dgSubContractors" runat="server" RowHighlightColor="255, 255, 128" BorderColor="#999999" 
		BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both"
		AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="True" 
		GroupingColumn="OrganisationName" GroupCountEnabled="False" AllowGrouping="False"  GroupRowColor="#FDA16F" GroupForeColor="Black" 
		AllowCollapsing="False" StartCollapsed="False" width="1000px" FixedHeaders="False" RowHighlightingEnabled="True">
		<SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
		<ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ALTERNATINGITEMSTYLE>
		<ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black" VerticalAlign="Top"></ITEMSTYLE>
		<HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True" HorizontalAlign="Center"></HEADERSTYLE>
		<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
		<COLUMNS>
			<asp:BoundColumn HeaderText="Organisation Name" DataField="OrganisationName"></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Primary Contact" DataField="PrimaryContact"></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Phone Number" DataField="PhoneNumber"></asp:BoundColumn>
			<asp:TemplateColumn HeaderText="Head Office" ItemStyle-VerticalAlign="Top">
				<ItemTemplate>
					<input type="hidden" runat="server" id="hidIdentityId" value='<%# DataBinder.Eval(Container.DataItem, "IdentityId") %>' NAME="hidIdentityId"/>
					<asp:Label id="lblAddress" runat="server"></asp:Label>
				</ItemTemplate>
			</asp:TemplateColumn>
		</COLUMNS>
		<PAGERSTYLE BackColor="#A1C0F6" visible="False" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Right"></PAGERSTYLE>
	</P1:PrettyDataGrid>
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
	<div style="HEIGHT:10px"></div>
	<div class="buttonbar">
		<asp:button id="btnAddSubContractor" runat="server" Text="Add Sub-Contractor"></asp:button>
		<asp:button id="btnExport" runat="server" Text="Export" style="width:75px;" />
	</div>
</form>
<uc1:footer id="Footer1" runat="server"></uc1:footer>