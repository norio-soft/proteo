<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageMultiTrunk.aspx.cs" Inherits="Orchestrator.WebUI.Traffic.ManageMultiTrunk" MasterPageFile="~/default_tableless.master" Title="Haulier Enterprise" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
    <!--
        function openWindow(url, title) {
            window.open(url,'','width=550, height=500, location=no, left=30');
        }
    //-->
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Multi-Trunks for Your Organisation</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">

            

    
    <telerik:RadGrid runat="server" ID="multiTrunksRadGrid" AllowSorting="False" AutoGenerateColumns="false" >
        <MasterTableView DataKeyNames="" >
            <Columns>
                <telerik:GridTemplateColumn UniqueName="multiTrunkDescription" HeaderText="Multi-Trunk Description">
                    <ItemTemplate>
                        <a href="javascript:openWindow('addUpdateMultiTrunk.aspx?multiTrunkId=<%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).MultiTrunkId.ToString() %>','Multi-Trunk Maintenance',550,800);">
                    <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).Description %>
                    </a>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="multiTrunkPointDescription" ItemStyle-Width="400px" HeaderText="Point Descriptions">
                    <ItemTemplate>
                        <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).HtmlTableFormattedTrunkPointDescriptionsWithTravelTimes %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="isEnabled" HeaderText="Is Enabled">
                    <ItemTemplate>
                        <asp:CheckBox ID="chkIsEnabled" runat="server" Enabled="false" Text="" Checked='<%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).IsEnabled %>' />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="createDate" HeaderText="Create Date">
                    <ItemTemplate>
                        <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).CreateDate.ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="createUserId" HeaderText="Create User">
                    <ItemTemplate>
                        <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).CreateUserId.ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="lastUpdateDate" HeaderText="Update Date" >
                    <ItemTemplate>
                        <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).LastUpdateDate.ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="lastUpdateUserId" HeaderText="Update User">
                    <ItemTemplate>
                        <%# ((Orchestrator.Entities.MultiTrunk)Container.DataItem).LastUpdateUserId.ToString() %>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>                 
        </MasterTableView>
    </telerik:RadGrid>
    
    <div class="buttonbar">
        <input type="button" value="Add Multi-Trunk" onclick="javascript:openWindow('addUpdateMultiTrunk.aspx','Multi-Trunk Maintenance');" />
        <asp:button id="btnRefresh" runat="server" text="Refresh" Width="75" />
    </div>
</asp:Content>