export async function initECharts3D(measurement, data) {
    var chartDom = document.getElementById('chart-container');
    var myChart = echarts.init(chartDom);
    window.myChart = myChart;
    window.currentMeasurement = measurement; // Store current measurement globally

    function processData(data, measurement) {
        return data.map(function (dataItem) {
            return [
                dataItem.longitude,
                dataItem.latitude,
                parseFloat(dataItem.measurements[measurement]).toFixed(2),
                dataItem.deviceId
            ];
        }).concat([[0, 0, 1, '']]);
    }

    var processedData = processData(data, measurement);

    var option = {
        backgroundColor: '#000',
        tooltip: {
            show: true,
            formatter: function (params) {
                return 'Device ID: ' + params.value[3] + '<br />' +
                    'Coordinates: (' + params.value[1] + ', ' + params.value[0] + ')<br />' +
                    window.currentMeasurement + ': ' + params.value[2]; // Use global measurement
            }
        },
        globe: {
            baseTexture: '/asset/world.topo.bathy.200401.jpg',
            heightTexture: '/asset/world.topo.bathy.200401.jpg',
            shading: 'lambert',
            environment: '/asset/starfield.jpg',
            light: {
                main: {
                    intensity: 2
                }
            },
            viewControl: {
                autoRotate: false,
                autoRotateSpeed: 10,
                autoRotateAfterStill: 5,
                distance: 150
            }
        },
        series: [
            {
                type: 'bar3D',
                coordinateSystem: 'globe',
                data: processedData.map(function (dataItem) {
                    return {
                        value: dataItem,
                        itemStyle: {
                            color: getColorForDevice(dataItem[3])
                        },
                        emphasis: {
                            itemStyle: {
                                color: getColorForDevice(dataItem[3])
                            }
                        }
                    };
                }),
                barSize: 0.6,
                minHeight: 0.2,
                silent: false,
                label: {
                    show: true,
                    formatter: function (params) {
                        return params.value[2];
                    },
                    position: 'top',
                    distance: 10,
                    textStyle: {
                        color: '#fff',
                        fontSize: 12
                    }
                }
            }
        ]
    };

    myChart.setOption(option);

    // Add click event listener
    myChart.on('click', function (params) {
        alert('Device ID: ' + params.value[3] + '\nCoordinates: (' + params.value[1] + ', ' + params.value[0] + ')\n' + window.currentMeasurement + ': ' + params.value[2]); // Use global measurement
    });
}

export async function updateEChartsData(measurement, data) {
    window.currentMeasurement = measurement; // Update global measurement

    function processData(data, measurement) {
        return data.map(function (dataItem) {
            return [
                dataItem.longitude,
                dataItem.latitude,
                parseFloat(dataItem.measurements[measurement]).toFixed(2),
                dataItem.deviceId
            ];
        }).concat([[0, 0, 1, '']]);
    }

    var processedData = processData(data, measurement);

    window.myChart.setOption({
        series: [{
            data: processedData.map(function (dataItem) {
                return {
                    value: dataItem,
                    itemStyle: {
                        color: getColorForDevice(dataItem[3])
                    }
                };
            })
        }]
    });
}

function getColorForDevice(deviceId) {
    if (!window.colorMap) {
        window.colorMap = {};
    }

    if (!window.colorMap[deviceId]) {
        window.colorMap[deviceId] = echarts.color.modifyHSL('#ff4500', Math.random() * 360);
    }

    return window.colorMap[deviceId];
}
