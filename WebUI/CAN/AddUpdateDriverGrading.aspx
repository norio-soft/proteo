<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="AddUpdateDriverGrading.aspx.cs" Inherits="Orchestrator.WebUI.CAN.AddUpdateDriverGrading" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Grading Weights
    </h1>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset style="padding: 5px; margin-top: 10px; margin-bottom: 10px;">
        <table>
            <tr>
                <td>
                    Award
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtDistancePoints"  runat="server" Width="60px" />
                    <asp:RequiredFieldValidator ID="rfvDistancePoints" runat="server" ControlToValidate="txtDistancePoints" Display="Dynamic" ErrorMessage="A value is required"><img src="../images/error.png" title="A value is required" /></asp:RequiredFieldValidator>
                </td>
                <td>
                    points per km travelled
                </td>
            </tr>
            <tr>
                <td>
                    Deduct
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtIdlingPoints"  runat="server" Width="60px"/>
                    <asp:RequiredFieldValidator ID="rfvIdlingPoints" runat="server" ControlToValidate="txtIdlingPoints" Display="Dynamic" ErrorMessage="A value is required"><img src="../images/error.png" title="A value is required" /></asp:RequiredFieldValidator>
                </td>
                <td>
                    percentage of points per 1% idling time
                </td>
            </tr>
            <tr>
                <td>
                    Deduct
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtBrakingPoints"  runat="server" Width="60px"/>
                    <asp:RequiredFieldValidator ID="rfvBrakingPoints" runat="server" ControlToValidate="txtBrakingPoints" Display="Dynamic" ErrorMessage="A value is required"><img src="../images/error.png" title="A value is required" /></asp:RequiredFieldValidator>
                </td>
                <td>
                    points per severe braking occurence
                </td>
            </tr>
            <tr>
                <td>
                    Deduct
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtSpeedingPoints"  runat="server" Width="60px"/>
                    <asp:RequiredFieldValidator ID="rfvSpeedingPoints" runat="server" ControlToValidate="txtSpeedingPoints" Display="Dynamic" ErrorMessage="A value is required"><img src="../images/error.png" title="A value is required" /></asp:RequiredFieldValidator>
                </td>
                <td>
                    points per speeding occurence
                </td>
            </tr>
            <tr>
                <td>
                    Deduct
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtOverRevvingPoints"  runat="server" Width="60px"/>
                    <asp:RequiredFieldValidator ID="rfvOverRevvingPoints" runat="server" ControlToValidate="txtOverRevvingPoints" Display="Dynamic" ErrorMessage="A value is required"><img src="../images/error.png" title="A value is required" /></asp:RequiredFieldValidator>
                </td>
                <td>
                    points per over-revving occurence
                </td>
            </tr>
            <tr>
                <td>
                    Deduct
                </td>
                <td>
                    <telerik:RadNumericTextBox ID="txtUneconomicalPoints"  runat="server" Width="60px"/>
                    <asp:RequiredFieldValidator ID="rfvUneconomicalPoints" runat="server" ControlToValidate="txtUneconomicalPoints" Display="Dynamic" ErrorMessage="A value is required"><img src="../images/error.png" title="A value is required" /></asp:RequiredFieldValidator>
                </td>
                <td>
                    percentage of points per 1% non-economical driving time
                </td>
            </tr>
        </table>
        
    </fieldset>
    <div class="buttonbar" >
        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" Width="100" />
    </div>
</asp:Content>
