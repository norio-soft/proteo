<%@ Page Title="" Language="C#" MasterPageFile="~/wizardmasterpage.Master" AutoEventWireup="true" CodeBehind="addupdatepoint2.aspx.cs" Inherits="Orchestrator.WebUI.Point.addupdatepoint2" %>
<%@ Register TagPrefix="p1" TagName="Point" Src="~/UserControls/point.ascx" %>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p1:Point CanClearPoint="false" CanChangePoint="true" runat="server" ID="ucPointCtl" EditMode="true" />
         <div class="buttonbar">
        <input type="button" onclick="self.close();" value="Close"  runat="server" id="btnClose" />
        
    </div>
</asp:Content>
