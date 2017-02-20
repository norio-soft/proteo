<%@ Page Language="C#" AutoEventWireup="true" Inherits="Traffic_TSHeader" Codebehind="TSHeader.aspx.cs" %>

<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Traffic Sheet Header</title>
    <link href="<%=Page.ResolveUrl("~/style/Styles.css")%>" type="text/css" rel="stylesheet">

    <script language="javascript" src="<%=Page.ResolveUrl("~/script/scripts.js")%>" type="text/javascript"></script>

    <script language="javascript" type="text/javascript">
		<!--
			var returnUrlFromPopUp = window.location;

			function SubmitSearchRedirect(strUrl)
			{
				var frm = document.forms["frmSiteSearch"];
				if (frm != null)
				{
					frm.action = strUrl;
					frm.submit();
				}
			}
			
			function checkKeyPress(myForm)
			{
				if (window.event && window.event.keyCode == 13)
					myForm.submit();
				else
					return true;
			}
		//-->
    </script>

</head>
<body bgcolor="#cccccc" leftmargin="0" topmargin="0" rightmargin="0" bottommargin="0">
    <form id="form1" runat="server">
        <table id="ControlPanelLayout" style="border-collapse: collapse" bordercolor="#336699"
            cellspacing="0" cellpadding="0" width="100%" align="center" bgcolor="#ffffff"
            border="1" height="100%">
            <tr>
                <td background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r1_c1.gif")%>" height="53"
                    id="noPrintTD">
                    <div id="noPrint1">
                        <table cellspacing="0" cellpadding="0" width="100%" border="0">
                            <tr height="49">
                                <td style="padding-top: 0px;" width="120">
                                    <a href="<%=Page.ResolveUrl("~/default.aspx") %>">
                                        <img src="<%=Page.ResolveUrl("~/images/orchestrator.gif")%>" border="0" /></a>
                                </td>
                                <td class="ControlTitleCanvas" align="left">
                                    &nbsp;<asp:Label runat="server" ID="lblTitle" CssClass="ControlTitleHeader">Orchestrator</asp:Label>
                                    <br>
                                    &nbsp;&nbsp;<asp:Label runat="server" ID="lblSubTitle" CssClass="ControlSubTitleHeader"></asp:Label>
                                </td>
                                <td height="49" align="right" valign="middle" nowrap>
                                    <form id="frmSiteSearch" action="<%=Page.ResolveUrl("~/job/jobsearch.aspx")%>" method="get">
                                        <div>
                                            <input type="checkbox" id="chkFilterDates" name="chkFilterDates" checked="true" /><label
                                                for="chkFilterDates" style="padding-bottom: 31px; color: white;">Filter Dates</label>
                                            <input type="text" name="searchString" onkeydown="checkKeyPress(this.form);" style="vertical-align: -5px;" />
                                            <a target="_self" onclick="javascript:SubmitSearchRedirect('<%=Page.ResolveUrl("~/job/jobsearch.aspx")%>'); javascript:return false;"
                                                title="Go" accesskey="0">
                                                <img src="<%=Page.ResolveUrl("~/images/dev/gosearch.gif")%>" align="absmiddle" title="Go"
                                                    accesskey="0" style="vertical-align: -7px;" border="0"></a>&nbsp;
                                        </div>
                                    </form>
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td height="24" id="noPrintTD1">
                    <table style="color: #ffffff; background-color: #3568cc" cellspacing="0" cellpadding="2"
                        width="100%" border="0">
                        <tr>
                            <td align="center" width="165" background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r2_c1.gif")%>">
                            </td>
                            <td align="center" width="40" background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r2_c1.gif")%>">
                            </td>
                            <td align="right" background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r2_c1.gif")%>">
                                <a title="Home" href="<%=Page.ResolveUrl("~/default.aspx")%>">
                                    <img src="<%=Page.ResolveUrl("~/images/dev/orangedots.gif")%>" border="0"></a></td>
                            <td valign="middle" background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r2_c1.gif")%>">
                                <div id="PageMenu1_maxMenu" style="z-index: 99;">
                                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                        <tr>
                                            <td>
                                                <ComponentArt:Menu ID="mnuMainMenu" Orientation="Horizontal" CssClass="TopGroup"
                                                    DefaultGroupCssClass="MenuGroup" SiteMapXmlFile="~/UserControls/adminMenu.xml"
                                                    DefaultItemLookId="DefaultItemLook" TopGroupItemSpacing="1" DefaultGroupItemSpacing="1"
                                                    ImagesBaseUrl="~/images/" ExpandDelay="50" runat="server" EnableViewState="False"
                                                    OverlayWindowedElements="true">
                                                    <ItemLooks>
                                                        <ComponentArt:ItemLook LookId="TopItemLook" CssClass="TopMenuItem" HoverCssClass="TopMenuItemHover"
                                                            ExpandedCssClass="TopMenuItemActive" LabelPaddingLeft="10" LabelPaddingRight="10"
                                                            LabelPaddingTop="2" LabelPaddingBottom="2" />
                                                        <ComponentArt:ItemLook LookId="DefaultItemLook" CssClass="MenuItem" HoverCssClass="MenuItemHover"
                                                            ExpandedCssClass="MenuItemHover" LabelPaddingLeft="10" LabelPaddingRight="10"
                                                            LabelPaddingTop="3" LabelPaddingBottom="4" />
                                                        <ComponentArt:ItemLook LookId="BreakItem" CssClass="MenuBreak" />
                                                    </ItemLooks>
                                                </ComponentArt:Menu>
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td align="right" background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r2_c3.gif")%>"
                                width="118">
                                <!--<input type="button" onClick="javascript:window.location.href='<%=Page.ResolveUrl("~/traffic/myTrafficSheet.aspx")%>';" value="Traffic Sheet" style="height: 20px; font-size: 8pt;">-->
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
