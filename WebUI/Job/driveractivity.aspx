<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="True" CodeBehind="driveractivity.aspx.cs" Inherits="Orchestrator.WebUI.Job.driveractivity" Title="Driver Activity" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Driver Activity</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
    <legend>Driver Choice</legend>
    <table>
        <tr>
            <td class="formCellLabel">Driver</td>
            <td class="formCellField">
              <telerik:RadComboBox ID="cboDriver" runat="server" Width="250" EnableLoadOnDemand="true" ItemRequestTimeout="500" RadControlsDir="~/script/RadControls/" MarkFirstMatch="true" ShowMoreResultsBox="true" AllowCustomText="true" SkinsPath="~/RadControls/ComboBox/Skins/" Skin="Orchestrator" Overlay="true">
                </telerik:RadComboBox>
                <asp:RequiredFieldValidator ID="rfvDriver" runat="server" ControlToValidate="cboDriver" Display="Dynamic" 
                ErrorMessage="Please select a driver."><img src="../../images/newMasterPage/icon-warning.png" 
                title="Please select a driver." />
                </asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="formCellLabel">Start date</td>
            <td class="formCellField"><telerik:RadDatePicker  CssClass="dateInputBox" runat="server" id="dteStartDate"></telerik:RadDatePicker></td>
        </tr>
        <tr>
            <td class="formCellLabel">End date</td>
            <td class="formCellField"><telerik:RadDatePicker  CssClass="dateInputBox" runat="server" id="dteEndDate"></telerik:RadDatePicker></td>
        </tr>
        <tr>
            <td class="formCellLabel">Leg States</td>
            <td class="formCellField"><asp:CheckBoxList ID="cblStates" runat="server" RepeatDirection=Horizontal></asp:CheckBoxList></td>
        </tr>
    </table>
</fieldset>

<div class="buttonbar">
    <asp:button id="btnFilter" runat="server" text="Get Activity" />
    <asp:Button ID="btnExport" runat="server" Text="Export to CSV" Visible="false" />
    <asp:Button ID="btnEmail" runat="server" Text="Email This" Visible="false" />
    <asp:Button ID="btnReset" runat="server" Text="Reset" OnClientClick="location.href=location.href; return false;" />
</div>

 <telerik:RadGrid ID="grdTrafficSheet" runat="server" AutoGenerateColumns="false" EnableViewState="false">
    <MasterTableView Name="MasterTable" CommandItemDisplay="None" EditMode="InPlace">
        <Columns>
            <telerik:GridBoundColumn DataField="InstructionID" Visible="false"></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="" ItemStyle-Width="5">
                <ItemTemplate>
                    <img src="<%# GetImage(Eval("LegPosition").ToString()) %>" height="20" width="5" />
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="ID" SortExpression="JobId">
                <ItemTemplate>
                    <a href="javascript:openJobDetailsWindow(<%#Eval("JobId") %>)"><%#Eval("JobId") %></a>
                </ItemTemplate>
            </telerik:GridTemplateColumn>                    
            <telerik:GridTemplateColumn HeaderText="Load No" SortExpression="Load_Number" ItemStyle-Width="70">
                <ItemTemplate>
                    <%#GetLoadLinks(Eval("LoadsWithOrders").ToString()) %>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Customer" Visible = "false">
                <ItemTemplate>
                    <span id="spnStartPointDisplay" onclick="" onmouseover="ShowOrganisationDetailsToolTip(this, <%# Eval("CustomerIdentityId") %>);" onmouseout="hideAd();" class="orchestratorLink"><%# Eval("OrganisationName") %></span>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridTemplateColumn HeaderText="Customer" Visible="false">
                <ItemTemplate>
                    <%# GetCustomers((int)Eval("StartInstructionTypeID"), (int)Eval("EndInstructionTypeID"), Eval("StartOrders").ToString(), Eval("EndOrders").ToString())%>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Start" DataField="LegPlannedStartDateTime" DataFormatString="{0:dd/MM HH:mm}" ItemStyle-Width="80"  />
            <telerik:GridTemplateColumn HeaderText="From">
                <ItemTemplate>
                    <span id="spnStartPointDisplay" onmouseover="javascript:ShowPointWithInstructionNotesToolTip(this, <%# Eval("StartPointId") == DBNull.Value ? -1 : Eval("StartPointId") %>,<%# Eval("StartInstructionId") == DBNull.Value ? -1 : Eval("StartInstructionId") %>);"
                    onmouseout="hideAd();" class="orchestratorLink">
                    <%# Eval("StartPointDisplay") %>
                    <b><font color="red">
                        <%# Eval("StartShortCodes") %></font></b></span>
                    <div style="background-color:#E7EBF1"><%#GetStartAction(Container.DataItem) %></div>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn HeaderText="Finish" DataField="LegPlannedEndDateTime" DataFormatString="{0:dd/MM HH:mm}" ItemStyle-Width="80"></telerik:GridBoundColumn>
            <telerik:GridTemplateColumn HeaderText="To">
                <ItemTemplate>
                    <span id="spnEndPointDisplay" onmouseover="javascript:ShowPointWithInstructionNotesToolTip(this, <%# Eval("EndPointId") == DBNull.Value ? -1 : Eval("EndPointId") %>,<%# Eval("EndInstructionId") == DBNull.Value ? -1 : Eval("EndInstructionId") %>);"
                    onmouseout="hideAd();" class="orchestratorLink">
                    <%# Eval("EndPointDisplay") %>
                    <b><font color="red">
                        <%# Eval("EndShortCodes") %></font></b></span>
                    <div style="background-color:#E7EBF1"><%#GetEndAction(Container.DataItem) %></div>
                </ItemTemplate>
            </telerik:GridTemplateColumn>
            <telerik:GridBoundColumn DataField="RegNo" HeaderText="Vehicle"></telerik:GridBoundColumn>
            <telerik:GridBoundColumn DataField="TrailerRef" HeaderText="Trailer"></telerik:GridBoundColumn>
        </Columns>
    </MasterTableView>
    <ClientSettings>
        <Selecting AllowRowSelect="false" />
    </ClientSettings>
    
</telerik:RadGrid>

<script type="text/javascript">
    function openJobDetailsWindow(jobId)
    {
        var url = 'Job.aspx?wiz=true&jobId=' + jobId + getCSID();

        openDialogWithScrollbars(url,'1220','870');
    }
    
    function viewOrderProfile(orderID)
    {
        var url = "<%=Page.ResolveUrl("~/Groupage/ManageOrder.aspx")%>?wiz=true&oID=" + orderID; // Resolve the URL as RadWindow will not understand the tilde (~) notation.
            
        var wnd = window.radopen("about:blank", "largeWindow");                                  
        wnd.setUrl(url);
        wnd.SetTitle("Add/Update Order");
    }
</script>
</asp:Content>
