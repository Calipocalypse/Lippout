//switcher
var idDivImages = ["frame0", "frame1", "frame2", "frame3", "frame4", "frame5", "frame6", "frame7", "frame8"];
var displayedFrame = 0;

function onStartDivImages() {
    try {
        idDivImages.forEach(x => document.getElementById(x).style.display = 'none');
        document.getElementById("frame0").style.display = 'block';
    }
    catch (error) {
        alert(error);
    }
}

function updateFrame(time) {
    var data = JSON.parse(JSON.stringify(theChart.data.datasets[0].data));
    let foundElement = null;

    for (const element of data) {
        if (element.x > time) {
            foundElement = element;
            break;
        }
    }

    if (foundElement != null) {
        document.getElementById("phonemFrame").textContent = JSON.stringify(foundElement);
        changeFrame(Math.floor(foundElement.y));
    }

}

function changeFrame(newFrame) {
    try {
        if (displayedFrame != newFrame) {
            displayedFrame = newFrame;
            idDivImages.forEach(x => document.getElementById(x).style.display = 'none');
            document.getElementById(idDivImages[newFrame]).style.display = 'block';
        }
    }
    catch (error) {
        alert(error);
    }
}