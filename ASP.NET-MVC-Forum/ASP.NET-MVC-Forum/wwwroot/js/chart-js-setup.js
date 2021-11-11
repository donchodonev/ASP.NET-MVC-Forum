const ctx = document.getElementById('myChart').getContext('2d');
const myChart = new Chart(ctx, {
    type: 'bar',
    data: {
        labels: ['Red', 'Blue', 'Yellow', 'Green', 'Purple', 'Orange', 'Orange'],
        datasets: [{
            label: '# of Comments',
            data: [12, 19, 3, 5, 2, 3, 44],
            backgroundColor: [
                'red',
                'green',
                'blue',
                'yellow',
                'purple',
                'orange',
                'brown',
            ],
            borderColor: [
                'brown',
                'orange',
                'purple',
                'yellow',
                'blue',
                'green',
                'red',
            ],
            borderWidth: 1
        }]
    },
    options: {
        animation: {
            onComplete: function () {
                element = document.getElementById('download-chart-image');
                element.href = this.toBase64Image();
                element.download = 'chart.png';
            }
        },
        scales: {
            y: {
                beginAtZero: true
            }
        }
    }
});


