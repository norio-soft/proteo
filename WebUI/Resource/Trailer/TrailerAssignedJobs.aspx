<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/WizardMasterPage.master" CodeBehind="TrailerAssignedJobs.aspx.cs" Inherits="Orchestrator.WebUI.Resource.Trailer.TrailerAssignedJobs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <h1>Jobs for trailer</h1> 
        <table>
            <tr>
            <td class="formCellLabel-Horizontal">
                Trailer Ref
            </td>
            <td class="formCellField-Horizontal">
                <asp:DropDownList ID="cboTrailer" runat="server" AutoPostBack="true">
                </asp:DropDownList>
            </td>
            </tr>
        </table>            
        <div class="buttonbar">
            <asp:Button ID="btnRefreshTop" runat="server" CssClass="buttonClass" Text="Refresh" />
        </div>
        <telerik:RadGrid runat="server" ID="rgJobsAssigned" AutoGenerateColumns="false" AllowSorting="true" Width="100%" EnableAjaxSkinRendering="true">
            <MasterTableView>
                <Columns>   
                    <telerik:GridTemplateColumn HeaderText="Job Id">
                        <ItemTemplate>  
                            <a id="lnkJob" runat="server"></a>
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
    </div>
    <div class="buttonbar">
        <asp:Button ID="btnRefreshBottom" runat="server" CssClass="buttonClass" Text="Refresh" />
        <asp:Button ID="btnClose" runat="server" CssClass="buttonClass" Text="Close" OnClientClick="window.close(); return true;" />
    </div>
    <br />
    <script type="text/javascript">
        function ViewJob(jobID) {
            window.open("/job/job.aspx?jobId=" + jobID + getCSID());
        }
    </script>

</asp:Content>
