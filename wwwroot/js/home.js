
function createLineChart(ctx, labels, values, name) {
    var chart = new Chart(ctx, {
        type: 'line',

        data: {
            labels: labels,
            datasets: [{
                label: name,
                backgroundColor: '' + getRandomColor(),
                borderColor: '' + getRandomColor(),
                data: values
            }]
        },

        options: {
            legend: {
                display: false,
            }
        }
    });
}


function createHorizontalBarChart(ctx, labels, values, name) {

    var chart = new Chart(ctx, {
        type: 'horizontalBar',

        data: {
            labels: labels,
            datasets: [{
                label: 'Tags',
                backgroundColor: '' + getRandomColor(),
                borderColor: '' + getRandomColor(),
                data: values,
            }]
        },
        options: {
            legend: {
                display: false,
            }
        }
    });
}

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

