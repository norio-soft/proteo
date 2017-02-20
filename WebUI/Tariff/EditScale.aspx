<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="EditScale.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.EditScale" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Edit Scale
    </h1>

    <telerik:RadInputManager runat="server" id="rimScaleValues">
        <telerik:NumericTextBoxSetting Type="Number" BehaviorID="ScaleValues" DecimalDigits="2" SelectionOnFocus="SelectAll" AllowRounding="false" >
        <TargetControls>
        <telerik:TargetInput ControlID="grdValues" Enabled="true" />
        </TargetControls> 
        </telerik:NumericTextBoxSetting>
    </telerik:RadInputManager>

    <fieldset runat="server" id="fsTariff" class="invisiableFieldset">
            <table width="520px">
                <tr>
                    <td class="formCellLabel">
                        Scale Description
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtScaleDescription" CssClass="fieldInputBox" runat="server" Width="250"
                            MaxLength="39"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvScaleDescription" runat="server" ErrorMessage="Please enter a description for this Scale"
                            ControlToValidate="txtScaleDescription" Display="Dynamic">
                                <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this Scale" />
                        </asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Is Enabled
                    </td>
                    <td class="formCellField">
                        <asp:CheckBox runat="server" ID="chkIsEnabled" />
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Metric
                    </td>
                    <td class="formCellField">
                        <asp:Label runat="server" ID="lblMetric"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Values
                    </td>
                   <td>
                        <table runat="server" id="grdValues" cellpadding="0" cellspacing="0" >
                        </table>
                        <input type="hidden" runat="server" id="hidValueChanges" />
                   </td>
            </table>
            <div class="buttonbar">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                <asp:Button ID="btnScaleList" runat="server" Text="Scale List" />
            </div>
    </fieldset> 
    
        <script type="text/javascript">

            function Value_onchange(input) {
                var hidValueChanges = $get("<%= hidValueChanges.ClientID %>");
                if (hidValueChanges.value.length > 0)
                    hidValueChanges.value += ",";
                hidValueChanges.value += input.name;
                hidValueChanges.value += ":" + input.value;
            }

    </script>

</asp:Content>
