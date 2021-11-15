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

    clearUlChildren(); // removes all ul childred (<li>) which show additional post data (post title with anchor tag and count span pill)

    $.get(selectedChartApiUrl,
        function (data) {

            clearChartData(myChart.data);

            fillChart(myChart.data, data.chartData);

            $("#download-chart-image").addClass("disabled"); //hide chart image download button

            myChart.options.animation.onComplete = function () {
                let element = document.getElementById('download-chart-image');
                element.href = this.toBase64Image();
                element.download = data.fileDownLoadName;

                $("#download-chart-image").removeClass("disabled"); //show chart image download button
            };

            myChart.update();

            generateHtmlDynamically(data.chartData, ulNode);
        });
}

window.onload = function () {
    getChart();
};

function fillChart (chartData, newData) {
    for (var i = 0; i < newData.length; i++) {
        chartData.labels.push(newData[i].count);
        chartData.datasets[0].data.push(newData[i].count);
        chartData.datasets[0].backgroundColor.push(newData[i].color);
        chartData.datasets[0].borderColor.push(newData[i].color);
    }
}

function clearChartData(chartData) {
    while (chartData.labels.length > 0) {
        chartData.labels.pop();
    };

    while (chartData.datasets[0].data.length > 0) {
        chartData.datasets[0].data.pop();
        chartData.datasets[0].backgroundColor.pop();
        chartData.datasets[0].borderColor.pop();
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
        a.setAttribute("display", "block");
        a.style = "width: 100%";

        let span = document.createElement("span")
        span.className = "badge badge-primary badge-pill";
        span.textContent = chartData[i].count;

        listNode.append(a);
        listNode.append(span);
        ulRootElement.append(listNode);
    }
}

