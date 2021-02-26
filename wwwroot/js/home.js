
function createOverviewChart(ctx, labels, values) {
    var chart = new Chart(ctx, {
        type: 'pie',
        data: {
            datasets: [{
                data: values,
                backgroundColor: [
                    'red',
                    'blue',
                    'green',
                    'black',
                    'grey',
                    'orange',
                    'purple'
                ],
            }],

            labels: labels
        },
        options: {
            legend: {
                position: 'right'
            },
            plugins: {
                labels: {
                    render: 'value',
                    fontSize: 20,
                    fontColor: 'white'
                }
            }
        }
    });
}

