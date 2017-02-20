<%@ Page language="c#" MasterPageFile="~/WizardMasterPage.Master" Inherits="Orchestrator.WebUI.Traffic.JobManagement.bookingInPODs" Codebehind="bookingInPODs.aspx.cs" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI"
    TagPrefix="cc1" %>
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

    <link rel="stylesheet" type="text/css" href="/style/styles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newmasterpage.css" />

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/popaddress.js" type="text/javascript"></script>
    
    <cc1:Dialog ID="dlgDocumentWizard" URL="/scan/wizard/ScanOrUpload.aspx" Width="550" Height="550" AutoPostBack="true" Mode="Modal"
        runat="server" ReturnValueExpected="true">
    </cc1:Dialog>
    
    <script language="javascript" type="text/javascript">
	<!--
        window.focus();
        var returnUrlFromPopUp = window.location;
        // We have enums, please use them.
        var pcvType = <%=((int)Orchestrator.eFormTypeId.PCV) %>;
        var podType = <%=((int)Orchestrator.eFormTypeId.POD) %>;
        var dehireFormType = <%=((int)Orchestrator.eFormTypeId.DehireReceipt) %>;
        
        function OpenPODWindow(jobId, collectDropId) {
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&JobId=" + jobId;
            qs += "&CollectDropId=" + collectDropId;
            
            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }

        function OpenPODWindowForEdit(scannedFormId, jobId) {
        
            var podFormType = 2;
            var qs = "ScannedFormTypeId=" + podFormType;
            qs += "&JobId=" + jobId;
            qs += "&ScannedFormId=" + scannedFormId;
            
            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }
        
        function OpenDehireWindow(jobId, orderId, dehireReceiptId, receiptNumber)
        {
            var qs = "ScannedFormTypeId=" + dehireFormType;
            qs += "&JobId=" + jobId;
            qs += "&OrderId=" + orderId;
            qs += "&DeHireReceiptId=" + dehireReceiptId;
            qs += "&DHNo=" + receiptNumber;
            
           <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }
        
        function OpenDehireWindowForEdit(scannedFormId, jobId, orderId, dehireReceiptId, receiptNumber)
        {
            var qs = "ScannedFormTypeId=" + dehireFormType;
            qs += "&JobId=" + jobId;
            qs += "&ScannedFormId=" + scannedFormId;
            qs += "&OrderId=" + orderId;
            qs += "&DeHireReceiptId=" + dehireReceiptId;
            qs += "&DHNo=" + receiptNumber;
            
            <%=dlgDocumentWizard.ClientID %>_Open(qs);
        }
    	
	    function OpenFormViewer(scannedFormId)
	    {
		    openResizableDialogWithScrollbars('../../scan/viewer.aspx?UseLocal=0&ScannedFormId=' + scannedFormId + '&PageNumber=0&TYPEID=POD', 505, 488, null);
	    }
    	
	    function OpenPCVWindowForEdit(scannedFormId)
	    {
		    var qs = "ScannedFormTypeId=" + pcvType;
		    qs += "&ScannedFormId=" + scannedFormId;
            <%=dlgDocumentWizard.ClientID %>_Open(qs);
	    }
    	
	    function viewOrder(orderID)
        {
            var oManager = GetRadWindowManager();
               
            var oWnd = oManager.GetWindowByName("singleton");
            var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" +  orderID;
                
            oWnd.SetUrl(url);
            oWnd.SetTitle("Add/Update Order");
            oWnd.SetSize(1180, 900);
            oWnd.Show();
        }
        
        function openUpload(collectDropID, docketNo, sigDate)
        {
            var url = '<%=this.ResolveUrl("/document/upload.aspx?cdid=")%>' + collectDropID +  "&ticket=" + docketNo + "&sigDate=" + sigDate;
            openDialog(url, 405,388,null);
        }
        
    //-->
    </script>

</asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Booking in POD's</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

	<table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">      
                    <div class="buttonbar" style="text-align:left;">
                        <table border="0" cellpadding="0" cellspacing="2">
							<tr>
								<td><input type="button" style="width:75px" value="Details" onclick="javascript:window.location='<%=Page.ResolveUrl("~/job/job.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
									<td><input type="button" style="width:75px" value="Coms" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/driverCommunications.aspx?")%>wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
									<td><input type="button" style="width:75px" value="Call-In" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/DriverCallIn/CallIn.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
									<td><input type="button" style="width:75px" value="PODs" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/bookingInPODs.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
									<td><input type="button" style="width:75px;display:<%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>" value="Pricing" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/pricing2.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>    '+ getCSID();" /></td>
								<td width="100%" align="right"><iframe marginheight="0" marginwidth="0" frameborder="no" scrolling="no" width="300px" height="22px" style="visible:true;" src='<%=Page.ResolveUrl("~/traffic/jobManagement/CallInSelector.aspx?JobId=")%><%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'></iframe></td>
							</tr>
						</table>
                    </div>
                    <asp:label id="lblConfirmation" runat="server" cssclass="confirmation" visible="false" />
                    <asp:Panel id="pnlUpdateJob" runat="server">
                        <fieldset>
                            <legend>
	                            <strong>Update run status</strong>
                            </legend>
                            <table width="100%">
	                            <tr>
		                            <td>
			                            <asp:Checkbox runat="server" id="chkBookingInComplete" runat="server" text="Booking In Complete" TextAlign="left"></asp:Checkbox>
			                            &nbsp;&nbsp;
			                            <asp:Button id="btnUpdateJobState" runat="server" text="Update" onclick="btnUpdateJobState_Click" CssClass="buttonClass"></asp:Button>
		                            </td>
	                            </tr>
                            </table>
                        </fieldset>
                    </asp:Panel>
                    <table width="100%" cellpadding="0" cellspacing="0">
                        <tr>
                            <td width="100%" valign="top">
	                            <table width="100%" cellpadding="0" cellspacing="0">
		                            <tr>
			                            <td valign="top" width="100%">
				                            <fieldset>
					                            <legend><strong>Run details</strong></legend>
					                            <asp:Panel id="pnlJobDetail" runat="server">
						                            <asp:DataGrid id="dgCollectionDropSummary" runat="server" OnItemDataBound="dgCollectionDropSummary_ItemDataBound" AutoGenerateColumns="False" CssClass="Grid" CellPadding="3" CellSpacing="0" width="100%">
							                            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
							                            <ItemStyle CssClass="DataGridListItem" VerticalAlign="top"></ItemStyle>
							                            <HeaderStyle CssClass="HeadingRow" Height="22" VerticalAlign="middle" ></HeaderStyle>
							                            <Columns>
								                            <asp:BoundColumn DataField="InstructionID" HeaderText="Instruction ID" Visible="False"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="OrganisationName" HeaderText="Client" Visible="false"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="Description" HeaderText="Deliver To"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="PostTown" HeaderText="Town" Visible="false"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="BookedDateTime" HeaderText="Booked Date Time" DataFormatString="{0:dd/MM/yy HH:mm}"></asp:BoundColumn>
								                            <asp:TemplateColumn>
									                            <ItemTemplate>
										                            <asp:DataGrid id="dgCollectionDrop" runat="server" OnItemDataBound="dgCollectionDrop_ItemDataBound" AutoGenerateColumns="False" 
													                            CssClass="Grid" CellPadding="0" CellSpacing="0" OnPreRender="dgCollectionDrop_PreRender" Width="100%">
											                            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
											                            <ItemStyle CssClass="DataGridListItem"></ItemStyle>
											                            <HeaderStyle CssClass="DataGridListHeadSmall" Height="22" VerticalAlign="middle"></HeaderStyle>
											                            <Columns>
												                            <asp:BoundColumn DataField="CollectDropID" HeaderText="Collect Drop ID" Visible="False"></asp:BoundColumn>
												                            <asp:TemplateColumn HeaderText="OrderID">
												                                <ItemTemplate>
												                                    <a href='javascript:viewOrder(<%# DataBinder.Eval(Container.DataItem, "OrderID") %>)'><%# DataBinder.Eval(Container.DataItem, "OrderID") %></a>
												                                </ItemTemplate>
												                            </asp:TemplateColumn>
												                            <asp:BoundColumn DataField="ClientsCustomerReference" HeaderText="Ref"></asp:BoundColumn>
												                            <asp:BoundColumn DataField="DocketNumber" HeaderText="Docket"></asp:BoundColumn>
												                            <asp:BoundColumn DataField="Weight" HeaderText="Weight" DataFormatString="{0:F0}"></asp:BoundColumn>
												                            <asp:BoundColumn DataField="NoPallets" HeaderText="No Pallets"></asp:BoundColumn>
												                            <asp:BoundColumn DataField="RefusalReceiptNumber" HeaderText="Rcpt No"></asp:BoundColumn>
												                            <asp:BoundColumn DataField="ProductName" HeaderText="Product"></asp:BoundColumn>
												                            <asp:TemplateColumn HeaderText="POD Scanning">
													                            <ItemTemplate>
														                            <asp:HyperLink id="lnkPODScanning" runat="server"/><br />
														                            <asp:HyperLink ID="lnkPODUpload" runat="server"></asp:HyperLink>
													                            </ItemTemplate>
												                            </asp:TemplateColumn>
												                            <asp:TemplateColumn HeaderText="View">
													                            <ItemTemplate>
														                            <asp:HyperLink id="lnkPODView" Visible="False" Text="View" runat="server"  />
														                            
													                            </ItemTemplate>
												                            </asp:TemplateColumn>
												                            <asp:TemplateColumn HeaderText="Unassign POD" ItemStyle-HorizontalAlign="Center">
													                            <ItemTemplate>
														                            <asp:CheckBox id="chkUnassignPOD" runat="server" GroupName="UnassignPOD"/>
													                            </ItemTemplate>
												                            </asp:TemplateColumn>
												                            <asp:TemplateColumn HeaderText="Assign POD">
													                            <ItemTemplate>
													                            <uc:RdoBtnGrouper id="rbgCollectionDrop" runat="server" GroupName="CollectionDrop"/>
													                            </ItemTemplate>
												                            </asp:TemplateColumn>
												                            <asp:BoundColumn DataField="CreateUserID" HeaderText="Scanned By"></asp:BoundColumn>
												                            <asp:BoundColumn DataField="CreateDate" HeaderText="Scanned On" DataFormatString="{0:dd/MM/yy HH:mm}" />
											                            </Columns>
										                            </asp:DataGrid>
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:BoundColumn DataField="CollectDropDateTime" Visible="False"/>
							                            </Columns>
						                            </asp:DataGrid>
						                            <table width="100%">
							                            <tr>
								                            <td align="right">
									                            <asp:Label id="lblUnassignPODsError" cssclass="ControlErrorMessage" runat="server" Visible="False" />
									                            &nbsp;
									                            <asp:Button id="btnUnassignPODs" runat="server" Text="Unassign PODs" onclick="btnUnassignPODs_Click" />
								                            </td>
							                            </tr>
						                            </table>
					                            </asp:Panel>
				                            </fieldset>
				                            <asp:Panel ID="pnlUnattachedDeHire" runat="server" Visible="false">
				                                <div style="height:10px;"></div>
				                                <fieldset>
				                                    <legend>Pallet De-Hire's</legend>
				                                    <asp:DataGrid ID="dgUnattachedDeHire" runat="server" AutoGenerateColumns="false" CssClass="Grid" CellPadding="0" CellSpacing="0" Width="100%" OnItemDataBound="dgUnattachedDeHire_ItemDataBound">
				                                        <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
							                            <ItemStyle CssClass="DataGridListItem" VerticalAlign="top"></ItemStyle>
							                            <HeaderStyle CssClass="HeadingRow" Height="22" VerticalAlign="middle" ></HeaderStyle>
				                                        <Columns>
				                                            <asp:BoundColumn DataField="OrderID" HeaderText="OrderID" Visible="false"></asp:BoundColumn>
				                                            <asp:BoundColumn DataField="DehireReceiptId" HeaderText="DehireReceiptID" Visible="false"></asp:BoundColumn>
				                                            <asp:BoundColumn DataField="ReceiptNumber" HeaderText="ReceiptNumber" Visible="false" />
				                                            <asp:BoundColumn DataField="ScannedFormId" HeaderText="ScannedFormId" Visible="false"></asp:BoundColumn>
				                                            <asp:BoundColumn DataField="Description" HeaderText="Deliver To"></asp:BoundColumn>
				                                            <asp:BoundColumn DataField="BookedDateTime" HeaderText="Booked Date Time" DataFormatString="{0:dd/MM/yy HH:mm}"></asp:BoundColumn>
								                            <asp:TemplateColumn HeaderText="OrderID">
								                                <ItemTemplate>
								                                    <a href='javascript:viewOrder(<%# DataBinder.Eval(Container.DataItem, "OrderID") %>)'><%# DataBinder.Eval(Container.DataItem, "OrderID") %></a>
								                                </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:BoundColumn DataField="ClientsCustomerReference" HeaderText="Ref"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="DocketNumber" HeaderText="Docket"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="Weight" HeaderText="Weight" DataFormatString="{0:F0}"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="NoPallets" HeaderText="No Pallets"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="DehireReceiptNumber" HeaderText="Rcpt No"></asp:BoundColumn>
								                            <asp:TemplateColumn HeaderText="POD Scanning">
									                            <ItemTemplate>
										                            <asp:HyperLink id="lnkPODScanning" runat="server"/><br />
										                            <asp:HyperLink ID="lnkPODUpload" runat="server"></asp:HyperLink>
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:TemplateColumn HeaderText="View">
									                            <ItemTemplate>
										                            <asp:HyperLink id="lnkPODView" Visible="False" Text="View" runat="server"  />
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:TemplateColumn HeaderText="Unassign POD" ItemStyle-HorizontalAlign="Center" Visible="false">
									                            <ItemTemplate>
										                            <asp:CheckBox id="chkUnassignPOD" runat="server" GroupName="UnassignPOD"/>
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:TemplateColumn HeaderText="Assign POD" Visible="false">
									                            <ItemTemplate>
									                            <uc:RdoBtnGrouper id="rbgCollectionDrop" runat="server" GroupName="CollectionDrop"/>
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:BoundColumn DataField="CreateUserID" HeaderText="Scanned By"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="CreateDate" HeaderText="Scanned On" DataFormatString="{0:dd/MM/yy HH:mm}" />
							                            </Columns>
				                                    </asp:DataGrid>
				                                </fieldset>
				                            </asp:Panel>
				                            <asp:panel id="pnlPCVs" runat="server" visible="false">
		                                        <div style="height:10px;"></div>
                                                <fieldset>
                                                    <legend>PCV's Included on this Run</legend>
                                                    <asp:DataGrid ID="dgPCVs" runat="server" Width="100%" AutoGenerateColumns="False" AllowSorting="True" AllowPaging="False" CssClass="Grid" CellPadding="3" CellSpacing="0">
                                                        <Columns>
                                                            <asp:BoundColumn DataField="PCVId" HeaderText="PCV Id" Visible="false"></asp:BoundColumn>
                                                            <asp:TemplateColumn HeaderText="Voucher Number">
                                                                <ItemTemplate>
                                                                    <a href='javascript:OpenPCVWindowForEdit(<%# DataBinder.Eval(Container.DataItem, "ScannedFormId") %>)'><%# DataBinder.Eval(Container.DataItem, "VoucherNo") %></a>
                                                                </ItemTemplate>
                                                            </asp:TemplateColumn>
                                                            <asp:BoundColumn DataField="DeliveryPoint" HeaderText="Issued At" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="DateOfIssue" HeaderText="Issued On" DataFormatString="{0:dd/MM/yy HH:mm}" ItemStyle-Wrap="False"></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="NoOfPalletsOutstanding" HeaderText="Pallets Outstanding" ItemStyle-HorizontalAlign="Right"></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="PCVStatus" HeaderText="Status" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="RedemptionStatus" HeaderText="Redemption Status" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="ReasonForIssuing" HeaderText="Reason for Issuing" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="NoOfSignings" HeaderText="Number of Signing(s)" ></asp:BoundColumn>
					                                        <asp:BoundColumn DataField="CreateUserID" HeaderText="Scanned By"></asp:BoundColumn>
					                                        <asp:BoundColumn DataField="CreateDate" HeaderText="Scanned On" DataFormatString="{0:dd/MM/yy HH:mm}" />
                                                        </Columns>
                                                        <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
                                                        <ItemStyle CssClass="DataGridListItem" VerticalAlign="Top"></ItemStyle>
                                                        <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
                                                        <PagerStyle CssClass="DataGridListPagerStyle" Height="10"></PagerStyle>
                                                    </asp:DataGrid>
                                                </fieldset>
                                            </asp:panel>
                                            <asp:panel id="pnlRefusals" runat="server" visible="false">
                                                <div style="height:10px"></div>
                                                <fieldset>
                                                    <legend>Refusals For this Run</legend>
                                                
                                                    <asp:DataGrid id="dgRefusals" runat="server" cssclass="DataGridStyle" AutoGenerateColumns="False"
                                                        cellpadding="2" backcolor="White" border="1" AllowPaging="True" PagerStyle-Mode="NumericPages" pagesize="10" width="100%">
                                                        <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
                                                        <ItemStyle CssClass="DataGridListItem"  ></ItemStyle>
                                                        <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
                                                        <Columns>
                                                            <asp:BoundColumn DataField="RefusalId" HeaderText=" " Visible="False"></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="DocketNumber" HeaderText="Docket"></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="RefusalType" HeaderText="Refusal Type" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="RefusalStatus" HeaderText="Refusal Status" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="ProductName" HeaderText="Product" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="PackSize" HeaderText="Size" ></asp:BoundColumn>
                                                            <asp:BoundColumn DataField="QuantityRefused" HeaderText="Quantity" ></asp:BoundColumn>
                                                        </Columns>
                                                        <PagerStyle HorizontalAlign="Right" PageButtonCount="2" CssClass="DataGridListPagerStyle"></PagerStyle>
                                                    </asp:DataGrid>
                                                </fieldset>
                                            </asp:panel>
			                            </td>
		                            </tr>
	                            </table>
                            </td>
                            <td width="260" valign="top">
	                            <asp:Panel id="pnlUnassignedPODs" runat="server">
		                            <fieldset>
			                            <legend>
				                            <strong>PODs not assigned to any run</strong>
			                            </legend>
			                            <table width="260">
				                            <tr>
					                            <td>
						                            <asp:DataGrid id="dgUnassignedPODs" runat="server" cssclass="DataGridStyle" AutoGenerateColumns="False"
							                            cellpadding="2" backcolor="White" border="1" AllowPaging="True" PagerStyle-Mode="NumericPages" pagesize="10" OnItemDataBound="dgUnassignedPODs_ItemDataBound" OnPageIndexChanged="dgUnassignedPODs_Page" width="250">
							                            <AlternatingItemStyle CssClass="DataGridListItemAlt"></AlternatingItemStyle>
							                            <ItemStyle CssClass="DataGridListItem"></ItemStyle>
							                            <HeaderStyle CssClass="DataGridListHead"></HeaderStyle>
							                            <Columns>
								                            <asp:BoundColumn DataField="PODId" HeaderText="POD ID" Visible="False"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="TicketNo" HeaderText="Ticket No"></asp:BoundColumn>
								                            <asp:BoundColumn DataField="SignatureDate" HeaderText="Signature Date" DataFormatString="{0:dd/MM/yy}"></asp:BoundColumn>
								                            <asp:TemplateColumn HeaderText="View">
									                            <ItemTemplate>
										                            <asp:HyperLink id="lnkViewPOD" Text="View" runat="server" />
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
								                            <asp:TemplateColumn HeaderText="Add to drop" ItemStyle-HorizontalAlign="Center">
									                            <ItemTemplate>
										                            <uc:RdoBtnGrouper id="rbgPODId" runat="server" GroupName="POD" value='<%#DataBinder.Eval(Container.DataItem, "PODId") %>' />
									                            </ItemTemplate>
								                            </asp:TemplateColumn>
							                            </Columns>
							                            <PagerStyle HorizontalAlign="Right" PageButtonCount="2" CssClass="DataGridListPagerStyle"></PagerStyle>
						                            </asp:DataGrid>
					                            </td>
				                            </tr>
				                            <tr>
					                            <td align="right">
						                            <asp:Label id="lblAddToJobError" cssclass="ControlErrorMessage" runat="server" Visible="False" />
						                            &nbsp;
						                            <asp:Button id="btnAddToJob" runat="server" text="Add to drop" onclick="btnAddToJob_Click"></asp:Button>
					                            </td>	
				                            </tr>
			                            </table>	
		                            </fieldset>
	                            </asp:Panel>
                            </td>
                        </tr>
                    </table>
                    <asp:Panel id="pnlProgress" runat="server" Visible="False">
                        <table width="100%">
                            <tr>
	                            <td>
		                            <fieldset>
		                            <table border="0" cellpadding="5" cellspacing="10" width="99%">
			                            <tr>
				                            <td valign="top" width="50%" style="font-size:120%">
					                            <span class="bBold"><asp:HyperLink id="hlProgress" runat="server"/></span><span class="orangeBold">&raquo;</span><br>
					                            <asp:Label id="lblProgress" runat="server" Text="You are now able to price this job."/>
				                            </td>
				                            <td valign="top" width="50%" style="font-size:120%">
					                            &nbsp;
				                            </td>
			                            </tr>
		                            </table>
		                            </fieldset>
	                            </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </div>
            </td>
        </tr>
    </table>
                            
    <telerik:RadWindowManager ID="rmwAdmin" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false" Skin="Office2007">
        <Windows>
            <telerik:RadWindow ID="singleton" Width="600px" Height="900px" Modal="true" runat="server"/>
        </Windows>
    </telerik:RadWindowManager>

    
    
</asp:Content>
