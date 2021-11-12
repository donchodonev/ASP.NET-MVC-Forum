const ctx = document.getElementById('myChart').getContext('2d');
let ulNode = document.getElementById("itemslist");

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
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});

function getChart() {
    let selectedChartApiUrl = document.getElementById("select-chart").value;

    clearUlChildren();
    $("#download-chart-image").addClass("visually-hidden");
    $.get(selectedChartApiUrl,
        function (data) {
            while (myChart.data.labels.length > 0) {
                myChart.data.labels.pop();
            };

            clearChartData(myChart.data.datasets);

            fillChart(myChart.data, data.chartData);

            myChart.options.animation.onComplete = function () {
                let element = document.getElementById('download-chart-image');
                element.href = this.toBase64Image();
                element.download = data.fileDownLoadName;
                $("#download-chart-image").removeClass("visually-hidden");
            };

            myChart.update();

            generateHtmlDynamically(data.chartData, ulNode);

        });
}

window.onload = function () {
    getChart();
};

let fillChart = function (chartData, newData) {
    for (var i = 0; i < newData.length; i++) {
        chartData.labels.push(newData[i].count);
        chartData.datasets[0].data.push(newData[i].count);
        chartData.datasets[0].backgroundColor.push(newData[i].color);
        chartData.datasets[0].borderColor.push(newData[i].color);
    }
}

let clearChartData = function (chartDataDatasets) {
    while (chartDataDatasets[0].data.length > 0) {
        chartDataDatasets[0].data.pop();
        chartDataDatasets[0].backgroundColor.pop();
        chartDataDatasets[0].borderColor.pop();
    }
}

function insertAfter(referenceNode, newNode) {
    referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling);
}

function clearUlChildren() {
    while (ulNode.hasChildNodes) {

        if (ulNode.firstChild == null) {
            break;
        }

        ulNode.firstChild.remove();
    };
}

function generateHtmlDynamically(chartData, ulRootElement) {
    for (var i = 0; i < chartData.length; i++) {
        let listNode = document.createElement("li");
        listNode.className = 'chart-list-item list-group-item d-flex justify-content-between align-items-center m-2';
        listNode.style.backgroundColor = chartData[i].color;

        let a = document.createElement("a");
        a.href = "Posts/ViewPost?postId=" + chartData[i].id;
        a.textContent = chartData[i].title;
        a.className = 'text-decoration-none text-white';

        let span = document.createElement("span")
        span.className = "badge badge-primary badge-pill";
        span.textContent = chartData[i].count;

        listNode.append(a);
        listNode.append(span);
        ulRootElement.append(listNode);
    }
}