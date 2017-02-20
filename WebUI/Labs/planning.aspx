<%@ Page Title="" Language="C#" MasterPageFile="~/default_tableless.Master" AutoEventWireup="true" CodeBehind="planning.aspx.cs" Inherits="Orchestrator.WebUI.Labs.planning" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Header" runat="server">
   <link rel="stylesheet" href="http://code.jquery.com/ui/1.10.2/themes/smoothness/jquery-ui.css" />
   <script src="http://code.jquery.com/ui/1.10.2/jquery-ui.js"></script>
    <script src="/script/jquery.layout-latest.min.js"></script>
    <style>
    #sortable { list-style-type: none; margin: 0; padding: 0; width: 60%; }
    #sortable li { margin: 0 3px 3px 3px; padding: 0.4em; padding-left: 1.5em; font-size: 1.4em; height: 18px; }
    #sortable li span { position: absolute; margin-left: -1.3em; }

    /**
	 *	Basic Layout Theme
	 * 
	 *	This theme uses the default layout class-names for all classes
	 *	Add any 'custom class-names', from options: paneClass, resizerClass, togglerClass
	 */

	.ui-layout-pane { /* all 'panes' */ 
		background: #FFF; 
		border: 1px solid #BBB; 
		padding: 10px; 
		overflow: auto;
	} 

	.ui-layout-resizer { /* all 'resizer-bars' */ 
		background: #DDD; 
	} 

	.ui-layout-toggler { /* all 'toggler-buttons' */ 
		background: #AAA; 
	} 



  </style>
  <script>
      $(function () {
          $("#sortable").sortable();
          $("#sortable").disableSelection();
      });

      var myLayout; // a var is required because this page utilizes: myLayout.allowOverflow() method

      $(document).ready(function () {
          myLayout = $('.masterpagelite_contentHolder').layout({
              // enable showOverflow on west-pane so popups will overlap north pane
              west__showOverflowOnHover: true

              //,	west__fxSettings_open: { easing: "easeOutBounce", duration: 750 }
          });
      });


  </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderPlaceHolder1" runat="server">
    Drag And Drop Planning - DEMO
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="ui-layout-west">
	  <ul id="sortable">
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 1</li>
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 2</li>
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 3</li>
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 4</li>
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 5</li>
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 6</li>
        <li class="ui-state-default"><span class="ui-icon ui-icon-arrowthick-2-n-s"></span>Order 7</li>
    </ul>
</div>

<div class="ui-layout-center">
	<p>resource timeline</p>
</div>


  
</asp:Content>
