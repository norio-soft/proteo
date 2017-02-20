<%@ Page Language="C#" AutoEventWireup="true" Codebehind="SearchMwfPhotos.aspx.cs" Inherits="Orchestrator.WebUI.mwf.SearchMwfPhotos" MasterPageFile="~/default_tableless.Master" Title="Search and Manage MWF Photos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Search and Manage MWF Photos</h1>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script>

        function showMap(latitude, longitude)
        {
            var url = "/gps/getcurrentlocation.aspx?lat=" + latitude + "&lng=" + longitude;

            var newWindow = window.open(url, 'name', 'height=550,width=600,toolbar=no,menu=no,scrollbars=no');

            if (window.focus)
            {
                newWindow.focus();
            }
        }

        function openNewPopupWindow(winName, url, height, width, scrollbars)
        {
            var newWindow = window.open(url, winName, 'height=' + height + ',width=' + width + ',toolbar=no,menu=no,scrollbars=' + scrollbars);

            if (window.focus) {
                newWindow.focus();
            }
        }
    </script>

    <h2>Search and Manage MWF Photos</h2>
    
    <fieldset>
        <table>
            <tr>
                <td class="formCellLabel">Date of Photo</td>
                <td class="formCellField">
                    <telerik:RadDatePicker id="dtePhotoDate" runat="server" Width="100"><DateInput ID="DateInput1" runat="server" dateformat="dd/MM/yy"></DateInput></telerik:RadDatePicker>
                </td>
                <td class="formCellLabel">Photo Type</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboPhotoType" runat="server" Width="250" MarkFirstMatch="true">
                        <Items>
                            <telerik:RadComboBoxItem Value="" Text="- Select -" />
                            <telerik:RadComboBoxItem Value="1" Text="Order" />
                            <telerik:RadComboBoxItem Value="0" Text="No Order" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel">Order Id</td>
                <td class="formCellField"><telerik:RadNumericTextBox ID="rntbOrderId" runat="server" Type="Number" Width="100px"><NumberFormat GroupSeparator="" DecimalDigits="0" /></telerik:RadNumericTextBox></td>
                <td class="formCellLabel">Run Id</td>
                <td class="formCellField"><telerik:RadNumericTextBox ID="rntbRunId" runat="server" Type="Number" Width="100px"><NumberFormat GroupSeparator="" DecimalDigits="0" /></telerik:RadNumericTextBox></td>
            </tr>
            <tr>
                <td class="formCellLabel">Driver</td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboDriver" runat="server" Width="300px" MarkFirstMatch="true">
                    </telerik:RadComboBox> 
                </td>
                <td class="formCellLabel">Instruction Id</td>
                <td class="formCellField"><telerik:RadNumericTextBox ID="rntbInstructionId" runat="server" Type="Number" Width="100px"><NumberFormat GroupSeparator="" DecimalDigits="0" /></telerik:RadNumericTextBox></td>
            </tr>
        </table>
    </fieldset>
    
    <div class="buttonBar">
        <asp:Button ID="btnSearch" runat="server" Text="Search" />
    </div>
    
    <telerik:RadGrid ID="grdPhotos" runat="server" Skin="Orchestrator" AutoGenerateColumns="false" EnableAJAX="True">
        <MasterTableView DataKeyNames="PhotoId" >
            <Columns>

                <telerik:GridTemplateColumn HeaderText="Photo" UniqueName="Photo" HeaderStyle-Width="110px">
                    <ItemTemplate><%# GetPhotoImageLink(((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).PhotoFileName) %></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn HeaderText="Photo Date" DataField="PhotoDateTime" HeaderStyle-Width="60px"></telerik:GridBoundColumn>

                <telerik:GridTemplateColumn HeaderText="Location" UniqueName="Location" HeaderStyle-Width="190px">
                    <ItemTemplate><a href="javascript:showMap(<%# ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).Latitude %>, <%# ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).Longitude %>)"><%# String.Format("{0},{1}", ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).Latitude, ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).Longitude) %></a></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Driver" UniqueName="Driver" HeaderStyle-Width="110px">
                    <ItemTemplate><a href="javascript:openNewPopupWindow('driver', '<%# String.Format("/resource/driver/addupdatedriver.aspx?identityId={0}", ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).DriverId) %>', 700, 600, 'no')"><%# ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).DriverName %></a></ItemTemplate>
                </telerik:GridTemplateColumn>


                <telerik:GridTemplateColumn HeaderText="Order Id" UniqueName="OrderId" HeaderStyle-Width="90px">
                    <ItemTemplate><a href="javascript:openNewPopupWindow('order', '<%# String.Format("/Groupage/ManageOrder.aspx?wiz=true&oID={0}", ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).OrderId) %>', 550, 600, 'yes')"><%# ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).OrderId %></a></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridTemplateColumn HeaderText="Run Id" UniqueName="RunId" HeaderStyle-Width="90px">
                    <ItemTemplate><a href="javascript:openNewPopupWindow('run', '<%# String.Format("/Job/Job.aspx?jobID={0}&csid=xx", ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).RunId) %>', 1000, 1400, 'yes')"><%# ((Orchestrator.Repositories.DTOs.FindPhotoRow)(Container.DataItem)).RunId %></a></ItemTemplate>
                </telerik:GridTemplateColumn>

                <telerik:GridBoundColumn HeaderText="Comments" DataField="PhotoComment"></telerik:GridBoundColumn>

                <telerik:GridButtonColumn HeaderText="Action" HeaderStyle-Width="60" ItemStyle-HorizontalAlign="Center" ButtonType="PushButton" CommandName="remove" Text="Remove" ConfirmText="Are you sure you want to remove this photo?"></telerik:GridButtonColumn>

            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>