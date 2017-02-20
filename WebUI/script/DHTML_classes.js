//SCREEN ELEMENT CLASS
function ScreenElement(){
	//properties
	this.name = "DRD ScreenElement";
	//initialization
}
ScreenElement.getXPosition = function(el) {
    xPos = el.offsetLeft;
    tempEl = el.offsetParent;
    while (tempEl != null) {
        xPos += tempEl.offsetLeft;
        tempEl = tempEl.offsetParent;
    }
    return xPos;
}
ScreenElement.getYPosition = function(el) {
    yPos = el.offsetTop;
    tempEl = el.offsetParent;
    while (tempEl != null) {
        yPos += tempEl.offsetTop;
        tempEl = tempEl.offsetParent;
    }
    return yPos;
}

//COLOR CLASS
function Color(){
	//properties
	this.name = "DRD Color";
	//initialization
}
Color.combineRGB = function(red,green,blue){
	var RGB = (red<<16) | (green<<8) | blue;
	return RGB;	
};
Color.splitRGB = function(colorHexValue){
	var output = new Object();
	output.red = (colorHexValue >> 16) & 0xFF;
	output.green = (colorHexValue >> 8) & 0xFF;
	output.blue = colorHexValue & 0xFF;
	return output;
};
Color.fromString = function(colorString){
	var colorCode;
	var hexColorString;
	var hexColor;
	
	// Firefox/Netscape tweak. they use rgb(0,0,0) vs #000000
	if(colorString.indexOf('rgb') != -1){
		colorCode = colorString.substring(4,colorString.length-1);
		var colorCodeArray = colorCode.split(",");
		hexColor = Color.combineRGB(colorCodeArray[0], colorCodeArray[1], colorCodeArray[2]);
	} else {
		colorCode = colorString.substring(1,colorString.length);
		hexColorString = "0x" + colorCode;
		hexColor = parseInt(hexColorString);
	}
	
	return hexColor;
}
Color.toString = function(colorNum){
	function decConvertToBase(num, base){
		var newNum="";
		var result=num;
		var remainder=0;
		while (result>0){
			result=Math.floor(num/base);
			remainder=num%base;
			num=result;
			
			if (remainder>=10){
				if (remainder==10) remainder='A';
				if (remainder==11) remainder='B';
				if (remainder==12) remainder='C';
				if (remainder==13) remainder='D';
				if (remainder==14) remainder='E';
				if (remainder==15) remainder='F';
			}
			// append the next remainder to the beginning of the string
			newNum=""+remainder+newNum;
		};
		return newNum;
	} 
	return decConvertToBase(colorNum, 16);
}

//PHYSICS CLASS
function Physics(){
	//properties
	this.name = "DRD Physics";
	//initialization
};

Physics.GRAVITY = 9.8; //meters per second per second
Physics.GRAVITY_PIXELS = 2500; //pixels per second per second

Physics.getProjectileDisplacement = function(v, a, t){
	var result = v * t + .5 * a * Math.pow(t,2);
	return result;
};

//GEOMETRY CLASS
function Geometry(){
	//properties
	this.name = "DRD Geometry";
	//initialization
};
Geometry.degreesToRadians = function(degrees){
	return (degrees / 180) * Math.PI;
};
Geometry.radiansToDegrees = function(radians){
	return (radians / Math.PI) * 180;
};
Geometry.distanceFormula = function(x1,y1,x2,y2){
	return Math.sqrt(Math.pow((x2 - x1),2) + Math.pow((y2 - y1),2));
};
Geometry.midPoint = function(x1,y1,x2,y2){
	var resultObj = new Object();
	resultObj.x = (x1 + x2) / 2;
	resultObj.y = (y1 + y2) / 2;
	return resultObj;
};
Geometry.pointToPolar = function(x,y){
	var resultObj = new Object();
	resultObj.vector = Math.sqrt(Math.pow(x,2)+Math.pow(y,2));
	resultObj.theta = this.radiansToDegrees(Math.atan(y/x));
	return resultObj;
};
Geometry.polarToPoint = function(r,theta) {
	var resultObj = new Object();
	theta = this.degreesToRadians(theta);
	resultObj.x = r * Math.cos(theta);
	resultObj.y = r * Math.sin(theta);
	return resultObj;
};
