const ctx = document.getElementById('myChart').getContext('2d');

function insertAfter(referenceNode, newNode) {
    referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling);
}

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

            let ulNode = document.getElementById("itemslist");

            for (var i = 0; i < data.chartData.length; i++) {
                let listNode = document.createElement("li");
                listNode.className = 'chart-list-item list-group-item d-flex justify-content-between align-items-center m-2';
                listNode.style.backgroundColor = data.chartData[i].color;

                let a = document.createElement("a");
                a.href = "Posts/ViewPost?postId=" + data.chartData[i].id;
                a.textContent = data.chartData[i].title;
                a.className = 'text-decoration-none text-white';

                let span = document.createElement("span")
                span.className = "badge badge-primary badge-pill";
                span.textContent = data.chartData[i].commentsCount;

                listNode.append(a);
                listNode.append(span);
                ulNode.append(listNode);
            }
        });
}


window.onload = function () {
    getChart();
};


