﻿
export function caricavideo(video, href) {

    try {
        video.src = href;
        video.load();
    }
    catch(err) {
        console.error( `caricavideo eccezione: ${err}`);
    }
}



export function getvideosize() {

    try {
        return {
            width : window.innerWidth,
            height : window.innerHeight
        }

    }
    catch (err) {
        console.error(`getvideosize eccezione: ${err}`);
    }
}

  
export function getlineposition(vLine) {
    return parseInt(getComputedStyle(vLine)['margin-left']);
}

export function setlineposition(vLine, position) {
    vLine.style['margin-left'] = position + "px";
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


export function InitKeyboard(stopbutton) {
    window.addEventListener('keydown', function (e) {
        if (e.key == " " ||
            e.code == "Space"
            //|| e.keyCode == 32
            ) {

            stopbutton.click();
            e.preventDefault();
        }
        
    }, false);
}


let dotnetHelper = null;
let media_recorder = null;
let blobs_recorded = [];
let makerecord = true;

export function StartRec(video, stopbutton, idcamera, dotHelper, cancelbutton) {

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
        console.error("no suitable mimetype found for this device");
        return;
    }


    console.log( "tipo3 " + tipo);

    navigator.mediaDevices.getUserMedia({ video: { deviceId: idcamera } })
        .then(function (stream) {


            //if ("srcObject" in video) {
                video.srcObject = stream;
            //} else {
            //    video.src = window.URL.createObjectURL(stream);
            //}


            video.onloadedmetadata = function (e) {
                console.log("Play");
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

                console.log("media stop");

                if (makerecord) {

                    console.log("make record");
                    var optionblob = { type: `video/${tipo}` };

                    // create local object URL from the recorded video blobs
                    let video_local = URL.createObjectURL(new Blob(blobs_recorded, optionblob));

                    dotnetHelper.invokeMethodAsync("SalvaUrlVideo", video_local, tipo);
                }
                else {
                    console.log("cancel record");
                    dotnetHelper.invokeMethodAsync("CancelRec");
                }

                //??
                //media_recorder.removeEventListener('stop', arguments.callee);
            });

            
            stopbutton.addEventListener('click', function onst() {

                console.log("click stop");

                makerecord = true;
                if (media_recorder.state != "inactive")
                    media_recorder.stop();

                this.removeEventListener('click', onst);
            });

            cancelbutton.addEventListener('click', function onst2() {

                console.log("click cancel");

                makerecord = false;

                if (media_recorder.state != "inactive")
                    media_recorder.stop();

                this.removeEventListener('click', onst2);
            });
           


            // start recording with each recorded blob having 1 second video
            media_recorder.start(1000);
        }).catch((err) => {
            console.error( `The following error occurred: ${err}`);
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

export function SettaAttributeVideo(video, attribute, setta) {

    if (setta) {
        video.setAttribute(attribute, attribute)
    } else {
        video.removeAttribute(attribute)
    }

}



export function SettaCurrentTime(video, time) {
    video.currentTime = time;
}

export function AdvanceFrame(video, steps) {
    const expectedFramerate = 60; // yourVideo's framerate

    if (video.paused) video.currentTime += Math.sign(steps) * 1 / expectedFramerate
    //else video.currentTime += d


}