// Import Mapbox GL JS
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';

// Mapbox access token
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

// Store the map instance to prevent duplicates
let mapInstance = null;

// Initialize Mapbox
function initializeMapbox() {
    mapboxgl.accessToken = mapboxToken;
    console.log("Mapbox initialized with token");
    return mapboxgl;
}

// Global function to load map that can be called from Blazor
window.loadMap = function(center) {
    try {
        console.log(`Attempting to load map with center: [${center}]`);

        // Check if the map container exists
        const mapContainer = document.getElementById('map');
        if (!mapContainer) {
            console.error("Map container 'map' not found in DOM");
            return false;
        }

        // If a map already exists, remove it first
        if (mapInstance) {
            console.log("Removing existing map instance before creating a new one");
            mapInstance.remove();
            mapInstance = null;
        }

        // Initialize Mapbox
        initializeMapbox();

        // Create map instance
        mapInstance = new mapboxgl.Map({
            container: mapContainer,
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: center,
            zoom: 16,
            attributionControl: true
        });

        // Wait for map to load before adding controls
        mapInstance.on('load', () => {
            console.log('Map loaded successfully');

            // Add navigation controls (after map load)
            mapInstance.addControl(new mapboxgl.NavigationControl(), 'bottom-right');

            // Add scale control (after map load)
            mapInstance.addControl(new mapboxgl.ScaleControl({
                maxWidth: 200,
                unit: 'metric'
            }), 'bottom-left');

            // Add fullscreen control (after map load)
            mapInstance.addControl(new mapboxgl.FullscreenControl(), 'top-right');

            // Store the map instance on the DOM element
            mapContainer.mapboxgl = mapInstance;

            // If we have pending function to run after load, run it now
            if (window._pendingDrawOperation) {
                console.log("Executing pending draw operation");
                window._pendingDrawOperation();
                window._pendingDrawOperation = null;
            }
        });

        console.log("Map initialization successful");
        return true;
    } catch (error) {
        console.error('Error initializing map:', error);
        return false;
    }
};

// Function to draw field boundaries on the map
window.drawFieldBoundary = function(coordinates, fieldName) {
    try {
        console.log("Attempting to draw field boundary");

        // Use stored map instance
        if (!mapInstance) {
            console.error("Map instance not found");
            return false;
        }

        // Function to actually add the layers
        const drawField = () => {
            try {
                console.log("Drawing field boundary with coordinates:", coordinates);

                // Clean up any existing field layers
                if (mapInstance.getLayer('field-outline')) mapInstance.removeLayer('field-outline');
                if (mapInstance.getLayer('field-fill')) mapInstance.removeLayer('field-fill');
                if (mapInstance.getSource('field-source')) mapInstance.removeSource('field-source');

                // Add the field boundary source
                mapInstance.addSource('field-source', {
                    'type': 'geojson',
                    'data': {
                        'type': 'Feature',
                        'geometry': {
                            'type': 'Polygon',
                            'coordinates': [coordinates]
                        },
                        'properties': {
                            'name': fieldName
                        }
                    }
                });

                // Add the field outline layer
                mapInstance.addLayer({
                    'id': 'field-outline',
                    'type': 'line',
                    'source': 'field-source',
                    'layout': {},
                    'paint': {
                        'line-color': '#ff5500',
                        'line-width': 2
                    }
                });

                // Add the field fill layer
                mapInstance.addLayer({
                    'id': 'field-fill',
                    'type': 'fill',
                    'source': 'field-source',
                    'layout': {},
                    'paint': {
                        'fill-color': '#ff9966',
                        'fill-opacity': 0.1
                    }
                });

                console.log("Field boundary drawn successfully");
                return true;
            } catch (err) {
                console.error("Error in drawField function:", err);
                return false;
            }
        };

        // Check if map is loaded
        if (!mapInstance.loaded()) {
            console.log("Map not fully loaded, storing draw function for later execution");
            // Store the draw operation to execute after load
            window._pendingDrawOperation = drawField;
        } else {
            return drawField();
        }

        return true;
    } catch (error) {
        console.error("Error drawing field boundary:", error);
        return false;
    }
};

// Function to explicitly destroy the map
window.destroyMap = function() {
    if (mapInstance) {
        console.log("Destroying map instance");
        try {
            mapInstance.remove();
        } catch (e) {
            console.error("Error removing map:", e);
        }
        mapInstance = null;
        return true;
    }
    console.log("No map instance to destroy");
    return false;
};

// Function to check if map is initialized
window.isMapInitialized = function() {
    return !!mapInstance;
};

export { initializeMapbox };