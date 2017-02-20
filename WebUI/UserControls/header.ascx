<%@ Reference Control="~/usercontrols/title.ascx" %>
<%@ Control Language="c#" Inherits="Orchestrator.WebUI.UserControls.header" Codebehind="header.ascx.cs" %>
<%@ Register TagPrefix="uc1" TagName="callInSelector" Src="~/UserControls/CallInSelector.ascx" %>
<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<!-- Running on :<%=HttpContext.Current.Server.MachineName%>.  -->
<HTML>
	<!--
	Load the LPK (license package file for ctSchedule).
	Only required for runtime deployment.
	This must be the first object tag in a page.
	The LPK file is created with the LPK Tool from Microsoft.
	The license information for all licensed objects on a page must
	included in this single file.
	See the FAQ at www.dbi-tech.com for more information
	-->
	
	<head id="hdr" runat="server">
		<title><%=PageTitle%></title>
		<base target="_self">
		
		<link href="/style/Styles.css" type="text/css" rel="stylesheet" />
		<link href="/style/newMasterPage.css" type="text/css" rel="stylesheet" />
		<link href="/style/newStyles.css" type="text/css" rel="stylesheet" />
        <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js" type="text/javascript"></script>
        <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
        <script src="/script/jquery-migrate-1.2.1.js" type="text/javascript"></script>
        <script src="/script/show-modal-dialog.js"></script>
		<script language="javascript" src="/script/jquery.cycle.all.js" type="text/javascript"></script>
		<script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
        <script language="javascript" src="/script/cookie-session-id.js" type="text/javascript"></script>
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
$(document).ready(function() {
    var json = "{}";
    $.ajax({
        type: "POST",
        contentType: "application/json; charset=utf-8",
        url: "/ws/news.asmx/GetNews",
        data: json,
        dataType: "json",
        success: ShowNews,
        error: ShowError
    });
});

function ShowNews(result) {
    
    $('#traffic').html(result.d);
    

    $('#traffic').cycle({
        fx: 'scrollRight',
        delay: -1000,
        next: '#traffic'
    });
}
function ShowError(o) {
    $('#traffic').html("");
}
		//-->
		</script>
	</head>
<body onload="<%=m_onLoadScript%>" style="margin: 0px;">
    <table id="ControlPanelLayout" class="layoutContainer" cellspacing="0" cellpadding="0" width="<%=PageWidth%>" align="center" height="100%">
        <asp:Panel ID="pnl1" runat="server">
        
            <!--Header row - Contains the logo, traffic news and search box-->
            <asp:Panel ID="pnlDialogHeading" runat="server" Visible="true">
                <tr style="display:<%=IsWizard%>">
                    <td class="layoutHeaderRow" id="tdHeaderBackground" runat="server">
                        <div id="noPrint1">
                            <table cellSpacing="0" cellPadding="0" width="100%" border="0">
                                <tr height="49">
                                    <td class="layoutHeaderLogo" width="50" align="left">
                                        <a href="<%=Page.ResolveUrl("~/default.aspx") %>"><span class="HeaderLogoImg"></span></a>
                                    </td>
                                    <td>
                                         <asp:panel ID="pnlNews" runat="server">
					                         <div id="traffic">
					                            <span class="LoadingNews">Loading news...</span>
                                             </div>
							             </asp:panel>
                                    </td>
                                    <td height="49" align="right" valign="middle">
                                        <asp:Panel ID="pnlSearch" runat="server">
                                        <form id="frmSiteSearch" action="<%=Page.ResolveUrl("~/job/jobsearch.aspx")%>" method="get">
                                            <div style="display:<%=IsWizard%>">
                                                <div class="layoutSearch">
                                                    <div class="layoutSearchDropDown">
                                                        <select name="state" style="VERTICAL-ALIGN: -5px;display:none;"><!-- size="1" multiple="true"-->
                                                            <option selected="true" value="0">All States</option>
                                                            <option value="1">Booked</option>
                                                            <option value="2">Planned</option>
                                                            <option value="3">In Progress</option>
                                                            <option value="4">Completed</option>
                                                            <option value="5">Booking in Incomplete</option>
                                                            <option value="6">Booking in Complete</option>
                                                            <option value="7">Ready to Invoice</option>
                                                            <option value="8">Invoiced</option>
                                                            <option value="9">Cancelled</option>
                                                        </select>
                                                        &nbsp;
                                                        <select name="field" style="VERTICAL-ALIGN: -5px;display:none;">
                                                            <option selected="true" value="0">All Fields</option>
                                                            <option value="1">Job ID</option>
                                                            <option value="2">Load Number</option>
                                                            <option value="3">Docket Number</option>
                                                            <option value="4">Driver First Name(s)</option>
                                                            <option value="5">Driver Surname</option>
                                                            <option value="6">Vehicle Registration</option>
                                                            <option value="7">Trailer Reference</option>
                                                            <option value="8">Job Reference</option>
                                                            <option value="9">Organisation Name</option>
                                                        </select>
                                                    </div>
                                                    <div class="layoutSearchBox">
                                                        <div class="layoutSearchBoxInner">
                                                            <input type="text" name="searchString" onkeydown="checkKeyPress(this.form);" />
                                                            <a target="_self" onclick="javascript:SubmitSearchRedirect('<%=Page.ResolveUrl("~/job/jobsearch.aspx")%>'); javascript:return false;" title="Go" accesskey="0"><img src="<%=Page.ResolveUrl("~/images/newMasterPage/icon-search.png")%>" align="absmiddle" title="Go" accesskey="0" border="0" /></a>&nbsp;
                                                        </div>
                                                    </div>
                                                    <div style="clear: both; height: 0px; line-height: 0px;">
                                                    
                                                    </div>
                                                    <div class="layoutSearchBoxFilter">
                                                        <input type="checkbox" id="chkFilterDates" name="chkFilterDates" checked="true" /><label for="chkFilterDates" style="padding-bottom:31px;color:white;" >Filter Dates</label>
                                                    </div>
                                                </div>
                                            </div>
                                        </form>
</asp:Panel>
									<asp:Image ID="imgPoweredByLogo" runat="server" Visible="false" ImageUrl="/images/newmasterpage/orch-poweredby-logo.png" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>	
                </tr>
            </asp:Panel>
            
            <!--Header Seperator - The blue bar that seperates the header  -->
            <tr>
                <td class="layoutHeaderSeperator">
                    &nbsp;
                </td>
            </tr>
            
            <!--Navigation row - Contains the home link, navigation links and version # -->
            <tr style="display:<%=HasMenu%>">
                <td height="24" id="noPrintTD1">
                    <table cellSpacing="0" cellPadding="2" width="100%" border="0">						
                        <tr>
                            <td align="right"  class="layoutNavigationHomeLink">    
                                <A title="Home" href="<%=Page.ResolveUrl("~/default.aspx")%>"><img src="/images/newMasterpage/icon-home.png" alt="Home" /></A>
                            </td>
                            <td vAlign="middle" class="layoutNavigationMiddle">
                                <div id="PageMenu1_maxMenu" style="z-index:99;"> 
                                    <table cellpadding="0" cellspacing="0" border="0" width="100%">
                                        <tr>
						                    <td>
						                        <asp:Literal ID="litWhiteLabelmenu" runat="server"></asp:Literal>
						                        
						                        
                                                <telerik:RadMenu runat="server" GroupSettings-OffsetX="0" GroupSettings-OffsetY="0" AppendDataBoundItems="True" ID="RadMenu1" >
                        </telerik:RadMenu> 
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </td>
                            <td align="right" class="layoutNavigationVersion" width="118" runat="server" id="tdMenuBar5">
                            <%=Orchestrator.WebUI.Utilities.GetVersionNumber(this.Page.Cache) %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            
            
        </asp:Panel>
        
        
    <tr>
        <td vAlign="top">
            <table cellSpacing="0" cellPadding="0" width="100%" height="100%" >
                <tr height="100%">
                    <td vAlign="top" >
                        <table cellSpacing="0" cellPadding="0" width="100%" height="100%">
                            <asp:Panel ID="pnlWizardHeading" runat="server" Visible="false">
                            <tr>
                                <td vAlign="top" background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r1_c1.gif")%>" height="53">
                                <!-- Current Control - Header -->
                                    <table cellSpacing="0" cellPadding="4" width="100%" style="display: <%=Request.QueryString["jobId"] == null ? "none" : "inline" %>">
                                        <tr>
                                            <td class="ControlTitleCanvas">
                                                &nbsp;<asp:label runat="server" id="lblTitle1" cssclass="ControlTitleHeader">Orchestrator</asp:label><br>&nbsp;&nbsp;<asp:label runat="server" id="lblSubTitle1" cssclass="ControlSubTitleHeader" ></asp:label>
                                            </td>
                                            <td >
                                                <table cellSpacing="0" cellPadding="0" width="100%">
                                                    <tr>
                                                        <td align="right">
                                                            <table cellPadding="0">
                                                                <tr>
                                                                    <td>
                                                                        <iframe marginheight="0" marginwidth="0" frameborder="no" scrolling="no" width="200px" height="20px" style="visible:true;" src='<%=Page.ResolveUrl("~/traffic/jobManagement/CallInSelector.aspx?JobId=")%><%=Request.QueryString["JobId"]%>'></iframe>
                                                                    </td>
                                                                <td>
                                                                </td>
                                                                <!--td><A href="javascript:openHelp(<%=HelpId%>);"><IMG alt="Get Help" src="<%=Page.ResolveUrl("~/images/dev/help_button.gif")%>" border="0"></A></td-->
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            
                            <tr>
                                <td background="<%=Page.ResolveUrl("~/images/hms_menu_bar_r2_c1.gif")%>" height="24">
                                    <span style="display: <%=Request.QueryString["JobId"] == null ? "none" : "inline" %>">
                                        <span style="display: <%=Request.QueryString["wiz"] != null ? "none" : "inline" %>">
                                            <!-- These must open in the pop up window -->
                                            <table border="0" cellpadding="0" cellspacing="2">
                                                <tr>
                                                    <td><input type="button" style="width:75px" value="Details" onclick="javascript:openDialogWithScrollbars('<%=Page.ResolveUrl("~/job/job.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID(),'600','400');" /></td>
                                                    <td><input type="button" style="width:75px" value="Coms" onclick="javascript:openDialogWithScrollbars('<%=Page.ResolveUrl("~/traffic/JobManagement/driverCommunications.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID(),'600','400');" /></td>
                                                    <td><input type="button" style="width:75px" value="Call-In" onclick="javascript:openDialogWithScrollbars('<%=Page.ResolveUrl("~/traffic/JobManagement/DriverCallIn/CallIn.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID(),'600','400');" /></td>
                                                    <td><input type="button" style="width:75px" value="PODs" onclick="javascript:openDialogWithScrollbars('<%=Page.ResolveUrl("~/traffic/JobManagement/bookingInPODs.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID(),'600','400');" /></td>
                                                    <td><input type="button" style="width:75px" value="Pricing" onclick="javascript:openDialogWithScrollbars('<%=Page.ResolveUrl("~/traffic/JobManagement/pricing2.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID(),'600','400');" /></td>
                                                </tr>
                                            </table>
                                        </span>
                                        <span style="display: <%= Request.QueryString["wiz"] == null ? "none" : "inline" %>">
                                        <!-- These must open in this window -->
                                            <table border="0" cellpadding="0" cellspacing="2">
                                                <tr>
                                                    <td><input type="button" style="width:75px" value="Details" onclick="javascript:window.location='<%=Page.ResolveUrl("~/job/job.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                                    <td><input type="button" style="width:75px" value="Coms" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/driverCommunications.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                                    <td><input type="button" style="width:75px" value="Call-In" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/DriverCallIn/CallIn.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                                    <td><input type="button" style="width:75px" value="PODs" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/bookingInPODs.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                                    <td><input type="button" style="width:75px" value="Pricing" onclick="javascript:window.location='<%=Page.ResolveUrl("~/traffic/JobManagement/pricing2.aspx")%>?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" /></td>
                                                </tr>
                                            </table>
                                        </span>
                                    </span>
                                </td>
                            </tr>
                            </asp:Panel>
                            <tr>
                                <td class="layoutContentTop">&nbsp;</td>
                            </tr>
                            <tr>
                                <td class="layoutContentMiddle" vAlign="top" align="left" style="<%= Request.QueryString["wiz"] != null ? "" : "padding-left:5px; padding-right:5px;" %>" >
                                    <div class="layoutContentMiddleInner">
                                        <!-- Begin Content -->
