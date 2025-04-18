import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-draw/v1.3.0/mapbox-gl-draw.js';

// Mapbox access token
mapboxgl.accessToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

// Track drawing mode globally
let isDrawingModeActive = false;
let drawControl = null;

/**
 * Creates and returns a new Mapbox map instance
 * @param {HTMLElement} element - The DOM element to attach the map to
 * @returns {Object} The map instance
 */
export function addMapToElement(element) {
    try {
        // Create and return a new map instance
        const map = new mapboxgl.Map({
            container: element,
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: [31.053028, -17.824858], // Default center (Zimbabwe)
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

        // Wait for map to load before returning
        map.on('load', () => {
            console.log('Map loaded successfully');
        });

        return map;
    } catch (error) {
        console.error('Error initializing map:', error);
        throw error;
    }
}

// Create a custom filter control class
class FilterControl {
    constructor(dotNetRef) {
        this._dotNetRef = dotNetRef;
    }

    onAdd(map) {
        this._map = map;
        this._container = document.createElement('div');
        this._container.className = 'mapboxgl-ctrl mapboxgl-ctrl-group filter-control-container';

        const button = document.createElement('button');
        button.className = 'mapboxgl-ctrl-icon custom-filter-ctrl';
        button.type = 'button';
        button.title = 'Filter Schools';

        // Add filter icon (using Font Awesome styling)
        button.innerHTML = '<i class="fa fa-filter"></i>';

        // Add click handler
        button.onclick = () => {
            // Call the .NET method on the Blazor component
            if (this._dotNetRef) {
                this._dotNetRef.invokeMethodAsync('ToggleFilterModal');
            }
        };

        this._container.appendChild(button);
        return this._container;
    }

    onRemove() {
        this._container.parentNode.removeChild(this._container);
        this._map = undefined;
    }
}

/**
 * Add a filter button to the map
 * @param {Object} map - The Mapbox map instance
 * @param {Object} dotNetRef - Reference to .NET component
 * @returns {boolean} Success indicator
 */
export function addFilterButton(map, dotNetRef) {
    try {
        // Create and add filter control
        const filterControl = new FilterControl(dotNetRef);
        map.addControl(filterControl, 'top-right');

        // Add fullscreen control after filter control
        map.addControl(new mapboxgl.FullscreenControl(), 'top-right');

        // Add CSS for the filter button
        addFilterButtonStyles();

        console.log('Filter button added to map');
        return true;
    } catch (error) {
        console.error('Error adding filter button:', error);
        return false;
    }
}

/**
 * Add custom CSS styles for the filter button
 */
export function addFilterButtonStyles() {
    try {
        // Create style element if it doesn't exist
        let style = document.getElementById('filter-button-styles');
        if (!style) {
            style = document.createElement('style');
            style.id = 'filter-button-styles';
            style.textContent = `
                .custom-filter-ctrl {
                    width: 30px;
                    height: 30px;
                    cursor: pointer;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background: white;
                }
                .custom-filter-ctrl:hover {
                    background-color: #f2f2f2;
                }
                .custom-filter-ctrl i {
                    font-size: 14px;
                }
                .filter-control-container {
                    margin-right: 5px;
                }
            `;
            document.head.appendChild(style);
            console.log('Filter button styles added');
        }
        return true;
    } catch (error) {
        console.error('Error adding filter button styles:', error);
        return false;
    }
}

// Initialize map draw controls
export function initializeDrawControls(map) {
    try {
        // Check if MapboxDraw is available
        if (typeof MapboxDraw === 'undefined') {
            console.error('MapboxDraw plugin not available');
            return null;
        }

        // Add event handler to intercept events while in drawing mode
        map.on('click', function(e) {
            // If we're in drawing mode (detected by presence of active class on button)
            if (isDrawingModeActive) {
                // Prevent propagation of the click event to other handlers
                e.originalEvent.stopPropagation();

                // Ensure we don't get popup on click while drawing
                const popups = document.querySelectorAll('.mapboxgl-popup');
                if (popups.length > 0) {
                    popups.forEach(popup => popup.remove());
                }
            }
        }, true); // Use capturing phase to intercept before other handlers

        // Create a new MapboxDraw instance with polygon-only drawing tools
        drawControl = new MapboxDraw({
            displayControlsDefault: false,
            controls: {
                polygon: true,
                trash: true
            },
            defaultMode: 'simple_select', // Start in selection mode, not drawing mode
            styles: [
                // Style for the polygon fill
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
                // Style for the polygon outline
                {
                    'id': 'gl-draw-polygon-stroke',
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
                // Style for the vertices
                {
                    'id': 'gl-draw-polygon-and-line-vertex-active',
                    'type': 'circle',
                    'filter': ['all', ['==', 'meta', 'vertex'], ['==', '$type', 'Point']],
                    'paint': {
                        'circle-radius': 6,
                        'circle-color': '#ff5500'
                    }
                }
            ]
        });

        // Add draw controls to the map but hide the default UI - we'll use our custom button
        // We still need to add it to have the functionality, but we don't need to show the default buttons
        map.addControl(drawControl);

        // Hide the default MapboxDraw controls
        let style = document.getElementById('draw-controls-style');
        if (!style) {
            style = document.createElement('style');
            style.id = 'draw-controls-style';
            style.textContent = `
                /* Hide the default MapboxDraw controls */
                .mapbox-gl-draw_ctrl-draw-btn {
                    display: none !important;
                }
                .mapboxgl-ctrl-group.mapboxgl-ctrl-group--draw {
                    display: none !important;
                }
            `;
            document.head.appendChild(style);
        }

        console.log('Draw controls initialized successfully');
        return drawControl;
    } catch (error) {
        console.error('Error initializing draw controls:', error);
        return null;
    }
}

// Setup drawing mode with event blocking
export function setupDrawingMode(map, draw, dotNetRef) {
    try {
        // Create a new button element for drawing mode toggle
        const drawButton = document.createElement('button');
        drawButton.id = 'field-drawing-button';
        drawButton.className = 'mapboxgl-ctrl-icon';
        drawButton.type = 'button';
        drawButton.title = 'Draw Field Boundary';
        drawButton.innerHTML = '<i class="fa fa-draw-polygon"></i>';
        drawButton.style.cssText = `
            width: 30px;
            height: 30px;
            cursor: pointer;
            background: white;
            border: none;
            border-radius: 4px;
            display: flex;
            align-items: center;
            justify-content: center;
            box-shadow: 0 0 0 2px rgba(0,0,0,0.1);
            position: absolute;
            top: 130px; /* Position below fullscreen control */
            right: 10px;
            z-index: 999;
            display: none; /* Initially hidden */
        `;

        // Add button to map container
        const mapContainer = map.getContainer();
        mapContainer.appendChild(drawButton);

        // Add hover and active styles
        drawButton.addEventListener('mouseenter', () => {
            if (!isDrawingModeActive) {
                drawButton.style.background = '#f2f2f2';
            }
        });

        drawButton.addEventListener('mouseleave', () => {
            if (!isDrawingModeActive) {
                drawButton.style.background = 'white';
            }
        });

        // Add click handler to toggle drawing mode
        drawButton.addEventListener('click', () => {
            toggleDrawingMode(map, draw, dotNetRef, drawButton);
        });

        // Add event listener to block popup creation during drawing
        map.on('click', (e) => {
            if (isDrawingModeActive) {
                // For Mapbox events, we use originalEvent to access the DOM event
                if (e.originalEvent) {
                    e.originalEvent.preventDefault();
                    e.originalEvent.stopPropagation();
                }

                // Set a flag to tell other handlers to ignore this click
                window._ignoreMapClicks = true;

                // Schedule removal of the flag
                setTimeout(() => {
                    window._ignoreMapClicks = false;
                }, 100);

                // Remove any popups that might have appeared
                removeAllPopups();
            }
        });

        // Listen for draw.create event to capture when a polygon is completed
        map.on('draw.create', (e) => {
            console.log('Polygon creation completed');

            // Check if we have a valid polygon
            if (e.features && e.features.length > 0) {
                // Calculate the area of the polygon
                const polygon = e.features[0];
                if (polygon && polygon.geometry && polygon.geometry.coordinates &&
                    polygon.geometry.coordinates.length > 0 && polygon.geometry.coordinates[0].length > 2) {

                    const coordinates = polygon.geometry.coordinates[0];

                    // Calculate area using Turf.js
                    calculatePolygonAreaInHectares(coordinates)
                        .then(areaInHectares => {
                            // Round to 2 decimal places
                            const roundedArea = Math.round(areaInHectares * 100) / 100;

                            // Send the calculated area to .NET
                            if (dotNetRef) {
                                try {
                                    dotNetRef.invokeMethodAsync('UpdateFieldSize', roundedArea);
                                    console.log('Sent calculated area to .NET:', roundedArea, 'hectares');
                                } catch (error) {
                                    console.error('Error sending area to .NET:', error);
                                }
                            }
                        })
                        .catch(error => {
                            console.error('Error calculating area with Turf.js:', error);
                        });
                }

                // Exit drawing mode
                toggleDrawingMode(map, draw, dotNetRef, drawButton, true);
            }
        });

        // Listen for escape key to cancel drawing
        document.addEventListener('keydown', (e) => {
            if (e.key === 'Escape' && isDrawingModeActive) {
                // Cancel drawing mode
                toggleDrawingMode(map, draw, dotNetRef, drawButton, false);
            }
        });

        console.log('Drawing mode setup complete');

        // Return the button so it can be referenced later
        return { draw, drawButton };
    } catch (error) {
        console.error('Error setting up drawing mode:', error);
        return null;
    }
}

// Helper function to toggle drawing mode
function toggleDrawingMode(map, draw, dotNetRef, drawButton, hasCompletedPolygon = false) {
    // If we completed a polygon, we're turning off drawing mode
    if (hasCompletedPolygon) {
        isDrawingModeActive = false;
    } else {
        // Otherwise toggle the state
        isDrawingModeActive = !isDrawingModeActive;
    }

    // Update button UI
    updateDrawButtonState(drawButton);

    // Notify .NET about the state change
    if (dotNetRef) {
        try {
            dotNetRef.invokeMethodAsync('SetDrawingMode', isDrawingModeActive);
        } catch (error) {
            console.warn('Error notifying .NET of drawing mode change:', error);
        }
    }

    if (isDrawingModeActive) {
        // Activate draw mode
        removeAllPopups();

        // Enable drawing mode
        if (draw) {
            try {
                draw.changeMode('draw_polygon');
            } catch (error) {
                console.warn('Error changing to draw_polygon mode:', error);
            }
        }

        // Set drawing cursor
        setCrosshairCursor(map);
    } else {
        // Get coordinates if we have a completed polygon
        if (hasCompletedPolygon && draw) {
            try {
                const coordinates = getDrawnPolygonCoordinates(draw);
                if (coordinates && dotNetRef) {
                    console.log('Sending coordinates to .NET:', coordinates);
                    dotNetRef.invokeMethodAsync('OnPolygonDrawn', coordinates);
                }
            } catch (error) {
                console.warn('Error getting polygon coordinates:', error);
            }
        }

        // Reset cursor
        resetCursor(map);

        // Don't clear drawing if we just completed a polygon - the user might want to see what they drew
        if (!hasCompletedPolygon && draw) {
            try {
                draw.deleteAll();
            } catch (error) {
                console.warn('Error clearing drawings:', error);
            }
        }
    }
}

/**
 * Calculates the area of a polygon in hectares using Turf.js
 * @param {Array} coordinates - Array of [longitude, latitude] coordinates
 * @returns {number} Area in hectares
 */
export function calculatePolygonAreaInHectares(coordinates) {
    try {
        if (!coordinates || coordinates.length < 3) {
            console.warn('Not enough coordinates to calculate area');
            return 0;
        }

        // Dynamically load Turf.js from CDN if not already loaded
        return loadTurf().then(turf => {
            // Create a polygon from the coordinates
            const polygon = turf.polygon([[...coordinates, coordinates[0]]]);

            // Calculate area in square meters
            const areaInSquareMeters = turf.area(polygon);

            // Convert to hectares (1 hectare = 10,000 square meters)
            const hectares = areaInSquareMeters / 10000;

            console.log(`Turf.js calculated area: ${hectares.toFixed(2)} hectares`);
            return hectares;
        }).catch(error => {
            console.error('Error using Turf.js, falling back to approximate calculation:', error);

            // Fallback to a simple calculation if Turf.js fails
            // This is a very rough approximation for small areas
            const latLngPoints = coordinates.map(coord => ({ lng: coord[0], lat: coord[1] }));
            const approximateArea = calculateApproximateArea(latLngPoints);
            console.log(`Fallback area calculation: ${approximateArea.toFixed(2)} hectares`);
            return approximateArea;
        });
    } catch (error) {
        console.error('Error calculating polygon area:', error);
        return Promise.resolve(0);
    }
}

/**
 * Load Turf.js from CDN
 * @returns {Promise} Promise that resolves with the turf object
 */
function loadTurf() {
    return new Promise((resolve, reject) => {
        // Check if Turf is already loaded
        if (window.turf) {
            resolve(window.turf);
            return;
        }

        // Create script element
        const script = document.createElement('script');
        script.src = 'https://cdn.jsdelivr.net/npm/@turf/turf@6/turf.min.js';
        script.integrity = 'sha512-Wm2tQKurvPeBdx5ZPQjTvdGQS0OZNnSbzLNL1l35y5td2Xu8wt+a+/a4EoTFAcchZoVLqI8bhkYYTiPwP5xXw==';
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
 * Fallback function to calculate approximate area in hectares
 * @param {Array} latLngPoints - Array of {lat, lng} points
 * @returns {number} Approximate area in hectares
 */
function calculateApproximateArea(latLngPoints) {
    // Simple shoelace formula implementation
    // Convert lat/lng to x/y using rough approximation
    // This is not accurate for large areas or areas far from the equator

    if (latLngPoints.length < 3) return 0;

    // Average latitude to calculate conversion factors
    const avgLat = latLngPoints.reduce((sum, point) => sum + point.lat, 0) / latLngPoints.length;
    const latRad = avgLat * Math.PI / 180;

    // Conversion factors - approximate
    const metersPerDegLat = 111320; // meters per degree latitude
    const metersPerDegLng = 111320 * Math.cos(latRad); // meters per degree longitude at this latitude

    // Convert lat/lng to x/y meters
    const xyPoints = latLngPoints.map(point => ({
        x: point.lng * metersPerDegLng,
        y: point.lat * metersPerDegLat
    }));

    // Apply shoelace formula
    let area = 0;
    for (let i = 0; i < xyPoints.length; i++) {
        const j = (i + 1) % xyPoints.length;
        area += xyPoints[i].x * xyPoints[j].y;
        area -= xyPoints[j].x * xyPoints[i].y;
    }

    // Take absolute value and divide by 2
    area = Math.abs(area) / 2;

    // Convert square meters to hectares
    return area / 10000;
}

export function toggleDrawButtonVisibility(visible) {
    try {
        const drawButton = document.getElementById('field-drawing-button');
        if (drawButton) {
            // Set visibility
            drawButton.style.display = visible ? 'flex' : 'none';

            // Add or remove pulsing animation class
            if (visible) {
                drawButton.classList.add('pulsing');

                // Pulse for a few seconds to draw attention, then stop
                setTimeout(() => {
                    drawButton.classList.remove('pulsing');
                }, 6000);
            } else {
                drawButton.classList.remove('pulsing');
            }

            console.log(`Draw button visibility set to: ${visible ? 'visible' : 'hidden'}`);
        } else {
            console.warn('Draw button not found');
        }
        return true;
    } catch (error) {
        console.error('Error toggling draw button visibility:', error);
        return false;
    }
}

// Helper to update the draw button state
function updateDrawButtonState(drawButton) {
    if (!drawButton) return;

    if (isDrawingModeActive) {
        drawButton.style.background = '#ff5500';
        drawButton.style.color = 'white';
    } else {
        drawButton.style.background = 'white';
        drawButton.style.color = 'black';
    }
}

// Remove all mapbox popups
function removeAllPopups() {
    setTimeout(() => {
        const popups = document.querySelectorAll('.mapboxgl-popup');
        popups.forEach(popup => popup.remove());
    }, 0);
}

// Set crosshair cursor for drawing
function setCrosshairCursor(map) {
    if (!map) return;

    try {
        // Store original cursor
        window._originalMapCursor = map.getCanvas().style.cursor;

        // Set drawing cursor
        map.getCanvas().style.cursor = 'crosshair';
    } catch (error) {
        console.warn('Error setting cursor:', error);
    }
}

// Reset cursor to original
function resetCursor(map) {
    if (!map) return;

    try {
        // Restore original cursor
        if (window._originalMapCursor) {
            map.getCanvas().style.cursor = window._originalMapCursor;
        } else {
            map.getCanvas().style.cursor = '';
        }
    } catch (error) {
        console.warn('Error resetting cursor:', error);
    }
}

// Get polygon coordinates - with added safety checks
function getDrawnPolygonCoordinates(draw) {
    try {
        if (!draw) {
            console.warn('Draw control is null');
            return null;
        }

        // Get all features safely
        let features = [];
        try {
            const allFeatures = draw.getAll();
            if (allFeatures && allFeatures.features) {
                features = allFeatures.features;
            }
        } catch (error) {
            console.warn('Error getting features:', error);
            return null;
        }

        if (features.length === 0) {
            console.log('No polygons have been drawn');
            return null;
        }

        // Just get the first polygon if multiple exist
        const polygon = features.find(feature =>
            feature && feature.geometry && feature.geometry.type === 'Polygon');

        if (!polygon) {
            console.log('No polygon features found');
            return null;
        }

        // Check if we have coordinates and they're in the expected structure
        if (polygon.geometry.coordinates &&
            Array.isArray(polygon.geometry.coordinates) &&
            polygon.geometry.coordinates.length > 0 &&
            Array.isArray(polygon.geometry.coordinates[0])) {

            // Return the first (and usually only) ring of coordinates
            return polygon.geometry.coordinates[0];
        }

        console.warn('Unexpected coordinates structure:', polygon.geometry.coordinates);
        return null;
    } catch (error) {
        console.error('Error getting polygon coordinates:', error);
        return null;
    }
}

// Clear any drawn polygons with safety checks
export function clearDrawnPolygons(draw) {
    try {
        if (draw) {
            try {
                draw.deleteAll();
                console.log('All drawn polygons cleared');
            } catch (error) {
                console.warn('Error clearing polygons:', error);
            }
            return true;
        }
        return false;
    } catch (error) {
        console.error('Error in clearDrawnPolygons:', error);
        return false;
    }
}

/**
 * Draws a polygon on the map (for farm boundaries or fields)
 * @param {Object} map - The Mapbox map instance
 * @param {string} id - Unique identifier for the polygon
 * @param {Array} coordinates - Polygon coordinates [[lng, lat], [lng, lat], ...]
 * @param {boolean} isField - Whether this polygon represents a field (vs farm)
 * @param {string} name - Name to display in popup
 * @param {Object} cropData - Optional crop data for field polygons
 * @param {boolean} clearExisting - Whether to clear existing layers before drawing
 * @returns {Promise<boolean>} Success indicator
 */
export async function drawPolygon(map, id, coordinates, isField = false, name = 'Farm', cropData = null, clearExisting = false) {
    // Define source and layer IDs with unique prefixes to avoid conflicts
    const prefix = isField ? 'field-' : 'farm-';
    const sourceId = `${prefix}source-${id}`;
    const outlineLayerId = `${prefix}outline-${id}`;
    const fillLayerId = `${prefix}fill-${id}`;
    const hoverFillLayerId = `${prefix}hover-fill-${id}`;

    try {
        // If clearExisting flag is true, clear all layers before drawing
        if (clearExisting) {
            await resetMapLayers(map);
        }

        // Check if this source already exists and remove it to avoid duplicates
        if (sourceExists(map, sourceId)) {
            // Remove associated layers first
            if (layerExists(map, outlineLayerId)) {
                removeLayerSafely(map, outlineLayerId);
            }

            if (layerExists(map, fillLayerId)) {
                removeLayerSafely(map, fillLayerId);
            }

            if (layerExists(map, hoverFillLayerId)) {
                removeLayerSafely(map, hoverFillLayerId);
            }

            // Then remove the source
            removeSourceSafely(map, sourceId);
        }

        // Add the source for this polygon
        map.addSource(sourceId, {
            'type': 'geojson',
            'data': {
                'type': 'Feature',
                'geometry': {
                    'type': 'Polygon',
                    'coordinates': coordinates
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

        // Add outline layer (always visible)
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

        // Add invisible fill layer (for hit detection)
        map.addLayer({
            'id': fillLayerId,
            'type': 'fill',
            'source': sourceId,
            'layout': {},
            'paint': {
                'fill-color': fillColor,
                'fill-opacity': 0 // Initially transparent
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
            const isField = properties.isField;
            const name = properties.name;
            const cropData = properties.cropData;

            // Build popup content
            let popupContent = `<div class="popup-content" style="max-width: 250px; word-wrap: break-word;">
                                    <h4 style="margin-bottom: 5px;">${name}</h4>`;

            if (isField) {
                if (cropData && Object.keys(cropData).length > 0) {
                    // Add crop information to popup if available
                    const plantedDate = cropData.plantedDate ?
                        new Date(cropData.plantedDate).toLocaleDateString() : 'Not set';
                    const harvestDate = cropData.harvestDate ?
                        new Date(cropData.harvestDate).toLocaleDateString() : 'Not set';

                    popupContent += `
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Current Crop:</strong> ${cropData.crop}</p>
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Area Planted:</strong> ${cropData.size} ha</p>
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Planted:</strong> ${plantedDate}</p>
                        <p style="margin-bottom: 5px; font-size: 12px;"><strong>Expected Harvest:</strong> ${harvestDate}</p>
                        <p style="margin-bottom: 0; font-size: 12px;"><strong>Irrigation:</strong> ${cropData.irrigation ? 'Yes' : 'No'}</p>
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
 * Loads farm data as clustered points on the map
 * @param {Object} map - The Mapbox map instance
 * @param {Object} farms - GeoJSON FeatureCollection of farm data
 */
export function loadFarms(map, farms) {
    try {
        // Add debug to see if function is being called with data
        console.log("loadFarms called with data:", farms);

        if (!map) {
            console.error("Map instance is not initialized");
            return;
        }

        if (!farms || !farms.features || farms.features.length === 0) {
            console.warn("No farm data provided for clusters");
            return;
        }

        // Clean up existing sources and layers
        if (sourceExists(map, 'farms')) {
            // Remove layers first
            if (layerExists(map, 'clusters'))
                removeLayerSafely(map, 'clusters');

            if (layerExists(map, 'cluster-count'))
                removeLayerSafely(map, 'cluster-count');

            if (layerExists(map, 'unclustered-point'))
                removeLayerSafely(map, 'unclustered-point');

            // Then remove source
            removeSourceSafely(map, 'farms');
        }

        // Add new source with farm data
        map.addSource('farms', {
            type: 'geojson',
            data: farms,
            cluster: true,
            clusterMaxZoom: 14, // Max zoom level for clusters
            clusterRadius: 50,  // Cluster radius
            clusterProperties: {
                'count': ['+', ['get', 'count']]
            }
        });

        // Add layer for clusters
        map.addLayer({
            id: 'clusters',
            type: 'circle',
            source: 'farms',
            filter: ['has', 'point_count'],
            paint: {
                // Color circles by size using step expression
                'circle-color': [
                    'step',
                    ['get', 'point_count'],
                    '#51bbd6', // Small clusters
                    10,
                    '#f1f075', // Medium clusters
                    25,
                    '#f28cb1'  // Large clusters
                ],
                'circle-radius': [
                    'step',
                    ['get', 'point_count'],
                    20, // Radius for small clusters
                    10,
                    30, // Radius for medium clusters
                    25,
                    40  // Radius for large clusters
                ],
                'circle-stroke-width': 1,
                'circle-stroke-color': '#fff'
            }
        });

        // Add layer for cluster counts
        map.addLayer({
            id: 'cluster-count',
            type: 'symbol',
            source: 'farms',
            filter: ['has', 'point_count'],
            layout: {
                'text-field': '{point_count_abbreviated}',
                'text-font': ['DIN Offc Pro Medium', 'Arial Unicode MS Bold'],
                'text-size': 12
            },
            paint: {
                'text-color': '#ffffff'
            }
        });

        // Add layer for individual points
        map.addLayer({
            id: 'unclustered-point',
            type: 'circle',
            source: 'farms',
            filter: ['!', ['has', 'point_count']],
            paint: {
                'circle-color': '#11b4da',
                'circle-radius': 8,
                'circle-stroke-width': 1,
                'circle-stroke-color': '#fff'
            }
        });

        // Add click handler for clusters
        map.on('click', 'clusters', (e) => {
            // Skip if in drawing mode
            if (isDrawingModeActive) return;

            const features = map.queryRenderedFeatures(e.point, {
                layers: ['clusters']
            });

            const clusterId = features[0].properties.cluster_id;

            // Zoom in to expand cluster
            map.getSource('farms').getClusterExpansionZoom(
                clusterId,
                (err, zoom) => {
                    if (err) return;

                    map.easeTo({
                        center: features[0].geometry.coordinates,
                        zoom: zoom
                    });
                }
            );
        });

        // Add click handler for individual points
        map.on('click', 'unclustered-point', (e) => {
            // Skip if in drawing mode
            if (isDrawingModeActive) return;

            const coordinates = e.features[0].geometry.coordinates.slice();
            const props = e.features[0].properties;
            const name = props.name;
            const size = props.size || 'N/A';

            // Ensure popup appears over the copy being clicked
            while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
                coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
            }

            // Create and display popup
            new mapboxgl.Popup({
                closeButton: true,
                closeOnClick: true,
                maxWidth: '300px'
            })
                .setLngLat(coordinates)
                .setHTML(`
                    <div style="max-width: 250px; word-wrap: break-word;">
                        <h4 style="margin-bottom: 5px;">${name}</h4>
                        <p style="margin-bottom: 0; font-size: 12px;">Size: ${size} ha</p>
                    </div>
                `)
                .addTo(map);
        });

        // Add hover effects
        map.on('mouseenter', 'clusters', () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = 'pointer';
            }
        });

        map.on('mouseenter', 'unclustered-point', () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = 'pointer';
            }
        });

        map.on('mouseleave', 'clusters', () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = '';
            }
        });

        map.on('mouseleave', 'unclustered-point', () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = '';
            }
        });

        // IMPORTANT: Signal successful load with console message
        console.log('Farms loaded successfully');

        return true; // Return success value
    } catch (error) {
        console.error('Error loading farms:', error);
        return false; // Return failure value
    }
}

/**
 * Set the map's center and zoom level
 * @param {Object} map - The Mapbox map instance
 * @param {number} latitude - Latitude coordinate
 * @param {number} longitude - Longitude coordinate
 * @param {number} zoom - Optional zoom level (default: 14)
 */
export function setMapCenter(map, latitude, longitude, zoom = 14) {
    try {
        map.easeTo({
            center: [longitude, latitude],
            zoom: zoom,
            duration: 1000 // Smooth transition over 1 second
        });
        console.log(`Map centered at [${longitude}, ${latitude}], zoom: ${zoom}`);
    } catch (error) {
        console.error('Error setting map center:', error);
    }
}

/**
 * Change the map's visual style
 * @param {Object} map - The Mapbox map instance
 * @param {string} style - Style identifier
 */
export function setMapStyle(map, style) {
    try {
        const styleUrl = `mapbox://styles/mapbox/${style}`;
        map.setStyle(styleUrl);
        console.log(`Map style changed to: ${style}`);
    } catch (error) {
        console.error('Error setting map style:', error);
    }
}

export function clearMap(map) {
    try {
        console.log('Clearing map...');

        // Store the current camera position
        const currentCenter = map.getCenter();
        const currentZoom = map.getZoom();

        // Clear all custom layers and sources
        resetMapLayers(map)
            .then(() => {
                console.log('Map cleared successfully');
            });

        return true;
    } catch (error) {
        console.error('Error clearing map:', error);
        return false;
    }
}

/**
 * Reset all custom map layers and sources
 * @param {Object} map - The Mapbox map instance
 * @returns {Promise<boolean>} Success indicator
 */
export function resetMapLayers(map) {
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
                        if (layerExists(map, layer.id)) {
                            // Remove event handlers first
                            try {
                                map.off('click', layer.id);
                                map.off('mouseenter', layer.id);
                                map.off('mouseleave', layer.id);
                            } catch (e) {
                                console.warn(`Error removing handlers for layer ${layer.id}:`, e);
                            }

                            // Then remove the layer
                            removeLayerSafely(map, layer.id);
                        }
                    });

                    // Then remove custom sources
                    customSourceIds.forEach(sourceId => {
                        removeSourceSafely(map, sourceId);
                    });

                    // Trigger map repaint to apply changes
                    map.triggerRepaint();

                    console.log('All custom layers and sources removed');
                    resolve(true);
                } catch (e) {
                    console.error('Error in resetMapLayers:', e);
                    resolve(false);
                }
            }, 100); // Small delay to ensure map is ready
        } catch (e) {
            console.error('Fatal error in resetMapLayers:', e);
            resolve(false);
        }
    });
}

/**
 * Safely checks if a source exists on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} sourceId - Source identifier
 * @returns {boolean} Whether the source exists
 */
export function sourceExists(map, sourceId) {
    try {
        return !!map.getSource(sourceId);
    } catch (e) {
        console.warn(`Error checking if source ${sourceId} exists:`, e);
        return false;
    }
}

/**
 * Safely checks if a layer exists on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} layerId - Layer identifier
 * @returns {boolean} Whether the layer exists
 */
export function layerExists(map, layerId) {
    try {
        return !!map.getLayer(layerId);
    } catch (e) {
        console.warn(`Error checking if layer ${layerId} exists:`, e);
        return false;
    }
}

/**
 * Safely removes a source from the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} sourceId - Source identifier
 */
function removeSourceSafely(map, sourceId) {
    try {
        if (sourceExists(map, sourceId)) {
            map.removeSource(sourceId);
            console.log(`Removed source: ${sourceId}`);
        }
    } catch (e) {
        console.warn(`Error removing source ${sourceId}:`, e);
    }
}

/**
 * Safely removes a layer from the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} layerId - Layer identifier
 */
function removeLayerSafely(map, layerId) {
    try {
        if (layerExists(map, layerId)) {
            map.removeLayer(layerId);
            console.log(`Removed layer: ${layerId}`);
        }
    } catch (e) {
        console.warn(`Error removing layer ${layerId}:`, e);
    }
}

/**
 * Highlights a specific field on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} fieldId - ID of the field to highlight
 */
export function highlightFieldOnMap(map, fieldId) {
    try {
        // Skip highlighting if in drawing mode
        if (isDrawingModeActive) return;

        // The field layer IDs follow the pattern 'field-outline-{fieldId}' and 'field-hover-fill-{fieldId}'
        const outlineLayerId = `field-outline-${fieldId}`;
        const hoverFillLayerId = `field-hover-fill-${fieldId}`;

        // Check if the layers exist to avoid errors
        if (layerExists(map, outlineLayerId) && layerExists(map, hoverFillLayerId)) {
            // Change the outline color to red and make it thicker
            map.setPaintProperty(outlineLayerId, 'line-color', '#ff0000');
            map.setPaintProperty(outlineLayerId, 'line-width', 3);

            // Show the fill with opacity
            map.setPaintProperty(hoverFillLayerId, 'fill-opacity', 0.3);

            console.log(`Highlighted field ${fieldId}`);
        } else {
            console.warn(`Could not find layers for field ${fieldId}`);
        }
    } catch (error) {
        console.error(`Error highlighting field ${fieldId}:`, error);
    }
}

/**
 * Removes highlight from a specific field on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} fieldId - ID of the field to unhighlight
 */
export function unhighlightFieldOnMap(map, fieldId) {
    try {
        // Skip unhighlighting if in drawing mode
        if (isDrawingModeActive) return;

        // The field layer IDs follow the pattern 'field-outline-{fieldId}' and 'field-hover-fill-{fieldId}'
        const outlineLayerId = `field-outline-${fieldId}`;
        const hoverFillLayerId = `field-hover-fill-${fieldId}`;

        // Check if the layers exist to avoid errors
        if (layerExists(map, outlineLayerId) && layerExists(map, hoverFillLayerId)) {
            // Reset the outline color to orange and width back to normal
            map.setPaintProperty(outlineLayerId, 'line-color', '#ff5500');
            map.setPaintProperty(outlineLayerId, 'line-width', 2);

            // Hide the fill
            map.setPaintProperty(hoverFillLayerId, 'fill-opacity', 0);

            console.log(`Unhighlighted field ${fieldId}`);
        } else {
            console.warn(`Could not find layers for field ${fieldId}`);
        }
    } catch (error) {
        console.error(`Error unhighlighting field ${fieldId}:`, error);
    }
}