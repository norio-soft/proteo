<%@ Page language="c#" Inherits="Orchestrator.WebUI.Resource.Driver.DriverRequests" Codebehind="DriverRequests.aspx.cs" MasterPageFile="~/WizardMasterPage.Master" Title="Haulier Enterprise" %>
<%@ Register TagPrefix="P1" Namespace="P1TP.Components.Web.UI" Assembly="P1TP.Components" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitlePlaceHolder1" runat="server">Driver Requests</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <script language="javascript" src="/script/scripts.js" type="text/javascript"></script>
    <script language="javascript" src="/script/popaddress.js" type="text/javascript"></script>
    
    <script language="javascript" type="text/javascript">
        <!--
        function EditRequest(requestId) 
        {
            window.location.href = 'AddUpdateDriverRequest.aspx?requestId=' + requestId;
        }
        //-->
    </script>
    
    <div>
        This page shows the requests that <asp:Label id="lblDriver" runat="server" Font-Bold="True"></asp:Label> has made that apply to dates from <asp:Label id="lblStartDate" runat="server" Font-Bold="True"></asp:Label>.<br>You can click on the links to manage that requests' details.
        </br>
        <asp:Label id="lblConfirmation" runat="server" CssClass="confirmation"></asp:Label>
        <P1:PrettyDataGrid id="dgRequests" runat="server" RowHighlightColor="255, 255, 128" BorderColor="#999999" 
            BorderStyle="None" BorderWidth="1px" BackColor="White" CellPadding="6" GridLines="Both" 
            AutoGenerateColumns="False" RowClickEventCommandName="Select" AllowSorting="False" RowSelectionEnabled="False" 
            GroupingColumn="" AllowGrouping="False" GroupRowColor="#FDA16F" GroupForeColor="Black" 
            AllowCollapsing="False" StartCollapsed="False" width="500">
            <SELECTEDITEMSTYLE BackColor="#008A8C" ForeColor="White" Font-Bold="True"></SELECTEDITEMSTYLE>
            <ALTERNATINGITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ALTERNATINGITEMSTYLE>
            <ITEMSTYLE ForeColor="Black" BorderStyle="Dotted" BorderColor="Black"></ITEMSTYLE>
            <HEADERSTYLE BackColor="#A1C0F6" ForeColor="Black" Font-Bold="True" Width="410px"></HEADERSTYLE>
            <FOOTERSTYLE BackColor="#CCCCCC" ForeColor="Black"></FOOTERSTYLE>
            <COLUMNS>
                <asp:TemplateColumn HeaderText="Driver" SortExpression="FullName" ItemStyle-VerticalAlign="Top" ItemStyle-Wrap="False">
                    <ItemTemplate>
                        <%# DataBinder.Eval(Container.DataItem, "FullName") %>
                        <input type="hidden" id="hidRequestId" runat="server" value='<%# DataBinder.Eval(Container.DataItem, "DriverRequestId") %>' name="hidRequestId" />
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:BoundColumn HeaderText="Date" DataField="AppliesUntil" SortExpression="AppliesUntil" ItemStyle-VerticalAlign="Top" DataFormatString="{0:dd/MM/yy}"></asp:BoundColumn>
                <asp:BoundColumn HeaderText="Request" DataField="RequestText" SortExpression="RequestText" ItemStyle-VerticalAlign="Top"></asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Edit" ItemStyle-VerticalAlign="Top">
                    <ItemTemplate>
                        <a href="javascript:EditRequest('<%# DataBinder.Eval(Container.DataItem, "DriverRequestId") %>')">Edit</a>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:ButtonColumn HeaderText="Delete" CommandName="Delete" ButtonType="LinkButton" Text="Delete" ItemStyle-VerticalAlign="Top"></asp:ButtonColumn>
            </COLUMNS>
            <PAGERSTYLE BackColor="#A1C0F6" ForeColor="Black" Mode="NumericPages" HorizontalAlign="Center"></PAGERSTYLE>
        </P1:PrettyDataGrid>
    </div>

</asp:Content>