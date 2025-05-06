// FarmAdd.razor.js - Improved element reference handling
import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-geocoder/v4.7.2/mapbox-gl-geocoder.min.js';

// Mapbox access token
mapboxgl.accessToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

/**
 * Creates and returns a new Mapbox map instance with flexible element handling
 * @param {HTMLElement} element - The DOM element to attach the map to
 * @returns {Object} The map instance
 */
export function addMapToElement(element) {
    try {
        console.log('Initializing map...', element);
        
        let center = [31.053028, -17.824858]  // Default center (Zimbabwe)
        
        // Create and return a new map instance
        const map = new mapboxgl.Map({
            container: element,
            style: 'mapbox://styles/mapbox/satellite-streets-v12',
            center: center,
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

        // Add current marker if coordinates are valid
        if (center && center.length === 2 && (center[0] !== 0 || center[1] !== 0)) {
            const marker = new mapboxgl.Marker({
                color: "#FF5500",
                draggable: true
            })
                .setLngLat(center)
                .addTo(map);

            // Store marker on the map for later access
            map.currentMarker = marker;
        }

        // Wait for map to load before returning
        map.on('load', () => {
            console.log('Map loaded successfully');

            // Add geocoder if available
            if (typeof MapboxGeocoder !== 'undefined') {
                addGeocoderControl(map);
            }
        });

        return map;
    } catch (error) {
        console.error('Error initializing map:', error);
        throw error;
    }
}

/**
 * Add geocoder control to the map
 * @param {Object} map - The Mapbox map instance
 */
function addGeocoderControl(map) {
    try {
        const geocoder = new MapboxGeocoder({
            accessToken: mapboxgl.accessToken,
            mapboxgl: mapboxgl,
            marker: false,
            placeholder: 'Search for a location'
        });

        map.addControl(geocoder, 'top-left');

        // Handle result selection
        geocoder.on('result', (e) => {
            const coordinates = e.result.center; // [longitude, latitude]

            // Update marker position
            if (map.currentMarker) {
                map.currentMarker.setLngLat(coordinates);
            } else {
                map.currentMarker = new mapboxgl.Marker({
                    color: "#FF5500",
                    draggable: true
                })
                    .setLngLat(coordinates)
                    .addTo(map);
            }

            // Call the .NET method to update coordinates if available
            if (map.dotNetReference) {
                map.dotNetReference.invokeMethodAsync('UpdateCoordinates', coordinates[0], coordinates[1]);
            }
        });
    } catch (error) {
        console.error('Error adding geocoder control:', error);
    }
}

/**
 * Adds click handler to map for setting coordinates
 * @param {Object} map - The Mapbox map instance
 * @param {Object} dotNetRef - Reference to .NET component
 */
export function addClickHandler(map, dotNetRef) {
    if (!map) {
        console.error('Map instance is null');
        return false;
    }

    // Store the .NET reference on the map
    map.dotNetReference = dotNetRef;

    // Add click event listener for setting location
    map.on('click', (e) => {
        const { lng, lat } = e.lngLat;
        console.log(`Map clicked at [${lng}, ${lat}]`);

        // Update marker position
        if (map.currentMarker) {
            map.currentMarker.setLngLat([lng, lat]);
        } else {
            map.currentMarker = new mapboxgl.Marker({
                color: "#FF5500",
                draggable: true
            })
                .setLngLat([lng, lat])
                .addTo(map);

            // Add drag event for marker
            map.currentMarker.on('dragend', function() {
                const lngLat = map.currentMarker.getLngLat();
                if (map.dotNetReference) {
                    map.dotNetReference.invokeMethodAsync('UpdateCoordinates', lngLat.lng, lngLat.lat);
                }
            });
        }

        // Update coordinates in Blazor
        dotNetRef.invokeMethodAsync('UpdateCoordinates', lng, lat);
    });

    // Add drag event for marker
    if (map.currentMarker) {
        map.currentMarker.on('dragend', function() {
            const lngLat = map.currentMarker.getLngLat();
            if (dotNetRef) {
                dotNetRef.invokeMethodAsync('UpdateCoordinates', lngLat.lng, lngLat.lat);
            }
        });
    }

    return true;
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
        if (!map) {
            console.error('Map instance is null');
            return false;
        }

        map.flyTo({
            center: [longitude, latitude],
            zoom: zoom,
            duration: 1000 // Smooth transition over 1 second
        });
        console.log(`Map centered at [${longitude}, ${latitude}], zoom: ${zoom}`);
        return true;
    } catch (error) {
        console.error('Error setting map center:', error);
        return false;
    }
}