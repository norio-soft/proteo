<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="ListWindows.aspx.cs" Inherits="Orchestrator.WebUI.administration.DeliveryWindow.ListWindows" %>

<asp:Content ID="contentTitle" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    <h1>Delivery Windows
    </h1>

</asp:Content>

<asp:Content ID="contentHeader" ContentPlaceHolderID="Header" runat="server">
    <script type="text/javascript" src="/script/jquery-ui-1.9.2.min.js"></script>
    <style type="text/css">
        .formCellLabel {
            width: 200px;
        }
    </style>
</asp:Content>

<asp:Content ID="contentBody" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlListWindows" runat="server">
        <fieldset>
            <legend>Collection / Delivery Windows</legend>

            <telerik:RadGrid ID="RadGrid1" OnSortCommand="RadGrid1_SortCommand" OnPageIndexChanged="RadGrid1_PageIndexChanged"
                Width="97%" OnPageSizeChanged="RadGrid1_PageSizeChanged" AllowSorting="True"
                PageSize="15" AllowPaging="True" AllowMultiRowSelection="True" runat="server"
                GridLines="None" AutoGenerateColumns="false">
                <MasterTableView Width="30%" Summary="RadGrid table">
                    <Columns>
                        <telerik:GridHyperLinkColumn
                            HeaderText="Description" DataTextField="Description" DataNavigateUrlFields="DeliveryWindowMatrixId" UniqueName="EditLink" DataNavigateUrlFormatString="Matrix.aspx?ID={0}&Mode=Edit" Text="Edit">
                        </telerik:GridHyperLinkColumn>
                        <telerik:GridBoundColumn HeaderText="Zone" DataField="Zone"></telerik:GridBoundColumn>
                        <telerik:GridHyperLinkColumn
                            DataNavigateUrlFields="DeliveryWindowMatrixId" UniqueName="DeleteLink" DataNavigateUrlFormatString="javascript:deleteMatrix({0});" Text="Delete">
                        </telerik:GridHyperLinkColumn>
                    </Columns>

                </MasterTableView>
                <PagerStyle Mode="NextPrevAndNumeric"></PagerStyle>
            </telerik:RadGrid>
        </fieldset>
        <div class="buttonBar">
            <input type="button" id="btnCreateDeliveryWindow" value="Add New" />
        </div>
    </asp:Panel>

    <div id="createDeliveryWindowPopup" style="display: none;">
        <table>
            <tr>
                <td>Description</td>
                <td>
                    <asp:TextBox runat="server" ID="txtDescription"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" ID="rfvDescription" ControlToValidate="txtDescription" Text="Description cannot be blank." ValidationGroup="vgCreateDeliveryWindow" />
                </td>
            </tr>
            <tr>
                <td>Zone Map</td>
                <td>
                    <asp:DropDownList runat="server" ID="cboZoneMap" DataValueField="ZoneMapID" DataTextField="Description"></asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ID="rfvZoneMap" ControlToValidate="cboZoneMap" Text="Please select a zone map." ValidationGroup="vgCreateDeliveryWindow" />
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        $('#createDeliveryWindowPopup')
            .dialog({
                title: 'Create Delivery Window',
                autoOpen: false,
                modal: true,
                resizable: false,
                buttons: {
                    'OK': function () {
                        if (!Page_ClientValidate('vgCreateDeliveryWindow')) {
                            return;
                        }

                        PageMethods.CreateDeliveryWindow(
                            $('[id$=txtDescription]').val(),
                            $('[id$=cboZoneMap]').val(),
                            function (result) {
                                window.location.href = 'matrix.aspx?id=' + result;
                            },
                            function (error) {
                                var msg = 'Error creating delivery window';
                                if (error.get_message) {
                                    msg += ':\n\n' + error.get_message();
                                }
                                alert(msg);
                            });
                    },
                    'Cancel': function () {
                        // Clear validators
                        Page_ClientValidate('');
                        $(this).dialog('close');
                    }
                }
            })
            .keydown(function (event) {
                // Trigger the OK button on pressing Enter in the dialog
                if (event.keyCode == 13) {
                    $(this).parent().find("button:eq(0)").trigger("click");
                }
            });

        $('#btnCreateDeliveryWindow').click(function () {
            $('#createDeliveryWindowPopup').dialog('open');
        });

        function deleteMatrix(deliveryWindowMatrixID) {
            if (confirm('Delete this delivery window matrix?')) {
                location.href = 'ListWindows.aspx?ID=' + deliveryWindowMatrixID + '&Mode=Delete';
            }
        }
    </script>
</asp:Content>
