var theChart = null;

var options = {
    // IMPORTANT - You need a chart of type "scatter"
    // in order to drag a line chart along the x axis
    type: 'scatter',
    data: {
        "datasets": [
            {
                "label": "A dataset",
                "data": [
                    {
                        "x": 0,
                        "y": 5
                    },
                    {
                        "x": 512788,
                        "y": 1
                    }
                ],
                "backgroundColor": "rgba(255, 99, 132, 1)",
                borderWidth: 5.5,
                fill: false,
                pointRadius: 10,
                pointHitRadius: 25,
                showLine: true
            }
        ]
    },
    options: {
        layout: {
            padding: {
                top: 20,
                bottom: 10
            }
        },
        scales: {
            x: {
                grid: {
                    color: '#00000000'
                },
                min: 0,
                max: 20000700
            },
            y: {
                beginAtZero: true,
                grid: {
                    color: '#ff0000'
                },
                min: 0,
                max: 8
            }
        },
        plugins: {
            dragData: {
                round: 0,
                showTooltip: true,
                dragX: true
            }
        }
    }
}

function loadMarkersUi() {
    console.log(document.getElementById('markersContainer'));
    var ctx = document.getElementById('markersContainer').getContext('2d');
    theChart = new Chart(ctx, options);
    theChart.canvas.parentNode.style.height = '512px';
    theChart.canvas.parentNode.style.width = '100%';
}

function loadNewLipData() {
    try {

        let newdata = [
            {
                "label": "A dataset",
                "data": [],
                "backgroundColor": "rgba(255, 99, 132, 1)",
                "borderWidth": 5.5,
                "fill": false,
                "pointRadius": 10,
                "pointHitRadius": 25,
                "showLine": true
            }];


        theChart.data.datasets = newdata;
        theChart.update();
    }
    catch (error) {
        alert(error);
    }
}

function getMarkersPhonemData() {
    var data = JSON.parse(JSON.stringify(theChart.data.datasets[0].data));

    data.forEach(point => {
        point.marker = point.x;
        delete point.x;
        point.phonem = point.y;
        delete point.y;
    });
    var toReturn = JSON.stringify(data);
    return toReturn;
}

function getPointsFromChart() {
    var data = JSON.parse(JSON.stringify(theChart.data.datasets[0].data));
    var toReturn = JSON.stringify(data);
    return toReturn;
}

function putNewMarker(markerPos, phonemPos) {
    newData2 = { "x": markerPos, "y": phonemPos };

    theChart.data.datasets.forEach((dataset) => {
        dataset.data.push(newData2);
    });
    theChart.update();
}

function loadLipMarkers(phonomarkers, max_x) {
    var phonomarkers = JSON.parse(phonomarkers);
    max_x = Math.floor(max_x);
    options.options.scales.x.max = max_x;
    theChart.clear();
    loadNewLipData();
    phonomarkers.sort(function (a, b) {
        return a.markerPos - b.markerPos;
    });
    phonomarkers.forEach((phonomarker) => {
        newData3 = { "x": phonomarker.markerPos, "y": phonomarker.phonemPos };
        theChart.data.datasets[0].data.push(newData3);
    })
    theChart.update();
}
