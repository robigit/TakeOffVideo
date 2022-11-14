
export function caricavideo(video, href) {

    try {
    video.src = href;
    }
    catch(err) {
        dotnetHelper.invokeMethodAsync("ReportJS", `caricavideo eccezione: ${err}`);
    }
}


export function move(direction, container, vLine, movement) {



    if (container == null || vLine == null) {
        console.error("move not found");
    }


    let lineWidth = parseInt(getComputedStyle(vLine)['width'])
    let marginLeftVline = parseInt(getComputedStyle(vLine)['margin-left']);
    let containerWidth = parseInt(getComputedStyle(container)['width']);

    //console.info(lineWidth);
    //console.info(marginLeftVline);
    //console.info(containerWidth);


    switch (direction) {
        case 'sx':
            if (Math.abs(marginLeftVline) <= containerWidth / 2 - lineWidth - movement || marginLeftVline > 0) {
                vLine.style['margin-left'] = marginLeftVline - movement + "px";

            }
            break;
        case 'dx':
            if (Math.abs(marginLeftVline) <= containerWidth / 2 - movement || marginLeftVline < 0) {
                vLine.style['margin-left'] = marginLeftVline + movement + "px";
            }
            break;
    }
}


export function startVideo(video, id) {
    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {

        //console.info("id " + id);

        navigator.mediaDevices.getUserMedia({ video: { deviceId: { exact : id } } }).then(function (stream) {
            
            //if ("srcObject" in video) {
                video.srcObject = stream;
            //} else {
            //    video.src = window.URL.createObjectURL(stream);
            //}


            //video.onloadedmetadata = function (e) {
            //    video.play();
            //};
            //mirror image
            //video.style.webkitTransform = "scaleX(-1)";
            //video.style.transform = "scaleX(-1)";
        });
    }
}


let dotnetHelper = null;
let media_recorder = null;
let blobs_recorded = [];

export function StartRec(video, stopbutton, idcamera, dotHelper) {

    dotnetHelper = dotHelper

    let options;
    let tipo;

    //if (MediaRecorder.isTypeSupported('video/webm; codecs=vp9')) {
    //    options = { mimeType: 'video/webm; codecs=vp9' };
    //    tipo = "webm";
    //} else
    if (MediaRecorder.isTypeSupported('video/webm')) {
        options = { mimeType: 'video/webm' };
        tipo = "webm";
    } else if (MediaRecorder.isTypeSupported('video/mp4')) {
        options = { mimeType: 'video/mp4', videoBitsPerSecond: 100000 };
        tipo = "mp4";
    } else {
        dotnetHelper.invokeMethodAsync("ReportJS", "no suitable mimetype found for this device");
        return;
    }


    dotnetHelper.invokeMethodAsync("ReportJS", "tipo3 " + tipo);

    navigator.mediaDevices.getUserMedia({ video: { deviceId: idcamera } })
        .then(function (stream) {

            //dotnetHelper.invokeMethodAsync("ReportJS", "StartRec2");

            //if ("srcObject" in video) {
                video.srcObject = stream;
            //} else {
            //    video.src = window.URL.createObjectURL(stream);
            //}


            video.onloadedmetadata = function (e) {
                dotnetHelper.invokeMethodAsync("ReportJS", "Play");
                video.play();
            };
            //mirror image
            //video.style.webkitTransform = "scaleX(-1)";
            //video.style.transform = "scaleX(-1)";

            blobs_recorded = [];

            // set MIME type of recording as video/webm
            media_recorder = new MediaRecorder(stream, options );

            
            // event : new recorded video blob available 
            media_recorder.addEventListener('dataavailable', function (e) {
                
                blobs_recorded.push(e.data);
            });

            // event : recording stopped & all blobs sent
            media_recorder.addEventListener('stop', function () {

                dotnetHelper.invokeMethodAsync("ReportJS", "media stop");

                var optionblob = { type: `video/${tipo}` };

                // create local object URL from the recorded video blobs
                let video_local = URL.createObjectURL(new Blob(blobs_recorded, optionblob));

                dotnetHelper.invokeMethodAsync("SalvaUrlVideo", video_local, tipo);

                //??
                //media_recorder.removeEventListener('stop', arguments.callee);
            });

            
            stopbutton.addEventListener('click', function onst() {

                dotnetHelper.invokeMethodAsync("ReportJS", "click stop");

                media_recorder.stop();

                this.removeEventListener('click', onst);
            });

            // start recording with each recorded blob having 1 second video
            media_recorder.start(1000);
        }).catch((err) => {
            dotnetHelper.invokeMethodAsync("ReportJS", `The following error occurred: ${err}`);
        });

}

export function pressbutton(button) {
    button.click(); 
}

export function SettaControlsVideo(video, setta) {

    if (setta) {
        video.setAttribute("controls", "controls")
    } else {
        video.removeAttribute("controls")
    }

}