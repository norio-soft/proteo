<%@ Reference Control="~/usercontrols/reportviewer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.POD.ViewHistoricPODs" Codebehind="viewhistoricpods.aspx.cs" %>
<%@ Register TagPrefix="cc2" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Historic POD Queries</h1></asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
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
    <style>
		.PageNumbers { FONT-SIZE: 10pt; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: none }
		.CurrentPage { FONT-WEIGHT: bold; FONT-SIZE: 10pt; COLOR: red; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: underline }
	</style>
	
	<h2>A list of Historic PODs is displayed below</h2>
    
                <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <div class="toolbarText"><asp:Label id="lblError" runat="server" Text="No PODs found with criteria entered" cssclass="ControlErrorMessage" Visible="False"/></div>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
	<table>
		<tr valign="top">
			<td>
			    <fieldset>
			        <legend><strong>Filtering Options</strong></legend>
			        <table>
			            <tr>
			                <td class="formCellLabel">Docket No</td>
			                <td class="formCellField"><asp:TextBox ID="docketNoTextbox" runat="server" width="150"></asp:TextBox></td>
			            </tr>
			            <tr>
			                <td class="formCellLabel">Order No</td>
			                <td class="formCellField"><asp:TextBox ID="orderIdTextbox" runat="server" width="150"></asp:TextBox></td>
			            </tr>							            
					    <tr>
						    <td class="formCellLabel">Scan Date</td>
						    <td class="formCellField"><telerik:RaddateInput id="dteDateOfScan" runat="server" dateformat="dd/MM/yy"></telerik:RaddateInput></td>
					    </tr>
					    <tr>
						    <td class="formCellLabel">Scan Date range from </td>
						    <td class="formCellField">
							    <table cellpadding="0" cellspacing="0">
								    <tr>
									    <td><telerik:RadDateInput id="dteDateOfScanFrom" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
									    <td class="formCellLabel">to</td>
									    <td><telerik:RadDateInput id="dteDateOfScanTo" runat="server" DisplayDateFormat="dd/MM/yy" dateformat="dd/MM/yy"></telerik:RadDateInput></td>
									    <td><asp:CustomValidator id="cfvDateOfScanFrom" runat="server" ControlToValidate="dteDateOfScanFrom" ErrorMessage="Date from should fall before date to."><img src="../images/Error.gif" height="16" width="16" title="Date from should fall before date to." /></asp:CustomValidator></td>
								    </tr>
							    </table>
						    </td>
					    </tr>
			        </table>
			    </fieldset>
			</td>
		</tr>
    </table>
    	<div class="buttonbar">
		<nfvc:NoFormValButton id="btnFilter" runat="server" text="Get PODS"></nfvc:NoFormValButton>
		<input type="reset" Text="reset" />
        <cs:webmodalanchor id="MyClientSideAnchor" title="Email PDF" runat="server" clientsidesupport="true"
            windowwidth="580" windowheight="532" scrolling="false" url="../scan/emailPDF.aspx" handledevent="onclick"
            linkedcontrolid="btnFilter"></cs:webmodalanchor>
	</div>
    </div>


	<ComponentArt:Grid id="dgPODList" 
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
        ShowSearchBox="false"
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
        Width="100%" runat="server"
        KeyboardEnabled ="true"
        LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
        
        EnableViewState="false">
        <Levels>
            <ComponentArt:GridLevel 
                 DataKeyField="PODId"
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
                 GroupHeadingCssClass="GroupHeading" 

                 >
                 <Columns>
                    <componentart:GridColumn DataField="OrderID" HeadingText="Order No" />
                    <ComponentArt:GridColumn DataField="DocketNo" HeadingText="Docket No" />
                    <ComponentArt:GridColumn DataField="PODUrl" DataCellClientTemplateId="PDFUrlTemplate" HeadingText="View POD"/>
                    <ComponentArt:GridColumn DataField="PODUrl" DataCellClientTemplateId="EmailPDFTemplate" HeadingText="Email POD" />
                    <ComponentArt:GridColumn DataField="HistoricPodId" Visible="false"  />
                 </Columns>
                </ComponentArt:GridLevel>
        </Levels>
        
        <ClientTemplates>
            <ComponentArt:ClientTemplate ID="PDFUrlTemplate">
       		    <a href='## DataItem.GetMember("PODUrl").Value ##' target="_blank">View</a>
			</ComponentArt:ClientTemplate>
			
			<ComponentArt:ClientTemplate ID="EmailPDFTemplate">
			    <a href="javascript:openEmailPDFWindow('## DataItem.GetMember("PODUrl").Value ##');">Email</a>
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
                            <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("SignatureDate").Value ##</nobr></div></td>
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
                         Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgPODList.PageCount ##</b>
                         </td>
                         <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                         Organisation <b>## DataItem.Index + 1 ##</b> of <b>## dgPODList.RecordCount ##</b>
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
                            <td><img src=../images/spinner.gif" width="16" height="16" border="0"></td>
                          </tr>
                          </table>
                          </ComponentArt:ClientTemplate>
                          </ClientTemplates>
                  </ComponentArt:Grid>			
	<uc1:ReportViewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800" EnableViewState="False"></uc1:ReportViewer>
	<script language="javascript" type="text/javascript">
	<!--
         //Window Code
        function _showModalDialog(url, width, height, windowTitle) {
            var myClientSideAnchor = <%=MyClientSideAnchor.ClientID %>
            myClientSideAnchor.WindowHeight= height + "px";
            myClientSideAnchor.WindowWidth= width + "px";
            
            myClientSideAnchor.URL = url;
            myClientSideAnchor.Title = windowTitle;
            var returnvalue = myClientSideAnchor.Open();

            if (returnvalue == true)
            {
                if(myClientSideAnchor.OutputData.indexOf('filterData') > 0)
                    {
                       top.document.getElementById('tsResource').src=top.document.getElementById('tsResource').src ;
                    }
                document.all.Form1.submit();
            }
            return returnvalue;	        
        }
        
        function openEmailPDFWindow(pdfLocation)
        {
            var url = "../scan/emailPDF.aspx?type=POD&pdfLocation=" + pdfLocation;
            _showModalDialog(url, 500, 221, "Email PDF");
        }
    //-->

    FilterOptionsDisplayHide();
	</script>
</asp:Content>
