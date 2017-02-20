function changeStyle(objectID, propertyName, propertyValue){
	document.getElementById(objectID).style[propertyName] = propertyValue;
};
function changeProperty(objectID, propertyName, propertyValue){
	document.getElementById(objectID)[propertyName] = propertyValue;
};
function getStyleValue(objectID, propertyName){
	return document.getElementById(objectID).style[propertyName];
};
function getPropertyValue(objectID, propertyName){
	return document.getElementById(objectID)[propertyName];
};


function colorBlend(objectName, propertyName, startColor, endColor, blendSeconds){
	var objectRef = document.getElementById(objectName);

	if(typeof objectRef.colorBlendID == "number") clearInterval(objectRef.colorBlendID);//clear interval just incase we invoke a blend before another has finished
	var time = blendSeconds*1000;//duration of the blend in miliseconds
	var steps = 30*blendSeconds;//steps in the animation (should be framerate*("var time" in seconds))
	var startColorSplit = Color.splitRGB(startColor);
	var endColorSplit = Color.splitRGB(endColor);
	var redArray = new Array();
	var greenArray = new Array();
	var blueArray = new Array();
	
	//calculate red blend
	var redIncr = (endColorSplit.red - startColorSplit.red)/steps;
	for(x=0; x<=steps; x++){
		redArray.push(Math.round(startColorSplit.red + (x * redIncr)));
	}
	//calculate green blend
	var greenIncr = (endColorSplit.green - startColorSplit.green)/steps;
	for(x=0; x<=steps; x++){
		greenArray.push(Math.round(startColorSplit.green + (x * greenIncr)));
	}
	//calculate blue blend
	var blueIncr = (endColorSplit.blue - startColorSplit.blue)/steps;
	for(x=0; x<=steps; x++){
		blueArray.push(Math.round(startColorSplit.blue + (x * blueIncr)));
	}
	
	//animate blend using an inner function
	objectRef.blendStep = 0;
	objectRef.nameString = objectName;
	objectRef.propString = propertyName;
	var blendInterval = time/steps;
	objectRef.colorBlendID = setInterval(blendFunction,blendInterval);
	var selfReference = objectRef;
	
	function blendFunction(){
		selfReference.blendStep++;
		if(selfReference.blendStep <= steps){
			var tempColor = "#" + Color.toString(Color.combineRGB(redArray[selfReference.blendStep],greenArray[selfReference.blendStep],blueArray[selfReference.blendStep]));
			changeStyle(selfReference.nameString, objectRef.propString, tempColor);
		} else {
			var tempColor = "#" + Color.toString(endColor);
			changeStyle(selfReference.nameString, objectRef.propString, tempColor);
			clearInterval(selfReference.colorBlendID);
		}
	}
};
function killColorBlend(objectName){
	var objectRef = document.getElementById(objectName);
	if(typeof objectRef.slideID== "number")clearInterval(objectRef.colorBlendID);
};


function slide(objectName,startX,endX,startY,endY,slideTime){
	var objectRef = document.getElementById(objectName);
	
	if(typeof objectRef.slideID== "number")clearInterval(objectRef.slideID);
	var time = slideTime*1000;//duration of the blend in miliseconds
	var steps = 30*slideTime;//steps in the animation (should be framerate*("var time" in seconds))
	var slideXArray = new Array();
	var slideYArray = new Array();
	
	//calculate changes in x and y coordinates
	var slideDeltaX = (endX - startX)/steps;
	var slideDeltaY = (endY - startY)/steps;
	for(x=0; x<=steps; x++){
		slideXArray.push(Math.round(startX + (x * slideDeltaX)));
		slideYArray.push(Math.round(startY + (x * slideDeltaY)));
	}
	
	//animate slide using an inner function
	objectRef.slideStep = 0;
	objectRef.nameString = objectName;
	var slideInterval = time/steps;
	objectRef.slideID = setInterval(slideFunction,slideInterval);
	var selfReference = objectRef;
	function slideFunction(){
		selfReference.slideStep++;
		if(selfReference.slideStep <= steps){
			//reposition object!!
			var tempX = slideXArray[selfReference.slideStep] + "px";
			var tempY = slideYArray[selfReference.slideStep] + "px";
			changeStyle(selfReference.nameString, 'left', tempX);
			changeStyle(selfReference.nameString, 'top', tempY);
		} else {
			//account for remainder in case "step" is not a whole-number value
			var tempX = endX + "px";
			var tempY = endY + "px";
			changeStyle(selfReference.nameString, 'left', tempX);
			changeStyle(selfReference.nameString, 'top', tempY);
			clearInterval(selfReference.slideID);
		}
	}
};
function killSlide(objectName){
	var objectRef = document.getElementById(objectName);
	if(typeof objectRef.slideID == "number")clearInterval(objectRef.slideID);
};


function accelerationSlide(objectName,startX,endX,startY,endY,easeDirection,slideTime){
	var objectRef = document.getElementById(objectName);
	
	if(typeof objectRef.accelerationSlideID == "number")clearInterval(objectRef.accelerationSlideID);
	var time = slideTime*1000;//duration of the blend in miliseconds
	var steps = 30*slideTime;//steps in the animation (should be framerate*("var time" in seconds))
	var slideXArray = new Array();
	var slideYArray = new Array();
	
	//calculate changes in x and y coordinates
	var xDistance = (endX - startX);
	var yDistance = (endY - startY);
	
	//determine direction
	var xMod = 1;
	var yMod = 1;	
	var slideDeltaX = Math.pow(xDistance,1/steps);
	var slideDeltaY = Math.pow(yDistance,1/steps);
	if (xDistance < 0) { 
		slideDeltaX = Math.pow(-xDistance,1/steps);
		xMod = -1;
	}
	if (yDistance < 0) {
		slideDeltaY = Math.pow(-yDistance,1/steps);
		yMod = -1;
	}
			
	for(x=0; x<=steps; x++){
		if(easeDirection == "out"){
			slideXArray.push(Math.floor(endX - (xMod*(Math.pow(slideDeltaX,steps-x)))));
			slideYArray.push(Math.round(endY - (yMod*(Math.pow(slideDeltaY,steps-x)))));
		} else {
			slideXArray.push(Math.round(startX + (xMod*(Math.pow(slideDeltaX,x)))));	
			slideYArray.push(Math.round(startY + (yMod*(Math.pow(slideDeltaY,x)))));
		}
	}
	
	//animate slide using an inner function
	objectRef.slideStep = 0;
	objectRef.nameString = objectName;
	var slideInterval = time/steps;
	objectRef.accelerationSlideID = setInterval(accelerationSlideFunction,slideInterval);
	var selfReference = objectRef;
	function accelerationSlideFunction(){
		selfReference.slideStep++;
		if(selfReference.slideStep <= steps){
			//reposition object!!
			var tempX = slideXArray[selfReference.slideStep] + "px";
			var tempY = slideYArray[selfReference.slideStep] + "px";
			changeStyle(selfReference.nameString, 'left', tempX);
			changeStyle(selfReference.nameString, 'top', tempY);
		} else {
			//account for remainder in case "step" is not a whole-number value
			var tempX = endX + "px";
			var tempY = endY + "px";
			changeStyle(selfReference.nameString, 'left', tempX);
			changeStyle(selfReference.nameString, 'top', tempY);
			clearInterval(selfReference.accelerationSlideID);
		}
	}
};
function killAccelerationSlide(objectName){
	var objectRef = document.getElementById(objectName);
	if(typeof objectRef.accelerationSlideID == "number") clearInterval(objectRef.accelerationSlideID);
};


function bounce(objectName,startY,endY){
	var objectRef = document.getElementById(objectName);

	//killBounce(objectName);
	objectRef.bouncing = true;
	
	var bounceYArray = new Array();
	
	//calculate duration of drop
	var dropDistance = endY - startY;
	var acceleration = Physics.GRAVITY_PIXELS;
	var dropTime = Math.sqrt(dropDistance/(.5 * acceleration));
	var time = dropTime * 1000; //converted to milliseconds
	var steps = 30 * dropTime;
	
	for(x=0; x<=steps; x++){
		var t = (x/steps)*dropTime;
		var a = Physics.GRAVITY_PIXELS;
		var v = 0;
		var deltaY = Physics.getProjectileDisplacement(v, a, t);
		bounceYArray.push(startY + deltaY);
	}
	
	//animate bounce using an inner function
	objectRef.bounceStep = 0;
	
	var bounceInterval = time/steps;
	objectRef.bounceID = setInterval(bounceFunction,bounceInterval);
	objectRef.nameString = objectName;
	var selfReference = objectRef;
	
	function bounceFunction(){
		selfReference.bounceStep++;
		if(selfReference.bounceStep <= steps){
			//selfReference._y = bounceYArray[selfReference.bounceStep];
			var tempYString = bounceYArray[selfReference.bounceStep] + "px";
			changeStyle(selfReference.nameString, 'top', tempYString);
		} else {
			//selfReference._y = endY;//account for remainder in case "step" is not a whole-number value
			var tempYString = endY + "px";
			changeStyle(selfReference.nameString, 'top', tempYString);
			clearInterval(selfReference.bounceID);
			rebound();
		}
	}
	
	function rebound(){
		//calculate duration of rebound
		var reboundDistance = (endY - startY)/2;
		var acceleration = Physics.GRAVITY_PIXELS;
		var dropTime = Math.sqrt(reboundDistance/(.5 * acceleration));
		var time = dropTime * 1000; //converted to milliseconds
		var steps = 30 * dropTime;
		var reboundArray = new Array();
		
		for(x=0; x<=steps; x++){
			var t = (x/steps)*dropTime;
			var a = Physics.GRAVITY_PIXELS;
			var v = 0;
			var deltaY = Physics.getProjectileDisplacement(v, a, t);
			reboundArray.push((endY - reboundDistance)+deltaY);
		}
		reboundArray.reverse();
		//animate rebound using an inner-inner function
		selfReference.reboundStep = 0;
		var reboundInterval = time/steps;
		selfReference.reboundID = setInterval(reboundFunction,reboundInterval);

		function reboundFunction(){
			selfReference.reboundStep++;
			if(selfReference.reboundStep <= steps){
				//selfReference._y = reboundArray[selfReference.reboundStep];
				var tempYString = reboundArray[selfReference.reboundStep] + "px";
				changeStyle(selfReference.nameString, 'top', tempYString);
			} else {
				//selfReference._y = endY-reboundDistance;//account for remainder in case "step" is not a whole-number value
				var tempYString = (endY-reboundDistance) + "px";
				changeStyle(selfReference.nameString, 'top', tempYString);
				clearInterval(selfReference.reboundID);
				bound(endY, selfReference.nameString);
			}
		}
	}
	
	function bound(endY, thisName){
		var thisRef = document.getElementById(thisName);
		
		//only make it bound if the distance is greater than 1 pixel (needs to stop bouncing eventually)
		//if(thisRef._y+1 >= endY){
		var tempY = ScreenElement.getYPosition(thisRef)
		
		if(tempY+1 >= endY){
			//end it
			//thisRef._y = endY;
			changeStyle(thisName, 'top', endY);
			thisRef.bouncing = false;
			killBounce(thisName);
		} else {
			//make it bounce!!
			bounce(thisName,tempY,endY);
		}
	}
};
function killBounce(objectName){
	var objectRef = document.getElementById(objectName);
	if(typeof objectRef.bounceID == "number") clearInterval(objectRef.bounceID);
	if(typeof objectRef.reboundID == "number") clearInterval(objectRef.reboundID);
};