<%@ Page language="c#" ValidateRequest="false" Inherits="Orchestrator.WebUI.Traffic.JobManagement.driverCommunications" Codebehind="driverCommunications.aspx.cs" MasterPageFile="~/WizardMasterPage.master" AutoEventWireup="True" %>

<%@ MasterType TypeName="Orchestrator.WebUI.WizardMasterPage" %>

<%@ Register TagPrefix="uc1" TagName="jobstateindicator" Src="~/UserControls/jobStateIndicator.ascx" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
<style type="text/css">
body
{
    min-width:1070px;
}

.masterpagepopup_contentBottom
{
    margin-top: -10px;
}
</style>


    
    <telerik:RadCodeBlock runat="server">
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
        <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
        <script src="/script/jquery-migrate-1.2.1.js"></script>
        <script src="/script/show-modal-dialog.js"></script>
        <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
        <script language="javascript" src="/script/popaddress.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
	    <!--
	        moveTo((screen.width - 1220) / 2, (screen.height - 870) / 2);
		    resizeTo(1220, 870);
		    window.focus();

		    var returnUrlFromPopUp = window.location;
		    var jobId = <%=Request.QueryString["jobId"]%>
    		
	    //-->
	    </script>
    	

        
        <style type="text/css">
           .stateBooked  td.DataCell 
            { 
              cursor: default;
              padding: 3px; 
              padding-top: 2px; 
              padding-bottom: 1px; 
              border-bottom: 1px solid #EAE9E1; 
              font-family: verdana; 
              font-size: 11px; 
              background-color: <%=Orchestrator.WebUI.Utilities.GetJobStateColourForHTML(Orchestrator.eJobState.Booked) %>; 
              height:25px; 
            }
        </style>
    </telerik:RadCodeBlock>
    </asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <form id="Form2">
        <cc1:Dialog ID="dlgCommunicateThis" runat="server" URL="/Traffic/communicateThis.aspx" Width="500" Height="635" AutoPostBack="True" ReturnValueExpected="False" Mode="Modal" />
        <cc1:Dialog ID="dlgPrinterFriendly" runat="server" URL="/Traffic/JobManagement/printerFriendlyCommunication.aspx" Width="1024" Height="768" AutoPostBack="False" ReturnValueExpected="False" Mode="Normal" />
        <div class="buttonbar" style="text-align:left;">
                            <table border="0" cellpadding="0" cellspacing="2">
						        <tr>
							        <td><input type="button" style="width:75px" value="Details" onclick="javascript:window.location='<%=Page.ResolveUrl("~/job/job.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
						            <td><input type="button" style="width:75px" value="Coms" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/driverCommunications.aspx?")%>wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
						            <td><input type="button" style="width:75px" value="Call-In" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/DriverCallIn/CallIn.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
						            <td><input type="button" style="width:75px" value="PODs" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/bookingInPODs.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
							        <td><input type="button" style="width:75px;display:<%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>" value="Pricing" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/pricing2.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
							        <td width="100%" align="right"><iframe marginheight="0" marginwidth="0" frameborder="no" scrolling="no" width="300px" height="22px" visible="true" src='<%=Page.ResolveUrl("~/traffic/jobManagement/CallInSelector.aspx?JobId=")%><%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'></iframe></td>
						        </tr>
					        </table>
                        </div>
        

                        
                            <asp:Label ID="lblConfirmation" runat="server"></asp:Label>
                            
                                
                                    <div style="float:left">
                                        <fieldset style="height:120px; width:200px; margin-right:10px;">
	                                        <legend>Run</legend>
	                                        <table width="100%" >
		                                        <tr>
			                                        <td width="50%">Run Id:</td>
			                                        <td><span style="FONT-WEIGHT:bold; font-size:12px"><asp:Label id="lblJobId" runat="server"></asp:Label></span></td>
		                                        </tr>
		                                        <tr>
			                                        <td>Run State</td>
			                                        <td><uc1:jobstateindicator id="ucJobStateIndicator" runat="server"></uc1:jobstateindicator></td>
		                                        </tr>
		                                        <tr>
			                                        <td>Run Type</td>
			                                        <td><asp:Label id="lblJobType" runat="server"></asp:Label></td>
		                                        </tr>
		                                        <tr>
			                                        <td nowrap="false">Current Traffic Area</td>
			                                        <td><asp:Label id="lblCurrentTrafficArea" runat="server"></asp:Label></td>
		                                        </tr>
		                                        <tr>
			                                        <td>Taking PCVs</td>
			                                        <td><span style="FONT-WEIGHT:BOLD; FONT-SIZE:12px; COLOR:RED"><asp:Label id="lblTakingPCVs" runat="server"></asp:Label></span></td>
		                                        </tr>
	                                        </table>
                                        </fieldset>
                                    </div>
                                    <div style="width: 15%; vertical-align: top; float:left">
                                        <fieldset style="height:120px;width:200px;">
	                                        <legend>Run Details</legend>
	                                        <table>
		                                        <tr>
			                                        <td width="50%"><asp:Label id="lblLoadNumberText" runat="server"></asp:Label></td>
			                                        <td><asp:Label id="lblLoadNumber" runat="server"></asp:Label></td>
		                                        </tr>
		                                        <tr>
			                                        <td>Return Reference Number</td>
			                                        <td><asp:Label id="lblReturnReferenceNumber" runat="server"></asp:Label></td>
		                                        </tr>
		                                        <tr>
			                                        <td>Number of Pallets</td>
			                                        <td><asp:Label id="lblNumberOfPallets" runat="server"></asp:Label></td>
		                                        </tr>
		                                        <tr>
			                                        <td nowrap="nowrap">Marked for Cancellation</td>
			                                        <td><asp:Label id="lblMarkedForCancellation" runat="server"></asp:Label></td>
		                                        </tr>
	                                        </table>
                                        </fieldset>
                                    </div>
                                    

                                        <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
                                            <div class="MessagePanel">
                                                <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" /><asp:Label ID="lblMessage" runat="server" ></asp:Label>
                                            </div>
                                        </asp:Panel>
                                   
                                        <div style="height:10px;"></div>
                                        <fieldset>
                                            <legend>Uncommunicated Legs</legend>
                                        
                                             <ComponentArt:Grid id="dgTrafficSheet" 
                                                RunningMode="Client" 
                                                EnableViewState="false"
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
                                                ShowHeader="false"
                                                ShowSearchBox="false"
                                                SearchOnKeyPress="True"
                                                PageSize="100" 
                                                ShowFooter="false"
                                                ScrollBar ="auto"
                                                ScrollTopBottomImagesEnabled="true"
                                                ScrollImagesFolderUrl="~/images/scroller"
                                                ScrollBarCssClass="ScrollBar"
                                                ScrollGripCssClass="ScrollGrip"
                                                ScrollPopupClientTemplateId="ScrollPopupTemplate"
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
                                                Width="1000"
                                                 runat="server"
                                                KeyboardEnabled ="true"
                                                LoadingPanelClientTemplateId="LoadingFeedbackTemplate"
                                                
                                                >
                                                
                                                <Levels>
                                                    <ComponentArt:GridLevel 
                                                         DataKeyField="InstructionId"
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
                                                            <ComponentArt:GridColumn DataField="InstructionId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="JobId" HeadingText="ID" FixedWidth="true" Width="40" IsSearchable="true" Visible="false"  />
                                                            <ComponentArt:GridColumn DataField="DepotCode" HeadingText="TA" FixedWidth="true" Width="35" Visible="false"/>
                                                            <ComponentArt:GridColumn DataField="Load_Number" HeadingText="Load No" IsSearchable="true" />
                                                            <ComponentArt:GridColumn DataField="OrganisationName" HeadingText="Customer" width="150"/>
                                                            <ComponentArt:GridColumn DataField="LegPlannedStartDateTime" HeadingText="Start" FormatString="dd/MM HH:mm" FixedWidth="true" Width="80" />
                                                            <ComponentArt:GridColumn DataField="StartPointDisplay" HeadingText="From" DataCellClientTemplateId="StartPointDisplayTemplate" />
                                                            <ComponentArt:GridColumn DataField="LegPlannedEndDateTime" HeadingText="Finish" FormatString="dd/MM HH:mm" FixedWidth="true" Width="80" />
                                                            <ComponentArt:GridColumn DataField="EndPointDisplay" HeadingText="To" DataCellClientTemplateId="EndPointDisplayTemplate" />
                                                            <ComponentArt:GridColumn DataField="FullName" HeadingText="Driver" Width="80"/>
                                                            <ComponentArt:GridColumn DataField="RegNo" HeadingText="Vehicle" Width="72" FixedWidth="true"/>
                                                            <ComponentArt:GridColumn DataField="TrailerRef" HeadingText="Trailer" Width="42"  FixedWidth="true"/>
                                                            <ComponentArt:GridColumn DataField="StartPointId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="EndPointId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="InstructionStateId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="DriverResourceId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="VehicleResourceId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="TrailerResourceId" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="LastUpdateDate" Visible="false" />
                                                            <ComponentArt:GridColumn DataField="InstructionId" Visible="true" DataCellClientTemplateId="CommunicateThisTemplate" HeadingText=" " />
                                                         </Columns>
                                                           <ConditionalFormats>
                                                            <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('InstructionStateId').Value == 1" RowCssClass="stateBooked" />

                                                         </ConditionalFormats>
                                                        </ComponentArt:GridLevel>
                                                </Levels>
                                                
                                                <ClientTemplates>
                                                    <ComponentArt:ClientTemplate ID="JobTemplate">
                                                        <a href='javascript:openJobDetailsWindow(## DataItem.GetMember("JobId").Value ##);' title="Open this job">## DataItem.GetMember("JobId").Value ##</a>
                                                    </ComponentArt:ClientTemplate>
                                                    
                                                    <ComponentArt:ClientTemplate ID="CommunicateThisTemplate">
                                                        <a href='javascript:openCommunicateWindow(## DataItem.GetMember("InstructionId").Value ##, "## DataItem.GetMember("FullName").Value ##", ## DataItem.GetMember("DriverResourceId").Value ##, ## DataItem.GetMember("JobId").Value ##, "## DataItem.GetMember("LastUpdateDate").Value ##");' title="Communicate This Leg">Communicate This Leg</a>
                                                    </ComponentArt:ClientTemplate>
                                                    
                                                    <ComponentArt:ClientTemplate ID="StartPointDisplayTemplate">
                                                        <span onmouseover='ShowPoint("~/point/getPointAddresshtml.aspx", ## DataItem.GetMember("StartPointId").Value ##);' onmouseout="HidePoint();">## DataItem.GetMember("StartPointDisplay").Value ##</span>
                                                    </ComponentArt:ClientTemplate>
                                                    <ComponentArt:ClientTemplate ID="EndPointDisplayTemplate">
                                                        <span onmouseover='ShowPoint("~/point/getPointAddresshtml.aspx", ## DataItem.GetMember("EndPointId").Value ##);' onmouseout="HidePoint();">## DataItem.GetMember("EndPointDisplay").Value ##</span>
                                                    </ComponentArt:ClientTemplate>
                                                    <ComponentArt:ClientTemplate ID="TrafficAreaTemplate">
                                                            <a href="javascript:openTrafficAreaWindow('setTrafficArea.aspx?LegId=## DataItem.GetMember("InstructionID").Value ##&LastUpdateDate=## DataItem.GetMember("LastUpdateDate").Value ##&JobId=## DataItem.GetMember("JobId").Value ##')">
                                                                ## DataItem.GetMember("DepotCode").Value ##				
                                                            </a>
                                                    </ComponentArt:ClientTemplate> 
                                                    <ComponentArt:ClientTemplate ID="DriverTemplate">
                                                            <a href="#">
                                                                ## DataItem.GetMember("FullName").Value.length > 0 ? DataItem.GetMember("FullName").Value : "Not Assigned" ##				
                                                            </a>
                                                    </ComponentArt:ClientTemplate> 
                                                    <ComponentArt:ClientTemplate ID="VehicleTemplate">
                                                            <a href="javascript:openResourceWindow(## DataItem.GetMember("InstructionId").Value ##,'## DataItem.GetMember("FullName").Value ##', ## DataItem.GetMember("DriverResourceId").Value == '' ? 0 : DataItem.GetMember("DriverResourceId").Value ##, '## DataItem.GetMember("RegNo").Value ##', ## DataItem.GetMember("VehicleResourceId").Value == '' ? 0 : DataItem.GetMember("VehicleResourceId").Value ##, '## DataItem.GetMember("TrailerRef").Value ##', ## DataItem.GetMember("TrailerResourceId").Value == '' ? 0 : DataItem.GetMember("TrailerResourceId").Value ## , '## DataItem.GetMember("LegPlannedStartDateTime").Value ##', '## DataItem.GetMember("LegPlannedEndDateTime").Value ##', '## DataItem.GetMember("DepotCode").Value ##', '## DataItem.GetMember("LastUpdateDate").Value ##', '## DataItem.GetMember("JobId").Value ##' )">
                                                                ## DataItem.GetMember("RegNo").Value ##				
                                                            </a>
                                                    </ComponentArt:ClientTemplate> 
                                                    <ComponentArt:ClientTemplate ID="TrailerTemplate" >
                                                            <a href="javascript:openResourceWindow(## DataItem.GetMember("InstructionId").Value ##,'## DataItem.GetMember("FullName").Value ##', ## DataItem.GetMember("DriverResourceId").Value == '' ? 0 : DataItem.GetMember("DriverResourceId").Value ##, '## DataItem.GetMember("RegNo").Value ##', ## DataItem.GetMember("VehicleResourceId").Value == '' ? 0 : DataItem.GetMember("VehicleResourceId").Value ##, '## DataItem.GetMember("TrailerRef").Value ##', ## DataItem.GetMember("TrailerResourceId").Value == '' ? 0 : DataItem.GetMember("TrailerResourceId").Value ## , '## DataItem.GetMember("LegPlannedStartDateTime").Value ##', '## DataItem.GetMember("LegPlannedEndDateTime").Value ##', '## DataItem.GetMember("DepotCode").Value ##', '## DataItem.GetMember("LastUpdateDate").Value ##', '## DataItem.GetMember("JobId").Value ##' )">
                                                                ## DataItem.GetMember("TrailerRef").Value ##				
                                                            </a>
                                                    </ComponentArt:ClientTemplate> 
                                                    <ComponentArt:ClientTemplate ID="SubContractTemplate">
                                                            ## GetLegAction(DataItem.GetMember("InstructionStateId").Value, DataItem.GetMember("JobId").Value, DataItem.GetMember("LastUpdateDate").Value ##
                                                    </ComponentArt:ClientTemplate> 
                            			           
                                                     <ComponentArt:ClientTemplate Id="ScrollPopupTemplate">
                                                        <table cellspacing="0" cellpadding="2"
                                                         border="0" class="ScrollPopup">
                                                        <tr>
                                                     <td style="width:50px;">
                                                         <div style="font-size:10px;font-family:MS Sans Serif; text-overflow:ellipsis;overflow:hidden;">
                                                         <nobr>## DataItem.GetMember("LegPlannedStartDateTime").Value ##</nobr>
                                                         </div>
                                                         </td>
                                                     <td style="width:50px;">
                                                         <div style="font-size:10px;font-family: MS Sans Serif; text-overflow:ellipsis;overflow:hidden;">
                                                         <nobr>## DataItem.GetMember("StartPointDisplay").Value ##</nobr>
                                                         </div>
                                                     </td>
                                                     <td style="width:50px;" align="right">
                                                         <div style="font-size:10px;font-family:MS Sans Serif; text-overflow:ellipsis;overflow:hidden;">
                                                         <nobr>##  DataItem.GetMember("EndPointDisplay").Value ##</nobr>
                                                         </div>
                                                     </td>
                                                        </tr>
                                                        </table>
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
                                                                <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("LegPlannedStartDateTime").Value ##</nobr></div></td>
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
                                                             Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgTrafficSheet.PageCount ##</b>
                                                             </td>
                                                             <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                                                             Organisation <b>## DataItem.Index + 1 ##</b> of <b>## dgTrafficSheet.RecordCount ##</b>
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
                                                        <td><img src="../../images/spinner.gif" width="16" height="16" border="0" alt="" /></td>
                                                      </tr>
                                                      </table>
                                                      </ComponentArt:ClientTemplate>
                                                      </ClientTemplates>
                                                 
                                            </ComponentArt:Grid>
                                        </fieldset>  


                                        <div style="height:10px"></div>
                                           
                                        <fieldset>
                                            <legend>Communications for This Run</legend>
                                           
                                            <asp:ObjectDataSource id="odsCommunications" runat="server" selectmethod="GetForJobId" TypeName="Orchestrator.Facade.Resource" ></asp:ObjectDataSource>
                                            <asp:GridView ID="gvCommunications" runat="server" AutoGenerateColumns="false" CssClass="Grid" DataSourceID="odsCommunications">
                                                <headerstyle cssclass="HeadingRowLite" height="22" verticalalign="middle" />
                                                    <rowStyle  cssclass="Row" />
                                                    <AlternatingRowStyle  backcolor="WhiteSmoke" />
                                                    <SelectedRowStyle  cssclass="SelectedRow" />
                                                    <columns>
                                                        <asp:BoundField headerText="ID" DataField="DriverCommunicationId" ReadOnly="true" />
                                                        <asp:BoundField HeaderText="Driver" DataField="FullName" ItemStyle-Width="120" />
                                                        <asp:TemplateField HeaderText="Status" ItemStyle-Width="80px">
				                                            <ItemTemplate>
					                                            <%# ((Orchestrator.eDriverCommunicationStatus) (int) ((System.Data.DataRowView) Container.DataItem)["DriverCommunicationStatusId"]).ToString() %>
				                                            </ItemTemplate>					                            
		                                                </asp:TemplateField>
		                                                <asp:TemplateField HeaderText="Type" ItemStyle-Width="80px">
			                                                <ItemTemplate>
				                                                <%# ((Orchestrator.eDriverCommunicationType) (int) ((System.Data.DataRowView) Container.DataItem)["DriverCommunicationTypeId"]).ToString() %>
			                                                </ItemTemplate>							                            
			                                            </asp:TemplateField>
		                                                <asp:TemplateField HeaderText="Comments" ItemStyle-Width="300px">
			                                                <ItemTemplate>
				                                                <%# ((System.Data.DataRowView) Container.DataItem)["Comments"] %>
			                                                </ItemTemplate>								                            
		                                                </asp:TemplateField>
		                                                <asp:TemplateField HeaderText="Text" ItemStyle-Width="300px">
			                                                <ItemTemplate>
				                                                <%# ((System.Data.DataRowView) Container.DataItem)["Text"] %>
			                                                </ItemTemplate>
		                                                </asp:TemplateField>
			                                                <asp:BoundField HeaderText="Last Updated On" DataField="LastUpdatedOn" ReadOnly="True" DataFormatString="{0:dd/MM/yy HH:mm}" ItemStyle-Width="150px" ItemStyle-HorizontalAlign="Right" HtmlEncode="false" />
		                                                <asp:TemplateField HeaderText="Printer Friendly View" ItemStyle-Width="134px">
			                                                <ItemTemplate>
				                                                <input type="button" value="Printer Friendly" onclick="javascript:OpenPrinterFriendlyView('<%# DataBinder.Eval(Container.DataItem, "DriverCommunicationId") %>');" />
			                                                </ItemTemplate>
		                                                </asp:TemplateField>
                                                    </columns>
                                            </asp:GridView>                                                
                                        </fieldset>


			                   
                    
        <div align="center" id="inProgress" style="display: none">
            <table width="99%" cellspacing="2" cellpadding="0" border="0">
                <tr valign="top">
                    <td align="center">
                        <h1>Please wait while your action is processed...</h1>
                    </td>
                </tr>
            </table>
        </div>
	                          
	    <script language="javascript" type="text/javascript">
	    <!--
    	
            function openCommunicateWindow(InstructionID, Driver, DriverResourceId, JobId, lastUpdateDate)
            {
                var qs = "iId=" + InstructionID + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&jobId=" + JobId + "&LastUpdateDate=" + lastUpdateDate ;
                <%=dlgCommunicateThis.ClientID %>_Open(qs);
            }
            
            function OpenPrinterFriendlyView(communicationId)
            {
                var qs = "communicationId=" + communicationId;
                <%=dlgPrinterFriendly.ClientID %>_Open(qs);
            }
    	
		    window.focus();

		    function HideTop()
		    {
			    if (typeof(Page_ClientValidate) == 'function')
			    {
				    if (Page_ClientValidate())
				    {
					    var topPortion = document.getElementById("topPortion");
					    var inProgress = document.getElementById("inProgress");
    					
					    if (topPortion != null && inProgress != null)
					    {
						    topPortion.style.display = "none";
						    inProgress.style.display = "";
					    }
				    }
			    }
			    else
			    {
				    var topPortion = document.getElementById("topPortion");
				    var inProgress = document.getElementById("inProgress");
    				
				    if (topPortion != null && inProgress != null)
				    {
					    topPortion.style.display = "none";
					    inProgress.style.display = "";
				    }
			    }
		    }
    		
		    function setNumberUsed(e)
		    {			
			    var ctl		 = document.getElementById("txtNumber");
			    ctl.disabled = false;
			    ctl.value	 = e;			
		    }
    		
		    function enableOtherTextBox()
		    {
			    var e = document.getElementById("txtNumber");
			    e.disabled=false;
			    e.focus();
		    }
    		
		    function cboCommunicationTypeIndex_Changed()
		    {
			    var cbo = document.getElementById('cboCommunicationType');
			    var selectedType = cbo.options[cbo.selectedIndex];
    			
			    var rfvNumber	= document.getElementById('rfvNumber');
			    var rfvSMSText	= document.getElementById('rfvSMSText');
			    var txtSMSText	= document.getElementById('txtSMSText');
    			
			    switch(selectedType.value)
			    {
				    case "Phone":
					    rfvNumber.disabled	= false;
					    rfvSMSText.disabled = true;
					    txtSMSText.disabled = true;
					    break;
				    case "Text":
					    rfvNumber.disabled	= true;
					    rfvSMSText.disabled = false;
					    txtSMSText.disabled = false;
					    break;
				    case "In Person":
					    rfvNumber.disabled	= true;
					    rfvSMSText.disabled = true;
					    txtSMSText.disabled = true;
					    break;
			    }
    			
		    }
	    //-->
	    </script>
    </form>
</asp:Content>
