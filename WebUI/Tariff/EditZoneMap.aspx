<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true"
    CodeBehind="EditZoneMap.aspx.cs" Inherits="Orchestrator.WebUI.Tariff.EditZoneMap" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h1>
        Edit Zone Map
    </h1>

    <fieldset runat="server" id="fsTariff" class="invisiableFieldset">
            <table width="520px">
                <tr>
                    <td class="formCellLabel">
                        Zone Map Description
                    </td>
                    <td class="formCellField">
                        <asp:TextBox ID="txtZoneMapDescription" CssClass="fieldInputBox" runat="server" Width="250"
                            MaxLength="39"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="rfvZoneMapDescription" runat="server" ErrorMessage="Please enter a description for this zone map"
                            ControlToValidate="txtZoneMapDescription" Display="Dynamic">
                                <img src="/images/Error.png" height="16" width="16" title="Please enter a description for this zone map" />
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
                        Zone Type
                    </td>
                    <td class="formCellField">
                        <asp:Label runat="server" ID="lblZoneType"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="formCellLabel">
                        Zones
                    </td>
                   <td>
                        <table runat="server" id="grdZones" cellpadding="0" cellspacing="0" >
                        </table>
                        <input type="hidden" runat="server" id="hidZoneChanges" />
                   </td>
            </table>
            <div class="buttonbar">
                <asp:Button ID="btnSave" runat="server" Text="Save Changes" />
                <asp:Button ID="btnZonePostcodes" runat="server" Text="Zone Postcodes" />
                <asp:Button ID="btnZoneMapList" runat="server" Text="Zone Map List" />
            </div>
    </fieldset> 
    
        <script type="text/javascript">

            function Zone_onchange(input) {
                var hidZoneChanges = $get("<%= hidZoneChanges.ClientID %>");

                if (hidZoneChanges.value.indexOf(input.name) < 0) {

                    if (hidZoneChanges.value.length > 0)
                        hidZoneChanges.value += ",";
                    hidZoneChanges.value += input.name;
                }
                else
                {
                    if (input.value == "")
                    {
                        hidZoneChanges.value = hidZoneChanges.value.replace(input.name, "");
                    }
                }
            }

    </script>

</asp:Content>
