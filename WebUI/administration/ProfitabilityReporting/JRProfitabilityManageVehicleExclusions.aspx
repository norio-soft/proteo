<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JRProfitabilityManageVehicleExclusions.aspx.cs" Inherits="Orchestrator.WebUI.administration.ProfitabilityReporting.JRProfitabilityManageVehicleExclusions" MasterPageFile="~/WizardMasterPage.master" Title="Manage Profitability Vehicle Exclusions" EnableEventValidation="false"  %>
<%@ Import Namespace="Orchestrator.Models" %>

<asp:content id="Titlebar1" ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    Manage Profitability Vehicle Exclusions for <%# VehicleRegNo %>
</asp:content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <script src="/script/scripts.js" type="text/javascript"></script>

    <fieldset style="width:500px;">

    <h2>Profitability Report Exclusions History for Vehicle <%# VehicleRegNo %></h2>

        <telerik:RadGrid ID="grdProfitabilityVehicleExclusions" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" EnableAJAX="True">
        <MasterTableView DataKeyNames="ProfitReportVehicleExclusionId" >
            <Columns>
                <telerik:GridBoundColumn HeaderText="From Date" DataField="VariableExclusionFromDate" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="50px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="To Date" DataField="VariableExclusionToDate" DataFormatString="{0:dd/MM/yy}" HeaderStyle-Width="50px"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Type" DataField="VehicleExclusionTypeDescription" HeaderStyle-Width="150px"></telerik:GridBoundColumn>

                <telerik:GridTemplateColumn HeaderText="Edit" UniqueName="EditColumn" HeaderStyle-Width="6px">
                    <ItemTemplate>
                        <a href="/administration/ProfitabilityReporting/JRProfitabilityEditVehicleExclusionPopup.aspx?ProfitReportVehicleExclusionId=<%# ((ProfitReportVehicleExclusion)(Container.DataItem)).ProfitReportVehicleExclusionID %>&VehicleId=<%= VehicleId %>">Edit</a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridButtonColumn HeaderText="Action" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" ButtonType="PushButton" CommandName="remove" Text="Remove" ConfirmText="Are you sure you want to remove this vehicle exclusion?"></telerik:GridButtonColumn>

            </Columns>
        </MasterTableView>
    </telerik:RadGrid>


    <div  style="padding-top:20px">
        <div class="buttonBar">
            <asp:Button ID="btnCancel" runat="server" Text="Cancel" Width="75" CausesValidation="false" />
            <asp:Button ID="btnAdd" runat="server" Text="Add" Width="75" CausesValidation="false" />
        </div>
   </div>        


    <div style="padding-top:20px">
        <asp:Label id="ErrorMessage" runat="server" CssClass="errorMessage"></asp:Label>
    </div>

    </fieldset>


</asp:Content>