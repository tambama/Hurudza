// Import shared map drawing functionality
import * as mapDrawing from '../../js/modules/mapDrawingModule.js';

/**
 * Creates and returns a new Mapbox map instance
 * @param {HTMLElement} element - The DOM element to attach the map to
 * @returns {Object} The map instance
 */
export function addMapToElement(element) {
    console.log("Initializing main map view");
    return mapDrawing.initializeMap(element);
}

/**
 * Draw a polygon on the map (for farm boundaries or fields)
 * @param {Object} map - The Mapbox map instance
 * @param {string} id - Unique identifier for the polygon
 * @param {Array} coordinates - Polygon coordinates [[lng, lat], [lng, lat], ...]
 * @param {boolean} isField - Whether this polygon represents a field (vs farm)
 * @param {string} name - Name to display in popup
 * @param {Object} cropData - Optional crop data for field polygons
 * @param {boolean} clearExisting - Whether to clear existing layers before drawing
 * @returns {Promise<boolean>} Success indicator
 */
export function drawPolygon(map, id, coordinates, isField = false, name = 'Farm', cropData = null, clearExisting = false) {
    console.log(`Drawing ${isField ? 'field' : 'farm'} polygon: ${name}`);
    return mapDrawing.drawPolygon(map, id, coordinates, isField, name, cropData, clearExisting);
}

/**
 * Clears all custom sources and layers from the map
 * @param {Object} map - The Mapbox map instance
 * @returns {boolean} Success indicator
 */
export function clearMap(map) {
    console.log("Clearing main map view");
    return mapDrawing.clearMapLayers(map);
}

/**
 * Set the map's center and zoom level
 * @param {Object} map - The Mapbox map instance
 * @param {number} latitude - Latitude coordinate
 * @param {number} longitude - Longitude coordinate
 * @param {number} zoom - Optional zoom level (default: 14)
 */
export function setMapCenter(map, latitude, longitude, zoom = 14) {
    console.log(`Centering main map on [${longitude}, ${latitude}] with zoom ${zoom}`);
    return mapDrawing.centerMap(map, latitude, longitude, zoom);
}

/**
 * Load farms data as clustered points on the map
 * @param {Object} map - The Mapbox map instance
 * @param {Object} farms - GeoJSON FeatureCollection of farm data
 */
export function loadFarms(map, farms) {
    try {
        console.log("Loading farms with classification on main map");

        if (!map) {
            console.error("Map instance is not initialized");
            return false;
        }

        if (!farms || !farms.features || farms.features.length === 0) {
            console.log("No farm data to display");
            return false;
        }

        // Function to add sources and layers
        const addSourcesAndLayers = () => {
            // First, clean up any existing sources
            try {
                if (map.getSource('farms-data')) {
                    map.removeLayer('clusters');
                    map.removeLayer('cluster-count');
                    map.removeLayer('unclustered-point');
                    map.removeSource('farms-data');
                }
            } catch (e) {
                // Source doesn't exist, which is fine
            }

            // For now, use the existing pattern from your codebase
            // Just add all farms/schools as before, without classification
            map.addSource('farms-data', {
                type: 'geojson',
                data: farms,
                cluster: true,
                clusterMaxZoom: 14,
                clusterRadius: 50
            });

            // Cluster layer
            map.addLayer({
                id: 'clusters',
                type: 'circle',
                source: 'farms-data',
                filter: ['has', 'point_count'],
                paint: {
                    'circle-color': [
                        'step',
                        ['get', 'point_count'],
                        '#51bbd6',
                        10,
                        '#f1f075',
                        20,
                        '#f28cb1'
                    ],
                    'circle-radius': [
                        'step',
                        ['get', 'point_count'],
                        20,
                        10,
                        30,
                        20,
                        40
                    ]
                }
            });

            // Cluster count layer
            map.addLayer({
                id: 'cluster-count',
                type: 'symbol',
                source: 'farms-data',
                filter: ['has', 'point_count'],
                layout: {
                    'text-field': '{point_count_abbreviated}',
                    'text-font': ['DIN Offc Pro Medium', 'Arial Unicode MS Bold'],
                    'text-size': 12
                }
            });

            // Individual points layer
            map.addLayer({
                id: 'unclustered-point',
                type: 'circle',
                source: 'farms-data',
                filter: ['!', ['has', 'point_count']],
                paint: {
                    'circle-color': '#11b4da',
                    'circle-radius': 6,
                    'circle-stroke-width': 2,
                    'circle-stroke-color': '#fff'
                }
            });

            // Add click handlers for clusters and points
            map.on('click', 'clusters', (e) => {
                const features = map.queryRenderedFeatures(e.point, {
                    layers: ['clusters']
                });
                const clusterId = features[0].properties.cluster_id;
                map.getSource('farms-data').getClusterExpansionZoom(
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

            // Change cursor on hover
            map.on('mouseenter', 'clusters', () => {
                map.getCanvas().style.cursor = 'pointer';
            });
            map.on('mouseleave', 'clusters', () => {
                map.getCanvas().style.cursor = '';
            });

            console.log(`Loaded ${farms.features.length} farms on map`);
        };

        // Check if the map is loaded
        if (map.loaded()) {
            addSourcesAndLayers();
        } else {
            // Wait for the map to load
            map.once('load', addSourcesAndLayers);
        }

        return true;

    } catch (error) {
        console.error("Error loading farms:", error);
        return false;
    }
}

/**
 * Adds a custom marker to the map for schools or farms
 * @param {Object} map - The Mapbox map instance
 * @param {string} id - Unique identifier for the marker
 * @param {number} lat - Latitude coordinate
 * @param {number} lng - Longitude coordinate
 * @param {string} name - Name to display in the popup
 * @param {string} color - Hex color for the marker (e.g., '#FF6B6B')
 * @param {string} type - Type of marker ('school' or 'farm')
 * @returns {boolean} Success indicator
 */
export function addMarker(map, id, lat, lng, name, color, type) {
    try {
        // Create a custom marker element
        const el = document.createElement('div');
        el.className = 'custom-marker';
        el.style.backgroundColor = color;
        el.style.width = '30px';
        el.style.height = '30px';
        el.style.borderRadius = type === 'school' ? '50%' : '5px';
        el.style.border = '2px solid white';
        el.style.boxShadow = '0 2px 4px rgba(0,0,0,0.3)';
        el.style.cursor = 'pointer';

        // Add marker to map
        const marker = new mapboxgl.Marker(el)
            .setLngLat([lng, lat])
            .setPopup(new mapboxgl.Popup().setHTML(`
                <h6>${name}</h6>
                <p>Type: ${type === 'school' ? 'School' : 'Farm'}</p>
            `))
            .addTo(map);

        // Store marker reference
        if (!window.mapMarkers) {
            window.mapMarkers = {};
        }
        window.mapMarkers[id] = marker;

        return true;
    } catch (error) {
        console.error('Error adding marker:', error);
        return false;
    }
}

/**
 * Enables click-to-add location functionality on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} callbackMethodName - Name of the C# method to invoke
 * @returns {boolean} Success indicator
 */
export function enableLocationPicker(map, callbackMethodName) {
    try {
        map.getCanvas().style.cursor = 'crosshair';

        const clickHandler = (e) => {
            const { lng, lat } = e.lngLat;

            // Add temporary marker
            const el = document.createElement('div');
            el.className = 'temp-marker';
            el.style.backgroundColor = '#FF6B6B';
            el.style.width = '20px';
            el.style.height = '20px';
            el.style.borderRadius = '50%';
            el.style.border = '2px solid white';
            el.style.boxShadow = '0 2px 4px rgba(0,0,0,0.3)';

            if (window.tempMarker) {
                window.tempMarker.remove();
            }

            window.tempMarker = new mapboxgl.Marker(el)
                .setLngLat([lng, lat])
                .addTo(map);

            // Invoke callback with coordinates
            DotNet.invokeMethodAsync('Hurudza.UI.Web', callbackMethodName, lat, lng);

            // Reset cursor and remove handler
            map.getCanvas().style.cursor = '';
            map.off('click', clickHandler);
        };

        map.on('click', clickHandler);

        return true;
    } catch (error) {
        console.error('Error enabling location picker:', error);
        return false;
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

                // Add filter icon
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

        // Create and add filter control
        const filterControl = new FilterControl(dotNetRef);
        map.addControl(filterControl, 'top-right');

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

/**
 * Initializes drawing controls on the map - using default MapboxDraw controls
 * @param {Object} map - The Mapbox map instance
 * @param {Object} dotNetRef - Reference to .NET component
 * @returns {Object} The draw control
 */
export function initializeDrawControls(map, dotNetRef) {
    console.log("Initializing default draw controls for main map");
    return mapDrawing.initializeDrawControls(map, dotNetRef);
}

/**
 * Sets up the drawing mode with event handlers - simplified version
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The drawing control
 * @param {Object} dotNetRef - Reference to .NET component
 */
export function setupDrawingEvents(map, draw, dotNetRef) {
    try {
        console.log("Setting up drawing events for main map");

        // Set up standard drawing events using the imported mapDrawingModule function
        mapDrawing.setupDrawingEvents(map, draw, dotNetRef);

        // Return success object that matches the expected format
        return {
            Success: true,
            Message: "Drawing controls setup successfully"
        };
    } catch (error) {
        console.error("Error setting up drawing controls:", error);
        return {
            Success: false,
            Message: error.message
        };
    }
}

/**
 * Load an existing polygon into the drawing control for editing
 * @param {Object} map - The Mapbox map instance
 * @param {Object} draw - The drawing control
 * @param {Array} coordinates - Array of coordinates
 * @returns {boolean} Success indicator
 */
export function loadPolygonForEditing(map, draw, coordinates) {
    console.log(`Loading polygon with ${coordinates.length} points for editing in main map`);
    return mapDrawing.loadPolygonForEditing(map, draw, coordinates);
}

/**
 * Highlight a specific field on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} fieldId - ID of the field to highlight
 */
export function highlightFieldOnMap(map, fieldId) {
    console.log(`Highlighting field ${fieldId} on main map`);
    return mapDrawing.highlightField(map, fieldId);
}

/**
 * Removes highlight from a specific field on the map
 * @param {Object} map - The Mapbox map instance
 * @param {string} fieldId - ID of the field to unhighlight
 */
export function unhighlightFieldOnMap(map, fieldId) {
    console.log(`Removing highlight from field ${fieldId} on main map`);
    return mapDrawing.unhighlightField(map, fieldId);
}

/**
 * Reset all custom map layers
 * @param {Object} map - The Mapbox map instance
 * @returns {Promise<boolean>} Success indicator
 */
export function resetMapLayers(map) {
    console.log("Resetting all map layers on main map");
    return mapDrawing.clearMapLayers(map);
}

/**
 * Calculate field size from coordinates using Turf.js
 * @param {Array} coordinates - Array of coordinate points
 * @returns {Promise<number>} Area in hectares
 */
export async function calculateFieldSizeFromCoordinates(coordinates) {
    try {
        // Load Turf.js if needed
        if (typeof turf === 'undefined') {
            await mapDrawing.loadTurf();
        }

        if (!coordinates || !Array.isArray(coordinates) || coordinates.length < 3) {
            console.warn('Not enough coordinates to calculate area');
            return 0;
        }

        // Create a closed polygon if not already closed
        let polygonCoords = [...coordinates];
        if (polygonCoords.length > 0 &&
            (polygonCoords[0][0] !== polygonCoords[polygonCoords.length - 1][0] ||
                polygonCoords[0][1] !== polygonCoords[polygonCoords.length - 1][1])) {
            polygonCoords.push([...polygonCoords[0]]);
        }

        // Create a Turf.js polygon from coordinates
        const polygon = turf.polygon([polygonCoords]);

        // Calculate area in square meters
        const areaInSquareMeters = turf.area(polygon);

        // Convert to hectares (1 hectare = 10,000 square meters)
        const hectares = areaInSquareMeters / 10000;

        // Round to 2 decimal places
        return Math.round(hectares * 100) / 100;
    } catch (error) {
        console.error('Error calculating field size from coordinates:', error);
        return 0;
    }
}

// Export other utility functions
export const loadTurf = mapDrawing.loadTurf;
export const clearDrawnPolygons = mapDrawing.clearAllDrawings;
export const clearAllDrawings = mapDrawing.clearAllDrawings;