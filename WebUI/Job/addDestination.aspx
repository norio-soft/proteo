<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/WizardMasterPage.Master" Inherits="Orchestrator.WebUI.Job.addDestination" Codebehind="addDestination.aspx.cs" Title="Haulier Enterprise" %>

<%@ Register TagPrefix="uc1" TagName="infringementDisplay" Src="~/UserControls/BusinessRuleInfringementDisplay.ascx" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Add Destination</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div style="height: 250px; width: 100%; overflow: auto;">
        <uc1:infringementDisplay ID="infrigementDisplay" Visible="false" runat="server" />
        <table width="99%">
            <tr>
                <td>
                    <asp:Label ID="lblMessage" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Panel ID="pnlAddDestination" runat="server">
            <table width="99%">
                <tr>
                    <td>
                        Proceed To (Point)</td>
                    <td>
                        <p1:Point runat="server" ID="ucPoint" ShowFullAddress="true" PointSelectionRequired="true" CanCreateNewPoint="false" CanClearPoint="true"
                            CanUpdatePoint="false" ShowPointOwner="true" Visible="true" IsDepotVisible="false" />           
                    </td>
                </tr>
                <tr>
                    <td nowrap="nowrap">
                        Use these resources</td>
                    <td colspan="2">
                        <asp:CheckBox ID="chkDriver" runat="server" Text="Driver" /> &nbsp; <asp:CheckBox ID="chkVehicle" runat="server" Text="Vehicle" /> &nbsp; <asp:CheckBox ID="chkTrailer" runat="server" Text="Trailer" />
                    </td>
                </tr>
                <tr>
                    <td>When you should <b>arrive</b> at the selected point</td>
                    <td colspan="2">
                        <telerik:RadDateInput ID="rdiArrivalDate" runat="server" DateFormat="dd/MM/yy" Width="100px" ></telerik:RadDateInput> (dd/mm/yy)
                        <asp:RequiredFieldValidator ID="rfvArrivalDate" runat="server" ControlToValidate="rdiArrivalDate" Display="dynamic" ErrorMessage="<img src='../images/error.png' title='Please enter the date you will arrive at the specified point.' />"></asp:RequiredFieldValidator>
                        <br />
                        <telerik:RadDateInput ID="rdiArrivalTime" runat="server" DateFormat="HH:mm" Width="50px" ></telerik:RadDateInput> (hh:mm)
                        <asp:RequiredFieldValidator ID="rfvtimeArrivalDate" runat="server" ControlToValidate="rdiArrivalTime" Display="dynamic" ErrorMessage="<img src='../images/error.png' title='Please enter the time you will arrive at the specified point.' />"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td colspan="3" height="50">&nbsp;</td>
                </tr>
            </table>
        </asp:Panel>
    </div>

    <div class="buttonbar">
        <asp:Button ID="btnAddDestination" runat="server" Text="Confirm" Width="75" OnClientClick="return checkDate()"></asp:Button>
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="False"  />
    </div>
    <script src="//cdnjs.cloudflare.com/ajax/libs/moment.js/2.0.0/moment.min.js"></script>
    <script>

        function checkDate() {
            var datePicker = $find('<%=rdiArrivalDate.ClientID%>');
            var timePicker = $find('<%=rdiArrivalTime.ClientID%>');
            var retVal = true;
            if (datePicker.get_selectedDate() != null)
            {
                var dateToCheck = moment(datePicker.get_selectedDate());
                var timeToCheck = moment(timePicker.get_selectedDate());
                var dateTimeToCheck = moment(dateToCheck.format("YYYY-MM-DD") + " " + timeToCheck.format("HH:mm"));
                var minDate = moment('<%=this.MinDate%>');

                var notValid = dateTimeToCheck.isBefore(minDate);
                if (notValid)
                {
                    alert("The Arrival Date cannot be before the departure date of the previous leg");
                    retVal = false;
                }
            }
            return retVal
        }
    </script>
</asp:Content>