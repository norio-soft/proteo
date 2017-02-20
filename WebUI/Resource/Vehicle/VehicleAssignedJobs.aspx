<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/WizardMasterPage.master" CodeBehind="VehicleAssignedJobs.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Vehicle.VehicleAssignedJobs" %>
<asp:Content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <script type="text/javascript" language="javascript">
        function ViewJob(jobID) {
            window.open("/job/job.aspx?jobId=" + jobID + getCSID());
        }
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Jobs assigned to vehicle</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <table>
            <tr>
            <td class="formCellLabel-Horizontal">
                Vehicle Ref
            </td>
            <td class="formCellField-Horizontal">
                <asp:DropDownList ID="cboVehicle" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </td>
            </tr>
        </table>     
        <div class="buttonbar">
            <asp:Button ID="btnRefreshTop" runat="server" CssClass="buttonClass" Text="Refresh" />
        </div>
        <telerik:RadGrid runat="server" ID="rgJobsAssigned" AutoGenerateColumns="false" AllowSorting="true" EnableAjaxSkinRendering="true" Width="100%">
            <MasterTableView>
                <Columns>   
                    <telerik:GridTemplateColumn HeaderText="Job Id">
                        <ItemTemplate>  
                            <a id="lnkJob" runat="server" visible="true"></a>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn headertext="">
                        <ItemTemplate>
                            <img src="/images/<%#GetInstructionTypeImage((int)Eval("InstructionTypeId"))%>" width="20"/> 
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="PointDescription" HeaderText="Point"></telerik:GridBoundColumn>
                    <telerik:GridBoundColumn DataField="OrdersOnInstruction" HeaderText="No. Orders"></telerik:GridBoundColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Scrolling AllowScroll="true" ScrollHeight="340" UseStaticHeaders="true" />
            </ClientSettings>
        </telerik:RadGrid>
        <div class="buttonbar">
            <asp:Button ID="btnRefreshBottom" runat="server" CssClass="buttonClass" Text="Refresh" />
            <asp:Button ID="btnClose" runat="server" CssClass="buttonClass" Text="Close" OnClientClick="window.close(); return true;" />
        </div>
    </div>
</asp:Content>