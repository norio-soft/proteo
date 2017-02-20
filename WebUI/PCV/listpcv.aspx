<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>

<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Page language="c#" MasterPageFile = "~/default_tableless.master" Inherits="Orchestrator.WebUI.Resource.PCV.ListPCV" Codebehind="ListPCV.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="chklstVal" Namespace="P1TP.Components.Web.CheckBoxListValidator" Assembly="P1TP.Components"%> 

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>PCV Search</h1></asp:Content>




<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<script language="javascript" type="text/javascript">
    // Function to show the filter options overlay box
    function FilterOptionsDisplayShow() {
        $("#overlayedClearFilterBox").css({ 'display': 'block' });
        $("#filterOptionsDiv").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'block' });
    }

    function FilterOptionsDisplayHide() {
        $("#overlayedClearFilterBox").css({ 'display': 'none' });
        $("#filterOptionsDivHide").css({ 'display': 'none' });
        $("#filterOptionsDiv").css({ 'display': 'block' });
    }

  
</script>
    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
        <div class="infoPanel">
            <asp:Image ID="imgIcon" runat="server" ImageUrl="~/App_Themes/Orchestrator/img/MasterPage/icon-info.png" /><asp:Label cssclass="ControlErrorMessage" id="lblError" runat="server" Text="No PCVs found with specified criteria"/>
        </div>
    </asp:Panel>

               <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
             <div class="toolbarText">
        &nbsp Number of PCV's :: <asp:label id="lblPCVCount" runat="server">0</asp:label> &nbsp&nbsp;|&nbsp;&nbsp;Number of Pallets :: <asp:label id="lblPalletCount" runat="server">0</asp:label></div>  
   
	    </div>
    <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
		<legend>PCV Filter</legend>
		<table >
			<tr>
				<td class="formCellLabel">Run Id</td>
				<td class="formCellField"><asp:TextBox id="txtJobId" runat="server"></asp:TextBox></td>
				<td class="formCellLabel">Date From</td>
				<td class="formCellField" colspan="2"><telerik:RadDatePicker id="dteStartDate" Width="100" runat="server">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
			</tr>
			<tr>
				<td class="formCellLabel">Voucher No</td>
				<td class="formCellField"><asp:TextBox id="txtVoucherNo" runat="server"></asp:TextBox></td>
				<td class="formCellLabel">Date To</td>
				<td class="formCellField"><telerik:RadDatePicker id="dteEndDate" Width="100" runat="server">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
				<td class="formCellField"><asp:CustomValidator id="cfvEndDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Date from should fall before date to."><img src="../images/Error.gif" height="16" width="16" title="Date from should fall before date to." /></asp:CustomValidator></td>
			</tr>
			<tr>
				<td class="formCellLabel">Status</td>
				<td><asp:CheckBoxList id="chkPCVStatus" runat="server" RepeatDirection="Horizontal" RepeatColumns="2"></asp:CheckBoxList>
				<chklstVal:RequiredFieldValidatorForCheckBoxLists Display="Dynamic" id="rfvPCVStatus" EnableClientScript=false runat="server" ErrorMessage="Please select at least one PCV Status..."
           ControlToValidate="chkPCVStatus"><img src="../images/Error.gif" height='16' width='16' title='Please select at least one PCV Status...'></chklstVal:RequiredFieldValidatorForCheckBoxLists>
				</td>
				<td class="formCellLabel">Client</td>
				<td class="formCellField" style="vertical-align: top;" colspan="2"><telerik:RadComboBox ID="cboClient" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px" DataTextField="OrganisationName" DataValueField="IdentityId" ></telerik:RadComboBox></td>
			</tr>
			<tr>
				<td class="formCellLabel">Redemption Status <chklstVal:RequiredFieldValidatorForCheckBoxLists Display="Dynamic" EnableClientScript=true id="rfvPCVRedemptionStatus" runat="server" ErrorMessage="Please select at least one PCV Redemption Status..." ControlToValidate="chkPCVRedemptionStatus"><img src="../images/Error.gif" height='16' width='16' title='Please select at least one PCV Redemption Status...'></chklstVal:RequiredFieldValidatorForCheckBoxLists></td>
				<td class="formCellField"><asp:CheckBoxList id="chkPCVRedemptionStatus" DataTextField="Description" DataValueField="PCVRedemptionStatusId" runat="server" RepeatDirection="Horizontal" RepeatColumns="2"></asp:CheckBoxList>
				</td>
				<td class="formCellLabel">Client's Customer</td>
				<td class="formCellField" style="vertical-align: top;"><telerik:RadComboBox ID="cboClientsCustomer" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px" DataTextField="OrganisationName" DataValueField="IdentityId" ></telerik:RadComboBox></td>
			</tr>
			    <asp:Panel id="pnlRequiresDehire" runat="server" Visible="False">
				    <tr>
					    <td class="formCellLabel">View</td>
					    <td class="formCellField" colspan="3">
						    <asp:DropDownList id="cboView" runat="server">
							    <asp:ListItem Value="0" Text="Unsent PCVs"/>
							    <asp:ListItem Value="1" Text="Sent PCVs"/>
						    </asp:DropDownList>
					    </td>
				    </tr>
			    </asp:Panel>
	        </table>
            	<div class="buttonbar" align="left">
		<nfvc:NoFormValButton id="btnSearch_bottom" runat="server" cssclass="Button" text="Filter PCVs"></nfvc:NoFormValButton>
		<nfvc:NoFormValButton id="btnReport_bottom" runat="server" Text="Generate Report" Visible="False"></nfvc:NoFormValButton>
	</div>
            </div>
	
 


    <ComponentArt:Grid id="dgPCVs" 
        RunningMode="Client" 
        CssClass="Grid" 
        AllowEditing="False"
        HeaderCssClass="GridHeader" 
        FooterCssClass="GridFooter"
        
        GroupByTextCssClass="GroupByText"
        GroupingNotificationTextCssClass="GridHeaderText"
        GroupBySortAscendingImageUrl="group_asc.gif"
        GroupBySortDescendingImageUrl="group_desc.gif"
        GroupBySortImageWidth="10"
        GroupBySortImageHeight="10"
        GroupingPageSize = "25"
        ShowHeader="true"
        ShowSearchBox="True"
        SearchOnKeyPress="True"
        PageSize="25" 
        PagerStyle="Slider" 
        PagerTextCssClass="GridFooterText"
        PagerButtonWidth="41"
        PagerButtonHeight="22"
        SliderHeight="20"
        PreExpandOnGroup="false"
        SliderWidth="150" 
        SliderGripWidth="9" 
        SliderPopupOffsetX="20"
        SliderPopupClientTemplateId="SliderTemplate" 
        ImagesBaseUrl="../images/" 
        PagerImagesFolderUrl="../images/pager/"
        TreeLineImagesFolderUrl="../images/lines/" 
        TreeLineImageWidth="22" 
        TreeLineImageHeight="19" 
        Width="99%" runat="server"
        KeyboardEnabled ="true"
        AllowPaging="false"
        
        >
        <Levels>
            <ComponentArt:GridLevel 
                DataKeyField="PCVId"
                HeadingCellCssClass="HeadingCell" 
                HeadingRowCssClass="HeadingRow" 
                HeadingTextCssClass="HeadingCellText"
                DataCellCssClass="DataCell" 
                RowCssClass="Row" 
                SelectedRowCssClass="SelectedRow"
                SortAscendingImageUrl="asc.gif" 
                SortDescendingImageUrl="desc.gif" 
                SortImageWidth="10"
                SortImageHeight="10"
                GroupHeadingCssClass="GroupHeading" >
                <Columns>
                    <ComponentArt:GridColumn DataField="PCVId" HeadingText="PCVId" Visible="False" />
                    <ComponentArt:GridColumn DataField="ScannedFormId" Width="60" DataCellClientTemplateId="ScanTemplate" HeadingText="Scan"/>
                    <ComponentArt:GridColumn DataField="VoucherNo" TextWrap="true"  IsSearchable="true" HeadingText="Voucher No" DataCellClientTemplateId="VoucherNoTemplate" />
                    <ComponentArt:GridColumn DataField="ScannedFormId" TextWrap="true" HeadingText="View" DataCellClientTemplateId="ViewTemplate" FixedWidth=true />
                    <ComponentArt:GridColumn DataField="PointName" TextWrap="true"  HeadingText="Delivery Point" />
                    <ComponentArt:GridColumn DataField="DateOfIssue" HeadingText="Issue Date" FormatString="dd/MM/yy" />
                    <ComponentArt:GridColumn DataField="NoOfPalletsOutstanding" HeadingText="Owed" Align="Right" />
                    <ComponentArt:GridColumn DataField="NoOfSignings" HeadingText="Signed" Align="Right" />
                    <ComponentArt:GridColumn DataField="PCVRedemptionStatus" TextWrap="true" HeadingText="Redemption Status" />
                    <ComponentArt:GridColumn DataField="PCVStatus" HeadingText="Status" />
                    <ComponentArt:GridColumn DataField="RequiresScan" HeadingText="To Scan" DataCellClientTemplateId="ScannedTemplate" />
                    <ComponentArt:GridColumn DataField="JobOfIssue" HeadingText="Issuing Run" DataCellClientTemplateId="JobOfIssueTemplate" />
                    <ComponentArt:GridColumn DataField="JobId" HeadingText="Last Sent On" DataCellClientTemplateId="LastSentJobTemplate" />
                    <ComponentArt:GridColumn DataField="LoadNo" HeadingText="Load No"/>
                    <ComponentArt:GridColumn DataField="OrganisationName" TextWrap="true"  HeadingText="Client" />
                    <ComponentArt:GridColumn DataField="ClientsCustomer" TextWrap="true"  HeadingText="Customer" />
                    <ComponentArt:GridColumn DataField="ActualDeliveryDate" HeadingText="Delivery Date" FormatString="dd/MM/yy HH:mm" />
                    <ComponentArt:GridColumn DataField="FullName" Visible="false" HeadingText="Driver Name" />
                    <ComponentArt:GridColumn DataField="ScannedFormPDF" Visible="false" />
                </Columns>
            </ComponentArt:GridLevel>
        </Levels>

        <ClientTemplates>
          <ComponentArt:ClientTemplate ID="ScannedTemplate">
                    ## DataItem.GetMember("RequiresScan").Value == "true" ? "Yes" : "No" ##
    		   </ComponentArt:ClientTemplate>
    		   
    		   <ComponentArt:ClientTemplate ID="ScanTemplate">
                     <a href='javascript:OpenPCVScan(## DataItem.GetMember("ScannedFormId").Value ##)' >Scan</a>
    		   </ComponentArt:ClientTemplate>
        
            <ComponentArt:ClientTemplate ID="ViewTemplate" >
                    <a href='## DataItem.GetMember("ScannedFormPDF").Value ##' target="_blank" >## DataItem.GetMember("ScannedFormPDF").Value == "" ? "" : "View" ##</a>
            </ComponentArt:ClientTemplate>
            
            <ComponentArt:ClientTemplate ID="VoucherNoTemplate" >
                <Template>
                    <a href='#'>## DataItem.GetMember("VoucherNo").Value ##</a>
                </Template>
            </ComponentArt:ClientTemplate>

            <ComponentArt:ClientTemplate ID="JobOfIssueTemplate" >
                <Template>
					<a href="javascript:openResizableDialogWithScrollbars('../Traffic/JobManagement.aspx?wiz=true&jobId=## DataItem.GetMember("JobOfIssue").Value ##' + getCSID(),'1100','800');" title="Manage this job.">## DataItem.GetMember("JobOfIssue").Value ##</a>
                </Template>
            </ComponentArt:ClientTemplate>

            <ComponentArt:ClientTemplate ID="LastSentJobTemplate" >
                <Template>
					<a href="javascript:openResizableDialogWithScrollbars('../Traffic/JobManagement.aspx?wiz=true&jobId=## DataItem.GetMember("JobId").Value ##'+ getCSID(),'1100','800');" title="Manage this job.">## DataItem.GetMember("JobId").Value ##</a>
                </Template>
            </ComponentArt:ClientTemplate>

            <ComponentArt:ClientTemplate Id="SliderTemplate">
                <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td valign="top" style="padding:5px;">
                            <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                                            <tr>
                                                <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("VoucherNo").Value ##</nobr></div></td>
                                            </tr>
                                            <tr>
                                                <td colspan="2"></td>
                                            </tr>
                                        </table>    
                                    </td>
                                </tr>
                             </table>  
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" style="height:14px;background-color:#757598;">
                            <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td style="padding-left:5px;color:white;font-family:verdana;font-size:10px;">
                                    Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgPCVs.PageCount ##</b>
                                    </td>
                                    <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                    PCV <b>## DataItem.Index + 1 ##</b> of <b>## dgPCVs.RecordCount ##</b>
                                    </td>
                                </tr>
                            </table>  
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:grid>

    <div class="whitepacepusher"></div>

    <div class="buttonbar" align="left">
	    <nfvc:NoFormValButton id="btnSearch" runat="server" cssclass="Button" text="Filter PCVs" ></nfvc:NoFormValButton>
	    <nfvc:NoFormValButton id="btnReport" runat="server" Text="Generate Report"></nfvc:NoFormValButton>
    </div>

    <script language="javascript" type="text/javascript">
    <!--
        FilterOptionsDisplayHide();
        function OpenPCVScan(scannedFormId) {
            var pcvType = 1;
            var url = '<%=this.ResolveUrl("~/scan/wizard/ScanOrUpload.aspx")%>';
            url += "?ScannedFormTypeId=" + pcvType;
            url += "&ScannedFormId=" + scannedFormId;
            openResizableDialogWithScrollbars(url, 550, 500);
        }

	    function OpenFormViewer(scannedFormId)
	    {
	        if (scannedFormId == 0)
	            alert("No scan is on record for this PCV.");
	        else
		        openResizableDialogWithScrollbars('../scan/viewer.aspx?UseLocal=0&ScannedFormId=' + scannedFormId + '&PageNumber=0&TYPEID=PCV', 505, 488, null);
	    }
    	
    	/*
	    function getViewLink(formId)
	    {
    	    
	        if (formId != null && formId != 0 && formId != "")
	            return "<a href=\"/pdfs/" + formId + "\" target=\"_blank\">View</a>";
	        else
	            return "No Scan";    
    	        
	    }
	    */
    //-->
    </script>

</asp:Content>
