<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.myResource" Theme="Outlook" Codebehind="myResource.aspx.cs" %>
<%@ Register TagPrefix="Componentart" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <link href="../style/Styles.css" rel="stylesheet" type="text/css" />
    <script language="javascript" src="../script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="../script/popAddress.js" type="text/javascript"></script>
    <style type="text/css">
.CalendarStyle
{
	FONT-SIZE: 11px; 
	COLOR: #333333;
	FONT-FAMILY: Arial,Verdana;
	width: 100%;
	text-decoration: none;
	margin-left: 0px;
	height: 26px;
	padding:0px
}
</style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <ComponentArt:NavBar ID="NavBar" SiteMapXmlFile="navbar.sitemap" runat="server" >
      <Templates>
       <ComponentArt:NavigationCustomTemplate id="NavDriverTemplate">
        <Template>
             <ComponentArt:Grid id="dgDrivers" 
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
                        ShowHeader="false"
                        ShowFooter="false"
                        ShowSearchBox="false"
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
                        Width="500" runat="server"
                        KeyboardEnabled ="true"
                        
                        GroupBy="DriverType" Height="100%">
                        <Levels>
                            <ComponentArt:GridLevel 
                                 DataKeyField="DriverResourceId"
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
                                 AlternatingRowCssClass="AlternatingRow">
                                 <Columns>
                                    <ComponentArt:GridColumn DataField="DriverResourceId" HeadingText=" " DataCellClientTemplateId="PreSelectDriverTemplate" FixedWidth="true" Width="18" />
                                    <ComponentArt:GridColumn DataField="FullName" HeadingText="Full Name" DataCellClientTemplateId="DriverTemplate"  />
                                    <ComponentArt:GridColumn DataField="LastLocation" HeadingText="Last Call In" AllowSorting="false" DataCellClientTemplateId="valignTopTemplate" />
                                    <ComponentArt:GridColumn DataField="DriverResourceId" HeadingText="Now/Next"   AllowSorting="false" DataCellServerTemplateId="NowTemplate"/>
                                    <ComponentArt:GridColumn DataField="DriverResourceId" HeadingText="Tomorrow"   AllowSorting="false" DataCellServerTemplateId="TomorrowTemplate"/>
                                    <ComponentArt:GridColumn DataField="DriverType" visible="false" HeadingText=" "/>
                                    <ComponentArt:GridColumn DataField="FullName" visible="false"/>
                                    <ComponentArt:GridColumn DataField="RegNo" visible="false"/>
                                    <ComponentArt:GridColumn DataField="HasFuture" visible="false"/>
                                    <ComponentArt:GridColumn DataField="OrganisationLocationName" visible="false"/>
                                    <ComponentArt:GridColumn DataField="TravelNotes" visible="false"/>
                                 </Columns>
                                </ComponentArt:GridLevel>
                        </Levels>

                        <ClientTemplates>
                            <ComponentArt:ClientTemplate ID="valignTopTemplate">
    			                <div style="height:100%; vertical-align:top;">## DataItem.GetCurrentMember().Value ##</div>
                            </ComponentArt:ClientTemplate>
                            <ComponentArt:ClientTemplate ID="PreSelectDriverTemplate">
    			                <input type="radio" value="## DataItem.GetMember("DriverResourceId").Value ##" group="driver" />
                            </ComponentArt:ClientTemplate>
                            <ComponentArt:ClientTemplate ID="DriverTemplate">
                               <a target="_blank" href="javascript:ShowFuture('## DataItem.GetMember("DriverResourceId").Value ##', '3');">## DataItem.GetMember("FullName").Value ##</a>
                               <br />
                               <a target="_blank" href="javascript:ShowFuture('## DataItem.GetMember("DriverResourceId").Value ##', '1');">## DataItem.GetMember("RegNo").Value ##</a>
                               <br />
                               ## DataItem.GetMember("OrganisationLocationName").Value ##
                               <br />
                               <a href="javascript:openTravelNotesWindow(## DataItem.GetMember("DriverResourceId").Value ##)">##GetSetNotesString(DataItem)##</a>
                               <br />
                               
                            </ComponentArt:ClientTemplate>
                        </ClientTemplates>
                        <ServerTemplates>
                            <ComponentArt:GridServerTemplate id="NowTemplate">
                                 <Template>
                                    
                                 </Template>
                            </ComponentArt:GridServerTemplate>
                            <ComponentArt:GridServerTemplate id="TomorrowTemplate">
                                 <Template>
                                    
                                 </Template>
                            </ComponentArt:GridServerTemplate>
                        </ServerTemplates>
                </ComponentArt:Grid>
        </Template>
       
       </ComponentArt:NavigationCustomTemplate>
       
      </Templates>
      </ComponentArt:NavBar>
      
      
          
    </div>
    
    
    </form>
    
    <script language="javascript" type="text/javascript">
        function GetSetNotesString(note)
        {
            var notes = note.GetMember("TravelNotes").Value;
            if (notes.length > 0 )
                return notes;
            else
                return "Set  Notes";
        }
    </script>
</body>
</html>
