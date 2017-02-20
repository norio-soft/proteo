<%@ Page Language="C#" MasterPageFile="~/default_tableless.master" AutoEventWireup="True" Inherits="Orchestrator.WebUI.Organisation.MergeClientCustomers" Title="Merge Client Customers" CodeBehind="MergeClientCustomers.aspx.cs" %>
<%@ Register TagPrefix="uc" Namespace="Orchestrator.WebUI.Controls" Assembly="WebUI" %>

<asp:Content ID="Content2" ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">

        function cboOrganisation_OnSelectedIndexChanged(sender, eventArgs) {
            try {
                var divIdentityId = $('div[id*=divIdentityId]')[0].innerText = 'Identity Id: ' + eventArgs.get_item().get_value();
            }
            catch (err) { }
        }

        function giveWarning() {
            return confirm("WARNING! Merging client customers is an irreversible action. The selected 'Merge From' client customers will be irretrievably deleted; are you sure you would like to proceed?");
        }

        function removeMessage() {
            $('span[id*=lblMessage]')[0].innerText = '';
        }

    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Merge Client Customers</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div>
        <h3>Client Customer to Merge To</h3>
        <fieldset>
            <telerik:RadComboBox ID="cboOrganisation" runat="server" EnableLoadOnDemand="true"
                ItemRequestTimeout="500" AutoPostBack="false" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/"
                ShowMoreResultsBox="true" Skin="WindowsXP" Width="400px" OnClientSelectedIndexChanged="cboOrganisation_OnSelectedIndexChanged">
            </telerik:RadComboBox>
            <div style="height: 10px;">
            </div>
            <div id="divIdentityId">
            </div>
        </fieldset>
    </div>
    
    <h3>Client Customers to Merge</h3>
    <div>
        <br />
        <asp:TextBox runat="server" ID="txtClientCustomerFiler"></asp:TextBox><asp:Button
            ID="btnFilter" Text="Filter" OnClientClick="removeMessage" runat="server" />
        <telerik:RadGrid Width="400" ID="repClientCustomers" runat="server" AllowAutomaticUpdates="True"
            AllowSorting="True" Skin="Office2007" AutoGenerateColumns="False" EnableAJAXLoadingTemplate="True"
            LoadingTemplateTransparency="50" GridLines="None">
            <clientsettings>
                <Selecting AllowRowSelect="True" />
            </clientsettings>
            <mastertableview name="ClientCustomers" allowmulticolumnsorting="True">
                <Columns>
                    <telerik:GridBoundColumn HeaderText="Client Customer Name" DataField="OrganisationName"
                        ReadOnly="True" UniqueName="OrganisationName" HeaderStyle-Width="300">
                    </telerik:GridBoundColumn>
                    <telerik:GridBoundColumn HeaderText="Identity Id" DataField="IdentityId" ReadOnly="True"
                        UniqueName="IdentityId" HeaderStyle-Width="50">
                    </telerik:GridBoundColumn>
                    <telerik:GridTemplateColumn UniqueName="chkReference" HeaderText="Merge" HeaderStyle-Width="20">
                        <ItemTemplate>
                            <asp:CheckBox ID="chkRow" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </mastertableview>
        </telerik:RadGrid>
        <div class="buttonbar">
            <asp:Button ID="btnMergeClientCustomers" runat="server" Text="Merge Client Customers"
                OnClientClick="removeMessage();if(!giveWarning()) {return false;} else { return true; }" />&nbsp;
            <asp:Label runat="server" ID="lblMessage" ForeColor="Red"></asp:Label>
        </div>
    </div>
    
    <div class="clearDiv"></div>
</asp:Content>