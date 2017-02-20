<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="feedbackPanel.ascx.cs" Inherits="Orchestrator.WebUI.UserControls.feedbackPanel" %>
<div style="border:solid 1pt #666666;background-color:#F4F4F4;width:500px;margin:5px;">
    <div style="float:left; padding:15px;">
        <asp:Image ID="imgError" runat="server" ImageUrl="~/images/ico_critical.gif" />
    </div>
    <div style="float:left; padding:15px;">
        <asp:Label ID="lblFeedbackMessage" runat="server" Font-Size="11pt"></asp:Label>
    </div>
</div>