<%@ Page language="c#" Inherits="Orchestrator.WebUI.administration.audit.ActivityLog" Codebehind="ActivityLog.aspx.cs" MasterPageFile="~/default_tableless.Master" Title="Activity Log" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Activity Log</h1></asp:Content>
    
<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBox" id="overlayedClearFilterBox" style="display: block;">
                <fieldset>
                <legend>Filter Options</legend>    
		<table>
			<tr>
				<td>
					User
				</td>
				<td>
					<asp:TextBox id="txtUser" runat="server"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td>
					Action
				</td>
				<td>
					<asp:DropDownList id="cboAction" runat="server"></asp:DropDownList>
				</td>
			</tr>
			<tr>
				<td>
					Run
				</td>
				<td>
					<asp:TextBox id="txtJob" runat="server"></asp:TextBox>
					
				</td>
				<td>
					<asp:CustomValidator id="cfvJob" runat="server" ControlToValidate="txtJob" ErrorMessage="Please supply a numeric value for JobId"><img src="~/images/Error.gif" height="16" width="16" title="The start date must be before the end date." /></asp:CustomValidator>
				</td>
			</tr>
			<tr>
				<td>
					Date From
				</td>	
				<td>
				    <telerik:RadDatePicker runat="server" ID="dteDateFrom" Width="100">
                    <DateInput runat="server"
                    DisplayDateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
				</td>
				<td>
					<asp:CustomValidator id="cfvDateFrom" runat="server" ControlToValidate="dteDateFrom" ErrorMessage="Please supply a numeric value for JobId" />
				</td>
			</tr>
			<tr>
				<td>	
					Date To
				</td>
				<td>
				    <telerik:RadDatePicker runat="server" id="dteDateTo" Width="100">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yy">
                    </DateInput>
                    </telerik:RadDatePicker>
				</td>
			</tr>
		</table>
        </fieldset>
        	    <div class="buttonBar">
			<asp:Button id="btnFilter" width="75px" runat="server" Text="Filter"/>
			<!-- <input type="reset" width="75px" value="Reset"> -->
		</div>
        </div>
	    

		
		<table width="100%">
			<tr>
				<td><asp:Label id="lblError" runat="server" cssclass="ControlErrorMessage" Text="No activity recorded with criteria provided" Visible="False"/></td>
			</tr>
			<tr>
				<td width="100%">
					<ComponentArt:Grid id="dgActivityLog" 
                        RunningMode="Client" 
                        CssClass="Grid" 
                        HeaderCssClass="GridHeader" 
                        FooterCssClass="GridFooter"
                        
                        GroupByTextCssClass="GroupByText"
                        GroupingNotificationTextCssClass="GridHeaderText" 
                        GroupBySortAscendingImageUrl="group_asc.gif"
                        GroupBySortDescendingImageUrl="group_desc.gif"
                        GroupBySortImageWidth="10"
                        GroupBySortImageHeight="10"
                        GroupingPageSize = "100"
                        ShowHeader="true"
                        ShowSearchBox="False"
                        SearchOnKeyPress="False"
                        PageSize="25" 
                        PagerStyle="Slider" 
                        PagerTextCssClass="GridFooterText"
                        PagerButtonWidth="41"
                        PagerButtonHeight="22"
                        SliderHeight="20"
                        PreExpandOnGroup="true"
                        SliderWidth="150" 
                        SliderGripWidth="9" 
                        SliderPopupOffsetX="20"
                        SliderPopupClientTemplateId="SliderTemplate2" 
                        ImagesBaseUrl="../../images/" 
                        PagerImagesFolderUrl="../../images/pager/"
                        TreeLineImagesFolderUrl="../../images/lines/" 
                        TreeLineImageWidth="22" 
                        TreeLineImageHeight="19" 
                        Width="100%" runat="server"
                        DataKeyField="CreateDate"
                        KeyboardEnabled ="true"
                        LoadingPanelClientTemplateId="LoadingTemplate2"
                        LoadingPanelPosition="MiddleCenter"
                         >
                        <Levels>
                        <ComponentArt:GridLevel 
                             DataKeyField="CreateDate"
                             HeadingCellCssClass="HeadingCell" 
                             HeadingRowCssClass="HeadingRow" 
                             HeadingTextCssClass="HeadingCellText"
                             SelectedRowCssClass="SelectedRow"
                             DataCellCssClass="DataCell" 
                             RowCssClass="Row" 
                             SortAscendingImageUrl="asc.gif" 
                             SortDescendingImageUrl="desc.gif" 
                             SortImageWidth="10"
                             SortImageHeight="10"
                             GroupHeadingCssClass="GroupHeading">
                         <Columns>
                         <ComponentArt:GridColumn HeadingText="Event Date" FormatString="dd/MM/yy HH:mm" DataField="CreateDate" />
                         <ComponentArt:GridColumn HeadingText="User Name" DataField="UserName"  />
                         <ComponentArt:GridColumn HeadingText="Event Desciption" DataField="Description"  />
                         <ComponentArt:GridColumn HeadingText="" DataField="ObjectId" />
                         <ComponentArt:GridColumn HeadingText="Email Recipient" DataField="EmailRecipient" visible="false" />
                         <ComponentArt:GridColumn HeadingText="Email Report Type" DataField="EmailReportType" visible="false"/>
                         <ComponentArt:GridColumn HeadingText="Fax Number" DataField="FaxNumber" visible="false"/>
                         <ComponentArt:GridColumn HeadingText="Report Type" DataField="FaxNumber" visible="false"/>
                         <ComponentArt:GridColumn HeadingText="SMS Number" DataField="SMSRecipient" visible="false"/>
                         <ComponentArt:GridColumn HeadingText="SMS Text" DataField="SMSMessageText" visible="false"/>
                         <ComponentArt:GridColumn HeadingText="Organisation Name" DataField="OrganisationName" visible="false"/>
                         </Columns>
                     </ComponentArt:GridLevel>
                </Levels>
             <ClientTemplates>
                 <ComponentArt:ClientTemplate Id="SliderTemplate2">
                         <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                         <tr>
                           <td valign="top" style="padding:5px;">
                           <table width="100%" cellspacing="0" cellpadding="0" border="0">
                           <tr>
                             <td>
                             <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                             <tr>
                                <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;">Create Date:<nobr>## DataItem.GetMember("CreateDate").Value ##</nobr></div></td>
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
                             Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgActivityLog.PageCount ##</b>
                             </td>
                             <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                             Sub Contract Invoice Job<b>## DataItem.Index + 1 ##</b> of <b>## dgActivityLog.RecordCount ##</b>
                             </td>
                           </tr>
                           </table>  
                           </td>
                         </tr>
                         </table>
                       </ComponentArt:ClientTemplate>
                       
                       <ComponentArt:ClientTemplate Id="LoadingTemplate2">
                          <table cellspacing="0" cellpadding="0" border="0">
                          <tr>
                            <td style="font-size:10px;font-family:Verdana;">Loading...&nbsp;</td>
                            <td><img src="../../images/spinner.gif" width="16" height="16" border="0"></td>
                          </tr>
                          </table>
                        </ComponentArt:ClientTemplate>
                    </ClientTemplates>
             </ComponentArt:Grid>
				</td>
			</tr>
		</table>
<script type="text/javascript">
    FilterOptionsDisplayHide();
</script>
</asp:Content>