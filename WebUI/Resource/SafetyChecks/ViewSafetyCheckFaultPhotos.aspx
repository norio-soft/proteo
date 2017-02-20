<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ViewSafetyCheckFaultPhotos.aspx.cs" Inherits="Orchestrator.WebUI.Resource.SafetyChecks.Photos" MasterPageFile="~/WizardMasterPage.master" %>

<asp:content ContentPlaceHolderID="HeaderContentFromChildPage" runat="server">
    <style>
        .imageWrapper {
            padding:5px;
            width:410px;
        }

        .imageWrapper > img { 
             display: inline-block; 
             border-style: solid;
             border-color: #222;
             border-width: 1px !important;
         }
    </style>
</asp:content>

<asp:Content ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <div class="imageWrapper" id="imageWrapper" runat="server"></div>

</asp:Content>
