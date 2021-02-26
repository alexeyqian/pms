
function getRandomColor() {
    var letters = '0123456789ABCDEF';
    var color = '#';
    for (var i = 0; i < 6; i++) {
        color += letters[Math.floor(Math.random() * 16)];
    }
    return color;
}

function getRandomColors(length) {
    var colors = [];
    for (var i = 0; i < length; i++)
        colors[i] = getRandomColor();

    return colors;
}

function createPieChart(ctx, labels, values, name) {
    var chart = new Chart(ctx, {
        type: 'pie',
        data: {
            datasets: [{
                data: values,
                backgroundColor: getRandomColors(values.length)
            }],

            labels: labels,
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
