﻿const ctx = document.getElementById('myChart').getContext('2d');

let myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: [],
        datasets: [{
            label: '',
            data: [],
            backgroundColor: [],
            borderColor: [],
            borderWidth: 1,
            tooltip: []
        }]
    },
    options: {
        plugins: {
            legend: {
                display: false
            }
        },
        animation: {
            onComplete: function () {
                element = document.getElementById('download-chart-image');
                element.href = this.toBase64Image();
                element.download = 'chart image not generated yet.png';
            }
        },
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});


function getChart() {

    let selectedChartApiUrl = document.getElementById("select-chart").value;

    $.get(selectedChartApiUrl,
        function (data) {
            while (myChart.data.labels.length > 0) {
                myChart.data.labels.pop();
            };

            while (myChart.data.datasets[0].data.length > 0) {
                myChart.data.datasets[0].data.pop();
                myChart.data.datasets[0].backgroundColor.pop();
                myChart.data.datasets[0].borderColor.pop();
            }

            for (var i = 0; i < data.chartData.length; i++) {
                myChart.data.labels.push(data.chartData[i].commentsCount);
                myChart.data.datasets[0].data.push(data.chartData[i].commentsCount);
                myChart.data.datasets[0].backgroundColor.push(data.chartData[i].color);
                myChart.data.datasets[0].borderColor.push(data.chartData[i].color);
            }

            myChart.options.animation.onComplete = function () {
                let element = document.getElementById('download-chart-image');
                element.href = this.toBase64Image();
                element.download = data.fileDownLoadName;
                $("#download-chart-image").removeClass("visually-hidden");
            };

            myChart.update();
        });
}


window.onload = function () {
    getChart();
};
