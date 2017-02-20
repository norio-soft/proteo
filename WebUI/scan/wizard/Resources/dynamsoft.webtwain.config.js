//
// Dynamsoft JavaScript Library for Basic Initiation of Dynamic Web TWAIN
// More info on DWT: http://www.dynamsoft.com/Products/WebTWAIN_Overview.aspx
//
// Copyright 2016, Dynamsoft Corporation 
// Author: Dynamsoft Team
// Version: 11.3
//
/// <reference path="dynamsoft.webtwain.initiate.js" />
var Dynamsoft = Dynamsoft || { WebTwainEnv: {} };

Dynamsoft.WebTwainEnv.AutoLoad = true;
///
Dynamsoft.WebTwainEnv.Containers = [{ContainerId:'dwtcontrolContainer', Width:270, Height:350}];
///
Dynamsoft.WebTwainEnv.ProductKey = '******';
///
Dynamsoft.WebTwainEnv.Trial = false;
///
Dynamsoft.WebTwainEnv.ActiveXInstallWithCAB = false;
///
Dynamsoft.WebTwainEnv.Debug = false; // only for debugger output
///
// Dynamsoft.WebTwainEnv.ResourcesPath = 'Resources';

/// All callbacks are defined in the dynamsoft.webtwain.install.js file, you can customize them.

// Dynamsoft.WebTwainEnv.RegisterEvent('OnWebTwainReady', function(){
// 		// webtwain has been inited
// });

