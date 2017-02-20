<%@ Page Language="C#" AutoEventWireup="true" Inherits="Groupage_LOADsHEET" Codebehind="loadsheet.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Load Sheet Details</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div>
    
        <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
            <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Load Sheet Details</div>
             <telerik:RadGrid ID="grdLoadSheet" runat="server" Skin="Office2007" AutoGenerateColumns="false">
                <MasterTableView>
                    <Columns>
                        <telerik:GridBoundColumn DataField="Seq" HeaderText=""></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="CustomerOrderNumber" HeaderText="Order Number" UniqueName="CustomerOrderNumber"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="DeliveryOrderNumber" HeaderText="Consignment Number"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Cases" HeaderText="Cartons"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="NoPallets" HeaderText="Pallets"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="OrderID" HeaderText="ID"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Customer" HeaderText="Customer"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Order Delivery Point" HeaderText="Destination"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Destination Town" HeaderText="Destination"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Checked" HeaderText="Checked"></telerik:GridBoundColumn>
                        <telerik:GridBoundColumn DataField="Cross Docked Ex" HeaderText="Cross Dock Ex"></telerik:GridBoundColumn>
                    </Columns>
                </MasterTableView>
            </telerik:RadGrid>
        </fieldset>
        
        <fieldset>
            <legend>Driver Instructions [Will Appear on the Load Sheet]</legend>
            <asp:Label ID="lblUpdateConfirmation" runat="server" Text="The Instructions have been updated." style="border:solid 1pt black; color:Blue; font-size:11px;" Visible="false"></asp:Label>
            <asp:TextBox ID="txtDriverNotes" runat="server" TextMode="MultiLine" Rows="5" Width="100%">
            </asp:TextBox>
        </fieldset>
        
        <div class="buttonbar">
            <asp:Button ID="btnUpdateDriverInstructions" runat="server" Text="Save Instructions" />
            <asp:Button ID="btnPrintLoadSheet" runat="server" Text="Print" Width="75" />
            <asp:button ID="btncancel" runat="server" Text="Back" Width="75"/>
        </div>
        
    </div>
    
</asp:Content>