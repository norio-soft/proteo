
// if you are unsure as to why this is here please as greg (t.lunken@p1tp.com)
function GetRadWindow()
{
    var oWindow = null;
    if (window.radWindow) oWindow = window.radWindow; //Will work in Moz in all cases, including clasic dialog
    else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;//IE (and Moz az well) 
    return oWindow;
}

function CloseOnReload()
{
    GetRadWindow().Close();
}

function RefreshParentPage()
{
    GetRadWindow().BrowserWindow.location.reload();
    GetRadWindow().Close();
}


