/**
 * Hurudza Farm Management System
 * Chart rendering functions for crop reports and statistics
 */

// Chart instances for reuse and updating
let chartInstances = {};

/**
 * Initialize the charting system
 */
function initCharts() {
    console.log('Chart system initialized');

    // Clean up any existing charts when page changes
    window.addEventListener('beforeunload', function() {
        destroyAllCharts();
    });
}

/**
 * Destroy all chart instances to prevent memory leaks
 */
function destroyAllCharts() {
    for (const key in chartInstances) {
        if (chartInstances[key]) {
            try {
                chartInstances[key].destroy();
            } catch (e) {
                console.warn(`Failed to destroy chart ${key}:`, e);
            }
        }
    }
    chartInstances = {};
}

/**
 * Create or get a canvas element within a container
 * @param {string} containerId - ID of the container element
 * @returns {HTMLCanvasElement|null} - Canvas element or null if container not found
 */
function getOrCreateCanvas(containerId) {
    const container = document.getElementById(containerId);
    if (!container) {
        console.error(`Chart container not found: ${containerId}`);
        return null;
    }

    // Check if container already has a canvas
    let canvas = container.querySelector('canvas');

    // If no canvas exists, create one
    if (!canvas) {
        canvas = document.createElement('canvas');
        container.innerHTML = ''; // Clear container
        container.appendChild(canvas);
        console.log(`Created new canvas in container ${containerId}`);
    }

    return canvas;
}

/**
 * Renders a bar chart showing crop distribution by hectares
 * @param {string[]} labels - Array of crop names
 * @param {number[]} values - Array of hectare values for each crop
 */
function renderCropSummaryChart(labels, values) {
    try {
        console.log('Attempting to render crop summary chart with data:', { labels, values });

        if (!labels || !values || labels.length === 0) {
            console.error('Invalid data for crop summary chart');
            return;
        }

        const canvas = getOrCreateCanvas('cropSummaryChart');
        if (!canvas) return;

        // Destroy previous chart instance if it exists
        if (chartInstances.cropSummary) {
            chartInstances.cropSummary.destroy();
        }

        // Convert values to numbers and ensure they're valid
        const safeValues = values.map(v => Number(v) || 0);

        // Create color array
        const backgroundColors = safeValues.map(() => 'rgba(76, 175, 80, 0.7)');
        const borderColors = safeValues.map(() => 'rgba(76, 175, 80, 1)');

        // Create the chart
        chartInstances.cropSummary = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: 'Hectares',
                    data: safeValues,
                    backgroundColor: backgroundColors,
                    borderColor: borderColors,
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Hectares'
                        }
                    }
                },
                plugins: {
                    legend: {
                        display: false
                    },
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `${context.parsed.y.toFixed(2)} ha`;
                            }
                        }
                    }
                }
            }
        });

        console.log('Crop summary chart rendered successfully');
    } catch (err) {
        console.error('Error rendering crop summary chart:', err);
    }
}

/**
 * Renders a pie chart showing irrigated vs non-irrigated distribution
 * @param {number} irrigatedArea - Total irrigated hectares
 * @param {number} nonIrrigatedArea - Total non-irrigated hectares
 */
function renderIrrigationChart(irrigatedArea, nonIrrigatedArea) {
    try {
        console.log('Attempting to render irrigation chart with data:', { irrigatedArea, nonIrrigatedArea });

        // Try both possible container IDs
        let canvas = getOrCreateCanvas('irrigationChart');
        if (!canvas) {
            canvas = getOrCreateCanvas('irrigationSummaryChart');
            if (!canvas) {
                console.error('Could not find irrigation chart container');
                return;
            }
        }

        // Destroy previous chart instance if it exists
        if (chartInstances.irrigation) {
            chartInstances.irrigation.destroy();
        }

        // Convert to proper numbers and ensure they're not zero
        const safeIrrigated = Number(irrigatedArea) || 0.01;
        const safeNonIrrigated = Number(nonIrrigatedArea) || 0.01;

        // Create the chart
        chartInstances.irrigation = new Chart(canvas, {
            type: 'pie',
            data: {
                labels: ['Irrigated', 'Non-Irrigated'],
                datasets: [{
                    data: [safeIrrigated, safeNonIrrigated],
                    backgroundColor: [
                        'rgba(76, 175, 80, 0.7)',
                        'rgba(245, 124, 0, 0.7)'
                    ],
                    borderColor: [
                        'rgba(76, 175, 80, 1)',
                        'rgba(245, 124, 0, 1)'
                    ],
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                const total = safeIrrigated + safeNonIrrigated;
                                const percentage = ((context.parsed / total) * 100).toFixed(1);
                                return `${context.label}: ${context.parsed.toFixed(2)} ha (${percentage}%)`;
                            }
                        }
                    }
                }
            }
        });

        console.log('Irrigation chart rendered successfully');
    } catch (err) {
        console.error('Error rendering irrigation chart:', err);
    }
}

/**
 * Renders a bar chart showing crop distribution by farm
 * @param {string[]} farms - Array of farm names
 * @param {number[]} hectares - Array of hectare values for each farm
 */
function renderCropDistributionChart(farms, hectares) {
    try {
        console.log('Attempting to render crop distribution chart with data:', { farms, hectares });

        if (!farms || !hectares || farms.length === 0) {
            console.error('Invalid data for crop distribution chart');
            return;
        }

        const canvas = getOrCreateCanvas('cropDistributionChart');
        if (!canvas) return;

        // Destroy previous chart instance if it exists
        if (chartInstances.cropDistribution) {
            chartInstances.cropDistribution.destroy();
        }

        // Convert values to numbers and ensure they're valid
        const safeHectares = hectares.map(v => Number(v) || 0);

        // Create the chart
        chartInstances.cropDistribution = new Chart(canvas, {
            type: 'bar',
            data: {
                labels: farms,
                datasets: [{
                    label: 'Hectares',
                    data: safeHectares,
                    backgroundColor: 'rgba(54, 162, 235, 0.7)',
                    borderColor: 'rgba(54, 162, 235, 1)',
                    borderWidth: 1
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true,
                        title: {
                            display: true,
                            text: 'Hectares'
                        }
                    }
                },
                plugins: {
                    tooltip: {
                        callbacks: {
                            label: function(context) {
                                return `${context.parsed.y.toFixed(2)} ha`;
                            }
                        }
                    }
                }
            }
        });

        console.log('Crop distribution chart rendered successfully');
    } catch (err) {
        console.error('Error rendering crop distribution chart:', err);
    }
}

// Initialize the chart system when the script loads
initCharts();