<%@ Page language="c#" Inherits="Orchestrator.WebUI.Traffic.JobManagement.tabPCVS" Codebehind="tabPCVS.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>
<%@ Register TagPrefix="uc1" TagName="callInTabStrip" Src="~/UserControls/DriverCallInTabStrip.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">PCVs</asp:Content>
 
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <link rel="stylesheet" type="text/css" href="/style/styles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newMasterpage.css" /> 

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/popaddress.js" type="text/javascript"></script>
    <script language="javascript" type="text/javascript">
	<!--
        //	    moveTo((screen.width - 1220) / 2, (screen.height - 870) / 2);
        //		resizeTo(1220, 870);
        window.focus();

        var returnUrlFromPopUp = window.location;
	//-->
	</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
	<table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">      
                    <div class="buttonbar" style="text-align:left;">
                        <table width="99%" border="0" cellpadding="0" cellspacing="2">
						    <tr>
							    <td><input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td><input type="button" style="width: 75px; display: <%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>" value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                <td width="100%" align="right"><iframe marginheight="0" marginwidth="0" frameborder="0" scrolling="no" width="360px" height="22px" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'></iframe></td>
						    </tr>
					    </table>
                    </div>
                    
                    <uc1:callInTabStrip id="CallInTabStrip1" runat="server" SelectedTab="0"></uc1:callInTabStrip>
                    <div style="padding-bottom:10px;"></div>
                    <asp:Panel id="pnlPCVs" runat="server">
                        <asp:datagrid id="dgPCVs" runat="server" Width="100%" AutoGenerateColumns="False" AllowPaging="False" AllowSorting="False" ShowFooter="True" CssClass="DataGridStyle">
                            <Columns>
                                <asp:TemplateColumn HeaderText="" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle">
                                    <ItemTemplate>
                                        <img src="../../../images/i_sideways.gif" alt="This PCV has been taken back for updating." width="15" height="15">
                                    </ItemTemplate>
                                </asp:TemplateColumn>
                                <asp:HyperLinkColumn Text="VoucherNo" DataNavigateUrlField="ScannedFormId" DataNavigateUrlFormatString="javascript:OpenPCVWindowForEdit({0});"
                                    DataTextField="VoucherNo" SortExpression="PCVId" HeaderText="Voucher No"></asp:HyperLinkColumn>
                                <asp:BoundColumn DataField="DateOfIssue" HeaderText="Date Of Issue" DataFormatString="{0:dd/MM/yy}"></asp:BoundColumn>
                                <asp:BoundColumn DataField="PCVStatusDescription" HeaderText="Status"></asp:BoundColumn>
                                <asp:BoundColumn DataField="NoOfPalletsOutstanding" HeaderText="Pallets Outstanding"></asp:BoundColumn>
                                <asp:BoundColumn DataField="NoOfSignings" HeaderText="Number of Signings"></asp:BoundColumn>
                                <asp:BoundColumn DataField="RedemptionStatus" HeaderText="Redemption Status"></asp:BoundColumn>					
                                <asp:TemplateColumn HeaderText="Issued or Attached?">
                                    <ItemTemplate>
                                    </ItemTemplate>
                                </asp:TemplateColumn>				
                            </Columns>
                            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
                            <ItemStyle CssClass="DataGridListItem"></ItemStyle>
                            <HeaderStyle CssClass="DataGridListHeadSmall"></HeaderStyle>
                            <PagerStyle HorizontalAlign="Right" PageButtonCount="2" CssClass="DataGridListPagerStyle"></PagerStyle>
                        </asp:datagrid>
			            <div class="buttonBar"><input type="button" value="Refresh" onclick="javascript:RefreshPCVs();" /></div>   
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
        
	<script language="javascript">
	function RefreshPCVs()
	{
	    location.href = location.href;
	}
	
	function OpenPCVWindow(jobId, pointId)
	{
	    var pcvType = 1;
	    var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
	    url += "?ScannedFormTypeId=" + pcvType;
	    url += "&JobId=" + jobId;
	    url += "&PointId=" + pointId;
	    openResizableDialogWithScrollbars(url, 550, 500);
	}

	function OpenPCVWindowForEdit(scannedFormId) {
	    var pcvType = 1;
	    var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
	    url += "?ScannedFormTypeId=" + pcvType;
	    url += "&ScannedFormId=" + scannedFormId;
	    openResizableDialogWithScrollbars(url, 550, 500);
	}
	
	var lastHighlightedRow = "";
	var lastHighlightedRowColour = "";
	
	function HighlightRow(row)
	{
		var rowElement;
		
		if (lastHighlightedRow != "")
		{
			rowElement = document.getElementById(lastHighlightedRow);
			rowElement.style.backgroundColor = lastHighlightedRowColour;
		}

		rowElement = document.getElementById(row);
		lastHighlightedRow = row;
		lastHighlightedRowColour = rowElement.style.backgroundColor;
		rowElement.style.backgroundColor = 'yellow';
	}

	</script>

</asp:Content>