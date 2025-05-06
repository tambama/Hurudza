// Import Mapbox GL JS
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
// Import MapboxDraw for boundary drawing
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-draw/v1.3.0/mapbox-gl-draw.js';

// Mapbox access token - this should match the one used in Map.razor.js
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

// Global variables to maintain state
let drawButton = null;
let deleteButton = null;
let isDrawingModeActive = false;

// Initialize Mapbox
function initializeMapbox() {
    mapboxgl.accessToken = mapboxToken;
    console.log("Mapbox initialized with token");
    return mapboxgl;
}

// Initialize map on the provided element
export function initializeMap(element) {
    try {
        console.log("Initializing map for field creation");

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

// Initialize drawing controls
export function initializeDrawControls(map, dotNetRef) {
    try {
        console.log("Initializing draw controls");

        // Create Mapbox Draw instance
        const draw = new MapboxDraw({
            displayControlsDefault: false,
            controls: {
                polygon: true,
                trash: true
            },
            // Style options for drawing
            styles: [
                // Polygon fill
                {
                    id: 'gl-draw-polygon-fill',
                    type: 'fill',
                    filter: ['all', ['==', '$type', 'Polygon'], ['!=', 'mode', 'static']],
                    paint: {
                        'fill-color': '#ff9966',
                        'fill-outline-color': '#ff5500',
                        'fill-opacity': 0.3
                    }
                },
                // Polygon outline
                {
                    id: 'gl-draw-polygon-stroke-active',
                    type: 'line',
                    filter: ['all', ['==', '$type', 'Polygon'], ['!=', 'mode', 'static']],
                    layout: {
                        'line-cap': 'round',
                        'line-join': 'round'
                    },
                    paint: {
                        'line-color': '#ff5500',
                        'line-width': 2
                    }
                },
                // Polygon mid points
                {
                    id: 'gl-draw-polygon-midpoint',
                    type: 'circle',
                    filter: ['all', ['==', '$type', 'Point'], ['==', 'meta', 'midpoint']],
                    paint: {
                        'circle-radius': 4,
                        'circle-color': '#fbb03b'
                    }
                },
                // Polygon vertices
                {
                    id: 'gl-draw-polygon-vertex-active',
                    type: 'circle',
                    filter: ['all', ['==', 'meta', 'vertex'], ['==', '$type', 'Point']],
                    paint: {
                        'circle-radius': 6,
                        'circle-color': '#fbb03b'
                    }
                }
            ]
        });

        // Add draw controls to map
        map.addControl(draw);

        // Create custom drawing and delete buttons
        createCustomControls(map, draw, dotNetRef);

        console.log("Draw controls initialized");
        return draw;
    } catch (error) {
        console.error("Error initializing draw controls:", error);
        throw error;
    }
}

// Create custom drawing and delete buttons
function createCustomControls(map, draw, dotNetRef) {
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
            toggleDrawingMode(map, draw, dotNetRef);
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
            map.getCanvas().style.cursor = '';

            // Notify .NET
            dotNetRef.invokeMethodAsync('OnDrawingDeleted');
        });

        console.log("Custom controls created");
        return { drawButton, deleteButton };
    } catch (error) {
        console.error("Error creating custom controls:", error);
        return null;
    }
}

// Toggle drawing mode
function toggleDrawingMode(map, draw, dotNetRef) {
    try {
        // Toggle state
        isDrawingModeActive = !isDrawingModeActive;

        // Update button appearance
        updateDrawButtonState();

        if (isDrawingModeActive) {
            // When activating drawing mode:

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
            map.getCanvas().style.cursor = '';

            // 2. Switch to selection mode
            draw.changeMode('simple_select');

            console.log("Drawing mode deactivated");
        }

        // Notify .NET of drawing mode change
        dotNetRef.invokeMethodAsync('SetDrawingMode', isDrawingModeActive).catch(error => {
            console.warn("Error notifying .NET of drawing mode change:", error);
        });

        return true;
    } catch (error) {
        console.error("Error toggling drawing mode:", error);
        return false;
    }
}

// Update draw button appearance based on active state
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

// Setup event handlers for drawing
export async function setupDrawingEvents(map, draw, dotNetRef) {
    try {
        console.log("Setting up drawing event handlers");

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
            map.getCanvas().style.cursor = '';

            // Get the new polygon from draw
            const data = draw.getAll();
            if (data.features.length > 0) {
                // Always use the last created polygon
                const polygon = data.features[data.features.length - 1];

                // Calculate the area using Turf.js
                if (polygon.geometry.type === 'Polygon') {
                    // Calculate area in square meters, convert to hectares
                    const area = turf.area(polygon);
                    const areaInHectares = area / 10000; // 1 hectare = 10,000 sqm

                    console.log(`Calculated area: ${areaInHectares.toFixed(2)} hectares`);

                    // Update field size in the form
                    dotNetRef.invokeMethodAsync('UpdateFieldSize', areaInHectares);

                    // Extract coordinates 
                    const coordinates = polygon.geometry.coordinates[0].map(coord => {
                        return [coord[0], coord[1]]; // longitude, latitude
                    });

                    // Remove the last coordinate (which is the same as the first for closed polygons)
                    coordinates.pop();

                    // Send coordinates to .NET
                    dotNetRef.invokeMethodAsync('OnPolygonDrawn', coordinates);
                }
            }
        });

        // Handle update events
        map.on('draw.update', function(e) {
            // Get the updated polygon from draw
            const data = draw.getAll();
            if (data.features.length > 0) {
                // Always use the last created polygon
                const polygon = data.features[data.features.length - 1];

                // Calculate the area using Turf.js
                if (polygon.geometry.type === 'Polygon') {
                    // Calculate area in square meters, convert to hectares
                    const area = turf.area(polygon);
                    const areaInHectares = area / 10000; // 1 hectare = 10,000 sqm

                    console.log(`Updated area: ${areaInHectares.toFixed(2)} hectares`);

                    // Update field size in the form
                    dotNetRef.invokeMethodAsync('UpdateFieldSize', areaInHectares);

                    // Extract coordinates 
                    const coordinates = polygon.geometry.coordinates[0].map(coord => {
                        return [coord[0], coord[1]]; // longitude, latitude
                    });

                    // Remove the last coordinate (which is the same as the first for closed polygons)
                    coordinates.pop();

                    // Send coordinates to .NET
                    dotNetRef.invokeMethodAsync('OnPolygonDrawn', coordinates);
                }
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
            dotNetRef.invokeMethodAsync('OnDrawingDeleted');
        });

        console.log("Drawing event handlers set up successfully");
    } catch (error) {
        console.error("Error setting up drawing events:", error);
    }
}

// Highlight the draw tool to guide users
export function highlightDrawTool() {
    if (!drawButton) return false;

    // Apply pulse animation class
    drawButton.classList.add('pulse-animation');

    // Remove animation after 5 seconds
    setTimeout(() => {
        drawButton.classList.remove('pulse-animation');
    }, 5000);

    return true;
}

/**
 * Clears all custom sources and layers from the map
 * @param {Object} map - The Mapbox map instance
 * @returns {boolean} Success indicator
 */
export function clearMap(map) {
    try {
        console.log('Clearing map...');

        // Store the current camera position
        const currentCenter = map.getCenter();
        const currentZoom = map.getZoom();

        // Get all map style layers
        const style = map.getStyle();
        if (!style || !style.layers || !style.sources) {
            console.warn('Map style not fully loaded, cannot reset layers');
            return false;
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

        console.log('Map cleared successfully');
        return true;
    } catch (error) {
        console.error('Error clearing map:', error);
        return false;
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

// Center map on coordinates with specific zoom level
export function centerMap(map, latitude, longitude, zoom) {
    try {
        console.log(`Centering map on [${longitude}, ${latitude}] with zoom ${zoom}`);

        map.flyTo({
            center: [longitude, latitude],
            zoom: zoom,
            essential: true,
            duration: 1500 // 1.5 seconds animation
        });

        return true;
    } catch (error) {
        console.error("Error centering map:", error);
        return false;
    }
}

/**
 * Enhanced function to draw farm boundary with improved styling and interaction
 * @param {Object} map - The Mapbox map instance
 * @param {string} farmId - ID of the farm
 * @param {Array} polygons - Array of coordinate arrays for the farm boundary
 * @param {string} farmName - Name of the farm
 * @returns {boolean} Success indicator
 */
export function drawFarmBoundary(map, farmId, polygons, farmName) {
    try {
        console.log(`Drawing farm boundary for ${farmName}`);

        // Validate polygon data
        if (!polygons || !Array.isArray(polygons) || polygons.length === 0 ||
            !polygons[0] || !Array.isArray(polygons[0]) || polygons[0].length < 3) {
            console.warn("Invalid farm polygon data", polygons);
            return false;
        }

        // Check if source already exists
        if (map.getSource(`farm-${farmId}`)) {
            // Remove any existing layers first
            if (map.getLayer(`farm-fill-${farmId}`)) {
                map.removeLayer(`farm-fill-${farmId}`);
            }
            if (map.getLayer(`farm-outline-${farmId}`)) {
                map.removeLayer(`farm-outline-${farmId}`);
            }
            if (map.getLayer(`farm-hover-${farmId}`)) {
                map.removeLayer(`farm-hover-${farmId}`);
            }
            // Then remove the source
            map.removeSource(`farm-${farmId}`);
        }

        // Add source for the farm boundary
        map.addSource(`farm-${farmId}`, {
            'type': 'geojson',
            'data': {
                'type': 'Feature',
                'properties': {
                    'name': farmName,
                    'id': farmId
                },
                'geometry': {
                    'type': 'Polygon',
                    'coordinates': polygons[0] // First polygon in the array
                }
            }
        });

        // Add fill layer with improved styling
        map.addLayer({
            'id': `farm-fill-${farmId}`,
            'type': 'fill',
            'source': `farm-${farmId}`,
            'layout': {},
            'paint': {
                'fill-color': '#40b7d5',
                'fill-opacity': 0.2,
                'fill-outline-color': '#40b7d5'
            }
        });

        // Add hover effect layer (initially transparent)
        map.addLayer({
            'id': `farm-hover-${farmId}`,
            'type': 'fill',
            'source': `farm-${farmId}`,
            'layout': {},
            'paint': {
                'fill-color': '#40b7d5',
                'fill-opacity': 0 // Initially transparent
            }
        });

        // Add outline layer with improved styling
        map.addLayer({
            'id': `farm-outline-${farmId}`,
            'type': 'line',
            'source': `farm-${farmId}`,
            'layout': {},
            'paint': {
                'line-color': '#40b7d5',
                'line-width': 3,
                'line-opacity': 0.8,
                'line-dasharray': [0.5, 1] // Create a dashed line effect
            }
        });

        // Add click handler for popup with improved content
        map.on('click', `farm-fill-${farmId}`, (e) => {
            // Skip if in drawing mode
            if (isDrawingModeActive) return;

            // Get coordinates at click point
            const coordinates = e.lngLat;

            // Create popup with improved styling
            new mapboxgl.Popup({
                closeButton: true,
                closeOnClick: true,
                maxWidth: '300px',
                className: 'farm-popup' // For custom CSS styling
            })
                .setLngLat(coordinates)
                .setHTML(`
                <div style="max-width: 280px; word-wrap: break-word;">
                    <h5 style="margin-bottom: 10px; font-weight: 600; color: #40b7d5;">${farmName}</h5>
                    <div style="background-color: #f8f9fa; padding: 10px; border-radius: 5px; margin-bottom: 10px;">
                        <p class="mb-1" style="font-size: 13px;"><i class="fas fa-info-circle me-2" style="color: #40b7d5;"></i>Farm boundary shown in blue</p>
                        <p class="mb-0" style="font-size: 13px;"><i class="fas fa-draw-polygon me-2" style="color: #ff5500;"></i>Click the draw tool to add a field boundary</p>
                    </div>
                </div>
            `)
                .addTo(map);
        });

        // Add hover effects
        map.on('mouseenter', `farm-fill-${farmId}`, () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = 'pointer';
                map.setPaintProperty(`farm-hover-${farmId}`, 'fill-opacity', 0.4);
                map.setPaintProperty(`farm-outline-${farmId}`, 'line-width', 4);
            }
        });

        map.on('mouseleave', `farm-fill-${farmId}`, () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = '';
                map.setPaintProperty(`farm-hover-${farmId}`, 'fill-opacity', 0);
                map.setPaintProperty(`farm-outline-${farmId}`, 'line-width', 3);
            }
        });

        console.log("Farm boundary drawn successfully");
        return true;
    } catch (error) {
        console.error("Error drawing farm boundary:", error);
        return false;
    }
}

/**
 * Fits the map view to the provided bounds with padding
 * @param {Object} map - The Mapbox map instance
 * @param {number} minLat - Minimum latitude of bounds
 * @param {number} minLng - Minimum longitude of bounds
 * @param {number} maxLat - Maximum latitude of bounds
 * @param {number} maxLng - Maximum longitude of bounds
 * @returns {boolean} Success indicator
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

