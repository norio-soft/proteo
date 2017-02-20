<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.POD.POD_podviewer" Codebehind="podviewer.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="header" Src="~/UserControls/header.ascx" %>
<%@ Register TagPrefix="uc2" TagName="footer" Src="~/UserControls/footer.ascx" %>

<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<html>
    <head runat="server">
        <link rel="stylesheet" href="../style/styles.css" type="text/css" />
    </head>
    <body leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    
<style>
    <!--
       
.Thumbnail 
{
  border: 1px solid #DDDDDD;
  cursor: pointer; 
}

.ThumbnailHover 
{
  border: 1px solid #AAAAAA;
  cursor: pointer; 
}

.ThumbnailSelected
{
  border: 1px solid #DD3409;
  cursor: pointer; 
}

.ScrollItem 
{
  color:black; 
  border: 1px solid #919B9C; 
  font-family:MS Sans Serif, Verdana; 
  font-size:10px; 
  cursor:pointer;
}

.ScrollItemHover 
{
  color:black; 
  border: 1px solid #919B9C; 
  background-color: #E3E3E3; 
  font-family:MS Sans Serif, Verdana; 
  font-size:10px; 
  cursor:pointer;
}

.ScrollItemActive
{
  color:black; 
  background-color: #E3E3E3; 
  border: 1px solid #DD3409; 
  font-family:MS Sans Serif, Verdana; 
  font-size:10px; 
  cursor:pointer;
}

    -->
</style>
<script type="text/javascript">
    // Image preloading
    var img1 = new Image();
    img1.src = 'images/spinner.gif';

    function initGallery()
    {
      if(!AjaxThumbnails.GetParameter())
      {
       selectPage(0)
      }
    }

    function setCategory(categoryXml, firstImageUrl, firstImageName, init)
    {
      ThumbnailsTabStrip.Destroy();
      AjaxThumbnails.Callback(categoryXml)
      selectImage(firstImageUrl, firstImageName); 
    }

    function selectPage(pageId)
    {
      //AjaxImage.Callback(pageId); 
      
      AjaxImageTitle.Callback(pageId + 1);     
      
      var surl = "/pod/getimage.aspx?scannedformid=148&page=" + pageId +"&.jpg";
      var retVal  = Form1.WebTwain1.HTTPDownload("localhost", surl);
      if (!retVal)
      {
        alert(Form1.WebTwain1.ErrorString);
      }

      
    }
    </script>

<form runat="server" Id="Form1">
       
                                <cs:webmodalwindowhelper id="mwhelper" runat="server" showversion="false"></cs:webmodalwindowhelper>    
                              <table height="100%" width="100%" >
                              <tr>
                              
                              <td>
                                <table cellspacing="0" cellpadding="0" border="0" style="background-image:url(../images/thumbnailsHeading.gif);width:100px;height:101px;" align="center">
                                    <tr>
                                      <td style="font-face:verdana;font-size:11px;font-weight:bold;color:#999999;padding-left:5px;width:100px;height:21px;">
                                        Current Page: 
                                      </td>
                                      <td style="font-face:verdana;font-size:11px;font-weight:bold;color:#666666;width:435px;height:21px;">
                                       <ComponentArt:CallBack id="AjaxImageTitle" CacheContent="true" runat="server">
                                         <Content>
                                           <asp:Label id="ImageTitle" runat="server" /> 
                                         </Content>
                                       </ComponentArt:CallBack> 
                                     </td>
                                    </tr>
                                    <tr>   
                                     <td colspan="2" style="background-color:#F3F3F3; border:1px solid #888888;border-top-color:#CCCCCC;width:535px;height:80px;">
                                       <ComponentArt:CallBack id="AjaxThumbnails" CacheContent="true" runat="server" Height="75">
                                         <Content>
                                           <ComponentArt:TabStrip id="ThumbnailsTabStrip" 
                                             DefaultItemLookId="DefaultTabLook"
                                             DefaultSelectedItemLookId="SelectedTabLook"
                                             DefaultGroupTabSpacing="2"
                                             ImagesBaseUrl="../images"
                                             ScrollingEnabled="true"
                                             ScrollLeftLookId="ScrollItem"
                                             ScrollRightLookId="ScrollItem"
                                             Width="533"
                                             Visible="true"
                                             runat="server"
                                             >
                                           <ItemLooks>
                                             <ComponentArt:ItemLook LookId="DefaultTabLook" CssClass="Thumbnail" HoverCssClass="ThumbnailHover" />
                                             <ComponentArt:ItemLook LookId="SelectedTabLook" CssClass="ThumbnailSelected" />
                                             <ComponentArt:ItemLook LookId="ScrollItem" CssClass="ScrollItem" HoverCssClass="ScrollItemHover" ActiveCssClass="ScrollItemActive" LabelPaddingLeft="1" LabelPaddingRight="1" />
                                           </ItemLooks>
                                           </ComponentArt:TabStrip>
                                          <input type="button" value="Edit Image" onclick="return editImage();" />
                                          <input type="button" value="Rotate Left" onclick="return rotateLeft();" />
                                           <input type="button" value="Change Size" onclick="return changeSize();" />
                                         </Content>
                                         <LoadingPanelClientTemplate>
                                           <table width="533" height="75" cellspacing="0" cellpadding="0" border="0">
                                           <tr>
                                             <td align="center">
                                             <table cellspacing="0" cellpadding="0" border="0">
                                             <tr>
                                                <td style="font-size:10px;">Loading... </td>
                                                <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                             </tr>
                                             </table>      
                                             </td>
                                           </tr>
                                           </table>
                                         </LoadingPanelClientTemplate>
                                       </ComponentArt:CallBack> 
                                     </td>
                                    </tr>
                                   </table> 
                                  </td>
                                  </tr>
                                  <tr height="90%">
                                    <td>
                                        <object classid="clsid:FFC6F181-A5CF-4EC4-A441-093D7134FBF2"  width="100%" height="100%" id="WebTwain1" ondblclick="javascript:alert('click');" >
                                        </object>
                                    </td>
                                 </tr>
                                 </table>
                                <ComponentArt:CallBack id="AjaxImage" CacheContent="true" runat="server" Style="border: 1px solid #888888;" Visible="false"  >
                                     <Content>
                                        <asp:Image id="CurrentImage" Width="10" Height="10" runat="server" /> 
                                     </Content>
                                     <LoadingPanelClientTemplate>
                                       <table width="10" height="10" cellspacing="0" cellpadding="0" border="0">
                                       <tr>
                                         <td align="center">
                                         <table cellspacing="0" cellpadding="0" border="0">
                                         <tr>
                                    <td style="font-size:10px;">Loading... </td>
                                    <td><img src="../images/spinner.gif" width="16" height="16" border="0"></td>
                                         </tr>
                                         </table>      
                                         </td>
                                       </tr>
                                       </table>
                                     </LoadingPanelClientTemplate>
                                </ComponentArt:CallBack> 
                                
                                
    <script language="javascript" defer>
        //initGallery();
        
        
            var surl = "/images/scan/scan.jpg";
            var retVal  = Form1.WebTwain1.HTTPDownload("localhost", surl);
            if (!retVal)
            {
                alert(Form1.WebTwain1.ErrorString);
            }

            function editImage()
            {
//                Form1.WebTwain1.ImageEditorIfEnableEnumerator  = true;
                //Form1.WebTwain1.ShowImageEditor();
                Form1.WebTwain1.IfFitWindow = !Form1.WebTwain1.IfFitWindow ;
            }
            
            function rotateLeft()
            {
                Form1.WebTwain1.RotateLeft(0);
            }
            
            function showEditor()
            {
                alert(here);
                Form1.WebTwain1.ShowImageEditor();
            }        
            
            function changeSize()
            {
                Form1.WebTwain1.ChangeImageSize(0, 1024, 1280,0);
            }
    </script>
 </form>
</body>
</html>
