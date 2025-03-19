import 'https://api.mapbox.com/mapbox-gl-js/v3.6.0/mapbox-gl.js';
import 'https://api.mapbox.com/mapbox-gl-js/plugins/mapbox-gl-draw/v1.3.0/mapbox-gl-draw.js';

mapboxgl.accessToken = 'pk.eyJ1IjoicGVuaWVsdCIsImEiOiJjbGt3Y2gxM3YweWtrM3FwbW9jaWNkMWVyIn0.cDAgTWNXN-TVJROjgoWQiw';

export function addMapToElement(element) {
    return new mapboxgl.Map({
        container: element,
        style: 'mapbox://styles/mapbox/satellite-streets-v12',
        center: [31.053028, -17.824858],
        zoom: 7
    });
}

// Completely refactored drawPolygon function
export async function drawPolygon(map, id, coordinates, isField = false, name = 'Farm', cropData = null) {
    // Wait for map reset to complete
    await resetMapForField(map);

    // Define source and layer IDs
    const sourceId = 'hurudza-' + id;
    const outlineLayerId = 'outline-' + id;
    const fillLayerId = 'fill-' + id;

    try {
        // Add the source
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
                    'cropData': cropData
                }
            }
        });

        // Set color based on field type
        let lineColor = isField ? '#ff0000' : '#40b7d5';

        // Add outline layer
        map.addLayer({
            'id': outlineLayerId,
            'type': 'line',
            'source': sourceId,
            'layout': {},
            'paint': {
                'line-color': lineColor,
                'line-width': isField ? 1 : 3
            }
        });

        // Add fill layer for fields
        if (isField) {
            map.addLayer({
                'id': fillLayerId,
                'type': 'fill',
                'source': sourceId,
                'layout': {},
                'paint': {
                    'fill-color': cropData ? '#74c476' : '#0080ff',
                    'fill-opacity': 0.2
                }
            });

            // Add click handler for fields
            map.on('click', fillLayerId, (e) => {
                const coordinates = e.lngLat;
                const feature = e.features[0];
                const name = feature.properties.name;
                const cropData = feature.properties.cropData;

                let popupContent = `<div><h4>${name}</h4>`;

                if (cropData && Object.keys(cropData).length > 0) {
                    popupContent += `
            <p><strong>Current Crop:</strong> ${cropData.crop}</p>
            <p><strong>Planted:</strong> ${cropData.plantedDate ? new Date(cropData.plantedDate).toLocaleDateString() : 'Not set'}</p>
            <p><strong>Expected Harvest:</strong> ${cropData.harvestDate ? new Date(cropData.harvestDate).toLocaleDateString() : 'Not set'}</p>
            <p><strong>Area Planted:</strong> ${cropData.size} ha</p>
            <p><strong>Irrigation:</strong> ${cropData.irrigation ? 'Yes' : 'No'}</p>
          `;
                } else {
                    popupContent += `<p>No crops currently planted</p>`;
                }

                popupContent += `</div>`;

                new mapboxgl.Popup()
                    .setLngLat(coordinates)
                    .setHTML(popupContent)
                    .addTo(map);
            });

            // Add mouse interaction handlers
            map.on('mouseenter', fillLayerId, () => {
                map.getCanvas().style.cursor = 'pointer';
            });

            map.on('mouseleave', fillLayerId, () => {
                map.getCanvas().style.cursor = '';
            });
        }

        console.log(`Successfully drew polygon for ${id}`);
        return true;
    } catch (error) {
        console.error(`Error drawing polygon for ${id}:`, error);
        return false;
    }
}

export function loadFarms(map, farms) {
    // Check if the source already exists and remove it
    if (map.getSource('farms')) {
        if (map.getLayer('clusters')) map.removeLayer('clusters');
        if (map.getLayer('cluster-count')) map.removeLayer('cluster-count');
        if (map.getLayer('unclustered-point')) map.removeLayer('unclustered-point');
        map.removeSource('farms');
    }

    // Add a new source from the GeoJSON data
    map.addSource('farms', {
        type: 'geojson',
        data: farms,
        cluster: true,
        clusterMaxZoom: 14, // Max zoom to cluster points on
        clusterRadius: 50 // Radius of each cluster when clustering points
    });

    // Add a layer showing the clusters
    map.addLayer({
        id: 'clusters',
        type: 'circle',
        source: 'farms',
        filter: ['has', 'point_count'],
        paint: {
            // Use step expressions for three types of circles:
            // Small blue circles for clusters with < 10 points
            // Medium yellow circles for clusters with < 50 points
            // Large pink circles for clusters with >= 50 points
            'circle-color': [
                'step',
                ['get', 'point_count'],
                '#51bbd6', // Blue
                10,
                '#f1f075', // Yellow
                50,
                '#f28cb1' // Pink
            ],
            'circle-radius': [
                'step',
                ['get', 'point_count'],
                20, // 20px radius when point count is less than 10
                10,
                30, // 30px radius when point count is between 10 and 50
                50,
                40 // 40px radius when point count is greater than or equal to 50
            ]
        }
    });

    // Add a layer for the cluster counts
    map.addLayer({
        id: 'cluster-count',
        type: 'symbol',
        source: 'farms',
        filter: ['has', 'point_count'],
        layout: {
            'text-field': ['get', 'point_count_abbreviated'],
            'text-font': ['DIN Offc Pro Medium', 'Arial Unicode MS Bold'],
            'text-size': 12
        }
    });

    // Add a layer for individual points
    map.addLayer({
        id: 'unclustered-point',
        type: 'circle',
        source: 'farms',
        filter: ['!', ['has', 'point_count']],
        paint: {
            'circle-color': '#11b4da', // Blue color for individual points
            'circle-radius': 8,
            'circle-stroke-width': 1,
            'circle-stroke-color': '#fff' // White outline
        }
    });

    // Inspect a cluster on click
    map.on('click', 'clusters', (e) => {
        const features = map.queryRenderedFeatures(e.point, {
            layers: ['clusters']
        });
        const clusterId = features[0].properties.cluster_id;
        map.getSource('farms').getClusterExpansionZoom(
            clusterId,
            (err, zoom) => {
                if (err) return;

                // Center the map on the clicked cluster
                map.easeTo({
                    center: features[0].geometry.coordinates,
                    zoom: zoom
                });
            }
        );
    });

    // Show farm name and size in a popup when clicking on an individual point
    map.on('click', 'unclustered-point', (e) => {
        const coordinates = e.features[0].geometry.coordinates.slice();
        const name = e.features[0].properties.name;
        const size = e.features[0].properties.size || 'N/A';

        // Ensure that if the map is zoomed out such that multiple
        // copies of the feature are visible, the popup appears
        // over the copy being pointed to.
        while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
            coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
        }

        new mapboxgl.Popup()
            .setLngLat(coordinates)
            .setHTML(`
                <div>
                    <h4>${name}</h4>
                    <p>Size: ${size} ha</p>
                </div>
            `)
            .addTo(map);
    });

    // Change the cursor to a pointer when hovering over a cluster or point
    map.on('mouseenter', 'clusters', () => {
        map.getCanvas().style.cursor = 'pointer';
    });
    map.on('mouseenter', 'unclustered-point', () => {
        map.getCanvas().style.cursor = 'pointer';
    });

    // Change back to default cursor when not hovering over a cluster or point
    map.on('mouseleave', 'clusters', () => {
        map.getCanvas().style.cursor = '';
    });
    map.on('mouseleave', 'unclustered-point', () => {
        map.getCanvas().style.cursor = '';
    });
}

export function loadClusters(map, farms){
    console.log(farms);
    map.addSource('earthquakes', {
        type: 'geojson',
        // Point to GeoJSON data. This example visualizes all M1.0+ earthquakes
        // from 12/22/15 to 1/21/16 as logged by USGS' Earthquake hazards program.
        data: 'https://docs.mapbox.com/mapbox-gl-js/assets/earthquakes.geojson',
        cluster: true,
        clusterMaxZoom: 14, // Max zoom to cluster points on
        clusterRadius: 50 // Radius of each cluster when clustering points (defaults to 50)
    });

    map.addLayer({
        id: 'clusters',
        type: 'circle',
        source: 'earthquakes',
        filter: ['has', 'point_count'],
        paint: {
            // Use step expressions (https://docs.mapbox.com/style-spec/reference/expressions/#step)
            // with three steps to implement three types of circles:
            //   * Blue, 20px circles when point count is less than 100
            //   * Yellow, 30px circles when point count is between 100 and 750
            //   * Pink, 40px circles when point count is greater than or equal to 750
            'circle-color': [
                'step',
                ['get', 'point_count'],
                '#51bbd6',
                100,
                '#f1f075',
                750,
                '#f28cb1'
            ],
            'circle-radius': [
                'step',
                ['get', 'point_count'],
                20,
                100,
                30,
                750,
                40
            ]
        }
    });

    map.addLayer({
        id: 'cluster-count',
        type: 'symbol',
        source: 'earthquakes',
        filter: ['has', 'point_count'],
        layout: {
            'text-field': ['get', 'point_count_abbreviated'],
            'text-font': ['DIN Offc Pro Medium', 'Arial Unicode MS Bold'],
            'text-size': 12
        }
    });

    map.addLayer({
        id: 'unclustered-point',
        type: 'circle',
        source: 'earthquakes',
        filter: ['!', ['has', 'point_count']],
        paint: {
            'circle-color': '#11b4da',
            'circle-radius': 4,
            'circle-stroke-width': 1,
            'circle-stroke-color': '#fff'
        }
    });

    // inspect a cluster on click
    map.on('click', 'clusters', (e) => {
        const features = map.queryRenderedFeatures(e.point, {
            layers: ['clusters']
        });
        const clusterId = features[0].properties.cluster_id;
        map.getSource('earthquakes').getClusterExpansionZoom(
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

    // When a click event occurs on a feature in
    // the unclustered-point layer, open a popup at
    // the location of the feature, with
    // description HTML from its properties.
    map.on('click', 'unclustered-point', (e) => {
        const coordinates = e.features[0].geometry.coordinates.slice();
        const mag = e.features[0].properties.mag;
        const tsunami =
            e.features[0].properties.tsunami === 1 ? 'yes' : 'no';

        // Ensure that if the map is zoomed out such that
        // multiple copies of the feature are visible, the
        // popup appears over the copy being pointed to.
        if (['mercator', 'equirectangular'].includes(map.getProjection().name)) {
            while (Math.abs(e.lngLat.lng - coordinates[0]) > 180) {
                coordinates[0] += e.lngLat.lng > coordinates[0] ? 360 : -360;
            }
        }

        new mapboxgl.Popup()
            .setLngLat(coordinates)
            .setHTML(
                `magnitude: ${mag}<br>Was there a tsunami?: ${tsunami}`
            )
            .addTo(map);
    });

    map.on('mouseenter', 'clusters', () => {
        map.getCanvas().style.cursor = 'pointer';
    });
    map.on('mouseleave', 'clusters', () => {
        map.getCanvas().style.cursor = '';
    });
}

export function setMapStyle(map, style) {
    map.setStyle('mapbox://styles/mapbox/' + style);
}

export function setMapCenter(map, latitude, longitude) {
    map.setCenter([longitude, latitude]);
    map.setZoom(14)
}

// Improved clearMap function
export function clearMap(map) {
    return resetMapForField(map).then(() => {
        // Reset the map view
        map.setCenter([31.053028, -17.824858]);
        map.setZoom(7);
        return true;
    });
}

function filterObjectsByName(objects, startsWith) {
    return objects.filter(obj => obj.source.startsWith(startsWith));
}

function getUniqueItemsById(items) {
    const uniqueItems = {};
    items.forEach(item => {
        if (!uniqueItems[item.id]) {
            uniqueItems[item.id] = item;
        }
    });
    return Object.values(uniqueItems);
}

function getUniqueLayersById(items) {
    const uniqueItems = {};
    items.forEach(item => {
        if (!uniqueItems[item.layer.id]) {
            uniqueItems[item.layer.id] = item;
        }
    });
    return Object.values(uniqueItems);
}

function getUniqueSources(items) {
    const uniqueItems = {};
    items.forEach(item => {
        if (!uniqueItems[item.source]) {
            uniqueItems[item.source] = item;
        }
    });
    return Object.values(uniqueItems);
}

// This function safely checks if a source exists
export function sourceExists(map, sourceId) {
    try {
        return !!map.getSource(sourceId);
    } catch (e) {
        console.warn(`Error checking if source ${sourceId} exists:`, e);
        return false;
    }
}

// This function safely checks if a layer exists
export function layerExists(map, layerId) {
    try {
        return !!map.getLayer(layerId);
    } catch (e) {
        console.warn(`Error checking if layer ${layerId} exists:`, e);
        return false;
    }
}

// This function completely resets a map's sources and layers before drawing
export function resetMapForField(map, fieldId) {
    return new Promise((resolve) => {
        try {
            // 1. Force wait a moment to ensure all async operations have completed
            setTimeout(() => {
                try {
                    // 2. Get a list of all sources and layers that need to be removed
                    const hurudzaSources = Object.keys(map.getStyle().sources || {})
                        .filter(id => id.startsWith('hurudza'));

                    const hurudzaLayers = map.getStyle().layers
                        .filter(layer => layer.source && hurudzaSources.includes(layer.source))
                        .map(layer => layer.id);

                    // 3. Remove all layers first (must be done before removing sources)
                    hurudzaLayers.forEach(layerId => {
                        if (layerExists(map, layerId)) {
                            // Remove any event handlers
                            try {
                                map.off('click', layerId);
                                map.off('mouseenter', layerId);
                                map.off('mouseleave', layerId);
                            } catch (e) {
                                console.warn(`Error removing handlers for layer ${layerId}:`, e);
                            }

                            // Remove the layer
                            try {
                                map.removeLayer(layerId);
                                console.log(`Removed layer: ${layerId}`);
                            } catch (e) {
                                console.warn(`Error removing layer ${layerId}:`, e);
                            }
                        }
                    });

                    // 4. Then remove all sources
                    hurudzaSources.forEach(sourceId => {
                        if (sourceExists(map, sourceId)) {
                            try {
                                map.removeSource(sourceId);
                                console.log(`Removed source: ${sourceId}`);
                            } catch (e) {
                                console.warn(`Error removing source ${sourceId}:`, e);
                            }
                        }
                    });

                    // 5. Force the map to redraw
                    map.triggerRepaint();

                    // 6. Return success
                    console.log(`Reset map for field: ${fieldId}`);
                    resolve(true);
                } catch (e) {
                    console.error("Error in resetMapForField:", e);
                    resolve(false);
                }
            }, 100); // Small delay to ensure map is ready
        } catch (e) {
            console.error("Fatal error in resetMapForField:", e);
            resolve(false);
        }
    });
}