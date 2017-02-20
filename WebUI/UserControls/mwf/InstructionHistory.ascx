<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="InstructionHistory.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.mwf.InstructionHistory" %>

<asp:ScriptManagerProxy runat="server" ID="scriptManagerProxy">
    <Scripts>
        <asp:ScriptReference Path="~/UserControls/mwf/InstructionHistory.ascx.js" />
    </Scripts>
</asp:ScriptManagerProxy>


<div id="InstructionHistoryDialog" style="display: none; background-color: White;" title="MWF Instruction History">
    <table>
        <tr>
            <td class="">
                <span id="spnInstructionHistoryStartHeader"></span>
            </td>
            <td class="">
                <span id="spnInstructionHistoryEndHeader"></span>
            </td>
        </tr>
        <tr>
            <td class="">
                <ul id="ulInstructionHistoryStartEventDetails" style="list-style: none; padding-left: 10px">
                </ul>
            </td>
            <td class="">
                <ul id="ulInstructionHistoryEndEventDetails" style="list-style: none; padding-left: 10px">
                </ul>
            </td>
        </tr>
        <tr>
            <td class="">
                <span id="spnInstructionHistoryStartDeviceIdentifier"></span>
            </td>
            <td class="">
                <span id="spnInstructionHistoryEndDeviceIdentifier"></span>
            </td>
        </tr>
    </table>


</div>
