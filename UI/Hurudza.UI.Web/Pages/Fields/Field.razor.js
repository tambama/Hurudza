// Import Mapbox GL JS
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';

// Mapbox access token
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

// Initialize Mapbox
function initializeMapbox() {
    mapboxgl.accessToken = mapboxToken;
    console.log("Mapbox initialized with token");
    return mapboxgl;
}

// Global function to load map that can be called from Blazor
window.loadMap = function(center) {
    try {
        console.log(`Loading map with center: [${center}]`);

        // Initialize Mapbox
        initializeMapbox();

        // Create map instance
        const map = new mapboxgl.Map({
            container: 'map',
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: center,
            zoom: 14,
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
        return null;
    }
};

// Function to draw field boundaries on the map
window.drawFieldBoundary = function(coordinates, fieldName) {
    try {
        // Get the map instance
        const map = document.getElementById('map').mapboxgl;
        if (!map) {
            console.error("Map instance not found");
            return false;
        }

        // Clean up any existing field layers
        if (map.getLayer('field-outline')) map.removeLayer('field-outline');
        if (map.getLayer('field-fill')) map.removeLayer('field-fill');
        if (map.getSource('field-source')) map.removeSource('field-source');

        // Add the field boundary source
        map.addSource('field-source', {
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
        map.addLayer({
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
        map.addLayer({
            'id': 'field-fill',
            'type': 'fill',
            'source': 'field-source',
            'layout': {},
            'paint': {
                'fill-color': '#ff9966',
                'fill-opacity': 0.3
            }
        });

        console.log("Field boundary drawn successfully");
        return true;
    } catch (error) {
        console.error("Error drawing field boundary:", error);
        return false;
    }
};

export { initializeMapbox };