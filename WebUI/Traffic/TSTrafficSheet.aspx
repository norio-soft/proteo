<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.TSTrafficSheet" EnableEventValidation="false" ValidateRequest="false" CodeBehind="TSTrafficSheet.aspx.cs" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>
<%@ Register TagPrefix="uc" TagName="MwfDriverMessaging" Src="~/UserControls/mwf/DriverMessaging.ascx" %>
<%@ Register TagPrefix="uc" TagName="MwfInstructionHistory" Src="~/UserControls/mwf/InstructionHistory.ascx" %>

<!doctype html>
<html lang="en">

<head id="Head1" runat="server">
    <meta charset="utf-8" />

    <title>Orchestrator - Traffic Sheet</title>

    <script src="//ajax.googleapis.com/ajax/libs/jquery/1.10.2/jquery.min.js"></script>
    <script>window.jQuery || document.write('<script src="/script/jquery-1.10.2.min.js">\x3C/script>')</script>
    <script src="/script/jquery-migrate-1.2.1.js"></script>
    <script src="/script/show-modal-dialog.js"></script>
    <script src="/script/jquery-ui-1.9.2.min.js"></script>
    <script src="/script/jquery.blockUI-2.64.0.min.js"></script>
    <script src="/script/jquery.quicksearch-1.3.1.js"></script>
    <script src="/script/Silverlight.js"></script>
    <script src="/script/tooltippopups.js"></script>
    <script src="/script/jquery.qtip-1.0.0-rc3.min.js"></script>
    <script src="../bower_components/moment/moment.js"></script>

    <style type="text/css">
        html, body {
            margin: 0;
            padding: 0;
            border: none;
            height: 0px !important;
        }

        .stateBooked td.DataCell {
            cursor: default;
            padding: 3px;
            padding-top: 2px;
            padding-bottom: 1px;
            border-bottom: 1px solid #EAE9E1;
            font-family: verdana;
            font-size: 10px;
            background-color: #ffffff;
        }

        .stateInProgress td.DataCell {
            height: 20px;
            background-color: #99FF99;
            cursor: default;
            border-bottom: 1px solid #EAE9E1;
        }

        .stateCompleted td.DataCell {
            height: 20px;
            background-color: LightBlue;
            cursor: default;
            border-bottom: 1px solid #EAE9E1;
        }

        .AlternatingRow td.DataCell {
            cursor: default;
            padding: 3px;
            padding-top: 2px;
            padding-bottom: 1px;
            border-bottom: 1px solid #EAE9E1;
            font-family: verdana;
            font-size: 10px;
        }

        .trafficNotes {
            color: #ff0000;
            font-weight: bold;
        }

        form.quicksearch {
            background-image: url('../images/newMasterpage/bluetoolbar-bg.jpg');
            background-repeat: repeat-x;
            background-position: bottom;
            background-color: #475c75;
            color: #fff;
            padding-top: 2px;
            border: 0px solid #000;
            height: 23px;
            position: fixed;
            top: 26px;
            width: 100%;
            text-align: right;
        }

            form.quicksearch label {
                display: inline;
                font-size: 11px;
                color: #fff;
            }

            form.quicksearch input {
                margin-top: 2px;
                margin-left: 5px;
                margin-right: 5px;
                width: 100px;
                border: 1px solid #18202a;
            }

            form.quicksearch img {
                vertical-align: middle;
                margin-left: 5px;
            }

        .draggableLeg {
            cursor: move;
            width: 23px;
            height: 22px;
            border: none;
            background: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABcAAAAWCAYAAAArdgcFAAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwQAADsEBuJFr7QAAABh0RVh0U29mdHdhcmUAcGFpbnQubmV0IDQuMC4zjOaXUAAAAdpJREFUSEudlL9KA0EQxu/U6KmIRgLmRIkBbYIeF6yFvICQ0sbOVrCwsLewsMoT+AD+aSy11zewEmysfAALi/P7lp1zsvc3Dvwy2d3Zb2Z3NvHiOA5AWEKQPL17wkRGgcFgkBSB9U5VAr2mScWtiK64U5TANS2o0eKhXpB5N4HVy5gRc62uuErQ4hpgr8gWmLFytEaSJPRTWrzNmTJxGzfaPDxLNJhj0h0j6HnTwDeICHzTLhjD2H1FugcHMqcShFEULWErhakTaPFVTKyAm36/vwy/AHJPwu8y55xgHyxynzHZBD+PIY915/v+MzzvMQTGJK5M3Cb4a7xsgl/DkOKs+hY8gnU7xzhek7ka7ikSlwS9Xm9Pi6dVwlj1NfgAp4BJmohpMA6YN09vx2lfVIIwFefzEWCsnhYDnuIL/IA3Oz4CtA5gIRvuScbE6RlACoz3GIFj8AJewS64B1dV4myEPmIeLQjNAj63C8BjElMRY3LFq0DcJ2ABvApeG6sXcbJNLVc8fQV1QOyw2+2ywUPwAL6t+HlGvKDrGv4aR9a3AWPlD4zPlL/sE3CJ+ax4GbKBXs9bm+OHG/sf8ToNH3/neYIaBsmGSagrbhqeJ1BGLXFiE+RdQwlx8Atwe1D7UF0FAgAAAABJRU5ErkJggg==');
        }
    </style>
</head>

<body style="background-color: #FFF;" onmousemove="javascript:closeToolTip();">
    <form id="form1" runat="server">
        <asp:ScriptManager ID="smTSTrafficSheet" runat="server" EnablePageMethods="true" EnablePartialRendering="true" />

        <asp:Panel ID="pnlConfirmation" runat="server" Visible="false">
            <div class="MessagePanel">
                <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_warning.gif" />You
            do not have a default filter, please contact support.
            </div>
        </asp:Panel>

        <cc1:Dialog ID="dlgFilter" URL="/Traffic/Filters/specifyfilter.aspx" Width="675" Height="825" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true" ></cc1:Dialog>
        <cc1:Dialog ID="dlgTrailerType" URL="/Traffic/changetrailertype.aspx" Width="400" Height="200" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgResourceThis" URL="/Traffic/resourcethis.aspx" Width="1000" Left="10"  Height="800" AutoPostBack="true" Mode="Normal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgTrunk" URL="/Traffic/trunk.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgTrafficArea" URL="/Traffic/settrafficarea.aspx" Width="500" Height="221" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgBookedTimes" URL="/Traffic/changebookedtimes.aspx" Width="860" Height="550" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgPlannedTimes" URL="/Traffic/changeplannedtimes.aspx" Width="700" Height="320" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgSubcontract" URL="/Traffic/subcontract.aspx" Width="650" Height="580" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgCommunicate" URL="/Traffic/communicatethis.aspx" Width="800" Height="650" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgRemoveTrunk" URL="/Traffic/removetrunk.aspx" Width="550" Height="358" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgMultiTrunk" URL="/Traffic/multitrunk.aspx" Width="720" Height="450" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
        <cc1:Dialog ID="dlgAddDestination" URL="/job/adddestination.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true" ></cc1:Dialog>
        <cc1:Dialog ID="dlgAddMultipleDestination" URL="/job/addmultidestination.aspx" Width="800" Height="600" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true" ></cc1:Dialog>
        <cc1:Dialog ID="dlgRemoveDestination" URL="/job/removedestination.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true" UseCookieSessionID="true" ></cc1:Dialog>
        <cc1:Dialog ID="dlgChangePlanningDepot" URL="/traffic/changedepot.aspx" Width="550" Height="380" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>

        <telerik:RadContextMenu ID="RadMenu1" runat="server" OnClientItemClicked="radMenu1_itemClicked" OnClientShowing="radMenu1_showing" Skin="Black">

            <Items>
                <telerik:RadMenuItem Text="Resource This" Value="resourcethis" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
                <telerik:RadMenuItem Text="Sub-Contract Leg" Value="subcontractleg" />
                <telerik:RadMenuItem Text="Un-Contract Leg" Value="unsubcontractleg" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
                <telerik:RadMenuItem Text="Communicate This" Value="communicate" />
                <telerik:RadMenuItem Text="Quick Communicate This" Value="quickcommunicate" />
                <telerik:RadMenuItem Text="Remove Communication" Value="uncommunicate" />
                <telerik:RadMenuItem Text="Send MWF Message" Value="sendmwfmessage" Enabled="false" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
                <telerik:RadMenuItem Text="Trunk" Value="trunk" />
                <telerik:RadMenuItem Text="Multi-Trunk" Value="multitrunk" />
                <telerik:RadMenuItem Text="Remove Trunk" Value="removetrunk" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
                <telerik:RadMenuItem Text="Remove Links" Value="removelinks" Visible="false" />
                <telerik:RadMenuItem Text="Change Booked Times" Value="changebookedtimes" Visible="true" />
                <telerik:RadMenuItem Text="Change Planned Times" Value="changeplannedtimes" />

                <telerik:RadMenuItem Text="Job Details" Value="JobDetails" Visible="false" />
                <telerik:RadMenuItem Text="Call In" Value="callin" />

                <telerik:RadMenuItem Text="Show Load Order" Value="showloadorder" Visible="false" />
                <telerik:RadMenuItem Text="Add Destination" Value="adddestination" />
                <telerik:RadMenuItem Text="Add Multiple Destinations" Value="addmultipledestinations" />
                <telerik:RadMenuItem Text="Remove Destination" Value="removedestination" />
                <telerik:RadMenuItem Text="Give Resource" Value="giveresourceto" Visible="false" />
                <telerik:RadMenuItem Text="Link Job" Value="linkjob" Visible="false" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
                <telerik:RadMenuItem Text="Change Planning Category" Value="changetrailertype" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />
                <telerik:RadMenuItem Text="Copy Run" Value="copyrun" />
                <telerik:RadMenuItem runat="server" IsSeparator="True" Text="|" />  
                <telerik:RadMenuItem Text="Change Depot" Value="changedepot" />
            </Items>
            <CollapseAnimation Type="none" />
        </telerik:RadContextMenu>

        <div>
            <div id="dvSearch" style="position: fixed; top: 0px; left: 0px; height: 52px; width: 100%">
                <table style="width: 100%;" cellpadding="0" cellspacing="0" id="filters">
                    <tr class="HeadingCell">
                        <td class="ToolbarSilver" style="border-width: 0px 0px 1px 0px;">
                            <table cellpadding="0" cellspacing="0">
                                <tr>
                                    <td>
                                        <b>
                                            <asp:Label ID="lblDateRange" runat="server" Text="StartDateTime"></asp:Label></b>
                                    </td>
                                    <td id="counts" style="padding-left: 25px; vertical-align: middle; color: black;">
                                        <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
                                            Number of runs to plan : <%=NumberOfJobsToPlan %>&nbsp;|&nbsp;Number of legs to plan : <%=NumberOfLegsToPlan %>
                                        </telerik:RadCodeBlock>
                                    </td>
                                </tr>
                            </table>
                        </td>
                        <td style="width: 150px; text-align: right; border-width: 0px 0px 1px 0px;" class="ToolbarSilver">
                            <asp:Label ID="lblCollectionFilter" runat="server" Style="font-weight: bold;"></asp:Label>
                            <asp:TextBox ID="txtGridSearch" runat="server" onkeydown="searchGrid;" Style="display: none;"></asp:TextBox>
                        </td>
                        <td class="ToolbarSilver" style="width: 350px; border-width: 0px 0px 1px 0px; padding: 0px; line-height: 0px;" align="right" nowrap="nowrap">
                            <img id="imbFilter" src="../images/newMasterPage/button-changefilter.jpg" onclick="openTrafficSheetFilterWindow();" border="0" style="cursor: pointer; margin: 0px;" alt="" />
                            <img id="imbPrint" src="../images/newMasterPage/button-print.jpg" onclick="window.print();" border="0" style="cursor: pointer; margin: 0px;" alt="" />
                            <asp:ImageButton ID="imbRefresh" runat="server" ImageUrl="../images/newMasterPage/button-refresh.jpg"></asp:ImageButton>
                        </td>
                    </tr>
                </table>
            </div>

            <asp:ObjectDataSource ID="objJobs" runat="server" SelectMethod="GetTrafficSheet" TypeName="Orchestrator.Facade.Job"></asp:ObjectDataSource>

            <div style="margin: 51px 0 0;">
                <div id="grdTrafficSheetFilter" style="float: right;"></div>
                <div class="clearDiv"></div>
                <asp:GridView ID="gvTrafficSheet" runat="server" DataSourceID="objJobs" AllowSorting="true"
                    AutoGenerateColumns="false" Width="100%" EnableViewState="true" CellSpacing="0"
                    CellPadding="0" BorderWidth="0" CssClass="Grid">
                    <HeaderStyle CssClass="HeadingRowLite" VerticalAlign="middle" />
                    <RowStyle CssClass="Row" />
                    <SelectedRowStyle CssClass="SelectedRow" />
                    <Columns>
                        <asp:BoundField DataField="InstructionID" Visible="false" />
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label ID="lblHiddenKeys" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" ItemStyle-Width="23">
                            <ItemTemplate>
                                <div class="draggableLeg" draggable="true" style="display: <%# (Orchestrator.eInstructionState)Eval("InstructionStateId") == Orchestrator.eInstructionState.Booked || (Orchestrator.eInstructionState)Eval("InstructionStateId") == Orchestrator.eInstructionState.Planned ? "" : "none" %>" startinstructionid="<%# Eval("StartInstructionID")%>" endinstructionid="<%# Eval("EndInstructionID")%>" jobid="<%#Eval("JobId") %>" trailerresourceid="<%#Eval("TrailerResourceID")%>" joblastupdatedatetime="<%#Eval("LastUpdateDate") %>">&nbsp;</div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" ItemStyle-Width="5">
                            <ItemTemplate>
                                <asp:Literal ID="litLegImage" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="ID" SortExpression="JobId">
                            <ItemTemplate>
                                <a href="javascript:oJDW(<%#Eval("JobId") %>)"><%#Eval("JobId") %></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" ItemStyle-Width="20">
                            <ItemTemplate>
                                <img id="imgInstructionOrderNotes" runat="server" src="~/images/postit.gif" alt="" />
                                <img id="imgHasPCV" runat="server" src="/App_Themes/Orchestrator/Img/MasterPage/icon-pcv.png" alt="There are PCVs attached to this run." />
                                <div class="trafficNotes"><%# Eval("TrafficNotes") %></div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="TA" SortExpression="DepotCode" Visible="false">
                            <ItemStyle Width="35" />
                            <ItemTemplate>
                                <a href="javascript:openTrafficAreaWindow('InstructionID=<%# Eval("StartInstructionID")%>&LastUpdateDate=<%#Eval("LastUpdateDate")%>&JobId=<%#Eval("JobId")%>');">
                                    <%#Eval("DepotCode")%>
                                </a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Load No" SortExpression="Load_Number" ItemStyle-Width="70">
                            <ItemTemplate>
                                <%#GetLoadLinks(Eval("LoadsWithOrders").ToString()) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Customer" SortExpression="SortingClients">
                            <ItemTemplate>
                                <%# GetCustomerName(0, 0, (string)Eval("StartingClients"), (string)Eval("EndingClients")) %>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="" ItemStyle-Width="22">
                            <ItemTemplate>
                                <img src="/images/<%#GetInstructionTypeImage((int)Eval("StartInstructionTypeID"))%>" width="20" alt="" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Load Booked" SortExpression="LoadBookedDateFrom">
                            <ItemTemplate>
                                <%# GetBookedDateTime((DateTime)Eval("LoadBookedDateFrom"), (DateTime)Eval("LoadBookedDateTo"), (bool)Eval("LoadBookedIsTimed"), (bool)Eval("LoadBookedIsAnytime"),(int)Eval("StartInstructionTypeID"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Start" DataField="LegPlannedStartDateTime" DataFormatString="{0:dd/MM HH:mm}" HtmlEncode="false" ItemStyle-Width="80" SortExpression="LegPlannedStartDateTime" />
                        <asp:TemplateField HeaderText="From" SortExpression="StartPointDisplay">
                            <ItemTemplate>
                                <a class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" onclick="openAlterPointWindow(this);" pointid="<%# Eval("StartPointId") == DBNull.Value ? -1 : Eval("StartPointId") %>" instructionid="<%# Eval("StartInstructionId") == DBNull.Value ? -1 : Eval("StartInstructionId") %>">

                                    <%# Eval("StartPointDisplay") %>
                                    <b><font color="red">
                                    <%# Eval("StartShortCodes") %></font></b></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="" ItemStyle-Width="22">
                            <ItemTemplate>
                                <img src="/images/<%#GetInstructionTypeImage((int)Eval("EndInstructionTypeID"))%>" width="20" alt="" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Booked" SortExpression="BookedDateFrom">
                            <ItemTemplate>
                                <%# GetBookedDateTime((DateTime)Eval("BookedDateFrom"), (DateTime)Eval("BookedDateTo"), (bool)Eval("BookedIsTimed"), (bool)Eval("BookedIsAnytime"),(int)Eval("EndInstructionTypeID"))%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Planned" DataField="LegPlannedEndDateTime" DataFormatString="{0:HH:mm}" HtmlEncode="false" ItemStyle-Width="60" SortExpression="LegPlannedEndDateTime" />
                        <asp:TemplateField HeaderText="To" SortExpression="EndPointDisplay">
                            <ItemTemplate>
                                <a class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" onclick="openAlterPointWindow(this);" pointid="<%# Eval("EndPointId") == DBNull.Value ? -1 : Eval("EndPointId") %>" instructionid="<%# Eval("EndInstructionId") == DBNull.Value ? -1 : Eval("EndInstructionId") %>">
                                    <%# Eval("EndPointDisplay") %>
                                    <b><font color="red">
                                    <%# Eval("EndShortCodes") %></font></b></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Kg" DataField="Weight" ItemStyle-Width="25" SortExpression="Weight" DataFormatString="{0:#.##}" />
                        <asp:BoundField HeaderText="Pallets" DataField="NoPallets" ItemStyle-Width="25" SortExpression="NoPallets" DataFormatString="{0:#.##}" />
                        <asp:BoundField HeaderText="Catgeory" DataField="PlanningCategory" SortExpression="PlanningCategory" Visible="false" />
                        <asp:TemplateField HeaderText="Resource" SortExpression="ResourceSortColumn" HeaderStyle-Width="200" ItemStyle-Width="200">
                            <ItemTemplate>
                                <div style="display: block; width: 180px">
                                    <%# GetResourceLink((System.Data.DataRowView)Container.DataItem) %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <%# GetLinkImage(Eval("HasLinks").ToString()) %>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="PlanningCategoryID" Visible="false" />
                        <asp:TemplateField HeaderText="TT" ItemStyle-Width="40">
                            <ItemTemplate>
                                <%--<a class="ShowMWFTooltip" rel="/traffic/CommunicationHistory.aspx" startinstructionid="<%#((Eval("StartInstructionID").ToString()))%>" endinstructionid="<%#((Eval("EndInstructionID").ToString()))%>">
                                <div style='float:left;width:50%;'>
                                    <img src="<%#(String.IsNullOrEmpty(Eval("TTCommunicationStatusID").ToString()) && !((bool)Eval("AllowTTCommunication")) ? "" : "/images/icons/")%><%#GetTTCommunicationStatusImage(Eval("TTCommunicationStatusID"), (bool)Eval("AllowTTCommunication"))%>" 
                                        width="16" height="16" 
                                        style="display:<%#(String.IsNullOrEmpty(Eval("TTCommunicationStatusID").ToString()) && !((bool)Eval("AllowTTCommunication")) ? "none" : "inline")%>"
                                    /> 
                                </div>
                            </a>--%>
                                <div style='float: left; width: 50%;'>
                                    <a title="Click to view history" href="javascript:showMwfInstructionHistory(<%#((Eval("StartInstructionID").ToString()))%>,<%#((Eval("EndInstructionID").ToString()))%>)">
                                        <img src="<%#(String.IsNullOrEmpty(Eval("TTCommunicationStatusID").ToString()) && !((bool)Eval("AllowTTCommunication")) ? "" : "/images/icons/")%><%#GetTTCommunicationStatusImage(Eval("TTCommunicationStatusID"), (bool)Eval("AllowTTCommunication"), (Eval("StartInstructionID").ToString()),(Eval("EndInstructionID").ToString()))%>"
                                            width="16" height="16"
                                            style="display: <%#(String.IsNullOrEmpty(Eval("TTCommunicationStatusID").ToString()) && !((bool)Eval("AllowTTCommunication")) ? "none" : "inline")%>" />
                                    </a>
                                </div>

                                <div style='float: left; width: 50%;'>
                                    <img src="/images/msg_unread.gif" width="20" alt="" style="display: <%#(((bool)(Eval("HasTTMessage"))) ? "inline" : "none")%>" />
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <div style="font-size: 11px; height: 50px;">
                    <p style="font-weight: bold;">Bold means that this is Anytime</p>
                    <p style="font-style: italic">Italics means that the leg planned start date is tomorrow</p>
                </div>
            </div>
        </div>

        <asp:Panel ID="pnlReload" runat="server" Visible="false" EnableViewState="false">
            <script type="text/javascript" language="javascript">
                showResources();
            </script>
        </asp:Panel>

        <asp:Panel ID="pnlResetResourceOptions" runat="server" Visible="false">

            <script language="javascript" type="text/javascript">
        <!--
    top.document.getElementById('hidDriverResourceId').value = "";
    top.document.getElementById('hidDriverResourceName').value = "";
        
    top.document.getElementById('hidVehicleResourceId').value = "";
    top.document.getElementById('hidVehicleResourceName').value = "";
            
    top.document.getElementById('hidTrailerResourceId').value = "";
    top.document.getElementById('hidTrailerResourceName').value = "";
            
    top.document.getElementById('hidLinkJobSourceJobId').value = "";
    top.document.getElementById('hidLinkJobSourceInstructionId').value = "";
    //-->
            </script>

        </asp:Panel>

        <div class="masterpagelite_toolTip" id="toolTip" style="display: none;">
            <div id="toolTipInner">
                <h1 id="toolTipTitle"></h1>
            </div>
        </div>

        <uc:MwfDriverMessaging runat="server" ID="mwfDriverMessaging" />
        <uc:MwfInstructionHistory runat="server" ID="mwfInstructionHistory" />

        <script type="text/javascript">

            var _jobId;
            var _instructionId;
            var _driver;
            var _driverResourceId;
            var _regNo;
            var _vehicleResourceId;
            var _trailerRef;
            var _trailerResourceId;
            var _legPlannedStart;
            var _legPlannedEnd;
            var _depotCode;
            var _lastUpdateDate;
            var _instructionStateId;
            var _allowTTCommunication;
            var _linkJobSourceJobId;
            var _linkJobSourceLegId;
            var _startInstructionID = -1;
            var _startInstructionStateID = -1;
        
            var filterCollapseRuns = null;
        
            var userName = "<%=((Page.User) as Orchestrator.Entities.CustomPrincipal).UserName %>";
            var collectionFilter = false;
            
            $(document).ready(function() {
                $('table[id=gvTrafficSheet] > tbody > tr:not(:eq(0))').quicksearch({
                    position: 'before',
                    attached: 'table[id=gvTrafficSheet]',
                    labelText: "<a style=\"color:white;\" href='javascript:toggleAll(\"false\");'>Expand All</a> <a style=\"color:white;\" href='javascript:toggleAll(\"true\");'>Collapse All</a><span style=\"padding-top: 3px;\"> Filter the traffic sheet: </span>",
                    delay: 100,
                    loaderText: '',
                    onAfter: function(){
                        StoreSearchText();
                    }
                });
            
                $('.qs_input').keyup(function(event){
                    if (event.keyCode == '13')
                    {
                        event.preventDefault();
                        if ($('.qs_input').val().indexOf('col:') >= 0)
                        {
                            $('#txtGridSearch').val($('.qs_input').val());
                            $('.qs_input').val('');
                            top.document.getElementById('hidQuickSearchText').value = '';
                            collectionFilter = true;
                            $('#<%=imbRefresh.ClientID %>').click();
                    }
                }
            });
            
            filterCollapseRuns = "<%=CollapseRuns %>";

            $('.ShowPointTooltip').each(function(i, item){
                $(item).qtip({
                    style: {name: 'dark',
                        width:{min:176}
                    },
                    position: { adjust: { screen: true } },
                    content: {
                        url:$(item).attr('rel') ,
                        data: { pointId: $(item).attr('pointid')}, 
                        method: 'get'
                    }
                }
                            
                            
                );
            });

            
            $('.ShowMWFTooltip').each(function(i, item){
                $(item).qtip({
                    style: {name: 'dark',
                        width:{min:176}
                    },
                    position: { adjust: { screen: true } },
                    content: {
                        url:$(item).attr('rel') ,
                        data: {pointId: $(item).attr('pointid'),startinstructionId: $(item).attr('startinstructionid'),endinstructionId: $(item).attr('endinstructionid')}, 
                        method: 'get'
                    }
                }
                            
                            
                );
            });
        });      
        
        
        function greg(source, returnvalue) 
        {
            alert("return " + returnvalue);
        }

        function StoreSearchText() 
        {
            if (collectionFilter) {
                $('.qs_input').val(" ")
                collectionFilter = false;
                return;
            }
            var searchText = $('.qs_input').val();

            top.document.getElementById('hidQuickSearchText').value = searchText;

            if (searchText == '')
                toggleAll('false');

            // Update counts
            var legCount = $('#gvTrafficSheet > tbody > tr:visible').length - 1;

            var a = {};
            var runCount = 0;

            $('#gvTrafficSheet > tbody > tr:visible td:nth-child(2) a').each(function() {
                if (!a[$(this).text()]) {
                    runCount++;
                    a[$(this).text()] = true;
                }
            });

            $('#counts').html('Number of runs to plan : ' + runCount + ' | Number of legs to plan : ' + legCount);
        }

        function toggleAll(inputValue) 
        {
            var action = inputValue.toLowerCase();

            var imgs = $('.tsToggleImage');

            for (var i = 0; i < imgs.length; i++) {
                var src = imgs[i].src;

                if (action == 'true' && src.endsWith('col.gif'))
                    src = src.replace('col.gif', 'exp.gif');
                else if (action == 'false' && src.endsWith('exp.gif'))
                    src = src.replace('exp.gif', 'col.gif');

                imgs[i].src = src;

                var jobID = $(imgs[i]).attr('JobId');

                if (action == 'true') {
                    $("img[JobId=" + jobID + "]:not(.tsToggleImage)").hide();
                    $("span[JobId=" + jobID + "]").closest('tr').parent().closest('tr').hide();
                } else {
                    $("img[JobId=" + jobID + "]:not(.tsToggleImage)").show();
                    $("span[JobId=" + jobID + "]").closest('tr').parent().closest('tr').show();
                }
            }
        }

        function toggleJobs(img, jobId, instructionId)
        {
            var spans = $("span[JobId="+ jobId + "]").closest('tr').parent().closest('tr');
            var legPositionImages = $("img[InstructionId="+ instructionId +"]");
            var src = img.src;
            var updateImages = false;
            
            for(var i = 0; i < spans.length; i++)
            {
                var currentSpanRow = $('#' + spans[i].id);
            
                if((currentSpanRow.css('display') == "none" && src.endsWith('exp.gif')) || (currentSpanRow.css('display') != "none" && src.endsWith('col.gif')))
                    updateImages = true;
                
                if(updateImages)
                {
                    if(currentSpanRow.css('display') == "none")
                        currentSpanRow.show();
                    else
                        currentSpanRow.hide();
                }
            }
            
            if(updateImages)
            {
                if (src.endsWith('col.gif')) {
                    src = src.replace('col.gif', 'exp.gif');
                }
                else {
                    src = src.replace('exp.gif', 'col.gif');
                }
                img.src = src;
                
                if(legPositionImages.length > 0)
                {
                    var legPositionImage = $(legPositionImages[1]);
                    
                    if(legPositionImage.css('display') == "none")
                        legPositionImage.show();
                    else
                        legPositionImage.hide();
                }
            }
        }
            
        var quickSearchForm = $('FORM[id*=quicksearch]').children().wrapAll('<div id="divQuickSearchLabel" style="float: right"></div>');
        var spanJobsLegToPlan = $('span[id*=spnJobsLegsToPlan]');
        var divQuickSearchLabel = $('div[id=divQuickSearchLabel]');
        divQuickSearchLabel.before(spanJobsLegToPlan);
        
        // Filter data
        var activeControlAreaId = <%=ControlAreaId %>;
        var activeTrafficAreaIds = "<%=TrafficAreaIds %>";
            
            function GetImage(legPosition)
            {
                switch (legPosition)
                {
                    case "First":
                        return "<img src='../images/legTop.gif' height=20 width=5 />";
                    case "Middle":
                        return "<img src='../images/legMiddle.gif' height=20 width=5 />";
                    case "Last":
                        return "<img src='../images/legBottom.gif' height=20 width=5 />";
                }
                return "<img src='../images/spacer.gif' height=20 width=5 />";;
            }

            //openJobDetailsWindow
            function oJDW(jobId)
            {
                window.open('/Job/Job.aspx?wiz=true&jobId=' + jobId + getCSID(), "JobDetails", "Width=1120, Height=800, Scrollbars=1, Resizable=1, Left=60, Top=60");
            }

            function openTrafficSheetFilterWindow()
            {
                var qs = "caid=<%=ControlAreaId %>&taid=<%=TrafficAreaIds %>&start=<%=StartDate %>&end=<%=EndDate %>&showPlanned=<%=ShowPlannedLegs %>&showSM=<%=ShowStockMovementJobs %>&depotId=<%=DepotId %>&jt=<%=JobTypes %>";

                // add randmo string to stop caching issues
                qs += "&__rs=" + randomString();
                <%=dlgFilter.ClientID %>_Open(qs);
            }
        
            function openTrailerTypeWindow(jobId)
            {
                var qs = "jid=" + jobId;
                // add randmo string to stop caching issues
                qs += "&__rs=" + randomString();
                <%=dlgTrailerType.ClientID %>_Open(qs);
        }
        
        function randomString() {
            var chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
            var string_length = 8;
            var randomstring = '';
            for (var i=0; i<string_length; i++) {
                var rnum = Math.floor(Math.random() * chars.length);
                randomstring += chars.substring(rnum,rnum+1);
            }
            return randomstring;
        }

        function openTrafficAreaWindow(qs)
        {
            <%=dlgTrafficArea.ClientID %>_Open(qs);
        }

        
        function openAlterBookedTimesWindow(jobId, lastUpdateDate)
        {
            var qs = "jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            <%=dlgBookedTimes.ClientID %>_Open(qs)
            
        }
                
        function openAlterPlannedTimesWindow(jobId, lastUpdateDate)
        {
            var qs = "jobId=" + jobId + "&LastUpdateDate=" + lastUpdateDate;
            <%=dlgPlannedTimes.ClientID %>_Open(qs);
        }
                
        function openSubContractWindow(jobId, lastUpdateDate)
        {
            var qs = "jobId=" + jobId + "&lastUpdateDate=" + lastUpdateDate + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds;
            <%=dlgSubcontract.ClientID %>_Open(qs);
        }

        //openResourceWindow
        function oRW(InstructionId, Driver, DriverResourceId, RegNo, VehicleResourceId, TrailerRef, TrailerResourceId, legStart, legEnd, DepotCode, lastUpdateDate, jobId)
        {
            if (top.document.getElementById('hidDriverResourceId').value != '')
            {
                DriverResourceId = top.document.getElementById('hidDriverResourceId').value;
                Driver = top.document.getElementById('hidDriverResourceName').value;
            }
            if (top.document.getElementById('hidVehicleResourceId').value != '')
            {
                VehicleResourceId = top.document.getElementById('hidVehicleResourceId').value;
                RegNo = top.document.getElementById('hidVehicleResourceName').value;
            }
            if (top.document.getElementById('hidTrailerResourceId').value != '')
            {
                TrailerResourceId = top.document.getElementById('hidTrailerResourceId').value;
                TrailerRef = top.document.getElementById('hidTrailerResourceName').value;
            }

            var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&RegNo=" + RegNo + "&VR=" + VehicleResourceId + "&TrailerRef=" + TrailerRef + "&TR=" + TrailerResourceId + "&LS=" + legStart + "&LE=" + legEnd + "&DC=" + DepotCode + "&CA=" + activeControlAreaId + "&TA=" + activeTrafficAreaIds + "&LastUpdateDate=" + lastUpdateDate + "&jobId=" + jobId + "&depotId=<%=DepotId %>";
            <%=dlgResourceThis.ClientID %>_Open(qs)
        }

        function openCommunicateWindow(InstructionId, Driver, DriverResourceId, SubbyId, JobId, lastUpdateDate)
        {
            var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&SubbyId=" + SubbyId + "&jobId=" + JobId + "&LastUpdateDate=" + lastUpdateDate ;
            <%=dlgCommunicate.ClientID %>_Open(qs);
        }

        function openTrunkWindow(InstructionId, Driver, RegNo, LastUpdateDate)
        {
            var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&RegNo=" + RegNo + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgTrunk.ClientID %>_Open(qs);
        }

        function openRemoveTrunkWindow(JobId, InstructionID, LastUpdateDate)
        {
            var qs = "jobId=" + JobId + "&iID=" + InstructionID + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgRemoveTrunk.ClientID %>_Open(qs);
        }

        function openMultiTrunkWindow(JobId, InstructionID, LastUpdateDate)
        {
            var qs = "jobID=" + JobId + "&iID=" + InstructionID + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgMultiTrunk.ClientID %>_Open(qs);
        }

        function openLinkJobWindow(JobId, SourceJobJobId, SourceJobInstructionId, LastUpdateDate)
        {
            var url = "linkJob.aspx?jobId=" + JobId + "&SourceJobJobId=" + SourceJobJobId + "&SourceJobInstructionId=" + SourceJobInstructionId + "&LastUpdateDate=" + LastUpdateDate;
            _showModalDialog(url, 550, 358, "Link Jobs");
        }

        function openUpdateLocation(instructionId)
        {
            var url = "updateResourceLocations.aspx?instructionid=" + instructionId;
            _showModalDialog(url, 400, 320, 'Update Resource Locations');
        }

        function openPalletHandlingWindow(jobId)
        {
            var url = "JobManagement/addupdatepallethandling.aspx?jobId=" + jobId;
            
            window.open(url,'pallet', 'height=550,width=725');
        }

        function openAlterPointWindow(sender)
        {
            var pointId = $(sender).attr('pointid');
            var url = "/Point/Geofences/GeofenceManagement.aspx?pointId=" + pointId;
            window.open(url, "Point",  "Width=1034, Height=600, Scrollbars=0, Resizable=1")
        }

        function OpenJobDetails(jobId)
        {
            openDialogWithScrollbars('../job/job.aspx?wiz=true&jobId=' + jobId + getCSID(),'1220','870');
        }
        
        function openAddDestination(jobId, LastUpdateDate)
        {
            var qs = 'wiz=true&jobId=' + jobId + '&LastUpdateDate=' + LastUpdateDate;
            <%=dlgAddDestination.ClientID %>_Open(qs);
            
        }

        function openAddMultipleDestinations(jobId, LastUpdateDate)
        {
            var qs = "wiz=true&jobId=" + jobId + "&LastUpdateDate=" + LastUpdateDate;
            <%=dlgAddMultipleDestination.ClientID %>_Open(qs);
        }

        function openRemoveDestination(jobId, LastUpdateDate)
        {
            var qs = 'wiz=true&jobId=' + jobId + '&LastUpdateDate=' + LastUpdateDate;
            <%=dlgRemoveDestination.ClientID%>_Open(qs);
            
        }

        function openChangeDepot(jobId, instructionID)
        {
            var qs = 'wiz=true&jId=' + jobId + '&iid=' + instructionID;
            <%=dlgChangePlanningDepot.ClientID%>_Open(qs);
        }

        function GetLegAction(instructionStateId, jobId, lastUpdateDate)
        {
            if (instructionStateId == 1)
                return "<a href='javascript:openSubContractWindow(" + jobId + ", " + lastUpdateDate +"'>Sub-Contract</a>";
        }

        function GetShowPoint(dataItem)
        {
            if(dataItem.GetMember("PointId").Value != "")
                return "<span onmouseover=\"ShowPoint('../point/getPointAddresshtml.aspx', " + dataItem.GetMember("PointId").Value + ");\" onmouseout=\"HidePoint();\"> " + dataItem.GetMember("Description").Value  + "</span>";
            else
                return "";
        }
                    
        function showResources()
        {
            top.tsResource.location.href="TSResource.aspx?csid=<%=this.CookieSessionID %>";
            top.startCollapsed();
        }
    
        function openGiveResourcesWindow(instructionId)
        {
            var url = "giveResources.aspx?instructionId=" + instructionId + "&dr=null&vr=null&tr=null&ca=null&ta=null";
            window.open(url,'giveresources', 'height=400, width=320');
        }
        
        function UnCommunicate(instructionId, jobId, lastUpdateDate)
        {
            
            PageMethods.UnCommunicate(jobId, instructionId, UnCommunicate_Success, UnCommunicate_Failure);
        }
        function CopyRun (jobID)
        {   
            PageMethods.LoadRunIntoCache(jobID, LoadRunIntoCache_Success);
                        
        }
        function UnCommunicate_Success(result)
        {
            $get("<%=imbRefresh.ClientID %>").click();
        }
        

        function UnCommunicate_Failure(error)
        {
            alert(error.get_message());
        }
        
        function CommunicateThis(instructionID, driver, driverResourceId, vehicleResourceId, subContractorId, jobID)
        {
            PageMethods.Communicate(instructionID, driver, driverResourceId, vehicleResourceId, subContractorId, jobID, userName, Communicate_Success, Communicate_Failure);
        }
        
        function Communicate_Success(result)
        {
            $get("<%=imbRefresh.ClientID %>").click();
        }
        function LoadRunIntoCache_Success(result)
        {
            PageMethods.CreateACopyOfRun(result, '<%=Page.User.Identity.Name%>',CreateACopyOfRun_Success, CreateACopyOfRun_Failure );
        }
        function Communicate_Failure(error)
        {
            alert(error.get_message());
        }
        function CreateACopyOfRun_Success(result)
        {
            window.location.href = window.location.href;
        }
        
        function CreateACopyOfRun_Failure(error)
        {
            alert(error.get_message());
        }

        //new for TT
        function GetTTCommunicationStatusTextWS(result)
        {
            PageMethods.GetTTCommunicationStatusTextWS(1,true,'test' );
        }
        
        //ShowContextMenuLite
        function sCML(e, jobId, instructionId, driver, driverResourceId, subContractorId, regNo, vehicleResourceId, trailerRef, trailerResourceId, legPlannedStart, legPlannedEnd, depotCode, lastUpdateDate, instructionStateId, allowTTCommunication, rowId) {
           
            _jobId = jobId;
            _instructionId = instructionId;
            _driver = driver;
            _driverResourceId = driverResourceId;
            _subContractorId = subContractorId;
            _regNo = regNo;
            _vehicleResourceId = vehicleResourceId;
            _trailerRef = trailerRef;
            _trailerResourceId = trailerResourceId;
            _legPlannedStart = legPlannedStart;
            _legPlannedEnd = legPlannedEnd;
            _depotCode = depotCode;
            _lastUpdateDate = lastUpdateDate;
            _instructionStateId = instructionStateId;
            _allowTTCommunication = allowTTCommunication;

            var yMousePos,xMousePos;

            if (window.event) {
                yMousePos = window.event.y;
                xMousePos = window.event.x;
            }
            else {
                yMousePos = e.clientY;
                xMousePos = e.clientX;
            }

            var contextMenu = $find("<%= RadMenu1.ClientID %>");

            if ((!e.relatedTarget) || (!$telerik.isDescendantOrSelf(contextMenu.get_element(), e.relatedTarget))) {
                contextMenu.show(e);
            }

            $telerik.cancelRawEvent(e);

            return false;
        }

        function radMenu1_itemClicked(sender, eventArgs) {
            var mnuCall = eventArgs.get_item().get_value();
            eventArgs.get_item().get_menu().hide();

            switch (mnuCall.toLowerCase()) {
                case "subcontractleg":
                    openSubContractWindow(_jobId, _lastUpdateDate);
                    break;
                case "unsubcontractleg":
                    UnSubContractLegPostBack(_jobId, _instructionId, _instructionStateId);
                    break;
                case "changebookedtimes":
                    openAlterBookedTimesWindow(_jobId, _lastUpdateDate);
                    break;
                case "changeplannedtimes":
                    openAlterPlannedTimesWindow(_jobId, _lastUpdateDate);
                    break;
                case "trunk":
                    openTrunkWindow(_instructionId, _driver, _regNo, _lastUpdateDate);
                    break;
                case "multitrunk":
                    openMultiTrunkWindow(_jobId, _instructionId, _lastUpdateDate);
                    break;
                case "resourcethis":
                    oRW(_instructionId, _driver, _driverResourceId, _regNo, _vehicleResourceId, _trailerRef, _trailerResourceId, _legPlannedStart, _legPlannedEnd, _depotCode, _lastUpdateDate, _jobId);
                    break;
                case "Job Details":
                    OpenJobDetails(_jobId);
                    break;
                case "communicate": 
                    if (_instructionStateId == 2)
                        openCommunicateWindow(_instructionId, _driver, _driverResourceId, _subContractorId, _jobId, _lastUpdateDate);
                    else
                        alert("You can only communicate Planned legs");
                    break;
                case "quickcommunicate":
                    if (_instructionStateId == 2)
                        CommunicateThis(_instructionId, _driver, _driverResourceId, _vehicleResourceId, _subContractorId, _jobId);
                    else
                        alert("You can only communicate Planned legs");
                    break;
                case "sendmwfmessage":
                    sendMwfMessage(_jobId, _driverResourceId);
                    break;
                case "removetrunk":
                    openRemoveTrunkWindow(_jobId, _instructionId, _lastUpdateDate);
                    break;
                case "linkjob":
                    if(_linkJobSourceJobId != '' && _linkJobSourceLegId != '') {
                        openLinkJobWindow(_jobId, _linkJobSourceJobId, _linkJobSourceLegId, _lastUpdateDate);
                    }
                    else if (item.Text == "Remove Links") {
                        openLinkJobWindow(_jobId, "undefined", "undefined", _lastUpdateDate);
                    }
                    break;
                case "showloadorder":
                    openResizableDialogWithScrollbars('LoadOrder.aspx?jobid=' + _jobId, '700', '258');
                    break;
                case "giveresourceto":
                    if (_driverResourceId != 0 || _vehicleResourceId != 0 || _trailerResourceId != 0)
                        openGiveResourcesWindow(_instructionId);
                    else
                        alert("There is no resource to Give.");
                    break;
                case "adddestination":
                    openAddDestination(_jobId, _lastUpdateDate);
                    break;
                case "addmultipledestinations":
                    openAddMultipleDestinations(_jobId, _lastUpdateDate);
                    break;
                case "removedestination":
                    openRemoveDestination(_jobId, _lastUpdateDate);
                    break;
                case "callin":
                    openResizableDialogWithScrollbars("/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + _jobId + getCSID(), 900, 600);
                    break;
                case "changetrailertype":
                    openTrailerTypeWindow(_jobId);
                    break;
                case "uncommunicate":
                    if (_instructionStateId == 3)
                        UnCommunicate(_instructionId, _jobId, _lastUpdateDate);
                    else
                        alert("You can only UnCommunicate legs in Progress.");
                    break;
                case "copyrun":
                    CopyRun(_jobId);
                    break;
                case "changedepot":
                    openChangeDepot(_jobId, _instructionId);
                    break;
            }
        }

        function radMenu1_showing(sender, eventArgs) {
            var enableMwfMessaging = Boolean.parse(_allowTTCommunication);
            sender.findItemByValue("sendmwfmessage").set_enabled(enableMwfMessaging);
        }

        function ContextMenuClickHandler(item) {
            if (top.document.getElementById('hidDriverResourceId').value != '') {
                _driverResourceId = top.document.getElementById('hidDriverResourceId').value;
                _driver = top.document.getElementById('hidDriverResourceName').value;
            }
            if (top.document.getElementById('hidVehicleResourceId').value != '') {
                _vehicleResourceId = top.document.getElementById('hidVehicleResourceId').value;
                _regNo = top.document.getElementById('hidVehicleResourceName').value;
            }
            if (top.document.getElementById('hidTrailerResourceId').value != '') {
                _trailerResourceId = top.document.getElementById('hidTrailerResourceId').value;
                _trailerRef = top.document.getElementById('hidTrailerResourceName').value;
            }
            if (top.document.getElementById('hidLinkJobSourceJobId').value != '') {
                _linkJobSourceJobId = top.document.getElementById('hidLinkJobSourceJobId').value;
            }
            else
                _linkJobSourceJobId = '';
            if (top.document.getElementById('hidLinkJobSourceInstructionId').value != '') {
                _linkJobSourceLegId = top.document.getElementById('hidLinkJobSourceInstructionId').value;
            }
            else
                _linkJobSourceLegId = '';

            GridContextMenu.Hide();
        }

        function UnSubContractLegPostBack(jobId, instructionId, instructionStateId) {
            if (instructionStateId != 3) {
                alert("This leg has not been subcontracted.");
            }
            else {
                var params = jobId + "," + instructionId
                __doPostBack('uncontractleg', params);
            }
        }

        function onGridDoubleClick(gi) {
            var jobId = gi.GetMember("JobId").Value;
            oJDW(jobId);
        }

        function sendMwfMessage(jobID, driverID) {
            new MwfDriverMessaging([driverID], jobID).sendMessage()
                .done(function() {
                    alert('The message has been sent.');
                })
                .fail(function (error) {
                    alert(error);
                });
        }

        function showMwfInstructionHistory(startInstructionID, endInstructionID) {
            new MwfInstructionHistory(startInstructionID, endInstructionID).show()
                .done(function() {
                })
                .fail(function (error) {
                    alert(error);
                });
        }

        $(document).ready(function() {
            if (top.document.getElementById('hidQuickSearchText').value != "") {
                $('.qs_input').val(top.document.getElementById('hidQuickSearchText').value);
                $('.qs_input').keydown();
            }

            // Legs can be dragged from the traffic sheet to the Leg Planning screen.
            $('.draggableLeg').on('dragstart', function (e){
                $(this).closest('tr').css('opacity', '0.4');
                event.dataTransfer.effectAllowed  = 'move';

                var legData = { 
                    startInstructionID: $(this).attr('startInstructionID') ,
                    endInstructionID: $(this).attr('endInstructionID') ,
                    jobID: $(this).attr('jobID'),
                    trailerResourceID: $(this).attr('trailerResourceID'),
                    jobLastUpdateDateTime: $(this).attr('jobLastUpdateDateTime')
                };

                var sendObj = {data: legData};

                event.dataTransfer.setData('text', JSON.stringify(sendObj)); // IE currently only allows the first parameter to be 'text' or 'URL'
            });
           
            $('.draggableLeg').on('dragend', function(e){
                $(this).closest('tr').css('opacity', '1.0');
            });
        });

        //#endregion
        
        </script>

    </form>
</body>
</html>
