import {echartsTheme} from "./config/echarts-theme.js";
export async function renderEChart(elementId, dataJson, measurementProperty) {
    var data = JSON.parse(dataJson);
    data = data.sort((a, b) => (new Date(a.x)) - (new Date(b.x)));
    echarts.registerTheme('wl-dark', echartsTheme);
    const container = document.getElementById(elementId);
    const chart = echarts.init(container, 'wl-dark');

    const option = {
        title: {
            text: `${measurementProperty}`,
            textStyle: {
                color: '#ffffff'
            }
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
            type: "time",
            boundaryGap: false,
            axisLabel: {
                formatter: function (value, index) {
                    const date = new Date(value);
                    const day = date.getDate();
                    const month = date.toLocaleString('default', { month: 'short' });
                    // Only display month name for the first day of the month
                    if (day === 1) {
                        return `${month}`;
                    }  else if(day === 29) {
                        return '';
                    }else {
                        return `${day}`;
                    }
                },
                align: 'right',
            },
            splitLine: {
                show: false // Optional: Hide the grid lines for a cleaner look
            }
        },
        yAxis: {
            type: 'value'
        },
        series: [{
            name: measurementProperty,
            data: data.map(item => [item.x, item.y]),
            type: 'line',
        }],
        dataZoom: [
            {
                type: "slider",
                start: 0,
                end: 100,
            },
            {
                type: "inside",
                start: 0,
                end: 100,
            },
        ],
    };

    chart.setOption(option);
    
    var lastWidth = container.offsetWidth;
    const resize = () => {
        if(chart) chart.resize();
        if(container.offsetWidth !== lastWidth) requestAnimationFrame(resize);
        lastWidth = container.offsetWidth;
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
