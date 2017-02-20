<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Vehicle.VehicleRequiringService" Codebehind="VehicleRequiringService.aspx.cs" MasterPageFile="~/default_tableless.master"   Title="Services and MOT Expiry Report" %>

<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
    <!--
        var MyClientSideAnchor = null;
    
        function handleGridDoubleClick(item) {
            var id = item.GetMember("ResourceId").Value;
            var url = "addupdatevehicle.aspx?resourceId=" + id + "&wiz=true";
            _showModalDialog(url);
            return false;
        }
        function openInNewWindow(url) {
            _showModalDialog(url + "&wiz=true", 650, 580);
        }

        function GetShowFutureLink(resourceId) {
            var dateString = '<%= DateTime.UtcNow.AddDays(-1).ToString("ddMMyyyy")%>' + '0000';
            var output
            output = "<a href=\"javascript:ShowFuture('" + resourceId + "','1' ,'" + dateString + "');\">Resource Future</a>"
            return output
        }

        function ShowFuture(resourceId, resourceTypeId, fromDate) {
            var url = '../../Resource/Future.aspx?wiz=true&resourceId=' + resourceId + '&resourceTypeId=' + resourceTypeId + '&fromDateTime=' + fromDate;
            _showModalDialog(url, 300, 650);
        }


        function _showModalDialog(url) {
            MyClientSideAnchor = <%= MyClientSideAnchor.ClientID %>;
            MyClientSideAnchor.WindowHeight = "580px";
            MyClientSideAnchor.WindowWidth = "532px";
            MyClientSideAnchor.URL = url;
            var returnvalue = MyClientSideAnchor.Open();
            if (returnvalue == true)
                document.all.form1.submit();
            return true;
        }

        function _showModalDialog(url, height, width) {
            MyClientSideAnchor = <%= MyClientSideAnchor.ClientID %>;
            MyClientSideAnchor.WindowHeight = height + "px";
            MyClientSideAnchor.WindowWidth = width + "px";

            MyClientSideAnchor.URL = url;
            var returnvalue = MyClientSideAnchor.Open();
            if (returnvalue == true)
                document.all.form1.submit();
            return true;

        }

        function addVehicle() {
            _showModalDialog("addupdatevehicle.aspx?wiz=true");
        }
    //-->

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
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Services and MOT Expiry Report</h1></asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Services and MOTs due within the specified time period</h2>
                  <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">  
    <fieldset>
        <legend>Filter Options</legend>			
        <table>
	        <tr>
		        <td class="formCellLabel"><asp:Label id="lblSeviceDate" runat="server">Service/MOT Date:</asp:Label></td>
		        <td class="formCellField">
		            <telerik:RadDatePicker ID="rdiMOTDueDate" runat="server" Width="100">
                    <DateInput runat="server"
                    DateFormat="dd/MM/yyyy">
                    </DateInput>
                    </telerik:RadDatePicker>
		        </td>
	        </tr>
        </table>
    </fieldset>
        <div class="buttonbar">
        <asp:button id="btnGetVehicles" runat="server" Text="View Vehicles" />
    </div>  
    </div>
    

	
	<cs:WebModalAnchor ID="MyClientSideAnchor" Title="Update Trailer" runat="server"
        ClientSideSupport="true" WindowWidth="580" WindowHeight="600" Scrolling="false"
        URL="addupdatevehicle.aspx" HandledEvent="onclick" LinkedControlID="dgVehicles">
    </cs:WebModalAnchor>
		
	<ComponentArt:Grid id="dgVehicles" 
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
        ImagesBaseUrl="../../images/" 
        PagerImagesFolderUrl="../../images/pager/"
        TreeLineImagesFolderUrl="../../images/lines/" 
        TreeLineImageWidth="22" 
        TreeLineImageHeight="19" 
        Width="100%" runat="server"
        ClientSideOnDoubleClick="handleGridDoubleClick"
        DataKeyField="ResourceId"
        KeyboardEnabled ="true"
        LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
        >
        <Levels>
            <ComponentArt:GridLevel 
                 DataKeyField="ResourceId"
                 HeadingCellCssClass="HeadingCell" 
                 HeadingRowCssClass="HeadingRow" 
                 HeadingTextCssClass="HeadingCellText"
                 DataCellCssClass="DataCell" 
                 RowCssClass="Row"
                 AlternatingRowCssClass="AlternatingRow" 
                 SelectedRowCssClass="SelectedRow"
                 SortAscendingImageUrl="asc.gif" 
                 SortDescendingImageUrl="desc.gif" 
                 SortImageWidth="10"
                 SortImageHeight="10"
                 GroupHeadingCssClass="GroupHeading" >
                 <Columns>
                    <ComponentArt:GridColumn DataField="RegNo" DataCellClientTemplateId="RegNoTemplate" HeadingText="Reg"/>
                    <ComponentArt:GridColumn DataField="RegularDriver" HeadingText="Full Name" />
                    <ComponentArt:GridColumn DataField="VehicleManufacturer" HeadingText="Manufacturer"/>
                    <ComponentArt:GridColumn DataField="ChassisNo" HeadingText="Chassis No"/>
                    <ComponentArt:GridColumn DataField="MOTExpiry" HeadingText="MOT Expires" FormatString="dd/MM/yy"/>
                    <ComponentArt:GridColumn DataField="TelephoneNumber" HeadingText="Cab Phone"/>
			        <ComponentArt:GridColumn DataField="ServiceDueDate" HeadingText="Service Due Date"/>
			        <ComponentArt:GridColumn DataField="ResourceId" HeadingText="Service History" DataCellClientTemplateId="ServiceHistoryTemplate" />
                 </Columns>
                </ComponentArt:GridLevel>
        </Levels>
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="RegNoTemplate">
               		<a href="javascript:openInNewWindow('addupdatevehicle.aspx?resourceId=## DataItem.GetMember("ResourceId").Value ##')">
						## DataItem.GetMember("RegNo").Value ##				
					</a>
			</ComponentArt:ClientTemplate> 
			
			 <ComponentArt:ClientTemplate ID="ServiceHistoryTemplate">
               		<a href="javascript:openInNewWindow('VehicleService.aspx?vehicleId=## DataItem.GetMember("ResourceId").Value ##')">
						View Service History
					</a>
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
                            <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("RegNo").Value ##</nobr></div></td>
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
                         Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgVehicles.PageCount ##</b>
                         </td>
                         <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                         Organisation <b>## DataItem.Index + 1 ##</b> of <b>## dgVehicles.RecordCount ##</b>
                         </td>
                       </tr>
                       </table>  
                       </td>
                     </tr>
                     </table>
                   </ComponentArt:ClientTemplate>
                     <ComponentArt:ClientTemplate Id="LoadingFeedbackTemplate">
                          <table cellspacing="0" cellpadding="0" border="0">
                          <tr>
                            <td style="font-size:10px;font-family:Verdana;">Loading...&nbsp;</td>
                            <td><img src="../../images/spinner.gif" width="16" height="16" border="0"></td>
                          </tr>
                          </table>
                          </ComponentArt:ClientTemplate>
                          </ClientTemplates>
                  </ComponentArt:grid>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>