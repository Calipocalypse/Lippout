function loadWaveForm() {
    var markerOffsetFixed = 0;
    var timerSlowDown = 0;

    const wavesurfer = WaveSurfer.create({
        container: '#waveform',
        waveColor: 'lime',
        progressColor: 'red',
        url: '/LIEUT1.wav'
    })

    wavesurfer.on('interaction', () => {
        wavesurfer.play()
    })

    wavesurfer.on('timeupdate', function () {
        if (timerSlowDown > 3) {
            var currentTime = wavesurfer.getCurrentTime();
            var markerOffset = 22100 * 4 * currentTime;
            markerOffsetFixed = markerOffset.toFixed();
            document.getElementById("debugDiv").textContent = markerOffsetFixed;
            updateFrame(markerOffsetFixed);
            timerSlowDown = 0;
        }
        else {
            timerSlowDown++;
        }
    });

    wavesurfer.on('click', function () {
        var currentTime = wavesurfer.getCurrentTime();
        var markerOffset = 22100 * 4 * currentTime;
    });
}