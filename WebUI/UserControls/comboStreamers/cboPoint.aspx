<%@ Page Language="C#" AutoEventWireup="true" Inherits="UserControls_comboStreamers_cboPoint"
    EnableViewStateMac="false" Codebehind="cboPoint.aspx.cs" %>

<telerik:RadComboBox ID="cboPoint" runat="server" HighlightTemplatedItems="true">
    <ItemTemplate>
        <table border="0" style="border-color: Gray;" cellspacing="1" cellpadding="1" style="width: 460px;
            text-align: left">
            <tr>
                <td style="width: 125px; vertical-align: top; text-align: left;">
                    <%# ((Orchestrator.WebUI.Controls.PointComboItem)Container.DataItem).OrganisationName%>
                </td>
                <td style="width: 125px; vertical-align: top; text-align: left;">
                    <%# ((Orchestrator.WebUI.Controls.PointComboItem)Container.DataItem).Description%>
                </td>
                <td style="width: 210px; vertical-align: top; text-align: left;">
                    <%# ((Orchestrator.WebUI.Controls.PointComboItem)Container.DataItem).Address%>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</telerik:RadComboBox>
