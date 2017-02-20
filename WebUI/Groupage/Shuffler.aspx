<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Shuffler.aspx.cs" Inherits="Orchestrator.WebUI.Groupage.Shuffler" %>

<!doctype html>
<html lang="en">

<head runat="server">
    <meta charset="utf-8" />

    <title></title>
    <script>
        if (!window.showModalDialog) {
            window.showModalDialog = function (arg1, arg2, arg3) {

                var w;
                var h;
                var resizable = "no";
                var scroll = "no";
                var status = "no";

                // get the modal specs
                var mdattrs = arg3.split(";");
                for (i = 0; i < mdattrs.length; i++) {
                    var mdattr = mdattrs[i].split("=");

                    var n = mdattr[0];
                    var v = mdattr[1];
                    if (n) { n = n.trim().toLowerCase(); }
                    if (v) { v = v.trim().toLowerCase(); }

                    if (n == "dialogheight") {
                        h = v.replace("px", "");
                    } else if (n == "dialogwidth") {
                        w = v.replace("px", "");
                    } else if (n == "resizable") {
                        resizable = v;
                    } else if (n == "scroll") {
                        scroll = v;
                    } else if (n == "status") {
                        status = v;
                    }
                }

                var left = window.screenX + (window.outerWidth / 2) - (w / 2);
                var top = window.screenY + (window.outerHeight / 2) - (h / 2);
                var targetWin = window.open(arg1, arg1, 'toolbar=no, location=no, directories=no, status=' + status + ', menubar=no, scrollbars=' + scroll + ', resizable=' + resizable + ', copyhistory=no, width=' + w + ', height=' + h + ', top=' + top + ', left=' + left);
                targetWin.focus();
            };
        }

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager runat="server"></asp:ScriptManager>
        
        <iframe id="shuffleFrame" runat="server" width="1200px" height="1400px" marginheight="0" marginwidth="0" scrolling="no" style="border-style:none;"></iframe>
        
        <input type="hidden" runat="server" id="hidRecordPagerIds" />
        
        <div id="divRowShuffler" runat="server" class="overlayedDataBox" style="overflow: hidden;">
            <table width="100%" cellpadding="1">
                <tr>
                    <td>
                        <img src="/App_Themes/Orchestrator/Img/MasterPage/icon-arrow-first.png"
                            alt="View first record" id="btnFirst" onclick="javascript:goToFirst();" />
                    </td>
                    <td>
                        <img src="/App_Themes/Orchestrator/Img/MasterPage/icon-arrow-prev.png"
                            alt="View previous record" id="btnPrevious"
                            onclick="javascript:goToPrevious();" />
                    </td>
                    <td style="text-align: center;">
                        <span id="spRowDisplay"  runat="server"></span>
                    </td>
                    <td>
                        <img src="/App_Themes/Orchestrator/Img/MasterPage/icon-arrow-next.png"
                            alt="View next record" id="btnNext" onclick="javascript:goToNext();" />
                    </td>
                    <td>
                        <img src="/App_Themes/Orchestrator/Img/MasterPage/icon-arrow-last.png"
                            alt="View last record" id="btnLast" onclick="javascript:goToLast();"  />
                    </td>
                </tr>
            </table>
        </div>
    </form>
    
    <script type="text/javascript" language="javascript">

        ////////////////////////////////////////////////////////////

        try {

            // The iFrame that hosts the row record.
            var iframe = document.getElementById('shuffleFrame');

            // The rows (ids) collection that the shuffler shuffles through.
            // Note: This is a comma delimited string of ids.
            var shufflerRowIds = window.opener.shufflerRowIds;

            // The id of the current row.
            var currentRowId = window.opener.currentRowId;

            // The url (with placeholders for the ids) that the shuffler displays
            var shufflerUrl = window.opener.shufflerUrl;

            // The label that displays the row no. and total rows e.g. "6 of 10"
            var rowDisplay = document.getElementById('spRowDisplay');

            // The rows (ids) collection that the shuffler shuffles through.
            // Note: This is an array of ids.
            var rowIdArray = shufflerRowIds.split(',');

            // The total count of rows to shuffle through
            var rowCount = rowIdArray.length;

            // The current row number that the shuffler is displaying.
            var currentRowNumber = 0;

            // Set the current row number that the shuffler window is displaying
            var iCounter = 0;

            for (var i = 0; i < rowCount; i++) {
                if (rowIdArray[i] == currentRowId) {
                    currentRowNumber = i + 1;
                    break;
                }
                iCounter++;
            }

            setRowIndicator();

            // Set the iFrame url
            if (iframe != null) {
                iframe.src = shufflerUrl.replace('|||', currentRowId);
            }

        } catch (err) { }

        function setRowIndicator() {
            // Set the row "x of y" row indicator.
            rowDisplay.innerHTML = currentRowNumber + ' of ' + rowCount;
            
            if (rowCount == 1) {
                // Hide the row indicator div
                var divRowShuffler = document.getElementById('divRowShuffler');
                divRowShuffler.style.display = 'none';
            } else {
                var divRowShuffler = document.getElementById('divRowShuffler');
                divRowShuffler.style.display = '';
            }    
        }

        ////////////////////////////////////////////////////////////

        function goToFirst() {
            try {
                if (iframe != null) {
                    iframe.src = shufflerUrl.replace('|||', rowIdArray[0]);
                    currentRowNumber = 1;
                    setRowIndicator();
                }
            } catch (err) { }
        }

        function goToPrevious() {
            try {
                if (iframe != null && !(currentRowNumber == 1)) {
                    currentRowNumber--;
                    iframe.src = shufflerUrl.replace('|||', rowIdArray[currentRowNumber - 1]);
                    setRowIndicator();
                }
            } catch (err) { }
        }

        function goToNext() {
            try {
                if (iframe != null && !(currentRowNumber == rowCount)) {
                    currentRowNumber++;
                    iframe.src = shufflerUrl.replace('|||', rowIdArray[currentRowNumber - 1]);
                    setRowIndicator();
                }
            } catch (err) { }
        }

        function goToLast() {
            try {
                if (iframe != null) {
                    iframe.src = shufflerUrl.replace('|||', rowIdArray[rowCount - 1]);
                    currentRowNumber = rowCount;
                    setRowIndicator();
                }
            } catch (err) { }
        }
        
    </script>
</body>
</html>