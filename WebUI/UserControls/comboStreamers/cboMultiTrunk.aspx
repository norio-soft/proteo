<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="cboMultiTrunk.aspx.cs"
    Inherits="Orchestrator.WebUI.UserControls.comboStreamers.cboMultiTrunkPoint" %>

<telerik:RadComboBox ID="rcbMultiTrunk" runat="server" HighlightTemplatedItems="true">
    <ItemTemplate>
        <table border="0" style="border-color: Gray;" cellspacing="1" cellpadding="1" style="width: 550px;
            text-align: left">
            <tr>
                <td nowrap style="width: 175px; vertical-align: top; text-align: left;">
                    <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).Description %>
                </td>
                <td nowrap style="width: 325px; vertical-align: top; text-align: left;">
                    <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).HtmlTableFormattedTrunkPointDescriptionsWithTravelTimes %>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</telerik:RadComboBox>
