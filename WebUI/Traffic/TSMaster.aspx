<%@ Page Language="C#" AutoEventWireup="true" Inherits="Traffic_TSMaster" ValidateRequest="false" CodeBehind="TSMaster.aspx.cs" Title="Traffic Sheet" %>

<%@ Register TagPrefix="ComponentArt" Namespace="ComponentArt.Web.UI" Assembly="ComponentArt.Web.UI" %>

<!doctype html>
<html lang="en">

<head id="Head1" runat="server">
    <meta charset="utf-8" />

    <title>Haulier Enterprise</title>
    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
    <script src="/script/jquery-migrate-1.2.1.js"></script>
    <script src="/script/show-modal-dialog.js"></script>
    <script src="/script/toolTipPopUps.js" type="text/javascript"></script>
    <script src="/script/jquery.cycle.all.js" type="text/javascript"></script>
    <script src="/script/jquery.quicksearch.pack.js" type="text/javascript"></script>
    <script src="/script/shortcut.js" type="text/javascript"></script>
    <script src="/script/Silverlight.js" type="text/javascript"></script>
    <script src="/script/cookie-session-id.js" type="text/javascript"></script>

    <script type="text/javascript">
        var returnUrlFromPopUp = window.location;
        var search = true;

        function performSearch(searchString) {
            var dateFilter = document.getElementById('chkFilterDates');

            var url = "/job/jobsearch.aspx";
            var surl = url + "?state=0&field=0&searchString=" + searchString + "&chkFilterDates=" + dateFilter.checked;
            top.window.location.href = surl;
            return false;
        }

        function checkKeyPress(sender) {
            if (window.event && window.event.keyCode == 13) {
                SubmitSearchRedirect(sender.value);
                return false;
            }
            else
                return true;
        }

        var hasBeenExpanded = false;
        function onResourceExpand(DomElementId) {
            if (!hasBeenExpanded)
                loadResourceFrame();
            hasBeenExpanded = true;

        }

        function startCollapsed() {
            if (Splitter1.Panes[1].Panes[0].Collapsed == false) {
                loadResourceFrame();
            }
            else
                hasBeenExpanded = false;
        }

        function loadResourceFrame() {

            var resourceUrl = "TSResource.aspx" + getCSIDSingle();

            document.getElementById('tsResource').contentWindow.document.location.href = resourceUrl;
        }

        $(document).ready(function () {

            $(".collapsingFieldset legend:first").addClass("active");
            $(".collapsingFieldset .collapsingFieldsetInner").hide();


            $(".collapsingFieldset legend").click(function () {

                $(this).next(".collapsingFieldsetInner").slideToggle("slow")
	            .siblings("collapsingFieldsetInner:visible").slideUp("slow");
                $(this).toggleClass("active");
                $(this).siblings("legend").removeClass("active");

            });

            $("#tsResource").height($("#splitterContainer").height() + 28);
            $("#tsTrafficSheet").height($("#splitterContainer").height() - 72);

        });

        function popup(url, width, height) {
            var options = 'height=' + height + ',width=' + width + ',toolbar=no,menu=no,scrollbars=yes';
            newwindow = window.open(url, 'name', options);
            if (window.focus) { newwindow.focus() }
        }

    </script>
    <style type="text/css">
        body
        {
            margin: 0px;
            overflow: hidden;
            width: 100%;
            height: 100%;
        }
        
        .masterpagelite_headerNews
        {
            margin: 0px !important;
        }
    </style>
    <link href="/style/Styles.css" type="text/css" rel="stylesheet" />
    <link href="/style/splitter.css" rel="stylesheet" type="text/css" />
    <link href="/style/newMasterPage.css" type="text/css" rel="stylesheet" />
    <link href="/style/newStyles.css" type="text/css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server" style="height: 100%;">
        <asp:ScriptManager ID="scriptManager" runat="server" EnablePageMethods="True" EnablePartialRendering="true">
        </asp:ScriptManager>
        <input type="hidden" id="hidDriverResourceId" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidVehicleResourceId" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidTrailerResourceId" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidDriverResourceName" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidVehicleResourceName" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidTrailerResourceName" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidLinkJobSourceJobId" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidLinkJobSourceInstructionId" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidGridFilter" runat="server" value="" enableviewstate="false" />
        <input type="hidden" id="hidResourceIsOpen" runat="server" />
        <input type="hidden" id="hidQuickSearchText" value="" />
    
        <div class="masterpagelite_layoutContainer">
            <div class="masterpagelite_layoutHeaderInner">
                <div runat="server" id="divMenuBar">
                    <div class="masterpagelite_headerLogo">
                        <a href="/default.aspx">
                            <span class="masterpagelite_headerLogoImg"></span>
                        </a>
                    </div>
                    <div class="masterpagelite_headerNavSearchContainer">
                        <div class="masterpagelite_headerTitle">
                            <h1>Traffic Sheet</h1>
                        </div>
                        <div class="masterpagelite_searchBox">
                            <div class="masterpagelite_searchInput">
                                <asp:TextBox ID="txtSearchString" runat="server" AutoPostBack="false" onkeydown="if(!checkKeyPress(this)) return false;" />
                                <a target="_self" onclick="javascript:SubmitSearchRedirect();" title="Go" accesskey="0">
                                    <span class="masterpagelite_searchImage"></span>
                                </a>
                                <div class="masterpagelite_clearDiv">
                                </div>
                            </div>
                            <div class="masterpagelite_clearDiv">&nbsp;</div>
                        </div>
                        <div class="masterpagelite_searchFilterOptions">
                            <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
                                <asp:RadioButtonList ID="cblSearchType" runat="server" RepeatDirection="Horizontal" ForeColor="White">
                                    <asp:ListItem Text="Orders" Value="orders" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Runs" Value="runs"></asp:ListItem>
                                </asp:RadioButtonList>
                            </telerik:RadCodeBlock>
                        </div>
                        <div class="masterpage_searchCheckbox">
                            <input type="checkbox" id="chkFilterDates" name="chkFilterDates" checked="true" />
                            <label for="chkFilterDates" style="color: white;">Filter Dates</label>
                        </div>

                        <div class="masterpagelite_navControl">
                            <telerik:RadMenu runat="server" EnableOverlay="true" GroupSettings-OffsetX="0" GroupSettings-OffsetY="0"
                                AppendDataBoundItems="True" ID="RadMenu1" CausesValidation="false" ClickToOpen="true">
                            </telerik:RadMenu>
                        </div>
                        <div class="masterpagelite_layoutHeader" runat="server" id="divLayoutHeader">
                            <div class="masterpagelite_layoutNav" runat="server" id="divLayoutNav">
                                <div class="masterpagelite_layoutNavInner">
                                    <div class="masterpagelite_clearDiv">
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="masterpagelite_headerNews">
                        </div>
                        <div class="masterpagelite_headerSearch">
                            <div class="masterpagelite_clearDiv">
                            </div>
                        </div>
                    </div>
                </div>

                <input type="button" runat="server" id="btnSearch" style="display: none;" />
            </div>
        </div>

        <div id="splitterContainer" style="height: 100%; width:99%">
            <ComponentArt:Splitter runat="server" ID="Splitter1" ImagesBaseUrl="~/images/splitter/"
                FillWidth="true" FillContainer="true" ClientSideOnExpand="onResourceExpand">
                <Layouts>
                    <ComponentArt:SplitterLayout>
                        <Panes Orientation="Vertical" Resizable="false" SplitterBarCssClass="VerticalSplitterBar"
                            SplitterBarActiveCssClass="ActiveSplitterBar" SplitterBarWidth="2">
                            <ComponentArt:SplitterPane Width="100%">
                                <Panes Orientation="Horizontal" SplitterBarCollapseImageUrl="~/images/splitter/splitter_horCol.gif"
                                    SplitterBarCollapseHoverImageUrl="~/images/splitter/splitter_horColHover.gif"
                                    SplitterBarExpandImageUrl="~/images/splitter/splitter_horExp.gif" SplitterBarExpandHoverImageUrl="~/images/splitter/splitter_horExpHover.gif"
                                    SplitterBarCollapseImageWidth="13" SplitterBarCollapseImageHeight="116" SplitterBarCssClass="HorizontalSplitterBar"
                                    SplitterBarCollapsedCssClass="CollapsedHorizontalSplitterBar" SplitterBarActiveCssClass="ActiveSplitterBar"
                                    SplitterBarWidth="8">
                                    <ComponentArt:SplitterPane PaneContentId="ResourceContent" Width="550" MinWidth="100"
                                        CssClass="SplitterPane" Collapsed="true" />
                                    <ComponentArt:SplitterPane PaneContentId="TrafficSheetContent" CssClass="SplitterPane" />
                                </Panes>
                            </ComponentArt:SplitterPane>
                        </Panes>
                    </ComponentArt:SplitterLayout>
                </Layouts>
                <Content>
                    <ComponentArt:SplitterPaneContent ID="ResourceContent">
                        <iframe src="about:blank" width="100%" scrolling="no" id="tsResource" frameborder="0" style="width: 100%;"></iframe>
                    </ComponentArt:SplitterPaneContent>
                    <ComponentArt:SplitterPaneContent ID="TrafficSheetContent" Style="overflow: hidden;">
                        <iframe width="100%" scrolling="auto" id="tsTrafficSheet" frameborder="0" style="width: 100%;"></iframe>
                    </ComponentArt:SplitterPaneContent>
                </Content>
            </ComponentArt:Splitter>
        </div>
        <telerik:RadCodeBlock ID="RadCodeBlock2" runat="server">
            <script language="javascript" type="text/javascript"> <!-- 
            function SubmitSearchRedirect() { 
            // if searching for orders. 
            if ($('input:radio:checked[id*=cblSearchType]').val()== "orders") 
            location.href = "/groupage/findorder.aspx?ss=" + $('input[id*=txtSearchString]').val() + '&filterDates=' + $('input[id*=chkFilterDates]').val(); 
            else 
                location.href = "/job/jobsearch.aspx?searchString=" + $('input[id*=txtSearchString]').val() + '&filterDates=' + $('input[id*=chkFilterDates]').val();
            //$get("<%=btnSearch.ClientID %>").click(); 
        } //--> 
        </script>
        </telerik:RadCodeBlock>
    </form>

    <script type = "text/javascript">

        $(document).ready(function () {

            if (sessionStorage) {
                getCookieSessionID();
                setIFrameSource('tsTrafficSheet', 'TSTrafficSheet.aspx?csid=' + sessionStorage.sessionID);
            }
            else {
                setIFrameSource('tsTrafficSheet', 'TSTrafficSheet.aspx' + sessionStorage.sessionID);
            }
        });

        function setIFrameSource(cid, url) {
            var myframe = document.getElementById(cid);
            if (myframe !== null) {
                if (myframe.src) {
                    myframe.src = url;
                }
                else if (myframe.contentWindow !== null && myframe.contentWindow.location !== null) {
                    myframe.contentWindow.location = url;
                }
                else {
                    myframe.setAttribute('src', url);
                }
            }
        }


    </script>

</body>
</html>
