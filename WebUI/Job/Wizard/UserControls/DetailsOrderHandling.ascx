
<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Reference Control="~/job/wizard/usercontrols/jobtype.ascx" %>
<%@ Reference Page="~/job/wizard/wizard.aspx" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DetailsOrderHandling.ascx.cs" Inherits="Orchestrator.WebUI.Job.Wizard.UserControls.DetailsOrderHandling" %>
<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<cc1:Dialog runat="server" ID="dlgAddOrder" ReturnValueExpected="true" AutoPostBack="true" URL="/job/addorder.aspx" Height="600" Width="800"></cc1:Dialog>
<cc1:Dialog runat="server" ID="dlgOrder" ReturnValueExpected="true" AutoPostBack="true" URL="/groupage/manageorder.aspx" Height="900" Width="1200" ></cc1:Dialog>
<cc1:Dialog runat="server" ID="dlgConvertInstruction" ReturnValueExpected="true" AutoPostBack="true" URL="/Job/ConvertInstruction.aspx" Height="600" Width="1000"></cc1:Dialog>

<script type="text/javascript" language="javascript" src="/script/popAddress.js"></script>

<link runat="server" id="linkJobStyles" href="" type="text/css" rel="stylesheet" />

<asp:ScriptManagerProxy runat="server" ID="scriptManagerProxy">
    <Scripts>
        <asp:ScriptReference Path="~/script/handlebars-1.0.rc.2.js" />
        <asp:ScriptReference Path="~/script/templates.js" />
    </Scripts>
</asp:ScriptManagerProxy>

<asp:Panel ID="pnlGroupageJob" runat="server">
    <asp:Repeater ID="repOH" runat="server">
        <HeaderTemplate>
            <table width="100%" cellspacing="0">
                <thead>
                    <tr>
                        <td colspan="2">
                            <h3>Orders</h3>
                        </td>
                    </tr>
                </thead>
                <tbody>
        </HeaderTemplate>
        <ItemTemplate>
            <tr runat="server" id="tblRowCollectDrops">
                <td class="InstructionBoxContainerLeft" style="width: 350px; vertical-align: top;">
                    <table width="100%" cellspacing="0">
                        <tr>
                            <td>
                                <table class="InstructionPoint" width="100%" cellspacing="0" cellpadding="2">
                                    <tr>
                                        <td style="width: 55px;">
                                            <img runat="server" id="imgInstructionType" align="left"  alt="instruction type"/>
                                        </td>
                                        <td>
                                            <asp:Label ID="lblInstructionType" runat="server" Font-Bold="true"></asp:Label><br />
                                            <span id="spnCollectionPoint" class="ShowPointTooltip" rel="/point/getpointaddresshtml.aspx" pointid="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PointId %>">
                                                <%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.OrganisationName %>
                                                <br />
                                                <%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %>
                                            </span>
                                        </td>
                                        <td align="right">
                                            <asp:ImageButton ID="imgBtnDown" runat="server" ImageUrl="~/App_Themes/Orchestrator/img/MasterPage/icon-arrow-down.png" CausesValidation="false" CommandName="Down" ToolTip="Move instruction down" />
                                            <asp:ImageButton ID="imgBtnUp" runat="server" ImageUrl="~/App_Themes/Orchestrator/img/MasterPage/icon-arrow-up.png" CausesValidation="false" CommandName="Up" ToolTip="Move instruction up" />
                                            <asp:ImageButton ID="imgBtnMerge" runat="server" ImageUrl="~/App_Themes/Orchestrator/img/MasterPage/icon-merge.png" CausesValidation="false" CommandName="Merge" ToolTip="Merge this instruction with the following instruction" />
                                        </td>
                                        <td align="right" >
                                            <asp:Button ID="btnRedeliver" Visible="false" runat="server" Text="" CssClass="buttonClassRedeliver-Add" ToolTip="Attempt Later" />
                                            <asp:Button ID="btnRecordCallIn" Text="" CssClass="buttonClassCallIn-Remove"  runat="server" />
                                            <asp:Button ID="btnAddOrder" visible="false" runat="server" Text="" CssClass="buttonClassAdd" ToolTip="Add order" /> 
                                            <asp:Button ID="btnConvertDrop" visible="false" runat="server" Text="" CssClass="buttonClassConvert" ToolTip="Convert instruction" />
                                        </td>
                                    </tr>
                                </table>
                                <input type="hidden" id="hidInstructionId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionID %>" />
                                <input type="hidden" id="hidInstructionTypeId" runat="server" value="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionTypeId %>" />
                            </td>
                        </tr>
                    </table>
                    <table class="InstructionLeft">
                        <tr>
                            <td style="padding-left: 2px;">
                                Pallets On:
                            </td>
                            <td>
                                <asp:Label ID="lblPalletsOn" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 2px;">
                                Booked:
                            </td>
                            <td>
                                <asp:Label ID="lblBookedDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 2px;">
                                Planned:
                            </td>
                            <td>
                                <asp:Label ID="lblPlannedDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="tblrowArrival" runat="server" visible="false">
                            <td style="padding-left: 2px;">
                                Arrival:
                            </td>
                            <td>
                                <asp:Label ID="lblArrivalDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="tblrowDeparture" runat="server" visible="false">
                            <td style="padding-left: 2px;">
                                Departure:
                            </td>
                            <td>
                                <asp:Label ID="lblDepartureDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="tblrowCallin" runat="server" visible="false">
                            <td style="padding-left: 2px;">
                                Called-in:
                            </td>
                            <td>
                                <asp:Label ID="lblCallIn" runat="server" ></asp:Label>
                            </td>
                        </tr>
                        <tr id="tblrowMwfStatus" runat="server" visible="false">
                            <td style="padding-left: 2px;">
                                MWF Status:
                            </td>
                            <td>
                                <asp:Label ID="lblMwfStatus" runat="server" />
                            </td>
                        </tr>
                      
                        <tr>
                            <td colspan="2">
                                <div style="height: 3px;">
                                </div>
                            </td>
                        </tr>
                        <tr id="tblrowPhotos" runat="server" visible="false">
                            <td>Photos</td>
                            <td>
                                <asp:HyperLink ID="lnkPhotographs" runat="server" Text="View Photos " Target="_blank" ToolTip="Click to view the photos belonging to this run."></asp:HyperLink>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 2px;">
                                <a style="color: Blue; border-bottom-style: none; border-width: 0px;" href="#Legs">
                                    <img style="padding-left: 2px; border-width: 0px;" id="imgLegsPlanning" runat="server"
                                        alt="Go to Leg Planning" />
                                </a>
                            </td>
                            <td>
                                <asp:Panel runat="server" ID="pnlInstructionActual">
                                    <div style="float: left;">
                                        <asp:PlaceHolder ID="phInstructionAction" runat="server"></asp:PlaceHolder>
                                        <input type="button" value="Go" id="btnInstructionActionGo" runat="server" />&nbsp;
                                        <img id="imgInstructionInfo" runat="server" alt="" visible="false" />
                                    </div>

                                    <div style="float: left; padding: 4px 0 0 12px;">
                                        <asp:ListView ID="lvSignatures" runat="server" ItemType="Orchestrator.Repositories.DTOs.MWFSignature">
                                            <LayoutTemplate>
                                                <ul class="unformattedList">
                                                    <asp:PlaceHolder ID="itemPlaceholder" runat="server" />
                                                </ul>
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <li>
										            <a class="signatureLink" href="#" data-image-name="<%# Item.ImageName %>" data-signed-by="<%# Item.SignedBy %>" data-comment="<%# Item.Comment %>" data-instruction-complete-date-time="<%# Item.InstructionCompleteDateTime.HasValue ? Item.InstructionCompleteDateTime.Value.ToString("dd/MM/yy HH:mm") : string.Empty %>" data-latitude="<%# Item.Latitude %>" data-longitude="<%# Item.Longitude %>">
                                                        Signed
										            </a>
                                                </li>
                                            </ItemTemplate>
                                        </asp:ListView>
                                    </div>

                                    <div style="clear: both;"></div>
                                </asp:Panel>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top" style="padding-top: 2px;">
                    <table class="CollectDrop" width="100%" cellspacing="0">
                        <asp:Repeater ID="repOCD" runat="server" EnableViewState="False" DataSource="<%# ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionTypeId != (int)Orchestrator.eInstructionType.Trunk && ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionTypeId != (int)Orchestrator.eInstructionType.LeavePallets && ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionTypeId != (int)Orchestrator.eInstructionType.DeHirePallets && ((Orchestrator.Entities.Instruction) Container.DataItem).InstructionTypeId != (int)Orchestrator.eInstructionType.PickupPallets ? ((Orchestrator.Entities.Instruction) Container.DataItem).CollectDrops : null %>"
                            OnItemDataBound="repOrderCollectDrops_ItemDataBound">
                            <HeaderTemplate>
                                <thead>
                                    <tr class="DataGridListHeadSmall">
                                        <td style="width: 50px; border-left: 1px solid #000; border-bottom: 1px solid #000; display:none;">
                                            Action
                                        </td>
                                        <td style="width: 95px; border-bottom: 1px solid #000;">
                                            Customer Name
                                        </td>
                                        <td style="width: 60px; border-bottom: 1px solid #000;">
                                            Status
                                        </td>
                                        <td style="width: 75px; border-bottom: 1px solid #000;">
                                            Order Id
                                        </td>
                                        <td style="width: 190px; border-bottom: 1px solid #000;">
                                            <asp:label ID="lblLoadNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemLoadNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 190px; border-bottom: 1px solid #000;">
                                            <asp:label ID="lblDocketNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemDocketNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 70px; border-bottom: 1px solid #000;">
                                            Weight kg
                                        </td>
                                        <td style="width: 35px; border-bottom: 1px solid #000;">
                                            Cases
                                        </td>
                                        <td style="width: 40px; border-bottom: 1px solid #000;">
                                            Pallets
                                        </td>
                                        <td style="width: 50px; border-bottom: 1px solid #000;">
                                            Spaces
                                        </td>
                                        <td style="width: 70px; border-bottom: 1px solid #000;">
                                            Rate
                                        </td>
                                        <td style="width: 70px; border-bottom: 1px solid #000;" runat="server" id="trforeignRate">
                                            <asp:Label ID="lblRate" runat="server" Font-Size="10px" Text="Rate (£)" />
                                        </td>
                                        <td style="border-bottom: 1px solid #000;">
                                            Service Level
                                        </td>
                                        <td style="width: 80px; border-bottom: 1px solid #000;">
                                            Goods Type
                                        </td>
                                    </tr>
                                </thead>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td style="width: 50px; display:none;">
                                        <asp:Label ID="lblOrderAction" runat="server" />
                                    </td>
                                    <td style="width: 95px;">
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.CustomerName %>
                                    </td>
                                    <td style="width: 60px;">
                                        <asp:PlaceHolder runat="server" ID="phOrderStatus"></asp:PlaceHolder>
                                    </td>
                                    <td style="width: 75px;" nowrap>
                                        <span style="vertical-align: top;">
                                            <img style="padding-left: 2px;" runat="server" onmouseover="this.style.cursor='hand'"
                                                onmouseout="this.style.cursor='default'" alt="" align="middle" id="imgRemoveOrder"
                                                src="~/images/itxt_xButton.gif" />
                                            <a runat="server" id="hypOrderId" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                <%#((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderID.ToString()  %>
                                            </a>
                                            <img style="padding-left:2px;" class="ShowOrderNotesTooltip" runat="server" alt="" id="imgOrderCollectionDeliveryNotes" src="~/images/postit_small.gif" />
                                            <asp:Image ID="imgHasExtra" runat="server" ImageUrl="/images/ico_extra.png" AlternateText="There is an extra attached to this order" />
                                            
                                        </span>
                                    </td>
                                    <td style="width: 190px;">
                                        <a runat="server" id="hypLoadNumber" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.CustomerOrderNumber %>
                                        </a>
                                    </td>
                                    <td style="width: 190px;">
                                        <a runat="server" id="hypDocketNumber" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.DeliveryOrderNumber %>
                                        </a>
                                    </td>
                                    <td style="width: 70px;">
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Weight.ToString("F0") %>
                                        <asp:Label runat="server" ID="lblWeightActual"></asp:Label>
                                    </td>
                                    <td style="width: 35px;">
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoCases %>
                                        <asp:Label runat="server" ID="lblNoCasesActual"></asp:Label>
                                    </td>
                                    <td style="width: 50px;">
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoPallets %>
                                        <asp:Label runat="server" ID="lblPalletsActual"></asp:Label>
                                    </td>
                                    <td style="width: 50px;">
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.PalletSpaces %>
                                    </td>
                                    <td style="width: 70px;" runat="server" id="tdforeignRate">
                                        <asp:Label ID="lblRate" runat="server" />
                                    </td>
                                    <td style="width: 70px;">
                                        <asp:Label ID="lblGBPRate" runat="server" />
                                    </td>
                                    <td>
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderServiceLevel %>
                                    </td>
                                    <td style="width: 80px;">
                                        <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsTypeDescription %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <tr>
                                    <td style="width: 50px; display:none;">
                                    </td>
                                    <td style="width: 95px;">
                                    </td>
                                    <td style="width: 60px;">
                                    </td>
                                    <td style="width: 75px;" nowrap>
                                        <asp:Label runat="server" ID="lblOrderCount"></asp:Label>
                                    </td>
                                    <td style="width: 190px;">
                                    </td>
                                    <td style="width: 190px; text-align: right;">
                                        Totals:&nbsp;
                                    </td>
                                    <td style="width: 70px;">
                                        <asp:Label runat="server" ID="lblWeightTotal"></asp:Label>&nbsp;<asp:Label runat="server" ID="lblWeightActualTotal"></asp:Label>
                                    </td>
                                    <td style="width: 35px;">
                                        <asp:Label runat="server" ID="lblNoCasesTotal"></asp:Label>&nbsp;<asp:Label runat="server" ID="lblNoCasesActualTotal"></asp:Label>
                                    </td>
                                    <td style="width: 50px;">
                                        <asp:Label runat="server" ID="lblPalletsTotal"></asp:Label>&nbsp;<asp:Label runat="server" ID="lblPalletsActualTotal"></asp:Label>
                                    </td>
                                    <td style="width: 50px;">
                                        <asp:Label runat="server" ID="lblPalletSpacesTotal"></asp:Label>&nbsp;
                                    </td>
                                    <td style="width: 70px;" runat="server"
                                        id="frforeignRate">
                                        &nbsp;
                                    </td>
                                    <td style="width: 70px;">
                                        &nbsp;
                                    </td>
                                    <td style="width: 80px;">
                                    </td>
                                </tr>
                            </FooterTemplate>
                        </asp:Repeater>
                    </table>
                    <asp:Repeater ID="repTurnedAwayOCD" runat="server" EnableViewState="False" OnItemDataBound="repTurnedAwayOrderCollectDrops_ItemDataBound"
                        OnItemCommand="repTurnedAwayOrderCollectDrops_ItemCommand">
                        <HeaderTemplate>
                            <span style="font-weight: bold; font-size: smaller">Refusals</span><br />
                            <table class="CollectDrop" width="100%" cellspacing="0">
                                <thead>
                                    <tr class="DataGridListHeadSmall">
                                        <td style="width: 50px;">
                                            <asp:ImageButton ID="imgBtnRemoveAttemptLater" runat="server" ImageUrl="~/images/itxt_xButton.gif"
                                                CausesValidation="false" CommandName="RemoveAttemptLater"  ToolTip="Remove this attempted delivery" />
                                        </td>
                                        <td style="width: 95px;">
                                            Customer Name
                                        </td>
                                        <td style="width: 60px;">
                                            Status
                                        </td>
                                        <td style="width: 75px;">
                                            Order Id
                                        </td>
                                        <td style="width: 190px;">
                                            <asp:label ID="lblLoadNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemLoadNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 190px;">
                                            <asp:label ID="lblDocketNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemDocketNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 70px;">
                                            Weight kg
                                        </td>
                                        <td style="width: 35px;">
                                            Cases
                                        </td>
                                        <td style="width: 50px;">
                                            Pallets
                                        </td>
                                        <td style="width: 50px;">
                                            Spaces
                                        </td>
                                        <td style="width: 70px;" runat="server" id="thforeignRate">
                                            Rate
                                        </td>
                                        <td style="width: 70px;">
                                            Rate (£)
                                        </td>
                                        <td style="width: 80px;">
                                            Reason
                                        </td>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style="width: 50px;">
                                </td>
                                <td style="width: 95px;">
                                    <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.CustomerName%>
                                </td>
                                <td style="width: 60px;">
                                    <asp:PlaceHolder runat="server" ID="phOrderStatus"></asp:PlaceHolder>
                                </td>
                                <td style="width: 75px;" nowrap>
                                    <span style="vertical-align: top;">
                                    <a runat="server" id="hypOrderId" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                        <%#((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.OrderID.ToString()%>
                                    </a></span>
                                </td>
                                <td style="width: 190px;">
                                    <a runat="server" id="hypLoadNumber" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                        <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.CustomerOrderNumber%>
                                    </a>
                                </td>
                                <td style="width: 190px;">
                                    <a runat="server" id="hypDocketNumber" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                        <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.DeliveryOrderNumber%>
                                    </a>
                                </td>
                                <td style="width: 70px;">
                                    <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.Weight.ToString("F0")%>
                                </td>
                                <td style="width: 35px;">
                                    <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.Cases%>
                                </td>
                                <td style="width: 50px;">
                                    <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.NoPallets%>
                                </td>
                                <td style="width: 50px;">
                                    <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).Order.PalletSpaces%>
                                </td>
                                <td style="width: 70px;" runat="server" id="tdforeignRate">
                                    &nbsp;
                                </td>
                                <td style="width: 70px;">
                                    &nbsp;
                                </td>
                                <td style="width: 80px;">
                                    <%# ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).CustomReason != string.Empty ? string.Format("{0}, {1}", ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).RedeliveryReasonText, ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).CustomReason) : ((Orchestrator.Entities.RedeliveryOrder)Container.DataItem).RedeliveryReasonText%>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            <tr>
                                <td style="width: 50px;">
                                </td>
                                <td style="width: 95px;">
                                </td>
                                <td style="width: 60px;">
                                </td>
                                <td style="width: 75px; border-top-style: solid; border-top-width: 2px;" nowrap>
                                    <asp:Label runat="server" ID="lblOrderCount"></asp:Label>
                                </td>
                                <td style="width: 190px;">
                                </td>
                                <td style="width: 190px; text-align: right; border-top-style: solid; border-top-width: 2px;">
                                    Totals:&nbsp;
                                </td>
                                <td style="width: 70px; border-top-style: solid; border-top-width: 2px;">
                                    <asp:Label runat="server" ID="lblWeightTotal"></asp:Label>
                                </td>
                                <td style="width: 35px; border-top-style: solid; border-top-width: 2px;">
                                    <asp:Label runat="server" ID="lblNoCasesTotal"></asp:Label>
                                </td>
                                <td style="width: 50px; border-top-style: solid; border-top-width: 2px;">
                                    <asp:Label runat="server" ID="lblPalletsTotal"></asp:Label>
                                </td>
                                <td style="width: 50px; border-top-style: solid; border-top-width: 2px;">
                                    <asp:Label runat="server" ID="lblPalletSpacesTotal"></asp:Label>
                                </td>
                                <td style="width: 70px; border-top-style: solid; border-top-width: 2px;" runat="server"
                                    id="frforeignRate">
                                    &nbsp;
                                </td>
                                <td style="width: 70px; border-top-style: solid; border-top-width: 2px;">
                                    &nbsp;
                                </td>
                                <td style="width: 80px; border-top-style: solid; border-top-width: 2px;">
                                </td>
                            </tr>
                            </tbody></table>
                        </FooterTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr id="rowPalletHandling" runat="server" visible="false">
                <td class="InstructionBoxContainerLeft" style="width: 350px; vertical-align: top;">
                    <table width="100%" cellspacing="0">
                        <tr>
                            <td>
                                <table class="InstructionPoint" cellpadding="3px" cellspacing="0px" width="100%">
                                    <tr>
                                        <td style="width: 55px; padding-left: 5px;">
                                            <img alt="Pallets" id="imgPalletHandling" runat="server" src="~/images/trunk.gif" align="middle" />
                                        </td>
                                        <td>
                                            <asp:Label ID="lblPalletHandling" runat="server" Font-Bold="true"></asp:Label><br />
                                            <span id="Span1" onclick="" class="orchestratorLink" onmouseover="javascript:ShowPointToolTip(this,<%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.PointId %>);"
                                                onmouseout="closeToolTip();">
                                                <%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.OrganisationName %>
                                                <br />
                                                <%# ((Orchestrator.Entities.Instruction) Container.DataItem).Point.Description %>
                                            </span>
                                        </td>
                                        <td align="right"></td>
                                        <td align="right" >
                                            <asp:Button ID="btnPalletHandling" Text="" CssClass="buttonClassPalletHandling" runat="server" ToolTip="Pallet Handling" />
                                            <asp:Button ID="btnPalletCallIn" Text="" CssClass="buttonClassCallIn-Remove" runat="server" ></asp:Button>
                                        </td>
                                    </tr>
                                </table>
                                <table class="InstructionLeft">
                                    <tr>
                                        <td style="padding-left: 2px;">Pallets On:</td>
                                        <td><asp:Label ID="lblphOn" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 62px; padding-left: 2px;">Booked:</td>
                                        <td><asp:Label ID="lblphBookedDateTime" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td style="width: 62px; padding-left: 2px;">Planned:</td>
                                        <td><asp:Label ID="lblphPlannedDateTime" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr id="phArrivalDateTime" runat="server" visible="false">
                                        <td style="padding-left: 2px;">Arrival:</td>
                                        <td><asp:Label ID="lblphArrivalDateTime" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr id="phDepartureDateTime" runat="server" visible="false">
                                        <td style="width: 62px; padding-left: 2px;">Departure:</td>
                                        <td><asp:Label ID="lblphDepartureDatetime" runat="server"></asp:Label></td>
                                    </tr>
                                    <tr id="phCallIn" runat="server" visible="false">
                                        <td style="padding-left: 2px;">Called-in:</td>
                                        <td><asp:Label ID="lblphCallIn" runat="server" ></asp:Label></td>
                                    </tr>
                                    <tr>
                                        <td colspan="2"><div style="height: 3px;"></div></td>
                                    </tr>
                                    <tr>
                                        <td style="padding-left: 2px;">
                                            <a style="color: Blue; border-bottom-style: none; border-width: 0px;" href="#Legs">
                                                <img style="padding-left: 2px; border-width: 0px;" id="imgphLegsPlanning" runat="server" alt="Go to Leg Planning" />
                                            </a>
                                        </td>
                                        <td>
                                            <asp:Panel runat="server" ID="pnlphInstructionActual">
                                                <asp:PlaceHolder ID="phPalletHandlingInstructionAction" runat="server"></asp:PlaceHolder>
                                                <input type="button" value="Go" id="btnphInstructionActionGo" runat="server" />&nbsp;
                                                <img id="imgphInstructionInfo" runat="server" alt="" visible="false" /></asp:Panel>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table class="CollectDrop" width="100%" cellspacing="0">
                        <asp:Repeater ID="repPalletHandling" runat="server" EnableViewState="false" >
                            <HeaderTemplate>
                                <thead>
                                    <tr class="DataGridListHeadSmall">
                                        <td style="width: 50px; display:none;">
                                            Action
                                        </td>
                                        <td style="width: 95px;">
                                            Customer Name
                                        </td>
                                        <td style="width: 60px;">
                                            Status
                                        </td>
                                        <td style="width: 75px;">
                                            Order Id
                                        </td>
                                        <td style="width: 190px;">
                                            <asp:label ID="lblLoadNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemLoadNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 190px;">
                                            <asp:label ID="lblDocketNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemDocketNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 50px;">
                                            Pallets
                                        </td>
                                        <td style="width: 70px;">
                                            Rate
                                        </td>
                                        <td style="width: 70px;" runat="server" id="trforeignRate">
                                            <asp:Label ID="lblRate" runat="server" Text="Rate (£)" />
                                        </td>
                                        <td id="thDehireReceipt" runat="server" visible="false">De-Hire Reciept Number</td>
                                    </tr>
                                </thead>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <tr>
                                        <td style="width: 50px; display:none;">
                                            <asp:Label ID="lblOrderAction" runat="server" />
                                        </td>
                                        <td style="width: 95px;">
                                            <%# ((CollectDropsWithActuals)Container.DataItem).CollectDrop.Order.CustomerName%>
                                        </td>
                                        <td style="width: 60px;">
                                            <asp:PlaceHolder runat="server" ID="phOrderStatus"></asp:PlaceHolder>
                                        </td>
                                        <td style="width: 75px;" nowrap>
                                            <span style="vertical-align: top;">
                                                <img style="padding-left: 2px;" runat="server" onmouseover="this.style.cursor='hand'"
                                                    onmouseout="this.style.cursor='default'" alt="" align="middle" id="imgRemoveOrder"
                                                    src="~/images/itxt_xButton.gif" />
                                                <a onclick="javascript:viewOrder(<%#((CollectDropsWithActuals) Container.DataItem).CollectDrop.Order.OrderID.ToString() %>);"
                                                    target="_blank" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                    <%#((CollectDropsWithActuals) Container.DataItem).CollectDrop.Order.OrderID.ToString()  %>
                                                </a>
                                            </span>
                                        </td>
                                        <td style="width: 190px;">
                                            <a onclick="javascript:viewOrder(<%#((CollectDropsWithActuals) Container.DataItem).CollectDrop.Order.OrderID.ToString()  %>);"
                                                target="_blank" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                <%# ((CollectDropsWithActuals)Container.DataItem).CollectDrop.Order.CustomerOrderNumber%>
                                            </a>
                                        </td>
                                        <td style="width: 190px;">
                                            <a onclick="javascript:viewOrder(<%#((CollectDropsWithActuals) Container.DataItem).CollectDrop.Order.OrderID.ToString()  %>);"
                                                target="_blank" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                <%# ((CollectDropsWithActuals)Container.DataItem).CollectDrop.Order.DeliveryOrderNumber%>
                                            </a>
                                        </td>
                                        <td style="width: 50px;">
                                            <%# ((CollectDropsWithActuals)Container.DataItem).CollectDrop.NoPallets%>
                                            <asp:Label runat="server" ID="lblPalletsActual"></asp:Label>
                                        </td>
                                        <td style="width: 70px;" runat="server" id="tdforeignRate">
                                            <asp:Label ID="lblRate" runat="server" />
                                        </td>
                                        <td style="width: 70px;">
                                            <asp:Label ID="lblGBPRate" runat="server" />
                                        </td>
                                        <td id="tdDehireReceipt" runat="server" visible="false">
                                            <asp:Label ID="lblDehireReceipt" runat="server" Text="" />
                                        </td>
                                    </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                    <tr>
                                        <td style="width: 50px; display:none;"></td>
                                        <td style="width: 95px;"></td>
                                        <td style="width: 60px;"></td>
                                        <td style="width: 75px;">
                                            <asp:Label runat="server" ID="lblOrderCount"></asp:Label>
                                        </td>
                                        <td style="width: 190px;"></td>
                                        <td style="width: 190px;">
                                            Totals:&nbsp;
                                        </td>
                                        <td style="width: 50px;">
                                            <asp:Label runat="server" ID="lblPalletsTotal"></asp:Label>
                                        </td>
                                        <td style="width: 70px;" runat="server" id="frforeignRate">
                                            &nbsp;
                                        </td>
                                        <td style="width: 70px;">
                                            &nbsp;
                                        </td>
                                        <td id="ftDehireReciept" runat="server" visible="false"></td>
                                    </tr>
                            </FooterTemplate>
                        </asp:Repeater>
                    </table>
                </td>
            </tr>
            <tr runat="server" id="rowTrunkSeparator" visible="false">
                <td colspan="2">
                </td>
            </tr>
            <tr runat="server" id="rowTrunk" visible="false">
                <td  colspan="1">
                    <table class="InstructionPoint" cellpadding="3px" cellspacing="0px" width="100%">
                        <tr>
                            <td style="width: 55px; padding-left: 5px;">
                                <img alt="Trunk" id="Img1" runat="server" src="/images/trunk.gif" align="middle" />
                            </td>
                            <td style="padding-left: 2px; width: 168px;">
                                <b><asp:Label ID="lblInstructionAction" runat="server" Text="" /></b><br />
                                <span id="spnTrunkPoint" runat="server" onclick="" class="orchestratorLink" onmouseout="closeToolTip();">
                                    <asp:Label runat="server" ID="lblTrunkPointOrganisation"></asp:Label>
                                    <br />
                                    <asp:Label runat="server" ID="lblTrunkPointDescription"></asp:Label>
                                </span>
                            </td>
                            <td align="right">
                                <img runat="server" src="~/images/newMasterPage/icon-cross.png" alt="Remove trunk" id="imgRemoveTrunk" />
                                <img runat="server" src="~/images/newMasterPage/icon-updatetrunk.gif" alt="Update trunk" id="imgUpdateTrunkPoint" />
                            </td>
                            <td align="right" >
                                <asp:Button ID="btnTipSheet" Text="" CssClass="buttonClassTipSheet" runat="server" OnClick="btnTipSheet_Click" ToolTip="Tip Sheet" />
                                <asp:Button ID="btnTrunkCallIn" Text="" CssClass="buttonClassCallIn-Remove"  runat="server" ></asp:Button>
                                <asp:Button ID="btnTrunkAddOrder" visible="false" runat="server" Text="" CssClass="buttonClassAdd" ToolTip="Add order" /> 
                                <asp:Button ID="btnConvertTrunk" visible="false" runat="server" Text="" CssClass="buttonClassConvert" ToolTip="Convert instruction" />
                            </td>
                        </tr>
                    </table>
                    <table class="InstructionLeft">
                        <tr id="trunkPalletsOn" runat="server">
                            <td style="padding-left: 2px;">
                                Pallets On:
                            </td>
                            <td>
                                <asp:Label ID="lblTrunkPalletsOn" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="min-width: 62px; padding-left: 2px;">
                                Booked:
                            </td>
                            <td>
                                <asp:Label ID="lblTrunkBookedDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 2px;">
                                Planned:
                            </td>
                            <td>
                                <asp:Label ID="lblTrunkPlannedDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="trTrunkToMwfStatus" runat="server" visible="false">
                            <td style="padding-left: 2px;">
                                MWF Status:
                            </td>
                            <td>
                                <asp:Label ID="lblTrunkToMwfStatus" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td style="padding-left: 2px;">
                                Leave:
                            </td>
                            <td>
                                <asp:Label ID="lblTrunkDepartureDateTime" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr id="trTrunkProceedFromMwfStatus" runat="server" visible="false">
                            <td style="padding-left: 2px;">
                                MWF Status:
                            </td>
                            <td>
                                <asp:Label ID="lblTrunkProceedFromMwfStatus" runat="server" /> (proceed-from)
                            </td>
                        </tr>
                    </table>
                </td>
                <td valign="top">
                    <table class="CollectDrop" width="100%" cellspacing="0">
                        <asp:Repeater id="repTrunkOrders" runat="server" OnItemDataBound="repOrderCollectDrops_ItemDataBound">
                            <HeaderTemplate>
                                <thead>
                                    <tr class="DataGridListHeadSmall">
                                        <td style="width: 50px; display:none;">
                                            Action
                                        </td>
                                        <td style="width: 95px;">
                                            Customer Name
                                        </td>
                                        <td style="width: 60px;">
                                            Status
                                        </td>
                                        <td style="width: 75px;">
                                            Order Id
                                        </td>
                                        <td style="width: 200px;">
                                            <asp:label ID="lblLoadNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemLoadNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 200px;">
                                            <asp:label ID="lblDocketNumber" runat="server" Text="<%# Orchestrator.Globals.Configuration.SystemDocketNumberText %>"></asp:label>
                                        </td>
                                        <td style="width: 70px;">
                                            Weight kg
                                        </td>
                                        <td style="width: 35px;">
                                            Cases
                                        </td>
                                        <td style="width: 50px;">
                                            Pallets
                                        </td>
                                        <td style="width: 50px;">
                                            Spaces
                                        </td>
                                        <td style="width: 70px;">
                                            Rate
                                        </td>
                                        <td style="width: 70px;" runat="server" id="trforeignRate">
                                            <asp:Label ID="lblRate" runat="server" Text="Rate (£)" />
                                        </td>
                                        <td style="width: 80px;">
                                            Goods Type
                                        </td>
                                    </tr>
                                </thead>
                            </HeaderTemplate>
                            <ItemTemplate>
                                    <tr>
                                        <td style="width: 50px; display:none;">
                                            <asp:Label ID="lblOrderAction" runat="server" />
                                        </td>
                                        <td style="width: 95px;">
                                             <%# ((Orchestrator.Entities.CollectDrop)Container.DataItem).Order.CustomerName %>
                                        </td>
                                        <td style="width: 60px;">
                                            <asp:PlaceHolder runat="server" ID="phOrderStatus"></asp:PlaceHolder>
                                        </td>
                                        <td style="width: 75px;" nowrap>
                                            <span style="vertical-align: top;">
                                                <img style="padding-left: 2px;" runat="server" onmouseover="this.style.cursor='hand'"
                                                    onmouseout="this.style.cursor='default'" alt="" align="middle" id="imgRemoveOrder"
                                                    src="~/images/itxt_xButton.gif" />
                                                <a runat="server" id="hypOrderId" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                    <%#((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.OrderID.ToString()  %>
                                                </a></span>
                                                <asp:Image ID="imgHasExtra" runat="server" ImageUrl="/images/ico_extra.png" AlternateText="There is an extra attached to this order" />
                                        </td>
                                        <td style="width: 200px;">
                                            <a runat="server" id="hypLoadNumber" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.CustomerOrderNumber %>
                                            </a>
                                        </td>
                                        <td style="width: 200px;">
                                            <a runat="server" id="hypDocketNumber" onmouseover="this.style.cursor='hand'" onmouseout="this.style.cursor='default'">
                                                <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.DeliveryOrderNumber %>
                                            </a>
                                        </td>
                                        <td style="width: 70px;">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Weight.ToString("F0") %>
                                            <asp:Label runat="server" ID="lblWeightActual"></asp:Label>
                                        </td>
                                        <td style="width: 35px;">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoCases %>
                                            <asp:Label runat="server" ID="lblNoCasesActual"></asp:Label>
                                        </td>
                                        <td style="width: 50px;">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).NoPallets %>
                                            <asp:Label runat="server" ID="lblPalletsActual"></asp:Label>
                                        </td>
                                        <td style="width: 50px;">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).Order.PalletSpaces %>
                                        </td>
                                        <td style="width: 70px;" runat="server" id="tdforeignRate">
                                            <asp:Label ID="lblRate" runat="server" />
                                        </td>
                                        <td style="width: 70px;">
                                            <asp:Label ID="lblGBPRate" runat="server" />
                                        </td>
                                        <td style="width: 80px;">
                                            <%# ((Orchestrator.Entities.CollectDrop) Container.DataItem).GoodsTypeDescription %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                                <FooterTemplate>
                                    <tr>
                                        <td style="width: 50px; display:none;">
                                        </td>
                                        <td style="width: 95px;">
                                        </td>
                                        <td style="width: 60px;">
                                        </td>
                                        <td style="width: 75px; border-top-style: solid; border-top-width: 2px;" nowrap>
                                            <asp:Label runat="server" ID="lblOrderCount"></asp:Label>
                                            
                                        </td>
                                        <td style="width: 200px;">
                                        </td>
                                        <td style="width: 200px; text-align: right; border-top-style: solid; border-top-width: 2px;">
                                            Totals:&nbsp;
                                        </td>
                                        <td style="width: 70px; border-top-style: solid; border-top-width: 2px;">
                                            <asp:Label runat="server" ID="lblWeightTotal"></asp:Label>&nbsp;<asp:Label runat="server"
                                                ID="lblWeightActualTotal"></asp:Label>
                                        </td>
                                        <td style="width: 35px; border-top-style: solid; border-top-width: 2px;">
                                            <asp:Label runat="server" ID="lblNoCasesTotal"></asp:Label>&nbsp;<asp:Label runat="server" ID="lblNoCasesActualTotal"></asp:Label>
                                        </td>
                                        <td style="width: 50px; border-top-style: solid; border-top-width: 2px;">
                                            <asp:Label runat="server" ID="lblPalletsTotal"></asp:Label>&nbsp;<asp:Label runat="server" ID="lblPalletsActualTotal"></asp:Label>
                                        </td>
                                        <td style="width: 50px; border-top-style: solid; border-top-width: 2px;">
                                            <asp:Label runat="server" ID="lblPalletSpacesTotal"></asp:Label>&nbsp;
                                        </td>
                                        <td style="width: 70px; border-top-style: solid; border-top-width: 2px;" runat="server"
                                            id="frforeignRate">
                                            &nbsp;
                                        </td>
                                        <td style="width: 70px; border-top-style: solid; border-top-width: 2px;">
                                            &nbsp;
                                        </td>
                                        <td style="width: 80px; border-top-style: solid; border-top-width: 2px;">
                                        </td>
                                    </tr>
                            </FooterTemplate>
                         </asp:Repeater>    
                    </table>  
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div style="height: 1px; border-top: 1px dotted #CCC; margin-bottom: 4px;">
                    </div>
                </td>
            </tr>
            <tr runat="server" id="rowTrunkSpacer" visible="false">
                <td colspan="2">
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </tbody> </table>
        </FooterTemplate>
    </asp:Repeater>

    <telerik:RadToolTip runat="server" ID="radToolTip" EnableShadow="true" HideEvent="LeaveTargetAndToolTip"
        ShowEvent="FromCode" Width="500px" RelativeTo="Element" Position="MiddleLeft" MouseTrailing="true"
        ShowCallout="false">
    </telerik:RadToolTip>

    <script type="text/javascript">
        $(function () {
            $('.signatureLink').on('click', signatureLink_click);        
        });

        function signatureLink_click() {
            var tooltip = $find('<%= radToolTip.ClientID %>');
	        tooltip.set_targetControl(this);

	        var data = $(this).data();
	        data.imageBaseUrl = '<%= Orchestrator.WebUI.Utilities.GetSignatureImageBaseUri().AbsoluteUri %>';

	        if (data.comment == 'Signed') {
	            data.comment = '';
	        }

	        var compiledTemplate = getTemplate('signature');
	        var html = compiledTemplate(data);
            
	        tooltip.set_content(html);
	        tooltip.show();
        }

    </script>
</asp:Panel>
