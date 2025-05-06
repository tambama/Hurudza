// Correct import path for the shared map drawing module
import * as mapDrawing from '../../js/modules/mapDrawingModule.js';

let localDotNetRef = null;

/**
 * Initialize the map for field creation
 * @param {HTMLElement} element - The DOM element to attach the map to
 * @returns {Object} The map instance
 */
export function initializeMap(element) {
    console.log("Initializing map for field creation");
    return mapDrawing.initializeMap(element);
}

/**
 * Initialize the drawing controls with custom styling
 * @param {Object} map - The Mapbox map instance
 * @param {Object} dotNetRef - Reference to .NET component
 * @returns {Object} The drawing control instance
 */
export function initializeDrawControls(map, dotNetRef) {
    console.log("Initializing draw controls for field creation");

    // Store reference locally in this module
    if (dotNetRef) {
        localDotNetRef = dotNetRef;
        console.log("Stored dotNetRef locally in CreateField.razor.js");
    } else {
        console.warn("No dotNetRef provided to initializeDrawControls");
    }
    
    return mapDrawing.initializeDrawControls(map, dotNetRef);
}

/**
 * Setup all drawing event handlers
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The MapboxDraw instance
 * @param {Object} dotNetRef - Reference to .NET component
 */
export function setupDrawingEvents(map, draw, dotNetRef) {
    console.log("Setting up drawing event handlers for field creation");

    if (dotNetRef) {
        localDotNetRef = dotNetRef;
        console.log("Updated dotNetRef in setupDrawingEvents");
    } else if (localDotNetRef) {
        // Use locally stored reference if none provided
        dotNetRef = localDotNetRef;
        console.log("Using stored dotNetRef");
    } else {
        console.warn("No dotNetRef available");
    }

    // Make sure we're passing the dotNetRef to mapDrawing module
    return mapDrawing.setupDrawingEvents(map, draw, dotNetRef);
}

/**
 * Draw a farm boundary on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} farmId - Farm ID
 * @param {Array} polygons - Farm boundary polygon coordinates
 * @param {string} farmName - Farm name for display
 * @returns {boolean} Success indicator
 */
export function drawFarmBoundary(map, farmId, polygons, farmName) {
    console.log(`Drawing farm boundary for ${farmName} in field creation view`);
    return mapDrawing.drawFarmBoundary(map, farmId, polygons, farmName);
}

/**
 * Clears all custom sources and layers from the map
 * @param {Object} map - The Mapbox map instance
 * @returns {boolean} Success indicator
 */
export function clearMap(map) {
    console.log("Clearing map layers in field creation view");
    return mapDrawing.clearMap(map);
}

/**
 * Center map on coordinates with zoom level
 * @param {Object} map - The Mapbox map instance
 * @param {number} latitude - Latitude coordinate
 * @param {number} longitude - Longitude coordinate
 * @param {number} zoom - Zoom level (default: 14)
 * @returns {boolean} Success indicator
 */
export function centerMap(map, latitude, longitude, zoom = 14) {
    console.log(`Centering field creation map on [${longitude}, ${latitude}] with zoom ${zoom}`);
    return mapDrawing.centerMap(map, latitude, longitude, zoom);
}

/**
 * Fit the map view to bounds with padding
 * @param {Object} map - The Mapbox map instance
 * @param {number} minLat - Minimum latitude
 * @param {number} minLng - Minimum longitude
 * @param {number} maxLat - Maximum latitude
 * @param {number} maxLng - Maximum longitude
 * @returns {boolean} Success indicator
 */
export function fitMapToBounds(map, minLat, minLng, maxLat, maxLng) {
    console.log(`Fitting field creation map to bounds: SW(${minLng}, ${minLat}), NE(${maxLng}, ${maxLat})`);
    return mapDrawing.fitMapToBounds(map, minLat, minLng, maxLat, maxLng);
}

/**
 * Add visual attention to the draw tool button to guide users
 * @returns {boolean} Success indicator
 */
export function highlightDrawTool() {
    console.log("Highlighting draw tool to guide user");
    return mapDrawing.highlightDrawTool();
}

/**
 * Load an existing polygon for editing
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The drawing control
 * @param {Array} coordinates - Array of coordinates
 * @returns {boolean} Success indicator
 */
export function loadExistingPolygon(map, draw, coordinates) {
    console.log(`Loading existing polygon with ${coordinates?.length || 0} points for editing`);
    return mapDrawing.loadPolygonForEditing(map, draw, coordinates);
}

// Export all standard drawing functions from the module for use in CreateField.razor
export const loadTurf = mapDrawing.loadTurf;
export const clearAllDrawings = mapDrawing.clearAllDrawings;
export const highlightField = mapDrawing.highlightField;
export const unhighlightField = mapDrawing.unhighlightField;