<%@ Page Language="C#" MasterPageFile="~/WizardMasterPage.master" AutoEventWireup="true" Inherits="Orchestrator.WebUI.RelateClient" Title="Relate a Customer" Codebehind="relateclient.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <script language="javascript" type="text/javascript">
 
    </script>
    <fieldset style="padding: 0px; margin-top: 5px; margin-bottom: 5px; font-size:11px;padding-bottom:3px;">
        <div style="height: 22px; border-bottom: solid 1pt silver; padding: 2px; margin-bottom: 5px;
            color: #ffffff; background-color: #5d7b9d; font-size:11px">
            Relate Clients</div>
            This allows you add an existing organisation (client or customer) to another client.
        
    </fieldset>
    <br />
        <div style="width:520px; margin-bottom:5px;font-size:11px;">
            <asp:Label ID="lblRelated" runat="server" Text="Add the Organisation to this Clients <b>[{0}]</b> list of customers."></asp:Label>
        </div>
        
        <div style="width:480px">
            <div style="float:left; width:100px;font-size:11px;">Select Client</div>
            <div style="float:right; width:355px; text-align:left">
                <telerik:RadComboBox ID="cboClient" runat="server" DataTextField="OrganisationName" DataValueField="IdentityId" EnableLoadOnDemand="true" ItemRequestTimeout="500" MarkFirstMatch="true" RadControlsDir="~/script/RadControls/" AllowCustomText="False" ShowMoreResultsBox="false" Skin="Orchestrator" SkinsPath="~/RadControls/ComboBox/Skins/" Width="343px" Height="300px"></telerik:RadComboBox>
                <asp:RequiredFieldValidator ID="rfvClient" runat="server" ControlToValidate="cboClient" >
                    <img src="/images/Error.gif" height="16" width="16" title="Please select a Client" alt="" />
                </asp:RequiredFieldValidator>

            </div>
        </div>
        <div style="height:22px; margin-top:5px;padding:2px;color:#ffffff; background-color:#99BEDE;text-align:right;margin-top:135px;">            
            <asp:Button id="btnRelate" Text="OK" Width="75" runat="server" />
            <asp:Button id="btnCancel" Text="Cancel" Width="75" runat="server" />
        </div>
  
        <asp:Label ID="lblInjectScript" runat="server"></asp:Label>
</asp:Content>

