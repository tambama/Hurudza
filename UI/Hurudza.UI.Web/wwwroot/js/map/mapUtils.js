// Import Mapbox GL JS
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';

// Mapbox access token
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

// Store map instances by container ID
const mapInstances = {};

// Initialize Mapbox
export function initializeMapbox() {
    mapboxgl.accessToken = mapboxToken;
    console.log("Mapbox initialized with token");
    return mapboxgl;
}

// Create a new map instance
export function createMap(containerId, center = [31.053028, -17.824858], zoom = 14) {
    try {
        console.log(`Creating map in container #${containerId} at [${center}] with zoom ${zoom}`);

        // Initialize Mapbox if needed
        initializeMapbox();

        // Find the map container
        const container = document.getElementById(containerId);
        if (!container) {
            console.error(`Map container #${containerId} not found`);
            return null;
        }

        // Create and store the map instance
        const map = new mapboxgl.Map({
            container: containerId,
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: center,
            zoom: zoom,
            attributionControl: true
        });

        // Add navigation controls
        map.addControl(new mapboxgl.NavigationControl(), 'bottom-right');

        // Add scale control
        map.addControl(new mapboxgl.ScaleControl({
            maxWidth: 200,
            unit: 'metric'
        }), 'bottom-left');

        // Store the map instance
        mapInstances[containerId] = map;

        // Wait for map to load before returning
        map.on('load', () => {
            console.log(`Map #${containerId} loaded successfully`);
        });

        return map;
    } catch (error) {
        console.error(`Error creating map #${containerId}:`, error);
        return null;
    }
}

// Get an existing map instance
export function getMap(containerId) {
    return mapInstances[containerId] || null;
}

// Draw a polygon on the map
export function drawFieldPolygon(map, fieldId, coordinates, name = 'Field') {
    try {
        if (!map) {
            console.error("Map instance is null or undefined");
            return false;
        }

        // Wait for map to be fully loaded
        if (!map.loaded()) {
            map.on('load', () => {
                drawFieldPolygonInternal(map, fieldId, coordinates, name);
            });
        } else {
            drawFieldPolygonInternal(map, fieldId, coordinates, name);
        }

        return true;
    } catch (error) {
        console.error(`Error drawing field polygon for ${fieldId}:`, error);
        return false;
    }
}

// Internal function to draw field polygon
function drawFieldPolygonInternal(map, fieldId, coordinates, name) {
    try {
        // Define source and layer IDs
        const sourceId = `field-source-${fieldId}`;
        const outlineLayerId = `field-outline-${fieldId}`;
        const fillLayerId = `field-fill-${fieldId}`;

        // Remove existing layers and source if they exist
        if (map.getLayer(outlineLayerId)) map.removeLayer(outlineLayerId);
        if (map.getLayer(fillLayerId)) map.removeLayer(fillLayerId);
        if (map.getSource(sourceId)) map.removeSource(sourceId);

        // Ensure coordinates are in the correct format for a polygon
        // If coordinates are a flat array of [lng, lat] pairs, convert to GeoJSON format
        let polygonCoordinates = coordinates;
        if (Array.isArray(coordinates) && coordinates.length > 0 &&
            !Array.isArray(coordinates[0]) && typeof coordinates[0] === 'number') {
            // Convert flat [lng1, lat1, lng2, lat2, ...] to [[lng1, lat1], [lng2, lat2], ...]
            const pairs = [];
            for (let i = 0; i < coordinates.length; i += 2) {
                if (i + 1 < coordinates.length) {
                    pairs.push([coordinates[i], coordinates[i + 1]]);
                }
            }
            polygonCoordinates = [pairs];
        } else if (Array.isArray(coordinates) && coordinates.length > 0 &&
            Array.isArray(coordinates[0]) && coordinates[0].length === 2) {
            // Convert [[lng1, lat1], [lng2, lat2], ...] to [[[lng1, lat1], [lng2, lat2], ...]]
            polygonCoordinates = [coordinates];
        }

        // Check if we have valid coordinates for a polygon (at least 3 points)
        if (!polygonCoordinates || !Array.isArray(polygonCoordinates) ||
            !polygonCoordinates[0] || polygonCoordinates[0].length < 3) {
            console.warn(`Not enough coordinates to draw polygon for ${fieldId}`);
            return false;
        }

        // Add the source for this polygon
        map.addSource(sourceId, {
            'type': 'geojson',
            'data': {
                'type': 'Feature',
                'geometry': {
                    'type': 'Polygon',
                    'coordinates': polygonCoordinates
                },
                'properties': {
                    'name': name,
                    'id': fieldId
                }
            }
        });

        // Add outline layer
        map.addLayer({
            'id': outlineLayerId,
            'type': 'line',
            'source': sourceId,
            'layout': {},
            'paint': {
                'line-color': '#ff5500',
                'line-width': 2
            }
        });

        // Add fill layer
        map.addLayer({
            'id': fillLayerId,
            'type': 'fill',
            'source': sourceId,
            'layout': {},
            'paint': {
                'fill-color': '#ff9966',
                'fill-opacity': 0.3
            }
        });

        // Add click handler for popup
        map.on('click', fillLayerId, (e) => {
            const coordinates = e.lngLat;

            // Create and display the popup
            new mapboxgl.Popup({
                closeButton: true,
                closeOnClick: true,
                maxWidth: '300px'
            })
                .setLngLat(coordinates)
                .setHTML(`
                    <div style="max-width: 250px; word-wrap: break-word;">
                        <h4 style="margin-bottom: 5px;">${name}</h4>
                        <p style="margin-bottom: 0; font-size: 12px;">Field ID: ${fieldId}</p>
                    </div>
                `)
                .addTo(map);
        });

        // Add mouse interaction handlers for hover effects
        map.on('mouseenter', fillLayerId, () => {
            map.getCanvas().style.cursor = 'pointer';
        });

        map.on('mouseleave', fillLayerId, () => {
            map.getCanvas().style.cursor = '';
        });

        console.log(`Successfully drew field polygon for ${fieldId}`);
        return true;
    } catch (error) {
        console.error(`Error in drawFieldPolygonInternal for ${fieldId}:`, error);
        return false;
    }
}

// Dispose a map instance
export function disposeMap(containerId) {
    const map = mapInstances[containerId];
    if (map) {
        map.remove();
        delete mapInstances[containerId];
        console.log(`Map #${containerId} disposed`);
        return true;
    }
    return false;
}

// Center map on coordinates
export function centerMap(containerId, lng, lat, zoom = 14) {
    const map = mapInstances[containerId];
    if (map) {
        map.flyTo({
            center: [lng, lat],
            zoom: zoom,
            essential: true
        });
        console.log(`Map #${containerId} centered on [${lng}, ${lat}] with zoom ${zoom}`);
        return true;
    }
    console.warn(`Map #${containerId} not found`);
    return false;
}

// Convert field locations to GeoJSON polygon
export function fieldLocationsToPolygon(locations) {
    try {
        if (!locations || !Array.isArray(locations) || locations.length < 3) {
            console.warn('Not enough coordinates for a valid polygon');
            return null;
        }

        // Sort locations by created date if available
        const sortedLocations = [...locations].sort((a, b) => {
            if (a.createdDate && b.createdDate) {
                return new Date(a.createdDate) - new Date(b.createdDate);
            }
            return 0;
        });

        // Extract coordinates
        const coordinates = sortedLocations.map(loc => [loc.longitude, loc.latitude]);

        // Close the polygon if needed (first point should equal last point)
        if (coordinates.length > 0 &&
            (coordinates[0][0] !== coordinates[coordinates.length - 1][0] ||
                coordinates[0][1] !== coordinates[coordinates.length - 1][1])) {
            coordinates.push([...coordinates[0]]);
        }

        return coordinates;
    } catch (error) {
        console.error('Error converting field locations to polygon:', error);
        return null;
    }
}