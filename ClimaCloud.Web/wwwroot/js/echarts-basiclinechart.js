import {echartsTheme} from "./config/echarts-theme.js";

export async function renderEChart(elementId, dataJson, measurementProperty) {
    const data = JSON.parse(dataJson);
    console.log('h')
    echarts.registerTheme('wl-dark', echartsTheme);
    const chart = echarts.init(document.getElementById(elementId), 'wl-dark');

    const option = {
        title: {
            text: `${measurementProperty}`
        },
        tooltip: {
            trigger: 'axis',
            axisPointer: {
                type: 'cross'
            },
            formatter: function (params) {
                const date = new Date(params[0].name);
                const formattedDate = formatDate(date);
                return `${formattedDate}<br/>${params[0].marker}${params[0].seriesName}: ${params[0].data[1]}`;
            }
        },
        grid: {
            bottom: 80, // Increase bottom padding to accommodate rotated labels
            left: 60 // Increase left padding for better spacing
        },
        xAxis: {
            type: 'category',
            data: data.map(item => item.x),
            axisLabel: {
                formatter: function (value) {
                    return formatDate(new Date(value));
                },
                rotate: 45, // Rotate labels to avoid cutting off
                padding: [10, 0, 0, 0], // Add padding to prevent cutoff
                margin: 20 // Increase margin between labels and axis
            }
        },
        yAxis: {
            type: 'value'
        },
        series: [{
            name: measurementProperty,
            data: data.map(item => [item.x, item.y]),
            type: 'line'
        }]
    };

    chart.setOption(option);
    
    const resize = () => {
        if(chart) chart.resize();
        requestAnimationFrame(resize);
        console.log('h');
    }
    requestAnimationFrame(resize);
    
}

function formatDate(date) {
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    const year = date.getFullYear();
    const hours = String(date.getHours()).padStart(2, '0');
    const minutes = String(date.getMinutes()).padStart(2, '0');
    const seconds = String(date.getSeconds()).padStart(2, '0');
    return `${month}/${day}/${year} ${hours}:${minutes}:${seconds}`;
}
