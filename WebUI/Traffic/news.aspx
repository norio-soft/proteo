<%@ Page Language="C#" AutoEventWireup="true" Inherits="Traffic_news"  Codebehind="news.aspx.cs" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body style="margin-top:0px; margin-bottom:0px;background-image:url(<%=Page.ResolveUrl("~/images/header_bg_1.gif")%>);" scroll="no">
    <form id="form1" runat="server">
    <div style="font-family:Tahoma; font-size:10px;color:White; ">
                            <telerik:RadRotator 
                            ScrollDirection="left" 
                            DataMember="item"
                            ScrollSpeed="30"
                            UseSmoothScroll="false"
                            FrameTimeout="1"
                            width="100%"
                            Height="45" 
                            TransitionType="scroll" 
                            FramesToShow="1"
                            id="Rotator1" 
                            runat="server">
                                <FrameTemplate>
                                    <table cellspacing="0" border="0" >
                                        <tr><td style="font-size:10px;"><a target="_blank" style="color:White;" href="<%# DataBinder.Eval(Container.DataItem, "[link]") %>"><%# DataBinder.Eval(Container.DataItem, "[pubdate]") %></a></td>
                                        <td style="font-size:10px;"><%# DataBinder.Eval(Container.DataItem, "[title]") %></td></tr>
                                        <tr><td style="font-size:10px;" colspan="2"><%# DataBinder.Eval(Container.DataItem, "[description]") %></td></tr>
                                    </table>
                                </FrameTemplate>
                            </telerik:RadRotator>
                       

            </div>
    </form>
</body>
</html>
