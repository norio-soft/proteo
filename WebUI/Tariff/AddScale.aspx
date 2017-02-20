<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="AddScale.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.AddScale" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Create New Scale</h1>
    <fieldset runat="server" id="fsScale" class="invisiableFieldset">
        <br />
        <br />
        <table>
            <tr>
                <td class="formCellLabel">
                    Scale Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtScaleDescription" CssClass="fieldInputBox" runat="server" Width="250"
                        MaxLength="50" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvScaleDescription" runat="server" ErrorMessage="Please enter a description for this Scale"
                        ControlToValidate="txtScaleDescription" Display="Dynamic">
                                    <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this Scale" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            

        </table>
        <br />
        <br />
        <asp:RadioButton ID="optCopyScale" runat="server" Text="Copy from Scale" GroupName="CreateScale"
            Checked="true"></asp:RadioButton>
        <br />
        <div id="divCopyScale">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px;">
                        Scale
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromScale" Width="250" />
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <br />
        <asp:RadioButton ID="optEmptyScale" runat="server" Text="Empty Scale" GroupName="CreateScale">
        </asp:RadioButton>
        <table style="margin-left: 50px">
           <tr>
                <td class="formCellLabel" >
                    Metric
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboMetric" Width="250"  />
                </td>
            </tr>
       </table>
        <br />
        <br />
        <div class="buttonbar">
            <asp:Button ID="btnSave" runat="server" Text="Save" />
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" CausesValidation="false" />
        </div>
    </fieldset>

</asp:Content>
