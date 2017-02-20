<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageDriverBenchmarks.aspx.cs"  MasterPageFile="~/default_tableless.Master" Inherits="Orchestrator.WebUI.CAN.ManageDriverBenchmarks" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>
        Manage Driver Benchmarks
    </h1>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap.min.css">
    <link rel="stylesheet" href="//netdna.bootstrapcdn.com/bootstrap/3.1.1/css/bootstrap-theme.min.css">

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     
    
      <div class="alert alert-success" role="alert" id="divSuccess" runat="server" visible="false">The Benchmarks have been updated successfully.</div>
    
    <div class="container-fluid">
         <div>
          Please select an Organisation Unit: <telerik:RadComboBox runat="server" ID="cboOrgunit" DataTextField="Name" DataValueField="OrgUnitId" OnSelectedIndexChanged="cboOrgunit_SelectedIndexChanged" AutoPostBack="true" ></telerik:RadComboBox>
      </div>
    <div class="col-md-6">
    <table class="table table-condensed" >
        <tr>
            <th style="text-align:left;">Benchmark</th>
            <th style="text-align:left;">Baseline</th>
            <th style="text-align:left;">Target</th>
        </tr>
        <tr>
            <td>Speeding Instances</td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnSpeedingBaseline" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnSpeedingTarget" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>Over Rev Instances</td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnOverRevBaseline" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnOverRevTarget" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>Harsh Braking Instances</td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnHarshBrakingBaseline" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnHarshBrakingTarget" NumberFormat-DecimalDigits="0"></telerik:RadNumericTextBox>
            </td>
        </tr>
       <tr>
            <td>MPG</td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnMPGBaseline"></telerik:RadNumericTextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnMPGTarget"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>Idling Percentage</td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnIdlingBaseline"></telerik:RadNumericTextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnIdlingTarget"></telerik:RadNumericTextBox>
            </td>
        </tr>
        <tr>
            <td>Grading Percentage</td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnGradingBaseline"></telerik:RadNumericTextBox>
            </td>
            <td>
                <telerik:RadNumericTextBox runat="server" ID="rnGradingTarget"></telerik:RadNumericTextBox>
            </td>
        </tr>
    </table>
        <div class="buttonbar" style="margin-top:10px;" >
        <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" Width="100" OnClick="btnSaveChanges_Click" />
    </div>
</div>    
        </div>
    
    <script src="//netdna.bootstrapcdn.com/bootstrap/3.1.1/js/bootstrap.min.js"></script>
</asp:Content>

