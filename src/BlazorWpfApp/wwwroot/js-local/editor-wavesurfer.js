let editorWavesurfer;

async function loadWaveForm() {
    var markerOffsetFixed = 0;
    var timerSlowDown = 0;

    const WaveSurfer = (await import('https://unpkg.com/wavesurfer.js@7/dist/wavesurfer.esm.js')).default;

        editorWavesurfer = WaveSurfer.create({
        container: '#waveform',
        waveColor: 'lime',
        progressColor: 'red'
    })

    editorWavesurfer.on('interaction', () => {
        editorWavesurfer.play()
    })

    editorWavesurfer.on('timeupdate', function () {
        if (timerSlowDown > 3) {
            var currentTime = editorWavesurfer.getCurrentTime();
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

    editorWavesurfer.on('click', function () {
        var currentTime = editorWavesurfer.getCurrentTime();
        var markerOffset = 22100 * 4 * currentTime;
    });
}