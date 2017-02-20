<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.Traffic.linkJob" Codebehind="linkJob.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" %>

<%@ Register TagPrefix="cs" Namespace="Codesummit" Assembly="WebModalControls" %>
<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Link Job</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/popAddress.js" type="text/javascript"></script>


    <div style="height: 260px; width: 100%; overflow: auto;">
        <asp:Panel id="pnlLinkJobs" runat="server">
            <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
            <table border="0" >
                <tr>
                    <td colspan="6"><asp:CheckBox ID="chkLinkDriver" runat="server" Text="Link Driver" />&nbsp;<asp:CheckBox ID="chkLinkVehicle" runat="server" Text="Link Vehicle" />&nbsp;<asp:CheckBox ID="chkLinkTrailer" runat="server" Text="Link Trailer" /></td>
                </tr>
                <tr>
                    <td colspan="6"><hr noshade /></td>
                </tr>
                <tr>
                    <td colspan="6"><asp:CheckBox ID="chkCheckAll" runat="server" Text="Check All" AutoPostBack="true" /></td>
                </tr>
                <asp:Repeater ID="repLegs" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td valign="top" width="5%">
                                <asp:CheckBox ID="chkThisLeg" runat="server" />
                            </td>
                            <td valign="top" width="20%">
                                <div onMouseOver="javascript:ShowPoint('~/point/getPointAddresshtml.aspx', <%# ((Orchestrator.Entities.LegView) Container.DataItem).StartLegPoint.Point.PointId.ToString() %>);" onMouseOut="javascript:HidePoint();"><b><%# ((Orchestrator.Entities.LegView) Container.DataItem).StartLegPoint.Point.Description %></b></div>
                                <input type="hidden" id="hidInstructionId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "InstructionId" ) %>' />
                            </td>
                            <td valign="top" width="20%">
                                <div onMouseOver="javascript:ShowPoint('~/point/getPointAddresshtml.aspx', <%# ((Orchestrator.Entities.LegView) Container.DataItem).EndLegPoint.Point.PointId.ToString() %>);" onMouseOut="javascript:HidePoint();"><b><%# ((Orchestrator.Entities.LegView)Container.DataItem).EndLegPoint.Point.Description %></b></div>
                            </td>
                            <td valign="top" width="20%">
                                <%# ((Orchestrator.Entities.LegView) Container.DataItem).Driver != null ? ((Orchestrator.Entities.LegView) Container.DataItem).Driver.Individual.FullName : string.Empty %>
                            </td>
                            <td valign="top" width="20%">
                                <%# ((Orchestrator.Entities.LegView) Container.DataItem).Vehicle != null ? ((Orchestrator.Entities.LegView) Container.DataItem).Vehicle.RegNo: string.Empty %>
                            </td>
                            <td valign="top" width="15%">
                                <%# ((Orchestrator.Entities.LegView) Container.DataItem).Trailer != null ? ((Orchestrator.Entities.LegView) Container.DataItem).Trailer.TrailerRef: string.Empty %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr>
                    <td colspan="5">
                        You can use this page to link resources from a job coming in to one of the jobs on your traffic sheet.  Select the resources to link with and then select the legs those resources will be used on.  They will be added to the job once the planner sending them into your area has communicated with them.
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlError" runat="server" visible="false">
            <div class="MessagePanel">
                <asp:Image ID="imgIcon" runat="server" ImageUrl="~/images/ico_info.gif" />
                <asp:Label ID="lblMessage" runat="server">Please make sure you have selected a job to link with this job, or that the job has some existing links to remove.</asp:Label>
            </div>
        </asp:Panel>
    </div>
    <cs:WebModalWindowHelper ID="mwhelper" runat="server" ShowVersion="false">
    </cs:WebModalWindowHelper>
            
    <div class="buttonbar">
        <asp:Button ID="btnConfirm" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
    
    <div id="divPointAddress" style="z-index=5;display:none; background-color:Wheat;padding:2px 2px 2px 2px;">
	    <table style="background-color: white; border:solid 1pt black; " cellpadding="2">
		    <tr>
			    <td><span id="spnPointAddress"></span></td>
		    </tr>
	    </table>
    </div>
    
</asp:Content>