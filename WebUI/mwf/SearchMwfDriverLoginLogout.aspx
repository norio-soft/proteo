<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SearchMwfDriverLoginLogout.aspx.cs" Inherits="Orchestrator.WebUI.mwf.SearchMwfDriverLoginLogout" MasterPageFile="~/default_tableless.Master" Title="View Driver Logins and Logouts" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>View Driver Logins and Logouts</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script>

        function openNewPopupWindow(winName, url, height, width, scrollbars)
        {
            var newWindow = window.open(url, winName, 'height=' + height + ',width=' + width + ',toolbar=no,menu=no,scrollbars=' + scrollbars);

            if (window.focus) {
                newWindow.focus();
            }
        }
    </script>

    <h2>View Driver Logins and Logouts</h2>
    
    <fieldset>
        <table>
            <tr>
                <td class="formCellLabel">Start Date Time</td>
                <td class="formCellField">
                    <telerik:RadDateTimePicker id="dteStartDate" runat="server" Width="150"><DateInput ID="DateInput1" runat="server" dateformat="dd/MM/yy HH:mm"></DateInput></telerik:RadDateTimePicker>
                </td>
                <td class="formCellLabel">End Date Time</td>
                <td class="formCellField">
                    <telerik:RadDateTimePicker id="dteEndDate" runat="server" Width="150"><DateInput ID="DateInput2" runat="server" dateformat="dd/MM/yy HH:mm"></DateInput></telerik:RadDateTimePicker>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Driver</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboDriver" runat="server" Width="300px" MarkFirstMatch="true">
                    </telerik:RadComboBox> 
                </td>
                <td class="formCellLabel">Vehicle</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboVehicle" runat="server" Width="300px" MarkFirstMatch="true">
                    </telerik:RadComboBox> 
                </td>
            </tr>
        </table>
    </fieldset>
    
    <div class="buttonBar">
        <asp:Button ID="btnSearch" runat="server" Text="Search" />
    </div>
    
    <telerik:RadGrid ID="grdDriverLoginLogout" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" EnableAJAX="True">
        <MasterTableView DataKeyNames="DriverLoginLogoffId" >
            <Columns>

                <telerik:GridTemplateColumn HeaderText="Driver" UniqueName="DriverColumn" HeaderStyle-Width="110px">
                    <ItemTemplate><a href="javascript:openNewPopupWindow('driver', '<%# String.Format("/resource/driver/addupdatedriver.aspx?identityId={0}", ((Orchestrator.Repositories.DTOs.FindDriverLoginLogoffRow)(Container.DataItem)).DriverIdentityId) %>', 700, 600, 'no')"><%# ((Orchestrator.Repositories.DTOs.FindDriverLoginLogoffRow)(Container.DataItem)).DriverName %></a></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Vehicle" UniqueName="VehicleColumn" HeaderStyle-Width="110px">
                    <ItemTemplate><a href="javascript:openNewPopupWindow('vehicle', '<%# String.Format("/resource/Vehicle/addupdatevehicle.aspx?resourceId={0}", ((Orchestrator.Repositories.DTOs.FindDriverLoginLogoffRow)(Container.DataItem)).VehicleId) %>', 600, 900, 'no')"><%# ((Orchestrator.Repositories.DTOs.FindDriverLoginLogoffRow)(Container.DataItem)).VehicleRegNo %></a></ItemTemplate>
                </telerik:GridTemplateColumn>


                <telerik:GridBoundColumn HeaderText="Login/Out" DataField="LogOnLogOffTypeDescription" HeaderStyle-Width="50px"></telerik:GridBoundColumn>

                <telerik:GridBoundColumn HeaderText="When" DataField="LogonLogOffTimestamp" HeaderStyle-Width="60px"></telerik:GridBoundColumn>

            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>