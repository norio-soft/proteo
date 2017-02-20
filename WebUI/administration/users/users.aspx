<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.administration.users.Users" MasterPageFile="~/default_tableless.master" Codebehind="users.aspx.cs" Title ="Users" %>

<%@ Register Assembly="Orchestrator.WebUI.Dialog" Namespace="Orchestrator.WebUI" TagPrefix="cc1" %>

<asp:Content ContentPlaceHolderID="Header" runat="server">
    <script language="javascript" type="text/javascript">
    <!--   
	    function ShowUser(identityId)
	    {
	        var qs = 'identityId=' + identityId;
	        <%=dlgUser.ClientID %>_Open(qs);
	    }
	    
	    function AddUser()
	    {
	        var qs = "IsClient=<%=m_isClient %>";
	        <%=dlgUser.ClientID %>_Open(qs);
	    }
    //-->
    </script>
</asp:Content>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1><%= m_isClient ? "Client Portal Users" : "Users" %></h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <cc1:Dialog ID="dlgUser" URL="addupdateuser.aspx" Width="640" Height="480" AutoPostBack="true" Mode="Modal" runat="server" ReturnValueExpected="true"></cc1:Dialog>
    
    <div class="buttonbar" style="margin-bottom:5px;">
        <div style="display: <%= m_isClient ? "none" : "" %> !important;">
	        <p style="font-size: 12px; font-weight: bold; padding-top: 3px; display: <%= this.TotalFullTimeLicenseCount > 0 ? "" : "none" %>;">Number of active full time licenses: <%= this.ActiveFullTimeLicenseCount %> of <%= this.TotalFullTimeLicenseCount %></p>
	        <p style="font-size: 12px; font-weight: bold; padding-top: 3px; display: <%= this.TotalPartTimeLicenseCount > 0 ? "" : "none" %>;">Number of active part time licenses: <%= this.ActivePartTimeLicenseCount %> of <%= this.TotalPartTimeLicenseCount %></p>
	        <p style="font-size: 12px; font-weight: bold; padding-top: 3px;">If you need to add a new user please contact the Proteo Service Desk on 0845 644 3720.</p>
        </div>
        <div style="display: <%= m_isClient ? "" : "none" %> !important;">
            <input type="button" onclick="AddUser();" value="Add Client Portal User" />
        </div>
    </div> 

    <telerik:RadGrid runat="server" ID="grdUsers" AutoGenerateColumns="false" GroupingEnabled="true">
        <MasterTableView DataKeyNames="IdentityId, OrganisationId" GroupsDefaultExpanded="false" >
            <Columns>
                <telerik:GridBoundColumn HeaderText="Client" DataField="OrganisationName"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="First Name" DataField="Firstnames"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Last Name" DataField="LastName"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn HeaderText="Status" DataField="UserStatus"></telerik:GridBoundColumn>
                <telerik:GridHyperLinkColumn DataTextFormatString="Click to Update '{0}'"
                DataNavigateUrlFields="IdentityId" UniqueName="OrganisationName" DataNavigateUrlFormatString="javascript:ShowUser({0});" HeaderText="Action" DataTextField="UserName">
                </telerik:GridHyperLinkColumn>
            </Columns>
            <GroupByExpressions>
                <telerik:GridGroupByExpression>
                    <SelectFields>
                        <telerik:GridGroupByField FieldAlias="Client" FieldName="OrganisationName" />
                    </SelectFields>
                    <GroupByFields>
                        <telerik:GridGroupByField FieldAlias="Client" FieldName="OrganisationName" ></telerik:GridGroupByField>
                    </GroupByFields>
                </telerik:GridGroupByExpression>
            </GroupByExpressions>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </telerik:RadGrid>
                    
    <asp:button id="btnTest" runat="server" text="test" visible="false" />
</asp:Content>