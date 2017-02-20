<%@ Page language="c#" Inherits="Orchestrator.WebUI.Job.MissingReturnReceiptNumber" Codebehind="MissingReturnReceiptNumber.aspx.cs" MasterPageFile="~/Default_tableless.master" Title="Goods Return Jobs Missing a Return Receipt Number" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
<h1>Goods Return Jobs</h1>
</asp:Content>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript">
        //Window Code
        function _showModalDialog(url, width, height, windowTitle) {
            MyClientSideAnchor.WindowHeight = height + "px";
            MyClientSideAnchor.WindowWidth = width + "px";
            MyClientSideAnchor.URL = url;
            MyClientSideAnchor.Title = windowTitle;

            var returnvalue = MyClientSideAnchor.Open();

            if (returnvalue == true) {
                document.Form1.submit();
            }

            return true;
        }
     </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
	
	<h2>A list of completed runs which are missing a return receipt number is shown below.</h2>
	
	<asp:DataGrid id="dgJobs" cssclass="DataGridStyle" runat="server" Width="100%" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="True" PagerStyle-HorizontalAlign="Right" PagerStyle-Mode="NumericPages" PageSize="20">
		<Columns>
			<asp:BoundColumn HeaderText="Run Id" DataField="JobId" SortExpression="JobId" ReadOnly="True"></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Client" DataField="OrganisationName" SortExpression="OrganisationName" ReadOnly="True"></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Load No" DataField="LoadNo" ></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Docket No" DataField="DocketNumber" ></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Product Name" DataField="ProductName" ></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Quantity Refused" DataField="QuantityRefused" ></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Refused At" DataField="Refused At" ></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Return Location" DataField="Return Location" ></asp:BoundColumn>
			<asp:BoundColumn HeaderText="Return Receipt Number" DataField="RefusalReceiptNumber" SortExpression="ReturnReceiptNumber"></asp:BoundColumn>
			<asp:TemplateColumn HeaderText="Delivery Date" SortExpression="BookedDateTime">
				<ItemTemplate>
					<asp:Label id="lblDeliveryDate" runat="server"></asp:Label>
				</ItemTemplate>
				<EditItemTemplate>
					<asp:Label id="lblDeliveryDate" runat="server"></asp:Label>
				</EditItemTemplate>
				<ItemStyle HorizontalAlign="Right"></ItemStyle>
			</asp:TemplateColumn>
			<asp:TemplateColumn HeaderText="Edit">
			    <ItemTemplate>
					<a id=lnkCallIn href='javascript:openDialogWithScrollbars("../Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobId=<%# DataBinder.Eval(Container.DataItem, "JobId") %>&instructionId=0"+ getCSID(), 1024, 860)'>Edit</A>
			    </ItemTemplate>
			</asp:TemplateColumn>
			<asp:EditCommandColumn HeaderText="Edit Receipt Number" Visible="false" ButtonType="LinkButton" EditText="Edit" UpdateText="Update" CancelText="Cancel"></asp:EditCommandColumn>
		</Columns>
		<HeaderStyle cssclass="DataGridListHead"></HeaderStyle>
		<PagerStyle cssclass="DataGridListPagerStyle"></PagerStyle>
		<ItemStyle cssclass="DataGridListItem" width="25%"></ItemStyle>
		<EditItemStyle cssclass="DataGridListItem" width="25%"></EditItemStyle>
		<AlternatingItemStyle cssclass="DataGridListItemAlt"></AlternatingItemStyle>
	</asp:DataGrid>
    
</asp:Content>