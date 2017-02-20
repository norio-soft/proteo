<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.InstructionAttempts" Codebehind="InstructionAttempts.ascx.cs" %>
<asp:DataGrid ID="dgRedeliveries" Runat="server" BorderColor="#999999" 
	BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both" EnableViewState="False"
	AutoGenerateColumns="False" AllowSorting="False" width="100%">
	<SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
	<ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black" BackColor="#E0DFE3"></ALTERNATINGITEMSTYLE>
	<ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black" BackColor="#FFFFFF"></ITEMSTYLE>
	<HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True" Width="410px"></HEADERSTYLE>
	<FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
	<Columns>
		<asp:BoundColumn HeaderText="Reason" DataField="Description"></asp:BoundColumn>		
		<asp:BoundColumn HeaderText="Location" DataField="PointName"></asp:BoundColumn>
		<asp:BoundColumn HeaderText="Recorded By" DataField="CreateUserId"></asp:BoundColumn>
		<asp:BoundColumn HeaderText="Recorded At" DataField="CreateDate" DataFormatString="{0:dd/MM/yy HH:mm}"></asp:BoundColumn>
		<asp:TemplateColumn HeaderText="Original Booked Time">
			<ItemTemplate>
				<%# DataBinder.Eval(Container.DataItem, "OriginalBookedDateTime", "{0:dd/MM/yy}") %> <%# Convert.ToBoolean(DataBinder.Eval(Container.DataItem, "OriginalIsAnyTime")) ? "Anytime" : DataBinder.Eval(Container.DataItem, "OriginalBookedDateTime", "{0:HH:mm}") %>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:DataGrid>
