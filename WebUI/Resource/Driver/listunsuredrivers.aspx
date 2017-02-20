<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.ListUnsureDrivers" Codebehind="ListUnsureDrivers.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>

<uc1:header id="Header1" runat="server" Title="Drivers Awaiting Instruction" SubTitle="The drivers displayed below are awaiting instruction before they can continue with their journey."></uc1:header>

<form id="Form1" method="post" runat="server">
	<div align="center">
		<table width="99%">
			<tr valign="top">
				<td width="100%">
					
					<p><asp:Label id="lblError" runat="server" cssclass="confirmation"></asp:Label></p>
					
					<P1:PrettyDataGrid id="dgDriversAwaitingInstruction" runat="server" RowHighlightColor="255, 255, 128" BorderColor="#999999" 
						BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both" 
						AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="False" 
						GroupingColumn="AwaitingType" GroupCountEnabled="True" AllowGrouping="True"  GroupRowColor="#FDA16F" GroupForeColor="Black" 
						AllowCollapsing="True" StartCollapsed="False" Width="100%" FixedHeaders="False" RowHighlightingEnabled="True">
						<SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
						<ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ALTERNATINGITEMSTYLE>
						<ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ITEMSTYLE>
						<HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True" HorizontalAlign="Center"></HEADERSTYLE>
						<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
						<COLUMNS>
							<asp:BoundColumn HeaderText="Driver" DataField="FullName" HeaderStyle-Width="10%" ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Vehicle" DataField="RegNo" HeaderStyle-Width="10%" ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Trailer" DataField="TrailerRef" HeaderStyle-Width="10%" ItemStyle-VerticalAlign="Top" ItemStyle-Width="10%"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Last Leg" DataField="LastLeg" HeaderStyle-Width="30%" ItemStyle-VerticalAlign="Top" ItemStyle-Width="30%"></asp:BoundColumn>
							<asp:BoundColumn HeaderText="Next Leg" DataField="NextLeg" HeaderStyle-Width="30%" ItemStyle-VerticalAlign="Top" ItemStyle-Width="30%"></asp:BoundColumn>
							<asp:HyperLinkColumn DataNavigateUrlField="JobId" DataNavigateUrlFormatString="javascript:openDialogWithScrollbars('../../job/job.aspx?wiz=true&jobId={0}' + getCSID(),'600','400');" DataTextField="JobId" DataTextFormatString="Open Job {0}" HeaderStyle-Width="10%" ItemStyle-VerticalAlign="Top" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="10%"></asp:HyperLinkColumn>
						</COLUMNS>
						<PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
					</P1:PrettyDataGrid>
				</td>
			</tr>
		</table>
	</div>
</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>