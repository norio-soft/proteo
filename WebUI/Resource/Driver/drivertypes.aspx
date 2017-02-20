<%@ Page Language="C#" AutoEventWireup="true" Inherits="Resource_Driver_drivertypes" MasterPageFile="~/default_tableless.master" Codebehind="drivertypes.aspx.cs" Title="Driver Type Administration" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Driver Type Administration</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Please note that you can add or amend existing driver types, but you cannot remove driver types.</h2>

    <telerik:RadGrid ID="grdNominalCodes" runat="server" Skin="Orchestrator" AutoGenerateColumns="false">
        <MasterTableView>
            <Columns>
                <telerik:GridHyperLinkColumn  HeaderText="Driver Type" DataTextField="Description" DataNavigateUrlFormatString="javascript:manageDriverType({0})" DataNavigateUrlFields="DriverTypeID"></telerik:GridHyperLinkColumn>
                <telerik:GridTemplateColumn HeaderText="Monday" UniqueName="Monday">
                    <ItemTemplate>
                        <%# Eval("Monday") != DBNull.Value ? (bool) Eval("Monday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Tuesday" UniqueName="Tuesday">
                    <ItemTemplate>
                        <%# Eval("Tuesday") != DBNull.Value ? (bool) Eval("Tuesday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Wednesday" UniqueName="Wednesday">
                    <ItemTemplate>
                        <%# Eval("Wednesday") != DBNull.Value ? (bool) Eval("Wednesday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Thursday" UniqueName="Thursday">
                    <ItemTemplate>
                        <%# Eval("Thursday") != DBNull.Value ? (bool) Eval("Thursday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Friday" UniqueName="Friday">
                    <ItemTemplate>
                        <%# Eval("Friday") != DBNull.Value ? (bool) Eval("Friday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Saturday" UniqueName="Saturday">
                    <ItemTemplate>
                        <%# Eval("Saturday") != DBNull.Value ? (bool) Eval("Saturday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Sunday" UniqueName="Sunday">
                    <ItemTemplate>
                        <%# Eval("Sunday") != DBNull.Value ? (bool) Eval("Sunday") ? "Yes" : "No" : "" %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="StartTime" HeaderText="Start Time" DataFormatString="{0:HH:mm}"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="FinishTime" HeaderText="Finish Time" DataFormatString="{0:HH:mm}"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreateDate" HeaderText="Created" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CreateUserID" HeaderText="Created By"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastUpdateDate" HeaderText="Last Updated" DataFormatString="{0:dd/MM/yy}"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="LastUpdateUserID" HeaderText="Last Updated By"></telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
         
    <div class="buttonbar">
        <input type="button" onclick="javascript:manageDriverType(-1);" value="Add New Driver Type" />
        <asp:button id="btnRefresh" runat="server" text="Refresh" />    
    </div>
    
           <telerik:RadWindowManager ID="rwmDriverTypes" runat="server" Modal="true" ShowContentDuringLoad="false" ReloadOnShow="true" KeepInScreenBounds="true" VisibleStatusbar="false">
            <Windows>
                <telerik:RadWindow ID="mediumWindow" runat="server" Height="450" Width="500" />
            </Windows>
        </telerik:RadWindowManager>
        
    <script language="javascript" type="text/javascript">
    function manageDriverType(driverTypeID)
    {
        var url = "managedriverType.aspx";
        if (driverTypeID > 0)
            url += "?dt=" + driverTypeID;
            
        var wnd = window.radopen("about:blank", "mediumWindow");                                  
        wnd.SetUrl(url);
        wnd.SetTitle("Manage Driver Type");
    }
    </script>
      
</asp:Content>