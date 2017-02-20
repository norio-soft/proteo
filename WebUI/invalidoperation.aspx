<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.invalidoperation" MasterPageFile="~/default_tableless.master" Codebehind="invalidoperation.aspx.cs" %>
<asp:Content runat="server" ContentPlaceHolderID="ContentPlaceHolder1">
    <div style="background-color:White; padding:10px;">
        
        <div style="margin-left:70px; margin-top:30px; width:900px; color:#A60000;" >
            <img src="./images/exception.gif" />
            <p>There was a problem trying to action your last request, this could have been caused by others making changes to the information you requested  or this could be a temporary internet connection issue.</p>
            <p>If this is the first time that you have seen this screen whilst trying to perform this task can you briefly describe what you were trying to do and include any reference numbers (Job Id, Driver etc). This information will allow us to better understand why this problem might have happened.</p>
            <p>With this information we may be able to make changes to prevent the problem happening again.</p>
            <p>If this problem persists please contact P1 Technology Partners on 0845 644 3720.</p>
            <p>
                <asp:TextBox ID="txtMoreInformation" runat="server" TextMode="MultiLine" Columns="100" Rows="5" style="width:100%"></asp:TextBox>
                <asp:label ID="lblThankYou" runat="server" Visible="false" Text="Thank you for supplying more information."></asp:label>
            </p>
            <span style="text-align:right; width:100%;"><asp:Button ID="btnSend" runat="server" Text="Send Information" /></span><br />
            <asp:HyperLink ID="hlReturn" runat="server" Text="Click here to return to your last page" NavigateUrl="#" Visible="false"></asp:HyperLink>
        </div>
    </div>
    <div>
        <!--<asp:PlaceHolder Id="errorDetails" Runat="server"></asp:PlaceHolder>-->
    </div>
</asp:Content >