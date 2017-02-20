<%@ Page Title="" Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="true"
    CodeBehind="SetBenchmark.aspx.cs" Inherits="Orchestrator.WebUI.CAN.SetBenchmark" %>

<asp:Content ID="Content4" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script language="JavaScript">
        window.resizeTo(500, 540)
    </script>
    <fieldset style="padding:5px;margin-top:10px; margin-bottom:10px;">
        <table>
            <tr>
                <td>
                    Grouping:
                </td>
                <td>
                    <asp:Label ID="lblGroupingValue" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Period:
                </td>
                <td>
                    <asp:Label ID="lblPeriodValue" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Driver:
                </td>
                <td>
                    <asp:Label ID="lblDriverValue" runat="server" Text=""></asp:Label>
                </td>
            </tr>
        </table>
    </fieldset>
    <fieldset style="padding:5px;margin-top:10px; margin-bottom:10px;">
    <legend>Benchmark Type</legend>
        <table>
        <tr>
        <td>
        <asp:RadioButtonList ID="optBenchMarkType" runat="server">
            <asp:ListItem Text="Set the baseline benchmark" Value="1" />
            <asp:ListItem Text="Set the target benchmark" Value="2" />
        </asp:RadioButtonList>
        </td>
        <td style="width:30px">
        <asp:RequiredFieldValidator ID="rfvBenchMarkType" runat="server" ControlToValidate="optBenchMarkType" Display="Dynamic" ErrorMessage="Please select a benchmark type"><img src="../images/error.png" title="Please select a benchmark type" /></asp:RequiredFieldValidator>
        </td>
        </tr>
        </table>
    </fieldset>
    <fieldset style="padding:5px;margin-top:10px; margin-bottom:10px;" title="Benchmark Values">
    <legend>Benchmark Values</legend>
       <asp:RadioButtonList ID="optBenchmarkValues" runat="server">
            <asp:ListItem Text="Update all benchmark values" Value="All" Selected="True" />
            <asp:ListItem Text="Only update Fuel Consumption (MPG) benchmark" Value="MPGOnly"/>
        </asp:RadioButtonList>
    </fieldset>
    <div class="wizardbuttonbar">
        <asp:Label ID="lblSuccess" runat="server" Text="The new benchmark was successfully set." ForeColor="Blue" Visible="false"></asp:Label>
        <asp:Button ID="btnSetBenchmark" runat="server" Text="Set" Width="75"></asp:Button>
        <input type="button" onclick="window.close();" value="Cancel" />
    </div>
</asp:Content>
