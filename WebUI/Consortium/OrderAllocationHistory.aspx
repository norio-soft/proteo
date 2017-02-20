<%@ Page MasterPageFile="~/WizardMasterPage.master" Language="C#" AutoEventWireup="true" CodeBehind="OrderAllocationHistory.aspx.cs" Inherits="Orchestrator.WebUI.Consortium.OrderAllocationHistory" %>

<asp:Content runat="server" ContentPlaceHolderID="PageTitlePlaceHolder1">
    Allocation History
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <telerik:RadGrid ID="rgHistory" runat="server" AutoGenerateColumns="false" Width="650px">
        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn DataField="CreateDate" DataFormatString="{0:dd/MM/yyyy HH:mm}" HeaderText="Date/Time" />
                <telerik:GridBoundColumn DataField="AllocatedToName" HeaderText="Allocated To" />
                <telerik:GridBoundColumn DataField="SubcontractorName" HeaderText="Subcontracted To" />
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Changed By" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>