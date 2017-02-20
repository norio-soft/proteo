<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.Master" AutoEventWireup="True" CodeBehind="ListpcvforJob.aspx.cs" Inherits="Orchestrator.WebUI.PCV.ListpcvforJob" Title="Haulier Enterprise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">PCV's For Job</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <script language="javascript" type="text/javascript" src="/script/jquery.fixedheader.js"></script>

    <h3>PCVs Attached to Job</h3>
    <asp:ListView ID="lvPCVs" runat="server">
        <LayoutTemplate>
            <table cellpadding="0" cellspacing="0" id="pcvs" width="100%">
                <thead>
                    <tr class="HeadingRow">
                        <th>PCV ID</th>
                        <th>Voucher No</th>
                        <th>Delivery Point</th>
                        <th>PalletType</th>
                    </tr>
                </thead>
                <tbody>
                    <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
                </tbody>
            </table>
        </LayoutTemplate>
        <ItemTemplate>
            <tr class="Row">
                <td><%# ((System.Data.DataRowView)Container.DataItem)["PCVID"].ToString() %></td>
                <td><%# ((System.Data.DataRowView)Container.DataItem)["VoucherNo"].ToString() %></td>
                <td><%# ((System.Data.DataRowView)Container.DataItem)["DeliveryPoint"].ToString() %></td>
                <td><%# ((System.Data.DataRowView)Container.DataItem)["PalletType"].ToString() %></td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
    
    <div class="buttonBar">
        <asp:Button ID="btnClose" runat="server" Text="Close" />
    </div>
    
    <script language="javascript" type="text/javascript">
        $(document).ready(function() {
            $("#pcvs").fixedHeader({
                width: $('#pcvs').width(), height: 175
            });
        })
    
    </script>

</asp:Content>