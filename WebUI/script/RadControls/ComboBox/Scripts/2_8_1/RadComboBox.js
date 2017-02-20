if(typeof window.RadControlsNamespace=="undefined"){
window.RadControlsNamespace={};
}
if(typeof (window.RadControlsNamespace.DomEventMixin)=="undefined"||typeof (window.RadControlsNamespace.DomEventMixin.Version)==null||window.RadControlsNamespace.DomEventMixin.Version<3){
RadControlsNamespace.DomEventMixin={Version:3,Initialize:function(_1){
_1.CreateEventHandler=this.CreateEventHandler;
_1.AttachDomEvent=this.AttachDomEvent;
_1.DetachDomEvent=this.DetachDomEvent;
_1.DisposeDomEventHandlers=this.DisposeDomEventHandlers;
_1._domEventHandlingEnabled=true;
_1.EnableDomEventHandling=this.EnableDomEventHandling;
_1.DisableDomEventHandling=this.DisableDomEventHandling;
_1.RemoveHandlerRegister=this.RemoveHandlerRegister;
_1.GetHandlerRegister=this.GetHandlerRegister;
_1.AddHandlerRegister=this.AddHandlerRegister;
_1.handlerRegisters=[];
},EnableDomEventHandling:function(){
this._domEventHandlingEnabled=true;
},DisableDomEventHandling:function(){
this._domEventHandlingEnabled=false;
},CreateEventHandler:function(_2,_3){
var _4=this;
return function(e){
if(!_4._domEventHandlingEnabled&&!_3){
return;
}
return _4[_2](e||window.event);
};
},AttachDomEvent:function(_6,_7,_8,_9){
var _a=this.CreateEventHandler(_8,_9);
var _b=this.GetHandlerRegister(_6,_7,_8);
if(_b!=null){
this.DetachDomEvent(_b.Element,_b.EventName,_8);
}
var _c={"Element":_6,"EventName":_7,"HandlerName":_8,"Handler":_a};
this.AddHandlerRegister(_c);
if(_6.addEventListener){
_6.addEventListener(_7,_a,false);
}else{
if(_6.attachEvent){
_6.attachEvent("on"+_7,_a);
}
}
},DetachDomEvent:function(_d,_e,_f){
var _10=null;
var _11="";
if(typeof _f=="string"){
_11=_f;
_10=this.GetHandlerRegister(_d,_e,_11);
if(_10==null){
return;
}
_f=_10.Handler;
}
if(!_d){
return;
}
if(_d.removeEventListener){
_d.removeEventListener(_e,_f,false);
}else{
if(_d.detachEvent){
_d.detachEvent("on"+_e,_f);
}
}
if(_10!=null&&_11!=""){
this.RemoveHandlerRegister(_10);
_10=null;
}
},DisposeDomEventHandlers:function(){
for(var i=0;i<this.handlerRegisters.length;i++){
var _13=this.handlerRegisters[i];
if(_13!=null){
this.DetachDomEvent(_13.Element,_13.EventName,_13.Handler);
}
}
this.handlerRegisters=[];
},RemoveHandlerRegister:function(_14){
try{
var _15=_14.index;
for(var i in _14){
_14[i]=null;
}
this.handlerRegisters[_15]=null;
}
catch(e){
}
},GetHandlerRegister:function(_17,_18,_19){
for(var i=0;i<this.handlerRegisters.length;i++){
var _1b=this.handlerRegisters[i];
if(_1b!=null&&_1b.Element==_17&&_1b.EventName==_18&&_1b.HandlerName==_19){
return this.handlerRegisters[i];
}
}
return null;
},AddHandlerRegister:function(_1c){
_1c.index=this.handlerRegisters.length;
this.handlerRegisters[this.handlerRegisters.length]=_1c;
}};
RadControlsNamespace.DomEvent={};
RadControlsNamespace.DomEvent.PreventDefault=function(e){
if(!e){
return true;
}
if(e.preventDefault){
e.preventDefault();
}
e.returnValue=false;
return false;
};
RadControlsNamespace.DomEvent.StopPropagation=function(e){
if(!e){
return;
}
if(e.stopPropagation){
e.stopPropagation();
}else{
e.cancelBubble=true;
}
};
RadControlsNamespace.DomEvent.GetTarget=function(e){
if(!e){
return null;
}
return e.target||e.srcElement;
};
RadControlsNamespace.DomEvent.GetRelatedTarget=function(e){
if(!e){
return null;
}
return e.relatedTarget||(e.type=="mouseout"?e.toElement:e.fromElement);
};
RadControlsNamespace.DomEvent.GetKeyCode=function(e){
if(!e){
return 0;
}
return e.which||e.keyCode;
};
};if(typeof (window.RadControlsNamespace)=="undefined"){
window.RadControlsNamespace=new Object();
}
RadControlsNamespace.AppendStyleSheet=function(_1,_2,_3){
if(!_3){
return;
}
if(!_1){
document.write("<"+"link"+" rel='stylesheet' type='text/css' href='"+_3+"' />");
}else{
var _4=document.createElement("LINK");
_4.rel="stylesheet";
_4.type="text/css";
_4.href=_3;
document.getElementById(_2+"StyleSheetHolder").appendChild(_4);
}
};
function RadComboItem(){
this.ComboBox=null;
this.ClientID=null;
this.Index=0;
this.Highlighted=false;
this.Enabled=true;
this.Selected=0;
this.Text="";
this.Value="";
this.Attributes=new Array();
}
RadComboItem.prototype.Initialize=function(_5){
for(var _6 in _5){
this[_6]=_5[_6];
}
};
RadComboItem.prototype.Select=function(){
if(this.ComboBox.FireEvent(this.ComboBox.OnClientSelectedIndexChanging,this)===false){
return;
}
var _7=this.ComboBox.GetText();
var _8=this.ComboBox.GetLastSeparatorIndex(_7);
var _9=_7.substring(0,_8+1)+this.Text;
this.ComboBox.SetText(_9);
this.ComboBox.SetValue(this.Value);
this.ComboBox.SelectedItem=this;
this.ComboBox.SelectedIndex=this.Index;
this.Highlight();
this.ComboBox.FireEvent(this.ComboBox.OnClientSelectedIndexChanged,this);
this.ComboBox.PostBack();
};
RadComboItem.prototype.Highlight=function(){
if(!this.Enabled){
return;
}
if(!this.ComboBox.IsTemplated||this.ComboBox.HighlightTemplatedItems){
if(this.ComboBox.HighlightedItem){
this.ComboBox.HighlightedItem.SetCssClass(this.ComboBox.ItemCssClass);
}
this.SetCssClass(this.ComboBox.ItemCssClassHover);
}
this.ComboBox.HighlightedItem=this;
this.Highlighted=true;
};
RadComboItem.prototype.UnHighlight=function(){
if(!this.ComboBox.IsTemplated||this.ComboBox.HighlightTemplatedItems){
this.SetCssClass(this.ComboBox.ItemCssClass);
}
this.ComboBox.HighlightedItem=null;
this.Highlighted=false;
};
RadComboItem.prototype.ScrollIntoView=function(){
var _a=this.GetDomElement().offsetTop;
var _b=this.GetDomElement().offsetHeight;
var _c=this.ComboBox.DropDownDomElement.scrollTop;
var _d=this.ComboBox.DropDownDomElement.offsetHeight;
if(_a+_b>_c+_d){
this.ComboBox.DropDownDomElement.scrollTop=_a+_b-_d;
}else{
if(_a+_b<=_c){
this.ComboBox.DropDownDomElement.scrollTop=_a;
}
}
};
RadComboItem.prototype.ScrollOnTop=function(){
this.ComboBox.DropDownDomElement.scrollTop=this.GetDomElement().offsetTop;
};
RadComboItem.prototype.NextItem=function(){
return this.ComboBox.Items[this.Index+1];
};
RadComboItem.prototype.GetDomElement=function(){
if(!this.DomElement){
this.DomElement=document.getElementById(this.ClientID);
}
return this.DomElement;
};
RadComboItem.prototype.SetCssClass=function(_e){
this.GetDomElement().className=_e;
};
function RadComboBox(_f,_10,_11){
var _12=window[_10];
if(_12!=null&&typeof (_12.Dispose)!="undefined"){
_12.Dispose();
}
if(window.tlrkComboBoxes==null){
window.tlrkComboBoxes=new Array();
}
tlrkComboBoxes[tlrkComboBoxes.length]=this;
this.Items=new Array();
this.ItemMap=new Object();
this.Created=false;
this.ID=_f;
this.ClientID=_10;
this.TagID=_10;
this.DropDownID=_10+"_DropDown";
this.InputID=_10+"_Input";
this.ImageID=_10+"_Image";
this.DropDownPlaceholderID=_10+"_DropDownPlaceholder";
this.MoreResultsBoxID=_10+"_MoreResultsBox";
this.MoreResultsBoxImageID=_10+"_MoreResultsBoxImage";
this.MoreResultsBoxMessageID=_10+"_MoreResultsBoxMessage";
this.Header=_10+"_Header";
this.Changed=false;
this.Focused=false;
this.InputDomElement=document.getElementById(this.InputID);
this.CachedText=this.OriginalText=this.InputDomElement.value;
this.ImageDomElement=document.getElementById(this.ImageID);
this.DropDownPlaceholder=document.getElementById(this.DropDownPlaceholderID);
this.DropDownDomElement=document.getElementById(this.DropDownID);
this.MoreResultsImageDomElement=document.getElementById(this.MoreResultsBoxImageID);
this.MoreResultsBoxMessageDomElement=document.getElementById(this.MoreResultsBoxMessageID);
this.DomElement=document.getElementById(this.ClientID);
this.ValueHidden=document.getElementById(this.ClientID+"_value");
this.TextHidden=document.getElementById(this.ClientID+"_text");
this.ClientWidthHidden=document.getElementById(this.ClientID+"_clientWidth");
this.ClientHeightHidden=document.getElementById(this.ClientID+"_clientHeight");
this.Enabled=true;
this.DropDownVisible=false;
this.LoadOnDemandUrl=null;
this.HighlightedItem=null;
this.SelectedItem=null;
this.ItemRequestTimeout=300;
this.EnableLoadOnDemand=false;
this.AutoPostBack=false;
this.ShowMoreResultsBox=false;
this.OpenDropDownOnLoad=false;
this.MarkFirstMatch=false;
this.IsCaseSensitive=false;
this.SelectOnTab=true;
this.PostBackReference=null;
this.LoadingMessage="Loading...";
this.ScrollDownImage=null;
this.ScrollDownImageDisabled=null;
this.Overlay=null;
this.RadComboBoxImagePosition="Right";
this.ItemCssClass=null;
this.ItemCssClassHover=null;
this.ItemCssClassDisabled=null;
this.ImageCssClass=null;
this.ImageCssClassHover=null;
this.InputCssClass=null;
this.InputCssClassHover=null;
this.LoadingMessageCssClass="ComboBoxLoadingMessage";
this.AutoCompleteSeparator=null;
this.ExternalCallBackPage=null;
this.OnClientSelectedIndexChanging=null;
this.OnClientDropDownOpening=null;
this.OnClientDropDownClosing=null;
this.OnClientItemsRequesting=null;
this.OnClientSelectedIndexChanged=null;
this.OnClientItemsRequested=null;
this.OnClientKeyPressing=null;
this.OnClientBlur=null;
this.OnClientFocus=null;
this.Skin="Classic";
this.HideTimeoutID=0;
this.RequestTimeoutID=0;
this.IsDetached=false;
this.TextPriorToCallBack=null;
this.AllowCustomText=false;
this.ExpandEffectString=null;
this.HighlightTemplatedItems=false;
this.CausesValidation=false;
this.ClientDataString=null;
this.ShowDropDownOnTextboxClick=true;
this.ShowWhileLoading=_11;
this.MoreResultsImageHovered=false;
this.ErrorMessage=null;
this.AfterClientCallBackError=null;
this.PostBackActive=false;
this.SelectedIndex=-1;
this.IsTemplated=false;
this.CurrentText=null;
this.OffsetX=0;
this.OffsetY=0;
this.Disposed=false;
var me=this;
this.DetermineDirection();
this.InputDomElement.setAttribute("autocomplete","off");
this.DropDownPlaceholder.onselectstart=function(){
return false;
};
RadControlsNamespace.DomEventMixin.Initialize(this);
if(this.ImageDomElement){
this.AttachDomEvent(this.ImageDomElement,"click","OnImageClick");
}
this.AttachDomEvent(document,"click","OnDocumentClick");
this.AttachDomEvent(this.InputDomElement,"click","OnInputClick");
this.AttachDomEvent(this.InputDomElement,"keydown","OnKeyDown");
this.AttachDomEvent(this.InputDomElement,"focus","OnFocus");
this.AttachDomEvent(this.InputDomElement,"input","OnInputChange");
this.AttachDomEvent(this.InputDomElement,"propertychange","OnInputPropertyChange");
this.AttachDomEvent(this.DropDownPlaceholder,"mouseover","OnDropDownOver");
this.AttachDomEvent(this.DropDownPlaceholder,"mouseout","OnDropDownOut");
this.AttachDomEvent(this.DropDownPlaceholder,"click","OnDropDownClick");
if(this.MoreResultsImageDomElement){
this.AttachDomEvent(this.MoreResultsImageDomElement,"mouseover","OnMoreResultsImageOver");
this.AttachDomEvent(this.MoreResultsImageDomElement,"mouseout","OnMoreResultsImageOut");
this.AttachDomEvent(this.MoreResultsImageDomElement,"click","OnMoreResultsImageClick");
}
if(typeof (RadCallbackNamespace)!="undefined"){
window.setTimeout(function(){
me.FixUp(me.InputDomElement,true);
},100);
}else{
if(window.addEventListener){
if(window.opera){
this.AttachDomEvent(window,"load","OnWindowLoad");
}else{
this.OnWindowLoad();
}
}else{
if(document.getElementById(this.ClientID).offsetWidth==0){
this.AttachDomEvent(window,"load","OnWindowLoad");
}else{
this.OnWindowLoad();
}
}
}
this.AttachDomEvent(window,"resize","OnWindowResize");
this.AttachDomEvent(window,"unload","Dispose");
}
RadComboBox.prototype.OnWindowResize=function(){
if(this.DropDownVisible){
this.PositionDropDown();
}
};
RadComboBox.prototype.Initialize=function(_14,_15){
this.LoadConfiguration(_14);
if(!this.Enabled){
this.Disable();
}
this.CreateItems(_15);
this.InitCssNames();
if(this.OpenDropDownOnLoad){
this.AttachDomEvent(window,"load","OpenOnLoad");
}
};
RadComboBox.prototype.OpenOnLoad=function(){
this.FixUp(this.InputDomElement,false);
this.ShowDropDown();
};
RadComboBox.prototype.OnWindowLoad=function(){
this.FixUp(this.InputDomElement,true);
};
RadComboBox.Keys={Shift:16,Escape:27,Up:38,Down:40,Left:37,Right:39,Enter:13,Tab:9,Space:32,PageUp:33,Del:46,F1:112,F12:123};
RadComboBox.prototype.FireEvent=function(_16,_17,_18,_19){
if(!_16){
return;
}
var _1a=_16.lastIndexOf(")");
if(_1a==_16.length-1){
return eval(_16);
}
RadComboBoxGlobalFirstParam=_17;
RadComboBoxGlobalSecondParam=_18;
RadComboBoxGlobalThirdParam=_19;
var s=_16;
s=s+"(RadComboBoxGlobalFirstParam";
s=s+",RadComboBoxGlobalSecondParam";
s=s+",RadComboBoxGlobalThirdParam";
s=s+");";
return eval(s);
};
RadComboBox.prototype.PostBack=function(){
if(this.PostBackActive){
return;
}
this.PostBackActive=true;
if(this.AutoPostBack){
if(this.CausesValidation){
if(typeof (WebForm_DoPostBackWithOptions)!="function"&&!(typeof (Page_ClientValidate)!="function"||Page_ClientValidate())){
return;
}
}
eval(this.PostBackReference);
this.PostBackActive=false;
}
};
RadComboBox.prototype.SelectFirstMatch=function(){
var _1c=this.FindItemToSelect();
if(_1c&&_1c.Enabled){
_1c.Highlight();
this.SelectedItem=_1c;
}
};
RadComboBox.prototype.SelectText=function(_1d,_1e){
if(this.InputDomElement.createTextRange){
var _1f=this.InputDomElement.createTextRange();
if(_1d==0&&_1e==0){
_1f.collapse(true);
return;
}
_1f.moveStart("character",_1d);
_1f.moveEnd("character",_1e);
_1f.select();
}else{
this.InputDomElement.setSelectionRange(_1d,_1d+_1e);
}
};
RadComboBox.prototype.OnInputClick=function(){
this.SelectFirstMatch();
this.SelectText(0,this.GetText().length);
if(this.ShowDropDownOnTextboxClick&&!this.DropDownVisible){
this.ShowDropDown();
}
};
RadComboBox.prototype.OnInputPropertyChange=function(){
if(event.propertyName=="value"){
var _20=this.GetText();
if(this.CachedText!=_20){
this.CachedText=_20;
this.OnInputChange();
}
}
};
RadComboBox.prototype.OnInputChange=function(){
this.SetValue("");
this.TextHidden.value=this.InputDomElement.value;
if(this.EnableLoadOnDemand&&!this.SuppressChange){
var me=this;
if(this.RequestTimeoutID>0){
window.clearTimeout(this.RequestTimeoutID);
this.RequestTimeoutID=0;
}
if(!this.DropDownVisible){
this.ShowDropDown();
}
this.RequestTimeoutID=window.setTimeout(function(){
me.RequestItems(me.GetText(),false);
},this.ItemRequestTimeout);
return;
}
if(!this.SuppressChange&&this.ShouldHighlight()){
this.HighlightMatches();
}
};
RadComboBox.prototype.OnImageClick=function(){
this.SelectFirstMatch();
this.ToggleDropDown();
};
RadComboBox.prototype.OnDocumentClick=function(e){
if(!e){
e=event;
}
var _23=e.target||e.srcElement;
while(_23.nodeType!==9){
if(_23==this.DomElement||_23==this.DropDownPlaceholder){
return;
}
_23=_23.parentNode;
}
if(this.DropDownVisible){
this.HideDropDown();
}
if(this.Focused){
this.RaiseClientBlur();
this.SelectItemOnBlur();
this.Focused=false;
}
};
RadComboBox.prototype.SelectItemOnBlur=function(){
var _24=this.FindItemToSelect();
if(!_24&&!this.AllowCustomText&&this.Items.length>0){
if(this.MarkFirstMatch){
if(this.GetText()==""){
this.SetText(this.OriginalText);
}
this.HighlightMatches();
this.SelectText(0,0);
_24=this.HighlightedItem;
}
}
this.PerformSelect(_24);
};
RadComboBox.prototype.FindItemToSelect=function(){
var _25=this.FindItemByValue(this.get_value());
if(!_25){
_25=this.FindItemByText(this.GetText());
}
return _25;
};
RadComboBox.prototype.OnMoreResultsImageOver=function(){
this.MoreResultsImageDomElement.style.cursor="pointer";
this.MoreResultsImageDomElement.src=this.ScrollDownImage;
this.MoreResultsImageHovered=true;
};
RadComboBox.prototype.OnMoreResultsImageOut=function(){
this.MoreResultsImageDomElement.style.cursor="default";
this.MoreResultsImageDomElement.src=this.ScrollDownImageDisabled;
this.MoreResultsImageHovered=false;
};
RadComboBox.prototype.OnMoreResultsImageClick=function(){
this.RequestItems(this.GetText(),true);
};
RadComboBox.prototype.OnDropDownOver=function(e){
var _27=this.GetEventTarget(e);
var _28=this.FindNearestItem(_27);
if(_28){
_28.Highlight();
}
};
RadComboBox.prototype.OnDropDownOut=function(e){
if(!e){
e=event;
}
var _2a;
try{
_2a=e.toElement||e.relatedTarget||e.fromElement;
while(_2a.nodeType!==9){
if(_2a.parentNode==this.DropDownDomElement){
return;
}
_2a=_2a.parentNode;
}
}
catch(e){
}
if(this.HighlightedItem){
this.HighlightedItem.UnHighlight();
}
};
RadComboBox.prototype.OnDropDownClick=function(e){
var _2c=this.GetEventTarget(e);
var _2d=this.FindNearestItem(_2c);
if(!_2d||!_2d.Enabled){
return;
}
this.HideDropDown();
this.PerformSelect(_2d);
};
RadComboBox.prototype.GetEventTarget=function(e){
return e.target||e.srcElement;
};
RadComboBox.prototype.FindNearestItem=function(_2f){
while(_2f.nodeType!==9){
if(_2f.parentNode==this.DropDownDomElement){
return this.ItemMap[_2f.id];
}
_2f=_2f.parentNode;
}
return null;
};
RadComboBox.prototype.GetViewPortSize=function(){
var _30=0;
var _31=0;
var _32=document.body;
if(window.innerWidth){
_30=window.innerWidth;
_31=window.innerHeight;
}else{
if(document.compatMode&&document.compatMode=="CSS1Compat"){
_32=document.documentElement;
}
_30=_32.clientWidth;
_31=_32.clientHeight;
}
_30+=_32.scrollLeft;
_31+=_32.scrollTop;
return {width:_30-6,height:_31-6};
};
RadComboBox.prototype.GetElementPosition=function(el){
var _34=null;
var pos={x:0,y:0};
var box;
if(el.getBoundingClientRect){
box=el.getBoundingClientRect();
var _37=document.documentElement.scrollTop||document.body.scrollTop;
var _38=document.documentElement.scrollLeft||document.body.scrollLeft;
pos.x=box.left+_38-2;
pos.y=box.top+_37-2;
return pos;
}else{
if(document.getBoxObjectFor){
box=document.getBoxObjectFor(el);
pos.x=box.x-1;
pos.y=box.y-1;
}else{
pos.x=el.offsetLeft;
pos.y=el.offsetTop;
_34=el.offsetParent;
if(_34!=el){
while(_34){
pos.x+=_34.offsetLeft;
pos.y+=_34.offsetTop;
_34=_34.offsetParent;
}
}
}
}
if(window.opera){
_34=el.offsetParent;
while(_34&&_34.tagName!="BODY"&&_34.tagName!="HTML"){
pos.x-=_34.scrollLeft;
pos.y-=_34.scrollTop;
_34=_34.offsetParent;
}
}else{
_34=el.parentNode;
while(_34&&_34.tagName!="BODY"&&_34.tagName!="HTML"){
pos.x-=_34.scrollLeft;
pos.y-=_34.scrollTop;
_34=_34.parentNode;
}
}
return pos;
};
RadComboBox.prototype.Dispose=function(){
if(this.Disposed){
return;
}
if(this.RequestTimeoutID>0){
window.clearTimeout(this.RequestTimeoutID);
this.RequestTimeoutID=0;
}
this.HideDropDown();
if(this.Overlay&&this.Overlay.parentNode){
this.Overlay.parentNode.removeChild(this.Overlay);
this.Overlay=null;
}
if(this.LoadingDiv){
if(this.LoadingDiv.parentNode){
this.LoadingDiv.parentNode.removeChild(this.LoadingDiv);
}
this.LoadingDiv=null;
}
if(this.DropDownPlaceholder!=null&&this.DropDownPlaceholder.parentNode!=null){
try{
this.DropDownPlaceholder.parentNode.removeChild(this.DropDownPlaceholder);
}
catch(e){
}
}
this.DisposeDomEventHandlers();
this.InputDomElement=null;
this.ImageDomElement=null;
this.DropDownPlaceholder=null;
this.DropDownDomElement=null;
this.MoreResultsImageDomElement=null;
this.MoreResultsBoxMessageDomElement=null;
this.DomElement=null;
this.ValueHidden=null;
this.ClientWidthHidden=null;
this.ClientHeightHidden=null;
tlrkComboBoxes[this.ID]=null;
this.Disposed=true;
};
RadComboBox.prototype.LoadConfiguration=function(_39){
for(var _3a in _39){
this[_3a]=_39[_3a];
}
};
RadComboBox.prototype.InitCssNames=function(){
this.ItemCssClass="ComboBoxItem_"+this.Skin;
this.ItemCssClassHover="ComboBoxItemHover_"+this.Skin;
this.ItemCssClassDisabled="ComboBoxItemDisabled_"+this.Skin;
this.ImageCssClass="ComboBoxImage_"+this.Skin;
this.ImageCssClassHover="ComboBoxImageHover_"+this.Skin;
this.InputCssClass="ComboBoxInput_"+this.Skin;
this.InputCssClassHover="ComboBoxInputHover_"+this.Skin;
this.LoadingMessageCssClass="ComboBoxLoadingMessage_"+this.Skin;
};
RadComboBox.prototype.FindParentForm=function(){
var _3b=document.getElementById(this.TagID);
while(_3b.tagName!="FORM"){
_3b=_3b.parentNode;
}
return _3b;
};
RadComboBox.prototype.DropDownRequiresForm=function(){
var _3c=this.DropDownPlaceholder.getElementsByTagName("input");
return _3c.length>0;
};
RadComboBox.prototype.DetachDropDown=function(){
if((!document.readyState||document.readyState=="complete")&&(!this.IsDetached)){
var _3d=document.body;
if(this.DropDownRequiresForm()){
_3d=this.FindParentForm();
}
this.DropDownPlaceholder.parentNode.removeChild(this.DropDownPlaceholder);
this.DropDownPlaceholder.style.marginLeft="0";
_3d.insertBefore(this.DropDownPlaceholder,_3d.firstChild);
this.IsDetached=true;
}
};
RadComboBox.prototype.CreateItems=function(_3e){
for(var i=0;i<_3e.length;i++){
var _40=new RadComboItem();
_40.ComboBox=this;
_40.Index=this.Items.length;
_40.Initialize(_3e[i]);
if(_40.Selected){
this.SelectedItem=_40;
}
this.ItemMap[_40.ClientID]=_40;
this.Items[this.Items.length]=_40;
}
};
RadComboBox.prototype.ShowOverlay=function(x,y){
if(document.readyState&&document.readyState!="complete"){
return;
}
if(!document.all||window.opera){
return;
}
if(this.Overlay==null){
this.Overlay=document.createElement("iframe");
this.Overlay.src="javascript:''";
this.Overlay.id=this.ClientID+"_Overlay";
this.Overlay.frameBorder=0;
this.Overlay.style.position="absolute";
this.Overlay.style.display="none";
this.DetachDropDown();
this.DropDownPlaceholder.parentNode.insertBefore(this.Overlay,this.DropDownPlaceholder);
this.Overlay.style.zIndex=this.DropDownPlaceholder.style.zIndex-1;
}
this.Overlay.style.left=x;
this.Overlay.style.top=y;
this.Overlay.style.width=this.DropDownPlaceholder.offsetWidth+"px";
this.Overlay.style.height=this.DropDownPlaceholder.offsetHeight+"px";
this.Overlay.style.display="block";
};
RadComboBox.prototype.HideOverlay=function(){
if(!document.all||window.opera){
return;
}
if(this.Overlay!=null){
this.Overlay.style.display="none";
}
};
RadComboBox.prototype.DetermineDirection=function(){
var el=document.getElementById(this.ClientID+"_wrapper");
while(el.tagName.toLowerCase()!="html"){
if(el.dir){
this.RightToLeft=(el.dir.toLowerCase()=="rtl");
return;
}
el=el.parentNode;
}
this.RightToLeft=false;
};
RadComboBox.prototype.PositionDropDown=function(){
this.DetachDropDown();
var _44=this.DomElement.firstChild;
var _45=this.GetElementPosition(_44);
if(this.ExpandEffectString!=null&&document.all){
try{
this.DropDownPlaceholder.style.filter=this.ExpandEffectString;
this.DropDownPlaceholder.filters[0].Apply();
this.DropDownPlaceholder.filters[0].Play();
}
catch(e){
}
}
this.DropDownPlaceholder.style.position="absolute";
this.DropDownPlaceholder.style.top=_45.y+this.OffsetY+this.InputDomElement.offsetHeight+"px";
this.DropDownPlaceholder.style.left=_45.x+this.OffsetX+"px";
this.DropDownPlaceholder.style.display="block";
var _46=this.DomElement.offsetWidth;
this.DropDownPlaceholder.style.width=_46+"px";
var _47=this.DropDownPlaceholder.offsetWidth-_46;
if(_47>0&&_47<_46){
this.DropDownPlaceholder.style.width=_46-_47+"px";
}
if(this.RightToLeft){
this.DropDownPlaceholder.dir="rtl";
}
var _48=this.GetViewPortSize();
if(this.ElementOverflowsBottom(_48,this.DropDownPlaceholder,this.InputDomElement)){
var y=_45.y-this.DropDownPlaceholder.offsetHeight;
if(y>=0){
this.DropDownPlaceholder.style.top=y+"px";
}
}
this.ShowOverlay(this.DropDownPlaceholder.style.left,this.DropDownPlaceholder.style.top);
this.DropDownVisible=true;
};
RadComboBox.prototype.ShowDropDown=function(_4a){
if(this.FireEvent(this.OnClientDropDownOpening,this)===false){
return;
}
this.PositionDropDown();
this.InputDomElement.focus();
if(this.EnableLoadOnDemand&&this.Items.length==0&&!_4a){
this.RequestItems(this.GetText(),false);
}
};
RadComboBox.prototype.ClearItems=function(){
this.Items=[];
this.ItemMap=new Object();
this.DropDownDomElement.innerHTML="";
};
RadComboBox.prototype.RequestItems=function(_4b,_4c){
if(this.Disposed){
return;
}
if(this.FireEvent(this.OnClientItemsRequesting,this,_4b,_4c)==false){
return;
}
if(!this.LoadingDiv){
this.LoadingDiv=document.createElement("div");
this.LoadingDiv.className=this.LoadingMessageCssClass;
this.LoadingDiv.id=this.ClientID+"_LoadingDiv";
this.LoadingDiv.innerHTML=this.LoadingMessage;
this.DropDownDomElement.insertBefore(this.LoadingDiv,this.DropDownDomElement.firstChild);
}
var _4d=this.GetAjaxUrl(_4b,this.GetText(),this.get_value(),_4c);
var _4e=this.CreateXmlHttpRequest();
var me=this;
_4e.onreadystatechange=function(){
if(_4e.readyState!=4){
return;
}
if(!me.Disposed){
me.OnCallBackResponse(_4e,_4c,_4b,_4d);
}
};
_4e.open("GET",_4d,true);
_4e.setRequestHeader("Content-Type","application/json; charset=utf-8");
_4e.send("");
};
RadComboBox.prototype.OnCallBackResponse=function(_50,_51,_52,_53){
if(this.LoadingDiv){
if(this.LoadingDiv.parentNode){
this.LoadingDiv.parentNode.removeChild(this.LoadingDiv);
}
this.LoadingDiv=null;
}
if(_50.status==500){
if(this.FireEvent(this.AfterClientCallBackError,this)===false){
return;
}
alert("RadComboBox: Server error in the ItemsRequested event handler, press ok to view the result.");
var _54;
this.Dispose();
if(this.ErrorMessage){
_54=this.ErrorMessage;
}else{
_54=_50.responseText;
}
document.body.innerHTML=_54;
return;
}
if(_50.status==404){
if(this.FireEvent(this.AfterClientCallBackError,this)===false){
return;
}
alert("RadComboBox: Load On Demand Page not found: "+_53);
this.Dispose();
var _54;
if(this.ErrorMessage){
_54=this.ErrorMessage;
}else{
_54="RadComboBox: Load On Demand Page not found: "+_53+"<br/>";
_54+="Please, try using ExternalCallBackPage to map to the exact location of the callbackpage you are using.";
}
document.body.innerHTML=_54;
return;
}
try{
eval("var callBackData = "+_50.responseText+";");
}
catch(e){
if(this.FireEvent(this.AfterClientCallBackError,this)===false){
return;
}
alert("RadComboBox: load on demand callback error. Press Enter for more information");
this.Dispose();
var _54;
if(this.ErrorMessage){
_54=this.ErrorMessage;
}else{
_54="If RadComboBox is not initially visible on your ASPX page, you may need to use streamers (the ExternallCallBackPage property)";
_54+="<br/>Please, read our online documentation on this problem for details";
_54+="<br/><a href='http://www.telerik.com/help/radcombobox/v2%5FNET2/?combo_externalcallbackpage.html'>http://www.telerik.com/help/radcombobox/v2%5FNET2/combo_externalcallbackpage.html</a>";
}
document.body.innerHTML=_54;
return;
}
if(this.GetText()!=callBackData.Text){
this.RequestItems(this.GetText(),_51);
return;
}
if(!_51){
this.ClearItems();
}
this.SelectedItem=null;
this.HighlightedItem=null;
var _55=this.Items.length;
if(_51){
for(var i=0;i<this.Items.length;i++){
this.Items[i].DomElement=null;
}
}
this.CreateItems(callBackData.Items);
if(_51){
this.DropDownDomElement.innerHTML+=callBackData.DropDownHtml;
if(this.Items[_55+1]!=null){
this.Items[_55+1].ScrollIntoView();
}
}else{
this.DropDownDomElement.innerHTML=callBackData.DropDownHtml;
}
if(this.ShowMoreResultsBox){
this.MoreResultsBoxMessageDomElement.innerHTML=callBackData.Message;
}
this.FireEvent(this.OnClientItemsRequested,this,callBackData.Text,_51);
if(this.ShouldHighlight()){
this.HighlightMatches();
}
};
RadComboBox.prototype.CreateXmlHttpRequest=function(){
if(typeof (XMLHttpRequest)!="undefined"){
return new XMLHttpRequest();
}
if(typeof (ActiveXObject)!="undefined"){
return new ActiveXObject("Microsoft.XMLHTTP");
}
};
RadComboBox.prototype.ClearSelection=function(){
this.SetText("");
this.SetValue("");
this.SelectedItem=null;
this.HighLightedItem=null;
};
RadComboBox.prototype.GetAjaxUrl=function(_57,_58,_59,_5a){
_57=_57.replace(/'/g,"&squote");
var url=window.unescape(this.LoadOnDemandUrl)+"&text="+this.EncodeURI(_57);
url=url+"&comboText="+this.EncodeURI(_58);
url=url+"&comboValue="+this.EncodeURI(_59);
url=url+"&skin="+this.EncodeURI(this.Skin);
if(_5a){
url=url+"&itemCount="+this.Items.length;
}
if(this.ExternalCallBackPage!=null){
url=url+"&external=true";
}
if(this.ClientDataString!=null){
url+="&clientDataString="+this.EncodeURI(this.ClientDataString);
}
url=url+"&timeStamp="+encodeURIComponent((new Date()).getTime());
return url;
};
RadComboBox.prototype.EncodeURI=function(s){
if(typeof (encodeURIComponent)!="undefined"){
return encodeURIComponent(this.EscapeQuotes(s));
}
if(escape){
return escape(this.EscapeQuotes(s));
}
};
RadComboBox.prototype.EscapeQuotes=function(_5d){
if(typeof (_5d)!="number"){
return _5d.replace(/'/g,"&squote");
}
};
RadComboBox.prototype.ToggleDropDown=function(){
if(this.DropDownVisible){
this.HideDropDown();
}else{
this.ShowDropDown();
if(this.HighlightedItem){
this.HighlightedItem.ScrollIntoView();
}
}
};
RadComboBox.prototype.HideDropDown=function(){
if(this.FireEvent(this.OnClientDropDownClosing,this)===false){
return;
}
this.DropDownPlaceholder.style.display="none";
this.HideOverlay();
this.DropDownVisible=false;
};
RadComboBox.prototype.SetText=function(_5e){
this.SuppressChange=true;
this.InputDomElement.value=_5e;
this.TextHidden.value=_5e;
this.ValueHidden.value="";
if(this.InputDomElement.fireEvent){
var _5f=document.createEventObject();
this.InputDomElement.fireEvent("onchange",_5f);
}else{
if(this.InputDomElement.dispatchEvent){
var _60=true;
var _5f=document.createEvent("HTMLEvents");
_5f.initEvent("change",_60,true);
this.InputDomElement.dispatchEvent(_5f);
}
}
this.SuppressChange=false;
};
RadComboBox.prototype.SetValue=function(_61){
this.ValueHidden.value=_61;
};
RadComboBox.prototype.GetValue=function(){
return this.ValueHidden.value;
};
RadComboBox.prototype.PerformSelect=function(_62){
if(_62&&_62!=this.SelectedItem&&!this.EnableLoadOnDemand){
_62.Select();
return;
}
if(_62&&_62==this.SelectedItem&&this.GetText()!=_62.Text&&this.AllowCustomText){
this.SetText(_62.Text);
return;
}
if(_62&&_62==this.SelectedItem){
return;
}
if(_62&&this.OriginalText!=_62.Text){
_62.Select();
return;
}
if(_62&&(!this.SelectedItem||this.SelectedItem.Value!=_62.Value)){
_62.Select();
return;
}
if(this.OriginalText!=this.GetText()){
if(this.HighlightedItem){
this.HighlightedItem.UnHighlight();
}
this.PostBack();
}
};
RadComboBox.prototype.OnKeyDown=function(e){
if(!e){
e=event;
}
this.Changed=true;
this.FireEvent(this.OnClientKeyPressing,this,e);
var _64=e.keyCode||e.which;
this.LastKeyCode=_64;
if(_64==RadComboBox.Keys.Escape&&this.DropDownVisible){
this.HideDropDown();
return;
}
if(_64===RadComboBox.Keys.Enter){
this.HideDropDown();
this.PerformSelect(this.HighlightedItem);
e.returnValue=false;
if(e.preventDefault){
e.preventDefault();
}
return;
}else{
if(_64===RadComboBox.Keys.Down){
e.returnValue=false;
if(e.altKey){
this.ToggleDropDown();
return;
}
this.HighlightNextItem();
return;
}else{
if(_64===RadComboBox.Keys.Up){
e.returnValue=false;
if(e.altKey){
this.ToggleDropDown();
return;
}
this.HighlightPreviousItem();
return;
}else{
if(_64===RadComboBox.Keys.Tab){
this.HideDropDown();
this.RaiseClientBlur();
this.SelectItemOnBlur();
this.Focused=false;
return;
}
}
}
}
if(_64==RadComboBox.Keys.Left||_64==RadComboBox.Keys.Right){
return;
}
};
RadComboBox.prototype.GetLastWord=function(_65){
var _66=-1;
if(this.AutoCompleteSeparator!=null){
_66=this.GetLastSeparatorIndex(_65);
}
var _67=_65.substring(_66+1,_65.length);
return _67;
};
RadComboBox.prototype.GetLastSeparatorIndex=function(_68){
var _69=-1;
if(!this.AutoCompleteSeparator){
return _69;
}
for(var i=0;i<this.AutoCompleteSeparator.length;i++){
var _6b=this.AutoCompleteSeparator.charAt(i);
var _6c=_68.lastIndexOf(_6b);
if(_6c>_69){
_69=_6c;
}
}
return _69;
};
RadComboBox.prototype.GetLastSeparator=function(_6d){
if(!this.AutoCompleteSeparator){
return null;
}
var _6e=this.GetLastSeparatorIndex(_6d);
return _6d.charAt(_6e);
};
RadComboBox.prototype.ShouldHighlight=function(){
if(this.LastKeyCode<RadComboBox.Keys.Space){
return false;
}
if(this.LastKeyCode>=RadComboBox.Keys.PageUp&&this.LastKeyCode<=RadComboBox.Keys.Del){
return false;
}
if(this.LastKeyCode>=RadComboBox.Keys.F1&&this.LastKeyCode<=RadComboBox.Keys.F12){
return false;
}
return true;
};
RadComboBox.prototype.HighlightMatches=function(){
if(!this.MarkFirstMatch){
return;
}
var _6f=this.GetText();
var _70=this.GetLastWord(_6f);
if(this.GetLastSeparator(_6f)==_6f.charAt(_6f.length-1)){
return;
}
var _71=this.FindFirstMatch(_70);
if(this.HighlightedItem){
this.HighlightedItem.UnHighlight();
}
if(!_71){
if(!this.AllowCustomText){
if(_6f){
var _72=this.GetLastSeparatorIndex(_6f);
if(_72<_6f.length-1){
this.SetText(_6f.substring(0,_6f.length-1));
this.HighlightMatches();
}
}
}
return;
}
_71.Highlight();
_71.ScrollOnTop();
var _72=this.GetLastSeparatorIndex(_6f);
var _73=_6f.substring(0,_72+1)+_71.Text;
if(_6f!=_73){
this.SetText(_73);
}
this.SetValue(_71.Value);
var _74=_72+_70.length+1;
var _75=_73.length-_74;
this.SelectText(_74,_75);
};
RadComboBox.prototype.FindFirstMatch=function(_76){
if(!_76){
return null;
}
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Text.length<_76.length){
continue;
}
if(this.Items[i].Enabled==false){
continue;
}
var _78=this.Items[i].Text.substring(0,_76.length);
if(!this.IsCaseSensitive){
if(_78.toLowerCase()==_76.toLowerCase()){
return this.Items[i];
}
}else{
if(_78==_76){
return this.Items[i];
}
}
}
return null;
};
RadComboBox.prototype.OnFocus=function(e){
if(this.Focused){
return;
}
if(!e){
e=event;
}
this.Focused=true;
this.FireEvent(this.OnClientFocus,this);
};
RadComboBox.prototype.RaiseClientBlur=function(){
if(this.Focused){
this.FireEvent(this.OnClientBlur,this);
}
};
RadComboBox.prototype.FindNextAvailableIndex=function(_7a){
for(var i=_7a;i<this.Items.length;i++){
if(this.Items[i].Enabled){
return i;
}
}
return this.Items.length;
};
RadComboBox.prototype.FindPrevAvailableIndex=function(_7c){
for(var i=_7c;i>=0;i--){
if(this.Items[i].Enabled){
return i;
}
}
return -1;
};
RadComboBox.prototype.HighlightNextItem=function(){
var _7e=this.HighlightedItem;
var _7f=0;
if(_7e){
_7f=_7e.Index+1;
}
_7f=this.FindNextAvailableIndex(_7f);
if(_7f<this.Items.length){
this.Items[_7f].Highlight();
this.Items[_7f].ScrollIntoView();
this.Items[_7f].Text;
var _80=this.GetLastSeparatorIndex(this.GetText());
var _81=this.GetText().substring(0,_80+1)+this.Items[_7f].Text;
this.SetText(_81);
this.SetValue(this.Items[_7f].Value);
}
};
RadComboBox.prototype.HighlightPreviousItem=function(){
var _82=this.HighlightedItem;
var _83=0;
if(_82){
_83=_82.Index-1;
}
_83=this.FindPrevAvailableIndex(_83);
if(_83>=0){
this.Items[_83].Highlight();
this.Items[_83].ScrollIntoView();
this.Items[_83].Text;
var _84=this.GetLastSeparatorIndex(this.GetText());
var _85=this.GetText().substring(0,_84+1)+this.Items[_83].Text;
this.SetText(_85);
this.SetValue(this.Items[_83].Value);
}
};
RadComboBox.prototype.ElementOverflowsBottom=function(_86,_87,_88){
var _89=this.GetElementPosition(_88).y+_87.offsetHeight;
return _89>_86.height;
};
RadComboBox.prototype.FindItemByText=function(_8a){
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Text==_8a){
return this.Items[i];
}
}
return null;
};
RadComboBox.prototype.FindItemByValue=function(_8c){
if(!_8c){
return null;
}
for(var i=0;i<this.Items.length;i++){
if(this.Items[i].Value==_8c){
return this.Items[i];
}
}
return null;
};
RadComboBox.prototype.CancelPropagation=function(_8e){
if(_8e.stopPropagation){
_8e.stopPropagation();
}else{
_8e.cancelBubble=true;
}
};
RadComboBox.prototype.PreventDefault=function(_8f){
if(_8f.preventDefault){
_8f.preventDefault();
}else{
_8f.returnValue=false;
}
};
RadComboBox.prototype.GetText=function(){
return this.InputDomElement.value;
};
RadComboBox.prototype.Enable=function(){
this.EnableDomEventHandling();
this.InputDomElement.disabled=false;
this.Enabled=true;
};
RadComboBox.prototype.Disable=function(){
this.Enabled=false;
this.TextHidden.value=this.GetText();
this.InputDomElement.disabled="disabled";
this.DisableDomEventHandling();
};
RadComboBox.prototype.FixUp=function(_90,_91){
if((this.ClientWidthHidden.value!="")&&(this.ClientHeightHidden.value!="")){
if(_90.style.width!=this.ClientWidthHidden.value){
_90.style.width=this.ClientWidthHidden.value;
}
if(_90.style.height!=this.ClientHeightHidden.value){
_90.style.height=this.ClientHeightHidden.value;
}
this.ShowWrapperElement();
return;
}
var _92=_90.parentNode.getElementsByTagName("img")[0];
if(_91&&_92&&(_92.offsetWidth==0)){
var _93=this;
if(document.attachEvent){
if(document.readyState=="complete"){
window.setTimeout(function(){
_93.FixUp(_90,false);
},100);
}else{
window.attachEvent("onload",function(){
_93.FixUp(_90,false);
});
}
}else{
window.addEventListener("load",function(){
_93.FixUp(_90,false);
},false);
}
return;
}
var _94=null;
if(_90.currentStyle){
_94=_90.currentStyle;
}else{
if(document.defaultView&&document.defaultView.getComputedStyle){
_94=document.defaultView.getComputedStyle(_90,null);
}
}
if(_94==null){
this.ShowWrapperElement();
return;
}
var _95=parseInt(_94.height);
var _96=parseInt(_90.offsetWidth);
var _97=parseInt(_94.paddingTop);
var _98=parseInt(_94.paddingBottom);
var _99=parseInt(_94.paddingLeft);
var _9a=parseInt(_94.paddingRight);
var _9b=parseInt(_94.borderTopWidth);
if(isNaN(_9b)){
_9b=0;
}
var _9c=parseInt(_94.borderBottomWidth);
if(isNaN(_9c)){
_9c=0;
}
var _9d=parseInt(_94.borderLeftWidth);
if(isNaN(_9d)){
_9d=0;
}
var _9e=parseInt(_94.borderRightWidth);
if(isNaN(_9e)){
_9e=0;
}
if(document.compatMode&&document.compatMode=="CSS1Compat"){
if(!isNaN(_95)&&(this.ClientHeightHidden.value=="")){
_90.style.height=_95-_97-_98-_9b-_9c+"px";
this.ClientHeightHidden.value=_90.style.height;
}
}
if(!isNaN(_96)&&_96&&(this.ClientWidthHidden.value=="")){
var _9f=0;
if(_92){
_9f=_92.offsetWidth;
}
if(document.compatMode&&document.compatMode=="CSS1Compat"){
var _a0=_96-_9f-_99-_9a-_9d-_9e;
if(_a0>=0){
_90.style.width=_a0+"px";
}
this.ClientWidthHidden.value=_90.style.width;
}else{
_90.style.width=_96-_9f;
}
}
this.ShowWrapperElement();
};
RadComboBox.prototype.ShowWrapperElement=function(){
if(!this.ShowWhileLoading){
document.getElementById(this.ClientID+"_wrapper").style.visibility="visible";
}
};;//BEGIN_ATLAS_NOTIFY
if (typeof(Sys) != "undefined"){if (Sys.Application != null && Sys.Application.notifyScriptLoaded != null){Sys.Application.notifyScriptLoaded();}}
//END_ATLAS_NOTIFY
