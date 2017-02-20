'use strict';

var map;
var behavior;
var ui;
var pin; // pin indicating notional centre point of the geofence
var pointID = -1;
var Point;

var polygon = null;
var points = [];

var dragHandleLayer;

var isDrawing = false;
var isEditing = false;

var geofencePreviousLoc = null;

var screenName = "Geofence";

// ==================== Document Ready and Initialization ====================

$(document).ready(function () {

    pointID = $('#ctl00_ContentPlaceHolder1_lblPointID').text();
    screenName = $('#ctl00_ContentPlaceHolder1_lblScreen').text();

    if (screenName == "Geofence") {

        initializeMap();

        $('#btnEdit').click(toggleEdit);
        $('#btnDelete').click(clearAll);
        $('#btnDraw').click(toggleDrawing);
        $('#btnSave').click(updateGeofence);

        loadPoint();
    }
});


function initializeMap() {
    var defaultLayers = platform.createDefaultLayers();

    map = new H.Map(
        document.getElementById('map'),
        defaultLayers.satellite.map,
        {
            zoom: 16,
            center: { lat: 52.6047401428223, lng: 1.24522602558136 }
        }

    );

    behavior = new H.mapevents.Behavior(new H.mapevents.MapEvents(map));
    ui = H.ui.UI.createDefault(map, defaultLayers);

    dragHandleLayer = new H.map.Group();

    addMapDragHandlers();

}

// ==================== Map Event Handling ====================

function addMapDragHandlers() {
    map.addEventListener('dragstart', mapDragStartHandler, false);
    map.addEventListener('dragend', mapDragEndHandler, false);
    map.addEventListener('drag', mapDragHandler, false);
}

// disable map dragging if target is a marker or a polygon
function mapDragStartHandler(ev) {
    var target = ev.target;
    if (isDrawing || target instanceof H.map.Marker || target instanceof H.map.Polygon) {
        behavior.disable();
    }
}

// reenable map dragging  target is a marker or a polygon
function mapDragEndHandler(ev) {
    var target = ev.target;
    if (isDrawing || target instanceof H.map.Marker || target instanceof H.map.Polygon) {
        behavior.enable();
    }
}

// if dragging a marker then update its position
function mapDragHandler(ev) {
    var target = ev.target,
        pointer = ev.currentPointer;
    if (target instanceof mapsjs.map.Marker ) {
        target.setPosition(map.screenToGeo(pointer.viewportX, pointer.viewportY));
        if (dragHandleLayer.contains(target)) {
            updatePolygonFromDragHandles();
        }
    }

}

function addPolygonDragHandlers() {
    polygon.addEventListener('dragstart', polygonDragStartHandler, false);
    polygon.addEventListener('drag', polygonDragHandler, false);
}

function removePolygonDragHandlers() {
    polygon.removeEventListener('dragstart', polygonDragStartHandler, false);
    polygon.removeEventListener('drag', polygonDragHandler, false);
}

function polygonDragStartHandler(ev) {
    var pointer = ev.currentPointer;
    geofencePreviousLoc = map.screenToGeo(pointer.viewportX, pointer.viewportY);
}

function polygonDragHandler(ev) {

    var pointer = ev.currentPointer;
    var loc = map.screenToGeo(pointer.viewportX, pointer.viewportY);

    var latVariance = loc.lat - geofencePreviousLoc.lat;
    var lngVariance = loc.lng - geofencePreviousLoc.lng;

    geofencePreviousLoc = loc; 

    // update pin position
    var pinPosition = pin.getPosition();
    pinPosition.lat += latVariance;
    pinPosition.lng += lngVariance;
    pin.setPosition(pinPosition);


    //update geofence position
    var currentPoints = pointsFromStrip(polygon.getStrip());

    for (var i = 0; i < currentPoints.length; i++) {
        currentPoints[i].lat += latVariance;
        currentPoints[i].lng += lngVariance;
    }

    points = currentPoints;

    polygon.setStrip(stripFromPoints(points));

}

function attachDrawingHandlers() {

    map.addEventListener('pointerdown', mapMouseDownHandler, false);
    map.addEventListener('pointermove', mapMouseMoveHandler, false);
    
}

function detachDrawingHandlers() {
    map.removeEventListener('pointerdown', mapMouseDownHandler, false);
    map.removeEventListener('pointermove', mapMouseMoveHandler, false);
}

function mapMouseDownHandler(ev) {

        var pointer = ev.currentPointer;
        var loc = map.screenToGeo(pointer.viewportX, pointer.viewportY);
        var locNext = map.screenToGeo(pointer.viewportX + 1, pointer.viewportY);
        var locNext2 = map.screenToGeo(pointer.viewportX + 2, pointer.viewportY);

        if (!polygon) {

            // as this is a new polygon we need at least 3 points
            var newPoints = [];          
            newPoints.push(locNext);
            newPoints.push(loc);
            newPoints.push(locNext2);

            polygon = polygonFromPoints(newPoints);
            
            map.addObject(polygon);
           
        }
        else {          
            var strip = polygon.getStrip()
            // remove last point
            strip.removePoint(strip.getPointCount() - 1);
            strip.pushPoint(loc);
            strip.pushPoint(locNext);
            polygon.setStrip(strip);
        }

   
}

function mapMouseMoveHandler(ev) {

    if (polygon)
    {
        var pointer = ev.currentPointer;
        var loc = map.screenToGeo(pointer.viewportX, pointer.viewportY);
        var strip = polygon.getStrip()
        strip.removePoint(strip.getPointCount() - 1);
        strip.pushPoint(loc);
        polygon.setStrip(strip);
    }

}


// ==================== Point Service Calls ====================

function updateGeofence() {

    points = pointsFromStrip(polygon.getStrip());


    Point.GeofencePoints = points.map(function (point) {
        return { Latitude: point.lat, Longitude: point.lng }
    });

    Point.Radius = 0;

    Point.Latitude = pin.getPosition().lat;
    Point.Longitude = pin.getPosition().lng;

    PageMethods.UpdatePoint(Point, onUpdatePointSuccess, onUpdatePointError);

    return false;
}

function onUpdatePointSuccess(result) {
    showToast('Saved changes.')   
}

function onUpdatePointError(error) {
    showToast('Unable to save changes.')
}

function loadPoint() {
    var point = PageMethods.LoadPoint(pointID, onloadPointSuccess, onloadPointError);
}

function onloadPointSuccess(result) {
    
    Point = result;

    if (Point.PointID == 0)
        Point.PointID = pointID;
    else {
        displayPoint();
    }
}

function onloadPointError(error) {
    alert("loadPoint Failed: " + error);
}

// ==================== Functions ====================

function displayPoint() {

    clearAll();

    // Set the point pin location
    var loc = { lat: Point.Latitude, lng: Point.Longitude, alt: 0, };

    createPin(loc);

    points = Point.GeofencePoints.map(function (point) {
        return { lat: point.Latitude, lng: point.Longitude }
    });

    // Draw the Geofence
    polygon = polygonFromPoints(points)
    polygon.draggable = true;
    map.addObject(polygon);

    addPolygonDragHandlers(polygon);

}

function toggleEdit() {

    if (polygon) {

        if (isEditing) {
            addPolygonDragHandlers();
            dragHandleLayer.removeAll();
            map.removeObject(dragHandleLayer);
        }
        else {
            removePolygonDragHandlers();
            drawDragHandles();
        }

        $("#btnEdit").text(isEditing ? 'Edit Geofence' : 'Stop Editing');
        isEditing = !isEditing;


    }

    return false;
}

function clearAll() {
    // reset all objects first.
    if (polygon) {
        removePolygonDragHandlers(polygon);
    }

    points = [];
    polygon = null;
    dragHandleLayer.removeAll();
    map.removeObjects(map.getObjects());

    return false;
}

function toggleDrawing() {
    if (!isDrawing) {
        $('#map > *').css('cursor', 'crosshair');
        $('#btnDraw').text('Finish Drawing');
        $('#btnEdit').prop('disabled', true);
        $('#btnSave').prop('disabled', true);

        clearAll();

        attachDrawingHandlers();
    }
    else {

        $('#map > *').css('cursor', 'auto');
        $('#btnDraw').text('Begin Drawing');
        $('#btnEdit').prop('disabled', false);      

        if (polygon) {

            $('#btnSave').prop('disabled', false);
            updatePolygonAfterDrawing();
        }

        detachDrawingHandlers();
    }

    isDrawing = !isDrawing;

    return false;
}

function getAveragePoint(points) {

    var latSum = 0.0;
    var lngSum = 0.0;

    points.forEach(function (point) {
        latSum += point.lat;
        lngSum += point.lng;
    })

    return { lat: latSum / points.length, lng: lngSum / points.length };
}

function pointsFromStrip(strip) {

    var points = [];

    for (var i = 0; i < strip.getPointCount() ; i++) {
        points.push(strip.extractPoint(i));
    }

    return points;
}

function stripFromPoints(points) {

    var strip = new H.geo.Strip();

    points.forEach(function (point) {
        strip.pushPoint(point);
    });

    return strip;
}

function polygonFromPoints(points) {

    var polygonStrip = stripFromPoints(points);

    var polygon = new H.map.Polygon(polygonStrip, { style: { strokeColor: 'rgba(0, 0, 200, 0.8)', fillColor: 'rgba(0, 0, 200, 0.3)', lineWidth: 2 } });

    return polygon;
}

function updatePolygonFromDragHandles() {
    var newPoints = dragHandleLayer.getObjects().map(function (point) {
        return point.getPosition();
    });

    polygon.setStrip(stripFromPoints(newPoints));
}

function updatePolygonAfterDrawing() {

    // get rid of first and last points (last one has not had a mouse press and first one was needed only to to get a polygon started with at least threee points)
    var strip = polygon.getStrip();
    // we need at least 3 points
    if (strip.getPointCount() >= 5) {
        strip.removePoint(strip.getPointCount() - 1);
        strip.removePoint(0);
        polygon.setStrip(strip);
    }

    var centrePoint = getAveragePoint(pointsFromStrip(polygon.getStrip()));

    createPin(centrePoint);
}

function createPin(point) {
    pin = new H.map.Marker(point);
    pin.draggable = true;
    map.addObject(pin);
    map.setCenter(point);
}

function createDragIcon() {
    return new H.map.Icon('/images/draghandle2.gif', {
        size: { w: 10, h: 10 },
        anchor: { x: 5, y: 5 }
    });
}

function drawDragHandles() {

    var dragPoints = pointsFromStrip(polygon.getStrip());

    // Set up the geofence point dragging layer
    dragPoints.forEach(function (point) {
        var dragHandle = new H.map.Marker(point, { icon: createDragIcon() });
        dragHandle.draggable = true;
        dragHandleLayer.addObject(dragHandle);
    });

    map.addObject(dragHandleLayer);

}

function showToast(message) {
    $('#savedMessage').text(message)
    $('#savedMessage').show();
    setTimeout(function () { $('#savedMessage').fadeOut(); }, 1000);
}



