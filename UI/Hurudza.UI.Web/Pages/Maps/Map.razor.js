import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-draw/v1.3.0/mapbox-gl-draw.js';

// Mapbox access token
mapboxgl.accessToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

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

        // Add fullscreen control
        map.addControl(new mapboxgl.FullscreenControl(), 'top-right');

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
                    'isField': isField
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

        // Add fill layer for all polygons
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

        // Add click handler for popup
        map.on('click', fillLayerId, (e) => {
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
            map.getCanvas().style.cursor = 'pointer';
        });

        map.on('mouseleave', fillLayerId, () => {
            map.getCanvas().style.cursor = '';
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
            const coordinates = e.features[0].geometry.coordinates.slice();
            const props = e.features[0].properties;
            const name = props.name;
            const size = props.size || 'N/A';
            const address = props.address || '';

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
                        <p style="margin-bottom: 5px; font-size: 12px;">${address}</p>
                        <p style="margin-bottom: 0; font-size: 12px;">Size: ${size} ha</p>
                    </div>
                `)
                .addTo(map);
        });

        // Add hover effects
        map.on('mouseenter', 'clusters', () => {
            map.getCanvas().style.cursor = 'pointer';
        });

        map.on('mouseenter', 'unclustered-point', () => {
            map.getCanvas().style.cursor = 'pointer';
        });

        map.on('mouseleave', 'clusters', () => {
            map.getCanvas().style.cursor = '';
        });

        map.on('mouseleave', 'unclustered-point', () => {
            map.getCanvas().style.cursor = '';
        });

        console.log('Farms loaded successfully');
    } catch (error) {
        console.error('Error loading farms:', error);
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

/**
 * Completely clears and resets the map
 * @param {Object} map - The Mapbox map instance
 * @returns {Promise<boolean>} Success indicator
 */
export function clearMap(map) {
    try {
        // Clear all custom layers and sources
        resetMapLayers(map)
            .then(() => {
                // Reset view to default
                map.setCenter([31.053028, -17.824858]);
                map.setZoom(7);
                console.log('Map cleared and reset to default view');
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