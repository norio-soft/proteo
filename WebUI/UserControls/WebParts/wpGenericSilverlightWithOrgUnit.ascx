<%@ Control Language="C#" AutoEventWireup="true" Inherits="Orchestrator.WebUI.WebParts.GenericSilverlightWithOrgUnit"  Codebehind="wpGenericSilverlightWithOrgUnit.ascx.cs" %>

<div style="background-color:White; width:100%; height:100%">
<object width="<%=Width%>" height="<%=Height%>"
    data="data:application/x-silverlight-2," 
    type="application/x-silverlight-2" >
    <param name="source" value="/ClientBin/<%=CacheProofXap%>" />
    <param name="initParams" value="Xaml=<%=Xaml%>,OrgUnitId=<%=OrgUnitId%>" />
	<!--param name="onError" value="onSilverlightError" /-->
	<param name="background" value="white" />
	<!--param name="minRuntimeVersion" value="4.0.50401.0" /-->
	<param name="autoUpgrade" value="true" />
</object>
</div>