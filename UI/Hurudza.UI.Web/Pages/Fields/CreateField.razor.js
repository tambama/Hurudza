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

// Clear the map of all sources and layers
export function clearMap(map) {
    try {
        console.log("Clearing map");

        // Store current camera position
        const center = map.getCenter();
        const zoom = map.getZoom();

        // Get all sources from the map
        const style = map.getStyle();
        if (!style || !style.sources) {
            console.warn("Map style not loaded yet");
            return false;
        }

        // Find custom sources (non-mapbox sources)
        const customSources = Object.keys(style.sources).filter(source =>
            !source.startsWith('mapbox.') && source !== 'composite');

        // Find custom layers
        const customLayers = style.layers.filter(layer =>
            (layer.source && customSources.includes(layer.source)) ||
            layer.id.startsWith('farm-') ||
            layer.id.startsWith('field-'));

        // Remove custom layers first
        customLayers.forEach(layer => {
            if (map.getLayer(layer.id)) {
                map.removeLayer(layer.id);
            }
        });

        // Then remove custom sources
        customSources.forEach(source => {
            if (map.getSource(source)) {
                map.removeSource(source);
            }
        });

        // Restore camera position
        map.jumpTo({
            center: center,
            zoom: zoom
        });

        console.log("Map cleared successfully");
        return true;
    } catch (error) {
        console.error("Error clearing map:", error);
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

// Draw farm boundary on the map
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

        // Add fill layer
        map.addLayer({
            'id': `farm-fill-${farmId}`,
            'type': 'fill',
            'source': `farm-${farmId}`,
            'layout': {},
            'paint': {
                'fill-color': '#40b7d5',
                'fill-opacity': 0.2
            }
        });

        // Add outline layer
        map.addLayer({
            'id': `farm-outline-${farmId}`,
            'type': 'line',
            'source': `farm-${farmId}`,
            'layout': {},
            'paint': {
                'line-color': '#40b7d5',
                'line-width': 3
            }
        });

        // Add click handler for popup
        map.on('click', `farm-fill-${farmId}`, (e) => {
            // Skip if in drawing mode
            if (isDrawingModeActive) return;

            // Create popup at click location
            new mapboxgl.Popup({
                closeButton: true,
                closeOnClick: true
            })
                .setLngLat(e.lngLat)
                .setHTML(`
                <div style="max-width: 250px; word-wrap: break-word;">
                    <h5 style="margin-bottom: 5px; font-weight: bold;">${farmName}</h5>
                    <p class="small mb-0">Click the draw tool to add a field boundary</p>
                </div>
            `)
                .addTo(map);
        });

        // Add hover effect
        map.on('mouseenter', `farm-fill-${farmId}`, () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = 'pointer';
            }
        });

        map.on('mouseleave', `farm-fill-${farmId}`, () => {
            if (!isDrawingModeActive) {
                map.getCanvas().style.cursor = '';
            }
        });

        console.log("Farm boundary drawn successfully");
        return true;
    } catch (error) {
        console.error("Error drawing farm boundary:", error);
        return false;
    }
}