<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MultiTrunk.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.MultiTrunk" %>
<table>
    <tr>
        <td>
            <table style="background-color: White;" width="590px">
                <tr>
                    <td style="width: 200px;">
                        <b>Name
                    </td>
                    <td style="width: 250px;">
                        <b>Points</b>
                    </td>
                    <td style="width: 140px;">
                        <b>Travel Time From Previous Point</b>
                    </td>
                </tr>
            </table>
            <telerik:RadComboBox ID="rcbMultiTrunk" runat="server" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="false"
                AutoPostBack="true" ShowMoreResultsBox="false" Width="660px" Height="300px" Overlay="true" AllowCustomText="false"
                HighlightTemplatedItems="true">
                <WebServiceSettings Path="/ws/combostreamers.asmx" Method="GetMultiTrunk" />
            </telerik:RadComboBox>
            <asp:RequiredFieldValidator ID="rfvMultiTrunk" runat="server" Display="dynamic" ControlToValidate="rcbMultiTrunk"
                ErrorMessage="Please select a multi-trunk.">
                <img id="imgErr" src="~/images/error.png" runat="server" title="Please enter the point description."></asp:RequiredFieldValidator>
        </td>
    </tr>
</table>
<asp:Panel ID="pnlMultiTrunk" runat="server" Visible="false">
    <div style="height: 110px; border-bottom: solid 1pt silver; padding: 2px; color: black;
        background-color: white; width: 675px; text-align: left; font-size: 12px">
        <asp:Label ID="lblMultiTrunk" runat="server" Visible="True" Text="Multi-Trunk Points" />
        <br />
    </div>
</asp:Panel>
<table runat="server" visible="false" id="tblArriveDepart">
    <tr>
        <td>
            <table>
                <tr>
                    <td nowrap="nowrap">
                        Use these resources
                    </td>
                    <td colspan="2">
                        <asp:CheckBox ID="chkDriver" runat="server" Text="Driver" />
                        &nbsp;
                        <asp:CheckBox ID="chkVehicle" runat="server" Text="Vehicle" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>
            <table>
                <tr>
                    <td valign="top">
                        When do you expect to <b>arrive</b> at
                        <asp:Label ForeColor="Red" runat="server" ID="lblArrivalPoint"></asp:Label>&nbsp;&nbsp;
                    </td>
                    <td valign="top">
                        <telerik:RadDatePicker ID="dteArrival" runat="server" Width="100" />
                        <asp:RequiredFieldValidator ID="rfvArrivalDate" runat="server" ControlToValidate="dteArrival" Display="Dynamic" />
                    </td>
                    <td valign="top">
                        <telerik:RadTimePicker ID="tmeArrival" runat="server" Width="65" />
                        <asp:RequiredFieldValidator ID="rfvArrivalTime" runat="server" ControlToValidate="tmeArrival" Display="Dynamic" />
                    </td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<input type="hidden" runat="server" id="hidMultiTrunkId" />

<script type="text/javascript">
<!--   
    function <%=this.ClientID %>_MultiTrunkRequesting(sender, eventArgs)
    {
        var multiTrunkCombo = $find("<%=rcbMultiTrunk.ClientID %>");

        var searchString = "";
        if(sender.get_text() != "")
            searchString = sender.get_text();
            
        var context = eventArgs.get_context();
        context["FilterString"] = searchString;
    }   
//-->
</script>

