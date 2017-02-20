<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.wpTrafficNews" Codebehind="wpTrafficNews.ascx.cs" %>
<div>
<telerik:RadRotator id="Rotator1" runat="server" FrameTimeout="9000" FramesToShow="2"  TransitionType="scroll" height="180px" ItemHeight="90px" width="100%" ItemWidth="100%" ScrollDirection="Up">
    <ItemTemplate>
        <div><a style="font-size:11px;" target="_blank" href="<%# DataBinder.Eval(Container.DataItem, "[link]") %>"><%# DataBinder.Eval(Container.DataItem, "[title]") %></a></div>
        <div style="font-size:11px;color:Gray"><%# DataBinder.Eval(Container.DataItem, "[pubdate]") %> </div>
        <div style="font-family:Verdana; font-size:11px;">
           <%# DataBinder.Eval(Container.DataItem, "[description]") %>
        </div>
    </ItemTemplate> 
</telerik:RadRotator>
</div>