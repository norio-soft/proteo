<%@ Page Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.OfflineProcessInitiationFailed" MasterPageFile="~/default_tableless.master" Title="Process Initiation Failed" Codebehind="OfflineProcessInitiationFailed.aspx.cs" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Process Failed</h1></asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <fieldset style="padding:0px;margin-top:5px; margin-bottom:5px; background-color:Yellow; ">
        <div style="height:22px; border-bottom:solid 1pt silver;padding:2px;margin-bottom:5px; color:#ffffff; background-color:#5d7b9d;">Your request is not being actioned.</div>
        <img src="~/images/ico_critical.gif" alt="Not possible to complete your request." runat="server" />
        <p>
            The task you have asked Orchestrator to perform could <b>not</b> be submitted for background processing.
            <br /><br />
            Your support team have been sent an email to notify them of the problem.
        </p>
    </fieldset>
</asp:Content>
