<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/WizardMasterPage.master" Inherits="Orchestrator.WebUI.Traffic.Trunk" Codebehind="trunk.aspx.cs" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server"><asp:Label ID="lblTrunk" runat="server" Text="Trunk Leg" /></asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">

    <script language="javascript" type="text/javascript">

  
    </script>
    
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <uc1:infringementDisplay ID="infringementDisplay" Visible="false" runat="server" />

    <div style="height: 250; width: 100%; overflow: auto; margin-bottom:15px;">
        <table width="99%">
            <tr>
                <td>Trunk To (Point)</td>
                <td>
                    <p1:Point runat="server" ID="ucPoint" ShowFullAddress="true" PointSelectionRequired="true" CanCreateNewPoint="false" CanClearPoint="true"
                        CanUpdatePoint="false" ShowPointOwner="true" Visible="true" IsDepotVisible="false" />
                </td>
            </tr>
            <tr id="trResources" runat="server">
                <td nowrap="nowrap">Use these resources</td>
                <td colspan="2">
                    <asp:CheckBox ID="chkDriver" runat="server" Text="Driver" /> &nbsp; <asp:CheckBox ID="chkVehicle" runat="server" Text="Vehicle" />
                </td>
            </tr>
            <tr>
                <td>When you should <b>arrive</b> at trunk point</td>
                <td colspan="2">
                    <telerik:RadDateInput ID="rdiStartDate" runat="server" DateFormat="dd/MM/yyyy" />
                    <telerik:RadDateInput ID="rdiStartTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm" />
                    <asp:RequiredFieldValidator ID="rfvStartDate" runat="server" ControlToValidate="rdiStartDate" Display="Dynamic" />
                    <asp:RequiredFieldValidator ID="rfvStartTime" runat="server" ControlToValidate="rdiStartTime" Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td>When you should <b>leave</b> at trunk point</td>
                <td colspan="2">
                    <telerik:RadDateInput ID="rdiEndDate" runat="server" DateFormat="dd/MM/yyyy" />
                    <telerik:RadDateInput ID="rdiEndTime" runat="server" DateFormat="HH:mm" DisplayDateFormat="HH:mm"/>
                    <asp:RequiredFieldValidator ID="rfvEndDate" runat="server" ControlToValidate="rdiEndDate" Display="Dynamic" />
                    <asp:RequiredFieldValidator ID="rfvEndTime" runat="server" ControlToValidate="rdiEndTime" Display="Dynamic" />
                </td>
            </tr>
        </table>
    </div>

    <div class="buttonbar" style="margin-top:auto;">
        <asp:Button ID="btnTrunk" runat="server" Text="Confirm" Width="75"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False" />
    </div>
    
</asp:Content>