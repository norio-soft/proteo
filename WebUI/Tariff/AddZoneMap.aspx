<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="AddZoneMap.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.AddZoneMap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Create New Zone Map</h1>
    <fieldset runat="server" id="fsZoneMap" class="invisiableFieldset">
        <br />
        <br />
        <table>
            <tr>
                <td class="formCellLabel">
                    Zone Map Description
                </td>
                <td class="formCellField">
                    <asp:TextBox ID="txtZoneMapDescription" CssClass="fieldInputBox" runat="server" Width="250"
                        MaxLength="50" ></asp:TextBox>
                    <asp:RequiredFieldValidator ID="rfvZoneMapDescription" runat="server" ErrorMessage="Please enter a description for this zone map"
                        ControlToValidate="txtZoneMapDescription" Display="Dynamic">
                                    <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this zone map" />
                    </asp:RequiredFieldValidator>
                </td>
            </tr>
            

        </table>
        <br />
        <br />
        <asp:RadioButton ID="optCopyZoneMap" runat="server" Text="Copy from Zone Map" GroupName="CreateZoneMap"
            Checked="true"></asp:RadioButton>
        <br />
        <div id="divCopyZoneMap">
            <table style="margin-left: 50px">
                <tr>
                    <td class="formCellLabel" style="width: 150px;">
                        Zone Map
                    </td>
                    <td class="formCellField">
                        <telerik:RadComboBox runat="server" ID="cboCopyFromZoneMap" Width="250" />
                    </td>
                </tr>
            </table>
        </div>
        <br />
        <br />
        <asp:RadioButton ID="optEmptyZoneMap" runat="server" Text="Empty Zone Map" GroupName="CreateZoneMap">
        </asp:RadioButton>
        <table style="margin-left: 50px">
           <tr>
                <td class="formCellLabel" >
                    Zone Type
                </td>
                <td class="formCellField">
                    <telerik:RadComboBox runat="server" ID="cboZoneType" Width="250"  />
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
