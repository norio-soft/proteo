<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" AutoEventWireup="true" Inherits="Groupage_DeliveryNote" Title="Delivery List" Codebehind="DeliveryNote.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    
    <table id="tblDeliveries" runat="server" cellspacing="0" cellpadding="2" style="width:100%">
        <tr>
            <td>
                <asp:Label ID="lblNotice" runat="server" Text="The display is in Delivery order" />
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadGrid runat="server" ID="grdDeliveries" Skin="Office2007" AutoGenerateColumns="false" AllowMultiRowSelection="true">
                    <MasterTableView DataKeyNames="CollectDropID" Width="100%">
                        <Columns>
                            <telerik:GridClientSelectColumn uniquename="checkboxSelectColumn" HeaderStyle-HorizontalAlign="Left" HeaderStyle-Width="40" HeaderText="" ></telerik:GridClientSelectColumn>
                            <telerik:GridBoundColumn HeaderText="Order Number" DataField="CustomerOrderNumber"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Destination" DataField="Description"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Collecting Order" DataField="OrderID" UniqueName="OrderID"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="Leg Order" DataField="LegOrder" Display="false"></telerik:GridBoundColumn>
                            <telerik:GridBoundColumn HeaderText="CollectDropID" DataField="CollectDropID" Display="false"></telerik:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                    <ClientSettings>
                        <Selecting AllowRowSelect="true" />
                        <ClientEvents OnRowSelected="RowSelected" OnRowDeselected="RowDeSelected" />
                    </ClientSettings>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>
    
    <div class="buttonbar" style="height:22px; bottom:auto;" >
        <asp:Button ID="btnCreate" runat="server" Text="Create" Enabled="false" style="float:right; width:100px;" />
    </div>
    
    <script type="text/javascript" language="javascript">
        var totalSelected = 0;

        function RowSelected(row)
        {
            totalSelected++;
            var btn = document.getElementById("<%=btnCreate.ClientID %>")
            btn.disabled = false;
        }
        
        function RowDeSelected(row)
        {
            totalSelected--;
            var btn = document.getElementById("<%=btnCreate.ClientID %>")
            btn.disabled = totalSelected < 1;
        }
    </script>
    
</asp:Content>

