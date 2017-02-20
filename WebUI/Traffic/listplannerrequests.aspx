<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.ListPlannerRequests" Codebehind="ListPlannerRequests.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" runat="server" Title="List Planner Requests" SubTitle="A list of planner requests is shown below."></uc1:header>

<form id="Form1" method="post" runat="server">
	<div align="center">
		<table width="99%">
			<tr>
				<td width="100%">
					<fieldset>
						<legend>Planner Requests</legend>
						<table width="100%">
							<tr>
								<td width="20%" valign="top">Show requests made by:</td>
								<td width="80%">
									<asp:CheckBoxList id="chkPlanners" runat="server" RepeatDirection="Horizontal" RepeatColumns="3"></asp:CheckBoxList>
								</td>
							</tr>
							<tr>
								<td valign="top">For Job:</td>
								<td valign="top">
									<asp:TextBox id="txtJobId" runat="server" Width="60px"></asp:TextBox>
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<div class="buttonbar">
										<asp:Button id="btnFilter" runat="server" Text="Filter"></asp:Button>
									</div>
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<asp:Label id="lblConfirmation" runat="server" CssClass="confirmation"></asp:Label>
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<P1:PrettyDataGrid id="dgRequests" runat="server" RowHighlightColor="255, 255, 128" BorderColor="#999999" 
										BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both" 
										AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="False" 
										GroupingColumn="" AllowGrouping="False" GroupRowColor="#FDA16F" GroupForeColor="Black" 
										AllowCollapsing="False" StartCollapsed="False" width="800">
										<SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
										<ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ALTERNATINGITEMSTYLE>
										<ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ITEMSTYLE>
										<HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True"></HEADERSTYLE>
										<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
										<COLUMNS>
											<asp:BoundColumn HeaderText="Request Id" DataField="PlannerRequestId" Visible="False"></asp:BoundColumn>
											<asp:TemplateColumn HeaderText="Source Job Id" SortExpression="SourceJobId" ItemStyle-VerticalAlign="Top" HeaderStyle-Wrap="False">
												<ItemTemplate>
													<a href=javascript:ViewJob('<%# DataBinder.Eval(Container.DataItem, "SourceJobId") %>')><%# DataBinder.Eval(Container.DataItem, "SourceJobId") %></a>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Target Job Id" SortExpression="TargetJobId" ItemStyle-VerticalAlign="Top" HeaderStyle-Wrap="False">
												<ItemTemplate>
													<a href=javascript:ViewJob('<%# DataBinder.Eval(Container.DataItem, "TargetJobId") %>')><%# DataBinder.Eval(Container.DataItem, "TargetJobId") %></a>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:BoundColumn HeaderText="Request" DataField="RequestText" SortExpression="RequestText" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
											<asp:TemplateColumn HeaderText="Use Driver" SortExpression="UseDriver" ItemStyle-VerticalAlign="Top" HeaderStyle-Wrap="False">
												<ItemTemplate>
													<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "UseDriver")) ? "Yes" : "No" %>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Use Vehicle" SortExpression="UseVehicle" ItemStyle-VerticalAlign="Top" HeaderStyle-Wrap="False">
												<ItemTemplate>
													<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "UseVehicle")) ? "Yes" : "No" %>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:TemplateColumn HeaderText="Use Trailer" SortExpression="UseTrailer" ItemStyle-VerticalAlign="Top" HeaderStyle-Wrap="False">
												<ItemTemplate>
													<%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "UseTrailer")) ? "Yes" : "No" %>
												</ItemTemplate>
											</asp:TemplateColumn>
											<asp:ButtonColumn HeaderText="Edit" CommandName="Edit" ButtonType="LinkButton" ItemStyle-VerticalAlign="Top" Text="Edit"></asp:ButtonColumn>
											<asp:ButtonColumn HeaderText="Delete" CommandName="Delete" ButtonType="LinkButton" ItemStyle-VerticalAlign="Top" Text="Delete"></asp:ButtonColumn>
										</COLUMNS>
										<PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
									</P1:PrettyDataGrid>
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<div class="buttonbar">
										<asp:Button id="btnAddRequest" runat="server" Text="Add Request" CausesValidation="False"></asp:Button>
									</div>
								</td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
		</table>
	</div>
</form>

<script language="javascript" type="text/javascript">
<!--
function ViewJob(jobId)
{
    window.location.href = '../Job/job.aspx?jobId=' + jobId + getCSID();
}
//-->
</script>

<uc1:footer id="Footer1" runat="server"></uc1:footer>
