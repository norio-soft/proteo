<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="ReportViewer" Src="~/UserControls/ReportViewer.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components"%>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Invoicing.InvoicePrepation" Codebehind="oldinvoicepreparation.aspx.cs" %>
<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>


<uc1:header id="Header1" title="Invoice Preparation" runat="server" ShowLeftMenu="false" SubTitle="The Invoice preparation summary is below."
	XMLPath="InvoicingContextMenu.xml"></uc1:header>
  <style>
        .batched td.DataCell 
        { 
            height:20px;
            background-color: #FF8080; 
            cursor: default;
            border-bottom: 1px solid #EAE9E1; 
        }

        table.vr {
border-collapse:collapse;
border-left:2px solid lightblue;
border-right:2px solid lightblue
}
   </style>
<FORM id="Form1" method="post" runat="server">
	<asp:label id="lblJobCount" runat="server" Font-Size="Medium" Width="100%"></asp:label>
	<asp:label id="lblDetails" runat="server" Width="100%"></asp:label>
	
	<input type="hidden" id="hidJobCount" runat="server" value="0"/>
	<input type="hidden" id="hidJobTotal" runat="server" value="0"/>
	<input type="hidden" id="hidSelectedJobs" runat="server" value="" />
	
	<fieldset>
	<legend><strong>Main Filter</strong></legend>
	<TABLE width="100%" border="0">
		<TR>
			<TD vAlign="top"><asp:Label id="lblClient" Text="Client" Runat="server"></asp:Label></TD>
			<TD><telerik:RadComboBox ID="cboClient" Overlay="true" height="400px" ZIndex="500" runat="server" RadControlsDir="~/script/RadControls/" EnableLoadOnDemand="true" ShowMoreResultsBox="true" MarkFirstMatch="true" Skin="WindowsXP" ItemRequestTimeout="500" Width="355px"  ></telerik:RadComboBox>
			</TD>
			<TD align="right" >Date From</TD>
			<TD><telerik:RadDateInput id="dteStartDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></TD>
			<td rowspan="3" width="40%">
				<asp:panel id="pnlFilterOptions" runat="server" visible="true">
		        <FIELDSET>
			    <P><LEGEND><STRONG>Filter Options</STRONG></LEGEND></P>
				<asp:label id="lblSaveProgressNotification" runat="server" Visible="true"></asp:label>
				<nfvc:noformvalbutton id="btnLoadFilter" runat="server" visible="true" text="Load Filter" ServerClick="btnLoadFilter_Click" NoFormValList="rfvClient"></nfvc:noformvalbutton>
				<nfvc:noformvalbutton id="btnSaveFilter" runat="server" visible="false" text="Save Current Filter" ServerClick="btnSaveFilter_Click"></nfvc:noformvalbutton>
				<nfvc:noformvalbutton id="btnClearFilter" runat="server" visible="false" text="Clear Filter" ServerClick="btnClear_Click"></nfvc:noformvalbutton>
    	        </FIELDSET>
	            </asp:panel>
			</td>
		</TR>
		<TR>
			<TD><asp:Label id="lblJobState" visible="false" Text="Client" Runat="server">Job State</asp:Label></TD>
			<TD><asp:dropdownlist id="cboJobState" visible="false"  runat="server" ></asp:dropdownlist></TD>
			<TD align="right" >Date To</TD>
			<TD ><telerik:RadDateInput id="dteEndDate" runat="server" dateformat="dd/MM/yy"></telerik:RadDateInput></TD>
		</TR>
		<tr>
		<td colspan="4"> 
			<asp:label id="lblOnHold" runat="server" visible="false" cssclass="confirmation" text=""></asp:label>
		</td>
		</tr>
		</table>
		</fieldset>
	
	<DIV class="buttonbar" align="left">
			<nfvc:noformvalbutton id="btnFilter2" runat="server" text="Generate Jobs To Invoice" ServerClick="btnFilter_Click"></nfvc:noformvalbutton>
			<nfvc:noformvalbutton id="btnClear2" runat="server" visible="false" text="Clear" ServerClick="btnClear_Click"></nfvc:noformvalbutton>
    		<nfvc:noformvalbutton id="btnCreateInvoice2" runat="server" visible="false" text="Create Invoice" ServerClick="btnCreateInvoice_Click"></nfvc:noformvalbutton>
            <nfvc:noformvalbutton id="btnExport" visible="false" runat="server" text="Export to CSV" ServerClick="btnExport_Click"></nfvc:noformvalbutton>
    </DIV>

	<asp:panel id="pnlNormalJob" runat="server" visible="true">
		<FIELDSET>
			<P><LEGEND><STRONG>Jobs To Invoice</STRONG></LEGEND></P>
			<table width="100%"> 
			<tr>
			<td align="right">
			    <P>
				    <asp:CheckBox id="chkMarkAll" runat="server" visible="true" Text="Un/Mark All" AutoPostBack="True"
					Font-Bold="True" TextAlign="Left"></asp:CheckBox></P>
            </td>
			<td align="left">
			<p>
		        <asp:Checkbox id="chkOnlyShowTicked" runat="server" Text="Only Ticked Jobs" TextAlign="Left" AutoPostBack="True" Font-Bold="True"></asp:Checkbox>
		    </p>
		    </td>
		    </tr>
		   
		    </table>
			<ComponentArt:Grid id="dgJobs" 
                        RunningMode="Client" 
                        CssClass="Grid" 
                        HeaderCssClass="GridHeader" 
                        FooterCssClass="GridFooter"
                        ScrollBar="Off" 
                        ScrollTopBottomImagesEnabled="true"
                        ScrollTopBottomImageHeight="2" 
                        ScrollTopBottomImageWidth="16" 
                        ScrollImagesFolderUrl="~/images/scroller"
                        ScrollButtonWidth="16" 
                        ScrollButtonHeight="17" 
                        ScrollBarCssClass="ScrollBar"
                        ScrollGripCssClass="ScrollGrip" 
                        ScrollBarWidth="16" 
                        ScrollPopupClientTemplateId="ScrollPopupTemplate"
                        
                        GroupByTextCssClass="GroupByText"
                        GroupingNotificationTextCssClass="GridHeaderText" 
                        GroupBySortAscendingImageUrl="group_asc.gif"
                        GroupBySortDescendingImageUrl="group_desc.gif"
                        GroupBySortImageWidth="10"
                        GroupBySortImageHeight="10"
                        GroupingPageSize = "100"
                        AllowPaging=false
                        ShowHeader="true"
                        ShowSearchBox="False"
                        SearchOnKeyPress="False"
                        PageSize="200" 
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
                        DataKeyField="JobId"
                        KeyboardEnabled ="true"
                        LoadingPanelClientTemplateId="LoadingTemplate"
                        LoadingPanelPosition="MiddleCenter"
                         
                        ClientSideOnCheckChanged="GetCheckedItems">
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
                             GroupHeadingCssClass="GroupHeading">
                         <Columns>
			                <ComponentArt:GridColumn HeadingText="Old Inc." Visible="false" DataCellServerTemplateId="CheckBoxTemplate" />
			                <componentart:GridColumn HeadingText="Inc." Visible="true" ColumnType="CheckBox" DataField="Include"></componentart:GridColumn>
			                <ComponentArt:GridColumn HeadingText="Customer" AllowGrouping="True" visible="true" DataField="OrganisationName" DataCellClientTemplateId="URLOrganisationNameTemplate" />
			                <ComponentArt:GridColumn HeadingText="OrganisationName" AllowGrouping="True" visible="false" DataField="OrganisationName"/>
			                <ComponentArt:GridColumn HeadingText="OrganisationId" Visible="False" DataField="IdentityId" />
							<ComponentArt:GridColumn HeadingText="Load" DataField="LoadNo" />
							<ComponentArt:GridColumn HeadingText="Docket No." Width="100" TextWrap="true" DataField="DocketNumbers" />
							<ComponentArt:GridColumn HeadingText="Batch Id" DataField="BatchId" />
							<ComponentArt:GridColumn HeadingText="Charge" DataField="ChargeAmount" FormatString="C"  /> 
							<ComponentArt:GridColumn AllowGrouping="False" Width="180" TextWrap="true" HeadingText="Delivery"  DataCellServerTemplateId="DeliveryPointTemplate" />
							<ComponentArt:GridColumn HeadingText="Job Id" DataField="JobId" DataCellClientTemplateId="URLJobIdTemplate" />
							<ComponentArt:GridColumn AllowGrouping="False" Width="150" TextWrap="true" HeadingText="Collection" DataField="CollectionPoint" />
							<ComponentArt:GridColumn HeadingText="Date Completed" Width="90" DataField="CompleteDate" FormatString="dd/MM HH:mm" />
							<ComponentArt:GridColumn HeadingText="Type" DataField="ChargeType" />
							<ComponentArt:GridColumn AllowGrouping="False" TextWrap="true" HeadingText="References"  DataCellServerTemplateId="ReferencesTemplate" />					
                         </Columns>
                         <ConditionalFormats>
                                <ComponentArt:GridConditionalFormat ClientFilter="DataItem.GetMember('BatchId').Value != ''" RowCssClass="batched" />
                         </ConditionalFormats>
                     </ComponentArt:GridLevel>
                </Levels>
             <ServerTemplates>
               <ComponentArt:GridServerTemplate  ID="CheckBoxTemplate">
                    <Template >
                        <asp:CheckBox id="chkIncludeJob" runat="server" Text=""></asp:CheckBox>
			        </Template>
		       </ComponentArt:GridServerTemplate>

               <ComponentArt:GridServerTemplate ID="DeliveryPointTemplate">
                  <Template >
                    <asp:Repeater id="repDeliveryPoints" Runat="server" EnableViewState="False">
                        <ItemTemplate>
	                        <li><%# DataBinder.Eval(Container.DataItem, "CustomerName")%></li>
                        </ItemTemplate>
                    </asp:Repeater>
                    </Template>
		       </ComponentArt:GridServerTemplate>
		       
		       <ComponentArt:GridServerTemplate ID="ReferencesTemplate">
	                <Template>
	                <asp:Repeater id="repReferences" runat="server" EnableViewState="False" OnItemDataBound="repReferences_ItemDataBound">
		                <ItemTemplate>
			                <li><nowrap><%# DataBinder.Eval(Container.DataItem, "OrganisationReference.Description") %>: <%# DataBinder.Eval(Container.DataItem, "Value") %></nowrap></li>
		                </ItemTemplate>
	                </asp:Repeater>
	                </Template>
		       </ComponentArt:GridServerTemplate>  
             </ServerTemplates>

             <ClientTemplates>
		       <ComponentArt:ClientTemplate ID="URLJobIdTemplate">
	              <a href="javascript:openDialogWithScrollbars('~/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=## DataItem.GetMember("JobId").Value ##'+ getCSID(), '800','600');">
						    ## DataItem.GetMember("JobId").Value ##
					    </a>
		       </ComponentArt:ClientTemplate>     
    		   
		       <ComponentArt:ClientTemplate ID="URLOrganisationNameTemplate">     
    		      <a href='InvoicePreparation.aspx?identityId=## DataItem.GetMember("IdentityId").Value ##&ResetDates=1'>
						    ## DataItem.GetMember("OrganisationName").Value ##
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
                             Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgJobs.PageCount ##</b>
                             </td>
                             <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
                             Self Bill Jobs <b>## DataItem.Index + 1 ##</b> of <b>## dgJobs.RecordCount ##</b>
                             </td>
                           </tr>
                           </table>  
                           </td>
                         </tr>
                         </table>
                       </ComponentArt:ClientTemplate>
                       
                       <ComponentArt:ClientTemplate Id="LoadingTemplate">
                          <table cellspacing="0" cellpadding="0" border="0">
                          <tr>
                            <td style="font-size:10px;font-family:Verdana;">Loading...&nbsp;</td>
                            <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                          </tr>
                          </table>
                        </ComponentArt:ClientTemplate>

                      <ComponentArt:ClientTemplate Id="ScrollPopupTemplate">
                        <table cellspacing="0" cellpadding="2"
                         border="0" class="ScrollPopup">
                        <tr>
                     <td style="width:50px;">
                         <div style="font-size:10px;font-family:MS Sans Serif;
                       text-overflow:ellipsis;overflow:hidden;">
                         <nobr>## DataItem.GetMember("JobId").Value ##</nobr>
                         </div>
                         </td>
                        </tr>
                        </table>
                    </ComponentArt:ClientTemplate>


                    </ClientTemplates>
             </ComponentArt:grid>
		</FIELDSET>
	</asp:panel>
	<DIV class="buttonbar" align="left">
			<nfvc:noformvalbutton id="btnFilter" runat="server" text="Generate Jobs To Invoice" ServerClick="btnFilter_Click"></nfvc:noformvalbutton>
			<nfvc:noformvalbutton id="btnClear" runat="server" visible="false" text="Clear" ServerClick="btnClear_Click"></nfvc:noformvalbutton>
			<nfvc:noformvalbutton id="btnCreateInvoice" runat="server" visible="false" text="Create Invoice" ServerClick="btnCreateInvoice_Click"></nfvc:noformvalbutton>
    </DIV>
<uc1:footer id="Footer1" runat="server"></uc1:footer>
</FORM>
<script language="javascript">
<!--
	var message = "You have selected "
	var message1 = " job(s), and the total amount is £"
	var hidSelectedJobs = document.getElementById('<%=hidSelectedJobs.ClientID %>');
    var hidJobCount = document.getElementById('<%=hidJobCount.ClientID %>');
	var hidJobTotal = document.getElementById('<%=hidJobTotal.ClientID %>');

    function GetCheckedItems(gridItem, index, checkBoxElement)
    {
        amount = gridItem.Cells[8].Value;
        jobId = gridItem.Cells[10].Value;
        //hidSelectedJobs.value = "";
        
        // Check the current CheckBox that we have just dealt with 
        if (checkBoxElement.checked)
        {
            // We've selected this item
            hidJobCount.value++;
	        hidJobTotal.value = parseFloat(hidJobTotal.value) + parseFloat(amount);
	        
            hidSelectedJobs.value = hidSelectedJobs.value + jobId + ",";
        }
        else
        {
            // We've deselected this item
            hidJobCount.value--;
			hidJobTotal.value = parseFloat(hidJobTotal.value) - parseFloat(amount);
			
			if (hidSelectedJobs.value.substr(0, (new String(jobId).length) + 1) == jobId + ",")
			{
			    if (hidSelectedJobs.value == (jobId + ","))
			    {
			        hidSelectedJobs.value = "";
			    }
			    else
			    {
			        hidSelectedJobs.value = hidSelectedJobs.value.substr((new String(jobId).length) + 1);
			    }
		    }
			else
			{
			    var location = hidSelectedJobs.value.indexOf("," + jobId + ",");
                hidSelectedJobs.value = hidSelectedJobs.value.substr(0, location + 1) + hidSelectedJobs.value.substr(location + ("," + jobId + ",").length);
			}
        }

        hidJobTotal.value  = Math.round(hidJobTotal.value * 100)/100;
        
    	var text = message + hidJobCount.value + message1 + hidJobTotal.value;
	
		setLabelText("lblDetails", text);
      
        return true;
    }
	   
    function setLabelText(ID, Text)
  	{
  	  	document.getElementById(ID).innerHTML = Text;
  	}
	-->
</script>
