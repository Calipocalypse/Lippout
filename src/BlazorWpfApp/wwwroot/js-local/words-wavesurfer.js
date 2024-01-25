let WaveSurfer;
let RegionsPlugin;
let wsRegions;
let wavesurfer;

async function loadWordsWaveForm()
{
    WaveSurfer = (await import('https://unpkg.com/wavesurfer.js@7/dist/wavesurfer.esm.js')).default;
    RegionsPlugin = (await import('https://unpkg.com/wavesurfer.js@7/dist/plugins/regions.esm.js')).default;

    wavesurfer = WaveSurfer.create({
        container: '#wordsWaveForm',
        waveColor: 'white',
        progressColor: 'white'
    })

    wsRegions = wavesurfer.registerPlugin(RegionsPlugin.create());

    wavesurfer.on('interaction', () => {
        wavesurfer.play()
    })

    let loop = true;
    let activeRegion = null
    wsRegions.on('region-in', (region) => {
        console.log('region-in', region)
        activeRegion = region
    })
    wsRegions.on('region-out', (region) => {
        console.log('region-out', region)
        if (activeRegion === region) {
            if (loop) {
                region.play()
            } else {
                activeRegion = null
            }
        }
    })
    wsRegions.on('region-clicked', (region, e) => {
        e.stopPropagation() // prevent triggering a click on the waveform
        activeRegion = region
        region.play()
        region.setOptions({ color: randomColor() })
    })
    // Reset the active region when the user clicks anywhere in the waveform
    wavesurfer.on('interaction', () => {
        activeRegion = null
    })
}


function createGeneratedRegions(newRegions) {
    wsRegions.getRegions().forEach(region => region.remove());
    var parsedRegions = JSON.parse(newRegions);
    parsedRegions.forEach(region => {
        const random = (min, max) => Math.random() * (max - min) + min;
        const randomColor = () => `rgba(${random(0, 255)}, ${random(0, 255)}, ${random(0, 255)}, 0.5)`;
        wsRegions.addRegion({
            start: region.Start,
            end: region.End,
            content: region.Word,
            color: randomColor(),
            minLength: 0.1,
            maxLength: 10,
        })
    })


}

function getCurrentRegions() {
    var toReturn = [];

    wsRegions.getRegions().forEach(region => {
        let rg = {
            Start: region.start,
            End: region.end,
            Word: ''
        };
        toReturn.push(rg);
    })
    return JSON.stringify(toReturn);
}

function loadBlobToWords(blob) {
    var parsedBlob = JSON.parse(blob);
    var decodedData = base64ToArrayBuffer(parsedBlob.Base64);

    // Utwórz BLOB z zdekodowanych danych
    var blob = new Blob([decodedData], { type: 'audio/wav' });

    // Wczytaj BLOB do wavesurfer
    wavesurfer.loadBlob(blob);
    editorWavesurfer.loadBlob(blob);
}

function base64ToArrayBuffer(base64) {
    var binaryString = window.atob(base64);
    var len = binaryString.length;
    var bytes = new Uint8Array(len);
    for (var i = 0; i < len; i++) {
        bytes[i] = binaryString.charCodeAt(i);
    }
    return bytes;
}