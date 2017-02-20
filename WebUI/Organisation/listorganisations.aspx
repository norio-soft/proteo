<%@ Register TagPrefix="uc1" TagName="footer" Src="~/UserControls/footer.ascx" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="cc3" Namespace="Orchestrator.WebUI.Pagers" Assembly="WebUI" %>
<%@ Register TagPrefix="componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Page language="c#" Inherits="Orchestrator.WebUI.Organisation.listorganisations" Codebehind="listorganisations.aspx.cs" %>
<uc1:header id="pageHeader" title="Find a Client" SubTitle="Please Choose a Client." XMLPath="organisationContextMenu.xml" runat="server"></uc1:header>

<style>
		.PageNumbers { FONT-SIZE: 10pt; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: none }
		.CurrentPage { FONT-WEIGHT: bold; FONT-SIZE: 10pt; COLOR: red; FONT-FAMILY: Tahoma, Verdana, Arial; TEXT-DECORATION: underline }
	</style>
<form id="Form1" runat="server">
    <div style="font-size:14pt"	 >This Page should not be used, can you please notify support how you accessed this page.</div>
		
		<table width="100%">
			<tr>
				<td>
                   <ComponentArt:Grid id="dgClients" 
                        RunningMode="Client" 
                        CssClass="Grid" 
                        HeaderCssClass="GridHeader" 
                        FooterCssClass="GridFooter"
                        GroupByTextCssClass="GroupByText"
                        GroupingNotificationTextCssClass="GridHeaderText"
                        ShowHeader="true"
                        ShowSearchBox="True"
                        SearchOnKeyPress="True"
                        PageSize="25" 
                        PagerStyle="Slider" 
                        PagerTextCssClass="GridFooterText"
                        PagerButtonWidth="41"
                        PagerButtonHeight="22"
                        SliderHeight="20"
                        SliderWidth="150" 
                        SliderGripWidth="9" 
                        SliderPopupOffsetX="20"
                        SliderPopupClientTemplateId="SliderTemplate" 
                        PreExpandOnGroup="true"
                        GroupingPageSize="25"
                        ImagesBaseUrl="../images/" 
                        PagerImagesFolderUrl="../images/pager/"
                        TreeLineImagesFolderUrl="../images/lines/" 
                        TreeLineImageWidth="22" 
                        TreeLineImageHeight="19" 
                        Width="100%" Height="100%" runat="server"
                        ClientSideOnDoubleClick="handleGridDoubleClick"
                        DataKeyField="IdentityId"
                        >

                    <Levels>
               <ComponentArt:GridLevel 
                 DataKeyField="IdentityId"
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
                 >
                 <Columns>
                    <ComponentArt:GridColumn DataField="IdentityId" HeadingText="Id"/>
                   <ComponentArt:GridColumn DataField="OrganisationName" HeadingText="Organisation Name" DataCellClientTemplateId="OrganisationNameTemplate" />
                   <ComponentArt:GridColumn DataField="PrimaryContact" HeadingText="Primary Contact" />
                   
                 </Columns>
               </ComponentArt:GridLevel>
                    </Levels>
                    <ClientTemplates>
                        <ComponentArt:ClientTemplate ID="OrganisationNameTemplate">
                            <a href="addupdateorganisation.aspx?identityId=## DataItem.GetMember("IdentityId").Value ##">## DataItem.GetMember("OrganisationName").Value ##</a>
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
            <td style="font-family:verdana;font-size:11px;"><div style="overflow:hidden;width:115px;"><nobr>## DataItem.GetMember("OrganisationName").Value ##</nobr></div></td>
         </tr>
         <tr>
    <td colspan="2">
   
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
         Page <b>## DataItem.PageIndex + 1 ##</b> of <b>## dgClients.PageCount ##</b>
         </td>
         <td style="padding-right:5px;color:white;font-family:verdana;font-size:10px;" align="right">
         Organisation <b>## DataItem.Index + 1 ##</b> of <b>## dgClients.RecordCount ##</b>
         </td>
       </tr>
       </table>  
       </td>
     </tr>
     </table>
   </ComponentArt:ClientTemplate>

                    </ClientTemplates>
                  </ComponentArt:grid>

				</td>
			</tr>
		</table>
		<div class="buttonbar">
		    <asp:button id="btnExport" runat="server" text="Export" style="width:75px;" />
		</div>
		<script>
		    function handleGridDoubleClick(item)
		    {
		        var id = item.GetMember("IdentityId").Value;
		        location.href="addupdateorganisation.aspx?identityId=" + id;
		        return false;
		    }
		    
		    function resetGrid()
		    {
		        //alert('here');
		        //var grid = document.getElementById("<%=dgClients.ClientID%>");
		        dgClients.Search("");
		        
		    }
		</script>

</form>

<uc1:footer id="Footer1" runat="server"></uc1:footer>
