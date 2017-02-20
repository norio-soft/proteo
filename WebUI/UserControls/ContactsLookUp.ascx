<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ContactsLookUp.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.ContactsLookUp" %>

<telerik:RadCodeBlock runat="server" ID="rdb1">
    <h1><%=(IsCalledFromHotKey) ? "Contacts for client/sub contractor" : (IsClient) ? "Contacts for client" : "Contacts for sub contractor"%></h1>
</telerik:RadCodeBlock>
<telerik:RadAjaxPanel id="RadAjaxPanel1" runat="server">
    <fieldset>
        <table>
            <tr>
                <td class="formCellLabel"><asp:Label ID="clientLabel" runat="server" Text=""></asp:Label></td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboClient" runat="server" AutoPostBack="true" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="orchestrator" Width="355px" Height="200px" HighlightTemplatedItems="true"></telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="formCellLabel"><asp:Label ID="contactLabel" runat="server" Text="Select contact"></asp:Label></td>
                <td class="formCellField">
                    <telerik:RadComboBox ID="cboIndividualContacts" runat="server" AutoPostBack="true" EnableLoadOnDemand="true" ItemRequestTimeout="500"
                        MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False"
                        ShowMoreResultsBox="false" Skin="orchestrator" Width="355px" Height="150px" HighlightTemplatedItems="true">   
                        <HeaderTemplate>
                            <table style="width:355px; text-align:left;">
                                <tr><th style="width:65%;">Contact Name</th><th style="width:35%;">Contact Type</th></tr>
                            </table>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <table style="width:345px; text-align:left;">
                                <tr>
                                    <td style="width:65%;">
                                        <%#DataBinder.Eval(Container.DataItem, "FullName")%>
                                    </td>
                                    <td style="width:35%;">
                                        <%#DataBinder.Eval(Container.DataItem, "ContactType")%>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                    </telerik:RadComboBox>
                </td>
            </tr>
        </table>
    </fieldset>
    <telerik:RadGrid runat="server" ID="grdContacts" Width="100%" AutoGenerateColumns="false" >
        <MasterTableView>
            <Columns>
             <telerik:GridBoundColumn UniqueName="Email" DataField="Email" HeaderText="Email" ItemStyle-Width="160">
             </telerik:GridBoundColumn>
             <telerik:GridBoundColumn UniqueName="Telephone" DataField="Telephone" HeaderText="Telephone" ItemStyle-Width="75">
             </telerik:GridBoundColumn>
             <telerik:GridBoundColumn UniqueName="Fax" DataField="Fax" HeaderText="Fax" ItemStyle-Width="75">
             </telerik:GridBoundColumn>
             <telerik:GridBoundColumn UniqueName="MobilePhone" DataField="MobilePhone" HeaderText="Mobile" ItemStyle-Width="75">
             </telerik:GridBoundColumn>
             <telerik:GridBoundColumn UniqueName="PersonalMobile" DataField="PersonalMobile" HeaderText="Personal Mobile" ItemStyle-Width="100">
             </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>    
</telerik:RadAjaxPanel> 
