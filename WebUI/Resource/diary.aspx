<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="diary.aspx.cs" Inherits="Orchestrator.WebUI.Resource.diary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
<script language="javascript" src="/script/jquery.quicksearch.pack.js" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset>
        <h1>Options</h1>
        <div>
            <telerik:RadDatePicker ID="dteStartDate" runat="server"></telerik:RadDatePicker>
            <telerik:RadDatePicker ID="dteEndDate" runat="server"></telerik:RadDatePicker>
            <asp:Button ID="btnGetdata" runat="server" Text="Show" />
        </div>
        <div></div>
    </fieldset>
    <telerik:RadGrid runat="server" ID="grdDiary" AutoGenerateColumns="false">
        <MasterTableView>
            <Columns>
                <telerik:GridBoundColumn HeaderStyle-Width="120" DataField="FirstNames" HeaderText="First name" UniqueName="firstnames"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderStyle-Width="120" DataField="LastName" HeaderText="Last name" UniqueName="lastnames"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="OrderDetails" HeaderText="Doing" UniqueName="doing"></telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
    
    <script type="text/javascript">
        $(document).ready(function() {
            $('.summaryOrder').bind("click", function() {
                if ($(this).children('img').attr("src") == "/images/expand.jpg")
                    $(this).children('img').attr("src", "/images/collapse.jpg");
                else
                    $(this).children('img').attr("src", "/images/expand.jpg");
                $(this).next().toggle();
            });

            $('.summaryOrder').bind("mouseenter", function() {
                $(this).css("cursor", "hand");
            });
            $('.summaryOrder').bind("mouseleave", function() {
                $(this).css("cursor", "default");
            });

            
        });

        function viewOrder(orderID) {
            var url = "/Groupage/ManageOrder.aspx?wiz=true&oID=" + orderID;

            var wnd = window.open(url, "Order", "width=1180, height=900, resizable=1, scrollbars=1");
        }
        function viewManifest(resourceManifestID) {
            if (resourceManifestID > 0)
                window.open("/manifest/viewmanifest.aspx?rmID=" + resourceManifestID + "&excludeFirstLine=false&extraRows=0&usePlannedTimes=false&useScript=true");
        }

    </script>
</asp:Content>
