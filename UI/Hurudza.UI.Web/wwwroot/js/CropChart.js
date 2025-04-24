// Function to render the crop distribution chart
window.renderCropDistributionChart = function (farmNames, farmSizes) {
    // Check if the chart container exists
    const chartContainer = document.getElementById('cropDistributionChart');
    if (!chartContainer) return;

    // Clear any existing chart
    chartContainer.innerHTML = '';

    // Create the chart using Chart.js or any other library you have available
    // This example uses Chart.js
    const ctx = document.createElement('canvas');
    chartContainer.appendChild(ctx);

    // Ensure Chart.js is available
    if (typeof Chart === 'undefined') {
        console.error('Chart.js is not loaded');
        return;
    }

    new Chart(ctx, {
        type: 'bar',
        data: {
            labels: farmNames,
            datasets: [{
                label: 'Hectares',
                data: farmSizes,
                backgroundColor: 'rgba(66, 135, 245, 0.7)',
                borderColor: 'rgba(66, 135, 245, 1)',
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
                },
                x: {
                    title: {
                        display: true,
                        text: 'Farm'
                    }
                }
            }
        }
    });
};

// Function to render the irrigation chart
window.renderIrrigationChart = function (irrigatedSize, nonIrrigatedSize) {
    // Check if the chart container exists
    const chartContainer = document.getElementById('irrigationChart');
    if (!chartContainer) return;

    // Clear any existing chart
    chartContainer.innerHTML = '';

    // Create the chart using Chart.js or any other library you have available
    const ctx = document.createElement('canvas');
    chartContainer.appendChild(ctx);

    // Ensure Chart.js is available
    if (typeof Chart === 'undefined') {
        console.error('Chart.js is not loaded');
        return;
    }

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: ['Irrigated', 'Non-Irrigated'],
            datasets: [{
                data: [irrigatedSize, nonIrrigatedSize],
                backgroundColor: [
                    'rgba(75, 192, 192, 0.7)',
                    'rgba(201, 203, 207, 0.7)'
                ],
                borderColor: [
                    'rgba(75, 192, 192, 1)',
                    'rgba(201, 203, 207, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'bottom'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.label || '';
                            let value = context.raw || 0;
                            let sum = context.dataset.data.reduce((a, b) => a + b, 0);
                            let percentage = ((value * 100) / sum).toFixed(1) + '%';
                            return `${label}: ${value.toFixed(2)} Ha (${percentage})`;
                        }
                    }
                }
            }
        }
    });
};

// Function to create a summary chart showing crop distribution across all farms
window.renderCropSummaryChart = function(cropNames, cropAreas) {
    // Check if the chart container exists
    const chartContainer = document.getElementById('cropSummaryChart');
    if (!chartContainer) return;

    // Clear any existing chart
    chartContainer.innerHTML = '';

    // Create the chart using Chart.js
    const ctx = document.createElement('canvas');
    chartContainer.appendChild(ctx);

    // Ensure Chart.js is available
    if (typeof Chart === 'undefined') {
        console.error('Chart.js is not loaded');
        return;
    }

    // Generate colors for each crop
    const generateColors = (count) => {
        const colors = [];
        for (let i = 0; i < count; i++) {
            const hue = (i * 137) % 360; // Use golden angle approximation for good distribution
            colors.push(`hsla(${hue}, 70%, 60%, 0.7)`);
        }
        return colors;
    };

    const backgroundColor = generateColors(cropNames.length);
    const borderColor = backgroundColor.map(color => color.replace('0.7', '1'));

    new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: cropNames,
            datasets: [{
                data: cropAreas,
                backgroundColor: backgroundColor,
                borderColor: borderColor,
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                },
                tooltip: {
                    callbacks: {
                        label: function(context) {
                            let label = context.label || '';
                            let value = context.raw || 0;
                            let sum = context.dataset.data.reduce((a, b) => a + b, 0);
                            let percentage = ((value * 100) / sum).toFixed(1) + '%';
                            return `${label}: ${value.toFixed(2)} Ha (${percentage})`;
                        }
                    }
                }
            }
        }
    });
};