<%@ Page language="c#" MasterPageFile="~/default_tableless.master" Title="Take PCVs on a Run" Inherits="Orchestrator.WebUI.Traffic.JobManagement.takePCVs" Codebehind="takePCVs.aspx.cs" %>
<%@ Register TagPrefix="uc1" TagName="TakePCVs" Src="~/UserControls/TakePCVs.ascx" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Attach PCVs on a Run</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Specify which PCVs to take on the Run</h2>
	<uc1:TakePCVs id="takePCVs1" runat="server"></uc1:TakePCVs>
</asp:Content>