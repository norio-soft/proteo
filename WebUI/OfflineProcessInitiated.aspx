<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.OfflineProcessInitiated" MasterPageFile="~/default_tableless.master" Title="Process Initiated" Codebehind="OfflineProcessInitiated.aspx.cs" %>

<asp:Content ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Process Initiated</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px;">
        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Your request is being actioned.</div>
        <p>
            The task you have asked Orchestrator to perform has been submitted for background processing.
        </p>
        <br />
        <p>
            You will receive an email when the process has been completed and your task can be continued.
        </p>
    </fieldset>
</asp:Content>