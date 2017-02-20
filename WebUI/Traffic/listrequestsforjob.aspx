<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.ListRequestsForJob" Codebehind="ListRequestsForJob.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<uc1:header id="Header1" runat="server" title="Planner Requests" ShowLeftMenu="false" SubTitle="This job's planner requests are shown below."></uc1:header>

<form id="Form1" method="post" runat="server">
	<div align="center" id="topPortion">
		<table width="99%" cellspacing="2" cellpadding="0" border="0">
			<tr valign="top">
				<td>
					This page shows the planner requests that have been attached to job <asp:Label id="lblJobId" runat="server" Font-Bold="True"></asp:Label>.<br>You can click on the links to manage that requests' details.
					<br>
					<asp:Label id="lblConfirmation" runat="server" CssClass="confirmation"></asp:Label>
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
							<asp:TemplateColumn HeaderText="Edit" ItemStyle-VerticalAlign="Top" HeaderStyle-Wrap="False">
								<ItemTemplate>
									<a href=javascript:EditRequest('<%# DataBinder.Eval(Container.DataItem, "PlannerRequestId") %>')>Edit</a>
								</ItemTemplate>
							</asp:TemplateColumn>
							<asp:ButtonColumn HeaderText="Delete" CommandName="Delete" ButtonType="LinkButton" ItemStyle-VerticalAlign="Top" Text="Delete"></asp:ButtonColumn>
						</COLUMNS>
						<PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
					</P1:PrettyDataGrid>
				</td>
			</tr>
		</table>
	</div>
</form>

<script language="javascript" type="text/javascript">
<!--
function EditRequest(requestId)
{
	if (window.opener != null)
		window.opener.location.href = 'AddUpdatePlannerRequest.aspx?requestId=' + requestId;
	else
		window.location.href = 'AddUpdatePlannerRequest.aspx?requestId=' + requestId;

	window.close();
}

function ViewJob(jobId)
{
	if (window.opener != null)
	    window.opener.location.href = '../Job/job.aspx?jobId=' + jobId + getCSID();
	else
	    window.location.href = '../Job/job.aspx?jobId=' + jobId + getCSID();
	
	window.close();
}
//-->
</script>

<uc1:footer id="Footer1" runat="server"></uc1:footer>