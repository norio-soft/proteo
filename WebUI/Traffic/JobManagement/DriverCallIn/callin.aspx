<%@ Page Language="c#" MasterPageFile="~/WizardMasterPage.Master" Inherits="Orchestrator.WebUI.Traffic.JobManagement.DriverCallIn.CallIn"
    CodeBehind="CallIn.aspx.cs" AutoEventWireup="True" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="nfvc" Namespace="P1TP.Components.Web.Validation" Assembly="P1TP.Components" %>
<%@ Register TagPrefix="uc1" TagName="callInTabStrip" Src="~/UserControls/DriverCallInTabStrip.ascx" %>
<%@ Register TagPrefix="cc1" Namespace="Orchestrator.WebUI" Assembly="Orchestrator.WebUI.Dialog" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Driver Call-in</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script src="/script/scripts.js" type="text/javascript" language="javascript"></script>
    <script src="/script/popaddress.js" type="text/javascript" language="javascript"></script>
    <script src="/script/helptip.js" type="text/javascript" language="javascript"></script>

    <style type="text/css">
        #backgroundPopup{  
            display:none;  
            position:fixed;  
            _position:absolute; /* hack for internet explorer 6*/  
            height:100%;  
            width:100%;  
            top:0;  
            left:0;  
            background:#000000;  
            border:1px solid #cecece;  
            z-index:1;  
        }
        
        #popupContact{  
            display:none;  
            position:fixed;  
            _position:absolute; /* hack for internet explorer 6*/  
            height:90px;  
            width:408px;  
            background:#FFFFFF;  
            border:2px solid #cecece;  
            z-index:2;  
            padding:12px;  
            font-size:13px;  
        } 
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script language="javascript" type="text/javascript">        window.focus();</script>
    <link rel="stylesheet" type="text/css" href="/style/helpTip.css" />
    <link rel="stylesheet" type="text/css" href="/style/newStyles.css" />
    <link rel="stylesheet" type="text/css" href="/style/newMasterpage.css" />
    <cc1:Dialog ID="dlgResource" runat="server" URL="/Traffic/ResourceThis.aspx" Width="1100"
        Height="850" AutoPostBack="true" ReturnValueExpected="true" Mode="Modal" UseCookieSessionID="true"/>
    <cc1:Dialog ID="dlgPalletHandling" runat="server" URL="/Traffic/JobManagement/AddUpdatePalletHandling.aspx"
        Width="1115" Height="750" AutoPostBack="true" ReturnValueExpected="true" Mode="Normal" />
    <cc1:Dialog ID="dlgDriverClockIn" runat="server" URL="/Resource/Driver/EnterDriverStartTimes.aspx"
        Width="500" Height="290" AutoPostBack="true" ReturnValueExpected="true" Mode="Modal" />
    <cc1:Dialog runat="server" ID="dlgOrder" ReturnValueExpected="true" AutoPostBack="true"
        URL="/groupage/manageorder.aspx" Height="900" Width="1200" Mode="Modal" />
    <asp:Label ID="lblNotYourInstruction" runat="server" Visible="false"></asp:Label>
    <table id="Table2" width="100%" cellpadding="0" cellspacing="0" border="0" runat="server">
        <tr>
            <td class="layoutContentMiddle" valign="top" align="left">
                <div class="layoutContentMiddleInner">
                    <div runat="server" id="buttonBar" class="buttonbar" style="text-align: left;">
                        <telerik:RadCodeBlock ID="codeBlock2" runat="server">
                            <table border="0" cellpadding="0" cellspacing="2" width="99%">
                                <tr>
                                    <td>
                                        <input type="button" style="width: 75px" value="Details" onclick="javascript:window.location='/job/job.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px" value="Coms" onclick="javascript:window.location='/traffic/JobManagement/driverCommunications.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px" value="Call-In" onclick="javascript:window.location='/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px" value="PODs" onclick="javascript:window.location='/traffic/JobManagement/bookingInPODs.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td>
                                        <input type="button" style="width: 75px; display: <%= m_job.JobType == Orchestrator.eJobType.Groupage ? "none" : "" %>"
                                            value="Pricing" onclick="javascript:window.location='/traffic/JobManagement/pricing2.aspx?wiz=true&jobId=<%=Request.QueryString["jobId"]%>'+ getCSID();" />
                                    </td>
                                    <td width="100%" align="right">
                                        <iframe id="ifCallInSelector" marginheight="0" marginwidth="0" frameborder="0" scrolling="no"
                                            width="360px" height="22px" src='/traffic/jobManagement/CallInSelector.aspx?JobId=<%=Request.QueryString["JobId"]%>&amp;csid=<%=this.CookieSessionID %>'>
                                        </iframe>
                                    </td>
                                </tr>
                            </table>
                        </telerik:RadCodeBlock>
                    </div>
                    <uc1:callInTabStrip ID="tabStrip1" runat="server" SelectedTab="0"></uc1:callInTabStrip>
                    <div style="padding-bottom: 10px;">
                    </div>
                    <telerik:RadAjaxPanel ID="raxPanel" runat="server" LoadingPanelID="LoadingPanel1">
                        <uc1:infringementDisplay ID="infringementDisplay" runat="server" />
                        <div align="left" id="topPortion">
                            <fieldset>
                                <h3 onclick="javascript:document.getElementById('divElapsedTime').style.display='';">
                                    <asp:Label ID="lblInstructionDescription" runat="server"></asp:Label>
                                </h3>
                                <table style="width: 780px; border: 0px;" rules="all">
                                    <tr id="trInformation" runat="server">
                                        <td colspan="3">
                                            <div class="MessagePanel">
                                                <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" />
                                                <asp:Label ID="lblMessage" runat="server"></asp:Label>
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">
                                            Run Id
                                        </td>
                                        <td class="formCellField">
                                            <telerik:RadCodeBlock ID="codeBlock1" runat="server">
                                                <%=m_jobId.ToString()%>
                                                <input type="hidden" id="hidInstructionId" runat="server" value='<%=m_instruction.ToString()%>'
                                                    name="hidInstructionId" />
                                            </telerik:RadCodeBlock>
                                            <input type="hidden" id="hidInstructionType" runat="server" name="hidInstructionType" />
                                            <input type="hidden" id="hidInstructionActualId" runat="server" value="0" name="hidInstructionActualId" />
                                        </td>
                                        <td class="formCellField" rowspan="7" valign="top">
                                            <asp:Panel ID="pnlCreateUser" runat="server" Visible="false">
                                                <table border="0" rules="all">
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px;">
                                                            Created By
                                                        </td>
                                                        <td class="formCellField" colspan="2">
                                                            <asp:Label ID="lblCreatedBy" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px;">
                                                            Created Date
                                                        </td>
                                                        <td class="formCellField" colspan="2">
                                                            <asp:Label ID="lblCreatedDate" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px;">
                                                            Updated By
                                                        </td>
                                                        <td class="formCellField" colspan="2">
                                                            <asp:Label ID="lblUpdatedBy" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px;">
                                                            Updated Date
                                                        </td>
                                                        <td class="formCellField" colspan="2">
                                                            <asp:Label ID="lblUpdatedDate" runat="server"></asp:Label>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </asp:Panel>
                                            <asp:Panel ID="pnlResourceDetails" runat="server">
                                                <table border="0" rules="all">
                                                    <tr>
                                                        <td class="formCellField" colspan="2">
                                                            <asp:HyperLink runat="server" ID="hlChangeResources" Text="Change Resources" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px;">
                                                            Driver
                                                        </td>
                                                        <td class="formCellField">
                                                            <span>
                                                                <asp:Label ID="lblDriver" runat="server" Text=""></asp:Label></span>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px">
                                                            Trailer
                                                        </td>
                                                        <td class="formCellField">
                                                            <telerik:RadCodeBlock ID="codeBlock3" runat="server">
                                                                <asp:TextBox ID="txtTrailer" runat="server" Text="None" ReadOnly="true" Style="border: none;
                                                                    width: 250px;"></asp:TextBox>
                                                                <asp:CompareValidator ID="cvTrailer" runat="server" ControlToValidate="txtTrailer"
                                                                    ErrorMessage="The trailer is Unknown. Please click Change Resource to set it."
                                                                    Display="Dynamic" EnableClientScript="False" ValueToCompare="Unknown" Operator="NotEqual">
                                                                <img alt="The trailer is Unknown. Please click Change Resource to set it." height="16" src='/images/Error.gif' width="16" />
                                                                </asp:CompareValidator>
                                                            </telerik:RadCodeBlock>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel" style="width: 100px">
                                                            <asp:Label ID="lblVehicleText" runat="server" Text="Vehicle"></asp:Label>
                                                        </td>
                                                        <td class="formCellField">
                                                            <asp:Label ID="lblVehicle" runat="server" Text="Own Vehicle"></asp:Label>
                                                        </td>
                                                    </tr>
                                                    <tr id="trTakeTrailerOn" runat="server">
                                                        <td class="formCellLabel" colspan="2">
                                                            <asp:CheckBox ID="chkMoveTrailer" runat="server" TextAlign="right" Text="Driver will take trailer on to " />
                                                            <input type="hidden" id="hidTrailerResourceId" runat="server" />
                                                            <input type="hidden" id="hidTakeTrailerToPointId" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td class="formCellLabel">
                                                            Driver Start Time
                                                        </td>
                                                        <td class="formCellField">
                                                            <img runat="server" src="/images/clockin.gif" id="imgClockIn" style="cursor: pointer" />
                                                            <asp:Label ID="lblStartTime" runat="server"></asp:Label>
                                                            <asp:HiddenField ID="hidDriverResourceID" runat="server" />
                                                        </td>
                                                    </tr>
                                                    <asp:Panel ID="pnlEmptyPalletCount" runat="server" Visible="false">
                                                        <tr>
                                                            <td colspan="2" style="border-style: none;">
                                                                <h3>
                                                                    Empty Pallet Count</h3>
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td colspan="2" style="border-style: none;">
                                                                <asp:HiddenField ID="hidResourceID" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formCellLabel">
                                                                Job Pallets
                                                            </td>
                                                            <td class="formCellField">
                                                                <asp:Label ID="lblTotalEmptyPalletCount" runat="server" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formCellLabel">
                                                                Resource Pallets
                                                            </td>
                                                            <td class="formCellField">
                                                                <asp:Label ID="lblResourceEmptyPalletCount" runat="server" /><asp:Label Font-Size="Smaller"
                                                                    Font-Bold="true" ID="lblResourceDescription" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </asp:Panel>
                                                </table>
                                            </asp:Panel>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel" nowrap="true" style="width: 150px">
                                            Planned to happen at
                                        </td>
                                        <td class="formCellField">
                                            <asp:Label ID="lblPlannedTime" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">
                                            Booked Time
                                        </td>
                                        <td class="formCellField">
                                            <asp:Label ID="lblBookedDate" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trGPSArrival" runat="server">
                                        <td class="formCellLabel">
                                            GPS Arrival Time
                                        </td>
                                        <td class="formCellField">
                                            <asp:Label ID="lblGPSArrivalTime" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td class="formCellLabel">
                                            Arrival Time
                                        </td>
                                        <td class="formCellField">
                                            <table border="0" cellpadding="0" cellspacing="2" style="width: 150px;">
                                                <tr>
                                                    <td>
                                                        <telerik:RadDateInput ID="dteArrivalDate" runat="server" Width="60px" DisplayDateFormat="dd/MM/yy"
                                                            DateFormat="dd/MM/yy" ToolTip="The actual arrival date" OnClientDateChanged="validateDepartureDate" />
                                                    </td>
                                                    <td>
                                                        <telerik:RadDateInput ID="dteArrivalTime" runat="server" Width="45px" DisplayDateFormat="HH:mm"
                                                            DateFormat="HH:mm" ToolTip="The actual arrival Time" OnClientDateChanged="validateDepartureDate" />
                                                    </td>
                                                    <td>
                                                        <asp:RequiredFieldValidator ID="rfvArrivalDate" runat="server" ErrorMessage="Please enter a valid arrival date"
                                                            ControlToValidate="dteArrivalDate" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid arrival date" /></asp:RequiredFieldValidator>
                                                        <asp:RequiredFieldValidator ID="rfvArrivalTime" runat="server" ErrorMessage="Please enter a valid arrival date"
                                                            ControlToValidate="dteArrivalTime" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid arrival time" /></asp:RequiredFieldValidator>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                    <tr id="trGPSDeptTime" runat="server">
                                        <td class="formCellLabel">
                                            GPS Departure Time
                                        </td>
                                        <td class="formCellField">
                                            <asp:Label ID="lblGPSDepartureTime" runat="server"></asp:Label>
                                        </td>
                                    </tr>
                                    <tr id="trDeptTime" runat="server">
                                        <td class="formCellLabel">
                                            Departure Time
                                        </td>
                                        <td class="formCellField">
                                            <table border="0" cellpadding="0" cellspacing="2" style="width: 150px;">
                                                <tr>
                                                    <td>
                                                        <telerik:RadDateInput ID="dteDepartureDate" runat="server" Width="60px" DisplayDateFormat="dd/MM/yy"
                                                            DateFormat="dd/MM/yy" ToolTip="The actual departure date" OnClientDateChanged="validateDepartureDate" />
                                                    </td>
                                                    <td>
                                                        <telerik:RadDateInput ID="dteDepartureTime" runat="server" Width="45px" DisplayDateFormat="HH:mm"
                                                            DateFormat="HH:mm" ToolTip="The actual departure Time" OnClientDateChanged="validateDepartureDate" />
                                                    </td>
                                                    <td valign="top">
                                                        <asp:RequiredFieldValidator ID="rfvDepartureDate" runat="server" ErrorMessage="Please enter a valid departure date"
                                                            ControlToValidate="dteDepartureDate" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid departure date" /></asp:RequiredFieldValidator>
                                                        <asp:RequiredFieldValidator ID="rfvDepartureTime" runat="server" ErrorMessage="Please enter a valid departure time"
                                                            ControlToValidate="dteDepartureTime" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" alt="Please enter a valid departure time" /></asp:RequiredFieldValidator>
                                                        <div id="divDepartureDateValidation" style="display: none;">
                                                            <img src="/images/Error.gif" height="16" width="16" alt="The departure date must be after the arrival date" />
                                                        </div>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <asp:Panel ID="pnlAttachedPCVDetails" runat="server" Visible="false">
                                <fieldset>
                                    <asp:HiddenField ID="hidCheckForChanges" runat="server" />
                                    <table>
                                        <tr>
                                            <td valign="top">
                                                <h3>
                                                    Attached PCVs</h3>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td>
                                                            Redeemed
                                                        </td>
                                                        <td style="width: 16px; background-color: #A7F1A7;">
                                                        </td>
                                                        <td style="padding-left: 15px;">
                                                            Refused : PCV Signed
                                                        </td>
                                                        <td style="width: 16px; background-color: #AEBFDB;">
                                                        </td>
                                                        <td style="padding-left: 15px;">
                                                            Refused : PCV Unsigned
                                                        </td>
                                                        <td style="width: 16px; background-color: #FFB6B3;">
                                                        </td>
                                                    </tr>
                                                </table>
                                                <div id="pcvWrapper">
                                                    <asp:ListView ID="lvAttachedPCVs" runat="server">
                                                        <LayoutTemplate>
                                                            <table cellspacing="0" cellpadding="0">
                                                                <thead>
                                                                    <tr class="HeadingRow">
                                                                        <th>
                                                                            <asp:CheckBox ID="chkHeaderSelect" runat="server" onclick="javascript:chkSelectAllAttachedPDFs(this);" />
                                                                        </th>
                                                                        <th>
                                                                            PCV ID
                                                                        </th>
                                                                        <th>
                                                                            Voucher No
                                                                        </th>
                                                                        <th>
                                                                            Client
                                                                        </th>
                                                                        <th>
                                                                            PalletType
                                                                        </th>
                                                                        <th>
                                                                            No Of Signings
                                                                        </th>
                                                                        <th>
                                                                            Pallets Oustanding
                                                                        </th>
                                                                        <th>
                                                                            &nbsp;
                                                                        </th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
                                                                </tbody>
                                                            </table>
                                                        </LayoutTemplate>
                                                        <ItemTemplate>
                                                            <tr class="Row" id="rpcv" runat="server">
                                                                <td>
                                                                    <asp:CheckBox ID="chkSelectedPCV" runat="server" onclick="javascript:chkSelectedPCV_Checked(this);" /><asp:HiddenField
                                                                        ID="hidPCVID" runat="server" />
                                                                </td>
                                                                <td>
                                                                    <%# ((Orchestrator.Entities.PCV)Container.DataItem).PCVId.ToString() %>
                                                                </td>
                                                                <td>
                                                                    <%# ((Orchestrator.Entities.PCV)Container.DataItem).VoucherNo.ToString()%>
                                                                </td>
                                                                <td>
                                                                    <%# ((Orchestrator.Entities.PCV)Container.DataItem).Client %>
                                                                </td>
                                                                <td>
                                                                    <%# ((Orchestrator.Entities.PCV)Container.DataItem).PalletType%>
                                                                </td>
                                                                <td>
                                                                    <asp:Label ID="lblNoOfSignings" runat="server" />
                                                                </td>
                                                                <td>
                                                                    <%# ((Orchestrator.Entities.PCV)Container.DataItem).NoOfPalletsOutstanding.ToString()%>
                                                                </td>
                                                                <td>
                                                                    <asp:LinkButton ID="lbResetPCV" runat="server" Text="Reset" CommandArgument='<%# ((Orchestrator.Entities.PCV)Container.DataItem).PCVId.ToString() %>'
                                                                        OnClick="btnResetPCV_Click" Visible="false" />
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:ListView>
                                                </div>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td valign="top">
                                                <fieldset>
                                                    <table style="width: 400px;">
                                                        <tr>
                                                            <td class="formCellLabel">
                                                                PCV Action
                                                            </td>
                                                            <td class="formCellField" style="width: 275px;">
                                                                <telerik:RadComboBox ID="rcbPCVAction" runat="server" DataTextField="Description"
                                                                    DataValueField="RedemptionDetailStatusID" AutoPostBack="false" Style="width: 275px;" />
                                                            </td>
                                                        </tr>
                                                        <tr>
                                                            <td class="formCellLabel">
                                                                Client Contact
                                                            </td>
                                                            <td class="formCellField">
                                                                <asp:TextBox ID="txtClientContact" runat="server" MaxLength="100" Width="100%" />
                                                                <asp:RequiredFieldValidator ID="rfvClientContact" runat="server" ControlToValidate="txtClientContact"
                                                                    ValidationGroup="grpAttachedPCVs"><img src="/images/Error.gif" alt="Please specify a contact name." /></asp:RequiredFieldValidator>
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <div class="buttonbar" id="btnUpdateAttachedPCVs_Wrapper" style="display: none;">
                                                        <asp:Button ID="btnUpdateAttachedPCVs" runat="server" Text="Update PCVs" ValidationGroup="grpAttachedPCVs" />
                                                    </div>
                                                </fieldset>
                                            </td>
                                        </tr>
                                    </table>
                                </fieldset>
                            </asp:Panel>
                            <fieldset>
                                <table>
                                    <asp:Panel ID="pnlDeliveryCallIn" runat="server">
                                        <asp:Repeater ID="repClient" runat="server">
                                            <HeaderTemplate>
                                                <tr>
                                                    <td>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <div>
                                                    <h3>
                                                        <asp:Label ID="lblClient" runat="server"></asp:Label></h3>
                                                    <asp:Repeater ID="repPalletType" runat="server">
                                                        <ItemTemplate>
                                                            <div id="divClientPalletWrapper">
                                                                <asp:HiddenField ID="hidTrackingPalletType" runat="server" />
                                                                <asp:HiddenField ID="hidCaptureDebriefForClient" runat="server" />
                                                                <asp:HiddenField ID="hidCollectDropCount" runat="server" />
                                                                <asp:Label ID="lblPalletType" runat="server" />
                                                                <asp:Repeater ID="repCollectDrop" runat="server">
                                                                    <ItemTemplate>
                                                                        <input type="hidden" id="hidCollectDropId" runat="server" value='<%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).CollectDropId %>'
                                                                            name="hidCollectDropId" />
                                                                        <input type="hidden" id="hidCollectDropActualId" runat="server" value="0" name="hidCollectDropActualId" />
                                                                        <asp:HiddenField ID="hidPalletsPrevious" runat="server" Value="0" />
                                                                        <table>
                                                                            <tr>
                                                                                <td class="formCellLabel" style="width: 150px;">
                                                                                    <asp:Label ID="lblDocketNumberText" runat="server"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:Label ID="lblDocketDisplay" runat="server" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formCellLabel">
                                                                                    <asp:Label ID="lblCustomerReferenceDisplay" runat="server"></asp:Label>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:Label ID="lblCustomerReference" runat="server"></asp:Label>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <th class="formCellTableHeader">
                                                                                    &#160;
                                                                                </th>
                                                                                <th class="formCellTableHeader">
                                                                                    Despatched
                                                                                </th>
                                                                                <th class="formCellTableHeader">
                                                                                    Delivered
                                                                                </th>
                                                                                <td colspan="2" align="right" style="padding-right: 5px;">
                                                                                    <telerik:RadComboBox ID="rcbPalletType" runat="server" Width="140px" OnClientSelectedIndexChanged="rcbPalletType" />
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formCellLabel">
                                                                                    Cases
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtQuantityDespatched" runat="server" Width="50px"></asp:TextBox><asp:RequiredFieldValidator
                                                                                        ID="rfvQuantityDespatched" runat="server" ControlToValidate="txtQuantityDespatched"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of cases despatched."><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of cases despatched." /></asp:RequiredFieldValidator>
                                                                                    <asp:CustomValidator ID="cfvQuantityDespatched" runat="server" ControlToValidate="txtQuantityDespatched" ClientValidationFunction="validateCases"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of cases despatched.  This must be a whole number and at least 0 and 99,999."><img src="/images/Error.gif" height="16" width="16" title="This must be a whole number and between 0 and 99,999." /></asp:CustomValidator>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtQuantityDelivered" runat="server" Width="50px"></asp:TextBox><asp:RequiredFieldValidator
                                                                                        ID="rfvQuantityDelivered" runat="server" ControlToValidate="txtQuantityDelivered"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of cases delivered."><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of cases delivered." /></asp:RequiredFieldValidator>
                                                                                    <asp:CustomValidator ID="cfvQuantityDelivered" runat="server" ControlToValidate="txtQuantityDelivered"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of cases delivered.  This must be a whole number and at least 0."><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of cases delivered.  This must be a whole number and at least 0." /></asp:CustomValidator>
                                                                                </td>
                                                                                <td class="formCellLabel">
                                                                                    Shortage Reference
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtQuantityShortageReference" runat="server"></asp:TextBox>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formCellLabel">
                                                                                    Pallets
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtPalletsDespatched" runat="server" Width="50px"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="rfvPalletsDespatched" runat="server" ControlToValidate="txtPalletsDespatched"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of pallets despatched."><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of pallets despatched." /></asp:RequiredFieldValidator>
                                                                                    <asp:CustomValidator ID="cfvPalletsDespatched" runat="server" ControlToValidate="txtPalletsDespatched" ClientValidationFunction="validatePallets"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of pallets despatched.  This must be a whole number and at least 0."><img src="/images/Error.gif" height="16" width="16" title="This must be a whole number and between 0 and 999." /></asp:CustomValidator>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtPalletsDelivered" runat="server" Width="50px"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="rfvPalletsDelivered" runat="server" ControlToValidate="txtPalletsDelivered"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of pallets delivered."><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of pallets delivered." /></asp:RequiredFieldValidator>
                                                                                    <asp:CustomValidator ID="cfvPalletsDelivered" runat="server" ControlToValidate="txtPalletsDelivered"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of pallets delivered.  This must be a whole number and at least 0."><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of pallets delivered.  This must be a whole number and at least 0." /></asp:CustomValidator>
                                                                                </td>
                                                                                <td colspan="2">
                                                                                    <table id="tblPalletReturned" runat="server" cellpadding="0" cellspacing="0">
                                                                                        <tr>
                                                                                            <td class="formCellLabel" style="width: 115px;">
                                                                                                <asp:Label ID="lblPalletsReturned" runat="server">Pallets Returned</asp:Label>
                                                                                            </td>
                                                                                            <td style="padding-left: 2px;">
                                                                                                <asp:TextBox ID="txtPalletsReturned" runat="server" Width="50px" Text="0"></asp:TextBox>
                                                                                                <asp:RequiredFieldValidator ID="rfvPalletsReturned" runat="server" ControlToValidate="txtPalletsReturned"
                                                                                                    Display="Dynamic" ErrorMessage="Please provide the number of pallets returned.">
                                                                                                    <img src="/images/Error.gif" height="16" width="16" title="Please provide the number of pallets returned." />
                                                                                                </asp:RequiredFieldValidator>
                                                                                            </td>
                                                                                        </tr>
                                                                                    </table>
                                                                                </td>
                                                                            </tr>
                                                                            <tr>
                                                                                <td class="formCellLabel">
                                                                                    Weight
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtWeightDespatched" runat="server" Width="50px"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="rfvWeightDespatched" runat="server" ControlToValidate="txtWeightDespatched"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the amount of weight despatched.">
                                                                                        <img src="/images/Error.gif" height="16" width="16" alt="" title="Please provide the amount of weight despatched." />
                                                                                    </asp:RequiredFieldValidator>
                                                                                    <asp:CustomValidator ID="cfvWeightDespatched" runat="server" ControlToValidate="txtWeightDespatched"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the amount of weight despatched.  This must be a number and at least 0."
                                                                                        ClientValidationFunction="validateWholeNumber">
                                                                                        <img src="/images/Error.gif" height="16" width="16" alt="" title="Please provide the amount of weight despatched.  This must be a number and at least 0." />
                                                                                    </asp:CustomValidator>
                                                                                </td>
                                                                                <td>
                                                                                    <asp:TextBox ID="txtWeightDelivered" runat="server" Width="50px"></asp:TextBox>
                                                                                    <asp:RequiredFieldValidator ID="rfvWeightDelivered" runat="server" ControlToValidate="txtWeightDelivered"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of cases despatched.">
                                                                                        <img src="/images/Error.gif" height="16" width="16" title="Please provide the number of cases despatched." />
                                                                                    </asp:RequiredFieldValidator>
                                                                                    <asp:CustomValidator ID="cfvWeightDelivered" runat="server" ControlToValidate="txtWeightDelivered"
                                                                                        Display="Dynamic" ErrorMessage="Please provide the number of cases despatched.  This must be a number and at least 0."
                                                                                        ClientValidationFunction="validateWholeNumber">
                                                                                        <img src="/images/Error.gif" height="16" width="16" title="Please provide the number of cases despatched.  This must be a number and at least 0." />
                                                                                    </asp:CustomValidator>
                                                                                </td>
                                                                            </tr>
                                                                        </table>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                                <asp:Panel ID="pnlPalletTypePCV" runat="server" Style="display: none;" CssClass="wrapperPalletTypePCV">
                                                                    <hr />
                                                                    <asp:Label ID="lblPCVRequired" runat="server" Text="A PCV should have been issued, ask the driver for details."
                                                                        ForeColor="Red" Font-Bold="true"></asp:Label>
                                                                    <asp:HiddenField ID="hidPalletType" runat="server" />
                                                                    <asp:HiddenField ID="hidInstructionID" runat="server" />
                                                                    <asp:HiddenField ID="hidClientID" runat="server" />
                                                                    <table>
                                                                        <tr>
                                                                            <th class="formCellTableHeader">
                                                                                Pallet Type
                                                                            </th>
                                                                            <th class="formCellTableHeader">
                                                                                Number of Pallets
                                                                            </th>
                                                                            <th class="formCellTableHeader">
                                                                                PCV Voucher Code
                                                                            </th>
                                                                            <th class="formCellTableHeader">
                                                                                Reason for Issue
                                                                            </th>
                                                                            <th>
                                                                            </th>
                                                                        </tr>
                                                                        <tr>
                                                                            <td>
                                                                                <asp:Label ID="lblPalletTypeFooter" runat="server"></asp:Label>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtPCVPallets" runat="server" Width="50px" Text="0"></asp:TextBox>
                                                                                <asp:RequiredFieldValidator ID="rfvPCVPallets" runat="server" ControlToValidate="txtPCVPallets"
                                                                                    Display="Dynamic" ValidationGroup="vgReturnPallets" ErrorMessage="Please provide the number of pallets for the PCV">
                                                                                    <img src="/images/Error.png" height="16" id="imgPCVPallets" width="16" alt="" title="Please provide the number of pallets for the PCV" />
                                                                                </asp:RequiredFieldValidator>
                                                                                <asp:CustomValidator ID="cfvPCVPallets" runat="server" ControlToValidate="txtPCVPallets"
                                                                                    Display="Dynamic" ValidationGroup="vgReturnPallets" ErrorMessage="Please provide the number of pallets on the PCV. This, combined with the pallets returned must equal the pallets delivered."
                                                                                    ClientValidationFunction="validatePalletsReturned"><img src="/images/Error.gif" height="16" width="16" title="Please provide the number of pallets on the PCV. This, combined with the pallets returned must equal the pallets delivered." /></asp:CustomValidator>
                                                                            </td>
                                                                            <td>
                                                                                <asp:TextBox ID="txtPCVVoucherCode" runat="server"></asp:TextBox>
                                                                                <asp:CustomValidator ID="cfvPCVVoucherCode" runat="server" ControlToValidate="txtPCVVoucherCode"
                                                                                    Display="Dynamic" ValidationGroup="vgReturnPallets" ErrorMessage="Please provide the PCV Voucher Code.  This must be a whole number."
                                                                                    ClientValidationFunction="validatePCVVoucherNumber">
                                                                                    <img src="/images/Error.png" height="16" id="imgPCVVoucherCodeError" width="16" alt="" title="Please provide the PCV Voucher Code.  This must be a whole number." />
                                                                                </asp:CustomValidator>
                                                                            </td>
                                                                            <td>
                                                                                <telerik:RadComboBox ID="rcbReasonForIssue" runat="server" />
                                                                            </td>
                                                                            <td>
                                                                                <div style="margin-left: 20px;">
                                                                                    <asp:CheckBox ID="chkNoPCVIssued" runat="server" Text="No PCV Issued" AutoPostBack="false" />
                                                                                </div>
                                                                            </td>
                                                                        </tr>
                                                                    </table>
                                                                    <hr />
                                                                </asp:Panel>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </td> </tr>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlLeavePalletsCallIn" runat="server">
                                        <tr>
                                            <td>
                                                <asp:ListView ID="lvLeavePallets" runat="server">
                                                    <LayoutTemplate>
                                                        <table>
                                                            <thead>
                                                                <tr>
                                                                    <th style="width: 170px;">
                                                                        Pallet Type
                                                                    </th>
                                                                    <th style="width: 150px;">
                                                                        Planned Pallets Left
                                                                    </th>
                                                                    <th style="width: 150px;">
                                                                        Actual Pallets Left
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr id="itemPlaceHolder" runat="server" />
                                                            </tbody>
                                                        </table>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="formCellLabel">
                                                                <%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.PalletType.ToString()%>
                                                            </td>
                                                            <td>
                                                                <%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.NoPallets.ToString()%>
                                                            </td>
                                                            <td>
                                                                <telerik:RadNumericTextBox ID="rntActualPallets" runat="server" MinValue="0" MaxValue='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.NoPallets %>'
                                                                    NumberFormat-DecimalDigits="0" Type="Number" />
                                                                <asp:RequiredFieldValidator ID="rfvActualPallets" runat="server" ControlToValidate="rntActualPallets"
                                                                    Display="Dynamic">
                                                                    <img src="/images/error.png" alt="Please specify how many pallets were picked up." />
                                                                </asp:RequiredFieldValidator>
                                                                <asp:HiddenField ID="hidCollectDropID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.CollectDropId.ToString() %>' />
                                                                <asp:HiddenField ID="hidCollectDropActual" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDropActual != null ? ((CollectDropWithCollectDropActual)Container.DataItem).CollectDropActual.CollectDropActualId : -1 %>' />
                                                                <asp:HiddenField ID="hidPalletTypeID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.PalletTypeID.ToString() %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <EmptyDataTemplate>
                                                        There are no pallets to leave at this location.
                                                    </EmptyDataTemplate>
                                                </asp:ListView>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlDehirePalletsCallIn" runat="server">
                                        <tr>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td class="formCellLabel">
                                                            <b>De-hire Date</b>
                                                        </td>
                                                        <td class="formCellField">
                                                            <table border="0" cellpadding="0" cellspacing="0">
                                                                <tr>
                                                                    <td valign="top">
                                                                        <telerik:RadDateInput ID="dteDeHireDate" runat="server" Width="80px" DisplayDateFormat="dd/MM/yy"
                                                                            DateFormat="dd/MM/yy" ToolTip="The date the pallets were dehired" />
                                                                    </td>
                                                                    <td valign="top">
                                                                        <asp:RequiredFieldValidator ID="rfvDeHireDate" runat="server" ErrorMessage="Please enter the date the pallets were de-hired"
                                                                            ControlToValidate="dteDeHireDate" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" title="Please enter the date the pallets were de-hired" /></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </table>
                                                <asp:ListView ID="lvDeHirePallets" runat="server">
                                                    <LayoutTemplate>
                                                        <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <fieldset>
                                                            <table>
                                                                <tr>
                                                                    <td class="formCellLabel">
                                                                        <b>De-hire Pallets For</b>
                                                                    </td>
                                                                    <td class="formCellField">
                                                                        <asp:Label ID="lblDeHirePalletsFor" runat="server"></asp:Label>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="formCellLabel" style="width: 112px;">
                                                                        <b>De-Hire Receipt</b>
                                                                    </td>
                                                                    <td class="formCellField">
                                                                        <asp:TextBox ID="txtDeHireReceipt" runat="server"></asp:TextBox>
                                                                        <asp:RequiredFieldValidator ID="rfvDeHireReceipt" runat="server" ErrorMessage="Please enter the de-hire receipt number"
                                                                            ControlToValidate="txtDeHireReceipt" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" title="Please enter the de-hire receipt number" /></asp:RequiredFieldValidator>
                                                                    </td>
                                                                </tr>
                                                                <tr>
                                                                    <td class="formCellLabel" style="width: 112px;">
                                                                        <b>Receipt Type</b>
                                                                    </td>
                                                                    <td class="formCellField">
                                                                        <asp:DropDownList ID="cboDeHireReceiptType" runat="server" Style="width: 137px;">
                                                                        </asp:DropDownList>
                                                                    </td>
                                                                </tr>
                                                            </table>
                                                            <table>
                                                                <thead>
                                                                    <tr>
                                                                        <th class="formCellLabel" style="width: 112px;">
                                                                            Pallet Type
                                                                        </th>
                                                                        <th class="formCellLabel" style="width: 100px;">
                                                                            Planned De-hired
                                                                        </th>
                                                                        <th class="formCellLabel">
                                                                            Actual De-Hired
                                                                        </th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                                    <tr>
                                                                        <td class="formCellLabel">
                                                                            <%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.PalletType.ToString()%>
                                                                        </td>
                                                                        <td>
                                                                            <%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.NoPallets.ToString()%>
                                                                        </td>
                                                                        <td>
                                                                            <telerik:RadNumericTextBox ID="rntActualPallets" runat="server" MinValue="0" MaxValue='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.NoPallets %>'
                                                                                NumberFormat-DecimalDigits="0" Type="Number" />
                                                                            <asp:RequiredFieldValidator ID="rfvActualPallets" runat="server" ControlToValidate="rntActualPallets"
                                                                                Display="Dynamic">
                                                                                <img src="/images/error.png" alt="Please specify how many pallets were picked up." />
                                                                            </asp:RequiredFieldValidator>
                                                                            <asp:HiddenField ID="hidCollectDropID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.CollectDropId.ToString() %>' />
                                                                            <asp:HiddenField ID="hidOrderID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.Order.OrderID.ToString() %>' />
                                                                            <asp:HiddenField ID="hidCollectDropActual" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDropActual != null ? ((CollectDropWithCollectDropActual)Container.DataItem).CollectDropActual.CollectDropActualId : -1 %>' />
                                                                            <asp:HiddenField ID="hidPalletTypeID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.PalletTypeID.ToString() %>' />
                                                                        </td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </fieldset>
                                                    </ItemTemplate>
                                                    <EmptyDataTemplate>
                                                        There are no pallets to de-hire at this point
                                                    </EmptyDataTemplate>
                                                </asp:ListView>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlPickupPallets" runat="server">
                                        <tr>
                                            <td>
                                                <asp:ListView ID="lvPickUpPallets" runat="server">
                                                    <LayoutTemplate>
                                                        <table cellpadding="0" cellspacing="0">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width: 175px;">
                                                                        Pallet Type
                                                                    </th>
                                                                    <th style="width: 100px;">
                                                                        Planned
                                                                    </th>
                                                                    <th>
                                                                        Actual
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                                <tr id="itemPlaceHolder" runat="server" />
                                                            </tbody>
                                                        </table>
                                                    </LayoutTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="formCellLabel">
                                                                <%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.PalletType.ToString()%>
                                                            </td>
                                                            <td>
                                                                <%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.NoPallets.ToString()%>
                                                            </td>
                                                            <td>
                                                                <telerik:RadNumericTextBox ID="rntActualPallets" runat="server" MinValue="0" MaxValue='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.NoPallets %>'
                                                                    NumberFormat-DecimalDigits="0" Type="Number" />
                                                                <asp:RequiredFieldValidator ID="rfvActualPallets" runat="server" ControlToValidate="rntActualPallets"
                                                                    Display="Dynamic">
                                                                    <img src="/images/error.png" alt="Please specify how many pallets were picked up." />
                                                                </asp:RequiredFieldValidator>
                                                                <asp:HiddenField ID="hidCollectDropID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.CollectDropId.ToString() %>' />
                                                                <asp:HiddenField ID="hidCollectDropActual" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDropActual != null ? ((CollectDropWithCollectDropActual)Container.DataItem).CollectDropActual.CollectDropActualId : -1 %>' />
                                                                <asp:HiddenField ID="hidPalletTypeID" runat="server" Value='<%# ((CollectDropWithCollectDropActual)Container.DataItem).CollectDrop.PalletTypeID.ToString() %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <EmptyDataTemplate>
                                                        There are no pallets to collect at this point
                                                    </EmptyDataTemplate>
                                                </asp:ListView>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlReturnGoodsCallIn" runat="server">
                                        <tr>
                                            <td>
                                                <table>
                                                    <tr>
                                                        <td nowrap="true">
                                                            <h3>
                                                                Job Return Receipt Number</h3>
                                                        </td>
                                                        <td>
                                                            <asp:TextBox ID="txtReturnReceiptNumber" runat="server"></asp:TextBox>
                                                            <asp:RequiredFieldValidator ID="rfvJobReturnReceiptNumber" runat="server" ErrorMessage="Please enter the return receipt number"
                                                                ControlToValidate="txtReturnReceiptNumber" Display="Dynamic" EnableClientScript="False"><img src="/images/Error.gif" height="16" width="16" title="Please enter the return receipt number" /></asp:RequiredFieldValidator>
                                                            <a href="javascript:CopyReceiptNumber();" title="Click this link to copy the job return receipt number against each individual item.">
                                                                Copy Receipt Number</a>
                                                        </td>
                                                    </tr>
                                                    <tr>
                                                        <td colspan="2">
                                                            <asp:Repeater ID="repReturns" runat="server">
                                                                <HeaderTemplate>
                                                                    <table>
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td class="formCellLabel" style="width: 150px;">
                                                                            Product Name
                                                                        </td>
                                                                        <td class="formCellField">
                                                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsRefusal.ProductName %>
                                                                            <input type="hidden" id="hidCollectDropId" runat="server" value='<%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).CollectDropId %>' />
                                                                            <input type="hidden" id="hidCollectDropActualId" runat="server" value="0" />
                                                                            <input type="hidden" id="hidGoodsRefusalId" runat="server" value='<%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsRefusal.RefusalId %>'
                                                                                name="hidGoodsRefusalId" />
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="formCellLabel">
                                                                            Return Receipt Number
                                                                        </td>
                                                                        <td class="formCellField">
                                                                            <asp:TextBox ID="txtReturnReceiptNumber" runat="server" Text=""></asp:TextBox>
                                                                            <asp:RequiredFieldValidator ID="rfvReturnReceiptNumber" runat="server" ErrorMessage="Please enter the return receipt number"
                                                                                ControlToValidate="txtReturnReceiptNumber" Display="Dynamic" EnableClientScript="True"><img src="/images/Error.gif" height="16" width="16" title="Please enter the return receipt number" /></asp:RequiredFieldValidator>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="formCellField">
                                                                        </td>
                                                                        <td class="formCellField">
                                                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsRefusal.Docket %>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="formCellLabel">
                                                                            Quantity Refused
                                                                        </td>
                                                                        <td class="formCellField">
                                                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsRefusal.QuantityRefused %>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="formCellLabel">
                                                                            Product Code
                                                                        </td>
                                                                        <td class="formCellField">
                                                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsRefusal.ProductCode %>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="formCellLabel">
                                                                            Reference Number
                                                                        </td>
                                                                        <td class="formCellField">
                                                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsRefusal.Docket %>
                                                                        </td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td class="formCellLabel">
                                                                            Refusal Notes
                                                                        </td>
                                                                        <td class="formCellField">
                                                                        </td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                                <FooterTemplate>
                                                                    </table>
                                                                </FooterTemplate>
                                                                <SeparatorTemplate>
                                                                    <tr>
                                                                        <td colspan="2">
                                                                            <hr />
                                                                        </td>
                                                                    </tr>
                                                                </SeparatorTemplate>
                                                            </asp:Repeater>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <asp:Panel ID="pnlTrunk" runat="server">
                                        <tr>
                                            <td>
                                                <asp:Repeater ID="repOrderHandling" runat="server" OnItemDataBound="repOrderHandling_ItemDataBound">
                                                    <HeaderTemplate>
                                                        <table width="100%" border="1" cellpadding="1" cellspacing="0" bordercolor="#FFFFFF">
                                                            <thead bgcolor="#F3F5F8">
                                                                <tr>
                                                                    <th valign="top">
                                                                        Action
                                                                    </th>
                                                                    <th valign="top">
                                                                        Customer Ord No
                                                                    </th>
                                                                    <th valign="top">
                                                                        Goods Type
                                                                    </th>
                                                                    <th valign="top">
                                                                        Pallets
                                                                    </th>
                                                                    <th valign="top">
                                                                        Weight
                                                                    </th>
                                                                    <th>
                                                                        Take Refusals Off Trailer
                                                                    </th>
                                                                </tr>
                                                            </thead>
                                                            <tbody>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td>
                                                                <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).OrderAction.ToString().Replace("_", " ") %>
                                                            </td>
                                                            <td>
                                                                <a href="<%# string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderID))) %>">
                                                                    <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.CustomerOrderNumber %></a>
                                                            </td>
                                                            <td>
                                                                <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsTypeDescription %>
                                                            </td>
                                                            <td align="right">
                                                                <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoPallets %>
                                                            </td>
                                                            <td align="right">
                                                                <%# ((decimal)((Orchestrator.Entities.CollectDrop) Container.DataItem).Weight).ToString("0") %><asp:Label
                                                                    ID="lblWeightCode" runat="server" />
                                                            </td>
                                                            <td align="center">
                                                                <asp:CheckBox ID="chkTakeOffTrailer" runat="server" ToolTip="Take goods off Trailer"
                                                                    Visible="false" />
                                                                <asp:HiddenField ID="hidOrderID" runat="server" Value='<%#((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderID.ToString() %>' />
                                                            </td>
                                                        </tr>
                                                    </ItemTemplate>
                                                    <FooterTemplate>
                                                        </tbody> </table>
                                                    </FooterTemplate>
                                                </asp:Repeater>
                                            </td>
                                        </tr>
                                    </asp:Panel>
                                    <tr>
                                        <td>
                                            <table>
                                                <tr>
                                                    <td class="formCellLabel" style="width: 150px;">
                                                        Discrepancies
                                                    </td>
                                                    <td class="formCellField">
                                                        <asp:TextBox ID="txtDiscrepancies" runat="server" TextMode="MultiLine" Columns="64"
                                                            Rows="3"></asp:TextBox>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="formCellLabel">
                                                        Driver's Notes
                                                    </td>
                                                    <td class="formCellField">
                                                        <asp:TextBox ID="txtDriverNotes" runat="server" TextMode="MultiLine" Columns="64"
                                                            Rows="3"></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            <div class="buttonbar">
                                <asp:Button ID="btnPrevious" runat="server" CausesValidation="False" Text="Prev Call-in" />
                                <asp:Button ID="btnStoreDriverContact" runat="server" Text="Record Progress" CausesValidation="false" />
                                <asp:Button ID="btnStoreActual" runat="server" Text="Add Claused Call-in" OnClientClick="if(!checkPageValidation()) return false;" />
                                <asp:Button ID="btnRemoveActual" runat="server" Text="Remove Call-in" CausesValidation="False" />
                                <asp:Button ID="btnRedeliver" runat="server" Text="Claused" OnClientClick="if(!btnRedeliverClientSide()) return false;" />
                                <asp:Button ID="btnHandlePallets" runat="server" Text="Configure Pallet Handling" CausesValidation="False" />
                                <asp:Button ID="btnNext" runat="server" CausesValidation="False" Text="Next Call-in" OnClientClick="" />
                                <asp:Button ID="btnAddMoveNext" runat="server" Text="Add Clean Call-in" OnClientClick="" />
                                <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="__dialogCallBack(window, 'true');window.returnValue='true';window.close();" />
                            </div>
                            <div id="divElapsedTime" style="text-align: right; font-size: 8pt; display: none;">
                                <asp:Label ID="lblElapsedTime" runat="server"></asp:Label>
                            </div>
                        </div>
                        <asp:Label ID="InjectScript" runat="server" Text="" />
                    </telerik:RadAjaxPanel>
                </div>
            </td>
        </tr>
    </table>
    <telerik:RadAjaxLoadingPanel ID="LoadingPanel1" runat="server">
        <table width="99%" cellspacing="2" cellpadding="0" border="0">
            <tr valign="top">
                <td align="center">
                    <h1>Please wait while your action is processed...</h1>
                </td>
            </tr>
        </table>
    </telerik:RadAjaxLoadingPanel>
    <div id="divLoading" style="height: 700px; display: none;">
        <table width="99%" cellspacing="2" cellpadding="0" border="0">
            <tr valign="top">
                <td align="center">
                    <h1>
                        Please wait while your action is processed...</h1>
                </td>
            </tr>
        </table>
    </div>
    <div id="backgroundPopup">
    </div>
    <div id="popupContact" style="display: none;">
        <h1>Confirm Refusal Type</h1>
        <p>
            Please confirm the type of Refusal.
        </p>
        <div class="buttonBar">
            <asp:Button ID="btnFull" runat="server" Text="Full Refusal" CausesValidation="false" OnClientClick="btnCreateRefusal('Full'); return false;" />
            <asp:Button ID="btnPartial" runat="server" Text="Partial Refusal" CausesValidation="false" OnClientClick="btnCreateRefusal('Partial'); return false;" />
            <asp:Button ID="btnCancelRefusal" runat="server" Text="Cancel" CausesValidation="false" OnClientClick="btnCancelRefusalSelection(); return false;" />
        </div>
    </div>

    <asp:HiddenField ID="hiddenPanelArray" runat="server" />
    <asp:HiddenField ID="hidRefusalType" runat="server" />
    <asp:Button ID="btnConfirmRefusalType" runat="server" Text="" Style="display:none;" />
    
    <asp:Label ID="lblInjectScript" runat="server"></asp:Label>
    
    <telerik:RadCodeBlock ID="ScriptRadCodeblock" runat="server">
        <telerik:RadAjaxManager ID="ramCallin" runat="server" ClientEvents-OnResponseEnd="ramCallInAjaxRequest_OnResponseEnd">
        </telerik:RadAjaxManager>
        <script type="text/javascript" language="javascript">
        //<!--
            function btnCreateRefusal(refusalType)
            {
                $("#<%=hidRefusalType.ClientID%>").val(refusalType);
                $get("<%=btnConfirmRefusalType.ClientID%>").click();
            }

            function btnRedeliverClientSide(sender, eventArgs)
            {
                //centering with css  
                centerPopup();  
                //load popup  
                loadPopup();  

                return false;
            }

            function btnCancelRefusalSelection(sender, eventArgs)
            {
                disablePopup();
            }

            var popupStatus = 0;  

            function loadPopup()
            {  
                //loads popup only if it is disabled  
                if(popupStatus==0){  
                    $("#backgroundPopup").css({  
                        "opacity": "0.7"  
                    });  
                    
                    $("#backgroundPopup").fadeIn("slow");  
                    $("#popupContact").fadeIn("slow");  
                    
                    popupStatus = 1;  
                }  
            } 

            function disablePopup()
            {  
                //disables popup only if it is enabled  
                if(popupStatus==1){  
                    $("#backgroundPopup").fadeOut("slow");  
                    $("#popupContact").fadeOut("slow");  
                    popupStatus = 0;  
                }  
            }  

            function centerPopup(){  
                //request data for centering  
                var windowWidth = document.documentElement.clientWidth;  
                var windowHeight = document.documentElement.clientHeight;  
                var popupHeight = $("#popupContact").height();  
                var popupWidth = $("#popupContact").width();  
                //centering  
                $("#popupContact").css({  
                    "position": "absolute",  
                    "top": windowHeight/2-popupHeight/2,  
                    "left": windowWidth/2-popupWidth/2  
                });  
                //only need force for IE6  
  
                $("#backgroundPopup").css({  
                    "height": windowHeight  
                });  
  
            } 
    
            var ospnAddPCV = document.getElementById('spnAddPCV');
    
            var isChecked = false;
            var allChecked = true;

            $(document).ready(function() {
                $('#divLoading').hide();
                $(".wrapperPalletTypePCV[visible='true']").show();

                //Click out event!  
                $("#backgroundPopup").click(function(){  
                    disablePopup();  
                });  
                //Press Escape event!  
                $(document).keypress(function(e){  
                    if(e.keyCode==27 && popupStatus == 1){  
                        disablePopup();  
                    }  
                });  
            });

            
    
            function validatePCVVoucherNumber(sender, eventArgs)
            {
                var value = eventArgs.Value;
        
                if(value.length <= 0)
                    eventArgs.IsValid = false;
            
                if(isNaN(value))
                    eventArgs.IsValid = false;
            
                if(!parseInt(value, 10))
                    eventArgs.IsValid = false;
    
                if(!eventArgs.IsValid)
                    $('#' + sender.id).closest(".wrapperPalletTypePCV").show();
            }
    
            function refreshIframe(jobID)
            {

                var url = "/traffic/jobManagement/CallInSelector.aspx?JobId=" + jobID+ getCSID();

                $("#ifCallInSelector").attr("src", url);
            }
    
            function chkSelectAllAttachedPDFs(chkHeaderSelect)
            {
                var btnRemoveActual = $("#<%=btnRemoveActual.ClientID%>");
                var checkBoxes = $("#pcvWrapper :checkbox"); //All PCV Checkboxes
                var btnUpdateAttachedPCVs = $("#btnUpdateAttachedPCVs_Wrapper"); //Buttonbar (div)
        
                isChecked = false;
        
                for(var i = 0; i < checkBoxes.length; i++)
                    if(checkBoxes[i].id != chkHeaderSelect.id)
                    {
                        if(!checkBoxes[i].disabled)
                            checkBoxes[i].checked = chkHeaderSelect.checked;
                
                        if(chkHeaderSelect.checked)
                            isChecked = chkHeaderSelect.checked;
                    }
            
                if(isChecked && btnRemoveActual[0] != undefined)
                    btnUpdateAttachedPCVs.show();
                else
                    btnUpdateAttachedPCVs.hide();
        
            }

            function chkSelectedPCV_Checked(chkSelectedPCV)
            {
                var btnRemoveActual = $("#<%=btnRemoveActual.ClientID%>");
                var checkBoxes = $("#pcvWrapper :checkbox"); //All PCV Checkboxes
                var btnUpdateAttachedPCVs = $("#btnUpdateAttachedPCVs_Wrapper"); //Buttonbar (div)
                var chkHeaderPCV = $("#pcvWrapper :checkbox:first"); //Header Checkbox
        
                isChecked = false;
                allChecked = true;
        
                for(var i = 0; i < checkBoxes.length; i++)
                {
                    if(checkBoxes[i].checked && checkBoxes[i].id != chkHeaderPCV[0].id)
                        isChecked = checkBoxes[i].checked;
                
                    if(!checkBoxes[i].checked && checkBoxes[i].id != chkHeaderPCV[0].id && !checkBoxes[i].disabled)
                        allChecked = checkBoxes[i].checked;    
                }
        
                if(isChecked) //If any child checkboxes are selected.
                {
                    chkHeaderPCV[0].checked = allChecked;
            
                    if(btnRemoveActual[0] != undefined)
                        btnUpdateAttachedPCVs.show();
                }
                else
                {
                    btnUpdateAttachedPCVs.hide();
                    chkHeaderPCV[0].checked = isChecked; //If any of the child checkboxes are unticked then de-select the header checkbox.
                }
            }
    
            function openResourceWindow(InstructionId, Driver, DriverResourceId, RegNo, VehicleResourceId, TrailerRef, TrailerResourceId, JobId, LastUpdateDate) 
            {
                var qs = "iID=" + InstructionId + "&Driver=" + Driver + "&DR=" + DriverResourceId + "&RegNo=" + RegNo + "&VR=" + VehicleResourceId + "&TrailerRef=" + TrailerRef + "&TR=" + TrailerResourceId + "&jobId=" + JobId + "&LastUpdateDate=" + LastUpdateDate;
                <%=dlgResource.ClientID %>_Open(qs);
            }

            function openPalletHandlingWindow(jobId)
            {
                var qs = "jobId=" + jobId;
                <%=dlgPalletHandling.ClientID %>_Open(qs);
            }
    
            function ClockIn(resourceId)
            {
                var qs = "resourceId=" + resourceId + "&date=<%=DateTime.UtcNow.ToString("dd/MM/yyyy")%>";
                <%=dlgDriverClockIn.ClientID %>_Open(qs);
            }
    
            function ChangePage(url)
            {
                url;
            }

            function CopyReceiptNumber()
            {
                var jobReturnNumber = "<%=txtReturnReceiptNumber.ClientID %>";
                var control = document.getElementById(jobReturnNumber);
        
                if (control != null)
                {
                    var value = control.value;
                    var textboxes = "<%=RetrieveReturnReceiptTextBoxClientIDs() %>";
            
                    if (textboxes.length > 0)
                    {
                        var arrTextboxes = textboxes.split(',');
                        for (textboxIndex = 0; textboxIndex < arrTextboxes.length; textboxIndex++)
                            document.getElementById(arrTextboxes[textboxIndex]).value = value;
                    }
                }
            }

            function PromptForPCV(deliveredClientId, returnedClientId, pcvPalletsID, divPCV, previousPalletReturnedCount, totalPalletCount, cfvPCVVoucherCodeId, cfvPCVPalletsId)
            {
                var hiddenPanelArray = $("#hiddenPanelArray");
                var delivered = document.getElementById(deliveredClientId);
                var returned = document.getElementById(returnedClientId);
                var previousReturned = document.getElementById(previousPalletReturnedCount);
                var cfvPCVVoucherCode = document.getElementById(cfvPCVVoucherCodeId);
                var cfvPCVPallets = document.getElementById(cfvPCVVoucherCodeId);
    	
                if (delivered != null && returned != null && divPCV != null)
                {
                    var blnDeliveredIsNotANumber = isNaN(delivered.value);
                    var blnReturnedIsNotANumber = isNaN(returned.value);
    		
                    if (!blnDeliveredIsNotANumber && !blnReturnedIsNotANumber && (delivered.value.indexOf('.') == -1) && (returned.value.indexOf('.') == -1))
                    {
                        var deliveredCount = parseInt(delivered.value, 10);
                        var returnedCount = parseInt(returned.value, 10);
                        var oldReturnedCount = parseInt(previousReturned.value, 10);
                        var pallets = parseInt($("#"+pcvPalletsID).val());
                
                        if(!isNaN(returnedCount))
                        {
                            if ($("#"+pcvPalletsID).val() == "")
                                pallets = 0;
        			
		                    var newCount = (deliveredCount - returnedCount);
                    
                            pallets -= oldReturnedCount;
                            pallets += newCount;

                            if(pallets < 0)
                                pallets = 0;
                    
                            $("#"+pcvPalletsID).val(pallets);
                            $("#"+previousPalletReturnedCount).val(newCount);
                   
                            var current = hiddenPanelArray.val();
                            hiddenPanelArray.val(current + "," + divPCV);
                    
                            $("#" +divPCV).css("display", "");

                            if(pallets <= 0) {
                                $("#" +divPCV).css("display", "none");
                                if(cfvPCVVoucherCode != null)
                                    ValidatorEnable(cfvPCVVoucherCode, false);
                                    ValidatorEnable(cfvPCVPallets, false);
                            
                            } else {
                                if(cfvPCVVoucherCode != null)
                                    ValidatorEnable(cfvPCVVoucherCode, true);
                                    ValidatorEnable(cfvPCVPallets, true);
                            }
                        }
                    }
                }
            }

            function NoPCVIssuedForDeliveryPoint(txtPCVPallets, txtVoucherNo, rcbReasonForIssueId, rfvPCVPallets, cfvPCVPallets, chkNoPCVIssued)
            {
                var rcbReasonForIssue = $find(rcbReasonForIssueId);    
                var isChecked = chkNoPCVIssued.checked;
        
                txtPCVPallets.disabled = isChecked;
                txtVoucherNo.disabled = isChecked;
       
                var cssObj;
        
                if(isChecked)
                {
                    cssObj = {'background-color' : '#B8B8B8'}
                    rcbReasonForIssue.disable();
                    ValidatorEnable(rfvPCVPallets, false);
                    ValidatorEnable(cfvPCVPallets, false);
                }
                else
                {
                    cssObj = {'background-color' : '#FFFFFF'}    
                    rcbReasonForIssue.enable();
                    ValidatorEnable(rfvPCVPallets, true);
                    ValidatorEnable(cfvPCVPallets, true);
                }
        
                $(txtPCVPallets).css(cssObj)
                $(txtVoucherNo).css(cssObj)
            }
    
            function rcbPalletType(sender, eventArgs)
            {
                //This is present, as when the select method is called below, we exit this function as its already being actioned from the origianl call.
                if(eventArgs.get_domEvent() == null)
                    return;

                var parent = $('#' + sender.get_id()).parents('[id*=divClientPalletWrapper]');
                var collectDropItem = $('#' + sender.get_id()).parent().parent().parent().parent().parent().parent();
    
                var tblPalletReturned = parent.find('table[id*=tblPalletReturned]');
                var hidTrackingPalletType = parent.find('input[id*=hidTrackingPalletType]');

                if(tblPalletReturned != null && tblPalletReturned.length > 0)
                {
                    var selectedItem = sender.get_selectedItem();
                    var isTracked = selectedItem.get_attributes().getAttribute("IsTracked");

                    var rcbPalletTypes = parent.find('input[id*=rcbPalletType]');

                    if(rcbPalletTypes.length > 1)
                        for (var i = 0; i < rcbPalletTypes.length; i++)
                            if(rcbPalletTypes[i].id != (sender.get_id() + "_Input"))
                            {
                                var comboBox = $find(rcbPalletTypes[i].id.replace("_Input", ""));
                                if(comboBox != null)
                                {
                                    var item = comboBox.findItemByValue(selectedItem.get_value());
                                    item.select(null, null);
                                }
                            }
            
                    // Set the hid pallet tracking variable correctly.
                    hidTrackingPalletType.val(isTracked);
                    var cfvPCVVoucherCode = collectDropItem.find("span[id*=cfvPCVVoucherCode]");
                    var rfvPCVPallets = collectDropItem.find("span[id*=rfvPCVPallets]");
                    var cfvPCVPallets = collectDropItem.find("span[id*=cfvPCVPallets]");

                    var lblPalletTypeFooter = collectDropItem.find("span[id*=lblPalletTypeFooter]");
                    var hidPalletType = collectDropItem.find("input[id*=hidPalletType]");
            
                    hidPalletType.val(selectedItem.get_value());
            
                    if(isTracked.toLowerCase() == "false") {
                        var txtPalletsReturned = parent.find("input[id*=txtPalletsReturned]");
                        var pnlPalletTypePCV = collectDropItem.find("div[id*=pnlPalletTypePCV]");
                
                        txtPalletsReturned.val("0");
                        
                        ValidatorEnable(cfvPCVVoucherCode[0], false);
                        ValidatorEnable(rfvPCVPallets[0], false);
                        ValidatorEnable(cfvPCVPallets[0], false);

                        pnlPalletTypePCV.hide();
                        tblPalletReturned.hide();
                        parent.find(".wrapperPalletTypePCV").hide();
                    }
                    else {
                        ValidatorEnable(cfvPCVVoucherCode[0], true);
                        ValidatorEnable(rfvPCVPallets[0], true);
                        ValidatorEnable(cfvPCVPallets[0], true);

                        lblPalletTypeFooter.text(selectedItem.get_text());
                        tblPalletReturned.show();
                        parent.find(".wrapperPalletTypePCV").show();
                    }
                }        
            }

            function btn_MoveNext(url)
            {
                location.href = url;
                return false;
            }

            function btn_MovePrevious(url)
            {
                location.href = url;
                return false;
            }

            function btn_AddMoveNext(url) 
            {
                var divLoading = $('#divLoading');
                
                if(checkPageValidation())
                {
                    var ramPreInvoice = $find("<%=ramCallin.ClientID %>");
                    var raxPanel = $('#' + '<%=raxPanel.ClientID%>');

                    raxPanel.hide();
                    divLoading.show();
                    ramPreInvoice.ajaxRequest(url);
                }
                else
                    divLoading.hide();

                return false;
            }

            function checkPageValidation()
            {
                var isValid = Page_ClientValidate("vgReturnPallets");
                
                if(isValid && !$('#divDepartureDateValidation').is(":visible"))
                    return true;
                else
                    return false;
            }

            function ramCallInAjaxRequest_OnResponseEnd(sender, eventArgs)
            {
//                alert('Response end initiated by: ' + eventArgs.get_eventTarget());
                location.href = eventArgs.EventArgument;
            }

            function validatePalletsReturned(sender, eventsArgs)
            {
                eventsArgs.IsValid = false;

                var parent = $('#' + sender.id).parents('[id*=divClientPalletWrapper]');

                var hidCollectDropCount = parent.find('[id*=hidCollectDropCount]');
                var txtPCVPallets = parent.find('[id*=txtPCVPallets]');

                var collectDropCount = parseInt(hidCollectDropCount.val());

                var palletsReturnedCount = 0, palletsDeliveredCount = 0;

                for(var i = 0; i < collectDropCount; i++)
                {
                    var txtPalletsReturned = $('#' + eval('sender.txtPalletsReturned' + i));
                    var txtPalletsDelivered = $('#' + eval('sender.txtPalletsDelivered' + i));
                
                    if(txtPalletsReturned.val() != "")
                        palletsReturnedCount = palletsReturnedCount + parseInt(txtPalletsReturned.val(), 10);

                    if(txtPalletsDelivered.val() != "")
                        palletsDeliveredCount = palletsDeliveredCount + parseInt(txtPalletsDelivered.val(), 10);
                }

                if((palletsReturnedCount + parseInt(txtPCVPallets.val(), 10)) >= palletsDeliveredCount)
                    eventsArgs.IsValid = true;
            }

            function validateDepartureDate(sender, eventArgs)
            {
                var dteArrivalDate = $find("<%=dteArrivalDate.ClientID %>");
                var dteArrivalTime = $find("<%=dteArrivalTime.ClientID %>");
                var dteDepartureDate = $find("<%=dteDepartureDate.ClientID %>");
                var dteDepartureTime = $find("<%=dteDepartureTime.ClientID %>");

                var arrivalDate = new Date();
                var departureDate = new Date();

                var arrivalTime = new Date();
                var departureTime = new Date();

                arrivalDate = dteArrivalDate.get_selectedDate();
                departureDate = dteDepartureDate.get_selectedDate();

                arrivalTime = dteArrivalTime.get_selectedDate();
                departureTime = dteDepartureTime.get_selectedDate();

                arrivalDate.setHours(arrivalTime.getHours(), arrivalTime.getMinutes(), 0);
                departureDate.setHours(departureTime.getHours(), departureTime.getMinutes(), 0);
                
                if(arrivalDate > departureDate)
                    $('#divDepartureDateValidation').show();
                else
                    $('#divDepartureDateValidation').hide();
            }

            function validateWholeNumber(sender, eventArgs)
            {

            }

            function validateCases(sender, eventArgs)
            {
                eventArgs.IsValid = false;

                if (eventArgs.Value < 0 || eventArgs.Value > 99999)
                    eventArgs.IsValid = false;
                else
                    eventArgs.IsValid = true;
            }

            function validatePallets(sender, eventArgs)
            {
                eventArgs.IsValid = false;

                if (eventArgs.Value < 0 || eventArgs.Value > 999)
                    eventArgs.IsValid = false;
                else
                    eventArgs.IsValid = true;
            }

            //-->    
        </script>
    </telerik:RadCodeBlock>
</asp:Content>
