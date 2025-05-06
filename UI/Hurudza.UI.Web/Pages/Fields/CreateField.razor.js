// Import Mapbox GL JS
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
// Import MapboxDraw for boundary drawing
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-draw/v1.3.0/mapbox-gl-draw.js';

// Mapbox access token - this should match the one used in Map.razor.js
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

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
                        'fill-color': '#3bb2d0',
                        'fill-outline-color': '#3bb2d0',
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
                        'line-color': '#3bb2d0',
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

        console.log("Draw controls initialized");
        return draw;
    } catch (error) {
        console.error("Error initializing draw controls:", error);
        throw error;
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
            // Notify .NET that the drawing was deleted
            dotNetRef.invokeMethodAsync('OnDrawingDeleted');
        });

        console.log("Drawing event handlers set up successfully");
    } catch (error) {
        console.error("Error setting up drawing events:", error);
    }
}

// Clear the map of all sources and layers
export function clearMap(map) {
    try {
        console.log("Clearing map");

        // Get all sources
        const sources = Object.keys(map.getStyle().sources || {});

        // Keep only non-mapbox sources (user-added sources)
        const userSources = sources.filter(source => !source.startsWith('mapbox.') && source !== 'composite');

        // Remove each user source
        userSources.forEach(source => {
            if (map.getSource(source)) {
                map.removeSource(source);
            }
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
            essential: true
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
                'fill-color': '#888888',
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
                'line-color': '#888888',
                'line-width': 2,
                'line-dasharray': [2, 1]
            }
        });

        console.log("Farm boundary drawn successfully");
        return true;
    } catch (error) {
        console.error("Error drawing farm boundary:", error);
        return false;
    }
}