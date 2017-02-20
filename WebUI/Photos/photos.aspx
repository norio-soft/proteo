<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="photos.aspx.cs" Inherits="Orchestrator.WebUI.Photos.photos" %>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server"><h1>Photo List</h1></asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
<script src="../script/jquery-ui-1.9.2.min.js" type="text/javascript"></script>
 <script src="../script/jquery.blockUI-2.64.0.min.js" type="text/javascript"></script>
 <script type="text/javascript">
     $(function () {
         var dlg = jQuery('#PhotoDialog').dialog({ autoOpen: false, modal: true, width:830, height:670, dialogClass: "dialogWithDropShadow" });
     });


     function showPhoto(Driver,photoURL, When, Latitude, Longitude) {
         
         $('#PhotoDialog img:first').attr("src", photoURL);
         $('#PhotoDialog #spnDriver').text(Driver);
         $('#PhotoDialog #spnDate').text(When);

         var url = "showLocation('/gps/getcurrentlocation.aspx?lat=" + Latitude + "&lng=" + Longitude + "')";
         var content = "<a href=\"javascript:" + url + "\">Show Where Taken</a><br/>";

         $('#PhotoDialog #showLocation').html(content);
         $('#PhotoDialog').dialog('open');
     }

     function showLocation(url) {
         newwindow = window.open(url, 'name', 'height=550,width=600,toolbar=no,menu=no,scrollbars=no');
         if (window.focus) { newwindow.focus() }
     }
 </script>
 <style type="text/css">
  .dialogWithDropShadow
        {
            -webkit-box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.5);  
            -moz-box-shadow: 0px 0px 10px rgba(0, 0, 0, 0.5); 
        }
 </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
Please select the Photo Date <asp:DropDownList ID="cboPictureDates" runat="server" AutoPostBack="true" DataTextFormatString="{0:dd/MM/yyyy}"></asp:DropDownList>
<asp:ListView runat="server" ID="lvPhotos">
<LayoutTemplate>
    <table runat="server" id="table1" >
      <tr runat="server" id="itemPlaceholder" ></tr>
    </table>
  </LayoutTemplate>
  <ItemTemplate>
    <tr id="Tr1" runat="server">
      <td id="Td1" runat="server">
        <a onclick="showPhoto('<%#Eval("Driver") %>', '<%#Eval("Url") %>', '<%#Eval("Date") %>', <%#Eval("Latitude") %>,<%#Eval("Longitude") %>)">
            <asp:image ID="img" runat="server" ImageUrl='<%#Eval("Url") %>' width="200" Height="150" />
        </a>
        <div><asp:Label ID="NameLabel" runat="server" Text='<%#Eval("Driver") %>' /></div>
      </td>
    </tr>
  </ItemTemplate>
  <EmptyDataTemplate>
    <h3>No photos to show for the selected date</h3>
  </EmptyDataTemplate>
</asp:ListView>

 <div id="PhotoDialog" style="display:none;background-color:White;">
        <h3>Photo Taken by <span id="spnDriver"></span> at <span id="spnDate"></span></h3>
        <img src="" id="imgPhoto" alt="Photo"/>
        <div id="showLocation"></div>
  </div>
</asp:Content>
