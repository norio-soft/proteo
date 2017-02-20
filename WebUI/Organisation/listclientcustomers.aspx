<%@ Page language="c#" Inherits="Orchestrator.WebUI.Organisation.listclientcustomers" Codebehind="listclientcustomers.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="cc3" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<!DOCTYPE HTML PUBLIC "http://www.w3.org/TR/html4/loose.dtd" >  

<uc1:header id="pageHeader" title="Find a Client Customer" SubTitle="Please Choose a Client Customer." XMLPath="organisationContextMenu.xml" runat="server"></uc1:header>
<style>
		.PageNumbers { FONT-SIZE: 10pt; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: none }
		.CurrentPage { FONT-WEIGHT: bold; FONT-SIZE: 10pt; COLOR: red; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: underline }
	</style>

<form id="Form1" runat="server">
	<fieldset>
		<legend>Clients</legend>
		<table width="100%">
			<tr>
				<td>
                    <telerik:RadComboBox ID="cboClient" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" ShowMoreResultsBox="true" Width="355px">
                    </telerik:RadComboBox>
					<asp:RequiredFieldValidator id="rfvClient" runat="server" ControlToValidate="cboClient" Display="Dynamic" ErrorMessage="Please select a client's customer."><img src="../images/error.gif" alt="Please select a client's customer." /></asp:RequiredFieldValidator>
					<asp:button id="btnUpdate" runat="server" text="Update" onclick="btnUpdate_Click"></asp:button>
				</td>
			</tr>
			<tr>
				<td>
					<P1:PrettyDataGrid id="dgClients" runat="server" RowHighlightColor="255, 255, 128" BorderColor="#999999" 
						BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both" AllowPaging="True" PageSize="20"
						AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="True" 
						GroupingColumn="OrganisationName" GroupCountEnabled="False" AllowGrouping="False"  GroupRowColor="#FDA16F" GroupForeColor="Black" 
						AllowCollapsing="False" StartCollapsed="False" width="1000px" FixedHeaders="True" RowHighlightingEnabled="True">
						<SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
						<ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ALTERNATINGITEMSTYLE>
						<ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black" VerticalAlign="Top"></ITEMSTYLE>
						<HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True" HorizontalAlign="Center"></HEADERSTYLE>
						<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
						<COLUMNS>
							<asp:BoundColumn HeaderText="Organisation Name" DataField="OrganisationName"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Organisation Type" DataField="OrganisationType"></asp:BoundColumn>
							<asp:TemplateColumn HeaderText="Head Office" ItemStyle-VerticalAlign="Top">
								<ItemTemplate>
									<input type="hidden" runat="server" id="hidIdentityId" value='<%# DataBinder.Eval(Container.DataItem, "IdentityId") %>' NAME="hidIdentityId"/>
									<asp:Label id="lblAddress" runat="server"></asp:Label>
								</ItemTemplate>
							</asp:TemplateColumn>
						</COLUMNS>
						<PAGERSTYLE Visible="False" BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Right"></PAGERSTYLE>
					</P1:PrettyDataGrid>
						<table align="center">
		<tr>
			<td>
				<cc3:FirstLastPager id="Firstlastpager2" runat="server" Text="&amp;lt;&amp;lt;" PagingDirection="First"
					Devider="|" CssClass="PageNumbers"></cc3:FirstLastPager></td>
			<td><cc3:nextbackpager id="NextBackPager2" runat="server" PageCount="10" CssClass="PageNumbers" IsCyclic="False"
					PagingDirection="Back" Text="&amp;lt;Back"></cc3:nextbackpager></td>

			<td><cc3:pagenumberspager id="PageNumbersPager1" runat="server" PageCount="5" PageNumbersCSSClass="PageNumbers"
					PageNumbersCurrentPageCSSClass="CurrentPage"></cc3:pagenumberspager></td>

			<td><cc3:nextbackpager id="NextBackPager1" runat="server" PageCount="10" CssClass="PageNumbers"></cc3:nextbackpager></td>
			<td><cc3:FirstLastPager id="Firstlastpager1" runat="server" CssClass="PageNumbers" Devider="|"></cc3:FirstLastPager></td>
		</tr>
	</table>

				</td>
			</tr>
		</table>
	</fieldset>
</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>
