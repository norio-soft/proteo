<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="resourcedescription.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.views.resourcedescription" %>
<form runat="server">
<style type="text/css">
.HeadingRowLite
{ 
  padding-left:3px;
  background-color: #E0DFE3; 
  background-image: url(/images/header_rowBg.gif);   
}
.Row 
{ 
 height:20px;
  background-color: #FFFFFF; 
}
</style>
<div >
    <asp:panel ID="pnlJobs" runat="server">
    <div>Assigned jobs</div>
    <div>
        <asp:GridView runat="server" ID="grdLegs" AutoGenerateColumns="false" EnableViewState="false">
            <HeaderStyle CssClass="HeadingRowLite" Height="22" VerticalAlign="middle" />
            <RowStyle CssClass="Row" />
            <AlternatingRowStyle BackColor="WhiteSmoke" />
            <Columns>
               
                <asp:TemplateField HeaderText="ID" SortExpression="JobId">
                    <ItemTemplate>
                        <a href="javascript:ShowJob(<%#Eval("JobId") %>)">
                            <%#Eval("JobId") %></a>
                    </ItemTemplate>
                </asp:TemplateField>
                
                <asp:TemplateField HeaderText="Load No" SortExpression="Load_Number" ItemStyle-Width="70">
                    <ItemTemplate>
                        <%#Eval("LoadsWithOrders").ToString()%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Customer" DataField="OrganisationName" />
                <asp:BoundField HeaderText="Start" DataField="LegPlannedStartDateTime" DataFormatString="{0:dd/MM HH:mm}"
                    HtmlEncode="false" ItemStyle-Width="80" SortExpression="LegPlannedStartDateTime" />
                <asp:BoundField HeaderText="From" DataField="StartPointDisplay" />
                <asp:BoundField HeaderText="Finish" DataField="LegPlannedEndDateTime" DataFormatString="{0:dd/MM HH:mm}"
                    HtmlEncode="false" ItemStyle-Width="80" SortExpression="LegPlannedEndDateTime" />
                <asp:BoundField HeaderText="From" DataField="EndPointDisplay" />
            </Columns>
        </asp:GridView>        
    </div>
    </asp:panel>
    <asp:Panel ID="pnlNoJobs" runat="server" Visible="false">
        <div style="font-weight:bold;">There are no Jobs assigned to this resource.</div>
    </asp:Panel>
</div>

</form>