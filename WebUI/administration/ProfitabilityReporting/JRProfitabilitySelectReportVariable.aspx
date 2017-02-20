<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JRProfitabilitySelectReportVariable.aspx.cs" Inherits="Orchestrator.WebUI.administration.ProfitabilityReporting.JRProfitabilitySelectReportVariable" MasterPageFile="~/default_tableless.Master"  Title="Select Profitability Report Variable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Select Profitability Report Variable</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <h2>Current Profitability Report Variables</h2>

    <telerik:RadGrid ID="grdProfitabilityReportVariables" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" EnableAJAX="True" Width="800">
        <MasterTableView DataKeyNames="VariableId" >
            <Columns>

                <telerik:GridTemplateColumn HeaderText="Variable Name" UniqueName="VariableNameColumn" HeaderStyle-Width="110px">
                    <ItemTemplate><a href="JRProfitabilityUpdateReportVariable.aspx?ReportVariableId=<%# ((Orchestrator.Repositories.DTOs.ProfitabilityReport.JRProfitabilityReportVariableWithTypeRow)(Container.DataItem)).VariableID %>"><%# ((Orchestrator.Repositories.DTOs.ProfitabilityReport.JRProfitabilityReportVariableWithTypeRow)(Container.DataItem)).VariableTypeName %></a></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn HeaderText="Value" DataField="VariableValue" HeaderStyle-Width="50px"></telerik:GridBoundColumn>

                <telerik:GridTemplateColumn HeaderText="Valid From" UniqueName="VariableNameColumn" HeaderStyle-Width="110px">
                    <ItemTemplate><%# ((Orchestrator.Repositories.DTOs.ProfitabilityReport.JRProfitabilityReportVariableWithTypeRow)(Container.DataItem)).VariableValidFromDate.ToString("dd/MM/yyyy") %></ItemTemplate>
                </telerik:GridTemplateColumn>

            </Columns>
        </MasterTableView>
    </telerik:RadGrid>

</asp:Content>