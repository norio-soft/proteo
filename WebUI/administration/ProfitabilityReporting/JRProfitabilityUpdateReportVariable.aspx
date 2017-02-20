<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="JRProfitabilityUpdateReportVariable.aspx.cs" Inherits="Orchestrator.WebUI.administration.ProfitabilityReporting.JRProfitabilityUpdateReportVariable" MasterPageFile="~/default_tableless.Master"  Title="Update Profitability Report Variable"%>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Update Profitability Report Variable</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <fieldset style="width:500px;">

    <h2><%= jRProfitabilityReportVariableWithTypeRow.VariableTypeName %> History</h2>

        <telerik:RadGrid ID="grdProfitabilityReportVariable" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" EnableAJAX="True">
        <MasterTableView DataKeyNames="VariableId" >
            <Columns>

                <telerik:GridTemplateColumn HeaderText="Valid From" UniqueName="ValidFromColumn" HeaderStyle-Width="110px">
                    <ItemTemplate><%# ((Orchestrator.Models.ProfitReportVariable)(Container.DataItem)).VariableValidFromDate.ToString("dd/MM/yyyy") %></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn HeaderText="Value" DataField="VariableValue" HeaderStyle-Width="50px"></telerik:GridBoundColumn>

            </Columns>
        </MasterTableView>
    </telerik:RadGrid>


    <div style="padding-top:20px">
        <h2>Add New Variable Value for <%= jRProfitabilityReportVariableWithTypeRow.VariableTypeName %></h2>

        <table>
            <tr>
                <td class="formCellLabel">New From Date</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="VariableFromDate" runat="server" Width="100"><DateInput ID="DateInput1" runat="server" dateformat="dd/MM/yy"></DateInput></telerik:RadDatePicker>
                </td>
                <td class="formCellLabel">New Value</td>
                <td class="formCellField"><telerik:RadNumericTextBox ID="VariableValue" runat="server" Type="Number" Width="100px"><NumberFormat GroupSeparator="" DecimalDigits="10" /></telerik:RadNumericTextBox></td>
            </tr>
        </table>
    </div>
    
    <div  style="padding-top:20px">
        <div class="buttonBar">
            <asp:Button ID="btnBack" runat="server" Text="Back" /> &nbsp;
            <asp:Button ID="btnSave" runat="server" Text="Save" />
        </div>
   </div>        


    <div style="padding-top:20px">
        <asp:Label id="ErrorMessage" runat="server" CssClass="errorMessage"></asp:Label>
    </div>

    </fieldset>


</asp:Content>