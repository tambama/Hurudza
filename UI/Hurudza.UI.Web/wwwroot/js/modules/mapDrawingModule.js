// Map Drawing Module - A standardized implementation for polygon drawing
// This module can be used by both Map.razor.js and CreateField.razor.js

// Import Mapbox GL JS
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
// Import MapboxDraw for boundary drawing
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-draw/v1.3.0/mapbox-gl-draw.js';

// Mapbox access token
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

// Global variables to maintain state
let drawButton = null;
let deleteButton = null;
let isDrawingModeActive = false;
let originalCursor = '';

// Store the .NET reference at module level to ensure it's available for all callbacks
let dotNetReference = null;

/**
 * Initialize Mapbox with the access token
 */
export function initializeMapbox() {
    mapboxgl.accessToken = mapboxToken;
    console.log("Mapbox initialized with token");
    return mapboxgl;
}

/**
 * Initialize map on the provided element
 * @param {HTMLElement} element - The DOM element to attach the map to
 * @returns {Object} - The map instance
 */
export function initializeMap(element) {
    try {
        console.log("Initializing map");

        // Initialize Mapbox
        initializeMapbox();

        // Create map instance
        const map = new mapboxgl.Map({
            container: element,
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: [31.053028, -17.824858], // Default to Zimbabwe
            zoom: 7,
            attributionControl: true
        });

        // Add navigation controls
        map.addControl(new mapboxgl.NavigationControl(), 'bottom-right');

        // Add scale control
        map.addControl(new mapboxgl.ScaleControl({
            maxWidth: 200,
            unit: 'metric'
        }), 'bottom-left');

        // Add fullscreen control
        map.addControl(new mapboxgl.FullscreenControl(), 'top-right');

        // Wait for map to load before returning
        map.on('load', () => {
            console.log("Map fully loaded");
        });

        console.log("Map initialized successfully");
        return map;
    } catch (error) {
        console.error("Error initializing map:", error);
        throw error;
    }
}

/**
 * Initialize drawing controls
 * @param {Object} map - The Mapbox map instance
 * @param {Object} dotNetRef - Reference to .NET component
 * @returns {Object} - The drawing control instance
 */
export function initializeDrawControls(map, dotNetRef) {
    try {
        console.log("Initializing draw controls");

        // Store the .NET reference at module level
        dotNetReference = dotNetRef;

        // Create Mapbox Draw instance with standardized options
        const draw = new MapboxDraw({
            displayControlsDefault: false,
            controls: {
                polygon: true,
                trash: true
            },
            // Standardized styles for both implementations
            styles: [
                // Polygon fill
                {
                    'id': 'gl-draw-polygon-fill',
                    'type': 'fill',
                    'filter': ['all', ['==', '$type', 'Polygon'], ['!=', 'mode', 'static']],
                    'paint': {
                        'fill-color': '#ff9966',
                        'fill-outline-color': '#ff5500',
                        'fill-opacity': 0.3
                    }
                },
                // Polygon outline
                {
                    'id': 'gl-draw-polygon-stroke-active',
                    'type': 'line',
                    'filter': ['all', ['==', '$type', 'Polygon'], ['!=', 'mode', 'static']],
                    'layout': {
                        'line-cap': 'round',
                        'line-join': 'round'
                    },
                    'paint': {
                        'line-color': '#ff5500',
                        'line-width': 2
                    }
                },
                // Polygon mid points
                {
                    'id': 'gl-draw-polygon-midpoint',
                    'type': 'circle',
                    'filter': ['all', ['==', '$type', 'Point'], ['==', 'meta', 'midpoint']],
                    'paint': {
                        'circle-radius': 4,
                        'circle-color': '#fbb03b'
                    }
                },
                // Polygon vertices
                {
                    'id': 'gl-draw-polygon-vertex-active',
                    'type': 'circle',
                    'filter': ['all', ['==', 'meta', 'vertex'], ['==', '$type', 'Point']],
                    'paint': {
                        'circle-radius': 6,
                        'circle-color': '#fbb03b'
                    }
                }
            ]
        });

        // Add draw controls to map
        map.addControl(draw);

        // Create custom drawing and delete buttons
        createCustomControls(map, draw);

        console.log("Draw controls initialized");
        return draw;
    } catch (error) {
        console.error("Error initializing draw controls:", error);
        throw error;
    }
}

/**
 * Create custom drawing and delete buttons
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The MapboxDraw instance
 * @returns {Object} - References to the created buttons
 */
function createCustomControls(map, draw) {
    try {
        // Create a container for custom controls
        const controlContainer = document.createElement('div');
        controlContainer.className = 'mapboxgl-ctrl-top-right';
        controlContainer.style.marginTop = '80px'; // Position below fullscreen control
        map.getContainer().appendChild(controlContainer);

        // Create the drawing button
        drawButton = document.createElement('button');
        drawButton.id = 'custom-draw-button';
        drawButton.className = 'mapboxgl-ctrl mapboxgl-ctrl-group';
        drawButton.innerHTML = '<span class="mapboxgl-ctrl-icon" style="display:flex;align-items:center;justify-content:center;height:100%;"><i class="fas fa-draw-polygon"></i></span>';
        drawButton.title = 'Draw Field Boundary';
        drawButton.style.cursor = 'pointer';
        controlContainer.appendChild(drawButton);

        // Create the delete button
        deleteButton = document.createElement('button');
        deleteButton.id = 'custom-delete-button';
        deleteButton.className = 'mapboxgl-ctrl mapboxgl-ctrl-group';
        deleteButton.innerHTML = '<span class="mapboxgl-ctrl-icon" style="display:flex;align-items:center;justify-content:center;height:100%;color:#dc3545;"><i class="fas fa-trash-alt"></i></span>';
        deleteButton.title = 'Delete Field Boundary';
        deleteButton.style.cursor = 'pointer';
        deleteButton.style.marginTop = '10px';
        deleteButton.style.display = 'none'; // Initially hidden
        controlContainer.appendChild(deleteButton);

        // Drawing button click handler
        drawButton.addEventListener('click', () => {
            toggleDrawingMode(map, draw);
        });

        // Delete button click handler
        deleteButton.addEventListener('click', () => {
            // Delete all drawings
            draw.deleteAll();

            // Hide delete button
            deleteButton.style.display = 'none';

            // Turn off drawing mode
            isDrawingModeActive = false;
            updateDrawButtonState();

            // Reset drawing cursor
            map.getCanvas().style.cursor = originalCursor || '';

            // Notify .NET - use module level reference
            if (dotNetReference) {
                dotNetReference.invokeMethodAsync('OnDrawingDeleted')
                    .catch(error => {
                        console.warn("Error notifying .NET of drawing deletion:", error);
                    });
            }
        });

        console.log("Custom controls created");
        return { drawButton, deleteButton };
    } catch (error) {
        console.error("Error creating custom controls:", error);
        return null;
    }
}

/**
 * Toggle drawing mode
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The MapboxDraw instance
 * @returns {boolean} - Success indicator
 */
function toggleDrawingMode(map, draw) {
    try {
        // Toggle state
        isDrawingModeActive = !isDrawingModeActive;

        // Update button appearance
        updateDrawButtonState();

        if (isDrawingModeActive) {
            // When activating drawing mode:

            // Store original cursor
            originalCursor = map.getCanvas().style.cursor;

            // 1. Set cursor to crosshair
            map.getCanvas().style.cursor = 'crosshair';

            // 2. Enter drawing mode
            draw.changeMode('draw_polygon');

            // 3. Remove any popups
            const popups = document.querySelectorAll('.mapboxgl-popup');
            popups.forEach(popup => popup.remove());

            console.log("Drawing mode activated");
        } else {
            // When deactivating drawing mode:

            // 1. Reset cursor
            map.getCanvas().style.cursor = originalCursor || '';

            // 2. Switch to selection mode
            draw.changeMode('simple_select');

            console.log("Drawing mode deactivated");
        }

        // Notify .NET of drawing mode change using module-level reference
        if (dotNetReference) {
            console.log("Notifying .NET of drawing mode change");
            dotNetReference.invokeMethodAsync('SetDrawingMode', isDrawingModeActive)
                .catch(error => {
                    console.warn("Error notifying .NET of drawing mode change:", error);
                });
        } else {
            console.warn("dotNetReference is null, cannot notify .NET of drawing mode change");
        }

        return true;
    } catch (error) {
        console.error("Error toggling drawing mode:", error);
        return false;
    }
}

/**
 * Update draw button appearance based on active state
 */
function updateDrawButtonState() {
    if (!drawButton) return;

    if (isDrawingModeActive) {
        drawButton.style.background = '#ff5500';
        drawButton.querySelector('.mapboxgl-ctrl-icon').style.color = 'white';
    } else {
        drawButton.style.background = '';
        drawButton.querySelector('.mapboxgl-ctrl-icon').style.color = '';
    }
}

/**
 * Load Turf.js for geometric calculations
 * @returns {Promise} - Promise that resolves with the turf object
 */
export function loadTurf() {
    return new Promise((resolve, reject) => {
        // Check if Turf is already loaded
        if (window.turf) {
            resolve(window.turf);
            return;
        }

        // Create script element
        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/@turf/turf@6/turf.min.js';
        script.crossOrigin = 'anonymous';

        script.onload = () => {
            if (window.turf) {
                resolve(window.turf);
            } else {
                reject(new Error('Turf.js loaded but not available in window.turf'));
            }
        };

        script.onerror = () => {
            reject(new Error('Failed to load Turf.js'));
        };

        document.head.appendChild(script);
    });
}

/**
 * Setup event handlers for drawing
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The MapboxDraw instance
 * @param {Object} dotNetRef - Reference to .NET component
 */
export async function setupDrawingEvents(map, draw, dotNetRef) {
    try {
        console.log("Setting up drawing event handlers");

        // Ensure we store the .NET reference at module level
        if (dotNetRef) {
            dotNetReference = dotNetRef;
            console.log("Successfully stored .NET reference");
        } else {
            console.warn("No dotNetRef provided to setupDrawingEvents");
        }

        // Load Turf.js if needed
        if (typeof turf === 'undefined') {
            await loadTurf();
        }

        // Handle create events
        map.on('draw.create', function(e) {
            // Show delete button when a polygon is created
            if (deleteButton) {
                deleteButton.style.display = 'block';
            }

            // Turn off drawing mode after creation
            isDrawingModeActive = false;
            updateDrawButtonState();

            // Reset cursor
            map.getCanvas().style.cursor = originalCursor || '';

            // Process the drawn polygon
            if (e.features && e.features.length > 0) {
                const polygon = e.features[0];
                processDrawnPolygon(polygon);
            }
        });

        // Handle update events
        map.on('draw.update', function(e) {
            // Process the updated polygon
            if (e.features && e.features.length > 0) {
                const polygon = e.features[0];
                processDrawnPolygon(polygon);
            }
        });

        // Handle delete events
        map.on('draw.delete', function(e) {
            // Hide delete button when all polygons are deleted
            const data = draw.getAll();
            if (data.features.length === 0 && deleteButton) {
                deleteButton.style.display = 'none';
            }

            // Notify .NET that the drawing was deleted
            if (dotNetReference) {
                dotNetReference.invokeMethodAsync('OnDrawingDeleted')
                    .catch(error => {
                        console.warn("Error notifying .NET of drawing deletion:", error);
                    });
            }
        });

        // Add mouse blocking during draw mode to prevent unwanted interactions
        map.on('click', function(e) {
            if (isDrawingModeActive) {
                // For Mapbox events, we use originalEvent to access the DOM event
                if (e.originalEvent) {
                    e.originalEvent.preventDefault();
                    e.originalEvent.stopPropagation();
                }

                // Remove any popups that might have appeared
                const popups = document.querySelectorAll('.mapboxgl-popup');
                popups.forEach(popup => popup.remove());
            }
        }, true); // Use capturing phase to intercept before other handlers

        console.log("Drawing event handlers set up successfully");
    } catch (error) {
        console.error("Error setting up drawing events:", error);
    }
}

/**
 * Process a drawn polygon - calculate area and extract coordinates
 * @param {Object} polygon - The GeoJSON polygon feature
 */
function processDrawnPolygon(polygon) {
    if (polygon && polygon.geometry && polygon.geometry.type === 'Polygon') {
        try {
            // Calculate area in square meters, convert to hectares
            const area = turf.area(polygon);
            const areaInHectares = area / 10000; // 1 hectare = 10,000 sqm

            console.log(`Calculated area: ${areaInHectares.toFixed(2)} hectares`);

            // Extract coordinates (omitting the last point which duplicates the first)
            const coordinates = polygon.geometry.coordinates[0].slice(0, -1).map(coord => {
                return [coord[0], coord[1]]; // longitude, latitude
            });

            // Use module-level .NET reference
            if (dotNetReference) {
                // Update field size in the form
                dotNetReference.invokeMethodAsync('UpdateFieldSize', areaInHectares)
                    .catch(error => {
                        console.warn("Error notifying .NET of field size:", error);
                    });

                // Send coordinates to .NET
                dotNetReference.invokeMethodAsync('OnPolygonDrawn', coordinates)
                    .catch(error => {
                        console.warn("Error notifying .NET of polygon coordinates:", error);
                    });
            } else {
                console.warn("dotNetReference is null, cannot send polygon data to .NET");
            }
        } catch (error) {
            console.error("Error processing polygon:", error);
        }
    }
}

/**
 * Clear existing polygons and hide the delete button
 * @param {Object} draw - The MapboxDraw instance
 */
export function clearAllDrawings(draw) {
    try {
        if (draw) {
            draw.deleteAll();
            console.log("All drawings cleared");
        }

        // Hide delete button
        if (deleteButton) {
            deleteButton.style.display = 'none';
        }

        return true;
    } catch (error) {
        console.error("Error clearing drawings:", error);
        return false;
    }
}

/**
 * Loads an existing polygon for editing
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The MapboxDraw instance
 * @param {Array} coordinates - Array of [lng, lat] coordinates
 * @returns {boolean} - Success indicator
 */
export function loadPolygonForEditing(map, draw, coordinates) {
    try {
        console.log(`Loading polygon with ${coordinates.length} points for editing`);

        if (!coordinates || !Array.isArray(coordinates) || coordinates.length < 3) {
            console.warn("Not enough coordinates for a valid polygon");
            return false;
        }

        // Clear any existing drawings
        draw.deleteAll();

        // Create a GeoJSON polygon feature
        const polygonFeature = {
            type: 'Feature',
            properties: {},
            geometry: {
                type: 'Polygon',
                coordinates: [
                    // Ensure polygon is closed by adding first point at the end if needed
                    coordinates.length > 0 &&
                    (coordinates[0][0] !== coordinates[coordinates.length - 1][0] ||
                        coordinates[0][1] !== coordinates[coordinates.length - 1][1])
                        ? [...coordinates, [...coordinates[0]]]
                        : coordinates
                ]
            }
        };

        // Add to draw control and get the ID
        const ids = draw.add(polygonFeature);

        if (ids && ids.length > 0) {
            // Show delete button
            if (deleteButton) {
                deleteButton.style.display = 'block';
            }

            // Switch to direct select mode for the added polygon
            draw.changeMode('direct_select', { featureId: ids[0] });

            console.log(`Loaded polygon with ID: ${ids[0]}`);
            return true;
        } else {
            console.warn("Failed to add polygon to draw control");
            return false;
        }
    } catch (error) {
        console.error("Error loading polygon for editing:", error);
        return false;
    }
}

/**
 * Draw a farm or field boundary on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} id - The entity ID
 * @param {Array} polygons - The polygon coordinates
 * @param {boolean} isField - Whether this is a field (vs farm)
 * @param {string} name - Name to display in popup
 * @param {Object} cropData - Optional crop data for field polygons
 * @param {boolean} clearExisting - Whether to clear existing layers
 * @returns {boolean} - Success indicator
 */
export function drawPolygon(map, id, polygons, isField = false, name = 'Farm', cropData = null, clearExisting = false) {
    try {
        console.log(`Drawing ${isField ? 'field' : 'farm'} polygon: ${name}`);

        // Define source and layer IDs with unique prefixes to avoid conflicts
        const prefix = isField ? 'field-' : 'farm-';
        const sourceId = `${prefix}source-${id}`;
        const outlineLayerId = `${prefix}outline-${id}`;
        const fillLayerId = `${prefix}fill-${id}`;
        const hoverFillLayerId = `${prefix}hover-fill-${id}`;

        // If clearExisting flag is true, clear all layers before drawing
        if (clearExisting) {
            clearMapLayers(map);
        }

        // Check if this source already exists and remove it to avoid duplicates
        if (map.getSource(sourceId)) {
            // Remove associated layers first
            if (map.getLayer(outlineLayerId)) {
                map.removeLayer(outlineLayerId);
            }

            if (map.getLayer(fillLayerId)) {
                map.removeLayer(fillLayerId);
            }

            if (map.getLayer(hoverFillLayerId)) {
                map.removeLayer(hoverFillLayerId);
            }

            // Then remove the source
            map.removeSource(sourceId);
        }

        // Add the source for this polygon
        map.addSource(sourceId, {
            'type': 'geojson',
            'data': {
                'type': 'Feature',
                'geometry': {
                    'type': 'Polygon',
                    'coordinates': polygons
                },
                'properties': {
                    'name': name,
                    'cropData': cropData,
                    'isField': isField,
                    'id': id
                }
            }
        });

        // Set colors based on whether this is a field or farm
        const outlineColor = isField ? '#ff5500' : '#40b7d5';
        const outlineWidth = isField ? 2 : 3;
        const fillColor = isField ? (cropData ? '#74c476' : '#ff9966') : '#40b7d5';
        const fillOpacity = isField ? 0.3 : 0.2;

        // Add fill layer (visible)
        map.addLayer({
            'id': fillLayerId,
            'type': 'fill',
            'source': sourceId,
            'layout': {},
            'paint': {
                'fill-color': fillColor,
                'fill-opacity': fillOpacity
            }
        });

        // Add outline layer
        map.addLayer({
            'id': outlineLayerId,
            'type': 'line',
            'source': sourceId,
            'layout': {},
            'paint': {
                'line-color': outlineColor,
                'line-width': outlineWidth
            }
        });

        // Add hover fill layer (initially invisible)
        map.addLayer({
            'id': hoverFillLayerId,
            'type': 'fill',
            'source': sourceId,
            'layout': {},
            'paint': {
                'fill-color': isField ? '#ff0000' : fillColor, // Red for fields on hover
                'fill-opacity': 0 // Initially transparent
            }
        });

        // Add click handler for popup
        map.on('click', fillLayerId, (e) => {
            // Only show popups when not in drawing mode
            if (isDrawingModeActive) return;

            // Get the coordinates of the click point
            const coordinates = e.lngLat;
            const feature = e.features[0];
            const properties = feature.properties;
            const isFieldProp = properties.isField;
            const entityName = properties.name;
            const entityCropData = properties.cropData;

            // Build popup content
            let popupContent = `<div class="popup-content" style="max-width: 250px; word-wrap: break-word;">
                                    <h4 style="margin-bottom: 5px;">${entityName}</h4>`;

            if (isFieldProp) {
                if (entityCropData && Object.keys(entityCropData).length > 0) {
                    // Add crop information to popup if available
                    const plantedDate = entityCropData.plantedDate ?
                        new Date(entityCropData.plantedDate).toLocaleDateString() : 'Not set';
                    const harvestDate = entityCropData.harvestDate ?
                        new Date(entityCropData.harvestDate).toLocaleDateString() : 'Not set';

                    popupContent += `
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Current Crop:</strong> ${entityCropData.crop}</p>
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Area Planted:</strong> ${entityCropData.size} ha</p>
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Planted:</strong> ${plantedDate}</p>
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Expected Harvest:</strong> ${harvestDate}</p>
                        <p style="margin-bottom: 0; font-size: 12px;"><strong>Irrigation:</strong> ${entityCropData.irrigation ? 'Yes' : 'No'}</p>
                    `;
                } else {
                    popupContent += `<p>No crops currently planted</p>`;
                }
            }

            popupContent += `</div>`;

            // Create and display the popup
            new mapboxgl.Popup({
                closeButton: true,
                closeOnClick: true,
                maxWidth: '300px'
            })
                .setLngLat(coordinates)
                .setHTML(popupContent)
                .addTo(map);
        });

        // Add mouse interaction handlers for hover effects
        map.on('mouseenter', fillLayerId, () => {
            if (isDrawingModeActive) return; // Skip hover effects while drawing

            map.getCanvas().style.cursor = 'pointer';

            // Only apply the fill effect for field layers, not farm boundaries
            if (isField) {
                map.setPaintProperty(hoverFillLayerId, 'fill-opacity', 0.3);

                // Also change the outline to red for fields
                map.setPaintProperty(outlineLayerId, 'line-color', '#ff0000');
                map.setPaintProperty(outlineLayerId, 'line-width', 3); // Make it slightly thicker
            }
        });

        map.on('mouseleave', fillLayerId, () => {
            map.getCanvas().style.cursor = '';

            // Reset fill opacity
            map.setPaintProperty(hoverFillLayerId, 'fill-opacity', 0);

            // Reset outline color for fields
            if (isField) {
                map.setPaintProperty(outlineLayerId, 'line-color', outlineColor);
                map.setPaintProperty(outlineLayerId, 'line-width', outlineWidth);
            }
        });

        console.log(`Successfully drew ${isField ? 'field' : 'farm'} polygon for ${id}`);
        return true;
    } catch (error) {
        console.error(`Error drawing polygon for ${id}:`, error);
        return false;
    }
}

/**
 * Draw a farm boundary with standardized styling
 * @param {Object} map - The Mapbox map instance
 * @param {string} farmId - Farm ID
 * @param {Array} polygons - Farm boundary polygons
 * @param {string} farmName - Farm name
 * @returns {boolean} - Success indicator
 */
export function drawFarmBoundary(map, farmId, polygons, farmName) {
    return drawPolygon(map, farmId, polygons, false, farmName, null, true);
}

/**
 * Highlight a field on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} fieldId - Field ID to highlight
 */
export function highlightField(map, fieldId) {
    try {
        // Skip highlighting if in drawing mode
        if (isDrawingModeActive) return;

        // The field layer IDs follow the pattern 'field-outline-{fieldId}' and 'field-hover-fill-{fieldId}'
        const outlineLayerId = `field-outline-${fieldId}`;
        const hoverFillLayerId = `field-hover-fill-${fieldId}`;

        // Check if the layers exist to avoid errors
        if (map.getLayer(outlineLayerId) && map.getLayer(hoverFillLayerId)) {
            // Change the outline color to red and make it thicker
            map.setPaintProperty(outlineLayerId, 'line-color', '#ff0000');
            map.setPaintProperty(outlineLayerId, 'line-width', 3);

            // Show the fill with opacity
            map.setPaintProperty(hoverFillLayerId, 'fill-opacity', 0.3);

            console.log(`Highlighted field ${fieldId}`);
        }
    } catch (error) {
        console.error(`Error highlighting field ${fieldId}:`, error);
    }
}

/**
 * Remove highlight from a field
 * @param {Object} map - The Mapbox map instance
 * @param {string} fieldId - Field ID to unhighlight
 */
export function unhighlightField(map, fieldId) {
    try {
        // Skip unhighlighting if in drawing mode
        if (isDrawingModeActive) return;

        // The field layer IDs follow the pattern 'field-outline-{fieldId}' and 'field-hover-fill-{fieldId}'
        const outlineLayerId = `field-outline-${fieldId}`;
        const hoverFillLayerId = `field-hover-fill-${fieldId}`;

        // Check if the layers exist to avoid errors
        if (map.getLayer(outlineLayerId) && map.getLayer(hoverFillLayerId)) {
            // Reset the outline color to orange and width back to normal
            map.setPaintProperty(outlineLayerId, 'line-color', '#ff5500');
            map.setPaintProperty(outlineLayerId, 'line-width', 2);

            // Hide the fill
            map.setPaintProperty(hoverFillLayerId, 'fill-opacity', 0);

            console.log(`Unhighlighted field ${fieldId}`);
        }
    } catch (error) {
        console.error(`Error unhighlighting field ${fieldId}:`, error);
    }
}

/**
 * Reset all custom map layers
 * @param {Object} map - The Mapbox map instance
 * @returns {Promise<boolean>} - Success indicator
 */
export function clearMapLayers(map) {
    return new Promise((resolve) => {
        try {
            // Wait for any pending operations to complete
            setTimeout(() => {
                try {
                    // Get all map style layers
                    const style = map.getStyle();

                    if (!style || !style.layers || !style.sources) {
                        console.warn('Map style not fully loaded, cannot reset layers');
                        resolve(false);
                        return;
                    }

                    // Find custom layers (farm and field layers)
                    const customLayers = style.layers.filter(layer => {
                        return (layer.id.startsWith('farm-') ||
                            layer.id.startsWith('field-') ||
                            layer.id === 'clusters' ||
                            layer.id === 'cluster-count' ||
                            layer.id === 'unclustered-point');
                    });

                    // Find custom sources
                    const customSourceIds = Object.keys(style.sources).filter(id => {
                        return (id.startsWith('farm-') ||
                            id.startsWith('field-') ||
                            id === 'farms');
                    });

                    // Remove all custom layers first
                    customLayers.forEach(layer => {
                        if (map.getLayer(layer.id)) {
                            // Remove event handlers first
                            try {
                                map.off('click', layer.id);
                                map.off('mouseenter', layer.id);
                                map.off('mouseleave', layer.id);
                            } catch (e) {
                                console.warn(`Error removing handlers for layer ${layer.id}:`, e);
                            }

                            // Then remove the layer
                            map.removeLayer(layer.id);
                        }
                    });

                    // Then remove custom sources
                    customSourceIds.forEach(sourceId => {
                        if (map.getSource(sourceId)) {
                            map.removeSource(sourceId);
                        }
                    });

                    // Trigger map repaint to apply changes
                    map.triggerRepaint();

                    console.log('All custom layers and sources removed');
                    resolve(true);
                } catch (e) {
                    console.error('Error in clearMapLayers:', e);
                    resolve(false);
                }
            }, 100); // Small delay to ensure map is ready
        } catch (e) {
            console.error('Fatal error in clearMapLayers:', e);
            resolve(false);
        }
    });
}

/**
 * Center map on specific coordinates with zoom level
 * @param {Object} map - The Mapbox map instance
 * @param {number} latitude - Latitude coordinate
 * @param {number} longitude - Longitude coordinate
 * @param {number} zoom - Zoom level
 * @returns {boolean} - Success indicator
 */
export function centerMap(map, latitude, longitude, zoom = 14) {
    try {
        console.log(`Centering map on [${longitude}, ${latitude}] with zoom ${zoom}`);

        map.flyTo({
            center: [longitude, latitude],
            zoom: zoom,
            essential: true,
            duration: 1000 // Animation duration in milliseconds
        });

        return true;
    } catch (error) {
        console.error('Error centering map:', error);
        return false;
    }
}

/**
 * Fit map view to bounds with padding
 * @param {Object} map - The Mapbox map instance
 * @param {number} minLat - Minimum latitude
 * @param {number} minLng - Minimum longitude
 * @param {number} maxLat - Maximum latitude
 * @param {number} maxLng - Maximum longitude
 * @returns {boolean} - Success indicator
 */
export function fitMapToBounds(map, minLat, minLng, maxLat, maxLng) {
    try {
        console.log(`Fitting map to bounds: [${minLng}, ${minLat}], [${maxLng}, ${maxLat}]`);

        // Add padding to ensure the entire polygon is visible
        const padding = 50; // Pixels of padding around the bounds

        map.fitBounds(
            [
                [minLng, minLat], // Southwest corner
                [maxLng, maxLat]  // Northeast corner
            ],
            {
                padding: padding,
                duration: 1000, // Animation duration in milliseconds
                maxZoom: 16 // Prevent zooming in too close
            }
        );

        console.log("Map fitted to bounds successfully");
        return true;
    } catch (error) {
        console.error("Error fitting map to bounds:", error);
        return false;
    }
}

/**
 * Highlight the draw button to guide users
 * @returns {boolean} - Success indicator
 */
export function highlightDrawTool() {
    if (!drawButton) return false;

    try {
        // Add pulsing animation class
        drawButton.classList.add('pulse-animation');

        // Remove animation after 5 seconds
        setTimeout(() => {
            drawButton.classList.remove('pulse-animation');
        }, 5000);

        return true;
    } catch (error) {
        console.error("Error highlighting draw tool:", error);
        return false;
    }
}

/**
 * Toggle drawing tool button visibility
 * @param {boolean} show - Whether to show the button
 * @returns {boolean} - Success indicator
 */
export function toggleDrawButtonVisibility(show = true) {
    try {
        if (drawButton) {
            drawButton.style.display = show ? 'block' : 'none';
            console.log(`Draw button visibility set to ${show ? 'visible' : 'hidden'}`);
            return true;
        }
        return false;
    } catch (error) {
        console.error("Error toggling draw button visibility:", error);
        return false;
    }
}

// Export module-level variables and additional utility functions
export { isDrawingModeActive };

// Helper function for clearing the map
export function clearMap(map) {
    console.log("Clearing map layers");
    return clearMapLayers(map);
}