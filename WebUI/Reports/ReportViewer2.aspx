<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer2.aspx.cs" Inherits="Orchestrator.WebUI.Reports.ReportViewer2" %>


<html xmlns="http://www.w3.org/1999/xhtml" id="html">
<head id="Head1" runat="server">
	<title>Report Viewer</title>
	<style type="text/css">			
		html#html, body#body, form#form1, div#content, center#center
		{	
			border: 0px solid black;
			padding: 0px;
			margin: 0px;
			height: 100%;
		}
		
		#Calendar1
		{
			font-family:Verdana;
			font-size:11px;
			background-color:Blue;
		}
		
		#calendar th
		{
			
		}
	</style>
</head>
<body>
	<form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server" />
	<div id="content" >
		<center id="center">
			
			<telerik:RadToolBar ID="RadToolBar1" runat="server" OnClientButtonClicked="OnClientButtonClickingHandler" OnButtonClick="RadToolBar1_ButtonClick" Width="100%" >
				<CollapseAnimation Duration="200" Type="OutQuint" />
				<Items>
					<telerik:RadToolBarButton runat="server" ToolTip="First Page" ImageUrl="Skins/Default/FirstPage.gif"
						DisabledImageUrl="Skins/Default/FirstPageDisabled.gif" PostBack="False">
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" ToolTip="Previous Page" ImageUrl="Skins/Default/PrevPage.gif"
						DisabledImageUrl="Skins/Default/PrevPageDisabled.gif" PostBack="False">
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" PostBack="False">
						<ItemTemplate>
							<asp:TextBox ID="TextBox1" Width="30px" runat="server" ToolTip="Current Page">
							</asp:TextBox>
						</ItemTemplate>
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server">
						<ItemTemplate>
							<asp:Label ID="Label1" Width="40px" runat="server" ToolTip="Total Pages">
							</asp:Label>
						</ItemTemplate>
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" ToolTip="Next Page" DisabledImageUrl="Skins/Default/NextPageDisabled.gif"
						ImageUrl="Skins/Default/NextPage.gif" PostBack="False">
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" ToolTip="Last Page" DisabledImageUrl="Skins/Default/LastPageDisabled.gif"
						ImageUrl="Skins/Default/LastPage.gif" PostBack="False">
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" ToolTip="Refresh" DisabledImageUrl="Skins/Default/RefreshDisabled.gif"
						ImageUrl="Skins/Default/Refresh.gif" PostBack="False">
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" PostBack="False">
						<ItemTemplate>
							<asp:DropDownList ID="DropDownList2" runat="server" ToolTip="Select Format">
								<asp:ListItem Value="Select">Select Print Format</asp:ListItem>
								<asp:ListItem Value="PDF" Selected="True">Adobe Reader(PDF)</asp:ListItem>
							</asp:DropDownList>
						</ItemTemplate>
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" ToolTip="Print" DisabledImageUrl="Skins/Default/PrintDisabled.gif"
						ImageUrl="Skins/Default/Print.gif" PostBack="False">
					</telerik:RadToolBarButton>
					<telerik:RadToolBarButton runat="server" Value="ExportData" DisabledImageUrl="Skins/Default/csv_small_d.png"
						ImageUrl="Skins/Default/csv_small.png" ToolTip="Export Data to SpreadSheet">
					</telerik:RadToolBarButton>
				</Items>
			</telerik:RadToolBar>
					
			<telerik:ReportViewer  ID="ReportViewer" runat="server" ShowZoomSelect="true" style="border:1px solid #ccc; height:1100px; width:100%; background-color: #eeeeee;" >
                <Resources ReportParametersNullText="All" />
                </telerik:ReportViewer>

		</center>
	</div>

	 <script type="text/javascript" language="javascript">

		var viewer = <%=ReportViewer.ClientID%>;
		var RPToolbar = document.getElementById(viewer.toolbarID);
		
		RPToolbar.style.display = "none";

		var toolbar = null;
		var firstP = null;
		var prevP = null;
		var lastP = null;
		var nextP = null;
		var exportItem = null;
		var select = null;
				
		function pageLoad()
		{ 
			toolbar = $find('<%=RadToolBar1.ClientID %>');
			firstP = toolbar.get_items().getItem(0);
			prevP = toolbar.get_items().getItem(1);
			nextP = toolbar.get_items().getItem(4);
			lastP = toolbar.get_items().getItem(5);
			exportItem = toolbar.get_items().getItem(9);

			prevP.disable();
			firstP.disable();
			exportItem.disable();
		}

		viewer.baseOnReportLoaded = viewer.OnReportLoaded;
		   
		viewer.OnReportLoaded = function()
		{  
			this.baseOnReportLoaded();
			var textbox = document.getElementById("RadToolBar1_i2_TextBox1");
			textbox.value = this.get_CurrentPage();
			var label = document.getElementById("RadToolBar1_i3_Label1");
			label.innerHTML = " of " + viewer.get_TotalPages();
			exportItem.enable();
		}
			
		function OnClientButtonClickingHandler(sender, eventArgs)
		{
			debugger;
			switch(eventArgs.get_item().get_index())
			{
				case 0:
					firstPage();
					break;
				case 1:
					previousPage();
					break;
				case 4:
					nextPage();
					break;
				case 5:
					lastPage();
					break;
				case 6:
					refreshReport();
					break;
				case 8:
					printReport();
					break;
				default:
					break;
			}
		}       

		function firstPage()
		{
			viewer.set_CurrentPage(1);
			prevP.disable();
			firstP.disable();
			nextP.enable();
			lastP.enable();
		}

		function previousPage()
		{
			if (viewer.get_CurrentPage() > 2)
			{
				viewer.set_CurrentPage(viewer.get_CurrentPage() - 1);
				nextP.enable();
				lastP.enable();
			}
			else if (viewer.get_CurrentPage() == 2)
			{
				viewer.set_CurrentPage(viewer.get_CurrentPage() - 1);
				prevP.disable();
				firstP.disable();
				nextP.enable();
				lastP.enable();
			}
		}

		function nextPage()
		{
			if (viewer.get_TotalPages() > viewer.get_CurrentPage() + 1)
			{
				viewer.set_CurrentPage(viewer.get_CurrentPage()+ 1);
				firstP.enable();
				prevP.enable();
			}
			else if (viewer.get_TotalPages() == viewer.get_CurrentPage() + 1)
			{
				viewer.set_CurrentPage(viewer.get_CurrentPage()+ 1);
				firstP.enable();
				prevP.enable();
				nextP.disable();
				lastP.disable();
			}
		}

		function lastPage()
		{
			viewer.set_CurrentPage(viewer.get_TotalPages());
			firstP.enable();
			prevP.enable();
			nextP.disable();
			lastP.disable();
		}

		function refreshReport()
		{
			viewer.RefreshReport();
		}

		function printReport()
		{
			var select = document.getElementById("RadToolBar1_i7_DropDownList2");

			if (select.value == "Select")
			{
				alert('Please Select Print Format');
			}
			else
			{
				viewer.PrintAs(select.value);
			}
		}

	</script>

	</form>
</body>
</html>
