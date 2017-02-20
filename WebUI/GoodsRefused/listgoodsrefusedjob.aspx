<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.GoodRefused.ListGoodsRefusedJob" Codebehind="listgoodsrefusedjob.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>

<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>List Goods Refused</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">

    <script language="javascript" type="text/javascript">
    <!--
        var hidSelectedRefusalJobs = document.getElementById('<%=hidSelectedRefusalJobs.ClientID %>');

        function GetCheckedRefusalJobs(gridItem, index, checkBoxElement) {
            jobId = gridItem.Cells[0].Value;
            hidSelectedRefusalJobs.value = "";

            // Check the current CheckBox that we have just dealt with 
            if (checkBoxElement.checked)
                hidSelectedRefusalJobs.value = hidSelectedRefusalJobs.value + jobId + ",";

            // Do the rest of the CheckBoxes
            var gridItem1;
            var itemIndex = 0;

            while (gridItem1 = dgReturnJobs.Table.GetRow(itemIndex)) {
                checked = gridItem1.Cells[5].Value;
                jobId = gridItem1.Cells[0].Value;

                if (checked) // If checked
                    hidSelectedRefusalJobs.value = hidSelectedRefusalJobs.value + jobId + ",";

                itemIndex++;
            }

            return true;
        }

        function _showModalDialog(url) {
            MyClientSideAnchor.WindowHeight = "580px";
            MyClientSideAnchor.WindowWidth = "532px";
            MyClientSideAnchor.URL = url;

            var returnvalue = MyClientSideAnchor.Open();

            if (returnvalue == true)
                document.all.Form1.submit();

            return true;
        }

        function _showModalDialog(url, height, width) {
            MyClientSideAnchor.WindowHeight = height + "px";
            MyClientSideAnchor.WindowWidth = width + "px";
            MyClientSideAnchor.URL = url;

            var returnvalue = MyClientSideAnchor.Open();

            if (returnvalue == true)
                document.all.Form1.submit();

            return true;
        }
    //-->

        $(document).ready(function () {
            SetFilterArea();
            FilterOptionsDisplayHide();
        });

        function SetFilterArea() {
            var width = $("#overlayedClearFilterBox").width();
            var height = $("#overlayedClearFilterBox").height();
            var position = $("#overlayedClearFilterBox").position();
            $("#overlayedIframe").css("width", width + 10);
            $("#overlayedIframe").css("height", height + 25);
            $("#overlayedIframe").css("top", position.top);
            $("#overlayedIframe").css("left", position.left);
        }

        function FilterOptionsDisplayShow() {
            $("#overlayedClearFilterBox").css({ 'display': 'block' });
            $("#filterOptionsDiv").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'block' });

            SetFilterArea();

            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "block";
        }

        function FilterOptionsDisplayHide() {
            $("#overlayedClearFilterBox").css({ 'display': 'none' });
            $("#filterOptionsDivHide").css({ 'display': 'none' });
            $("#filterOptionsDiv").css({ 'display': 'block' });
            var fr = document.getElementById("overlayedIframe");
            fr.style.display = "none";

        }

    </script>
    
</asp:Content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <h2>Find a Goods Refused/Return Run by completing any of the information below</h2>
    
	<input type="hidden" id="hidSelectedRefusalJobs" runat="server" value="" />
	
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
            <asp:Button id="btnReport" Text="Generate Report" runat="server" CausesValidation="false"></asp:Button>
            <asp:Button id="btnReportBottom" Text="Generate Report" runat="server" CausesValidation="false"></asp:Button>
	    </div>
        <!--Hidden Filter Options-->
    <iframe id="overlayedIframe" style="position: absolute; z-index: 95; background:white;"></iframe>
    <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block; padding-bottom:5px;">
    <fieldset>
        <legend>Filter Options</legend>
        <table>
            <tr style="display: none;">
                <td class="formCellLabel">Return Job</td>
                <td class="formCellField" colspan="5">
                    <p><asp:radiobuttonlist id="rdoGoodsRefusedFilterType" runat="server" RepeatDirection="Horizontal" AutoPostBack="True"></asp:radiobuttonlist></p>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="width: 140px;"><asp:label id="lblClient" runat="server">Client</asp:label></td>
                <td class="formCellField" colspan="5">
                    <telerik:RadComboBox ID="cboClient" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" ItemRequestTimeout="500" Width="355px"></telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel" style="width: 140px;">Date From</td>
                <td class="formCellField"><telerik:RadDatePicker id="dteStartDate" Width="100" runat="server">
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
                <td>
                    <asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please specify a start date." Display="Dynamic">
                        <img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." alt="" />
                    </asp:RequiredFieldValidator>
                </td>
                <td class="formCellLabel" style="width: 75px;">Date To</td>
                <td class="formCellField"><telerik:RadDatePicker id="dteEndDate" Width="100" runat="server" >
                <DateInput runat="server"
                dateformat="dd/MM/yy">
                </DateInput>
                </telerik:RadDatePicker></td>
                <td>
                    <asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please specify a end date." Display="Dynamic">
                        <img src="../images/Error.gif" height="16" width="16" title="Please specify a start date." alt="" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel"><asp:label id="lblClientReceipt" runat="server" Visible="False">Client Receipt/Return No</asp:label></td>
                <td class="formCellField" colspan="5">
                    <asp:radiobuttonlist id="rdoClientReceipt" runat="server" RepeatDirection="Horizontal" Visible="False"></asp:radiobuttonlist>
                    <asp:requiredfieldvalidator id="rfvClientReceipt" runat="server" ControlToValidate="rdoClientReceipt" ErrorMessage="Do you want to get all return jobs with receipt no. or without receipt no. or all. " Display="Dynamic" visible="false">
                        <img src="../images/Error.gif" height='16' width='16' title='Do you want to get all return jobs with receipt no. or without receipt no. or all. '>
                    </asp:requiredfieldvalidator>
                </td>							
            </tr>
        </table>
    </fieldset>
    	<div class="buttonBar" align="left">
		<nfvc:NoFormValButton id="btnSearch" Text="Search" runat="server" NoFormValList=""></nfvc:NoFormValButton>
		
	</div>
    </div>
					

	
    <asp:panel id="pnlReturnJobs" runat="server" Visible="False">
        <ComponentArt:Grid id="dgReturnJobs" 
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
        PageSize="100" 
        PagerStyle="Slider" 
        PagerTextCssClass="GridFooterText"
        PagerButtonWidth="41"
        PagerButtonHeight="22"
        SliderHeight="20"
        PreExpandOnGroup="true"
        SliderWidth="150" 
        SliderGripWidth="9" 
        SliderPopupOffsetX="20"
        SliderPopupClientTemplateId="SliderTemplate1" 
        ImagesBaseUrl="../images/" 
        PagerImagesFolderUrl="../images/pager/"
        TreeLineImagesFolderUrl="../images/lines/" 
        TreeLineImageWidth="22" 
        TreeLineImageHeight="19" 
        Width="100%" runat="server"
        DataKeyField="JobId"
        KeyboardEnabled ="true"
        LoadingPanelClientTemplateId="LoadingTemplate1"
        LoadingPanelEnabled="true" 
        LoadingPanelPosition="MiddleCenter"
        ClientSideOnCheckChanged="GetCheckedRefusalJobs">
            <Levels>
                <ComponentArt:GridLevel 
                DataKeyField="JobId"
                HeadingCellCssClass="HeadingCell" 
                HeadingRowCssClass="HeadingRow" 
                HeadingTextCssClass="HeadingCellText"
                DataCellCssClass="DataCell" 
                RowCssClass="Row" 
                SortAscendingImageUrl="asc.gif" 
                SortDescendingImageUrl="desc.gif" 
                SortImageWidth="10"
                SortImageHeight="10"
                GroupHeadingCssClass="GroupHeading">
                    <Columns>
                        <ComponentArt:GridColumn HeadingText="Run Id" DataField="JobId" DataCellClientTemplateId="URLJobIdTemplate1" />
                        <ComponentArt:GridColumn HeadingText="Client" DataField="OrganisationName"/>
                        <ComponentArt:GridColumn HeadingText="Load No" DataField="LoadNo"/>
                        <ComponentArt:GridColumn HeadingText="Completed Date" DataField="JobCompletionTime" FormatString="dd/MM/yy HH:mm" />
                        <ComponentArt:GridColumn HeadingText="Count" DataField="GoodsCount"/>
                        <Componentart:GridColumn HeadingText="Inc." ColumnType="CheckBox" DataField="Include"></componentart:GridColumn>
                    </Columns>
                </ComponentArt:GridLevel>
            </Levels>
            <ClientTemplates>
                <ComponentArt:ClientTemplate ID="URLJobIdTemplate1">
                    <a href="javascript:openDialogWithScrollbars('~/Traffic/JobManagement.aspx?wiz=true&jobId=## DataItem.GetMember("JobId").Value ##'+ getCSID(),'600','400');">
                    ## DataItem.GetMember("JobId").Value ##
                    </a>
                </ComponentArt:ClientTemplate>     
                <ComponentArt:ClientTemplate Id="SliderTemplate1">
                    <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td valign="top" style="padding:5px;">
                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td>
                                            <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                                                <tr>
                                                    <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;">LOAD NO:<nobr>## DataItem.GetMember("LoadNo").Value ##</nobr></div></td>
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
                                            Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgReturnJobs.PageCount ##</b>
                                        </td>
                                        <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                            Refusal Jobs<b>## DataItem.Index + 1 ##</b> of <b>## dgReturnJobs.RecordCount ##</b>
                                        </td>
                                    </tr>
                                </table>  
                            </td>
                        </tr>
                    </table>
                </ComponentArt:ClientTemplate></ClientTemplates></ComponentArt:Grid></asp:panel>

	<asp:label id="lblNote" runat="server" Visible="False" ForeColor="Red"></asp:label>
		
		
	<uc1:reportviewer id="reportViewer" runat="server" Visible="False" ViewerWidth="100%" ViewerHeight="800"></uc1:reportviewer>
	<script type="text/javascript">
	    FilterOptionsDisplayHide();
    </script>
</asp:Content>