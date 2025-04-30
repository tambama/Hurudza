// FarmAdd.razor.js - Should be placed in UI/Hurudza.UI.Web/Pages/Farms/

// Initialize Mapbox
const mapboxToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';
const mapInstances = {};

// Initialize a map on the specified container
export function initializeMap(containerId, center, zoom) {
    try {
        console.log(`Creating map in container #${containerId} at [${center}] with zoom ${zoom}`);

        // Initialize Mapbox
        mapboxgl.accessToken = mapboxToken;

        // Find the map container
        const container = document.getElementById(containerId);
        if (!container) {
            console.error(`Map container #${containerId} not found`);
            return false;
        }

        // Create and store the map instance
        const map = new mapboxgl.Map({
            container: containerId,
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: center,
            zoom: zoom || 8,
            attributionControl: true
        });

        // Add navigation controls
        map.addControl(new mapboxgl.NavigationControl(), 'bottom-right');

        // Add scale control
        map.addControl(new mapboxgl.ScaleControl({
            maxWidth: 200,
            unit: 'metric'
        }), 'bottom-left');

        // Add current marker if coordinates are provided
        if (center && center.length === 2 && center[0] !== 0 && center[1] !== 0) {
            const marker = new mapboxgl.Marker({
                color: "#FF5500",
                draggable: false
            })
                .setLngLat(center)
                .addTo(map);

            // Store marker in the map object
            map.currentMarker = marker;
        }

        // Store the map instance
        mapInstances[containerId] = map;

        // Wait for map to load before returning
        map.on('load', () => {
            console.log(`Map #${containerId} loaded successfully`);
        });

        return true;
    } catch (error) {
        console.error(`Error creating map #${containerId}:`, error);
        return false;
    }
}

// Add a click handler to a map
export function addMapClickHandler(containerId, dotNetRef) {
    const map = mapInstances[containerId];
    if (!map) {
        console.error(`Map #${containerId} not found`);
        return false;
    }

    // Add click event listener
    map.on('click', (e) => {
        const { lng, lat } = e.lngLat;
        console.log(`Map clicked at [${lng}, ${lat}]`);

        // Update the marker position
        if (map.currentMarker) {
            map.currentMarker.setLngLat([lng, lat]);
        } else {
            map.currentMarker = new mapboxgl.Marker({
                color: "#FF5500",
                draggable: false
            })
                .setLngLat([lng, lat])
                .addTo(map);
        }

        // Call the .NET method to update the form
        dotNetRef.invokeMethodAsync('UpdateCoordinates', lng, lat);
    });

    return true;
}

// Draw a polygon for a field
export function drawFieldPolygon(containerId, fieldId, coordinates, name) {
    try {
        const map = mapInstances[containerId];
        if (!map) {
            console.error(`Map #${containerId} not found`);
            return false;
        }

        // Define source and layer IDs
        const sourceId = `field-source-${fieldId}`;
        const outlineLayerId = `field-outline-${fieldId}`;
        const fillLayerId = `field-fill-${fieldId}`;

        // Remove existing layers and source if they exist
        if (map.getLayer(outlineLayerId)) map.removeLayer(outlineLayerId);
        if (map.getLayer(fillLayerId)) map.removeLayer(fillLayerId);
        if (map.getSource(sourceId)) map.removeSource(sourceId);

        // Ensure coordinates are in the correct format
        let polygonCoordinates = coordinates;
        if (!Array.isArray(coordinates[0][0])) {
            // Convert [[lng1, lat1], [lng2, lat2], ...] to [[[lng1, lat1], [lng2, lat2], ...]]
            polygonCoordinates = [coordinates];
        }

        // Check if coordinates form a valid polygon (at least 3 points)
        if (!polygonCoordinates[0] || polygonCoordinates[0].length < 3) {
            console.warn(`Not enough coordinates to draw polygon for ${fieldId}`);
            return false;
        }

        // Make sure polygon is closed (first point equals last point)
        if (polygonCoordinates[0][0][0] !== polygonCoordinates[0][polygonCoordinates[0].length - 1][0] ||
            polygonCoordinates[0][0][1] !== polygonCoordinates[0][polygonCoordinates[0].length - 1][1]) {
            polygonCoordinates[0].push(polygonCoordinates[0][0]);
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

        return true;
    } catch (error) {
        console.error(`Error drawing field polygon for ${fieldId}:`, error);
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