const zIndexHeight = 1;

export async function initEChartsGlobe(dotNetHelper, data) {
    var chartDom = document.getElementById('globe-container');
    if (!chartDom) {
        console.error('Chart container element not found');
        return;
    }
    var myChart = echarts.init(chartDom);
    window.myChart = myChart;

    var processedData = processData(data);

    var option = getChartOption(processedData);

    myChart.setOption(option);

    myChart.on('click', function (params) {
        if (params.seriesType === 'scatter3D') {
            dotNetHelper.invokeMethodAsync('OnChartClick', params.value[4], params.value[3], params.value[0], params.value[1]);
        }
    });
}

export async function updateEChartsData(data) {
    if (!window.myChart) {
        console.error('Chart is not initialized');
        return;
    }

    var processedData = processData(data);
    window.myChart.setOption({
        series: [{
            data: processedData.map((dataItem, index) => {
                const isLastPoint = index > processedData.length - 2;
                return {
                    value: dataItem,
                    itemStyle: {
                        color: getColorForDevice(dataItem[3]),
                        opacity: isLastPoint ? 0 : 1
                    },
                };
            })
        }]
    });
}

export function disposeEChartsGlobe() {
    if (window.myChart) {
        window.myChart.dispose();
        window.myChart = null;
    }
}

function processData(data) {
    data = data.map(dataItem => [
        dataItem.longitude,
        dataItem.latitude,
        zIndexHeight,
        dataItem.deviceId,
        dataItem.orgId
    ]);
    data.push([0, 0, 999, 'fake', 'fake']);
    return data;
}

function getChartOption(processedData) {
    return {
        backgroundColor: '#000',
        tooltip: {
            show: true,
            trigger: 'item',
            formatter: params => `Org ID: ${params.value[4]}<br />Device ID: ${params.value[3]}<br />
                                  Coordinates: (${params.value[1]}, ${params.value[0]})`
        },
        globe: {
            baseTexture: '/asset/world.topo.bathy.200401.jpg',
            heightTexture: '/asset/world.topo.bathy.200401.jpg',
            shading: 'realistic',
            realisticMaterial: {
                roughness: 0.8,
                metalness: 0
            },
            environment: '/asset/starfield.jpg',
            light: {
                main: { intensity: 3 }
            },
            viewControl: {
                autoRotate: true,
                autoRotateSpeed: 5,
                autoRotateAfterStill: 20000,
                distance: 200
            }
        },
        series: [{
            type: 'scatter3D',
            coordinateSystem: 'globe',
            data: processedData.map((dataItem, index) => {
                const isLastPoint = index > processedData.length - 2;
                return {
                    value: dataItem,
                    itemStyle: {
                        color: getColorForDevice(dataItem[3]),
                        opacity: isLastPoint ? 0 : 1
                    },
                };
            }),
            minHeight: 0,
            silent: false,
            symbolSize: 30,
        }]
    };
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

export async function setWidth(value) {
    window.myChart.option.graphic.elements[0].shape.width = value;
    window.myChart.setOption(window.myChart.getOption()); // Apply changes
}

export async function resizeGlobe(value) {
    var globeContainer = document.getElementById('globe-container');
    globeContainer.style.width = value;
    window.myChart.resize();
}
