<%@ Page Title="" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true"
    CodeBehind="GeofenceManagement.aspx.cs" Inherits="Orchestrator.WebUI.Point.Geofences.GeofenceManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <style type="text/css">
        .masterpagepopup_contentHolder
        {
            min-height: 700px !important;
        }
        .masterpagepopup_layoutHeaderInner
        {
            padding-bottom: 1px !important;
        }
        h2
        {
            color: white !important;
            font-size: 15px !important;
        }
        .buttonclass
        {
            width: 125px !important;
        }
        #wizSteps
        {
            float: left;
            width: 130px;
            height: 650px;
        }
        #wizSteps table
        {
            padding:0 !important;
        }
        #wizMain
        {
            float: Left;
            position: relative;
            width: 850px;
            height: 650px;
        }
        #mode
        {
            color: Gray;
            padding: 2px;
            position: absolute;
            top: 40px;
        }
        
        #btnEdit
        {
            margin-top: 2px;
            width: 100px;
        }
        #btnDelete
        {
            margin-top: 2px;
            width: 100px;
        }
        #btnDraw
        {
            margin-top: 2px;
            width: 100px;
        }
        #btnSave
        {
            margin-top: 2px;
            width: 100px;
        }
        #btnMove
        {
            margin-top: 2px;
            width: 100px;
        }
        #btnClose
        {
            margin-top: 2px;
            width: 100px;
        }
        
        
        #savedMessage
        {
            height: 35px;
            width: 200px;
            border: 2pt solid white;
            border-radius: 5px;
            text-align: center;
            font-size: 14px;
            color: Black;
            background: rgba(178, 206, 255, 100);
            position: absolute;
            top: 85px;
            left: 320px;
            z-index: 9999;
            padding-top: 13px;
            display: none;
        }
        #toolbar
        {
            position: absolute;
            z-index: 9999;
            background-color: #FAF7F5;
            height: 29px;
            width: auto;
            left: 160px;
            padding-left: 30px;
            padding-right: 30px;
        }
        #wizSteps table
        {
            padding-left: 20px;
            padding-top: 20px;
        }
        
        .WAP
        {
            font-size: 6pt;
            width:100%;
        }
        .active1
        {
            background-color: green;
        }
        .inactive
        {
            background-color: blue;
        }
        
        .btnSetActiveTimes
        {
            font-size:15px;
            width:140px;
        }
        
        .ui-tabs
        {
            border-bottom:none !important;
        }
    </style>
    

    <!--Here Maps Scripts-->
    <script src="<%=HereMapsCoreJS%>"></script>
    <script src="<%=HereMapsServiceJS%>"></script>
    <script src="<%=HereMapsEventsJS%>"></script>
    <script src="<%=HereMapsUIJS%>"></script>
    <script src="<%=HereMapsClusteringJS%>"></script>
    <link rel="stylesheet" type="text/css" href="<%=HereMapsUICSS%>" />

     <script>
        //HERE Maps
        var platform = new H.service.Platform({
            app_id: '<%=HereMapsApplicationId%>',
            app_code: '<%=HereMapsApplicationCode%>'
        });
    </script>
    <!-- End Here Maps Scripts -->

    <script src="../../script/jquery-ui-min.js" type="text/javascript"></script>
    <script type="text/javascript" src="GeofenceManagement.aspx.js"></script>
    <script type="text/javascript" src="geofencenotifications.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">
    <h2><asp:Label ID="lblPointDescription" runat="server"></asp:Label></h2>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <telerik:RadAjaxManager runat="server">
        <AjaxSettings>
            <telerik:AjaxSetting AjaxControlID="btnAddContact">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="rgContacts" />
                    <telerik:AjaxUpdatedControl ControlID="txtNotificationName" />
                    <telerik:AjaxUpdatedControl ControlID="txtContactDetail" />
                </UpdatedControls>
            </telerik:AjaxSetting>
            <telerik:AjaxSetting AjaxControlID="rtvGrouping">
                <UpdatedControls>
                    <telerik:AjaxUpdatedControl ControlID="lbVehicles" />
                    <telerik:AjaxUpdatedControl ControlID="lbVehiclesInView" />
                </UpdatedControls>
            </telerik:AjaxSetting>
        </AjaxSettings>
    </telerik:RadAjaxManager>

    <telerik:RadWindowManager runat="server"></telerik:RadWindowManager>
    <div style="width:100%;">
    <div id="wizSteps" class="buttonBar" style="float:left;" >
        
        <table>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnGeofence" Text="Change Geofence" CssClass="buttonclass" CausesValidation="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnVehicles" Text="Which Vehicles" CssClass="buttonclass" CausesValidation="false" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnNotifications" Text="Notifications" CssClass="buttonclass" CausesValidation="false" />
                </td>
            </tr>
        </table>
    </div>

    <asp:Label ID="lblPointID" runat="server" CssClass="hidden"></asp:Label>
    <div class="cleardiv">
    </div>
    <div id="wizMain">
        <telerik:RadMultiPage runat="server" ID="rmpGeofence" RenderSelectedPageOnly="true">

            <telerik:RadPageView ID="pvGeofence" runat="server" Selected="true">
                <div id="toolbar" style="float:left;">
                    <div id="mode"></div>
                    <button id="btnDraw" class="buttonClass">Begin Drawing</button>
                    <button id="btnEdit" class="buttonClass">Edit Geofence</button>
                    <button id="btnDelete" class="buttonClass">Clear</button>
                    <button id="btnSave" class="buttonClass">Save Changes</button>
                </div>
                <div id="savedMessage">Changes saved</div>
                <div id="map" style="position:relative; float:right; width: 850px; height: 660px;"></div>
            </telerik:RadPageView>

            <telerik:RadPageView ID="pvVehicles" runat="server">
                <table>
                    <tr>
                        <th>
                            Grouping
                        </th>
                        <th>
                            Vehicles
                        </th>
                        <th>
                            Vehicles in List
                        </th>
                    </tr>
                    <tr style="vertical-align: top;">
                        <td>
                            <telerik:RadTreeView ID="rtvGrouping" runat="server" EnableDragAndDrop="True" Width="200"
                                EnableDragAndDropBetweenNodes="false" MultipleSelect="false" Height="400">
                            </telerik:RadTreeView>
                        </td>
                        <td>
                            <telerik:RadListBox runat="server" ID="lbVehicles" Width="200" EnableDragAndDrop="true"
                                AllowTransfer="true" TransferToID="lbvehiclesInView" AllowTransferDuplicates="false"
                                Height="600" MultipleSelect="true" AllowTransferOnDoubleClick="true" AutoPostBackOnTransfer="false"
                                SelectionMode="Multiple" />
                        </td>
                        <td>
                            <telerik:RadListBox runat="server" ID="lbVehiclesInView" Width="200" EnableDragAndDrop="true"
                                Height="600" SelectionMode="Multiple" />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="3">
                            <div class="buttonBar" style="text-align: right;">
                                <asp:Button ID="btnVehicleSave" runat="server" Text="Save and Next >" CssClass="buttonClass" />
                            </div>
                        </td>
                    </tr>
                </table>
            </telerik:RadPageView>

            <telerik:RadPageView ID="pvNotifications" runat="server">
               <h3>Notification Details</h3>
                <asp:CustomValidator runat="server" ID="cvNotificationContacts" ValidationGroup="Notification" ErrorMessage="You must enter at least one contact for this notification." CssClass="error"></asp:CustomValidator>
                    <h3>Existing Notifications</h3>
                    <telerik:RadGrid runat="server" ID="rgNotifications" AutoGenerateColumns="false">
                        <MasterTableView NoMasterRecordsText="There are no existing Notifications for this Point" DataKeyNames="NotificationId">
                            <Columns>
                                <telerik:GridBoundColumn DataField="Description" HeaderText="Title">
                                </telerik:GridBoundColumn>
                                <telerik:GridTemplateColumn HeaderText="Recipients">
                                    <ItemTemplate>
                                        <asp:Label ID="lblRecipients" runat="server"></asp:Label>
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridButtonColumn ButtonType="LinkButton" Text="Change" CommandName="Select"></telerik:GridButtonColumn>
                                <telerik:GridButtonColumn ButtonType="LinkButton" Text="Delete" CommandName="Delete" ConfirmText="Please confirm that you want to remove this Notification" ConfirmDialogType="Classic" ConfirmTitle="Delete?"></telerik:GridButtonColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                    <div id="tabs">
                         <ul>
                             <li><a href="#tabs-1"><span>Details</span></a></li>
                             <li><a href="#tabs-2"><span>Recipients</span></a></li>
                             <li><a href="#tabs-3"><span>Active Times</span></a></li>
                         </ul>
                    </div>

                 
                   <div id="tabs-1">
                         <table>
                            <tr>
                                <td class="fieldLabel">
                                    Name of this Notification List
                                </td>
                                <td class="fieldInput">
                                    <asp:TextBox ID="txtNotificationTitle" runat="server" Width="250"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator1"
                                        runat="server" ControlToValidate="txtNotificationTitle" ErrorMessage="Please enter a title for this Notification"
                                        Display="Dynamic"></asp:RequiredFieldValidator>
                                </td>
                            </tr>
                         <tr>
                            <td class="fieldLabel">
                                When to Alert
                            </td>
                            <td>
                                <asp:CheckBox ID="chkIncoming" runat="server" Text="Incoming" />
                                <asp:CheckBox ID="chkOutgoing" runat="server" Text="Outgoing" />
                            </td>
                         </tr>
                            <tr>
                                <td>
                                </td>
                                <td class="fieldInput">
                                    <asp:CheckBox runat="server" ID="chkNotificationEnabled" Text="Is turned on" />
                                </td>
                            </tr>
                        </table>
                    </div>
                    <div id="tabs-2">
                         <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="Contact" runat="server" DisplayMode="BulletList" />
                            <table>
                                <tr>
                                    <td class="fieldLabel">
                                        How to Contact
                                    </td>
                                    <td class="fieldInput">
                                        <asp:DropDownList runat="server" ID="cboContactType">
                                            <asp:ListItem Text="Email" Value="1" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Text message" Value="2"></asp:ListItem>
                                        </asp:DropDownList>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fieldLabel">
                                        Name
                                    </td>
                                    <td class="fieldInput">
                                        <asp:TextBox runat="server" ID="txtNotificationName"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator2"
                                            ValidationGroup="Contact" runat="server" ControlToValidate="txtNotificationName"
                                            ErrorMessage="The Contact name cannot be empty." Display="None"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="fieldLabel">
                                        Email address or Mobile Number
                                    </td>
                                    <td class="fieldInput">
                                        <asp:TextBox runat="server" ID="txtContactDetail" Width="250"></asp:TextBox><asp:RequiredFieldValidator ID="RequiredFieldValidator3"
                                            ValidationGroup="Contact" runat="server" ControlToValidate="txtContactDetail"
                                            ErrorMessage="You must enter the contact detail"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2" >
                                        <div class="buttonBar">
                                            <asp:Button runat="server" Text="Add Contact" ID="btnAddContact" ValidationGroup="Contact" CssClass="buttonclass" />
                                        </div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <telerik:RadGrid runat="server" ID="rgContacts" AutoGenerateColumns="false">
                                            <MasterTableView DataKeyNames="IdentityId, Recipient">
                                                <Columns>
                                                    <telerik:GridBoundColumn DataField="Username" HeaderText="Name">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="ContactType" HeaderText="How">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridBoundColumn DataField="Recipient" HeaderText="Detail">
                                                    </telerik:GridBoundColumn>
                                                    <telerik:GridButtonColumn CommandName="Delete" ButtonType="LinkButton" Text="Delete" ConfirmText="Please confirm that you want to delete this contact" ConfirmTitle="Delete ?"></telerik:GridButtonColumn>
                                                </Columns>
                                            </MasterTableView>
                                        </telerik:RadGrid>
                                    </td>
                                </tr>
                        
                            </table>
                    </div>
                    <div id="tabs-3">
                         <span id="tempfocus"></span>
                            <asp:Table ID="tblMain" runat="server" CssClass="WAP" CellPadding="2" CellSpacing="1"
                            GridLines="Horizontal">
                        </asp:Table>
                        <div style="margin-top:5px;">
                            You can select multiple time slots by clicking and dragging to highlight the times, clicking or dragging again will change the active state of the time slot.
                        </div>
                        <asp:HiddenField runat="server" ID="hiddenActives" />
                    </div>
                

                <div class="buttonbar" style="text-align: right; margin-top:20px;"">
                    <asp:Button ID="btnAddNotification" runat="server" Text="Add Notification" OnClientClick="if (!SaveWAP()) return false;"  ValidationGroup="Notification" CssClass="buttonclass"/>
               </div>
               <telerik:RadCodeBlock runat="server">
               <script type="text/javascript">
                   var _wapeditprefix = 'ctl00_ContentPlaceHolder1_cell_';
                   var tbl = $get('<%=tblMain.ClientID %>');
                   var hidden = $get('<%= hiddenActives.ClientID %>');
                   Init();

                </script>
                </telerik:RadCodeBlock>
            </telerik:RadPageView>

        </telerik:RadMultiPage>
    </div>
    </div>
    <asp:Label ID="lblScreen" runat="server" Text="Geofence" CssClass="hidden"></asp:Label>
</asp:Content>
