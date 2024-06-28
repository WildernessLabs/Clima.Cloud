export async function initEChartsGlobe(dotNetHelper, data) {
    var zIndexHeight = 1;
    var chartDom = document.getElementById('globe-container');
    if (!chartDom) {
        console.error('Chart container element not found');
        return;
    }
    var myChart = echarts.init(chartDom);
    window.myChart = myChart;

    var processedData = processData(data, zIndexHeight);

    var option = getChartOption(processedData);

    myChart.setOption(option);

    myChart.on('click', function (params) {
        dotNetHelper.invokeMethodAsync('OnChartClick', params.value[4], params.value[3], params.value[0], params.value[1]);
    });
}

export async function updateEChartsData(data) {
    var zIndexHeight = 5;
    if (!window.myChart) {
        console.error('Chart is not initialized');
        return;
    }

    var processedData = processData(data, zIndexHeight);

    window.myChart.setOption({
        series: [{
            data: processedData.map(dataItem => ({
                value: dataItem,
                itemStyle: { color: getColorForDevice(dataItem[3]) }
            }))
        }]
    });
}

export function disposeEChartsGlobe() {
    if (window.myChart) {
        window.myChart.dispose();
        window.myChart = null;
    }
}

function processData(data, zIndexHeight) {
    return data.map(dataItem => [
        dataItem.longitude,
        dataItem.latitude,
        zIndexHeight,
        dataItem.deviceId,
        dataItem.orgId
    ]);
}

function getChartOption(processedData) {
    return {
        backgroundColor: '#000',
        tooltip: {
            show: true,
            formatter: params => `Org ID: ${params.value[4]}<br /> Device ID: ${params.value[3]}<br />
                                  Coordinates: (${params.value[1]}, ${params.value[0]})`
        },
        globe: {
            baseTexture: '/asset/world.topo.bathy.200401.jpg',
            heightTexture: '/asset/world.topo.bathy.200401.jpg',
            shading: 'lambert',
            environment: '/asset/starfield.jpg',
            light: {
                main: { intensity: 2 }
            },
            viewControl: {
                autoRotate: true,
                autoRotateSpeed: 5,
                autoRotateAfterStill: 15,
                distance: 200
            }
        },
        series: [{
            type: 'bar3D',
            coordinateSystem: 'globe',
            data: processedData.map(dataItem => ({
                value: dataItem,
                itemStyle: { color: getColorForDevice(dataItem[3]) },
                emphasis: { itemStyle: { color: getColorForDevice(dataItem[3]) } }
            })),
            barSize: 1,
            minHeight: 0.2,
            silent: false,
            //label: {
            //    show: true,
            //    formatter: params => params.value[2],
            //    position: 'top',
            //    distance: 10,
            //    textStyle: {
            //        color: '#fff',
            //        fontSize: 12
            //    }
            //}
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
    myChart.setOption(option);
}

export async function resizeGlobe(value) {
    var globeContainer = document.getElementById('globe-container');
    // Example of changing the width dynamically
    globeContainer.style.width = value;
    //globeContainer.style.height = '600px'; // Adjust height if necessary

    // Resize the ECharts instance
    window.myChart.resize();
}