<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Inherits="Orchestrator.WebUI.Job.CancelJob" Codebehind="canceljob.aspx.cs" %>

<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <h1>Job Cancellation</h1>
    <h2>Please enter the relevant filter fields to find jobs that are 'Marked For Cancellation'</h2>
    <script language="javascript" type="text/javascript" src="<%=Page.ResolveUrl("~/script/popaddress.js")%>"></script>
    <div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
		<table style="background-color: white; border:solid 1pt black; " cellpadding="2">
			<tr>
				<td><span id="spnPointAddress"></span></td>
			</tr>
		</table>
	</div>
    <fieldset>
	    <legend>Filter Details</legend>
	    <table>
	        <tr>
			    <td class="formCellLabel">Control Area</td>
			    <td class="formCellInput"><asp:dropdownlist id="cboTrafficeArea" runat="server"></asp:dropdownlist></td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Client</td>
			    <td class="formCellInput"><telerik:RadComboBox ID="cboClient" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px"></telerik:RadComboBox></td>
		    </tr>
    		
		    <tr>
			    <td class="formCellLabel">Date From</td>
			    <td class="formCellInput"><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Date for the Booked Collection"></telerik:RadDateInput></td>
		    </tr>
		    <tr>
			    <td class="formCellLabel">Date To</td>
			    <td class="formCellInput"><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy" ToolTip="The Date for the Booked Collection"></telerik:RadDateInput></td>
		    </tr>
	    </table>
    </fieldset>
    <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
        <div class="MessagePanel" style="vertical-align:middle;">
            <table>
                <tr>
                    <td><asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /></td>
                    <td><asp:Label cssclass="ControlErrorMessage" id="lblNote" runat="server" /></td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <div class="buttonbar">
        <asp:button id="btnFilter1" runat="server" Text="Apply Filter" onclick="btnFilter_Click"></asp:button>
        <asp:Button id="btnReset1" runat="server" Text="Reset" onclick="btnReset_Click"></asp:Button>
    </div>
    <ComponentArt:Grid id="dgJobs" 
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
        ShowHeader="false"
        ShowSearchBox="False"
        SearchOnKeyPress="false"
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
        KeyboardEnabled ="true">
        <Levels>
            <ComponentArt:GridLevel 
                DataKeyField="JobId"
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
                    <ComponentArt:GridColumn DataField="JobStateId" Visible="False" />
                    <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="True" HeadingText="Job Id" DataCellServerTemplateId="JobId" Width="50" FixedWidth="true" />
                    <ComponentArt:GridColumn DataField="LoadNo" AllowGrouping="False" AllowSorting="True" HeadingText="Load Number" Width="68" Align="Right" FixedWidth="true" />
                    <ComponentArt:GridColumn DataField="JobDate" AllowGrouping="True" AllowSorting="True" HeadingText="Job Date" FormatString="dd/MM/yy" Width="70" FixedWidth="true" />
                    <ComponentArt:GridColumn DataField="OrganisationName" AllowGrouping="True" AllowSorting="True" HeadingText="Client" Width="200" />
                    <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Collections" DataCellServerTemplateId="Collections" Width="350" />
                    <ComponentArt:GridColumn DataField="JobId" AllowGrouping="False" AllowSorting="False" HeadingText="Deliveries" DataCellServerTemplateId="Deliveries" Width="350" />
                    <ComponentArt:GridColumn DataField="LastUpdateDate" Visible="false" />
                    <ComponentArt:GridColumn DataField="IssuedPCVs" Visible="False" />
                    <ComponentArt:GridColumn DataField="Requests" Visible="false" />
                    <ComponentArt:GridColumn DataField="ForCancellationReason" HeadingText="Reason" Width="150"/>
                    <ComponentArt:GridColumn DataField="CancelDate" HeadingText="Cancel Date" FormatString="dd/MM/yy" Width="70" FixedWidth="true" />
                    <ComponentArt:GridColumn DataField="CancelUser" HeadingText="Cancelled By"/>
                    <ComponentArt:GridColumn DataField="JobId" DataCellServerTemplateId="ReinstateJobTemplate" Width="80" FixedWidth="true" HeadingText=" " AllowGrouping="false" AllowSorting="false" AllowReordering="false" Align="Center"/>
                    <ComponentArt:GridColumn DataField="JobId" DataCellServerTemplateId="CancelJobTemplate" Width="80" FixedWidth="true" HeadingText=" " AllowGrouping="false" AllowSorting="false" AllowReordering="false" Align="Center"/>
                </Columns>
                <ConditionalFormats>
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 1" RowCssClass="stateBooked" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 2" RowCssClass="statePlanned" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 3" RowCssClass="stateInProgress" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 4" RowCssClass="stateCompleted" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 5" RowCssClass="stateBookingInIncomplete" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 6" RowCssClass="stateBookingInComplete" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 7" RowCssClass="stateReadyToInvoice" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 8" RowCssClass="stateInvoiced" />
                    <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('JobStateId').Value == 9" RowCssClass="stateCancelled" />
                </ConditionalFormats>
            </ComponentArt:GridLevel>
        </Levels>
        <ServerTemplates>
            <ComponentArt:GridServerTemplate Id="JobId">
                <Template>
				    <table width="60">
					    <tr>
						    <td colspan="3" align="center" align="right">
							    <a id="lnkManageJob" runat="server" title="Manage this job."></a>
						    </td>
					    </tr>
					    <tr>
						    <td width="33%"><img id="imgRequiresCallIn" runat="server" src="../images/error.gif" alt="Requires Call-in" style="VERTICAL-ALIGN: -3px;"></td>
						    <td width="33%"><img id="imgHasRequests" runat="server" src="../images/star.gif" alt="The job has at least one planner request attached to it, click this icon to review them." style="VERTICAL-ALIGN: -3px;cursor: hand"></td>
						    <td width="33%"><img id="imgHasNewPCVs" runat="server" width="10" height="10" src="../images/yellow_tick.gif" alt="PCVs were issued on this job" style="VERTICAL-ALIGN: -3px;"></td>
					    </tr>
				    </table>
                </Template>
            </ComponentArt:GridServerTemplate>
            <ComponentArt:GridServerTemplate ID="Collections">
                <Template>
					<asp:Table id="tblCollections" runat="server" Width="350px" CellSpacing="2"></asp:Table>
                </Template>
            </ComponentArt:GridServerTemplate>
            <ComponentArt:GridServerTemplate ID="Deliveries">
                <Template>
					<asp:Table id="tblDeliveries" runat="server" Width="350px" CellSpacing="2"></asp:Table>
                </Template>
            </ComponentArt:GridServerTemplate> 
            <ComponentArt:GridServerTemplate ID="ReinstateJobTemplate">
                <Template>
					<asp:button id="btnReinstate" runat="server" Text="Reinstate" CommandName="reinstate" />
                </Template>
            </ComponentArt:GridServerTemplate>        
            <ComponentArt:GridServerTemplate ID="CancelJobTemplate">
                <Template>
					<asp:button id="btnCancel" runat="server" Text="Cancel" CommandName="cancel" />
                </Template>
            </ComponentArt:GridServerTemplate>
        </ServerTemplates>
        <ClientTemplates>                               
            <ComponentArt:ClientTemplate Id="SliderTemplate">
                <table class="SliderPopup" cellspacing="0" cellpadding="0" border="0">
                    <tr>
                        <td valign="top" style="padding:5px;">
                            <table width="100%" cellspacing="0" cellpadding="0" border="0">
                                <tr>
                                    <td>
                                        <table cellspacing="0" cellpadding="2" border="0" style="width:255px;">
                                            <tr>
                                                <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("LoadNo").Value ##</nobr></div></td>
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
                                    Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgJobs.PageCount ##</b>
                                    </td>
                                    <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                    Job <b>## DataItem.Index + 1 ##</b> of <b>## dgJobs.RecordCount ##</b>
                                    </td>
                                </tr>
                            </table>  
                        </td>
                    </tr>
                </table>
            </ComponentArt:ClientTemplate>
        </ClientTemplates>
    </ComponentArt:Grid>
    <div class="whitespacepusher"></div>
    <div class="buttonbar">
	    <asp:button id="btnFilter" runat="server" Text="Apply Filter" onclick="btnFilter_Click"></asp:button>
	    <asp:Button id="btnReset" runat="server" Text="Reset" onclick="btnReset_Click"></asp:Button>
    </div>
</asp:Content>