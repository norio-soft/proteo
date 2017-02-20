<%@ Page language="c#" Inherits="Orchestrator.WebUI.administration.audit.sentFaxes" Codebehind="sentFaxes.aspx.cs" MasterPageFile="~/default_tableless.Master"   %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ContentPlaceHolderID="Header" runat="server"></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Sent Faxes</h1></asp:Content>

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
    <h2>Below is a list of sent faxes</h2>
    <asp:Label id="lblConfirmation" runat="server" visible="false" cssclass="confirmation"></asp:Label>
                    <div class="ToolbarBlue" style="height: 24px; padding: 1px 1px 1px 3px; background-position: top;">
		    <div class="overlayedFilterIconOff" id="filterOptionsDiv" onclick="FilterOptionsDisplayShow()" style="display: none;">Show filter Options</div>
		    <div class="overlayedFilterIconOn" id="filterOptionsDivHide" onclick="FilterOptionsDisplayHide()">Close filter Options</div>
	    </div>
        <!--Hidden Filter Options-->
        <div class="overlayedFilterBoxBelowText" id="overlayedClearFilterBox" style="display: block;">
	<fieldset>
		<legend>Filter Options</legend>
        <table id="mainTable" runat="server">
			<tr>
				<td class="formCellLabel">Start Date</td>
				<td class="formCellField">
					<table>
						<tr>
							<td><telerik:RadDateInput id="dteStartDate" runat="server" dateFormat="dd/MM/yy"></telerik:RadDateInput></td>
							<td><asp:RequiredFieldValidator id="rfvStartDate" runat="server" ControlToValidate="dteStartDate" ErrorMessage="Please enter a start date." Display="Dynamic"><img src="../../images/Error.gif" height='16' width='16' title='Please enter a start date.'></asp:RequiredFieldValidator></td>
						</tr>
					</table>
				</td>
			</tr>
			<tr>
				<td class="formCellLabel">End Date</td>
				<td class="formCellField">
					<table>
						<tr>
							<td><telerik:RadDateInput id="dteEndDate" runat="server" DateFormat="dd/MM/yy"></telerik:RadDateInput></td>
							<td><asp:RequiredFieldValidator id="rfvEndDate" runat="server" ControlToValidate="dteEndDate" ErrorMessage="Please enter an end date." Display="Dynamic"><img src="../../images/Error.gif" height='16' width='16' title='Please enter an end date.'></asp:RequiredFieldValidator></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</fieldset>
    	<div class="buttonbar">
		<asp:Button id="btnFilter" width="75px" runat="server" Text="Filter"></asp:Button>
	</div>
    </div>

    <asp:Label id="lblNote" Runat="server" Text=""></asp:Label>
    <asp:panel id="pnlFaxes" runat="server" visible="false">
        <ComponentArt:Grid id="dgFaxes" 
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
            DataKeyField="TransactionId"
            KeyboardEnabled ="true"
            LoadingPanelClientTemplateId="LoadingTemplate2"
            LoadingPanelPosition="MiddleCenter">
            <Levels>
                <ComponentArt:GridLevel 
                 DataKeyField="TransactionId"
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
                        <ComponentArt:GridColumn HeadingText="Fax Type" DataField="Description" />
                        <ComponentArt:GridColumn HeadingText="Sent To" DataField="FaxNumber"  />
                        <ComponentArt:GridColumn HeadingText="Sent By" DataField="UserName"  />
                        <ComponentArt:GridColumn HeadingText="Sent On" DataField="CreateDate" FormatString="dd/MM/yy HH:mm" />
                        <ComponentArt:GridColumn HeadingText="View Sent Fax" DataCellClientTemplateId="ViewFaxTemplate" />
                        <ComponentArt:GridColumn HeadingText="Resend Fax" DataCellServerTemplateId="ResendFaxTemplate" />
                        <ComponentArt:GridColumn HeadingText="TransactionId" DataField="TransactionId" visible="false"/>
                    </Columns>
                </ComponentArt:GridLevel>
            </Levels>
            <ServerTemplates >
                <ComponentArt:GridServerTemplate  ID="ResendFaxTemplate">
                    <Template>
                        <asp:Button id="btnResend" runat="server" Text="Resend Fax" onclick="btnResend_Click" ></asp:Button>
                    </Template>
                </ComponentArt:GridServerTemplate>
            </ServerTemplates>
            <ClientTemplates>
                <ComponentArt:ClientTemplate Id="ViewFaxTemplate">
                    <Template>
                        <a href="javascript:openDialogWithScrollbars('sentFaxes.aspx?wiz=true&GETFAXIMAGE=true&transactionId=## DataItem.GetMember("TransactionId").Value ##', 700, 600)">
                            View Sent Fax
                        </a>                       
                    </Template>
                </ComponentArt:ClientTemplate>
                <ComponentArt:ClientTemplate Id="SliderTemplate2">
                    <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                        <tr>
                            <td valign="top" style="padding:5px;">
                                <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                    <tr>
                                        <td>
                                            <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                                                <tr>
                                                    <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;">Sent To:<nobr>## DataItem.GetMember("FaxNumber").Value ##</nobr></div></td>
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
                                        Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgFaxes.PageCount ##</b>
                                        </td>
                                        <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                        Faxes<b>## DataItem.Index + 1 ##</b> of <b>## dgFaxes.RecordCount ##</b>
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
        </ComponentArt:grid>
    </asp:panel>
    <script type="text/javascript">
        FilterOptionsDisplayHide();
    </script>
</asp:Content>
